Imports System.Configuration.ConfigurationManager
Imports IFM.VR.Common.Workflow

Public Class VREPolicyInfoPage
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Sub Populate()
        ctlTreeView.RefreshQuote()
    End Sub

    Private Sub VREPolicyInfo_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit
        Me.MasterPageFile = "Velocirater.master"
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As Workflow.WorkflowSection)
        Populate()
    End Sub
End Class