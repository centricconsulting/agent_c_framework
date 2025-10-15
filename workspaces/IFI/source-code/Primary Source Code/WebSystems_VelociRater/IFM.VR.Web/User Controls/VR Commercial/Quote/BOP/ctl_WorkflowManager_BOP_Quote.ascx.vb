Imports IFM.VR.Common.Workflow
Imports IFM.VR.Common.QuoteSearch
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects

Public Class ctl_WorkflowManager_BOP_Quote
    Inherits VRMasterControlBase

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddScriptLine("$(""#divEditControls"").fadeIn('fast');")
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As Workflow.WorkflowSection)
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
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location

                Dim subWorkFlowText As String = If(Request.QueryString("locationNum") IsNot Nothing AndAlso QQHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("locationNum").ToString()), CInt(Request.QueryString("locationNum")) - 1, "")
                subWorkFlowText += If(Request.QueryString("buildingNum ") IsNot Nothing AndAlso QQHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("buildingNum").ToString()), "|" + CInt(Request.QueryString("buildingNum ")) - 1, "")
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, subWorkFlowText)
            Case Common.Workflow.Workflow.WorkflowSection.printFriendly 'Updated 09/21/2021 for BOP Endorsements Task 61506 MLW, changed from drivers to printFriendly
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.printFriendly, "")
            Case Common.Workflow.Workflow.WorkflowSection.policyHistory 'Added 09/20/2021 for BOP Endorsements Task 63917 MLW 
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation 'Added 09/20/2021 for BOP Endorsements Task 63914 MLW
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
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

    Public Sub HideAllControls()
        For Each cntrl As VRControlBase In Me.ChildVrControls
            cntrl.Visible = False
        Next
        Me.ctl_Comm_NewQuoteForClient.Visible = True
    End Sub

    Public Overrides Sub SetCurrentWorkFlow(workflow As Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                Me.ctlIsuredList.Visible = True
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                Me.ctl_BOP_PolicyLevelCoverages.Visible = True
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location
                'Me.ctl_BOP_LocationList.Visible = True
                ' NEW
                Me.ctl_BOP_LocationList.Visible = True
                If subWorkFlowIndex.Contains("|") = False Then
                    ctl_BOP_LocationList.ActiveLocationIndex = subWorkFlowIndex
                Else
                    'has location index and building index split by a pipe |
                    If Int32.TryParse(subWorkFlowIndex.Split("|")(0), Nothing) AndAlso Int32.TryParse(subWorkFlowIndex.Split("|")(1), Nothing) Then
                        Dim selectedBuilding As ctl_BOP_Building = (From b In Me.GatherChildrenOfType(Of ctl_BOP_Building)(False)
                                                                    Where b.LocationIndex = CInt(subWorkFlowIndex.Split("|")(0)) AndAlso
                                                                      b.BuildingIndex = CInt(subWorkFlowIndex.Split("|")(1)) Select b).FirstOrDefault()
                        If selectedBuilding IsNot Nothing Then
                            selectedBuilding.OpenAllParentAccordionsOnNextLoad(selectedBuilding.ScrollToControlId)
                        End If
                    End If
                End If
                ' END NEW



                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctl_BOP_Quote_Summary.Visible = True
                Me.ctl_BOP_IRPM.Populate()
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.farmIRPM
                Me.ctl_BOP_IRPM.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.printFriendly 'Updated 09/21/2021 for BOP Endorsements Task 61506 MLW, changed from drivers to printFriendly
                Me.Ctl_PF_BOPQuoteSummary.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.proposal
                Me.ctlProposalSelection.Populate()
                Me.ctlProposalSelection.Visible = True
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory
                'Added 09/20/2021 for BOP Endorsements Task 63917 MLW 
                Me.ctlPolicyHistory.Visible = True
                Me.ctlPolicyHistory.Populate()
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation
                'Added 09/20/2021 for BOP Endorsements Task 63914 MLW 
                Me.ctlBillingInfo.Visible = True
                Me.ctlBillingInfo.Populate()
            Case Else
                Exit Select
        End Select
        Me.CurrentWorkFlow = workflow
#If DEBUG Then
        Debug.WriteLine(String.Format("Workflow Changed -  CurrentWorkflow: {1}  Control:{0}", Me.ToString(), CurrentWorkFlow.ToString()))
#End If
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            DirectCast(Me.Page, BasePage).DirtyFormDiv = "divEditControls"
            Me.Populate()
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Me.ctlIsuredList.ValidateControl(valArgs)
        Me.ctl_BOP_PolicyLevelCoverages.ValidateControl(valArgs)
        Me.ctl_BOP_LocationList.ValidateControl(valArgs)
        Me.ctl_BOP_IRPM.ValidateControl(valArgs)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Me.ValidationHelper.GroupName = "Policy Level"

        Dim valList = IFM.VR.Validation.ObjectValidation.CommLines.LOB.BOP.PolicyLevelValidations.ValidatePolicyLevel(Me.Quote, valArgs.ValidationType)
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

    Protected Overrides Sub RateWasRequested()
        'Throw New NotImplementedException()

        ControlsToValidate_Custom.Add(Me)
        Dim valArgs As New VRValidationArgs(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
        Me.Save_FireSaveEvent(True)
        RequestRate.RateWasRequested(Quote, Me, ctlTreeView, ValidationSummmary, Me.ctl_BOP_Quote_Summary, Me.ctl_BOP_LocationList)
        'If Me.ValidationSummmary.HasErrors() = False Then
        '    Dim saveErr As String = Nothing
        '    Dim loadErr As String = Nothing

        '    'check for AIs that won't pass validation
        '    IFM.VR.Common.Helpers.AdditionalInterest.RemoveIncompleteAIs_BOP_CAP(Me.Quote)

        '    'Dim ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr)
        '    'updated 2/18/2019
        '    Dim ratedQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        '    If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
        '        'no rate
        '    ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
        '        Dim successfulEndorsementRate As Boolean = Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedEndorsementQuoteFromContext(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, qqEndorsementResults:=ratedQuote, errorMessage:=saveErr)
        '    Else
        '        ratedQuote = Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr)
        '    End If

        '    ' Check for quote stop or kill - DM 8/30/2017
        '    If Me.Quote IsNot Nothing AndAlso (Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled OrElse Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled) Then
        '        IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Session)
        '    End If

        '    If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
        '        Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
        '    ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
        '        Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
        '    Else
        '        IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId)
        '    End If

        '    ctl_BOP_LocationList.Populate()

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
        '                    ctl_BOP_Quote_Summary.Populate()
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

    Public Overrides Function Save() As Boolean
        Select Case CurrentWorkFlow
            Case Else
                Me.SaveChildControls()
        End Select

        Return True
    End Function

#Region "Save Requests"

    Public Overrides Sub AfterSaveOccurrs(args As VrControlBaseSaveEventArgs)
        MyBase.AfterSaveOccurrs(args)

        Select Case CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders

            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages

            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location
                CallCapeComPreLoad()
                CallBetterViewComPreLoad()
                CallProtectionClassPreLoad()
        End Select
        ctlTreeView.RefreshQuote()
    End Sub

    Private Sub ctlIsuredList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlIsuredList.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctlIsuredList)
    End Sub

    Private Sub ctl_BOP_PolicyLevelCoverages_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_BOP_PolicyLevelCoverages.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_BOP_PolicyLevelCoverages)
    End Sub

    Private Sub ctl_BOP_LocationList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_BOP_LocationList.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_BOP_LocationList)
    End Sub


