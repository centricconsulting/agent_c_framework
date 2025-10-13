Public Class ctl_BOP_PolicyLevelCoverages
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Sub HandleEffectiveDateChange(ByVal NewDate As String, ByVal OldDate As String)
        Me.ctl_BOP_GeneralInformation.UpdateControlsAfterEffectiveDateChange(NewDate)
        Exit Sub
    End Sub

    Public Sub PopulateCoverages()
        ctl_BOP_Coverages.PopulateCyberCoverage()
    End Sub

End Class