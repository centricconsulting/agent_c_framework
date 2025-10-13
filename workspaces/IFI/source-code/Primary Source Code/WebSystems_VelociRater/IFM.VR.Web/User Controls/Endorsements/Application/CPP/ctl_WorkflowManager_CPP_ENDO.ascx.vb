Imports IFM.VR.Common.Workflow
Imports IFM.VR.Common.QuoteSearch
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports EndorsementStructures
Imports IFM.VR.Common.Helpers.CPR

Public Class ctl_WorkflowManager_CPP_ENDO
    Inherits VRMasterControlBase

    Public Property TransactionLimitReached As Boolean
        Get
            'If ViewState("vs_CppTransactionLimitReached") Is Nothing Then
            TransactionCountRequested()
            'End If
            Return ViewState.GetBool("vs_CppTransactionLimitReached", False, True)
        End Get
        Set(value As Boolean)
            ViewState("vs_CppTransactionLimitReached") = value
        End Set
    End Property

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddScriptLine("$(""#divEditControls"").fadeIn('fast');")
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As Workflow.WorkflowSection)

        If IsQuoteEndorsement() Then
            If workflow <> Common.Workflow.Workflow.WorkflowSection.summary AndAlso workflow <> Common.Workflow.Workflow.WorkflowSection.fileUpload Then
                Dim typeOfEndorsement2 As String = QQDevDictionary_GetItem("Type_Of_Endorsement_Selected")
                Select Case TypeOfEndorsement()
                    Case EndorsementStructures.EndorsementTypeString.CPP_AmendMailing
                        workflow = Common.Workflow.Workflow.WorkflowSection.policyHolders
                    Case EndorsementStructures.EndorsementTypeString.CPP_AddDeleteLocationLienholder
                        workflow = Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations
                    Case EndorsementStructures.EndorsementTypeString.CPP_AddDeleteContractorsEquipmentLienholder
                        workflow = Common.Workflow.Workflow.WorkflowSection.InlandMarine
                End Select
            End If
        End If

        ctlTreeView.RefreshQuote()
        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                If Me.ctlDisplayDiamondRatingErrors.ValidationHelper.HasErrros = False Then

                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "")
                End If
                ctlTreeView.RefreshRatedQuote()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "")
            Case Common.Workflow.Workflow.WorkflowSection.InlandMarine
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.InlandMarine, "")
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations, "")
                Exit Select
            'Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location

            '    Dim subWorkFlowText As String = If(Request.QueryString("locationNum") IsNot Nothing AndAlso QQHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("locationNum").ToString()), CInt(Request.QueryString("locationNum")) - 1, "")
            '    subWorkFlowText += If(Request.QueryString("buildingNum ") IsNot Nothing AndAlso QQHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("buildingNum").ToString()), "|" + CInt(Request.QueryString("buildingNum ")) - 1, "")
            '    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations, subWorkFlowText)
            Case Common.Workflow.Workflow.WorkflowSection.printFriendly 'Updated 09/21/2021 for BOP Endorsements Task 61506, changed from drivers to printFriendly
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.printFriendly, "")
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload 'Added 12/21/2021 for CPP Endorsements Task 66834 MLW
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "")
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
        'Me.ctl_Comm_NewQuoteForClient.Visible = True
    End Sub

    Public Overrides Sub SetCurrentWorkFlow(workflow As Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                Me.ctlIsuredList.Visible = True
                Exit Select
                'Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location

                '    ' NEW - Tree view to 
                '    'Me.ctl_BOP_ENDO_LocationList.Visible = True
                '    If subWorkFlowIndex.Contains("|") = False Then
                '        'ctl_BOP_ENDO_LocationList.ActiveLocationIndex = (subWorkFlowIndex)
                '    Else
                '        'has location index and building index split by a pipe |
                '        If Int32.TryParse(subWorkFlowIndex.Split("|")(0), Nothing) AndAlso Int32.TryParse(subWorkFlowIndex.Split("|")(1), Nothing) Then
                '            Dim selectedBuilding As ctl_BOP_ENDO_Building = (From b In Me.GatherChildrenOfType(Of ctl_BOP_ENDO_Building)(False)
                '                                                             Where b.LocationIndex = CInt(subWorkFlowIndex.Split("|")(0)) AndAlso
                '                                                          b.BuildingIndex = CInt(subWorkFlowIndex.Split("|")(1)) Select b).FirstOrDefault()
                '            If selectedBuilding IsNot Nothing Then
                '                selectedBuilding.OpenAllParentAccordionsOnNextLoad(selectedBuilding.ScrollToControlId)
                '            End If
                '        End If
                '    End If
                '    ' END NEW

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
                    Me.ctl_CPP_ENDO_Liability_LocationList.Populate()
                    Save_FireSaveEvent(False)
                End If
                Me.ctl_CPP_ENDO_Liability_LocationList.Populate()
                Me.ctl_CPP_ENDO_Liability_LocationList.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations
                CheckForLocationAndBuilding()
                ' Need to load the location page here because changing the Business Income ALS checkbox will affect the location.
                Me.ctl_CPR_ENDO_LocationList.Populate()
                Me.ctl_CPR_ENDO_LocationList.Visible = True

                If Not subWorkFlowIndex.Contains("|") Then
                    ctl_CPR_ENDO_LocationList.ActiveLocationIndex = subWorkFlowIndex
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
            Case Common.Workflow.Workflow.WorkflowSection.InlandMarine
                Me.ctl_CPP_ENDO_InlandMarine.Populate()
                Me.ctl_CPP_ENDO_InlandMarine.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctl_CPP_QuoteSummary.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.printFriendly  'Updated 09/21/2021 for BOP Endorsements Task 61506, changed from drivers to printFriendly
                Me.ctl_CPP_PFSummary.Visible = True
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload 'Added 12/21/2020 for CPP Endorsements Task 66834 MLW
                Me.ctl_AttachmentUpload.Visible = True
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
        AddHandlers()
        If Not IsPostBack Then
            DirectCast(Me.Page, BasePage).DirtyFormDiv = "divEditControls"
            Me.Populate()
        End If
    End Sub

    Private Sub AddHandlers()
        AddHandler ctl_CPR_ENDO_LocationList.TriggerTransactionCount, AddressOf TransactionCountRequested
        AddHandler ctl_CPR_ENDO_LocationList.TriggerUpdateRemarks, AddressOf UpdateRemarks
        AddHandler ctl_CPP_ENDO_InlandMarine.TriggerTransactionCount, AddressOf TransactionCountRequested
        AddHandler ctl_CPP_ENDO_InlandMarine.TriggerUpdateRemarks, AddressOf UpdateRemarks
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Select Case TypeOfEndorsement()
            Case EndorsementStructures.EndorsementTypeString.CPP_AmendMailing
                Me.ctlIsuredList.ValidateControl(valArgs)
            Case EndorsementStructures.EndorsementTypeString.CPP_AddDeleteLocationLienholder
                Me.ctl_CPR_ENDO_LocationList.ValidateControl(valArgs)
                Me.ctl_CPP_ENDO_Liability_LocationList.ValidateControl(valArgs)
            Case EndorsementStructures.EndorsementTypeString.CPP_AddDeleteContractorsEquipmentLienholder
                Me.ctl_CPP_ENDO_InlandMarine.ValidateControl(valArgs)
        End Select



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

        ControlsToValidate_Custom.Add(Me)
        Dim valArgs As New VRValidationArgs(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
        Me.Save_FireSaveEvent(True)
        CheckTranactionLimitAtRate()
        'check  Quote.TransactionReasonId = 10169 for add; 10168 for delete
        Quote.TransactionReasonId = New DevDictionaryHelper.AllCommercialDictionary(Quote).GetTransactionReasonId().ToString

        If Quote?.TransactionRemark?.Length > 1100 Then
            Me.ValidationHelper.AddError("Underwriting overview is required for completion of this endorsement. Please use the Route to Underwriting button.")
        End If

        If Me.ValidationSummmary.HasErrors() = False Then
            Dim saveErr As String = Nothing
            Dim loadErr As String = Nothing

            'updated 2/18/2019
            Dim ratedQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
                'no rate
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Dim successfulEndorsementRate As Boolean = Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedEndorsementQuoteFromContext(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, qqEndorsementResults:=ratedQuote, errorMessage:=saveErr)
            Else
                ratedQuote = Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr)
            End If

            ' Check for quote stop or kill - DM 8/30/2017
            If Me.Quote IsNot Nothing AndAlso (Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled OrElse Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled) Then
                IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Session)
            End If

            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
            Else
                IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId)
            End If

            'ctl_BOP_ENDO_LocationList.Populate()

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
                            ctl_CPP_QuoteSummary.Populate()
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

    Public Overrides Function Save() As Boolean
        Select Case TypeOfEndorsement()
            Case EndorsementStructures.EndorsementTypeString.CPP_AddDeleteLocationLienholder
                Me.ctl_CPR_ENDO_LocationList.Save()
                Me.ctl_CPP_ENDO_Liability_LocationList.Save()
                Me.ControlsToValidate_Custom.Add(ctl_CPR_ENDO_LocationList)
                Me.ControlsToValidate_Custom.Add(ctl_CPP_ENDO_Liability_LocationList)
            Case EndorsementStructures.EndorsementTypeString.CPP_AddDeleteContractorsEquipmentLienholder
                Me.ctl_CPP_ENDO_InlandMarine.Save()
                Me.ControlsToValidate_Custom.Add(ctl_CPP_ENDO_InlandMarine)
            Case EndorsementStructures.EndorsementTypeString.CPP_AmendMailing
                ctlIsuredList.Save()
                Me.ControlsToValidate_Custom.Add(ctlIsuredList)
            Case Else
                Me.SaveChildControls()
        End Select
        ctl_AttachmentUpload.Save() 'Added 12/29/2021 for CPP Endorsements Task 66834 MLW

        Return True
    End Function

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
                ctl_CPR_ENDO_LocationList.Populate()
        End Select
        ctlTreeView.RefreshQuote()
    End Sub



    Private Sub ctlIsuredList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlIsuredList.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctlIsuredList)
    End Sub

    'Added 12/21/2021 for CPP Endorsements Task 66834 MLW
    Private Sub ctl_AttachmentUpload_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_AttachmentUpload.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_AttachmentUpload)
    End Sub

    Private Sub ctl_TreeView_Update() Handles ctl_CPR_ENDO_LocationList.CallTreeUpdate
        ctlTreeView.RefreshQuote()
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

    'Added 12/21/2021 for CPP Endorsements Task 66834 MLW
    Private Sub ctlTreeView_ShowFileUpload(sender As Object, e As EventArgs) Handles ctlTreeView.ShowFileUpload
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "")
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
        'Me.ctl_BOP_ENDO_LocationList.EffectiveDateChanging(NewDate, OldDate)
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

    Private Sub ctlTreeView_ShowInlandMarine() Handles ctlTreeView.ShowInlandMarine
        SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.InlandMarine, "")
    End Sub

