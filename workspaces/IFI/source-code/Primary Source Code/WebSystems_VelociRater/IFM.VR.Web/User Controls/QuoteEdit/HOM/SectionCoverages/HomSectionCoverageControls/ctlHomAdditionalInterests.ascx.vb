Imports IFM.PrimativeExtensions
Imports IFM.VR.Validation
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctlHomAdditionalInterests
    Inherits ctlSectionCoverageControlBase

    'Added 2/2/18 control for HOM Upgrade MLW
    Dim _qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    'Private QuickQuoteSectionIAndIIICoverage As Object

    Dim flaggedForDelete As Boolean = False 'added 4/1/2020 for Home Endorsements Bug 44392 MLW

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

    Public Property aiCounter As Int32
        Get
            If ViewState("vs_aiCounter") Is Nothing Then
                ViewState("vs_aiCounter") = 0
            End If
            Return CInt(ViewState("vs_aiCounter"))
        End Get
        Set(value As Int32)
            ViewState("vs_aiCounter") = value
        End Set
    End Property

    Public Property MyAiList As List(Of QuickQuoteAdditionalInterest)
        Get
            Dim AiList As List(Of QuickQuoteAdditionalInterest) = Nothing
            AiList = Me.MySectionCoverage.AdditionalInterests
            Return AiList
        End Get
        Set(value As List(Of QuickQuoteAdditionalInterest))
            Me.Quote.Locations(0).AdditionalInterests = value
        End Set
    End Property

    Public ReadOnly Property MyAdditionalInterest As QuickQuoteAdditionalInterest
        Get
            If MyAiList IsNot Nothing Then
                Return MyAiList(Me.aiCounter)
            End If
            Return Nothing
        End Get
    End Property

    'Added 03/24/2020 for Home Endorsement Bug 44392 MLW
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
    'Added 03/24/2020 for Home Endorsement Bug 44392 MLW
    Public ReadOnly Property ReturnToQuoteEvent() As String
        Get
            Return "ReturnToQuoteEvent" + "_" + Quote.PolicyId + "|" + Quote.PolicyImageNum
        End Get
    End Property
    'Added 03/24/2020 for Home Endorsement Bug 44392 MLW
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
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
            Me.VRScript.CreateJSBinding(lnkAddAdress, ctlPageStartupScript.JsEventType.onclick, "$('#{0}').show(); $('#{1}').text('View/Edit Address'); return false;".FormatIFM(Me.divAddress.ClientID, Me.lnkAddAdress.ClientID))
            If Me.ctlHomAdditionalInterestAddress.ValidationHelper.ItemsWereCopiedToOtherValidationhelper = False And Me.ctlHomAdditionalInterestAddress.HasSomeAddressinformation = False Then
                Me.VRScript.AddScriptLine("$('#{0}').hide();".FormatIFM(Me.divAddress.ClientID)) ' run at startup
            Else

                Me.VRScript.AddScriptLine("$('#{1}').text('View/Edit Address');".FormatIFM(Me.divAddress.ClientID, Me.lnkAddAdress.ClientID)) ' run at startup
                If Me.ctlHomAdditionalInterestAddress.ValidationHelper.ItemsWereCopiedToOtherValidationhelper = False Then
                    Me.VRScript.AddScriptLine("$('#{0}').hide();".FormatIFM(Me.divAddress.ClientID)) ' run at startup
                End If
            End If
        End If

        Me.VRScript.CreateJSBinding(Me.lnkDelete.ClientID, "click", "return confirm('Are you sure you want to delete this item?');")

        'Added 03/24/2020 for Home Endorsements Bug 44392 MLW 
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
        LoadStaticData()
        'Updated 03/24/2020 for Home Endorsement Bug 44392 MLW
        'If MySectionCoverage IsNot Nothing Then
        '    If MySectionCoverage.AdditionalInterests IsNot Nothing AndAlso MySectionCoverage.AdditionalInterests.Count > 0 AndAlso MySectionCoverage.AdditionalInterests.Count > aiCounter Then
        '        Me.txtName.Text = MySectionCoverage.AdditionalInterests(aiCounter).Name.CommercialName1
        '        Me.txtDescription.Text = MySectionCoverage.AdditionalInterests(aiCounter).Description
        '    End If
        'Else
        '    Me.ClearControl()
        'End If
        If MySectionCoverage IsNot Nothing Then
            If MySectionCoverage.AdditionalInterests IsNot Nothing AndAlso MySectionCoverage.AdditionalInterests.Count > 0 AndAlso MySectionCoverage.AdditionalInterests.Count > aiCounter Then
                Me.txtAiId.Text = MyAdditionalInterest.ListId
                Me.txtName.Text = MyAdditionalInterest.Name.CommercialName1
                If IsNumeric(MyAdditionalInterest.ListId) Then
#If DEBUG Then
                    If Me.AdditionalInterestIdsCreatedThisSession.Contains(MyAdditionalInterest.ListId) Then
                        Me.lblExpanderText.ToolTip = Me.lblExpanderText.Text + " Created this Session EDITABLE AI"
                    Else
                        Me.lblExpanderText.ToolTip = Me.lblExpanderText.Text + " READ-ONLY AI - Any changes will create a new AI record."
                    End If
