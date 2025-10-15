Imports IFM.VR.Common.QuoteSearch
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects

Public Class ctl_WorkflowManager_Quote_CGL
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
                ' We need to populate the coverages page here because we could have removed EPLI on the locations page
                ' and the coverages page needs to reflect that
                Me.ctl_CGL_PolicyLevelCoverages.Populate()
                Me.ctl_CGL_PolicyLevelCoverages.Visible = True
                Exit Select
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location
                ' If no classifications on the quote at either the policy or location level, we need to create one
                If Not QuoteHasClassifications() Then
                    If Quote.GLClassifications Is Nothing Then Quote.GLClassifications = New List(Of QuickQuote.CommonObjects.QuickQuoteGLClassification)
                    ' Add a new classification
                    Dim newcls As New QuickQuote.CommonObjects.QuickQuoteGLClassification()
                    newcls.ClassCode = "9999999"
                    Quote.GLClassifications.Add(newcls)
                    Me.ctl_CGL_LocationsWF.Populate()
                    Save_FireSaveEvent(False)
                End If
                Me.ctl_CGL_LocationsWF.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.farmIRPM
                Me.ctl_CGL_IRPM.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctl_CGL_QuoteSummary.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.drivers  ' This enum used for the Print-Friendly summary MGB
                Me.Ctl_CGL_PFSummary.Populate()
                Me.Ctl_CGL_PFSummary.Visible = True
                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.farmIRPM
                Me.ctl_CGL_IRPM.Visible = True

                Exit Select
            Case Common.Workflow.Workflow.WorkflowSection.proposal
                Me.ctlProposalSelection.Populate()
                Me.ctlProposalSelection.Visible = True
                Exit Select
            Case Else
                Exit Select
        End Select
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

        Dim valList = IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.PolicyLevelValidations.ValidatePolicyLevel(Me.Quote, valArgs.ValidationType)
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

            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location

        End Select
        ctlTreeView.RefreshQuote()
    End Sub

    Private Sub ctlIsuredList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlIsuredList.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctlIsuredList)

    End Sub

    Private Sub ctl_CGL_GeneralInformation_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_CGL_PolicyLevelCoverages.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_CGL_PolicyLevelCoverages)
    End Sub



    Private Sub ctl_CGL_LocationsWF_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_CGL_LocationsWF.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_CGL_LocationsWF)
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
    Private Sub ctlTreeView_ShowIRPM(sender As Object, e As EventArgs) Handles ctlTreeView.ShowIRPM
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmIRPM, "")
    End Sub

    Private Sub ctlTreeView_ShowQuoteSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowQuoteSummary
        SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.summary, "")
    End Sub

    Private Sub EffectiveDateChanging(ByVal NewDate As String, ByVal OldDate As String) Handles ctlTreeView.EffectiveDateChanging
        Me.ctl_CGL_PolicyLevelCoverages.EffectiveDateChanging(NewDate, OldDate)
        Exit Sub
    End Sub

    Private Sub ctlTreeView_QuoteUpdated(sender As Object, e As EventArgs) Handles ctlTreeView.QuoteUpdated
        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
            VR.Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            VR.Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
        Else
            IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId)
        End If

        Me.ctl_CGL_PolicyLevelCoverages.Populate()
    End Sub

#End Region



    Protected Overrides Sub RateWasRequested()
        ControlsToValidate_Custom.Add(Me)
        Dim valArgs As New VRValidationArgs(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
        Me.Save_FireSaveEvent(True)
        RequestRate.RateWasRequested(Quote, Me, ctlTreeView, ValidationSummmary, Me.ctl_CGL_QuoteSummary)
        'If Me.ValidationSummmary.HasErrors() = False Then
        '    good to rate
        '     Do rate
        '    Dim saveErr As String = Nothing
        '        Dim loadErr As String = Nothing

        '        Dim ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr)
        '        updated 2 / 18 / 2019
        '    Dim ratedQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        '        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
        '            no Rate
        '    ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
        '            Dim successfulEndorsementRate As Boolean = Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedEndorsementQuoteFromContext(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, qqEndorsementResults:=ratedQuote, errorMessage:=saveErr)
        '        Else
        '            ratedQuote = Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr)
        '        End If

        '        Check for quote stop Or kill - DM 8/30/2017
        '    If Me.Quote IsNot Nothing AndAlso (Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled OrElse Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled) Then
        '            IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Session)
        '        End If

        '     set this per page life cycle cache with newest - 6-3-14
        '    If ratedQuote IsNot Nothing Then
        '            DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(False, ratedQuote) 'sets the rated quote cache
        '        Else
        '            you can't set a Nothing quote with this method you'll just have to let it find out for itself that the last rated quote was nothing - should never happen
        '            DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(True)
        '        End If

        '        If String.IsNullOrWhiteSpace(saveErr) = False Or String.IsNullOrWhiteSpace(loadErr) = False Then
        '            failed
        '            If String.IsNullOrWhiteSpace(saveErr) = False Then
        '                Me.ValidationHelper.AddError(saveErr)
        '            End If
        '            If String.IsNullOrWhiteSpace(loadErr) = False Then
        '                Me.ValidationHelper.AddError(loadErr)
        '            End If

        '        Else
        '            did Not fail to call service but may have validation Items
        '        If ratedQuote IsNot Nothing Then
        '                WebHelper_Personal.GatherRatingErrorsAndWarnings(ratedQuote, Me.ValidationHelper)
        '                If ratedQuote.Success Then
        '                    If ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped OrElse ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped Then
        '                        stay where you are - don't show summary - stop message will be contained in validation messages
        '                    Else
        '                        Me.ctl_CGL_QuoteSummary.Populate()
        '                        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
        '                        ctlTreeView.RefreshRatedQuote()
        '                    End If
        '                Else
        '                    stay where you are - probably coverages
        '            End If

        '            End If
        '        End If
        'End If
    End Sub

    Private Sub ctlTreeView_EffectiveDateChangedFromTree(qqTranType As QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String) Handles ctlTreeView.EffectiveDateChangedFromTree
        Me.ctl_CGL_QuoteSummary.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
        ' This OH check can be removed after 3/1/2021
        If IFM.VR.Common.Helpers.MultiState.General.SubQuotesContainsState(Quote.MultiStateQuotes, "OH") AndAlso CDate(newEffectiveDate) < CDate("2/1/2021") Then
            IFM.VR.Common.Helpers.MultiState.General.ShowOhioEffectiveDatePopup(Me.Page)
        End If
        Helpers.EffectiveDateHelper.CheckDateCrossing(Quote, newEffectiveDate, oldEffectiveDate, ValidationHelper.ValidationErrors)
    End Sub
End Class