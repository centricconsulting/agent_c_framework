Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctl_Master_HOM_APP
    Inherits VRMasterControlBase

    Public Event QuoteRated()

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return False
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub Page_Load1(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Me.Populate()
        End If
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection)
        Dim isRated As Boolean = False
        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Dim ratedQuote = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()
                If ratedQuote IsNot Nothing AndAlso ratedQuote.Success Then
                    isRated = True
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                End If
        End Select

        If isRated = False Then
            ' do Clue Report Lookup
            If Me.Quote IsNot Nothing Then
                ctl_OrderClueAndOrMVR.LoadReport(Web.ctl_OrderClueAndOrMVR.ReportRequestType.clue)
            Else
                Me.LockForm() ' Me.VRScript.AddScriptLine("DisableMainFormOnSaveRemoves();")
                Me.VRScript.AddScriptLine("alert('This quote must be rated prior to entering the application process.');", True)

            End If

            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
            Else
                Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            End If
            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
        End If
    End Sub

#Region "Save Requests"

    Private Sub ctlUWQuestions_SaveRequested(index As Integer, WhichControl As String) Handles ctlUWQuestions.SaveRequested
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub

    Private Sub ctlUWQuestions_RequestNavigationToApplication(sender As Object, QuoteID As String) Handles ctlUWQuestions.RequestNavigationToApplication
        ' Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False))
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
    End Sub

    Private Sub ctlUWQuestions_RequestNavigationToApplication_Endorsements(sender As Object, PolicyId As Integer, PolicyImageNum As Integer) Handles ctlUWQuestions.RequestNavigationToApplication_Endorsements 'added 2/28/2019
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
    End Sub

    Private Sub ctlUWQuestions_RequestNavigationToApplication_ReadOnly(sender As Object, PolicyId As Integer, PolicyImageNum As Integer) Handles ctlUWQuestions.RequestNavigationToApplication_ReadOnly 'added 2/28/2019
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
    End Sub

    Private Sub ctl_HOM_App_Section_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_HOM_App_Section.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_HOM_App_Section)
    End Sub

#End Region

#Region "Work Flows"

    Public Overrides Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()
        
        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions
                Me.ctlUWQuestions.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.app
                Me.ctl_HOM_App_Section.Populate()
                Me.ctl_HOM_App_Section.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctlQuoteSummary_HOM.Populate()
                Me.ctlQuoteSummary_HOM.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                Me.ctl_AttachmentUpload.Visible = True
        End Select
        CurrentWorkFlow = workflow
    End Sub

    Private Sub HideAllControls()
        Me.ctlUWQuestions.Visible = False
        Me.ctl_HOM_App_Section.Visible = False
        Me.ctlQuoteSummary_HOM.Visible = False
        Me.ctl_AttachmentUpload.Visible = False
    End Sub

#End Region

    Private Sub ctl_HOM_App_Section_App_Rate_ApplicationRatedSuccessfully() Handles ctl_HOM_App_Section.App_Rate_ApplicationRatedSuccessfully
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
    End Sub

    Private Sub ctl_HOM_App_Section_QuoteRated() Handles ctl_HOM_App_Section.QuoteRated
        'happens even if rating fails - any rate errors will be show from logic in the ctl_App_Rate control
        RaiseEvent QuoteRated()
    End Sub

    Protected Overrides Sub RateWasRequested()

    End Sub
End Class