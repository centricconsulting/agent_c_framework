Imports IFM.VR.Common
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Public MustInherit Class BasePage
    Inherits System.Web.UI.Page

    Public ReadOnly Property Master_VelociRater As VelociRater
        Get
            Try
                Return DirectCast(Me.Page.Master, VelociRater)
            Catch ex As Exception
#If DEBUG Then
                Debugger.Break()
#End If
            End Try
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property Master_Base As BaseMasterPage
        Get
            Try
                Return DirectCast(Me.Page.Master, BaseMasterPage)
            Catch ex As Exception
#If DEBUG Then
                Debugger.Break()
#End If
            End Try
            Return Nothing
        End Get
    End Property
    Public ReadOnly Property PageViewState As StateBag
        Get
            Return Me.ViewState
        End Get
    End Property
    Public Property QuoteSummaryActionsValidationHelper As ControlValidationHelper 'added here from VrControlBaseEssentials for controls that don't inherit from that but still use BasePage (i.e. ctl_Comm_EmailUW)
        Get
            Dim cvh As ControlValidationHelper = Nothing
            Dim vs As StateBag = Me.PageViewState
            If vs IsNot Nothing Then
                If vs("QuoteSummaryActionsValidationHelper") IsNot Nothing Then
                    cvh = CType(vs("QuoteSummaryActionsValidationHelper"), ControlValidationHelper)
                End If
            End If
            Return cvh
        End Get
        Set(value As ControlValidationHelper)
            Dim vs As StateBag = Me.PageViewState
            If vs IsNot Nothing Then
                vs("QuoteSummaryActionsValidationHelper") = value
            End If
        End Set
    End Property
    Public Property RiskGradeSearchIsVisible As Boolean
        Get
            Dim isVisible As Boolean = False
            Dim vs As StateBag = Me.PageViewState
            If vs IsNot Nothing Then
                If vs("RiskGradeSearchIsVisible") IsNot Nothing Then
                    isVisible = CType(vs("RiskGradeSearchIsVisible"), Boolean)
                End If
            End If
            Return isVisible
        End Get
        Set(value As Boolean)
            Dim vs As StateBag = Me.PageViewState
            If vs IsNot Nothing Then
                vs("RiskGradeSearchIsVisible") = value
            End If
        End Set
    End Property
    Public Property InsuredListProcessedCommercialDataPrefillFirmographicsResultsOnLastSave As Boolean
        Get
            Dim justProcessed As Boolean = False
            Dim vs As StateBag = Me.PageViewState
            If vs IsNot Nothing Then
                If vs("InsuredListProcessedCommercialDataPrefillFirmographicsResultsOnLastSave") IsNot Nothing Then
                    justProcessed = CType(vs("InsuredListProcessedCommercialDataPrefillFirmographicsResultsOnLastSave"), Boolean)
                End If
            End If
            Return justProcessed
        End Get
        Set(value As Boolean)
            Dim vs As StateBag = Me.PageViewState
            If vs IsNot Nothing Then
                vs("InsuredListProcessedCommercialDataPrefillFirmographicsResultsOnLastSave") = value
            End If
        End Set
    End Property
    Public Property HasAttemptedCommercialDataPrefillFirmographicsPreload As Boolean
        Get
            Dim hasTried As Boolean = False
            Dim vs As StateBag = Me.PageViewState
            If vs IsNot Nothing Then
                If vs("HasAttemptedCommercialDataPrefillFirmographicsPreload") IsNot Nothing Then
                    hasTried = CType(vs("HasAttemptedCommercialDataPrefillFirmographicsPreload"), Boolean)
                End If
            End If
            Return hasTried
        End Get
        Set(value As Boolean)
            Dim vs As StateBag = Me.PageViewState
            If vs IsNot Nothing Then
                vs("HasAttemptedCommercialDataPrefillFirmographicsPreload") = value
            End If
        End Set
    End Property
    Public Property HasAttemptedCommercialDataPrefillPropertyPreload As Boolean
        Get
            Dim hasTried As Boolean = False
            Dim vs As StateBag = Me.PageViewState
            If vs IsNot Nothing Then
                If vs("HasAttemptedCommercialDataPrefillPropertyPreload") IsNot Nothing Then
                    hasTried = CType(vs("HasAttemptedCommercialDataPrefillPropertyPreload"), Boolean)
                End If
            End If
            Return hasTried
        End Get
        Set(value As Boolean)
            Dim vs As StateBag = Me.PageViewState
            If vs IsNot Nothing Then
                vs("HasAttemptedCommercialDataPrefillPropertyPreload") = value
            End If
        End Set
    End Property

    Public Property DirtyFormDiv As String
        Get
            If ViewState("vs_dirtyFormMainDiv") Is Nothing Then
                ViewState("vs_dirtyFormMainDiv") = ""
            End If
            Return ViewState("vs_dirtyFormMainDiv")
        End Get
        Set(value As String)
            ViewState("vs_dirtyFormMainDiv") = value
        End Set
    End Property

    Public ReadOnly Property VRScript As ctlPageStartupScript
        Get
            Try
                Dim m = DirectCast(Me.Master, VelociRater)
                Return m.StartUpScriptManager
            Catch ex As Exception
