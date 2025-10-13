Imports IFM.VR.Common.Workflow
Imports QuickQuote.CommonMethods

Public Class ctl_WorkflowManager_App_CGL
    Inherits VRMasterControlBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            DirectCast(Me.Page, BasePage).DirtyFormDiv = "divAppEditControls"
            Me.Populate()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddScriptLine("$(""#divAppEditControls"").fadeIn('fast');")
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As Workflow.WorkflowSection)
        Dim isRated As Boolean = False
        Dim ratedQuote = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()
        If ratedQuote IsNot Nothing AndAlso ratedQuote.Success Then
            isRated = True
        End If

        ctlTreeView.QuoteObject = Me.Quote
        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.app
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary

                If Me.ctlDisplayDiamondRatingErrors.ValidationHelper.HasErrros = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
                End If
                ctlTreeView.RatedQuoteObject = DirectCast(Me.Page, BasePage).Master_VelociRater.GetRatedQuotefromCache()

            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload, "")
            Case Else
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
        End Select

        If isRated = False Then
            'need to app rate it ' usually put into this status by report ordering
            Dim saveErr As String = ""
            Dim loadErr As String = ""

            Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, saveErr, QuickQuoteXML.QuickQuoteSaveType.AppGap)
        End If

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
    End Sub

    Private Sub HideAllControls()
        For Each ctl In Me.ChildVrControls
            ctl.Visible = False
        Next
        Me.ctl_ReturnToQuoteSide.Visible = True 'always visible
    End Sub

    Public Overrides Sub SetCurrentWorkFlow(workflow As Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()
        Select Case workflow
            Case Common.Workflow.Workflow.WorkflowSection.uwQuestions
                ' add uw control here
                Me.ctl_AppSection_CGL.Visible = True ' just for now until you create uw control
            Case Common.Workflow.Workflow.WorkflowSection.app
                Me.ctl_AppSection_CGL.Visible = True
            Case Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctl_Summary_CGL.Visible = True
        End Select
    End Sub



    Protected Overrides Sub RateWasRequested()

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

#Region "Save Requests"
    Private Sub ctl_AppSection_CGL_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_AppSection_CGL.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_AppSection_CGL)
    End Sub

    '' add one for Uw questions


#End Region


#Region "Tree Events"
    Private Sub ctlTreeView_ShowApplication(sender As Object, e As EventArgs) Handles ctlTreeView.ShowApplication
        Me.SetCurrentWorkFlow(Workflow.WorkflowSection.app, "")
    End Sub

    Private Sub ctlTreeView_ShowUnderwritingQuestions(sender As Object, e As EventArgs) Handles ctlTreeView.ShowUnderwritingQuestions
        Me.SetCurrentWorkFlow(Workflow.WorkflowSection.uwQuestions, "")
    End Sub

    Private Sub ctlTreeView_ShowApplicationSummary(sender As Object, e As EventArgs) Handles ctlTreeView.ShowApplicationSummary
        Me.SetCurrentWorkFlow(Workflow.WorkflowSection.summary, "")
    End Sub
#End Region



End Class