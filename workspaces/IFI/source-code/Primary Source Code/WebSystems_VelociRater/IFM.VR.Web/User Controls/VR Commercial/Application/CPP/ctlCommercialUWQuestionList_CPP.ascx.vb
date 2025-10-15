Imports System.Data.SqlClient
Imports IFM.PrimativeExtensions ' Added 4-30-17 Matt A
Imports IFM.VR.Common.UWQuestions
Imports QuickQuote.CommonObjects
Imports IFM.VR.Common.Helpers.CPP

Public Class ctlCommercialUWQuestionList_CPP
    Inherits VRControlBase

    Private msgLOBNotSupported As String = "LOB Not Supported!"

    Private programLookupCache As New Dictionary(Of String, String) ' Matt A 4-30-17



    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.Quote IsNot Nothing Then
            Me.VRScript.AddVariableLine($"var ctlCommercialUWQuestionList_LOB='{IFM.VR.Common.Helpers.LOBHelper.GetAbbreviatedLOBPrefix(Me.Quote.LobType)}';")
        End If

        Me.VRScript.AddScriptLine("$(""#UWQuestionsDiv"").accordion({collapsible: false, heightStyle: ""content""});")
        Me.VRScript.AddScriptLine("$("".UWQuestionsLOBSection"").accordion({collapsible: true, heightStyle: ""content""});")
        Me.VRScript.AddScriptLine("$("".UWQuestionsSection"").accordion({collapsible: true, heightStyle: ""content""});")

        Me.VRScript.AddScriptLine("$("".UWQuestionsLOBSection"").each(function () {if ($(this).hasClass(""CPR"")) {$(this).accordion('option', 'active', 0);} else {$(this).accordion('option', 'active', false)} });")

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Dim SectionsWithQuestions As New List(Of List(Of VR.Common.UWQuestions.VRUWQuestion))

        CIMButton.Visible = False
        CRMButton.Visible = False
        CRMButton2.Visible = False

        'added 8/9/2018 for multi-state; removed 8/11/2018 to use new Properties
        'Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
        'Dim quoteToUse As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        'If Me.Quote IsNot Nothing Then
        '    multiStateQuotes = QQHelper.MultiStateQuickQuoteObjects(Me.Quote) 'should always return at least Me.Quote in the list
        '    If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
        '        quoteToUse = multiStateQuotes.Item(0) 'will just use the 1st stateQuote found (or Me.Quote) since all questions are currently the same for all states
        '    End If
        'End If

#Region "Start CPR Processing"
        Dim CPRsections = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCPPUnderwritingQuestions_CPR()
                          Group question By question.SectionName
                                   Into Group
                          Select Group

        ' Populate sections with quote information
        'If Me.Quote IsNot Nothing And Me.Quote.PolicyUnderwritings IsNot Nothing Then
        'updated 8/9/2018; updated 8/11/2018 to use new Property
        If SubQuoteFirst IsNot Nothing And SubQuoteFirst.PolicyUnderwritings IsNot Nothing Then
            'For Each question In Me.Quote.PolicyUnderwritings
            'updated 8/9/2018
            For Each question In SubQuoteFirst.PolicyUnderwritings
                Dim codeID = question.PolicyUnderwritingCodeId
                Dim answer = question.PolicyUnderwritingAnswer
                Dim addInfo = question.PolicyUnderwritingExtraAnswer
                For Each section In CPRsections
                    For Each qID In section
                        If (qID.PolicyUnderwritingCodeId = codeID) Then
                            qID.PolicyUnderwritingExtraAnswer = ""
                            If (answer = 1) Then
                                qID.QuestionAnswerYes = True
                                qID.QuestionAnswerNo = False
                                qID.PolicyUnderwritingExtraAnswer = addInfo
                            ElseIf (answer = -1) Then
                                qID.QuestionAnswerNo = True
                                qID.QuestionAnswerYes = False
                            Else
                                qID.QuestionAnswerNo = False
                                qID.QuestionAnswerYes = False
                            End If
                        End If
                    Next
                Next
            Next
        End If

        Me.rptCPR.DataSource = CPRsections
        Me.rptCPR.DataBind()
#End Region