#If DEBUG Then
                Debugger.Break()
#End If
            End Try
            Return Nothing
        End Get
    End Property

    Protected ReadOnly Property QuoteId As String
        Get
            Return DirectCast(Me.Master, BaseMasterPage).QuoteId
        End Get
    End Property

    ''' <summary>
    ''' Can return (quote, quote rated, app, and app rated) images. The call will automatically get quote on quote side and app on app side.
    ''' If you want rated image you need to set the property below ' UseRatedQuoteImage'
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            Dim errCreateQSO As String = ""
            If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/15/2019; original logic in ELSE
                Return VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyImageFromAnywhere(ReadOnlyPolicyId, ReadOnlyPolicyImageNum, saveTypeView:=If(Me.Master_Base.IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), errorMessage:=errCreateQSO)
            ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                Return VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteFromAnywhere(EndorsementPolicyId, EndorsementPolicyImageNum, saveTypeView:=If(Me.Master_Base.IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), errorMessage:=errCreateQSO)
            Else
                If Me.Master_Base.IsOnAppPage Then
                    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, QuoteId, errCreateQSO, True, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                Else
                    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, QuoteId, errCreateQSO, True)
                End If
            End If
            Return Nothing
        End Get
    End Property

    'added 2/15/2019
    Protected ReadOnly Property EndorsementPolicyId As Integer
        Get
            Return DirectCast(Me.Master, BaseMasterPage).EndorsementPolicyId
        End Get
    End Property
    Protected ReadOnly Property EndorsementPolicyImageNum As Integer
        Get
            Return DirectCast(Me.Master, BaseMasterPage).EndorsementPolicyImageNum
        End Get
    End Property
    Protected ReadOnly Property EndorsementPolicyIdAndImageNum As String
        Get
            Return DirectCast(Me.Master, BaseMasterPage).EndorsementPolicyIdAndImageNum
        End Get
    End Property
    Protected ReadOnly Property ReadOnlyPolicyId As Integer
        Get
            Return DirectCast(Me.Master, BaseMasterPage).ReadOnlyPolicyId
        End Get
    End Property
    Protected ReadOnly Property ReadOnlyPolicyImageNum As Integer
        Get
            Return DirectCast(Me.Master, BaseMasterPage).ReadOnlyPolicyImageNum
        End Get
    End Property
    Protected ReadOnly Property ReadOnlyPolicyIdAndImageNum As String
        Get
            Return DirectCast(Me.Master, BaseMasterPage).ReadOnlyPolicyIdAndImageNum
        End Get
    End Property

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack = False Then
            If Request.QueryString(IFM.VR.Common.Workflow.Workflow.WorkflowSection_qs) IsNot Nothing Then
                Select Case Request.QueryString(IFM.VR.Common.Workflow.Workflow.WorkflowSection_qs)

                    Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders.ToString()
                        HandleStartUpWorkFlowSelection(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders)
                    Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages.ToString()
                        HandleStartUpWorkFlowSelection(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages)
                    Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                        HandleStartUpWorkFlowSelection(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary)
                    Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.printFriendly.ToString() 'Added 01/12/2021 for CAP Endorsements Task 52976 MLW
                        HandleStartUpWorkFlowSelection(IFM.VR.Common.Workflow.Workflow.WorkflowSection.printFriendly)
                    Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions.ToString()
                        HandleStartUpWorkFlowSelection(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions)

                    Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers.ToString()
                        HandleStartUpWorkFlowSelection(IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers)
                    Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles.ToString()
                        HandleStartUpWorkFlowSelection(IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles)

                    Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_.ToString()

                    Case Else
                        HandleStartUpWorkFlowSelection(IFM.VR.Common.Workflow.Workflow.WorkflowSection.na)
                End Select
            Else
                HandleStartUpWorkFlowSelection(IFM.VR.Common.Workflow.Workflow.WorkflowSection.na)
            End If

        End If
    End Sub

    Private Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If String.IsNullOrWhiteSpace(DirtyFormDiv) = False Then
            Me.VRScript.AddScriptLine(String.Format("ifm.vr.ui.InitDirtyForms(""{0}"");", DirtyFormDiv))
        End If
    End Sub

    Public MustOverride Sub HandleStartUpWorkFlowSelection(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection)

End Class