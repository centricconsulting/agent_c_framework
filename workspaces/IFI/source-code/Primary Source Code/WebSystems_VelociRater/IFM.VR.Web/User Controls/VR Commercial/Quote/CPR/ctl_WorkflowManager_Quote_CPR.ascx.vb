Imports IFM.VR.Common.Helpers.CPR
Imports IFM.VR.Common.QuoteSearch
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects

Public Class ctl_WorkflowManager_Quote_CPR
    Inherits VRMasterControlBase

    Public Sub HandleBlanketDeductibleChange()
        Me.ctl_CPR_LocationsList.HandleBlanketDeductibleChange()
    End Sub

    Public Sub HandleAgreedAmountChange(ByVal newvalue As Boolean)
        Me.ctl_CPR_LocationsList.HandleAgreedAmountChange(newvalue)
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddScriptLine("$(""#divEditControls"").fadeIn('fast');")
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
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location

                Dim subWorkFlowText As String = If(Request.QueryString("locationNum") IsNot Nothing AndAlso QQHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("locationNum").ToString()), CInt(Request.QueryString("locationNum")) - 1, "")
                subWorkFlowText += If(Request.QueryString("buildingNum ") IsNot Nothing AndAlso QQHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("buildingNum").ToString()), "|" + CInt(Request.QueryString("buildingNum ")) - 1, "")
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, subWorkFlowText)
            Case Common.Workflow.Workflow.WorkflowSection.drivers
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.drivers, "")  ' This enum used for the Print-Friendly summary MGB
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

    Public Overrides Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                Me.ctlIsuredList.Visible = True
                Me.ctlIsuredList.Populate()
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                ' Add a location and building to the quote if there are none already
                'If Quote.Locations Is Nothing Then Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
                'If Quote.Locations.Count = 0 Then
                '    Quote.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation())
                '    If PropertyAddressProtectionClassHelper.ispaProtectionClassUnitsAvailable(Quote) Then
                '        Dim MyLocation = Me.Quote.Locations?.LastOrDefault
                '        If MyLocation IsNot Nothing Then
                '            MyLocation.FeetToFireHydrant = "1000"
                '            MyLocation.MilesToFireDepartment = "5"
                '        End If
                '    End If
                'End If
                'If Quote.Locations(0).Buildings Is Nothing Then Quote.Locations(0).Buildings = New List(Of QuickQuote.CommonObjects.QuickQuoteBuilding)
                'If Quote.Locations(0).Buildings.Count = 0 Then Quote.Locations(0).Buildings.Add(New QuickQuote.CommonObjects.QuickQuoteBuilding)
                'Save_FireSaveEvent(False)
                'updated 9/28/2023 to start using the code below so we don't perform a Save every time we go to Coverages screen
                '' Below code makes the above save event conditional. Requested not to include with initial CPR release.
                Dim doSave As Boolean = False
                If Quote.Locations Is Nothing Then
                    Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
                    doSave = True
                End If
                If Quote.Locations.Count = 0 Then
                    Quote.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation())
                    If PropertyAddressProtectionClassHelper.ispaProtectionClassUnitsAvailable(Quote) Then
                        Dim MyLocation = Me.Quote.Locations?.LastOrDefault
                        If MyLocation IsNot Nothing Then
                            MyLocation.FeetToFireHydrant = "1000"
                            MyLocation.MilesToFireDepartment = "5"
                        End If
                    End If
                    doSave = True
                End If
                If Quote.Locations(0).Buildings Is Nothing Then
                    Quote.Locations(0).Buildings = New List(Of QuickQuote.CommonObjects.QuickQuoteBuilding)
                    doSave = True
                End If
                If Quote.Locations(0).Buildings.Count = 0 Then
                    Quote.Locations(0).Buildings.Add(New QuickQuote.CommonObjects.QuickQuoteBuilding)
                    doSave = True
                End If
                If doSave Then
                    Save_FireSaveEvent(False)
                End If

                Me.ctl_CPR_PolicyCoverages.Populate()
                Me.ctl_CPR_PolicyCoverages.Visible = True
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location
                ' Need to load the location page here because changing the Business Income ALS checkbox will affect the location.
                'Me.ctl_CPR_LocationsList.Populate()
                'Me.ctl_CPR_LocationsList.Visible = True
                'updated visibility to be set before Populate so control can tell that it's visible (for CommDataPrefill Preload)
                Me.ctl_CPR_LocationsList.Visible = True
                Me.ctl_CPR_LocationsList.Populate()

                If subWorkFlowIndex.Contains("|") = False Then
                    ctl_CPR_LocationsList.ActiveLocationIndex = subWorkFlowIndex
                Else
                    'has location index and building index split by a pipe |
                    Dim parts As String() = subWorkFlowIndex.Split("|")
                    If parts.Count = 2 Then
                        Dim BuildingControls As List(Of ctl_CPR_Building)
                        BuildingControls = Me.GatherChildrenOfType(Of ctl_CPR_Building)(False)
                        For Each B As ctl_CPR_Building In BuildingControls
                            If B.LocationIndex = parts(0) AndAlso B.BuildingIndex = parts(1) Then
                                B.OpenAllParentAccordionsOnNextLoad(B.ScrollToControlId)
                                Exit For
                            End If
                        Next
                    End If
                End If

                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.farmIRPM
                Me.ctl_CPR_IRPM.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctl_CPR_QuoteSummary.Populate()
                Me.ctl_CPR_QuoteSummary.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.drivers  ' This enum used for the Print-Friendly summary MGB
                Me.Ctl_CPR_PFSummary.Populate()
                Me.Ctl_CPR_PFSummary.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.proposal
                Me.ctlProposalSelection.Populate()
                Me.ctlProposalSelection.Visible = True
                Exit Select
            Case Else
                Exit Select
        End Select
        Me.CurrentWorkFlow = workflow
