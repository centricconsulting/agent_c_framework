Imports IFM.PrimativeExtensions

Public Class ctl_CPR_BuildingList
    Inherits VRControlBase

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Public Sub HandleBlanketDeductibleChange()
        Dim mybuildings As List(Of ctl_CPR_Building) = Me.GatherChildrenOfType(Of ctl_CPR_Building)()
        For Each B As ctl_CPR_Building In mybuildings
            B.HandleBlanketDeductibleChange()
        Next
    End Sub

    Public Sub HandleAgreedAmountChange(ByVal newvalue As Boolean)
        Dim mybuildings As List(Of ctl_CPR_Building) = Me.GatherChildrenOfType(Of ctl_CPR_Building)()
        For Each B As ctl_CPR_Building In mybuildings
            B.HandleAgreedAmountChange(newvalue)
        Next
    End Sub

    Public Event BuildingZeroDeductibleChanged()

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Session($"BuildingIsNew_{Me.QuoteId}") IsNot Nothing AndAlso Session($"BuildingIsNew_{Me.QuoteId}") = "True" Then
            Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Nothing, Quote.Locations(CInt(Session($"BuildingLocationIndex_{Me.QuoteId}"))).Buildings.Count - 1, "0")
            Session($"BuildingIsNew_{Me.QuoteId}") = Nothing
            Session($"BuildingLocationIndex_{Me.QuoteId}") = Nothing
        ElseIf Session("CCLookupPerformed_" & Session.SessionID.ToString) IsNot Nothing Then
            ' The value of the session variable will be the building index to focus on
            Dim ndx As Integer = 0
            If IsNumeric(Session("CCLookupPerformed_" & Session.SessionID.ToString)) Then ndx = CInt(Session("CCLookupPerformed_" & Session.SessionID.ToString))
            Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Nothing, ndx, "0")
            Session("CCLookupPerformed_" & Session.SessionID.ToString) = Nothing
        Else
            Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            If Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) Then
                Dim loc As QuickQuote.CommonObjects.QuickQuoteLocation = Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex)

                If loc.IsNotNull Then
                    If loc.Buildings.IsNotNull AndAlso loc.Buildings.Count > 0 Then
                        Me.Repeater1.DataSource = loc.Buildings
                        Me.Repeater1.DataBind()
                    End If
                End If

                Me.FindChildVrControls()
                Dim bIndex As Int32 = 0
                For Each cnt In Me.GatherChildrenOfType(Of ctl_CPR_Building)
                    cnt.LocationIndex = Me.LocationIndex
                    cnt.BuildingIndex = bIndex
                    cnt.Populate()
                    bIndex += 1
                Next
            End If
        End If
    End Sub

    Private Sub ctl_CPR_BuildingList_Init(sender As Object, e As EventArgs) Handles Me.Init
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ListAccordionDivId = Me.divBuildingList.ClientID
        AttachBuildingControlEvents()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub AttachBuildingControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim BldgControl As ctl_CPR_Building = cntrl.FindControl("ctl_CPR_Building")
            AddHandler BldgControl.NewBuildingRequested, AddressOf buildingControlNewBuildingRequested
            AddHandler BldgControl.DeleteBuildingRequested, AddressOf buildingControlDeleteBuildingRequested
            AddHandler BldgControl.ClearBuildingRequested, AddressOf buildingControlClearBuildingRequested
            AddHandler BldgControl.BuildingZeroDeductibleChanged, AddressOf HandleBuildingZeroDeductibleChange
            index += 1
        Next
    End Sub

    Public Sub HandleBuildingZeroDeductibleChange()
        RaiseEvent BuildingZeroDeductibleChanged()
    End Sub

    Public Sub buildingControlNewBuildingRequested(ByVal LocIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(LocIndex) Then
                If Quote.Locations(LocIndex).Buildings Is Nothing Then
                    Quote.Locations(LocIndex).Buildings = New List(Of QuickQuote.CommonObjects.QuickQuoteBuilding)
                End If
                ' Add the new building
                Dim newbld As New QuickQuote.CommonObjects.QuickQuoteBuilding()
                Quote.Locations(LocIndex).Buildings.Add(newbld)
                Save_FireSaveEvent(False)
                Populate()
                Me.hdnAccord.Value = (Me.Quote.Locations(LocIndex).Buildings.Count - 1).ToString
            End If
        End If
    End Sub

    Private Sub buildingControlDeleteBuildingRequested(ByVal LocIndex As Integer, ByVal BldgIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(LocIndex) Then
                If Quote.Locations(LocIndex).Buildings IsNot Nothing AndAlso Quote.Locations(LocIndex).Buildings.HasItemAtIndex(BldgIndex) Then
                    Quote.Locations(LocIndex).Buildings.RemoveAt(BldgIndex)
                    Save_FireSaveEvent(False)
                    Populate_FirePopulateEvent()
                    Me.hdnAccord.Value = (Me.Quote.Locations(LocIndex).Buildings.Count - 1).ToString
                End If
            End If
        End If
    End Sub

    Private Sub buildingControlClearBuildingRequested(ByVal LocIndex As Integer, ByVal BldgIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(LocIndex) Then
                If Quote.Locations(LocIndex).Buildings.HasItemAtIndex(BldgIndex) Then
                    ' Remove the building with the data in it then add a new building
                    Quote.Locations(LocIndex).Buildings.RemoveAt(BldgIndex)
                    Quote.Locations(LocIndex).Buildings.Insert(BldgIndex, New QuickQuote.CommonObjects.QuickQuoteBuilding())
                    Populate()
                    Me.hdnAccord.Value = (Me.Quote.Locations(LocIndex).Buildings(BldgIndex)).ToString
                End If
            End If
        End If
    End Sub


    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctl As ctl_CPR_Building = ri.FindControl("ctl_CPR_Building")
            If ctl IsNot Nothing Then
                ctl.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
            End If
        Next
    End Sub

    'Added 10/20/2022 for task 77527 MLW
    Public Sub PopulateInflationGuard()
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctl As ctl_CPR_Building = ri.FindControl("ctl_CPR_Building")
            If ctl IsNot Nothing Then
                ctl.ToggleInflationGuardOptions()
                ctl.PopulateInflationGuard()
            End If
        Next
    End Sub

    Public Sub PopulateCPRBuildingInformation()
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctl As ctl_CPR_Building = ri.FindControl("ctl_CPR_Building")
            If ctl IsNot Nothing Then
                ctl.PopulateNewCoFields()
                ctl.PopulateBuildingCoverages()
            End If
        Next
    End Sub

    Public Sub RemoveFunctionalReplacementCost()
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctl As ctl_CPR_Building = ri.FindControl("ctl_CPR_Building")
            If ctl IsNot Nothing Then
                ctl.RemoveFunctionalReplacementCost()
            End If
        Next
    End Sub

End Class