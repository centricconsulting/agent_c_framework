Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Common.Helpers.BOP
Imports IFM.VR.Web.EndorsementStructures
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects

Public Class ctl_BOP_ENDO_Location
    Inherits VRControlBase

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
            Me.ctlProperty_Address.MyLocationIndex = value
            Me.ctl_BOP_ENDO_Location_Coverages.LocationIndex = value
            Me.ctl_BOP_ENDO_BuildingList.LocationIndex = value
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

    Private Property _BopDictItems As DevDictionaryHelper.AllCommercialDictionary
    Public ReadOnly Property BopDictItems As DevDictionaryHelper.AllCommercialDictionary
        Get
            If Quote IsNot Nothing Then
                If _BopDictItems Is Nothing Then
                    _BopDictItems = New DevDictionaryHelper.AllCommercialDictionary(Quote)
                End If
            End If
            Return _BopDictItems
        End Get
    End Property

    Public ReadOnly Property TransactionLimitReached As Boolean
        Get
            'get from Direct Parent ViewState
            If Me.ParentVrControl IsNot Nothing AndAlso TypeOf Me.ParentVrControl Is ctl_BOP_ENDO_LocationList Then
                Dim Parent = CType(ParentVrControl, ctl_BOP_ENDO_LocationList)
                Return Parent.TransactionLimitReached
            End If
            Return False
        End Get
    End Property

    Public Event NewLocationRequested()
    Public Event DeleteLocationRequested(ByRef LocIndex As Integer)
    Public Event ClearLocationRequested(ByRef LocIndex As Integer)
    Public Event AddLocationBuildingRequested(ByRef LocIndex As Integer)
    Public Event CountTransactions()
    Public Event UpdateRemarks()

    Public Sub HandleAddressChange() Handles ctlProperty_Address.PropertyAddressChanged
        Me.ctl_BOP_ENDO_BuildingList.HandleAddressChange()
        Exit Sub
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateConfirmDialog(Me.lnkClear.ClientID, "Clear?")
        Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "If this location is deleted the buildings and coverages associated with the location will be deleted. Are you sure you want to delete this location?")
        'Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Delete Location?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkNew.ClientID)
        Me.VRScript.AddVariableLine(String.Format("var locationHeader_{0} = ""{1}"";", LocationIndex, Me.lblAccordHeader.ClientID)) 'used to set the address text in this header - used by property_address control

        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
            Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
            AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
            Select Case TypeOfEndorsement()
                Case EndorsementTypeString.BOP_AddDeleteContractorsEquipment,
                 EndorsementTypeString.BOP_AddDeleteContractorsEquipmentLienholder,
                 EndorsementTypeString.BOP_AddDeleteLocationLienholder,
                 EndorsementTypeString.BOP_AddDeleteLocation
                    If AllPreExistingItems.PreExisting_Locations.isPreExistingLocationByLocationObject(MyLocation) Then
                        DisableControls()
                        DisableHeaderLinks()
                        Me.btnAddBuilding.Visible = False
                    End If
            End Select
        End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

        If Me.MyLocation.IsNotNull Then

            DisableHeaderLinks(False)
            Me.btnAddBuilding.Visible = True
            lnkNew.Visible = True
            lnkDelete.Visible = True


            If LocationIndex = 0 Then
                lnkDelete.Visible = False
            Else
                lnkDelete.Visible = True
            End If

            'Total Transactions Counted and UI components removed.
            If TransactionLimitReached Then
                Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
                AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
                If AllPreExistingItems.PreExisting_Locations.isPreExistingLocationByLocationObject(MyLocation) Then
                    lnkDelete.Visible = False
                End If
                lnkNew.Visible = False
                DisableHeaderLinks()
            End If

            UpdateAccordHeader()


        End If

        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandler Me.ctlProperty_Address.ProtectionClassLookupNeeded, AddressOf HandlePropertyAddressProtectionClassNeeded
        AddHandler Me.ctlProperty_Address.PropertyAddressChanged, AddressOf HandlePropertyAddressChange
    End Sub

    Private Sub UpdateAccordHeader()
        Dim title As String = ""
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
        UpdateAccordHeader()
        SaveChildControls()
        BopDictItems.UpdateLocations(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, MyLocation, LocationIndex)

        Select Case Session("BOPCheckLimitEventTrigger")
            Case "AddNewLocation", "SaveLocButton", "RateButton"
                CheckWindHailMinimumLimits(LocationIndex)
                If OnePctWindHailLessorsRiskHelper.IsOnePctWindHailLessorsRiskAvailable(Quote) Then
                    SetWindHailLessorsRisk(LocationIndex)
                End If
        End Select
        RaiseEvent UpdateRemarks()
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

    ''' <summary>
    ''' Disables all the controls on the page
    ''' </summary>
    Private Sub DisableControls()
        VRScript.AddScriptLine("$(document).ready(function () {ifm.vr.ui.SingleContainerContentDisable(['" + Me.divContents.ClientID + "']);});")
    End Sub

    Private Sub DisableHeaderLinks(Optional choice As Boolean = True)
        Me.lnkClear.Visible = Not choice
        Me.lnkSave.Visible = Not choice
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
        Populate_FirePopulateEvent()
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
        Me.ctl_BOP_ENDO_BuildingList.buildingControlNewBuildingRequested(LocationIndex)
    End Sub



    Private Sub HandlePropertyAddressProtectionClassNeeded()
        Me.ctl_BOP_ENDO_BuildingList.LoadProtectionClasses()
    End Sub

    Private Sub HandlePropertyAddressChange()
        Me.ctl_BOP_ENDO_BuildingList.PropertyAddressChanged()
    End Sub

    'added 11/15/2017 for Equipment Breakdown MBR
    Public Sub PopulateLocationCoverages()
        Me.ctl_BOP_ENDO_Location_Coverages.Populate()

    End Sub

    Public Sub EffectiveDateChanging(ByVal NewDate As String, ByVal OldDate As String)
        ' Put any code here to handle when the effective date changes
        Me.ctl_BOP_ENDO_BuildingList.EffectiveDateChanging(NewDate, OldDate)
        Exit Sub
    End Sub

End Class