#Region "Start CGL Processing"

        ''' Contractor Section
        Dim DetermineShowContractorsSection = New Func(Of Boolean)(Function()
                                                                       ' Check Location Level
                                                                       For Each location As QuickQuote.CommonObjects.QuickQuoteLocation In Me.Quote.Locations ' If Locations is null just let the exception happen
                                                                           If location.GLClassifications.IsLoaded Then ' again don't
                                                                               For Each locationClass As QuickQuote.CommonObjects.QuickQuoteGLClassification In location.GLClassifications
                                                                                   If String.IsNullOrWhiteSpace(locationClass.ClassCode) = False AndAlso locationClass.ClassCode.Substring(0, 1) = "9" Then
                                                                                       Return True
                                                                                   End If
                                                                               Next
                                                                           End If
                                                                       Next
                                                                       'Check Policy Level
                                                                       '8/9/2018 note: backend will likely need to pull these up to the top quote level like is done for Locations; or all stateQuotes would need to be checked
                                                                       '8/21/2018 note: reverted code back since QQ library will now return all GLClassifications to the top level
                                                                       If Quote.GLClassifications.IsLoaded Then ' again don't
                                                                           For Each policyClass As QuickQuote.CommonObjects.QuickQuoteGLClassification In Quote.GLClassifications
                                                                               If String.IsNullOrWhiteSpace(policyClass.ClassCode) = False AndAlso policyClass.ClassCode.Substring(0, 1) = "9" Then
                                                                                   Return True
                                                                               End If
                                                                           Next
                                                                       End If
                                                                       'updated 8/9/2018 for multiState; now looking for classCode in any stateQuote; may need to be changed if we pull all up to top level; updated 8/11/2018 to use new Property
                                                                       'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                                                                       '    For Each msq As QuickQuoteObject In SubQuotes
                                                                       '        If msq.GLClassifications.IsLoaded Then ' again don't
                                                                       '            For Each policyClass As QuickQuote.CommonObjects.QuickQuoteGLClassification In msq.GLClassifications
                                                                       '                If String.IsNullOrWhiteSpace(policyClass.ClassCode) = False AndAlso policyClass.ClassCode.Substring(0, 1) = "9" Then
                                                                       '                    Return True
                                                                       '                End If
                                                                       '            Next
                                                                       '        End If
                                                                       '    Next
                                                                       'End If
                                                                       Return False
                                                                   End Function)
        Dim showContractorsSection As Boolean = DetermineShowContractorsSection()

        ''' Employee Benefits Section
        Dim DetermineShowEmployeeBenefitsSection = New Func(Of Boolean)(Function()
                                                                            'If IsNumeric(Quote.EmployeeBenefitsLiabilityText) Then
                                                                            '    Return True
                                                                            'End If
                                                                            'updated 8/9/2018 for multiState; now looking for any stateQuote to have EmployeeBenefits; updated 8/11/2018 to use new Property
                                                                            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                                                                                For Each msq As QuickQuoteObject In SubQuotes
                                                                                    If IsNumeric(msq.EmployeeBenefitsLiabilityText) Then
                                                                                        Return True
                                                                                    End If
                                                                                Next
                                                                            End If
                                                                            Return False
                                                                        End Function)
        Dim showEmployeeBenefitsSection As Boolean = DetermineShowEmployeeBenefitsSection()

        'group by section and possible hide questions
        Dim CGLsections = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCPPUnderwritingQuestions_CGL()
                          Where (showContractorsSection Or question.SectionName.ToUpper() <> "General Liability - Contractors".ToUpper()) And
                                       (showEmployeeBenefitsSection Or question.SectionName.ToUpper() <> "Company – Employee Benefits".ToUpper()) And
                                       (question.SectionName.ToUpper() <> "Applicant Information".ToUpper())
                          Group question By question.SectionName
                                   Into Group
                          Select Group

        'If Me.Quote IsNot Nothing And Me.Quote.PolicyUnderwritings IsNot Nothing Then
        'updated 8/9/2018; updated 8/11/2018 to use new Property
        If SubQuoteFirst IsNot Nothing And SubQuoteFirst.PolicyUnderwritings IsNot Nothing Then
            'For Each question In Me.Quote.PolicyUnderwritings
            'updated 8/9/2018
            For Each question In SubQuoteFirst.PolicyUnderwritings
                Dim codeID = question.PolicyUnderwritingCodeId
                Dim answer = question.PolicyUnderwritingAnswer
                Dim addInfo = question.PolicyUnderwritingExtraAnswer
                For Each section In CGLsections
                    For Each qID In section
                        If (qID.PolicyUnderwritingCodeId = codeID) Then
                            qID.PolicyUnderwritingExtraAnswer = ""
                            If (answer = 1) Then
                                qID.QuestionAnswerYes = True
                                qID.QuestionAnswerNo = False
                                qID.PolicyUnderwritingExtraAnswer = addInfo
                            ElseIf (answer = -1) Then
                                qID.QuestionAnswerNo = True
                                qID.QuestionAnswerYes = False
                            Else
                                qID.QuestionAnswerNo = False
                                qID.QuestionAnswerYes = False
                            End If
                        End If
                    Next
                Next
            Next
        End If

        Me.rptCGL.DataSource = CGLsections
        Me.rptCGL.DataBind()
