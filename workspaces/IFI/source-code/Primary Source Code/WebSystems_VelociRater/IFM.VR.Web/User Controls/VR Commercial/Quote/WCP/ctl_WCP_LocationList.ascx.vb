Imports IFM.PrimativeExtensions

Public Class ctl_WCP_LocationList
    Inherits VRControlBase

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
        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Protected Sub AttachLocationControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim LocControl As ctl_WCP_Location = cntrl.FindControl("ctl_WCP_LocationItem")
            AddHandler LocControl.NewLocationRequested, AddressOf NewLocationRequested
            AddHandler LocControl.DeleteLocationRequested, AddressOf locationControlDeleteLocationRequested
            AddHandler LocControl.ClearLocationRequested, AddressOf locationControlClearLocationRequested
            index += 1
        Next
    End Sub


    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            Me.Repeater1.DataSource = Me.Quote.Locations
            Me.Repeater1.DataBind()

            Me.FindChildVrControls()

            Dim lIndex As Int32 = 0
            For Each cnt In Me.GatherChildrenOfType(Of ctl_WCP_Location)
                cnt.LocationIndex = lIndex
                cnt.Populate()
                lIndex += 1
            Next
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divMainList.ClientID
            Me.hdnAccord.Value = "0"
        End If

        AttachLocationControlEvents()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()

        If ActiveLocationIndex = "" Then
            ActiveLocationIndex = "false"
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub locationControlDeleteLocationRequested(ByRef LocIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                If Quote.Locations.HasItemAtIndex(LocIndex) Then Quote.Locations.RemoveAt(LocIndex)
                Populate()
                Save_FireSaveEvent(False)
                Me.hdnAccord.Value = (Me.Quote.Locations.Count - 1).ToString
            End If
        End If
    End Sub

    Private Sub locationControlClearLocationRequested(ByRef LocIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                ' Remove the location with the data in it then add a new location
                If Quote.Locations.HasItemAtIndex(LocIndex) Then Quote.Locations.RemoveAt(LocIndex)
                Quote.Locations.Insert(LocIndex, New QuickQuote.CommonObjects.QuickQuoteLocation())
                Populate()
                Me.hdnAccord.Value = LocIndex
            End If
        End If
    End Sub

    Private Sub NewLocationRequested()
        If Quote IsNot Nothing Then
            Save_FireSaveEvent(False)
            If Quote.Locations Is Nothing Then
                Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
            End If
            Quote.Locations.AddNew()
            Populate_FirePopulateEvent()
            'Populate()
            Save_FireSaveEvent(False)
            Me.hdnAccord.Value = (Me.Quote.Locations.Count - 1).ToString
        End If
    End Sub

End Class