#End Region



#Region "TreeView Navigations"

    Private Sub ctlTreeView_ShowPolicyholders(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyholders
        SetCurrentWorkFlow(Workflow.WorkflowSection.policyHolders, "")
    End Sub

    Private Sub ctlTreeView_ShowPolicyLevelCoverages(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyLevelCoverages
        SetCurrentWorkFlow(Workflow.WorkflowSection.coverages, "")
    End Sub

    Private Sub ctlTreeView_ShowLocations(sender As Object, e As EventArgs) Handles ctlTreeView.ShowLocations
        SetCurrentWorkFlow(Workflow.WorkflowSection.location, "")
    End Sub

    Private Sub ctlTreeView_ShowQuoteSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowQuoteSummary
        SetCurrentWorkFlow(Workflow.WorkflowSection.summary, "")
    End Sub

    Private Sub ctlTreeView_ShowIRPM(sender As Object, e As EventArgs) Handles ctlTreeView.ShowIRPM
        SetCurrentWorkFlow(Workflow.WorkflowSection.farmIRPM, "")
    End Sub

    Private Sub ctlTreeView_EditLocation(locationNumber As Integer) Handles ctlTreeView.EditLocation
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, locationNumber - 1)
        'Me.ctl_BOP_LocationList.ActiveLocationIndex = locationNumber - 1
    End Sub

    Private Sub ctlTreeView_EditLocationBuilding(locationNumber As Integer, buildingNumber As Integer) Handles ctlTreeView.EditLocationBuilding
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, String.Format("{0}|{1}", locationNumber - 1, buildingNumber - 1)) 'change to be more specific
    End Sub

    Private Sub HandleEffectiveDateChange(ByVal NewDate As String, ByVal OldDate As String) Handles ctlTreeView.EffectiveDateChanging
        Me.ctl_BOP_LocationList.EffectiveDateChanging(NewDate, OldDate)
    End Sub

    'Added 09/20/2021 for BOP Endorsements Task 63922 MLW
    Private Sub ctlTreeView_ShowPolicyHistory(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyHistory
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
    End Sub
    'Added 09/20/2021 for BOP Endorsements Task 63922 MLW
    Private Sub ctlTreeView_ShowBillingInformation(sender As Object, e As EventArgs) Handles ctlTreeView.ShowBillingInformation
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub

#End Region

    'added 11/15/2017 for Equipment Breakdown MBR
#Region "Treeview Updated Events"
    Private _EffDateJustChanged As Boolean

    Private Sub ctlTreeView_QuoteUpdated(sender As Object, e As EventArgs) Handles ctlTreeView.QuoteUpdated
        If _EffDateJustChanged = True Then
            'Me.ctl_BOP_LocationList.PopulateLocationCoverages()
            Me.ctl_BOP_LocationList.Populate()
        End If
        _EffDateJustChanged = False
    End Sub

    Private Sub ctlTreeView_EffectiveDateChanging(NewEffectiveDate As String, OldEffectiveDate As String) Handles ctlTreeView.EffectiveDateChanging
        _EffDateJustChanged = True
    End Sub

    Private Sub ctlTreeView_EffectiveDateChangedFromTree(qqTranType As QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String) Handles ctlTreeView.EffectiveDateChangedFromTree
        Me.ctl_BOP_Quote_Summary.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
        Me.ctl_BOP_PolicyLevelCoverages.HandleEffectiveDateChange(newEffectiveDate, oldEffectiveDate)
        ' This OH check can be removed after 3/1/2021
        If IFM.VR.Common.Helpers.MultiState.General.SubQuotesContainsState(Quote.MultiStateQuotes, "OH") AndAlso CDate(newEffectiveDate) < CDate("2/1/2021") Then
            IFM.VR.Common.Helpers.MultiState.General.ShowOhioEffectiveDatePopup(Me.Page)
        End If
        If Quote IsNot Nothing AndAlso Common.Helpers.BOP.RemovePropDedBelow1k.RemovePropDedBelow1kEnabled() Then
            Helpers.EffectiveDateHelper.CheckDateCrossing(Quote, newEffectiveDate, oldEffectiveDate, ValidationHelper.ValidationErrors)
        End If
    End Sub

#End Region

    Private Sub ctlTreeView_PopulateBOPCoverages() Handles ctlTreeView.PopulateBOPCoverages
        Me.ctl_BOP_PolicyLevelCoverages.PopulateCoverages()
    End Sub

    Private Sub ctlTreeView_PopulateBOPBuildingInformation() Handles ctlTreeView.PopulateBOPBuildingInformation
        Me.ctl_BOP_LocationList.PopulateBOPBuildingInformation()
    End Sub

    Private Sub ctlUWQuestionsPopup_ToggleUWPopupShown() Handles ctlUWQuestionsPopup.ToggleUWPopupShown
        Me.ctlCommercialDataPrefillEntry.HidePopup()
    End Sub







End Class