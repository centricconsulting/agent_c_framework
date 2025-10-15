Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Common.Helpers
Imports IFM.ControlFlags
Imports IFM.VR.Flags

Public Class ctl_App_Master_Edit
    Inherits VRMasterControlBase

    Private ReadOnly _flags As IEnumerable(Of IFeatureFlag)

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW

    Public Event QuoteRated()

    Public Sub New()
        MyBase.New()

        _flags = New List(Of IFeatureFlag) From {New LOB.PPA}
    End Sub

    <Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor()>
    Public Sub New(flags As IEnumerable(Of IFeatureFlag))
        _flags = flags
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Me.Populate()
        End If

    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection)
        If Request.QueryString("printAccord") IsNot Nothing Then '7-22-14 Matt A
            ' no nothing but don't do all that other stuff below because it would change the status of the quote from app rated
        Else

            Dim isRated As Boolean = False
            Select Case workflow
                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                    Dim ratedQuote = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()
                    If ratedQuote IsNot Nothing AndAlso ratedQuote.Success Then
                        isRated = True
                        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                    Else
                        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
                    End If
                Case Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions, "")
            End Select

            If isRated = False Then
                ' do MVR and Clue Report Lookups
                If Me.Quote IsNot Nothing Then
                    If Me.Quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso Me.Quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then 'added IF 5/21/2019; previously happening all the time... just for New Business quoting now
                        ctl_OrderClueAndOrMVR.LoadReport(Web.ctl_OrderClueAndOrMVR.ReportRequestType.clueandmv)

                        '5/21/2019 note: this logic is only running for New Business Quoting; Endorsements should not have App side
                        If Me.Quote.Drivers IsNot Nothing Then
                            Dim driverNum As Int32 = 1
                            For Each d In Me.Quote.Drivers
                                If d.MVR_Record_Not_Found Or IFM.VR.Common.Helpers.PPA.MVRHelper.HaveMVRReportForDriver(Me.Quote, driverNum) = False Then
                                    Me.ValidationHelper.AddWarning(New WebValidationItem(String.Format("No MVR for Driver #{0}. Rate may be incorrect and policy must be referred to Underwriting for review.", driverNum), False))
                                    Session(Me.QuoteId + "_HasNoHitMVR") = True
                                End If
                                driverNum += 1
                            Next
                        End If
                    End If
                Else
                    Me.VRScript.AddScriptLine("DisableMainFormOnSaveRemoves();")
                    Me.VRScript.AddScriptLine("alert('This quote must be rated prior to entering the application process.');", True)

                End If

                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                    Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
                ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
                Else
                    Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                End If

            Else
                ' was already rated so not going to do MVR/Clue lookup but still need to do MVR no hit check
                If Me.Quote IsNot Nothing Then
                    If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 5/21/2019; original logic in ELSE
                        'nothing to do for ReadOnly
                    ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                        Session(Me.EndorsementPolicyIdAndImageNum & "_HasNoHitMVR") = IFM.VR.Common.Helpers.PPA.MVRHelper.QuoteHasNoHitMVRRecord(Me.Quote.PolicyId) Or IFM.VR.Common.Helpers.PPA.MVRHelper.AllRatedDriversHaveaMVRReport(Me.Quote) = False
                    Else
                        Session(Me.QuoteId + "_HasNoHitMVR") = IFM.VR.Common.Helpers.PPA.MVRHelper.QuoteHasNoHitMVRRecord(Me.Quote.PolicyId) Or IFM.VR.Common.Helpers.PPA.MVRHelper.AllRatedDriversHaveaMVRReport(Me.Quote) = False
                    End If
                End If
            End If
        End If

    End Sub

    Public Overrides Sub AddScriptAlways()
        Me.VRScript.AddScriptLine("$('#divMasterAppEdit').fadeIn('fast');")
    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            ''RemoveFakeAIs()
            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
                For Each vehicle In Me.Quote.Vehicles
                    If vehicle.ComprehensiveCoverageOnly Then
                        Me.ValidationHelper.AddError($"Vehicle #{vehicle.VehicleNum} - PHYSICAL DAMAGE ONLY (PARKED CAR) is no longer supported. Return to quote to change affected vehicle(s). ")
                    End If
                Next
            End If
            Me.PopulateChildControls()
        End If
    End Sub

    ''' <summary>
    ''' For PPA -
    ''' Remove any fake AI's we added on the quote side for Loan/Lease Bug 32112 MGB 4/15/19
    ''' </summary>
    Private Sub RemoveFakeAIs()
        If Quote.Vehicles IsNot Nothing Then
            For Each veh As QuickQuote.CommonObjects.QuickQuoteVehicle In Quote.Vehicles
                If veh.AdditionalInterests IsNot Nothing Then
                    Dim ndx As Integer = -1
                    For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In veh.AdditionalInterests
                        ndx += 1
                        If ai.Name IsNot Nothing Then
                            If ai.Name.FirstName.ToUpper = "FAKE" AndAlso ai.Name.LastName.ToUpper = "AI" Then
                                veh.AdditionalInterests.RemoveAt(ndx)
                                Save_FireSaveEvent(False)
                                Exit For
                            End If
                        End If
                    Next
                End If
            Next
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Application"
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            Me.SaveChildControls()
            Return True
        End If

        Return False
    End Function

