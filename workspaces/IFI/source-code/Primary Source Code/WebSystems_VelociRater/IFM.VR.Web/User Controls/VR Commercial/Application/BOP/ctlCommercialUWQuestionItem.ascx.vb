Imports IFM.PrimativeExtensions ' Added 4-30-17 Matt A

Public Class ctlCommercialUWQuestionItem
    Inherits VRControlBase

#Region "Declarations"
    'Private Const ClassName As String = "ctlCommercialUWQuestionItem" - not needed - let exceptions happen - no need to log them to database that no one looks at
    'Private msgQuoteObjNothing As String = "Quote Object is Nothing"
    'Private msgLOBNotSupported As String = "LOB Not Supported!"
#End Region

    Public MyQuestions As List(Of VR.Common.UWQuestions.VRUWQuestion) = Nothing

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()


        Select Case Me.Quote.LobType
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                'do nothing for now
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                ' Session used to track if contractor uw questions are present in Quote.PolicyUnderwritings when questions are not visible
                ' -- This prevents reevaluation for each control on save, when we only need to test it once.  
                Session("isContractorsPresent") = ""
        End Select

        Me.rptUWQ.DataSource = MyQuestions
        Me.rptUWQ.DataBind()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Visible Then
            Dim LOB As String = ""
            Dim qqItemList As New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)

            ' get Questions List
            Select Case Me.Quote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    MyQuestions = VR.Common.UWQuestions.UWQuestions.GetCommercialBOPUnderwritingQuestions()
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    MyQuestions = VR.Common.UWQuestions.UWQuestions.GetCommercialCAPUnderwritingQuestions()
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                    MyQuestions = VR.Common.UWQuestions.UWQuestions.GetCommercialWCPUnderwritingQuestions(Me.Quote.EffectiveDate)
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                    MyQuestions = VR.Common.UWQuestions.UWQuestions.GetCommercialCGLUnderwritingQuestions()
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                    MyQuestions = VR.Common.UWQuestions.UWQuestions.GetCommercialCPRUnderwritingQuestions()
            End Select

            'added 8/9/2018 for multi-state; removed 8/11/2018 to use new Property
            'Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
            'If Me.Quote IsNot Nothing Then
            '    multiStateQuotes = QQHelper.MultiStateQuickQuoteObjects(Me.Quote) 'should always return at least Me.Quote in the list
            'End If

            For Each ri As RepeaterItem In rptUWQ.Items
                Dim radioYes As RadioButton = ri.FindControl("rbYes")
                Dim radioNo As RadioButton = ri.FindControl("rbNo")
                Dim txtAdditionInfo As TextBox = ri.FindControl("txtUWQDescription")
                Dim tblAnswers As HtmlTable = ri.FindControl("tblRadioButtons")
                Dim diamondCode As String = tblAnswers.Attributes("data-diamondcode")
                LOB = tblAnswers.Attributes("data-lob")

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

            ' Add To Programatically Set Question items to an answer list.
            Select Case Me.Quote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                    ' Contractors Being Shown?
                    Dim showContractorsSection As Boolean
                    Try
                        'showContractorsSection = CBool(ViewState("showContractorSection"))
                        showContractorsSection = CBool(ParentVrControl.VrViewState("showContractorSection"))
                    Catch ex As Exception
                        showContractorsSection = False
                    End Try


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

                        'Are Contractor answers present already?
                        If String.IsNullOrEmpty(Session("isContractorsPresent")) Then
                            isContractorsPresent = isContractorsPresentCheck()
                        Else
                            Try
                                isContractorsPresent = CBool(Session("isContractorsPresent").ToString)
                            Catch ex As Exception
                                isContractorsPresent = False
                            End Try
                        End If




                        If Not isContractorsPresent Then
                            Dim qqItem As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting
                            Dim questionContractors = From c In MyQuestions
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

                Case Else
                    Exit Select
            End Select

            ' Add Answered Items from list into the Quote Object
            Select Case LOB
                Case "BOP"
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
                    Exit Select
                Case "CGL"
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
                    Exit Select
                Case Else
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
                    Exit Select
            End Select

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


    ' Matt A 4-30-17 -  LOB Abreviations should be a shared method under IFM.VR.Common.Helpers.QuickQuoteObjectHelper


End Class