#If DEBUG Then
        Debug.WriteLine(String.Format("Workflow Changed -  CurrentWorkflow: {1}  Control:{0}", Me.ToString(), CurrentWorkFlow.ToString()))
#End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandler Me.ctl_CPR_PolicyCoverages.BlanketDeductibleChanged, AddressOf HandleBlanketDeductibleChange
        AddHandler Me.ctl_CPR_LocationsList.BuildingZeroDeductibleChanged, AddressOf HandleBuildingZeroDeductibleChange
        AddHandler Me.ctl_CPR_PolicyCoverages.AgreedAmountChanged, AddressOf HandleAgreedAmountChange

        If Not IsPostBack Then
            DirectCast(Me.Page, BasePage).DirtyFormDiv = "divEditControls"
            Me.Populate()
        End If
    End Sub

    Private Sub HandleBuildingZeroDeductibleChange()
        Me.ctl_CPR_PolicyCoverages.UpdateBlanketDeductibleFromBuildingZero()
    End Sub

    Private Function QuoteHasClassifications() As Boolean
        Dim cnt As Integer = 0

        ' Count the Policy level classifications
        If Quote.GLClassifications IsNot Nothing Then
            If Quote.GLClassifications.Count > 0 Then cnt += Quote.GLClassifications.Count
        End If

        ' Count the location level classifications
        If Quote.Locations IsNot Nothing Then
            For Each loc As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                If loc.GLClassifications IsNot Nothing Then
                    If loc.GLClassifications.Count > 0 Then cnt += loc.GLClassifications.Count
                End If
            Next
        End If

        If cnt = 0 Then Return False Else Return True
    End Function

    Public Overrides Function Save() As Boolean
        Select Case CurrentWorkFlow
            Case Else
                Me.SaveChildControls()
        End Select

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Me.ValidationHelper.GroupName = "Policy Level"

        Dim valList = IFM.VR.Validation.ObjectValidation.CommLines.LOB.CPR.PolicyLevelValidations.ValidatePolicyLevel(Me.Quote, valArgs.ValidationType)
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

#Region "Save Requests"

    Public Overrides Sub AfterSaveOccurrs(args As VrControlBaseSaveEventArgs)
        MyBase.AfterSaveOccurrs(args)

        Select Case CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders

            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages

            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations
                CallCapeComPreLoad()
                CallBetterViewComPreLoad()
                CallProtectionClassPreLoad()
                ctl_CPR_PolicyCoverages.Populate()
        End Select
        ctlTreeView.RefreshQuote()
    End Sub

    Private Sub ctlIsuredList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlIsuredList.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctlIsuredList)
    End Sub

    Private Sub ctl_CPR_PolicyCoverages_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_CPR_PolicyCoverages.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_CPR_PolicyCoverages)
    End Sub
    Private Sub ctl_CPR_LocationsList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_CPR_LocationsList.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_CPR_LocationsList)
    End Sub
    Private Sub ctl_AttachmentUpload_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_AttachmentUpload.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_AttachmentUpload)
    End Sub
    Private Sub ctl_CPR_IRPM_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_CPR_IRPM.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_CPR_IRPM)
    End Sub

#End Region



