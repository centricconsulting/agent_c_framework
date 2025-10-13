Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers

Public Class ctl_WorkflowManager_Quote_Farm
    Inherits VRMasterControlBase

    Public Event QuoteRated(qq As QuickQuote.CommonObjects.QuickQuoteObject)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            DirectCast(Me.Page, BasePage).DirtyFormDiv = "divAppEditControls"
            Me.Populate()
        End If
        AddHandler Me.ctlFarmLocationCoverage.GL9Changed, AddressOf HandleGL9Changed
    End Sub

    Private Sub HandleGL9Changed(ByVal ClearIMRV As Boolean)
        Me.ctlTreeView.RefreshQuote()
        If ClearIMRV Then Me.ctlIMRVWatercraft.ClearControl()
        Exit Sub
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
            Case Common.Workflow.Workflow.WorkflowSection.policyHistory 'added 7/17/2019 for Home Endorsements Project Task 38928 MLW
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation 'added 7/15/2019 for Home Endorsements Project Task 38926 MLW
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Else
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
        End Select
    End Sub

#Region "Scripting"
    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddScriptLine("$(""#divAppEditControls"").fadeIn('fast');")
    End Sub
#End Region

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
    End Sub

    Public Overrides Function Save() As Boolean
        Select Case CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                ctl_Insured_Applicant_Manager.Save()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                ctlFarmPolicyCoverages.Save()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location
                'ctlFarmLocationList.Save()
                ctlFarmLocationCoverage.Save()
                ctlFarmPersonalProperty.Save()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmPP
                ctlFarmPersonalProperty.Save()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.InlandMarine
                ctlIMRVWatercraft.Save()
                ctlFarmPersonalProperty.Save()
                'ctlIMRVWater.Save()
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

        Dim valList = IFM.VR.Validation.ObjectValidation.FarmLines.PolicyLevelValidations.ValidatePolicyLevel(Me.Quote, valArgs.ValidationType)
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

#Region "Set workflow"
    Public Sub HideAllControls()
        For Each cntrl As VRControlBase In Me.ChildVrControls
            cntrl.Visible = False
        Next
        '#If DEBUG Then ' just do this for now remove whole debug block later
        '        ctlQuoteSummary_Farm.Visible = True
        '#End If

        Me.ctl_Comm_NewQuoteForClient.Visible = True
    End Sub

    Public Overrides Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                If Me.ctl_Insured_Applicant_Manager.Visible = False Then ' just in case but only if it wasn't already visible otherwise it might wipe data
                    Me.ctl_Insured_Applicant_Manager.Populate()
                End If
                Me.ctl_Insured_Applicant_Manager.Visible = True
                Me.ctl_Insured_Applicant_Manager.ActiveInsuredIndex = subWorkFlowIndex
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                ctlFarmPolicyCoverages.Visible = True
                ctlFarmPolicyCoverages.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location
                ctlFarmLocationCoverage.Visible = True
                ctlFarmLocationCoverage.Populate()  ' Required in case IM or RV was added to the quote.  The GL-9 section needs to know if the quote has IM or RV.
                If subWorkFlowIndex.Contains("|") = False Then
                    ctlFarmLocationCoverage.ActiveLocationIndex = subWorkFlowIndex
                Else
                    'has location index and building index split by a pipe |
                    If Int32.TryParse(subWorkFlowIndex.Split("|")(0), Nothing) AndAlso Int32.TryParse(subWorkFlowIndex.Split("|")(1), Nothing) Then
                        Dim selectedBuilding As ctl_FarBuilding = (From b In Me.GatherChildrenOfType(Of ctl_FarBuilding)(False)
                                                                   Where b.MyLocationIndex = CInt(subWorkFlowIndex.Split("|")(0)) AndAlso
                                                                      b.MyBuildingIndex = CInt(subWorkFlowIndex.Split("|")(1)) Select b).FirstOrDefault()

                        If selectedBuilding IsNot Nothing Then
                            selectedBuilding.OpenAllParentAccordionsOnNextLoad(selectedBuilding.GatherChildrenOfType(Of ctl_FarmBuilding_Property).FirstOrDefault().ScrollToControlId)
                        End If
                    End If
                End If

            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                ctlQuoteSummary_Farm.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmIRPM
                ctlIRPM.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmPP
                ctlFarmPersonalProperty.Visible = True
                ctlFarmPersonalProperty.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.InlandMarine
                ctlIMRVWatercraft.Visible = True
                'ctlIMRVWater.Visible = True
                '    ctlFarmPersonalProperty.Visible = True
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation 'added 7/15/2019 for Home Endorsements Project Task 38926 MLW
                Me.ctlBillingInfo.Visible = True
                Me.ctlBillingInfo.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory 'added 7/17/2019 for Home Endorsements Project Task 38928 MLW
                Me.ctlPolicyHistory.Visible = True
                Me.ctlPolicyHistory.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload 'added 7/19/2019 for Home Endorsements Project Tasks 38922, 38929
                Me.ctl_AttachmentUpload.Visible = True
            Case Else

        End Select
        Me.CurrentWorkFlow = workflow

