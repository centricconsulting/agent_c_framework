Imports IFM.VR.Common.Workflow
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects

Public Class ctl_WorkflowMgr_Quote_WCP
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
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                If Me.ctlDisplayDiamondRatingErrors.ValidationHelper.HasErrros = False Then

                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "")
                End If
                ctlTreeView.RefreshRatedQuote()
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.drivers
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.drivers, "")  ' This enum used for the Print-Friendly summary MGB
                Exit Select
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

    Private Sub HandleClassificationPopulateRequest()
        ' When the classification control requests a populate we need to save the quote and repopulate the classifications
        Dim errrr As String = ""

        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/15/2019; original logic in ELSE
            'no save
        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=errrr)
        Else
            IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, errrr)
        End If
        Me.ctl_WCPCoverages.Populate()

        Exit Sub
    End Sub

    Public Sub HideAllControls()
        For Each cntrl As VRControlBase In Me.ChildVrControls
            cntrl.Visible = False
        Next
        Me.ctl_Comm_NewQuoteForClient.Visible = True
    End Sub

    ''' <summary>
    ''' Returns the number of locations in the passed state
    ''' </summary>
    ''' <param name="qqstate"></param>
    ''' <returns></returns>
    Private Function NumberOfLocationsOnState(qqstate As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) As Integer '12/27/2018 note: this would only be current on initial quote retrieval; changes made to Quote.Locations will not be reflected on SubQuotes until Save
        Dim count As Integer = 0
        Dim foundit As Boolean = False
        Dim stQt As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(qqstate)
        If stQt IsNot Nothing Then
            If stQt.Locations IsNot Nothing Then
                Return stQt.Locations.Count
            End If
        End If
        Return 0
    End Function

    ''' <summary>
    ''' If the quote has any empty locations (location.address.quickquotestate = none) returns true
    ''' Also return the index of the first one it finds in the ByRef parameter IndexOfBlankLocation
    ''' 
    ''' If there are no empty locations found returns false and the IndexOfBlankLocation field will be set to -1
    ''' </summary>
    ''' <returns></returns>
    Private Function QuoteHasBlankLocation(ByRef IndexOfBlankLocation As Integer) As Boolean
        Dim ndx As Integer = -1
        If Quote.Locations IsNot Nothing Then
            For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                ndx += 1
                If L.Address Is Nothing Then
                    IndexOfBlankLocation = ndx
                    Return True
                End If
                If L.Address.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                    IndexOfBlankLocation = ndx
                    Return True
                End If
            Next
        End If
        Return False
    End Function
    Private Function QuoteHasUnassignedLocation(ByRef indexOfUnassignedLocation As Integer) As Boolean 'added 12/27/2018
        Dim ndx As Integer = -1
        If Quote.Locations IsNot Nothing Then
            For Each l As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                ndx += 1
                Dim stateForLoc As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = QQHelper.QuickQuoteStateForLocation(l)
                If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), stateForLoc) = False OrElse stateForLoc = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                    indexOfUnassignedLocation = ndx
                    Return True
                    Exit For 'should exit on Return but added anyway
                End If
            Next
        End If
        Return False
    End Function
    Private Sub AddLocationClassificationIfNeeded(ByRef l As QuickQuote.CommonObjects.QuickQuoteLocation, Optional ByRef needToSave As Boolean = False, Optional ByVal resetNeedToSave As Boolean = False) 'added 12/27/2018
        If resetNeedToSave = True Then
            needToSave = False
        End If

        If l IsNot Nothing Then
            If l.Classifications Is Nothing Then
                l.Classifications = New List(Of QuickQuote.CommonObjects.QuickQuoteClassification)
                needToSave = True
            End If
            If l.Classifications.Count = 0 Then
                Dim newclass As New QuickQuote.CommonObjects.QuickQuoteClassification()
                newclass.ClassificationTypeId = "9999999"
                l.Classifications.Add(newclass)
                needToSave = True
            End If
            If QQHelper.IsPositiveIntegerString(l.Classifications(0).ClassificationTypeId) = False Then
                l.Classifications(0).ClassificationTypeId = "9999999"
                needToSave = True
            End If
        End If
    End Sub

    Public Overrides Sub SetCurrentWorkFlow(workflow As Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                Me.ctlIsuredList.Visible = True
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                ' If no classifications on the quote location create one
                Dim needToSave As Boolean = False

                ' Create any new locations and classifications that are required - need 1 for each state on the quote
                ' 1 Location with 1 classification is required on each quote
                If Quote IsNot Nothing Then
                    Dim quoteStates As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = Quote.QuoteStates 'added 12/27/2018

                    'updated 12/27/2018 to only use the following IF block if multiState effDate w/ multiple states; any old quotes or new single-state quotes will use ELSE
                    If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) = True AndAlso quoteStates IsNot Nothing AndAlso quoteStates.Count > 1 Then
                        'updated 12/27/2018 for scalability... don't explicitly reference states unless needed... code should still work w/o additional updates if new states are added
                        Dim statesWithoutLoc As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = Nothing
                        For Each qs As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState In quoteStates
                            '1st pull all state locations and update state if missing (in the case that it was saved without a state and was defaulted into the 1st state part
                            Dim locsForState As List(Of QuickQuote.CommonObjects.QuickQuoteLocation) = QQHelper.LocationsForQuickQuoteState(Quote.Locations, qs)
                            If locsForState IsNot Nothing AndAlso locsForState.Count > 0 Then
                                Dim locCounter As Integer = 0
                                For Each l As QuickQuote.CommonObjects.QuickQuoteLocation In locsForState
                                    locCounter += 1
                                    Dim locAddressJustInitialized As Boolean = False
                                    If l Is Nothing Then 'shouldn't happen but handled anyway
                                        l = New QuickQuote.CommonObjects.QuickQuoteLocation
                                        locAddressJustInitialized = True
                                        needToSave = True
                                    End If
                                    If l.Address Is Nothing Then
                                        l.Address = New QuickQuote.CommonObjects.QuickQuoteAddress
                                        locAddressJustInitialized = True
                                        needToSave = True
                                    End If
                                    'If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), l.Address.QuickQuoteState) = False AndAlso l.Address.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                                    'better to check for invalid stateId as the location could really have another state on it that isn't a quickQuoteState
                                    'If locAddressJustInitialized = True OrElse QQHelper.IsPositiveIntegerString(l.Address.StateId) = False Then
                                    'updated 12/29/2018 to also handle for potential of new location having address defaulted to IN (HasData will be False if nothing is set except IN for the state)
                                    If locAddressJustInitialized = True OrElse QQHelper.IsPositiveIntegerString(l.Address.StateId) = False OrElse (l.Address.HasData = False AndAlso l.Address.QuickQuoteState <> qs) Then
                                        l.Address.QuickQuoteState = qs
                                        needToSave = True
                                    End If
                                    'check 1st loc for classifications
                                    If locCounter = 1 Then
                                        AddLocationClassificationIfNeeded(l, needToSave:=needToSave, resetNeedToSave:=False)
                                    End If
                                Next
                            Else
                                QuickQuote.CommonMethods.QuickQuoteHelperClass.AddQuickQuoteStateToList(qs, statesWithoutLoc)
                            End If
                        Next
                        If statesWithoutLoc IsNot Nothing AndAlso statesWithoutLoc.Count > 0 Then
                            'add loc and class for each state without a loc
                            For Each s As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState In statesWithoutLoc
                                'see if unassigned loc 1st
                                Dim l As QuickQuote.CommonObjects.QuickQuoteLocation = Nothing
                                Dim ndx As Integer = -1
                                If QuoteHasUnassignedLocation(ndx) = True Then
                                    'use unassigned
                                    l = Quote.Locations(ndx)
                                Else
                                    'create new
                                    l = New QuickQuote.CommonObjects.QuickQuoteLocation
                                    Quote.Locations.Add(l)
                                End If
                                If l.Address Is Nothing Then
                                    l.Address = New QuickQuote.CommonObjects.QuickQuoteAddress
                                End If
                                l.Address.QuickQuoteState = s
                                AddLocationClassificationIfNeeded(l)
                            Next
                            needToSave = True
                        End If
                    Else '12/27/2018 note: ELSE can now be used for multiState effDate that only has 1 state; code below will try to default loc.address.stateId to govStateId
                        ' NOT MULTISTATE
                        If Quote.Locations Is Nothing Then
                            Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
                            needToSave = True
                        End If
                        If Quote.Locations.Count = 0 Then
                            Dim newloc As New QuickQuote.CommonObjects.QuickQuoteLocation()
                            newloc.Address = New QuickQuote.CommonObjects.QuickQuoteAddress
                            'newloc.Address.StateId = "16"
                            'updated 12/27/2018 to try to use governingStateId; will still default to IN if needed
                            Dim stateIdToUse As String = "16"
                            If QQHelper.IsPositiveIntegerString(Quote.StateId) = True Then
                                stateIdToUse = Quote.StateId
                            End If
                            newloc.Address.StateId = stateIdToUse
                            Quote.Locations.Add(newloc)
                            needToSave = True
                        ElseIf Quote.Locations.Count = 1 Then 'added 12/29/2018 to default initial location to use governing state for address if needed
                            Dim isNewAddress As Boolean = False
                            If Quote.Locations(0) Is Nothing Then
                                Quote.Locations(0) = New QuickQuote.CommonObjects.QuickQuoteLocation
                                isNewAddress = True
                                needToSave = True
                            End If
                            If Quote.Locations(0).Address Is Nothing Then
                                Quote.Locations(0).Address = New QuickQuote.CommonObjects.QuickQuoteAddress
                                isNewAddress = True
                                needToSave = True
                            End If
                            If Quote.Locations(0).Address.HasData = False Then
                                isNewAddress = True
                            End If
                            If isNewAddress = True AndAlso QQHelper.IsPositiveIntegerString(Quote.StateId) = True AndAlso QuickQuote.CommonMethods.QuickQuoteHelperClass.isTextMatch(Quote.Locations(0).Address.StateId, Quote.StateId, matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing) = False Then
                                Quote.Locations(0).Address.StateId = Quote.StateId
                                needToSave = True
                            End If
                        End If
                        If Quote.Locations(0).Classifications Is Nothing Then
                            Quote.Locations(0).Classifications = New List(Of QuickQuote.CommonObjects.QuickQuoteClassification)
                            needToSave = True
                        End If
                        If Quote.Locations(0).Classifications.Count = 0 Then
                            Dim newclass As New QuickQuote.CommonObjects.QuickQuoteClassification()
                            newclass.ClassificationTypeId = "9999999"
                            Quote.Locations(0).Classifications.Add(newclass)
                            needToSave = True
                        End If
                    End If

                End If

                If needToSave Then
                    Save_FireSaveEvent(False)
                End If
                ' Me.ctl_WCPCoverages.Populate()
                Populate_FirePopulateEvent() 'ZS 9/8/17   ' MGB We always need to fire this populate because if it's the first time through we want the exp mod values defaulted

                Me.ctl_WCPCoverages.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctl_WCP_QuoteSummary.Populate()
                Me.ctl_WCP_QuoteSummary.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.farmIRPM
                Me.ctl_WCP_IRPM.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.drivers  ' This enum used for the Print-Friendly summary MGB
                Me.Ctl_WCP_PFSummary.Populate()
                Me.Ctl_WCP_PFSummary.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.proposal
                Me.ctlProposalSelection.Populate()
                Me.ctlProposalSelection.Visible = True
                Exit Select

            Case Else
                Exit Select
        End Select
