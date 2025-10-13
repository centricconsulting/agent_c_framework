Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers

Public Class ctl_BOP_Building
    Inherits VRControlBase

    Public ReadOnly Property ScrollToControlId As String
        Get
            Return ctl_BOP_Building_Information.MyScrollToID
        End Get
    End Property

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
            Me.ctl_BOP_Building_Information.LocationIndex = value
            Me.ctl_BOP_BuildingCoverages.LocationIndex = value
        End Set
    End Property

    Public Property BuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
            Me.ctl_BOP_Building_Information.BuildingIndex = value
            Me.ctl_BOP_BuildingCoverages.BuildingIndex = value
        End Set
    End Property

    Private ReadOnly Property MyBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.GetItemAtIndex(Me.BuildingIndex)
            End If
            Return Nothing
        End Get

    End Property


    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return BuildingIndex
        End Get
    End Property

    Public Event NewBuildingRequested(ByVal LocIndex As Integer)
    Public Event DeleteBuildingRequested(ByVal LocIndex As Integer, ByVal BldgIndex As Integer)
    Public Event ClearBuildingRequested(ByVal LocIndex As Integer, ByVal BldgIndex As Integer)

    Public Sub EffectiveDateChanging(ByVal NewDate As String, ByVal OldDate As String)
        Me.ctl_BOP_BuildingCoverages.EffectiveDateChanging(NewDate, OldDate)
        Exit Sub
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Private Sub ClassificationChanged(ByVal LocNdx As Integer, ByVal BldNdx As Integer, ByVal ClsNdx As Integer, ByVal NewClassCode As String)
        Save()
        Me.ctl_BOP_BuildingCoverages.ClassificationChanged()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Me.VRScript.CreateAccordion(MainAccordionDivId, Me.hdnAccordList, "0")
        Me.VRScript.CreateConfirmDialog(Me.lnkClear.ClientID, "Clear?")
        Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Delete Building?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkNew.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Sub HandleAddressChange()
        Me.ctl_BOP_Building_Information.Populate()
        Exit Sub
    End Sub

    Public Sub UpdateProtectionClasses()
        Me.ctl_BOP_Building_Information.UpdateProtectionClasses()
    End Sub

    Public Overrides Sub Populate()
        If MyBuilding.IsNotNull Then
            ' Don't show the delete button on the first building
            If BuildingIndex = 0 Then lnkDelete.Attributes.Add("style", "display:none") Else lnkDelete.Attributes.Add("style", "display:''")
            ' Set the building header
            Me.lblAccordHeader.Text = "Building #{0} - {1}".FormatIFM(Me.BuildingIndex + 1, Me.MyBuilding.Description).Ellipsis(55)
        End If

        'Added 09/09/2021 for BOP Endorsements Task 63912 MLW
        If IsQuoteReadOnly() Then
            btnSave.Visible = False
            'Added 10/14/2021 for BOP Endorsements Task 65816 MLW
            ctl_BOP_ENDO_App_Building.Visible = True
            ctl_BOP_ENDO_App_Building.LocationIndex = Me.LocationIndex
            ctl_BOP_ENDO_App_Building.BuildingIndex = BuildingIndex
        Else
            'Added 10/14/2021 for BOP Endorsements Task 65816 MLW
            ctl_BOP_ENDO_App_Building.Visible = False
        End If

        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandler Me.ctl_BOP_Building_Information.BuildingClassificationChanged, AddressOf ClassificationChanged
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        BopNewCoEarthQuakeHelper.ProcessEarthQuake(Quote)
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        RaiseEvent ClearBuildingRequested(LocationIndex, BuildingIndex)
    End Sub

    Private Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        RaiseEvent DeleteBuildingRequested(LocationIndex, BuildingIndex)
    End Sub

    Private Sub lnkNew_Click(sender As Object, e As EventArgs) Handles lnkNew.Click
        RaiseEvent NewBuildingRequested(LocationIndex)
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Save_FireSaveEvent()
        Populate() 'Added 12/31/18 for Bug 30676 MLW
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Save_FireSaveEvent()
    End Sub

    Public Sub PopulateBOPBuildingInformation()
        ctl_BOP_Building_Information.PopulateBOPBuildingInformation()
    End Sub
End Class