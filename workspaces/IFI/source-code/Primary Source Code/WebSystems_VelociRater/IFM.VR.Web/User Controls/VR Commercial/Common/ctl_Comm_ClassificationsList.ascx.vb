Imports IFM.PrimativeExtensions
Public Class ctl_Comm_ClassificationsList
    Inherits VRControlBase

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Public Property BuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
        End Set
    End Property

    Private ReadOnly Property MyBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.GetItemAtIndex(Me.BuildingIndex)
            End If
            Return Nothing
        End Get

    End Property

    Public Event BuildingClassificationChanged(ByVal LocIndex As Integer, ByVal BldgIndex As Integer, ByVal ClassificationIndex As Integer, ByVal NewClassCode As String)

    Public Overrides Sub AddScriptAlways()
    End Sub

    Protected Sub AttachClassificationControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim ClassControl As ctl_Comm_Classifications = cntrl.FindControl("ctl_BuildingClassificationItem")
            AddHandler ClassControl.BuildingClassificationDeleteRequested, AddressOf classificationControlDeleteClassificationRequested
            AddHandler ClassControl.BuildingClassificationClearRequested, AddressOf classificationControlClearClassificationRequested
            AddHandler ClassControl.BuildingPrimaryClassificationChanged, AddressOf ClassificationControlPrimaryClassificationChanged
            AddHandler ClassControl.BuildingClassificationChanged, AddressOf ClassificationControlClassificationChanged
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

    Private Sub ClassificationControlClassificationChanged(ByVal LocNdx As Integer, ByVal BldNdx As Integer, ByVal ClsNdx As Integer, ByVal NewClassCode As String)
        RaiseEvent BuildingClassificationChanged(LocNdx, BldNdx, ClsNdx, NewClassCode)
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            If Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) Then
                Dim loc As QuickQuote.CommonObjects.QuickQuoteLocation = Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex)
                If loc.Buildings.HasItemAtIndex(Me.BuildingIndex) Then
                    Repeater1.DataSource = loc.Buildings(BuildingIndex).BuildingClassifications
                    Repeater1.DataBind()
                End If

                Me.FindChildVrControls()
                Dim ClassificationIndex As Int32 = 0
                For Each cnt In Me.GatherChildrenOfType(Of ctl_Comm_Classifications)
                    cnt.LocationIndex = Me.LocationIndex
                    cnt.BuildingIndex = BuildingIndex
                    cnt.ClassificationIndex = ClassificationIndex
                    cnt.Populate()
                    ClassificationIndex += 1
                Next
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divClassificationsList.ClientID
        Me.ListAccordionDivId = Me.divList.ClientID
        AttachClassificationControlEvents()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Added 10/18/2021 for BOP Endorsements task 61660 MLW
        Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
        If Not endorsementsPreexistHelper.IsPreexistingLocation(LocationIndex) Then
            MyBase.ValidateControl(valArgs)

            Me.ValidationHelper.GroupName = "Location #" & LocationIndex + 1.ToString & ", Building #" & BuildingIndex + 1.ToString & " Classifications"

            If Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications Is Nothing OrElse Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications.Count = 0 Then
                Me.ValidationHelper.AddError("Building must have at least one Classification assigned.", Nothing)
            Else
                Dim PrimaryClassSet As Boolean = False
                For Each c As QuickQuote.CommonObjects.QuickQuoteClassification In Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications
                    If c.PredominantOccupancy Then
                        PrimaryClassSet = True
                        Exit For
                    End If
                Next
                If Not PrimaryClassSet Then
                    Me.ValidationHelper.AddError("The 'Primary Classification' checkbox must be checked on one classification", Nothing)
                End If
            End If

            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Private Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkBtnAdd.Click
        Dim bc As New QuickQuote.CommonObjects.QuickQuoteClassification With {.ClassificationTypeId = "0"}

        If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) AndAlso Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications Is Nothing Then
            Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications = New List(Of QuickQuote.CommonObjects.QuickQuoteClassification)
        End If
        ' First classification should default to primary classification MGB 5-10-17
        If Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications.Count = 0 Then bc.PredominantOccupancy = True
        Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications.Add(bc)
        Save_FireSaveEvent(False)
        Me.Populate()
        Me.hdnAccordList.Value = (Me.Quote.Locations(LocationIndex).Buildings(BuildingIndex).BuildingClassifications.Count - 1).ToString
    End Sub

    Private Sub classificationControlDeleteClassificationRequested(ByVal LocIndex As Integer, ByVal BldgIndex As Integer, ByVal ClassificationIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                If Quote.Locations.HasItemAtIndex(LocIndex) Then
                    If Quote.Locations(LocIndex).Buildings IsNot Nothing AndAlso Quote.Locations(LocIndex).Buildings.HasItemAtIndex(BldgIndex) Then
                        If Quote.Locations(LocIndex).Buildings(BldgIndex).BuildingClassifications IsNot Nothing AndAlso Quote.Locations(LocIndex).Buildings(BldgIndex).BuildingClassifications.HasItemAtIndex(ClassificationIndex) Then
                            Quote.Locations(LocIndex).Buildings(BldgIndex).BuildingClassifications.RemoveAt(ClassificationIndex)
                            Save_FireSaveEvent(False)
                            Populate() '_FirePopulateEvent()
                            Me.hdnAccordList.Value = (Me.Quote.Locations(LocIndex).Buildings(BldgIndex).BuildingClassifications.Count - 1).ToString
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub classificationControlClearClassificationRequested(ByVal LocIndex As Integer, ByVal BldgIndex As Integer, ByVal ClassificationIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                If Quote.Locations.HasItemAtIndex(LocIndex) Then
                    If Quote.Locations(LocIndex).Buildings IsNot Nothing AndAlso Quote.Locations(LocIndex).Buildings.HasItemAtIndex(BldgIndex) Then
                        If Quote.Locations(LocIndex).Buildings(BldgIndex).BuildingClassifications IsNot Nothing AndAlso Quote.Locations(LocIndex).Buildings(BldgIndex).BuildingClassifications.HasItemAtIndex(ClassificationIndex) Then
                            Quote.Locations(LocIndex).Buildings(BldgIndex).BuildingClassifications.RemoveAt(ClassificationIndex)
                            Save_FireSaveEvent(False)
                            Populate()
                            Quote.Locations(LocIndex).Buildings(BldgIndex).BuildingClassifications.Insert(BldgIndex, New QuickQuote.CommonObjects.QuickQuoteClassification With {.ClassificationTypeId = "0"})
                            Save_FireSaveEvent(False)
                            Populate()
                            Me.hdnAccordList.Value = ClassificationIndex
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub ClassificationControlPrimaryClassificationChanged(ByVal LocIndex As Integer, ByVal BldgIndex As Integer, ByVal ClassificationIndex As Integer)
        Dim ndx As Integer = -1
        For Each ri As RepeaterItem In Repeater1.Items
            ndx += 1
            Dim cls As ctl_Comm_Classifications = ri.FindControl("ctl_BuildingClassificationItem")
            If cls IsNot Nothing Then
                If ndx <> ClassificationIndex Then
                    Dim chk As CheckBox = cls.FindControl("chkPrimaryClassification")
                    If chk IsNot Nothing Then
                        chk.Checked = False
                    End If
                End If
            End If
        Next
    End Sub

End Class