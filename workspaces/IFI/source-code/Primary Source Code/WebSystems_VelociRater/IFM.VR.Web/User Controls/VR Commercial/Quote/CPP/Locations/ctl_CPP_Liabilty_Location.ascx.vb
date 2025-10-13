Imports IFM.PrimativeExtensions

Public Class ctl_CPP_Liabilty_Location
    Inherits VRControlBase

    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
            'ctlProperty_Address.MyLocationIndex = value
            'ctl_LocationCoverages.MyLocationIndex = value
            'Ctl_CPR_PIO.LocationIndex = value
            'ctl_CPR_BldgList.LocationIndex = value
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

    'Protected Overrides ReadOnly Property MyAccordionIndex As Integer
    '    Get
    '        Return MyLocationIndex
    '    End Get
    'End Property

    'Public Event LocationChanged(ByVal LocIndex As Integer)
    'Public Event AddLocationRequested()
    'Public Event CopyLocationRequested(LocIndex As Integer)
    'Public Event DeleteLocationRequested(LocIndex As Integer)
    'Public Event ClearLocationRequested(LocIndex As Integer)

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.divLocationItem.ClientID, Me.hdnAccord, "0")
        Me.VRScript.AddScriptLine("$( '#" + Me.divLocationItem.ClientID + "' ).accordion('option', 'active', 'false');") 'used to collapse all at start, since they do not contain data.
        'Me.lnkRemove.Visible = Not Me.HideFromParent  ' I set this in populate now MGB
        'Me.VRScript.StopEventPropagation(Me.lnkAdd.ClientID)
        'Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        'Me.VRScript.StopEventPropagation(Me.lnkClear.ClientID)
        'Me.VRScript.StopEventPropagation(Me.lnkCopyLocation.ClientID)
        'Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Are you sure you want to delete this location?")
        'If Me.divContents.Visible Then
        '    Me.VRScript.AddVariableLine(String.Format("var locationHeader_{0} = ""{1}"";", MyLocationIndex, Me.lblAccordHeader.ClientID)) 'used to set the address text in this header - used by residence_address control
        'End If

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    'Protected Sub HandlePropertyAddressChange()
    '    RaiseEvent LocationChanged(MyLocationIndex)
    '    UpdateHeader()
    'End Sub

    'Protected Sub HandlePropertyClear()
    '    Me.lblAccordHeader.Text = "Location"
    'End Sub

    Public Overrides Sub Populate()
        'If MyLocationIndex = 0 Then lnkDelete.Visible = False Else lnkDelete.Visible = True
        'If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
        '    lblCPPMessage.Attributes.Add("style", "display:''")
        'Else
        '    lblCPPMessage.Attributes.Add("style", "display:none")
        'End If
        UpdateHeader()
        Me.PopulateChildControls()
    End Sub

    Private Sub UpdateHeader()
        Dim txt As String = "Location #" & MyLocationIndex + 1.ToString
        If MyLocation IsNot Nothing AndAlso MyLocation.Address IsNot Nothing Then txt += " - " & MyLocation.Address.HouseNum & " " & Me.MyLocation.Address.StreetName & " " & Me.MyLocation.Address.City
        Me.lblAccordHeader.Text = txt.Ellipsis(34)
        'Me.ctlProperty_Address.MyLocationIndex = Me.MyLocationIndex
        'Me.ctl_LocationCoverages.MyLocationIndex = Me.MyLocationIndex
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Me.divContents.Visible = Not HideFromParent
        'AddHandler ctlProperty_Address.PropertyAddressChanged, AddressOf HandlePropertyAddressChange
        'AddHandler ctlProperty_Address.PropertyCleared, AddressOf HandlePropertyClear

        Exit Sub
    End Sub

    Public Overrides Function Save() As Boolean
        UpdateHeader()
        Me.SaveChildControls()

        Return True
    End Function

    'Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
    '    Me.Save_FireSaveEvent()
    'End Sub

    'Protected Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
    '    RaiseEvent DeleteLocationRequested(MyLocationIndex)
    'End Sub

    'Protected Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkAdd.Click
    '    RaiseEvent AddLocationRequested()
    'End Sub

    'Protected Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
    '    ctlProperty_Address.ClearControl()
    '    RaiseEvent ClearLocationRequested(MyLocationIndex)
    'End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'MyBase.ValidateControl(valArgs)

        'Me.ValidationHelper.GroupName = "Locations"

        '' Class Code
        'If Quote.Locations Is Nothing OrElse Quote.Locations.Count = 0 OrElse Quote.Locations(0).Buildings Is Nothing OrElse Quote.Locations(0).Buildings.Count = 0 Then
        '    Me.ValidationHelper.AddError("Quote must have at least one building on the first Location.")
        'End If

        Me.ValidateChildControls(valArgs)
    End Sub

    'Private Sub btnAddBuilding_Click(sender As Object, e As EventArgs) Handles btnAddBuilding.Click
    '    Me.ctl_CPR_BldgList.buildingControlNewBuildingRequested(MyLocationIndex)
    'End Sub

    'Private Sub lnkCopyLocation_Click(sender As Object, e As EventArgs) Handles lnkCopyLocation.Click
    '    RaiseEvent CopyLocationRequested(MyLocationIndex)
    'End Sub

End Class