#Region "TreeView Navigations"
    Private Sub ctlTreeView_ShowPolicyholders(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyholders
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "")
    End Sub

    Private Sub ctlTreeView_ShowCoverages(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyLevelCoverages
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "")
    End Sub

    Private Sub ctlTreeView_ShowLocations(sender As Object, e As EventArgs) Handles ctlTreeView.ShowLocations
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, "")
    End Sub

    Private Sub ctlTreeView_NewLocation(locationNumber As Integer) Handles ctlTreeView.NewLocation
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, "")
    End Sub

    Private Sub ctlTreeView_EditLocation(locationNumber As Integer) Handles ctlTreeView.EditLocation
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, locationNumber - 1)
    End Sub

    Private Sub ctlTreeView_EditLocationBuilding(locationNumber As Integer, buildingNumber As Integer) Handles ctlTreeView.EditLocationBuilding
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, String.Format("{0}|{1}", locationNumber - 1, buildingNumber - 1)) 'change to be more specific
    End Sub


    Private Sub ctlTreeView_ShowIRPM(sender As Object, e As EventArgs) Handles ctlTreeView.ShowIRPM
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmIRPM, "")
    End Sub

    Private Sub ctlTreeView_ShowQuoteSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowQuoteSummary
        SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.summary, "")
    End Sub

    Private Sub HandleEffectiveDateChange(NewDate As String, OldDate As String) Handles ctlTreeView.EffectiveDateChanging
        Me.ctl_CPR_PolicyCoverages.EffectiveDateChanging(NewDate, OldDate)
        Exit Sub
    End Sub

    'Added 6/29/2022 for task 75037 MLW
    Private Sub ctlTreeView_QuoteUpdated(sender As Object, e As EventArgs) Handles ctlTreeView.QuoteUpdated
        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
            VR.Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            VR.Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
        Else
            IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId)
        End If
        Me.ctl_CPR_PolicyCoverages.Populate()
        Me.ctl_CPR_LocationsList.Populate()
    End Sub


#End Region



    Protected Overrides Sub RateWasRequested()
        ControlsToValidate_Custom.Add(Me)
        Dim valArgs As New VRValidationArgs(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
        Me.Save_FireSaveEvent(True)
        RequestRate.RateWasRequested(Quote, Me, ctlTreeView, ValidationSummmary)
        'If Me.ValidationSummmary.HasErrors() = False Then
        '    'good to rate
        '    ' do rate
        '    Dim saveErr As String = Nothing
        '    Dim loadErr As String = Nothing

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

    Private Sub ctlTreeView_EffectiveDateChangedFromTree(qqTranType As QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String) Handles ctlTreeView.EffectiveDateChangedFromTree
        Me.ctl_CPR_LocationsList.EffectiveDateChanged(newEffectiveDate, oldEffectiveDate)
        Me.ctl_CPR_QuoteSummary.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
        ' This OH check can be removed after 3/1/2021
        If IFM.VR.Common.Helpers.MultiState.General.SubQuotesContainsState(Quote.MultiStateQuotes, "OH") AndAlso CDate(newEffectiveDate) < CDate("2/1/2021") Then
            IFM.VR.Common.Helpers.MultiState.General.ShowOhioEffectiveDatePopup(Me.Page)
        End If
        'Added 6/29/2022 for task 75037 MLW
        Helpers.EffectiveDateHelper.CheckDateCrossing(Quote, newEffectiveDate, oldEffectiveDate, ValidationHelper.ValidationErrors)
    End Sub

    'Added 10/20/2022 for task 77527 MLW
    Private Sub ctlTreeView_PopulateInflationGuard() Handles ctlTreeView.PopulateInflationGuard
        Me.ctl_CPR_LocationsList.PopulateInflationGuard()
    End Sub

    Private Sub ctlUWQuestionsPopup_ToggleUWPopupShown() Handles ctlUWQuestionsPopup.ToggleUWPopupShown
        Me.ctlCommercialDataPrefillEntry.HidePopup()
    End Sub

    Private Sub ctlTreeView_PopulateCPRCoverages() Handles ctlTreeView.PopulateCPRCoverages
        Me.ctl_CPR_PolicyCoverages.LoadBlanketDeductible()
        Me.ctl_CPR_PolicyCoverages.PopulateBlanketDeductible()
    End Sub



    Private Sub ctlTreeView_PopulateCPRBuildingInformation() Handles ctlTreeView.PopulateCPRBuildingInformation
        Me.ctl_CPR_LocationsList.PopulateCPRBuildingInformation()
    End Sub

    Private Sub ctlTreeView_RemoveFunctionalReplacementCost() Handles ctlTreeView.RemoveFunctionalReplacementCost
        Me.ctl_CPR_LocationsList.RemoveFunctionalReplacementCost()
    End Sub
End Class