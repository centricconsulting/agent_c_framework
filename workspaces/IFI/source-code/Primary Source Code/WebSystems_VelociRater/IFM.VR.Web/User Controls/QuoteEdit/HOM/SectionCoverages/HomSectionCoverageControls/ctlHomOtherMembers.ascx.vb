Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption

Public Class ctlHomOtherMembers
    Inherits ctlSectionCoverageControlBase

    'Added 1/31/18 control for HOM Upgrade MLW
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

    Public ReadOnly Property HasSomeMemberinformation As Boolean
        Get
            'Updated 8/23/18 for multi state MLW
            'If Me.MySectionCoverage.IsNotNull Then
            If Me.MySectionCoverage IsNot Nothing Then
                'If Me.MySectionCoverage.Name.IsNotNull() Then
                If Me.MySectionCoverage.Name IsNot Nothing Then
                    Return IFM.PrimativeExtensions.OneorMoreIsNotEmpty(Me.MySectionCoverage.Description, Me.MySectionCoverage.Name.FirstName, Me.MySectionCoverage.Name.MiddleName,
                                                                                   Me.MySectionCoverage.Name.LastName, Me.MySectionCoverage.Name.SuffixName)
                End If
            End If
            Return False
        End Get
    End Property
    Public ReadOnly Property HasRequiredMemberinformation As Boolean
        Get
            'Updated 8/23/18 for multi state MLW
            'If Me.MySectionCoverage.IsNotNull Then
            If Me.MySectionCoverage IsNot Nothing Then
                'If Me.MySectionCoverage.Name.IsNotNull() Then
                If Me.MySectionCoverage.Name IsNot Nothing Then
                    Dim hasReq As Boolean
                    If NoneAreNullEmptyorWhitespace(Me.MySectionCoverage.Name.FirstName, Me.MySectionCoverage.Name.LastName) Then
                        hasReq = True
                        If NoneAreNullEmptyorWhitespace(Me.MySectionCoverage.Description) Then
                            hasReq = True
                        Else
                            hasReq = False
                        End If
                    Else
                        hasReq = False
                    End If

                    Return hasReq
                End If
            End If
            Return False
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Me.VRScript.CreateJSBinding(lnkView, ctlPageStartupScript.JsEventType.onclick, "$('#{0}').show();$('#{1}').show(); $('#{2}').text(''); return false;".FormatIFM(Me.divOtherName.ClientID, Me.divOtherDescription.ClientID, Me.lnkView.ClientID))
            If HasRequiredMemberinformation = False Then
                Me.VRScript.AddScriptLine("$('#{0}').show();$('#{1}').show();$('#{2}').text('');".FormatIFM(Me.divOtherName.ClientID, Me.divOtherDescription.ClientID, Me.lnkView.ClientID)) ' run at startup
            Else
                Me.VRScript.AddScriptLine("$('#{0}').hide();$('#{1}').hide();$('#{2}').text('View/Edit Other Member');".FormatIFM(Me.divOtherName.ClientID, Me.divOtherDescription.ClientID, Me.lnkView.ClientID)) ' run at startup
            End If
            Me.VRScript.CreateJSBinding(Me.lnkDelete.ClientID, "click", "return confirm('Are you sure you want to delete this item?');")
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddOtherSuffix, QuickQuoteClassName.QuickQuoteName, QuickQuotePropertyName.SuffixName, SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            LoadStaticData()
            If MySectionCoverage IsNot Nothing Then
                Me.txtOtherDescription.Text = MySectionCoverage.Description
                Me.txtOtherFirstName.Text = MySectionCoverage.Name.FirstName
                Me.txtOtherMiddleName.Text = MySectionCoverage.Name.MiddleName
                Me.txtOtherLastName.Text = MySectionCoverage.Name.LastName
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddOtherSuffix, MySectionCoverage.Name.SuffixName)
            Else
                Me.ClearControl()
            End If

            'Updated 7/3/2019 for Home Endorsements Project Task 38913, 38925 MLW
            If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                If Me.txtOtherDescription.Text = "" Then
                    Me.txtOtherDescription.Text = "OTHER MEMBER #" & (Me.CoverageIndex + 1).ToString()
                Else
                    If Left(Me.txtOtherDescription.Text, 14) = "OTHER MEMBER #" Then
                        Me.txtOtherDescription.Text = "OTHER MEMBER #" & (Me.CoverageIndex + 1).ToString()
                    End If
                End If
            End If

            'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
            If Me.IsQuoteReadOnly Then
                lnkDelete.Visible = False
            End If

            Me.PopulateChildControls()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            If MySectionCoverage IsNot Nothing Then
                MySectionCoverage.Description = Me.txtOtherDescription.Text.Trim()
                MySectionCoverage.Name.FirstName = Me.txtOtherFirstName.Text.Trim()
                MySectionCoverage.Name.MiddleName = Me.txtOtherMiddleName.Text.Trim()
                MySectionCoverage.Name.LastName = Me.txtOtherLastName.Text.Trim()
                MySectionCoverage.Name.SuffixName = Me.ddOtherSuffix.SelectedValue
            End If


            SaveChildControls()
        End If
        Return True
    End Function
    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = Me.CoverageName
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            If Me.MySectionCoverage IsNot Nothing Then
                Dim valList = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
                If valList.Any() Then
                    For Each v In valList
                        Select Case v.FieldId
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.Description
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtOtherDescription, v, accordList)
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.OtherFirstName
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtOtherFirstName, v, accordList)
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.OtherLastName
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtOtherLastName, v, accordList)
                        End Select
                    Next

                End If
            End If
            Me.ValidateChildControls(valArgs)
        End If
    End Sub
    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Me.Save_FireSaveEvent(False)
            Me.DeleteMySectionCoverage()
            Me.Populate_FirePopulateEvent()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub
    Public Overrides Sub ClearControl()
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            MyBase.ClearControl()
            Me.txtOtherDescription.Text = ""
            Me.txtOtherFirstName.Text = ""
            Me.txtOtherMiddleName.Text = ""
            Me.txtOtherLastName.Text = ""
            Me.ddOtherSuffix.SelectedIndex = -1
        End If
    End Sub

End Class