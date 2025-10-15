Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions


Public MustInherit Class VRMasterControlBase
    Inherits VRControlBase

#Region "Properties"

    ''' <summary>
    ''' A list that holds the controls that should be validated when validation is called. This list only lasts for the request lifecycle and its contents are based on the current workflow.
    ''' </summary>
    Public ControlsToValidate_Custom As New List(Of VRControlBase)



    Public ReadOnly Property WorkFlowHistory As List(Of IFM.VR.Common.Workflow.Workflow.WorkflowSection)
        Get
            If Me.ViewState("vs_workflow_history") Is Nothing Then
                Me.ViewState("vs_workflow_history") = New List(Of IFM.VR.Common.Workflow.Workflow.WorkflowSection)
                'DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.workflowPosition = '';")
            Else
                If DirectCast(Me.ViewState("vs_workflow_history"), List(Of IFM.VR.Common.Workflow.Workflow.WorkflowSection)) IsNot Nothing AndAlso DirectCast(Me.ViewState("vs_workflow_history"), List(Of IFM.VR.Common.Workflow.Workflow.WorkflowSection)).Count > 0 Then
                    DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.workflowPosition = '" + DirectCast(Me.ViewState("vs_workflow_history"), List(Of IFM.VR.Common.Workflow.Workflow.WorkflowSection)).Last().ToString() + "';")
                End If
            End If

                Return DirectCast(Me.ViewState("vs_workflow_history"), List(Of IFM.VR.Common.Workflow.Workflow.WorkflowSection))
        End Get
    End Property


    ''' <summary>
    ''' Holds an enum that indicates the current workflow.
    ''' </summary>
    ''' <returns></returns>
    Public Property CurrentWorkFlow As IFM.VR.Common.Workflow.Workflow.WorkflowSection
        Get
            If WorkFlowHistory.IsLoaded Then
                Return WorkFlowHistory.Last()
            End If
            Return IFM.VR.Common.Workflow.Workflow.WorkflowSection.na
        End Get
        Set(value As IFM.VR.Common.Workflow.Workflow.WorkflowSection)
            WorkFlowHistory.Add(value)
        End Set
    End Property

    Public ReadOnly Property CurrentWorkflowName As String
        Get
            Return IFM.VR.Common.Workflow.Workflow.GetWorkflowName(Me.CurrentWorkFlow, Me.Quote.LobType, Me.IsOnAppPage)
        End Get
    End Property

    ''' <summary>
    ''' Holds an enum that indicates the workflow prior to the current.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property PriorWorkFlow As IFM.VR.Common.Workflow.Workflow.WorkflowSection
        Get
            Return If(WorkFlowHistory.HasItemAtIndex(WorkFlowHistory.Count - 2), WorkFlowHistory.GetItemAtIndex(WorkFlowHistory.Count - 2), Me.CurrentWorkFlow)
        End Get
    End Property

    Public ReadOnly Property PriorWorkflowName As String
        Get
            Return IFM.VR.Common.Workflow.Workflow.GetWorkflowName(Me.PriorWorkFlow, Me.Quote.LobType, Me.IsOnAppPage)
        End Get
    End Property

#End Region

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.PerformsDirectSave = True
        Me._ParentVrControl = Me
    End Sub

    Private Sub Page_Load1(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack = False Then
            If Request.QueryString(IFM.VR.Common.Workflow.Workflow.WorkflowSection_qs) IsNot Nothing Then
                Dim wfText As String = Request.QueryString(IFM.VR.Common.Workflow.Workflow.WorkflowSection_qs).Trim()
                Try
                    HandleStartUpWorkFlowSelection(DirectCast([Enum].Parse(GetType(IFM.VR.Common.Workflow.Workflow.WorkflowSection), wfText), IFM.VR.Common.Workflow.Workflow.WorkflowSection))
                Catch ex As Exception
#If DEBUG Then
                    Debugger.Break() ' no startup enum matches 
#End If
                    HandleStartUpWorkFlowSelection(IFM.VR.Common.Workflow.Workflow.WorkflowSection.na)
                End Try
            Else
                HandleStartUpWorkFlowSelection(IFM.VR.Common.Workflow.Workflow.WorkflowSection.na)
            End If

        End If

    End Sub

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        MyBase.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        Me.EffectiveDateChangedNotifyChildControls(NewEffectiveDate, OldEffectiveDate)
    End Sub

    ''' <summary>
    ''' Provides a place to setup the intended startup workflow(as determined by querystrings).
    ''' </summary>
    ''' <param name="workflow"></param>
    Public MustOverride Sub HandleStartUpWorkFlowSelection(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection)

    ''' <summary>
    ''' Provides a place to handle workflow changes.
    ''' </summary>
    ''' <param name="workflow"></param>
    ''' <param name="subWorkFlowIndex">Usually indicates the accordion index that should be opened.</param>
    Public MustOverride Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)

    Private Sub Page_PreRender1(sender As Object, e As EventArgs) Handles Me.PreRender
        Me.VRScript.AddScriptLine(String.Format("$(""a[href^='{0}p=']"").attr('href',$(""a[href^='{0}p=']"").attr('href') + '&s={1}');", ConfigurationManager.AppSettings("VRHelpLink"), CurrentWorkFlow))
    End Sub

    Protected MustOverride Sub RateWasRequested()

    Private Sub VRMasterControlBase_BroadcastGenericEvent(type As BroadCastEventType) Handles Me.BroadcastGenericEvent
        Select Case type
            Case BroadCastEventType.RateRequested
                RateWasRequested()
        End Select

    End Sub

    Public Sub ReturnToPriorWorkflow()
        If Me.PriorWorkFlow <> Common.Workflow.Workflow.WorkflowSection.na Then
            Me.SetCurrentWorkFlow(Me.PriorWorkFlow, "")
        Else
            If Me.IsOnAppPage Then
                Me.SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
            Else
                Me.SetCurrentWorkFlow(Common.Workflow.Workflow.WorkflowSection.policyHolders, "")
            End If
        End If

    End Sub

End Class