
Imports IFM.VR.Common.Helpers.DFR
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctl_WorkFlowManager_DFR_App
    Inherits VRMasterControlBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            DirectCast(Me.Page, BasePage).DirtyFormDiv = "divAppEditControls"
            Me.Populate()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddScriptLine("$(""#divAppEditControls"").fadeIn('fast');")
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection)
        ctlTreeView.RefreshQuote()
        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.app
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")

            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                If Me.ctlDisplayDiamondRatingErrors.ValidationHelper.HasErrros = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
                End If
                ctlTreeView.RefreshRatedQuote()

            Case Else
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
        End Select

        If Me.HasRatedQuoteAvailable = False Then
            If Me.Quote IsNot Nothing Then
                ctl_OrderClueAndOrMVR.LoadReport(Web.ctl_OrderClueAndOrMVR.ReportRequestType.clue)
            Else
                Me.LockForm() ' Me.VRScript.AddScriptLine("DisableMainFormOnSaveRemoves();")
                Me.VRScript.AddScriptLine("alert('This quote must be rated prior to entering the application process.');", True)

            End If
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
            Else
                Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            End If
            ctlTreeView.RefreshQuote()
            ctlTreeView.RefreshRatedQuote()
            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
        End If

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
    End Sub

    Private Sub HideAllControls()
        Me.ctlUWQuestions.Visible = False
        Me.ctl_UnderwritingQuestionsByLob.Visible = False 'added 8/25/2022 KLJ for 75690
        Me.ctl_DFR_AppSection.Visible = False
        Me.ctlQuoteSummary_DFR.Visible = False
        Me.ctl_AttachmentUpload.Visible = False
    End Sub

    Public Overrides Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)
        Me.HideAllControls()

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.app
                Me.ctl_DFR_AppSection.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions
                'added 8/25/2022 KLJ for 75690
                If DFRStandaloneHelper.isDFRStandaloneAvailable(Me.Quote) Then
                    Me.ctl_UnderwritingQuestionsByLob.Populate()
                    Me.ctl_UnderwritingQuestionsByLob.Visible = True
                Else
                    Me.ctlUWQuestions.Visible = True
                End If
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctlQuoteSummary_DFR.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                Me.ctl_AttachmentUpload.Visible = True
            Case Else
                Me.ctl_DFR_AppSection.Visible = True
        End Select

        Me.CurrentWorkFlow = workflow
    End Sub

    Protected Overrides Sub RateWasRequested()

    End Sub

    'Added 8/1/2022 for task 75911 MLW
    Public Sub UpdateUWQuestion() Handles ctl_DFR_AppSection.UpdatesUWQuestions
        ctlUWQuestions.InitializeQuestions()
    End Sub



#Region "Save Requests"
    Private Sub ctlUWQuestions_SaveRequested(index As Integer, WhichControl As String) Handles ctlUWQuestions.SaveRequested
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub
    Private Sub ctl_UnderwritingQuestionsByLob_UnderwritingQuestionsSaved(ByVal sender As Object, ByVal QuoteID As String) Handles ctl_UnderwritingQuestionsByLob.UnderwritingQuestionsSaved 'modified 8/25/2022 KLJ for 75690
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub
    Private Sub ctlUWQuestions_RequestNavigationToApplication(sender As Object, QuoteID As String) Handles ctlUWQuestions.RequestNavigationToApplication, ctl_UnderwritingQuestionsByLob.RequestNavigationToApplication 'modified 8/25/2022 KLJ for 75690
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
    End Sub

    Private Sub ctlUWQuestions_RequestNavigationToApplication_Endorsements(sender As Object, PolicyId As Integer, PolicyImageNum As Integer) Handles ctlUWQuestions.RequestNavigationToApplication_Endorsements, ctl_UnderwritingQuestionsByLob.RequestNavigationToApplication_Endorsements 'modified 8/25/2022 KLJ for 75690 'added 2/28/2019
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
    End Sub

    Private Sub ctlUWQuestions_RequestNavigationToApplication_ReadOnly(sender As Object, PolicyId As Integer, PolicyImageNum As Integer) Handles ctlUWQuestions.RequestNavigationToApplication_ReadOnly, ctl_UnderwritingQuestionsByLob.RequestNavigationToApplication_ReadOnly 'modified 8/25/2022 KLJ for 75690 'added 2/28/2019
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
    End Sub

    Private Sub ctl_DFR_AppSection_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_DFR_AppSection.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_DFR_AppSection)
    End Sub


#End Region

#Region "Tree Navigations"
    Private Sub ctlTreeView_ShowUnderwritingQuestions(sender As Object, e As EventArgs) Handles ctlTreeView.ShowUnderwritingQuestions
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
    End Sub

    Private Sub ctlTreeView_ShowApplication(sender As Object, e As EventArgs) Handles ctlTreeView.ShowApplication
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
    End Sub

    Private Sub ctlTreeView_ShowApplicationSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowApplicationSummary
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
    End Sub

    Private Sub ctlTreeView_ShowFileUpload(sender As Object, e As EventArgs) Handles ctlTreeView.ShowFileUpload
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "")
    End Sub


#End Region

#Region "Reports"
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
#End Region


    Private Sub ctl_DFR_App_Section_App_Rate_ApplicationRatedSuccessfully() Handles ctl_DFR_AppSection.App_Rate_ApplicationRatedSuccessfully
        If DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()?.Success Then
            Me.ctlQuoteSummary_DFR.Populate()
            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
        End If

    End Sub

    Private Sub ctl_DFR_App_Section_QuoteRated() Handles ctl_DFR_AppSection.QuoteRated
        'happens even if rating fails - any rate errors will be show from logic in the ctl_App_Rate control
        ctlTreeView.RefreshRatedQuote()
    End Sub


End Class