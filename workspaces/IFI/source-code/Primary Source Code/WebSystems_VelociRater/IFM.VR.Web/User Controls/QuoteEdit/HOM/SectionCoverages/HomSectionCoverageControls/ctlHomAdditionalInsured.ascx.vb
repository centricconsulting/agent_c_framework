Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

'Added control 4/30/18 for HOM Upgrade Bug 26102 MLW
Public Class ctlHomAdditionalInsured
    Inherits ctlSectionCoverageControlBase

    'added 1/5/18 for HOM Upgrade - MLW
    Dim _qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass

    Dim flaggedForDelete As Boolean = False 'added 4/2/2020 for Home Endorsements Bug 44392 MLW

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
            If _qqh.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property
    'Added 04/01/2020 for Home Endorsement Bug 44392 MLW
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
    'Added 04/01/2020 for Home Endorsement Bug 44392 MLW
    Public ReadOnly Property MyAdditionalInterest As QuickQuoteAdditionalInterest
        Get
            If MyAiList IsNot Nothing AndAlso MyAiList.Count > 0 Then
                Return MyAiList(0)
            End If
            Return Nothing
        End Get
    End Property

    'Added 04/01/2020 for Home Endorsement Bug 44392 MLW
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
    'Added 04/01/2020 for Home Endorsement Bug 44392 MLW
    Public ReadOnly Property ReturnToQuoteEvent() As String
        Get
            Return "ReturnToQuoteEvent" + "_" + Quote.PolicyId + "|" + Quote.PolicyImageNum
        End Get
    End Property
    'Added 04/01/2020 for Home Endorsement Bug 44392 MLW
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

        'Me.VRScript.CreateJSBinding(lnkAddAdress, ctlPageStartupScript.JsEventType.onclick, "$('#{0}').show(); $('#{1}').text('View/Edit Address'); return false;".FormatIFM(Me.divAddress.ClientID, Me.lnkAddAdress.ClientID))
        'If Me.ctlHomSectionCoverageAddress.ValidationHelper.ItemsWereCopiedToOtherValidationhelper = False And Me.ctlHomSectionCoverageAddress.HasSomeAddressinformation = False Then
        '    Me.VRScript.AddScriptLine("$('#{0}').hide();".FormatIFM(Me.divAddress.ClientID)) ' run at startup
        'Else

        '    Me.VRScript.AddScriptLine("$('#{1}').text('View/Edit Address');".FormatIFM(Me.divAddress.ClientID, Me.lnkAddAdress.ClientID)) ' run at startup
        '    If Me.ctlHomSectionCoverageAddress.ValidationHelper.ItemsWereCopiedToOtherValidationhelper = False Then
        '        Me.VRScript.AddScriptLine("$('#{0}').hide();".FormatIFM(Me.divAddress.ClientID)) ' run at startup
        '    End If
        'End If
        'Me.VRScript.CreateJSBinding(Me.lnkDelete.ClientID, "click", "return confirm('Are you sure you want to delete this item?');")
        'Me.VRScript.CreateTextBoxFormatter(Me.txtNumOfFamilies, ctlPageStartupScript.FormatterType.NumericNoCommas, ctlPageStartupScript.JsEventType.onkeyup)

        ''Me.VRScript.AddScriptLine("$('#{0}').find('.{1}').css('color','red')".FormatIFM(Me.lnkAddAdress.ClientID, "sectionCoverageAddress"))

        'Added 04/01/2020 for Home Endorsements Bug 44392 MLW 
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
        'QQHelper.LoadStaticDataOptionsDropDown(Me.ddNumOfFamilies, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.NumberOfFamiliesId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If MySectionCoverage IsNot Nothing Then
            'Updated 04/01/2020 for home endorsements bug 44392 MLW
            'If MySectionCoverage.AdditionalInterests Is Nothing Then
            '    MySectionCoverage.AdditionalInterests = Nothing
            '    MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            '    Dim dummyItem As New QuickQuoteAdditionalInterest
            '    dummyItem.Address.StateId = "0" 'Need to initially save to blank, will save again in child control - added 4/30/18 for Bug 26344 MLW
            '    dummyItem.TypeId = "81" 'Relative
            '    MySectionCoverage.AdditionalInterests.Add(dummyItem)
            'End If
            'Me.txtName.Text = MySectionCoverage.AdditionalInterests(0).Name.CommercialName1
            'Me.txtDescription.Text = MySectionCoverage.AdditionalInterests(0).Description

            If MySectionCoverage.AdditionalInterests Is Nothing OrElse MySectionCoverage.AdditionalInterests.Count = 0 Then
                MySectionCoverage.AdditionalInterests = Nothing
                MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                Dim dummyItem As New QuickQuoteAdditionalInterest
                dummyItem.Address.StateId = "0" 'Need to initially save to blank, will save again in child control - added 4/30/18 for Bug 26344 MLW
                dummyItem.TypeId = "81" 'Relative
                MySectionCoverage.AdditionalInterests.Add(dummyItem)
            End If
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
        Else
            Me.ClearControl()
        End If

        'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
        If Me.IsQuoteReadOnly Then
            lnkDelete.Visible = False
        End If

        Me.ctlHomAdditionalInterestAddress.aiCounter = 0 'will only ever have 1 item in the AI list for this coverage
        Me.ctlHomAdditionalInterestAddress.CoverageIndex = Me.CoverageIndex
        Me.ctlHomAdditionalInterestAddress.InitFromExisting(Me)
        Me.ctlHomAdditionalInterestAddress.Populate()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean

        'Updated 04/01/2020 for Home Endorsements Bug 44392 MLW
        'If MySectionCoverage IsNot Nothing Then
        '    MySectionCoverage.AdditionalInterests = Nothing

        '    If MySectionCoverage.AdditionalInterests Is Nothing Then
        '        'create the additional interest section, then save fields
        '        MySectionCoverage.AdditionalInterests = Nothing
        '        MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
        '        Dim dummyItem As New QuickQuoteAdditionalInterest
        '        dummyItem.Address.StateId = "0" 'Need to initially save to blank, will save again in child control - added 4/30/18 for Bug 26344 MLW      
        '        dummyItem.TypeId = "81" 'Relative
        '        MySectionCoverage.AdditionalInterests.Add(dummyItem)
        '    End If

        '    MySectionCoverage.AdditionalInterests(0).Description = Me.txtDescription.Text.Trim()
        '    MySectionCoverage.AdditionalInterests(0).Name.CommercialName1 = Me.txtName.Text.Trim()
        '    MySectionCoverage.AdditionalInterests(0).TypeId = "81"

        '    Me.ctlHomAdditionalInterestAddress.aiCounter = 0 'will only ever have 1 item in the AI list for this coverage
        '    Me.ctlHomAdditionalInterestAddress.CoverageIndex = Me.CoverageIndex
        '    Me.ctlHomAdditionalInterestAddress.Save()
        'End If
        If MySectionCoverage Is Nothing Then
            Me.CreateMySectionCoverage()
        End If
        If MySectionCoverage IsNot Nothing Then
            If MySectionCoverage.AdditionalInterests Is Nothing OrElse MySectionCoverage.AdditionalInterests.Count = 0 Then
                'create the additional interest section, then save fields
                MySectionCoverage.AdditionalInterests = Nothing
                MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                Dim dummyItem As New QuickQuoteAdditionalInterest
                dummyItem.Address.StateId = "0" 'Need to initially save to blank, will save again in child control - added 4/30/18 for Bug 26344 MLW      
                dummyItem.TypeId = "81" 'Relative
                MySectionCoverage.AdditionalInterests.Add(dummyItem)
            End If
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
                                MyAiList(0) = loadedAi
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
            '    Me.ctlHomAdditionalInterestAddress.aiCounter = 0 'will only ever have 1 item in the AI list for this coverage
            '    Me.ctlHomAdditionalInterestAddress.CoverageIndex = Me.CoverageIndex
            '    Me.ctlHomAdditionalInterestAddress.Save()
        End If


        Return True
    End Function

    'Added 04/01/2020 for Home Endorsement Bug 44392 MLW
    Private Sub InjectFormIntoAI(interest As QuickQuoteAdditionalInterest)
        If interest.Name IsNot Nothing Then
            interest.Name.CommercialName1 = Me.txtName.Text.Trim().ToUpper()
        End If
        interest.Description = Me.txtDescription.Text.Trim()
        interest.TypeId = "81" 'Relative
        If interest.Address IsNot Nothing Then
            interest.Address.StateId = "0" 'Need to initially save to blank, will save again in child control - added 4/30/18 for Bug 26344 MLW
        End If
        Me.ctlHomAdditionalInterestAddress.aiCounter = 0 'will only ever have 1 item in the AI list for this coverage
        Me.ctlHomAdditionalInterestAddress.CoverageIndex = Me.CoverageIndex
        Me.ctlHomAdditionalInterestAddress.InjectFormIntoAI(interest)
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = Me.CoverageName
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        If Me.MySectionCoverage IsNot Nothing Then
            If Me.MySectionCoverage.AdditionalInterests IsNot Nothing Then
                Dim aiVals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AdditionalInterestRelativeValidation(MySectionCoverage.AdditionalInterests(0), valArgs.ValidationType, MySectionCoverage)
                For Each v In aiVals
                    Select Case v.FieldId
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.CommercialName
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtName, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.Description
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescription, v, accordList)
                    End Select
                Next
            End If

        End If
        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        flaggedForDelete = True 'Added 04/02/2020 for Home Endorsement Bug 44392 MLW
        Me.Save_FireSaveEvent(False)
        Me.DeleteMySectionCoverage()
        Me.Populate_FirePopulateEvent()
        Me.Save_FireSaveEvent(False)
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        Me.txtDescription.Text = ""
        Me.txtName.Text = ""
        Me.ctlHomAdditionalInterestAddress.ClearControl()
    End Sub

    Protected Sub lnkAddAdress_Click(sender As Object, e As EventArgs) Handles lnkAddAdress.Click
        'Me.Save_FireSaveEvent(False)
    End Sub
End Class