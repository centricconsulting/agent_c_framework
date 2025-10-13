Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctl_WorkflowManager_Billing_update
    Inherits VRMasterControlBase

    'Added 7/24/2019 for Auto Endorsements MLW

    Public Event QuoteRated()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 5/21/2019; original logic in ELSE
                'nothing to do for ReadOnly
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Session(Me.EndorsementPolicyIdAndImageNum & "_HasNoHitMVR") = Nothing
            Else
                Session(Me.QuoteId + "_HasNoHitMVR") = Nothing '9-11-14 Matt A
            End If
        End If
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection)
        Select Case workflow
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Dim ratedQuote = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()
                If ratedQuote IsNot Nothing Then
                    If ratedQuote.Success Then
                        SetCurrentWorkFlow(workflow, "0")
                    Else
                        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
                    End If
                End If
            Case Else
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
        End Select
        RefreshBillingInfo()
    End Sub

    Public Overrides Sub AddScriptAlways()
        If Me.Quote IsNot Nothing Then
            Me.VRScript.AddVariableLine("var effectiveDate = new Date('" + Me.Quote.EffectiveDate + "');")
        End If
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddScriptLine("$('#BillingUpdate').fadeIn('fast');", True)
        Me.VRScript.AddScriptLine("ifm.vr.theming.LoadThemeFromCookie(""PayplanChange"");")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.ctlBillingInfo.Populate()
    End Sub

    Public Overrides Function Save() As Boolean
        Select Case Me.CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                'do nothing
            Case Else
                Me.ctlBillingInfo.Save()
        End Select
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ctlEffectiveDateChecker.ValidateControl(valArgs) ' Matt A 9-29-14
        Me.ctlBillingInfo.ValidateControl(valArgs)
    End Sub

#Region "Saves"

    Public Overrides Sub AfterSaveOccurrs(args As VrControlBaseSaveEventArgs)
        MyBase.AfterSaveOccurrs(args)
        Select Case Me.CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation
                RefreshBillingInfo()
        End Select
    End Sub

#End Region

#Region "Show Work flows"

    Public Overrides Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()
        Me.VRScript.AddVariableLine("var DirtyFormException = false;")
        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctlBillingChangeSummary.Visible = True
                Me.ctlBillingChangeSummary.Populate()
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation
                Me.ctlBillingInfo.Visible = True
                Me.ctlBillingInfo.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                Me.ctl_AttachmentUpload.Visible = True
        End Select
        Me.CurrentWorkFlow = workflow
    End Sub

    Public Sub HideAllControls()
        Me.ctlBillingInfo.Visible = False
        Me.ctlBillingChangeSummary.Visible = False
        Me.ctl_AttachmentUpload.Visible = False
    End Sub

#End Region

#Region "Refreshes"
    Public Sub RefreshBillingInfo()
        Me.ctlBillingInfo.Populate()
    End Sub
#End Region

    Public Sub TreeChangedQuote()
        Me.ctlBillingInfo.Populate()
    End Sub

    'Updated to include ctlBillingChangeSummary 05/18/2020 for Bug 43588 MLW
    Private Sub ctlBillingInfo_QuoteRated() Handles ctlBillingInfo.QuoteRateRequested, ctlBillingChangeSummary.QuoteRateRequested
        RateWasRequested()
    End Sub

    Protected Overrides Sub RateWasRequested()
        ControlsToValidate_Custom.Add(Me)
        Dim valArgs As New VRValidationArgs(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, valArgs))
        If Me.ValidationSummmary.HasErrors = False Then
            ' do rate
            Dim saveErr As String = Nothing
            Dim loadErr As String = Nothing

            'check for AIs that won't pass validation
            IFM.VR.Common.Helpers.AdditionalInterest.RemoveIncompleteAis_Auto(Me.Quote)

            ' do a auto symbol - VIN verify
            For Each va In IFM.VR.Common.Helpers.PPA.AutoSymbolHelpers.RescanAllAutoSymbols(Me.Quote)
                If va.IsWarning Then
                    Me.ValidationHelper.AddWarning(va.Message)
                Else
                    Me.ValidationHelper.AddError(va.Message)
                End If
            Next

            'manually save
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/15/2019; original logic in ELSE
                'no save
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=saveErr)
            Else
                VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, saveErr, loadErr) '2/15/2019 note: last param (loadErr) is not the correct type for the method (QuickQuoteXml.QuickQuoteSaveType)
            End If

            If Me.ValidationSummmary.HasErrors = False Then
                'Dim ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr)
                'updated 2/15/2019
                Dim ratedQuote As QuickQuoteObject = Nothing
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
                    'no rate
                ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    Dim successfulEndorsementRate As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedEndorsementQuoteFromContext(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, qqEndorsementResults:=ratedQuote, errorMessage:=saveErr)
                Else
                    ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr)
                End If

                ' Check for quote kill - DM 8/30/2017
                If Me.Quote IsNot Nothing AndAlso (Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled OrElse Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled) Then
                    IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Session)
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

                        'RaiseEvent QuoteRated() ' always fire so tree gets even attempt rates 4-14-14
                        'If ratedQuote.Success Then
                        '    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                        'Else
                        '    'stay where you are - probably coverages
                        'End If

                        If ratedQuote.Success Then
                            If ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped OrElse ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped Then
                                'stay where you are - don't show summary - stop message will be contained in validation messages
                            Else
                                RaiseEvent QuoteRated()
                                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                            End If
                        Else
                            'stay where you are - probably coverages
                        End If
                    End If
                End If
            End If
        End If
    End Sub

End Class