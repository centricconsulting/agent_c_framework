Imports IFM.PrimativeExtensions

Public Class ctl_CPR_LocationList
    Inherits VRControlBase

    Public Event LocationChanged(ByVal LocIndex As Integer)

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Property ActiveLocationIndex As String
        Get
            Return hdnAccord.Value
        End Get
        Set(value As String)
            hdnAccord.Value = value
        End Set
    End Property

    Public Overrides Sub AddScriptWhenRendered()
        If Me.divNewLocation.Visible Then
            Me.VRScript.CreateAccordion(Me.divNewLocation.ClientID, Nothing, "false", True)
        Else
            If Me.divLocationList.Visible Then
                Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
            End If
        End If

    End Sub
    Private Sub AddHandlers()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim LocControl As ctl_CPR_Location = cntrl.FindControl("ctl_CPR_Location")
            AddHandler LocControl.LocationChanged, AddressOf HandleAddressChange
            AddHandler LocControl.AddLocationRequested, AddressOf AddNewLocation
            AddHandler LocControl.CopyLocationRequested, AddressOf CopyLocation
            AddHandler LocControl.DeleteLocationRequested, AddressOf DeleteLocation
            AddHandler LocControl.ClearLocationRequested, AddressOf ClearLocation
            index += 1
        Next
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Protected Sub HandleAddressChange(ByVal LocIndex As Integer)
        RaiseEvent LocationChanged(LocIndex)
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            Me.divNewLocation.Visible = False
            Me.divLocationList.Visible = False
            If Me.Quote.Locations.IsLoaded() Then
                Me.divLocationList.Visible = True
                Me.Repeater1.DataSource = Me.Quote.Locations
                Me.Repeater1.DataBind()
                Me.FindChildVrControls()

                Dim index As Int32 = 0
                For Each Loc As ctl_CPR_Location In Me.GatherChildrenOfType(Of ctl_CPR_Location)
                    Loc.MyLocationIndex = index
                    'Loc.Populate()
                    index += 1
                Next
            Else
                Me.divNewLocation.Visible = True
                Me.Repeater1.DataSource = Nothing
                Me.Repeater1.DataBind()
            End If
        End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandlers()
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divLocationList.ClientID
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Sub AddNewLocation()
        If Me.Quote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.Quote.Locations.AddNew()
            Me.ParentVrControl.Populate()
            Me.Save_FireSaveEvent(False)
            hdnAccord.Value = (Quote.Locations.Count - 1).ToString()
        End If
    End Sub

    Protected Sub DeleteLocation(LocationIndex)
        If Me.Quote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.Quote.Locations.RemoveAt(LocationIndex)
            Me.ParentVrControl.Populate()
            Me.Save_FireSaveEvent(False)
            hdnAccord.Value = (Quote.Locations.Count - 1).ToString()
        End If
    End Sub

    Protected Sub CopyLocation(LocationIndex)
        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(LocationIndex) Then
            Dim newloc As QuickQuote.CommonObjects.QuickQuoteLocation = Quote.Locations(LocationIndex)
            Quote.Locations.Add(newloc)
            Save_FireSaveEvent()
            Populate_FirePopulateEvent()
            hdnAccord.Value = (Quote.Locations.Count - 1).ToString()
        End If
    End Sub

    Protected Sub ClearLocation(LocationIndex)
        ' Clear the Wind/Hail, Property In the Open, and Building controls
    End Sub

    Private Sub btnAddLocation_Click(sender As Object, e As EventArgs) Handles btnAddLocation.Click
        AddNewLocation()
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnRate.Click
        Me.Save_FireSaveEvent()
        If sender.Equals(btnRate) Then
            If Me.ValidationSummmary.HasErrors = False Then
                Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
            End If
        End If
    End Sub

End Class