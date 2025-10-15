Public Class ctl_CGL_PolicyLevelCoverages
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

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnSaveAndGotoLocations.Click
        Me.Save_FireSaveEvent()
        If sender.Equals(btnSaveAndGotoLocations) Then
            If Me.ValidationSummmary.HasErrors = False Then
                Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.location, "")
            End If
        End If
    End Sub
End Class