Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers

Public Class VR3AutoApp
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Title = "Application"
        If IsPostBack = False Then
            Me.DirtyFormDiv = "divAppEditControls"
        End If
        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 7/17/2019; original logic in ELSE
            VR.Common.Workflow.Workflow.SetAppOrQuote(Common.Workflow.Workflow.WorkflowAppOrQuote.App, Me.ReadOnlyPolicyIdAndImageNum)
        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            VR.Common.Workflow.Workflow.SetAppOrQuote(Common.Workflow.Workflow.WorkflowAppOrQuote.App, Me.EndorsementPolicyIdAndImageNum)
        Else
            VR.Common.Workflow.Workflow.SetAppOrQuote(Common.Workflow.Workflow.WorkflowAppOrQuote.App, Me.QuoteId)
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

    Private Sub ctl_App_Master_Edit_QuoteRated() Handles ctl_App_Master_Edit.QuoteRated
        'ctlTreeView.RefreshQuote()

        ' Switched this to rated quote because treeview wasn't showing discounts & messages on rate, only when quote was reloaded 4/1/2019 MGB 
        ctlTreeView.RefreshRatedQuote()
    End Sub

    Private Sub ctl_App_Master_Edit_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_App_Master_Edit.SaveRequested
        ctlTreeView.RefreshQuote()
    End Sub

#Region "Tree Events"
    Private Sub ctlTreeView_QuoteUpdated(sender As Object, e As EventArgs) Handles ctlTreeView.QuoteUpdated
        ' get the newest quote info from the tree
        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
            Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
        Else
            VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
        End If
    End Sub

    Private Sub ctlTreeView_ShowApplication(sender As Object, e As EventArgs) Handles ctlTreeView.ShowApplication
        ' have the UW questions been answered
        If Me.Quote IsNot Nothing Then
            Dim uwAnswered As Int32 = 0
            '9/17/18 No updates needed for multi-state since this is for PPA only MLW
            If Me.Quote.PolicyUnderwritings IsNot Nothing Then
                For Each q In Me.Quote.PolicyUnderwritings
                    If q.PolicyUnderwritingAnswer <> "" Then
                        uwAnswered += 1
                    End If
                Next
                Select Case Me.Quote.LobId
                    Case 1
                        If uwAnswered >= 16 Then
                            Me.ctl_App_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
                        Else
                            ' Me.ValidationHelper.AddError("You must complete the Underwriting Question before proceeding.")
                        End If
                    Case Else

                End Select
            End If

        End If

    End Sub

    Private Sub ctlTreeView_ShowApplicationSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowApplicationSummary

        Me.ctl_App_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
    End Sub

    Private Sub ctlTreeView_ShowUnderwritingQuestions(sender As Object, e As EventArgs) Handles ctlTreeView.ShowUnderwritingQuestions
        Me.ctl_App_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
    End Sub

    Private Sub ctlTreeView_ShowFileUpload(sender As Object, e As EventArgs) Handles ctlTreeView.ShowFileUpload
        Me.ctl_App_Master_Edit.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "")
    End Sub



#End Region

#Region "Reports"
    Private Sub ctlTreeView_ViewDriverCreditReport(driverNumber As Integer) Handles ctlTreeView.ViewDriverCreditReport
        Dim Err As String = Nothing
        Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.GetCreditReport(Common.CreditReportSubject.Driver, Me.Quote, Err, driverNumber, True)
        If ReportFile IsNot Nothing Then
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("CreditReport_{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
            Response.BinaryWrite(ReportFile)
        Else
            Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
            _script.AddScriptLine("alert('" + Server.HtmlEncode(Err) + "');")
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
            Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
            _script.AddScriptLine("alert('" + Server.HtmlEncode(Err) + "');")
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
            Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
            _script.AddScriptLine("alert('No Loss Records Found.');")
            '_script.AddScriptLine("alert('" + Server.HtmlEncode(Err) + "');")
        End If
    End Sub



#End Region

End Class