Imports IFM.VR.Common.Workflow
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctl_WorkflowManager_App_CGL
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

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As Workflow.WorkflowSection)

        ctlTreeView.RefreshQuote()
        Select Case workflow
            Case Common.Workflow.Workflow.WorkflowSection.na
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.app
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
            Case Common.Workflow.Workflow.WorkflowSection.uwQuestions
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary

                If Me.ctlDisplayDiamondRatingErrors.ValidationHelper.HasErrros = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
                End If
                ctlTreeView.RefreshRatedQuote()

            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "")
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.farmIRPM
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmIRPM, "")
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.documentPrinting
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.documentPrinting, "")
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.drivers
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.drivers, "")  ' This enum used for the Print-Friendly summary MGB
                Exit Select
            Case Else
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")

        End Select

        If Me.HasRatedQuoteAvailable = False Then
            'need to app rate it ' usually put into this status by report ordering
            Dim saveErr As String = ""
            Dim loadErr As String = ""
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/15/2019; original logic in ELSE
                'no save
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=saveErr, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
            Else
                Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, saveErr, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            End If
        End If

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
    End Sub

    Private Sub HideAllControls()
        For Each ctl As VRControlBase In Me.ChildVrControls
            ctl.Visible = False
        Next
        Me.ctl_ReturnToQuoteSide.Visible = True 'always visible
    End Sub

    Public Overrides Sub SetCurrentWorkFlow(workflow As Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()
        Select Case workflow
            Case Common.Workflow.Workflow.WorkflowSection.app
                Me.ctl_AppSection_CGL.Visible = True
                wf_returnButton.Visible = True
            Case Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctl_CGL_QuoteSummary.Visible = True
                wf_returnButton.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions
                Me.ctlCommercialUWQuestionList.Visible = True
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                Me.ctl_AttachmentUpload.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.farmIRPM
                Me.ctlCommercial_IRPM.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.documentPrinting
                Me.ctlCommercial_DocPrint.Populate()
                Me.ctlCommercial_DocPrint.Visible = True
                wf_returnButton.Visible = False
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.drivers  ' This enum used for the Print-Friendly summary MGB
                Me.Ctl_CGL_PFSummary.Populate()
                Me.Ctl_CGL_PFSummary.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.na
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
                wf_returnButton.Visible = True
                Exit Select
        End Select

        Me.CurrentWorkFlow = workflow
    End Sub



    Protected Overrides Sub RateWasRequested()
        ControlsToValidate_Custom.Add(Me)
        Dim valArgs As New IFM.VR.Web.VRValidationArgs(If(Me.IsOnAppPage, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate, VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate))
        'Dim SaveType = If(Me.IsOnAppPage, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate, VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(If(Me.IsOnAppPage, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate, VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate))))
        RequestRate.RateWasRequested(Quote, Me, ctlTreeView, ValidationSummmary)
        'If Me.ValidationSummmary.HasErrors() = False Then
        '    Dim saveErr As String = Nothing
        '    Dim loadErr As String = Nothing

        '    'Dim ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr, SaveType)
        '    'updated to hard-code QuickQuoteSaveType
        '    'Dim ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr, QuickQuoteXML.QuickQuoteSaveType.AppGap)
        '    'updated 2/18/2019
        '    Dim ratedQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        '    If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
        '        'no rate
        '    ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
        '        Dim successfulEndorsementRate As Boolean = Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedEndorsementQuoteFromContext(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, qqEndorsementResults:=ratedQuote, errorMessage:=saveErr, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
        '    Else
        '        ratedQuote = Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr, QuickQuoteXML.QuickQuoteSaveType.AppGap)
        '    End If

        '    ' Check for quote stop or kill - DM 8/30/2017
        '    If Me.Quote IsNot Nothing AndAlso (Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled OrElse Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled) Then
        '        IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Session)
        '    End If

        '    'IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId)
        '    'updated 2/18/2019
        '    If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
        '        Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
        '    ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
        '        Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
        '    Else
        '        Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId, saveType:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
        '    End If

        '    ' set this per page life cycle cache with newest - 6-3-14
        '    If ratedQuote IsNot Nothing Then
        '        DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(False, ratedQuote) 'sets the rated quote cache
        '    Else
        '        ' you can't set a Nothing quote with this method you'll just have to let it find out for itself that the last rated quote was nothing - should never happen
        '        DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(True)
        '    End If

        '    If String.IsNullOrWhiteSpace(saveErr) = False Or String.IsNullOrWhiteSpace(loadErr) = False Then
        '        'failed
        '        If String.IsNullOrWhiteSpace(saveErr) = False Then
        '            Me.ValidationHelper.AddError(saveErr)
        '        End If
        '        If String.IsNullOrWhiteSpace(loadErr) = False Then
        '            Me.ValidationHelper.AddError(loadErr)
        '        End If

        '    Else
        '        ' did not fail to call service but may have validation Items
        '        ' Quote/App stopped update - CH 8/30/2017
        '        If ratedQuote IsNot Nothing Then
        '            WebHelper_Personal.GatherRatingErrorsAndWarnings(ratedQuote, Me.ValidationHelper)
        '            If ratedQuote.Success Then
        '                If ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped OrElse ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped Then
        '                    'stay where you are - don't show summary - stop message will be contained in validation messages
        '                Else
        '                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
        '                    ctlTreeView.RefreshRatedQuote()
        '                End If
        '            Else
        '                'stay where you are - probably coverages
        '            End If

        '        End If
        '    End If
        'End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Me.ValidationHelper.GroupName = "Policy Level"

        Dim valList = IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.PolicyLevelValidations.ValidatePolicyLevel(Me.Quote, valArgs.ValidationType)
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

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

#Region "Save Requests"
    Private Sub ctl_AppSection_CGL_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_AppSection_CGL.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_AppSection_CGL)
    End Sub

    Private Sub ctl_AppSection_CAP_App_Rate_ApplicationRatedSuccessfully() Handles ctl_AppSection_CGL.App_Rate_ApplicationRatedSuccessfully
        If DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()?.Success Then
            Me.ctl_CGL_QuoteSummary.Populate()
            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
        End If
    End Sub

    Private Sub ctl_AppSection_CAP_QuoteRated() Handles ctl_AppSection_CGL.QuoteRated
        ctlTreeView.RefreshRatedQuote()
    End Sub


#End Region


#Region "Tree Events"
    Private Sub ctlTreeView_ShowApplication(sender As Object, e As EventArgs) Handles ctlTreeView.ShowApplication
        Me.SetCurrentWorkFlow(Workflow.WorkflowSection.app, "")
    End Sub

    Private Sub ctlTreeView_ShowUnderwritingQuestions(sender As Object, e As EventArgs) Handles ctlTreeView.ShowUnderwritingQuestions
        Me.SetCurrentWorkFlow(Workflow.WorkflowSection.uwQuestions, "")
    End Sub

    Private Sub ctlTreeView_ShowApplicationSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowApplicationSummary
        Me.SetCurrentWorkFlow(Workflow.WorkflowSection.summary, "")
    End Sub
    Private Sub ctlTreeView_ShowFileUpload(sender As Object, e As EventArgs) Handles ctlTreeView.ShowFileUpload
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "")
    End Sub
    Private Sub ctlTreeView_ShowIRPM(sender As Object, e As EventArgs) Handles ctlTreeView.ShowIRPM
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmIRPM, "")
    End Sub

    Private Sub ctlTreeView_ShowAppSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowApplicationSummary
        SetCurrentWorkFlow(Workflow.WorkflowSection.summary, "")
    End Sub
#End Region



End Class