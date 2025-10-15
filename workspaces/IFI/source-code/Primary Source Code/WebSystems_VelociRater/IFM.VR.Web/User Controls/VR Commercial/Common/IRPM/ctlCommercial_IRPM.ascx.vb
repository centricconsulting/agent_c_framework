Imports IFM.VR.Common.IRPMData
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers
Public Class ctlCommercial_IRPM
    Inherits VRControlBase


    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.Quote IsNot Nothing Then
            Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID, False)
            Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
            _script.AddScriptLine("$(""#IRPMDiv"").accordion({collapsible: false, heightStyle: ""content""});")
            _script.AddScriptLine("$("".IRPMSection"").accordion({collapsible: false, heightStyle: ""content""});")
            Me.VRScript.CreateJSBinding(Me.btnSubmitRate.ClientID, "mousedown", "$(this).focus(); return ValidateForm();")
            Me.VRScript.CreateJSBinding(Me.btnSubmitRateDual.ClientID, "mousedown", "$(this).focus(); return ValidateForm();")

            'added 1/6/2021 (Interoperability)
            If Me.Quote.HasRuleOverride() = True Then
                Me.VRScript.AddScriptLine("ifm.vr.ui.ForceDisableContent(['IRPMDiv']);", onlyAllowOnce:=True)
                'Me.IRPMDiv.Attributes.Item("title") = "Edit Mode disabled due to Rule Override"
                'note: will just use js for the tooltip too
                'Me.VRScript.AddScriptLine("document.getElementById('" & Me.IRPMDiv.ClientID & "').title='Edit Mode disabled due to Rule Override';", onlyAllowOnce:=True)
                'note: div doesn't runat server; code would need to be updated if it ever does
                Me.VRScript.AddScriptLine("document.getElementById('IRPMDiv').title='Edit Mode disabled due to Rule Override';", onlyAllowOnce:=True)
            End If
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        'Updated 04/09/2021 for CAP Endorsements Task 52975 MLW
        If Not (IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto) Then

            If CAP.CAPScheduledCredit.IsCAPScheduledCreditAvailable(Quote) Then
                sectionMain.Visible = False
                sectionMainDual.Visible = True
            Else
                sectionMain.Visible = True
                sectionMainDual.Visible = False
            End If


            DataBind()
            Dim Ratings = Me.SubQuoteFirst.ScheduledRatings

            If Ratings Is Nothing OrElse Ratings.Count = 0 Then 'added 12/2/2020 (Interoperability)
                Dim qqXml As New QuickQuote.CommonMethods.QuickQuoteXML
                qqXml.QuoteObjectConversion_PolicyScheduledRatings(Me.SubQuoteFirst, Ratings, useConvertedFlag:=False)
            End If

            Select Case Me.Quote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                'Nothing Required
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    Me.lblTitle.Text = "Credits/Debits"
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                    Me.lblTitle.Text = "Credits/Debits"
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                    Me.lblTitle.Text = "Credits/Debits"
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                    'Nothing Required

            End Select

            Ratings = IRPMData.GetIRPMData(Ratings, Me.Quote.LobType)

            If CAP.CAPScheduledCredit.IsCAPScheduledCreditAvailable(Quote) Then
                Me.rptIrpmDual.DataSource = ConvertDualDisplay(Ratings)
                Me.rptIrpmDual.DataBind()
            Else
                Me.rptIRPM.DataSource = Ratings
                Me.rptIRPM.DataBind()
            End If

            If IsOnAppPage _
                AndAlso Helpers.NewCo_Prefill_Helper.ShouldReturnToQuoteForPreFillOrNewCo(Me.Quote, Me.DefaultValidationType, Me.QuoteIdOrPolicyIdPipeImageNumber) Then
                If CAP.CAPScheduledCredit.IsCAPScheduledCreditAvailable(Quote) Then
                    VRScript.FakeDisableSingleElement(sectionMainDual)
                    VRScript.FakeDisabledSingleElement_ReEnable(btnCancelDual)
                    VRScript.FakeDisabledSingleElement_ReEnable(Button3)
                    btnSubmitRateDual.Visible = False
                Else
                    VRScript.FakeDisableSingleElement(sectionMain)
                    VRScript.FakeDisabledSingleElement_ReEnable(btnCancel)
                    VRScript.FakeDisabledSingleElement_ReEnable(Button6)
                    btnSubmitRate.Visible = False
                End If
            End If


        End If
    End Sub

    Protected Function ConvertDualDisplay(ratings As List(Of QuickQuote.CommonObjects.QuickQuoteScheduledRating)) As List(Of DualDisplayIrpm)
        Dim dualIrpmList = New List(Of DualDisplayIrpm)
        Dim dualIrpmItem = New DualDisplayIrpm
        For Each item As QuickQuote.CommonObjects.QuickQuoteScheduledRating In ratings
            dualIrpmItem = New DualDisplayIrpm
            If item.ScheduleRatingTypeId = "1" Then
                With dualIrpmItem
                    .Description = item.Description
                    .Maximum = item.Maximum
                    .Minimum = item.Minimum
                    .DetailStatusCode = item.DetailStatusCode
                    .PolicyId = item.PolicyId
                    .PolicyImageNum = item.PolicyImageNum
                    .Remark = item.Remark
                    .RiskCharacteristicTypeId = item.RiskCharacteristicTypeId
                    .RiskFactor = item.RiskFactor
                    .ScheduleRatingTypeId = item.ScheduleRatingTypeId
                End With
                dualIrpmList.Add(dualIrpmItem)
            End If
            If item.ScheduleRatingTypeId = "2" Then
                For Each irpm In dualIrpmList
                    If irpm.RiskCharacteristicTypeId = item.RiskCharacteristicTypeId Then
                        irpm.RightColumnValue = item.RiskFactor
                    End If
                Next
            End If
        Next
        Return dualIrpmList
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'DataBind()
            'Populate() 'removed 12/2/2020 (Interoperability) since it should already be called by a parent control
            'Me.FindChildVrControls()
        End If

    End Sub

    Public Overrides Function Save() As Boolean

        'Updated 04/09/2021 for CAP Endorsements Task 52975 MLW
        'If Me.Visible Then
        If Me.Visible AndAlso Not (IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto) Then
            Dim qqScheduledRating = SubQuoteFirst.ScheduledRatings

            If qqScheduledRating Is Nothing OrElse qqScheduledRating.Count = 0 Then 'added 12/2/2020 (Interoperability)
                Dim qqXml As New QuickQuote.CommonMethods.QuickQuoteXML
                qqXml.QuoteObjectConversion_PolicyScheduledRatings(Me.SubQuoteFirst, qqScheduledRating, useConvertedFlag:=False)
            End If

            If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                qqScheduledRating.RemoveAll(Function(rating) rating.ScheduleRatingTypeId = "2")
            End If

            ' Adding the same filter for CAP above to CGL
            If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Then
                qqScheduledRating.RemoveAll(Function(rating) rating.ScheduleRatingTypeId = "5")
            End If

            If CAP.CAPScheduledCredit.IsCAPScheduledCreditAvailable(Quote) Then
                For Each ri As RepeaterItem In rptIrpmDual.Items
                    Dim txtRisk As TextBox = CType(ri.FindControl("txtRisk"), TextBox)
                    Dim txtRiskDual As TextBox = CType(ri.FindControl("txtRiskDual"), TextBox)
                    Dim txtIRPMDescription As TextBox = CType(ri.FindControl("txtIRPMDescription"), TextBox)
                    Dim lblRatingDescription As Label = CType(ri.FindControl("lblQuestionText"), Label)
                    Dim lblRiskCharacteristicTypeId As HiddenField = CType(ri.FindControl("hdnRiskCharacteristicTypeId"), HiddenField)
                    Dim qqItem As QuickQuote.CommonObjects.QuickQuoteScheduledRating = New QuickQuote.CommonObjects.QuickQuoteScheduledRating

                    If qqItem IsNot Nothing Then
                        qqItem.ScheduleRatingTypeId = "1"
                        qqItem.RiskFactor = VRToDiamondConversion(txtRisk.Text)
                        qqItem.Remark = txtIRPMDescription.Text
                        qqItem.Description = lblRatingDescription.Text
                        qqItem.RiskCharacteristicTypeId = lblRiskCharacteristicTypeId.Value
                        ParseScheduledRatingItem(qqItem)
                        qqItem.ScheduleRatingTypeId = "2"
                        qqItem.RiskFactor = VRToDiamondConversion(txtRiskDual.Text)
                        ParseScheduledRatingItem(qqItem)
                    End If
                Next
            Else
                If qqScheduledRating IsNot Nothing Then 'added IF 12/2/2020 (Interoperability)
                    For Each ri As RepeaterItem In rptIRPM.Items
                        Dim txtRisk As TextBox = CType(ri.FindControl("txtRisk"), TextBox)
                        Dim txtIRPMDescription As TextBox = CType(ri.FindControl("txtIRPMDescription"), TextBox)
                        Dim itemIndex = ri.ItemIndex
                        If qqScheduledRating.Count > itemIndex Then 'added IF 12/2/2020 (Interoperability)
                            Dim qqItem = qqScheduledRating(itemIndex)

                            If qqItem IsNot Nothing Then 'added IF 12/2/2020 (Interoperability)
                                qqItem.RiskFactor = VRToDiamondConversion(txtRisk.Text)
                                qqItem.Remark = txtIRPMDescription.Text

                                ParseScheduledRatingItem(qqItem)
                            End If
                        End If
                    Next
                End If
            End If
        End If

        Return True

    End Function

    Private Sub Save_Click(sender As Object, e As EventArgs) Handles btnSubmitRate.Click, btnSubmitRateDual.Click


        If sender Is btnSubmitRate OrElse sender Is btnSubmitRateDual Then
            If Me.ValidationSummmary.HasErrors = False Then
                Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
                'Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "0")
            End If
        Else
            Save_FireSaveEvent()
        End If
    End Sub

    Private Sub Cancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click, btnCancelDual.Click
        Populate()
        'Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "0")

        Dim workFlowManager = Me.FindFirstVRParentOfType(Of VRMasterControlBase)
        If workFlowManager.IsNotNull Then
            workFlowManager.ReturnToPriorWorkflow()
        End If
    End Sub


    Public Shared Function DiamondToVRConversion(diamondValue As String) As String
        If diamondValue Is Nothing Then
            Return 0.ToString()
        End If
        Dim diamondToVRDecimal As Decimal = Decimal.Parse(diamondValue) - 1
        Dim vrValue As Integer

        vrValue = CInt(diamondToVRDecimal * 100)

        Return vrValue.ToString()
    End Function

    Public Shared Function DiamondToVRConversionDecimal(diamondValue As String) As String
        If diamondValue Is Nothing Then
            Return 0.ToString()
        End If
        Dim diamondToVRDecimal As Decimal = Decimal.Parse(diamondValue) - 1
        Dim vrValue As Decimal

        vrValue = Math.Round(diamondToVRDecimal * 100, 1)

        Return vrValue.ToString()
    End Function

    Private Function VRToDiamondConversion(vrValue As String) As String
        If vrValue Is Nothing Then
            Return 1.ToString()
        End If
        Dim vrValueDecimal As Decimal = Decimal.Parse(vrValue) / 100
        Dim diamondValue As Decimal

        diamondValue = vrValueDecimal + 1

        Return diamondValue.ToString()
    End Function

    ''' <summary>
    ''' used to parse thru scheduled ratings and set different properties (for IRPM)
    ''' </summary>
    ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
    Private Sub ParseScheduledRatingItem(Item As QuickQuote.CommonObjects.QuickQuoteScheduledRating)
        If Item IsNot Nothing Then
            For Each stateQuote As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                Select Case Item.ScheduleRatingTypeId
                    Case "4" 'IRPM
                        If Item.RiskCharacteristicTypeId <> "" AndAlso IsNumeric(Item.RiskCharacteristicTypeId) AndAlso CInt(Item.RiskCharacteristicTypeId) > 0 Then
                            Select Case Item.RiskCharacteristicTypeId
                                Case "14" 'Management/Cooperation
                                    stateQuote.IRPM_ManagementCooperation = Item.RiskFactor
                                    stateQuote.IRPM_ManagementCooperationDesc = Item.Remark
                                Case "1" 'Location
                                    stateQuote.IRPM_Location = Item.RiskFactor
                                    stateQuote.IRPM_LocationDesc = Item.Remark
                                Case "9" 'Building Features
                                    stateQuote.IRPM_BuildingFeatures = Item.RiskFactor
                                    stateQuote.IRPM_BuildingFeaturesDesc = Item.Remark
                                Case "2" 'Premises
                                    stateQuote.IRPM_Premises = Item.RiskFactor
                                    stateQuote.IRPM_PremisesDesc = Item.Remark
                                Case "4" 'Employees
                                    stateQuote.IRPM_Employees = Item.RiskFactor
                                    stateQuote.IRPM_EmployeesDesc = Item.Remark
                                Case "12" 'Protection
                                    stateQuote.IRPM_Protection = Item.RiskFactor
                                    stateQuote.IRPM_ProtectionDesc = Item.Remark
                                Case "15" 'Catastrophic Hazards
                                    stateQuote.IRPM_CatostrophicHazards = Item.RiskFactor
                                    stateQuote.IRPM_CatostrophicHazardsDesc = Item.Remark
                                Case "16" 'Management Experience
                                    stateQuote.IRPM_ManagementExperience = Item.RiskFactor
                                    stateQuote.IRPM_ManagementExperienceDesc = Item.Remark
                                Case "3" 'Equipment
                                    stateQuote.IRPM_Equipment = Item.RiskFactor
                                    stateQuote.IRPM_EquipmentDesc = Item.Remark
                                Case "19" 'Medical Facilities
                                    stateQuote.IRPM_MedicalFacilities = Item.RiskFactor
                                    stateQuote.IRPM_MedicalFacilitiesDesc = Item.Remark
                                Case "17" 'Classification Peculiarities
                                    stateQuote.IRPM_ClassificationPeculiarities = Item.RiskFactor
                                    stateQuote.IRPM_ClassificationPeculiaritiesDesc = Item.Remark
                                    'added 10/17/2012 for CPR IRPM
                                Case "5" 'Management
                                    stateQuote.IRPM_CPR_Management = Item.RiskFactor
                                    stateQuote.IRPM_CPR_ManagementDesc = Item.Remark
                                Case "24" 'Premises and Equipment
                                    stateQuote.IRPM_CPR_PremisesAndEquipment = Item.RiskFactor
                                    stateQuote.IRPM_CPR_PremisesAndEquipmentDesc = Item.Remark
                            End Select
                        Else 'use description
                            Select Case UCase(Item.Description)
                                Case "MANAGEMENT/COOPERATION"
                                    stateQuote.IRPM_ManagementCooperation = Item.RiskFactor
                                    stateQuote.IRPM_ManagementCooperationDesc = Item.Remark
                                Case "LOCATION"
                                    stateQuote.IRPM_Location = Item.RiskFactor
                                    stateQuote.IRPM_LocationDesc = Item.Remark
                                Case "BUILDING FEATURES"
                                    stateQuote.IRPM_BuildingFeatures = Item.RiskFactor
                                    stateQuote.IRPM_BuildingFeaturesDesc = Item.Remark
                                Case "PREMISES"
                                    stateQuote.IRPM_Premises = Item.RiskFactor
                                    stateQuote.IRPM_PremisesDesc = Item.Remark
                                Case "EMPLOYEES"
                                    stateQuote.IRPM_Employees = Item.RiskFactor
                                    stateQuote.IRPM_EmployeesDesc = Item.Remark
                                Case "PROTECTION"
                                    stateQuote.IRPM_Protection = Item.RiskFactor
                                    stateQuote.IRPM_ProtectionDesc = Item.Remark
                                Case "CATASTROPHIC HAZARDS"
                                    stateQuote.IRPM_CatostrophicHazards = Item.RiskFactor
                                    stateQuote.IRPM_CatostrophicHazardsDesc = Item.Remark
                                Case "MANAGEMENT EXPERIENCE"
                                    stateQuote.IRPM_ManagementExperience = Item.RiskFactor
                                    stateQuote.IRPM_ManagementExperienceDesc = Item.Remark
                                Case "EQUIPMENT"
                                    stateQuote.IRPM_Equipment = Item.RiskFactor
                                    stateQuote.IRPM_EquipmentDesc = Item.Remark
                                Case "MEDICAL FACILITIES"
                                    stateQuote.IRPM_MedicalFacilities = Item.RiskFactor
                                    stateQuote.IRPM_MedicalFacilitiesDesc = Item.Remark
                                Case "CLASSIFICATION PECULIARITIES"
                                    stateQuote.IRPM_ClassificationPeculiarities = Item.RiskFactor
                                    stateQuote.IRPM_ClassificationPeculiaritiesDesc = Item.Remark
                                    'added 10/17/2012 for CPR IRPM
                                Case "MANAGEMENT"
                                    stateQuote.IRPM_CPR_Management = Item.RiskFactor
                                    stateQuote.IRPM_CPR_ManagementDesc = Item.Remark
                                Case "PREMISES AND EQUIPMENT"
                                    stateQuote.IRPM_CPR_PremisesAndEquipment = Item.RiskFactor
                                    stateQuote.IRPM_CPR_PremisesAndEquipmentDesc = Item.Remark
                            End Select
                        End If
                    Case "5" 'Premises
                        If Item.RiskCharacteristicTypeId <> "" AndAlso IsNumeric(Item.RiskCharacteristicTypeId) AndAlso CInt(Item.RiskCharacteristicTypeId) > 0 Then
                            Select Case Item.RiskCharacteristicTypeId
                                Case "14" 'Management/Cooperation
                                    'If stateQuote.IRPM_GL_ManagementCooperation = "" Then
                                    stateQuote.IRPM_GL_ManagementCooperation = Item.RiskFactor
                                    stateQuote.IRPM_GL_ManagementCooperationDesc = Item.Remark
                                    'End If
                                Case "1" 'Location
                                    'If stateQuote.IRPM_GL_Location = "" Then
                                    stateQuote.IRPM_GL_Location = Item.RiskFactor
                                    stateQuote.IRPM_GL_LocationDesc = Item.Remark
                                    'End If
                                Case "2" 'Premises
                                    'If stateQuote.IRPM_GL_Premises = "" Then
                                    stateQuote.IRPM_GL_Premises = Item.RiskFactor
                                    stateQuote.IRPM_GL_PremisesDesc = Item.Remark
                                    'End If
                                Case "4" 'Employees
                                    'If stateQuote.IRPM_GL_Employees = "" Then
                                    stateQuote.IRPM_GL_Employees = Item.RiskFactor
                                    stateQuote.IRPM_GL_EmployeesDesc = Item.Remark
                                    'End If
                                Case "3" 'Equipment
                                    'If stateQuote.IRPM_GL_Equipment = "" Then
                                    stateQuote.IRPM_GL_Equipment = Item.RiskFactor
                                    stateQuote.IRPM_GL_EquipmentDesc = Item.Remark
                                    'End If
                                Case "17" 'Classification Peculiarities
                                    'If stateQuote.IRPM_GL_ClassificationPeculiarities = "" Then
                                    stateQuote.IRPM_GL_ClassificationPeculiarities = Item.RiskFactor
                                    stateQuote.IRPM_GL_ClassificationPeculiaritiesDesc = Item.Remark
                                    'End If
                            End Select
                        Else 'use description
                            Select Case UCase(Item.Description)
                                Case "MANAGEMENT/COOPERATION"
                                    'If stateQuote.IRPM_GL_ManagementCooperation = "" Then
                                    stateQuote.IRPM_GL_ManagementCooperation = Item.RiskFactor
                                    stateQuote.IRPM_GL_ManagementCooperationDesc = Item.Remark
                                    'End If
                                Case "LOCATION"
                                    'If stateQuote.IRPM_GL_Location = "" Then
                                    stateQuote.IRPM_GL_Location = Item.RiskFactor
                                    stateQuote.IRPM_GL_LocationDesc = Item.Remark
                                    'End If
                                Case "PREMISES"
                                    'If stateQuote.IRPM_GL_Premises = "" Then
                                    stateQuote.IRPM_GL_Premises = Item.RiskFactor
                                    stateQuote.IRPM_GL_PremisesDesc = Item.Remark
                                    'End If
                                Case "EMPLOYEES"
                                    'If stateQuote.IRPM_GL_Employees = "" Then
                                    stateQuote.IRPM_GL_Employees = Item.RiskFactor
                                    stateQuote.IRPM_GL_EmployeesDesc = Item.Remark
                                    'End If
                                Case "EQUIPMENT"
                                    'If stateQuote.IRPM_GL_Equipment = "" Then
                                    stateQuote.IRPM_GL_Equipment = Item.RiskFactor
                                    stateQuote.IRPM_GL_EquipmentDesc = Item.Remark
                                    'End If
                                Case "CLASSIFICATION PECULIARITIES"
                                    'If stateQuote.IRPM_GL_ClassificationPeculiarities = "" Then
                                    stateQuote.IRPM_GL_ClassificationPeculiarities = Item.RiskFactor
                                    stateQuote.IRPM_GL_ClassificationPeculiaritiesDesc = Item.Remark
                                    'End If
                            End Select
                        End If
                    Case "6" 'Products
                        If Item.RiskCharacteristicTypeId <> "" AndAlso IsNumeric(Item.RiskCharacteristicTypeId) AndAlso CInt(Item.RiskCharacteristicTypeId) > 0 Then
                            Select Case Item.RiskCharacteristicTypeId
                                Case "14" 'Management/Cooperation
                                    'If stateQuote.IRPM_GL_ManagementCooperation = "" Then
                                    stateQuote.IRPM_GL_ManagementCooperation = Item.RiskFactor
                                    stateQuote.IRPM_GL_ManagementCooperationDesc = Item.Remark
                                    'End If
                                Case "1" 'Location
                                    'If stateQuote.IRPM_GL_Location = "" Then
                                    stateQuote.IRPM_GL_Location = Item.RiskFactor
                                    stateQuote.IRPM_GL_LocationDesc = Item.Remark
                                    'End If
                                Case "2" 'Premises
                                    'If stateQuote.IRPM_GL_Premises = "" Then
                                    stateQuote.IRPM_GL_Premises = Item.RiskFactor
                                    stateQuote.IRPM_GL_PremisesDesc = Item.Remark
                                    'End If
                                Case "4" 'Employees
                                    'If stateQuote.IRPM_GL_Employees = "" Then
                                    stateQuote.IRPM_GL_Employees = Item.RiskFactor
                                    stateQuote.IRPM_GL_EmployeesDesc = Item.Remark
                                    'End If
                                Case "3" 'Equipment
                                    'If stateQuote.IRPM_GL_Equipment = "" Then
                                    stateQuote.IRPM_GL_Equipment = Item.RiskFactor
                                    stateQuote.IRPM_GL_EquipmentDesc = Item.Remark
                                    'End If
                                Case "17" 'Classification Peculiarities
                                    'If stateQuote.IRPM_GL_ClassificationPeculiarities = "" Then
                                    stateQuote.IRPM_GL_ClassificationPeculiarities = Item.RiskFactor
                                    stateQuote.IRPM_GL_ClassificationPeculiaritiesDesc = Item.Remark
                                    'End If
                            End Select
                        Else 'use description
                            Select Case UCase(Item.Description)
                                Case "MANAGEMENT/COOPERATION"
                                    'If stateQuote.IRPM_GL_ManagementCooperation = "" Then
                                    stateQuote.IRPM_GL_ManagementCooperation = Item.RiskFactor
                                    stateQuote.IRPM_GL_ManagementCooperationDesc = Item.Remark
                                    'End If
                                Case "LOCATION"
                                    'If stateQuote.IRPM_GL_Location = "" Then
                                    stateQuote.IRPM_GL_Location = Item.RiskFactor
                                    stateQuote.IRPM_GL_LocationDesc = Item.Remark
                                    'End If
                                Case "PREMISES"
                                    'If stateQuote.IRPM_GL_Premises = "" Then
                                    stateQuote.IRPM_GL_Premises = Item.RiskFactor
                                    stateQuote.IRPM_GL_PremisesDesc = Item.Remark
                                    'End If
                                Case "EMPLOYEES"
                                    'If stateQuote.IRPM_GL_Employees = "" Then
                                    stateQuote.IRPM_GL_Employees = Item.RiskFactor
                                    stateQuote.IRPM_GL_EmployeesDesc = Item.Remark
                                    'End If
                                Case "EQUIPMENT"
                                    'If stateQuote.IRPM_GL_Equipment = "" Then
                                    stateQuote.IRPM_GL_Equipment = Item.RiskFactor
                                    stateQuote.IRPM_GL_EquipmentDesc = Item.Remark
                                    'End If
                                Case "CLASSIFICATION PECULIARITIES"
                                    'If stateQuote.IRPM_GL_ClassificationPeculiarities = "" Then
                                    stateQuote.IRPM_GL_ClassificationPeculiarities = Item.RiskFactor
                                    stateQuote.IRPM_GL_ClassificationPeculiaritiesDesc = Item.Remark
                                    'End If
                            End Select
                        End If
                    Case "1" 'Liability (added logic 10/3/2012 for CAP IRPM)
                        If Item.RiskCharacteristicTypeId <> "" AndAlso IsNumeric(Item.RiskCharacteristicTypeId) AndAlso CInt(Item.RiskCharacteristicTypeId) > 0 Then
                            Select Case Item.RiskCharacteristicTypeId
                                Case "5" 'Management
                                    stateQuote.IRPM_CAP_Management = Item.RiskFactor
                                    stateQuote.IRPM_CAP_ManagementDesc = Item.Remark
                                Case "4" 'Employees
                                    stateQuote.IRPM_CAP_Employees = Item.RiskFactor
                                    stateQuote.IRPM_CAP_EmployeesDesc = Item.Remark
                                Case "3" 'Equipment
                                    stateQuote.IRPM_CAP_Equipment = Item.RiskFactor
                                    stateQuote.IRPM_CAP_EquipmentDesc = Item.Remark
                                Case "13" 'Safety Organization
                                    stateQuote.IRPM_CAP_SafetyOrganization = Item.RiskFactor
                                    stateQuote.IRPM_CAP_SafetyOrganizationDesc = Item.Remark
                            End Select
                        Else 'use description
                            Select Case UCase(Item.Description)
                                Case "MANAGEMENT"
                                    stateQuote.IRPM_CAP_Management = Item.RiskFactor
                                    stateQuote.IRPM_CAP_ManagementDesc = Item.Remark
                                Case "EMPLOYEES"
                                    stateQuote.IRPM_CAP_Employees = Item.RiskFactor
                                    stateQuote.IRPM_CAP_EmployeesDesc = Item.Remark
                                Case "EQUIPMENT"
                                    stateQuote.IRPM_CAP_Equipment = Item.RiskFactor
                                    stateQuote.IRPM_CAP_EquipmentDesc = Item.Remark
                                Case "SAFETY ORGANIZATION"
                                    stateQuote.IRPM_CAP_SafetyOrganization = Item.RiskFactor
                                    stateQuote.IRPM_CAP_SafetyOrganizationDesc = Item.Remark
                            End Select
                        End If
                    Case "2" 'Physical Damage (added logic 10/3/2012 for CAP IRPM)
                        If Item.RiskCharacteristicTypeId <> "" AndAlso IsNumeric(Item.RiskCharacteristicTypeId) AndAlso CInt(Item.RiskCharacteristicTypeId) > 0 Then
                            Select Case Item.RiskCharacteristicTypeId
                                Case "5" 'Management
                                    stateQuote.IRPM_CAP_Management_Phys_Damage = Item.RiskFactor
                                    stateQuote.IRPM_CAP_ManagementDesc_Phys_Damage = Item.Remark
                                Case "4" 'Employees
                                    stateQuote.IRPM_CAP_Employees_Phys_Damage = Item.RiskFactor
                                    stateQuote.IRPM_CAP_EmployeesDesc_Phys_Damage = Item.Remark
                                Case "3" 'Equipment
                                    stateQuote.IRPM_CAP_Equipment_Phys_Damage = Item.RiskFactor
                                    stateQuote.IRPM_CAP_EquipmentDesc_Phys_Damage = Item.Remark
                                Case "13" 'Safety Organization
                                    stateQuote.IRPM_CAP_SafetyOrganization_Phys_Damage = Item.RiskFactor
                                    stateQuote.IRPM_CAP_SafetyOrganizationDesc_Phys_Damage = Item.Remark
                            End Select
                        Else 'use description
                            Select Case UCase(Item.Description)
                                Case "MANAGEMENT"
                                    stateQuote.IRPM_CAP_Management_Phys_Damage = Item.RiskFactor
                                    stateQuote.IRPM_CAP_ManagementDesc_Phys_Damage = Item.Remark
                                Case "EMPLOYEES"
                                    stateQuote.IRPM_CAP_Employees_Phys_Damage = Item.RiskFactor
                                    stateQuote.IRPM_CAP_EmployeesDesc_Phys_Damage = Item.Remark
                                Case "EQUIPMENT"
                                    stateQuote.IRPM_CAP_Equipment_Phys_Damage = Item.RiskFactor
                                    stateQuote.IRPM_CAP_EquipmentDesc_Phys_Damage = Item.Remark
                                Case "SAFETY ORGANIZATION"
                                    stateQuote.IRPM_CAP_SafetyOrganization_Phys_Damage = Item.RiskFactor
                                    stateQuote.IRPM_CAP_SafetyOrganizationDesc_Phys_Damage = Item.Remark
                            End Select
                        End If
                End Select
            Next
        End If

        Exit Sub
    End Sub

    Public Function GetTotalMax() As String
        If Quote IsNot Nothing AndAlso String.IsNullOrEmpty(Me.Quote.LobType.ToString) = False Then
            Return IRPMData.GetIRPMTotalMax(Me.Quote, Quote.LobType)
        Else
            Return "0"
        End If
    End Function

    Public Function GetTotalMin() As String
        If Quote IsNot Nothing AndAlso String.IsNullOrEmpty(Me.Quote.LobType.ToString) = False Then
            Return IRPMData.GetIRPMTotalMin(Me.Quote, Quote.LobType)
        Else
            Return "0"
        End If
    End Function

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Save_FireSaveEvent()
    End Sub

    Public Class DualDisplayIrpm
        Inherits QuickQuote.CommonObjects.QuickQuoteScheduledRating
        Public Property RightColumnValue As String
    End Class
End Class