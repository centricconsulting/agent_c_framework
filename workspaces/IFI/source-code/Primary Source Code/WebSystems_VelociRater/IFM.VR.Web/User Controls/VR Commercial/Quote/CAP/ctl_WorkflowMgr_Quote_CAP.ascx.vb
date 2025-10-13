Imports IFM.VR.Common.Workflow
Imports IFM.VR.Web.Helpers
Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects

Public Class ctl_WorkflowMgr_Quote_CAP
    Inherits VRMasterControlBase

    Public ReadOnly CAPEndorsementsDictionaryName = "CAPEndorsementsDetails" 'Added 04/01/2021 for CAP Endorsements Task 52974 MLW

    'Added 04/01/2021 for CAP Endorsements Task 52974 MLW
    Private Property _devDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
    Public ReadOnly Property ddh() As DevDictionaryHelper.DevDictionaryHelper
        Get
            If _devDictionaryHelper Is Nothing Then
                If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(CAPEndorsementsDictionaryName) = False Then
                    _devDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, CAPEndorsementsDictionaryName, Quote.LobType)
                End If
            End If
            Return _devDictionaryHelper
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddScriptLine("$(""#divEditControls"").fadeIn('fast');")
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As Workflow.WorkflowSection)

        'Added 12/16/2020 for CAP Endorsements Task 52969 MLW
        If IsQuoteEndorsement() Then
            If workflow <> Common.Workflow.Workflow.WorkflowSection.summary AndAlso workflow <> Common.Workflow.Workflow.WorkflowSection.fileUpload Then
                'Dim typeOfEndorsement As String = QQDevDictionary_GetItem("Type_Of_Endorsement_Selected")
                Select Case TypeOfEndorsement()
                    Case "Amend Mailing Address"
                        workflow = Common.Workflow.Workflow.WorkflowSection.policyHolders
                    Case "Add/Delete Vehicle", "Add/Delete Additional Interest"
                        workflow = Common.Workflow.Workflow.WorkflowSection.vehicles
                    Case "Add/Delete Driver"
                        workflow = Common.Workflow.Workflow.WorkflowSection.drivers
                End Select
            End If
        End If

        ctlTreeView.RefreshQuote()
        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "")
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                If Me.ctlDisplayDiamondRatingErrors.ValidationHelper.HasErrros = False Then

                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                Else
                    'Updated 01/14/2021 for CAP Endorsements Task 52976 MLW
                    If IsQuoteEndorsement() Then
                        Select Case TypeOfEndorsement()
                            Case "Amend Mailing Address"
                                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "")
                            Case "Add/Delete Vehicle", "Add/Delete Additional Interest"
                                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles, "")
                            Case "Add/Delete Driver"
                                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers, "")
                        End Select
                    Else
                        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "")
                    End If
                    'SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "")
                End If
                ctlTreeView.RefreshRatedQuote()
            Case Common.Workflow.Workflow.WorkflowSection.printFriendly
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.printFriendly, "")
            Case Common.Workflow.Workflow.WorkflowSection.drivers
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.drivers, "")
            'Case Common.Workflow.Workflow.WorkflowSection.drivers
            '    SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.drivers, "")  ' This enum used for the Print-Friendly summary MGB 'Also used for CAP Endorsements Drivers MLW
            Case Common.Workflow.Workflow.WorkflowSection.vehicles
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.vehicles, "0")  'Added 12/16/2020 for CAP Endorsements Tasks 52969 and 52974 MLW
            Case Common.Workflow.Workflow.WorkflowSection.policyHistory 'Added 11/24/2020 for CAP Endorsements Task 52982 MLW 
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation 'Added 11/24/2020 for CAP Endorsements Task 53231 MLW
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
                Me.ctl_GeneralInfo.Populate()  ' We need to do this here because we won't have the polcyholder address until after the policyholder save
                Me.ctl_GeneralInfo.Visible = True
                'Me.ctl_Cap_Covs.Populate() ' Need to populate the coverages control as well.
                Me.ctl_Cap_Covs.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.vehicles
                ' If there are no vehicles on the quote, create one prior to going to the vehicle page, then populate the vehicle page with the new vehicle
                Session("DoNotValidateVehicles_" & Session.SessionID.ToString) = Nothing

                Dim populateWasFired As Boolean = False 'Added 07/02/2021 for CAP Endorsements code review MLW
                If Me.Quote IsNot Nothing Then
                    ' If no vehicles list create one
                    If Quote.Vehicles Is Nothing Then Quote.Vehicles = New List(Of QuickQuote.CommonObjects.QuickQuoteVehicle)

                    'If (Quote.HasGarageKeepersCollision OrElse Quote.HasGarageKeepersOtherThanCollision) AndAlso (Quote.Vehicles Is Nothing OrElse Quote.Vehicles.Count <= 0) Then
                    'updated 8/13/2018 to use 1st stateQuote instead of main quote
                    If (SubQuoteFirst.HasGarageKeepersCollision OrElse SubQuoteFirst.HasGarageKeepersOtherThanCollision) AndAlso (Quote.Vehicles Is Nothing OrElse Quote.Vehicles.Count <= 0) Then
                        ' If the quote has the Garage Keepers coverage then we don't want an empty vehicle on the quote - Bug 22369
                    Else
                        ' Quote does not have Garage Keeper's coverage so we need an empty vehicle on the quote
                        ' If no vehicles in the list create a new one
                        'Updated 04/06/2021 for CAP Endorsements task 42974 MLW
                        'If Quote.Vehicles.Count <= 0 Then
                        If Quote.Vehicles.Count <= 0 AndAlso Not (IsQuoteEndorsement() OrElse IsQuoteReadOnly()) Then
                            Dim newVeh As New QuickQuote.CommonObjects.QuickQuoteVehicle()
                            ' Set garaging address
                            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                                ' There is a location on the quote which means we have a garaging location.  Copy the garaging location to the vehicle garaging location.
                                newVeh.GaragingAddress.Address = Quote.Locations(0).Address
                            Else
                                ' There is no location.  Set the garaging location to PolicyHolder address
                                newVeh.GaragingAddress.Address = Quote.Policyholder.Address
                            End If
                            Quote.Vehicles.Add(newVeh)

                            'Added 08/02/2021 for CAP Endorsements Task 53030 MLW
                            Dim newVehIndex As Integer = 0
                            If Quote.Vehicles.Count > 0 Then
                                newVehIndex = Quote.Vehicles.Count - 1
                            End If
                            Dim vinDDH As DevDictionaryHelper.DevDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, "ValidVIN", Quote.LobType)
                            vinDDH.AddToMasterValueDictionary(newVehIndex + 1, "False") 'Add as false becaue we haven't given the user a chance to use the VIN lookup yet.

                            Save_FireSaveEvent(False) '04/27/2021 Moved from below for code review 61249 change
                            Populate_FirePopulateEvent() '07/02/2021 moved from below MLW
                            populateWasFired = True 'Added 07/02/2021 for CAP Endorsements code review MLW
                        End If

                        ''Updated 11/25/2020 for CAP Endorsements Task 52981 MLW - throws gs error when saving workflow line 273 below on view only endorsement
                        'If Not IsQuoteReadOnly() AndAlso Not IsQuoteEndorsement() Then
                        '    Save_FireSaveEvent(False)
                        'End If

                        'Populate_FirePopulateEvent() '07/07/2021 moved above MLW
                    End If
                End If
                ' Populate the vehicle control in case we changed the enhancement on the policy coverages page
                'Updated 07/02/2021 for CAP Endorsements code review MLW
                If populateWasFired = False Then
                    Me.ctl_CAP_VehList.Populate()
                End If
                'Me.ctl_CAP_VehList.Populate()
                Me.ctl_CAP_VehList.Visible = True
                If subWorkFlowIndex <> "" AndAlso IsNumeric(subWorkFlowIndex) Then
                    ctl_CAP_VehList.ActiveVehicleIndex = CInt(subWorkFlowIndex)
                End If
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctl_CAP_QuoteSummary.Populate()
                Me.ctl_CAP_QuoteSummary.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.farmIRPM
                Me.ctl_CAP_IRPM.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.printFriendly 'Added 01/12/2021 for CAP Endorsements Task 52976 MLW - moved print friendly from drivers enum to its own printFriendly enum
                Me.Ctl_CAP_PFSummary.Populate()
                Me.Ctl_CAP_PFSummary.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.drivers 'Updated 01/12/2021 for CAP Endorsemets Task 52976 MLW - moved print friendly from drivers enum to its own printFriendly enum, drivers is now only used for drivers 
                Me.ctl_CAP_DriverList.Populate()
                Me.ctl_CAP_DriverList.Visible = True
                If subWorkFlowIndex <> "" AndAlso IsNumeric(subWorkFlowIndex) Then
                    ctl_CAP_DriverList.ActiveDriverIndex = CInt(subWorkFlowIndex)
                End If
                Exit Select
            'Case Common.Workflow.Workflow.WorkflowSection.drivers  ' This enum used for the Print-Friendly summary MGB
            '    'Updated 11/18/2020 for CAP Endorsements task 52972 MLW
            '    If IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
            '        Me.ctl_CAP_DriverList.Populate()
            '        Me.ctl_CAP_DriverList.Visible = True
            '    Else
            '        Me.Ctl_CAP_PFSummary.Populate()
            '        Me.Ctl_CAP_PFSummary.Visible = True
            '    End If
            '    'Me.Ctl_CAP_PFSummary.Populate()
            '    'Me.Ctl_CAP_PFSummary.Visible = True
            '    Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.proposal
                Me.ctlProposalSelection.Populate()
                Me.ctlProposalSelection.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.policyHistory
                'Added 11/24/2020 for CAP Endorsements Task 52982 MLW 
                Me.ctlPolicyHistory.Visible = True
                Me.ctlPolicyHistory.Populate()
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation
                'Added 11/24/2020 for CAP Endorsements Task 53231 MLW 
                Me.ctlBillingInfo.Visible = True
                Me.ctlBillingInfo.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                'Added 03/15/2021 for CAP Endorsements Task 52977 MLW 
                Me.ctl_AttachmentUpload.Visible = True
            Case Else
                Exit Select
        End Select

        Me.CurrentWorkFlow = workflow 'Added 9-28-2018 Have no idea how this ever functioned properly without this here
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
        Me.ctl_GeneralInfo.ValidateControl(valArgs)
        Me.ctl_Cap_Covs.ValidateControl(valArgs)
        Me.ctl_CAP_VehList.ValidateControl(valArgs)
        'Added 01/28/2021 for CAP Endorsements Task 52973 MLW
        If IsQuoteEndorsement() Then
            Me.ctl_CAP_DriverList.ValidateControl(valArgs)
        End If

        MyBase.ValidateControl(valArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Me.ValidationHelper.GroupName = "Policy Level"

        Dim valList = IFM.VR.Validation.ObjectValidation.CommLines.LOB.CAP.PolicyLevelValidations.ValidatePolicyLevel(Me.Quote, valArgs.ValidationType)
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

        Me.Save_FireSaveEvent(True) 'Updated 08/23/2021 for Bug 64452 MLW

        'Added 06/09/2021 for CAP Endorsements Task 52974 MLW
        If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Additional Interest" Then
            Dim vals = IFM.VR.Validation.ObjectValidation.CommLines.LOB.CAP.AdditionalInterestListValidator.ValidateAdditionalInterestList(Me.Quote, valArgs.ValidationType)
            For Each v In vals
                Select Case v.FieldId
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CAP.AdditionalInterestListValidator.AssignAdditionalInterestToVehicle
                        Me.ValidationHelper.AddError(v.Message)
                End Select
            Next
        End If

        If IsQuoteEndorsement() AndAlso Quote?.TransactionRemark?.Length > 1100 Then
            Me.ValidationHelper.AddError("Underwriting overview is required for completion of this endorsement. Please use the Route to Underwriting button.")
        End If

        'Me.Save_FireSaveEvent(True) '08/23/2021 moved above validation for Bug 64452 MLW
        If Me.ValidationSummmary.HasErrors() = False Then
            Dim saveErr As String = Nothing
            Dim loadErr As String = Nothing

            'check for AIs that won't pass validation
            IFM.VR.Common.Helpers.AdditionalInterest.RemoveIncompleteAIs_BOP_CAP(Me.Quote)

            'Dim ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr)
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

            'ctl_BOP_LocationList.Populate()

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

    'added 5/19/2021 to match App side logic for AIs - for CAP Endorsements Task 52974 MLW
    Private _IsSaving As Boolean
    Private _NeedsToRepopulateVehicleList As Boolean

    Public Overrides Function Save() As Boolean
        Select Case CurrentWorkFlow
            Case Workflow.WorkflowSection.policyHolders
                Me.ctlRiskGradeSearch.Save()
                Me.ctlIsuredList.Save() 'Must be first to transfer form field data into Me.Quote


                Dim governingStateFirstLocation = IFM.VR.Common.Helpers.MultiState.Locations.LocationsForGoverningState(Me.Quote, Me.GoverningStateQuote)?.FirstOrDefault()
                If governingStateFirstLocation Is Nothing Then
                    Me.Quote.Locations.AddNew().Address.StateId = Me.GoverningStateQuote.StateId
                    governingStateFirstLocation = IFM.VR.Common.Helpers.MultiState.Locations.LocationsForGoverningState(Me.Quote, Me.GoverningStateQuote).FirstOrDefault()
                End If

                Dim ph As QuickQuote.CommonObjects.QuickQuotePolicyholder = Quote.Policyholder
                If ph?.Address?.StateId = Me.GoverningStateQuote.StateId Then
                    governingStateFirstLocation.Address.HouseNum = ph.Address.HouseNum
                    governingStateFirstLocation.Address.StreetName = ph.Address.StreetName
                    governingStateFirstLocation.Address.City = ph.Address.City
                    governingStateFirstLocation.Address.StateId = ph.Address.StateId
                    governingStateFirstLocation.Address.State = ph.Address.State
                    governingStateFirstLocation.Address.Zip = ph.Address.Zip
                    governingStateFirstLocation.Address.POBox = ph.Address.POBox
                    governingStateFirstLocation.Address.County = ph.Address.County
                End If
            Case Else
                _IsSaving = True 'added 5/19/2021 for CAP Endorsements Task 52974 MLW
                Me.SaveChildControls()
                _IsSaving = False 'added 5/19/2021
                If _NeedsToRepopulateVehicleList = True Then 'added 5/19/2021
                    _NeedsToRepopulateVehicleList = False
                    Me.ctl_CAP_VehList.PopulateAppVehicleInfo()
                End If
        End Select

        Return True
    End Function

    'Added 05/10/2021 for CAP Endorsements Task 52974 MLW
    Private Sub ctl_CAP_VehicleList_AIChanged() Handles ctl_CAP_VehList.AIChanged
        'Save_FireSaveEvent(False)
        'Me.ctl_CAP_VehList.Populate()
        If _IsSaving = True Then
            _NeedsToRepopulateVehicleList = True
        Else
            _NeedsToRepopulateVehicleList = False
            'note from App WorkflowMgr: this logic was previously in ctl_AppSection_CAP.HandleAIChange; not sure if Save is needed, but this likely won't get hit since the AI updated events should fire during save, and populate will happen after if needed (see Save code above)
            Save_FireSaveEvent(False)
            Me.ctl_CAP_VehList.PopulateAppVehicleInfo()
        End If
    End Sub

#Region "Save Requests"

    Public Overrides Sub AfterSaveOccurrs(args As VrControlBaseSaveEventArgs)
        MyBase.AfterSaveOccurrs(args)

        ctlTreeView.RefreshQuote()
    End Sub

    Private Sub ctlIsuredList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlIsuredList.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctlIsuredList)
    End Sub

    Private Sub ctl_Cap_Covs_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_Cap_Covs.SaveRequested, ctl_GeneralInfo.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_GeneralInfo)
        Me.ControlsToValidate_Custom.Add(Me.ctl_Cap_Covs)
    End Sub

    Private Sub ctl_CAP_VehList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_CAP_VehList.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_CAP_VehList)
    End Sub

    'Added 01/25/2021 for CAP Endorsements Task 52973 MLW
    Private Sub ctl_CAP_DriverList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_CAP_DriverList.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_CAP_DriverList)
    End Sub

