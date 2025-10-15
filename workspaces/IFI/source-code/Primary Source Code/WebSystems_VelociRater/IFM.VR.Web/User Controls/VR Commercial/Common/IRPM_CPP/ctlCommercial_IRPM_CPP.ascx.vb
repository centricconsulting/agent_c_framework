Imports IFM.VR.Common.IRPMData
Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Imports Diamond.Business.ThirdParty.ISO.ISO360
Imports QuickQuote.CommonMethods

Public Class ctlCommercial_IRPM_CPP
    Inherits VRControlBase


    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.Quote IsNot Nothing Then
            Dim LOB As String = Me.Quote.LobType.ToString
            'Me.VRScript.AddVariableLine("var ctlCommercial_IRPM_LOB='" + IFM.VR.Common.Helpers.LobHelper.GetAbbreviatedLOBPrefix(LOB) + "';")
            Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID, False)

            Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
            _script.AddScriptLine("$(""#IRPMDiv"").accordion({collapsible: false, heightStyle: ""content""});")
            _script.AddScriptLine("$("".IRPMSection"").accordion({collapsible: false, heightStyle: ""content""});")
            _script.AddScriptLine("$("".IRPMSection_GL"").accordion({collapsible: false, heightStyle: ""content""});")
            _script.AddScriptLine("ifm.vr.ui.ScrollToWindowTop();")

            Me.VRScript.CreateJSBinding(Me.btnSubmitRate.ClientID, "mousedown", "$(this).focus(); return ValidateForm();")

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
        DataBind()
        'Dim AllRatings = Me.Quote.ScheduledRatings
        'updated 8/17/2018
        ' Force reload of quote to fix the issue where all of the scheduled ratings are not loaded after rate Bug 33836 
        ' IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(QuoteId)

        Dim AllRatings = Me.SubQuoteFirst.ScheduledRatings

        Dim Ratings = New List(Of QuickQuoteScheduledRating)
        Dim Ratings_GL = New List(Of QuickQuoteScheduledRating)

        'added 10/29/2020 (Interoperability)
        'Dim reloadQuote As Boolean = False'removed 12/2/2020 to do this a different way
        Dim qqXml As QuickQuote.CommonMethods.QuickQuoteXML = Nothing

        ' Filter out all but 4's (property)
        'If reloadQuote = False Then 'added IF 10/29/2020 (Interoperability) 'removed IF 12/2/2020 to do this a different way
        If AllRatings IsNot Nothing AndAlso AllRatings.Count > 0 Then 'added IF 12/2/2020 (Interoperability)
            Ratings = (From s In AllRatings Where s.ScheduleRatingTypeId = "4").ToList()
        End If
        If Ratings Is Nothing OrElse Ratings.Count = 0 Then 'added 10/29/2020 (Interoperability)
            'reloadQuote = True 'removed 12/2/2020 to do this a different way
            'added 12/2/2020
            If qqXml Is Nothing Then
                qqXml = New QuickQuote.CommonMethods.QuickQuoteXML
            End If
            qqXml.QuoteObjectConversion_PolicyScheduledRatings(Me.SubQuoteFirst, Ratings, packagePartType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty, useConvertedFlag:=False)
        End If
        'End If

        ' Filter out all but 6's (GL)
        'If reloadQuote = False Then 'added IF 10/29/2020 (Interoperability) 'removed IF 12/2/2020 to do this a different way
        If AllRatings IsNot Nothing AndAlso AllRatings.Count > 0 Then 'added IF 12/2/2020 (Interoperability)
            Ratings_GL = (From s In AllRatings Where s.ScheduleRatingTypeId = "6").ToList()
        End If
        If Ratings_GL Is Nothing OrElse Ratings_GL.Count = 0 Then 'added 10/29/2020 (Interoperability)
            'reloadQuote = True 'removed 12/2/2020 to do this a different way
            'added 12/2/2020
            If qqXml Is Nothing Then
                qqXml = New QuickQuote.CommonMethods.QuickQuoteXML
            End If
            'note: CGL has duplicates for ScheduledRatingTypeId 5 and 6, but we only need to load one
            Dim allLiabilityRatings As List(Of QuickQuoteScheduledRating) = Nothing
            qqXml.QuoteObjectConversion_PolicyScheduledRatings(Me.SubQuoteFirst, allLiabilityRatings, packagePartType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability, useConvertedFlag:=False)
            If allLiabilityRatings IsNot Nothing AndAlso allLiabilityRatings.Count > 0 Then
                Ratings_GL = (From s In allLiabilityRatings Where s.ScheduleRatingTypeId = "6").ToList()
            End If
        End If
        'End If

        'added 10/29/2020 (Interoperability)
        'If reloadQuote = True Then 'removed 12/2/2020 to do this a different way
        '    IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(QuoteId)

        '    AllRatings = Me.SubQuoteFirst.ScheduledRatings

        '    ' Filter out all but 4's (property)
        '    Ratings = (From s In AllRatings Where s.ScheduleRatingTypeId = "4").ToList()

        '    ' Filter out all but 6's (GL)
        '    Ratings_GL = (From s In AllRatings Where s.ScheduleRatingTypeId = "6").ToList()
        'End If

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
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                Me.lblTitle.Text = "Credits/Debits"
        End Select

        Ratings = IFM.VR.Common.IRPMData.IRPMData.GetIRPMData(Ratings, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty)

        Me.rptIRPM.DataSource = Ratings
        Me.rptIRPM.DataBind()

        Ratings_GL = IFM.VR.Common.IRPMData.IRPMData.GetIRPMData(Ratings_GL, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability)

        Me.rptIRPM_GL.DataSource = Ratings_GL
        Me.rptIRPM_GL.DataBind()

        If IsOnAppPage _
               AndAlso Helpers.NewCo_Prefill_Helper.ShouldReturnToQuoteForPreFillOrNewCo(Me.Quote, Me.DefaultValidationType, Me.QuoteIdOrPolicyIdPipeImageNumber) Then
            VRScript.FakeDisableSingleElement(tblMain)
            VRScript.FakeDisabledSingleElement_ReEnable(btnCancel)
            VRScript.FakeDisabledSingleElement_ReEnable(btnEmailForUWAssistance)
            btnSubmitRate.Visible = False
        End If

        Dim irpmTotalCredit As String = ""
        If IsQuoteEndorsement() = False AndAlso IsNewCo() Then
            irpmTotalCredit = "15%"
        Else
            irpmTotalCredit = "25%"
        End If

        Me.pnIRPMMessage.Text = "Please note, the total credit cannot exceed " + irpmTotalCredit + " per line of business, total credits greater than " + irpmTotalCredit + "  per line of business will require Underwriting approval."

        'End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'Populate() 'removed 12/2/2020 (Interoperability) since it should already be called by a parent control
        End If

    End Sub

    Public Overrides Function Save() As Boolean

        If Me.Visible Then

            Dim AllRatings = SubQuoteFirst.ScheduledRatings
            Dim qqScheduledRating = New List(Of QuickQuoteScheduledRating)
            Dim qqScheduledRating_GL = New List(Of QuickQuoteScheduledRating)

            Dim qqXml As QuickQuote.CommonMethods.QuickQuoteXML = Nothing 'added 12/2/2020 (Interoperability)

            ' Filter out all but 4's (property)
            If AllRatings IsNot Nothing AndAlso AllRatings.Count > 0 Then 'added IF 12/2/2020 (Interoperability)
                qqScheduledRating = (From s In AllRatings Where s.ScheduleRatingTypeId = "4").ToList()
            End If
            If qqScheduledRating Is Nothing OrElse qqScheduledRating.Count = 0 Then 'added 12/2/2020 (Interoperability)
                If qqXml Is Nothing Then
                    qqXml = New QuickQuote.CommonMethods.QuickQuoteXML
                End If
                qqXml.QuoteObjectConversion_PolicyScheduledRatings(Me.SubQuoteFirst, qqScheduledRating, packagePartType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty, useConvertedFlag:=False)
            End If

            ' Filter out all but 6's (GL)
            If AllRatings IsNot Nothing AndAlso AllRatings.Count > 0 Then 'added IF 12/2/2020 (Interoperability)
                qqScheduledRating_GL = (From s In AllRatings Where s.ScheduleRatingTypeId = "6").ToList()
            End If
            If qqScheduledRating_GL Is Nothing OrElse qqScheduledRating_GL.Count = 0 Then 'added 12/2/2020 (Interoperability)
                If qqXml Is Nothing Then
                    qqXml = New QuickQuote.CommonMethods.QuickQuoteXML
                End If
                'note: CGL has duplicates for ScheduledRatingTypeId 5 and 6, but we only need to load one
                Dim allLiabilityRatings As List(Of QuickQuoteScheduledRating) = Nothing
                qqXml.QuoteObjectConversion_PolicyScheduledRatings(Me.SubQuoteFirst, allLiabilityRatings, packagePartType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability, useConvertedFlag:=False)
                If allLiabilityRatings IsNot Nothing AndAlso allLiabilityRatings.Count > 0 Then
                    qqScheduledRating_GL = (From s In allLiabilityRatings Where s.ScheduleRatingTypeId = "6").ToList()
                End If
            End If

            If qqScheduledRating IsNot Nothing Then 'added IF 11/30/2020 (Interoperability)
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

            If qqScheduledRating_GL IsNot Nothing Then 'added IF 11/30/2020 (Interoperability)
                For Each ri As RepeaterItem In rptIRPM_GL.Items
                    Dim txtRisk As TextBox = CType(ri.FindControl("txtRisk"), TextBox)
                    Dim txtIRPMDescription As TextBox = CType(ri.FindControl("txtIRPMDescription"), TextBox)
                    Dim itemIndex = ri.ItemIndex
                    If qqScheduledRating_GL.Count > itemIndex Then 'added IF 12/2/2020 (Interoperability)
                        Dim qqItem = qqScheduledRating_GL(itemIndex)

                        If qqItem IsNot Nothing Then 'added IF 12/2/2020 (Interoperability)
                            qqItem.RiskFactor = VRToDiamondConversion(txtRisk.Text)
                            qqItem.Remark = txtIRPMDescription.Text

                            ParseScheduledRatingItem(qqItem)
                        End If
                    End If
                Next
            End If
        End If

        Return True

    End Function

    Private Sub Save_Click(sender As Object, e As EventArgs) Handles btnSubmitRate.Click, lnkSave.Click
        Me.Save_FireSaveEvent()

        If sender Is btnSubmitRate Then
            If Me.ValidationSummmary.HasErrors = False Then
                Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
            End If
        End If
    End Sub

    Private Sub Cancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Populate()
        'Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "0")

        Dim workFlowManager = Me.FindFirstVRParentOfType(Of VRMasterControlBase)
        If workFlowManager.IsNotNull Then
            workFlowManager.ReturnToPriorWorkflow()
        End If
    End Sub


    Public Shared Function DiamondToVRConversion(diamondValue As String) As String
        If String.IsNullOrWhiteSpace(diamondValue) = True OrElse IsNumeric(diamondValue) = False Then 'added 3/15/2021 to handle for Nothing or EmptyString; happens at Populate.DataBind whenever the previously bound data reference is no longer available (i.e. at Populate_FirePopulateEvent() call from insuredList on state change)
            diamondValue = "1.000"
        End If
        Dim diamondToVRDecimal As Decimal = Decimal.Parse(diamondValue) - 1
        Dim vrValue As Integer

        vrValue = CInt(diamondToVRDecimal * 100)

        Return vrValue.ToString()
    End Function

    Private Function VRToDiamondConversion(vrValue As String) As String
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
        If Quote IsNot Nothing AndAlso SubQuotes IsNot Nothing Then
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
                    End Select
                Next
            End If
        End If
    End Sub

    Public Function GetTotalMax(Optional LOB As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty) As String
        Dim indexLOB As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType
        Select Case LOB
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                indexLOB = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
            Case Else
                indexLOB = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
        End Select
        Return IRPMData.GetIRPMTotalMax(Quote, indexLOB)
    End Function

    Public Function GetTotalMin(Optional LOB As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty) As String
        Dim indexLOB As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType
        Select Case LOB
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                indexLOB = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
            Case Else
                indexLOB = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
        End Select
        Return IRPMData.GetIRPMTotalMin(Quote, indexLOB)
    End Function

End Class