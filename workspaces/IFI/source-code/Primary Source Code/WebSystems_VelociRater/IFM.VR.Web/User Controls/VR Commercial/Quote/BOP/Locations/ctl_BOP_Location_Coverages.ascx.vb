Imports QuickQuote.CommonObjects
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports IFM.VR.Web.Helpers.WebHelper_Personal
Imports IFM.Common.InputValidation.InputHelpers
Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports QuickQuote.CommonMethods
Imports IFM.VR.Common.Helpers

Public Class ctl_BOP_Location_Coverages
    Inherits VRControlBase

#Region "Declarations"

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

#End Region

#Region "Methods and Functions"

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "0")
        Me.VRScript.CreateConfirmDialog(Me.lnkClear.ClientID, "Clear?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)

        If Me.Quote IsNot Nothing AndAlso QuickQuote.CommonMethods.QuickQuoteHelperClass.IsValidEffectiveDateForMBREquipmentBreakdownVersion(Me.Quote.EffectiveDate) = True Then 'added 11/13/2017; original logic in ELSE
            'EB MBR doesn't use dropdown
        Else
            chkEquipmentBreakdown.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkEquipmentBreakdown.ClientID & "','" & trEquipmentBreakdownRow.ClientID & "','','');")
        End If
        chkMoneySecuritiesONPremises.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkMoneySecuritiesONPremises.ClientID & "','" & trMoneySecuritiesONPremisesRow.ClientID & "','','');")
        chkMoneySecuritiesOFFPremises.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkMoneySecuritiesOFFPremises.ClientID & "','" & trMoneySecuritiesOFFPremisesRow.ClientID & "','','');")
        chkOutdoorSigns.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkOutdoorSigns.ClientID & "','" & trOutdoorSignsRow.ClientID & "','','');")

    End Sub

    Public Overrides Sub Populate()
        Try
            If Me.Quote IsNot Nothing Then
                LoadStaticData()

                If Me.Quote.Locations(Me.LocationIndex) IsNot Nothing Then
                    Dim loc As QuickQuoteLocation = Me.Quote.Locations(Me.LocationIndex)

                    ' Equipment Breakdown
                    'If loc.EquipmentBreakdownDeductibleId IsNot Nothing AndAlso IsNumeric(loc.EquipmentBreakdownDeductibleId) Then
                    '    chkEquipmentBreakdown.Checked = True
                    '    trEquipmentBreakdownRow.Attributes.Add("style", "display:''")
                    '    'trEquipmentBreakdownRow.Visible = True
                    '    ddlEquipmentBreakdownDeductible.SelectedValue = loc.EquipmentBreakdownDeductibleId
                    'Else
                    '    chkEquipmentBreakdown.Checked = False
                    '    trEquipmentBreakdownRow.Attributes.Add("style", "display:none")
                    'End If
                    'updated 11/13/2017
                    Me.trEB_MBR_Ineligible.Visible = False
                    Me.chkEquipmentBreakdown.Enabled = True
                    If QuickQuote.CommonMethods.QuickQuoteHelperClass.IsValidEffectiveDateForMBREquipmentBreakdownVersion(Me.Quote.EffectiveDate) = False Then
                        If loc.EquipmentBreakdownDeductibleId IsNot Nothing AndAlso IsNumeric(loc.EquipmentBreakdownDeductibleId) Then
                            chkEquipmentBreakdown.Checked = True
                            trEquipmentBreakdownRow.Attributes.Add("style", "display:''")
                            'trEquipmentBreakdownRow.Visible = True
                            ddlEquipmentBreakdownDeductible.SelectedValue = loc.EquipmentBreakdownDeductibleId
                        Else
                            chkEquipmentBreakdown.Checked = False
                            trEquipmentBreakdownRow.Attributes.Add("style", "display:none")
                        End If
                    Else
                        'IsSpecialEquipmentBreakdownMBRIneligibleRiskGrade -ineligible list removed 3/24/2022 for bug 72793 MLW
                        'Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                        'Dim currentlyHasEquipBreakdown As Boolean = qqHelper.EquipmentBreakdownDeductibleIdLooksValid(loc.EquipmentBreakdownDeductibleId)
                        'If qqHelper.IsSpecialEquipmentBreakdownMBRIneligibleRiskGrade(qqHelper.IntegerForString(Me.Quote.RiskGradeLookupId)) = True Then
                        '    Dim msgToShow As String = "Underwriting Approval required for Equipment Breakdown on this risk."
                        '    If currentlyHasEquipBreakdown = True Then
                        '        'show msg that Equipment Breakdown is being removed because of ineligible risk grade
                        '        msgToShow &= " Coverage has been removed."
                        '    Else
                        '        'show msg that Equipment Breakdown is unavailable because of ineligible risk grade

                        '    End If
                        '    msgToShow &= " Continue building quote then contact Underwriting for Equipment Breakdown quote."
                        '    Me.lblEB_MBR_Ineligible.Text = msgToShow
                        '    chkEquipmentBreakdown.Checked = False
                        '    'Updated 09/09/2021 for BOP Endorsements Task 63912 MLW
                        '    If IsQuoteReadOnly() Then
                        '        Me.trEB_MBR_Ineligible.Visible = False
                        '    Else
                        '        Me.trEB_MBR_Ineligible.Visible = True
                        '    End If
                        '    'Me.trEB_MBR_Ineligible.Visible = True
                        '    Me.chkEquipmentBreakdown.Enabled = False
                        'Else
                        'chkEquipmentBreakdown.Checked = currentlyHasEquipBreakdown
                        'updated 11/16/2017 to default for BOP
                        Me.lblEB_MBR_Ineligible.Text = "Equipment Breakdown is required for BOP policies. The premium is included in the quote."
                        chkEquipmentBreakdown.Checked = True
                        'Updated 09/09/2021 for BOP Endorsements Task 63912 MLW
                        If IsQuoteReadOnly() Then
                            Me.trEB_MBR_Ineligible.Visible = False
                        Else
                            Me.trEB_MBR_Ineligible.Visible = True
                        End If
                        'Me.trEB_MBR_Ineligible.Visible = True
                        Me.chkEquipmentBreakdown.Enabled = False
                        'End If
                        trEquipmentBreakdownRow.Attributes.Add("style", "display:none")
                    End If

                    ' M&S ON
                    If loc.MoneySecuritiesOnPremises IsNot Nothing AndAlso IsNumeric(loc.MoneySecuritiesOnPremises) Then
                        chkMoneySecuritiesONPremises.Checked = True
                        trMoneySecuritiesONPremisesRow.Attributes.Add("style", "display:''")
                        If loc.MoneySecuritiesOnPremises <> "0" Then
                            txtMoneySecuritiesONPremisesLimit.Text = loc.MoneySecuritiesOnPremises
                        End If
                    Else
                        chkMoneySecuritiesONPremises.Checked = False
                        trMoneySecuritiesONPremisesRow.Attributes.Add("style", "display:none")
                    End If

                    ' M&S OFF
                    If loc.MoneySecuritiesOffPremises IsNot Nothing AndAlso IsNumeric(loc.MoneySecuritiesOffPremises) Then
                        chkMoneySecuritiesOFFPremises.Checked = True
                        trMoneySecuritiesOFFPremisesRow.Attributes.Add("style", "display:''")
                        If loc.MoneySecuritiesOffPremises <> "0" Then
                            txtMoneySecuritiesOFFPremisesLimit.Text = loc.MoneySecuritiesOffPremises
                        End If
                    Else
                        chkMoneySecuritiesOFFPremises.Checked = False
                        trMoneySecuritiesOFFPremisesRow.Attributes.Add("style", "display:none")
                    End If

                    ' OUTDOOR SIGNS
                    If loc.OutdoorSignsLimit IsNot Nothing AndAlso IsNumeric(loc.OutdoorSignsLimit) Then
                        chkOutdoorSigns.Checked = True
                        trOutdoorSignsRow.Attributes.Add("style", "display:''")
                        If loc.OutdoorSignsLimit <> "0" Then
                            txtOutdoorSignsLimit.Text = loc.OutdoorSignsLimit
                        End If
                    Else
                        chkOutdoorSigns.Checked = False
                        trOutdoorSignsRow.Attributes.Add("style", "display:none")
                    End If

                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddlWindHail, loc.WindHailDeductibleLimitId, QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.WindHailDeductibleLimitId, loc.WindHailDeductibleLimitId))
                End If

                'Added 09/09/2021 for BOP Endorsements Task 63912 MLW
                If IsQuoteReadOnly() Then
                    trMoneyAndSecuritiesOnPremisesInfoText.Attributes.Add("style", "display:none")
                    trMoneyAndSecuritiesOffPremisesInfoText.Attributes.Add("style", "display:none")
                    trOutdoorSignsInfoText.Attributes.Add("style", "display:none")
                    trRedInfoText.Attributes.Add("style", "display:none")
                End If
            End If
        Catch ex As Exception
            HandleError("Populate", ex)
            Exit Sub
        End Try
    End Sub

    Public Overrides Sub LoadStaticData()
        '' Doesn't work for some reason MGB
        'QQHelper.LoadStaticDataOptionsDropDown(Me.ddlEquipmentBreakdownDeductible, QuickQuoteClassName.QuickQuoteName, QuickQuotePropertyName.EquipmentBreakdownDeductibleId, SortBy.None, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddlWindHail, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.WindHailDeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)

        Dim NAOption As ListItem = ddlWindHail.Items.FindByText("N/A")
        If OnePctWindHailLessorsRiskHelper.IsOnePctWindHailLessorsRiskAvailable(Quote) Then
            Dim removeNAWindHailOption As Boolean = OnePctWindHailLessorsRiskHelper.CheckWindHailLessorsRisk(Quote, LocationIndex)
            If removeNAWindHailOption Then
                If NAOption IsNot Nothing Then
                    ddlWindHail.Items.Remove(NAOption)
                End If
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Dim loc As QuickQuoteLocation = Nothing
        Dim newloc As Boolean = False

        Try
            If Me.Quote IsNot Nothing Then
                If Me.Quote.Locations(LocationIndex) Is Nothing Then
                    loc = New QuickQuoteLocation()
                    newloc = True
                Else
                    loc = Me.Quote.Locations(LocationIndex)
                End If

                If chkEquipmentBreakdown.Checked Then
                    If QuickQuote.CommonMethods.QuickQuoteHelperClass.IsValidEffectiveDateForMBREquipmentBreakdownVersion(Me.Quote.EffectiveDate) = False Then 'added 11/13/2017; original logic in IF; new logic in ELSE
                        loc.EquipmentBreakdownDeductibleId = ddlEquipmentBreakdownDeductible.SelectedValue
                    Else
                        loc.EquipmentBreakdownDeductibleId = QQHelper.DefaultMBREquipmentBreakdownDeductibleId().ToString
                    End If
                Else
                    loc.EquipmentBreakdownDeductibleId = ""
                End If

                If chkMoneySecuritiesONPremises.Checked Then
                    If txtMoneySecuritiesONPremisesLimit.Text.Trim <> "" Then
                        loc.MoneySecuritiesOnPremises = txtMoneySecuritiesONPremisesLimit.Text
                    Else
                        loc.MoneySecuritiesOnPremises = "0"
                    End If
                Else
                    loc.MoneySecuritiesOnPremises = ""
                End If

                If chkMoneySecuritiesOFFPremises.Checked Then
                    If txtMoneySecuritiesOFFPremisesLimit.Text.Trim <> "" Then
                        loc.MoneySecuritiesOffPremises = txtMoneySecuritiesOFFPremisesLimit.Text
                    Else
                        loc.MoneySecuritiesOffPremises = "0"
                    End If
                Else
                    loc.MoneySecuritiesOffPremises = ""
                End If

                If chkOutdoorSigns.Checked Then
                    If txtOutdoorSignsLimit.Text.Trim <> "" Then
                        loc.OutdoorSignsLimit = txtOutdoorSignsLimit.Text
                    Else
                        loc.OutdoorSignsLimit = "0"
                    End If
                Else
                    loc.OutdoorSignsLimit = ""
                End If

                loc.WindHailDeductibleLimitId = ddlWindHail.SelectedValue
            End If

            Populate()

            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Try
            MyBase.ValidateControl(valArgs)
            ValidationHelper.GroupName = "Location #" & LocationIndex + 1 & " Location Level Coverages"
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            lblMsg.Text = String.Empty

            If chkEquipmentBreakdown.Checked Then
                If Me.Quote IsNot Nothing AndAlso QuickQuote.CommonMethods.QuickQuoteHelperClass.IsValidEffectiveDateForMBREquipmentBreakdownVersion(Me.Quote.EffectiveDate) = False Then 'added IF 11/9/2017
                    If ddlEquipmentBreakdownDeductible.SelectedIndex < 0 Then
                        Me.ValidationHelper.AddError(ddlEquipmentBreakdownDeductible, "Missing Deductible", accordList)
                    End If
                End If
            End If

            If chkMoneySecuritiesONPremises.Checked Then
                If txtMoneySecuritiesONPremisesLimit.Text = String.Empty Then
                    Me.ValidationHelper.AddError(txtMoneySecuritiesONPremisesLimit, "Missing Total Limit", accordList)
                Else
                    If Not IsNumeric(txtMoneySecuritiesONPremisesLimit.Text) Then
                        Me.ValidationHelper.AddError(txtMoneySecuritiesONPremisesLimit, "Total Limit is invalid", accordList)
                    ElseIf CDec(txtMoneySecuritiesONPremisesLimit.Text) <= 10000 Then
                        Me.ValidationHelper.AddError("Total Limit must be greater than $10,000", txtMoneySecuritiesONPremisesLimit.ClientID)
                    End If
                End If
            End If
            If chkMoneySecuritiesOFFPremises.Checked Then
                If txtMoneySecuritiesOFFPremisesLimit.Text = String.Empty Then
                    Me.ValidationHelper.AddError(txtMoneySecuritiesOFFPremisesLimit, "Missing Total Limit", accordList)
                Else
                    If Not IsNumeric(txtMoneySecuritiesOFFPremisesLimit.Text) Then
                        Me.ValidationHelper.AddError(txtMoneySecuritiesOFFPremisesLimit, "Total Limit is invalid", accordList)
                    ElseIf CDec(txtMoneySecuritiesOFFPremisesLimit.Text) <= 5000 Then
                        Me.ValidationHelper.AddError(txtMoneySecuritiesOFFPremisesLimit, "Total Limit must be greater than $5,000", accordList)
                    End If
                End If
            End If
            If chkOutdoorSigns.Checked Then
                If txtOutdoorSignsLimit.Text = String.Empty Then
                    Me.ValidationHelper.AddError(txtOutdoorSignsLimit, "Missing Total Limit", accordList)
                Else
                    If Not IsNumeric(txtOutdoorSignsLimit.Text) Then
                        Me.ValidationHelper.AddError(txtOutdoorSignsLimit, "Total Limit must be numeric.", accordList)
                    ElseIf CDec(txtOutdoorSignsLimit.Text) <= 10000 Then
                        Me.ValidationHelper.AddError(txtOutdoorSignsLimit, "Total Limit must be greater than $10,000", accordList)
                    End If
                End If
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("ValidateControl", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub HandleError(ByVal RoutineName As String, ByVal ex As Exception)
        Dim str As String = RoutineName & ":  " & ex.Message
        If AppSettings("TestOrProd").ToUpper <> "PROD" Then lblMsg.Text = str Else Throw New Exception(ex.Message, ex)
    End Sub

#End Region


#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim js As String = "if(this.checked == false) { if (confirm(""Are you sure you want to delete this coverage?"")) {__doPostBack('');return true} else {return false;}}"
        Try
            Me.MainAccordionDivId = Me.divMain.ClientID
            If Not IsPostBack Then
                'chkEquipmentBreakdown.Attributes.Add("onclick", js)
                'chkMoneySecuritiesONPremises.Attributes.Add("onclick", js)
                'chkMoneySecuritiesOFFPremises.Attributes.Add("onclick", js)
                'chkOutdoorSigns.Attributes.Add("onclick", js)

                chkEquipmentBreakdown.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkEquipmentBreakdown.ClientID & "','" & trEquipmentBreakdownRow.ClientID & "','','');")
                chkMoneySecuritiesOFFPremises.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkMoneySecuritiesOFFPremises.ClientID & "','" & trMoneySecuritiesOFFPremisesRow.ClientID & "','','');")
                chkMoneySecuritiesONPremises.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkMoneySecuritiesONPremises.ClientID & "','" & trMoneySecuritiesONPremisesRow.ClientID & "','','');")
                chkOutdoorSigns.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkOutdoorSigns.ClientID & "','" & trOutdoorSignsRow.ClientID & "','','');")

            End If

            Exit Sub
        Catch ex As Exception
            HandleError("Page_Load", ex)
            Exit Sub
        End Try
    End Sub


    'Private Sub chkEquipmentBreakdown_CheckedChanged(sender As Object, e As EventArgs) Handles chkEquipmentBreakdown.CheckedChanged
    '    Try
    '        If chkEquipmentBreakdown.Checked Then
    '            trEquipmentBreakdownRow.Visible = True
    '        Else
    '            trEquipmentBreakdownRow.Visible = False
    '        End If

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("chkEquipmentBreakdown_CheckedChanged", ex)
    '    End Try
    'End Sub

    'Private Sub chkMoneySecuritiesONPremises_CheckedChanged(sender As Object, e As EventArgs) Handles chkMoneySecuritiesONPremises.CheckedChanged
    '    Try
    '        If chkMoneySecuritiesONPremises.Checked Then
    '            trMoneySecuritiesONPremisesRow.Visible = True
    '        Else
    '            trMoneySecuritiesONPremisesRow.Visible = False
    '        End If

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("chkMoneySecuritiesONPremises_CheckedChanged", ex)
    '        Exit Sub
    '    End Try
    'End Sub

    'Private Sub chkMoneySecuritiesOFFPremises_CheckedChanged(sender As Object, e As EventArgs) Handles chkMoneySecuritiesOFFPremises.CheckedChanged
    '    Try

    '        If chkMoneySecuritiesOFFPremises.Checked Then
    '            trMoneySecuritiesOFFPremisesRow.Visible = True
    '        Else
    '            trMoneySecuritiesOFFPremisesRow.Visible = False
    '        End If

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("chkMoneySecuritiesOFFPremises_CheckedChanged", ex)
    '        Exit Sub
    '    End Try
    'End Sub

    'Private Sub chkOutdoorSigns_CheckedChanged(sender As Object, e As EventArgs) Handles chkOutdoorSigns.CheckedChanged
    '    Try
    '        If chkOutdoorSigns.Checked Then
    '            trOutdoorSignsRow.Visible = True
    '        Else
    '            trOutdoorSignsRow.Visible = False
    '        End If

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("chkOutdoorSigns_CheckedChanged", ex)
    '        Exit Sub
    '    End Try
    'End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Private Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        Try
            chkEquipmentBreakdown.Checked = False
            ddlEquipmentBreakdownDeductible.SelectedIndex = -1
            trEquipmentBreakdownRow.Visible = False

            chkMoneySecuritiesONPremises.Checked = False
            txtMoneySecuritiesONPremisesLimit.Text = String.Empty
            trMoneySecuritiesONPremisesRow.Visible = False

            chkMoneySecuritiesOFFPremises.Checked = False
            txtMoneySecuritiesOFFPremisesLimit.Text = String.Empty
            trMoneySecuritiesOFFPremisesRow.Visible = False

            chkOutdoorSigns.Checked = False
            txtOutdoorSignsLimit.Text = String.Empty
            trOutdoorSignsRow.Visible = False

            Exit Sub
        Catch ex As Exception
            HandleError("lnkClear_Click", ex)
            Exit Sub
        End Try
    End Sub

#End Region

End Class