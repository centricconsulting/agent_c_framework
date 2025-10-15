Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.DFR
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.DFR

Public Class ctlDFRResidenceCoverages
    Inherits VRControlBase

    Dim pvHelper As New QuickQuotePropertyValuationHelperClass
    Dim ClassName As String = "ctlDFRQuote"
    Public Event QuoteRateRequested()

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
            Try
                Return MyLocation.FormTypeId
            Catch ex As Exception
                Return ""
            End Try

        End Get
    End Property

    Public Property DeductibleDivId As String
        Get
            If ViewState("vs_DeductibleDivId_") IsNot Nothing Then
                Return ViewState("vs_DeductibleDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_DeductibleDivId_") = value
        End Set
    End Property

    Public Property BaseCoverageDivId As String
        Get
            If ViewState("vs_BaseCoverageDivId_") IsNot Nothing Then
                Return ViewState("vs_BaseCoverageDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_BaseCoverageDivId_") = value
        End Set
    End Property

    Public Property OptionalCoverageDivId As String
        Get
            If ViewState("vs_OptionalCoverageDivId_") IsNot Nothing Then
                Return ViewState("vs_OptionalCoverageDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_OptionalCoverageDivId_") = value
        End Set
    End Property

    'Added 05/18/2020 for Bug 46773 MLW
    Public Property RouteToUwIsVisible As Boolean
        Get
            Return Me.ctl_RouteToUw.Visible
        End Get
        Set(value As Boolean)
            Me.ctl_RouteToUw.Visible = value
        End Set
    End Property

    Private Sub SetDefaultValues()
        If Me.MyLocation IsNot Nothing Then
            'Set dropdown lists to default values
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlDeductible, "1,000")
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlPersonalLiability, "100,000")
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlMedicalPayments, "1,000")

            'Zero out E2Value
            lblReplacementCCValue.Text = "0"

            'Zero out Base Coverages
            'If CurrentForm = "HO-6" Then
            '    txtDwLimit.Text = "1,000"
            'Else
            '    txtDwLimit.Text = "0"
            'End If

            txtDwellingChangeInLimit.Text = "0"
            txtDwellingTotalLimit.Text = txtDwLimit.Text
            'hiddenDwellingLimit.Value = txtDwLimit.Text
            'hiddenDwellingChange.Value = "0"
            'hiddenDwellingTotal.Value = hiddenDwellingLimit.Value

            txtOtherLimit.Text = "0"
            txtOtherChgInLimit.Text = "0"
            txtOtherTotalLimit.Text = "0"
            'hiddenOtherLimit.Value = "0"
            'hiddenOtherChange.Value = "0"
            'hiddenOtherTotal.Value = "0"

            txtPPLimit.Text = "0"
            If Integer.Parse(txtPPChgInLimit.Text.Replace(",", "")) < 5000 Then
                txtPPChgInLimit.Text = "0"
                txtPPTotalLimit.Text = "0"
            End If

            'hiddenPPLimit.Value = "0"
            'hiddenPPChange.Value = "0"
            'hiddenPPTotal.Value = "0"

            txtLivingLimit.Text = "0"
            txtLivingChgInLimit.Text = "0"
            txtLivingTotalLimit.Text = "0"
            'hiddenLivingLimit.Value = "0"
            'hiddenLivingChange.Value = "0"
            'hiddenLivingTotal.Value = "0"
        End If
    End Sub

    Private Function FormatCoverageGridNumber(ByVal num As String) As String
        Dim newnum As String = Nothing
        Try
            newnum = num.Replace(",", "")
            newnum = newnum.Replace("$", "")
            Return newnum
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        ValidationHelper.GroupName = "Dwelling Fire Coverage"
        Dim divCoverages As String = dvCoverages.ClientID

        Dim valList = ResidenceCoverageValidator.ValidateDFRLocationResidence(Quote, 0, valArgs.ValidationType)

        If valList.Any() Then
            For Each v In valList
                ' *********************
                ' Base Policy Coverages
                ' *********************
                Select Case v.FieldId
                    Case ResidenceCoverageValidator.CoverageAMin,
                        ResidenceCoverageValidator.MissingCoverageA
                        ValidationHelper.Val_BindValidationItemToControl(txtDwLimit, v, divCoverages, "0")
                    Case ResidenceCoverageValidator.MaxCovBTotal
                        ValidationHelper.Val_BindValidationItemToControl(txtOtherChgInLimit, v, divCoverages, "0")
                    Case ResidenceCoverageValidator.ReqMedPay
                        ValidationHelper.Val_BindValidationItemToControl(ddlMedicalPayments, v, divCoverages, "0")
                    Case ResidenceCoverageValidator.ReqPresLiab
                        ValidationHelper.Val_BindValidationItemToControl(ddlPersonalLiability, v, divCoverages, "0")
                End Select
            Next
        End If

        ValidateChildControls(valArgs)
    End Sub

    Protected Sub SaveQuote() 'Handles ctlOptionalCoverages.StructSaveRequest
        Save_FireSaveEvent()
        LoadStaticData()
        Populate()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lblCoverageForm.Text = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, CurrentForm)

        If Not IsPostBack Then
            MainAccordionDivId = dvCoverages.ClientID
            DeductibleDivId = dvDeductible.ClientID
            BaseCoverageDivId = dvBaseCoverage.ClientID
            OptionalCoverageDivId = dvOptionalCoverages.ClientID

            LoadStaticData()
            Populate()
        End If
    End Sub

    Protected Sub lnkClearCoverages_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkClearCoverages.Click
        Session("vendorValueDFR") = "False"
        ClearControl()
    End Sub

    Protected Sub lnkSaveCoverages_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSaveCoverages.Click
        Session("vendorValueDFR") = "False"
        SaveQuote()
    End Sub

    Protected Sub btnSaveBase_Click(sender As Object, e As EventArgs) Handles btnSaveBase.Click
        Session("vendorValueDFR") = "False"
        SaveQuote()
    End Sub

    Protected Sub btnRateBase_Click(sender As Object, e As EventArgs) Handles btnRateBase.Click
        Session("vendorValueDFR") = "False"
        RaiseEvent QuoteRateRequested()
    End Sub

    'Added 9/18/2019 for DFR Endorsements Project Task 40278 MLW
    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    'Added 9/18/2019 for DFR Endorsements Project Task 40278 MLW
    Protected Sub btnViewGotoNextSection_Click(sender As Object, e As EventArgs) Handles btnViewGotoNextSection.Click
        Me.Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub

    Private Sub RateFromOptional() Handles ctlDFROptionalCoverages.RateQuote
        Session("vendorValueDFR") = "False"
        RaiseEvent QuoteRateRequested()
    End Sub

    Protected Sub btnReplacementCC_Click(sender As Object, e As EventArgs) Handles btnReplacementCC.Click
        Dim errMsg As String = ""
        Dim wasSaveSuccessful As Boolean = False
        Dim valuationUrl As String = ""

        Dim chc As New CommonHelperClass

        If chc.ConfigurationAppSettingValueAsBoolean("VR_AllLines_Site360Valuation_Settings") = True Then
            pvHelper.PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(Quote, valuationUrl, 1, True, wasSaveSuccessful, errMsg, valuationVendor:=QuickQuotePropertyValuation.ValuationVendor.Verisk360)

        Else
            pvHelper.PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(Quote, valuationUrl, 1, True, wasSaveSuccessful, errMsg, valuationVendor:=QuickQuotePropertyValuation.ValuationVendor.e2Value)
        End If

        If valuationUrl <> "" Then

            If Not wasSaveSuccessful Then
                'SaveQuote()
            End If

            Session("vendorValueDFR") = "True"
            ' Need to clear session quote because it is going to change while on e2value side (basically) so you will want to reload session variable once you get back
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
            Else
                VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId) ' Matt A - 12-9-14
            End If

            Response.Redirect(valuationUrl, False)
        Else
            If errMsg = "" Then 'should have something if it gets here... ELSE will have already redirected to e2Value
                errMsg = "Problem Initiating Valuation Request"
            End If

            ShowError(errMsg)
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
                redirectPage = "MyVelociRater.aspx" 'use config key if available
            End If
            strScript &= " window.location.href='" & redirectPage & "';"
        End If
        strScript &= "</script>"

        Page.RegisterStartupScript("clientScript", strScript)

    End Sub

    Protected Sub lnkSaveDeductibles_Click(sender As Object, e As EventArgs) Handles lnkSaveDeductibles.Click
        Session("vendorValueDFR") = "False"
        SaveQuote()
    End Sub

    Protected Sub lnkSaveBase_Click(sender As Object, e As EventArgs) Handles lnkSaveBase.Click
        Session("vendorValueDFR") = "False"
        SaveQuote()
    End Sub

    Protected Sub lnkSaveOptional_Click(sender As Object, e As EventArgs) Handles lnkSaveOptional.Click
        Session("vendorValueDFR") = "False"
        SaveQuote()
    End Sub

    Protected Sub lnkClearOptional_Click(sender As Object, e As EventArgs) Handles lnkClearOptional.Click
        ClearChildControls()
    End Sub

    Protected Sub lnkClearDeductibles_Click(sender As Object, e As EventArgs) Handles lnkClearDeductibles.Click
        ClearDeductible()
    End Sub

    Protected Sub lnkClearBase_Click(sender As Object, e As EventArgs) Handles lnkClearBase.Click
        ClearBaseCoverages()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.CreateAccordion(MainAccordionDivId, hiddenCoverage, "0")
        VRScript.CreateAccordion(DeductibleDivId, hiddenDeductibles, "0")
        VRScript.CreateAccordion(BaseCoverageDivId, hiddenBase, "0")
        VRScript.CreateAccordion(OptionalCoverageDivId, hiddenOptional, "0")

        Me.VRScript.StopEventPropagation(Me.lnkClearCoverages.ClientID, False)
        Me.VRScript.StopEventPropagation(Me.lnkSaveCoverages.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkClearDeductibles.ClientID, False)
        Me.VRScript.StopEventPropagation(Me.lnkSaveDeductibles.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkClearBase.ClientID, False)
        Me.VRScript.StopEventPropagation(Me.lnkSaveBase.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkClearOptional.ClientID, False)
        Me.VRScript.StopEventPropagation(Me.lnkSaveOptional.ClientID)


        ' Calculate coverage from Dwelling A Limit
        Dim scriptCalcDwellingLimit As String = "CalculateFromDFRLimit(""" + txtDwLimit.ClientID + """, """ + txtDwellingTotalLimit.ClientID +
            """, """ + txtOtherLimit.ClientID + """, """ + txtOtherChgInLimit.ClientID + """, """ + txtOtherTotalLimit.ClientID +
            """, """ + txtPPLimit.ClientID + """, """ + txtPPChgInLimit.ClientID + """, """ + txtPPTotalLimit.ClientID +
            """, """ + txtLivingLimit.ClientID + """, """ + txtLivingChgInLimit.ClientID + """, """ + txtLivingTotalLimit.ClientID +
            """, """ + CurrentForm.ToString() + """, """ + hiddenCovALimit.ClientID + """, """ + hiddenCovATotal.ClientID +
            """, """ + hiddenCovBLimit.ClientID + """, """ + hiddenCovBChange.ClientID + """, """ + hiddenCovBTotal.ClientID +
            """, """ + hiddenCovCLimit.ClientID + """, """ + hiddenCovCChange.ClientID + """, """ + hiddenCovCTotal.ClientID +
            """, """ + hiddenCovDLimit.ClientID + """, """ + hiddenCovDChange.ClientID +
            """, """ + hiddenCovDTotal.ClientID + """);"
        txtDwLimit.Attributes.Add("onblur", scriptCalcDwellingLimit)
        txtOtherChgInLimit.Attributes.Add("onblur", scriptCalcDwellingLimit)
        txtPPChgInLimit.Attributes.Add("onblur", scriptCalcDwellingLimit)
        txtLivingChgInLimit.Attributes.Add("onblur", scriptCalcDwellingLimit)

        txtDwLimit.Attributes.Add("onfocus", "this.select()")
        txtDwellingChangeInLimit.Attributes.Add("onfocus", "this.select()")
        txtOtherChgInLimit.Attributes.Add("onfocus", "this.select()")
        txtPPLimit.Attributes.Add("onfocus", "this.select()")
        txtPPChgInLimit.Attributes.Add("onfocus", "this.select()")
        txtLivingChgInLimit.Attributes.Add("onfocus", "this.select()")
    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.MyLocation IsNot Nothing Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddlDeductible, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddlPersonalLiability, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PersonalLiabilityLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddlMedicalPayments, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)

            RemoveDeductibleLimitOptionsLessThan1k()

            SetDefaultValues()
        End If
    End Sub

    Public Overrides Sub Populate()
        If Me.MyLocation IsNot Nothing Then
            Select Case MyLocation.FormTypeId
                Case "8",
                    "9",
                    "10"
                    If MyLocation.UsageTypeId <> "1" Then
                        If MyLocation.NumberOfFamiliesId <> "3" AndAlso MyLocation.NumberOfFamiliesId <> "4" Then
                            txtDwLimit.ToolTip = "Coverage A minimum limit is $25,000 when Number of Families is 1 or 2 AND the Usage Type is Non-Seasonal"
                        End If

                        If MyLocation.NumberOfFamiliesId <> "1" AndAlso MyLocation.NumberOfFamiliesId <> "2" Then
                            txtDwLimit.ToolTip = "Coverage A minimum limit is $50,000 when Number of Families is 3 or 4 AND the Usage Type is Non-Seasonal"
                        End If
                    Else
                        txtDwLimit.ToolTip = "Coverage A minimum limit is $25,000 when the Usage Type is Seasonal"
                    End If
                Case "11",
                    "12"
                    If MyLocation.UsageTypeId <> "1" Then
                        If MyLocation.NumberOfFamiliesId <> "3" AndAlso MyLocation.NumberOfFamiliesId <> "4" Then
                            txtDwLimit.ToolTip = "Coverage A minimum limit is $40,000 when Number of Families is 1 or 2 AND the Usage Type is Non-Seasonal"
                        End If

                        If MyLocation.NumberOfFamiliesId <> "1" AndAlso MyLocation.NumberOfFamiliesId <> "2" Then
                            txtDwLimit.ToolTip = "Coverage A minimum limit is $80,000 when Number of Families is 3 or 4 AND the Usage Type is Non-Seasonal"
                        End If
                    Else
                        txtDwLimit.ToolTip = "Coverage A minimum limit is $40,000 when the Usage Type is Seasonal"
                    End If
            End Select

            If Quote.Locations Is Nothing OrElse Quote.Locations.Count <= 0 OrElse MyLocation Is Nothing Then Exit Sub

            PopulateDeductibleLimit()
            ''Updated 02/17/2020 for DFR Endorsements MLW
            ''If MyLocation.DeductibleLimitId <> "" Then
            ''    ddlDeductible.SelectedValue = MyLocation.DeductibleLimitId
            ''End If
            ''Updated 9/4/18 for multi state MLW - reverted. No changes needed for this on multi state
            'If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.DeductibleLimitId) Then
            '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlDeductible, MyLocation.DeductibleLimitId)
            'End If

            '
            ' COVERAGE A
            '
            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.A_Dwelling_LimitIncreased) Then
                txtDwLimit.Text = MyLocation.A_Dwelling_LimitIncreased
                hiddenCovALimit.Value = MyLocation.A_Dwelling_LimitIncreased
            Else
                txtDwLimit.Text = "0"
            End If

            If QQHelper.IsZeroPremium(lblReplacementCCValue.Text) Then
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.A_Dwelling_LimitIncluded) Then
                    txtDwellingChangeInLimit.Text = MyLocation.A_Dwelling_LimitIncluded
                    hiddenCovAChange.Value = MyLocation.A_Dwelling_LimitIncluded
                Else
                    txtDwellingChangeInLimit.Text = "0"
                End If
            Else
                txtDwellingChangeInLimit.Text = hiddenCovAChange.Value
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.A_Dwelling_Limit) Then
                txtDwellingTotalLimit.Text = MyLocation.A_Dwelling_Limit
                hiddenCovATotal.Value = MyLocation.A_Dwelling_Limit
            Else
                txtDwellingTotalLimit.Text = "0"
            End If

            '
            ' COVERAGE B
            '
            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.B_OtherStructures_LimitIncluded) Then
                txtOtherLimit.Text = MyLocation.B_OtherStructures_LimitIncluded
                hiddenCovBLimit.Value = MyLocation.B_OtherStructures_LimitIncluded
            Else
                txtOtherLimit.Text = "0"
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.B_OtherStructures_LimitIncreased) Then
                txtOtherChgInLimit.Text = MyLocation.B_OtherStructures_LimitIncreased
                hiddenCovBChange.Value = MyLocation.B_OtherStructures_LimitIncreased
            Else
                txtOtherChgInLimit.Text = "0"
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.B_OtherStructures_Limit) Then
                txtOtherTotalLimit.Text = MyLocation.B_OtherStructures_Limit
                hiddenCovBTotal.Value = MyLocation.B_OtherStructures_Limit
            Else
                txtOtherTotalLimit.Text = "0"
            End If

            '
            ' COVERAGE C
            '
            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.C_PersonalProperty_LimitIncluded) Then
                txtPPLimit.Text = MyLocation.C_PersonalProperty_LimitIncluded
                hiddenCovCLimit.Value = MyLocation.C_PersonalProperty_LimitIncluded
            Else
                txtPPLimit.Text = "0"
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.C_PersonalProperty_LimitIncreased) Then
                txtPPChgInLimit.Text = MyLocation.C_PersonalProperty_LimitIncreased
                hiddenCovCChange.Value = MyLocation.C_PersonalProperty_LimitIncreased
            Else
                txtPPChgInLimit.Text = "0"
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.C_PersonalProperty_Limit) Then
                txtPPTotalLimit.Text = MyLocation.C_PersonalProperty_Limit
                hiddenCovCTotal.Value = MyLocation.C_PersonalProperty_Limit
            Else
                txtPPTotalLimit.Text = "0"
            End If

            '
            ' COVERAGE D
            '
            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.D_LossOfUse_LimitIncluded) Then
                txtLivingLimit.Text = MyLocation.D_LossOfUse_LimitIncluded
                hiddenCovDLimit.Value = MyLocation.D_LossOfUse_LimitIncluded
            Else
                txtLivingLimit.Text = "0"
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.D_LossOfUse_LimitIncreased) Then
                txtLivingChgInLimit.Text = MyLocation.D_LossOfUse_LimitIncreased
                hiddenCovDChange.Value = MyLocation.D_LossOfUse_LimitIncreased
            Else
                txtLivingChgInLimit.Text = "0"
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.D_LossOfUse_Limit) Then
                txtLivingTotalLimit.Text = MyLocation.D_LossOfUse_Limit
                hiddenCovDTotal.Value = MyLocation.D_LossOfUse_Limit
            Else
                txtLivingTotalLimit.Text = "0"
            End If

            'Updated 9/4/18 for multi state MLW - reverted. No changes needed for this on multi state 
            ' If SubQuoteFirst IsNot Nothing Then
            '
            ' Personal Liability
            '
            'Updated 9/4/18 for multi state MLW - reverted. No changes needed for this on multi state
            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.PersonalLiabilityLimitId) Then
                'ddlPersonalLiability.SelectedValue = Quote.PersonalLiabilityLimitId
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlPersonalLiability, Quote.PersonalLiabilityLimitId)
            Else

            End If
            If Not IsQuoteEndorsement() Then
                PersonalLiabilitylimitTextDFR.Visible = True
            End If
            'If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(SubQuoteFirst.PersonalLiabilityLimitId) Then
            '        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlPersonalLiability, SubQuoteFirst.PersonalLiabilityLimitId)
            '    End If

            '
            ' Medical Payments
            '
            'Updated 9/4/18 for multi state MLW - reverted. No changes needed for this on multi state
            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.MedicalPaymentsLimitid) Then
                'ddlMedicalPayments.SelectedValue = Quote.MedicalPaymentsLimitid
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlMedicalPayments, Quote.MedicalPaymentsLimitid)
            Else

            End If
            'If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(SubQuoteFirst.MedicalPaymentsLimitid) Then
            '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlMedicalPayments, SubQuoteFirst.MedicalPaymentsLimitid)
            'End If
            'End If

            'E2Value
            If MyLocation.PropertyValuation IsNot Nothing AndAlso MyLocation.PropertyValuation.db_propertyValuationId <> "" _
                AndAlso MyLocation.PropertyValuation.VendorValuationId <> "" AndAlso MyLocation.RebuildCost <> "" Then

                ' Set E2Value
                lblCalValue.Visible = True
                lblReplacementCCValue.Visible = True
                lblReplacementCCValue.Text = MyLocation.RebuildCost.Substring(1).Split(".")(0)

                ' Set Coverage A
                If Session("vendorValueDFR") = "True" Then
                    txtDwLimit.Text = lblReplacementCCValue.Text
                End If

                txtDwellingTotalLimit.Text = (Integer.Parse(txtDwLimit.Text.Replace(",", "")) + Integer.Parse(txtDwellingChangeInLimit.Text.Replace(",", ""))).ToString("N0")
                hiddenCovALimit.Value = txtDwLimit.Text
                hiddenCovAChange.Value = txtDwellingChangeInLimit.Text
                hiddenCovATotal.Value = txtDwellingTotalLimit.Text

                ' Set Coverage B
                txtOtherLimit.Text = (Integer.Parse(txtDwellingTotalLimit.Text.Replace(",", "")) * 0.1).ToString("N0")
                txtOtherTotalLimit.Text = (Integer.Parse(txtOtherLimit.Text.Replace(",", "")) + Integer.Parse(txtOtherChgInLimit.Text.Replace(",", ""))).ToString("N0")
                hiddenCovBLimit.Value = txtOtherLimit.Text
                hiddenCovBChange.Value = txtOtherChgInLimit.Text
                hiddenCovBTotal.Value = txtOtherTotalLimit.Text

                ' Set Coverage C
                txtPPLimit.Text = "0"
                txtPPTotalLimit.Text = (Integer.Parse(txtPPLimit.Text.Replace(",", "")) + Integer.Parse(txtPPChgInLimit.Text.Replace(",", ""))).ToString("N0")
                hiddenCovCLimit.Value = txtPPLimit.Text
                hiddenCovCChange.Value = txtPPChgInLimit.Text
                hiddenCovCTotal.Value = txtPPTotalLimit.Text

                ' Set Coverage D
                txtLivingLimit.Text = (Integer.Parse(txtDwellingTotalLimit.Text.Replace(",", "")) * 0.1).ToString("N0")
                txtLivingTotalLimit.Text = (Integer.Parse(txtLivingLimit.Text.Replace(",", "")) + Integer.Parse(txtLivingChgInLimit.Text.Replace(",", ""))).ToString("N0")
                hiddenCovDLimit.Value = txtLivingLimit.Text
                hiddenCovDChange.Value = txtLivingChgInLimit.Text
                hiddenCovDTotal.Value = txtLivingTotalLimit.Text

                ' Shows date
                If IsDate(MyLocation.LastCostEstimatorDate) Then

                End If
            Else
                lblCalValue.Visible = False
                lblReplacementCCValue.Visible = False
                lblReplacementCCValue.Text = ""
            End If

            pnlReplacementCC.Visible = True 'Default it to visible true

            'StructureTypeId = "14" Mobile Manufacturer, Set Visible false when 360Valuation flag is enabled
            Dim chc As New CommonHelperClass
            If chc.ConfigurationAppSettingValueAsBoolean("VR_AllLines_Site360Valuation_Settings") = True AndAlso MyLocation.StructureTypeId = "14" Then
                pnlReplacementCC.Visible = False
            End If

            'Added 9/18/2019 for DFR Endorsements Project Task 40278 MLW
            If Me.IsQuoteReadOnly Then
                Dim policyNumber As String = Me.Quote.PolicyNumber
                Dim imageNum As Integer = 0
                Dim policyId As Integer = 0
                Dim toolTip As String = "Make a change to this policy"
                'Dim qqHelper As New QuickQuoteHelperClass
                Dim readOnlyViewPageUrl As String = QQHelper.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                    readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                End If

                pnlReplacementCC.Visible = False
                btnSaveBase.Visible = False
                btnRateBase.Visible = False
                btnMakeAChange.Visible = True
                btnViewGotoNextSection.Visible = True

                btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                btnMakeAChange.ToolTip = toolTip
                btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
            Else
                btnMakeAChange.Visible = False
                btnViewGotoNextSection.Visible = False
            End If

            PopulateChildControls()

            lblOptionalChosen.Text = ctlDFROptionalCoverages.SelectedOptCoverageCnt
        End If
    End Sub

    Public Sub PopulateDeductibleLimit()
        If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.DeductibleLimitId) Then
            If IsEndorsementRelated() Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddlDeductible, MyLocation.DeductibleLimitId, QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, MyLocation.DeductibleLimitId))
            Else
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlDeductible, MyLocation.DeductibleLimitId)
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If MyLocation IsNot Nothing Then
            Dim err As String = Nothing
            Dim WhereAmI As String = "G"

            Try
                Dim origDeductibleLimit As String = MyLocation.DeductibleLimitId
                MyLocation.DeductibleLimitId = ddlDeductible.SelectedValue

                '
                '
                ' Cov A
                '
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenCovALimit.Value.ToString) Then
                    MyLocation.A_Dwelling_LimitIncreased = hiddenCovALimit.Value.ToString
                Else
                    MyLocation.A_Dwelling_LimitIncreased = ""
                End If

                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenCovAChange.Value.ToString) Then
                    MyLocation.A_Dwelling_LimitIncluded = hiddenCovAChange.Value.ToString
                Else
                    MyLocation.A_Dwelling_LimitIncluded = ""
                End If

                MyLocation.A_Dwelling_Limit = hiddenCovATotal.Value.ToString

                '
                ' Cov B
                '
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenCovBLimit.Value.ToString) Then
                    MyLocation.B_OtherStructures_LimitIncluded = hiddenCovBLimit.Value.ToString
                Else
                    MyLocation.B_OtherStructures_LimitIncluded = ""
                End If

                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenCovBChange.Value.ToString) Then
                    MyLocation.B_OtherStructures_LimitIncreased = hiddenCovBChange.Value.ToString
                Else
                    MyLocation.B_OtherStructures_LimitIncreased = ""
                End If

                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenCovBTotal.Value.ToString) Then
                    MyLocation.B_OtherStructures_Limit = hiddenCovBTotal.Value.ToString
                Else
                    MyLocation.B_OtherStructures_Limit = ""
                End If

                '
                ' Cov C
                '
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenCovCLimit.Value.ToString) Then
                    MyLocation.C_PersonalProperty_LimitIncluded = hiddenCovCLimit.Value.ToString
                Else
                    MyLocation.C_PersonalProperty_LimitIncluded = ""
                End If

                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenCovCChange.Value.ToString) Then
                    MyLocation.C_PersonalProperty_LimitIncreased = hiddenCovCChange.Value.ToString
                Else
                    MyLocation.C_PersonalProperty_LimitIncreased = ""
                End If

                MyLocation.C_PersonalProperty_Limit = hiddenCovCTotal.Value.ToString

                '
                ' Cov D
                '
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenCovDLimit.Value.ToString) Then
                    MyLocation.D_LossOfUse_LimitIncluded = hiddenCovDLimit.Value.ToString
                Else
                    MyLocation.D_LossOfUse_LimitIncluded = ""
                End If

                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenCovDChange.Value.ToString) Then
                    MyLocation.D_LossOfUse_LimitIncreased = hiddenCovDChange.Value.ToString
                Else
                    MyLocation.D_LossOfUse_LimitIncreased = ""
                End If

                MyLocation.D_LossOfUse_Limit = hiddenCovDTotal.Value.ToString

                'Updated 9/4/18 for multi state MLW - reverted. No changes needed for this on multi state 
                '
                ' Personal Liability
                '
                If ddlPersonalLiability.SelectedValue <> "" Then
                    Quote.PersonalLiabilityLimitId = ddlPersonalLiability.SelectedValue
                Else
                    Quote.PersonalLiabilityLimitId = ""
                End If

                '
                ' Medical Payments
                '
                If ddlMedicalPayments.SelectedValue <> "" Then
                    Quote.MedicalPaymentsLimitid = ddlMedicalPayments.SelectedValue
                Else
                    Quote.MedicalPaymentsLimitid = ""
                End If
                'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                '    For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                '        '
                '        ' Personal Liability
                '        '
                '        If ddlPersonalLiability.SelectedValue <> "" Then
                '            sq.PersonalLiabilityLimitId = ddlPersonalLiability.SelectedValue
                '        Else
                '            sq.PersonalLiabilityLimitId = ""
                '        End If

                '        '
                '        ' Medical Payments
                '        '
                '        If ddlMedicalPayments.SelectedValue <> "" Then
                '            sq.MedicalPaymentsLimitid = ddlMedicalPayments.SelectedValue
                '        Else
                '            sq.MedicalPaymentsLimitid = ""
                '        End If
                '    Next
                'End If

                If Quote.PersonalLiabilityLimitId.TryToGetInt32() > 0 AndAlso Me.MyLocation.OccupancyCodeId.NotEqualsAny("14") Then ' if has Personal Liability and is not Owner Occupied

                    If Me.MyLocation.SectionIICoverages Is Nothing Then
                        Me.MyLocation.SectionIICoverages = New List(Of QuickQuoteSectionIICoverage)()
                    End If
                    Dim existingCov = Me.MyLocation.SectionIICoverages.Find(Function(i)
                                                                                Return i.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Non_OwnerOccupiedDwelling
                                                                            End Function)
                    If existingCov Is Nothing Then
                        existingCov = New QuickQuoteSectionIICoverage()
                        existingCov.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Non_OwnerOccupiedDwelling
                        Me.MyLocation.SectionIICoverages.Add(existingCov)
                    End If

                    Me.QQHelper.CopyQuickQuoteAddress(MyLocation.Address, existingCov.Address)

                Else
                    'This coverage is only acceptable when the above two conditions apply. It needs removed otherwise.
                    Me.MyLocation?.SectionIICoverages?.RemoveAll(Function(i)
                                                                     Return i.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Non_OwnerOccupiedDwelling
                                                                 End Function)
                End If


                SaveChildControls()

                If PolicyDeductibleNotLessThan1k.IsPolicyDeductibleNotLessThan1kAvailable(Quote) AndAlso IsQuoteEndorsement() AndAlso origDeductibleLimit <> MyLocation.DeductibleLimitId Then
                    Select Case origDeductibleLimit
                        Case "21", "22"
                            '21 = 250, 22 = 500
                            'Only needed for Endorsement that is forcing a value from Diamond that is not in VR and is present in the drop down so that it removes the value from the drop down when it is changed.
                            RemoveDeductibleLimitOptionsLessThan1k()
                    End Select
                End If

                Return True
            Catch ex As Exception
                Return False
            End Try
        End If
        Return True
    End Function

    Public Sub RemoveDeductibleLimitOptionsLessThan1k()
        If PolicyDeductibleNotLessThan1k.IsPolicyDeductibleNotLessThan1kAvailable(Quote) Then
            'Remove 250
            Dim removeDeductible250 = (From i As ListItem In Me.ddlDeductible.Items Where i.Text = "250" Select i).FirstOrDefault()
            If removeDeductible250 IsNot Nothing Then
                Me.ddlDeductible.Items.Remove(removeDeductible250)
            End If
            'Remove 500
            Dim removeDeductible500 = (From i As ListItem In Me.ddlDeductible.Items Where i.Text = "500" Select i).FirstOrDefault()
            If removeDeductible500 IsNot Nothing Then
                Me.ddlDeductible.Items.Remove(removeDeductible500)
            End If
        End If
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        ClearDeductible()
        ClearBaseCoverages()
        ClearChildControls()
    End Sub

    Private Sub ClearDeductible()
        Session("vendorValueDFR") = "False"
        'Set dropdown lists to default values
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlDeductible, "1,000")
        Save_FireSaveEvent(False)
    End Sub

    Private Sub ClearBaseCoverages()
        Session("vendorValueDFR") = "False"
        'Set dropdown lists to default values
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlPersonalLiability, "100,000")
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlMedicalPayments, "1,000")

        'Zero out E2Value
        lblReplacementCCValue.Text = "0"

        txtDwLimit.Text = "0"
        txtDwellingChangeInLimit.Text = "0"
        txtDwellingTotalLimit.Text = txtDwLimit.Text
        hiddenCovALimit.Value = txtDwLimit.Text
        hiddenCovAChange.Value = "0"
        hiddenCovATotal.Value = hiddenCovALimit.Value

        txtOtherLimit.Text = "0"
        txtOtherChgInLimit.Text = "0"
        txtOtherTotalLimit.Text = "0"
        hiddenCovBLimit.Value = "0"
        hiddenCovBChange.Value = "0"
        hiddenCovBTotal.Value = "0"

        txtPPLimit.Text = "0"
        txtPPChgInLimit.Text = "0"
        txtPPTotalLimit.Text = "0"
        hiddenCovCLimit.Value = "0"
        hiddenCovCChange.Value = "0"
        hiddenCovCTotal.Value = "0"

        txtLivingLimit.Text = "0"
        txtLivingChgInLimit.Text = "0"
        txtLivingTotalLimit.Text = "0"
        hiddenCovDLimit.Value = "0"
        hiddenCovDChange.Value = "0"
        hiddenCovDTotal.Value = "0"
        Save_FireSaveEvent(False)
    End Sub
End Class