#If DEBUG Then
        'Debug.WriteLine(String.Format("Workflow Changed -  CurrentWorkflow: {1}  Control:{0}", Me.ToString(), CurrentWorkFlow.ToString()))
#End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'AddHandler ctl_WCPCoverages.RequestClassificationPopulate, AddressOf Populate  ' This will repopulate the controls when classifications change MGB 11/5/18
        AddHandler ctl_WCPCoverages.RequestClassificationPopulate, AddressOf HandleClassificationPopulateRequest  ' This will repopulate the controls when classifications change MGB 11/5/18
        If Not IsPostBack Then
            DirectCast(Me.Page, BasePage).DirtyFormDiv = "divEditControls"
            Me.Populate()
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Me.ctlIsuredList.ValidateControl(valArgs)
        'Me.ctl_WCPCoverages.ValidateControl(valArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Me.ValidationHelper.GroupName = "Policy Level"

        Dim valList = IFM.VR.Validation.ObjectValidation.CommLines.LOB.WC.PolicyLevelValidations.ValidatePolicyLevel(Me.Quote, valArgs.ValidationType)
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
        If Me.ValidationSummmary.HasErrors() = False Then
            Dim saveErr As String = Nothing
            Dim loadErr As String = Nothing

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
            'If Me.Quote IsNot Nothing AndAlso (Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled OrElse Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled) Then
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
                    'RaiseEvent QuoteRated(ratedQuote) ' always fire so tree gets even attempt rates 4-14-14
                    If ratedQuote.Success Then
                        If ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped OrElse ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped Then
                            'stay where you are - don't show summary - stop message will be contained in validation messages
                        Else
                            'ctl_BOP_Quote_Summary.Populate()
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

        End Select
        ctlTreeView.RefreshQuote()
    End Sub

    Private Sub ctlIsuredList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlIsuredList.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctlIsuredList)
    End Sub

    Private Sub ctl_WCPCoverages_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_WCPCoverages.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_WCPCoverages)
    End Sub

