Imports IFM.PrimativeExtensions

Public Class ctl_BOP_BuildingList
    Inherits VRControlBase

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Public Sub HandleAddressChange()
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctl As ctl_BOP_Building = ri.FindControl("ctl_BOP_Building")
            If ctl IsNot Nothing Then ctl.HandleAddressChange()
        Next
        Exit Sub
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Session($"BuildingIsNew_{Me.QuoteId}") IsNot Nothing AndAlso Session($"BuildingIsNew_{Me.QuoteId}") = "True" Then
            Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Nothing, Quote.Locations(CInt(Session($"BuildingLocationIndex_{Me.QuoteId}"))).Buildings.Count - 1, "0")
            Session($"BuildingIsNew_{Me.QuoteId}") = Nothing
            Session($"BuildingLocationIndex_{Me.QuoteId}") = Nothing
        Else
            Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
        End If
    End Sub

    Public Sub EffectiveDateChanging(ByVal NewDate As String, ByVal OldDate As String)
        ' Put any code here to handle when the effective date changes
        For Each ri As RepeaterItem In Repeater1.Items
            Dim bld As ctl_BOP_Building = ri.FindControl("ctl_BOP_Building")
            If bld IsNot Nothing Then bld.EffectiveDateChanging(NewDate, OldDate)
        Next
    End Sub

    Public Sub PopulateBOPBuildingInformation()
        For Each ri As RepeaterItem In Repeater1.Items
            Dim bld As ctl_BOP_Building = ri.FindControl("ctl_BOP_Building")
            If bld IsNot Nothing Then bld.PopulateBOPBuildingInformation()
        Next
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Sub LoadProtectionClasses()
        For Each i As RepeaterItem In Repeater1.Items
            Dim ctl As ctl_BOP_Building = i.FindControl("ctl_BOP_Building")
            If ctl IsNot Nothing Then
                ctl.UpdateProtectionClasses()
            End If
        Next
    End Sub

    Public Sub PropertyAddressChanged()
        For Each i As RepeaterItem In Repeater1.Items
            Dim ctl As ctl_BOP_Building = i.FindControl("ctl_BOP_Building")
            If ctl IsNot Nothing Then
                ctl.UpdateProtectionClasses()
                'ctl.UpdateMineSubsidence()
            End If
        Next
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
                For Each cnt In Me.GatherChildrenOfType(Of ctl_BOP_Building)
                    cnt.LocationIndex = Me.LocationIndex
                    cnt.BuildingIndex = bIndex
                    cnt.Populate()
                    bIndex += 1
                Next
            End If
        End If
    End Sub

    Private Sub ctl_BOP_BuildingList_Init(sender As Object, e As EventArgs) Handles Me.Init
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
            Dim BldgControl As ctl_BOP_Building = cntrl.FindControl("ctl_BOP_Building")
            AddHandler BldgControl.NewBuildingRequested, AddressOf buildingControlNewBuildingRequested
            AddHandler BldgControl.DeleteBuildingRequested, AddressOf buildingControlDeleteBuildingRequested
            AddHandler BldgControl.ClearBuildingRequested, AddressOf buildingControlClearBuildingRequested
            index += 1
        Next
    End Sub

    Public Sub buildingControlNewBuildingRequested(ByVal LocIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(LocIndex) Then
                If Quote.Locations(LocIndex).Buildings Is Nothing Then
                    Quote.Locations(LocIndex).Buildings = New List(Of QuickQuote.CommonObjects.QuickQuoteBuilding)
                End If
                ' Add the new building
                Dim newbld As New QuickQuote.CommonObjects.QuickQuoteBuilding()
                ' Add a blank classification to the building
                newbld.BuildingClassifications = New List(Of QuickQuote.CommonObjects.QuickQuoteClassification)
                newbld.BuildingClassifications.AddNew()
                Quote.Locations(LocIndex).Buildings.Add(newbld)
                Save_FireSaveEvent(False)
                Populate()
                Me.hdnAccord.Value = (Me.Quote.Locations(LocIndex).Buildings.Count - 1).ToString

                ' Start Focus on building - CH 09/15/2017
                Dim lastBuilding = Me.GatherChildrenOfType(Of ctl_BOP_Building)().LastOrDefault() ' added 10-1-2015
                If lastBuilding IsNot Nothing Then
                    Dim lastBuilding_PropertyControl = lastBuilding.GatherChildrenOfType(Of ctl_BOP_Building_Information).FirstOrDefault()
                    If lastBuilding_PropertyControl IsNot Nothing Then
                        lastBuilding.OpenAllParentAccordionsOnNextLoad(lastBuilding_PropertyControl.ScrollToControlId)
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub buildingControlDeleteBuildingRequested(ByVal LocIndex As Integer, ByVal BldgIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(LocIndex) Then
                If Quote.Locations(LocIndex).Buildings IsNot Nothing AndAlso Quote.Locations(LocIndex).Buildings.HasItemAtIndex(BldgIndex) Then
                    Quote.Locations(LocIndex).Buildings.RemoveAt(BldgIndex)
                    Save_FireSaveEvent(False)
                    Populate()
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
                    'Save_FireSaveEvent(False)
                    'Populate()
                    Quote.Locations(LocIndex).Buildings.Insert(BldgIndex, New QuickQuote.CommonObjects.QuickQuoteBuilding())
                    'Save_FireSaveEvent(False)
                    Populate()
                    'Me.hdnAccord.Value = (Me.Quote.Locations(LocIndex).Buildings.Count - 1).ToString
                    Me.hdnAccord.Value = (Me.Quote.Locations(LocIndex).Buildings(BldgIndex)).ToString
                End If
            End If
        End If
    End Sub

End Class