Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects

Public Class ctlHomFarmLand
    Inherits ctlSectionCoverageControlBase

    'added 1/11/18 for HOM Upgrade - MLW
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
        'Added 1/11/18 for HOM Upgrade MLW
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
            If Me.SectionCoverageIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises Then

                Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                'txtIM_Description.Attributes.Add("onkeyup", scriptDescCount) 'replaced with the line below Matt A 11/12/2016
                VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)


                txtDescription.Attributes.Add("onfocus", "this.select()")
            End If
        End If

        'Added 7/8/2019 for Home Endorsements Project Task 38907
        If Me.SectionCoverageIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres AndAlso (Me.IsQuoteEndorsement) Then
            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)

            txtDescription.Attributes.Add("onfocus", "this.select()")
        End If

        Me.VRScript.CreateJSBinding(Me.lnkDelete.ClientID, "click", "return confirm('Are you sure you want to delete this item?');")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If MySectionCoverage IsNot Nothing Then
            Me.txtDescription.Text = MySectionCoverage.Description
        Else
            Me.ClearControl()
        End If

        'Added 1/11/18 for HOM Upgrade MLW
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
            'Added 1/18/18 for HOM Upgrade
            lblMaxCharCount.Visible = False
            lblMaxChar.Visible = False


            If Me.SectionCoverageIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres Then
                'Dim descNum As String = Me.CoverageIndex + 1
                'Dim indexLen As Integer = Len(descNum)
                'Updated 7/3/2019 for Home Endorsements Project Task 38907, 38925 MLW
                If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                    If Me.txtDescription.Text = "" Then
                        Me.txtDescription.Text = "FARM #" & (Me.CoverageIndex + 1).ToString()
                    Else
                        If Left(Me.txtDescription.Text, 6) = "FARM #" Then
                            'Dim strLen As Integer = Len(Me.txtDescription.Text)
                            'If strLen > 7 Then
                            '    Dim strEnd As String
                            '    'If Me.CoverageIndex > 7 Then 'do not envision more than 99 farms, so stopping at two digit check
                            '    If indexLen > 1 Then 'do not envision more than 99 farms, so stopping at two digit check
                            '        strEnd = txtDescription.Text.Substring(8)
                            '    Else
                            '        strEnd = txtDescription.Text.Substring(7)
                            '    End If
                            '    Me.txtDescription.Text = "FARM #" & (Me.CoverageIndex + 1).ToString() & strEnd
                            'Else
                            Me.txtDescription.Text = "FARM #" & (Me.CoverageIndex + 1).ToString()
                            'End If
                        End If
                    End If
                Else
                    'Added 7/8/2019 for Home Endorsements Project Task 38907
                    lblMaxCharCount.Visible = True
                    lblMaxChar.Visible = True
                    lblDescription.Text = "*Description"
                End If
            End If

            'Added 1/17/18 for HOM Upgrade MLW
            If Me.SectionCoverageIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises Then

                'Added 1/18/18 - Set Max Characters
                lblMaxCharCount.Visible = True
                lblMaxChar.Visible = True
                hiddenMaxCharCount.Value = 250
                hiddenMaxCharCount.Value = 250 - Len(txtDescription.Text)
                lblMaxCharCount.Text = hiddenMaxCharCount.Value

                'Updated 7/3/2019 for Home Endorsements Project Task 38909, 38925 MLW
                If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                    If Me.txtDescription.Text = "" Then
                        Me.txtDescription.Text = "OFF PREMISES LOCATION #" & (Me.CoverageIndex + 1).ToString()
                    Else
                        If Left(Me.txtDescription.Text, 23) = "OFF PREMISES LOCATION #" Then
                            Me.txtDescription.Text = "OFF PREMISES LOCATION #" & (Me.CoverageIndex + 1).ToString()
                        End If
                    End If
                Else
                    'Added 7/10/2019 for HOM Endorsements Project Task 38909 MLW
                    lblDescription.Text = "*Description"
                End If
            End If
        Else
            'Added 1/18/18 for HOM Upgrade
            lblMaxCharCount.Visible = False
            lblMaxChar.Visible = False
        End If

        'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
        If Me.IsQuoteReadOnly Then
            lnkDelete.Visible = False
            lblMaxCharCount.Visible = False
            lblMaxChar.Visible = False
        End If

        Me.ctlHomSectionCoverageAddress.CoverageIndex = Me.CoverageIndex
        Me.ctlHomSectionCoverageAddress.InitFromExisting(Me)

        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ctlHomSectionCoverageAddress.MyAddAddressLinkClientId = Me.lnkAddAdress.ClientID
    End Sub

    Public Overrides Function Save() As Boolean
        If MySubSectionCoverages IsNot Nothing Then
            MySectionCoverage.Description = Me.txtDescription.Text.Trim().ToMaxLength(250)

            If Me.CoverageIndex = 0 Then ' needs to be on the first coverage of this type
                MySectionCoverage.IntialFarmCheckbox = True
            Else
                MySectionCoverage.IntialFarmCheckbox = False
            End If
            'End If
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
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.Description
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescription, v, accordList)

                    End Select
                Next

            End If
        End If
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        Me.txtDescription.Text = ""
        Me.ctlHomSectionCoverageAddress.ClearControl()
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        Me.Save_FireSaveEvent(False)
        Me.DeleteMySectionCoverage()
        Me.Populate_FirePopulateEvent()
        Me.Save_FireSaveEvent(False)
    End Sub
End Class