#If DEBUG Then
        Debug.WriteLine(String.Format("Workflow Changed -  CurrentWorkflow: {1}  Control:{0}", Me.ToString(), CurrentWorkFlow.ToString()))
#End If
    End Sub
#End Region

#Region "Navigations"

    Private Sub ctlFarmLocation_RequestNavigationToCoverages() Handles ctlFarmPolicyCoverages.RequestNavigationToLocation
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, "0")
    End Sub

    Private Sub ctlFarmPolicyCoverages_RequestNavigationToIRPM() Handles ctlQuoteSummary_Farm.RequestNavToIRPM
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmIRPM, "0")
    End Sub

    Private Sub ctlFarmPolicyCoverages_RequestNavigationToPersonalProperty() Handles ctlFarmLocationCoverage.ReqNavFarmPP
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmPP, "0")
    End Sub

    Private Sub ctlFarmPolicyCoverages_RequestNavigationToQuoteSummary() Handles ctlIRPM.ReqNavToQuoteSummary
        If HasRatedQuoteAvailable Then
            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "0")
        Else
            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, "0")
        End If

    End Sub

    Private Sub ctlFarmPersonalProperty_RequestIMNavigation() Handles ctlFarmPersonalProperty.RequestIMNavigation, ctlFarmLocationCoverage.RequestIMNavigation
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.InlandMarine, "0")
    End Sub
#End Region

#Region "Save Events"
    Public Overrides Sub AfterSaveOccurrs(args As VrControlBaseSaveEventArgs)
        MyBase.AfterSaveOccurrs(args)

        Select Case CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                ctlFarmPolicyCoverages.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.InlandMarine
                ctlIMRVWatercraft.Populate()
        End Select
        ctlTreeView.RefreshQuote()
    End Sub

    Private Sub ctl_Insured_Applicant_Manager_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_Insured_Applicant_Manager.SaveRequested
        Me.ControlsToValidate_Custom.Add(ctl_Insured_Applicant_Manager)
    End Sub

    Private Sub ctlFarmPolicyCoverages_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlFarmPolicyCoverages.SaveRequested
        Me.ControlsToValidate_Custom.Add(ctlFarmPolicyCoverages)
    End Sub

    Private Sub ctlFarmPersonalProperty_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlFarmPersonalProperty.SaveRequested
        Me.ControlsToValidate_Custom.Add(ctlFarmPersonalProperty)
    End Sub

    Private Sub ctlFarmLocationCoverage_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlFarmLocationCoverage.SaveRequested
        Me.ControlsToValidate_Custom.Add(ctlFarmLocationCoverage)
    End Sub

    Private Sub ctlIMRVWatercraft_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlIMRVWatercraft.SaveRequested
        Me.ControlsToValidate_Custom.Add(ctlIMRVWatercraft)
    End Sub

    Private Sub ctlIRPM_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlIRPM.SaveRequested
        Me.ControlsToValidate_Custom.Add(ctlIRPM)
    End Sub

    Protected Overrides Sub RateWasRequested()
        ControlsToValidate_Custom.Add(Me)
        Dim valArgs As New VRValidationArgs(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, valArgs))
        If Me.ValidationSummmary.HasErrors() = False Then

            'check for AIs that won't pass validation
            If Me.Quote IsNot Nothing Then
                If Me.Quote.Locations IsNot Nothing Then
                    For Each l As QuickQuote.CommonObjects.QuickQuoteLocation In Me.Quote.Locations
                        Dim removeList As New List(Of Int32)
                        Dim index As Int32 = 0
                        If l.AdditionalInterests IsNot Nothing Then
                            For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In l.AdditionalInterests
                                If Common.Helpers.AdditionalInterest.AiIsComplete(ai) = False Then
                                    removeList.Add(index)
                                End If
                                index += 1
                            Next
                            removeList.Reverse() ' must do this to remove the proper indexes
                            For Each i In removeList
                                l.AdditionalInterests.RemoveAt(i)
                            Next
                        End If
                    Next
                End If
            End If

            If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Dim l As QuickQuote.CommonObjects.QuickQuoteLocation = Me.Quote.Locations(0)
                If l.SectionIICoverages IsNot Nothing Then
                    '' update 'NumberOfPersonsReceivingCare' so that if we delete a record that will be reflected on the quote side
                    Dim famMed As QuickQuoteSectionIICoverage = (From cov In l.SectionIICoverages Where cov.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments Select cov).FirstOrDefault()
                    If famMed IsNot Nothing Then
                        IFM.VR.Common.Helpers.FARM.ResidentNameHelper.RemoveIncompleteResidentNames(Me.Quote)
                    Else
                        IFM.VR.Common.Helpers.FARM.ResidentNameHelper.RemoveAllResidentNames(Me.Quote)
                    End If
                Else
                    IFM.VR.Common.Helpers.FARM.ResidentNameHelper.RemoveAllResidentNames(Me.Quote)
                End If
            End If

            'Matt A 8-3-15
            If IFM.VR.Common.Helpers.FARM.AcresOnlyHelper.HasIncompleteAcreageOnlyRecords(Me.Quote) Then
                IFM.VR.Common.Helpers.FARM.AcresOnlyHelper.RemoveIncompleteAcreageOnlyRecords(Me.Quote)
            End If

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

            ctlIMRVWatercraft.Populate()
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
                    RaiseEvent QuoteRated(ratedQuote) ' always fire so tree gets even attempt rates 4-14-14
                    'If ratedQuote.Success Then
                    '    ctlQuoteSummary_Farm.Populate()
                    '    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                    '    ctlTreeView.RefreshRatedQuote()
                    'Else
                    '    'stay where you are - probably coverages
                    'End If

                    If ratedQuote.Success Then
                        If ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped OrElse ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped Then
                            'stay where you are - don't show summary - stop message will be contained in validation messages
                        Else
                            ctlQuoteSummary_Farm.Populate()
                            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                            ctlTreeView.RefreshRatedQuote()
                        End If
                    Else
                        'stay where you are - probably coverages
                        'added 12/3/2020 (Interoperability); note: may not need to check for Interoperability since the VR validation would've already been removed
                        If Me.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote AndAlso QuickQuoteHelperClass.AllowVRToUpdateFromDiamond_Interoperability(QuickQuoteObject.QuickQuoteLobType.Farm) = True Then
                            If Me.ctlIRPM.Visible = True Then 'just for IRPM for now; may need to start showing RouteToUW on Coverages screen too
                                Me.ctlIRPM.RouteToUwIsVisible = True
                            ElseIf Me.ctlFarmLocationCoverage.Visible = True Then 'added 1/7/2021 (Interoperability)
                                Me.ctlFarmLocationCoverage.RouteToUwIsVisible = True
                            End If
                        End If
                    End If

                End If
            End If
        End If
    End Sub

    Private Sub ctlCoverages_FAR_QuoteRateRequested() Handles ctlFarmLocationCoverage.QuoteRateRequested, ctlIRPM.RaiseReRate, ctlFarmPersonalProperty.RatePersonalProperty, ctl_Insured_Applicant_Manager.QuoteRateRequested, ctlFarmPolicyCoverages.QuoteRateRequested ', ctlIMRVWatercraft.QuoteRateRequested
        RateWasRequested()
    End Sub
