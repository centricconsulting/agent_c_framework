Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers

Public Class VR3Home
    Inherits BasePage

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.DirtyFormDiv = Me.pnlMain.ClientID
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

    Private Sub ctlHomeInput_BroadcastGenericEvent(type As VRControlBase.BroadCastEventType) Handles ctlHomeInput.BroadcastGenericEvent
        Select Case type
            Case VRControlBase.BroadCastEventType.ThirdPartyReportOrdered
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                    Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.Quote)
                ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.Quote)
                Else
                    IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId, QuickQuoteXML.QuickQuoteSaveType.Quote)
                End If
                ctlTreeView.RefreshQuote()
        End Select

    End Sub

    Private Sub ctlHomeInput_QuoteRated() Handles ctlHomeInput.QuoteRated
        ctlTreeView.RefreshRatedQuote()
    End Sub

    Private Sub ctlHomeInput_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlHomeInput.SaveRequested
        ctlTreeView.RefreshQuote()
    End Sub

    Private Sub ctlTreeView_EffectiveDateChanging(NewQuoteEffectiveDate As String, OldQuoteEffectiveDate As String) Handles ctlTreeView.EffectiveDateChanging
        Me.ctlHomeInput.EffDateChanged(NewQuoteEffectiveDate, OldQuoteEffectiveDate)
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
        Me.ctlHomeInput.Populate()
    End Sub

    Private Sub ctlTreeView_ShowPolicyholders(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyholders
        ctlHomeInput.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
    End Sub

    Private Sub ctlTreeView_EditPolicyholder(policyholderNumber As Integer) Handles ctlTreeView.EditPolicyholder
        ctlHomeInput.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, (policyholderNumber - 1).ToString())
    End Sub

    Private Sub ctlTreeView_NewPolicyholder(policyholderNumber As Integer) Handles ctlTreeView.NewPolicyholder
        ctlHomeInput.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
    End Sub

    Private Sub ctlTreeView_ShowResidence(sender As Object, e As EventArgs) Handles ctlTreeView.ShowResidence
        ctlHomeInput.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_, "")
    End Sub

    Private Sub ctlTreeView_ShowCoverages(sender As Object, e As EventArgs) Handles ctlTreeView.ShowCoverages
        ctlHomeInput.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "")
    End Sub

    Private Sub ctlTreeView_ShowQuoteSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowQuoteSummary
        ctlHomeInput.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
    End Sub

    'Added 7/18/2019 for Home Endorsements Project Tasks 38926, 38927, 38928 MLW
    Private Sub ctlTreeView_ShowPrintHistory(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPrintHistory
        ctlHomeInput.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.printHistory, "0")
    End Sub
    Private Sub ctlTreeView_ShowPolicyHistory(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyHistory
        ctlHomeInput.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
    End Sub
    Private Sub ctlTreeView_ShowBillingInformation(sender As Object, e As EventArgs) Handles ctlTreeView.ShowBillingInformation
        ctlHomeInput.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub

    'Added 7/19/2019 for Home Endorsements Project Task 38929 MLW
    Private Sub ctlTreeView_ShowFileUpload(sender As Object, e As EventArgs) Handles ctlTreeView.ShowFileUpload
        ctlHomeInput.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "0")
    End Sub
#End Region

#Region "Reports"
    ' should look into doing this directly in the treeview
    Private Sub ctlTreeView_ViewApplicantCreditReport(applicantNumber As Integer) Handles ctlTreeView.ViewApplicantCreditReport
        Dim Err As String = Nothing
        Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.GetCreditReport(Common.CreditReportSubject.Applicant, Me.Quote, Err, applicantNumber, True)
        If ReportFile IsNot Nothing Then
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("CreditReport_{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
            Response.BinaryWrite(ReportFile)
        Else
            Me.VRScript.AddScriptLine("alert('" + Server.HtmlEncode(Err) + "');")
        End If
    End Sub

    'Added 7/19/2019 for Home Endorsements Project Tasks 38922 and 38929 MLW
    Private Sub ctlTreeView_ViewCluePropertyReport(sender As Object, e As EventArgs) Handles ctlTreeView.ViewCluePropertyReport
        Dim Err As String = Nothing
        Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.PERSONAL_HOME_GetCLUEReport(Me.Quote, Err, True)
        If ReportFile IsNot Nothing Then
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("ClueReport{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
            Response.BinaryWrite(ReportFile)
        Else
            Me.VRScript.AddScriptLine("alert('" + Server.HtmlEncode(Err) + "');")
        End If
    End Sub

    Private Sub ctlTreeView_EffectiveDateChangedFromTree(qqTranType As QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String) Handles ctlTreeView.EffectiveDateChangedFromTree
        Me.ctlHomeInput.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
    End Sub

#End Region

End Class