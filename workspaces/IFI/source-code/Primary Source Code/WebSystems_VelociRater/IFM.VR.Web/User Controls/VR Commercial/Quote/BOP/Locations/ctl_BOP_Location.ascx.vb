Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Common.Helpers.BOP

Public Class ctl_BOP_Location
    Inherits VRControlBase

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
            Me.ctlProperty_Address.MyLocationIndex = value
            Me.ctl_BOP_Location_Coverages.LocationIndex = value
            Me.ctl_BOP_BuildingList.LocationIndex = value
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

    'Public Event PopupRequest(ByVal DialogMessage As String, ByVal DialogTitle As String)

    'Private Sub HandlePopupRequest(ByVal DialogMsg As String, ByVal DialogTitle As String) Handles ctl_BOP_BuildingList.PopupRequest
    '    RaiseEvent PopupRequest(DialogMsg, DialogTitle)
    'End Sub

    Public Event NewLocationRequested()
    Public Event DeleteLocationRequested(ByRef LocIndex As Integer)
    Public Event ClearLocationRequested(ByRef LocIndex As Integer)
    Public Event AddLocationBuildingRequested(ByRef LocIndex As Integer)

    Public Sub HandleAddressChange() Handles ctlProperty_Address.PropertyAddressChanged
        Me.ctl_BOP_BuildingList.HandleAddressChange()
        Exit Sub
    End Sub

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
        'Dim title As String = Nothing 'Handled in UpdateAccordHeader()
        'Dim titleLen As Integer = 40 'Handled in UpdateAccordHeader()

        If Me.MyLocation.IsNotNull Then
            If LocationIndex = 0 Then
                lnkDelete.Visible = False
            Else
                lnkDelete.Visible = True
                'titleLen = 30 'Handled in UpdateAccordHeader()
            End If
            UpdateAccordHeader()
            'If MyLocation.Address IsNot Nothing AndAlso MyLocation.Address.DisplayAddress IsNot Nothing AndAlso MyLocation.Address.DisplayAddress <> String.Empty Then
            '    title = "Location #" & (Me.LocationIndex + 1).ToString & " - "
            '    If MyLocation.Address.DisplayAddress.Length > titleLen Then
            '        title += MyLocation.Address.DisplayAddress.Substring(0, titleLen) & "..."
            '    Else
            '        title += MyLocation.Address.DisplayAddress
            '    End If
            'Else
            '    title = "Location #" & (Me.LocationIndex + 1).ToString
            'End If
            'lblAccordHeader.Text = title
        End If

        'Added 09/09/2021 for BOP Endorsements Task 63912 MLW
        If IsQuoteReadOnly() Then
            btnAddBuilding.Visible = False
        End If

        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandler Me.ctlProperty_Address.ProtectionClassLookupNeeded, AddressOf HandlePropertyAddressProtectionClassNeeded
        AddHandler Me.ctlProperty_Address.PropertyAddressChanged, AddressOf HandlePropertyAddressChange
    End Sub

    Private Sub UpdateAccordHeader()
        Dim title As String = ""
        'Dim titlelen As Integer = 31 '35 does not fit.  Changed to 31 CAH 9/22/2017 and tested
        Dim titleLen As Integer = 38


        If MyLocation IsNot Nothing AndAlso MyLocation.Address IsNot Nothing Then
            If LocationIndex <> 0 Then
                titleLen = 30
            End If

            If MyLocation.Address IsNot Nothing AndAlso MyLocation.Address.DisplayAddress IsNot Nothing AndAlso MyLocation.Address.DisplayAddress <> String.Empty Then
                title = "Location #" & (Me.LocationIndex + 1).ToString & " - "
                If MyLocation.Address.DisplayAddress.Length > titleLen Then
                    title += MyLocation.Address.DisplayAddress.Substring(0, titleLen) & "..."
                Else
                    title += MyLocation.Address.DisplayAddress
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

        Select Case Session("BOPCheckLimitEventTrigger")
            Case "AddNewLocation", "SaveLocButton", "RateButton"
                CheckWindHailMinimumLimits(LocationIndex)
                If OnePctWindHailLessorsRiskHelper.IsOnePctWindHailLessorsRiskAvailable(Quote) Then
                    SetWindHailLessorsRisk(LocationIndex)
                End If
        End Select
        'Populate() never call populate in a save - Matt A 5-18-17
        Return True
    End Function

    Private Sub CheckWindHailMinimumLimits(locIndex As Integer)
        MinBldgLimitsHelper.CheckWindHailMinimumLimits(Quote, locIndex, Me.Page)
    End Sub

    Private Sub SetWindHailLessorsRisk(locIndex As Integer)
        OnePctWindHailLessorsRiskHelper.SetWindHailLessorsRisk(Quote, locIndex, Me.Page)
    End Sub


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

    Private Sub btnAddBuilding_Click(sender As Object, e As EventArgs) Handles btnAddBuilding.Click
        Me.ctl_BOP_BuildingList.buildingControlNewBuildingRequested(LocationIndex)
        'RaiseEvent AddLocationBuildingRequested(LocationIndex)
    End Sub

    Private Sub HandlePropertyAddressProtectionClassNeeded()
        Me.ctl_BOP_BuildingList.LoadProtectionClasses()
    End Sub

    Private Sub HandlePropertyAddressChange()
        Me.ctl_BOP_BuildingList.PropertyAddressChanged()
    End Sub

    'added 11/15/2017 for Equipment Breakdown MBR
    Public Sub PopulateLocationCoverages()
        Me.ctl_BOP_Location_Coverages.Populate()

    End Sub

    Public Sub EffectiveDateChanging(ByVal NewDate As String, ByVal OldDate As String)
        ' Put any code here to handle when the effective date changes
        Me.ctl_BOP_BuildingList.EffectiveDateChanging(NewDate, OldDate)
        Exit Sub
    End Sub

    Public Sub PopulateBOPBuildingInformation()
        ctl_BOP_BuildingList.PopulateBOPBuildingInformation()
    End Sub

End Class