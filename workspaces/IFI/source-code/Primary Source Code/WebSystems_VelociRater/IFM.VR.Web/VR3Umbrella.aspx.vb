Imports IFM.VR.Common.Workflow

Public Class VR3Umbrella
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 7/17/2019; original logic in ELSE
            VR.Common.Workflow.Workflow.SetAppOrQuote(Common.Workflow.Workflow.WorkflowAppOrQuote.Quote, Me.ReadOnlyPolicyIdAndImageNum)
        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            VR.Common.Workflow.Workflow.SetAppOrQuote(Common.Workflow.Workflow.WorkflowAppOrQuote.Quote, Me.EndorsementPolicyIdAndImageNum)
        Else
            VR.Common.Workflow.Workflow.SetAppOrQuote(Common.Workflow.Workflow.WorkflowAppOrQuote.Quote, Me.QuoteId)
        End If
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection)
        If workflow = IFM.VR.Common.Workflow.Workflow.WorkflowSection.na AndAlso Me.ctl_WorkflowManager_Quote_fuppup.CurrentWorkFlow = IFM.VR.Common.Workflow.Workflow.WorkflowSection.na Then
            Me.ctl_WorkflowManager_Quote_fuppup.CurrentWorkFlow = IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
        End If
    End Sub

End Class