Imports IFM.VR.Common.Helpers
Imports IFM.VR.Common.Helpers.CPR
Imports IFM.VR.Common.QuoteSearch
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects

Public Class ctl_WorkflowManager_Quote_CPP
    Inherits VRMasterControlBase

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
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Coverages
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Coverages, "")
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Coverages
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Coverages, "")
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Locations
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Locations, "")
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations, "")
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                If Me.ctlDisplayDiamondRatingErrors.ValidationHelper.HasErrros = False Then

                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "")
                End If
                ctlTreeView.RefreshRatedQuote()
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location
                Dim subWorkFlowText As String = If(Request.QueryString("locationNum") IsNot Nothing AndAlso QQHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("locationNum").ToString()), CInt(Request.QueryString("locationNum")) - 1, "")
                subWorkFlowText += If(Request.QueryString("buildingNum ") IsNot Nothing AndAlso QQHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("buildingNum").ToString()), "|" + CInt(Request.QueryString("buildingNum ")) - 1, "")
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, subWorkFlowText)
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.printFriendly 'Updated 12/20/2021 for CPP Endorsements Task 67649
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.printFriendly, "")
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.policyHistory 'Added 11/23/2021 for CPP Endorsements Task 65412 MLW
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation 'Added 11/23/2021 for CPP Endorsements Task 65562 MLW
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Else
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                Exit Select
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
            Case Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Coverages
                CheckForLocationAndBuilding()
                Me.ctl_CGL_Coverages.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Coverages
                CheckForLocationAndBuilding()
                Me.ctl_CPR_Coverages.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Locations
                CheckForLocationAndBuilding()
                ' If no classifications on the quote at either the policy or location level, we need to create one
                If Not QuoteHasClassifications() Then
                    If Quote.GLClassifications Is Nothing Then Quote.GLClassifications = New List(Of QuickQuote.CommonObjects.QuickQuoteGLClassification)
                    ' Add a new classification
                    Dim newcls As New QuickQuote.CommonObjects.QuickQuoteGLClassification()
                    newcls.ClassCode = "9999999"
                    Quote.GLClassifications.Add(newcls)
                    Me.ctl_CPP_CGL_Locations.Populate()
                    Save_FireSaveEvent(False)
                End If
                Me.ctl_CPP_CGL_Locations.Populate()
                Me.ctl_CPP_CGL_Locations.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations
                CheckForLocationAndBuilding()
                ' Need to load the location page here because changing the Business Income ALS checkbox will affect the location.
                'Me.ctl_CPR_Locations.Populate()
                'Me.ctl_CPR_Locations.Visible = True
                'updated visibility to be set before Populate so control can tell that it's visible (for CommDataPrefill Preload)
                Me.ctl_CPR_Locations.Visible = True
                Me.ctl_CPR_Locations.Populate()

                If Not subWorkFlowIndex.Contains("|") Then
                    ctl_CPR_Locations.ActiveLocationIndex = subWorkFlowIndex
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
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.InlandMarine
                Me.ctl_CPP_InlandMarine.Populate()
                Me.ctl_CPP_InlandMarine.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.farmIRPM
                'Required for CPP IRPM needs quote reloaded to combine scheduled ratings that have been overwritten on save.
                Me.ctlCommercial_IRPM_CPP.Populate()
                Me.ctlCommercial_IRPM_CPP.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctl_CPP_QuoteSummary.Populate()
                Me.ctl_CPP_QuoteSummary.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.printFriendly 'Updated 12/20/2021 for CPP Endorsements Task 67649
                Me.Ctl_CPP_PFSummary.Populate()
                Me.Ctl_CPP_PFSummary.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.Crime
                Me.ctl_CPP_Crime.Visible = True
                Exit Select
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.proposal
                Me.ctlProposalSelection.Populate()
                Me.ctlProposalSelection.Visible = True
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory
                'Added 11/23/2021 for CPP Endorsements Task 65412 MLW
                Me.ctlPolicyHistory.Visible = True
                Me.ctlPolicyHistory.Populate()
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation
                'Added 11/23/2021 for CPP Endorsements Task 65562 MLW 
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

    Private Sub CheckForLocationAndBuilding()
        If Quote IsNot Nothing Then
            ' Add a location and building to the quote if there are none already
            'added bools 12/2/2020 (Interoperability)
            Dim needsLocation As Boolean = False
            Dim needsBuilding As Boolean = False

            If Quote.Locations Is Nothing Then Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
            'If Quote.Locations.Count = 0 Then Quote.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation())
            'updated 12/2/2020
            If Quote.Locations.Count = 0 Then needsLocation = True
            If needsLocation = True Then
                Quote.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation())
                If PropertyAddressProtectionClassHelper.ispaProtectionClassUnitsAvailable(Quote) Then
                    Dim MyLocation = Me.Quote.Locations?.LastOrDefault
                    If MyLocation IsNot Nothing Then
                        MyLocation.FeetToFireHydrant = "1000"
                        MyLocation.MilesToFireDepartment = "5"
                    End If
                End If
            End If
            If Quote.Locations(0).Buildings Is Nothing Then Quote.Locations(0).Buildings = New List(Of QuickQuote.CommonObjects.QuickQuoteBuilding)
            'If Quote.Locations(0).Buildings.Count = 0 Then Quote.Locations(0).Buildings.Add(New QuickQuote.CommonObjects.QuickQuoteBuilding)
            'updated 12/2/2020 (Interoperability)
            If Quote.Locations(0).Buildings.Count = 0 Then needsBuilding = True
            If needsBuilding = True Then Quote.Locations(0).Buildings.Add(New QuickQuote.CommonObjects.QuickQuoteBuilding)
            If needsLocation = True OrElse needsBuilding = True Then 'added IF 12/2/2020 so we're not triggering Save every time; not even sure that the Save will work correctly if it isn't calling Populate after adding the loc and/or building since the Save might wipe out what was added if it's not on the screen (either way this should be better than before)
                Save_FireSaveEvent(False)
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandler Me.ctl_CPR_Coverages.BlanketDeductibleChanged, AddressOf HandleBlanketDeductibleChange
        AddHandler Me.ctl_CPR_Locations.BuildingZeroDeductibleChanged, AddressOf HandleBuildingZeroDeductibleChange
        AddHandler Me.ctl_CPR_Coverages.AgreedAmountChanged, AddressOf HandleAgreedAmountChange
        AddHandler Me.ctlRiskGradeSearch.populateCPPCoverageForCondoDandO, AddressOf HandlePopulateCPPCoverageForCondoDandO 'Added 09/02/2021 for bug 51550 MLW
        If Not IsPostBack Then
            DirectCast(Me.Page, BasePage).DirtyFormDiv = "divEditControls"
            Me.Populate()
        End If
    End Sub

    Public Sub HandleBlanketDeductibleChange()
        Me.ctl_CPR_Locations.HandleBlanketDeductibleChange()
    End Sub

    Private Sub HandleAgreedAmountChange(ByVal newvalue As Boolean)
        Me.ctl_CPR_Locations.HandleAgreedAmountChange(newvalue)
    End Sub

    Public Sub HandleBuildingZeroDeductibleChange()
        Me.ctl_CPR_Coverages.HandleBuildingZeroDeductibleChanged()
    End Sub

    'Added 09/02/2021 for bug 51550 MLW
    Public Sub HandlePopulateCPPCoverageForCondoDandO()
        Me.ctl_CGL_Coverages.Populate()
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

        Dim valList = IFM.VR.Validation.ObjectValidation.CommLines.LOB.CPP.PolicyLevelValidations.ValidatePolicyLevel(Me.Quote, valArgs.ValidationType)
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
                ctl_CPR_Coverages.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Locations
                ctl_CGL_Coverages.Populate()
                ctl_CPR_Coverages.Populate()
        End Select
        ctlTreeView.RefreshQuote()
    End Sub


    Private Sub ctlIsuredList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlIsuredList.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctlIsuredList)
    End Sub

    Private Sub ctl_CPR_Coverages_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_CPR_Coverages.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_CPR_Coverages)
    End Sub

    Private Sub ctl_CGL_Coverages_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_CGL_Coverages.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_CGL_Coverages)
    End Sub

    Private Sub ctl_CGL_Locations_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_CPP_CGL_Locations.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_CPP_CGL_Locations)
    End Sub

    Private Sub ctl_CPR_Locations_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_CPR_Locations.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_CPR_Locations)
    End Sub
    Private Sub ctl_AttachmentUpload_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_AttachmentUpload.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_AttachmentUpload)
    End Sub
    Private Sub ctlCommercial_IRPM_CPP_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlCommercial_IRPM_CPP.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctlCommercial_IRPM_CPP)
    End Sub
    Private Sub ctl_CPP_InlandMarine_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_CPP_InlandMarine.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_CPP_InlandMarine)
    End Sub
    Private Sub ctl_CPP_Crime_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_CPP_Crime.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_CPP_Crime)
    End Sub

