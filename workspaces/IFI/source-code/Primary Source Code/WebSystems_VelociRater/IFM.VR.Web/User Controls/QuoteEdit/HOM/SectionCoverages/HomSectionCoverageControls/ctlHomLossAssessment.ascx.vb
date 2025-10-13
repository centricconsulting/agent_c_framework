Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctlHomLossAssessment
    Inherits ctlSectionCoverageControlBase

    'added 1/30/18 control for HOM Upgrade - MLW
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
            If _qqh.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Dim script As String = "$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas(($('#{1}').val().toFloat() + $('#{2} option:selected').text().toFloat()).toString()));".FormatIFM(Me.txtTotalLimit.ClientID, Me.txtIncludedLimit.ClientID, Me.ddIncreasedLimit.ClientID)
            Me.VRScript.CreateJSBinding(Me.ddIncreasedLimit, ctlPageStartupScript.JsEventType.onchange, script, True)

            Me.VRScript.CreateJSBinding(lnkAddAdress, ctlPageStartupScript.JsEventType.onclick, "$('#{0}').show(); $('#{1}').text('View/Edit Address'); return false;".FormatIFM(Me.divAddress.ClientID, Me.lnkAddAdress.ClientID))
            If Me.ctlHomSectionCoverageAddress.ValidationHelper.ItemsWereCopiedToOtherValidationhelper = False And Me.ctlHomSectionCoverageAddress.HasSomeAddressinformation = False Then
                Me.VRScript.AddScriptLine("$('#{0}').hide();".FormatIFM(Me.divAddress.ClientID)) ' run at startup
            Else
                Me.VRScript.AddScriptLine("$('#{1}').text('View/Edit Address');".FormatIFM(Me.divAddress.ClientID, Me.lnkAddAdress.ClientID)) ' run at startup
                If Me.ctlHomSectionCoverageAddress.ValidationHelper.ItemsWereCopiedToOtherValidationhelper = False Then
                    Me.VRScript.AddScriptLine("$('#{0}').hide();".FormatIFM(Me.divAddress.ClientID)) ' run at startup
                End If
            End If
            Me.VRScript.CreateJSBinding(Me.lnkDelete.ClientID, "click", "return confirm('Are you sure you want to delete this item?');")
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
            Dim attribute As New QuickQuoteStaticDataAttribute
            With attribute
                .nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId
                .nvp_value = "70259"
            End With
            optionAttributes.Add(attribute)
            QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddIncreasedLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, optionAttributes)
        End If
    End Sub

    Public Overrides Sub Populate()
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            LoadStaticData()
            Me.txtIncludedLimit.Enabled = False
            Me.txtTotalLimit.Enabled = False
            If MySectionCoverage IsNot Nothing Then
                If MySectionCoverage.IncludedLimit <> "" Then
                    Me.txtIncludedLimit.Text = MySectionCoverage.IncludedLimit
                Else
                    Me.txtIncludedLimit.Text = "1,000"
                End If
                'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddIncreasedLimit, MySectionCoverage.IncreasedLimit)
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddIncreasedLimit, MySectionCoverage.IncreasedLimit)
                Me.txtTotalLimit.Text = MySectionCoverage.TotalLimit

            Else
                Me.ClearControl()
            End If

            Me.ctlHomSectionCoverageAddress.CoverageIndex = Me.CoverageIndex
            Me.ctlHomSectionCoverageAddress.InitFromExisting(Me)

            Me.PopulateChildControls()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Me.ctlHomSectionCoverageAddress.MyAddAddressLinkClientId = Me.lnkAddAdress.ClientID
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            If MySectionCoverage IsNot Nothing Then 'added 1/30/18 for testing MLW
                MySectionCoverage.IncludedLimit = Me.txtIncludedLimit.Text.Trim()
                'MySectionCoverage.IncreasedLimit = Me.ddIncreasedLimit.SelectedValue
                MySectionCoverage.IncreasedLimit = Me.ddIncreasedLimit.SelectedItem.Text
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
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.IncludedLimit
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtIncludedLimit, v, accordList)
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
            Me.txtIncludedLimit.Text = "1,000"
            Me.ddIncreasedLimit.SelectedIndex = -1
            Me.ctlHomSectionCoverageAddress.ClearControl()
        End If
    End Sub

    Protected Sub lnkAddAdress_Click(sender As Object, e As EventArgs) Handles lnkAddAdress.Click
        'Me.Save_FireSaveEvent(False)
    End Sub
End Class