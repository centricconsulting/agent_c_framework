Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Public Class ctlHomSpecifiedStructure
    Inherits ctlSectionCoverageControlBase

    Public ReadOnly Property ClearJsLogic As String
        Get
            Return "$('#{0}').val('');$('#{1}').val('');$('#{2}').val('');".FormatIFM(Me.txtLimit.ClientID, txtDescription.ClientID, Me.ddConstructionType.ClientID)
        End Get
    End Property

    'added 12/28/17 for HOM Upgrade - MLW
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
        'Added 1/3/18 for HOM Upgrade MLW
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
            'If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises Then
            If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures Then
                Me.VRScript.CreateJSBinding(Me.txtLimit, ctlPageStartupScript.JsEventType.onchange, "$('#{0}').val(ifm.vr.stringFormating.asRoundToNearest1000($('#{0}').val()));$('#{0}').val(ifm.vr.stringFormating.asPositiveWholeNumberWithCommas($('#{0}').val()));".FormatIFM(Me.txtLimit.ClientID))
            Else
                Me.VRScript.CreateTextBoxFormatter(Me.txtLimit, ctlPageStartupScript.FormatterType.PositiveNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
            End If
        Else
            Me.VRScript.CreateTextBoxFormatter(Me.txtLimit, ctlPageStartupScript.FormatterType.PositiveNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
        End If
        'Me.VRScript.CreateTextBoxFormatter(Me.txtLimit, ctlPageStartupScript.FormatterType.PositiveNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)

        'Added 7/8/2019 for Home Endorsements Project Task 38910 MLW
        If Me.SectionCoverageIIEnum = QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence AndAlso (Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote) Then
            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)

            txtDescription.Attributes.Add("onfocus", "this.select()")
        End If

    End Sub

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddConstructionType, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()

        LoadStaticData()
        If MySectionCoverage IsNot Nothing Then
            Me.txtLimit.Text = MySectionCoverage.IncreasedLimit ' doesn't do anything for HO-43 (HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence)
            Me.txtDescription.Text = MySectionCoverage.Description
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddConstructionType, MySectionCoverage.ConstructionTypeId)
        Else
            Me.ClearControl()
        End If
        If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures OrElse Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises Then 'verified correct for Specified Other Structures - On Premises 92-049
            'single location - HO-43 (HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence)
            Me.lnkAddAdress.Visible = False
            Me.lnkDelete.Visible = False

            'Added 1/3/18 for HOM Upgrade MLW
            'Other Structures On the Residence Premises or Specified Other Structures - On Premises
            If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                'Updated 7/3/2019 for Home Endorsements Project Task 38903, 38925 MLW
                If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                    If Me.txtDescription.Text = "" Then
                        Me.txtDescription.Text = "OTHER STRUCTURE"
                    End If
                Else
                    lblLimit.Text = "*Limit"
                    lblDescription.Text = "*Description"
                End If
                Me.txtDescription.Width = "150"
                Me.ddConstructionType.Visible = False
                Me.tdConst1.Visible = False
                'Added 7/8/2019 for Home Endorsements Project Task 38910 MLW
                lblMaxChar.Visible = False
                lblMaxCharCount.Visible = False
            End If
        Else

            If Me.SectionCoverageIIEnum = QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence Then 'for HO-43 
                Me.tdLimit1.Visible = False
                Me.tdLimit.Visible = False
                Me.tdConst1.Visible = False
                Me.tdConst2.Visible = False
                Me.txtDescription.TextMode = TextBoxMode.MultiLine
                Me.txtDescription.Width = "300"
            End If

            'Updated 12/28/17 for HOM Upgrade MLW
            If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises Then
                    'Specific Structures Away from Residence Premises (HO 0492) or Specified Other Structures - Off Premises (92-147)
                    'Updated 7/3/2019 for Home Endorsements Project Task 38903, 38925 MLW
                    If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                        If Me.txtDescription.Text = "" Then
                            Me.txtDescription.Text = "STRUCTURE #" & (Me.CoverageIndex + 1).ToString()
                        Else
                            If Left(Me.txtDescription.Text, 11) = "STRUCTURE #" Then
                                Me.txtDescription.Text = "STRUCTURE #" & (Me.CoverageIndex + 1).ToString()
                            End If
                        End If
                    Else
                        lblLimit.Text = "*Limit"
                        lblDescription.Text = "*Description"
                    End If
                    Me.ddConstructionType.Visible = False
                    Me.tdConst1.Visible = False
                    'Added 7/8/2019 for Home Endorsements Project Task 38910 MLW
                    lblMaxChar.Visible = False
                    lblMaxCharCount.Visible = False
                End If


                If Me.SectionCoverageIIEnum = QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence Then
                    'Permitted Incidental Occupancies Other Residence(HO 2443)
                    'Updated 7/3/2019 for Home Endorsements Project Task 38910, 38925 MLW
                    If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                        If Me.txtDescription.Text = "" Then
                            Me.txtDescription.Text = "INCIDENTAL OCCUPANCIES #" & (Me.CoverageIndex + 1).ToString()
                        Else
                            If Left(Me.txtDescription.Text, 24) = "INCIDENTAL OCCUPANCIES #" Then
                                Me.txtDescription.Text = "INCIDENTAL OCCUPANCIES #" & (Me.CoverageIndex + 1).ToString()
                            End If
                        End If
                        'Added 7/8/2019 for Home Endorsements Project Task 38910 MLW
                        lblMaxChar.Visible = False
                        lblMaxCharCount.Visible = False
                    Else
                        'Added 7/8/2019 for Home Endorsements Project Task 38910 MLW
                        lblMaxChar.Visible = True
                        lblMaxCharCount.Visible = True
                        lblDescription.Text = "*Description"
                    End If
                    Me.ddConstructionType.Visible = False
                    Me.tdConst1.Visible = False
                End If
            End If

            'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
            If Me.IsQuoteReadOnly Then
                lnkDelete.Visible = False
                lblMaxCharCount.Visible = False
                lblMaxChar.Visible = False
            End If

            'populate address info for 92-147 (HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises)
            Me.ctlHomSectionCoverageAddress.CoverageIndex = Me.CoverageIndex
            Me.ctlHomSectionCoverageAddress.InitFromExisting(Me)

        End If

        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ctlHomSectionCoverageAddress.MyAddAddressLinkClientId = Me.lnkAddAdress.ClientID
    End Sub

    Public Overrides Function Save() As Boolean
        If MySectionCoverage IsNot Nothing Then
            MySectionCoverage.IncreasedLimit = Me.txtLimit.Text.Trim() ' doesn't do anything for HO-43 (HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence)
            MySectionCoverage.Description = Me.txtDescription.Text.Trim()
            If Me.SectionCoverageIIEnum = QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.None Then
                MySectionCoverage.ConstructionTypeId = Me.ddConstructionType.SelectedValue
            End If
        End If
        Me.SaveChildControls()
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
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.IncreasedLimit
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLimit, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.Description
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescription, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ConstructionType
                            '10/24/17 added if stmt for HOM Upgrade MLW - bind was existing
                            If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises Then
                                Else
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddConstructionType, v, accordList)
                                End If
                                'Added 1/3/18 for HOM Upgrade MLW
                                If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures OrElse Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises Then
                                Else
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddConstructionType, v, accordList)
                                End If
                            Else
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddConstructionType, v, accordList)
                            End If


                    End Select
                Next

            End If
        End If
        Me.ValidateChildControls(valArgs)
    End Sub


    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        Me.txtDescription.Text = ""
        Me.txtLimit.Text = ""
        Me.ddConstructionType.SelectedIndex = -1
        Me.ctlHomSectionCoverageAddress.ClearControl()
    End Sub
    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        Me.Save_FireSaveEvent(False)
        Me.DeleteMySectionCoverage()
        Me.Populate_FirePopulateEvent()
        Me.Save_FireSaveEvent(False)
    End Sub
End Class