#End Region

#Region "Tree Events"

    Private Sub ctlTreeView_ClearDwelling(locationNumber As Integer) Handles ctlTreeView.ClearDwelling
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, locationNumber - 1) 'change to be more specific
        'next need to call Clear for loaded dwelling and then Save
        ctlFarmLocationCoverage.ClearDwellingFromTree(locationNumber - 1)
    End Sub

    Private Sub ctlTreeView_NewLocation(locationNumber As Integer) Handles ctlTreeView.NewLocation
        ctlFarmLocationCoverage.CreateNewLocationFromTree()
        Dim newLocIdx As Integer = 0
        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
            newLocIdx = Quote.Locations.Count - 1
        End If
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, newLocIdx.ToString)
    End Sub
    Private Sub ctlTreeView_EditPolicyholder(policyholderNumber As Integer) Handles ctlTreeView.EditPolicyholder, ctlTreeView.NewPolicyholder
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, policyholderNumber - 1)
    End Sub
    Private Sub ctlTreeView_ShowPolicyholders(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyholders
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
    End Sub
    'Private Sub ctlTreeView_ShowIRPM(sender As Object, e As EventArgs) Handles ctlTreeView.ShowFarmIRPM
    '    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmIRPM, "0")
    'End Sub

    Private Sub ctlTreeView_QuoteUpdated(sender As Object, e As EventArgs) Handles ctlTreeView.QuoteUpdated
        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
            VR.Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            VR.Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
        Else
            IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId)
        End If

        Me.Populate()
    End Sub

    Private Sub ctlTreeView_ShowLocations(sender As Object, e As EventArgs) Handles ctlTreeView.ShowLocations
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, "0")
    End Sub

    Private Sub ctlTreeView_ShowQuoteSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowQuoteSummary
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "0")
    End Sub

    Private Sub ctlTreeView_ShowCoverages(sender As Object, e As EventArgs) Handles ctlTreeView.ShowCoverages
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
    End Sub

    Private Sub ctlTreeView_ShowFarmPersonalProperty(sender As Object, e As EventArgs) Handles ctlTreeView.ShowFarmPersonalProperty

        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmPP, "0")
    End Sub

    Private Sub ctlTreeView_ShowInlandMarineAndRvWatercraft(sender As Object, e As EventArgs) Handles ctlTreeView.ShowInlandMarineAndRvWatercraft
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.InlandMarine, "0")
    End Sub

    Private Sub ctlTreeView_ShowPolicyLevelCoverages(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyLevelCoverages
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
    End Sub

    Private Sub ctlTreeView_EditLocation(locationNumber As Integer) Handles ctlTreeView.EditLocation
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, locationNumber - 1)
    End Sub

    Private Sub ctlTreeView_EditLocationBuilding(locationNumber As Integer, buildingNumber As Integer) Handles ctlTreeView.EditLocationBuilding
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, String.Format("{0}|{1}", locationNumber - 1, buildingNumber - 1)) 'change to be more specific
    End Sub

    Private Sub ctlTreeView_EditLocationDwelling(locationNumber As Integer) Handles ctlTreeView.EditLocationDwelling
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, locationNumber - 1) 'change to be more specific
    End Sub

    Private Sub ctlTreeView_ShowIRPM(sender As Object, e As EventArgs) Handles ctlTreeView.ShowIRPM
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmIRPM, "0")
    End Sub

    'Added 7/18/2019 for Home Endorsements Project Tasks 38926, 38927, 38928 MLW
    Private Sub ctlTreeView_ShowPolicyHistory(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyHistory
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
    End Sub
    Private Sub ctlTreeView_ShowBillingInformation(sender As Object, e As EventArgs) Handles ctlTreeView.ShowBillingInformation
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub

    'Added 7/19/2019 for Home Endorsements Project Task 38929 MLW
    Private Sub ctlTreeView_ShowFileUpload(sender As Object, e As EventArgs) Handles ctlTreeView.ShowFileUpload
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "0")
    End Sub

