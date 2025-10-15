Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass

Public Class ctlCoverage_PPA_Vehicle_List
    Inherits VRControlBase

    Public Event SaveAllCoverages(validate As Boolean)
    Public Event QuoteRateRequested()
    Public Event SetVehCovList(coverageList As String)
    Public Event ToggleVehPolicyCoverage(liabilityType As String)
    Public Event RequestPageRefresh()


    Public Property VehicleNumber As Int32
        Get
            If ViewState("vs_vehicleNum") Is Nothing Then
                ViewState("vs_vehicleNum") = 0
            End If
            Return CInt(ViewState("vs_vehicleNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_vehicleNum") = value
        End Set
    End Property

    Public Property ToggleCompOnlyPanel() As Boolean
        Get
            Return CBool(Session("sess_CompOnlyToggle"))
        End Get
        Set(ByVal value As Boolean)
            Session("sess_CompOnlyToggle") = value
        End Set
    End Property

    Public Property RouteToUwIsVisible As Boolean 'added 11/11/2019
        Get
            Return Me.ctl_RouteToUw.Visible
        End Get
        Set(value As Boolean)
            Me.ctl_RouteToUw.Visible = value
        End Set
    End Property

    Public ReadOnly Property ParentAsCoverage_PPA() As ctlCoverage_PPA
        Get
            Return TryCast(Me.Parent, ctlCoverage_PPA)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ListAccordionDivId = dvCoverageVehicleList.ClientID
            LoadStaticData()
        End If
        AttachCoverageControlEvents()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.AddVariableLine("var VehicleCoverageListControlDivTopMost = '" + hidden_VehicleCoverageListActive.ClientID + "';")
        VRScript.AddVariableLine("function ShowVehicle(index){SetActiveAccordionIndex('" + ListAccordionDivId + "',index);}")
        VRScript.CreateAccordion(ListAccordionDivId, hidden_VehicleCoverageListActive, "0")
        If IsQuoteEndorsement() Then
            VRScript.AddScriptLine("$('input[id*=""txtMotorEquip""]').on(""change"", function () { window.location = $(this).closest('div[id*=""dvMotorcycle""]').closest('div.ui-accordion-content').prev('h3').find('a[id*=""lnkBtnSave""]').attr('href');});")
        End If

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            If Quote.Vehicles IsNot Nothing Then
                Me.Repeater1.DataSource = Me.Quote.Vehicles
                Me.Repeater1.DataBind()
                FindChildVrControls()
                Dim index As Int32 = 0
                For Each c As ctlCoverage_PPA_VehicleSpecific In Me.GatherChildrenOfType(Of ctlCoverage_PPA_VehicleSpecific)()
                    c.VehicleNumber = index
                    c.Populate()
                    index += 1
                Next
            End If
        End If


        If hidden_VehicleCoverageListActive.Value.IsNumeric AndAlso Quote.Vehicles.HasItemAtIndex(hidden_VehicleCoverageListActive.Value) AndAlso Quote.Vehicles(Integer.Parse(hidden_VehicleCoverageListActive.Value)).ComprehensiveCoverageOnly Then ' Matt A - Added isnumeric 8-17-2015 because I've seen many error emails on this line of code
            ToggleCompOnlyPanel = True
        Else
            ToggleCompOnlyPanel = False
        End If

        If Me.Quote IsNot Nothing Then
            If IsQuoteReadOnly() Then
                Dim policyNumber As String = Me.Quote.PolicyNumber
                Dim imageNum As Integer = 0
                Dim policyId As Integer = 0
                Dim toolTip As String = "Make a change to this policy"
                'Dim qqHelper As New QuickQuoteHelperClass
                Dim readOnlyViewPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                    readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                End If

                divActionButtons.Visible = False
                divEndorsementButtons.Visible = True

                btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                btnMakeAChange.ToolTip = toolTip
                btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
            End If
        End If

        If IsQuoteEndorsement() Then
            ctl_Billing_Info_PPA.Visible = True
            PopulateChildControls()
        Else
            ctl_Billing_Info_PPA.Visible = False
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = String.Format("Vehicle #{0}", VehicleNumber + 1)

        ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean
        SaveChildControls()
        Return True
    End Function

    Private Sub Repeater1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
        Dim driverControl As ctlCoverage_PPA_VehicleSpecific = CType(e.Item.FindControl("ctlCoverage_PPA_VehicleSpecific"), ctlCoverage_PPA_VehicleSpecific)
        driverControl.VehicleNumber = e.Item.ItemIndex
        driverControl.Populate()
    End Sub

    Protected Sub AttachCoverageControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim vehicleCoverageControl As ctlCoverage_PPA_VehicleSpecific = CType(cntrl.FindControl("ctlCoverage_PPA_VehicleSpecific"), ctlCoverage_PPA_VehicleSpecific)
            AddHandler vehicleCoverageControl.SetVehPolicy, AddressOf SetVehiclePolicy
            AddHandler vehicleCoverageControl.SaveVehPolicy, AddressOf SaveVehiclePolicy
            AddHandler vehicleCoverageControl.RequestPageRefresh, AddressOf vehicleControlRequestPageRefresh
            AddHandler vehicleCoverageControl.SavePreviousVehicle, AddressOf SavePrevVehicle
            index += 1
        Next
    End Sub

    'Updated 9/27/18 for multi state MLW - added umCSL and umBI
    Private Sub SetVehiclePolicy(vehCovPlan As String, vehNum As Integer, bodilyInj As String, propDamage As String, ssl As String, medPay As String, umumiSSL As String, umCSL As String,
                                 umumiBI As String, umBI As String, umPD As String, umpdDeduct As String, autoEnhance As Boolean, multiPolicy As Boolean, marketCredit As Boolean, policyCoverage As String)
        If hiddenVehicleCoveragePlanList.Value = "" Then
            hiddenVehicleCoveragePlanList.Value = vehNum.ToString() + "!" + vehCovPlan
        Else
            hiddenVehicleCoveragePlanList.Value += "|" + vehNum.ToString() + "!" + vehCovPlan
        End If

        RaiseEvent SetVehCovList(hiddenVehicleCoveragePlanList.Value)

        Dim ddBI As DropDownList = CType(Parent.FindControl("ddBodilyInjury"), DropDownList)
        ddBI.SelectedValue = bodilyInj
        Dim ddPD As DropDownList = CType(Parent.FindControl("ddPropertyDamage"), DropDownList)
        ddPD.SelectedValue = propDamage
        Dim ddSingleLimit As DropDownList = CType(Parent.FindControl("ddSingleLimitLib"), DropDownList)
        ddSingleLimit.SelectedValue = ssl
        Dim ddMedPay As DropDownList = CType(Parent.FindControl("ddmedicalPayments"), DropDownList)
        ddMedPay.SelectedValue = medPay
        'Updated 9/27/18 for multi state MLW
        Select Case (Quote.QuickQuoteState)
            Case QuickQuoteHelperClass.QuickQuoteState.Ohio 'Added 1/17/2022 for OH task 66101 MLW
                Dim ddUmCSLStateExpansion As DropDownList = CType(Parent.FindControl("ddUMCSL"), DropDownList)
                ddUmCSLStateExpansion.SelectedValue = umCSL
                Dim txtUIMCSL As TextBox = CType(Parent.FindControl("txtUIMCSL"), TextBox)
                txtUIMCSL.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredCombinedSingleLimitId, Quote.QuickQuoteState, ddUmCSLStateExpansion.SelectedValue)
                Dim ddUmBIStateExpansion As DropDownList = CType(Parent.FindControl("ddUMBI"), DropDownList)
                ddUmBIStateExpansion.SelectedValue = umBI
                Dim txtUIMBI As TextBox = CType(Parent.FindControl("txtUIMBI"), TextBox)
                txtUIMBI.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, Quote.QuickQuoteState, ddUmBIStateExpansion.SelectedValue)
                '10/2/18 cannot set UMPD Limit here since it is on a different control now MLW
            Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                Dim ddUmCSLStateExpansion As DropDownList = CType(Parent.FindControl("ddUMCSL"), DropDownList)
                ddUmCSLStateExpansion.SelectedValue = umCSL
                Dim txtUIMCSL As TextBox = CType(Parent.FindControl("txtUIMCSL"), TextBox)
                txtUIMCSL.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredCombinedSingleLimitId, Quote.QuickQuoteState, ddUmCSLStateExpansion.SelectedValue)
                Dim ddUmBIStateExpansion As DropDownList = CType(Parent.FindControl("ddUMBI"), DropDownList)
                ddUmBIStateExpansion.SelectedValue = umBI
                Dim txtUIMBI As TextBox = CType(Parent.FindControl("txtUIMBI"), TextBox)
                txtUIMBI.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, Quote.QuickQuoteState, ddUmBIStateExpansion.SelectedValue)
                '10/2/18 cannot set UMPD Limit here since it is on a different control now MLW
            Case Else
                Dim ddUmSLL As DropDownList = CType(Parent.FindControl("ddUmUimSSl"), DropDownList)
                ddUmSLL.SelectedValue = umumiSSL
                Dim ddUmBI As DropDownList = CType(Parent.FindControl("ddUmUmiBi"), DropDownList)
                ddUmBI.SelectedValue = umumiBI
                Dim ddUmProperty As DropDownList = CType(Parent.FindControl("ddUmPd"), DropDownList)
                ddUmProperty.SelectedValue = umPD
                Dim ddPropDeduct As DropDownList = CType(Parent.FindControl("ddUmPdDeductible"), DropDownList)
                ddPropDeduct.SelectedValue = umpdDeduct
        End Select
        Dim ddLiabType As DropDownList = CType(Parent.FindControl("ddLiabType"), DropDownList)
        ddLiabType.SelectedValue = policyCoverage

        RaiseEvent ToggleVehPolicyCoverage(ddLiabType.SelectedValue)
    End Sub

    Private Sub SaveVehiclePolicy(ByRef vehicle As QuickQuoteVehicle, hiddenVehicleCoverage As String)
        If vehicle IsNot Nothing Then
            If hiddenVehicleCoverage <> "" Then
                If hiddenVehicleCoverage.IsNullEmptyorWhitespace = False _
                    AndAlso IsNumeric(hiddenVehicleCoverage) = True _
                    AndAlso hiddenVehicleCoverage = CInt(ENUMHelper.VehiclePolicyType.compOnly) Then
                    vehicle.ComprehensiveCoverageOnly = True
                    vehicle.Liability_UM_UIM_LimitId = "0"
                    vehicle.MedicalPaymentsLimitId = "0"
                    vehicle.BodilyInjuryLiabilityLimitId = "0"
                    vehicle.PropertyDamageLimitId = "0"

                    'Updated 10/2/18 for multi state MLW
                    Select Case (Quote.QuickQuoteState)
                        Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                            vehicle.UninsuredCombinedSingleLimitId = "0"
                            vehicle.UnderinsuredCombinedSingleLimitId = "0"
                            vehicle.UninsuredMotoristLiabilityLimitId = "" 'Added 4/5/2022 for bug 68773 MLW
                            vehicle.UninsuredBodilyInjuryLimitId = ""
                            vehicle.UnderinsuredBodilyInjuryLimitId = ""
                            vehicle.UninsuredMotoristPropertyDamageLimitId = "0"
                            vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ""
                        Case Else
                            vehicle.UninsuredCombinedSingleLimitId = "0"
                            vehicle.UninsuredMotoristLiabilityLimitId = "0"
                            vehicle.UninsuredMotoristPropertyDamageLimitId = "0"
                            vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = "0"
                    End Select

                Else
                    Dim ddBI As DropDownList = CType(Parent.FindControl("ddBodilyInjury"), DropDownList)
                    vehicle.BodilyInjuryLiabilityLimitId = ddBI.SelectedValue
                    Dim ddPD As DropDownList = CType(Parent.FindControl("ddPropertyDamage"), DropDownList)
                    vehicle.PropertyDamageLimitId = ddPD.SelectedValue
                    Dim ddSingleLimit As DropDownList = CType(Parent.FindControl("ddSingleLimitLib"), DropDownList)
                    vehicle.Liability_UM_UIM_LimitId = ddSingleLimit.SelectedValue
                    Dim ddMedPay As DropDownList = CType(Parent.FindControl("ddmedicalPayments"), DropDownList)
                    vehicle.MedicalPaymentsLimitId = ddMedPay.SelectedValue
                    'Updated 10/2/18 for multi state MLW
                    Select Case (Quote.QuickQuoteState)
                        Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW

                            'TODO: clean this up - the same code should not be in two different places
                            If Me.ParentAsCoverage_PPA?.SelectedLiabilityTypeValue = "1" Then
                                Dim ddUmCSLStateExpansion As DropDownList = CType(Parent.FindControl("ddUMCSL"), DropDownList)
                                vehicle.UninsuredCombinedSingleLimitId = ddUmCSLStateExpansion.SelectedValue
                                vehicle.UnderinsuredCombinedSingleLimitId = ddUmCSLStateExpansion.SelectedValue
                                vehicle.UninsuredBodilyInjuryLimitId = ""
                                vehicle.UnderinsuredBodilyInjuryLimitId = ""
                            Else
                                Dim ddUmBIStateExpansion As DropDownList = CType(Parent.FindControl("ddUMBI"), DropDownList)
                                vehicle.UninsuredBodilyInjuryLimitId = ddUmBIStateExpansion.SelectedValue
                                vehicle.UnderinsuredBodilyInjuryLimitId = ddUmBIStateExpansion.SelectedValue
                                vehicle.UninsuredCombinedSingleLimitId = ""
                                vehicle.UnderinsuredCombinedSingleLimitId = ""
                            End If
                            'vehicle.UninsuredMotoristPropertyDamageLimitId, txtUMPDLimit saved in ctlCoverage_PPA_VehicleSpecific
                            vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ""
                            vehicle.UninsuredMotoristLiabilityLimitId = ""
                        Case Else
                            Dim ddUmSLL As DropDownList = CType(Parent.FindControl("ddUmUimSSl"), DropDownList)
                            vehicle.UninsuredCombinedSingleLimitId = ddUmSLL.SelectedValue
                            Dim ddUmBI As DropDownList = CType(Parent.FindControl("ddUmUmiBi"), DropDownList)
                            vehicle.UninsuredMotoristLiabilityLimitId = ddUmBI.SelectedValue
                            Dim ddUmProperty As DropDownList = CType(Parent.FindControl("ddUmPd"), DropDownList)
                            vehicle.UninsuredMotoristPropertyDamageLimitId = ddUmProperty.SelectedValue
                            Dim ddPropDeduct As DropDownList = CType(Parent.FindControl("ddUmPdDeductible"), DropDownList)
                            vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ddPropDeduct.SelectedValue
                    End Select

                    Dim chkAutoEnhance As CheckBox = CType(Parent.FindControl("chkAutoEnhance"), CheckBox)

                    vehicle.ComprehensiveCoverageOnly = False
                End If
            Else
                If vehicle.BodyTypeId.TryToGetInt32 <> ENUMHelper.VehicleBodyType.bodyType_RecTrailer AndAlso vehicle.BodyTypeId.TryToGetInt32 <> ENUMHelper.VehicleBodyType.bodyType_OtherTrailer Then
                    'If hiddenVehicleCoverage = ENUMHelper.VehiclePolicyType.compOnly Then
                    If vehicle.ComprehensiveCoverageOnly Then
                        vehicle.ComprehensiveCoverageOnly = True
                        vehicle.Liability_UM_UIM_LimitId = "0"
                        vehicle.MedicalPaymentsLimitId = "0"
                        vehicle.BodilyInjuryLiabilityLimitId = "0"
                        vehicle.PropertyDamageLimitId = "0"

                        'Updated 10/2/18 for multi state MLW
                        Select Case (Quote.QuickQuoteState)
                            Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                                vehicle.UninsuredCombinedSingleLimitId = "0"
                                vehicle.UnderinsuredCombinedSingleLimitId = "0"
                                vehicle.UninsuredMotoristLiabilityLimitId = "" 'Added 4/5/2022 for bug 68773 MLW
                                vehicle.UninsuredBodilyInjuryLimitId = ""
                                vehicle.UnderinsuredBodilyInjuryLimitId = ""
                                vehicle.UninsuredMotoristPropertyDamageLimitId = "0"
                                vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ""
                            Case Else
                                vehicle.UninsuredCombinedSingleLimitId = "0"
                                vehicle.UninsuredMotoristLiabilityLimitId = "0"
                                vehicle.UninsuredMotoristPropertyDamageLimitId = "0"
                                vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = "0"
                        End Select
                    Else
                        Dim ddBI As DropDownList = CType(Parent.FindControl("ddBodilyInjury"), DropDownList)
                        vehicle.BodilyInjuryLiabilityLimitId = ddBI.SelectedValue
                        Dim ddPD As DropDownList = CType(Parent.FindControl("ddPropertyDamage"), DropDownList)
                        vehicle.PropertyDamageLimitId = ddPD.SelectedValue
                        Dim ddSingleLimit As DropDownList = CType(Parent.FindControl("ddSingleLimitLib"), DropDownList)
                        vehicle.Liability_UM_UIM_LimitId = ddSingleLimit.SelectedValue
                        Dim ddMedPay As DropDownList = CType(Parent.FindControl("ddmedicalPayments"), DropDownList)
                        vehicle.MedicalPaymentsLimitId = ddMedPay.SelectedValue
                        'Updated 10/2/18 for multi state MLW
                        Select Case (Quote.QuickQuoteState)
                            Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                                If Me.ParentAsCoverage_PPA?.SelectedLiabilityTypeValue = "1" Then
                                    Dim ddUmCSLStateExpansion As DropDownList = CType(Parent.FindControl("ddUMCSL"), DropDownList)
                                    vehicle.UninsuredCombinedSingleLimitId = ddUmCSLStateExpansion.SelectedValue
                                    vehicle.UnderinsuredCombinedSingleLimitId = ddUmCSLStateExpansion.SelectedValue
                                    vehicle.UninsuredBodilyInjuryLimitId = ""
                                    vehicle.UnderinsuredBodilyInjuryLimitId = ""
                                Else
                                    Dim ddUmBIStateExpansion As DropDownList = CType(Parent.FindControl("ddUMBI"), DropDownList)
                                    vehicle.UninsuredBodilyInjuryLimitId = ddUmBIStateExpansion.SelectedValue
                                    vehicle.UnderinsuredBodilyInjuryLimitId = ddUmBIStateExpansion.SelectedValue
                                    vehicle.UninsuredCombinedSingleLimitId = ""
                                    vehicle.UnderinsuredCombinedSingleLimitId = ""
                                End If
                                'vehicle.UninsuredMotoristPropertyDamageLimitId, txtUMPDLimit saved in ctlCoverage_PPA_VehicleSpecific
                                vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ""
                                vehicle.UninsuredMotoristLiabilityLimitId = ""
                            Case Else
                                Dim ddUmSLL As DropDownList = CType(Parent.FindControl("ddUmUimSSl"), DropDownList)
                                vehicle.UninsuredCombinedSingleLimitId = ddUmSLL.SelectedValue
                                Dim ddUmBI As DropDownList = CType(Parent.FindControl("ddUmUmiBi"), DropDownList)
                                vehicle.UninsuredMotoristLiabilityLimitId = ddUmBI.SelectedValue
                                Dim ddUmProperty As DropDownList = CType(Parent.FindControl("ddUmPd"), DropDownList)
                                vehicle.UninsuredMotoristPropertyDamageLimitId = ddUmProperty.SelectedValue
                                Dim ddPropDeduct As DropDownList = CType(Parent.FindControl("ddUmPdDeductible"), DropDownList)
                                vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ddPropDeduct.SelectedValue
                        End Select

                        Dim chkAutoEnhance As CheckBox = CType(Parent.FindControl("chkAutoEnhance"), CheckBox)

                        vehicle.ComprehensiveCoverageOnly = False
                    End If
                Else
                    vehicle.Liability_UM_UIM_LimitId = "0"
                    vehicle.MedicalPaymentsLimitId = "0"
                    vehicle.BodilyInjuryLiabilityLimitId = "0"
                    vehicle.PropertyDamageLimitId = "0"
                    'Updated 10/2/18 for multi state MLW
                    Select Case (Quote.QuickQuoteState)
                        Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                            vehicle.UninsuredCombinedSingleLimitId = "0"
                            vehicle.UnderinsuredCombinedSingleLimitId = "0"
                            vehicle.UninsuredMotoristLiabilityLimitId = "" 'Added 4/5/2022 for bug 68773 MLW
                            vehicle.UninsuredBodilyInjuryLimitId = ""
                            vehicle.UnderinsuredBodilyInjuryLimitId = ""
                            vehicle.UninsuredMotoristPropertyDamageLimitId = "0"
                            vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ""
                        Case Else
                            vehicle.UninsuredCombinedSingleLimitId = "0"
                            vehicle.UninsuredMotoristLiabilityLimitId = "0"
                            vehicle.UninsuredMotoristPropertyDamageLimitId = "0"
                            vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = "0"
                    End Select
                End If
            End If
        End If
    End Sub

    Private Sub vehicleCoverageControlRequestedSave(invokeValidations As Boolean)
        RaiseEvent SaveAllCoverages(invokeValidations)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Me.ctl_RouteToUw.Visible = False 'added 11/11/2019
        RaiseEvent SaveAllCoverages(True)
    End Sub

    Protected Sub btnRate_Click(sender As Object, e As EventArgs) Handles btnRate.Click
        Me.ctl_RouteToUw.Visible = False 'added 11/11/2019
        RaiseEvent QuoteRateRequested()
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
    End Sub

    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    Protected Sub btnViewGotoBilling_Click(sender As Object, e As EventArgs) Handles btnViewGotoBilling.Click
        'Added 8/5/2019 for Bug 39665 MLW
        Me.Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub

    Private Sub vehicleControlRequestPageRefresh()
        RaiseEvent RequestPageRefresh()
    End Sub

    Public Sub SavePrevVehicle(VehicleIndex As Integer)
        If VehicleIndex >= 0 Then
            Dim myVehicles As List(Of ctlCoverage_PPA_VehicleSpecific) = Me.GatherChildrenOfType(Of ctlCoverage_PPA_VehicleSpecific)()
            If myVehicles.HasItemAtIndex(VehicleIndex) Then
                myVehicles(VehicleIndex).Save()
                Dim errMsg As String = ""
                If IsQuoteEndorsement() Then
                    Dim success = IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Quote)
                Else
                    IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(QuoteId, Quote, errMsg)
                End If
            End If
        End If        
    End Sub
End Class