Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctlHomTrustList
    Inherits ctlSectionCoverageControlBase

    'Added 2/14/18 control for HOM Upgrade MLW   

    Dim _qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Protected ReadOnly Property HomeVersion As String
        Get
            Dim effectiveDate As DateTime
            If Me.Quote IsNot Nothing Then
                If Me.Quote.EffectiveDate IsNot Nothing AndAlso Me.Quote.EffectiveDate <> String.Empty Then
                    effectiveDate = Me.Quote.EffectiveDate
                Else
                    effectiveDate = Now()
                End If
            Else
                effectiveDate = Now()
            End If
            If _qqh.doUseNewVersionOfLOB(Quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property

    Public Property aiTrustCounter As Int32
        Get
            If ViewState("vs_aiTrustCounter") Is Nothing Then
                ViewState("vs_aiTrustCounter") = 0
            End If
            Return CInt(ViewState("vs_aiTrustCounter"))
        End Get
        Set(value As Int32)
            ViewState("vs_aiTrustCounter") = value
        End Set
    End Property

    Public Property MyAiList As List(Of QuickQuoteAdditionalInterest)
        Get
            Dim AiList As List(Of QuickQuoteAdditionalInterest) = Nothing
            If MySectionCoverage.AdditionalInterests IsNot Nothing Then
            Else
                MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            End If
            AiList = MySectionCoverage.AdditionalInterests
            Return AiList
        End Get
        Set(value As List(Of QuickQuoteAdditionalInterest))
            MySectionCoverage.AdditionalInterests = value
        End Set
    End Property

    Public Property MyAiTrusteeList As List(Of QuickQuoteAdditionalInterest)
        Get
            Dim AiTrusteeList As List(Of QuickQuoteAdditionalInterest) = New List(Of QuickQuoteAdditionalInterest)
            If MySectionCoverage.AdditionalInterests Is Nothing Then
                MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            End If
            For Each ai In MySectionCoverage.AdditionalInterests
                If ai.TypeId = "80" Then
                    AiTrusteeList.Add(ai)
                End If
            Next
            Return AiTrusteeList
        End Get
        Set(value As List(Of QuickQuoteAdditionalInterest))
            'MySectionCoverage.AdditionalInterests = value
        End Set
    End Property

    Public Property MyAiTrustList As List(Of QuickQuoteAdditionalInterest)
        Get
            Dim AiTrustList As List(Of QuickQuoteAdditionalInterest) = New List(Of QuickQuoteAdditionalInterest)
            'Dim AiTrustList As List(Of QuickQuoteAdditionalInterest) = Nothing
            If MySectionCoverage.AdditionalInterests Is Nothing Then
                MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            End If
            For Each ai In MySectionCoverage.AdditionalInterests
                If ai.TypeId = "79" Then
                    AiTrustList.Add(ai)
                End If
            Next
            Return AiTrustList
        End Get
        Set(value As List(Of QuickQuoteAdditionalInterest))
            'Me.Quote.Locations(0).AdditionalInterests = value
            'MySectionCoverage.AdditionalInterests = value
        End Set
    End Property


    Public ReadOnly Property MyAITrust As QuickQuoteAdditionalInterest
        Get
            If MyAiTrustList IsNot Nothing Then
                Return MyAiTrustList(Me.aiTrustCounter)
            End If
            Return Nothing
        End Get
    End Property

    'Added 04/03/2020 for Home Endorsement Bug 44392 MLW
    Public ReadOnly Property AdditionalInterestIdsCreatedThisSession As List(Of Int32)
        Get
            'If Session(Me.QuoteId + "_AI_Created_List") Is Nothing Then
            '    Session(Me.QuoteId + "_AI_Created_List") = New List(Of Int32)
            'End If

            'Return Session(Me.QuoteId + "_AI_Created_List")
            'updated 2/16/2021
            If Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List") Is Nothing Then
                Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List") = New List(Of Int32)
            End If

            Return Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List")
        End Get
    End Property

    'Added 04/03/2020 for Home Endorsement Bug 44392 MLW
    Public ReadOnly Property ReturnToQuoteEvent() As String
        Get
            Return "ReturnToQuoteEvent" + "_" + Quote.PolicyId + "|" + Quote.PolicyImageNum
        End Get
    End Property
    'Added 04/03/2020 for Home Endorsement Bug 44392 MLW
    Public Property ReturnToQuoteSession() As Boolean
        Get
            Dim ReturnToQuoteEventBool As Boolean
            If Session IsNot Nothing AndAlso Session(ReturnToQuoteEvent) IsNot Nothing Then
                If Boolean.TryParse(Session(ReturnToQuoteEvent).ToString, ReturnToQuoteEventBool) = False Then
                    ReturnToQuoteEventBool = False
                End If
            End If
            Return ReturnToQuoteEventBool
        End Get
        Set(ByVal value As Boolean)
            If Session IsNot Nothing AndAlso Session(ReturnToQuoteEvent) IsNot Nothing Then
                Session(ReturnToQuoteEvent) = value
            Else
                Session.Add(ReturnToQuoteEvent, value)
            End If
        End Set
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Added 04/03/2020 for Home Endorsements Bug 44392 MLW 
        Dim clearAiIdScript As String = "if ($(""#" + Me.txtIsEditable.ClientID + """).val().toString().toLowerCase() == 'false') {$(""#" + Me.txtAiId.ClientID + """).val('');}"
        Me.VRScript.CreateJSBinding(Me.txtTrustName, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.ctlHomAdditionalInterestAddress.HouseNumTextBox, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.ctlHomAdditionalInterestAddress.StreetNameTextBox, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.ctlHomAdditionalInterestAddress.ApartmentNumTextBox, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.ctlHomAdditionalInterestAddress.ZipCodeTextBox, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.ctlHomAdditionalInterestAddress.CityTextBox, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.ctlHomAdditionalInterestAddress.StateDropDown, ctlPageStartupScript.JsEventType.onchange, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.ctlHomAdditionalInterestAddress.CountyTextBox, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        '#If Not DEBUG Then
        Me.txtAiId.Attributes.Add("style", "display:none;")
        Me.lblExpanderText.Attributes.Add("style", "display:none;")
        '#End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        'Updated 04/03/2020 for Home Endorsements Bug 44392 MLW
        'If MySectionCoverage IsNot Nothing Then
        '    If MyAiTrustList IsNot Nothing Then
        '        If MySectionCoverage.AdditionalInterests.Count > 0 Then
        '            If MySectionCoverage.AdditionalInterests(aiTrustCounter).TypeId = "79" Then
        '                Me.txtTrustName.Text = MySectionCoverage.AdditionalInterests(aiTrustCounter).Name.CommercialName1
        '            End If
        '        End If
        '    End If
        '    If MyAiTrusteeList IsNot Nothing OrElse MyAiTrusteeList.Count > 0 Then
        '        Me.Repeater1.DataSource = MyAiTrusteeList
        '    Else
        '        Dim dummyListTrustee As New List(Of QuickQuoteAdditionalInterest)
        '        Dim dummyItemTrustee As New QuickQuoteAdditionalInterest
        '        dummyItemTrustee.Address.StateId = "0" 'added 4/16/18 for Bug 26103 MLW
        '        dummyItemTrustee.TypeId = "80" 'Trustee
        '        dummyListTrustee.Add(dummyItemTrustee)
        '        Me.Repeater1.DataSource = dummyListTrustee
        '    End If
        'Else
        '    'need to create a dummy list to show the initial fields since the control is not initializing it like the other controls - not a part of the coverage, but part of a list
        '    Dim dummyList As New List(Of QuickQuoteAdditionalInterest)
        '    Dim dummyItem As New QuickQuoteAdditionalInterest
        '    dummyItem.Address.StateId = "0" 'added 4/16/18 for Bug 26103 MLW
        '    dummyItem.TypeId = "80" 'Trustee
        '    dummyList.Add(dummyItem)
        '    Me.Repeater1.DataSource = dummyList
        'End If
        'Me.Repeater1.DataBind()
        'Me.FindChildVrControls()
        'Dim index As Int32 = 0
        'For Each c In Me.GatherChildrenOfType(Of ctlHomTrust)
        '    c.InitFromExisting(Me)
        '    If MySectionCoverage IsNot Nothing Then
        '        If MyAiTrusteeList IsNot Nothing Then
        '            c.aiTrusteeCounter = index
        '            index += 1
        '        End If
        '    End If
        'Next

        ''Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
        'If Me.IsQuoteReadOnly Then
        '    lnkAddAddress.Visible = False
        'End If

        'Me.ctlHomAdditionalInterestAddress.aiCounter = aiTrustCounter
        'Me.ctlHomAdditionalInterestAddress.CoverageIndex = Me.CoverageIndex
        'Me.ctlHomAdditionalInterestAddress.InitFromExisting(Me)

        'Me.PopulateChildControls()
        If MySectionCoverage IsNot Nothing Then
            If MyAiTrustList IsNot Nothing AndAlso MyAiTrustList.Count > 0 Then
                'If MySectionCoverage.AdditionalInterests.Count > 0 Then
                If MyAiTrustList(aiTrustCounter).TypeId = "79" Then
                    Me.txtTrustName.Text = MyAiTrustList(aiTrustCounter).Name.CommercialName1
                    Me.txtAiId.Text = MyAiTrustList(aiTrustCounter).ListId
                    If IsNumeric(MyAiTrustList(aiTrustCounter).ListId) Then
#If DEBUG Then
                        If Me.AdditionalInterestIdsCreatedThisSession.Contains(MyAiTrustList(aiTrustCounter).ListId) Then
                            Me.lblExpanderText.ToolTip = Me.lblExpanderText.Text + " Created this Session EDITABLE AI"
                        Else
                            Me.lblExpanderText.ToolTip = Me.lblExpanderText.Text + " READ-ONLY AI - Any changes will create a new AI record."
                        End If
#End If
                        ' lets js know if the Ai is updateable
                        If Me.AdditionalInterestIdsCreatedThisSession.Contains(MyAiTrustList(aiTrustCounter).ListId) Then
                            Me.txtIsEditable.Text = "true"
                        Else
                            Me.txtIsEditable.Text = "false"
                        End If
                    End If
                End If
                'End If
            Else
                If MyAiTrustList Is Nothing OrElse MyAiTrustList.Count = 0 Then
                    Dim dummyItem As New QuickQuoteAdditionalInterest
                    dummyItem.Address.StateId = "0" 'Need to initially save to blank, will save again in child control - added 4/30/18 for Bug 26344 MLW
                    dummyItem.TypeId = "79" 'Trust
                    MySectionCoverage.AdditionalInterests.Add(dummyItem)
                End If
            End If
            If MyAiTrusteeList IsNot Nothing OrElse MyAiTrusteeList.Count > 0 Then
                Me.Repeater1.DataSource = MyAiTrusteeList
            Else
                Dim dummyListTrustee As New List(Of QuickQuoteAdditionalInterest)
                Dim dummyItemTrustee As New QuickQuoteAdditionalInterest
                dummyItemTrustee.Address.StateId = "0" 'added 4/16/18 for Bug 26103 MLW
                dummyItemTrustee.TypeId = "80" 'Trustee
                dummyListTrustee.Add(dummyItemTrustee)
                Me.Repeater1.DataSource = dummyListTrustee
            End If
        Else
            'need to create a dummy list to show the initial fields since the control is not initializing it like the other controls - not a part of the coverage, but part of a list
            Dim dummyList As New List(Of QuickQuoteAdditionalInterest)
            Dim dummyItem As New QuickQuoteAdditionalInterest
            dummyItem.Address.StateId = "0" 'added 4/16/18 for Bug 26103 MLW
            dummyItem.TypeId = "80" 'Trustee
            dummyList.Add(dummyItem)
            Me.Repeater1.DataSource = dummyList
        End If
        Me.Repeater1.DataBind()
        Me.FindChildVrControls()
        Dim index As Int32 = 0
        For Each c In Me.GatherChildrenOfType(Of ctlHomTrust)
            c.InitFromExisting(Me)
            If MySectionCoverage IsNot Nothing Then
                If MyAiTrusteeList IsNot Nothing Then
                    c.aiTrusteeCounter = index
                    index += 1
                End If
            End If
        Next

        'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
        If Me.IsQuoteReadOnly Then
            lnkAddAddress.Visible = False
        End If

        'Updated 04/06/2020 for Home Endorsements Bug 44392 MLW
        If MySectionCoverage IsNot Nothing Then
            'Me.ctlHomAdditionalInterestAddress.aiCounter = aiTrustCounter
            Me.ctlHomAdditionalInterestAddress.aiCounter = MyAiList.IndexOf(MyAITrust)
            Me.ctlHomAdditionalInterestAddress.CoverageIndex = Me.CoverageIndex
            Me.ctlHomAdditionalInterestAddress.InitFromExisting(Me)
        End If

        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        ''ai.TypeId = 79 'Trust
        ''ai.TypeId = 80 'Trustee
        'Dim aiTrust As New QuickQuoteAdditionalInterest
        'aiTrust.Name.CommercialName1 = Me.txtTrustName.Text.Trim()
        'aiTrust.Description = "N/A"
        'aiTrust.TypeId = 79 'Trust

        'If MySectionCoverage Is Nothing Then
        '    Me.CreateMySectionCoverage()
        'End If
        'If MySectionCoverage IsNot Nothing Then
        '    MySectionCoverage.AdditionalInterests = Nothing
        '    MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
        '    If aiTrust.Address IsNot Nothing Then
        '        aiTrust.Address.StateId = "0" 'added 4/16/18 for Bug 26103 MLW
        '    End If
        '    MySectionCoverage.AdditionalInterests.Add(aiTrust)
        'End If
        'Me.SaveChildControls()
        'Return True

        If MySectionCoverage Is Nothing Then
            Me.CreateMySectionCoverage()
        End If
        If MySectionCoverage IsNot Nothing Then
            If MyAiList Is Nothing Then
                MyAiList = New List(Of QuickQuoteAdditionalInterest)()
            End If
            Dim currTrustCount As Integer = 0
            Dim aiTrustList As List(Of QuickQuoteAdditionalInterest) = MyAiTrustList
            If aiTrustList IsNot Nothing AndAlso aiTrustList.Count > 0 Then
                currTrustCount = aiTrustList.Count
            End If
            If currTrustCount < aiTrustCounter + 1 Then
                Dim aiTrustsToAdd As Integer = (aiTrustCounter + 1) - currTrustCount
                Do Until aiTrustsToAdd = 0
                    Dim newAiTrust As New QuickQuoteAdditionalInterest
                    newAiTrust.TypeId = "79" 'Trust
                    MyAiList.Add(newAiTrust)
                    aiTrustsToAdd -= 1
                    If aiTrustsToAdd = 0 Then
                        Exit Do
                    End If
                Loop
            End If
            If MyAITrust IsNot Nothing Then
                If ReturnToQuoteSession = False Then ' Don't pull from form on a Return To Quote Event (this has been previously saved)
                    If (String.IsNullOrWhiteSpace(Me.txtAiId.Text) Or IsNumeric(Me.txtAiId.Text) = False) Then
                        MyAITrust.ListId = "" ' basically lets this AI become something else
                        ' IS NEW AI
                        InjectFormIntoAI(MyAITrust) ' persist always
                        If Me.ValidationHelper.HasErrros() = False Then
                            ' create then set new AI Id to this object
                            ' ideally we wouldn't add this until rate but it is possible that this new ai is used twice on this one app and
                            ' had we not created it then it would create two basically identical AIs at rate
                            Dim qqXml As New QuickQuoteXML()
                            Dim diamondList As Diamond.Common.Objects.Policy.AdditionalInterestList = Nothing
                            qqXml.DiamondService_CreateAdditionalInterestList(MyAITrust, diamondList)
                            If diamondList IsNot Nothing Then
                                If IsNumeric(MyAITrust.ListId) Then
                                    Me.txtAiId.Text = MyAITrust.ListId ' just in case you don't repopulate the form it will know it was already created
                                    AdditionalInterestIdsCreatedThisSession.Add(CInt(MyAITrust.ListId))
                                    Me.txtIsEditable.Text = "true" 'added 2/16/2021
                                End If
                            End If
                        End If
                    Else
                        ' already existed
                        Dim qqXml As New QuickQuoteXML()
                        Dim loadedAi As QuickQuoteAdditionalInterest = Nothing

                        ' ************* you don't want to pass in the 'interest' object above  ***********************
                        'qqXml.LoadDiamondAdditionalInterestListIntoQuickQuoteAdditionalInterest(Me.txtAiId.Text.Trim(), loadedAi)
                        'If loadedAi IsNot Nothing AndAlso MyAITrust IsNot Nothing Then
                        '    If QQHelper.IsPositiveIntegerString(MyAITrust.Num) = True Then
                        '        loadedAi.Num = CInt(MyAITrust.Num).ToString
                        '    ElseIf QQHelper.IsPositiveIntegerString(loadedAi.Num) = False AndAlso String.IsNullOrWhiteSpace(MyAITrust.Num) = False Then
                        '        loadedAi.Num = MyAITrust.Num
                        '    End If
                        'End If
                        'updated 4/3/2020 to use new method that loads all base information in from MyAdditionalInterest to avoid dropping any information that may not be on VR form
                        qqXml.LoadDiamondAdditionalInterestListIntoQuickQuoteAdditionalInterest_OptionallyUseExistingAdditionalInterestForBaseInfo(Me.txtAiId.Text.Trim(), loadedAi, existingQQAdditionalInterest:=MyAITrust)
                        If Me.AdditionalInterestIdsCreatedThisSession.Contains(CInt(Me.txtAiId.Text.Trim())) Then
                            ' was created this session it is probably safe to edit

                            ' ************* you don't want to pass in the 'interest' object above  ***********************
                            'qqXml.LoadDiamondAdditionalInterestListIntoQuickQuoteAdditionalInterest(Me.txtAiId.Text.Trim(), loadedAi)
                            If loadedAi IsNot Nothing Then
                                InjectFormIntoAI(loadedAi)
                                loadedAi.OverwriteAdditionalInterestListInfoForDiamondId = True ' without this it will not commit the edits

                                ' save that loaded AI with any changes - well the changes will be saved soon
                                MyAiList(MySectionCoverage.AdditionalInterests.IndexOf(MyAITrust)) = loadedAi
                            End If
                        Else
                            '    MyAITrust.ListId = "" ' basically lets this AI become something else
                            '    ' IS NEW AI
                            '    InjectFormIntoAI(MyAITrust) ' persist always
                            '    If Me.ValidationHelper.HasErrros() = False Then
                            '        ' create then set new AI Id to this object
                            '        ' ideally we wouldn't add this until rate but it is possible that this new ai is used twice on this one app and
                            '        ' had we not created it then it would create two basically identical AIs at rate
                            '        Dim diamondList As Diamond.Common.Objects.Policy.AdditionalInterestList = Nothing
                            '        qqXml.DiamondService_CreateAdditionalInterestList(MyAITrust, diamondList)
                            '        If diamondList IsNot Nothing Then
                            '            If IsNumeric(MyAITrust.ListId) Then
                            '                Me.txtAiId.Text = MyAITrust.ListId ' just in case you don't repopulate the form it will know it was already created
                            '                AdditionalInterestIdsCreatedThisSession.Add(CInt(MyAITrust.ListId))
                            '            End If
                            '        End If
                            '    End If

                            'just update the info that isn't part of the AI List record (name, address, etc.)
                            'maintain the existing information when possible
                            If String.IsNullOrWhiteSpace(MyAITrust.Description) = True Then
                                MyAITrust.Description = "N/A"
                            End If
                            If QQHelper.IsPositiveIntegerString(MyAITrust.ListId) = False AndAlso QQHelper.IsPositiveIntegerString(Me.txtAiId.Text) = True Then
                                MyAITrust.ListId = Me.txtAiId.Text
                            End If
                        End If
                    End If
                End If
            End If
        End If
        Me.SaveChildControls()
        Return True
    End Function

    'Added 04/03/2020 for Home Endorsement Bug 44392 MLW
    Private Sub InjectFormIntoAI(interest As QuickQuoteAdditionalInterest)
        If interest.Name IsNot Nothing Then
            interest.Name.CommercialName1 = Me.txtTrustName.Text.Trim().ToUpper()
        End If
        'interest.Description = "N/A"
        'updated 4/13/2020 to maintain the existing information when possible
        If String.IsNullOrWhiteSpace(interest.Description) = True Then
            interest.Description = "N/A"
        End If
        interest.TypeId = "79" 'Trust
        interest.Address.StateId = "0" 'added 4/16/18 for Bug 26103 MLW
        Me.ctlHomAdditionalInterestAddress.InjectFormIntoAI(interest)
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = Me.CoverageName
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
            Dim aiVals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AITrustValidator.AdditionalInterestTrustValidation(MyAITrust, valArgs.ValidationType, MySectionCoverage)
            For Each v In aiVals
                Select Case v.FieldId
                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AITrustValidator.TrustName
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtTrustName, v, accordList)
                End Select
            Next
            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Protected Sub lnkAddAddress_Click(sender As Object, e As EventArgs) Handles lnkAddAddress.Click
        If MySectionCoverage Is Nothing Then
            'coverage could be nothing since we are not init the coverage in the control - not being passed, need to create it
            Me.CreateMySectionCoverage()
        End If
        Me.Save_FireSaveEvent(False) 'need to save first
        Dim ai As QuickQuoteAdditionalInterest = New QuickQuoteAdditionalInterest
        ai.Name.CommercialName1 = ""
        ai.Description = "N/A"
        ai.TypeId = "80" 'Trustee
        If MySectionCoverage.AdditionalInterests IsNot Nothing Then
            If ai.Address IsNot Nothing Then
                ai.Address.StateId = "0" 'added 4/16/18 for Bug 26103 MLW
            End If
            MySectionCoverage.AdditionalInterests.Add(ai)
        Else
            MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            If ai.Address IsNot Nothing Then
                ai.Address.StateId = "0" 'added 4/16/18 for Bug 26103 MLW
            End If
            MySectionCoverage.AdditionalInterests.Add(ai)
        End If
        'Updated 04/03/2020 for Home Endorsements Bug 44302 MLW
        'Me.Populate_FirePopulateEvent()
        'Me.Save_FireSaveEvent(False) 'need to save again
        Me.Populate()
    End Sub
End Class