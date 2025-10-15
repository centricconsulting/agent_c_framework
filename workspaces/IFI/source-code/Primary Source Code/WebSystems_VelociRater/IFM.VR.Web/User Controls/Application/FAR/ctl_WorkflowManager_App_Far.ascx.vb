Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers.FARM

Public Class ctl_WorkflowManager_App_Far
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
        Dim isRated As Boolean = False
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
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "")
            Case Else
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
        End Select

        If FarmClueHelper.IsFarClueAvailable(Quote) Then
            If HasRatedQuoteAvailable = False Then
                ' do Clue Report Lookup
                If Me.Quote IsNot Nothing Then
                    ctl_OrderClueAndOrMVR.LoadReport(Web.ctl_OrderClueAndOrMVR.ReportRequestType.clue)
                    'Else
                    '    Me.LockForm() ' Me.VRScript.AddScriptLine("DisableMainFormOnSaveRemoves();")
                    '    Me.VRScript.AddScriptLine("alert('This quote must be rated prior to entering the application process.');", True)

                End If

                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                    Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
                ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
                Else
                    Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                End If
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
            End If
        Else
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
    Private Sub HideAllControls()
        Me.ctlUWQuestionsFARM.Visible = False
        Me.ctl_AttachmentUpload.Visible = False
        Me.ctl_AppSection_Farm.Visible = False
        ctlQuoteSummary_Farm.Visible = False
        Me.ctlIRPM.Visible = False
    End Sub

    Public Overrides Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)
        Me.HideAllControls()

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.app
                Me.ctl_AppSection_Farm.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions
                Me.ctlUWQuestionsFARM.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctlQuoteSummary_Farm.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmIRPM
                Me.ctlIRPM.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                Me.ctl_AttachmentUpload.Visible = True
        End Select

        Me.CurrentWorkFlow = workflow
    End Sub
#End Region

#Region "Save Requests"
    Private Sub ctlUWQuestionsFARM_RequestNavigationToApplication(sender As Object, QuoteID As String) Handles ctlUWQuestionsFARM.RequestNavigationToApplication
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
    End Sub

    Private Sub ctl_AppSection_Farm_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_AppSection_Farm.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_AppSection_Farm)
    End Sub

    Private Sub ctlUWQuestionsFARM_SaveRequested(index As Integer, WhichControl As String) Handles ctlUWQuestionsFARM.SaveRequested
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
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

    'Private Sub ctlFarmPolicyCoverages_RequestNavigationToQuoteSummary() Handles ctlIRPM.ReqNavToQuoteSummary
    '    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "0")
    'End Sub
    Private Sub ctlFarmPolicyCoverages_RequestNavigationToQuoteSummary() Handles ctlIRPM.ReqNavToQuoteSummary
        If HasRatedQuoteAvailable Then
            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "0")
        Else
            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "0")
        End If

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

    Private Sub ctl_HOM_App_Section_App_Rate_ApplicationRatedSuccessfully() Handles ctl_AppSection_Farm.App_Rate_ApplicationRatedSuccessfully
        If DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()?.Success Then
            Me.ctlQuoteSummary_Farm.Populate() ' Matt A - 8/7/15
            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
        End If

    End Sub

    Private Sub ctl_HOM_App_Section_QuoteRated() Handles ctl_AppSection_Farm.QuoteRated
        'happens even if rating fails - any rate errors will be show from logic in the ctl_App_Rate control
        'RaiseEvent QuoteRated()
        ctlTreeView.RefreshRatedQuote()
    End Sub

    Protected Overrides Sub RateWasRequested()
        ControlsToValidate_Custom.Add(Me)
        Dim valArgs As New IFM.VR.Web.VRValidationArgs(If(Me.IsOnAppPage, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate, VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate))
        'Dim SaveType = If(Me.IsOnAppPage, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate, VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate) 'removed 8/15/2017... incorrect type... was mixing up ValidationType w/ QuickQuoteSaveType, but they have different enum values
        'validateEffectiveDate = True ' needed above in ValidateControl()
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(If(Me.IsOnAppPage, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate, VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate))))
        'validateEffectiveDate = False ' needed above in ValidateControl()
        'Me.Save_FireSaveEvent(True)
        If Me.ValidationSummmary.HasErrors() = False Then
            Dim saveErr As String = Nothing
            Dim loadErr As String = Nothing

            'Dim ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr, SaveType)
            'updated to hard-code QuickQuoteSaveType
            'Dim ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            'updated 2/18/2019
            Dim ratedQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
                'no rate
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Dim successfulEndorsementRate As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedEndorsementQuoteFromContext(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, qqEndorsementResults:=ratedQuote, errorMessage:=saveErr, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
            Else
                ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            End If

            ' Check for quote stop or kill - DM 8/30/2017
            If Me.Quote IsNot Nothing AndAlso (Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled OrElse Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled) Then
                IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Session)
            End If

            'IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId)
            'updated 2/18/2019
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
                VR.Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                VR.Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
            Else
                Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId, saveType:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
            End If

            ' set this per page life cycle cache with newest - 6-3-14
            If ratedQuote IsNot Nothing Then
                DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(False, ratedQuote) 'sets the rated quote cache
            Else
                ' you can't set a Nothing quote with this method you'll just have to let it find out for itself that the last rated quote was nothing - should never happen
                DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(True)
            End If

            If String.IsNullOrWhiteSpace(saveErr) = False Or String.IsNullOrWhiteSpace(loadErr) = False Then
                'failed
                If String.IsNullOrWhiteSpace(saveErr) = False Then
                    Me.ValidationHelper.AddError(saveErr)
                End If
                If String.IsNullOrWhiteSpace(loadErr) = False Then
                    Me.ValidationHelper.AddError(loadErr)
                End If

            Else
                ' did not fail to call service but may have validation Items
                If ratedQuote IsNot Nothing Then
                    WebHelper_Personal.GatherRatingErrorsAndWarnings(ratedQuote, Me.ValidationHelper)
                    If ratedQuote.Success Then
                        If ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped OrElse ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped Then
                            'stay where you are - don't show summary - stop message will be contained in validation messages
                        Else
                            ctlQuoteSummary_Farm.Populate()
                            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                            ctlTreeView.RefreshRatedQuote()
                        End If
                    Else
                        'stay where you are - probably coverages
                    End If

                End If
            End If
        End If
    End Sub

    Private Sub ctlCoverages_FAR_QuoteRateRequested() Handles ctlIRPM.RaiseReRate
        RateWasRequested()
    End Sub

    Private Sub ctlTreeView_ViewCluePropertyReport(sender As Object, e As EventArgs) Handles ctlTreeView.ViewCluePropertyReport
        Dim Err As String = Nothing
        Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.PERSONAL_HOME_GetCLUEReport(Me.Quote, Err, True)
        If ReportFile IsNot Nothing Then
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("ClueReport{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
            Response.BinaryWrite(ReportFile)
        Else
            Err = Err.Replace(vbCrLf, "\r\n") ' CAH B51631 convert VB error message to JS error message.
            Me.VRScript.AddScriptLine("alert('" + Server.HtmlEncode(Err) + "');")
        End If
    End Sub

End Class