#End Region

#Region "TreeView Navigations"

    Private Sub ctlTreeView_ShowPolicyholders(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyholders
        SetCurrentWorkFlow(Workflow.WorkflowSection.policyHolders, "")
    End Sub

    Private Sub ctl(sender As Object, e As EventArgs) Handles ctlTreeView.ShowCoverages
        SetCurrentWorkFlow(Workflow.WorkflowSection.coverages, "")
    End Sub

    Private Sub ctlTreeView_ShowQuoteSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowQuoteSummary
        SetCurrentWorkFlow(Workflow.WorkflowSection.summary, "")
    End Sub

    Private Sub ctlTreeView_ShowIRPM(sender As Object, e As EventArgs) Handles ctlTreeView.ShowIRPM
        SetCurrentWorkFlow(Workflow.WorkflowSection.farmIRPM, "")
    End Sub

    Private Sub ctlTreeView_EffectiveDateChanging(NewDate As String, OldDate As String) Handles ctlTreeView.EffectiveDateChanging
        Me.ctlIsuredList.PopulateAfterEffectiveDateChange(NewDate)
        'Populate_FirePopulateEvent()
    End Sub

    Private Sub ctlTreeView_EffectiveDateChangedFromTree(qqTranType As QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String) Handles ctlTreeView.EffectiveDateChangedFromTree
        Me.ctl_WCP_QuoteSummary.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
    End Sub

#End Region

End Class