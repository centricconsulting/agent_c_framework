Imports IFM.PrimativeExtensions

Public Class ctl_CPP_Property_Locations
    Inherits VRControlBase

    Public Event BuildingZeroDeductibleChanged()

    Public Property ActiveLocationIndex As String
        Get
            Return ctl_CPP_CPR_LocationList.ActiveLocationIndex
        End Get
        Set(value As String)
            ctl_CPP_CPR_LocationList.ActiveLocationIndex = value
        End Set
    End Property

    Public Sub HandleBlanketDeductibleChange()
        Me.ctl_CPP_CPR_LocationList.HandleBlanketDeductibleChange()
    End Sub

    Public Sub HandleAgreedAmountChange(ByVal newvalue As Boolean)
        Me.ctl_CPP_CPR_LocationList.HandleAgreedAmountChange(newvalue)
    End Sub

    Public Sub HandleBuildingZeroDeductibleChange()
        RaiseEvent BuildingZeroDeductibleChanged()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandler Me.ctl_CPP_CPR_LocationList.BuildingZeroDeductibleChanged, AddressOf HandleBuildingZeroDeductibleChange
    End Sub

    Public Overrides Function Save() As Boolean
        SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = ""
        ValidateChildControls(valArgs)
    End Sub

    'Added 10/20/2022 for task 77527 MLW       
    Public Sub PopulateInflationGuard()
        Me.ctl_CPP_CPR_LocationList.PopulateInflationGuard()
    End Sub

    Public Sub PopulateCPRBuildingInformation()
        Me.ctl_CPP_CPR_LocationList.PopulateCPRBuildingInformation()
    End Sub

    Public Sub RemoveFunctionalReplacementCost()
        Me.ctl_CPP_CPR_LocationList.RemoveFunctionalReplacementCost()
    End Sub

End Class