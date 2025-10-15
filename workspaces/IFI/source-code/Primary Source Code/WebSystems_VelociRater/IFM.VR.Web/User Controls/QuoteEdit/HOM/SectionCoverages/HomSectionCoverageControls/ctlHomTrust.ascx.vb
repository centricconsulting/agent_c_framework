Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption

Public Class ctlHomTrust
    Inherits ctlSectionCoverageControlBase
    'Added 2/14/18 control for HOM Upgrade MLW

    Dim flaggedForDelete As Boolean = False 'added 4/6/2020 for Home Endorsements Bug 44392 MLW

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

    Public Property aiTrusteeCounter As Int32
        Get
            If ViewState("vs_aiTrusteeCounter") Is Nothing Then
                ViewState("vs_aiTrusteeCounter") = 0
            End If
            Return CInt(ViewState("vs_aiTrusteeCounter"))
        End Get
        Set(value As Int32)
            ViewState("vs_aiTrusteeCounter") = value
        End Set
    End Property

    Public Property MyAiList As List(Of QuickQuoteAdditionalInterest)
        Get
            Dim AiList As List(Of QuickQuoteAdditionalInterest) = Nothing
            If MySectionCoverage.AdditionalInterests IsNot Nothing Then
            Else
                MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            End If
            AiList = Me.MySectionCoverage.AdditionalInterests
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

    Public ReadOnly Property MyAITrustee As QuickQuoteAdditionalInterest
        Get
            If MyAiTrusteeList IsNot Nothing Then
                Return MyAiTrusteeList(Me.aiTrusteeCounter)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property HasSomeTrusteeInformation As Boolean
        Get
            'Updated 8/23/18 for multi state MLW
            'If MySectionCoverage.IsNotNull Then
            If MySectionCoverage IsNot Nothing Then
                'If MyAITrustee.IsNotNull() Then
                If MyAITrustee IsNot Nothing Then
                    'need to add the address once it has been added to the control
                    Dim fullName As String = Nothing
                    Dim arrName As Array = Nothing
                    Dim firstName As String = Nothing
                    Dim middleName As String = Nothing
                    Dim lastName As String = Nothing
                    Dim suffixName As String = Nothing

                    If MySectionCoverage IsNot Nothing Then
                        If MyAiTrusteeList IsNot Nothing AndAlso MyAiTrusteeList.Count > 0 AndAlso MyAiTrusteeList.Count > aiTrusteeCounter Then
                            fullName = MyAiTrusteeList(aiTrusteeCounter).Name.CommercialName1
                            If fullName IsNot Nothing Then
                                'Removed 5/10/18 for bug 26115 MLW
                                'If fullName.Contains("|") Then
                                '    arrName = fullName.Split("|")
                                '    If arrName.Length > 0 Then
                                '        If arrName(0) IsNot Nothing Then
                                '            firstName = arrName(0)
                                '        End If
                                '    End If
                                '    If arrName.Length > 1 Then
                                '        If arrName(1) IsNot Nothing Then
                                '            middleName = arrName(1)
                                '        End If
                                '    End If
                                '    If arrName.Length > 2 Then
                                '        If arrName(2) IsNot Nothing Then
                                '            lastName = arrName(2)
                                '        End If
                                '    End If
                                '    If arrName.Length > 3 Then
                                '        If arrName(3) IsNot Nothing Then
                                '            suffixName = arrName(3)
                                '        End If
                                '    End If
                                'Else
                                '    firstName = fullName
                                'End If
                            End If
                        End If
                    End If
                    'Updated 5/10/18 for bug 26115 MLW
                    'Return IFM.PrimativeExtensions.OneorMoreIsNotEmpty(firstName, middleName, lastName, suffixName) 'need to include the address fields as well
                    Return IFM.PrimativeExtensions.OneorMoreIsNotEmpty(fullName, MyAiTrusteeList(aiTrusteeCounter).Address.HouseNum, MyAiTrusteeList(aiTrusteeCounter).Address.StreetName, MyAiTrusteeList(aiTrusteeCounter).Address.Zip, MyAiTrusteeList(aiTrusteeCounter).Address.City, MyAiTrusteeList(aiTrusteeCounter).Address.StateId, MyAiTrusteeList(aiTrusteeCounter).Address.County)
                End If
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property HasRequiredTrusteeInformation As Boolean
        Get
            'Updated 8/23/18 for mulit state
            'If Me.MySectionCoverage.IsNotNull Then
            If Me.MySectionCoverage IsNot Nothing Then
                'If MyAITrustee.IsNotNull() Then
                If MyAITrustee IsNot Nothing Then
                    Dim hasReq As Boolean
                    Dim fullName As String = Nothing
                    Dim arrName As Array = Nothing
                    Dim firstName As String = Nothing
                    Dim lastName As String = Nothing

                    If MySectionCoverage IsNot Nothing Then
                        If MyAiTrusteeList IsNot Nothing AndAlso MyAiTrusteeList.Count > 0 AndAlso MyAiTrusteeList.Count > aiTrusteeCounter Then
                            fullName = MyAiTrusteeList(aiTrusteeCounter).Name.CommercialName1
                            'Removed 5/10/18 for bug 26115 MLW
                            'If fullName IsNot Nothing Then
                            '    If fullName.Contains("|") Then
                            '        arrName = fullName.Split("|")
                            '        If arrName.Length > 0 Then
                            '            If arrName(0) IsNot Nothing Then
                            '                firstName = arrName(0)
                            '            End If
                            '        End If
                            '        If arrName.Length > 2 Then
                            '            If arrName(2) IsNot Nothing Then
                            '                lastName = arrName(2)
                            '            End If
                            '        End If
                            '    Else
                            '        firstName = fullName
                            '    End If
                            'End If
                        End If
                    End If
                    'Updated 02/17/2020 for Home Endorsements task 44249 MLW
                    If Me.IsQuoteEndorsement Then
                        If NoneAreNullEmptyorWhitespace(fullName, MyAiTrusteeList(aiTrusteeCounter).Address.HouseNum, MyAiTrusteeList(aiTrusteeCounter).Address.StreetName, MyAiTrusteeList(aiTrusteeCounter).Address.Zip, MyAiTrusteeList(aiTrusteeCounter).Address.City, MyAiTrusteeList(aiTrusteeCounter).Address.StateId) Then
                            hasReq = True
                        Else
                            hasReq = False
                        End If
                    Else
                        'Updated 5/10/18 for bug 26115 MLW
                        'If NoneAreNullEmptyorWhitespace(firstName, lastName, MyAiTrusteeList(aiTrusteeCounter).Address.HouseNum, MyAiTrusteeList(aiTrusteeCounter).Address.StreetName, MyAiTrusteeList(aiTrusteeCounter).Address.Zip, MyAiTrusteeList(aiTrusteeCounter).Address.City, MyAiTrusteeList(aiTrusteeCounter).Address.StateId, MyAiTrusteeList(aiTrusteeCounter).Address.County) Then
                        If NoneAreNullEmptyorWhitespace(fullName, MyAiTrusteeList(aiTrusteeCounter).Address.HouseNum, MyAiTrusteeList(aiTrusteeCounter).Address.StreetName, MyAiTrusteeList(aiTrusteeCounter).Address.Zip, MyAiTrusteeList(aiTrusteeCounter).Address.City, MyAiTrusteeList(aiTrusteeCounter).Address.StateId, MyAiTrusteeList(aiTrusteeCounter).Address.County) Then
                            hasReq = True
                        Else
                            hasReq = False
                        End If
                    End If

                    Return hasReq
                End If
            End If
            Return False
        End Get
    End Property

    'Added 04/06/2020 for Home Endorsement Bug 44392 MLW
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

    'Added 04/06/2020 for Home Endorsement Bug 44392 MLW
    Public ReadOnly Property ReturnToQuoteEvent() As String
        Get
            Return "ReturnToQuoteEvent" + "_" + Quote.PolicyId + "|" + Quote.PolicyImageNum
        End Get
    End Property
    'Added 04/06/2020 for Home Endorsement Bug 44392 MLW
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
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then

            'view/edit link
            Me.VRScript.CreateJSBinding(lnkViewEdit, ctlPageStartupScript.JsEventType.onclick, "$('#{0}').show();$('#{1}').text(''); return false;".FormatIFM(Me.divTrustee.ClientID, Me.lnkViewEdit.ClientID))
            If HasRequiredTrusteeInformation = False Then
                Me.VRScript.AddScriptLine("$('#{0}').show();$('#{1}').text('');".FormatIFM(Me.divTrustee.ClientID, Me.lnkViewEdit.ClientID)) ' run at startup
            Else
                Me.VRScript.AddScriptLine("$('#{0}').hide();$('#{1}').text('View/Edit Trustee');".FormatIFM(Me.divTrustee.ClientID, Me.lnkViewEdit.ClientID)) ' run at startup
            End If
        End If

        Me.VRScript.CreateJSBinding(Me.lnkDelete.ClientID, "click", "return confirm('Are you sure you want to delete this item?');")


        'Added 04/06/2020 for Home Endorsements Bug 44392 MLW 
        Dim clearAiIdScript As String = "if ($(""#" + Me.txtIsEditable.ClientID + """).val().toString().toLowerCase() == 'false') {$(""#" + Me.txtAiId.ClientID + """).val('');}"
        Me.VRScript.CreateJSBinding(Me.txtName, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
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
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            LoadStaticData()
            Dim fullName As String = Nothing
            Dim arrName As Array = Nothing

            If MySectionCoverage IsNot Nothing Then
                If MyAiTrusteeList IsNot Nothing AndAlso MyAiTrusteeList.Count > 0 AndAlso MyAiTrusteeList.Count > aiTrusteeCounter Then
                    fullName = MyAiTrusteeList(aiTrusteeCounter).Name.CommercialName1
                    If fullName IsNot Nothing Then
                        'If fullName.Contains("|") Then
                        '    arrName = fullName.Split("|")
                        '    If arrName.Length > 0 Then
                        '        If arrName(0) IsNot Nothing Then
                        '            Me.txtTrusteeFName.Text = arrName(0)
                        '        End If
                        '    End If
                        '    If arrName.Length > 1 Then
                        '        If arrName(1) IsNot Nothing Then
                        '            Me.txtTrusteeMName.Text = arrName(1)
                        '        End If
                        '    End If
                        '    If arrName.Length > 2 Then
                        '        If arrName(2) IsNot Nothing Then
                        '            Me.txtTrusteeLName.Text = arrName(2)
                        '        End If
                        '    End If
                        '    If arrName.Length > 3 Then
                        '        If arrName(3) IsNot Nothing Then
                        '            Me.ddTrusteeSuffix.SelectedValue = arrName(3)
                        '        End If
                        '    End If
                        'Else
                        Me.txtName.Text = fullName
                        'End If
                    End If
                    'Added 04/06/2020 for Home Endorsements Bug 44392 MLW
                    Me.txtAiId.Text = MyAiTrusteeList(aiTrusteeCounter).ListId
                    If IsNumeric(MyAiTrusteeList(aiTrusteeCounter).ListId) Then
#If DEBUG Then
                        If Me.AdditionalInterestIdsCreatedThisSession.Contains(MyAiTrusteeList(aiTrusteeCounter).ListId) Then
                            Me.lblExpanderText.ToolTip = Me.lblExpanderText.Text + " Created this Session EDITABLE AI"
                        Else
                            Me.lblExpanderText.ToolTip = Me.lblExpanderText.Text + " READ-ONLY AI - Any changes will create a new AI record."
                        End If
#End If
                        ' lets js know if the Ai is updateable
                        If Me.AdditionalInterestIdsCreatedThisSession.Contains(MyAiTrusteeList(aiTrusteeCounter).ListId) Then
                            Me.txtIsEditable.Text = "true"
                        Else
                            Me.txtIsEditable.Text = "false"
                        End If
                    End If
                End If
            Else
                Me.ClearControl()
            End If

            'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
            If Me.IsQuoteReadOnly Then
                lnkDelete.Visible = False
            End If

            'Updated 04/06/2020 for Home Endorsements Bug 44392 MLW
            If MySectionCoverage IsNot Nothing Then
                'Me.ctlHomAdditionalInterestAddress.aiCounter = aiTrusteeCounter + 1
                Me.ctlHomAdditionalInterestAddress.aiCounter = MyAiList.IndexOf(MyAITrustee)
                Me.ctlHomAdditionalInterestAddress.CoverageIndex = Me.CoverageIndex
                Me.ctlHomAdditionalInterestAddress.InitFromExisting(Me)
            End If

            Me.PopulateChildControls()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            'Updated 04/06/2020 for Home Endorsements Bug 44392 MLW
            'If MySectionCoverage IsNot Nothing Then
            '    Dim ai As New QuickQuoteAdditionalInterest
            '    'ai.Name.CommercialName1 = Replace(Me.txtTrusteeFName.Text.Trim(), "|", "") & "|" & Replace(Me.txtTrusteeMName.Text.Trim(), "|", "") & "|" & Replace(Me.txtTrusteeLName.Text.Trim(), "|", "") & "|" & Me.ddTrusteeSuffix.SelectedValue
            '    ai.Name.CommercialName1 = Me.txtName.Text
            '    ai.Description = "N/A"
            '    ai.TypeId = 80 'Trustee
            '    If ai.Address IsNot Nothing Then
            '        ai.Address.StateId = "0" 'need this to initially save state to blank, will save actual state in saveChildControls() 'added for Bug 26103 MLW
            '    End If
            '    MySectionCoverage.AdditionalInterests.Add(ai)
            'End If
            'SaveChildControls()
            If MySectionCoverage IsNot Nothing Then
                If MyAiList Is Nothing Then
                    MyAiList = New List(Of QuickQuoteAdditionalInterest)()
                End If
                Dim currTrusteeCount As Integer = 0
                Dim aiTrusteeList As List(Of QuickQuoteAdditionalInterest) = MyAiTrusteeList
                If aiTrusteeList IsNot Nothing AndAlso aiTrusteeList.Count > 0 Then
                    currTrusteeCount = aiTrusteeList.Count
                End If
                If currTrusteeCount < aiTrusteeCounter + 1 Then
                    Dim aiTrusteesToAdd As Integer = (aiTrusteeCounter + 1) - currTrusteeCount
                    Do Until aiTrusteesToAdd = 0
                        Dim newAiTrustee As New QuickQuoteAdditionalInterest
                        newAiTrustee.TypeId = "80" 'Trustee
                        MyAiList.Add(newAiTrustee)
                        aiTrusteesToAdd -= 1
                        If aiTrusteesToAdd = 0 Then
                            Exit Do
                        End If
                    Loop
                End If
                If MyAITrustee IsNot Nothing AndAlso flaggedForDelete = False Then
                    If ReturnToQuoteSession = False Then ' Don't pull from form on a Return To Quote Event (this has been previously saved)
                        If (String.IsNullOrWhiteSpace(Me.txtAiId.Text) Or IsNumeric(Me.txtAiId.Text) = False) Then
                            MyAITrustee.ListId = "" ' basically lets this AI become something else
                            ' IS NEW AI
                            InjectFormIntoAI(MyAITrustee) ' persist always
                            If Me.ValidationHelper.HasErrros() = False Then
                                ' create then set new AI Id to this object
                                ' ideally we wouldn't add this until rate but it is possible that this new ai is used twice on this one app and
                                ' had we not created it then it would create two basically identical AIs at rate
                                Dim qqXml As New QuickQuoteXML()
                                Dim diamondList As Diamond.Common.Objects.Policy.AdditionalInterestList = Nothing
                                qqXml.DiamondService_CreateAdditionalInterestList(MyAITrustee, diamondList)
                                If diamondList IsNot Nothing Then
                                    If IsNumeric(MyAITrustee.ListId) Then
                                        Me.txtAiId.Text = MyAITrustee.ListId ' just in case you don't repopulate the form it will know it was already created
                                        AdditionalInterestIdsCreatedThisSession.Add(CInt(MyAITrustee.ListId))
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
                            'If loadedAi IsNot Nothing AndAlso MyAITrustee IsNot Nothing Then
                            '    If QQHelper.IsPositiveIntegerString(MyAITrustee.Num) = True Then
                            '        loadedAi.Num = CInt(MyAITrustee.Num).ToString
                            '    ElseIf QQHelper.IsPositiveIntegerString(loadedAi.Num) = False AndAlso String.IsNullOrWhiteSpace(MyAITrustee.Num) = False Then
                            '        loadedAi.Num = MyAITrustee.Num
                            '    End If
                            'End If
                            'updated 4/3/2020 to use new method that loads all base information in from MyAdditionalInterest to avoid dropping any information that may not be on VR form
                            qqXml.LoadDiamondAdditionalInterestListIntoQuickQuoteAdditionalInterest_OptionallyUseExistingAdditionalInterestForBaseInfo(Me.txtAiId.Text.Trim(), loadedAi, existingQQAdditionalInterest:=MyAITrustee)
                            If Me.AdditionalInterestIdsCreatedThisSession.Contains(CInt(Me.txtAiId.Text.Trim())) Then
                                ' was created this session it is probably safe to edit

                                ' ************* you don't want to pass in the 'interest' object above  ***********************
                                'qqXml.LoadDiamondAdditionalInterestListIntoQuickQuoteAdditionalInterest(Me.txtAiId.Text.Trim(), loadedAi)
                                If loadedAi IsNot Nothing Then
                                    InjectFormIntoAI(loadedAi)
                                    loadedAi.OverwriteAdditionalInterestListInfoForDiamondId = True ' without this it will not commit the edits

                                    ' save that loaded AI with any changes - well the changes will be saved soon
                                    MyAiList(MySectionCoverage.AdditionalInterests.IndexOf(MyAITrustee)) = loadedAi
                                End If
                            Else
                                '    MyAITrustee.ListId = "" ' basically lets this AI become something else
                                '    ' IS NEW AI
                                '    InjectFormIntoAI(MyAITrustee) ' persist always
                                '    If Me.ValidationHelper.HasErrros() = False Then
                                '        ' create then set new AI Id to this object
                                '        ' ideally we wouldn't add this until rate but it is possible that this new ai is used twice on this one app and
                                '        ' had we not created it then it would create two basically identical AIs at rate
                                '        Dim diamondList As Diamond.Common.Objects.Policy.AdditionalInterestList = Nothing
                                '        qqXml.DiamondService_CreateAdditionalInterestList(MyAITrustee, diamondList)
                                '        If diamondList IsNot Nothing Then
                                '            If IsNumeric(MyAITrustee.ListId) Then
                                '                Me.txtAiId.Text = MyAITrustee.ListId ' just in case you don't repopulate the form it will know it was already created
                                '                AdditionalInterestIdsCreatedThisSession.Add(CInt(MyAITrustee.ListId))
                                '            End If
                                '        End If
                                '    End If

                                'just update the info that isn't part of the AI List record (name, address, etc.)
                                'maintain the existing information when possible
                                If String.IsNullOrWhiteSpace(MyAITrustee.Description) = True Then
                                    MyAITrustee.Description = "N/A"
                                End If
                                If QQHelper.IsPositiveIntegerString(MyAITrustee.ListId) = False AndAlso QQHelper.IsPositiveIntegerString(Me.txtAiId.Text) = True Then
                                    MyAITrustee.ListId = Me.txtAiId.Text
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If
        Return True
    End Function

    'Added 04/06/2020 for Home Endorsement Bug 44392 MLW
    Private Sub InjectFormIntoAI(interest As QuickQuoteAdditionalInterest)
        If interest.Name IsNot Nothing Then
            interest.Name.CommercialName1 = Me.txtName.Text.Trim().ToUpper()
        End If
        'interest.Description = "N/A"
        'updated 4/13/2020 to maintain the existing information when possible
        If String.IsNullOrWhiteSpace(interest.Description) = True Then
            interest.Description = "N/A"
        End If
        interest.TypeId = "80" 'Trustee
        If interest.Address IsNot Nothing Then
            interest.Address.StateId = "0" 'need this to initially save state to blank, will save actual state in saveChildControls() 'added for Bug 26103 MLW
        End If
        Me.ctlHomAdditionalInterestAddress.InjectFormIntoAI(interest)
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = Me.CoverageName
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
            Dim aiVals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AITrustValidator.AdditionalInterestTrustValidation(MyAITrustee, valArgs.ValidationType, MySectionCoverage)
            For Each v In aiVals
                Select Case v.FieldId
                    'Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AITrustValidator.TrusteeFirstName
                    '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtTrusteeFName, v, accordList)
                    'Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AITrustValidator.TrusteeLastName
                    '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtTrusteeLName, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AITrustValidator.TrusteeName
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtName, v, accordList)
                End Select
            Next
            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            flaggedForDelete = True 'Added 04/06/2020 for Home Endorsement Bug 44392 MLW
            Me.Save_FireSaveEvent(False)
            If MySectionCoverage IsNot Nothing Then
                If MyAiTrusteeList IsNot Nothing Then
                    'Updated 04/06/2020 for Home Endorsements Bug 44392 MLW
                    'MySectionCoverage.AdditionalInterests.Remove(MySectionCoverage.AdditionalInterests(aiTrusteeCounter + 1))
                    MySectionCoverage.AdditionalInterests.Remove(MyAITrustee)
                    If MyAiTrusteeList.Count <= 0 Then
                        Me.DeleteMySectionCoverage()
                    End If
                End If
            End If
            Me.Populate_FirePopulateEvent()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Public Overrides Sub ClearControl()
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            MyBase.ClearControl()
            'Me.txtTrusteeFName.Text = ""
            'Me.txtTrusteeMName.Text = ""
            'Me.txtTrusteeLName.Text = ""
            'Me.ddTrusteeSuffix.SelectedIndex = -1
            Me.txtName.Text = ""
        End If
    End Sub
End Class