#End Region

#Region "Reports"
    ' should look into doing this directly in the treeview
    Private Sub ctlTreeView_ViewApplicantCreditReport(applicantNumber As Integer) Handles ctlTreeView.ViewApplicantCreditReport
        Dim Err As String = Nothing
        Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.GetCreditReport(Common.CreditReportSubject.Applicant, Me.Quote, Err, applicantNumber, True)
        If ReportFile IsNot Nothing Then
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("CreditReport_{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
            Response.BinaryWrite(ReportFile)
        Else
            Me.VRScript.AddScriptLine("alert('" + Server.HtmlEncode(Err) + "');")
        End If
    End Sub

    Private Sub ctlTreeView_EffectiveDateChanging(NewQuoteEffectiveDate As String, OldQuoteEffectiveDate As String) Handles ctlTreeView.EffectiveDateChanging
        Me.EffectiveDateChanged(NewQuoteEffectiveDate, OldQuoteEffectiveDate)
    End Sub

    Private Sub ctlTreeView_EffectiveDateChangedFromTree(qqTranType As QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String) Handles ctlTreeView.EffectiveDateChangedFromTree
        Me.ctlQuoteSummary_Farm.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
        ' This OH check can be removed after 3/1/2021
        If IFM.VR.Common.Helpers.MultiState.General.SubQuotesContainsState(Quote.MultiStateQuotes, "OH") AndAlso CDate(newEffectiveDate) < CDate("2/1/2021") Then
            IFM.VR.Common.Helpers.MultiState.General.ShowOhioEffectiveDatePopup(Me.Page)
        End If
        Helpers.EffectiveDateHelper.CheckDateCrossing(Quote, newEffectiveDate, oldEffectiveDate, ValidationHelper.ValidationErrors)

    End Sub

#End Region

    Private Sub ctlUWQuestionsPopup_ToggleUWPopupShown() Handles ctlUWQuestionsPopup.ToggleUWPopupShown
        ctl_Farm_Basic_Policy_Info.HidePopup()
    End Sub

    Private Sub ctlTreeView_ViewCluePropertyReport(sender As Object, e As EventArgs) Handles ctlTreeView.ViewCluePropertyReport
        Dim Err As String = Nothing
        Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.PERSONAL_HOME_GetCLUEReport(Me.Quote, Err, True)
        If ReportFile IsNot Nothing Then
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("ClueReport{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
            Response.BinaryWrite(ReportFile)
        Else
            Err = Err.Replace(vbCrLf, "\r\n") ' CAH B51631 convert VB error message to JS error message.
            Me.VRScript.AddScriptLine("alert('" + Server.HtmlEncode(Err) + "');")
        End If
    End Sub

End Class