#End Region

    'added 11/15/2017 for Equipment Breakdown MBR
#Region "Treeview Updated Events"
    Private _EffDateJustChanged As Boolean

    Private Sub ctlTreeView_QuoteUpdated(sender As Object, e As EventArgs) Handles ctlTreeView.QuoteUpdated
        If _EffDateJustChanged = True Then
            'Me.ctl_BOP_ENDO_LocationList.PopulateLocationCoverages()
        End If
        _EffDateJustChanged = False
    End Sub

    Private Sub ctlTreeView_EffectiveDateChanging(NewEffectiveDate As String, OldEffectiveDate As String) Handles ctlTreeView.EffectiveDateChanging
        _EffDateJustChanged = True
    End Sub

    Private Sub ctlTreeView_EffectiveDateChangedFromTree(qqTranType As QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String) Handles ctlTreeView.EffectiveDateChangedFromTree
        Me.ctl_CPP_QuoteSummary.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
        'Me.ctl_BOP_ENDO_PolicyLevelCoverages.HandleEffectiveDateChange(newEffectiveDate, oldEffectiveDate)
        ' This OH check can be removed after 3/1/2021
        If IFM.VR.Common.Helpers.MultiState.General.SubQuotesContainsState(Quote.MultiStateQuotes, "OH") AndAlso CDate(newEffectiveDate) < CDate("2/1/2021") Then
            IFM.VR.Common.Helpers.MultiState.General.ShowOhioEffectiveDatePopup(Me.Page)
        End If
    End Sub