#End Region

#Region "TreeView Navigations"

    Private Sub ctlTreeView_ShowPolicyholders(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyholders
        ' Navigate to the PolicyHolders page
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "")
    End Sub

    ' CPP-CPR

    Private Sub ctlTreeView_ShowCPPCPRLocations(ByVal LocationIndex As Integer) Handles ctlTreeView.ShowCPPCPRLocations
        ' Navigate to the locations page
        SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations, "")
    End Sub

    Private Sub ctlTreeView_EditCPPCPRLocation(ByVal LocationIndex As Integer) Handles ctlTreeView.EditCPPCPRLocation
        ' Navigate to the selected location
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations, LocationIndex)
    End Sub

    Private Sub ctlTreeView_NewCPPCPRLocation(ByVal LocationIndex As Integer) Handles ctlTreeView.NewCPPCPRLocation
        ' Add new location
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations, "")
    End Sub

    Private Sub ctlTreeView_DeleteCPPCPRLocation(ByVal LocationIndex As Integer) Handles ctlTreeView.DeleteCPPCPRLocation
        ' Delete location requested
    End Sub

    Private Sub ctlTreeView_EditCPPCPRLocationBuilding(ByVal LocationIndex As Integer, ByVal BuildingIndex As Integer) Handles ctlTreeView.EditCPPCPRLocationBuilding
        ' Navigate to selected building
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations, String.Format("{0}|{1}", LocationIndex, BuildingIndex))
    End Sub

    Private Sub ctlTreeView_ShowCPPCPRCoverages() Handles ctlTreeView.ShowCPPCPRCoverages
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Coverages, "")
    End Sub

    ' CPP-CGL

    Private Sub ctlTreeView_ShowCPPCGLCoverages() Handles ctlTreeView.ShowCPPCGLCoverages
        Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Coverages, "")
    End Sub

    Private Sub ctlTreeView_ShowCPPCGLLocations(ByVal LocationIndex As Integer) Handles ctlTreeView.ShowCPPCGLLocations
        ' Navigate to the locations page
        SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Locations, "")
    End Sub

    Private Sub ctlTreeView_EditCPPCGLLocation(ByVal LocationIndex As Integer) Handles ctlTreeView.EditCPPCGLLocation
        ' Navigate to the selected location
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Locations, LocationIndex)
    End Sub

    Private Sub ctlTreeView_EditCPPCGLLocationBuilding(ByVal LocationIndex As Integer, ByVal BuildingIndex As Integer) Handles ctlTreeView.EditCPPCGLLocationBuilding
        ' Navigate to selected building
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Locations, String.Format("{0}|{1}", LocationIndex, BuildingIndex))
    End Sub

    Private Sub ctlTreeView_ShowCrime() Handles ctlTreeView.ShowCrime
        SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.Crime, "")
    End Sub

    Private Sub ctlTreeView_ShowInlandMarine() Handles ctlTreeView.ShowInlandMarine
        SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.InlandMarine, "")
    End Sub

    Private Sub ctlTreeView_ShowQuoteSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowQuoteSummary
        SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.summary, "")
    End Sub

    Private Sub ctlTreeView_ShowIRPM(sender As Object, e As EventArgs) Handles ctlTreeView.ShowIRPM
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmIRPM, "")
    End Sub

    Private Sub ctlTreeView_IMDeleteRequested() Handles ctlTreeView.DeleteCPP_InlandMarine
        Me.ctl_CPP_InlandMarine.Handle_DeleteIM_Request()
    End Sub

    Private Sub ctlTreeView_CrimeDeleteRequested() Handles ctlTreeView.DeleteCPP_Crime
        Me.ctl_CPP_Crime.Handle_DeleteCrime_Request()
    End Sub

    Private Sub HandleEffectiveDateChange(NewDate As String, OldDate As String) Handles ctlTreeView.EffectiveDateChanging
        Me.ctl_CPR_Coverages.EffectiveDateChanging(NewDate, OldDate)
        Me.ctl_CGL_Coverages.EffectiveDateChanging(NewDate, OldDate)
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

        Me.ctl_CPR_Coverages.Populate()
        Me.ctl_CGL_Coverages.Populate()
        Me.ctl_CPR_Locations.Populate()
        Me.ctl_CPP_CGL_Locations.Populate()
    End Sub

    'Added 11/24/2021 for CPP Endorsements Task 65408 MLW
    Private Sub ctlTreeView_ShowPolicyHistory(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyHistory
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
    End Sub
    'Added 11/24/2021 for CPP Endorsements Task 65408 MLW
    Private Sub ctlTreeView_ShowBillingInformation(sender As Object, e As EventArgs) Handles ctlTreeView.ShowBillingInformation
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub

#End Region

    Protected Overrides Sub RateWasRequested()
        ControlsToValidate_Custom.Add(Me)
        Dim valArgs As New VRValidationArgs(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
        Me.Save_FireSaveEvent(True)
        RequestRate.RateWasRequested(Quote, Me, ctlTreeView, ValidationSummmary)

        '    If Me.ValidationSummmary.HasErrors() = False Then
        '        'good to rate
        '        ' do rate
        '        Dim saveErr As String = Nothing
        '        Dim loadErr As String = Nothing

        '        Dim preRateCompanyId As Integer = 1
        '        Dim postRateCompanyId As Integer = 1
        '        Session("ShowNewCoLockMessage") = ""

        '        If Quote IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(Quote.Database_DiaCompanyId) Then
        '            preRateCompanyId = CInt(Quote.Database_DiaCompanyId)
        '        End If

        '        'Dim ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr)
        '        'updated 2/18/2019
        '        Dim ratedQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        '        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
        '            'no rate
        '        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
        '            Dim successfulEndorsementRate As Boolean = Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedEndorsementQuoteFromContext(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, qqEndorsementResults:=ratedQuote, errorMessage:=saveErr)
        '        Else
        '            ratedQuote = Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr)
        '        End If

        '        ' Check for quote stop or kill - DM 8/30/2017
        '        If Me.Quote IsNot Nothing AndAlso (Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled OrElse Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled) Then
        '            IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Session)
        '        End If

        '        ' set this per page life cycle cache with newest - 6-3-14
        '        If ratedQuote IsNot Nothing Then

        '            If QQHelper.IsPositiveIntegerString(ratedQuote.CompanyId) Then
        '                postRateCompanyId = CInt(ratedQuote.CompanyId)
        '                If postRateCompanyId > preRateCompanyId Then
        '                    Session("ShowNewCoLockMessage") = True
        '                End If
        '            End If

        '            DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(False, ratedQuote) 'sets the rated quote cache
        '        Else
        '            ' you can't set a Nothing quote with this method you'll just have to let it find out for itself that the last rated quote was nothing - should never happen
        '            DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(True)
        '        End If

        '        If String.IsNullOrWhiteSpace(saveErr) = False Or String.IsNullOrWhiteSpace(loadErr) = False Then
        '            'failed
        '            If String.IsNullOrWhiteSpace(saveErr) = False Then
        '                Me.ValidationHelper.AddError(saveErr)
        '            End If
        '            If String.IsNullOrWhiteSpace(loadErr) = False Then
        '                Me.ValidationHelper.AddError(loadErr)
        '            End If

        '        Else
        '            ' did not fail to call service but may have validation Items
        '            If ratedQuote IsNot Nothing Then
        '                WebHelper_Personal.GatherRatingErrorsAndWarnings(ratedQuote, Me.ValidationHelper)
        '                'RaiseEvent QuoteRated(ratedQuote) ' always fire so tree gets even attempt rates 4-14-14
        '                If ratedQuote.Success Then
        '                    ''Me.ctl_CPP_QuoteSummary.Populate()
        '                    'SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
        '                    'ctlTreeView.RefreshRatedQuote()

        '                    If ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped OrElse ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped Then
        '                        'stay where you are - don't show summary - stop message will be contained in validation messages
        '                    Else
        '                        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
        '                        ctlTreeView.RefreshRatedQuote()
        '                    End If
        '                Else
        '                    'stay where you are - probably coverages
        '                End If

        '            End If
        '        End If
        '    End If
    End Sub


    Private Sub ctlTreeView_EffectiveDateChangedFromTree(qqTranType As QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String) Handles ctlTreeView.EffectiveDateChangedFromTree
        Me.ctl_CPP_QuoteSummary.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
        Me.ctl_CPP_InlandMarine.EffectiveDateChanged(qqTranType, newEffectiveDate, oldEffectiveDate) ' Needed for Food Manufacturers Enhancement.  7/13/21
        ' This OH check can be removed after 3/1/2021
        If IFM.VR.Common.Helpers.MultiState.General.SubQuotesContainsState(Quote.MultiStateQuotes, "OH") AndAlso CDate(newEffectiveDate) < CDate("2/1/2021") Then
            IFM.VR.Common.Helpers.MultiState.General.ShowOhioEffectiveDatePopup(Me.Page)
        End If
        'Added 6/29/2022 for task 75037 MLW
        Helpers.EffectiveDateHelper.CheckDateCrossing(Quote, newEffectiveDate, oldEffectiveDate, ValidationHelper.ValidationErrors)

        'Me.ctl_CPR_Coverages.EffectiveDateChanging(newEffectiveDate, oldEffectiveDate)
        'Me.ctl_CGL_Coverages.EffectiveDateChanging(newEffectiveDate, oldEffectiveDate)
    End Sub

    Private Sub ctl_CPR_Coverages_NeedToClearCIMTransportation() Handles ctl_CPR_Coverages.NeedToClearCIMTransportation
        ctl_CPP_InlandMarine.ClearTransportation()
    End Sub

    Private Sub ctl_CPR_Coverages_NeedToReloadCIMTransportation() Handles ctl_CPR_Coverages.NeedToReloadCIMTransportation
        ctl_CPP_InlandMarine.ReloadTransportation()
    End Sub

    'Added 10/20/2022 for task 77527 MLW
    Private Sub ctlTreeView_PopulateInflationGuard() Handles ctlTreeView.PopulateInflationGuard
        Me.ctl_CPR_Locations.PopulateInflationGuard()
    End Sub

    Private Sub ctlTreeView_PopulateCPRBuildingInformation() Handles ctlTreeView.PopulateCPRBuildingInformation
        Me.ctl_CPR_Locations.PopulateCPRBuildingInformation()
    End Sub

    Private Sub ctlUWQuestionsPopup_ToggleUWPopupShown() Handles ctlUWQuestionsPopup.ToggleUWPopupShown
        Me.ctlCommercialDataPrefillEntry.HidePopup()
    End Sub

    Private Sub ctlTreeView_PopulateCPRCoverages() Handles ctlTreeView.PopulateCPRCoverages
        Me.ctl_CPR_Coverages.PopulateCPRCoverages()
    End Sub

    Private Sub ctlTreeView_RemoveFunctionalReplacementCost() Handles ctlTreeView.RemoveFunctionalReplacementCost
        Me.ctl_CPR_Locations.RemoveFunctionalReplacementCost()
    End Sub

End Class