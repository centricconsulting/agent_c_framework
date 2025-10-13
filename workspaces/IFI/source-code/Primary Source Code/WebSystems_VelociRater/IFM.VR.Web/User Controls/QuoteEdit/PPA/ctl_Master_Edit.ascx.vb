Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctl_Master_Edit
    Inherits VRMasterControlBase

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW

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
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                SetCurrentWorkFlow(workflow, "0")
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers
                SetCurrentWorkFlow(workflow, "0")
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles
                SetCurrentWorkFlow(workflow, "0")
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                SetCurrentWorkFlow(workflow, "0")
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Dim ratedQuote = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()

                If ratedQuote IsNot Nothing Then
                    If ratedQuote.Success Then
                        SetCurrentWorkFlow(workflow, "0")
                    Else
                        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
                    End If
                End If
            Case Common.Workflow.Workflow.WorkflowSection.printHistory 'added 6/12/2019
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.printHistory, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Common.Workflow.Workflow.WorkflowSection.policyHistory 'added 6/12/2019
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation 'added 6/13/2019
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Else
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
        End Select
        RefreshCoverage()
    End Sub

    Public Overrides Sub AddScriptAlways()
        If Me.Quote IsNot Nothing Then
            Me.VRScript.AddVariableLine("var effectiveDate = new Date('" + Me.Quote.EffectiveDate + "');")
        End If
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddScriptLine("$('#MasterEdit').fadeIn('fast');", True)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.ctlIsuredList.Populate()
        Me.ctlDriverList.Populate()
        Me.ctlVehicleList.Populate()
        Me.ctlCoverage_PPA.Populate()
        'Me.ctlCoverage_PPA_Vehicle_List.Populate()
        Me.ctl_Personal_NewQuoteForClient.Populate()

    End Sub

    Public Overrides Function Save() As Boolean
        Select Case Me.CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                Me.ctlIsuredList.Save()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers
                Me.ctlDriverList.Save()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles
                Me.ctlVehicleList.Save()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                Me.ctlCoverage_PPA.Save()
                ctlCoverage_PPA.Populate()
                'Me.ctlCoverage_PPA_Vehicle_List.Save()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                'do nothing
            Case Else
                Me.ctlIsuredList.Save()
                Me.ctlDriverList.Save()
                Me.ctlVehicleList.Save()
                'This is already performed elsewhere and causes double errors to display -- TLB - 06/30/14
                'Me.ctlCoverage_PPA.Save()
                'Me.ctlCoverage_PPA_Vehicle_List.Save()
                Me.ctlQsummary_PPA.Save() ' should do nothing
        End Select

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Me.ctlEffectiveDateChecker.ValidateControl(valArgs) ' Matt A 9-29-14
        Me.ctlIsuredList.ValidateControl(valArgs)
        Me.ctlDriverList.ValidateControl(valArgs)
        Me.ctlVehicleList.ValidateControl(valArgs)
        Me.ctlCoverage_PPA.ValidateControl(valArgs)
        'Me.ctlCoverage_PPA_Vehicle_List.ValidateControl(valArgs)
        'Me.ctlCoverage_PPA.ValidateForm()
        'Me.ctlCoverage_PPA_Vehicle_List.ValidateForm()
        'Me.ctlQsummary_PPA.ValidateForm() ' should do nothing
    End Sub

#Region "Saves"
    Private Sub ctlIsuredList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlIsuredList.SaveRequested
        ' add any controls that need validated
        Me.ControlsToValidate_Custom.Add(Me.ctlIsuredList)
        'save/validation is actually done by the base
    End Sub

    Private Sub ctlDriverList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlDriverList.SaveRequested
        ' add any controls that need validated
        Me.ControlsToValidate_Custom.Add(Me.ctlDriverList)
        'save/validation is actually done by the base
    End Sub

    Private Sub ctlVehicleList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlVehicleList.SaveRequested
        ' add any controls that need validated
        If Not Me.ControlsToValidate_Custom.Contains(Me.ctlVehicleList) Then
            Me.ControlsToValidate_Custom.Add(Me.ctlVehicleList)
        End If
        'save/validation is actually done by the base
    End Sub

    Private Sub ctlCoverage_PPA_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlCoverage_PPA.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctlCoverage_PPA)
    End Sub

    'Private Sub ctlCoverage_PPA_Vehicle_List_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlCoverage_PPA_Vehicle_List.SaveRequested
    '    Me.ControlsToValidate_Custom.Add(Me.ctlCoverage_PPA_Vehicle_List)
    'End Sub

    Public Overrides Sub AfterSaveOccurrs(args As VrControlBaseSaveEventArgs)
        MyBase.AfterSaveOccurrs(args)
        Select Case Me.CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers
                RefreshVehicles()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles
                RefreshCoverage()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                RefreshCoverage()
        End Select
    End Sub

