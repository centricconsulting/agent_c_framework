Imports IFM.VR.Common.Workflow
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctl_WorkflowManager_App_BOP
    Inherits VRMasterControlBase

    Public Event QuoteRated(qq As QuickQuote.CommonObjects.QuickQuoteObject)

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
                Exit Select
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
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "")
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.farmIRPM
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmIRPM, "")
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.documentPrinting
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.documentPrinting, "")
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.printFriendly 'Updated 09/21/2021 for BOP Endorsements Task 61506 MLW, changed from drivers to printFriendly
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.printFriendly, "")
                Exit Select
            Case Else
                Exit Select
        End Select

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            DirectCast(Me.Page, BasePage).DirtyFormDiv = "divAppEditControls"
            Me.Populate()
        End If
    End Sub


    Dim validateEffectiveDate As Boolean = False

    Protected Overrides Sub RateWasRequested()
        ControlsToValidate_Custom.Add(Me)
        Dim valArgs As New IFM.VR.Web.VRValidationArgs(If(Me.IsOnAppPage, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate, VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate))
        'Dim SaveType = If(Me.IsOnAppPage, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate, VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate) 'removed 8/15/2017... incorrect type... was mixing up ValidationType w/ QuickQuoteSaveType, but they have different enum values
        'validateEffectiveDate = True ' needed above in ValidateControl()
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(If(Me.IsOnAppPage, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate, VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate))))
        'validateEffectiveDate = False ' needed above in ValidateControl()
        'Me.Save_FireSaveEvent(True)
        RequestRate.RateWasRequested(Quote, Me, ctlTreeView, ValidationSummmary, Me.ctl_BOP_QuoteSummary)
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
        '        Dim successfulEndorsementRate As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedEndorsementQuoteFromContext(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, qqEndorsementResults:=ratedQuote, errorMessage:=saveErr, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
        '    Else
        '        ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr, QuickQuoteXML.QuickQuoteSaveType.AppGap)
        '    End If

        '    ' Check for quote stop or kill - DM 8/30/2017
        '    If Me.Quote IsNot Nothing AndAlso (Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled OrElse Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled) Then
        '        IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Session)
        '    End If

        '    'IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId)
        '    'updated 2/18/2019
        '    If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
        '        VR.Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
        '    ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
        '        VR.Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
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
        '        If ratedQuote IsNot Nothing Then
        '            WebHelper_Personal.GatherRatingErrorsAndWarnings(ratedQuote, Me.ValidationHelper)
        '            If ratedQuote.Success Then
        '                If ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped OrElse ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped Then
        '                    'stay where you are - don't show summary - stop message will be contained in validation messages
        '                Else
        '                    ctl_BOP_QuoteSummary.Populate()
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

    'added 9/22/2017
    Private _IsSaving As Boolean
    Private _NeedsToRepopulateLocationAndContractorsEquipmentLists As Boolean

    Public Overrides Function Save() As Boolean
        _IsSaving = True 'added 9/22/2017
        Me.SaveChildControls()
        _IsSaving = False 'added 9/22/2017
        If _NeedsToRepopulateLocationAndContractorsEquipmentLists = True Then 'added 9/22/2017
            _NeedsToRepopulateLocationAndContractorsEquipmentLists = False
            Me.ctl_AppSection_BOP.PopulateLocationAndContractorsEquipmentLists()
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Me.ValidateChildControls(valArgs)

        Me.ValidationHelper.GroupName = "Application Rate"
        MyBase.ValidateControl(valArgs)

        If validateEffectiveDate Then ' you check this because you only want to validate if it is coming from the btnFinalRate_Click() below - all other times you do no validate the effective date because it isn't even on the screen
            Dim valList = IFM.VR.Validation.ObjectValidation.PolicyLevelValidator.PolicyValidation(Me.Quote, Me.DefaultValidationType)
            If valList.Any() Then
                For Each v In valList
                    Select Case v.FieldId
                        Case IFM.VR.Validation.ObjectValidation.PolicyLevelValidator.EffectiveDate
                            Me.ValidationHelper.AddError(v.Message)
                    End Select
                Next
            End If

            Dim valList2 = IFM.VR.Validation.ObjectValidation.CommLines.LOB.BOP.PolicyLevelValidations.ValidatePolicyLevel(Me.Quote, valArgs.ValidationType)
            If valList2.Any() Then
                For Each v In valList2
                    If v.IsWarning Then
                        Me.ValidationHelper.AddWarning(v.Message)
                    Else
                        Me.ValidationHelper.AddError(v.Message)
                    End If
                Next
            End If

        End If
    End Sub