#End Region

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


#Region "Endorsement Functions"

    Public Sub TransactionCountRequested(Optional ByRef count As Integer = 0)
        Dim TransactionCount As Integer = 0
        Dim CppDictItems = New DevDictionaryHelper.AllCommercialDictionary(Quote)
        If TypeOfEndorsement() = EndorsementStructures.EndorsementTypeString.CPP_AddDeleteContractorsEquipmentLienholder Then
            TransactionCount = CppDictItems.GetTransactionCountInlandMarine(GoverningStateQuote)
        Else
            TransactionCount = CppDictItems.GetTransactionCount()
        End If
        If TransactionCount >= 3 Then
            TransactionLimitReached = True
        Else
            TransactionLimitReached = False
        End If
        count = TransactionCount

    End Sub

    Public Sub UpdateRemarks()
        Dim endorsementsRemarksHelper = New EndorsementsRemarksHelper(Quote)
        Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(EndorsementsRemarksHelper.RemarksType.AllCppRemarks)
        If String.IsNullOrWhiteSpace(updatedRemarks) = False Then
            Quote.TransactionRemark = updatedRemarks
        Else
            Quote.TransactionRemark = "No Changes"
        End If
    End Sub

    Public Sub CheckTranactionLimitAtRate()
        Dim TransactionCount As Integer = 0
        TransactionCountRequested(TransactionCount)
        If TransactionCount > 3 Then
            Me.ValidationHelper.AddError("CPP Endorsements allow a maximum of 3 changes per transaction.  You currently have " & TransactionCount & ". Please reduce the number of changes or route to underwriting for completion.")
        End If
    End Sub

#End Region




End Class