Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers

Public Class ctl_WorkflowManager_Quote_fuppup
    Inherits VRMasterControlBase

    Public Event QuoteRated(qq As QuickQuote.CommonObjects.QuickQuoteObject)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            DirectCast(Me.Page, BasePage).DirtyFormDiv = "divAppEditControls"
            Me.Populate()
        End If
        'AddHandler Me.ctlFarmLocationCoverage.GL9Changed, AddressOf HandleGL9Changed
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
            Case Common.Workflow.Workflow.WorkflowSection.printFriendlySummary
                SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.printFriendlySummary, "")
                Exit Select
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
                ctlUmbrellaPolicyCoverages.Save()
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

        Dim valList = IFM.VR.Validation.ObjectValidation.Umbrella.PolicyLevelValidations.ValidatePolicyLevel(Me.Quote, valArgs.ValidationType)
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
                ctlUmbrellaPolicyCoverages.Visible = True
                ctlUmbrellaPolicyCoverages.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                ctl_FUPPUP_QuoteSummary.Visible = True
                Me.ctl_FUPPUP_QuoteSummary.Populate()
            Case Common.Workflow.Workflow.WorkflowSection.printFriendlySummary
                Me.ctl_FUPPUP_PFSummary.Visible = True
                Me.ctl_FUPPUP_PFSummary.Populate()
                Exit Select
            Case Else

        End Select
        Me.CurrentWorkFlow = workflow

#If DEBUG Then
        Debug.WriteLine(String.Format("Workflow Changed -  CurrentWorkflow: {1}  Control:{0}", Me.ToString(), CurrentWorkFlow.ToString()))
#End If
    End Sub
#End Region

#Region "Navigations"


#End Region

#Region "Save Events"
    Public Overrides Sub AfterSaveOccurrs(args As VrControlBaseSaveEventArgs)
        MyBase.AfterSaveOccurrs(args)

        Select Case CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
        End Select
        ctlTreeView.RefreshQuote()
    End Sub

    Private Sub ctl_Insured_Applicant_Manager_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_Insured_Applicant_Manager.SaveRequested
        Me.ControlsToValidate_Custom.Add(ctl_Insured_Applicant_Manager)
    End Sub

    Private Sub ctlUmbrellaPolicyCoverages_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlUmbrellaPolicyCoverages.SaveRequested
        Me.ControlsToValidate_Custom.Add(ctlUmbrellaPolicyCoverages)
    End Sub

    Private Sub ctlCoverages_FUPPUP_QuoteRateRequested() Handles ctlUmbrellaPolicyCoverages.QuoteRateRequested
        RateWasRequested()
    End Sub


    Protected Overrides Sub RateWasRequested()
        ' #NeedsUpdate
        ControlsToValidate_Custom.Add(Me)
        Dim UmbrellaHelp = New UmbrellaUnderlyingValidation
        UmbrellaHelp.ValidateForSaveRateUnderlyingPolicies(Quote, Me.ValidationHelper)

        Dim valArgs As New VRValidationArgs(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, valArgs))
        If Me.ValidationSummmary.HasErrors() = False Then
            'good to rate
            ' do rate
            Dim saveErr As String = Nothing
            Dim loadErr As String = Nothing

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


#End Region

#Region "Tree Events"

    Private Sub ctlTreeView_EditPolicyholder(policyholderNumber As Integer) Handles ctlTreeView.EditPolicyholder, ctlTreeView.NewPolicyholder
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, policyholderNumber - 1)
    End Sub
    Private Sub ctlTreeView_ShowPolicyholders(sender As Object, e As EventArgs) Handles ctlTreeView.ShowPolicyholders
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
    End Sub

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

    Private Sub ctlTreeView_ShowQuoteSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowQuoteSummary
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "0")
    End Sub

    Private Sub ctlTreeView_ShowCoverages(sender As Object, e As EventArgs) Handles ctlTreeView.ShowCoverages
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
    End Sub


    Private Sub ctlTreeView_EffectiveDateChanging(NewQuoteEffectiveDate As String, OldQuoteEffectiveDate As String) Handles ctlTreeView.EffectiveDateChanging
        Me.EffectiveDateChanged(NewQuoteEffectiveDate, OldQuoteEffectiveDate)
    End Sub

#End Region

#Region "Reports"
    ' should look into doing this directly in the treeview
    'Private Sub ctlTreeView_ViewApplicantCreditReport(applicantNumber As Integer) Handles ctlTreeView.ViewApplicantCreditReport
    '    Dim Err As String = Nothing
    '    Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.GetCreditReport(Common.CreditReportSubject.Applicant, Me.Quote, Err, applicantNumber, True)
    '    If ReportFile IsNot Nothing Then
    '        Response.ContentType = "application/pdf"
    '        Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("CreditReport_{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
    '        Response.BinaryWrite(ReportFile)
    '    Else
    '        Me.VRScript.AddScriptLine("alert('" + Server.HtmlEncode(Err) + "');")
    '    End If
    'End Sub



#End Region

End Class