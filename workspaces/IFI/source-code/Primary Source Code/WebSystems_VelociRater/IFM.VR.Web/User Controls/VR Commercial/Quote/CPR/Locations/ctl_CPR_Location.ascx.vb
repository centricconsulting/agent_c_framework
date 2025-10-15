Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CPR
Imports QuickQuote.CommonObjects

Public Class ctl_CPR_Location
    Inherits VRControlBase

    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
            ctlProperty_Address.MyLocationIndex = value
            ctl_LocationCoverages.MyLocationIndex = value
            Ctl_CPR_PIO.LocationIndex = value
            ctl_CPR_BldgList.LocationIndex = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote.IsNotNull Then
                Return Me.Quote.Locations.GetItemAtIndex(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property LocationPropertyDeductibleClientID As String
        Get
            Return Me.ctl_LocationCoverages.LocationPropertyDeductibleClientID
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return MyLocationIndex
        End Get
    End Property

    Public Event LocationChanged(ByVal LocIndex As Integer)
    Public Event AddLocationRequested()
    Public Event CopyLocationRequested(LocIndex As Integer)
    Public Event DeleteLocationRequested(LocIndex As Integer)
    Public Event ClearLocationRequested(LocIndex As Integer)
    Public Event BuildingZeroDeductibleChanged()

    Public Overrides Sub AddScriptAlways()

    End Sub

    Dim needBlanketValidation As Boolean = False
    Public Overrides Sub AddScriptWhenRendered()
        'Me.lnkRemove.Visible = Not Me.HideFromParent  ' I set this in populate now MGB
        Me.VRScript.StopEventPropagation(Me.lnkAdd.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkClear.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkCopyLocation.ClientID)
        Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Are you sure you want to delete this location?")
        If Me.divContents.Visible Then
            Me.VRScript.AddVariableLine(String.Format("var locationHeader_{0} = ""{1}"";", MyLocationIndex, Me.lblAccordHeader.ClientID)) 'used to set the address text in this header - used by residence_address control
        End If
        Me.VRScript.AddVariableLine(String.Format("var locationWindHailAvailable = '{0}';", LocationWindHailHelper.IsLocationWindHailAvailable(Quote)))

        Me.VRScript.AddVariableLine(String.Format("var locationWindHailDefaultingAvailable = '{0}';", WindHailDefaultingHelper.IsWindHailDefaultingAvailable(Quote)))
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Sub HandleBlanketDeductibleChange()
        Me.ctl_CPR_BldgList.HandleBlanketDeductibleChange()
        Me.Ctl_CPR_PIO.HandleBlanketDeductibleChange()
    End Sub

    Protected Sub HandlePropertyAddressChange()
        RaiseEvent LocationChanged(MyLocationIndex)
        UpdateHeader()
    End Sub

    Public Sub HandleAgreedAmountChange(ByVal newvalue As Boolean)
        Me.ctl_CPR_BldgList.HandleAgreedAmountChange(newvalue)
        Me.Ctl_CPR_PIO.HandleAgreedAmountChange(newvalue)
    End Sub

    Protected Sub HandlePropertyClear()
        Me.lblAccordHeader.Text = "Location"
    End Sub

    Public Overrides Sub Populate()
        If MyLocationIndex = 0 Then lnkDelete.Visible = False Else lnkDelete.Visible = True
        If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
            lblCPPMessage.Attributes.Add("style", "display:''")
        Else
            lblCPPMessage.Attributes.Add("style", "display:none")
        End If
        UpdateHeader()
        Me.PopulateChildControls()
    End Sub

    Private Sub UpdateHeader()
        Dim txt As String = "Location #" & MyLocationIndex + 1.ToString
        If MyLocation IsNot Nothing AndAlso MyLocation.Address IsNot Nothing Then txt += " - " & MyLocation.Address.HouseNum & " " & Me.MyLocation.Address.StreetName & " " & Me.MyLocation.Address.City
        Me.lblAccordHeader.Text = txt.Ellipsis(34)
        Me.ctlProperty_Address.MyLocationIndex = Me.MyLocationIndex
        Me.ctl_LocationCoverages.MyLocationIndex = Me.MyLocationIndex
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.divContents.Visible = Not HideFromParent
        AddHandler ctlProperty_Address.PropertyAddressChanged, AddressOf HandlePropertyAddressChange
        AddHandler ctlProperty_Address.PropertyCleared, AddressOf HandlePropertyClear
        AddHandler ctl_CPR_BldgList.BuildingZeroDeductibleChanged, AddressOf HandleBuildingZeroDeductibleChange

        Exit Sub
    End Sub

    Private Sub HandleBuildingZeroDeductibleChange()
        RaiseEvent BuildingZeroDeductibleChanged()
    End Sub

    Public Overrides Function Save() As Boolean
        ' SAVE THE LOCATION BLANKET VALUES
        If Quote.HasBlanketBuilding Then
            MyLocation.CauseOfLossTypeId = Quote.BlanketBuildingCauseOfLossTypeId
            MyLocation.CoinsuranceTypeId = Quote.BlanketBuildingCoinsuranceTypeId
            MyLocation.ValuationMethodTypeId = Quote.BlanketBuildingValuationId
        ElseIf Quote.HasBlanketBuildingAndContents Then
            MyLocation.CauseOfLossTypeId = Quote.BlanketBuildingAndContentsCauseOfLossTypeId
            MyLocation.CoinsuranceTypeId = Quote.BlanketBuildingAndContentsCoinsuranceTypeId
            MyLocation.ValuationMethodTypeId = Quote.BlanketBuildingAndContentsValuationId
        ElseIf Quote.HasBlanketContents Then
            MyLocation.CauseOfLossTypeId = Quote.BlanketContentsCauseOfLossTypeId
            MyLocation.CoinsuranceTypeId = Quote.BlanketContentsCoinsuranceTypeId
            MyLocation.ValuationMethodTypeId = Quote.BlanketContentsValuationId
        End If

        UpdateHeader()
        Me.SaveChildControls()
        Select Case Session("CPPCPRCheckACVEventTrigger")
            Case "AddNewLocationLink", "AddNewLocationButton", "SaveLocationLink", "SaveOrRateButton", "ContinueButton", "RateButton"
                CheckValuationRC()
                If SubQuoteFirst IsNot Nothing AndAlso Not SubQuoteFirst.BlanketRatingOptionId = "0" Then
                    needBlanketValidation = CPR_CPP_BlanketRatingMessagingHelper.CheckForAllBuildingAndPropertiesBlanket(SubQuoteFirst)
                End If
                CheckWindHailAppliedAtLocation()
                If WindHailDefaultingHelper.IsWindHailDefaultingAvailable(Quote) Then '0 = N/A Then
                    ApplyDefaultWindHailToLocation()
                End If
        End Select
        Return True
    End Function

    Private Sub CheckValuationRC()
        If ValuationACVHelper.IsValuationACVAvailable(Quote) Then
            Dim valuationUpdated As Boolean = ValuationACVHelper.UpdateValuationACV_ForDwellingClass_ForLocation(MyLocation)
            If valuationUpdated Then
                ValuationACVHelper.ShowValuationRCPopupMessage(Me.Page)
            End If
        End If
    End Sub
    Private Sub CheckWindHailAppliedAtLocation()
        If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) AndAlso MyLocation.WindHailDeductibleLimitId.EqualsAny("32", "33", "34") Then
            '32=1%, 33=2%, 34=5%
            Dim hasWindHailApplied As Boolean = LocationWindHailHelper.LocationWindHailApplied(MyLocation, LocationWindHailHelper.PropertyInTheOpen)
            If hasWindHailApplied = False Then
                hasWindHailApplied = LocationWindHailHelper.LocationWindHailApplied(MyLocation, LocationWindHailHelper.BuildingCoverages)
            End If
            If hasWindHailApplied = False Then
                MyLocation.WindHailDeductibleLimitId = "0"
                LocationWindHailHelper.ShowWindHailNeedsAppliedPopupMessage(Me.Page)
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage AndAlso (Session("CPPCPRCheckACVEventTrigger") = "ContinueButton" OrElse Session("CPPCPRCheckACVEventTrigger") = "SaveLocationLink") Then
                    ctl_LocationCoverages.PopulateWindHailDropDown()
                End If
            End If
        End If
    End Sub

    Private Sub ApplyDefaultWindHailToLocation()
        ctl_LocationCoverages.DefaultWindHailDropDown()
    End Sub

    Private Function CheckCPRCPPExemptCodesBuilding(l As QuickQuoteLocation) As Boolean
        If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
            For Each b As QuickQuoteBuilding In l.Buildings
                If WindHailDefaultingHelper.CheckCPRCPPExemptCodes(b) Then
                    Return True
                    Exit For
                End If
            Next
        End If
        Return False
    End Function


    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Session("CPPCPRCheckACVEventTrigger") = "SaveLocationLink"
        Me.Save_FireSaveEvent()
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        RaiseEvent DeleteLocationRequested(MyLocationIndex)
    End Sub

    Protected Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkAdd.Click
        Session("CPPCPRCheckACVEventTrigger") = "AddNewLocationLink"
        RaiseEvent AddLocationRequested()
    End Sub

    Protected Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        ctlProperty_Address.ClearControl()
        RaiseEvent ClearLocationRequested(MyLocationIndex)
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = "Locations"

        ' Class Code
        If Quote.Locations Is Nothing OrElse Quote.Locations.Count = 0 OrElse Quote.Locations(0).Buildings Is Nothing OrElse Quote.Locations(0).Buildings.Count = 0 Then
            Me.ValidationHelper.AddError("Quote must have at least one building on the first Location.")
        End If

        If needBlanketValidation Then
            Me.ValidationHelper.AddError("Blanket rating has been selected, but either no property coverages are included or the selected coverages don't support it. Please remove the blanket rating or review the location coverages to ensure they support your selection.")
        End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub btnAddBuilding_Click(sender As Object, e As EventArgs) Handles btnAddBuilding.Click
        Me.ctl_CPR_BldgList.buildingControlNewBuildingRequested(MyLocationIndex)
    End Sub

    Private Sub lnkCopyLocation_Click(sender As Object, e As EventArgs) Handles lnkCopyLocation.Click
        RaiseEvent CopyLocationRequested(MyLocationIndex)
    End Sub

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        Me.ctl_CPR_BldgList.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
    End Sub

    'Added 10/20/2022 for task 77527 MLW
    Public Sub PopulateInflationGuard()
        ctl_CPR_BldgList.PopulateInflationGuard()
    End Sub

    Public Sub PopulateCPRBuildingInformation()
        Me.ctl_CPR_BldgList.PopulateCPRBuildingInformation()
        Me.Ctl_CPR_PIO.PopulatePropInOpen()
    End Sub

    Public Sub RemoveFunctionalReplacementCost()
        Me.ctl_CPR_BldgList.RemoveFunctionalReplacementCost()
    End Sub
End Class