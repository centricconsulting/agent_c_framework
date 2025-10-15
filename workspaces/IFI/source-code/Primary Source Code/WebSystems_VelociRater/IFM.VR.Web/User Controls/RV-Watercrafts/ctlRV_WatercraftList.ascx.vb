Public Class ctlRV_WatercraftList
    Inherits VRControlBase

    Public Event CommonSaveRVWater()
    Public Event CommonRateRVWater()
    Public Event ToggleRVWaterHdr()
    Public Event ActivePanel(panel As String)

    Public Property ActiveRVWaterPane As String
        Get
            Return hiddenActiveRVWatercraft.Value
        End Get
        Set(value As String)
            hiddenActiveRVWatercraft.Value = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Public Property MyLocationIndex As Int32
        Get
            If ViewState("vs_locationIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_locationIndex"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ListAccordionDivId = dvRVWatercraftList.ClientID
            LoadStaticData()
            Populate()
        End If

        AttachCoverageControlEvents()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.AddVariableLine("var dvRVWatercraftList = '" + hiddenActiveRVWatercraft.ClientID + "';")
        VRScript.AddVariableLine("function ShowRVWatercraft(index){SetActiveAccordionIndex('" + ListAccordionDivId + "',index);}")
        VRScript.CreateAccordion(dvRVWatercraftList.ClientID, hiddenActiveRVWatercraft, "0")
    End Sub

    Protected Sub AttachCoverageControlEvents()
        For Each cntrl As RepeaterItem In Me.rvWaterRepeater.Items
            Dim rvWaterControl As ctlRV_Watercraft = cntrl.FindControl("ctlRV_Watercraft")
            AddHandler rvWaterControl.CommonRefreshRVWater, AddressOf RefreshRVWatercraft
            AddHandler rvWaterControl.CommonRaiseRVWSave, AddressOf SaveRVWatercraft
            AddHandler rvWaterControl.CommonRaiseRVWRate, AddressOf RateRVWatercraft
            AddHandler rvWaterControl.CommonSetActivePanel, AddressOf SetActivePanel
            AddHandler rvWaterControl.NewVehicleRequested, AddressOf AddNewRVWater
            AddHandler rvWaterControl.NewBoatMotorRequested, AddressOf AddNewBoatMotor
        Next
    End Sub

    Private Sub SaveRVWatercraft()
        RaiseEvent CommonSaveRVWater()
    End Sub

    Private Sub RateRVWatercraft()
        RaiseEvent CommonRateRVWater()
    End Sub

    Public Sub RefreshRVWatercraft()
        'rvWaterRepeater.DataSource = Nothing
        'rvWaterRepeater.DataSource = MyLocation.RvWatercrafts
        'rvWaterRepeater.DataBind()
        Populate()
        RaiseEvent ToggleRVWaterHdr()
    End Sub

    Private Sub SetActivePanel(activePanel As String)
        RaiseEvent activePanel(activePanel)
        'ActiveRVWaterIndex = activePanel
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Dim allStatesRVWatercraftList = New List(Of allStatesRVWatercraftItem)
        If Me.Quote?.Locations IsNot Nothing Then
            For Each Loc As QuickQuote.CommonObjects.QuickQuoteLocation In Me.Quote.Locations
                If Loc IsNot Nothing AndAlso Loc.RvWatercrafts IsNot Nothing Then
                    For Each craft In Loc.RvWatercrafts
                        Dim allStatesCraft As allStatesRVWatercraftItem = New allStatesRVWatercraftItem()
                        allStatesCraft.Craft = craft
                        allStatesCraft.locationNumber = Me.Quote.Locations.IndexOf(Loc)
                        allStatesCraft.indexByLocation = Loc.RvWatercrafts.IndexOf(craft)
                        allStatesRVWatercraftList.Add(allStatesCraft)
                    Next
                End If
            Next
        End If


        If allStatesRVWatercraftList IsNot Nothing AndAlso allStatesRVWatercraftList.Count > 0 Then
            rvWaterRepeater.DataSource = allStatesRVWatercraftList
            rvWaterRepeater.DataBind()
            FindChildVrControls()

            For Each c In Me.GatherChildrenOfType(Of ctlRV_Watercraft)
                c.Populate()
            Next
        End If

        'Dim allStatesRVWatercraftList = New List(Of QuickQuote.CommonObjects.QuickQuoteRvWatercraft)
        'For Each Loc As QuickQuote.CommonObjects.QuickQuoteLocation In Me.Quote.Locations
        '    For Each craft In Loc.RvWatercrafts
        '        allStatesRVWatercraftList.Add(craft)
        '    Next
        'Next

        'If allStatesRVWatercraftList IsNot Nothing Then
        '    rvWaterRepeater.DataSource = allStatesRVWatercraftList
        '    rvWaterRepeater.DataBind()
        '    FindChildVrControls()

        '    For Each c In Me.GatherChildrenOfType(Of ctlRV_Watercraft)
        '        c.Populate()
        '    Next
        'End If

        'If MyLocation IsNot Nothing Then
        '    rvWaterRepeater.DataSource = MyLocation.RvWatercrafts
        '    rvWaterRepeater.DataBind()
        '    FindChildVrControls()

        '    For Each c In Me.GatherChildrenOfType(Of ctlRV_Watercraft)
        '        c.Populate()
        '    Next
        'End If
    End Sub

    Class allStatesRVWatercraftItem
        Public Craft As QuickQuote.CommonObjects.QuickQuoteRvWatercraft
        Public locationNumber As Int32
        Public indexByLocation As Int32
    End Class

    Public Overrides Function Save() As Boolean
        SaveChildControls()
        Return True
    End Function

    Public Sub AddNewRVWater()
        If Me.Quote IsNot Nothing AndAlso MyLocation IsNot Nothing Then
            If MyLocation.RvWatercrafts Is Nothing Then
                MyLocation.RvWatercrafts = New List(Of QuickQuote.CommonObjects.QuickQuoteRvWatercraft)()
            End If

            MyLocation.RvWatercrafts.Add(New QuickQuote.CommonObjects.QuickQuoteRvWatercraft())
            Save_FireSaveEvent(False)
            Populate()
            hiddenActiveRVWatercraft.Value = (MyLocation.RvWatercrafts.Count() - 1).ToString()
        End If
    End Sub

    Public Sub AddNewBoatMotor()
        If Me.Quote IsNot Nothing AndAlso MyLocation IsNot Nothing Then
            If MyLocation.RvWatercrafts Is Nothing Then
                MyLocation.RvWatercrafts = New List(Of QuickQuote.CommonObjects.QuickQuoteRvWatercraft)()
            End If
            Dim newBoatMotor = New QuickQuote.CommonObjects.QuickQuoteRvWatercraft()
            newBoatMotor.RvWatercraftTypeId = 3 'Boat Motor

            MyLocation.RvWatercrafts.Add(newBoatMotor)
            Save_FireSaveEvent(False)
            Populate()
        End If
    End Sub

    Private Sub rvWaterRepeater_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rvWaterRepeater.ItemDataBound
        If e?.Item?.DataItem IsNot Nothing Then
            Dim rvWatercraftControl As ctlRV_Watercraft = e.Item.FindControl("ctlRV_Watercraft")
            Dim craft As allStatesRVWatercraftItem = e.Item.DataItem
            rvWatercraftControl.RVWatercraftLocationNumber = craft.locationNumber
            rvWatercraftControl.RVWatercraftNumber = craft.indexByLocation
            rvWatercraftControl.Populate()
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidateChildControls(valArgs)
    End Sub
End Class