Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers

Public Class VR3Auto
    Inherits BasePage

    Protected Friend ReadOnly Property WorkFlowManager As ctl_Master_Edit 'added 8/1/2019
        Get
            Return Me.ctl_Master_Edit
        End Get
    End Property

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.DirtyFormDiv = "divQuoteEditControls"
        End If
        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 7/17/2019; original logic in ELSE
            VR.Common.Workflow.Workflow.SetAppOrQuote(Common.Workflow.Workflow.WorkflowAppOrQuote.Quote, Me.ReadOnlyPolicyIdAndImageNum)
        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            VR.Common.Workflow.Workflow.SetAppOrQuote(Common.Workflow.Workflow.WorkflowAppOrQuote.Quote, Me.EndorsementPolicyIdAndImageNum)
        Else
            VR.Common.Workflow.Workflow.SetAppOrQuote(Common.Workflow.Workflow.WorkflowAppOrQuote.Quote, Me.QuoteId)
        End If
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection)
        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Dim err As String = Nothing
                ctlTreeView.RefreshQuote()
                ctlTreeView.RefreshRatedQuote()
            Case Else
                ctlTreeView.RefreshQuote()
        End Select
    End Sub

    Private Sub ctl_Master_Edit_QuoteRated() Handles ctl_Master_Edit.QuoteRated
        ctlTreeView.RefreshRatedQuote()
    End Sub

    Private Sub ctl_Master_Edit_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_Master_Edit.SaveRequested
        ctlTreeView.RefreshQuote()
    End Sub

#Region "Tree Events"
    Private Sub ctlTreeView_QuoteUpdated(sender As Object, e As EventArgs) Handles ctlTreeView.QuoteUpdated
        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
            Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
        Else
            VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId)
        End If
        Me.ctl_Master_Edit.TreeChangedQuote()
    End Sub

    Private Sub ctlTreeView_EditDriver(driverNumber As Integer) Handles ctlTreeView.EditDriver
        ctl_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers, (driverNumber - 1).ToString())
    End Sub

    Private Sub ctlTreeView_EditPolicyholder(policyholderNumber As Integer) Handles ctlTreeView.EditPolicyholder
        ctl_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, (policyholderNumber - 1).ToString())
    End Sub

    Private Sub ctlTreeView_EditVehicle(vehicleNumber As Integer) Handles ctlTreeView.EditVehicle
        ctl_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles, (vehicleNumber - 1).ToString())
    End Sub

    Private Sub ctlTreeView_NewDriver(driverNumber As Integer) Handles ctlTreeView.NewDriver
        Me.ctl_Master_Edit.CreateNewDriver() ' do this here just to avoid this type of logic outside of the controls
        ctlTreeView.RefreshQuote()
    End Sub

    Private Sub ctlTreeView_NewVehicle(vehicleNumber As Integer) Handles ctlTreeView.NewVehicle
        Me.ctl_Master_Edit.CreateNewVehicle() ' do this here just to avoid this type of logic outside of the controls
        ctlTreeView.RefreshQuote()
    End Sub

    Private Sub ctlTreeView_NewPolicyholder(policyholderNumber As Integer) Handles ctlTreeView.NewPolicyholder
        ctl_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, (policyholderNumber - 1).ToString())
    End Sub
#End Region

#Region "Shows"
    Private Sub ctlTreeView_ShowPolicyholders(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyholders
        ctl_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
    End Sub

    Private Sub ctlTreeView_ShowDrivers(sender As Object, e As EventArgs) Handles ctlTreeView.ShowDrivers
        ctl_Master_Edit.RefreshVehicles()
        ctl_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers, "0")
    End Sub

    Private Sub ctlTreeView_ShowVehicles(sender As Object, e As EventArgs) Handles ctlTreeView.ShowVehicles
        ctl_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles, "0")
    End Sub

    Private Sub ctlTreeView_ShowCoverages(sender As Object, e As EventArgs) Handles ctlTreeView.ShowCoverages
        ctl_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
    End Sub

    Private Sub ctlTreeView_ShowQuoteSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowQuoteSummary
        ctl_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "0")
    End Sub

    Private Sub ctlTreeView_ShowPrintHistory(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPrintHistory
        ctl_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.printHistory, "0")
    End Sub
    Private Sub ctlTreeView_ShowPolicyHistory(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyHistory
        ctl_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
    End Sub
    Private Sub ctlTreeView_ShowBillingInformation(sender As Object, e As EventArgs) Handles ctlTreeView.ShowBillingInformation
        ctl_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub
    Private Sub ctlTreeView_ShowFileUpload(sender As Object, e As EventArgs) Handles ctlTreeView.ShowFileUpload
        ctl_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "0")
    End Sub
#End Region

#Region "Reports"
    ' Should look into handling this in the treeview directly - Matt A
    Private Sub ctlTreeView_ViewDriverCreditReport(driverNumber As Integer) Handles ctlTreeView.ViewDriverCreditReport
        Dim Err As String = Nothing
        Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.GetCreditReport(Common.CreditReportSubject.Driver, Me.Quote, Err, driverNumber, True)
        If ReportFile IsNot Nothing Then
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("CreditReport_{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
            Response.BinaryWrite(ReportFile)
        Else
            Me.VRScript.AddScriptLine("alert('" + Server.HtmlEncode(Err) + "');")
        End If
    End Sub

    Private Sub ctlTreeView_ViewDriverMvrReport(driverNumber As Integer) Handles ctlTreeView.ViewDriverMvrReport
        Dim Err As String = Nothing
        Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.PERSONAL_AUTO_GetMVRReport(Me.Quote, driverNumber, Err, True)
        If ReportFile IsNot Nothing Then
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("MVR_{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
            Response.BinaryWrite(ReportFile)
        Else
            Me.VRScript.AddScriptLine("alert('" + Server.HtmlEncode(Err) + "');")
        End If
    End Sub

    Private Sub ctlTreeView_ViewClueAutoReport(sender As Object, e As EventArgs) Handles ctlTreeView.ViewClueAutoReport
        Dim Err As String = Nothing
        Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.PERSONAL_AUTO_GetCLUEReport(Me.Quote, Err, True)
        If ReportFile IsNot Nothing Then
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("CLUE_{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
            Response.BinaryWrite(ReportFile)
        Else
            Me.VRScript.AddScriptLine("alert('No Loss Records Found.');")
        End If
    End Sub

    Private Sub ctlTreeView_EffectiveDateChangedFromTree(qqTranType As QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String) Handles ctlTreeView.EffectiveDateChangedFromTree
        Me.ctl_Master_Edit.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
    End Sub

    Private Sub ctlTreeView_EffectiveDateChanging(NewQuoteEffectiveDate As String, OldQuoteEffectiveDate As String) Handles ctlTreeView.EffectiveDateChanging
        Me.ctl_Master_Edit.EffDateChanged(NewQuoteEffectiveDate, OldQuoteEffectiveDate)
    End Sub

#End Region

End Class