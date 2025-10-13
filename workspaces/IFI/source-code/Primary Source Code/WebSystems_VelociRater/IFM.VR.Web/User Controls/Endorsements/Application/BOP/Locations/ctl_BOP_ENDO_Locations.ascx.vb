Public Class ctl_BOP_ENDO_Locations
    Inherits VRControlBase

    Public Property ActiveLocationIndex As String
        Get
            Return hdnAccord.Value
        End Get
        Set(value As String)
            hdnAccord.Value = value
        End Set
    End Property

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        ctl_BOP_ENDO_LocationList.ActiveLocationIndex = ActiveLocationIndex
        Me.PopulateChildControls()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        SaveChildControls()
        ctl_BOP_ENDO_LocationList.Populate()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Sub PopulateLocationCoverages()
        Me.ctl_BOP_ENDO_LocationList.PopulateLocationCoverages()
    End Sub

    Public Sub EffectiveDateChanging(ByVal NewDate As String, ByVal OldDate As String)
        Me.ctl_BOP_ENDO_LocationList.EffectiveDateChanging(NewDate, OldDate)
    End Sub

    Public Sub SetActiveLocation(ByVal Index As String)
        Me.ctl_BOP_ENDO_LocationList.ActiveLocationIndex = Index
    End Sub
End Class