#End Region



#Region "TreeView Navigations"

    Private Sub ctlTreeView_ShowPolicyholders(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyholders
        SetCurrentWorkFlow(Workflow.WorkflowSection.policyHolders, "")
    End Sub

    Private Sub ctlTreeView_ShowPolicyLevelCoverages(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyLevelCoverages
        'Save_FireSaveEvent(False)
        SetCurrentWorkFlow(Workflow.WorkflowSection.coverages, "")
    End Sub

    Private Sub ctlTreeView_ShowQuoteSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowQuoteSummary
        SetCurrentWorkFlow(Workflow.WorkflowSection.summary, "")
    End Sub

    Private Sub ctlTreeView_ShowIRPM(sender As Object, e As EventArgs) Handles ctlTreeView.ShowIRPM
        SetCurrentWorkFlow(Workflow.WorkflowSection.farmIRPM, "")
    End Sub

    Private Sub ctlTreeView_ShowVehicle(VehicleNumber As Integer) Handles ctlTreeView.EditVehicle
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles, VehicleNumber - 1)
    End Sub
    Private Sub ctlTreeView_GoToVehiclesPage() Handles ctlTreeView.ShowVehicles
        SetCurrentWorkFlow(Workflow.WorkflowSection.vehicles, "")
    End Sub

    'Added 11/23/2020 for CAP Endorsements Tasks 52977 and 52983 MLW
    Private Sub ctlTreeView_ShowDriver(DriverNumber As Integer) Handles ctlTreeView.EditDriver
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers, DriverNumber - 1)
    End Sub
    Private Sub ctlTreeView_GoToDriversPage() Handles ctlTreeView.ShowDrivers
        SetCurrentWorkFlow(Workflow.WorkflowSection.drivers, "")
    End Sub

    'Added 03/15/2021 for CAP Endorsements Task 52977 MLW
    Private Sub ctlTreeView_ShowFileUpload(sender As Object, e As EventArgs) Handles ctlTreeView.ShowFileUpload
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "0")
    End Sub

    Private Sub ctlTreeView_NewVehicle() Handles ctlTreeView.NewVehicle
        Dim addnew As Boolean = False

        If Quote IsNot Nothing Then
            If Quote.Vehicles Is Nothing OrElse Quote.Vehicles.Count <= 0 Then
                ' QUOTE HAS NO VEHICLES
                'If Quote.HasGarageKeepersCollision OrElse Quote.HasGarageKeepersOtherThanCollision Then
                'updated 8/13/2018 to use 1st stateQuote instead of main quote
                If SubQuoteFirst.HasGarageKeepersCollision OrElse SubQuoteFirst.HasGarageKeepersOtherThanCollision Then
                    ' Quote has garage keeper's coverage, navigate to the vehicles page but do not add a vehicle
                    addnew = False
                Else
                    ' Quote does not have GK, add a new vehicle
                    addnew = True
                End If
            Else
                ' QUOTE HAS VEHICLES - ADD NEW VEHICLE
                addnew = True
            End If
        End If

        ' Add new vehicle if required
        If addnew Then
            Quote.Vehicles.Add(New QuickQuote.CommonObjects.QuickQuoteVehicle)
            Save_FireSaveEvent(False)
            Populate_FirePopulateEvent()
        End If

        ' Navigate to the vehicles page
        SetCurrentWorkFlow(Workflow.WorkflowSection.vehicles, "")
    End Sub

    Private Sub ctlTreeView_EffectiveDateChangedFromTree(qqTranType As QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String) Handles ctlTreeView.EffectiveDateChangedFromTree
        Me.ctl_CAP_QuoteSummary.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
        ' This OH check can be removed after 3/1/2021
        If IFM.VR.Common.Helpers.MultiState.General.SubQuotesContainsState(Quote.MultiStateQuotes, "OH") AndAlso CDate(newEffectiveDate) < CDate("2/1/2021") Then
            IFM.VR.Common.Helpers.MultiState.General.ShowOhioEffectiveDatePopup(Me.Page)
        End If
        Helpers.EffectiveDateHelper.CheckDateCrossing(Quote, newEffectiveDate, oldEffectiveDate, ValidationHelper.ValidationErrors)
    End Sub

    'Added 11/24/2020 for CAP Endorsements Task 52982 MLW
    Private Sub ctlTreeView_ShowPolicyHistory(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyHistory
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
    End Sub
    'Added 11/24/2020 for CAP Endorsements Task 53231 MLW
    Private Sub ctlTreeView_ShowBillingInformation(sender As Object, e As EventArgs) Handles ctlTreeView.ShowBillingInformation
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub

    'Added 03/11/2021 for CAP Endorsements Task 52973 MLW
    Private Sub ctlTreeView_NewDriver() Handles ctlTreeView.NewDriver
        If Me.Quote IsNot Nothing Then
            If GoverningStateQuote.Drivers Is Nothing Then GoverningStateQuote.Drivers = New List(Of QuickQuoteDriver)
            Dim newDriver As New QuickQuote.CommonObjects.QuickQuoteDriver()
            GoverningStateQuote.Drivers.Add(newDriver)
            Populate_FirePopulateEvent()
            Save_FireSaveEvent(False)
            If GoverningStateQuote.Drivers.Count > 0 Then
                SetCurrentWorkFlow(Workflow.WorkflowSection.drivers, (Me.GoverningStateQuote.Drivers.Count - 1).ToString)
            Else
                SetCurrentWorkFlow(Workflow.WorkflowSection.drivers, "")
            End If
        End If
    End Sub

    Private Sub ctlTreeView_PopulateCAPCoverages() Handles ctlTreeView.PopulateCAPCoverages
        Me.ctl_Cap_Covs.PopulateCoverageLayout()
        Me.ctl_Cap_Covs.LoadLiabilityStaticData()
        Me.ctl_Cap_Covs.LoadUMStaticData()
        Me.ctl_Cap_Covs.LoadUMPDStaticData()
        Me.ctl_Cap_Covs.LoadMedPayStaticData()
        Me.ctl_Cap_Covs.LoadPolicyCompCollStaticData()
        Me.ctl_Cap_Covs.PopulateLiabilityDropDown()
        Me.ctl_Cap_Covs.PopulateUMDropDown()
        Me.ctl_Cap_Covs.PopulateUMPDCoverage()
        Me.ctl_Cap_Covs.PopulateMedPayDropDown()
        If SubQuoteFirst IsNot Nothing Then
            Me.ctl_Cap_Covs.PopulateHiredBorrowedNonOwned()
            Me.ctl_Cap_Covs.PopulateGarageKeepers()
        End If
        Me.ctl_CAP_VehList.PopulateVehicleCoverages()
    End Sub

#End Region

#Region "Reports"
    'Added 02/11/2021 for CAP Endorsements Tasks 52977 and 52983 MLW
    Private Sub ctlTreeView_ViewDriverMvrReport(driverNumber As Integer) Handles ctlTreeView.ViewDriverMvrReport
        Dim Err As String = Nothing
        Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.PERSONAL_AUTO_GetMVRReport(Me.Quote, driverNumber, Err, True)
        If ReportFile IsNot Nothing Then
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("MVR_{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
            Response.BinaryWrite(ReportFile)
        Else
            Me.VRScript.AddScriptLine("alert('" + Server.HtmlEncode(Err) + "');")
        End If
    End Sub
#End Region

End Class