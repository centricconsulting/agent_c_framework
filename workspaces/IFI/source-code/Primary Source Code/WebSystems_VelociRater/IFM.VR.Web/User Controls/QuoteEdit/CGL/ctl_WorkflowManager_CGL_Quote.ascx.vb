Imports IFM.VR.Common.QuoteSearch
Imports IFM.VR.Web.Helpers

Public Class ctl_WorkflowManager_CGL_Quote
    Inherits VRMasterControlBase

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddScriptLine("$(""#divEditControls"").fadeIn('fast');")
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection)
        ctlTreeView.QuoteObject = Me.Quote
        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "")
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                If Me.ctlDisplayDiamondRatingErrors.ValidationHelper.HasErrros = False Then

                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "")
                End If
                ctlTreeView.RatedQuoteObject = DirectCast(Me.Page, BasePage).Master_VelociRater.GetRatedQuotefromCache()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location

                Dim subWorkFlowText As String = If(Request.QueryString("locationNum") IsNot Nothing AndAlso QQHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("locationNum").ToString()), CInt(Request.QueryString("locationNum")) - 1, "")
                subWorkFlowText += If(Request.QueryString("buildingNum ") IsNot Nothing AndAlso QQHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("buildingNum").ToString()), "|" + CInt(Request.QueryString("buildingNum ")) - 1, "")
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, subWorkFlowText)
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
    End Sub

    Public Overrides Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                Me.ctlIsuredList.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                Me.ctl_CGL_PolicyLevelCoverages.Visible = True

            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location
                Me.ctl_CGL_LocationsWF.Visible = True

            Case Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctl_Summary_CGL.Visible = True
            Case Else

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
    End Sub

#Region "Save Requests"

    Public Overrides Sub AfterSaveOccurrs(args As VrControlBaseSaveEventArgs)
        MyBase.AfterSaveOccurrs(args)

        Select Case CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders

            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages

            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location

        End Select
        Me.ctlTreeView.QuoteObject = Me.Quote
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



#End Region



    Protected Overrides Sub RateWasRequested()
        ControlsToValidate_Custom.Add(Me)
        Dim valArgs As New VRValidationArgs(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
        Me.Save_FireSaveEvent(True)
        If Me.ValidationSummmary.HasErrors() = False Then
            'good to rate
            ' do rate
            Dim saveErr As String = Nothing
            Dim loadErr As String = Nothing

            Dim ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr)

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
                        Me.ctl_Summary_CGL.Populate()
                        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                        Me.ctlTreeView.RatedQuoteObject = ratedQuote
                    Else
                        'stay where you are - probably coverages
                    End If

                End If
            End If
        End If
    End Sub


End Class