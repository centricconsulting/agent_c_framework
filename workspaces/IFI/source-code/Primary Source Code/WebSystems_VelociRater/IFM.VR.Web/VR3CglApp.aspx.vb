Imports IFM.VR.Common.Workflow

Public Class VR3CglApp
    Inherits BasePage

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As Workflow.WorkflowSection)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Title = "Application"
        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 7/17/2019; original logic in ELSE
            VR.Common.Workflow.Workflow.SetAppOrQuote(Common.Workflow.Workflow.WorkflowAppOrQuote.App, Me.ReadOnlyPolicyIdAndImageNum)
        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            VR.Common.Workflow.Workflow.SetAppOrQuote(Common.Workflow.Workflow.WorkflowAppOrQuote.App, Me.EndorsementPolicyIdAndImageNum)
        Else
            VR.Common.Workflow.Workflow.SetAppOrQuote(Common.Workflow.Workflow.WorkflowAppOrQuote.App, Me.QuoteId)
        End If
    End Sub

End Class