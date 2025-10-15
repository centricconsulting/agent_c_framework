Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers

Public Class ctl_CPP_Crime
    Inherits VRControlBase

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddEmployee_Deductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.EmployeeTheftDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddInside_Deductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId, QuickQuoteStaticDataOption.SortBy.ValueAscending, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddOutside_Deductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.OutsideThePremisesDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)

        '''TODO: correct after adding new properties to quick quote
        '''BTW: These methods are doing way too much
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddForgeryAlteration_Deductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ForgeryAlterationDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddComputerFraud_Deductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerFraudDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddFundsTransferFraud_Deductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FundsTransferDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
    End Sub

    Public Sub Handle_DeleteCrime_Request()
        'Me.lnkCrimeCoveragesClear_Click(Me, New EventArgs())
        ClearControl()
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If Quote IsNot Nothing AndAlso GoverningStateQuote() IsNot Nothing Then
            If String.IsNullOrWhiteSpace(GoverningStateQuote.EmployeeTheftLimit) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.EmployeeTheftNumberOfAdditionalPremises) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.EmployeeTheftNumberOfRatableEmployees) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.EmployeeTheftDeductibleId) = False Then
                Me.chkTheft.Checked = True
                Me.txtEmployee_Limit.Text = GoverningStateQuote.EmployeeTheftLimit
                If String.IsNullOrWhiteSpace(GoverningStateQuote.EmployeeTheftDeductibleId) = False Then
                    'Updated 12/01/2021 for CPP Endorsement task 65632 MLW
                    If IsQuoteReadOnly() Then
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddEmployee_Deductible, GoverningStateQuote.EmployeeTheftDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.EmployeeTheftDeductibleId)
                    Else
                        WebHelper_Personal.SetdropDownFromValue(Me.ddEmployee_Deductible, GoverningStateQuote.EmployeeTheftDeductibleId)
                    End If
                    'WebHelper_Personal.SetdropDownFromValue(Me.ddEmployee_Deductible, GoverningStateQuote.EmployeeTheftDeductibleId)
                End If
                txtRateAbleEmployees.Text = GoverningStateQuote.EmployeeTheftNumberOfRatableEmployees

                If String.IsNullOrWhiteSpace(GoverningStateQuote.EmployeeTheftNumberOfAdditionalPremises) Then
                    ' use number of locations minus 1 - 04/04/2018 changed to be like old look. CAH
                    Me.txtEmployee_AdditionalPremises.Text = If(Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 1, (Quote.Locations.Count - 1).ToString(), "0")
                Else
                    Me.txtEmployee_AdditionalPremises.Text = GoverningStateQuote.EmployeeTheftNumberOfAdditionalPremises
                End If

                Me.txtEmployee_ERISA.Text = If(GoverningStateQuote.EmployeeTheftScheduledEmployeeBenefitPlans IsNot Nothing AndAlso GoverningStateQuote.EmployeeTheftScheduledEmployeeBenefitPlans.Any(), GoverningStateQuote.EmployeeTheftScheduledEmployeeBenefitPlans(0), "")
            Else
                Me.chkTheft.Checked = False
                Me.txtEmployee_Limit.Text = ""
                WebHelper_Personal.SetdropDownFromValue(Me.ddEmployee_Deductible, "8")
                txtRateAbleEmployees.Text = ""
                Me.txtEmployee_AdditionalPremises.Text = If(Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0, (Quote.Locations.Count - 1).ToString(), "0")
                Me.txtEmployee_ERISA.Text = ""
            End If

            If String.IsNullOrWhiteSpace(GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesLimit) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesNumberOfPremises) = False Then
                Me.chkInside.Checked = True
                Me.txtInside_Limit.Text = GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesLimit
                If String.IsNullOrWhiteSpace(GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId) = False Then
                    'Updated 12/01/2021 for CPP Endorsement task 65632 MLW
                    If IsQuoteReadOnly() Then
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddInside_Deductible, GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId)
                    Else
                        WebHelper_Personal.SetdropDownFromValue(Me.ddInside_Deductible, GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId)
                    End If
                    'WebHelper_Personal.SetdropDownFromValue(Me.ddInside_Deductible, GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId)
                End If
                If String.IsNullOrWhiteSpace(GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesNumberOfPremises) Then
                    Me.txtInside_PremisesCount.Text = If(Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0, Quote.Locations.Count.ToString(), "")
                Else
                    Me.txtInside_PremisesCount.Text = GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesNumberOfPremises
                End If

            Else
                Me.chkInside.Checked = False
                Me.txtInside_Limit.Text = ""
                WebHelper_Personal.SetdropDownFromValue(Me.ddInside_Deductible, "8")
                Me.txtInside_PremisesCount.Text = If(Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0, Quote.Locations.Count.ToString(), "")
            End If

            If String.IsNullOrWhiteSpace(GoverningStateQuote.OutsideThePremisesLimit) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.OutsideThePremisesDeductibleId) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.OutsideThePremisesNumberOfPremises) = False Then
                Me.chkOutside.Checked = True
                Me.txtOutside_Limit.Text = GoverningStateQuote.OutsideThePremisesLimit

                If String.IsNullOrWhiteSpace(GoverningStateQuote.OutsideThePremisesDeductibleId) = False Then
                    'Updated 12/01/2021 for CPP Endorsement task 65632 MLW
                    If IsQuoteReadOnly() Then
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddOutside_Deductible, GoverningStateQuote.OutsideThePremisesDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.OutsideThePremisesDeductibleId)
                    Else
                        WebHelper_Personal.SetdropDownFromValue(Me.ddOutside_Deductible, GoverningStateQuote.OutsideThePremisesDeductibleId)
                    End If
                    'WebHelper_Personal.SetdropDownFromValue(Me.ddOutside_Deductible, GoverningStateQuote.OutsideThePremisesDeductibleId)
                End If
                If String.IsNullOrWhiteSpace(GoverningStateQuote.OutsideThePremisesNumberOfPremises) Then
                    Me.txtOutside_PremisesCount.Text = If(Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0, Quote.Locations.Count.ToString(), "")
                Else
                    Me.txtOutside_PremisesCount.Text = GoverningStateQuote.OutsideThePremisesNumberOfPremises
                End If

            Else
                Me.chkOutside.Checked = False
                Me.txtOutside_Limit.Text = ""
                WebHelper_Personal.SetdropDownFromValue(Me.ddOutside_Deductible, "8")
                Me.txtOutside_PremisesCount.Text = If(Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0, Quote.Locations.Count.ToString(), "")
            End If

            'added to support new coverages
            If String.IsNullOrWhiteSpace(GoverningStateQuote.ForgeryAlterationLimit) = False Or
                String.IsNullOrWhiteSpace(GoverningStateQuote.ForgeryAlterationAdditionalPremises) = False Or
                String.IsNullOrWhiteSpace(GoverningStateQuote.ForgeryAlterationNumberOfRatableEmployees) = False Or
                String.IsNullOrWhiteSpace(GoverningStateQuote.ForgeryAlterationDeductibleId) = False Then
                Me.chkForgeryAlteration.Checked = True
                Me.txtForgeryAlteration_Limit.Text = GoverningStateQuote.ForgeryAlterationLimit
                If String.IsNullOrWhiteSpace(GoverningStateQuote.ForgeryAlterationDeductibleId) = False Then
                    'Updated 12/01/2021 for CPP Endorsement task 65632 MLW
                    If IsQuoteReadOnly() Then
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddForgeryAlteration_Deductible, GoverningStateQuote.ForgeryAlterationDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ForgeryAlterationDeductibleId)
                    Else
                        WebHelper_Personal.SetdropDownFromValue(Me.ddForgeryAlteration_Deductible, GoverningStateQuote.ForgeryAlterationDeductibleId)
                    End If
                    'WebHelper_Personal.SetdropDownFromValue(Me.ddForgeryAlteration_Deductible, GoverningStateQuote.ForgeryAlterationDeductibleId)
                End If
                txtForgeryAlteration_RatableEmployees.Text = GoverningStateQuote.ForgeryAlterationNumberOfRatableEmployees

                If String.IsNullOrWhiteSpace(GoverningStateQuote.ForgeryAlterationAdditionalPremises) Then
                    ' use number of locations minus 1 - 04/04/2018 changed to be like old look. CAH
                    Me.txtForgeryAlteration_AdditionalPremises.Text = If(Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0, (Quote.Locations.Count - 1).ToString(), "0")
                Else
                    Me.txtForgeryAlteration_AdditionalPremises.Text = GoverningStateQuote.ForgeryAlterationAdditionalPremises
                End If
            Else
                Me.chkForgeryAlteration.Checked = False
                Me.txtForgeryAlteration_Limit.Text = ""
                WebHelper_Personal.SetdropDownFromValue(Me.ddForgeryAlteration_Deductible, "8")
                txtForgeryAlteration_RatableEmployees.Text = ""
                Me.txtForgeryAlteration_AdditionalPremises.Text = If(Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0, (Quote.Locations.Count - 1).ToString(), "0")
            End If

            If String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerFraudLimit) = False OrElse
                String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerFraudAdditionalPremises) = False OrElse
                String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerFraudNumberOfRatableEmployees) = False OrElse
                String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerFraudDeductibleId) = False Then
                Me.chkComputerFraud.Checked = True
                Me.txtComputerFraud_Limit.Text = GoverningStateQuote.ComputerFraudLimit
                If String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerFraudDeductibleId) = False Then
                    'Updated 12/01/2021 for CPP Endorsement task 65632 MLW
                    If IsQuoteReadOnly() Then
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddComputerFraud_Deductible, GoverningStateQuote.ComputerFraudDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerFraudDeductibleId)
                    Else
                        WebHelper_Personal.SetdropDownFromValue(Me.ddComputerFraud_Deductible, GoverningStateQuote.ComputerFraudDeductibleId)
                    End If
                    'WebHelper_Personal.SetdropDownFromValue(Me.ddComputerFraud_Deductible, GoverningStateQuote.ComputerFraudDeductibleId)
                End If
                txtComputerFraud_RatableEmployees.Text = GoverningStateQuote.ComputerFraudNumberOfRatableEmployees

                If String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerFraudAdditionalPremises) Then
                    ' use number of locations minus 1 - 04/04/2018 changed to be like old look. CAH
                    Me.txtComputerFraud_AdditionalPremises.Text = If(Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 1, (Quote.Locations.Count - 1).ToString(), "0")
                Else
                    Me.txtComputerFraud_AdditionalPremises.Text = GoverningStateQuote.ComputerFraudAdditionalPremises
                End If
            Else
                Me.chkComputerFraud.Checked = False
                Me.txtComputerFraud_Limit.Text = ""
                WebHelper_Personal.SetdropDownFromValue(Me.ddComputerFraud_Deductible, "8")
                txtComputerFraud_RatableEmployees.Text = ""
                Me.txtComputerFraud_AdditionalPremises.Text = If(Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0, (Quote.Locations.Count - 1).ToString(), "0")
            End If

            If String.IsNullOrWhiteSpace(GoverningStateQuote.FundsTransferFraudLimit) = False OrElse
                String.IsNullOrWhiteSpace(GoverningStateQuote.FundsTransferFraudAdditionalPremises) = False OrElse
                String.IsNullOrWhiteSpace(GoverningStateQuote.FundsTransferFraudNumberOfRatableEmployees) = False OrElse
                String.IsNullOrWhiteSpace(GoverningStateQuote.FundsTransferFraudDeductibleId) = False Then
                Me.chkFundsTransferFraud.Checked = True
                Me.txtFundsTransferFraud_Limit.Text = GoverningStateQuote.FundsTransferFraudLimit
                If String.IsNullOrWhiteSpace(GoverningStateQuote.FundsTransferFraudDeductibleId) = False Then
                    'Updated 12/01/2021 for CPP Endorsement task 65632 MLW
                    If IsQuoteReadOnly() Then
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddFundsTransferFraud_Deductible, GoverningStateQuote.FundsTransferFraudDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FundsTransferDeductibleId)
                    Else
                        WebHelper_Personal.SetdropDownFromValue(Me.ddFundsTransferFraud_Deductible, GoverningStateQuote.FundsTransferFraudDeductibleId)
                    End If
                    'WebHelper_Personal.SetdropDownFromValue(Me.ddFundsTransferFraud_Deductible, GoverningStateQuote.FundsTransferFraudDeductibleId)
                End If
                txtFundsTransferFraud_RatableEmployees.Text = GoverningStateQuote.FundsTransferFraudNumberOfRatableEmployees

                If String.IsNullOrWhiteSpace(GoverningStateQuote.FundsTransferFraudAdditionalPremises) Then
                    ' use number of locations minus 1 - 04/04/2018 changed to be like old look. CAH
                    Me.txtFundsTransferFraud_AdditionalPremises.Text = If(Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 1, (Quote.Locations.Count - 1).ToString(), "0")
                Else
                    Me.txtFundsTransferFraud_AdditionalPremises.Text = GoverningStateQuote.FundsTransferFraudAdditionalPremises
                End If
            Else
                Me.chkFundsTransferFraud.Checked = False
                Me.txtFundsTransferFraud_Limit.Text = ""
                WebHelper_Personal.SetdropDownFromValue(Me.ddFundsTransferFraud_Deductible, "8")
                txtFundsTransferFraud_RatableEmployees.Text = ""
                Me.txtFundsTransferFraud_AdditionalPremises.Text = If(Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0, (Quote.Locations.Count - 1).ToString(), "0")
            End If

            'Updated 12/17/18 for multi state bug 30351 MLW
            If GoverningStateQuote.ClassificationCodes IsNot Nothing AndAlso GoverningStateQuote.ClassificationCodes.Any() Then
                Me.txtClassCode.Text = GoverningStateQuote.ClassificationCodes(0).ClassCode
                Me.txtDescription.Text = GoverningStateQuote.ClassificationCodes(0).ClassDescription
                hdnDIA_Id.Value = GoverningStateQuote.ClassificationCodes(0).ClassificationCodeNum
                hdnPMAID.Value = GoverningStateQuote.ClassificationCodes(0).PMA
                hdnGroupRate.Value = GoverningStateQuote.ClassificationCodes(0).RateGroup
                hdnClassLimit.Value = GoverningStateQuote.ClassificationCodes(0).ClassLimit
            End If
            'If Quote.ClassificationCodes IsNot Nothing AndAlso Quote.ClassificationCodes.Any() Then
            '    Me.txtClassCode.Text = Quote.ClassificationCodes(0).ClassCode
            '    Me.txtDescription.Text = Quote.ClassificationCodes(0).ClassDescription
            '    hdnDIA_Id.Value = Quote.ClassificationCodes(0).ClassificationCodeNum
            '    hdnPMAID.Value = Quote.ClassificationCodes(0).PMA
            '    hdnGroupRate.Value = Quote.ClassificationCodes(0).RateGroup
            '    hdnClassLimit.Value = Quote.ClassificationCodes(0).ClassLimit
            'End If

            Me.ctl_CPP_CrimeClassCodeLookup.ParentClassCodeTextboxId = Me.txtClassCode.ClientID
            Me.ctl_CPP_CrimeClassCodeLookup.ParentDescriptionTextboxId = Me.txtDescription.ClientID
            Me.ctl_CPP_CrimeClassCodeLookup.ParentIDHdnId = Me.hdnDIA_Id.ClientID
            Me.ctl_CPP_CrimeClassCodeLookup.ParentPMAIDHdnId = Me.hdnPMAID.ClientID
            Me.ctl_CPP_CrimeClassCodeLookup.ParentGroupRateHdnId = Me.hdnGroupRate.ClientID
            Me.ctl_CPP_CrimeClassCodeLookup.ParentClassLimitHdnId = Me.hdnClassLimit.ClientID

            'Added 11/23/2021 for CPP Endorsements task 65599 MLW
            If IsQuoteReadOnly() Then
                divActionButtons.Visible = False
                divEndorsementButtons.Visible = True

                Dim policyNumber As String = Me.Quote.PolicyNumber
                Dim imageNum As Integer = 0
                Dim policyId As Integer = 0
                Dim toolTip As String = "Make a change to this policy"
                Dim readOnlyViewPageUrl As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                    readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                End If

                btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                btnMakeAChange.ToolTip = toolTip
                btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
            Else
                divActionButtons.Visible = True
                divEndorsementButtons.Visible = False
            End If
        End If
        Me.PopulateChildControls()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID, False)
        Me.VRScript.StopEventPropagation(Me.btnCrimeCoveragesSave.ClientID, False)
        Me.VRScript.StopEventPropagation(Me.btnClassCodeSave.ClientID, False)

        Me.VRScript.StopEventPropagation(Me.btnClassCodeClear.ClientID, False)
        Me.VRScript.StopEventPropagation(Me.btnCrimeCoveragesClear.ClientID, False)

        'Me.VRScript.StopEventPropagation(Me.lnkClear.ClientID, False)
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        _script.AddScriptLine("$(""#CrimeDiv"").accordion({collapsible: false, heightStyle: ""content""});")
        Me.VRScript.CreateAccordion(divMainClassCode.ClientID, Nothing, Nothing, True)
        Me.VRScript.CreateAccordion(divMainCrimeCoverages.ClientID, Nothing, Nothing, True)
        Me.VRScript.CreateAccordion(calcEmployee.ClientID, Nothing, Nothing, True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            'Me.ValidateForm()
            If Me.chkTheft.Checked Then
                GoverningStateQuote.EmployeeTheftLimit = Me.txtEmployee_Limit.Text
                GoverningStateQuote.EmployeeTheftDeductibleId = Me.ddEmployee_Deductible.SelectedValue
                GoverningStateQuote.EmployeeTheftNumberOfAdditionalPremises = Me.txtEmployee_AdditionalPremises.Text
                GoverningStateQuote.EmployeeTheftScheduledEmployeeBenefitPlans = New List(Of String) 'always clear first
                GoverningStateQuote.EmployeeTheftNumberOfRatableEmployees = txtRateAbleEmployees.Text
                If String.IsNullOrWhiteSpace(Me.txtEmployee_ERISA.Text) = False Then
                    GoverningStateQuote.EmployeeTheftScheduledEmployeeBenefitPlans.Add(Me.txtEmployee_ERISA.Text)
                End If
            Else
                GoverningStateQuote.EmployeeTheftLimit = ""
                GoverningStateQuote.EmployeeTheftDeductibleId = ""
                GoverningStateQuote.EmployeeTheftNumberOfRatableEmployees = ""
                txtRateAbleEmployees.Text = ""
                GoverningStateQuote.EmployeeTheftNumberOfAdditionalPremises = ""
                GoverningStateQuote.EmployeeTheftScheduledEmployeeBenefitPlans = Nothing
            End If

            If Me.chkInside.Checked Then
                GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesLimit = Me.txtInside_Limit.Text
                GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId = Me.ddInside_Deductible.SelectedValue
                GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesNumberOfPremises = Me.txtInside_PremisesCount.Text
            Else
                GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesLimit = ""
                GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId = ""
                GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesNumberOfPremises = ""
            End If

            If Me.chkOutside.Checked Then
                GoverningStateQuote.OutsideThePremisesLimit = Me.txtOutside_Limit.Text
                GoverningStateQuote.OutsideThePremisesDeductibleId = Me.ddOutside_Deductible.SelectedValue
                GoverningStateQuote.OutsideThePremisesNumberOfPremises = Me.txtOutside_PremisesCount.Text
            Else
                'Updated 12/17/18 for multi state bug 30351 MLW
                GoverningStateQuote.OutsideThePremisesLimit = ""
                GoverningStateQuote.OutsideThePremisesDeductibleId = ""
                GoverningStateQuote.OutsideThePremisesNumberOfPremises = ""
            End If

            'added to support new coverages
            If chkForgeryAlteration.Checked Then
                GoverningStateQuote.ForgeryAlterationLimit = Me.txtForgeryAlteration_Limit.Text
                GoverningStateQuote.ForgeryAlterationDeductibleId = Me.ddForgeryAlteration_Deductible.SelectedValue
                GoverningStateQuote.ForgeryAlterationAdditionalPremises = Me.txtForgeryAlteration_AdditionalPremises.Text
                GoverningStateQuote.ForgeryAlterationNumberOfRatableEmployees = txtForgeryAlteration_RatableEmployees.Text
            Else
                GoverningStateQuote.ForgeryAlterationLimit = ""
                GoverningStateQuote.ForgeryAlterationDeductibleId = ""
                GoverningStateQuote.ForgeryAlterationNumberOfRatableEmployees = ""
                GoverningStateQuote.ForgeryAlterationAdditionalPremises = ""
                txtForgeryAlteration_RatableEmployees.Text = ""

            End If

            If chkComputerFraud.Checked Then
                GoverningStateQuote.ComputerFraudLimit = Me.txtComputerFraud_Limit.Text
                GoverningStateQuote.ComputerFraudDeductibleId = Me.ddComputerFraud_Deductible.SelectedValue
                GoverningStateQuote.ComputerFraudAdditionalPremises = Me.txtComputerFraud_AdditionalPremises.Text
                GoverningStateQuote.ComputerFraudNumberOfRatableEmployees = txtComputerFraud_RatableEmployees.Text
            Else
                GoverningStateQuote.ComputerFraudLimit = ""
                GoverningStateQuote.ComputerFraudDeductibleId = ""
                GoverningStateQuote.ComputerFraudNumberOfRatableEmployees = ""
                GoverningStateQuote.ComputerFraudAdditionalPremises = ""
                txtComputerFraud_RatableEmployees.Text = ""
            End If

            If chkFundsTransferFraud.Checked Then
                GoverningStateQuote.FundsTransferFraudLimit = Me.txtFundsTransferFraud_Limit.Text
                GoverningStateQuote.FundsTransferFraudDeductibleId = Me.ddFundsTransferFraud_Deductible.SelectedValue
                GoverningStateQuote.FundsTransferFraudAdditionalPremises = Me.txtFundsTransferFraud_AdditionalPremises.Text
                GoverningStateQuote.FundsTransferFraudNumberOfRatableEmployees = txtFundsTransferFraud_RatableEmployees.Text
            Else
                GoverningStateQuote.FundsTransferFraudLimit = ""
                GoverningStateQuote.FundsTransferFraudDeductibleId = ""
                GoverningStateQuote.FundsTransferFraudNumberOfRatableEmployees = ""
                GoverningStateQuote.FundsTransferFraudAdditionalPremises = ""
                txtFundsTransferFraud_RatableEmployees.Text = ""
            End If
            'Updated 12/17/18 for multi state bug 30351 MLW
            'Quote.ClassificationCodes = New List(Of QuickQuoteClassificationCode)
            GoverningStateQuote.ClassificationCodes = New List(Of QuickQuoteClassificationCode)

            'Updated 12/17/18 for multi state bug 30351 MLW
            'If Quote.ClassificationCodes IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Me.txtClassCode.Text) = False Then
            If GoverningStateQuote.ClassificationCodes IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Me.txtClassCode.Text) = False Then
                Dim classCodeItem As QuickQuoteClassificationCode = New QuickQuoteClassificationCode()
                classCodeItem.ClassCode = Me.txtClassCode.Text
                classCodeItem.ClassDescription = Me.txtDescription.Text
                classCodeItem.PMA = hdnPMAID.Value
                classCodeItem.ClassLimit = hdnClassLimit.Value
                classCodeItem.RateGroup = hdnGroupRate.Value
                classCodeItem.ClassificationCodeNum = Me.hdnDIA_Id.Value
                'Updated 12/17/18 for multi state bug 30351 MLW
                'Quote.ClassificationCodes.Add(classCodeItem)
                GoverningStateQuote.ClassificationCodes.Add(classCodeItem)

            End If

            'Updated 12/17/18 for multi state bug 30351 MLW
            'If Quote.ClassificationCodes.Any() AndAlso Quote.ClassificationCodes(0).ClassCode IsNot Nothing AndAlso (Me.chkTheft.Checked OrElse Me.chkInside.Checked OrElse Me.chkOutside.Checked) Then
            If GoverningStateQuote.ClassificationCodes.Any() AndAlso
                GoverningStateQuote.ClassificationCodes(0).ClassCode IsNot Nothing AndAlso
                (Me.chkTheft.Checked OrElse
                 Me.chkInside.Checked OrElse
                 Me.chkOutside.Checked OrElse
                 Me.chkForgeryAlteration.Checked OrElse
                 Me.chkComputerFraud.Checked OrElse
                 Me.chkFundsTransferFraud.Checked) Then
                GoverningStateQuote.CPP_Has_Crime_PackagePart = True
            Else
                GoverningStateQuote.CPP_Has_Crime_PackagePart = False
                'Me.txtClassCode.Text = Nothing
                'Me.txtDescription.Text = Nothing
                'hdnDIA_Id.Value = Nothing
                'hdnPMAID.Value = Nothing
                'hdnGroupRate.Value = Nothing
                'hdnClassLimit.Value = Nothing
            End If

            GoverningStateQuote.CPP_CRM_ProgramTypeId = "48"
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        If Quote IsNot Nothing Then
            Dim msg_prefix = ""

            If Me.chkTheft.Checked Then
                Dim valPrefix_Employee As String = "Employee Theft - "
                If String.IsNullOrWhiteSpace(Me.txtEmployee_Limit.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtEmployee_Limit.Text) < 1 Then
                    Me.ValidationHelper.AddError(String.Format("{0}Limit field must be a whole positive value.", valPrefix_Employee), txtEmployee_Limit.ClientID)
                Else
                    If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtEmployee_Limit.Text) <= IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.ddEmployee_Deductible.SelectedItem.Text) Then
                        Me.ValidationHelper.AddError(String.Format("{0}Deductible amount selected is equal to or greater than the limit. Please adjust either value.", valPrefix_Employee), txtEmployee_Limit.ClientID)
                    Else
                        If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtEmployee_Limit.Text) > 100000 Then
                            Me.ValidationHelper.AddError(String.Format("{0}Limit field must be 100,000 or less.", valPrefix_Employee), txtEmployee_Limit.ClientID)
                        End If
                    End If
                End If

                If String.IsNullOrWhiteSpace(Me.ddEmployee_Deductible.SelectedItem.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtOutside_Limit.Text) < 1 Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Deductible", valPrefix_Employee), ddEmployee_Deductible.ClientID)
                End If

                If String.IsNullOrWhiteSpace(Me.txtRateAbleEmployees.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtRateAbleEmployees.Text) < 1 Or Me.txtRateAbleEmployees.Text.Contains(".") Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Number of Ratable Employees", valPrefix_Employee), txtRateAbleEmployees.ClientID)
                End If

                If String.IsNullOrWhiteSpace(Me.txtEmployee_AdditionalPremises.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtEmployee_AdditionalPremises.Text) < 0 Or Me.txtEmployee_AdditionalPremises.Text.Contains(".") Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Number of Premises", valPrefix_Employee), txtEmployee_AdditionalPremises.ClientID)
                End If

            End If

            If Me.chkInside.Checked Then
                Dim valPrefix_Inside As String = "Inside Premises - "
                If String.IsNullOrWhiteSpace(Me.txtInside_Limit.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtInside_Limit.Text) < 1 Then
                    Me.ValidationHelper.AddError(String.Format("{0}Limit must be a whole positive value.", valPrefix_Inside), txtInside_Limit.ClientID)
                Else
                    If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtInside_Limit.Text) <= IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.ddInside_Deductible.SelectedItem.Text) Then
                        Me.ValidationHelper.AddError(String.Format("{0}Deductible amount selected is equal to or greater than the limit. Please adjust either value.", valPrefix_Inside), txtInside_Limit.ClientID)
                    Else
                        If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtInside_Limit.Text) > 50000 Then
                            Me.ValidationHelper.AddError(String.Format("{0}Limit must be 50,000 or less.", valPrefix_Inside), txtInside_Limit.ClientID)
                        End If
                    End If
                End If

                If String.IsNullOrWhiteSpace(Me.ddInside_Deductible.SelectedItem.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtOutside_Limit.Text) < 1 Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Deductible ", valPrefix_Inside), ddInside_Deductible.ClientID)
                End If

                If String.IsNullOrWhiteSpace(Me.txtInside_PremisesCount.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtInside_PremisesCount.Text) < 1 Or Me.txtInside_PremisesCount.Text.Contains(".") Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Number of Premises", valPrefix_Inside), txtInside_PremisesCount.ClientID)
                End If

            End If

            If Me.chkOutside.Checked Then
                Dim valPrefix_Outside As String = "Outside the Premises - "
                If String.IsNullOrWhiteSpace(Me.txtOutside_Limit.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtOutside_Limit.Text) < 1 Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Limit ", valPrefix_Outside), txtOutside_Limit.ClientID)
                Else
                    If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtOutside_Limit.Text) <= IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.ddOutside_Deductible.SelectedItem.Text) Then
                        Me.ValidationHelper.AddError(String.Format("{0}Deductible amount selected is equal to or greater than the limit. Please adjust either value.", valPrefix_Outside), txtOutside_Limit.ClientID)
                    Else
                        If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtOutside_Limit.Text) > 50000 Then
                            Me.ValidationHelper.AddError(String.Format("{0}Limit must be 50,000 or less.", valPrefix_Outside), txtOutside_Limit.ClientID)
                        End If
                    End If
                End If

                If String.IsNullOrWhiteSpace(Me.ddOutside_Deductible.SelectedItem.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtOutside_Limit.Text) < 1 Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Deductible ", valPrefix_Outside), ddOutside_Deductible.ClientID)
                End If

                If String.IsNullOrWhiteSpace(Me.txtOutside_PremisesCount.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtOutside_PremisesCount.Text) < 1 Or Me.txtOutside_PremisesCount.Text.Contains(".") Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Number of Premises", valPrefix_Outside), txtOutside_PremisesCount.ClientID)
                End If

            End If

            'added to support new coverages
            If Me.chkForgeryAlteration.Checked Then
                msg_prefix = "Forgery or Alteration - "
                If String.IsNullOrWhiteSpace(Me.txtForgeryAlteration_Limit.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtEmployee_Limit.Text) < 1 Then
                    Me.ValidationHelper.AddError(String.Format("{0}Limit field must be a whole positive value.", msg_prefix), txtForgeryAlteration_Limit.ClientID)
                Else
                    If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtForgeryAlteration_Limit.Text) <= IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.ddForgeryAlteration_Deductible.SelectedItem.Text) Then
                        Me.ValidationHelper.AddError(String.Format("{0}Deductible amount selected is equal to or greater than the limit. Please adjust either value.", msg_prefix), txtForgeryAlteration_Limit.ClientID)
                    Else
                        If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtForgeryAlteration_Limit.Text) > 100000 Then
                            Me.ValidationHelper.AddError(String.Format("{0}Limit field must be 100,000 or less.", msg_prefix), txtForgeryAlteration_Limit.ClientID)
                        End If
                    End If
                End If

                If String.IsNullOrWhiteSpace(Me.ddForgeryAlteration_Deductible.SelectedItem.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtOutside_Limit.Text) < 1 Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Deductible", msg_prefix), ddForgeryAlteration_Deductible.ClientID)
                End If

                If String.IsNullOrWhiteSpace(Me.txtForgeryAlteration_RatableEmployees.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtRateAbleEmployees.Text) < 1 Or Me.txtRateAbleEmployees.Text.Contains(".") Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Number of Ratable Employees", msg_prefix), txtForgeryAlteration_RatableEmployees.ClientID)
                End If

                If String.IsNullOrWhiteSpace(Me.txtForgeryAlteration_AdditionalPremises.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtEmployee_AdditionalPremises.Text) < 0 Or Me.txtEmployee_AdditionalPremises.Text.Contains(".") Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Number of Premises", msg_prefix), txtForgeryAlteration_AdditionalPremises.ClientID)
                End If

            End If

            If Me.chkComputerFraud.Checked Then
                msg_prefix = "Computer Fraud - "
                If String.IsNullOrWhiteSpace(Me.txtComputerFraud_Limit.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtEmployee_Limit.Text) < 1 Then
                    Me.ValidationHelper.AddError(String.Format("{0}Limit field must be a whole positive value.", msg_prefix), txtComputerFraud_Limit.ClientID)
                Else
                    If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtComputerFraud_Limit.Text) <= IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.ddComputerFraud_Deductible.SelectedItem.Text) Then
                        Me.ValidationHelper.AddError(String.Format("{0}Deductible amount selected is equal to or greater than the limit. Please adjust either value.", msg_prefix), txtComputerFraud_Limit.ClientID)
                    Else
                        If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtComputerFraud_Limit.Text) > 100000 Then
                            Me.ValidationHelper.AddError(String.Format("{0}Limit field must be 100,000 or less.", msg_prefix), txtComputerFraud_Limit.ClientID)
                        End If
                    End If
                End If

                If String.IsNullOrWhiteSpace(Me.ddComputerFraud_Deductible.SelectedItem.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtOutside_Limit.Text) < 1 Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Deductible", msg_prefix), ddComputerFraud_Deductible.ClientID)
                End If

                If String.IsNullOrWhiteSpace(Me.txtComputerFraud_RatableEmployees.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtRateAbleEmployees.Text) < 1 Or Me.txtRateAbleEmployees.Text.Contains(".") Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Number of Ratable Employees", msg_prefix), txtComputerFraud_RatableEmployees.ClientID)
                End If

                If String.IsNullOrWhiteSpace(Me.txtComputerFraud_AdditionalPremises.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtEmployee_AdditionalPremises.Text) < 0 Or Me.txtEmployee_AdditionalPremises.Text.Contains(".") Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Number of Premises", msg_prefix), txtComputerFraud_AdditionalPremises.ClientID)
                End If

            End If

            If Me.chkFundsTransferFraud.Checked Then
                msg_prefix = "Funds Transfer Fraud - "
                If String.IsNullOrWhiteSpace(Me.txtFundsTransferFraud_Limit.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtEmployee_Limit.Text) < 1 Then
                    Me.ValidationHelper.AddError(String.Format("{0}Limit field must be a whole positive value.", msg_prefix), txtFundsTransferFraud_Limit.ClientID)
                Else
                    If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtFundsTransferFraud_Limit.Text) <= IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.ddFundsTransferFraud_Deductible.SelectedItem.Text) Then
                        Me.ValidationHelper.AddError(String.Format("{0}Deductible amount selected is equal to or greater than the limit. Please adjust either value.", msg_prefix), txtFundsTransferFraud_Limit.ClientID)
                    Else
                        If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtFundsTransferFraud_Limit.Text) > 100000 Then
                            Me.ValidationHelper.AddError(String.Format("{0}Limit field must be 100,000 or less.", msg_prefix), txtFundsTransferFraud_Limit.ClientID)
                        End If
                    End If
                End If

                If String.IsNullOrWhiteSpace(Me.ddFundsTransferFraud_Deductible.SelectedItem.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtOutside_Limit.Text) < 1 Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Deductible", msg_prefix), ddFundsTransferFraud_Deductible.ClientID)
                End If

                If String.IsNullOrWhiteSpace(Me.txtFundsTransferFraud_RatableEmployees.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtRateAbleEmployees.Text) < 1 Or Me.txtRateAbleEmployees.Text.Contains(".") Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Number of Ratable Employees", msg_prefix), txtFundsTransferFraud_RatableEmployees.ClientID)
                End If

                If String.IsNullOrWhiteSpace(Me.txtFundsTransferFraud_AdditionalPremises.Text) Then 'Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtEmployee_AdditionalPremises.Text) < 0 Or Me.txtEmployee_AdditionalPremises.Text.Contains(".") Then
                    Me.ValidationHelper.AddError(String.Format("{0}Missing Number of Premises", msg_prefix), txtFundsTransferFraud_AdditionalPremises.ClientID)
                End If

            End If

            If (chkTheft.Checked = True OrElse
                chkInside.Checked = True OrElse
                chkOutside.Checked = True OrElse
                chkForgeryAlteration.Checked OrElse
                chkComputerFraud.Checked OrElse
                chkFundsTransferFraud.Checked) And
               String.IsNullOrEmpty(txtClassCode.Text) Then
                Me.ValidationHelper.AddError("You must add a crime class code. Contact the company if you need assistance.", txtClassCode.ClientID)
            End If

            'If String.IsNullOrEmpty(txtClassCode.Text) = False AndAlso (Me.chkTheft.Checked = False AndAlso Me.chkInside.Checked = False AndAlso Me.chkOutside.Checked = False) Then
            '    Me.ValidationHelper.AddError("You must add a crime coverage. Contact the company if you need assistance.", txtClassCode.ClientID)
            'End If

        End If


    End Sub

    Private Sub lnkClassCodeClear_Click(sender As Object, e As EventArgs) Handles btnClassCodeClear.Click
        Quote.ClassificationCodes = New List(Of QuickQuoteClassificationCode)
        Me.txtClassCode.Text = Nothing
        Me.txtDescription.Text = Nothing
        hdnDIA_Id.Value = Nothing
        hdnPMAID.Value = Nothing
        hdnGroupRate.Value = Nothing
        hdnClassLimit.Value = Nothing
        Save_FireSaveEvent()
        Populate()
    End Sub

    Private Sub lnkCrimeCoveragesClear_Click(sender As Object, e As EventArgs) Handles btnCrimeCoveragesClear.Click
        Me.chkTheft.Checked = False
        GoverningStateQuote.EmployeeTheftLimit = ""
        GoverningStateQuote.EmployeeTheftDeductibleId = ""
        GoverningStateQuote.EmployeeTheftNumberOfRatableEmployees = ""
        txtRateAbleEmployees.Text = ""
        GoverningStateQuote.EmployeeTheftNumberOfAdditionalPremises = ""
        GoverningStateQuote.EmployeeTheftScheduledEmployeeBenefitPlans = Nothing
        Me.chkInside.Checked = False
        GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesLimit = ""
        GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId = ""
        GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesNumberOfPremises = ""
        Me.chkOutside.Checked = False
        GoverningStateQuote.OutsideThePremisesLimit = ""
        GoverningStateQuote.OutsideThePremisesDeductibleId = ""
        GoverningStateQuote.OutsideThePremisesNumberOfPremises = ""

        'added to suport new coverages -- we need to remove this duplication
        Me.chkForgeryAlteration.Checked = False
        GoverningStateQuote.ForgeryAlterationLimit = ""
        GoverningStateQuote.ForgeryAlterationDeductibleId = ""
        GoverningStateQuote.ForgeryAlterationNumberOfRatableEmployees = ""
        GoverningStateQuote.ForgeryAlterationAdditionalPremises = ""
        txtForgeryAlteration_RatableEmployees.Text = ""

        Me.chkComputerFraud.Checked = False
        GoverningStateQuote.ComputerFraudLimit = ""
        GoverningStateQuote.ComputerFraudDeductibleId = ""
        GoverningStateQuote.ComputerFraudNumberOfRatableEmployees = ""
        GoverningStateQuote.ComputerFraudAdditionalPremises = ""
        txtComputerFraud_RatableEmployees.Text = ""

        Me.chkFundsTransferFraud.Checked = False
        GoverningStateQuote.FundsTransferFraudLimit = ""
        GoverningStateQuote.FundsTransferFraudDeductibleId = ""
        GoverningStateQuote.FundsTransferFraudNumberOfRatableEmployees = ""
        GoverningStateQuote.FundsTransferFraudAdditionalPremises = ""
        txtFundsTransferFraud_RatableEmployees.Text = ""

        Save_FireSaveEvent()
        Populate()
    End Sub

    Public Overrides Sub ClearControl()
        'Updated 12/17/18 for multi state bug 30351 MLW
        'Quote.ClassificationCodes = New List(Of QuickQuoteClassificationCode)
        GoverningStateQuote.ClassificationCodes = New List(Of QuickQuoteClassificationCode)
        Me.txtClassCode.Text = Nothing
        Me.txtDescription.Text = Nothing
        hdnDIA_Id.Value = Nothing
        hdnPMAID.Value = Nothing
        hdnGroupRate.Value = Nothing
        hdnClassLimit.Value = Nothing

        Me.chkTheft.Checked = False
        GoverningStateQuote.EmployeeTheftLimit = ""
        GoverningStateQuote.EmployeeTheftDeductibleId = ""
        GoverningStateQuote.EmployeeTheftNumberOfRatableEmployees = ""
        txtRateAbleEmployees.Text = ""
        GoverningStateQuote.EmployeeTheftNumberOfAdditionalPremises = ""
        GoverningStateQuote.EmployeeTheftScheduledEmployeeBenefitPlans = Nothing
        Me.chkInside.Checked = False
        GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesLimit = ""
        GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId = ""
        GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesNumberOfPremises = ""
        Me.chkOutside.Checked = False
        GoverningStateQuote.OutsideThePremisesLimit = ""
        GoverningStateQuote.OutsideThePremisesDeductibleId = ""
        GoverningStateQuote.OutsideThePremisesNumberOfPremises = ""

        'added to suport new coverages -- we need to remove this duplication
        Me.chkForgeryAlteration.Checked = False
        GoverningStateQuote.ForgeryAlterationLimit = ""
        GoverningStateQuote.ForgeryAlterationDeductibleId = ""
        GoverningStateQuote.ForgeryAlterationNumberOfRatableEmployees = ""
        GoverningStateQuote.ForgeryAlterationAdditionalPremises = ""
        txtForgeryAlteration_RatableEmployees.Text = ""

        Me.chkComputerFraud.Checked = False
        GoverningStateQuote.ComputerFraudLimit = ""
        GoverningStateQuote.ComputerFraudDeductibleId = ""
        GoverningStateQuote.ComputerFraudNumberOfRatableEmployees = ""
        GoverningStateQuote.ComputerFraudAdditionalPremises = ""
        txtComputerFraud_RatableEmployees.Text = ""

        Me.chkFundsTransferFraud.Checked = False
        GoverningStateQuote.FundsTransferFraudLimit = ""
        GoverningStateQuote.FundsTransferFraudDeductibleId = ""
        GoverningStateQuote.FundsTransferFraudNumberOfRatableEmployees = ""
        GoverningStateQuote.FundsTransferFraudAdditionalPremises = ""
        txtFundsTransferFraud_RatableEmployees.Text = ""
        GoverningStateQuote.CPP_Has_Crime_PackagePart = False
        Populate()
        Save_FireSaveEvent()
    End Sub


    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnRate.Click, lnkSave.Click, btnCrimeCoveragesSave.Click, btnClassCodeSave.Click
        Me.Save_FireSaveEvent()
        Populate()
        If sender.Equals(btnRate) Then
            Session("CPPCPRCheckACVEventTrigger") = "RateButton"
            If Me.ValidationSummmary.HasErrors = False Then
                Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
            End If
        End If
    End Sub

    Protected Sub btnSetEmployeeRateable_Click(sender As Object, e As EventArgs) Handles btnSetEmployeeRateable.Click
        If Quote IsNot Nothing Then
            Dim numOfficers As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtEmployee_OfficerCount.Text)
            Dim numHandleMoney As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtEmployee_HandleMoneyCount.Text)
            Dim numRemaining As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtEmployee_RemainingEmployees.Text)
            Dim calcedNumRemaining As Double = Math.Ceiling(numRemaining * 0.01)
            Dim calcedRatable As Double = CInt(numOfficers + numHandleMoney + calcedNumRemaining).ToString()

            Select Case hidTarget.Value
                Case "employeeTheft"
                    GoverningStateQuote.EmployeeTheftNumberOfRatableEmployees = calcedRatable
                    txtRateAbleEmployees.Text = calcedRatable
                Case "forgeryAlteration"
                    GoverningStateQuote.ForgeryAlterationNumberOfRatableEmployees = calcedRatable
                    txtForgeryAlteration_RatableEmployees.Text = calcedRatable
                Case "computerFraud"
                    GoverningStateQuote.ComputerFraudNumberOfRatableEmployees = calcedRatable
                    txtComputerFraud_RatableEmployees.Text = calcedRatable
                Case "fundsTransferFraud"
                    GoverningStateQuote.FundsTransferFraudNumberOfRatableEmployees = calcedRatable
                    txtFundsTransferFraud_RatableEmployees.Text = calcedRatable
            End Select
            'txtRateAbleEmployees.Text = Quote.EmployeeTheftNumberOfRatableEmployees
            'txtRateAbleEmployees.Text = GoverningStateQuote.EmployeeTheftNumberOfRatableEmployees
        End If

    End Sub

    Private Sub btnClassCodeLookup_Click(sender As Object, e As EventArgs) Handles btnClassCodeLookup.Click
        Me.ctl_CPP_CrimeClassCodeLookup.Show()
    End Sub

    'Added 11/23/2021 for CPP Endorsements task 65599 MLW
    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    'Added 11/23/2021 for CPP Endorsements task 65599 MLW
    Protected Sub btnViewBillingInformation_Click(sender As Object, e As EventArgs) Handles btnViewBillingInfo.Click
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub
End Class