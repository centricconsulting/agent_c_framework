Imports IFM.PrimativeExtensions

Public Class ctl_CGL_Location
    Inherits VRControlBase

    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
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

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return MyLocationIndex
        End Get
    End Property

    Public Event LocationChanged(ByVal LocIndex As Integer)
    Public Event AddLocationRequested()
    Public Event DeleteLocationRequested(LocIndex As Integer)


    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Me.lnkRemove.Visible = Not Me.HideFromParent  ' I set this in populate now MGB
        Me.VRScript.StopEventPropagation(Me.lnkBtnAdd.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
        Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Are you sure you want to delete this location?")
        If Me.divContents.Visible Then
            Me.VRScript.AddVariableLine(String.Format("var locationHeader_{0} = ""{1}"";", MyLocationIndex, Me.lblAccordHeader.ClientID)) 'used to set the address text in this header - used by residence_address control
        End If

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Protected Sub HandlePropertyAddressChange()
        RaiseEvent LocationChanged(MyLocationIndex)
        UpdateHeader()
    End Sub

    Protected Sub HandlePropertyClear()
        Me.lblAccordHeader.Text = "Location"
    End Sub

    Public Overrides Sub Populate()
        If MyLocationIndex = 0 Then lnkRemove.Visible = False Else lnkRemove.Visible = True
        UpdateHeader()
        Me.PopulateChildControls()
    End Sub

    Private Sub UpdateHeader()
        Me.lblAccordHeader.Text = String.Format("Location #" & MyLocationIndex + 1.ToString & " - {0} {1} {2}", Me.MyLocation.Address.HouseNum, Me.MyLocation.Address.StreetName, Me.MyLocation.Address.City)
        'Me.lblAccordHeader.Text = String.Format("Location - {0} {1} {2}", Me.MyLocation.Address.HouseNum, Me.MyLocation.Address.StreetName, Me.MyLocation.Address.City)
        Me.lblAccordHeader.Text = Me.lblAccordHeader.Text.Ellipsis(70)
        Me.ctlProperty_Address.MyLocationIndex = Me.MyLocationIndex
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.divContents.Visible = Not HideFromParent
        AddHandler ctlProperty_Address.PropertyAddressChanged, AddressOf HandlePropertyAddressChange
        AddHandler ctlProperty_Address.PropertyCleared, AddressOf HandlePropertyClear
    End Sub

    Public Overrides Function Save() As Boolean
        UpdateHeader()
        Me.SaveChildControls()
        Return True
    End Function

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        RaiseEvent DeleteLocationRequested(MyLocationIndex)
    End Sub

    Protected Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkBtnAdd.Click
        RaiseEvent AddLocationRequested()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

End Class