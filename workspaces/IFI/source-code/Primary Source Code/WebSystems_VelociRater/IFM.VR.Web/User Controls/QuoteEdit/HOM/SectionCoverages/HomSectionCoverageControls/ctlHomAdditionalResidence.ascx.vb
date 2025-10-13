Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctlHomAdditionalResidence
    Inherits ctlSectionCoverageControlBase

    'added 1/5/18 for HOM Upgrade - MLW
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
        Me.VRScript.CreateTextBoxFormatter(Me.txtNumOfFamilies, ctlPageStartupScript.FormatterType.NumericNoCommas, ctlPageStartupScript.JsEventType.onkeyup)

        'Me.VRScript.AddScriptLine("$('#{0}').find('.{1}').css('color','red')".FormatIFM(Me.lnkAddAdress.ClientID, "sectionCoverageAddress"))


    End Sub

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddNumOfFamilies, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.NumberOfFamiliesId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        Me.txtNumOfFamilies.Visible = False
        Me.ddNumOfFamilies.Visible = False
        If MySectionCoverage IsNot Nothing Then
            Me.txtDescription.Text = MySectionCoverage.Description
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddNumOfFamilies, MySectionCoverage.NumberOfFamilies)
            If MySectionCoverage.NumberOfFamilies <> "" Then
                Me.txtNumOfFamilies.Text = MySectionCoverage.NumberOfFamilies 'If(MySectionCoverage.NumberOfFamilies.EqualsAny("", "0"), "1", MySectionCoverage.NumberOfFamilies)
            Else
                Me.txtNumOfFamilies.Text = "1"
            End If

        Else
            Me.ClearControl()
        End If
        Select Case Me.SectionCoverageIIEnum
            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther
                Me.ddNumOfFamilies.Visible = True
                '1/5/18 added for HOM Upgrade MLW
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    'Updated 7/11/2019 for Home Endorsements Project Task 38905, 38925 MLW
                    If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                        If Me.txtDescription.Text = "" Then
                            Me.txtDescription.Text = "ADDITIONAL RESIDENCE #" & (Me.CoverageIndex + 1).ToString()
                        Else
                            If Left(Me.txtDescription.Text, 22) = "ADDITIONAL RESIDENCE #" Then
                                Me.txtDescription.Text = "ADDITIONAL RESIDENCE #" & (Me.CoverageIndex + 1).ToString()
                            End If
                        End If
                    Else
                        lblDescription.Text = "*Description"
                    End If
                    Me.txtDescription.Width = "175"
                End If
            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured
                Me.txtNumOfFamilies.Visible = True
                Me.tdDescription.Visible = False
                Me.tdDescription1.Visible = False
        End Select

        'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
        If Me.IsQuoteReadOnly Then
            lnkDelete.Visible = False
        End If

        Me.ctlHomSectionCoverageAddress.CoverageIndex = Me.CoverageIndex
        Me.ctlHomSectionCoverageAddress.InitFromExisting(Me)


        Me.PopulateChildControls()


    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ctlHomSectionCoverageAddress.MyAddAddressLinkClientId = Me.lnkAddAdress.ClientID
    End Sub

    Public Overrides Function Save() As Boolean
        If MySectionCoverage IsNot Nothing Then
            MySectionCoverage.Description = Me.txtDescription.Text.Trim() ' only applies to HOM_SectionIICoverageType.AdditionalResidenceRentedToOther
            Select Case Me.SectionCoverageIIEnum
                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther
                    MySectionCoverage.NumberOfFamilies = Me.ddNumOfFamilies.SelectedValue
                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured
                    MySectionCoverage.NumberOfFamilies = Me.txtNumOfFamilies.Text.Trim()
            End Select
        End If

        SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = Me.CoverageName
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        If Me.MySectionCoverage IsNot Nothing Then
            Dim valList = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
            If valList.Any() Then
                For Each v In valList
                    Select Case v.FieldId
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.Description
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescription, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.NumberOfFamilies
                            Select Case Me.SectionCoverageIIEnum
                                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddNumOfFamilies, v, accordList)
                                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtNumOfFamilies, v, accordList)
                            End Select

                    End Select
                Next

            End If
        End If
        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        Me.Save_FireSaveEvent(False)
        Me.DeleteMySectionCoverage()
        Me.Populate_FirePopulateEvent()
        Me.Save_FireSaveEvent(False)
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        Me.txtDescription.Text = ""
        Me.txtNumOfFamilies.Text = "1"
        Me.ddNumOfFamilies.SelectedIndex = -1
        Me.ctlHomSectionCoverageAddress.ClearControl()
    End Sub

    Protected Sub lnkAddAdress_Click(sender As Object, e As EventArgs) Handles lnkAddAdress.Click
        'Me.Save_FireSaveEvent(False)
    End Sub
End Class