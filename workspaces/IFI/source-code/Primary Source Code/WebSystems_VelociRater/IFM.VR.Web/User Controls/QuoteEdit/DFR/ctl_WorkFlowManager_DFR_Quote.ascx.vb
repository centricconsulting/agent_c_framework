Imports IFM.VR.Common.QuoteSearch
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects

Public Class ctl_WorkFlowManager_DFR_Quote
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
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "")
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                If Me.ctlDisplayDiamondRatingErrors.ValidationHelper.HasErrros = False Then

                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "")
                End If
                ctlTreeView.RefreshRatedQuote()
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation 'added 9/18/2019 for DFR Endorsements task 40278 MLW
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Common.Workflow.Workflow.WorkflowSection.printHistory 'added 9/18/2019 for DFR Endorsements task 40279 MLW
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.printHistory, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Common.Workflow.Workflow.WorkflowSection.policyHistory 'added 9/18/2019 for DFR Endorsements task 40280 MLW
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Else
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
        End Select
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
    End Sub

    Public Overrides Function Save() As Boolean
        Select Case CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                Me.ctlIsuredList.Save()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_
                Me.ctlProperty_HOM.Save()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                ctlDFRResidenceCoverages.Save()
                ctlDFRResidenceCoverages.Populate()
            Case Else
                SaveChildControls()
        End Select
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Sub HideAllControls()
        For Each cntrl As VRControlBase In Me.ChildVrControls
            cntrl.Visible = False
        Next
    End Sub

    Public Overrides Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                If Me.ctlIsuredList.Visible = False Then ' just in case but only if it wasn't already visible otherwise it might wipe data
                    Me.ctlIsuredList.Populate()
                End If
                Me.ctlIsuredList.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_
                Me.ctlProperty_HOM.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                ctlDFRResidenceCoverages.Visible = True
                ctlDFRResidenceCoverages.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                ctlQuoteSummary_DFR.Visible = True
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation 'Added 09/18/2019 for DFR Endorsements task 40278 MLW
                Me.ctlBillingInfo.Visible = True
                Me.ctlBillingInfo.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.printHistory 'Added 09/18/2019 for DFR Endorsements task 40279 MLW
                Me.ctlPrintHistory.Visible = True
                Me.ctlPrintHistory.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory 'Added 09/18/2019 for DFR Endorsements task 40280 MLW
                Me.ctlPolicyHistory.Visible = True
                Me.ctlPolicyHistory.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload 'Added 10/04/2019 for DFR Endorsement task 40275 MLW
                Me.ctl_AttachmentUpload.Visible = True
        End Select
        Me.CurrentWorkFlow = workflow
    End Sub

    Private Sub CallBetterViewPreLoad()
        Dim ih As New IntegrationHelper
        ih.CallBetterViewPreLoad(Me.Quote)
    End Sub

    Private Sub ctlDFRResidenceCoverages_QuoteRateRequested() Handles ctlDFRResidenceCoverages.QuoteRateRequested
        RateWasRequested()
    End Sub

    Protected Overrides Sub RateWasRequested()
        ControlsToValidate_Custom.Add(Me)
        Dim valArgs As New VRValidationArgs(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
        Me.Save_FireSaveEvent(True)
        PreRateEvents()
        If Me.ValidationSummmary.HasErrors() = False Then
            'good to rate
            ' do rate
            Dim saveErr As String = Nothing
            Dim loadErr As String = Nothing

            'Dim ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr)
            'updated 2/18/2019
            Dim ratedQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
                'no rate
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Dim successfulEndorsementRate As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedEndorsementQuoteFromContext(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, qqEndorsementResults:=ratedQuote, errorMessage:=saveErr)
            Else
                ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr)
            End If

            ' Check for quote stop or kill - DM 8/30/2017
            If Me.Quote IsNot Nothing AndAlso (Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled OrElse Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled) Then
                IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Session)
            End If

            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                VR.Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                VR.Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
            Else
                IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId)
            End If

            ctlProperty_HOM.Populate()

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
                    ''RaiseEvent QuoteRated(ratedQuote) ' always fire so tree gets even attempt rates 4-14-14
                    'If ratedQuote.Success Then
                    '    ctlQuoteSummary_DFR.Populate()
                    '    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                    '    ctlTreeView.RefreshRatedQuote()
                    'Else
                    '    'stay where you are - probably coverages
                    'End If

                    If ratedQuote.Success Then
                        If ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped OrElse ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped Then
                            'stay where you are - don't show summary - stop message will be contained in validation messages
                        Else
                            ctlQuoteSummary_DFR.Populate()
                            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                            ctlTreeView.RefreshRatedQuote()
                        End If
                    Else
                        'stay where you are - probably coverages
                        'Added 05/18/2020 for Bug 46773 MLW
                        'If String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                        'updated 11/20/2020 (Interoperability) to show Route option on NewBusinessQuoting; note: may not need to check for Interoperability since the VR validation would've already been removed
                        If Me.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse (Me.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote AndAlso QuickQuote.CommonMethods.QuickQuoteHelperClass.AllowVRToUpdateFromDiamond_Interoperability(QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal) = True) Then
                            ctlDFRResidenceCoverages.RouteToUwIsVisible = True
                        End If
                    End If

                End If
            End If
        End If
    End Sub


