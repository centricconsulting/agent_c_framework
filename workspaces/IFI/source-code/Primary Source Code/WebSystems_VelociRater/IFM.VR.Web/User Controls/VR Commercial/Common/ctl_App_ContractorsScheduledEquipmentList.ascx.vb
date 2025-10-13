Imports IFM.PrimativeExtensions

Public Class ctl_App_ContractorsScheduledEquipmentList
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()
    End Sub

    Protected Sub AttachControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim ContEqControl As ctl_App_ContractorsScheduledEquipmentItem = cntrl.FindControl("ctlScheduledItem")
            AddHandler ContEqControl.ContractorsItemDeleteRequested, AddressOf DeleteItemRequested
            AddHandler ContEqControl.ContractorsItemChanged, AddressOf ItemChanged
            index += 1
        Next
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "0")
        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccordList, "0")
        Me.VRScript.StopEventPropagation(Me.lnkBtnAdd.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            If Me.Quote.ContractorsEquipmentScheduledItems IsNot Nothing AndAlso Quote.ContractorsEquipmentScheduledItems.Count > 0 Then
                Repeater1.DataSource = Quote.ContractorsEquipmentScheduledItems
                Repeater1.DataBind()
            End If

            Me.FindChildVrControls()
            Dim ndx As Int32 = 0
            For Each cnt In Me.GatherChildrenOfType(Of ctl_App_ContractorsScheduledEquipmentItem)
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

        If Me.Quote IsNot Nothing Then
            If Quote.ContractorsEquipmentScheduledItems Is Nothing Then Quote.ContractorsEquipmentScheduledItems = New List(Of QuickQuote.CommonObjects.QuickQuoteContractorsEquipmentScheduledItem)
            Quote.ContractorsEquipmentScheduledItems.Add(ctitem)
            Save_FireSaveEvent(False)
            Me.Populate()
            Me.hdnAccordList.Value = Quote.ContractorsEquipmentScheduledItems.Count - 1.ToString
        End If
    End Sub

    Private Sub DeleteItemRequested(ByVal ItemIndex As Integer)
        If Quote IsNot Nothing AndAlso (Quote.ContractorsEquipmentScheduledItems IsNot Nothing AndAlso Quote.ContractorsEquipmentScheduledItems.Count > 0) Then
            If Quote.ContractorsEquipmentScheduledItems.HasItemAtIndex(ItemIndex) Then
                Quote.ContractorsEquipmentScheduledItems.RemoveAt(ItemIndex)
                Save_FireSaveEvent(False)
                Populate()
                Me.hdnAccordList.Value = Me.Quote.ContractorsEquipmentScheduledItems.Count - 1.ToString
            End If
        End If
    End Sub

    Private Sub ItemChanged(ByVal ItemIndex As Integer)
        Dim a As String = "Do something here"
    End Sub

End Class