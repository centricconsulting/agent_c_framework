Public Class ctl_Locations_displayAddress
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lbSaveWorkplaces.ClientID)
        'Me.VRScript.StopEventPropagation(Me.lbAddNewWorkplace.ClientID)

        ' Create the Workplaces main accordion
        Me.VRScript.CreateAccordion(divWorkplaces.ClientID, hdnAccordWorkplace, "0")
        ' Create the workplace item accordion
        Me.VRScript.CreateAccordion(divWorkplacesList.ClientID, hdnAccordWorkplaceList, "0")

    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        ' Update the number of workplaces in the Workplaces header
        lblWorkplacesAccordHeader.Text = "Locations"
        If Quote.Locations IsNot Nothing Then
            lblWorkplacesAccordHeader.Text = "Locations (" & Quote.Locations.Count & ")"
        End If

        ' *** WORKPLACES
        rptWorkplaces.DataSource = Nothing
        rptWorkplaces.DataBind()

        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                rptWorkplaces.DataSource = Quote.Locations
                rptWorkplaces.DataBind()

                Me.FindChildVrControls()
                Dim LocIndex As Int32 = 0
                For Each cnt In Me.GatherChildrenOfType(Of ctl_Location_displayAddress)
                    cnt.WorkplaceIndex = LocIndex
                    cnt.Populate()
                    LocIndex += 1
                Next
            End If
        End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AttachControlEvents()
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
        End If
        Me.SaveChildControls()
        Return True
    End Function

    Protected Sub AttachControlEvents()
        For Each cntrl As RepeaterItem In Me.rptWorkplaces.Items
            Dim WPControl As ctl_Location_displayAddress = cntrl.FindControl("ctl_Location_displayAddress")
            AddHandler WPControl.AddWorkplaceRequested, AddressOf AddNewWorkplace
            AddHandler WPControl.ClearWorkplaceRequested, AddressOf ClearWorkplace
            AddHandler WPControl.DeleteWorkplaceRequested, AddressOf DeleteWorkplace
        Next
    End Sub

    Private Sub AddNewWorkplace()
        ' Note that workplaces are added as locations
        If Quote IsNot Nothing Then
            If Quote.Locations Is Nothing Then Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
            If Quote.Locations.Count <= 0 Then Quote.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation)

            Quote.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation)

            Me.Populate()
            Save_FireSaveEvent(False)
            Me.Populate()

            Me.hdnAccordWorkplaceList.Value = Quote.Locations.Count - 1.ToString
        End If
        Exit Sub
    End Sub

    Private Sub DeleteWorkplace(ByVal WorkplaceIndex As String)
        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count >= WorkplaceIndex + 1 Then
            Quote.Locations.RemoveAt(WorkplaceIndex)
            Populate()
            Save_FireSaveEvent(False)
            Me.Populate()
        End If
    End Sub

    Private Sub ClearWorkplace(ByVal WorkplaceIndex As String)
        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count >= WorkplaceIndex + 1 Then
            Quote.Locations(WorkplaceIndex).Address = New QuickQuote.CommonObjects.QuickQuoteAddress
            Populate()
            Save_FireSaveEvent(False)
            Populate()
        End If
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lbSaveWorkplaces.Click
        Save_FireSaveEvent()
    End Sub

    'Private Sub lbAddNewWorkplace_Click(sender As Object, e As EventArgs) Handles lbAddNewWorkplace.Click
    '    AddNewWorkplace()
    'End Sub
End Class