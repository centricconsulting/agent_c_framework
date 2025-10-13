Imports IFM.PrimativeExtensions ' Added 4-30-17 Matt A
Imports IFM.VR.Common.Helpers.CPP
Public Class ctlCommercialUWQuestionItem_CPP
    Inherits VRControlBase

#Region "Declarations"
    'Private Const ClassName As String = "ctlCommercialUWQuestionItem" - not needed - let exceptions happen - no need to log them to database that no one looks at
    'Private msgQuoteObjNothing As String = "Quote Object is Nothing"
    'Private msgLOBNotSupported As String = "LOB Not Supported!"
#End Region

    Public Property vsLOB As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType
        Get
            If ViewState("vs_LOB") IsNot Nothing Then
                Return CType(ViewState("vs_LOB"), QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType)
            End If
            Return 0
        End Get
        Set(value As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType)
            ViewState("vs_LOB") = value
        End Set
    End Property

    Public MyQuestions As List(Of VR.Common.UWQuestions.VRUWQuestion) = Nothing

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

        Select Case Me.Quote.LobType
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                ' Session used to track if contractor uw questions are present in Quote.PolicyUnderwritings when questions are not visible
                ' -- This prevents reevaluation for each control on save, when we only need to test it once.  
                Session("isContractorsPresent") = ""
                Session("isSessionPresent") = ""
        End Select

        Me.rptUWQ.DataSource = MyQuestions
        Me.rptUWQ.DataBind()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Visible Then

            ' Generate Questions List
            Dim FullQuestionList As New MasterQuestionList(Quote)

            ' Choose a Questions List by LOB
            If vsLOB = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty Then
                MyQuestions = FullQuestionList.CPRList
            ElseIf vsLOB = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Then
                MyQuestions = FullQuestionList.CGLList
            ElseIf vsLOB = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine Then
                MyQuestions = FullQuestionList.CIMList
            ElseIf vsLOB = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialCrime Then
                MyQuestions = FullQuestionList.CRMList
            End If

            ' Set Temp List
            Dim qqItemList As New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)

            'added 8/9/2018 for multi-state; removed 8/11/2018 to use new Property
            'Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
            'If Me.Quote IsNot Nothing Then
            '    multiStateQuotes = QQHelper.MultiStateQuickQuoteObjects(Me.Quote) 'should always return at least Me.Quote in the list
            'End If

            ' Iterate over repeater
            For Each ri As RepeaterItem In rptUWQ.Items
                Dim radioYes As RadioButton = ri.FindControl("rbYes")
                Dim radioNo As RadioButton = ri.FindControl("rbNo")
                Dim txtAdditionInfo As TextBox = ri.FindControl("txtUWQDescription")
                Dim tblAnswers As HtmlTable = ri.FindControl("tblRadioButtons")
                Dim diamondCode As String = tblAnswers.Attributes("data-diamondcode")

                Dim qqItem As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting

                If radioYes.Checked OrElse radioNo.Checked OrElse String.IsNullOrEmpty(txtAdditionInfo.Text) = False Then
                    Dim question = From c In MyQuestions
                                   Where c.PolicyUnderwritingCodeId = diamondCode
                                   Select c
                    qqItem.PolicyId = question(0).PolicyId
                    qqItem.PolicyImageNum = question(0).PolicyImageNum
                    qqItem.PolicyUnderwriterDate = question(0).PolicyUnderwriterDate
                    qqItem.PolicyUnderwritingAnswer = If(radioYes.Checked, "1", If(radioNo.Checked, "-1", "0"))
                    qqItem.PolicyUnderwritingAnswerTypeId = question(0).PolicyUnderwritingAnswerTypeId
                    qqItem.PolicyUnderwritingCodeId = question(0).PolicyUnderwritingCodeId
                    qqItem.PolicyUnderwritingExtraAnswer = txtAdditionInfo.Text
                    qqItem.PolicyUnderwritingExtraAnswerTypeId = question(0).PolicyUnderwritingExtraAnswerTypeId
                    qqItem.PolicyUnderwritingLevelId = question(0).PolicyUnderwritingLevelId
                    qqItem.PolicyUnderwritingNum = question(0).PolicyUnderwritingNum
                    qqItem.PolicyUnderwritingTabId = question(0).PolicyUnderwritingTabId

                    qqItemList.Add(qqItem)
                End If
            Next

            ' Add To Programatically Set Question items to an answer list.
            Select Case vsLOB
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                    FullQuestionList.saveAddInfoList(qqItemList)
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                    ' Contractors Being Shown?
                    Dim showContractorsSection As Boolean = CBool(ViewState("showContractorSection"))

                    If Not showContractorsSection Then
                        Dim ContractorCodes() As String = {"9257", "9258", "9259", "9260", "9261", "9038"}
                        Dim isContractorsPresent As Boolean

                        Dim isContractorsPresentCheck = New Func(Of Boolean)(Function()
                                                                                 ' Check Location Level
                                                                                 'For Each item As QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting In Me.Quote.PolicyUnderwritings
                                                                                 '    If ContractorCodes.Contains(item.PolicyUnderwritingCodeId) Then
                                                                                 '        Return True
                                                                                 '    End If
                                                                                 'Next
                                                                                 'updated 8/9/2018 for multiState; now looking at each stateQuote; updated 8/11/2018 to use new SubQuotes Property
                                                                                 If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                                                                                     For Each msq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                                                                                         If msq.PolicyUnderwritings IsNot Nothing AndAlso msq.PolicyUnderwritings.Count > 0 Then
                                                                                             For Each item As QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting In msq.PolicyUnderwritings
                                                                                                 If ContractorCodes.Contains(item.PolicyUnderwritingCodeId) Then
                                                                                                     Return True
                                                                                                 End If
                                                                                             Next
                                                                                         End If
                                                                                     Next
                                                                                 End If
                                                                                 Return False
                                                                             End Function)

                        ' Are Contractor answers present already?
                        If String.IsNullOrEmpty(Session("isContractorsPresent")) Then
                            isContractorsPresent = isContractorsPresentCheck()
                        Else
                            isContractorsPresent = CBool(Session("isContractorsPresent").ToString)
                        End If

                        ' Default items to "No"
                        If Not isContractorsPresent Then
                            Dim qqItem As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting
                            Dim questionContractors = From c In FullQuestionList.CGLListWithContractor
                                                      Where ContractorCodes.Contains(c.PolicyUnderwritingCodeId)
                                                      Select c
                            For Each question In questionContractors
                                qqItem = New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting
                                qqItem.PolicyId = question.PolicyId
                                qqItem.PolicyImageNum = question.PolicyImageNum
                                qqItem.PolicyUnderwriterDate = question.PolicyUnderwriterDate
                                qqItem.PolicyUnderwritingAnswer = "-1"
                                qqItem.PolicyUnderwritingAnswerTypeId = question.PolicyUnderwritingAnswerTypeId
                                qqItem.PolicyUnderwritingCodeId = question.PolicyUnderwritingCodeId
                                qqItem.PolicyUnderwritingExtraAnswer = question.PolicyUnderwritingExtraAnswer
                                qqItem.PolicyUnderwritingExtraAnswerTypeId = question.PolicyUnderwritingExtraAnswerTypeId
                                qqItem.PolicyUnderwritingLevelId = question.PolicyUnderwritingLevelId
                                qqItem.PolicyUnderwritingNum = question.PolicyUnderwritingNum
                                qqItem.PolicyUnderwritingTabId = question.PolicyUnderwritingTabId

                                qqItemList.Add(qqItem)
                            Next

                            ' Add this to Session so we don't process again needlessly
                            Session("isContractorsPresent") = True

                        End If

                    End If
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine
                    If String.IsNullOrEmpty(Session("isSessionPresent")) OrElse CBool(Session("isSessionPresent").ToString) = False Then
                        FullQuestionList.saveTransportList(qqItemList)
                        Session("isSessionPresent") = True
                    End If

                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialCrime
                    For Each question As QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting In qqItemList
                        If question.PolicyUnderwritingCodeId = "9386" Then
                            question.PolicyUnderwritingAnswer = "1"
                        End If
                    Next
                    Exit Select
                Case Else
                    Exit Select
            End Select

            ' Add Answered Items from list into the Quote Object
            For Each question In qqItemList
                'Me.Quote.PolicyUnderwritings.Add(question)
                'updated 8/9/2018 for multiState; now setting on each stateQuote; updated 8/11/2018 to use new SubQuotes Property
                If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                    For Each msq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                        If msq.PolicyUnderwritings Is Nothing Then
                            msq.PolicyUnderwritings = New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
                        End If
                        msq.PolicyUnderwritings.Add(question)
                    Next
                End If
            Next

        End If
        Return True
    End Function

    Public Function SaveMeTest(sender As Object, e As System.EventArgs) As Boolean
        Dim qqItemList As New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
        Dim MyQuestions = VR.Common.UWQuestions.UWQuestions.GetCommercialBOPUnderwritingQuestions()

        For Each ri As RepeaterItem In rptUWQ.Items
            Dim radioYes As RadioButton = ri.FindControl("rbYes")
            Dim radioNo As RadioButton = ri.FindControl("rbNo")
            Dim txtAdditionInfo As TextBox = ri.FindControl("txtUWQDescription")
            Dim tblAnswers As HtmlTable = ri.FindControl("tblRadioButtons")
            Dim diamondCode As String = tblAnswers.Attributes("data-diamondcode")
            Dim LOB As String = tblAnswers.Attributes("data-lob")

            Dim qqItem As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting

            Select Case LOB
                Case "BOP"
                    If radioYes.Checked Or radioNo.Checked Then
                        Dim question = From c In MyQuestions
                                       Where c.PolicyUnderwritingCodeId = diamondCode
                                       Select c
                        qqItem.PolicyId = question(0).PolicyId
                        qqItem.PolicyImageNum = question(0).PolicyImageNum
                        qqItem.PolicyUnderwriterDate = question(0).PolicyUnderwriterDate
                        qqItem.PolicyUnderwritingAnswer = If(radioYes.Checked, "1", If(radioNo.Checked, "-1", "0"))
                        qqItem.PolicyUnderwritingAnswerTypeId = question(0).PolicyUnderwritingAnswerTypeId
                        qqItem.PolicyUnderwritingCodeId = question(0).PolicyUnderwritingCodeId
                        qqItem.PolicyUnderwritingExtraAnswer = txtAdditionInfo.Text
                        qqItem.PolicyUnderwritingExtraAnswerTypeId = question(0).PolicyUnderwritingExtraAnswerTypeId
                        qqItem.PolicyUnderwritingLevelId = question(0).PolicyUnderwritingLevelId
                        qqItem.PolicyUnderwritingNum = question(0).PolicyUnderwritingNum
                        qqItem.PolicyUnderwritingTabId = question(0).PolicyUnderwritingTabId

                        qqItemList.Add(qqItem)
                    End If
                    Exit Select
                Case Else
                    If radioYes.Checked Or radioNo.Checked Then
                        Dim question = From c In MyQuestions
                                       Where c.PolicyUnderwritingCodeId = diamondCode
                                       Select c
                        qqItem.PolicyId = question(0).PolicyId
                        qqItem.PolicyImageNum = question(0).PolicyImageNum
                        qqItem.PolicyUnderwriterDate = question(0).PolicyUnderwriterDate
                        qqItem.PolicyUnderwritingAnswer = If(radioYes.Checked, "1", If(radioNo.Checked, "-1", "0"))
                        qqItem.PolicyUnderwritingAnswerTypeId = question(0).PolicyUnderwritingAnswerTypeId
                        qqItem.PolicyUnderwritingCodeId = question(0).PolicyUnderwritingCodeId
                        qqItem.PolicyUnderwritingExtraAnswer = txtAdditionInfo.Text
                        qqItem.PolicyUnderwritingExtraAnswerTypeId = question(0).PolicyUnderwritingExtraAnswerTypeId
                        qqItem.PolicyUnderwritingLevelId = question(0).PolicyUnderwritingLevelId
                        qqItem.PolicyUnderwritingNum = question(0).PolicyUnderwritingNum
                        qqItem.PolicyUnderwritingTabId = question(0).PolicyUnderwritingTabId

                        qqItemList.Add(qqItem)
                    End If
                    Exit Select

            End Select

        Next
        'Me.Quote.PolicyUnderwritings = New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
        'For Each question In qqItemList
        '    Me.Quote.PolicyUnderwritings.Add(question)
        'Next
        'updated 8/9/2018 for multiState; now setting on each stateQuote
        'Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = QQHelper.MultiStateQuickQuoteObjects(Me.Quote) 'should always return at least Me.Quote in the list
        'If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
        '    For Each msq As QuickQuote.CommonObjects.QuickQuoteObject In multiStateQuotes
        '        msq.PolicyUnderwritings = New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
        '        For Each question In qqItemList
        '            msq.PolicyUnderwritings.Add(question)
        '        Next
        '    Next
        'End If
        'updated 8/11/2018 to use new Property
        If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
            For Each msq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                msq.PolicyUnderwritings = New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
                For Each question In qqItemList
                    msq.PolicyUnderwritings.Add(question)
                Next
            Next
        End If

        Return True
    End Function

    Public Class MasterQuestionList
        Public Property CPRList As List(Of VR.Common.UWQuestions.VRUWQuestion)
        Public Property CGLList As List(Of VR.Common.UWQuestions.VRUWQuestion)
        Public Property CGLListWithContractor As List(Of VR.Common.UWQuestions.VRUWQuestion)
        Public Property CIMList As List(Of VR.Common.UWQuestions.VRUWQuestion)
        Public Property CRMList As List(Of VR.Common.UWQuestions.VRUWQuestion)
        Public Property masterList As List(Of VR.Common.UWQuestions.VRUWQuestion)
        Public Property Quote As QuickQuote.CommonObjects.QuickQuoteObject

        Public Sub New(inQuote As QuickQuote.CommonObjects.QuickQuoteObject)
            Quote = inQuote

            'added 8/9/2018 for multi-state
            Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            If Quote IsNot Nothing Then
                multiStateQuotes = qqHelper.MultiStateQuickQuoteObjects(Quote) 'should always return at least Quote in the list
            End If

            'CPR Processing
            Dim CPRListPull = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCPPUnderwritingQuestions_CPR()
                              Group question By question.SectionName
                                   Into Group
                              Select Group
            For Each question In CPRListPull
                CPRList = question.ToList
            Next

            'CGL with Contractors Processing
            Dim CGLListWithContractorPull = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCPPUnderwritingQuestions_CGL()
                                            Where (question.SectionName.ToUpper() = "General Liability - Contractors".ToUpper())
                                            Group question By question.SectionName
                                   Into Group
                                            Select Group

            For Each questionGroup In CGLListWithContractorPull
                For Each question In questionGroup
                    CGLListWithContractor.AddItem(question)
                Next
            Next

            'CGL Processing
            Dim DetermineShowContractorsSection = New Func(Of Boolean)(Function()
                                                                           ' Check Location Level
                                                                           For Each location As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations ' If Locations is null just let the exception happen
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
                                                                           'updated 8/9/2018 for multiState; now looking for classCode in any stateQuote; may need to be changed if we pull all up to top level
                                                                           'If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                                                                           '    For Each msq As QuickQuote.CommonObjects.QuickQuoteObject In multiStateQuotes
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
                                                                                'updated 8/9/2018 for multiState; now looking for any stateQuote to have EmployeeBenefits
                                                                                If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                                                                                    For Each msq As QuickQuote.CommonObjects.QuickQuoteObject In multiStateQuotes
                                                                                        If IsNumeric(msq.EmployeeBenefitsLiabilityText) Then
                                                                                            Return True
                                                                                        End If
                                                                                    Next
                                                                                End If
                                                                                Return False
                                                                            End Function)
            Dim showEmployeeBenefitsSection As Boolean = DetermineShowEmployeeBenefitsSection()

            Dim CGLListPull = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCPPUnderwritingQuestions_CGL()
                              Where (showContractorsSection Or question.SectionName.ToUpper() <> "General Liability - Contractors".ToUpper()) And
                                           (showEmployeeBenefitsSection Or question.SectionName.ToUpper() <> "Company – Employee Benefits".ToUpper()) And
                                           (question.SectionName.ToUpper() <> "Applicant Information".ToUpper())
                              Group question By question.SectionName
                                       Into Group
                              Select Group
            For Each questionGroup In CGLListPull
                For Each question In questionGroup
                    CGLList.AddItem(question)
                Next
            Next

            'CIM Processing
            ''' Equipment Floater - has contractors equipment, not enhancement
            ''' String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentLeasedRentedFromOthersLimit) = False AndAlso (Quote.ContractorsEquipmentScheduledCoverages IsNot Nothing AndAlso Quote.ContractorsEquipmentScheduledCoverages.Any()) Or String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentScheduleDeductibleId) = False Or String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentScheduleCoinsuranceTypeId) = False Or String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentLeasedRentedFromOthersLimit) = False Or String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit) = False Or String.IsNullOrWhiteSpace(Quote.SmallToolsAnyOneLossCatastropheLimit) = False
            Dim DetermineShowEquipmentFloatersSection = New Func(Of Boolean)(Function()
                                                                                 'If Quote.HasContractorsEnhancement Then
                                                                                 'If String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentLeasedRentedFromOthersLimit) = False AndAlso (Quote.ContractorsEquipmentScheduledCoverages IsNot Nothing AndAlso Quote.ContractorsEquipmentScheduledCoverages.Any()) Or String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentScheduleDeductibleId) = False Or String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentScheduleCoinsuranceTypeId) = False Or String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentLeasedRentedFromOthersLimit) = False Or String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit) = False Or String.IsNullOrWhiteSpace(Quote.SmallToolsAnyOneLossCatastropheLimit) = False Then
                                                                                 '    Return True
                                                                                 'End If
                                                                                 'updated 8/9/2018 for multi-state
                                                                                 If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                                                                                     For Each msq As QuickQuote.CommonObjects.QuickQuoteObject In multiStateQuotes
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
                                                                    'updated 8/9/2018 for multi-state
                                                                    If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                                                                        For Each msq As QuickQuote.CommonObjects.QuickQuoteObject In multiStateQuotes
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
                                                                          'updated 8/9/2018 for multi-state
                                                                          If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                                                                              For Each msq As QuickQuote.CommonObjects.QuickQuoteObject In multiStateQuotes
                                                                                  If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
                                                                                      If String.IsNullOrWhiteSpace(msq.MotorTruckCargoUnScheduledVehicleDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(msq.MotorTruckCargoUnScheduledVehicleCatastropheLimit) = False OrElse String.IsNullOrWhiteSpace(msq.MotorTruckCargoUnScheduledAnyVehicleLimit) = False OrElse String.IsNullOrWhiteSpace(msq.MotorTruckCargoUnScheduledVehicleDescription) = False OrElse String.IsNullOrWhiteSpace(msq.MotorTruckCargoUnScheduledAnyVehicleLimit) = False OrElse String.IsNullOrWhiteSpace(msq.MotorTruckCargoUnScheduledNumberOfVehicles) = False Then
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
                                                                          'updated 8/9/2018 for multi-state
                                                                          If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                                                                              For Each msq As QuickQuote.CommonObjects.QuickQuoteObject In multiStateQuotes
                                                                                  If String.IsNullOrWhiteSpace(msq.OwnersCargoAnyOneOwnedVehicleDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(msq.OwnersCargoCatastropheLimit) = False Or String.IsNullOrWhiteSpace(msq.OwnersCargoAnyOneOwnedVehicleLimit) = False Or String.IsNullOrWhiteSpace(msq.OwnersCargoAnyOneOwnedVehicleDescription) = False Then
                                                                                      Return True
                                                                                  End If
                                                                              Next
                                                                          End If

                                                                          'Transporation
                                                                          'If String.IsNullOrWhiteSpace(Quote.TransportationCatastropheDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(Quote.TransportationAnyOneOwnedVehicleLimit) = False Or String.IsNullOrWhiteSpace(Quote.TransportationAnyOneOwnedVehicleNumberOfVehicles) = False Or String.IsNullOrWhiteSpace(Quote.TransportationCatastropheDescription) = False Then
                                                                          '    Return True
                                                                          'End If
                                                                          'updated 8/9/2018 for multi-state
                                                                          If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                                                                              For Each msq As QuickQuote.CommonObjects.QuickQuoteObject In multiStateQuotes
                                                                                  If String.IsNullOrWhiteSpace(msq.TransportationCatastropheDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(msq.TransportationAnyOneOwnedVehicleLimit) = False Or String.IsNullOrWhiteSpace(msq.TransportationAnyOneOwnedVehicleNumberOfVehicles) = False Or String.IsNullOrWhiteSpace(msq.TransportationCatastropheDescription) = False Then
                                                                                      Return True
                                                                                  End If
                                                                              Next
                                                                          End If

                                                                          Return False
                                                                      End Function)
            Dim showTransportationSection As Boolean = DetermineTransportationSection()


            Dim CIMListPull = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCPPUnderwritingQuestions_CIM()
                              Where (showEquipmentFloaterSection Or question.SectionName.ToUpper() <> "Equipment Floater".ToUpper()) And
                                   (showTransportationSection Or question.SectionName.ToUpper() <> "Transportation Section".ToUpper()) And
                                   (showComputerSection Or question.SectionName.ToUpper() <> "Electronic Data Processing Section – General Info".ToUpper()) And
                                   (showComputerSection Or question.SectionName.ToUpper() <> "Electronic Data Processing Section – Computer Room Info".ToUpper()) And
                                   (showComputerSection Or question.SectionName.ToUpper() <> "Electronic Data Processing Section – Media & Data (Software) Info".ToUpper()) And
                                   (question.SectionName.ToUpper() <> "Applicant Information".ToUpper())
                              Group question By question.SectionName
                               Into Group
                              Select Group
            For Each questionGroup In CIMListPull
                For Each question In questionGroup
                    CIMList.AddItem(question)
                Next
            Next

            'CRM Processing
            Dim DetermineShowEmployeeTheftSection = New Func(Of Boolean)(Function()
                                                                             'If String.IsNullOrWhiteSpace(Quote.EmployeeTheftLimit) = False Or String.IsNullOrWhiteSpace(Quote.EmployeeTheftNumberOfAdditionalPremises) = False Or String.IsNullOrWhiteSpace(Quote.EmployeeTheftNumberOfRatableEmployees) = False Or String.IsNullOrWhiteSpace(Quote.EmployeeTheftDeductibleId) = False Then
                                                                             '    Return True
                                                                             'End If
                                                                             'updated 8/9/2018 for multi-state
                                                                             If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                                                                                 For Each msq As QuickQuote.CommonObjects.QuickQuoteObject In multiStateQuotes
                                                                                     If String.IsNullOrWhiteSpace(msq.EmployeeTheftLimit) = False Or String.IsNullOrWhiteSpace(msq.EmployeeTheftNumberOfAdditionalPremises) = False Or String.IsNullOrWhiteSpace(msq.EmployeeTheftNumberOfRatableEmployees) = False Or String.IsNullOrWhiteSpace(msq.EmployeeTheftDeductibleId) = False Then
                                                                                         Return True
                                                                                     End If
                                                                                 Next
                                                                             End If
                                                                             Return False
                                                                         End Function)
            Dim showEmployeeTheftSection As Boolean = DetermineShowEmployeeTheftSection()

            Dim CRMListPull = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCPPUnderwritingQuestions_CRM()
                              Where (showEmployeeTheftSection Or question.SectionName.ToUpper() <> "Hiring Practices".ToUpper()) And
                                       (showEmployeeTheftSection Or question.SectionName.ToUpper() <> "Controls and Audit Procedures - Audits".ToUpper()) And
                                       (showEmployeeTheftSection Or question.SectionName.ToUpper() <> "Controls and Audit Procedures – Banking/Other".ToUpper()) And
                                       (showEmployeeTheftSection Or question.SectionName.ToUpper() <> "Risk Grade Questions".ToUpper()) And
                                       (question.SectionName.ToUpper() <> "Applicant Information".ToUpper())
                              Group question By question.SectionName
                                   Into Group
                              Select Group
            For Each questionGroup In CRMListPull
                For Each question In questionGroup
                    CRMList.AddItem(question)
                Next
            Next
        End Sub

        Public Sub saveAddInfoList(ByRef qqItemList As List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting))

            Dim newItems As New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
            Dim CGLListAddInfo As List(Of VR.Common.UWQuestions.VRUWQuestion)
            'Dim CIMListAddInfo As List(Of VR.Common.UWQuestions.VRUWQuestion)
            'Dim CRMListAddInfo As List(Of VR.Common.UWQuestions.VRUWQuestion)

            'CGL Processing
            Dim CGLListAddInfoPull = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCPPUnderwritingQuestions_CGL()
                                     Where (question.SectionName.ToUpper() = "Applicant Information".ToUpper())
                                     Group question By question.SectionName
                                        Into Group
                                     Select Group

            ''CIM Processing
            'Dim CIMListAddInfoPull = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCPPUnderwritingQuestions_CIM()
            '                         Where (question.SectionName.ToUpper() = "Applicant Information".ToUpper())
            '                         Group question By question.SectionName
            '                           Into Group
            '                         Select Group

            ''CRM Processing
            'Dim CRMListAddInfoPull = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCPPUnderwritingQuestions_CRM()
            '                         Where (question.SectionName.ToUpper() = "Applicant Information".ToUpper())
            '                         Group question By question.SectionName
            '                           Into Group
            '                         Select Case Group

            For Each question In CGLListAddInfoPull
                CGLListAddInfo = question.ToList
            Next
            'For Each question In CIMListAddInfoPull
            '    CIMListAddInfo = question.ToList
            'Next
            'For Each question In CRMListAddInfoPull
            '    CRMListAddInfo = question.ToList
            'Next


            For Each ri As QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting In qqItemList
                Dim UnderwritingAnswer As String = ri.PolicyUnderwritingAnswer
                Dim txtAdditionInfo As String = ri.PolicyUnderwritingExtraAnswer
                Dim diamondCode As String = ri.PolicyUnderwritingCodeId

                Dim qqItem As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting

                'CGL
                For Each question In CGLListAddInfo
                    If question.PolicyUnderwritingCodeId = diamondCode Then
                        qqItem.PolicyId = question.PolicyId
                        qqItem.PolicyImageNum = question.PolicyImageNum
                        qqItem.PolicyUnderwriterDate = question.PolicyUnderwriterDate
                        qqItem.PolicyUnderwritingAnswer = UnderwritingAnswer
                        qqItem.PolicyUnderwritingAnswerTypeId = question.PolicyUnderwritingAnswerTypeId
                        qqItem.PolicyUnderwritingCodeId = question.PolicyUnderwritingCodeId
                        qqItem.PolicyUnderwritingExtraAnswer = txtAdditionInfo
                        qqItem.PolicyUnderwritingExtraAnswerTypeId = question.PolicyUnderwritingExtraAnswerTypeId
                        qqItem.PolicyUnderwritingLevelId = question.PolicyUnderwritingLevelId
                        qqItem.PolicyUnderwritingNum = question.PolicyUnderwritingNum
                        qqItem.PolicyUnderwritingTabId = question.PolicyUnderwritingTabId

                        newItems.Add(qqItem)
                    End If
                Next

                'CGL
                For Each question In CGLListAddInfo
                    If question.PolicyUnderwritingCodeId = diamondCode Then
                        qqItem = New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting
                        '#3 "Any Exposure..." is different for CPP (9403) vs. CGL (9003) so we need to change the code
                        If question.PolicyUnderwritingCodeId = "9003" Then
                            question.PolicyUnderwritingCodeId = "9403"
                        End If
                        qqItem.PolicyId = question.PolicyId
                        qqItem.PolicyImageNum = question.PolicyImageNum
                        qqItem.PolicyUnderwriterDate = question.PolicyUnderwriterDate
                        qqItem.PolicyUnderwritingAnswer = UnderwritingAnswer
                        qqItem.PolicyUnderwritingAnswerTypeId = question.PolicyUnderwritingAnswerTypeId
                        qqItem.PolicyUnderwritingCodeId = question.PolicyUnderwritingCodeId
                        qqItem.PolicyUnderwritingExtraAnswer = txtAdditionInfo
                        qqItem.PolicyUnderwritingExtraAnswerTypeId = question.PolicyUnderwritingExtraAnswerTypeId
                        qqItem.PolicyUnderwritingLevelId = question.PolicyUnderwritingLevelId
                        qqItem.PolicyUnderwritingNum = question.PolicyUnderwritingNum
                        qqItem.PolicyUnderwritingTabId = "2"

                        newItems.Add(qqItem)
                    End If
                Next

                'If Quote.CPP_Has_InlandMarine_PackagePart Then
                '    'CIM
                '    For Each question In CIMListAddInfo
                '        If question.PolicyUnderwritingCodeId = diamondCode Then
                '            qqItem.PolicyId = question.PolicyId
                '            qqItem.PolicyImageNum = question.PolicyImageNum
                '            qqItem.PolicyUnderwriterDate = question.PolicyUnderwriterDate
                '            qqItem.PolicyUnderwritingAnswer = UnderwritingAnswer
                '            qqItem.PolicyUnderwritingAnswerTypeId = question.PolicyUnderwritingAnswerTypeId
                '            qqItem.PolicyUnderwritingCodeId = question.PolicyUnderwritingCodeId
                '            qqItem.PolicyUnderwritingExtraAnswer = txtAdditionInfo
                '            qqItem.PolicyUnderwritingExtraAnswerTypeId = question.PolicyUnderwritingExtraAnswerTypeId
                '            qqItem.PolicyUnderwritingLevelId = question.PolicyUnderwritingLevelId
                '            qqItem.PolicyUnderwritingNum = question.PolicyUnderwritingNum
                '            qqItem.PolicyUnderwritingTabId = question.PolicyUnderwritingTabId

                '            newItems.Add(qqItem)
                '        End If
                '    Next
                'End If

                'If Quote IsNot Nothing AndAlso Quote.CPP_Has_Crime_PackagePart Then
                '    'CRM
                '    For Each question In CRMListAddInfo
                '        If question.PolicyUnderwritingCodeId = diamondCode Then
                '            qqItem.PolicyId = question.PolicyId
                '            qqItem.PolicyImageNum = question.PolicyImageNum
                '            qqItem.PolicyUnderwriterDate = question.PolicyUnderwriterDate
                '            qqItem.PolicyUnderwritingAnswer = UnderwritingAnswer
                '            qqItem.PolicyUnderwritingAnswerTypeId = question.PolicyUnderwritingAnswerTypeId
                '            qqItem.PolicyUnderwritingCodeId = question.PolicyUnderwritingCodeId
                '            qqItem.PolicyUnderwritingExtraAnswer = txtAdditionInfo
                '            qqItem.PolicyUnderwritingExtraAnswerTypeId = question.PolicyUnderwritingExtraAnswerTypeId
                '            qqItem.PolicyUnderwritingLevelId = question.PolicyUnderwritingLevelId
                '            qqItem.PolicyUnderwritingNum = question.PolicyUnderwritingNum
                '            qqItem.PolicyUnderwritingTabId = question.PolicyUnderwritingTabId

                '            newItems.Add(qqItem)
                '        End If
                '    Next
                'End If
            Next

            ' Add Answered Items from list into the Quote Object
            For Each question In newItems
                qqItemList.Add(question)
            Next

        End Sub

        Public Sub saveTransportList(qqItemList As List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting))

            Dim QuestionsToAdd As New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
            Dim SourceCodes() As String = {"9448", "9449", "9517", "9451", "9518", "9519", "9524", "9528"}
            Dim TargetCodes() As String = {"9448", "9449", "9450", "9451", "9453", "9454", "9455", "9456"}

            Dim CodesToAdd As New List(Of String())
            CodesToAdd.Add(New String() {"9448", "9448"})
            CodesToAdd.Add(New String() {"9449", "9449"})
            CodesToAdd.Add(New String() {"9517", "9450"})
            CodesToAdd.Add(New String() {"9451", "9451"})
            CodesToAdd.Add(New String() {"9518", "9453"})
            CodesToAdd.Add(New String() {"9519", "9454"})
            CodesToAdd.Add(New String() {"9524", "9455"})
            CodesToAdd.Add(New String() {"9528", "9456"})


            Dim qqItem As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting
            Dim questionTansport = From c In qqItemList
                                   Where SourceCodes.Contains(c.PolicyUnderwritingCodeId)
                                   Select c

            For Each question In questionTansport
                For Each CodePair In CodesToAdd
                    If question.PolicyUnderwritingCodeId = CodePair(0) Then
                        qqItem = New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting
                        qqItem.PolicyId = question.PolicyId
                        qqItem.PolicyImageNum = question.PolicyImageNum
                        qqItem.PolicyUnderwriterDate = question.PolicyUnderwriterDate
                        qqItem.PolicyUnderwritingAnswer = question.PolicyUnderwritingAnswer
                        qqItem.PolicyUnderwritingAnswerTypeId = question.PolicyUnderwritingAnswerTypeId
                        qqItem.PolicyUnderwritingCodeId = CodePair(1)
                        qqItem.PolicyUnderwritingExtraAnswer = question.PolicyUnderwritingExtraAnswer
                        qqItem.PolicyUnderwritingExtraAnswerTypeId = question.PolicyUnderwritingExtraAnswerTypeId
                        qqItem.PolicyUnderwritingLevelId = question.PolicyUnderwritingLevelId
                        qqItem.PolicyUnderwritingNum = question.PolicyUnderwritingNum
                        qqItem.PolicyUnderwritingTabId = "5"

                        QuestionsToAdd.Add(qqItem)
                        Exit For
                    End If
                Next
            Next

            For Each question In QuestionsToAdd
                qqItemList.Add(question)
            Next

        End Sub

        Public Function hasBuildingComputers() As Boolean
            Dim buildingDataPresent As Boolean = False

            If Quote IsNot Nothing Then
                If Quote.Locations IsNot Nothing Then
                    For Each location In Quote.Locations
                        If location.Buildings IsNot Nothing Then
                            For Each building As QuickQuote.CommonObjects.QuickQuoteBuilding In location.Buildings
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


End Class