#End Region

#Region "Show Work flows"

    Public Overrides Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()
        Me.VRScript.AddVariableLine("var DirtyFormException = false;")
        ' workflow = Common.Workflow.Workflow.WorkflowSection.printHistory
        'workflow = Common.Workflow.Workflow.WorkflowSection.policyHistory

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                If Me.ctlIsuredList.Visible = False Then ' just in case but only if it wasn't already visible otherwise it might wipe data
                    Me.ctlIsuredList.Populate()
                End If
                Me.ctlIsuredList.Visible = True
                Me.ctlIsuredList.ActiveInsuredIndex = subWorkFlowIndex
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers
                If Me.ctlDriverList.Visible = False Then ' just in case but only if it wasn't already visible otherwise it might wipe data
                    Me.ctlDriverList.Populate()
                End If
                Me.ctlDriverList.Visible = True
                Me.ctlDriverList.ActiveDriverPane = subWorkFlowIndex
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles
                If Me.ctlVehicleList.Visible = False Then ' just in case but only if it wasn't already visible otherwise it might wipe data
                    Me.ctlVehicleList.Populate()
                End If
                Me.ctlVehicleList.Visible = True
                Me.ctlVehicleList.ActiveVehicleIndex = subWorkFlowIndex
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                Me.ctlCoverage_PPA.Visible = True
                ctlCoverage_PPA.Populate()
                'Me.ctlCoverage_PPA_Vehicle_List.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctlQsummary_PPA.Visible = True
                Me.ctlQsummary_PPA.Populate()
                Me.ctl_Personal_NewQuoteForClient.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.printHistory
                Me.ctlPrintHistory.Visible = True
                Me.ctlPrintHistory.Populate()
                Me.VRScript.AddVariableLine("var DirtyFormException = true;")
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory
                Me.ctlPolicyHistory.Visible = True
                Me.ctlPolicyHistory.Populate()
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation
                Me.ctlBillingInfo.Visible = True
                Me.ctlBillingInfo.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                Me.ctl_AttachmentUpload.Visible = True
        End Select
        Me.CurrentWorkFlow = workflow
    End Sub

    Public Sub HideAllControls()
        Me.ctlCoverage_PPA.Visible = False
        'Me.ctlCoverage_PPA_Vehicle_List.Visible = False
        Me.ctlDriverList.Visible = False
        Me.ctlIsuredList.Visible = False
        Me.ctlVehicleList.Visible = False
        Me.ctlQsummary_PPA.Visible = False
        Me.ctlPrintHistory.Visible = False
        Me.ctlPolicyHistory.Visible = False
        Me.ctlBillingInfo.Visible = False
        Me.ctl_AttachmentUpload.Visible = False
    End Sub

#End Region

#Region "Refreshes"

    Public Sub RefreshInsured()
        Me.ctlIsuredList.Populate()
    End Sub

    Public Sub RefreshDrivers()
        Me.ctlDriverList.Populate()
    End Sub

    Public Sub RefreshVehicles()
        Me.ctlVehicleList.Populate()
    End Sub

    Public Sub RefreshCoverage()
        Me.ctlCoverage_PPA.Populate()
        'Me.ctlCoverage_PPA_Vehicle_List.Populate()
    End Sub
#End Region

#Region "Tree View Create Requests"

    Public Sub CreateNewDriver() ' do this here just to avoid this type of logic outside of the controls
        If Me.Quote IsNot Nothing Then
            '9/17/18 No updates needed for multi-state since this is for PPA only
            If Me.Quote.Drivers Is Nothing Then
                Me.Quote.Drivers = New List(Of QuickQuoteDriver)()
            End If
            Me.Quote.Drivers.Add(New QuickQuote.CommonObjects.QuickQuoteDriver())
            Me.CurrentWorkFlow = IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers
            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers, (Me.Quote.Drivers.Count - 1).ToString())

            Me.RefreshDrivers()
        End If
    End Sub

    Public Sub CreateNewVehicle() ' do this here just to avoid this type of logic outside of the controls
        If Me.Quote IsNot Nothing Then
            If Me.Quote.Vehicles Is Nothing Then
                Me.Quote.Vehicles = New List(Of QuickQuoteVehicle)
            End If

            Dim newVehicle As New QuickQuote.CommonObjects.QuickQuoteVehicle()

            If Quote.Vehicles.Count = 0 Then
                Quote.HasBusinessMasterEnhancement = True
            End If

            IFM.VR.Common.Helpers.PPA.PrefillHelper.SetNewVehicleDefaults(newVehicle, Quote.QuickQuoteState) 'Updated 10/4/18 for multi state MLW - added Quote.QuickQuoteState

            Me.Quote.Vehicles.Add(newVehicle)

            Me.CurrentWorkFlow = IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles
            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

            Me.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles, (Me.Quote.Vehicles.Count - 1).ToString())

            Me.RefreshVehicles()
            Me.RefreshCoverage()
        End If
    End Sub

