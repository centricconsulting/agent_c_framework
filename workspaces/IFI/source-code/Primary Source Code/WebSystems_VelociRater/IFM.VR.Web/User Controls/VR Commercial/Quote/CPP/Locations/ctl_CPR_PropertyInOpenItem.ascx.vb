Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports System.Data
Imports System.Data.SqlClient
Imports PopupMessageClass
Public Class ctl_CPR_PropertyInOpenItem
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

    Public Property PropertyIndex As Int32
        Get
            Return ViewState.GetInt32("vs_PropertyIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_PropertyIndex") = value
        End Set
    End Property

    Private ReadOnly Property MyProperty As QuickQuote.CommonObjects.QuickQuotePropertyInTheOpenRecord
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).PropertyInTheOpenRecords.HasItemAtIndex(Me.PropertyIndex) Then
                Return Me.Quote.Locations(LocationIndex).PropertyInTheOpenRecords.GetItemAtIndex(Me.PropertyIndex)
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) Then
                Return Me.Quote.Locations(LocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return Me.PropertyIndex
        End Get
    End Property

    Public Event PIODeleteRequested(ByVal LocIndex As Integer, ByVal ItemIndex As Integer)
    'Public Event PIOClearRequested(ByVal LocIndex As Integer, ByVal ItemIndex As Integer)
    'Public Event PIOChanged(ByVal LocIndex As Integer, ByVal ItemIndex As Integer)

#End Region

#Region "Methods and Functions"

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Delete?")
        Me.VRScript.CreateConfirmDialog(Me.lnkClear.ClientID, "Clear?")

        ' These variables are needed for the class code lookup script
        Me.VRScript.AddVariableLine("var txtPIOClassCode = '" & txtSpecialClassCode.ClientID & "';")
        Me.VRScript.AddVariableLine("var txtPIODescription = '" & txtDescription.ClientID & "';")
        Me.VRScript.AddVariableLine("var txtPIOID = '" & txtClassCodeId.ClientID & "';")
        Me.VRScript.AddVariableLine("var PIOLookuWindowID = '" & Me.ctl_PIOClassCodeLookup.ClientID & "';")

        Me.VRScript.CreateJSBinding(Me.chkIncludedInBlanketRating, ctlPageStartupScript.JsEventType.onchange, "Cpr.BlanketCheckboxChanged('" & Me.chkIncludedInBlanketRating.ClientID & "','" & Me.trBlanketInfoRow.ClientID & "');")
    End Sub

    Public Overrides Sub LoadStaticData()
        If ddValuation.Items Is Nothing OrElse ddValuation.Items.Count <= 0 Then
            ' Valuation
            QQHelper.LoadStaticDataOptionsDropDown(ddValuation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BlanketContentsValuationId, , Quote.LobType)
            ' Deductible
            QQHelper.LoadStaticDataOptionsDropDown(ddDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, , Quote.LobType)
            ' Cause of Loss
            QQHelper.LoadStaticDataOptionsDropDown(ddCauseOfLoss, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BlanketContentsCauseOfLossTypeId, , Quote.LobType)
            ' Coinsurance
            QQHelper.LoadStaticDataOptionsDropDown(ddCoinsurance, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BlanketContentsCoinsuranceTypeId, , Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        Dim err As String = Nothing

        LoadStaticData()
        ClearInputFields()

        ' Set the defaults
        SetFromValue(ddCauseOfLoss, "4")
        SetFromValue(ddCoinsurance, "5")
        SetFromValue(ddValuation, "1")
        SetFromValue(ddDeductible, "8")

        If MyProperty IsNot Nothing Then
            UpdateHeader()
            txtClassCodeId.Text = MyProperty.SpecialClassCodeTypeId
            If MyProperty.SpecialClassCodeType.ToUpper <> "N/A" Then txtSpecialClassCode.Text = MyProperty.SpecialClassCodeType Else txtSpecialClassCode.Text = String.Empty
            txtCoverageLimit.Text = MyProperty.Limit
            txtDescription.Text = MyProperty.Description
            SetFromValue(ddValuation, MyProperty.ValuationId, "0")
            chkIncludedInBlanketRating.Checked = MyProperty.IncludedInBlanketCoverage
            If chkIncludedInBlanketRating.Checked Then
                trBlanketInfoRow.Attributes.Add("style", "display:''")
            Else
                trBlanketInfoRow.Attributes.Add("style", "display:none")
            End If
            SetFromValue(ddDeductible, MyProperty.DeductibleId, "0")
            SetFromValue(ddCauseOfLoss, MyProperty.CauseOfLossTypeId, "0")
            chkEarthquake.Checked = MyProperty.EarthquakeApplies
            SetFromValue(ddCoinsurance, MyProperty.CoinsuranceTypeId, "0")
            'chkAgreedAmount.Checked = ??
        End If

        Me.PopulateChildControls()

        Exit Sub
    End Sub

    Public Overrides Function Save() As Boolean
        If MyProperty IsNot Nothing Then
            If txtClassCodeId.Text <> "" Then
                MyProperty.SpecialClassCodeTypeId = txtClassCodeId.Text
                MyProperty.Limit = txtCoverageLimit.Text
                MyProperty.Description = txtDescription.Text
                MyProperty.IncludedInBlanketCoverage = chkIncludedInBlanketRating.Checked
                MyProperty.CauseOfLossTypeId = ddCauseOfLoss.SelectedValue
                MyProperty.DeductibleId = ddDeductible.SelectedValue
                MyProperty.OptionalTheftDeductibleId = ddDeductible.SelectedValue ' copied from old look & feel
                MyProperty.OptionalWindstormOrHailDeductibleId = ddDeductible.SelectedValue ' copied from old look & feel
                MyProperty.CoinsuranceTypeId = ddCoinsurance.SelectedValue
                MyProperty.ValuationId = ddValuation.SelectedValue
                MyProperty.EarthquakeApplies = chkEarthquake.Checked
                MyProperty.ConstructionTypeId = "1"  ' frame - copied from old look & feel
                MyProperty.RatingTypeId = "3" ' Special Class Rate - copied from old look & feel
                MyProperty.ProtectionClassId = MyLocation.ProtectionClassId 'copied from old look & feel
            Else
                ' If the class code id field is empty then the class code has been cleared or was not set
                MyProperty.SpecialClassCodeTypeId = ""
                MyProperty.Limit = ""
                MyProperty.Description = ""
                MyProperty.IncludedInBlanketCoverage = False
                MyProperty.CauseOfLossTypeId = ""
                MyProperty.DeductibleId = ""
                MyProperty.OptionalTheftDeductibleId = ""
                MyProperty.OptionalWindstormOrHailDeductibleId = ""
                MyProperty.CoinsuranceTypeId = ""
                MyProperty.ValuationId = ""
                MyProperty.EarthquakeApplies = False
                MyProperty.ConstructionTypeId = ""
                MyProperty.RatingTypeId = ""
                MyProperty.ProtectionClassId = ""
            End If
            'MyProperty.agreedamount = ???
        End If
        UpdateHeader()

        Me.SaveChildControls()

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Me.ValidationHelper.GroupName = "Location #" & LocationIndex + 1 & ", Property #" & PropertyIndex + 1.ToString

        If txtClassCodeId.Text = "" Then
            Me.ValidationHelper.AddError("No Class Code was selected.")
        End If
        If txtCoverageLimit.Text.Trim = "" Then
            Me.ValidationHelper.AddError(txtCoverageLimit, "Missing Coverage Limit", accordList)
        End If

        Me.ValidateChildControls(valArgs)

        Exit Sub
    End Sub

    Private Sub ClearInputFields()
        txtSpecialClassCode.Text = ""
        txtClassCodeId.Text = ""
        txtDescription.Text = ""
        txtCoverageLimit.Text = ""
        If ddValuation.Items.Count > 0 Then ddValuation.SelectedIndex = 0 Else ddValuation.SelectedIndex = -1
        chkIncludedInBlanketRating.Checked = False
        If ddDeductible.Items.Count > 0 Then ddDeductible.SelectedIndex = 0 Else ddDeductible.SelectedIndex = -1
        If ddCauseOfLoss.Items.Count > 0 Then ddCauseOfLoss.SelectedIndex = 0 Else ddCauseOfLoss.SelectedIndex = -1
        chkEarthquake.Checked = False
        If ddCoinsurance.Items.Count > 0 Then ddCoinsurance.SelectedIndex = 0 Else ddCoinsurance.SelectedIndex = -1
        chkAgreedAmount.Checked = False
        UpdateHeader()
        Exit Sub
    End Sub

    Public Sub UpdateHeader()
        lblAccordHeader.Text = "Property"
        If MyProperty IsNot Nothing Then
            Dim dsc As String = MyProperty.Description.ToUpper
            If dsc.Length > 20 Then dsc = MyProperty.Description.Substring(0, 20).ToUpper & "..."
            lblAccordHeader.Text = "Property # " & PropertyIndex + 1.ToString & " - " & dsc
        End If
    End Sub

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ctl_PIOClassCodeLookup.txtClassCodeId = Me.txtSpecialClassCode.ClientID
        Me.ctl_PIOClassCodeLookup.txtID = Me.txtClassCodeId.ClientID
        Exit Sub
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
        Exit Sub
    End Sub

    Private Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        ClearInputFields()
        Save_FireSaveEvent(False)
    End Sub

    Private Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        RaiseEvent PIODeleteRequested(LocationIndex, PropertyIndex)
    End Sub

    Private Sub btnClassCodeLookup_Click(sender As Object, e As EventArgs) Handles btnClassCodeLookup.Click
        Me.ctl_PIOClassCodeLookup.Show()
    End Sub

#End Region


End Class