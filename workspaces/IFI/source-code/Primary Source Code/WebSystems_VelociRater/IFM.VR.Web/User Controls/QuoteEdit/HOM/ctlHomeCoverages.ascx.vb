Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.HOM

Public Class ctlHomeCoverages
    Inherits VRControlBase

    Dim pvHelper As New QuickQuotePropertyValuationHelperClass
    'Dim qqHelper As New QuickQuoteHelperClass
    Dim ClassName As String = "ctlHomeQuote"
    'Public Event SaveRequested(invokeValidations As Boolean)
    Public Event HomeCoveragesChanged()

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Public Property ActiveRVWaterIndex As String
        Get
            Return Me.hiddenRVWatercraft.Value
        End Get
        Set(value As String)
            Me.hiddenRVWatercraft.Value = value
        End Set
    End Property

    Public ReadOnly Property CurrentForm As String
        Get
            Try
                'Updated 11/27/17 for HOM Upgrade MLW
                Return QQHelper.GetShortFormName(Quote)
                'Return QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, Quote.Locations(0).FormTypeId).Substring(0, 4)
            Catch ex As Exception
                Return ""
            End Try

        End Get
    End Property

    Public Property DeductibleDivId As String
        Get
            If ViewState("vs_DeductibleDivId_") IsNot Nothing Then
                Return ViewState("vs_DeductibleDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_DeductibleDivId_") = value
        End Set
    End Property

    Public Property BaseCoverageDivId As String
        Get
            If ViewState("vs_BaseCoverageDivId_") IsNot Nothing Then
                Return ViewState("vs_BaseCoverageDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_BaseCoverageDivId_") = value
        End Set
    End Property

    Public Property OptionalCoverageDivId As String
        Get
            If ViewState("vs_OptionalCoverageDivId_") IsNot Nothing Then
                Return ViewState("vs_OptionalCoverageDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_OptionalCoverageDivId_") = value
        End Set
    End Property

    Public Property InlandMarineDivId As String
        Get
            If ViewState("vs_InlandMarineDivId_") IsNot Nothing Then
                Return ViewState("vs_InlandMarineDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_InlandMarineDivId_") = value
        End Set
    End Property

    Public Property InitialRVWatercraftDivId As String
        Get
            If ViewState("vs_InitialRVWatercraftDivId_") IsNot Nothing Then
                Return ViewState("vs_InitialRVWatercraftDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_InitialRVWatercraftDivId_") = value
        End Set
    End Property

    'added 11/22/17 for HOM Upgrade - MLW
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
            If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property

    'Added 02/17/2020 for Home Endorsements Bug 43921 MLW
    Public Property RouteToUwIsVisible As Boolean
        Get
            Return Me.ctl_RouteToUw.Visible
        End Get
        Set(value As Boolean)
            Me.ctl_RouteToUw.Visible = value
        End Set
    End Property

    Private Sub SetDefaultValues()
        If Me.MyLocation IsNot Nothing Then
            'Set dropdown lists to default values
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlDeductible, "1,000")
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlWindHailDeductible, "N/A")
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlPersonalLiability, "100,000")
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlMedicalPayments, "1,000")

            'Zero out E2Value
            lblReplacementCCValue.Text = "0"

            'Zero out Base Coverages
            If CurrentForm = "HO-6" Then
                txtDwLimit.Text = "1,000"
            Else
                txtDwLimit.Text = "0"
            End If

            txtDwellingChangeInLimit.Text = "0"
            txtDwellingTotalLimit.Text = txtDwLimit.Text
            hiddenDwellingLimit.Value = txtDwLimit.Text
            hiddenDwellingChange.Value = "0"
            hiddenDwellingTotal.Value = hiddenDwellingLimit.Value

            txtRPSLimit.Text = "0"
            txtRPSChgInLimit.Text = "0"
            txtRPSTotalLimit.Text = "0"
            hiddenRPSLimit.Value = "0"
            hiddenRPSChange.Value = "0"
            hiddenRPSTotal.Value = "0"

            txtPPLimit.Text = "0"
            txtPPChgInLimit.Text = "0"
            txtPPTotalLimit.Text = "0"
            hiddenPPLimit.Value = "0"
            hiddenPPChange.Value = "0"
            hiddenPPTotal.Value = "0"

            txtLossLimit.Text = "0"
            txtLossChgInLimit.Text = "0"
            txtLossTotalLimit.Text = "0"
            hiddenLossLimit.Value = "0"
            hiddenLossChange.Value = "0"
            hiddenLossTotal.Value = "0"
        End If
    End Sub

    Private Function FormatCoverageGridNumber(ByVal num As String) As String
        Dim newnum As String = Nothing
        Try
            newnum = num.Replace(",", "")
            newnum = newnum.Replace("$", "")
            Return newnum
        Catch ex As Exception
            Return ""
        End Try
    End Function



    'Private Sub RecalculateIMCoverageTotal(optionalTotal) Handles ctlInlandMarineItem.RecalculateCoverageTotal
    '    lblIMChosen.Text = "(" + optionalTotal.ToString + ")"
    'End Sub



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            MainAccordionDivId = dvCoverages.ClientID
            DeductibleDivId = dvDeductible.ClientID
            BaseCoverageDivId = dvBaseCoverage.ClientID

            'InlandMarineDivId = dvInlandMarine.ClientID
            'InitialRVWatercraftDivId = dvInitRVWatercraft.ClientID

            'Session("CovCLimit") = MyLocation.C_PersonalProperty_Limit
        End If
    End Sub

    Protected Sub lnkClearCoverages_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkClearCoverages.Click
        Session("valuationValue") = "False"
        SetDefaultValues()
        'Populate()
    End Sub

    Protected Function RoundValue(number As Decimal) As Integer
        Dim returnNum As Integer = 0

        If number < 0 Then
            number = number * -1
            returnNum = Math.Ceiling(number / 1000) * 1000
            returnNum = returnNum * -1
        Else
            returnNum = Math.Ceiling(number / 1000) * 1000
        End If
        Return returnNum
    End Function

    Protected Sub lnkSaveCoverages_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSaveCoverages.Click
        Session("valuationValue") = "False"
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

        If ActiveRVWaterIndex = "" Then
            ActiveRVWaterIndex = "false"
        End If
    End Sub

    Protected Sub btnSaveBase_Click(sender As Object, e As EventArgs) Handles btnSaveBase.Click
        Session("valuationValue") = "False"
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

        If ActiveRVWaterIndex = "" Then
            ActiveRVWaterIndex = "false"
        End If
    End Sub

    Protected Sub btnRateBase_Click(sender As Object, e As EventArgs) Handles btnRateBase.Click
        btnRateInlandMarine_Click(Me, New EventArgs())
    End Sub

    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    Protected Sub btnViewGotoNextSection_Click(sender As Object, e As EventArgs) Handles btnViewGotoNextSection.Click
        'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
        Me.Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub

    Protected Sub btnReplacementCC_Click(sender As Object, e As EventArgs) Handles btnReplacementCC.Click
        Dim errMsg As String = ""
        Dim wasSaveSuccessful As Boolean = False
        Dim valuationUrl As String = ""

        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

        If ActiveRVWaterIndex = "" Then
            ActiveRVWaterIndex = "false"
        End If

        Dim chc As New CommonHelperClass
        If chc.ConfigurationAppSettingValueAsBoolean("VR_AllLines_Site360Valuation_Settings") = True Then
            pvHelper.PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(Quote, valuationUrl, 1, True, wasSaveSuccessful, errMsg, valuationVendor:=QuickQuotePropertyValuation.ValuationVendor.Verisk360)
        Else
            pvHelper.PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(Quote, valuationUrl, 1, True, wasSaveSuccessful, errMsg, valuationVendor:=QuickQuotePropertyValuation.ValuationVendor.e2Value)
        End If

        If valuationUrl <> "" Then
            Session("valuationValue") = "True"
            Response.Redirect(valuationUrl, False)

        Else
            If errMsg = "" Then 'should have something if it gets here... ELSE will have already redirected to e2Value
                errMsg = "Problem Initiating Valuation Request"
            End If

            ShowError(errMsg)
        End If
    End Sub

    Private Sub ShowError(ByVal message As String, Optional ByVal redirect As Boolean = False, Optional ByVal redirectPage As String = "")
        message = Replace(message, "\", "\\")
        message = Replace(message, "<br>", "\n")
        message = Replace(message, vbCrLf, "\n")

        Dim strScript As String = "<script language=JavaScript>"
        strScript &= "alert(""" & message & """);"
        If redirect = True Then
            If redirectPage = "" Then
                redirectPage = "MyVelociRater.aspx" 'use config key if available
            End If
            strScript &= " window.location.href='" & redirectPage & "';"
        End If
        strScript &= "</script>"

        Page.RegisterStartupScript("clientScript", strScript)

    End Sub

    Protected Sub lnkSaveDeductibles_Click(sender As Object, e As EventArgs) Handles lnkSaveDeductibles.Click
        Session("valuationValue") = "False"
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

        If ActiveRVWaterIndex = "" Then
            ActiveRVWaterIndex = "false"
        End If
    End Sub

    Protected Sub lnkSaveBase_Click(sender As Object, e As EventArgs) Handles lnkSaveBase.Click
        Session("valuationValue") = "False"
        Me.Save_FireSaveEvent()

        If ActiveRVWaterIndex = "" Then
            ActiveRVWaterIndex = "false"
        End If
    End Sub

    Protected Sub lnkClearDeductibles_Click(sender As Object, e As EventArgs) Handles lnkClearDeductibles.Click
        Session("valuationValue") = "False"
        'Set dropdown lists to default values
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlDeductible, "1,000")
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlWindHailDeductible, "N/A")
    End Sub

    Protected Sub lnkClearBase_Click(sender As Object, e As EventArgs) Handles lnkClearBase.Click
        Session("valuationValue") = "False"
        'Updated 1/11/18 for HOM Upgrade MLW
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") AndAlso (Me.Quote.Locations(0).OccupancyCodeId = "4" OrElse Me.Quote.Locations(0).OccupancyCodeId = "5") Then
            'Keep Occupancy Secondary or Seasonal set to N/A
            '8/21/18 No updates needed for multi-state since this is for HOM only
            Quote.PersonalLiabilityLimitId = "0"
            Quote.MedicalPaymentsLimitid = "0"
        Else
            'Set dropdown lists to default values
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlPersonalLiability, "100,000")
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlMedicalPayments, "1,000")
        End If

        'Zero out E2Value
        lblReplacementCCValue.Text = "0"

        'Zero out Base Coverages
        If CurrentForm = "HO-6" Then
            txtDwLimit.Text = "1,000"
        Else
            txtDwLimit.Text = "0"
        End If

        txtDwellingChangeInLimit.Text = "0"
        txtDwellingTotalLimit.Text = txtDwLimit.Text
        hiddenDwellingLimit.Value = txtDwLimit.Text
        hiddenDwellingChange.Value = "0"
        hiddenDwellingTotal.Value = hiddenDwellingLimit.Value

        txtRPSLimit.Text = "0"
        txtRPSChgInLimit.Text = "0"
        txtRPSTotalLimit.Text = "0"
        hiddenRPSLimit.Value = "0"
        hiddenRPSChange.Value = "0"
        hiddenRPSTotal.Value = "0"

        txtPPLimit.Text = "0"
        txtPPChgInLimit.Text = "0"
        txtPPTotalLimit.Text = "0"
        hiddenPPLimit.Value = "0"
        hiddenPPChange.Value = "0"
        hiddenPPTotal.Value = "0"

        txtLossLimit.Text = "0"
        txtLossChgInLimit.Text = "0"
        txtLossTotalLimit.Text = "0"
        hiddenLossLimit.Value = "0"
        hiddenLossChange.Value = "0"
        hiddenLossTotal.Value = "0"

        'ctlOptionalCoverages.ClearAllCoverages()
    End Sub

    Protected Sub lnkSpecialLimitsOfLiability_Click(sender As Object, e As EventArgs) Handles lnkSpecialLimitsOfLiability.Click
        'Updated 12/20/17 for HOM Upgrade MLW
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
        Else
            If CurrentForm.Substring(0, 2) = "HO" Then
                Response.Redirect("vrHelpMe.aspx?p=VR3Home&s=hospeclimits")
            Else
                Response.Redirect("vrHelpMe.aspx?p=VR3Home&s=mlspeclimits")
            End If
        End If
    End Sub


    Protected Sub btnRateInlandMarine_Click(sender As Object, e As EventArgs)
        Session("valuationValue") = "False"
        Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.CreateAccordion(MainAccordionDivId, hiddenCoverage, "0")
        VRScript.CreateAccordion(DeductibleDivId, hiddenDeductibles, "0")
        VRScript.CreateAccordion(BaseCoverageDivId, hiddenBase, "0")
        VRScript.CreateAccordion(OptionalCoverageDivId, hiddenOptional, "false")
        VRScript.CreateAccordion(InlandMarineDivId, hiddenInlandMarine, "false")
        VRScript.CreateAccordion(InitialRVWatercraftDivId, hiddenRVWatercraft, "false", True)

        Me.VRScript.StopEventPropagation(Me.lnkClearCoverages.ClientID, False)
        Me.VRScript.StopEventPropagation(Me.lnkSaveCoverages.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkClearDeductibles.ClientID, False)
        Me.VRScript.StopEventPropagation(Me.lnkSaveDeductibles.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkClearBase.ClientID, False)
        Me.VRScript.StopEventPropagation(Me.lnkSaveBase.ClientID)

        'Me.VRScript.StopEventPropagation(Me.lnkClearInland.ClientID, False)
        'Me.VRScript.StopEventPropagation(Me.lnkSaveinland.ClientID)
        'Me.VRScript.StopEventPropagation(Me.lnkAddRVWater.ClientID)

        txtDwLimit.Attributes.Add("OnBlur", "CalculateFromDwLimit()")
        txtDwellingChangeInLimit.Attributes.Add("OnBlur", "CalculateFromDwLimit()")
        txtRPSChgInLimit.Attributes.Add("OnBlur", "CalculateFromRPSLimit()")
        txtPPLimit.Attributes.Add("OnBlur", "CalculateFromPPLimit()")
        txtPPChgInLimit.Attributes.Add("OnBlur", "CalculateFromPPLimit()")
        txtLossChgInLimit.Attributes.Add("OnBlur", "CalculateFromLossLimit()")

        txtDwLimit.Attributes.Add("onfocus", "this.select()")
        txtDwellingChangeInLimit.Attributes.Add("onfocus", "this.select()")
        txtRPSChgInLimit.Attributes.Add("onfocus", "this.select()")
        txtPPLimit.Attributes.Add("onfocus", "this.select()")
        txtPPChgInLimit.Attributes.Add("onfocus", "this.select()")
        txtLossChgInLimit.Attributes.Add("onfocus", "this.select()")
        'Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        '_script.AddScriptLine("$(""#divRVWatercraft"").find("">:first-child"").find("">:first-child"").hide();")

        'Added 2/6/18 for HOM Upgrade MLW
        If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Me.lnkSpecialLimitsOfLiability.OnClientClick = "return false;"
            Me.lnkSpecialLimitsOfLiability.CssClass = "CovTableHeaderLabel"
            Dim specLimitsPopupMessage As String = "<div><b>Included Special Limits of Liability – Homeowners</b></div><div>"
            specLimitsPopupMessage = specLimitsPopupMessage & "Antennas, Tapes, Wires, Records, Disks & Media in a Motor Vehicle - $250<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Building Additions and Alterations – 10% of Coverage C<br />"
            'updated 4/9/18 for Bug 26083 - removed word "Personal" MLW
            specLimitsPopupMessage = specLimitsPopupMessage & "Business Property Increased Limits - $2,500<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Credit Card, Electronic Fund Transfer Card or Access Device, Forgery and Counterfeit Money Coverage - $2,500<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Damage to Property of Others - $1,000<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Debris Removal - Trees - $1,000<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Fire Department Service Charge - $500<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Firearms - $2,500<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Grave Markers - $5,000<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Jewelry, Watches, Furs and Precious Stones - $1,500<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Landlord’s Furnishings - $2,500<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Money & Bank Notes - $200<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Ordinance or Law – 10% of Coverage A<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Personal Property in Self-Storage Facilities – 10% of Coverage C or $1,000 whichever is greater<br />"
            'Added 6/28/18 for HOM2011 Upgrade post go-live changes MLW
            specLimitsPopupMessage = specLimitsPopupMessage & "Personal Property – Other Residence – 10% of Coverage C<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Portable Electronics in Motor Vehicle - $1,500<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Securities, Deeds, Evidence of Debt - $1,500<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Silverware, Goldware, Pewterware - $2,500<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Trailers (Non-Watercraft) - $1,500<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Trees, Shrubs and Other Plants – 5% of Coverage A or 10% of Coverage C<br />"
            specLimitsPopupMessage = specLimitsPopupMessage & "Watercraft (Unscheduled) - $1,500"
            specLimitsPopupMessage = specLimitsPopupMessage & "</div>"
            'updated 4/9/18 for Bug 26129 - updated pop up title with correct verbiage MLW
            Using popupSpecial As New PopupMessageClass.PopupMessageObject(Me.Page, specLimitsPopupMessage, "Included Special Limits of Liability Note:")
                With popupSpecial
                    .ControlEvent = PopupMessageClass.PopupMessageObject.ControlEvents.onmouseup
                    .BindScript = PopupMessageClass.PopupMessageObject.BindTo.Control
                    .isModal = False
                    .AddButton("OK", True)
                    .width = 550
                    .height = 500
                    .AddControlToBindTo(lnkSpecialLimitsOfLiability)
                    .divId = "lnkSpecialLimitsOfLiabilityPopup"
                    .CreateDynamicPopUpWindow()
                End With
            End Using
        End If

    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.MyLocation IsNot Nothing Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddlDeductible, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddlWindHailDeductible, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.WindHailDeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddlPersonalLiability, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PersonalLiabilityLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddlMedicalPayments, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)

            'N/A was added to ddlPersonalLiability and ddlMedicalPayments in the XML file for HOM Upgrade. MLW
            'Updated 12/19/17 for HOM Upgrade MLW
            If (Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                'keep N/A options
                'Updated 5/4/18 for Bug 26572 MLW - Only show N/A options when Occupancy Type is Secondary or Seasonal
                If (Me.Quote.Locations(0).OccupancyCodeId <> "4" AndAlso Me.Quote.Locations(0).OccupancyCodeId <> "5") Then
                    'remove N/A from options on old form types
                    Dim removePLNA As ListItem = ddlPersonalLiability.Items.FindByText("N/A")
                    If removePLNA IsNot Nothing Then
                        Me.ddlPersonalLiability.Items.Remove(removePLNA)
                    End If
                    Dim removeMPNA As ListItem = ddlMedicalPayments.Items.FindByText("N/A")
                    If removeMPNA IsNot Nothing Then
                        Me.ddlMedicalPayments.Items.Remove(removeMPNA)
                    End If
                End If
            Else
                'remove N/A from options on old form types
                Dim removePLNA As ListItem = ddlPersonalLiability.Items.FindByText("N/A")
                If removePLNA IsNot Nothing Then
                    Me.ddlPersonalLiability.Items.Remove(removePLNA)
                End If
                Dim removeMPNA As ListItem = ddlMedicalPayments.Items.FindByText("N/A")
                If removeMPNA IsNot Nothing Then
                    Me.ddlMedicalPayments.Items.Remove(removeMPNA)
                End If

                '750 is not valid for ML quotes
                If Me.CurrentForm.ToLower().StartsWith("ml") Then
                    'remove 750 (23)
                    'Me.ddlDeductible.Items.RemoveAt(2)
                    Dim removeItem = (From i As ListItem In Me.ddlDeductible.Items Where i.Text = "750" Select i).FirstOrDefault()
                    If removeItem IsNot Nothing Then
                        Me.ddlDeductible.Items.Remove(removeItem)
                    End If
                End If
            End If

            RemoveDeductibleLimitOptionsLessThan1k()

            SetDefaultValues()
        End If
    End Sub

    Private Sub RemoveDeductibleLimitOptionsLessThan1k()
        If PolicyDeductibleNotLessThan1kHelper.IsPolicyDeductibleNotLessThan1kAvailable(Quote) Then
            'Remove 250
            Dim removeDeductible250 = (From i As ListItem In Me.ddlDeductible.Items Where i.Text = "250" Select i).FirstOrDefault()
            If removeDeductible250 IsNot Nothing Then
                Me.ddlDeductible.Items.Remove(removeDeductible250)
            End If
            'Remove 500
            Dim removeDeductible500 = (From i As ListItem In Me.ddlDeductible.Items Where i.Text = "500" Select i).FirstOrDefault()
            If removeDeductible500 IsNot Nothing Then
                Me.ddlDeductible.Items.Remove(removeDeductible500)
            End If
            'Remove 750
            Dim removeDeductible750 = (From i As ListItem In Me.ddlDeductible.Items Where i.Text = "750" Select i).FirstOrDefault()
            If removeDeductible750 IsNot Nothing Then
                Me.ddlDeductible.Items.Remove(removeDeductible750)
            End If
        End If
    End Sub

    Public Overrides Sub Populate()
        If Me.MyLocation IsNot Nothing Then
            LoadStaticData()
            'updated 11/30/17 for HOM Upgrade MLW
            If ((MyLocation.FormTypeId = "22" OrElse MyLocation.FormTypeId = "25") AndAlso MyLocation.StructureTypeId = "2" AndAlso Quote.LobId = 2) Then
                Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute) 'Matt A 10-27-15
                Dim a1 As New QuickQuoteStaticDataAttribute
                a1.nvp_name = "StructureTypeId" 'only way to determine mobile types on the new form types is by having StructureTypeId set to 2. Therefore, only use this to get the formType description/name for mobile on new forms. MLW
                a1.nvp_value = 2
                optionAttributes.Add(a1)
                lblCoverageForm.Text = QQHelper.GetStaticDataTextForValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, optionAttributes, Quote.Locations(0).FormTypeId, Quote.LobType)
            Else
                lblCoverageForm.Text = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, Quote.Locations(0).FormTypeId)
            End If

            'updated 11/30/17 for HOM Upgrade MLW
            If Quote.LobId = 2 Then
                Dim QQHelper As New QuickQuoteHelperClass
                hiddenSelectedForm.Value = QQHelper.GetShortFormName(Quote)
            Else
                Try
                    hiddenSelectedForm.Value = lblCoverageForm.Text.Substring(0, 4)
                Catch
                    hiddenSelectedForm.Value = ""
                End Try
            End If
            'Added 12/19/17 for HOM Upgrade MLW
            hiddenFormTypeId.Value = MyLocation.FormTypeId



            Dim RVWaterCnt As Integer = 1

            If MyLocation.RvWatercrafts IsNot Nothing Then
                If MyLocation.RvWatercrafts.Count > 1 Then
                    RVWaterCnt = MyLocation.RvWatercrafts.Count
                End If
            End If

            'lblRVWatercraftHdr.Text = "RV/WATERCRAFT #" & RVWaterCnt & " - "

            If Quote.Locations Is Nothing OrElse Quote.Locations.Count <= 0 OrElse MyLocation Is Nothing Then Exit Sub

            '***************************
            '* COVERAGES
            '***************************
            Try
                Select Case MyLocation.DeductibleLimitId
                    Case "333"
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlDeductible, "7,500")
                    Case "157"
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlDeductible, "10,000")
                    Case Else
                        If IsEndorsementRelated() Then
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddlDeductible, MyLocation.DeductibleLimitId, QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, MyLocation.DeductibleLimitId))
                        Else
                            ddlDeductible.SelectedValue = MyLocation.DeductibleLimitId
                        End If
                End Select
            Catch
                ddlDeductible.SelectedValue = "24"
            End Try

            Select Case CurrentForm
                Case "HO-4",
                    "HO-6"
                    'Updated 12/19/17 for HOM Upgrade MLW - removed 4/11/18 will function as previously did MLW
                    'If (Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso MyLocation.FormTypeId.EqualsAny("25", "26")) Then
                    '    'Updated 4/11/18 to exclude HO-6 from Wind/Hail Deductible MLW
                    '    'If CurrentForm = "HO-6" Then
                    '    '    lblWindHailDeduct.Visible = True
                    '    '    ddlWindHailDeductible.Visible = True

                    '    '    If MyLocation.WindHailDeductibleLimitId <> "" Then
                    '    '        ddlWindHailDeductible.SelectedValue = MyLocation.WindHailDeductibleLimitId
                    '    '    Else
                    '    '        ddlWindHailDeductible.SelectedValue = "0"
                    '    '    End If

                    '    'Else
                    '    lblWindHailDeduct.Visible = False
                    '    ddlWindHailDeductible.Visible = False
                    '    'End If
                    'Else
                    lblWindHailDeduct.Visible = False
                    ddlWindHailDeductible.Visible = False
                    'End If

                    txtPPChgInLimit.Enabled = False
                    txtPPChgInLimit.BackColor = Drawing.Color.LightGray
                    txtPPChgInLimit.Enabled = False
                    txtPPLimit.ToolTip = "Minimum amount must be at least 8,000"
                    lblPersPropReq.Visible = True

                    Select Case CurrentForm
                        Case "HO-4"
                            trDwelling.Visible = False
                        Case "HO-6"
                            txtDwLimit.BackColor = Drawing.Color.LightGray
                            txtDwLimit.Enabled = False
                            trDwelling.Visible = True
                            lblDwellingReq.Visible = False
                            txtDwLimit.ToolTip = "Includes 1,000 Automatically"
                    End Select

                    trStructures.Visible = False

                    ddlDeductible.Items.Remove("7,500")
                    ddlDeductible.Items.Remove("10,000")
                Case Else
                    Select Case CurrentForm
                        Case "ML-2"
                            lblWindHailDeduct.Visible = False
                            ddlWindHailDeductible.Visible = False
                            tdLossOfUse.Visible = False
                            tdLossLimit.Visible = True
                            tdLossChange.Visible = True
                            tdLossTotal.Visible = True
                            txtDwLimit.Enabled = True
                            txtDwLimit.BackColor = Drawing.Color.White
                            txtDwLimit.ForeColor = Drawing.Color.Black
                            txtDwLimit.ToolTip = "Minimum amount must be at least 6,000"
                            txtDwellingChangeInLimit.Enabled = False
                            txtDwellingChangeInLimit.BackColor = Drawing.Color.LightGray
                            txtDwellingChangeInLimit.Enabled = False
                            txtPPLimit.BackColor = Drawing.Color.LightGray
                            txtPPLimit.Enabled = False
                        Case "ML-4"
                            lblWindHailDeduct.Visible = False
                            ddlWindHailDeductible.Visible = False
                            tdLossOfUse.Visible = False
                            tdLossLimit.Visible = True
                            tdLossChange.Visible = True
                            tdLossTotal.Visible = True
                            trDwelling.Visible = False
                            trStructures.Visible = False
                            txtPPLimit.BackColor = Drawing.Color.White
                            txtPPLimit.ForeColor = Drawing.Color.Black
                            txtPPChgInLimit.Enabled = False
                            txtPPChgInLimit.BackColor = Drawing.Color.LightGray
                            txtPPChgInLimit.Enabled = False
                            lblPersPropReq.Visible = True
                            txtDwLimit.ToolTip = ""
                            txtPPLimit.ToolTip = "Minimum amount must be at least 4,000"
                        Case Else
                            lblWindHailDeduct.Visible = True
                            ddlWindHailDeductible.Visible = True
                            tdLossOfUse.Visible = True
                            tdLossLimit.Visible = False
                            tdLossChange.Visible = False
                            tdLossTotal.Visible = False
                            txtDwellingChangeInLimit.Enabled = False
                            txtDwellingChangeInLimit.BackColor = Drawing.Color.LightGray

                            If MyLocation.WindHailDeductibleLimitId <> "" Then
                                ddlWindHailDeductible.SelectedValue = MyLocation.WindHailDeductibleLimitId
                            Else
                                ddlWindHailDeductible.SelectedValue = "0"
                            End If

                            trDwelling.Visible = True
                            trStructures.Visible = True
                            lblPersPropReq.Visible = False
                            txtPPLimit.BackColor = Drawing.Color.LightGray
                            txtPPLimit.Enabled = False
                            'txtDwLimit.ToolTip = "Minimum amount must be at least 25,000"
                    End Select
            End Select

            'Cov D is always hidden with HOM2011 - just show 'Actual Sustained Loss' label - Matt A 5-4-18
            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" Then
                tdLossOfUse.Visible = True
                tdLossLimit.Visible = False
                tdLossChange.Visible = False
                tdLossTotal.Visible = False
                If Not IsQuoteEndorsement() Then
                    PersonalLiabilitylimitTextHome.Visible = True
                End If
            End If

            ' *****************************
            ' Coverages ABCD grid
            ' *****************************

            '
            ' COVERAGE A
            '
            Select Case CurrentForm
                Case "HO-2",
                    "HO-3",
                    "ML-2"
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.A_Dwelling_LimitIncreased) Then
                        txtDwLimit.Text = MyLocation.A_Dwelling_LimitIncreased
                        hiddenDwellingLimit.Value = MyLocation.A_Dwelling_LimitIncreased
                    Else
                        txtDwLimit.Text = "0"
                    End If

                    'If lblReplacementCCValue.Text = "" Or lblReplacementCCValue.Text = "0" Then
                    If QQHelper.IsZeroPremium(lblReplacementCCValue.Text) Then
                        If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.A_Dwelling_LimitIncluded) Then
                            txtDwellingChangeInLimit.Text = MyLocation.A_Dwelling_LimitIncluded
                            hiddenDwellingChange.Value = MyLocation.A_Dwelling_LimitIncluded
                        Else
                            txtDwellingChangeInLimit.Text = "0"
                        End If
                    Else
                        txtDwellingChangeInLimit.Text = hiddenDwellingChange.Value
                    End If
                Case "HO-6"
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.A_Dwelling_LimitIncluded) Then
                        txtDwLimit.Text = MyLocation.A_Dwelling_LimitIncluded
                        hiddenDwellingLimit.Value = MyLocation.A_Dwelling_LimitIncluded
                    Else
                        txtDwLimit.Text = "0"
                    End If

                    'If lblReplacementCCValue.Text = "" OrElse lblReplacementCCValue.Text = "0" Then
                    If QQHelper.IsZeroPremium(lblReplacementCCValue.Text) Then
                        If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.A_Dwelling_LimitIncreased) Then
                            txtDwellingChangeInLimit.Text = MyLocation.A_Dwelling_LimitIncreased
                            hiddenDwellingChange.Value = MyLocation.A_Dwelling_LimitIncreased
                        Else
                            txtDwellingChangeInLimit.Text = "0"
                        End If
                    Else
                        txtDwellingChangeInLimit.Text = hiddenDwellingChange.Value
                    End If
                Case Else
                    txtDwLimit.Text = "0"
                    txtDwellingChangeInLimit.Text = "0"
            End Select

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.A_Dwelling_Limit) Then
                txtDwellingTotalLimit.Text = MyLocation.A_Dwelling_Limit
                hiddenDwellingTotal.Value = MyLocation.A_Dwelling_Limit
            Else
                txtDwellingTotalLimit.Text = "0"
            End If

            '
            ' COVERAGE B
            '
            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.B_OtherStructures_LimitIncluded) Then
                txtRPSLimit.Text = MyLocation.B_OtherStructures_LimitIncluded
                hiddenRPSLimit.Value = MyLocation.B_OtherStructures_LimitIncluded
            Else
                txtRPSLimit.Text = "0"
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.B_OtherStructures_LimitIncreased) Then
                txtRPSChgInLimit.Text = MyLocation.B_OtherStructures_LimitIncreased
                hiddenRPSChange.Value = MyLocation.B_OtherStructures_LimitIncreased
            Else
                txtRPSChgInLimit.Text = "0"
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.B_OtherStructures_Limit) Then
                txtRPSTotalLimit.Text = MyLocation.B_OtherStructures_Limit
                hiddenRPSTotal.Value = MyLocation.B_OtherStructures_Limit
            Else
                txtRPSTotalLimit.Text = "0"
            End If

            '
            ' COVERAGE C
            '
            Select Case CurrentForm
                Case "HO-2",
                    "HO-3",
                    "ML-2"
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.C_PersonalProperty_LimitIncluded) Then
                        txtPPLimit.Text = MyLocation.C_PersonalProperty_LimitIncluded
                        hiddenPPLimit.Value = MyLocation.C_PersonalProperty_LimitIncluded
                    Else
                        txtPPLimit.Text = "0"
                    End If

                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.C_PersonalProperty_LimitIncreased) Then
                        txtPPChgInLimit.Text = MyLocation.C_PersonalProperty_LimitIncreased
                        hiddenPPChange.Value = MyLocation.C_PersonalProperty_LimitIncreased
                    Else
                        txtPPChgInLimit.Text = "0"
                    End If
                Case "HO-4",
                        "HO-6",
                        "ML-4"
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.C_PersonalProperty_LimitIncreased) Then
                        txtPPLimit.Text = MyLocation.C_PersonalProperty_LimitIncreased
                        hiddenPPLimit.Value = MyLocation.C_PersonalProperty_LimitIncreased
                    Else
                        txtPPLimit.Text = "0"
                    End If

                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.C_PersonalProperty_LimitIncluded) Then
                        txtPPChgInLimit.Text = MyLocation.C_PersonalProperty_LimitIncluded
                        hiddenPPChange.Value = MyLocation.C_PersonalProperty_LimitIncluded
                    Else
                        txtPPChgInLimit.Text = "0"
                    End If
                Case Else

            End Select

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.C_PersonalProperty_Limit) Then
                txtPPTotalLimit.Text = MyLocation.C_PersonalProperty_Limit
                hiddenPPTotal.Value = MyLocation.C_PersonalProperty_Limit
            Else
                txtPPTotalLimit.Text = "0"
            End If

            '
            ' COVERAGE D
            '
            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.D_LossOfUse_LimitIncluded) Then
                txtLossLimit.Text = MyLocation.D_LossOfUse_LimitIncluded
                hiddenLossLimit.Value = MyLocation.D_LossOfUse_LimitIncluded
            Else
                txtLossLimit.Text = "0"
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.D_LossOfUse_LimitIncreased) Then
                txtLossChgInLimit.Text = MyLocation.D_LossOfUse_LimitIncreased
                hiddenLossChange.Value = MyLocation.D_LossOfUse_LimitIncreased
            Else
                txtLossChgInLimit.Text = "0"
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.D_LossOfUse_Limit) Then
                txtLossTotalLimit.Text = MyLocation.D_LossOfUse_Limit
                hiddenLossTotal.Value = MyLocation.D_LossOfUse_Limit
            Else
                txtLossTotalLimit.Text = "0"
            End If

            '
            ' Cov E & F
            '
            'Updated 12/20/17 for HOM Upgrade MLW
            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26") AndAlso (MyLocation.OccupancyCodeId = "4" OrElse MyLocation.OccupancyCodeId = "5") Then
                '8/21/18 No updates needed for multi-state since this is for HOM only
                '
                ' Cov E - Personal Liability
                '
                lblPrimaryResidenceLiab.Visible = True
                Quote.PersonalLiabilityLimitId = "0"
                ddlPersonalLiability.SelectedIndex = "0"
                ddlPersonalLiability.Enabled = False

                '
                ' Cov F - Medical Payments
                '
                Quote.MedicalPaymentsLimitid = "0"
                ddlMedicalPayments.SelectedIndex = "0"
                ddlMedicalPayments.Enabled = False
            Else
                ddlPersonalLiability.Enabled = True
                ddlMedicalPayments.Enabled = True
                '8/21/18 No updates needed for multi-state since this is for HOM only
                '
                ' Cov E - Personal Liability
                '
                lblPrimaryResidenceLiab.Visible = False
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.PersonalLiabilityLimitId) Then
                    'ddlPersonalLiability.SelectedValue = Quote.PersonalLiabilityLimitId
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlPersonalLiability, Quote.PersonalLiabilityLimitId)
                Else

                End If
                '8/21/18 No updates needed for multi-state since this is for HOM only
                '
                ' Cov F - Medical Payments
                '
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.MedicalPaymentsLimitid) Then
                    'ddlMedicalPayments.SelectedValue = Quote.MedicalPaymentsLimitid
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlMedicalPayments, Quote.MedicalPaymentsLimitid)
                Else

                End If
            End If

            pnlReplacementCC.Visible = True 'defualt to set it to true
            Select Case CurrentForm
                Case "HO-4"
                    pnlReplacementCC.Visible = False
                    Exit Select
                Case "HO-6"
                    txtDwLimit.Text = "1,000"
                    txtDwellingTotalLimit.Text = (Integer.Parse(txtDwLimit.Text.Replace(",", "")) + Integer.Parse(txtDwellingChangeInLimit.Text.Replace(",", ""))).ToString("N0")
                    hiddenDwellingLimit.Value = txtDwLimit.Text
                    hiddenDwellingTotal.Value = txtDwellingTotalLimit.Text
                    pnlReplacementCC.Visible = False
                    Exit Select
                Case "ML-2"
                    pnlReplacementCC.Visible = False
                    Exit Select
                Case "ML-4"
                    pnlReplacementCC.Visible = False
                    Exit Select
                Case Else
                    Dim chc As New CommonHelperClass
                    If chc.ConfigurationAppSettingValueAsBoolean("VR_AllLines_Site360Valuation_Settings") = True Then
                        If MyLocation.StructureTypeId = "20" Then
                            pnlReplacementCC.Visible = False
                        End If
                    End If
            End Select

            'E2Value, Verisk360
            If MyLocation.PropertyValuation IsNot Nothing AndAlso MyLocation.PropertyValuation.db_propertyValuationId <> "" _
                   AndAlso MyLocation.PropertyValuation.VendorValuationId <> "" AndAlso MyLocation.RebuildCost <> "" Then

                ' Set E2Value
                lblCalValue.Visible = True
                lblReplacementCCValue.Visible = True
                lblReplacementCCValue.Text = MyLocation.RebuildCost.Substring(1).Split(".")(0)

                ' Set Coverage A
                If Session("valuationValue") = "True" Then
                    txtDwLimit.Text = lblReplacementCCValue.Text
                End If

                txtDwellingTotalLimit.Text = (Integer.Parse(txtDwLimit.Text.Replace(",", "")) + Integer.Parse(txtDwellingChangeInLimit.Text.Replace(",", ""))).ToString("N0")
                hiddenDwellingLimit.Value = txtDwLimit.Text
                hiddenDwellingChange.Value = txtDwellingChangeInLimit.Text
                hiddenDwellingTotal.Value = txtDwellingTotalLimit.Text

                ' Set Coverage B
                txtRPSLimit.Text = (Integer.Parse(txtDwellingTotalLimit.Text.Replace(",", "")) * 0.1).ToString("N0")
                txtRPSTotalLimit.Text = (Integer.Parse(txtRPSLimit.Text.Replace(",", "")) + Integer.Parse(txtRPSChgInLimit.Text.Replace(",", ""))).ToString("N0")
                hiddenRPSLimit.Value = txtRPSLimit.Text
                hiddenRPSTotal.Value = txtRPSTotalLimit.Text

                ' Set Coverage C
                txtPPLimit.Text = (Integer.Parse(txtDwellingTotalLimit.Text.Replace(",", "")) * 0.6).ToString("N0")
                txtPPTotalLimit.Text = (Integer.Parse(txtPPLimit.Text.Replace(",", "")) + Integer.Parse(txtPPChgInLimit.Text.Replace(",", ""))).ToString("N0")
                hiddenPPLimit.Value = txtPPLimit.Text
                hiddenPPTotal.Value = txtPPTotalLimit.Text

                ' Shows date
                If IsDate(MyLocation.LastCostEstimatorDate) Then

                End If
            Else
                lblCalValue.Visible = False
                lblReplacementCCValue.Visible = False
                lblReplacementCCValue.Text = ""
            End If

            'Added 7/12/2019 for Home Endorsements Project Task 38925 MLW
            If Me.IsQuoteReadOnly Then
                Dim policyNumber As String = Me.Quote.PolicyNumber
                Dim imageNum As Integer = 0
                Dim policyId As Integer = 0
                Dim toolTip As String = "Make a change to this policy"
                'Dim qqHelper As New QuickQuoteHelperClass
                Dim readOnlyViewPageUrl As String = QQHelper.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                    readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                End If

                pnlReplacementCC.Visible = False
                btnSaveBase.Visible = False
                btnRateBase.Visible = False
                btnMakeAChange.Visible = True
                btnViewGotoNextSection.Visible = True

                btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                btnMakeAChange.ToolTip = toolTip
                btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
            Else
                btnMakeAChange.Visible = False
                btnViewGotoNextSection.Visible = False
            End If

            PopulateChildControls()
            'RefreshRVWatercraft()
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If MyLocation IsNot Nothing Then
            Dim err As String = Nothing
            Dim WhereAmI As String = "G"


            ' *********************
            ' COVERAGES
            ' *********************
            ' Deductible
            Dim origDeductibleLimit As String = MyLocation.DeductibleLimitId
            MyLocation.DeductibleLimitId = ddlDeductible.SelectedValue

            ' Wind/Hail Deductible
            'Updated 12/18/17 for HOM Upgrade MLW
            If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
                If ddlWindHailDeductible.SelectedValue <> "" Then
                    Select Case CurrentForm
                        Case "ML-2", "ML-4"
                            MyLocation.WindHailDeductibleLimitId = "0"
                        Case "HO-4", "HO-6" 'Updated 4/11/18 added HO-6 back MLW
                            MyLocation.WindHailDeductibleLimitId = "0"
                        Case Else
                            MyLocation.WindHailDeductibleLimitId = ddlWindHailDeductible.SelectedValue
                    End Select
                Else
                    MyLocation.WindHailDeductibleLimitId = "0"
                End If
            Else
                If ddlWindHailDeductible.SelectedValue <> "" Then
                    MyLocation.WindHailDeductibleLimitId = ddlWindHailDeductible.SelectedValue
                Else
                    MyLocation.WindHailDeductibleLimitId = ""
                End If
            End If


            ' Note that I don't set the Premium amount on any of the Coverage sections below
            '
            '
            ' Cov A
            '
            Select Case CurrentForm
                Case "HO-2",
                     "HO-3",
                     "ML-2"
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenDwellingLimit.Value.ToString) Then
                        MyLocation.A_Dwelling_LimitIncreased = hiddenDwellingLimit.Value.ToString
                    Else
                        MyLocation.A_Dwelling_LimitIncreased = ""
                    End If

                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenDwellingChange.Value.ToString) Then
                        MyLocation.A_Dwelling_LimitIncluded = hiddenDwellingChange.Value.ToString
                    Else
                        MyLocation.A_Dwelling_LimitIncluded = ""
                    End If
                Case "HO-6"
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenDwellingLimit.Value.ToString) Then
                        MyLocation.A_Dwelling_LimitIncluded = hiddenDwellingLimit.Value.ToString
                    Else
                        MyLocation.A_Dwelling_LimitIncluded = ""
                    End If

                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenDwellingChange.Value.ToString) Then
                        MyLocation.A_Dwelling_LimitIncreased = hiddenDwellingChange.Value.ToString
                    Else
                        MyLocation.A_Dwelling_LimitIncreased = ""
                    End If
                Case Else
                    MyLocation.A_Dwelling_LimitIncreased = ""
                    MyLocation.A_Dwelling_LimitIncluded = ""
            End Select

            MyLocation.A_Dwelling_Limit = hiddenDwellingTotal.Value.ToString

            '
            ' Cov B
            '
            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenRPSLimit.Value.ToString) Then
                MyLocation.B_OtherStructures_LimitIncluded = hiddenRPSLimit.Value.ToString
            Else
                MyLocation.B_OtherStructures_LimitIncluded = ""
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenRPSChange.Value.ToString) Then
                MyLocation.B_OtherStructures_LimitIncreased = hiddenRPSChange.Value.ToString
            Else
                MyLocation.B_OtherStructures_LimitIncreased = ""
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenRPSTotal.Value.ToString) Then
                MyLocation.B_OtherStructures_Limit = hiddenRPSTotal.Value.ToString
            Else
                MyLocation.B_OtherStructures_Limit = ""
            End If

            '
            ' Cov C
            '
            Select Case CurrentForm
                Case "HO-2",
                     "HO-3",
                     "ML-2"
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenPPLimit.Value.ToString) Then
                        MyLocation.C_PersonalProperty_LimitIncluded = hiddenPPLimit.Value.ToString
                    Else
                        MyLocation.C_PersonalProperty_LimitIncluded = ""
                    End If

                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenPPChange.Value.ToString) Then
                        MyLocation.C_PersonalProperty_LimitIncreased = hiddenPPChange.Value.ToString
                    Else
                        MyLocation.C_PersonalProperty_LimitIncreased = ""
                    End If
                Case "HO-4",
                     "HO-6",
                     "ML-4"
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenPPLimit.Value.ToString) Then
                        MyLocation.C_PersonalProperty_LimitIncreased = hiddenPPLimit.Value.ToString
                    Else
                        MyLocation.C_PersonalProperty_LimitIncreased = ""
                    End If

                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenPPChange.Value.ToString) Then
                        MyLocation.C_PersonalProperty_LimitIncluded = hiddenPPChange.Value.ToString
                    Else
                        MyLocation.C_PersonalProperty_LimitIncluded = ""
                    End If
                Case Else
                    MyLocation.C_PersonalProperty_LimitIncluded = ""
                    MyLocation.C_PersonalProperty_LimitIncreased = ""
            End Select

            MyLocation.C_PersonalProperty_Limit = hiddenPPTotal.Value.ToString

            '
            ' Cov D
            '
            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenLossLimit.Value.ToString) Then
                MyLocation.D_LossOfUse_LimitIncluded = hiddenLossLimit.Value.ToString
            Else
                MyLocation.D_LossOfUse_LimitIncluded = ""
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenLossChange.Value.ToString) Then
                MyLocation.D_LossOfUse_LimitIncreased = hiddenLossChange.Value.ToString
            Else
                MyLocation.D_LossOfUse_LimitIncreased = ""
            End If

            If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenLossTotal.Value.ToString) Then
                MyLocation.D_LossOfUse_Limit = hiddenLossTotal.Value.ToString
            Else
                MyLocation.D_LossOfUse_Limit = ""
            End If

            '8/21/18 No updates needed for multi-state since this is for HOM only
            '
            ' Cov E
            '
            If ddlPersonalLiability.SelectedValue <> "" Then
                Quote.PersonalLiabilityLimitId = ddlPersonalLiability.SelectedValue
            Else
                Quote.PersonalLiabilityLimitId = ""
            End If

            '8/21/18 No updates needed for multi-state since this is for HOM only
            '
            ' Cov F
            '
            If ddlMedicalPayments.SelectedValue <> "" Then
                Quote.MedicalPaymentsLimitid = ddlMedicalPayments.SelectedValue
            Else
                Quote.MedicalPaymentsLimitid = ""
            End If

            txtDwellingChangeInLimit.Text = hiddenDwellingChange.Value.ToString()
            txtDwellingTotalLimit.Text = hiddenDwellingTotal.Value.ToString()

            txtRPSLimit.Text = hiddenRPSLimit.Value.ToString()
            txtRPSTotalLimit.Text = hiddenRPSTotal.Value.ToString()

            txtPPLimit.Text = hiddenPPLimit.Value.ToString()
            txtPPTotalLimit.Text = hiddenPPTotal.Value.ToString()

            txtLossLimit.Text = hiddenLossLimit.Value.ToString()
            txtLossTotalLimit.Text = hiddenLossTotal.Value.ToString()

            SaveChildControls()

            If PolicyDeductibleNotLessThan1kHelper.IsPolicyDeductibleNotLessThan1kAvailable(Quote) AndAlso IsQuoteEndorsement() AndAlso origDeductibleLimit <> MyLocation.DeductibleLimitId Then
                Select Case origDeductibleLimit
                    Case "21", "22", "23"
                        '21 = 250, 22 = 500, 23 = 750
                        'Only needed for Endorsement that is forcing a value from Diamond that is not in VR and is present in the drop down so that it removes the value from the drop down when it is changed.
                        RemoveDeductibleLimitOptionsLessThan1k()
                End Select
            End If

            Return True

        End If
        Return True
    End Function


    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = "Base Policy Coverage"
        'Dim divCoverages As String = dvCoverages.ClientID
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim valList = Coverages_Hom_Validator.ValidateHOMCoverages(Me.Quote, valArgs.ValidationType)

        If valList.Any() Then
            For Each v In valList
                ' *********************
                ' Base Policy Coverages
                ' *********************
                Select Case v.FieldId
                    ' Coverage A
                    Case Coverages_Hom_Validator.CovA_Increased
                        ValidationHelper.Val_BindValidationItemToControl(txtDwellingChangeInLimit, v, accordList)
                    Case Coverages_Hom_Validator.CovA_Limit
                        ValidationHelper.Val_BindValidationItemToControl(txtDwLimit, v, accordList)

                    ' Coverage B
                    Case Coverages_Hom_Validator.CovB_Increased
                        ValidationHelper.Val_BindValidationItemToControl(txtRPSChgInLimit, v, accordList)
                    Case Coverages_Hom_Validator.CovB_Limit
                        ValidationHelper.Val_BindValidationItemToControl(txtRPSLimit, v, accordList)

                    ' Coverage C
                    Case Coverages_Hom_Validator.CovC_Increased
                        ValidationHelper.Val_BindValidationItemToControl(txtPPChgInLimit, v, accordList)
                    Case Coverages_Hom_Validator.CovC_Limit
                        ValidationHelper.Val_BindValidationItemToControl(txtPPLimit, v, accordList)

                    ' Coverage D
                    Case Coverages_Hom_Validator.CovD_Increased
                        ValidationHelper.Val_BindValidationItemToControl(txtLossChgInLimit, v, accordList)
                    Case Coverages_Hom_Validator.CovD_Limit
                        ValidationHelper.Val_BindValidationItemToControl(txtLossLimit, v, accordList)
                End Select


            Next
        End If

        ValidateChildControls(valArgs)
        'PopulateChildControls() ' This should not be here Matt A - 4-1-2016
    End Sub

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        Me.ctlHomSectionCoverages.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        Exit Sub
    End Sub

End Class