#End Region

    Public Sub TreeChangedQuote()
        Me.ctlIsuredList.Populate()
        Me.ctlDriverList.Populate()
        Me.ctlVehicleList.Populate()
        Me.ctlCoverage_PPA.Populate()
        'Me.ctlCoverage_PPA_Vehicle_List.Populate()
    End Sub

    'Private Sub ctlCoverage_Policy_Dropdown() Handles ctlCoverage_PPA_Vehicle_List.UpdatePolicyDropDowns
    '    ctlCoverage_PPA.Populate()
    'End Sub

    Protected Overrides Sub RateWasRequested()
        'Dim valArgs As New VRValidationArgs(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
        'Me.ValidateControl(valArgs) ' validate all form controls

        'Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, valArgs))
        'Me.Save() ' save all form controls - in this case we are about to rate so we don't do the normal fire save event
        'Me.ValidateControl(valArgs) ' validate all form controls

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
                            'If String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then 'added 11/11/2019
                            'updated 11/20/2020 (Interoperability) to show Route option on NewBusinessQuoting; note: may not need to check for Interoperability since the VR validation would've already been removed
                            If Me.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse (Me.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote AndAlso QuickQuoteHelperClass.AllowVRToUpdateFromDiamond_Interoperability(QuickQuoteObject.QuickQuoteLobType.AutoPersonal) = True) Then
                                Me.ctlCoverage_PPA.RouteToUwIsVisible = True
                            End If
                        End If

                    End If
                End If
            End If
        End If
    End Sub

    Private Sub ctlCoverage_PPA_Vehicle_List_QuoteRated() Handles ctlCoverage_PPA.QuoteRateRequested, ctlQsummary_PPA.QuoteRateRequested
        RateWasRequested()
    End Sub

    Private Sub ctlCoverage_PPA_Vehicle_List_SaveAllCoverages() Handles ctlCoverage_PPA.SaveAllCoverages
        'Dim valArgs As New VRValidationArgs(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
        'Me.ValidateControl(valArgs) ' validate all form controls

        'Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, valArgs))
        'Me.Save() ' save all form controls - in this case we are about to rate so we don't do the normal fire save event
        'Me.ValidateControl(valArgs) ' validate all form controls

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

            ctlCoverage_PPA.Populate()
        End If
    End Sub

    Private Sub ctl_Master_Edit_BroadcastGenericEvent(type As BroadCastEventType) Handles Me.BroadcastGenericEvent
        Select Case type
            Case BroadCastEventType.PrefillAddedDriversOrVehicles
                'Me.ctlIsuredList.SetClientIdTextboxFromQuoteIfNeeded() 'added 9/18/2019; 9/19/2019 - moved below to new JustFinishedPrefill event
                Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
                Me.Populate()
            Case BroadCastEventType.PreFillRequested 'should be caught by the prefill control itself but isnot because it is sibling - Matt A 8-20-15
                Me.ctlPrefill_PPA.FetchPreFillData()
        End Select
    End Sub

    Private Sub ctlPrefill_PPA_JustFinishedPrefill() Handles ctlPrefill_PPA.JustFinishedPrefill 'added 9/19/2019
        Me.ctlIsuredList.SetClientIdTextboxFromQuoteIfNeeded()
    End Sub

    'added 2/18/2020
    Public Sub CheckForReRateAfterEffDateChange(Optional ByVal qqTranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.None, Optional ByVal newEffectiveDate As String = "", Optional ByVal oldEffectiveDate As String = "")
        Me.ctlQsummary_PPA.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
    End Sub

    Public Sub EffDateChanged(ByVal NewEffDate As String, ByVal OldEffDate As String)
        Me.EffectiveDateChanged(NewEffDate, OldEffDate)
        Helpers.EffectiveDateHelper.CheckDateCrossing(Quote, NewEffDate, OldEffDate, ValidationHelper.ValidationErrors)
    End Sub
End Class