#End If
                    ' lets js know if the Ai is updateable
                    If Me.AdditionalInterestIdsCreatedThisSession.Contains(MyAdditionalInterest.ListId) Then
                        Me.txtIsEditable.Text = "true"
                    Else
                        Me.txtIsEditable.Text = "false"
                    End If
                End If
                Me.txtDescription.Text = MyAdditionalInterest.Description
            End If
        Else
            Me.ClearControl()
        End If

        'If Me.SectionCoverageIAndIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence Then
        '    lblName.Text = "*Name of Student:"
        '    lblDescription.Text = "*Name of School:"
        'End If
        If Me.SectionCoverageIAndIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage Then
            lblName.Text = "*Name of Relative:"
            lblDescription.Text = "*Name of Residency:"
        End If

        'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
        If Me.IsQuoteReadOnly Then
            lnkDelete.Visible = False
        End If

        Me.ctlHomAdditionalInterestAddress.aiCounter = aiCounter
        Me.ctlHomAdditionalInterestAddress.CoverageIndex = Me.CoverageIndex
        Me.ctlHomAdditionalInterestAddress.InitFromExisting(Me)

        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            'Updated 03/23/2020 for Home Endorsements Bug 44392 MLW
            'If MySectionCoverage IsNot Nothing Then
            '    Dim ai As New QuickQuoteAdditionalInterest
            '    ai.Name.CommercialName1 = Me.txtName.Text.Trim()
            '    ai.Description = Me.txtDescription.Text.Trim()
            '    ai.TypeId = 81 'Relative
            '    If MySectionCoverage.AdditionalInterests IsNot Nothing Then
            '        'assisted living is only for IN, all others want blank state id - for bug 26103 4/16/18 MLW
            '        If Me.SectionCoverageIAndIIEnum <> QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage Then
            '            If ai.Address IsNot Nothing Then
            '                ai.Address.StateId = "0" 'need this to initially save state to blank, will save actual state in saveChildControls() - for Bug 26103 4/16/18 MLW
            '            End If
            '        Else
            '            If ai.Address IsNot Nothing Then
            '                ai.Address.StateId = "16" 'need this to initially save state to blank, will save actual state in saveChildControls() - for Bug 26103 4/16/18 MLW
            '            End If
            '        End If
            '        MySectionCoverage.AdditionalInterests.Add(ai)
            '    End If
            'End If
            If MySectionCoverage IsNot Nothing Then
                If MyAiList Is Nothing Then
                    MyAiList = New List(Of QuickQuoteAdditionalInterest)()
                End If
                While (MyAiList.Count < aiCounter + 1)
                    MyAiList.Add(New QuickQuoteAdditionalInterest())
                End While
                If MyAdditionalInterest IsNot Nothing AndAlso flaggedForDelete = False Then
                    If ReturnToQuoteSession = False Then ' Don't pull from form on a Return To Quote Event (this has been previously saved)
                        If (String.IsNullOrWhiteSpace(Me.txtAiId.Text) Or IsNumeric(Me.txtAiId.Text) = False) Then
                            MyAdditionalInterest.ListId = "" ' basically lets this AI become something else
                            ' IS NEW AI
                            InjectFormIntoAI(MyAdditionalInterest) ' persist always
                            If Me.ValidationHelper.HasErrros() = False Then
                                ' create then set new AI Id to this object
                                ' ideally we wouldn't add this until rate but it is possible that this new ai is used twice on this one app and
                                ' had we not created it then it would create two basically identical AIs at rate
                                Dim qqXml As New QuickQuoteXML()
                                Dim diamondList As Diamond.Common.Objects.Policy.AdditionalInterestList = Nothing
                                qqXml.DiamondService_CreateAdditionalInterestList(MyAdditionalInterest, diamondList)
                                If diamondList IsNot Nothing Then
                                    If IsNumeric(MyAdditionalInterest.ListId) Then
                                        Me.txtAiId.Text = MyAdditionalInterest.ListId ' just in case you don't repopulate the form it will know it was already created
                                        AdditionalInterestIdsCreatedThisSession.Add(CInt(MyAdditionalInterest.ListId))
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
                            'If loadedAi IsNot Nothing AndAlso MyAdditionalInterest IsNot Nothing Then
                            '    If QQHelper.IsPositiveIntegerString(MyAdditionalInterest.Num) = True Then
                            '        loadedAi.Num = CInt(MyAdditionalInterest.Num).ToString
                            '    ElseIf QQHelper.IsPositiveIntegerString(loadedAi.Num) = False AndAlso String.IsNullOrWhiteSpace(MyAdditionalInterest.Num) = False Then
                            '        loadedAi.Num = MyAdditionalInterest.Num
                            '    End If
                            'End If
                            'updated 4/3/2020 to use new method that loads all base information in from MyAdditionalInterest to avoid dropping any information that may not be on VR form
                            qqXml.LoadDiamondAdditionalInterestListIntoQuickQuoteAdditionalInterest_OptionallyUseExistingAdditionalInterestForBaseInfo(Me.txtAiId.Text.Trim(), loadedAi, existingQQAdditionalInterest:=MyAdditionalInterest)
                            If Me.AdditionalInterestIdsCreatedThisSession.Contains(CInt(Me.txtAiId.Text.Trim())) Then
                                ' was created this session it is probably safe to edit

                                ' ************* you don't want to pass in the 'interest' object above  ***********************
                                'qqXml.LoadDiamondAdditionalInterestListIntoQuickQuoteAdditionalInterest(Me.txtAiId.Text.Trim(), loadedAi)
                                If loadedAi IsNot Nothing Then
                                    InjectFormIntoAI(loadedAi)
                                    loadedAi.OverwriteAdditionalInterestListInfoForDiamondId = True ' without this it will not commit the edits

                                    ' save that loaded AI with any changes - well the changes will be saved soon
                                    MyAiList(aiCounter) = loadedAi
                                End If
                            Else
                                '    MyAdditionalInterest.ListId = "" ' basically lets this AI become something else
                                '    ' IS NEW AI
                                '    InjectFormIntoAI(MyAdditionalInterest) ' persist always
                                '    If Me.ValidationHelper.HasErrros() = False Then
                                '        ' create then set new AI Id to this object
                                '        ' ideally we wouldn't add this until rate but it is possible that this new ai is used twice on this one app and
                                '        ' had we not created it then it would create two basically identical AIs at rate
                                '        Dim diamondList As Diamond.Common.Objects.Policy.AdditionalInterestList = Nothing
                                '        qqXml.DiamondService_CreateAdditionalInterestList(MyAdditionalInterest, diamondList)
                                '        If diamondList IsNot Nothing Then
                                '            If IsNumeric(MyAdditionalInterest.ListId) Then
                                '                Me.txtAiId.Text = MyAdditionalInterest.ListId ' just in case you don't repopulate the form it will know it was already created
                                '                AdditionalInterestIdsCreatedThisSession.Add(CInt(MyAdditionalInterest.ListId))
                                '            End If
                                '        End If
                                '    End If

                                'just update the info that isn't part of the AI List record (name, address, etc.)
                                MyAdditionalInterest.Description = Me.txtDescription.Text.Trim()
                            End If

                        End If
                    End If
                End If
            End If
        End If
        Return True
    End Function

    'Added 03/24/2020 for Home Endorsement Bug 44392 MLW
    Private Sub InjectFormIntoAI(interest As QuickQuoteAdditionalInterest)
        If interest.Name IsNot Nothing Then
            interest.Name.CommercialName1 = Me.txtName.Text.Trim().ToUpper()
        End If
        interest.Description = Me.txtDescription.Text.Trim()
        interest.TypeId = 81 'Relative
        If Me.SectionCoverageIAndIIEnum <> QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage Then
            If interest.Address IsNot Nothing Then
                interest.Address.StateId = "0" 'need this to initially save state to blank, will save actual state in saveChildControls() - for Bug 26103 4/16/18 MLW
            End If
        Else
            If interest.Address IsNot Nothing Then
                interest.Address.StateId = "16" 'need this to initially save state to blank, will save actual state in saveChildControls() - for Bug 26103 4/16/18 MLW
            End If
        End If
        Me.ctlHomAdditionalInterestAddress.InjectFormIntoAI(interest)
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = Me.CoverageName
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
            Dim aiVals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AdditionalInterestRelativeValidation(MyAdditionalInterest, valArgs.ValidationType, MySectionCoverage)
            For Each v In aiVals
                Select Case v.FieldId
                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.CommercialName
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtName, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.Description
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescription, v, accordList)
                        'Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.StreetAndPoBoxEmpty
                        '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                        '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreet, v, accordList)
                        '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBOX, v, accordList)
                        'Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.StreetAndPoxBoxAreSet
                        '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                        '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreet, v, accordList)
                        '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBOX, v, accordList)
                        'Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.HouseNumberID
                        '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                        'Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.StreetNameID
                        '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreet, v, accordList)
                        'Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.POBOXID
                        '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBOX, v, accordList)
                        'Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.ZipCodeID
                        '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtZipCode, v, accordList)
                        'Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.CityID
                        '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCity, v, accordList)
                        'Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.StateID
                        '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, accordList)
                End Select
            Next
            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            flaggedForDelete = True 'Added 04/01/2020 for Home Endorsement Bug 44392 MLW
            Me.Save_FireSaveEvent(False)
            If MySectionCoverage IsNot Nothing Then
                If MySectionCoverage.AdditionalInterests IsNot Nothing Then
                    Me.MySectionCoverage.AdditionalInterests.Remove(Me.MySectionCoverage.AdditionalInterests(aiCounter))
                    If MySectionCoverage.AdditionalInterests.Count <= 0 Then
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
            Me.txtName.Text = ""
            Me.txtDescription.Text = ""
        End If
    End Sub
End Class