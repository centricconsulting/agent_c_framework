Imports IFM.VR.Common.Workflow

Public Class VRECpp
    Inherits BasePage

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As Workflow.WorkflowSection)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        VR.Common.Workflow.Workflow.SetAppOrQuote(Common.Workflow.Workflow.WorkflowAppOrQuote.Endorsement, Me.EndorsementPolicyIdAndImageNum)
    End Sub

End Class