Imports IFM.PrimativeExtensions
Public Class ctl_WCP_Location
    Inherits VRControlBase

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
            Me.ctlProperty_Addresss.MyLocationIndex = value
        End Set
    End Property

    Private ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote.IsNotNull Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return Me.LocationIndex
        End Get
    End Property

    Public ReadOnly Property IsFirstLocationForStatePart As Boolean
        Get
            Dim _isLocationFirstForStatePart = False
            If Me.MyLocation IsNot Nothing Then
                For Each statePart_State In Me.Quote.QuoteStates
                    Dim loc = IFM.VR.Common.Helpers.MultiState.Locations.GetFirstLocationForStateQuote(Me.Quote, statePart_State)
                    If Me.MyLocation.Equals(loc) Then
                        _isLocationFirstForStatePart = True
                    End If
                Next
            End If
            Return _isLocationFirstForStatePart
        End Get
    End Property

    Public ReadOnly Property SaveButtonId As String
        Get
            Return lnkSave.ClientID
        End Get
    End Property

    Public ReadOnly Property ClearButtonId As String
        Get
            Return lnkClear.ClientID
        End Get
    End Property

    Public ReadOnly Property AddButtonId As String
        Get
            Return lnkNew.ClientID
        End Get
    End Property

    ''' If this is NOT governing state location 0
    ''' AND the address matches exactly except for the state
    ''' then the 'No Owned Location in this state' checkbox applies
    ''' Added for KY expansion MGB 4/29/19
    Private ReadOnly Property isWCPDummyLocation() As Boolean
        Get
            If Quote IsNot Nothing AndAlso Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation Then
                If MyLocation IsNot Nothing AndAlso MyLocation.Address IsNot Nothing Then
                    If GoverningStateQuote() IsNot Nothing AndAlso GoverningStateQuote.Locations IsNot Nothing AndAlso GoverningStateQuote.Locations.Count > 0 Then
                        If MyLocation.Equals(GoverningStateQuote.Locations(0)) Then
                            ' This is governing state quote location 0
                            Return False
                        Else
                            ' Not governing state location 0, check it
                            Dim myAddr As QuickQuote.CommonObjects.QuickQuoteAddress = MyLocation.Address
                            Dim govAddr As QuickQuote.CommonObjects.QuickQuoteAddress = GoverningStateQuote.Locations(0).Address
                            ' Don't check if the governing state address is empty
                            If Not (govAddr.HouseNum.IsNullEmptyorWhitespace _
                                AndAlso govAddr.StreetName.IsNullEmptyorWhitespace _
                                AndAlso govAddr.City.IsNullEmptyorWhitespace _
                                AndAlso govAddr.Zip.IsNullEmptyorWhitespace) Then
                                ' If the address matches governing state location 0 except for state the No Owned Location applies
                                If myAddr.HouseNum = govAddr.HouseNum AndAlso
                                myAddr.StreetName = govAddr.StreetName AndAlso
                                myAddr.City = govAddr.City AndAlso
                                myAddr.Zip = govAddr.Zip AndAlso
                                myAddr.County = govAddr.County AndAlso
                                myAddr.ApartmentNumber = govAddr.ApartmentNumber AndAlso
                                myAddr.State <> govAddr.State Then
                                    Return True
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            Return False
        End Get
    End Property

    Public Event NewLocationRequested()
    Public Event DeleteLocationRequested(ByRef LocIndex As Integer)
    Public Event ClearLocationRequested(ByRef LocIndex As Integer)
    Public Event AddLocationBuildingRequested(ByRef LocIndex As Integer)

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateConfirmDialog(Me.lnkClear.ClientID, "Clear?")
        Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Delete Location?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkNew.ClientID)
        Me.VRScript.AddVariableLine(String.Format("var locationHeader_{0} = ""{1}"";", LocationIndex, Me.lblAccordHeader.ClientID)) 'used to set the address text in this header - used by property_address control
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        lnkDelete.Visible = Not Me.IsFirstLocationForStatePart

        UpdateAccordHeader()
        Me.ctlProperty_Addresss.MyLocationIndex = LocationIndex

        Me.PopulateChildControls()
    End Sub

    Private Sub HideHeaderButtons()
        lnkNew.Attributes.Add("style", "visibility:hidden")
        lnkDelete.Attributes.Add("style", "visibility:hidden")
        lnkClear.Attributes.Add("style", "visibility:hidden")
        lnkSave.Attributes.Add("style", "visibility:hidden")

        Exit Sub
    End Sub

    Private Sub ShowHeaderButtons()
        lnkNew.Attributes.Add("style", "visibility:visible")
        lnkClear.Attributes.Add("style", "visibility:visible")
        lnkSave.Attributes.Add("style", "visibility:visible")

        Exit Sub
    End Sub

    Private Sub UpdateAccordHeader()
        Dim title As String = ""
        'Dim titlelen As Integer = 31 '35 does not fit.  Changed to 31 CAH 9/22/2017 and tested
        Dim titleLen As Integer = 38
        Dim AddressOk As Boolean = False

        Dim stateabbrev As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.StateAbbreviationForQuickQuoteState(MyLocation.Address.QuickQuoteState)

        ' If this is a fake location for the WCP 'No Owned locations' checkbox, show a special header
        If isWCPDummyLocation() Then
            lblAccordHeader.Text = "(" & stateabbrev & ")" & " No owned locations in this state"
            Exit Sub
        End If

        If MyLocation IsNot Nothing AndAlso MyLocation.Address IsNot Nothing Then
            If LocationIndex <> 0 Then
                titleLen = 30
            End If

            If MyLocation.Address.HouseNum <> "" AndAlso MyLocation.Address.StreetName <> "" AndAlso MyLocation.Address.City <> "" AndAlso MyLocation.Address.Zip <> "" Then
                AddressOk = True
            End If

            If AddressOk Then
                title = "Location #" & (Me.LocationIndex + 1).ToString & " - "
                If MyLocation.Address.DisplayAddress.Length > titleLen Then
                    title += MyLocation.Address.DisplayAddress.Substring(0, titleLen).ToUpper & "..."
                Else
                    title += MyLocation.Address.DisplayAddress.ToUpper
                End If
            Else
                title = "Location #" & (Me.LocationIndex + 1).ToString
            End If

            lblAccordHeader.Text = title
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        UpdateAccordHeader()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Private Sub lnkNew_Click(sender As Object, e As EventArgs) Handles lnkNew.Click
        RaiseEvent NewLocationRequested()
    End Sub

    Private Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        RaiseEvent ClearLocationRequested(LocationIndex)
    End Sub

    Private Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        RaiseEvent DeleteLocationRequested(LocationIndex)
    End Sub

    Private Sub ctl_WCP_Location_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Add handlers
        AddHandler ctlProperty_Addresss.HideHeaderButtons, AddressOf HideHeaderButtons
        AddHandler ctlProperty_Addresss.ShowHeaderButtons, AddressOf ShowHeaderButtons

        Exit Sub
    End Sub
End Class