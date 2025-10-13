Imports System.Data.SqlClient
Imports IFM.VR.Common.UWQuestions
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions ' Added 4-30-17 Matt A

Public Class ctlCommercialUWQuestionList
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
        Me.VRScript.AddScriptLine("$("".UWQuestionsSection"").accordion({collapsible: true, heightStyle: ""content""});")

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Dim SectionsWithQuestions As New List(Of List(Of VR.Common.UWQuestions.VRUWQuestion))
        Dim PopulateQuestions = New Func(Of IEnumerable(Of IEnumerable(Of VRUWQuestion)),
            List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting), IEnumerable(Of IEnumerable(Of VRUWQuestion)))(
            Function(sections, PolicyUnderwritings)
                If PolicyUnderwritings IsNot Nothing Then
                    For Each question In PolicyUnderwritings
                        Dim codeID = question.PolicyUnderwritingCodeId
                        Dim answer = question.PolicyUnderwritingAnswer
                        Dim addInfo = question.PolicyUnderwritingExtraAnswer
                        For Each section In sections
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
                Return sections
            End Function)

        Dim BindQuestions = New Action(Of IEnumerable(Of IEnumerable(Of VRUWQuestion)),
            List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting))(
            Sub(sections, policyUnderwritings)
                If policyUnderwritings IsNot Nothing Then
                    Me.rptUWQ.DataSource = PopulateQuestions(sections, policyUnderwritings)
                    Me.rptUWQ.DataBind()
                Else
                    Me.rptUWQ.DataSource = Nothing
                    Me.rptUWQ.DataBind()
                End If
            End Sub)

        If Me.Quote IsNot Nothing Then
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP

                    ' Check for an Apartment; skip left over the rest of the processing if any apartment is found.
                    Dim DetermineShowApartmentSection = New Func(Of Boolean)(
                        Function()
                            For Each location As QuickQuote.CommonObjects.QuickQuoteLocation In Me.Quote.Locations ' If Locations is null just let the exception happen
                                If location.Buildings.IsLoaded Then 'perfectly valid to have a location that doesn't have buildings - even if not this is not the place to care if it does or doesn't
                                    For Each building As QuickQuote.CommonObjects.QuickQuoteBuilding In location.Buildings
                                        If building.BuildingClassifications.IsLoaded Then ' again don't
                                            For Each buildingClass As QuickQuote.CommonObjects.QuickQuoteClassification In building.BuildingClassifications
                                                If GetProgramNameByBOPNewClassCode(buildingClass.ClassCode).ToUpper().Trim() = "Apartments".ToUpper() Then ' since this is in a loop cache the results to minimize database calls
                                                    Return True
                                                End If
                                            Next
                                        End If
                                    Next
                                End If
                            Next
                            Return False
                        End Function)

                    Dim showApartmentSection As Boolean = DetermineShowApartmentSection()

                    'group by section and possible hide the apartment questions when there is not a classification that has program name of 'Apartments'
                    Dim sections = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialBOPUnderwritingQuestions()
                                   Where showApartmentSection Or question.SectionName.ToUpper() <> "Business Owners - Apartments and Condos".ToUpper()
                                   Group question By question.SectionName
                                   Into Group
                                   Select Group

                    BindQuestions(sections, Me.SubQuoteFirst?.PolicyUnderwritings)
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    Dim sections = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCAPUnderwritingQuestions()
                                   Group question By question.SectionName
                                   Into Group
                                   Select Group

                    BindQuestions(sections, Me.SubQuoteFirst?.PolicyUnderwritings)

                    Exit Select

                Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                    Dim sections = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialWCPUnderwritingQuestions(Me.Quote.EffectiveDate)
                                   Group question By question.SectionName
                                   Into Group
                                   Select Group

                    BindQuestions(sections, Me.SubQuoteFirst?.PolicyUnderwritings)

                    Exit Select

                Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                    ' Check for an Contractor
                    Dim DetermineShowContractorsSection = New Func(Of Boolean)(
                        Function()
                            ' Check Location Level
                            For Each location As QuickQuote.CommonObjects.QuickQuoteLocation In Me.Quote.Locations ' If Locations is null just let the exception happen
                                If location.GLClassifications.IsLoaded Then ' again don't
                                    For Each locationClass As QuickQuote.CommonObjects.QuickQuoteGLClassification In location.GLClassifications
                                        If locationClass.ClassCode.StartsWithAny_NullSafe("9") Then
                                            Return True
                                        End If
                                    Next
                                End If
                            Next
                            'Check Policy Level
                            '8/8/2018 note: backend will likely need to pull these up to the top quote level like is done for Locations; or all stateQuotes would need to be checked
                            '8/21/2018 note: reverted code back since QQ library will now return all GLClassifications to the top level
                            If Quote.GLClassifications.IsLoaded Then ' again don't
                                For Each policyClass As QuickQuote.CommonObjects.QuickQuoteGLClassification In Quote.GLClassifications
                                    If String.IsNullOrWhiteSpace(policyClass.ClassCode) = False AndAlso policyClass.ClassCode.Substring(0, 1) = "9" Then
                                        Return True
                                    End If
                                Next
                            End If
                            'updated 8/8/2018 for multiState; now looking for classCode in any stateQuote; may need to be changed if we pull all up to top level
                            'If Me.SubQuotes IsNot Nothing Then
                            '    For Each sp In Me.SubQuotes
                            '        If sp.GLClassifications.IsLoaded() Then
                            '            For Each policyClass In sp.GLClassifications
                            '                If policyClass.ClassCode.StartsWithAny_NullSafe("9") Then
                            '                    Return True
                            '                End If
                            '            Next
                            '        End If
                            '    Next
                            'End If
                            Return False
                        End Function)
                    Dim showContractorsSection As Boolean = DetermineShowContractorsSection()
                    ViewState("showContractorSection") = showContractorsSection

                    ' Check for an Employee Benefits
                    Dim DetermineShowEmployeeBenefitsSection = New Func(Of Boolean)(
                        Function()
                            'If IsNumeric(Quote.EmployeeBenefitsLiabilityText) Then
                            '    Return True
                            'End If
                            'updated 8/8/2018 for multiState; now looking for any stateQuote to have EmployeeBenefits
                            If Me.SubQuotes IsNot Nothing Then
                                For Each sp In Me.SubQuotes
                                    If IsNumeric(sp.EmployeeBenefitsLiabilityText) Then
                                        Return True
                                    End If
                                Next
                            End If
                            Return False
                        End Function)
                    Dim showEmployeeBenefitsSection As Boolean = DetermineShowEmployeeBenefitsSection()

                    'group by section and possible hide the apartment questions when there is not a classification that has program name of 'Apartments'
                    Dim sections = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCGLUnderwritingQuestions()
                                   Where (showContractorsSection Or question.SectionName.ToUpper() <> "General Liability - Contractors".ToUpper()) And
                                       (showEmployeeBenefitsSection Or question.SectionName.ToUpper() <> "Company – Employee Benefits".ToUpper())
                                   Group question By question.SectionName
                                   Into Group
                                   Select Group

                    ' Populate sections with quote information
                    'If Me.Quote IsNot Nothing And Me.Quote.PolicyUnderwritings IsNot Nothing Then
                    'updated 8/8/2018 for multi-state
                    BindQuestions(sections, Me.SubQuoteFirst?.PolicyUnderwritings)

                    Exit Select

                Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                    Dim sections = From question In VR.Common.UWQuestions.UWQuestions.GetCommercialCPRUnderwritingQuestions()
                                   Group question By question.SectionName
                                   Into Group
                                   Select Group

                    ' Populate sections with quote information
                    'If Me.Quote IsNot Nothing And Me.Quote.PolicyUnderwritings IsNot Nothing Then
                    'updated 8/8/2018 for multi-state
                    BindQuestions(sections, Me.SubQuoteFirst?.PolicyUnderwritings)

                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    Exit Select
                Case Else
                    Throw New Exception(msgLOBNotSupported)
            End Select

            Me.FindChildVrControls()

        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' it is almost always a bad idea to do anything on page load with these VRControls
        'If Not IsPostBack Then
        'Populate() ' already done by the workflow control
        '    'Me.FindChildVrControls() ' Populate already finds its children 
        'End If
    End Sub

    Private Sub rptUWQ_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptUWQ.ItemDataBound

        Dim questions As IEnumerable(Of VR.Common.UWQuestions.VRUWQuestion) = e.Item.DataItem

        If questions.IsLoaded() Then
            Dim SectionTitleLabel As Label = e.Item.FindControl("lblAccordHeader")
            SectionTitleLabel.Text = questions.First().SectionName
            Dim subControl As ctlCommercialUWQuestionItem = e.Item.FindControl("ctlCommercialUWQuestionItem")
            subControl.MyQuestions = questions.ToList()
            subControl.Populate()
        End If

    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Visible Then
            ' Clear UW Questions in preparation for refill by repeater items
            'Me.Quote.PolicyUnderwritings = New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
            'updated 8/8/2018 for multi-state
            If Me.SubQuotes IsNot Nothing Then
                For Each sp In Me.SubQuotes
                    sp.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)()
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


End Class