#End Region

#Region "Start CIM Processing"
        If Quote IsNot Nothing Then

            ''' Equipment Floater
            Dim DetermineShowEquipmentFloatersSection = New Func(Of Boolean)(Function()
                                                                                 'If String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentLeasedRentedFromOthersLimit) = False AndAlso (Quote.ContractorsEquipmentScheduledCoverages IsNot Nothing AndAlso Quote.ContractorsEquipmentScheduledCoverages.Any()) Or String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentScheduleDeductibleId) = False Or String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentScheduleCoinsuranceTypeId) = False Or String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentLeasedRentedFromOthersLimit) = False Or String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit) = False Or String.IsNullOrWhiteSpace(Quote.SmallToolsAnyOneLossCatastropheLimit) = False Then
                                                                                 '    Return True
                                                                                 'End If
                                                                                 'updated 8/9/2018 for multi-state; updated 8/11/2018 to use new Property
                                                                                 If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                                                                                     For Each msq As QuickQuoteObject In SubQuotes
                                                                                         If String.IsNullOrWhiteSpace(msq.ContractorsEquipmentLeasedRentedFromOthersLimit) = False AndAlso (msq.ContractorsEquipmentScheduledCoverages IsNot Nothing AndAlso msq.ContractorsEquipmentScheduledCoverages.Any()) Or String.IsNullOrWhiteSpace(msq.ContractorsEquipmentScheduleDeductibleId) = False Or String.IsNullOrWhiteSpace(msq.ContractorsEquipmentScheduleCoinsuranceTypeId) = False Or String.IsNullOrWhiteSpace(msq.ContractorsEquipmentLeasedRentedFromOthersLimit) = False Or String.IsNullOrWhiteSpace(msq.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit) = False Or String.IsNullOrWhiteSpace(msq.SmallToolsAnyOneLossCatastropheLimit) = False Then
                                                                                             Return True
                                                                                         End If
                                                                                     Next
                                                                                 End If
                                                                                 Return False
                                                                             End Function)
            Dim showEquipmentFloaterSection As Boolean = DetermineShowEquipmentFloatersSection()

            ''' Computer
            Dim DetermineComputerSection = New Func(Of Boolean)(Function()
                                                                    'If String.IsNullOrWhiteSpace(Quote.ComputerAllPerilsDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(Quote.ComputerCoinsuranceTypeId) = False OrElse String.IsNullOrWhiteSpace(Quote.ComputerValuationMethodTypeId) = False OrElse String.IsNullOrWhiteSpace(Quote.ComputerEarthquakeVolcanicEruptionDeductible) = False OrElse String.IsNullOrWhiteSpace(Quote.ComputerMechanicalBreakdownDeductible) = False OrElse hasBuildingComputers() = True Then
                                                                    '    Return True
                                                                    'End If
                                                                    'updated 8/9/2018 for multi-state; updated 8/11/2018 to use new Property
                                                                    If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                                                                        For Each msq As QuickQuoteObject In SubQuotes
                                                                            If String.IsNullOrWhiteSpace(msq.ComputerAllPerilsDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(msq.ComputerCoinsuranceTypeId) = False OrElse String.IsNullOrWhiteSpace(msq.ComputerValuationMethodTypeId) = False OrElse String.IsNullOrWhiteSpace(msq.ComputerEarthquakeVolcanicEruptionDeductible) = False OrElse String.IsNullOrWhiteSpace(msq.ComputerMechanicalBreakdownDeductible) = False OrElse hasBuildingComputers() = True Then
                                                                                Return True
                                                                            End If
                                                                        Next
                                                                    End If
                                                                    Return False
                                                                End Function)
            Dim showComputerSection As Boolean = DetermineComputerSection()

            ''' Transportation
            Dim DetermineTransportationSection = New Func(Of Boolean)(Function()
                                                                          'Motor
                                                                          'If String.IsNullOrWhiteSpace(Quote.MotorTruckCargoScheduledVehicleDeductibleId) = False AndAlso (Quote.MotorTruckCargoScheduledVehicles IsNot Nothing AndAlso Quote.MotorTruckCargoScheduledVehicles.Any()) Or String.IsNullOrWhiteSpace(Quote.MotorTruckCargoScheduledVehicleDescription) = False Then
                                                                          '    Return True
                                                                          'End If
                                                                          'updated 8/9/2018 for multi-state; updated 8/11/2018 to use new Property
                                                                          If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                                                                              For Each msq As QuickQuoteObject In SubQuotes
                                                                                  If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
                                                                                      If String.IsNullOrWhiteSpace(msq.MotorTruckCargoUnScheduledVehicleDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(msq.MotorTruckCargoUnScheduledVehicleCatastropheLimit) = False OrElse String.IsNullOrWhiteSpace(msq.MotorTruckCargoUnScheduledAnyVehicleLimit) = False OrElse String.IsNullOrWhiteSpace(msq.MotorTruckCargoUnScheduledVehicleDescription) = False OrElse String.IsNullOrWhiteSpace(msq.MotorTruckCargoUnScheduledNumberOfVehicles) = False Then
                                                                                          Return True
                                                                                      End If
                                                                                  Else
                                                                                      If String.IsNullOrWhiteSpace(msq.MotorTruckCargoScheduledVehicleDeductibleId) = False AndAlso (msq.MotorTruckCargoScheduledVehicles IsNot Nothing AndAlso msq.MotorTruckCargoScheduledVehicles.Any()) Or String.IsNullOrWhiteSpace(msq.MotorTruckCargoScheduledVehicleDescription) = False Then
                                                                                          Return True
                                                                                      End If
                                                                                  End If
                                                                              Next
                                                                          End If
                                                                          'Owners Cargo
                                                                          'If String.IsNullOrWhiteSpace(Quote.OwnersCargoAnyOneOwnedVehicleDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(Quote.OwnersCargoCatastropheLimit) = False Or String.IsNullOrWhiteSpace(Quote.OwnersCargoAnyOneOwnedVehicleLimit) = False Or String.IsNullOrWhiteSpace(Quote.OwnersCargoAnyOneOwnedVehicleDescription) = False Then
                                                                          '    Return True
                                                                          'End If
                                                                          'updated 8/9/2018 for multi-state; updated 8/11/2018 to use new Property
                                                                          If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                                                                              For Each msq As QuickQuoteObject In SubQuotes
                                                                                  If String.IsNullOrWhiteSpace(msq.OwnersCargoAnyOneOwnedVehicleDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(msq.OwnersCargoCatastropheLimit) = False Or String.IsNullOrWhiteSpace(msq.OwnersCargoAnyOneOwnedVehicleLimit) = False Or String.IsNullOrWhiteSpace(msq.OwnersCargoAnyOneOwnedVehicleDescription) = False Then
                                                                                      Return True
                                                                                  End If
                                                                              Next
                                                                          End If

                                                                          'Transporation
                                                                          'If String.IsNullOrWhiteSpace(Quote.TransportationCatastropheDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(Quote.TransportationAnyOneOwnedVehicleLimit) = False Or String.IsNullOrWhiteSpace(Quote.TransportationAnyOneOwnedVehicleNumberOfVehicles) = False Or String.IsNullOrWhiteSpace(Quote.TransportationCatastropheDescription) = False Then
                                                                          '    Return True
                                                                          'End If
                                                                          'updated 8/9/2018 for multi-state; updated 8/11/2018 to use new Property
                                                                          If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                                                                              For Each msq As QuickQuoteObject In SubQuotes
                                                                                  If String.IsNullOrWhiteSpace(msq.TransportationCatastropheDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(msq.TransportationAnyOneOwnedVehicleLimit) = False Or String.IsNullOrWhiteSpace(msq.TransportationAnyOneOwnedVehicleNumberOfVehicles) = False Or String.IsNullOrWhiteSpace(msq.TransportationCatastropheDescription) = False Then
                                                                                      Return True
                                                                                  End If
                                                                              Next
                                                                          End If

                                                                          Return False
                                                                      End Function)
            Dim showTransportationSection As Boolean = DetermineTransportationSection()

            'added 8/9/2018 for multi-state; updated 8/11/2018 to use new Property
            Dim hasCIM As Boolean = False
            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                For Each msq As QuickQuoteObject In SubQuotes
                    If msq.CPP_Has_InlandMarine_PackagePart = True Then
                        hasCIM = True
                        Exit For
                    End If
                Next
            End If

            'If Quote.CPP_Has_InlandMarine_PackagePart AndAlso (showEquipmentFloaterSection Or showComputerSection Or showTransportationSection) Then
            'updated 8/9/2018 for multi-state
            If hasCIM AndAlso (showEquipmentFloaterSection Or showComputerSection Or showTransportationSection) Then
                CIM_Section.Visible = True
                CIMButton.Visible = True

                'group by section and possible hide questions
                Dim CIMsections = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCPPUnderwritingQuestions_CIM()
                                  Where (showEquipmentFloaterSection Or question.SectionName.ToUpper() <> "Equipment Floater".ToUpper()) And
                                       (showTransportationSection Or question.SectionName.ToUpper() <> "Transportation Section".ToUpper()) And
                                       (showComputerSection Or question.SectionName.ToUpper() <> "Electronic Data Processing Section – General Info".ToUpper()) And
                                       (showComputerSection Or question.SectionName.ToUpper() <> "Electronic Data Processing Section – Computer Room Info".ToUpper()) And
                                       (showComputerSection Or question.SectionName.ToUpper() <> "Electronic Data Processing Section – Media & Data (Software) Info".ToUpper()) And
                                       (question.SectionName.ToUpper() <> "Applicant Information".ToUpper())
                                  Group question By question.SectionName
                                   Into Group
                                  Select Group

                'If Me.Quote.PolicyUnderwritings IsNot Nothing Then
                'updated 8/9/2018; updated 8/11/2018 to use new Property
                If SubQuoteFirst IsNot Nothing And SubQuoteFirst.PolicyUnderwritings IsNot Nothing Then
                    'For Each question In Me.Quote.PolicyUnderwritings
                    'updated 8/9/2018
                    For Each question In SubQuoteFirst.PolicyUnderwritings
                        Dim codeID = question.PolicyUnderwritingCodeId
                        Dim answer = question.PolicyUnderwritingAnswer
                        Dim addInfo = question.PolicyUnderwritingExtraAnswer
                        For Each section In CIMsections
                            For Each qID In section
                                If (qID.PolicyUnderwritingCodeId = codeID) Then
                                    qID.PolicyUnderwritingExtraAnswer = ""
                                    If (answer = 1) Then
                                        qID.QuestionAnswerYes = True
                                        qID.QuestionAnswerNo = False
                                        qID.PolicyUnderwritingExtraAnswer = addInfo
                                    ElseIf (answer = -1) Then
                                        qID.QuestionAnswerNo = True
                                        qID.QuestionAnswerYes = False
                                        If Not String.IsNullOrEmpty(addInfo) Then
                                            qID.PolicyUnderwritingExtraAnswer = addInfo
                                        End If
                                    Else
                                        qID.QuestionAnswerNo = False
                                        qID.QuestionAnswerYes = False
                                        If Not String.IsNullOrEmpty(addInfo) Then
                                            qID.PolicyUnderwritingExtraAnswer = addInfo
                                        End If
                                    End If
                                End If
                            Next
                        Next
                    Next
                End If

                Me.rptCIM.DataSource = CIMsections
                Me.rptCIM.DataBind()
            End If
        End If
#End Region

#Region "Start CRM Processing"

        'added 8/9/2018 for multi-state; updated 8/11/2018 to use new Property
        Dim hasCRM As Boolean = False
        If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
            For Each msq As QuickQuoteObject In SubQuotes
                If msq.CPP_Has_Crime_PackagePart = True Then
                    hasCRM = True
                    Exit For
                End If
            Next
        End If

        'If Quote IsNot Nothing AndAlso Quote.CPP_Has_Crime_PackagePart Then
        'updated 8/9/2018 for multi-state
        If hasCRM Then

            CRM_Section.Visible = True
            CRMButton.Visible = True
            CRMButton2.Visible = True
            ''' Employee Theft
            Dim DetermineShowEmployeeTheftSection = New Func(Of Boolean)(Function()
                                                                             'If String.IsNullOrWhiteSpace(Quote.EmployeeTheftLimit) = False Or String.IsNullOrWhiteSpace(Quote.EmployeeTheftNumberOfAdditionalPremises) = False Or String.IsNullOrWhiteSpace(Quote.EmployeeTheftNumberOfRatableEmployees) = False Or String.IsNullOrWhiteSpace(Quote.EmployeeTheftDeductibleId) = False Then
                                                                             '    Return True
                                                                             'End If
                                                                             'updated 8/9/2018 for multi-state; updated 8/11/2018 to use new Property
                                                                             If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                                                                                 For Each msq As QuickQuoteObject In SubQuotes
                                                                                     If String.IsNullOrWhiteSpace(msq.EmployeeTheftLimit) = False Or String.IsNullOrWhiteSpace(msq.EmployeeTheftNumberOfAdditionalPremises) = False Or String.IsNullOrWhiteSpace(msq.EmployeeTheftNumberOfRatableEmployees) = False Or String.IsNullOrWhiteSpace(msq.EmployeeTheftDeductibleId) = False Then
                                                                                         Return True
                                                                                     End If
                                                                                 Next
                                                                             End If
                                                                             Return False
                                                                         End Function)
            Dim showEmployeeTheftSection As Boolean = DetermineShowEmployeeTheftSection()




            'group by section and possible hide questions
            Dim CRMsections = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCPPUnderwritingQuestions_CRM()
                              Where (showEmployeeTheftSection Or question.SectionName.ToUpper() <> "Hiring Practices".ToUpper()) And
                                       (showEmployeeTheftSection Or question.SectionName.ToUpper() <> "Controls and Audit Procedures - Audits".ToUpper()) And
                                       (showEmployeeTheftSection Or question.SectionName.ToUpper() <> "Controls and Audit Procedures – Banking/Other".ToUpper()) And
                                       (showEmployeeTheftSection Or question.SectionName.ToUpper() <> "Risk Grade Questions".ToUpper()) And
                                       (question.SectionName.ToUpper() <> "Applicant Information".ToUpper())
                              Group question By question.SectionName
                                   Into Group
                              Select Group

            'If Me.Quote.PolicyUnderwritings IsNot Nothing Then
            'updated 8/9/2018; updated 8/11/2018 to use new Property
            If SubQuoteFirst IsNot Nothing And SubQuoteFirst.PolicyUnderwritings IsNot Nothing Then
                'For Each question In Me.Quote.PolicyUnderwritings
                'updated 8/9/2018
                For Each question In SubQuoteFirst.PolicyUnderwritings
                    Dim codeID = question.PolicyUnderwritingCodeId
                    Dim answer = question.PolicyUnderwritingAnswer
                    Dim addInfo = question.PolicyUnderwritingExtraAnswer
                    For Each section In CRMsections
                        For Each qID In section
                            If (qID.PolicyUnderwritingCodeId = codeID) Then
                                qID.PolicyUnderwritingExtraAnswer = ""
                                If (answer = 1) Then
                                    qID.QuestionAnswerYes = True
                                    qID.QuestionAnswerNo = False
                                    qID.PolicyUnderwritingExtraAnswer = addInfo
                                ElseIf (answer = -1) Then
                                    qID.QuestionAnswerNo = True
                                    qID.QuestionAnswerYes = False
                                    If Not String.IsNullOrEmpty(addInfo) Then
                                        qID.PolicyUnderwritingExtraAnswer = addInfo
                                    End If
                                Else
                                    qID.QuestionAnswerNo = False
                                    qID.QuestionAnswerYes = False
                                    If Not String.IsNullOrEmpty(addInfo) Then
                                        qID.PolicyUnderwritingExtraAnswer = addInfo
                                    End If
                                End If
                            End If
                        Next
                    Next
                Next
            End If

            Me.rptCRM.DataSource = CRMsections
            Me.rptCRM.DataBind()
        End If
#End Region

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Private Sub rpt_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptCPR.ItemDataBound, rptCGL.ItemDataBound, rptCIM.ItemDataBound, rptCRM.ItemDataBound

        Dim questions As IEnumerable(Of VR.Common.UWQuestions.VRUWQuestion) = e.Item.DataItem

        If questions.IsLoaded() Then
            Dim SectionTitleLabel As Label = e.Item.FindControl("lblAccordHeader")
            SectionTitleLabel.Text = questions.First().SectionName
            Dim subControl As ctlCommercialUWQuestionItem_CPP = e.Item.FindControl("ctlCommercialUWQuestionItem_CPP")
            subControl.MyQuestions = questions.ToList()
            Select Case sender.ID
                Case rptCPR.ID
                    subControl.vsLOB = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                Case rptCGL.ID
                    subControl.vsLOB = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                Case rptCIM.ID
                    subControl.vsLOB = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine
                Case rptCRM.ID
                    subControl.vsLOB = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialCrime
            End Select
            subControl.Populate()
        End If

    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Visible Then
            ' Clear UW Questions in preparation for refill by repeater items
            'Me.Quote.PolicyUnderwritings = New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
            'updated 8/8/2018 for multi-state
            'If Me.Quote IsNot Nothing Then
            '    Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = QQHelper.MultiStateQuickQuoteObjects(Me.Quote) 'should always return at least Me.Quote in the list
            '    If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
            '        For Each msq As QuickQuoteObject In multiStateQuotes
            '            msq.PolicyUnderwritings = New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
            '        Next
            '    End If
            'End If
            'updated 8/11/2018 to use new Property
            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                For Each msq As QuickQuoteObject In SubQuotes
                    msq.PolicyUnderwritings = New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
                Next
            End If
            Me.SaveChildControls()
            Populate() ' I always wonder when I see a populate() inside a save() or a save() inside a populate() but I know there are some instances when it is needed this maybe one of them
        End If


        Return True
    End Function


    ' Matt A 4-30-17 -  This should be a shared method under IFM.VR.Common.Helpers.QuickQuoteObjectHelper
    'Public Function GetAbbreviatedLOBPrefix(LineOfBusiness)
    'End Function

    ''' <summary>
    ''' SUBMIT Button
    ''' This is the only postback on this control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnGoToApp.Click
        Me.Save_FireSaveEvent()

        If sender Is btnGoToApp Then
            If Me.ValidationSummmary.HasErrors = False Then
                Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "0")
            End If
        End If
    End Sub


    Private Function GetProgramNameByBOPNewClassCode(ByVal BOPNewClassCode As String) As String

        Dim ProgramName As String = String.Empty
        If programLookupCache.ContainsKey(BOPNewClassCode) Then
            ProgramName = programLookupCache(BOPNewClassCode)
        Else
            ' IF some how this fails then just let it fail Matt A 4-30-17
            ' the database part should be a shared method under IFM.VR.Common.Helpers.BOP.ClassificationHelper but keep that caching logic local to this control
            ProgramName = IFM.VR.Common.Helpers.BOP.ClassificationHelper.GetProgramNameByClassCode(BOPNewClassCode)
            programLookupCache.Add(BOPNewClassCode, ProgramName)
        End If
        Return ProgramName

    End Function

    Public Function hasBuildingComputers() As Boolean
        Dim buildingDataPresent As Boolean = False

        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                For Each location In Quote.Locations
                    If location.Buildings IsNot Nothing Then
                        For Each building As QuickQuoteBuilding In location.Buildings
                            If String.IsNullOrEmpty(building.ComputerHardwareLimit) = False OrElse String.IsNullOrEmpty(building.ComputerProgramsApplicationsAndMediaLimit) = False OrElse String.IsNullOrEmpty(building.ComputerBusinessIncomeLimit) = False Then
                                buildingDataPresent = True
                            End If
                        Next
                    End If
                Next
            End If
        End If
        Return buildingDataPresent
    End Function


End Class