#Region "Workflows"
    Private Sub HideAllControls()
        Me.ctlCommercialUWQuestionList.Visible = False
        Me.ctl_AttachmentUpload.Visible = False
        Me.ctl_AppSection_BOP.Visible = False
        Me.ctlCommercial_IRPM.Visible = False
        'Me.ctl_AppSection_BOP.Visible = False
        Me.ctlCommercial_DocPrint.Visible = False
        ctl_BOP_QuoteSummary.Visible = False
        Ctl_PF_BOPQuoteSummary.Visible = False
    End Sub

    Public Overrides Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)
        Me.HideAllControls()

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.app
                Me.ctl_AppSection_BOP.Visible = True
                wf_returnButton.Visible = True
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctl_BOP_QuoteSummary.Populate()
                Me.ctl_BOP_QuoteSummary.Visible = True
                wf_returnButton.Visible = True
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                Me.ctl_AttachmentUpload.Visible = True
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions
                Me.ctlCommercialUWQuestionList.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.farmIRPM
                Me.ctlCommercial_IRPM.Visible = True
                Me.ctlCommercial_IRPM.Populate()
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.documentPrinting
                Me.ctlCommercial_DocPrint.Populate()
                Me.ctlCommercial_DocPrint.Visible = True
                wf_returnButton.Visible = False
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.printFriendly 'Updated 09/21/2021 for BOP Endorsements Task 61506 MLW, changed from drivers to printFriendly
                Me.Ctl_PF_BOPQuoteSummary.Populate()
                Me.Ctl_PF_BOPQuoteSummary.Visible = True
                Exit Select
        End Select

        Me.CurrentWorkFlow = workflow
    End Sub



    Private Sub ctl_WorkflowManager_App_BOP_BroadcastWorkflowChangeRequestEvent(type As Workflow.WorkflowSection, subworkflowParm As String) Handles Me.BroadcastWorkflowChangeRequestEvent
        Select Case type
            Case Workflow.WorkflowSection.summary
                ' *****  Goto Summary
                'SetCurrentWorkFlow(type, subworkflowParm)
        End Select
    End Sub
#End Region

#Region "Save Requests"
    'Private Sub ctl_AppSection_Farm_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_AppSection_Farm.SaveRequested
    '    Me.ControlsToValidate_Custom.Add(Me.ctl_AppSection_Farm)
    'End Sub

    'Private Sub ctlCommercialUWQuestionList_SaveRequested(index As Integer, WhichControl As String) Handles ctlCommercialUWQuestionList.SaveRequested
    '    Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    'End Sub

    'Private Sub ctlCommercialUWQuestionList_RequestNavigationToApplication(sender As Object, QuoteID As String) Handles ctlCommercialUWQuestionList.RequestNavigationToApplication
    '    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
    'End Sub

#End Region

#Region "Tree Events"

    Private Sub ctlTreeView_ShowApplication(sender As Object, e As EventArgs) Handles ctlTreeView.ShowApplication
        ' have the UW questions been answered
        If Me.Quote IsNot Nothing Then
            ' First, are any of the existing policyunderwritings on the quote unanswered?
            Dim Unanswered As Boolean = False
            If Me.Quote.PolicyUnderwritings IsNot Nothing Then
                For Each q In Me.Quote.PolicyUnderwritings
                    If q.PolicyUnderwritingAnswer.Trim = "" Then
                        Unanswered = True
                        Exit For
                    End If
                Next
                If Unanswered Then
                    ' If any existing questions are unanswered throw the validation
                    Me.ValidationHelper.AddError("You must complete the Underwriting Question before proceeding.")
                Else
                    ' All existing questions are answered.
                    ' Are there more questions than just the initial kill questions?
                    Select Case Quote.LobType
                        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                            If Quote.PolicyUnderwritings IsNot Nothing AndAlso Quote.PolicyUnderwritings.Count > 6 Then ' BOP has 6 kill questions...
                                ' More than the kill questions, proceed to app
                                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
                            Else
                                ' Only the kill questions are answered, throw the validation
                                Me.ValidationHelper.AddError("You must complete the Underwriting Question before proceeding.")
                            End If
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

    Private Sub ctl_AppSection_BOP_App_Rate_ApplicationRatedSuccessfully() Handles ctl_AppSection_BOP.App_Rate_ApplicationRatedSuccessfully
        If DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()?.Success Then
            Me.ctl_BOP_QuoteSummary.Populate()
            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
        End If

    End Sub

    Private Sub ctl_AppSection_BOP_QuoteRated() Handles ctl_AppSection_BOP.QuoteRated
        'happens even if rating fails - any rate errors will be show from logic in the ctl_App_Rate control
        'RaiseEvent QuoteRated()
        ctlTreeView.RefreshRatedQuote()
    End Sub

    Private Sub ctl_AppSection_BOP_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_AppSection_BOP.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_AppSection_BOP)
        Me.ControlsToValidate_Custom.Add(Me.ctlCommercialUWQuestionList)
        Me.ControlsToValidate_Custom.Add(Me.ctlCommercial_IRPM)
    End Sub

    Private Sub ctl_AppSection_BOP_AIChanged() Handles ctl_AppSection_BOP.AIChanged 'added 9/22/2017
        If _IsSaving = True Then
            _NeedsToRepopulateLocationAndContractorsEquipmentLists = True
        Else
            _NeedsToRepopulateLocationAndContractorsEquipmentLists = False
            'this logic was previously in ctl_AppSection_BOP.HandleAIChange; not sure if Save is needed, but this likely won't get hit since the AI updated events should fire during save, and populate will happen after if needed (see Save code above)
            Save_FireSaveEvent(False)
            Me.ctl_AppSection_BOP.PopulateLocationAndContractorsEquipmentLists()
        End If
    End Sub
End Class