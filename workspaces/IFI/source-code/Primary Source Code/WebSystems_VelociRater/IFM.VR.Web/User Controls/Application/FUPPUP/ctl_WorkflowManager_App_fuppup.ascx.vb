Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers

Public Class ctl_WorkflowManager_App_fuppup
    Inherits VRMasterControlBase

    'Public Event QuoteRated()

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
            Case Common.Workflow.Workflow.WorkflowSection.printFriendlySummary
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.printFriendlySummary, "")
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "")
            Case Else
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
        End Select

        If Me.HasRatedQuoteAvailable = False Then
            'need to app rate it ' usually put into this status by report ordering
            Dim saveErr As String = ""
            Dim loadErr As String = ""
            'Todo Matt A - Why does it do this save?
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/15/2019; original logic in ELSE
                'no Save needed
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=saveErr, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
            Else
                Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, saveErr, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            End If
            'ratedQuote = Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr, QuickQuoteXML.QuickQuoteSaveType.AppGap)

            'If Not (ratedQuote IsNot Nothing AndAlso ratedQuote.Success) Then
            '    Me.LockForm()
            '    Me.VRScript.AddScriptLine("alert('This quote must be rated prior to entering the application process.');", True)

            '    Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            '    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
            'End If

        End If

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        'Me.ctlUWQuestionsFARM.
        Me.PopulateChildControls()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Me.ValidationHelper.GroupName = "Policy Level"

        Dim valList = IFM.VR.Validation.ObjectValidation.FarmLines.PolicyLevelValidations.ValidatePolicyLevel(Me.Quote, valArgs.ValidationType)
        If valList.Any() Then
            For Each v In valList
                If v.IsWarning Then
                    Me.ValidationHelper.AddWarning(v.Message)
                Else
                    Me.ValidationHelper.AddError(v.Message)
                End If
            Next
        End If
    End Sub

#Region "Workflows"
    Public Sub HideAllControls()
        For Each cntrl As VRControlBase In Me.ChildVrControls
            cntrl.Visible = False
        Next
        ctl_ReturnToQuoteSide.Visible = True
    End Sub

    Public Overrides Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)
        Me.HideAllControls()

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.app
                Me.ctl_AppSection_fuppup.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctl_FUPPUP_QuoteSummary.Visible = True
            Case Common.Workflow.Workflow.WorkflowSection.printFriendlySummary
                Me.ctl_FUPPUP_PFSummary.Populate()
                Me.ctl_FUPPUP_PFSummary.Visible = True
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                Me.ctl_AttachmentUpload.Visible = True
        End Select

        Me.CurrentWorkFlow = workflow
    End Sub
#End Region

#Region "Save Requests"
    ''Private Sub ctlUWQuestionsFARM_RequestNavigationToApplication(sender As Object, QuoteID As String) Handles ctlUWQuestionsFARM.RequestNavigationToApplication
    ''    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
    ''End Sub

    ''Private Sub ctl_AppSection_Farm_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_AppSection_Farm.SaveRequested
    ''    Me.ControlsToValidate_Custom.Add(Me.ctl_AppSection_Farm)
    ''End Sub

    ''Private Sub ctlUWQuestionsFARM_SaveRequested(index As Integer, WhichControl As String) Handles ctlUWQuestionsFARM.SaveRequested
    ''    Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    ''End Sub

    Private Sub ctl_AppSection_Farm_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_AppSection_fuppup.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_AppSection_fuppup)
    End Sub
#End Region

#Region "Tree Events"

    Private Sub ctlTreeView_ShowApplication(sender As Object, e As EventArgs) Handles ctlTreeView.ShowApplication
        ' have the UW questions been answered
        If Me.Quote IsNot Nothing Then
            Dim uwAnswered As Int32 = 0
            'Updated 9/10/2018 for multi state MLW
            If SubQuoteFirst IsNot Nothing Then
                If SubQuoteFirst.PolicyUnderwritings IsNot Nothing Then
                    For Each q In SubQuoteFirst.PolicyUnderwritings
                        If q.PolicyUnderwritingAnswer <> "" Then
                            uwAnswered += 1
                        End If
                    Next
                    Select Case Me.Quote.LobId
                        Case 17
                            If uwAnswered >= 24 Then
                                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
                            Else
                                Me.ValidationHelper.AddError("You must complete the Underwriting Question before proceeding.")
                            End If
                        Case Else

                    End Select
                End If
            End If

        End If

    End Sub

    Private Sub ctlTreeView_ShowApplicationSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowApplicationSummary
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
    End Sub

    Private Sub ctlTreeView_ShowUnderwritingQuestions(sender As Object, e As EventArgs) Handles ctlTreeView.ShowUnderwritingQuestions
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
    End Sub

    Private Sub ctlTreeView_ShowIRPM(sender As Object, e As EventArgs) Handles ctlTreeView.ShowIRPM
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmIRPM, "")
    End Sub

    Private Sub ctlTreeView_ShowFileUpload(sender As Object, e As EventArgs) Handles ctlTreeView.ShowFileUpload
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "")
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
#End Region

    Private Sub ctl_HOM_App_Section_App_Rate_ApplicationRatedSuccessfully() Handles ctl_AppSection_fuppup.App_Rate_ApplicationRatedSuccessfully
        If DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()?.Success Then
            Me.ctl_FUPPUP_QuoteSummary.Populate() ' Matt A - 8/7/15
            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
        End If

    End Sub

    'Private Sub ctl_HOM_App_Section_QuoteRated() Handles ctl_AppSection_Farm.QuoteRated
    '    'happens even if rating fails - any rate errors will be show from logic in the ctl_App_Rate control
    '    'RaiseEvent QuoteRated()
    '    ctlTreeView.RefreshRatedQuote()
    'End Sub

    Protected Overrides Sub RateWasRequested()

    End Sub


End Class