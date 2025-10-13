Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.FarmLines

Public Class ctlIRPM
    Inherits VRControlBase

    Public Event ReqNavToQuoteSummary()
    Public Event RaiseReRate()

    Public Property MyLocationIndex As Int32
        Get
            If ViewState("vs_locationIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_locationIndex"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyFarmLocation As List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations
            End If
            Return Nothing
        End Get
    End Property

    Public Property RouteToUwIsVisible As Boolean 'added 12/3/2020 (Interoperability)
        Get
            Return Me.ctl_RouteToUw.Visible
        End Get
        Set(value As Boolean)
            Me.ctl_RouteToUw.Visible = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MainAccordionDivId = dvFarmIRPM.ClientID
        ListAccordionDivId = dvFarmIRPMDesc.ClientID

        If Not IsPostBack Then
            'Populate()'removed 12/2/2020 (Interoperability) since it should already be called by a parent control
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.CreateAccordion(MainAccordionDivId, hiddenFarmIRPM, "0")
        VRScript.CreateAccordion(ListAccordionDivId, hiddenDesc, "0")

        VRScript.StopEventPropagation(lnkClearFarmIRPM.ClientID, False)
        VRScript.StopEventPropagation(lnkSaveFarmIRPM.ClientID, False)

        txtSupportingBusiness.Style("text-align") = "center"
        txtCareCondition.Style("text-align") = "center"
        txtDamage.Style("text-align") = "center"
        txtConcentration.Style("text-align") = "center"
        txtLocation.Style("text-align") = "center"
        txtMisc.Style("text-align") = "center"
        txtRoof.Style("text-align") = "center"
        txtStruct.Style("text-align") = "center"
        txtPastLosses.Style("text-align") = "center"
        txtRiceHulls.Style("text-align") = "center"
        txtPoultry.Style("text-align") = "center"
        lblTotalValue.Style("text-align") = "center"

        txtSupportingBusiness.Attributes.Add("onfocus", "this.select()")
        txtCareCondition.Attributes.Add("onfocus", "this.select()")
        txtDamage.Attributes.Add("onfocus", "this.select()")
        txtConcentration.Attributes.Add("onfocus", "this.select()")
        txtLocation.Attributes.Add("onfocus", "this.select()")
        txtMisc.Attributes.Add("onfocus", "this.select()")
        txtRoof.Attributes.Add("onfocus", "this.select()")
        txtStruct.Attributes.Add("onfocus", "this.select()")
        txtPastLosses.Attributes.Add("onfocus", "this.select()")
        txtRiceHulls.Attributes.Add("onfocus", "this.select()")
        txtPoultry.Attributes.Add("onfocus", "this.select()")

        ' Open Information Popup
        lbtnSupportingBusiness.Attributes.Add("onclick", "InitFarmPopupInfo('dvSupportingBusiness', 'Supporting Business'); return false;")
        lbtnCareCondition.Attributes.Add("onclick", "InitFarmPopupInfo('dvCareConditionInfoPopup', 'Care and condition of premises and equipment'); return false;")
        lbtnConcentration.Attributes.Add("onclick", "InitFarmPopupInfo('dvConcentration', 'Dispersion or concentration'); return false;")
        lbtnLocation.Attributes.Add("onclick", "InitFarmPopupInfo('dvLocation', 'Location'); return false;")
        lbtnMisc.Attributes.Add("onclick", "InitFarmPopupInfo('dvMisc', 'Miscellaneous protective features or hazards'); return false;")
        lbtnRoof.Attributes.Add("onclick", "InitFarmPopupInfo('dvRoof', 'Roof condition and other windstorm exposures'); return false;")
        lbtnStruct.Attributes.Add("onclick", "InitFarmPopupInfo('dvStruct', 'Superior or inferior structural features'); return false;")
        lbtnPastLosses.Attributes.Add("onclick", "InitFarmPopupInfo('dvPastLosses', 'Past Losses'); return false;")

        ' Close Information Popup
        btnSBOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvSupportingBusiness'); return false;")
        btnCCOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvCareConditionInfoPopup'); return false;")
        btnDCOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvConcentration'); return false;")
        btnLocOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvLocation'); return false;")
        btnMiscOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvMisc'); return false;")
        btnRoofOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvRoof'); return false;")
        btnStruct.Attributes.Add("onclick", "CloseFarmPopupInfo('dvStruct'); return false;")
        btnPLOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvPastLosses'); return false;")

        ' Recalculate Total
        Dim scriptRecalcTotal As String = "CalculateTotalIRPM(""" + txtSupportingBusiness.ClientID + """, """ + txtCareCondition.ClientID +
            """, """ + txtDamage.ClientID + """, """ + txtConcentration.ClientID + """, """ + txtLocation.ClientID +
            """, """ + txtMisc.ClientID + """, """ + txtRoof.ClientID + """, """ + txtStruct.ClientID + """, """ + txtPastLosses.ClientID +
            """, """ + txtRiceHulls.ClientID + """, """ + txtPoultry.ClientID + """, """ + lblTotalValue.ClientID + """, """ + btnSaveRate.ClientID + """);"
        txtSupportingBusiness.Attributes.Add("onblur", scriptRecalcTotal)
        txtCareCondition.Attributes.Add("onblur", scriptRecalcTotal)
        txtDamage.Attributes.Add("onblur", scriptRecalcTotal)
        txtConcentration.Attributes.Add("onblur", scriptRecalcTotal)
        txtLocation.Attributes.Add("onblur", scriptRecalcTotal)
        txtMisc.Attributes.Add("onblur", scriptRecalcTotal)
        txtRoof.Attributes.Add("onblur", scriptRecalcTotal)
        txtStruct.Attributes.Add("onblur", scriptRecalcTotal)
        txtPastLosses.Attributes.Add("onblur", scriptRecalcTotal)
        txtRiceHulls.Attributes.Add("onblur", scriptRecalcTotal)
        txtPoultry.Attributes.Add("onblur", scriptRecalcTotal)

        'added 1/6/2021 (Interoperability)
        If Me.Quote IsNot Nothing AndAlso Me.Quote.HasRuleOverride() = True Then
            Me.VRScript.AddScriptLine("ifm.vr.ui.ForceDisableContent(['dvFarmIRPM']);", onlyAllowOnce:=True)
            'Me.dvFarmIRPM.Attributes.Item("title") = "Edit Mode disabled due to Rule Override"
            'note: will just use js for the tooltip too
            Me.VRScript.AddScriptLine("document.getElementById('" & Me.dvFarmIRPM.ClientID & "').title='Edit Mode disabled due to Rule Override';", onlyAllowOnce:=True)
        End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Quote IsNot Nothing Then
            'Updated 9/6/18 for multi state MLW - Quote to SubQuoteFirst
            If SubQuoteFirst IsNot Nothing Then
                'If MyFarmLocation IsNot Nothing AndAlso SubQuoteFirst.ScheduledRatings IsNot Nothing AndAlso SubQuoteFirst.ScheduledRatings.Any() Then
                'updated 12/2/2020 (Interoperability) to just use properties instead of ScheduledRatings list
                If MyFarmLocation IsNot Nothing Then
                    'Added Additional Info Descriptions (Remark) from Diamond
                    'txtSupportingBusiness.Text = DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(11).RiskFactor)
                    'txtSupportingBusinessDescription.Text = SubQuoteFirst.ScheduledRatings(11).Remark
                    'updated 12/2/2020 (Interoperability) to just use properties instead of ScheduledRatings list
                    txtSupportingBusiness.Text = DiamondToVRConversionDecimal(SubQuoteFirst.IRPM_FAR_SupportingBusiness)
                    txtSupportingBusinessDescription.Text = SubQuoteFirst.IRPM_FAR_SupportingBusinessDesc

                    'txtCareCondition.Text = DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(0).RiskFactor)
                    'txtCareConditionDescription.Text = SubQuoteFirst.ScheduledRatings(0).Remark
                    'updated 12/2/2020 (Interoperability) to just use properties instead of ScheduledRatings list
                    txtCareCondition.Text = DiamondToVRConversionDecimal(SubQuoteFirst.IRPM_FAR_CareConditionOfEquipPremises)
                    txtCareConditionDescription.Text = SubQuoteFirst.IRPM_FAR_CareConditionOfEquipPremisesDesc

                    'txtDamage.Text = DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(2).RiskFactor)
                    'txtDamageDescription.Text = SubQuoteFirst.ScheduledRatings(2).Remark
                    'updated 12/2/2020 (Interoperability) to just use properties instead of ScheduledRatings list
                    txtDamage.Text = DiamondToVRConversionDecimal(SubQuoteFirst.IRPM_FAR_DamageSusceptibility)
                    txtDamageDescription.Text = SubQuoteFirst.IRPM_FAR_DamageSusceptibilityDesc

                    'txtConcentration.Text = DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(3).RiskFactor)
                    'txtConcentrationDescription.Text = SubQuoteFirst.ScheduledRatings(3).Remark
                    'updated 12/2/2020 (Interoperability) to just use properties instead of ScheduledRatings list
                    txtConcentration.Text = DiamondToVRConversionDecimal(SubQuoteFirst.IRPM_FAR_DispersionOrConcentration)
                    txtConcentrationDescription.Text = SubQuoteFirst.IRPM_FAR_DispersionOrConcentrationDesc

                    'txtLocation.Text = DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(6).RiskFactor)
                    'txtLocationDescription.Text = SubQuoteFirst.ScheduledRatings(6).Remark
                    'updated 12/2/2020 (Interoperability) to just use properties instead of ScheduledRatings list
                    txtLocation.Text = DiamondToVRConversionDecimal(SubQuoteFirst.IRPM_FAR_Location)
                    txtLocationDescription.Text = SubQuoteFirst.IRPM_FAR_LocationDesc

                    'txtMisc.Text = DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(7).RiskFactor)
                    'txtMiscDescription.Text = SubQuoteFirst.ScheduledRatings(7).Remark
                    'updated 12/2/2020 (Interoperability) to just use properties instead of ScheduledRatings list
                    txtMisc.Text = DiamondToVRConversionDecimal(SubQuoteFirst.IRPM_FAR_MiscProtectFeaturesOrHazards)
                    txtMiscDescription.Text = SubQuoteFirst.IRPM_FAR_MiscProtectFeaturesOrHazardsDesc

                    'txtRoof.Text = DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(8).RiskFactor)
                    'txtRoofDescription.Text = SubQuoteFirst.ScheduledRatings(8).Remark
                    'updated 12/2/2020 (Interoperability) to just use properties instead of ScheduledRatings list
                    txtRoof.Text = DiamondToVRConversionDecimal(SubQuoteFirst.IRPM_FAR_RoofCondition)
                    txtRoofDescription.Text = SubQuoteFirst.IRPM_FAR_RoofConditionDesc

                    'txtStruct.Text = DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(4).RiskFactor)
                    'txtStructDescription.Text = SubQuoteFirst.ScheduledRatings(4).Remark
                    'updated 12/2/2020 (Interoperability) to just use properties instead of ScheduledRatings list
                    txtStruct.Text = DiamondToVRConversionDecimal(SubQuoteFirst.IRPM_FAR_SuperiorOrInferiorStructureFeatures)
                    txtStructDescription.Text = SubQuoteFirst.IRPM_FAR_SuperiorOrInferiorStructureFeaturesDesc

                    'txtPastLosses.Text = DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(10).RiskFactor)
                    'txtPastLossesDescription.Text = SubQuoteFirst.ScheduledRatings(10).Remark
                    'updated 12/2/2020 (Interoperability) to just use properties instead of ScheduledRatings list
                    txtPastLosses.Text = DiamondToVRConversionDecimal(SubQuoteFirst.IRPM_FAR_PastLosses)
                    txtPastLossesDescription.Text = SubQuoteFirst.IRPM_FAR_PastLossesDesc

                    'txtRiceHulls.Text = DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(5).RiskFactor)
                    'txtRiceHullsDescription.Text = SubQuoteFirst.ScheduledRatings(5).Remark
                    'updated 12/2/2020 (Interoperability) to just use properties instead of ScheduledRatings list
                    txtRiceHulls.Text = DiamondToVRConversionDecimal(SubQuoteFirst.IRPM_FAR_UseOfRiceHullsOrFlameRetardantBedding)
                    txtRiceHullsDescription.Text = SubQuoteFirst.IRPM_FAR_UseOfRiceHullsOrFlameRetardantBeddingDesc

                    'txtPoultry.Text = DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(12).RiskFactor)
                    'txtPoultryDescription.Text = SubQuoteFirst.ScheduledRatings(12).Remark
                    'updated 12/2/2020 (Interoperability) to just use properties instead of ScheduledRatings list
                    txtPoultry.Text = DiamondToVRConversionDecimal(SubQuoteFirst.IRPM_FAR_RegularOnsiteInspections)
                    txtPoultryDescription.Text = SubQuoteFirst.IRPM_FAR_RegularOnsiteInspectionsDesc

                    Dim ShowPoultryOrTurkey As Boolean
                    For Each location In MyFarmLocation
                        If location.FarmTypePoultry OrElse location.FarmTypeTurkey Then
                            ShowPoultryOrTurkey = True
                        End If
                    Next

                    If Not ShowPoultryOrTurkey Then
                        tbRiceHulls.Attributes.Add("style", "display:none;")
                        tbPoultry.Attributes.Add("style", "display:none;")
                        txtRiceHulls.Text = "0"
                        txtPoultry.Text = "0"
                    End If


                    Dim totalIRPM = Double.Parse(txtSupportingBusiness.Text) + Double.Parse(txtCareCondition.Text) + Double.Parse(txtDamage.Text) +
                        Double.Parse(txtConcentration.Text) + Double.Parse(txtLocation.Text) + Double.Parse(txtMisc.Text) + Double.Parse(txtRoof.Text) +
                        Double.Parse(txtStruct.Text) + Double.Parse(txtPastLosses.Text) + Double.Parse(txtRiceHulls.Text) + Double.Parse(txtPoultry.Text)

                    lblTotalValue.Text = totalIRPM

                    If totalIRPM > -16 And totalIRPM < 16 Then
                        lblTotalValue.ForeColor = Drawing.Color.Black
                        btnSaveRate.Enabled = True
                    Else
                        lblTotalValue.ForeColor = Drawing.Color.Red
                        'btnSaveRate.Enabled = False
                        'updated 10/23/2020 (Interoperability)
                        btnSaveRate.Enabled = True
                    End If
                End If
            End If
        End If
        'Now setting Farm IRPM to disabled for New Business. Endorsements do not see this screen.
        txtSupportingBusiness.Enabled = False
        txtSupportingBusinessDescription.Enabled = False
        txtCareCondition.Enabled = False
        txtCareConditionDescription.Enabled = False
        txtDamage.Enabled = False
        txtDamageDescription.Enabled = False
        txtConcentration.Enabled = False
        txtConcentrationDescription.Enabled = False
        txtLocation.Enabled = False
        txtLocationDescription.Enabled = False
        txtMisc.Enabled = False
        txtMiscDescription.Enabled = False
        txtRoof.Enabled = False
        txtRoofDescription.Enabled = False
        txtStruct.Enabled = False
        txtStructDescription.Enabled = False
        txtPastLosses.Enabled = False
        txtPastLossesDescription.Enabled = False
        txtRiceHulls.Enabled = False
        txtRiceHullsDescription.Enabled = False
        txtPoultry.Enabled = False
        txtPoultryDescription.Enabled = False
        'btnSaveRate.Enabled = False 'update - do not want to disable the save and re-rate button
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            'Updated 9/6/18 for multi state MLW - Quote to sq
            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                    'Added Additional Info Descriptions To Diamond
                    sq.IRPM_FAR_SupportingBusiness = VRToDiamondConversion(txtSupportingBusiness.Text)
                    sq.IRPM_FAR_SupportingBusinessDesc = txtSupportingBusinessDescription.Text

                    sq.IRPM_FAR_CareConditionOfEquipPremises = VRToDiamondConversion(txtCareCondition.Text)
                    sq.IRPM_FAR_CareConditionOfEquipPremisesDesc = txtCareConditionDescription.Text

                    sq.IRPM_FAR_DamageSusceptibility = VRToDiamondConversion(txtDamage.Text)
                    sq.IRPM_FAR_DamageSusceptibilityDesc = txtDamageDescription.Text

                    sq.IRPM_FAR_DispersionOrConcentration = VRToDiamondConversion(txtConcentration.Text)
                    sq.IRPM_FAR_DispersionOrConcentrationDesc = txtConcentrationDescription.Text


                    sq.IRPM_FAR_Location = VRToDiamondConversion(txtLocation.Text)
                    sq.IRPM_FAR_LocationDesc = txtLocationDescription.Text

                    sq.IRPM_FAR_MiscProtectFeaturesOrHazards = VRToDiamondConversion(txtMisc.Text)
                    sq.IRPM_FAR_MiscProtectFeaturesOrHazardsDesc = txtMiscDescription.Text

                    sq.IRPM_FAR_RoofCondition = VRToDiamondConversion(txtRoof.Text)
                    sq.IRPM_FAR_RoofConditionDesc = txtRoofDescription.Text

                    sq.IRPM_FAR_SuperiorOrInferiorStructureFeatures = VRToDiamondConversion(txtStruct.Text)
                    sq.IRPM_FAR_SuperiorOrInferiorStructureFeaturesDesc = txtStructDescription.Text

                    sq.IRPM_FAR_PastLosses = VRToDiamondConversion(txtPastLosses.Text)
                    sq.IRPM_FAR_PastLossesDesc = txtPastLossesDescription.Text

                    sq.IRPM_FAR_UseOfRiceHullsOrFlameRetardantBedding = VRToDiamondConversion(txtRiceHulls.Text)
                    sq.IRPM_FAR_UseOfRiceHullsOrFlameRetardantBeddingDesc = txtRiceHullsDescription.Text

                    sq.IRPM_FAR_RegularOnsiteInspections = VRToDiamondConversion(txtPoultry.Text)
                    sq.IRPM_FAR_RegularOnsiteInspectionsDesc = txtPoultryDescription.Text
                Next
            End If

            'added 10/23/2020 to maintain the latest total, which could've been changed by javascript; on failed rate, it would just revert back to what was set on the initial load
            Dim totalIRPM = Double.Parse(txtSupportingBusiness.Text) + Double.Parse(txtCareCondition.Text) + Double.Parse(txtDamage.Text) +
                        Double.Parse(txtConcentration.Text) + Double.Parse(txtLocation.Text) + Double.Parse(txtMisc.Text) + Double.Parse(txtRoof.Text) +
                        Double.Parse(txtStruct.Text) + Double.Parse(txtPastLosses.Text) + Double.Parse(txtRiceHulls.Text) + Double.Parse(txtPoultry.Text)

            lblTotalValue.Text = totalIRPM

            If totalIRPM > -16 And totalIRPM < 16 Then
                lblTotalValue.ForeColor = Drawing.Color.Black
                btnSaveRate.Enabled = True
            Else
                lblTotalValue.ForeColor = Drawing.Color.Red
                'btnSaveRate.Enabled = False
                'updated 10/23/2020 (Interoperability)
                btnSaveRate.Enabled = True
            End If

            Dim valList = IFM.VR.Validation.ObjectValidation.FarmLines.IRPM_FarmValidator.ValidateIRPM(Me.Quote, Me.DefaultValidationType)
            If valList.Any() Then
                For Each v In valList
                    Select Case v.FieldId
                        Case IFM.VR.Validation.ObjectValidation.FarmLines.IRPM_FarmValidator.IRMP_Value, IFM.VR.Validation.ObjectValidation.FarmLines.IRPM_FarmValidator.IRMP_TotalValue
                            Me.ValidationHelper.AddError(v.Message)
                            'Updated 9/25/18 for multi state MLW - Quote to sq
                            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                                For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                                    sq.IRPM_FAR_SupportingBusiness = "1"
                                    sq.IRPM_FAR_CareConditionOfEquipPremises = "1"
                                    sq.IRPM_FAR_DamageSusceptibility = "1"
                                    sq.IRPM_FAR_DispersionOrConcentration = "1"
                                    sq.IRPM_FAR_Location = "1"
                                    sq.IRPM_FAR_MiscProtectFeaturesOrHazards = "1"
                                    sq.IRPM_FAR_RoofCondition = "1"
                                    sq.IRPM_FAR_SuperiorOrInferiorStructureFeatures = "1"
                                    sq.IRPM_FAR_PastLosses = "1"
                                    sq.IRPM_FAR_UseOfRiceHullsOrFlameRetardantBedding = "1"
                                    sq.IRPM_FAR_RegularOnsiteInspections = "1"
                                Next
                            End If
                    End Select
                Next
            End If

            Return True

        End If

        Return False
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = "IRPM"
        Dim valList = IFM.VR.Validation.ObjectValidation.FarmLines.IRPM_FarmValidator.ValidateIRPM(Me.Quote, Me.DefaultValidationType)
        If valList.Any() Then
            For Each v In valList
                Select Case v.FieldId
                    Case IFM.VR.Validation.ObjectValidation.FarmLines.IRPM_FarmValidator.IRMP_Value, IFM.VR.Validation.ObjectValidation.FarmLines.IRPM_FarmValidator.IRMP_TotalValue
                        Me.ValidationHelper.AddError(v.Message)
                End Select
            Next
        End If

        ValidateChildControls(valArgs)

    End Sub

    Protected Sub btnSaveRate_Click(sender As Object, e As EventArgs) Handles btnSaveRate.Click
        'Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New VRValidationArgs(DefaultValidationType)))
        RaiseEvent RaiseReRate()
        'RaiseEvent ReqNavToQuoteSummary()
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        ' Ignores any changes since last save, re-rates quote and displays Quote Summary
        RaiseEvent ReqNavToQuoteSummary()
    End Sub

    Public Shared Function DiamondToVRConversion(diamondValue) As String
        Dim diamondToVRDecimal As Decimal = Decimal.Parse(diamondValue) - 1
        Dim vrValue As Integer

        vrValue = diamondToVRDecimal * 100

        Return vrValue.ToString()
    End Function

    Private Function VRToDiamondConversion(vrValue As String) As String
        Dim vrValueDecimal As Decimal = Decimal.Parse(vrValue) / 100
        Dim diamondValue As Decimal

        diamondValue = vrValueDecimal + 1

        Return diamondValue.ToString()
    End Function

    Public Shared Function DiamondToVRConversionDecimal(diamondValue As String) As String
       
        Dim diamondToVRDecimal As Decimal = Decimal.Parse(diamondValue) - 1
        Dim vrValue As Decimal

        vrValue = Math.Round(diamondToVRDecimal * 100, 1)
        If vrValue = 0.0 Then
            vrValue = 0
        End If

        Return vrValue.ToString()
    End Function

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        txtSupportingBusiness.Text = "0"
        txtCareCondition.Text = "0"
        txtDamage.Text = "0"
        txtConcentration.Text = "0"
        txtLocation.Text = "0"
        txtMisc.Text = "0"
        txtRoof.Text = "0"
        txtStruct.Text = "0"
        txtPastLosses.Text = "0"
        txtRiceHulls.Text = "0"
        txtPoultry.Text = "0"
    End Sub

    Protected Sub lnkClearFarmIRPM_Click(sender As Object, e As EventArgs) Handles lnkClearFarmIRPM.Click
        ClearControl()
        Save_FireSaveEvent(False)
    End Sub
End Class