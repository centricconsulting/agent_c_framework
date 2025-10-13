Imports IFM.PrimativeExtensions

Public Class ctl_Endo_CTEQList
    Inherits VRControlBase

    '6/7/2017 - added new properties for ContractorsToolsEquipmentScheduled; QuickQuoteObject.ContractorsToolsEquipmentScheduled always returns total from list now, so OriginalTotalScheduledAmount should be used in Populate so it's added to ViewState at the beginning and is available later on
    Public ReadOnly Property OriginalTotalScheduledAmount As String
        Get
            Dim origTotSchedAmt As String = ""
            If Session($"ContractorsToolsEquipmentScheduled_{Me.QuoteId.ToString()}") IsNot Nothing Then
                origTotSchedAmt = Session($"ContractorsToolsEquipmentScheduled_{Me.QuoteId.ToString()}").ToString
            Else
                origTotSchedAmt = Me.CurrentTotalScheduledAmount
                Session.Add($"ContractorsToolsEquipmentScheduled_{Me.QuoteId.ToString()}", origTotSchedAmt)
            End If
            Return origTotSchedAmt
        End Get
    End Property
    Public ReadOnly Property CurrentTotalScheduledAmount As String
        Get
            If Me.Quote IsNot Nothing Then
                Return GoverningStateQuote.ContractorsToolsEquipmentScheduled
            Else
                Return ""
            End If
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()
    End Sub

    Protected Sub AttachControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim ContEqControl As ctl_Endo_CTEQ = cntrl.FindControl("ctlScheduledItem")
            AddHandler ContEqControl.ContractorsItemDeleteRequested, AddressOf DeleteItemRequested
            AddHandler ContEqControl.ContractorsItemChanged, AddressOf ItemChanged
            index += 1
        Next
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "0")
        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccordList, "0")
        Me.VRScript.StopEventPropagation(Me.lnkBtnAdd.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
        If IsEndorsementRelated() Then
            Me.VRScript.AddVariableLine($"var OriginalTotalContractorsScheduledEquipmentAmount = {IFM.Common.InputValidation.InputHelpers.TryToGetInt32(Me.OriginalTotalScheduledAmount)};") ' Matt A 6-8-17
            Me.VRScript.AddScriptLine("$('.txtCTEQLimit').each(function () {$(this).keyup(function () { CalculateContractorEquipmentScheduledAmount(); })});")
            Me.VRScript.AddScriptLine("CalculateContractorEquipmentScheduledAmount();")
        Else
            Me.VRScript.AddVariableLine($"var OriginalTotalContractorsScheduledEquipmentAmount = {IFM.Common.InputValidation.InputHelpers.TryToGetInt32(Me.OriginalTotalScheduledAmount)};") ' Matt A 6-8-17
            Me.VRScript.AddScriptLine("$('.txtCTEQLimit').each(function () {$(this).keyup(function () { CalculateContractorEquipmentRemaining(); })});")
            Me.VRScript.AddScriptLine("CalculateContractorEquipmentRemaining();")
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If GoverningStateQuote() IsNot Nothing Then
            Dim origTotSchedAmt As String = Me.OriginalTotalScheduledAmount
            If QQHelper.IsPositiveDecimalString(origTotSchedAmt) = True Then
                txtScheduledAmount.Text = Format(CDec(origTotSchedAmt), "$###,###,###,###")
            Else
                txtScheduledAmount.Text = ""
            End If

            Repeater1.DataSource = GoverningStateQuote.ContractorsEquipmentScheduledItems
            Repeater1.DataBind()

            Me.FindChildVrControls()
            Dim ndx As Int32 = 0
            For Each cnt In Me.GatherChildrenOfType(Of ctl_Endo_CTEQ)
                cnt.ItemIndex = ndx
                cnt.Populate()
                ndx += 1
            Next
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divContractorsEquipmentList.ClientID
        Me.ListAccordionDivId = Me.divList.ClientID
        AttachControlEvents()
        Me.Visible = IFM.Common.InputValidation.InputHelpers.TryToGetInt32(OriginalTotalScheduledAmount) > 0 OrElse IsEndorsementRelated() = True ' only show if there is something to itemize
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = "Contractors Equipment Scheduled Items"

        'If Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications Is Nothing OrElse Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications.Count = 0 Then
        '    Me.ValidationHelper.AddError("Building must have at least one Classification assigned.", Nothing)
        'Else
        '    Dim PrimaryClassSet As Boolean = False
        '    For Each c As QuickQuote.CommonObjects.QuickQuoteClassification In Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications
        '        If c.PredominantOccupancy Then
        '            PrimaryClassSet = True
        '            Exit For
        '        End If
        '    Next
        '    If Not PrimaryClassSet Then
        '        Me.ValidationHelper.AddError("The 'Primary Classification' checkbox must be checked on one classification", Nothing)
        '    End If
        'End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkBtnAdd.Click
        Dim ctitem As New QuickQuote.CommonObjects.QuickQuoteContractorsEquipmentScheduledItem()
        If IsQuoteEndorsement() Then
            ctitem.Limit = "1"
        End If


        If Me.GoverningStateQuote IsNot Nothing Then
            If GoverningStateQuote.ContractorsEquipmentScheduledItems Is Nothing Then GoverningStateQuote.ContractorsEquipmentScheduledItems = New List(Of QuickQuote.CommonObjects.QuickQuoteContractorsEquipmentScheduledItem)
            GoverningStateQuote.ContractorsEquipmentScheduledItems.Add(ctitem)
            Save_FireSaveEvent(False)
            Me.Populate()
            Me.hdnAccordList.Value = GoverningStateQuote.ContractorsEquipmentScheduledItems?.Count - 1.ToString
        End If
    End Sub

    Private Sub DeleteItemRequested(ByVal ItemIndex As Integer)
        Save_FireSaveEvent(False) ' save screen as is
        If GoverningStateQuote() IsNot Nothing AndAlso (GoverningStateQuote.ContractorsEquipmentScheduledItems IsNot Nothing AndAlso GoverningStateQuote.ContractorsEquipmentScheduledItems.Count > 0) Then
            If GoverningStateQuote.ContractorsEquipmentScheduledItems.HasItemAtIndex(ItemIndex) Then
                GoverningStateQuote.ContractorsEquipmentScheduledItems.RemoveAt(ItemIndex) ' remove
            End If
            Me.hdnAccordList.Value = Me.GoverningStateQuote.ContractorsEquipmentScheduledItems.Count - 1.ToString
        End If
        Populate() ' update screen with removed item
        Save_FireSaveEvent(False) ' save screen again

    End Sub

    Private Sub ItemChanged(ByVal ItemIndex As Integer)
        Dim origTotSchedAmt As String = Me.OriginalTotalScheduledAmount
        If QQHelper.IsPositiveDecimalString(origTotSchedAmt) = True Then
            txtScheduledAmount.Text = Format(CDec(origTotSchedAmt), "$###,###,###,###")
        Else
            txtScheduledAmount.Text = ""
        End If
    End Sub

    Private Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Save_FireSaveEvent()
    End Sub
End Class