#Region "Navigations"
    Private Sub ctlTreeView_ShowPolicyholders(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyholders
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "")
    End Sub

    Private Sub ctlTreeView_ShowResidence(sender As Object, e As EventArgs) Handles ctlTreeView.ShowResidence
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_, "")
    End Sub

    Private Sub ctlTreeView_ShowCoverages(sender As Object, e As EventArgs) Handles ctlTreeView.ShowCoverages
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "")
    End Sub

    Private Sub ctlTreeView_ShowQuoteSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowQuoteSummary
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
    End Sub

    'Added 09/18/2019 for DFR Endorsements task 40282 MLW
    Private Sub ctlTreeView_ShowBillingInformation(sender As Object, e As EventArgs) Handles ctlTreeView.ShowBillingInformation
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub
    'Added 09/18/2019 for DFR Endorsements task 40282 MLW
    Private Sub ctlTreeView_ShowPrintHistory(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPrintHistory
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.printHistory, "0")
    End Sub
    'Added 09/18/2019 for DFR Endorsements task 40282 MLW
    Private Sub ctlTreeView_ShowPolicyHistory(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyHistory
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
    End Sub
    'Added 09/18/2019 for DFR Endorsements task 40282 MLW
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
    'Added 09/20/2019 for DFR Endorsements task 40275 MLW
    Private Sub ctlTreeView_ShowFileUpload(sender As Object, e As EventArgs) Handles ctlTreeView.ShowFileUpload
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "0")
    End Sub
#End Region



#Region "Save Events"
    Public Overrides Sub AfterSaveOccurrs(args As VrControlBaseSaveEventArgs)
        Dim qqXML As New QuickQuote.CommonMethods.QuickQuoteXML()
        Dim reload As Boolean = False
        Dim success As Boolean = False
        Dim reportsOrdered As Boolean = False
        Dim manuallyOrdered As Boolean = False
        Dim manuallyFound As Boolean = False
        Dim problemWithDiamondAddressMatching As Boolean = False

        MyBase.AfterSaveOccurrs(args)

        Select Case CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_
                Me.ctlProperty_HOM.Populate() ' updates the address section with possible new data from the policyholder section

                ' Added 10/10/16 for PCC (MGB)
                If Me.Quote Is Nothing OrElse (Me.Quote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso Me.Quote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage) Then 'added IF 8/2/2019 to only order PPC report for new business quoting; was previously happening every time
                    qqXML.CheckQuoteForProtectionClassInfoAndOrderVeriskReportIfNeeded(Me.Quote, reload, success, manuallyOrdered, manuallyFound, problemWithDiamondAddressMatching)
                    CallBetterViewPreLoad()
                    If reload Then
                        'IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(QuoteId)
                        Me.ctlProperty_HOM.Populate()
                    End If
                End If

                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                Me.ctlProperty_HOM.Populate() ' updates the address section with possible new data from the policyholder section
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                CallBetterViewPreLoad()
                ctlDFRResidenceCoverages.Populate()
                Exit Select
        End Select
        ctlTreeView.RefreshQuote()
    End Sub

    Public Overrides Sub PreRateEvents()
        If IFM.VR.Common.Helpers.DFR.UnderConstructionEndorsementWarningHelper.isUCWarningAvailable(Quote) Then
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Any() AndAlso Quote.Locations.Count > 0 Then
                Dim MyLocation = Quote.Locations(0)
                Dim WasUnderConstruction As Boolean = False
                Dim WasUcItem As String = QQDevDictionary_GetItem("UnderConstructionOnPreviousImage")
                If WasUcItem.ToLower.Equals("true") Then
                    WasUnderConstruction = True
                End If
                If MyLocation.OccupancyCodeId = "7" AndAlso WasUnderConstruction = False Then
                    Me.ValidationHelper.AddError("Location Occupancy is Under Construction.  Refer to Underwriting for approval.")
                    ctlDFRResidenceCoverages.RouteToUwIsVisible = True
                End If
            End If
        End If
    End Sub

    Private Sub ctlIsuredList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlIsuredList.SaveRequested
        Me.ControlsToValidate_Custom.Add(ctlIsuredList)
    End Sub

    Private Sub ctlDFRResidenceCoverage_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlDFRResidenceCoverages.SaveRequested
        Me.ControlsToValidate_Custom.Add(ctlDFRResidenceCoverages)
    End Sub

    Private Sub ctlProperty_HOM_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlProperty_HOM.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctlProperty_HOM) ' Matt A 10-23-15
    End Sub

    Private Sub ctlTreeView_EffectiveDateChanging(NewQuoteEffectiveDate As String, OldQuoteEffectiveDate As String) Handles ctlTreeView.EffectiveDateChanging
        'Me.ctlProperty_HOM.EffectiveDateChanged(NewQuoteEffectiveDate, OldQuoteEffectiveDate)
        Me.EffectiveDateChanged(NewQuoteEffectiveDate, OldQuoteEffectiveDate)
        Populate()
    End Sub

    Private Sub ctlTreeView_EffectiveDateChangedFromTree(qqTranType As QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String) Handles ctlTreeView.EffectiveDateChangedFromTree
        Me.ctlQuoteSummary_DFR.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
        Helpers.EffectiveDateHelper.CheckDateCrossing(Quote, newEffectiveDate, oldEffectiveDate, ValidationHelper.ValidationErrors)
    End Sub

    Private Sub ctlTreeView_PopulateDFRCoverages() Handles ctlTreeView.PopulateDFRCoverages
        Me.ctlDFRResidenceCoverages.LoadStaticData()
        Me.ctlDFRResidenceCoverages.PopulateDeductibleLimit()
    End Sub
#End Region


End Class