#Region "Work Flows"
    Public Overrides Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions
                _flags.WithFlags(Of LOB.PPA) _
                      .When(Function(ppa) ppa.OhioEnabled AndAlso
                                          Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso
                                          Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio) _
                .Do(Sub()
                        Me.ctlUWQuestions_PPA.Visible = True
                    End Sub) _
                .Else(Sub()
                          Me.ctlUWQuestions.Visible = True
                      End Sub)
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.app
                Me.ctl_App_Section.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload
                Me.ctl_AttachmentUpload.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctlQsummary_PPA.Visible = True
                Me.ctlQsummary_PPA.Populate()
        End Select
        Me.CurrentWorkFlow = workflow
    End Sub

    Public Sub HideAllControls()
        ctlUWQuestions_PPA.Visible = False
        Me.ctlUWQuestions.Visible = False
        Me.ctl_App_Section.Visible = False
        Me.ctl_AttachmentUpload.Visible = False
        Me.ctlQsummary_PPA.Visible = False
    End Sub

#End Region

#Region "Saves"

    Private Sub ctlUWQuestions_PersonalAutoUWQuestionsSaved(sender As Object, QuoteID As String) Handles ctlUWQuestions.PersonalAutoUWQuestionsSaved, ctlUWQuestions_PPA.PersonalAutoUWQuestionsSaved
        ' Martins Control already validated itself
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        'save is actually done by the base
    End Sub

    Private Sub ctlUWQuestions_PersonalAutoUWQuestionsSaved_Endorsements(sender As Object, PolicyId As Integer, PolicyImageNum As Integer) Handles ctlUWQuestions.PersonalAutoUWQuestionsSaved_Endorsements, ctlUWQuestions_PPA.PersonalAutoUWQuestionsSaved_Endorsements
        ' Martins Control already validated itself
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        'save is actually done by the base
    End Sub

    Private Sub ctlUWQuestions_RequestNavigationToApplication(sender As Object, QuoteID As String) Handles ctlUWQuestions.RequestNavigationToApplication, ctlUWQuestions_PPA.RequestNavigationToApplication
        ' Martins Control already validated itself
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        'save is actually done by the base
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
    End Sub

    Private Sub ctlUWQuestions_RequestNavigationToApplication_Endorsements(sender As Object, PolicyId As Integer, PolicyImageNum As Integer) Handles ctlUWQuestions.RequestNavigationToApplication_Endorsements, ctlUWQuestions_PPA.RequestNavigationToApplication_Endorsements
        ' Martins Control already validated itself
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        'save is actually done by the base
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
    End Sub

    Private Sub ctlUWQuestions_RequestNavigationToApplication_ReadOnly(sender As Object, PolicyId As Integer, PolicyImageNum As Integer) Handles ctlUWQuestions.RequestNavigationToApplication_ReadOnly, ctlUWQuestions_PPA.RequestNavigationToApplication_ReadOnly
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.app, "")
    End Sub

    Private Sub ctl_App_Section_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctl_App_Section.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctl_App_Section)
    End Sub

#End Region

    Private Sub ctl_App_Rate_ApplicationRated() Handles ctl_App_Section.QuoteRated
        RaiseEvent QuoteRated() ' Informs tree
    End Sub

    Private Sub ctl_App_Rate_ApplicationRatedSuccessfully() Handles ctl_App_Section.ApplicationRatedSuccessfully
        SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")

    End Sub

    Protected Overrides Sub RateWasRequested()

    End Sub
End Class