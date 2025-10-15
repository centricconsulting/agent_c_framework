Imports System.Configuration.ConfigurationManager
Imports IFM.VR.Common.Workflow

Public Class VREBillingUpdate
    Inherits BasePage

    'Added 7/24/2019 for Home Endorsements MLW

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Sub Populate()
        ctlTreeView.RefreshQuote()
    End Sub

    Private Sub VREPolicyInfo_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit
        Me.MasterPageFile = "Velocirater.master"
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As Workflow.WorkflowSection)
        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Dim err As String = Nothing
                ctlTreeView.RefreshQuote()
                ctlTreeView.RefreshRatedQuote()
            Case Else
                ctlTreeView.RefreshQuote()
        End Select
    End Sub

    Private Sub ctlTreeView_ShowBillingInformation(sender As Object, e As EventArgs) Handles ctlTreeView.ShowBillingInformation
        ctl_WorkflowManager_Billing_Update.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub

    Private Sub ctlTreeView_ShowQuoteSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowQuoteSummary
        ctl_WorkflowManager_Billing_Update.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "0")
    End Sub

    Private Sub ctlTreeView_ShowFileUpload(sender As Object, e As EventArgs) Handles ctlTreeView.ShowFileUpload
        ctl_WorkflowManager_Billing_Update.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "0")
    End Sub

    Private Sub ctl_WorkflowManager_Billing_Update_QuoteRated() Handles ctl_WorkflowManager_Billing_Update.QuoteRated
        ctlTreeView.RefreshRatedQuote()
    End Sub
End Class