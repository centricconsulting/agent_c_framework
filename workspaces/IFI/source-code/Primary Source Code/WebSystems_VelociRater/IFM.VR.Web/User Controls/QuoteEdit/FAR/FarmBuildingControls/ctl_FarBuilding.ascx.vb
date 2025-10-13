Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions


Public Class ctl_FarBuilding
    Inherits VRControlBase


    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public Property MyBuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
        End Set
    End Property

    Public Property NewBuilding As String
        Get
            Return Me.isNewBuilding.Value
        End Get
        Set(value As String)
            Me.isNewBuilding.Value = value
        End Set
    End Property



    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            'Updated 9/10/18 for multi state MLW
            ' If Me.Quote.IsNotNull Then
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Locations.GetItemAtIndex(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property MyBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding
        Get
            'Updated 9/10/18 for multi state MLW
            'If Me.MyLocation.IsNotNull Then
            If Me.MyLocation IsNot Nothing Then
                Return Me.MyLocation.Buildings.GetItemAtIndex(MyBuildingIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return MyBuildingIndex
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.lnkRemove.Visible = Not Me.HideFromParent
        Me.VRScript.StopEventPropagation(Me.lnkBtnAdd.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
        Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Are you sure you want to delete this building?")
        'Me.VRScript.StopEventPropagation(Me.lnkRemove.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        PopulateHeaderLabel()

        Me.ctl_FarmBuilding_Coverages.MyLocationIndex = Me.MyLocationIndex
        Me.ctl_FarmBuilding_Coverages.MyBuildingIndex = Me.MyBuildingIndex

        Me.ctl_FarmBuilding_Property.MyLocationIndex = Me.MyLocationIndex
        Me.ctl_FarmBuilding_Property.MyBuildingIndex = Me.MyBuildingIndex

        Me.PopulateChildControls()

        Me.divAddButton.Visible = MyLocation.Buildings IsNot Nothing AndAlso MyLocation.Buildings.Count > 0 AndAlso (MyBuildingIndex + 1) = MyLocation.Buildings.Count
    End Sub

    Private Sub PopulateHeaderLabel()
        If MyBuilding IsNot Nothing Then
            Me.lblAccordHeader.Text = String.Format("Building #{0} - {1} {2}", (MyBuildingIndex + 1).ToString(), MyBuilding.YearBuilt, Me.QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.FarmStructureTypeId, MyBuilding.FarmStructureTypeId, Me.Quote.LobType))
        Else
            Me.lblAccordHeader.Text = "Buildings"
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.divContents.Visible = Not HideFromParent
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Building #" + (Me.MyBuildingIndex + 1).ToString()
        Me.ValidateChildControls(valArgs)

    End Sub

#Region "Link Buttons"
    Protected Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkBtnAdd.Click
        If Me.ParentVrControl IsNot Nothing Then
            If TypeOf Me.ParentVrControl Is ctl_FarBuildingList Then
                Dim buildingListControl As ctl_FarBuildingList = DirectCast(Me.ParentVrControl, ctl_FarBuildingList)
                buildingListControl.CreateNewBuilding()
            End If
        End If
        Session("valuationValue") = "False"
        If SubQuoteFirst.ProgramTypeId <> "7" Then
            NewBuilding = "False"
        End If
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        If Not HideFromParent Then
            Me.Save_FireSaveEvent(False)
            IFM.VR.Common.Helpers.FARM.FarmBuildingHelper.RemoveFarmBuilding(Me.Quote, Me.MyLocationIndex, Me.MyBuildingIndex)
            Me.Populate_FirePopulateEvent()
            'Me.ParentVrControl.Populate()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub
#End Region

    Public Overrides Function ToString() As String
        Return String.Format("LocationIndex:{0}  BuildingIndex:{1}  HidesFromParent:{2}", Me.MyLocationIndex, Me.MyBuildingIndex, Me.HideFromParent)
    End Function

    Private Sub ctl_FarBuilding_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If IsPostBack Then
            PopulateHeaderLabel()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        lnkBtnAdd_Click(Nothing, Nothing)
    End Sub

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        Me.ctl_FarmBuilding_Coverages.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        Exit Sub
    End Sub
End Class