Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.FarmLines
Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports IFM.VR.Common.Helpers.MultiState.General
Imports IFM.VR.Common.Helpers.FARM

Public Class ctlFarmPolicyCoverages
    Inherits VRControlBase

    Public Event RequestNavigationToLocation()
    Public Event RequestNavigationToIRPM()
    Public Event QuoteRateRequested()

    'Added 11/3/2022 for task 60749 MLW
    Private Enum AnimalType_enum
        None
        Swine
        Poultry
        Cattle
        Equine
        All
    End Enum

    Private Const DefaultRefFoodSpoilageIncludedLimit = "500"
    Private Const DefaultPrivatePowerPolesIncludedLimit = "5000"

    Public Property MyLocationIndex As Int32
        Get
            If ViewState("vs_locationIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_locationIndex"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public Property DDLFarmPollutionLimit As DropDownList
        Get
            Return ddlFPLimit
        End Get
        Set(value As DropDownList)
            ddlFPLimit = value
        End Set
    End Property

    Public ReadOnly Property MyFarmLocation As List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations
            End If
            Return Nothing
        End Get
    End Property

    'Public Property ResidentNameIndex As Int32
    '    Get
    '        Return ViewState.GetInt32("vs_residentIindex")
    '    End Get
    '    Set(value As Int32)
    '        ViewState("vs_residentIindex") = value
    '    End Set
    'End Property

    'Public ReadOnly Property ResidentName As QuickQuote.CommonObjects.QuickQuoteResidentName
    '    Get
    '        If Me.MyFarmLocation IsNot Nothing Then
    '            Return MyFarmLocation(0).ResidentNames.GetItemAtIndex(ResidentNameIndex)
    '        End If
    '        Return Nothing
    '    End Get
    'End Property

    Public Property LiabilityAccordionDivId As String
        Get
            If ViewState("vs_LiabilityAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_LiabilityAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_LiabilityAccordionDivId_") = value
        End Set
    End Property

    Public Property PropertyAccordionDivId As String
        Get
            If ViewState("vs_PropertyAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_PropertyAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_PropertyAccordionDivId_") = value
        End Set
    End Property

    Public ReadOnly Property PolicyHolderType() As String
        Get
            If Quote.Policyholder.Name.TypeId IsNot Nothing Then
                Return Quote.Policyholder.Name.TypeId
            Else
                Return "1"
            End If
        End Get
    End Property

    Public ReadOnly Property ProgramType() As String
        Get
            If MyFarmLocation IsNot Nothing Then
                Return MyFarmLocation(MyLocationIndex).ProgramTypeId
            Else
                Return "6"
            End If
        End Get
    End Property

    Private _customFarmErr As Boolean
    Public Property CustomFarmingErr() As Boolean
        Get
            Return _customFarmErr
        End Get
        Set(ByVal value As Boolean)
            _customFarmErr = value
        End Set
    End Property

    Private _setLiabilityExists As Boolean
    Public Property SetLiabilityExists() As Boolean
        Get
            Return _setLiabilityExists
        End Get
        Set(ByVal value As Boolean)
            _setLiabilityExists = value
        End Set
    End Property

    Private _setMedPayExists As Boolean
    Public Property SetMedPayExists() As Boolean
        Get
            Return _setMedPayExists
        End Get
        Set(ByVal value As Boolean)
            _setMedPayExists = value
        End Set
    End Property

    Private ReadOnly Property QuoteHasOhio As Boolean
        Get
            Return SubQuotesContainsState("OH")
        End Get
    End Property

    Private ReadOnly Property HasStopGap As Boolean
        Get
            Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()

            If gsQuote IsNot Nothing Then
                If QQHelper.IsPositiveIntegerString(gsQuote.StopGapLimitId) Then
                    Return True
                End If
            End If
            Return False
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            MainAccordionDivId = dvFarmPolicyCoverages.ClientID
            LiabilityAccordionDivId = dvFarmPolicyLiabilityCoverage.ClientID
            PropertyAccordionDivId = dvFarmPolicyPropertyCoverage.ClientID
            'LoadStaticData()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
        Dim scriptToggleEquipBreak As String = "ToggleEquipBreak(this, """ & chkEquipBreak.Checked & """);"
        VRScript.CreateJSBinding("dd_Residence_CoverageForm0", ctlPageStartupScript.JsEventType.onchange, scriptToggleEquipBreak)
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.CreateAccordion(MainAccordionDivId, hiddenPolicyLevel, "0", False)
        VRScript.CreateAccordion(LiabilityAccordionDivId, hiddenFarmLiabilityCoverage, "0")
        VRScript.CreateAccordion(PropertyAccordionDivId, hiddenFarmPropertyCoverage, "0")

        VRScript.StopEventPropagation(lnkClearGeneralInfo.ClientID, False)
        VRScript.StopEventPropagation(lnkSaveGeneralInfo.ClientID, False)
        VRScript.StopEventPropagation(lnkBtnClear.ClientID, False)
        VRScript.StopEventPropagation(lnkBtnSave.ClientID, False)
        VRScript.StopEventPropagation(lnkPropClear.ClientID, False)
        VRScript.StopEventPropagation(lnkPropSave.ClientID, False)

        'Ref Food Spoilage

        Me.VRScript.AddScriptLine("ifm.vr.ui.ElementDisabler($('#" + txtRefFoodSpoilageIncludedLimit.ClientID + "'));", onlyAllowOnce:=True)
        Me.VRScript.AddScriptLine("ifm.vr.ui.ElementDisabler($('#" + txtRefFoodSpoilageTotalLimit.ClientID + "'));", onlyAllowOnce:=True)
        Me.VRScript.AddScriptLine("ifm.vr.ui.ElementDisabler($('#" + chkRefFoodSpoilage.ClientID + "'));", onlyAllowOnce:=True)

        Dim scriptRefFoodSpoilageMath As String = "UpdateGenericPersonalPropertyControl(""" + txtRefFoodSpoilageIncludedLimit.ClientID + """,""" + txtRefFoodSpoilageIncreaseLimit.ClientID + """,""" + txtRefFoodSpoilageTotalLimit.ClientID + """);"
        Me.VRScript.CreateJSBinding(txtRefFoodSpoilageIncreaseLimit.ClientID, "blur", scriptRefFoodSpoilageMath)

        Dim scriptRefFoodSpoilageFilter As String = "$(this).val(FormatAsNumberNoCommaFormatting($(this).val()));"
        Me.VRScript.CreateJSBinding(txtRefFoodSpoilageIncreaseLimit.ClientID, "keyup", scriptRefFoodSpoilageFilter)

        Dim scriptRefFoodSpoilageCheck As String = "ToggleGenericPersonalPropertyControl(""" + tblRefFoodSpoilage.ClientID + """, """ + chkRefFoodSpoilage.ClientID + """, """ + txtRefFoodSpoilageIncreaseLimit.ClientID + """);"
        Me.VRScript.CreateJSBinding(chkRefFoodSpoilage.ClientID, "change", scriptRefFoodSpoilageCheck + scriptRefFoodSpoilageMath)

        ' Open Information Popup
        lbtnAdditionalIns.Attributes.Add("onclick", "InitFarmPopupInfo('dvAIPopup', 'Additional Insured'); return false;")
        If IFM.VR.Common.Helpers.FARM.FarmAllStarHelper.IsFarmAllStarAvailable(Quote) = False Then
            lbtnFarmAllStar.Attributes.Add("onclick", "InitFarmPopupInfo('dvAllStarInfoPopup', 'Farm All Star'); return false;")
            btnASOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvAllStarInfoPopup'); return false;")
        End If
        lbtnEquipBreak.Attributes.Add("onclick", "InitFarmPopupInfo('dvEquipBreakPopup', 'Equipment Breakdown'); return false;")
        lbtnExtraExpense.Attributes.Add("onclick", "InitFarmPopupInfo('dvExtraExpensePopup', 'Extra Expense'); return false;")
        If FarmExtenderHelper.IsFarmExtenderAvailable(Quote) Then
            lbtnFarmExtend.Visible = False
            lblFarmExtender.Visible = True
        Else
            lbtnFarmExtend.Visible = True
            lblFarmExtender.Visible = False
            If AppSettings("UseUpdatedFarmExtender") <> "No" AndAlso AppSettings("UseUpdatedFarmExtender") <> Nothing Then
                lbtnFarmExtend.Attributes.Add("onclick", "InitFarmPopupInfo('dvUpdatedFarmExtenderPopup', 'Farm Extender'); return false;")
                btnUpdFarmExtenderOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvUpdatedFarmExtenderPopup'); return false;")
            Else
                lbtnFarmExtend.Attributes.Add("onclick", "InitFarmPopupInfo('dvFarmExtenderPopup', 'Farm Extender'); return false;")
                btnFarmExtenderOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvFarmExtenderPopup'); return false;")
            End If
        End If
        
        ' Close Information Popup
        btnAIOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvAIPopup'); return false;")
        'btnASOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvAllStarInfoPopup'); return false;")
        btnEqipBreakOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvEquipBreakPopup'); return false;")
        btnExtraExpenseOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvExtraExpensePopup'); return false;")

        'Removed 08/03/2023 using ddlLiabCovType_SelectedIndexChanged to handle functionality instead of javascript
        'Dim scriptCoverageType As String = "ToggleExtraData(""" + ddlLiabCovType.ClientID + """, """ + dvLiability.ClientID + """, """ + dvMedPay.ClientID + """, """ + dvEmpLiab.ClientID + """, """ + dvBusinessPursuits.ClientID +
        '    """, """ + dvFamilyMedPay.ClientID + """, """ + dvCustomFarming.ClientID + """, """ + dvFarmPollution.ClientID + """, """ + dvEPLI.ClientID + """, """ + dvAdditionalIns.ClientID + """, """ + dvIdentityFraud.ClientID +
        '    """, """ + "" + """, """ + dvFarmAllStar.ClientID + """, """ + dvEquipBreak.ClientID + """, """ + chkEPLI.ClientID + """, """ + hiddenPolicyHolderType.ClientID + """);"

        'ddlLiabCovType.Attributes.Add("onchange", scriptCoverageType)

        'Liability Coverage
        Dim scriptEmpLiab As String = "ToggleEmpLiab(""" + chkEmpLiab.ClientID + """, """ + dvNumEmployees.ClientID + """, """ + txtFTEmp.ClientID + """, """ + txtPT41Days.ClientID + """, """ + txtPT40Days.ClientID + """);"
        chkEmpLiab.Attributes.Add("onclick", scriptEmpLiab)

        Dim scriptBusinessPursuits As String = "ToggleBusinessPursuits(""" + chkBusinessPursuits.ClientID + """, """ + dvBPInfo.ClientID + """, """ + ddlBPType.ClientID + """, """ + txtAnnualReceipts.ClientID + """);"
        chkBusinessPursuits.Attributes.Add("onclick", scriptBusinessPursuits)

        If IsQuoteEndorsement() = False AndAlso IsQuoteReadOnly() = False Then
            Dim scriptFamilyMedPay As String = "ToggleFamilyMedPay(""" + chkFamMedPay.ClientID + """, """ + dvFMPNumPer.ClientID + """, """ + txtFMPNumPer.ClientID + """);"
            chkFamMedPay.Attributes.Add("onclick", scriptFamilyMedPay)
        Else
            VRScript.AddScriptLine("$('#" + dvFMPNumPer.ClientID + "').css(""display"", ""none"");")
            'VRScript.AddScriptLine("document.getElementById(" + dvFMPNumPer.ClientID + ").style.display = ""none"";")
            Dim ScriptFamMedDialog As String = "if (document.getElementById(""" + chkFamMedPay.ClientID + """).checked == false) { 	if(ConfirmFarmDialog() == false) { document.getElementById(""" + chkFamMedPay.ClientID + """).checked = true;	return false;} }"
            'VRScript.CreateJSBinding(chkFamMedPay.ClientID, "onclick", ScriptFamMedDialog)
            chkFamMedPay.Attributes.Add("onclick", ScriptFamMedDialog)
        End If
        'Dim scriptFamilyMedPay As String = "ToggleFamilyMedPay(""" + chkFamMedPay.ClientID + """, """ + dvFMPNumPer.ClientID + """, """ + txtFMPNumPer.ClientID + """);"
        'chkFamMedPay.Attributes.Add("onclick", scriptFamilyMedPay)

        Dim scriptCustomFarming As String = "ToggleCustomfarming(""" + chkCustomFarming.ClientID + """, """ + dvCFInfo.ClientID + """, """ + ddlCFType.ClientID + """, """ + txtCFAnnualReceipts.ClientID + """);"
        chkCustomFarming.Attributes.Add("onclick", scriptCustomFarming)

        Dim scriptFarmPollution As String = "ToggleFarmPollution(""" + chkFarmPollution.ClientID + """, """ + dvFPIncreasedLimits.ClientID + """, """ + ddlFPLimit.ClientID + """, """ + Quote.Policyholder.Name.TypeId +
            """, """ + ddlLiability.ClientID + """);"
        chkFarmPollution.Attributes.Add("onclick", scriptFarmPollution)

        If FarmExtenderHelper.IsFarmExtenderAvailable(Quote) Then
            Me.VRScript.AddScriptLine("ifm.vr.ui.ElementDisabler($('#" + txtExtraExpenseIncludedLimit.ClientID + "'));", onlyAllowOnce:=True)
            Me.VRScript.AddScriptLine("ifm.vr.ui.ElementDisabler($('#" + txtExtraExpenseTotalLimit.ClientID + "'));", onlyAllowOnce:=True)
            'Need to handle enable/disable of Extra Expense here and not in populate code behind, because populate disabling causes issue at save when Farm Extender unchecked (sees it as still checked) after Farm Extender being checked and saved. When Farm Extender checked it auto-checks Extra Expense. When Farm Extender unchecked, it auto-unchecks Extra Expense.
            Me.VRScript.AddScriptLine("FarmExtenderEnableDisableExtraExpense(""" & chkFarmExtend.ClientID & """,""" & chkExtraExpense.ClientId & """);")
            
            Dim scriptExtraExpenseMath As String = "UpdateGenericPersonalPropertyControl(""" + txtExtraExpenseIncludedLimit.ClientID + """,""" + txtExtraExpenseIncreasedLimit.ClientID + """,""" + txtExtraExpenseTotalLimit.ClientID + """);"
            Me.VRScript.CreateJSBinding(txtExtraExpenseIncreasedLimit.ClientID, "blur", scriptExtraExpenseMath)

            Dim scriptExtraExpenseFilter As String = "$(this).val(FormatAsNumberNoCommaFormatting($(this).val()));"
            Me.VRScript.CreateJSBinding(txtExtraExpenseIncreasedLimit.ClientID, "keyup", scriptExtraExpenseFilter)

            Dim scriptFarmExtendWithExtraExpense As String = "FarmExtenderUpdatesExtraExpense(""" & chkFarmExtend.ClientID & """,""" & chkExtraExpense.ClientId & """,""" & dvExtraExpenseIncreasedLimits.ClientID & """,""" & txtExtraExpenseIncludedLimit.ClientID & """,""" & txtExtraExpenseIncreasedLimit.ClientID & """,""" & txtExtraExpenseTotalLimit.ClientID & """);"
            chkFarmExtend.Attributes.Add("onclick", scriptFarmExtendWithExtraExpense)
            
            Dim scriptExtraExpWithIncreasedLimits As String = "ToggleExtraExpenseWithIncreasedLimits(""" & chkExtraExpense.ClientID & """, """ & dvExtraExpenseIncreasedLimits.ClientID & """, """ & txtExtraExpenseIncludedLimit.ClientID & """, """ & txtExtraExpenseIncreasedLimit.ClientID & """,""" & txtExtraExpenseTotalLimit.ClientID + """);"
            chkExtraExpense.Attributes.Add("onclick", scriptExtraExpWithIncreasedLimits)
        Else
            Dim scriptExtraExpenseLimit As String = "ToggleExtraExpense(""" + chkExtraExpense.ClientID + """, """ + dvExtraExpenseLimit.ClientID + """, """ + txtExtraExpenseLimit.ClientID + """);"
            chkExtraExpense.Attributes.Add("onclick", scriptExtraExpenseLimit)
            
            Dim scriptFarmExtend As String = "ToggleCheckboxOnly(""" + chkFarmExtend.ClientID + """);"
            chkFarmExtend.Attributes.Add("onclick", scriptFarmExtend)
        End If

        Dim scriptEPLI As String = "ToggleCheckboxOnly(""" + chkEPLI.ClientID + """);"
        chkEPLI.Attributes.Add("onclick", scriptEPLI)

        Dim scriptIdentityFraud As String = "ToggleCheckboxOnly(""" + chkIdentityFraud.ClientID + """);"
        chkIdentityFraud.Attributes.Add("onclick", scriptIdentityFraud)

        If Quote.Policyholder.Name.TypeId = "2" Then ' Commercial
            Dim scriptLiabilityEnhance As String = "ToggleLiabilityEnhancement(""" + ddlLiability.ClientID + """, """ + chkFarmPollution.ClientID + """, """ + dvFPIncreasedLimits.ClientID + """, """ + ddlFPLimit.ClientID +
                """, """ + hiddenFarmPollution.ClientID + """);"
            ddlLiability.Attributes.Add("onchange", scriptLiabilityEnhance)
        End If



        ' Handle stop gap checkbox clicks
        Me.VRScript.CreateJSBinding(chkStopGap, ctlPageStartupScript.JsEventType.onclick, "HandleStopGapClicks('" & chkStopGap.ClientID & "','" & ddStopGapLimit.ClientID & "','" & dvStopGapData.ClientID & "');")

        'Added 11/3/2022 for task 60749 MLW
        If IFM.VR.Common.Helpers.FARM.FarmCustomFeeding.IsFARCustomFeedingAvailable(Quote) Then
            Me.VRScript.CreateJSBinding(chkCustomFeeding.ClientID, "click", "HandleCustomFeedingCheckboxClicks('" & chkCustomFeeding.ClientID & "','" & dvCustomFeedingData.ClientID & "');")
        End If

        ' State Approved Pesticide/Herbicide Applicator
        Me.VRScript.CreateJSBinding(chkPesticideHerbicideApplicatorOH, ctlPageStartupScript.JsEventType.onclick, "HandlePestClicks('" & chkPesticideHerbicideApplicatorOH.ClientID & "');")

        'Me.VRScript.CreateJSBinding(chkAddlResidenceRentedToOthers.ClientID, "onclick", "HandleAddlResidenceCheckboxClicks('" & chkAddlResidenceRentedToOthers.ClientID & "','" & ctlAddlResidenceList.CoverageDataTableClientId & "');")

        'Noticed that this was throwing an error. Looked into it and saw that the javascript had been commented out. Going to comment this out to avoid the error. -Dan Gugenheim - 05/08/2018
        'Dim scriptShowHideNewFarmPollutionUpdatedDDLOptions As String = "ShowHideFarmPollutionUpdatedDDLOptions(""" + ddlFPLimit.ClientID + """, """ + EffectiveDateHelper_FarmPollutionAndFarmEnhancement.GetStartDate() + """, """ + ddlLiabCovType.ClientID + """);"
        'VRScript.AddScriptLine(scriptShowHideNewFarmPollutionUpdatedDDLOptions)

        Dim imExists As Boolean = False
        Dim rvExists As Boolean = False

        ' Check if any Inland Marine items exist
        If Quote.Locations(0).InlandMarines IsNot Nothing Then
            If Quote.Locations(0).InlandMarines.Count > 0 Then
                imExists = True
            Else
                imExists = False
            End If
        End If

        ' Check if any RV/Watercraft items exist
        If Quote.Locations(0).RvWatercrafts IsNot Nothing Then
            If Quote.Locations(0).RvWatercrafts.Count > 0 Then
                rvExists = True
            Else
                rvExists = False
            End If
        End If

        ' Only allow Numbers
        txtFTEmp.Attributes.Add("onkeyup", "$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));")
        txtPT41Days.Attributes.Add("onkeyup", "$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));")
        txtPT40Days.Attributes.Add("onkeyup", "$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));")
        txtAnnualReceipts.Attributes.Add("onkeyup", "$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));")
        txtFMPNumPer.Attributes.Add("onkeyup", "$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));")
        txtCFAnnualReceipts.Attributes.Add("onkeyup", "$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));")
        txtExtraExpenseLimit.Attributes.Add("onkeyup", "$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));")

        ' Disable Enter Key
        txtFTEmp.Attributes.Add("onkeydown", "return (event.keyCode!=13);")
        txtPT41Days.Attributes.Add("onkeydown", "return (event.keyCode!=13);")
        txtPT40Days.Attributes.Add("onkeydown", "return (event.keyCode!=13);")
        txtAnnualReceipts.Attributes.Add("onkeydown", "return (event.keyCode!=13);")
        txtFMPNumPer.Attributes.Add("onkeydown", "return (event.keyCode!=13);")
        txtCFAnnualReceipts.Attributes.Add("onkeydown", "return (event.keyCode!=13);")
        txtExtraExpenseLimit.Attributes.Add("onkeydown", "return (event.keyCode!=13);")

        ' Round Number to nearest whole number
        'Updated 4/12/2022 for bug 74250 MLW
        'Dim scriptRoundFTEmp As String = "RoundToNearestNumber(""" + txtFTEmp.ClientID + """);"
        Dim scriptRoundFTEmp As String = "AlwaysRoundToNearestNumber(""" + txtFTEmp.ClientID + """);"
        txtFTEmp.Attributes.Add("onblur", scriptRoundFTEmp)

        'Updated 4/12/2022 for bug 74250 MLW
        'Dim scriptRoundPT41Emp As String = "RoundToNearestNumber(""" + txtPT41Days.ClientID + """);"
        Dim scriptRoundPT41Emp As String = "AlwaysRoundToNearestNumber(""" + txtPT41Days.ClientID + """);"
        txtPT41Days.Attributes.Add("onblur", scriptRoundPT41Emp)

        'Updated 4/12/2022 for bug 74250 MLW
        'Dim scriptRoundPT40Emp As String = "RoundToNearestNumber(""" + txtPT40Days.ClientID + """);"
        Dim scriptRoundPT40Emp As String = "AlwaysRoundToNearestNumber(""" + txtPT40Days.ClientID + """);"
        txtPT40Days.Attributes.Add("onblur", scriptRoundPT40Emp)

        Dim scriptBPAnnualReceipts As String = "RoundToNearestNumber(""" + txtAnnualReceipts.ClientID + """);"
        txtAnnualReceipts.Attributes.Add("onblur", scriptBPAnnualReceipts)

        Dim scriptFMPNumPerson As String = "RoundToNearestNumber(""" + txtFMPNumPer.ClientID + """);"
        txtFMPNumPer.Attributes.Add("onblur", scriptFMPNumPerson)

        Dim scriptCFAnnualReceipts As String = "RoundToNearestNumber(""" + txtCFAnnualReceipts.ClientID + """);"
        txtCFAnnualReceipts.Attributes.Add("onblur", scriptCFAnnualReceipts)

        Dim scriptExtraExpense As String = "RoundToNearestNumber(""" + txtExtraExpenseLimit.ClientID + """);"
        txtExtraExpenseLimit.Attributes.Add("onblur", scriptExtraExpense)

        'Property Coverage
        Dim isWaterDamageAvailable as Boolean = False
        If IFM.VR.Common.Helpers.FARM.FarmAllStarHelper.IsFarmAllStarAvailable(Quote) Then
            isWaterDamageAvailable = True
        End If
        Dim scriptFarmAllStar As String = "ToggleFarmAllStar(""" & chkFarmAllStar.ClientID & """, """ & dvBackSewerDrain.ClientID & """, """ & ddlBSDLimit.ClientID & """,""" & ddlWaterDamageLimit.ClientID & """,""" & tblWaterBUWaterDamage.ClientID & """,""" & lblBackSewerDrain.ClientID & """,""" & trWaterDamage.ClientID & """,""" & trWaterDamageLimit.ClientID & """,""" & isWaterDamageAvailable & """);"
        chkFarmAllStar.Attributes.Add("onclick", scriptFarmAllStar)

        Dim scriptEquipBreak As String = "ToggleCheckboxOnly(""" + chkEquipBreak.ClientID + """);"
        chkEquipBreak.Attributes.Add("onclick", scriptEquipBreak)

        Dim scriptPollution As String = "ToggleCheckboxOnly(""" + chkPollution.ClientID + """);"
        chkPollution.Attributes.Add("onclick", scriptPollution)

        txtFTEmp.Attributes.Add("onfocus", "this.select()")
        txtPT40Days.Attributes.Add("onfocus", "this.select()")
        txtPT41Days.Attributes.Add("onfocus", "this.select()")
        txtAnnualReceipts.Attributes.Add("onfocus", "this.select()")
        txtFMPNumPer.Attributes.Add("onfocus", "this.select()")
        txtCFAnnualReceipts.Attributes.Add("onfocus", "this.select()")
        txtExtraExpenseLimit.Attributes.Add("onfocus", "this.select()")

    End Sub

    Public Overrides Sub LoadStaticData()
        'Updated 9/13/18 for multi state MLW - Quote to SubQuoteFirst
        If SubQuoteFirst IsNot Nothing Then
            'If Quote.PolicyCoverages IsNot Nothing Then
            If SubQuoteFirst.PolicyCoverages IsNot Nothing Then
                Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                Dim attribute As New QuickQuoteStaticDataAttribute

                QQHelper.LoadStaticDataOptionsDropDown(ddlLiability, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.OccurrenceLiabilityLimitId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
                QQHelper.LoadStaticDataOptionsDropDown(ddlMedPay, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
                If IFM.VR.Common.Helpers.FARM.FarmAllStarHelper.IsFarmAllStarAvailable(Quote) Then
                    QQHelper.LoadStaticDataOptionsDropDown(ddlBSDLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmAllStarWaterBackupLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
                    QQHelper.LoadStaticDataOptionsDropDown(ddlWaterDamageLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmAllStarWaterDamageLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
                Else
                    QQHelper.LoadStaticDataOptionsDropDown(ddlBSDLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmAllStarLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
                End If
                QQHelper.LoadStaticDataOptionsDropDown(ddlBPType, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.BusinessPursuitTypeId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
                QQHelper.LoadStaticDataOptionsDropDown(ddlFPLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
                QQHelper.LoadStaticDataOptionsDropDown(ddStopGapLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.StopGapLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)

                'Added 11/3/2022 for task 60749 MLW
                If IFM.VR.Common.Helpers.FARM.FarmCustomFeeding.IsFARCustomFeedingAvailable(Quote) Then
                    ' Load the custom feeding limit dropdowns
                    If ddCFSwineLimit.Items Is Nothing OrElse ddCFSwineLimit.Items.Count = 0 Then
                        QQHelper.LoadStaticDataOptionsDropDown(ddCFSwineLimit, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FarmCustomFeedingSwineLimitId)
                        QQHelper.LoadStaticDataOptionsDropDown(ddCFPoultryLimit, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FarmCustomFeedingPoultryLimitId)
                        QQHelper.LoadStaticDataOptionsDropDown(ddCFCattleLimit, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FarmCustomFeedingCattleLimitId)
                        QQHelper.LoadStaticDataOptionsDropDown(ddCFEquineLimit, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FarmCustomFeedingEquineLimitId)
                    End If
                End If


                'optionAttributes = New List(Of QuickQuoteStaticDataAttribute)
                'attribute = New QuickQuoteStaticDataAttribute

                'With attribute
                '    .nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId
                '    .nvp_value = "40054"
                'End With

                'optionAttributes.Add(attribute)
                'QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(ddlFPLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, optionAttributes)


                QQHelper.LoadStaticDataOptionsDropDown(ddlLiabCovType, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.LiabilityOptionId)
                Dim removeNone As ListItem = ddlLiabCovType.Items.FindByText("NONE")
                If removeNone IsNot Nothing Then
                    ddlLiabCovType.Items.Remove(removeNone)
                End If    
                'ddlLiabCovType.Items.Clear()

                'If ProgramType = "7" Then
                '    ' Select-O-Matic
                '    ddlLiabCovType.Items.Clear()
                '    Dim newListItem As ListItem

                '    ' 01/29/2021 CAH - Bug 59458
                '    If IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
                '        newListItem = New ListItem("", "-1")
                '        ddlLiabCovType.Items.Add(newListItem)
                '    End If

                '    If Not IsCommercialPolicy Then ' If PolicyHolderType = "1" Then
                '        newListItem = New ListItem("Personal Liab", "1")
                '        ddlLiabCovType.Items.Add(newListItem)
                '        'Quote.LiabilityOptionId = "1"
                '    Else
                '        newListItem = New ListItem("Comm Liab", "2")
                '        ddlLiabCovType.Items.Add(newListItem)
                '        'Quote.LiabilityOptionId = "2"
                '    End If

                '    newListItem = New ListItem("None", "6")

                '    ddlLiabCovType.Items.Add(newListItem)
                '    dvLiabilityDropDown.Attributes.Add("style", "display:block;")
                '    dvLiabilityType.Attributes.Add("style", "display:none;")
                '    lblLiabCovType.Text = ""
                'Else
                '    dvLiabilityDropDown.Attributes.Add("style", "display:none;")
                '    dvLiabilityType.Attributes.Add("style", "display:block;")

                '    If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                '        If Quote.Locations(0).HobbyFarmCredit Then
                '            SubQuoteFirst.LiabilityOptionId = "1"
                '        End If
                '    End If

                '    If SubQuoteFirst.LiabilityOptionId = "1" Then
                '        lblLiabCovType.Text = "Personal Liab"
                '    Else
                '        lblLiabCovType.Text = "Comm Liab"
                '    End If
                '    'If PolicyHolderType = "1" Then ' Not needed Matt A 9-10-15 Bug 5463
                '    '    lblLiabCovType.Text = "Personal Liab"
                '    '    Quote.LiabilityOptionId = "1"
                '    'Else
                '    '    lblLiabCovType.Text = "Comm Liab"
                '    '    Quote.LiabilityOptionId = "2"
                '    'End If
                'End If

                'Updated 8/8/2022 for task 76030 MLW
                'Added 6/21/2022 for task 71215 MLW
                If (Not IsCommercialPolicy AndAlso PollutionLiability1MHelper.IsPollutionLiability1MAvailable(Quote) = False) OrElse (IsCommercialPolicy AndAlso LiabilityEnhancement1MHelper.IsLiabilityEnhancement1MAvailable(Quote) = False) Then
                    'If PollutionLiability1MHelper.IsPollutionLiability1MAvailable(Quote) = False OrElse IsCommercialPolicy Then
                    Dim ItemsToRemove As New List(Of ListItem)
                    For Each item As ListItem In ddlFPLimit.Items
                        If item.Value = "444" Then '444 = 1,000,000
                            ItemsToRemove.Add(item)
                        End If
                    Next
                    If ItemsToRemove IsNot Nothing AndAlso ItemsToRemove.Count > 0 Then
                        For Each item As ListItem In ItemsToRemove
                            ddlFPLimit.Items.Remove(item)
                        Next
                    End If
                End If
            End If
        End If
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        ctlVehicleAdditionalInterestList.Visible = False
        dvLiabilityType.Attributes.Add("style", "display:none;")
        If IsQuoteEndorsement() Then
            ddlLiabCovType.Enabled = False
        End If

        If CanineExclusionHelper.isCanineExclusionAvailable(Quote) Then
            dvCanine.Visible = True
        Else
            'Not elidgable hide and remove.
            dvCanine.Visible = False
            If Quote.Locations.HasItemAtIndex(0) Then
                Quote.Locations(0).SectionIICoverages.RemoveAll(Function(sc) sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion)
            End If
        End If

        If Quote IsNot Nothing Then
            'Updated 9/6/18 for multi state MLW - Quote replaced with SubQuoteFirst

             'Farm All Star
             trWaterDamage.Attributes.Add("style", "display:none;")
             trWaterDamageLimit.Attributes.Add("style", "display:none;")
            If IFM.VR.Common.Helpers.FARM.FarmAllStarHelper.IsFarmAllStarAvailable(Quote) Then
                lbtnFarmAllStar.Visible = False
                lblFarmAllStar.Visible = True
            Else
                lbtnFarmAllStar.Visible = True
                lblFarmAllStar.Visible = False
            End If

            If Not IsQuoteEndorsement() Then
                PersonalLiabilitylimitTextFarm.Visible = True
            End If

            ' OHIO coverages
            ' Show or hide Motorized Vehicles based on OH effective date
            ' Also State Approved Pesticide or Herbicide Applicators OH

            If IsOhioEffective(Quote) AndAlso IFM.VR.Common.Helpers.MultiState.General.SubQuotesContainsState(SubQuotes, "OH") Then
                ' Motorized Vehicles &State Approved Herbicide - Ohio must be on quote to be eligible.
                dvMotorizedVehicles.Attributes.Add("style", "display:block")
                ' State Approved Herbicide - OH must be on the quote
                dvAIStateApprovedPesticideHerbicideApplicator.Attributes.Add("style", "display:block")
                'If QuoteHasOhio() Then
                '    dvAIStateApprovedPesticideHerbicideApplicator.Attributes.Add("style", "display:block")
                'Else
                '    dvAIStateApprovedPesticideHerbicideApplicator.Attributes.Add("style", "display:none")
                '    RemoveOHPesticideHerbicideCoverages(Quote)
                'End If
            Else
                dvMotorizedVehicles.Attributes.Add("style", "display:none")
                If MyFarmLocation IsNot Nothing Then RemoveMotorizedVehiclesCoverages(MyFarmLocation(MyLocationIndex))
                'If MyFarmLocation(MyLocationIndex) IsNot Nothing Then RemoveMotorizedVehiclesCoverages(MyFarmLocation(MyLocationIndex))
                dvAIStateApprovedPesticideHerbicideApplicator.Attributes.Add("style", "display:none")
                RemoveOHPesticideHerbicideCoverages(SubQuotes)
            End If

            If SubQuoteFirst IsNot Nothing Then
                If SubQuoteFirst.LiabilityOptionId <> "" And SubQuoteFirst.LiabilityOptionId <> "0" Then
                    'ddlLiabCovType.SelectedValue = SubQuoteFirst.LiabilityOptionId
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlLiabCovType, SubQuoteFirst.LiabilityOptionId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.LiabilityOptionId)
                End If

                TogglePolicyCoverage()

                If SubQuoteFirst.OccurrenceLiabilityLimitId <> "" AndAlso SubQuoteFirst.OccurrenceLiabilityLimitId <> "0" Then

                    IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlLiability, SubQuoteFirst.OccurrenceLiabilityLimitId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.OccurrenceLiabilityLimitId)

                    'If QQHelper.IsPositiveIntegerString(SubQuoteFirst.OccurrenceLiabilityLimitId) AndAlso ddlLiability.Items.FindByValue(SubQuoteFirst.OccurrenceLiabilityLimitId) Is Nothing Then
                    '    Dim AITypeDescription As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.OccurrenceLiabilityLimitId, SubQuoteFirst.OccurrenceLiabilityLimitId)
                    '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(Me.ddlLiability, SubQuoteFirst.OccurrenceLiabilityLimitId, AITypeDescription)
                    'Else
                    '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddlLiability, SubQuoteFirst.OccurrenceLiabilityLimitId)
                    'End If

                    'ddlLiability.SelectedValue = SubQuoteFirst.OccurrenceLiabilityLimitId
                    SetLiabilityExists = True
                    'hiddenFarmPollution.Value = True
                End If

                If SubQuoteFirst.MedicalPaymentsLimitid <> "" AndAlso SubQuoteFirst.MedicalPaymentsLimitid <> "0" Then

                    IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlMedPay, SubQuoteFirst.MedicalPaymentsLimitid, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.MedicalPaymentsLimitId)

                    'If QQHelper.IsPositiveIntegerString(SubQuoteFirst.MedicalPaymentsLimitid) AndAlso ddlMedPay.Items.FindByValue(SubQuoteFirst.MedicalPaymentsLimitid) Is Nothing Then
                    '    Dim AITypeDescription As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.MedicalPaymentsLimitId, SubQuoteFirst.MedicalPaymentsLimitid)
                    '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(Me.ddlMedPay, SubQuoteFirst.MedicalPaymentsLimitid, AITypeDescription)
                    'Else
                    '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddlMedPay, SubQuoteFirst.MedicalPaymentsLimitid)
                    'End If

                    'ddlMedPay.SelectedValue = SubQuoteFirst.MedicalPaymentsLimitid
                    SetMedPayExists = True
                End If

                ' Employer's Liability - Farm Employees - Boolean
                ' Not allowed on  quotes with OH
                'If SubQuotesContainsState("OH") Then
                '    ' Do not show this coverage if there are any Ohio locations on the policy
                '    Me.dvEmpLiab.Attributes.Add("style", "display:none")
                '    dvNumEmployees.Attributes.Add("style", "display:none;")
                '    chkEmpLiab.Checked = False
                'Else
                ' Code above commented out to allow Employer's Liability - Farm Employees for OH. BD
                If SubQuoteFirst.HasFarmEmployersLiability Then
                    chkEmpLiab.Checked = True
                    txtFTEmp.Text = SubQuoteFirst.EmployeesFullTime
                    txtPT40Days.Text = SubQuoteFirst.EmployeesPartTime1To40Days
                    txtPT41Days.Text = SubQuoteFirst.EmployeesPartTime41To179Days
                    dvNumEmployees.Attributes.Add("style", "display:block;")
                Else
                    chkEmpLiab.Checked = False
                    dvNumEmployees.Attributes.Add("style", "display:none;")
                End If
                'End If

                ' Stop Gap
                '''TODO: evaluate how we should remove  this; create task
                If IsOhioEffective(Quote) Then
                    dvStopGap.Attributes.Add("style", "display:block")
                    dvStopGapData.Attributes.Add("style", "display:none")
                    ddStopGapLimit.Attributes.Add("style", "display:none")
                    If HasStopGap Then
                        Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()
                        chkStopGap.Checked = True
                        ddStopGapLimit.Attributes.Add("style", "display:block")
                        If gsQuote.StopGapLimitId IsNot Nothing AndAlso IsNumeric(gsQuote.StopGapLimitId) Then
                            ddStopGapLimit.SelectedValue = gsQuote.StopGapLimitId
                        Else
                            ddStopGapLimit.SelectedValue = "0"
                        End If
                        dvStopGapData.Attributes.Add("style", "display:block")
                        txtStopGapPayroll.Text = ""
                        If gsQuote.StopGapPayroll IsNot Nothing AndAlso IsNumeric(gsQuote.StopGapPayroll) Then
                            txtStopGapPayroll.Text = Format(CDec(gsQuote.StopGapPayroll), "#########")
                        End If
                    End If
                Else
                    chkStopGap.Checked = False
                    txtStopGapPayroll.Text = ""
                    dvStopGap.Attributes.Add("style", "display:none")
                    dvStopGapData.Attributes.Add("style", "display:none")
                    ddStopGapLimit.Attributes.Add("style", "display:none")
                    RemoveOHStopGapCoverageFromSubquotes(SubQuotes)
                End If

                'Added 11/3/2022 for task 60749 MLW
                If IFM.VR.Common.Helpers.FARM.FarmCustomFeeding.IsFARCustomFeedingAvailable(Quote) Then
                    ' CUSTOM FEEDING
                    dvCustomFeeding.Attributes.Add("style", "display:''")
                    chkCustomFeeding.Checked = QuoteHasCustomFeedingCoverage()
                    If chkCustomFeeding.Checked Then
                        dvCustomFeedingData.Attributes.Add("style", "display:''")
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddCFSwineLimit, SubQuoteFirst.FarmCustomFeedingSwineLimitId)
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddCFPoultryLimit, SubQuoteFirst.FarmCustomFeedingPoultryLimitId)
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddCFCattleLimit, SubQuoteFirst.FarmCustomFeedingCattleLimitId)
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddCFEquineLimit, SubQuoteFirst.FarmCustomFeedingEquineLimitId)
                        txtCFSwineDesc.Text = SubQuoteFirst.FarmCustomFeedingSwineDescription
                        txtCFPoultryDesc.Text = SubQuoteFirst.FarmCustomFeedingPoultryDescription
                        txtCFCattleDesc.Text = SubQuoteFirst.FarmCustomFeedingCattleDescription
                        txtCFEquineDesc.Text = SubQuoteFirst.FarmCustomFeedingEquineDescription
                    Else
                        dvCustomFeedingData.Attributes.Add("style", "display:none")
                        ddCFSwineLimit.SelectedIndex = -1
                        ddCFPoultryLimit.SelectedIndex = -1
                        ddCFCattleLimit.SelectedIndex = -1
                        ddCFEquineLimit.SelectedIndex = -1
                        txtCFSwineDesc.Text = ""
                        txtCFPoultryDesc.Text = ""
                        txtCFCattleDesc.Text = ""
                        txtCFEquineDesc.Text = ""
                        'txtCFCattleDesc.Attributes.Add("style", "disabled:false")
                        'txtCFEquineDesc.Attributes.Add("style", "disabled:false")
                        'txtCFPoultryDesc.Attributes.Add("style", "disabled:false")
                        'txtCFSwineDesc.Attributes.Add("style", "disabled:false")
                        'ddCFCattleLimit.Attributes.Add("style", "disabled:false")
                        'ddCFEquineLimit.Attributes.Add("style", "disabled:false")
                        'ddCFPoultryLimit.Attributes.Add("style", "disabled:false")
                        'ddCFSwineLimit.Attributes.Add("style", "disabled:false")
                        txtCFCattleDesc.Enabled = True
                        txtCFEquineDesc.Enabled = True
                        txtCFPoultryDesc.Enabled = True
                        txtCFSwineDesc.Enabled = True
                        ddCFCattleLimit.Enabled = True
                        ddCFEquineLimit.Enabled = True
                        ddCFPoultryLimit.Enabled = True
                        ddCFSwineLimit.Enabled = True
                    End If
                Else
                    dvCustomFeeding.Attributes.Add("style", "display:none")
                End If

                ' 80330 - EPLI
                chkEPLI.Checked = SubQuoteFirst.HasEPLI
                ' 01/29/2021 CAH Bug 59434 Disable EPLI on Endorsements
                If IsQuoteEndorsement() Then
                    chkEPLI.Enabled = False
                End If
            End If

            ' OH Pesticide/Herbicide Applicator
            chkPesticideHerbicideApplicatorOH.Checked = False
            If IsOhioEffective(Quote) AndAlso QuoteHasOhio() Then
                chkPesticideHerbicideApplicatorOH.Checked = SubQuoteFirst.HasHerbicidePersticideApplicator
            End If
            'End If


            ' 07/13/2022 CAH - Old Eff Date (feature flag) from 2016.  Safe to remove.
            'If EffectiveDateHelper.isQuoteEffectiveDatePastDate(Quote.EffectiveDate, EffectiveDateHelper_FarmPollutionAndFarmEnhancement.GetStartDate()) = False Then
            '    EffectiveDateHelper_FarmPollutionAndFarmEnhancement.RemoveValuesFromDropDown(ddlFPLimit)
            'End If

            ' Section II Coverages
            If MyFarmLocation IsNot Nothing Then
                ' Additional Insured
                If MyFarmLocation(0).AdditionalInterests IsNot Nothing Then
                    If MyFarmLocation(0).AdditionalInterests.Count > 0 Then
                        dvAIInfo.Attributes.Add("style", "display:block;")
                        chkAdditionalIns.Enabled = False
                        chkAdditionalIns.Checked = True
                    Else
                        dvAIInfo.Attributes.Add("style", "display:none;")
                        chkAdditionalIns.Enabled = True
                        chkAdditionalIns.Checked = False
                    End If
                End If

                If MyFarmLocation(MyLocationIndex).SectionIICoverages IsNot Nothing Then
                    dvFPIncreasedLimits.Attributes.Add("style", "display:none;")
                    dvFMPNumPer.Attributes.Add("style", "display:none;")
                    dvBPInfo.Attributes.Add("style", "display:none;")
                    dvCFInfo.Attributes.Add("style", "display:none;")

                    If chkCustomFarming.Checked Then
                        dvCFInfo.Attributes.Add("style", "display:block;")
                    Else
                        dvCFInfo.Attributes.Add("style", "display:none;")
                    End If

                    chkFarmPollution.Checked = False 'Added 6/22/2022 for task 71215 MLW
                    For Each sc As QuickQuoteSectionIICoverage In MyFarmLocation(MyLocationIndex).SectionIICoverages
                        Select Case sc.CoverageType
                        ' 40054 - Limited Farm Pollution Liability (Increased Limits) - Personal ONLY
                        ' 80094 - Liability Enhancement Endorsement - Commercial ONLY
                            Case QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_FarmPollutionLiability,
                                QuickQuoteSectionIICoverage.SectionIICoverageType.EnhancementEndorsement
                                chkFarmPollution.Checked = True
                                If sc IsNot Nothing AndAlso sc.TotalLimit IsNot Nothing Then
                                    'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlFPLimit, sc.TotalLimit)
                                    WebHelper_Personal.SetdropDownFromText_ForceDiamondText(ddlFPLimit, sc.TotalLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId)
                                End If
                                dvFPIncreasedLimits.Attributes.Add("style", "display:block;")
                        ' 70129 - Custom Farming (without spray)
                        ' 80115 - Custom Farming (with spray)
                            Case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_No_Spraying,
                                QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_With_Spraying
                                chkCustomFarming.Checked = True
                                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlCFType, sc.CoverageCodeId)
                                txtCFAnnualReceipts.Text = sc.EstimatedReceipts
                                dvCFInfo.Attributes.Add("style", "display:block;")

                        ' 70201 - Family Medical Payments
                            Case QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments
                                chkFamMedPay.Checked = True

                                ' Displays the number of residence names entered on app. If none exist then display
                                ' the number entered during quote
                                If MyFarmLocation(0).ResidentNames IsNot Nothing Then
                                    txtFMPNumPer.Text = MyFarmLocation(0).ResidentNames.Count
                                Else
                                    txtFMPNumPer.Text = sc.NumberOfPersonsReceivingCare
                                End If

                                dvFMPNumPer.Attributes.Add("style", "display:block;")

                            ' 70135 - Incidental Business Pursuits
                            Case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Incidental_Business_Exposures
                                chkBusinessPursuits.Checked = True
                                Try
                                    ddlBPType.SelectedValue = sc.BusinessPursuitTypeId
                                Catch ex As Exception
                                    ddlBPType.SelectedValue = ""
                                End Try

                                txtAnnualReceipts.Text = sc.EstimatedReceipts
                                dvBPInfo.Attributes.Add("style", "display:block;")
                            Case QuickQuoteSectionIICoverage.SectionIICoverageType.Motorized_Vehicles_Ohio
                                chkMotorizedVehicles.Checked = True
                                Exit Select
                        End Select
                    Next
                End If

                ' Section I Coverages
                If MyFarmLocation(MyLocationIndex).SectionICoverages IsNot Nothing Then
                    For Each sc As QuickQuoteSectionICoverage In MyFarmLocation(MyLocationIndex).SectionICoverages
                        Select Case sc.CoverageType
                        ' 70213 - Identity Fraud Expense
                            Case QuickQuoteSectionICoverage.SectionICoverageType.IdentityFraudExpense
                                chkIdentityFraud.Checked = True
                        End Select

                    Next
                End If

                tblRefFoodSpoilage.Attributes.Add("style", "display:none;")
                dvRefFoodSpoilage.Attributes.Add("style", "display:none;")
                If Common.Helpers.FARM.RefFoodSpoilageHelper.IsRefFoodSpoilageAvailable(Quote) Then
                    txtRefFoodSpoilageIncludedLimit.Text = DefaultRefFoodSpoilageIncludedLimit
                    Dim RefFoodSpoilage As QuickQuoteCoverage
                    ' 70148 - Ref Food Spoilage
                    If MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.FindAll(Function(p) p.CoverageCodeId = "70148").Count > 0 Then
                        RefFoodSpoilage = MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.Find(Function(p) p.CoverageCodeId = "70148")
                        If RefFoodSpoilage.ManualLimitIncreased = "0" Then
                            RefFoodSpoilage.ManualLimitIncreased = String.Empty
                        End If
                        If RefFoodSpoilage.ManualLimitAmount = "0" Then
                            RefFoodSpoilage.ManualLimitAmount = DefaultRefFoodSpoilageIncludedLimit
                        End If
                        txtRefFoodSpoilageIncreaseLimit.Text = Format(RefFoodSpoilage.ManualLimitIncreased.TryToGetInt32, "N0")
                        txtRefFoodSpoilageTotalLimit.Text = Format(RefFoodSpoilage.ManualLimitAmount.TryToGetInt32, "N0")
                    Else
                        RefFoodSpoilage = New QuickQuoteCoverage()
                        RefFoodSpoilage.CoverageCodeId = "70148"
                        RefFoodSpoilage.ManualLimitIncluded = DefaultRefFoodSpoilageIncludedLimit
                        RefFoodSpoilage.ManualLimitAmount = DefaultRefFoodSpoilageIncludedLimit
                        MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.Add(RefFoodSpoilage)

                        txtRefFoodSpoilageIncreaseLimit.Text = String.Empty
                        txtRefFoodSpoilageTotalLimit.Text = DefaultRefFoodSpoilageIncludedLimit
                    End If
                    chkRefFoodSpoilage.Checked = True
                    chkRefFoodSpoilage.Enabled = False
                    tblRefFoodSpoilage.Attributes.Add("style", "display:block;")
                    dvRefFoodSpoilage.Attributes.Add("style", "display:block;")
                End If

                'Updated 9/6/18 for multi state MLW - Quote replaced with SubQuoteFirst
                If SubQuoteFirst IsNot Nothing Then
                        ' 80125 - Farm All Star
                        If IFM.VR.Common.Helpers.FARM.FarmAllStarHelper.IsFarmAllStarAvailable(Quote) Then
                            If SubQuoteFirst.HasFarmAllStar Then
                                chkFarmAllStar.Checked = True
                                dvBackSewerDrain.Attributes.Add("style", "display:block;")
                                lblBackSewerDrain.Text = "Water Backup"
                                trWaterDamage.Attributes.Add("style", "display:'';")
                                trWaterDamageLimit.Attributes.Add("style", "display:'';")
                                tblWaterBUWaterDamage.Attributes.Remove("width")
                                IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlBSDLimit, SubQuoteFirst.FarmAllStarWaterBackupLimitId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FarmAllStarWaterBackupLimitId)
                                IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlWaterDamageLimit, SubQuoteFirst.FarmAllStarWaterDamageLimitId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FarmAllStarWaterDamageLimitId)
                            Else
                                chkFarmAllStar.Checked = False
                                dvBackSewerDrain.Attributes.Add("style", "display:none;")
                            End If
                        Else
                            If SubQuoteFirst.FarmAllStarLimitId <> "" And SubQuoteFirst.FarmAllStarLimitId <> "0" Then
                                chkFarmAllStar.Checked = True
                                ddlBSDLimit.SelectedValue = SubQuoteFirst.FarmAllStarLimitId
                                dvBackSewerDrain.Attributes.Add("style", "display:block;")
                                lblBackSewerDrain.Text = "Backup of Sewer or Drain"
                                tblWaterBUWaterDamage.Attributes.Add("width", "100%")
                            Else
                                chkFarmAllStar.Checked = False
                                dvBackSewerDrain.Attributes.Add("style", "display:none;")
                            End If
                        End If

                        chkExtraExpense.Checked = False
                        ' Farm Extender - Boolean
                        If SubQuoteFirst.HasFarmExtender Then
                            chkFarmExtend.Checked = True
                            If FarmExtenderHelper.IsFarmExtenderAvailable(Quote) Then
                                chkExtraExpense.Checked = True
                                'Need to handle enable/disable of Extra Expense in javascript AddScriptWhenRendered onload (FarmExtenderEnableDisableExtraExpense()) and not in populate code behind, because populate disabling causes issue at save when Farm Extender unchecked (sees it as still checked) after Farm Extender being checked and saved. When Farm Extender checked it auto-checks Extra Expense. When Farm Extender unchecked, it auto-unchecks Extra Expense.
                                'chkExtraExpense.Enabled = False 'Do not use this, it causes Extra Expense to save on the quote when Farm Extender unchecked after being checked and saved.
                            End If
                        Else
                            chkFarmExtend.Checked = False
                        End If

                        ' 80140 Extra Expense
                        dvExtraExpenseIncreasedLimits.Attributes.Add("style", "display:none;")
                        If SubQuoteFirst.OptionalCoverages IsNot Nothing Then
                            If SubQuoteFirst.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense).Count > 0 Then
                                Dim extraExpense As QuickQuoteOptionalCoverage = SubQuoteFirst.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)

                                If extraExpense IsNot Nothing Then
                                    chkExtraExpense.Checked = True
                                    If FarmExtenderHelper.IsFarmExtenderAvailable(Quote) Then
                                        dvExtraExpenseIncreasedLimits.Attributes.Add("style", "display:block;")
                                        If chkFarmExtend.Checked Then
                                            extraExpense.IncludedLimit = "5,000"
                                        Else
                                            extraExpense.IncludedLimit = String.Empty
                                        End If
                                        txtExtraExpenseIncludedLimit.Text = extraExpense.IncludedLimit
                                        If IsNullEmptyorWhitespace(extraExpense.IncreasedLimit) Then
                                            extraExpense.IncreasedLimit = "0"
                                        End If
                                        txtExtraExpenseIncreasedLimit.Text = FormatNumber(extraExpense.IncreasedLimit.TryToGetInt32, "0")
                                        If extraExpense.Coverage IsNot Nothing Then
                                            txtExtraExpenseTotalLimit.Text = FormatNumber(extraExpense.Coverage.ManualLimitAmount.TryToGetInt32, "0")
                                        Else
                                            txtExtraExpenseTotalLimit.Text = FormatNumber(extraExpense.IncludedLimit.TryToGetInt32 + extraExpense.IncreasedLimit.TryToGetInt32, "0")
                                        End If
                                        dvExtraExpenseLimit.Attributes.Add("style", "display:none;")
                                    Else
                                        dvExtraExpenseIncreasedLimits.Attributes.Add("style", "display:none;")
                                        txtExtraExpenseLimit.Text = extraExpense.IncreasedLimit
                                        dvExtraExpenseLimit.Attributes.Add("style", "display:block;")
                                    End If
                                Else
                                    If FarmExtenderHelper.IsFarmExtenderAvailable(Quote) Then
                                        dvExtraExpenseIncreasedLimits.Attributes.Add("style", "display:none;")
                                        txtExtraExpenseIncludedLimit.Text = ""
                                        txtExtraExpenseIncreasedLimit.Text = ""
                                        txtExtraExpenseTotalLimit.Text = ""
                                    End If
                                    dvExtraExpenseLimit.Attributes.Add("style", "display:none;")
                                End If
                            End If
                        End If

                        '' 80140 Extra Expense
                        'If SubQuoteFirst.OptionalCoverages IsNot Nothing Then
                        '    If SubQuoteFirst.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense).Count > 0 Then
                        '        Dim extraExpense As QuickQuoteOptionalCoverage = SubQuoteFirst.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)

                        '        If extraExpense IsNot Nothing Then
                        '            chkExtraExpense.Checked = True
                        '            txtExtraExpenseLimit.Text = extraExpense.IncreasedLimit
                        '            dvExtraExpenseLimit.Attributes.Add("style", "display:block;")
                        '        Else
                        '            chkExtraExpense.Checked = False
                        '            dvExtraExpenseLimit.Attributes.Add("style", "display:none;")
                        '        End If
                        '    End If
                        'End If

                        '' Farm Extender - Boolean
                        'If SubQuoteFirst.HasFarmExtender Then
                        '    chkFarmExtend.Checked = True
                        'Else
                        '    chkFarmExtend.Checked = False
                        'End If

                        ' Equipment Breakdown - Boolean
                        If Not SetLiabilityExists And Not SetMedPayExists Then
                            ' Sets Default Value
                            ' This is ok here because we remove equipment breakdown on Save
                            ' when the quote is not eligible.  MGB 1/6/2019
                            chkEquipBreak.Checked = True
                        Else
                            If SubQuoteFirst.HasFarmEquipmentBreakdown Then
                                chkEquipBreak.Checked = True
                            Else
                                chkEquipBreak.Checked = False
                            End If
                        End If

                        ' 70125 - Pollutant Clean Up and Removal - Increased Limits
                        If SubQuoteFirst.FarmIncidentalLimits IsNot Nothing Then
                            Dim increasedLimit = SubQuoteFirst.FarmIncidentalLimits.Find(Function(p) p.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Pollutant_Clean_Up_and_Removal)
                            If increasedLimit IsNot Nothing Then
                                If increasedLimit.IncreasedLimitId <> "0" And increasedLimit.IncreasedLimitId <> "" Then
                                    chkPollution.Checked = True
                                Else
                                    chkPollution.Checked = False
                                End If
                            Else
                                chkPollution.Checked = False
                            End If
                        End If


                    End If
                End If
            End If

            If Me.Quote IsNot Nothing Then
            btnRate.Visible = False
            btnRate.Enabled = False
            If IsQuoteEndorsement() Then
                Me.ctl_Billing_Info_PPA.Visible = True
                btnRate.Visible = True
                btnRate.Enabled = True
            End If

            If IsQuoteReadOnly() Then
                Me.ctl_Billing_Info_PPA.Visible = False
                Dim policyNumber As String = Me.Quote.PolicyNumber
                Dim imageNum As Integer = 0
                Dim policyId As Integer = 0
                Dim toolTip As String = "Make a change to this policy"
                'Dim qqHelper As New QuickQuoteHelperClass
                Dim readOnlyViewPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                'QuickQuoteHelperClass.configAppSettingValueAsString("")  'Unused CAH 07/21/2020
                If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                    readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                End If

                divActionButtons.Visible = False
                divEndorsementButtons.Visible = True

                btnRate_Endorsements.Enabled = False

                btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                btnMakeAChange.ToolTip = toolTip
                btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
            End If

            If IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
                Dim SectionIICoverage As QuickQuoteSectionIICoverage = Nothing

                chkFamMedPay.AutoPostBack = True
                dvAdditionalIns.Visible = False
                ctlVehicleAdditionalInterestList.Visible = True
                dvFamilyMedPayNames.Visible = False
                If MyFarmLocation(0).SectionIICoverages IsNot Nothing Then
                    Dim famMed As QuickQuoteSectionIICoverage = (From cov In MyFarmLocation(0).SectionIICoverages Where cov.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments Select cov).FirstOrDefault()
                    If famMed IsNot Nothing Then
                        If Me.MyFarmLocation(0).ResidentNames IsNot Nothing Then
                            If Me.MyFarmLocation(0).ResidentNames.Count > 0 Then
                                dvFamilyMedPayNames.Visible = True
                                ctlFamilyMedicalPayments_App.Visible = True
                            End If
                        End If
                        If IsNumeric(famMed.NumberOfPersonsReceivingCare) AndAlso QQHelper.IntegerForString(famMed.NumberOfPersonsReceivingCare) > 0 Then
                            dvFamilyMedPayNames.Visible = True
                            ctlFamilyMedicalPayments_App.Visible = True
                        End If
                    End If
                End If
            End If
        End If

        PopulateChildControls()
    End Sub

    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    Protected Sub btnViewGotoDrivers_Click(sender As Object, e As EventArgs) Handles btnViewGotoDrivers.Click
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, "0")
    End Sub

    ''' <summary>
    ''' If the quote is FarmOwners OR Select-O-Matic
    '''    AND
    ''' FormTypeId = 16 (FO-3 Dwelling Coverage - Special Form), 18 (FO 00 05 Dwelling Coverage - Special Building and Contents Form), or 13 (NA)
    ''' then the quote is eligible for Equipment Breakdown coverage
    ''' </summary>
    ''' <returns></returns>
    Private Function QuoteIsEligibleForEquipmentBreakdown() As Boolean
        If Quote Is Nothing Then Return False
        If Quote.Locations Is Nothing OrElse Quote.Locations.Count = 0 Then Return False

        If ProgramType = "6" Or ProgramType = "7" Then   ' 6 = Farmowners; 7 = Select=o-matic
            Dim formType As String = Quote.Locations(0).FormTypeId
            If formType = "15" OrElse formType = "16" OrElse formType = "18" OrElse formType = "13" Then
                Return True
            End If
        End If

        If isEndorsementF04F06() Then Return True

        Return False
    End Function

    Private Function isEndorsementF04F06() As Boolean
        If IsQuoteEndorsement() = True OrElse IsQuoteReadOnly() = True Then
            Dim location As QuickQuoteLocation = Quote.Locations(0)
            If location.FormTypeId = "17" Then 'FO-4 = 17 
                If location.Buildings.Any Then
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Private Sub TogglePolicyCoverage()
        hiddenPolicyHolderType.Value = If(IsCommercialPolicy, "2", "1") 'PolicyHolderType
        'Dim temp = hiddenFarmPollution.Value

        ' Equipment Breakdown - allow on Farmowners and Select-O-Matic with formTypeId of 16,18,13  MGB 1/6/2020 Task 39662
        If QuoteIsEligibleForEquipmentBreakdown() Then
            dvEquipBreak.Attributes.Add("style", "display:block;")
            chkEquipBreak.Checked = True
        Else
            chkEquipBreak.Checked = False
            dvEquipBreak.Attributes.Add("style", "display:none;")
        End If

        Select Case ProgramType
            Case "6"    ' Farm Owners
                ' Check to see if Commercial
                If IsCommercialPolicy Then 'If PolicyHolderType = "2" Then
                    dvEPLI.Attributes.Add("style", "display:block;")
                    'chkEPLI.Checked = True
                    setEPLI(True)
                    lblFarmPollution.Text = "Liability Enhancement Endorsement"
                    'chkFarmPollution.Enabled = False
                Else
                    dvEPLI.Attributes.Add("style", "display:none;")
                    'chkEPLI.Checked = False
                    setEPLI(False)
                    lblFarmPollution.Text = "Limited Farm Pollution (Increased Limits)"
                    chkFarmPollution.Enabled = True
                End If
            Case "7"    ' Select-O-Matic
                If ddlLiabCovType.SelectedValue = "6" Then ' "None" Selected
                    dvLiability.Attributes.Add("style", "display:none;")
                    dvMedPay.Attributes.Add("style", "display:none;")
                    dvEmpLiab.Attributes.Add("style", "display:none;")
                    dvCustomFarming.Attributes.Add("style", "display:none;")
                    dvFarmPollution.Attributes.Add("style", "display:none;")
                    dvEPLI.Attributes.Add("style", "display:none;")
                    'chkEPLI.Checked = False
                    setEPLI(False)
                    dvAdditionalIns.Attributes.Add("style", "display:none;")
                Else
                    dvLiability.Attributes.Add("style", "display:block;")
                    dvMedPay.Attributes.Add("style", "display:block;")
                    dvEmpLiab.Attributes.Add("style", "display:block;")
                    dvCustomFarming.Attributes.Add("style", "display:block;")
                    dvFarmPollution.Attributes.Add("style", "display:block;")
                    dvAdditionalIns.Attributes.Add("style", "display:block;")

                    ' Check to see if Commercial
                    If IsCommercialPolicy Then 'If PolicyHolderType = "2" Then
                        dvEPLI.Attributes.Add("style", "display:block;")
                        'chkEPLI.Checked = True
                        setEPLI(True)
                        lblFarmPollution.Text = "Liability Enhancement Endorsement"
                        'chkFarmPollution.Enabled = False
                    Else
                        dvEPLI.Attributes.Add("style", "display:none;")
                        'chkEPLI.Checked = False
                        setEPLI(False)
                        lblFarmPollution.Text = "Limited Farm Pollution (Increased Limits)"
                        chkFarmPollution.Enabled = True
                    End If
                End If

                dvBusinessPursuits.Attributes.Add("style", "display:none;")
                dvFamilyMedPay.Attributes.Add("style", "display:none;")
                dvIdentityFraud.Attributes.Add("style", "display:none;")
                dvFarmAllStar.Attributes.Add("style", "display:none;")
            Case "8"    ' Farm Liability
                dvBusinessPursuits.Attributes.Add("style", "display:none;")
                dvFamilyMedPay.Attributes.Add("style", "display:none;")
                dvCustomFarming.Attributes.Add("style", "display:none;")
                dvIdentityFraud.Attributes.Add("style", "display:none;")
                dvFarmAllStar.Attributes.Add("style", "display:none;")
                dvEquipBreak.Attributes.Add("style", "display:none;")
                dvPollution.Attributes.Add("style", "display:none;")
                dvFarmPolicyPropertyCoverage.Attributes.Add("style", "display:none;")

                ' Check to see if Commercial
                If IsCommercialPolicy Then 'If PolicyHolderType = "2" Then
                    dvEPLI.Attributes.Add("style", "display:block;")
                    'chkEPLI.Checked = True
                    setEPLI(True)
                    lblFarmPollution.Text = "Liability Enhancement Endorsement"
                    'chkFarmPollution.Enabled = False
                Else
                    dvEPLI.Attributes.Add("style", "display:none;")
                    'chkEPLI.Checked = False
                    setEPLI(False)
                    lblFarmPollution.Text = "Limited Farm Pollution (Increased Limits)"
                    chkFarmPollution.Enabled = True
                End If
        End Select
    End Sub

    Public Overrides Function Save() As Boolean
        CustomFarmingErr = False
        If Quote IsNot Nothing Then
            If MyFarmLocation IsNot Nothing Then
                Dim SectionICoverage As QuickQuoteSectionICoverage = Nothing
                Dim SectionIICoverage As QuickQuoteSectionIICoverage = Nothing
                Dim farmIncidentalCoverage As QuickQuoteFarmIncidentalLimit = Nothing
                Dim policyCoverage As QuickQuoteCoverage = Nothing

                If MyFarmLocation(MyLocationIndex).SectionICoverages Is Nothing Then
                    MyFarmLocation(MyLocationIndex).SectionICoverages = New List(Of QuickQuoteSectionICoverage)
                End If

                If MyFarmLocation(MyLocationIndex).SectionIICoverages Is Nothing Then
                    MyFarmLocation(MyLocationIndex).SectionIICoverages = New List(Of QuickQuoteSectionIICoverage)
                End If

                ' Unselect Equipment Breakdown if the policy is not eligible.  MGB 1/6/2020 Task 39662
                If Not QuoteIsEligibleForEquipmentBreakdown() Then chkEquipBreak.Checked = False

                ' Farm Unscheduled Personal Property and Extra Exoense need to be saved to the governing state quote MGB 2-1-2019 Bug 31175
                If GoverningStateQuote() IsNot Nothing Then
                    ' 80140 Extra Expense
                    If chkExtraExpense.Checked Then
                        Dim optionalCoverage As QuickQuoteOptionalCoverage

                        If GoverningStateQuote.OptionalCoverages Is Nothing Then
                            GoverningStateQuote.OptionalCoverages = New List(Of QuickQuoteOptionalCoverage)
                        End If

                        If GoverningStateQuote.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense).Count <= 0 Then
                            optionalCoverage = New QuickQuoteOptionalCoverage()
                            optionalCoverage.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense
                            If FarmExtenderHelper.IsFarmExtenderAvailable(Quote) Then
                                optionalCoverage.IncludedLimit = txtExtraExpenseIncludedLimit.Text
                                optionalCoverage.IncreasedLimit = txtExtraExpenseIncreasedLimit.Text
                            Else
                                optionalCoverage.IncludedLimit = ""
                                optionalCoverage.IncreasedLimit = txtExtraExpenseLimit.Text
                            End If
                            GoverningStateQuote.OptionalCoverages.Add(optionalCoverage)
                        Else
                            Dim updateExtraExpense As QuickQuoteOptionalCoverage = GoverningStateQuote.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)
                            If FarmExtenderHelper.IsFarmExtenderAvailable(Quote) Then
                                updateExtraExpense.IncludedLimit = txtExtraExpenseIncludedLimit.Text
                                updateExtraExpense.IncreasedLimit = txtExtraExpenseIncreasedLimit.Text
                            Else
                                updateExtraExpense.IncludedLimit = ""
                                updateExtraExpense.IncreasedLimit = txtExtraExpenseLimit.Text
                            End If
                        End If
                    Else
                        If GoverningStateQuote.OptionalCoverages IsNot Nothing Then
                            Dim extraExpense As QuickQuoteOptionalCoverage = GoverningStateQuote.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)
                            If extraExpense IsNot Nothing Then
                                dvExtraExpenseLimit.Attributes.Add("style", "display:none;")
                                extraExpense.IncreasedLimit = ""
                                GoverningStateQuote.OptionalCoverages.Remove(extraExpense)
                            End If
                        End If
                    End If

                    'Added 9/8/2022 for Bug 62410 MLW - missed with 76291 update
                    '** stop gap will only be added to the governing state quote to prevent overcharging for this coverage
                    ' Add stop gap at the quote level as well as the subquote level
                    With GoverningStateQuote
                        IFM.VR.Common.Helpers.MultiState.General.RemoveOHStopGapCoverageFromSubquotes(SubQuotes)
                        If chkStopGap.Checked Then
                            .StopGapLimitId = ddStopGapLimit.SelectedValue
                            .StopGapPayroll = txtStopGapPayroll.Text
                        End If
                    End With
                End If

                'Updated 9/6/18 for multi state MLW - replace quote with sq
                If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                    For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes

                        If sq.FarmIncidentalLimitCoverages Is Nothing Then
                            sq.FarmIncidentalLimits = New List(Of QuickQuoteFarmIncidentalLimit)
                        End If

                        sq.LiabilityOptionId = ddlLiabCovType.SelectedValue
                        If ProgramType = "7" Then ' Select O Matic
                            'sq.LiabilityOptionId = ddlLiabCovType.SelectedValue

                            If sq.LiabilityOptionId = "6" Then
                                ddlLiability.SelectedValue = "0"
                                ddlMedPay.SelectedValue = "0"
                                chkEmpLiab.Checked = False
                                chkBusinessPursuits.Checked = False
                                chkFamMedPay.Checked = False
                                chkCustomFarming.Checked = False
                                chkFarmPollution.Checked = False
                                'chkEPLI.Checked = False
                                setEPLI(False)
                                chkIdentityFraud.Checked = False
                                txtFTEmp.Text = ""
                                txtPT40Days.Text = ""
                                txtPT41Days.Text = ""

                                MyFarmLocation(0).AdditionalInterests = Nothing
                            End If
                        Else
                            'If PolicyHolderType = "1" Then ' Not needed Matt A 9-10-15 Bug 5463
                            '    Quote.LiabilityOptionId = "1" ' Personal
                            'Else
                            '    Quote.LiabilityOptionId = "2" ' Commercial
                            'End If
                        End If

                        sq.OccurrenceLiabilityLimitId = ddlLiability.SelectedValue
                        sq.MedicalPaymentsLimitid = ddlMedPay.SelectedValue

                        ' Employer's Liability - Farm Employees
                        sq.HasFarmEmployersLiability = chkEmpLiab.Checked
                        If sq.HasFarmEmployersLiability Then
                            sq.EmployeesFullTime = txtFTEmp.Text
                            sq.EmployeesPartTime1To40Days = txtPT40Days.Text
                            sq.EmployeesPartTime41To179Days = txtPT41Days.Text
                        Else
                            sq.EmployeesFullTime = ""
                            sq.EmployeesPartTime1To40Days = ""
                            sq.EmployeesPartTime41To179Days = ""
                        End If

                        ' Stop Gap
                        'MODIFIED 08/10/2022 FOR 76291
                        '** stop gap will only be added to the governing state quote to prevent overcharging for this coverage 
                        'If chkStopGap.Checked Then
                        '    sq.StopGapLimitId = ddStopGapLimit.SelectedValue
                        '    sq.StopGapPayroll = txtStopGapPayroll.Text
                        'Else
                        '    sq.StopGapLimitId = ""
                        '    sq.StopGapPayroll = ""
                        'End If

                        ' Pesticide Herbicide Applicaor - OH
                        If IsOhioEffective(Quote) AndAlso QuoteHasOhio() Then
                            sq.HasHerbicidePersticideApplicator = chkPesticideHerbicideApplicatorOH.Checked
                        Else
                            sq.HasHerbicidePersticideApplicator = False
                        End If

                    Next
                End If
                ' 70135 - Incidental Business Pursuits
                If chkBusinessPursuits.Checked Then
                    If MyFarmLocation(MyLocationIndex).SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Incidental_Business_Exposures).Count <= 0 Then
                        SectionIICoverage = New QuickQuoteSectionIICoverage()
                        SectionIICoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Incidental_Business_Exposures
                        SectionIICoverage.BusinessPursuitTypeId = ddlBPType.SelectedValue
                        SectionIICoverage.EstimatedReceipts = txtAnnualReceipts.Text
                        MyFarmLocation(MyLocationIndex).SectionIICoverages.Add(SectionIICoverage)
                    Else
                        Dim incBusPursuit As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Incidental_Business_Exposures)
                        incBusPursuit.BusinessPursuitTypeId = ddlBPType.SelectedValue
                        incBusPursuit.EstimatedReceipts = txtAnnualReceipts.Text
                    End If
                Else
                    Dim busPursuits As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Incidental_Business_Exposures)
                    If busPursuits IsNot Nothing Then
                        ddlBPType.SelectedValue = ""
                        txtAnnualReceipts.Text = ""
                        MyFarmLocation(MyLocationIndex).SectionIICoverages.Remove(busPursuits)
                    End If
                End If

                If IsQuoteEndorsement() = False AndAlso IsQuoteReadOnly() = False Then
                    ' 70201 Family Medical Payments
                    If chkFamMedPay.Checked Then
                        If MyFarmLocation(0).SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments).Count <= 0 Then
                            SectionIICoverage = New QuickQuoteSectionIICoverage()
                            SectionIICoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments
                            SectionIICoverage.NumberOfPersonsReceivingCare = txtFMPNumPer.Text
                            SectionIICoverage.IncludedLimit = "1,000" 'Added 5/30/18 for Bug 20406 MLW
                            SectionIICoverage.TotalLimit = "1,000" 'Added 5/30/18 for Bug 20406 MLW
                            MyFarmLocation(0).SectionIICoverages.Add(SectionIICoverage)

                            If MyFarmLocation(0).ResidentNames Is Nothing Then
                                MyFarmLocation(0).ResidentNames = New List(Of QuickQuoteResidentName)
                            End If

                            For inx As Integer = 0 To (Integer.Parse(SectionIICoverage.NumberOfPersonsReceivingCare) - 1)
                                MyFarmLocation(0).ResidentNames.Add(New QuickQuoteResidentName)
                                MyFarmLocation(0).ResidentNames(inx).Name.FirstName = "FIRSTN"
                                MyFarmLocation(0).ResidentNames(inx).Name.LastName = "LASTN"
                                MyFarmLocation(0).ResidentNames(inx).Name.BirthDate = "01/01/1900"
                                MyFarmLocation(0).ResidentNames(inx).Name.Salutation = "SELF"
                            Next
                        Else
                            Dim famMedPay As QuickQuoteSectionIICoverage = MyFarmLocation(0).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments)
                            famMedPay.NumberOfPersonsReceivingCare = txtFMPNumPer.Text
                            famMedPay.IncludedLimit = "1,000" 'Added 5/30/18 for Bug 20406 MLW
                            famMedPay.TotalLimit = "1,000" 'Added 5/30/18 for Bug 20406 MLW

                            If MyFarmLocation(0).ResidentNames IsNot Nothing Then
                                If Integer.Parse(famMedPay.NumberOfPersonsReceivingCare) > MyFarmLocation(0).ResidentNames.Count Then
                                    Dim diff = Integer.Parse(famMedPay.NumberOfPersonsReceivingCare) - MyFarmLocation(0).ResidentNames.Count

                                    For cnt As Integer = 0 To (diff - 1)
                                        Dim resident As QuickQuoteResidentName = New QuickQuoteResidentName
                                        resident.Name.FirstName = "FIRSTN"
                                        resident.Name.LastName = "LASTN"
                                        resident.Name.BirthDate = "01/01/1900"
                                        resident.Name.Salutation = "SELF"
                                        MyFarmLocation(0).ResidentNames.Add(resident)
                                    Next
                                End If

                                ' Per Sherry.. If the the Family Med Pay count is reduced on the quote side, then any Resident Names
                                ' that were entered on the App side are removed and replaced with the default information
                                If Integer.Parse(famMedPay.NumberOfPersonsReceivingCare) < MyFarmLocation(0).ResidentNames.Count Then
                                    If Integer.Parse(famMedPay.NumberOfPersonsReceivingCare) > 0 Then
                                        MyFarmLocation(0).ResidentNames = New List(Of QuickQuoteResidentName)
                                    End If

                                    For inx As Integer = 0 To (Integer.Parse(famMedPay.NumberOfPersonsReceivingCare) - 1)
                                        MyFarmLocation(0).ResidentNames.Add(New QuickQuoteResidentName)
                                        MyFarmLocation(0).ResidentNames(inx).Name.FirstName = "FIRSTN"
                                        MyFarmLocation(0).ResidentNames(inx).Name.LastName = "LASTN"
                                        MyFarmLocation(0).ResidentNames(inx).Name.BirthDate = "01/01/1900"
                                        MyFarmLocation(0).ResidentNames(inx).Name.Salutation = "SELF"
                                    Next
                                End If
                            End If
                        End If
                    Else
                        Dim famMed As QuickQuoteSectionIICoverage = MyFarmLocation(0).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments)
                        If famMed IsNot Nothing Then
                            txtFMPNumPer.Text = ""
                            famMed.NumberOfPersonsReceivingCare = ""
                            famMed.IncludedLimit = "0" 'Added 5/30/18 for Bug 26406 MLW
                            famMed.TotalLimit = "0" 'Added 5/30/18 for Bug 26406 MLW
                            MyFarmLocation(0).SectionIICoverages.Remove(famMed)
                            MyFarmLocation(0).ResidentNames = Nothing
                        End If
                    End If
                End If

                ' 70129 - Custom Farming (without spray)
                ' 80115 - Custom Farming (with spray)
                If chkCustomFarming.Checked Then
                    If ddlCFType.SelectedValue = "" Then
                        Dim custFarmNoSpray As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_No_Spraying)
                        If custFarmNoSpray IsNot Nothing Then
                            ddlCFType.SelectedValue = ""
                            txtCFAnnualReceipts.Text = ""
                            MyFarmLocation(MyLocationIndex).SectionIICoverages.Remove(custFarmNoSpray)
                        End If

                        Dim custFarmSpray As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_With_Spraying)
                        If custFarmSpray IsNot Nothing Then
                            ddlCFType.SelectedValue = ""
                            txtCFAnnualReceipts.Text = ""
                            MyFarmLocation(MyLocationIndex).SectionIICoverages.Remove(custFarmSpray)
                        End If
                        CustomFarmingErr = True
                    Else
                        'If ddlCFType.SelectedValue <> "" Then
                        If ddlCFType.SelectedValue = "70129" Then
                            If MyFarmLocation(MyLocationIndex).SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_No_Spraying).Count <= 0 Then
                                ' 70129 - Custom Farming (without spray)
                                SectionIICoverage = New QuickQuoteSectionIICoverage()
                                SectionIICoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_No_Spraying
                                SectionIICoverage.EstimatedReceipts = txtCFAnnualReceipts.Text

                                ' Remove Spray - If Exists
                                Dim custFarmSpray As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_With_Spraying)
                                If custFarmSpray IsNot Nothing Then
                                    txtCFAnnualReceipts.Text = ""
                                    MyFarmLocation(MyLocationIndex).SectionIICoverages.Remove(custFarmSpray)
                                End If

                                MyFarmLocation(MyLocationIndex).SectionIICoverages.Add(SectionIICoverage)
                            Else
                                Dim custFarmNoSpray As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_No_Spraying)
                                custFarmNoSpray.EstimatedReceipts = txtCFAnnualReceipts.Text
                            End If
                        Else
                            If MyFarmLocation(MyLocationIndex).SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_With_Spraying).Count <= 0 Then
                                ' 80115 - Custom Farming (with spray)
                                SectionIICoverage = New QuickQuoteSectionIICoverage()
                                SectionIICoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_With_Spraying
                                SectionIICoverage.EstimatedReceipts = txtCFAnnualReceipts.Text

                                ' Remove No Spray - If Exists
                                Dim custFarmNoSpray As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_No_Spraying)
                                If custFarmNoSpray IsNot Nothing Then
                                    txtCFAnnualReceipts.Text = ""
                                    MyFarmLocation(MyLocationIndex).SectionIICoverages.Remove(custFarmNoSpray)
                                End If

                                MyFarmLocation(MyLocationIndex).SectionIICoverages.Add(SectionIICoverage)
                            Else
                                Dim custFarmSpray As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_With_Spraying)
                                custFarmSpray.EstimatedReceipts = txtCFAnnualReceipts.Text
                            End If
                        End If
                    End If
                Else
                    Dim custFarmNoSpray As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_No_Spraying)
                    If custFarmNoSpray IsNot Nothing Then
                        ddlCFType.SelectedValue = ""
                        txtCFAnnualReceipts.Text = ""
                        MyFarmLocation(MyLocationIndex).SectionIICoverages.Remove(custFarmNoSpray)
                    End If

                    Dim custFarmSpray As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_With_Spraying)
                    If custFarmSpray IsNot Nothing Then
                        ddlCFType.SelectedValue = ""
                        txtCFAnnualReceipts.Text = ""
                        MyFarmLocation(MyLocationIndex).SectionIICoverages.Remove(custFarmSpray)
                    End If
                End If


                ' 40054 - Limited Farm Pollution Liability (Increased Limits) - Personal ONLY
                ' 80094 - Limited Enhancement Endorsement - Commercial ONLY
                'If IsCommercialPolicy Then 'If PolicyHolderType = "2" Then
                '    chkFarmPollution.Checked = Boolean.Parse(hiddenFarmPollution.Value)
                'End If

                If chkFarmPollution.Checked Then
                    Dim wasFound As Boolean
                    Dim farmPollution As QuickQuoteSectionIICoverage
                    If IsCommercialPolicy Then 'If PolicyHolderType = "2" Then
                        farmPollution = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.EnhancementEndorsement)
                    Else
                        farmPollution = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_FarmPollutionLiability)
                    End If
                    'CheckFarmPollutionUpdatedValues(farmPollution, True)
                    If farmPollution Is Nothing Then
                        farmPollution = New QuickQuoteSectionIICoverage()
                        wasFound = False
                        If IsCommercialPolicy Then 'If PolicyHolderType = "2" Then
                            farmPollution.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.EnhancementEndorsement
                        Else
                            farmPollution.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_FarmPollutionLiability
                        End If
                    Else
                        wasFound = True
                    End If

                    Dim increasedLimit = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, ddlFPLimit.SelectedValue, Me.Quote.LobType)
                    farmPollution.IncludedLimit = "25,000"

                    If ddlFPLimit.SelectedValue = "" Then
                        increasedLimit = "0"
                    End If

                    farmPollution.IncreasedLimitId = ddlFPLimit.SelectedValue
                    farmPollution.TotalLimit = Integer.Parse(increasedLimit.Replace(",", "")) 'Since we show the total amount in the dropdown, lets just use that

                    If IsCommercialPolicy Then 'If Me.Quote.Policyholder.Name.TypeId = 2 Then ' 10-12-2016 Sherry said not to send the 25,000 included on non-commericial policies
                        farmPollution.TotalLimit = If(farmPollution.TotalLimit = "0", "25,000", farmPollution.TotalLimit) ' BUG 7783 10-12-16
                    End If

                    If wasFound = False Then
                        MyFarmLocation(MyLocationIndex).SectionIICoverages.Add(farmPollution)
                    End If
                Else
                    Dim farmPollution As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_FarmPollutionLiability)
                    If farmPollution IsNot Nothing Then
                        farmPollution.IncludedLimit = ""
                        farmPollution.IncreasedLimit = ""
                        farmPollution.TotalLimit = ""
                        ddlFPLimit.SelectedValue = ""
                        MyFarmLocation(MyLocationIndex).SectionIICoverages.Remove(farmPollution)
                    End If

                    Dim enhanceEndorse As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.EnhancementEndorsement)
                    If enhanceEndorse IsNot Nothing Then
                        enhanceEndorse.IncludedLimit = ""
                        enhanceEndorse.IncreasedLimit = ""
                        enhanceEndorse.TotalLimit = ""
                        ddlFPLimit.SelectedValue = ""
                        MyFarmLocation(MyLocationIndex).SectionIICoverages.Remove(enhanceEndorse)
                    End If
                End If

                'If chkFarmPollution.Checked Then
                '    If PolicyHolderType = "2" Then
                '        ' 80094 - Limited Enhancement Endorsement - Commercial ONLY
                '        If MyFarmLocation(MyLocationIndex).SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.EnhancementEndorsement).Count <= 0 Then
                '            SectionIICoverage = New QuickQuoteSectionIICoverage()
                '            SectionIICoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.EnhancementEndorsement
                '            Dim increasedLimit = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, ddlFPLimit.SelectedValue, Me.Quote.LobType)
                '            SectionIICoverage.IncludedLimit = "25,000"

                '            If ddlFPLimit.SelectedValue = "" Then
                '                increasedLimit = "0"
                '            End If

                '            SectionIICoverage.IncreasedLimitId = ddlFPLimit.SelectedValue
                '            'SectionIICoverage.TotalLimit = Integer.Parse(increasedLimit.Replace(",", "")) + Integer.Parse(SectionIICoverage.IncludedLimit.Replace(",", ""))
                '            SectionIICoverage.TotalLimit = Integer.Parse(increasedLimit.Replace(",", "")) 'Since we show the total amount in the dropdown, lets just use that
                '            MyFarmLocation(MyLocationIndex).SectionIICoverages.Add(SectionIICoverage)
                '        Else
                '            Dim enhanceEndorse As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.EnhancementEndorsement)
                '            CheckFarmPollutionUpdatedValues(enhanceEndorse)
                '            Dim increasedLimit = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, ddlFPLimit.SelectedValue, Me.Quote.LobType)
                '            enhanceEndorse.IncludedLimit = "25,000"

                '            If ddlFPLimit.SelectedValue = "" Then
                '                increasedLimit = "0"
                '            End If

                '            enhanceEndorse.IncreasedLimitId = ddlFPLimit.SelectedValue
                '            'enhanceEndorse.TotalLimit = Integer.Parse(increasedLimit.Replace(",", "")) + Integer.Parse(enhanceEndorse.IncludedLimit.Replace(",", ""))
                '            enhanceEndorse.TotalLimit = Integer.Parse(increasedLimit.Replace(",", "")) 'Since we show the total amount in the dropdown, lets just use that
                '        End If
                '    Else
                '        ' 40054 - Limited Farm Pollution Liability (Increased Limits) - Personal ONLY
                '        If MyFarmLocation(MyLocationIndex).SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_FarmPollutionLiability).Count <= 0 Then
                '            SectionIICoverage = New QuickQuoteSectionIICoverage()
                '            SectionIICoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_FarmPollutionLiability
                '            Dim increasedLimit = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, ddlFPLimit.SelectedValue, Me.Quote.LobType)
                '            SectionIICoverage.IncludedLimit = "25,000"

                '            If ddlFPLimit.SelectedValue = "" Then
                '                increasedLimit = "0"
                '            End If

                '            SectionIICoverage.IncreasedLimitId = ddlFPLimit.SelectedValue
                '            'SectionIICoverage.TotalLimit = Integer.Parse(increasedLimit.Replace(",", "")) + Integer.Parse(SectionIICoverage.IncludedLimit.Replace(",", ""))
                '            SectionIICoverage.TotalLimit = Integer.Parse(increasedLimit.Replace(",", "")) 'Since we show the total amount in the dropdown, lets just use that
                '            MyFarmLocation(MyLocationIndex).SectionIICoverages.Add(SectionIICoverage)
                '        Else
                '            Dim farmPollution As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_FarmPollutionLiability)
                '            CheckFarmPollutionUpdatedValues(farmPollution)
                '            Dim increasedLimit = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, ddlFPLimit.SelectedValue, Me.Quote.LobType)
                '            farmPollution.IncludedLimit = "25,000"

                '            If ddlFPLimit.SelectedValue = "" Then
                '                increasedLimit = "0"
                '            End If

                '            farmPollution.IncreasedLimitId = ddlFPLimit.SelectedValue
                '            'farmPollution.TotalLimit = Integer.Parse(increasedLimit.Replace(",", "")) + Integer.Parse(farmPollution.IncludedLimit.Replace(",", ""))
                '            farmPollution.TotalLimit = Integer.Parse(increasedLimit.Replace(",", "")) 'Since we show the total amount in the dropdown, lets just use that
                '        End If
                '    End If
                'Else
                '    Dim farmPollution As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_FarmPollutionLiability)
                '    If farmPollution IsNot Nothing Then
                '        farmPollution.IncludedLimit = ""
                '        farmPollution.IncreasedLimit = ""
                '        farmPollution.TotalLimit = ""
                '        ddlFPLimit.SelectedValue = ""
                '        MyFarmLocation(MyLocationIndex).SectionIICoverages.Remove(farmPollution)
                '    End If

                '    Dim enhanceEndorse As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.EnhancementEndorsement)
                '    If enhanceEndorse IsNot Nothing Then
                '        enhanceEndorse.IncludedLimit = ""
                '        enhanceEndorse.IncreasedLimit = ""
                '        enhanceEndorse.TotalLimit = ""
                '        ddlFPLimit.SelectedValue = ""
                '        MyFarmLocation(MyLocationIndex).SectionIICoverages.Remove(enhanceEndorse)
                '    End If
                'End If

                'Updated 9/6/18 for multi state MLW - updated Quote to sq
                ' 01/29/2021 CAH Bug 59434 - Disable for Endorsements
                If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                    If IsQuoteEndorsement() = False Then
                        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                            ' 80330 - EPLI
                            If chkEPLI.Checked Then
                                sq.HasEPLI = True
                                sq.EPLICoverageTypeID = "22"
                                sq.EPLIDeductibleId = "16"
                                sq.EPLICoverageLimitId = "360"
                            Else
                                sq.HasEPLI = False
                                sq.EPLICoverageTypeID = ""
                                sq.EPLIDeductibleId = ""
                                sq.EPLICoverageLimitId = ""
                            End If
                        Next
                    End If
                End If

                ' 70213 - Identity Fraud Expense
                If chkIdentityFraud.Checked Then
                    If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.IdentityFraudExpense).Count <= 0 Then
                        SectionICoverage = New QuickQuoteSectionICoverage()
                        SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.IdentityFraudExpense
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                    End If
                Else
                    Dim identFraud As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.IdentityFraudExpense)
                    If identFraud IsNot Nothing Then
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(identFraud)
                    End If
                End If
                ' Motorized Vehicles - OH
                If chkMotorizedVehicles.Checked Then
                    If MyFarmLocation(MyLocationIndex).SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Motorized_Vehicles_Ohio).Count <= 0 Then
                        SectionIICoverage = New QuickQuoteSectionIICoverage()
                        SectionIICoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Motorized_Vehicles_Ohio
                        MyFarmLocation(MyLocationIndex).SectionIICoverages.Add(SectionIICoverage)
                    End If
                Else
                    Dim motoVeh As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Motorized_Vehicles_Ohio)
                    If motoVeh IsNot Nothing Then
                        MyFarmLocation(MyLocationIndex).SectionIICoverages.Remove(motoVeh)
                    End If
                End If

                'Added 11/3/2022 for task 60749 MLW
                If IFM.VR.Common.Helpers.FARM.FarmCustomFeeding.IsFARCustomFeedingAvailable(Quote) Then
                    'Custom Feeding
                    If chkCustomFeeding.Checked Then
                        If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                            For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                                'Policy Level Coverage - Custom Feeding
                                ' Swine
                                If ddCFSwineLimit.SelectedValue <> "" AndAlso ddCFSwineLimit.SelectedValue <> "0" Then
                                    sq.FarmCustomFeedingSwineDescription = txtCFSwineDesc.Text
                                    sq.FarmCustomFeedingSwineLimitId = ddCFSwineLimit.SelectedValue
                                Else
                                    sq.FarmCustomFeedingSwineDescription = ""
                                    sq.FarmCustomFeedingSwineLimitId = ""
                                End If
                                ' Poultry
                                If ddCFPoultryLimit.SelectedValue <> "" AndAlso ddCFPoultryLimit.SelectedValue <> "0" Then
                                    sq.FarmCustomFeedingPoultryDescription = txtCFPoultryDesc.Text
                                    sq.FarmCustomFeedingPoultryLimitId = ddCFPoultryLimit.SelectedValue
                                Else
                                    sq.FarmCustomFeedingPoultryDescription = ""
                                    sq.FarmCustomFeedingPoultryLimitId = ""
                                End If
                                ' Cattle
                                If ddCFCattleLimit.SelectedValue <> "" AndAlso ddCFCattleLimit.SelectedValue <> "0" Then
                                    sq.FarmCustomFeedingCattleDescription = txtCFCattleDesc.Text
                                    sq.FarmCustomFeedingCattleLimitId = ddCFCattleLimit.SelectedValue
                                Else
                                    sq.FarmCustomFeedingCattleDescription = ""
                                    sq.FarmCustomFeedingCattleLimitId = ""
                                End If
                                ' Equine
                                If ddCFEquineLimit.SelectedValue <> "" And ddCFEquineLimit.SelectedValue <> "0" Then
                                    sq.FarmCustomFeedingEquineDescription = txtCFEquineDesc.Text
                                    sq.FarmCustomFeedingEquineLimitId = ddCFEquineLimit.SelectedValue
                                Else
                                    sq.FarmCustomFeedingEquineDescription = ""
                                    sq.FarmCustomFeedingEquineLimitId = ""
                                End If
                            Next
                        End If
                    Else
                        ' Custom feeding NOT checked - remove any existing
                        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                            sq.FarmCustomFeedingSwineDescription = txtCFSwineDesc.Text
                            sq.FarmCustomFeedingSwineLimitId = ddCFSwineLimit.SelectedValue
                            sq.FarmCustomFeedingPoultryDescription = txtCFPoultryDesc.Text
                            sq.FarmCustomFeedingPoultryLimitId = ddCFPoultryLimit.SelectedValue
                            sq.FarmCustomFeedingCattleDescription = txtCFCattleDesc.Text
                            sq.FarmCustomFeedingCattleLimitId = ddCFCattleLimit.SelectedValue
                            sq.FarmCustomFeedingEquineDescription = txtCFEquineDesc.Text
                            sq.FarmCustomFeedingEquineLimitId = ddCFEquineLimit.SelectedValue
                        Next
                    End If
                End If

                'Updated 9/6/18 for multi state MLW - updated Quote to sq
                If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                    For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                        ' 80125 - Farm All Star
                        If chkFarmAllStar.Checked Then
                            If IFM.VR.Common.Helpers.FARM.FarmAllStarHelper.IsFarmAllStarAvailable(Quote) Then
                                sq.HasFarmAllStar = True
                                sq.FarmAllStarWaterBackupLimitId = ddlBSDLimit.SelectedValue
                                sq.FarmAllStarWaterDamageLimitId = ddlWaterDamageLimit.SelectedValue
                                sq.FarmAllStarLimitId = ""
                            Else
                                sq.HasFarmAllStar = False
                                sq.FarmAllStarWaterBackupLimitId = ""
                                sq.FarmAllStarWaterDamageLimitId = ""
                                sq.FarmAllStarLimitId = ddlBSDLimit.SelectedValue
                            End If                            
                        Else
                            sq.HasFarmAllStar = False
                            sq.FarmAllStarWaterBackupLimitId = ""
                            sq.FarmAllStarWaterDamageLimitId = ""
                            sq.FarmAllStarLimitId = ""
                        End If

                        ' 80140 Extra Expense  (See above, this is saved to Governing State now MGB 2-1-19 Bug 31175
                        'If chkExtraExpense.Checked Then
                        '    Dim optionalCoverage As QuickQuoteOptionalCoverage

                        '    If sq.OptionalCoverages Is Nothing Then
                        '        sq.OptionalCoverages = New List(Of QuickQuoteOptionalCoverage)
                        '    End If

                        '    If sq.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense).Count <= 0 Then
                        '        optionalCoverage = New QuickQuoteOptionalCoverage()
                        '        optionalCoverage.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense
                        '        optionalCoverage.IncreasedLimit = txtExtraExpenseLimit.Text
                        '        sq.OptionalCoverages.Add(optionalCoverage)
                        '    Else
                        '        Dim updateExtraExpense As QuickQuoteOptionalCoverage = sq.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)
                        '        updateExtraExpense.IncreasedLimit = txtExtraExpenseLimit.Text
                        '    End If
                        'Else
                        '    If sq.OptionalCoverages IsNot Nothing Then
                        '        Dim extraExpense As QuickQuoteOptionalCoverage = sq.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)
                        '        If extraExpense IsNot Nothing Then
                        '            dvExtraExpenseLimit.Attributes.Add("style", "display:none;")
                        '            extraExpense.IncreasedLimit = ""
                        '            sq.OptionalCoverages.Remove(extraExpense)
                        '        End If
                        '    End If
                        'End If

                        ' Farm Extender - Boolean
                        If chkFarmExtend.Checked Then
                            sq.HasFarmExtender = True
                        Else
                            sq.HasFarmExtender = False
                        End If

                        ' Equipment Breakdown - Boolean
                        If chkEquipBreak.Checked Then
                            sq.HasFarmEquipmentBreakdown = True
                        Else
                            sq.HasFarmEquipmentBreakdown = False
                        End If

                        ' 70125 - Pollutant Clean Up and Removal - Increased Limits
                        If chkPollution.Checked Then
                            If sq.FarmIncidentalLimits.FindAll(Function(p) p.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Pollutant_Clean_Up_and_Removal).Count <= 0 Then
                                farmIncidentalCoverage = New QuickQuoteFarmIncidentalLimit()
                                farmIncidentalCoverage.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Pollutant_Clean_Up_and_Removal
                                farmIncidentalCoverage.IncreasedLimitId = "48"
                                sq.FarmIncidentalLimits.Add(farmIncidentalCoverage)
                            Else
                                Dim increaseLimit = sq.FarmIncidentalLimits.Find(Function(p) p.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Pollutant_Clean_Up_and_Removal)
                                increaseLimit.IncreasedLimitId = "48"
                            End If
                        Else
                            If sq.FarmIncidentalLimits IsNot Nothing Then
                                Dim farmPollution As QuickQuoteFarmIncidentalLimit = sq.FarmIncidentalLimits.Find(Function(p) p.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Pollutant_Clean_Up_and_Removal)
                                If farmPollution IsNot Nothing Then
                                    farmPollution.IncreasedLimitId = ""
                                    'Quote.FarmIncidentalLimits.Remove(farmPollution)
                                End If
                            End If
                        End If

                        sq.AggregateLiabilityIncrementTypeId = "1" ' Sets Aggregate Liability Increment to "2"
                    Next
                End If

                If Common.Helpers.FARM.RefFoodSpoilageHelper.IsRefFoodSpoilageAvailable(Quote) Then
                    If chkRefFoodSpoilage.Checked Then
                        ' 70148 - Ref Food Spoilage
                        Dim RefFoodSpoilage As QuickQuoteCoverage
                        If MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages Is Nothing Then
                            MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages = New List(Of QuickQuoteCoverage)
                        End If
                        If MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.FindAll(Function(p) p.CoverageCodeId = "70148").Count > 0 Then
                            RefFoodSpoilage = MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.Find(Function(p) p.CoverageCodeId = "70148")
                            RefFoodSpoilage.ManualLimitIncreased = txtRefFoodSpoilageIncreaseLimit.Text
                            RefFoodSpoilage.ManualLimitIncluded = txtRefFoodSpoilageIncludedLimit.Text
                            RefFoodSpoilage.ManualLimitAmount = txtRefFoodSpoilageTotalLimit.Text
                        Else
                            RefFoodSpoilage = New QuickQuoteCoverage()
                            RefFoodSpoilage.CoverageCodeId = "70148"
                            RefFoodSpoilage.ManualLimitIncreased = txtRefFoodSpoilageIncreaseLimit.Text
                            RefFoodSpoilage.ManualLimitIncluded = txtRefFoodSpoilageIncludedLimit.Text
                            RefFoodSpoilage.ManualLimitAmount = txtRefFoodSpoilageTotalLimit.Text
                            MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.Add(RefFoodSpoilage)

                            txtRefFoodSpoilageIncreaseLimit.Text = Format(RefFoodSpoilage.ManualLimitIncreased.TryToGetInt32, "N0")
                            txtRefFoodSpoilageTotalLimit.Text = Format(RefFoodSpoilage.ManualLimitAmount.TryToGetInt32, "N0")
                        End If
                    End If
                End If

                SaveChildControls()
            End If
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = "Farm Policy Coverage"
        Dim divCoverages As String = dvFarmPolicyCoverages.ClientID

        Dim valList = PolicyCoverageValidator.ValidateFARCoverages(Quote, MyLocationIndex, valArgs.ValidationType, CustomFarmingErr, txtCFAnnualReceipts.Text)

        If valList.Any() Then
            For Each v In valList
                ' *********************
                ' Base Policy Coverages
                ' *********************
                Select Case v.FieldId
                    Case PolicyCoverageValidator.CovLRequired
                        ValidationHelper.Val_BindValidationItemToControl(ddlLiability, v, divCoverages, "0")
                    Case PolicyCoverageValidator.CovMRequired
                        ValidationHelper.Val_BindValidationItemToControl(ddlMedPay, v, divCoverages, "0")
                    Case PolicyCoverageValidator.NumEmpRequired
                        ValidationHelper.Val_BindValidationItemToControl(dvEmployeeLiab, v, divCoverages, "0")
                    Case PolicyCoverageValidator.CustFarmTypeRequired
                        ValidationHelper.Val_BindValidationItemToControl(ddlCFType, v, divCoverages, "0")
                    Case PolicyCoverageValidator.CustFarmReciptsRequired
                        ValidationHelper.Val_BindValidationItemToControl(txtCFAnnualReceipts, v, divCoverages, "0")
                    Case PolicyCoverageValidator.FMPNumPersRequired
                        ValidationHelper.Val_BindValidationItemToControl(txtFMPNumPer, v, divCoverages, "0")
                    Case PolicyCoverageValidator.BusinessTypeRequired
                        ValidationHelper.Val_BindValidationItemToControl(ddlBPType, v, divCoverages, "0")
                    Case PolicyCoverageValidator.BusinessReceiptsRequired
                        ValidationHelper.Val_BindValidationItemToControl(txtAnnualReceipts, v, divCoverages, "0")
                    Case PolicyCoverageValidator.ExtraExpenseLimitRequired
                        If FarmExtenderHelper.IsFarmExtenderAvailable(Quote) Then
                            ValidationHelper.Val_BindValidationItemToControl(txtExtraExpenseIncreasedLimit, v, divCoverages, "0")
                        Else
                            ValidationHelper.Val_BindValidationItemToControl(txtExtraExpenseLimit, v, divCoverages, "0")
                        End If
                    Case PolicyCoverageValidator.ExtraExpenseZero
                        If FarmExtenderHelper.IsFarmExtenderAvailable(Quote) Then
                            ValidationHelper.Val_BindValidationItemToControl(txtExtraExpenseIncreasedLimit, v, divCoverages, "0")
                        Else
                            ValidationHelper.Val_BindValidationItemToControl(txtExtraExpenseLimit, v, divCoverages, "0")
                        End If
                    Case PolicyCoverageValidator.InvalidValueForFarmPollutionLiabilityUpdate
                        ValidationHelper.Val_BindValidationItemToControl(ddlFPLimit, v, divCoverages, "0")
                    Case PolicyCoverageValidator.OccurrenceLiabilityIsLessThanFarmPollution
                        ValidationHelper.Val_BindValidationItemToControl(ddlFPLimit, v, divCoverages, "0")
                    Case PolicyCoverageValidator.InvalidPayrollAmount, PolicyCoverageValidator.PayrollRequired
                        ValidationHelper.Val_BindValidationItemToControl(txtStopGapPayroll, v, divCoverages, "0")
                    Case PolicyCoverageValidator.StopGapLimitRequired
                        ValidationHelper.Val_BindValidationItemToControl(ddStopGapLimit, v, divCoverages, "0")
                End Select
            Next
        End If

        'Added 11/3/2022 for task 60749 MLW
        If IFM.VR.Common.Helpers.FARM.FarmCustomFeeding.IsFARCustomFeedingAvailable(Quote) Then
            ' Custom Feeding
            If chkCustomFeeding.Checked Then
                If QQHelper.IsPositiveIntegerString(ddCFCattleLimit.SelectedValue) = False AndAlso QQHelper.IsPositiveIntegerString(ddCFEquineLimit.SelectedValue) = False AndAlso QQHelper.IsPositiveIntegerString(ddCFPoultryLimit.SelectedValue) = False AndAlso QQHelper.IsPositiveIntegerString(ddCFSwineLimit.SelectedValue) = False Then
                    Me.ValidationHelper.AddError("When Custom Feeding is selected you must select at least one limit for Cattle, Equine, Poultry or Swine and enter a description for your selection(s).")
                ElseIf IsQuoteEndorsement() = False Then
                    If QQHelper.IsPositiveIntegerString(ddCFCattleLimit.SelectedValue) Then
                        If txtCFCattleDesc.Text.Trim = "" Then Me.ValidationHelper.AddError("Cattle description is required", txtCFCattleDesc.ClientID)
                    End If
                    If QQHelper.IsPositiveIntegerString(ddCFEquineLimit.SelectedValue) Then
                        If txtCFEquineDesc.Text.Trim = "" Then Me.ValidationHelper.AddError("Equine description is required", txtCFEquineDesc.ClientID)
                    End If
                    If QQHelper.IsPositiveIntegerString(ddCFPoultryLimit.SelectedValue) Then
                        If txtCFPoultryDesc.Text.Trim = "" Then Me.ValidationHelper.AddError("Poultry description is required", txtCFPoultryDesc.ClientID)
                    End If
                    If QQHelper.IsPositiveIntegerString(ddCFSwineLimit.SelectedValue) Then
                        If txtCFSwineDesc.Text.Trim = "" Then Me.ValidationHelper.AddError("Swine description is required", txtCFSwineDesc.ClientID)
                    End If
                End If
            End If
        End If

        ValidateChildControls(valArgs)
        PopulateChildControls()
    End Sub

    Protected Sub lnkSaveGeneralInfo_Click(sender As Object, e As EventArgs) Handles lnkSaveGeneralInfo.Click, lnkBtnSave.Click, lnkPropSave.Click, btnSubmit.Click, btnSaveGotoNextSection.Click
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New VRValidationArgs(DefaultValidationType)))

        If sender Is btnSaveGotoNextSection Then
            If ValidationSummmary.HasErrors = False Then
                RaiseEvent RequestNavigationToLocation()
            Else
                Populate()
            End If
        Else
            Populate()
        End If
    End Sub

    Protected Sub lnkBtnClear_Click(sender As Object, e As EventArgs) Handles lnkBtnClear.Click
        ClearLiabilityCoverage()
    End Sub

    Protected Sub lnkPropClear_Click(sender As Object, e As EventArgs) Handles lnkPropClear.Click
        ClearPropertyCoverage()
    End Sub

    Protected Sub lnkClearGeneralInfo_Click(sender As Object, e As EventArgs) Handles lnkClearGeneralInfo.Click
        ClearControl()
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        ClearLiabilityCoverage()
        ClearPropertyCoverage()
        ClearChildControls()
    End Sub

    Private Sub ClearLiabilityCoverage()
        If ProgramType = "7" Then
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlLiabCovType, "0")
        End If

        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlLiability, "0")
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlMedPay, "0")

        If chkEmpLiab.Checked Then
            chkEmpLiab.Checked = False
            txtFTEmp.Text = ""
            txtPT40Days.Text = ""
            txtPT41Days.Text = ""
            dvNumEmployees.Attributes.Add("style", "display:none;")
        End If

        If chkBusinessPursuits.Checked Then
            chkBusinessPursuits.Checked = False
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlBPType, "0")
            txtAnnualReceipts.Text = ""
            dvBPInfo.Attributes.Add("style", "display:none;")
        End If

        If chkFamMedPay.Checked Then
            chkFamMedPay.Checked = False
            txtFMPNumPer.Text = ""
            dvFMPNumPer.Attributes.Add("style", "display:none;")
        End If

        If chkCustomFarming.Checked Then
            chkCustomFarming.Checked = False
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlCFType, "0")
            txtCFAnnualReceipts.Text = ""
            dvCFInfo.Attributes.Add("style", "display:none;")
        End If

        If chkFarmPollution.Checked Then
            chkFarmPollution.Checked = False
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlFPLimit, "0")
            dvFPIncreasedLimits.Attributes.Add("style", "display:none;")
        End If

        If IsCommercialPolicy Then 'If PolicyHolderType = "2" Then
            dvEPLI.Attributes.Add("style", "display:block;")
            'chkEPLI.Checked = True
            setEPLI(True)
        Else
            dvEPLI.Attributes.Add("style", "display:none;")
            'chkEPLI.Checked = False
            setEPLI(False)
        End If

        If chkAdditionalIns.Checked Then
            MyFarmLocation(0).AdditionalInterests = Nothing
            dvAIInfo.Attributes.Add("style", "display:none;")
            chkAdditionalIns.Checked = False
            chkAdditionalIns.Enabled = True
        End If

        chkIdentityFraud.Checked = False
        chkEPLI.Checked = False

        If dvCanine.Visible Then
            Cov_CanineExclusionList.ClearControl()
        End If

        'Added 11/3/2022 for task 60749 MLW
        'Custom Feeding
        Me.chkCustomFeeding.Checked = False
        Me.dvCustomFeedingData.Attributes.Add("style", "display:none")
        Me.ddCFCattleLimit.SelectedIndex = -1
        Me.ddCFEquineLimit.SelectedIndex = -1
        Me.ddCFPoultryLimit.SelectedIndex = -1
        Me.ddCFSwineLimit.SelectedIndex = -1
        Me.txtCFCattleDesc.Text = ""
        Me.txtCFEquineDesc.Text = ""
        Me.txtCFPoultryDesc.Text = ""
        Me.txtCFSwineDesc.Text = ""

        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub

    Private Sub ClearPropertyCoverage()
        If chkFarmAllStar.Checked Then
            chkFarmAllStar.Checked = False
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlBSDLimit, "15")
            If IFM.VR.Common.Helpers.FARM.FarmAllStarHelper.IsFarmAllStarAvailable(Quote) Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlWaterDamageLimit, "15")
            End If
            dvBackSewerDrain.Attributes.Add("style", "display:none;")
        End If

        chkEquipBreak.Checked = False

        If chkExtraExpense.Checked Then
            chkExtraExpense.Checked = False
            If FarmExtenderHelper.IsFarmExtenderAvailable(Quote) Then
                dvExtraExpenseIncreasedLimits.Attributes.Add("style", "display:none;")
                chkExtraExpense.Enabled = True
                txtExtraExpenseIncludedLimit.Text = ""
                txtExtraExpenseIncreasedLimit.Text = ""
                txtExtraExpenseTotalLimit.Text = ""
            Else
                txtExtraExpenseLimit.Text = ""
            End If
            dvExtraExpenseLimit.Attributes.Add("style", "display:none;")
        End If

        chkFarmExtend.Checked = False
        chkPollution.Checked = False

        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub

    Private Sub AddNewAdditionalInsured()
        If MyFarmLocation IsNot Nothing Then
            If MyFarmLocation(0).AdditionalInterests Is Nothing Then
                MyFarmLocation(0).AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)()
            End If

            Dim newAdditionalInsured As New QuickQuoteAdditionalInterest()

            newAdditionalInsured.TypeId = "-1"
            MyFarmLocation(0).AdditionalInterests.Add(newAdditionalInsured)
            chkAdditionalIns.Enabled = False

            SaveChildControls()
            PopulateChildControls()
            Save()
            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            Populate()
        End If
    End Sub

    Protected Sub chkAdditionalIns_CheckedChanged(sender As Object, e As EventArgs) Handles chkAdditionalIns.CheckedChanged, lnkAddAI.Click
        If MyFarmLocation IsNot Nothing Then
            AddNewAdditionalInsured()
            dvAIInfo.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Private Sub HideAddlInsuredInfo() Handles ctlAdditionalInsuredList.HideAdditionalInsuredInfo
        dvAIInfo.Attributes.Add("style", "display:none;")
        chkAdditionalIns.Enabled = True
        chkAdditionalIns.Checked = False
    End Sub

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        MyBase.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        ' 07/13/2022 CAH - Old Eff Date (feature flag) from 2016.  Safe to remove.
        'EffectiveDateHelper_FarmPollutionAndFarmEnhancement.CheckUpdatedFarmPollutionAndFarmEnhancement(Me.Quote, NewEffectiveDate, Me)
        EffectiveDateChangedNotifyChildControls(NewEffectiveDate, OldEffectiveDate)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles chkFamMedPay.CheckedChanged
        If IsQuoteEndorsement() Then
            isFMPChecked()
        End If
    End Sub

    Private Sub isFMPChecked()
        Dim SectionIICoverage As QuickQuoteSectionIICoverage = Nothing
        If chkFamMedPay.Checked Then
            dvFamilyMedPayNames.Visible = True

            If MyFarmLocation(0).SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments).Count <= 0 Then
                SectionIICoverage = New QuickQuoteSectionIICoverage()
                SectionIICoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments
                SectionIICoverage.NumberOfPersonsReceivingCare = "1"
                SectionIICoverage.IncludedLimit = "1,000" 'Added 5/30/18 for Bug 20406 MLW
                SectionIICoverage.TotalLimit = "1,000" 'Added 5/30/18 for Bug 20406 MLW
                MyFarmLocation(0).SectionIICoverages.Add(SectionIICoverage)

                If MyFarmLocation(0).ResidentNames Is Nothing Then
                    MyFarmLocation(0).ResidentNames = New List(Of QuickQuoteResidentName)
                End If

                MyFarmLocation(0).ResidentNames.Add(New QuickQuoteResidentName)
            End If
            Save_FireSaveEvent(False)
            Populate()
        Else
            Dim famMed As QuickQuoteSectionIICoverage = MyFarmLocation(0).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments)
            If famMed IsNot Nothing Then
                MyFarmLocation(0).SectionIICoverages.Remove(famMed)
            End If
            MyFarmLocation(0).ResidentNames = Nothing
            dvFamilyMedPayNames.Visible = False
            Save_FireSaveEvent(False)
            Populate()
        End If
    End Sub

    Public Sub setEPLI(bool As Boolean)
        If IsQuoteEndorsement() = False Then
            chkEPLI.Checked = bool
        End If
    End Sub

    Private Sub btnRate_Click(sender As Object, e As EventArgs) Handles btnRate_Endorsements.Click, btnRate.Click
        RaiseEvent QuoteRateRequested()
    End Sub

    'Added 11/3/2022 for task 60749 MLW
    Private Function QuoteHasCustomFeedingCoverage(Optional ByVal CustomFeedingType As AnimalType_enum = AnimalType_enum.All) As Boolean
        If CustomFeedingType = AnimalType_enum.All Then
            'If ((Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmCustomFeedingSwineLimitId) AndAlso SubQuoteFirst.FarmCustomFeedingSwineLimitId <> "0")) OrElse ((Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmCustomFeedingPoultryLimitId) AndAlso SubQuoteFirst.FarmCustomFeedingPoultryLimitId <> "0")) OrElse ((Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmCustomFeedingCattleLimitId)) AndAlso SubQuoteFirst.FarmCustomFeedingCattleLimitId <> "0") OrElse ((Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmCustomFeedingEquineLimitId)) AndAlso SubQuoteFirst.FarmCustomFeedingEquineLimitId <> "0") Then
            If QQHelper.IsPositiveIntegerString(SubQuoteFirst.FarmCustomFeedingSwineLimitId) OrElse QQHelper.IsPositiveIntegerString(SubQuoteFirst.FarmCustomFeedingPoultryLimitId) OrElse QQHelper.IsPositiveIntegerString(SubQuoteFirst.FarmCustomFeedingCattleLimitId) OrElse qqHelper.IsPositiveIntegerString(SubQuoteFirst.FarmCustomFeedingEquineLimitId) Then
                Return True
            End If
        Else
            Select Case CustomFeedingType
                Case AnimalType_enum.Swine
                    If qqHelper.IsPositiveIntegerString(SubQuoteFirst.FarmCustomFeedingSwineLimitId) Then Return True
                    'If Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmCustomFeedingSwineLimitId) Then Return True
                    Exit Select
                Case AnimalType_enum.Poultry
                    If qqHelper.IsPositiveIntegerString(SubQuoteFirst.FarmCustomFeedingPoultryLimitId) Then Return True
                    Exit Select
                Case AnimalType_enum.Cattle
                    If qqHelper.IsPositiveIntegerString(SubQuoteFirst.FarmCustomFeedingCattleLimitId) Then Return True
                    Exit Select
                Case AnimalType_enum.Equine
                    If qqHelper.IsPositiveIntegerString(SubQuoteFirst.FarmCustomFeedingEquineLimitId) Then Return True
                    Exit Select
            End Select
        End If

        Return False
    End Function

    Private Sub ddlLiabCovType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlLiabCovType.SelectedIndexChanged
        Dim msg As String = "The Liability Form has been changed on this quote. Please review the Policyholder information and update accordingly. With the change of Liability Forms there has been a change to the Included and Optional Coverages. Please review your quote in its entirety to verify the coverage applied."
        If ddlLiabCovType.SelectedValue = "1" Then
            'Switching from Commercial to Personal

            'Set EPLI defaulting, Equipment Breakdown defaulting, and Limited Farm Pollution defaulting - all other coverages will retain the value that the user had prior to switching between personal and commercial.
            'dvEPLI.Attributes.Add("style", "display:none;") 'keeping this here in case BU decides that they do not want to redirect the user to PH page. We'd need to show/hide this coverage as needed between personal and commercial.
            setEPLI(False)
            chkEquipBreak.Checked = True 'since users rarely uncheck EB, BU wants this to be checked back by default when toggling between personal and commercial.
            'lblFarmPollution.Text = "Limited Farm Pollution (Increased Limits)" 'keeping this here in case BU decides that they do not want to redirect the user to PH page. We'd need update the coverage name for this coverage as needed between personal and commercial.
            chkFarmPollution.Checked = False 'same coverage used between personal and commercial. It is defaulted to checked on commercial and unchecked on personal. BU wants it unchecked on personal and checked on commercial by default when toggling between the two.

            'save applicant Name - first, middle, last - as PH Name - first, middle, last; The Policyholder name field will automatically be populated using the Applicant name
            If Quote IsNot Nothing Then
                If Quote.Policyholder Is Nothing Then
                    Quote.Policyholder = New QuickQuotePolicyholder
                End If
                If Quote.Policyholder.Name Is Nothing Then
                    Quote.Policyholder.Name = New QuickQuoteName
                End If
                If GoverningStateQuote IsNot Nothing AndAlso GoverningStateQuote.Applicants IsNot Nothing AndAlso GoverningStateQuote.Applicants.Count > 0 Then
                    Quote.Policyholder.Name.FirstName = GoverningStateQuote.Applicants(0).Name.FirstName                   
                    Quote.Policyholder.Name.MiddleName = GoverningStateQuote.Applicants(0).Name.MiddleName           
                    Quote.Policyholder.Name.LastName = GoverningStateQuote.Applicants(0).Name.LastName
                End If
                Quote.Policyholder.Name.CommercialName1 = "" 'Otherwise this throws a validation error "Must be a personal name but you have a commercial name"
                Quote.Policyholder.Name.TypeId = 1 'Personal                
            End If            
        Else If ddlLiabCovType.SelectedValue = "2" Then
            'Switching from Personal to Commercial

            'Set EPLI defaulting, Equipment Breakdown defaulting, and Liability Enhancement Endorsement defaulting - all other coverages will retain the value that the user had prior to switching between personal and commercial.
            'dvEPLI.Attributes.Add("style", "display:block;") 'keeping this here in case BU decides that they do not want to redirect the user to PH page. We'd need to show/hide this coverage as needed between personal and commercial.
            setEPLI(True)
            chkEquipBreak.Checked = True 'since users rarely uncheck EB, BU wants this to be checked back by default when toggling between personal and commercial.
            'lblFarmPollution.Text = "Liability Enhancement Endorsement" 'keeping this here in case BU decides that they do not want to redirect the user to PH page. We'd need update the coverage name for this coverage as needed between personal and commercial.
            chkFarmPollution.Checked = True 'same coverage used between personal and commercial. It is defaulted to checked on commercial and unchecked on personal. BU wants it unchecked on personal and checked on commercial by default when toggling between the two.

            'save PH Name - first, middle, last - as applicant Name - first, middle, last; The applicant name field will automatically be populated using the Policyholder name
            If Quote IsNot Nothing AndAlso GoverningStateQuote IsNot Nothing Then
                If GoverningStateQuote.Applicants Is Nothing Then
                    GoverningStateQuote.Applicants = New List(Of QuickQuoteApplicant)()
                End If
                If Quote.Policyholder Is Nothing Then
                    Quote.Policyholder = New QuickQuotePolicyholder
                End If
                If Quote.Policyholder.Name Is Nothing Then
                    Quote.Policyholder.Name = New QuickQuoteName
                End If
                GoverningStateQuote.Applicants(0).Name.FirstName = Quote.Policyholder.Name.FirstName
                GoverningStateQuote.Applicants(0).Name.MiddleName = Quote.Policyholder.Name.MiddleName
                GoverningStateQuote.Applicants(0).Name.LastName = Quote.Policyholder.Name.LastName
                GoverningStateQuote.Applicants(0).Name.CommercialName1 = "" 'Otherwise this throws a validation error "Must be a personal name but you have a commercial name"
                Quote.Policyholder.Name.FirstName = "" 'Otherwise gives validation error "Must be a commercial name but you have a personal name"
                Quote.Policyholder.Name.MiddleName = ""
                Quote.Policyholder.Name.LastName = ""
                Quote.Policyholder.Name.SexId = "" 'Otherwise gives validation error "Invalid Gender"
                Quote.Policyholder.Name.TypeId = 2 'Commercial
            End If
        End If
        MessageBoxVRPers.Show(msg, Response, ScriptManager.GetCurrent(Me.Page), Me)
        Save_FireSaveEvent(False)
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                
    End Sub
End Class