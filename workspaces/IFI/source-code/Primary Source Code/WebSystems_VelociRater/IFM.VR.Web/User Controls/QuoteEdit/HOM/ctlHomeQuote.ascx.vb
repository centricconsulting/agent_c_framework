Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Diamond.Business.ThirdParty.GeneralLedger.NetsuiteService

Public Class ctlHomeQuote
    Inherits VRMasterControlBase

    Public Event QuoteRated()

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddScriptLine("$('#divEntireWorkFlow').fadeIn('fast');", True)
    End Sub

    Public Overrides Function Save() As Boolean
        Select Case Me.CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                ctlIsuredList.Save()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_
                ctlProperty_HOM.Save()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                ctlCoverages_HOM.Save()
            Case Else
                'save all - you actually want to not do this if possible
                ctlIsuredList.Save()
                ctlProperty_HOM.Save()
                ctlCoverages_HOM.Save()
        End Select

        'ctlInlandMarine.Save()
        'ctlRVWatercraft.Save()
        'ctlLossHistory2.Save()
        Return True
    End Function

    Public Overrides Sub Populate()
        ctlIsuredList.Populate()
        ctlProperty_HOM.Populate()
        ctlCoverages_HOM.Populate()
        Me.ctlQuoteSummary_HOM.Populate()
        Me.ctl_Personal_NewQuoteForClient.Populate()
        'ctlInlandMarine.Populate()
        'ctlRVWatercraft.Populate()
        'ctlLossHistory2.Populate()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ctlEffectiveDateChecker.ValidateControl(valArgs)
        ctlIsuredList.ValidateControl(valArgs)
        ctlProperty_HOM.ValidateControl(valArgs)
        Me.ctlCoverages_HOM.ValidateControl(valArgs)
        'ctlCoverages_HOM.ValidateControl(valArgs)
        'ctlCoverages_HOM.ValidateForm()
        'ctlInlandMarine.ValidateForm()
        'ctlRVWatercraft.ValidateForm()
        'ctlLossHistory2.ValidateForm()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandler Me.ctlProperty_HOM.PolicyHolderReloadNeeded, AddressOf HandlePHReloadRequest
        If Not IsPostBack Then
            Me.Populate()
        End If
    End Sub

    Private Sub HandlePHReloadRequest()
        Me.ctlIsuredList.Populate()
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection)
        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "")
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                If Me.ctlDisplayDiamondRatingErrors.ValidationHelper.HasErrros = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "")
                End If
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation 'added 7/15/2019 for Home Endorsements Project Task 38926 MLW
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Common.Workflow.Workflow.WorkflowSection.printHistory 'added 7/17/2019 for Home Endorsements Project Task 38927 MLW
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.printHistory, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Common.Workflow.Workflow.WorkflowSection.policyHistory 'added 7/17/2019 for Home Endorsements Project Task 38928 MLW
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
                Else
                    SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
                End If
            Case Else
                SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")
        End Select
    End Sub

#Region "Saves"

    Public Overrides Sub AfterSaveOccurrs(args As VrControlBaseSaveEventArgs)
        Dim qqXML As New QuickQuoteXML()
        Dim reload As Boolean = False
        Dim success As Boolean = False
        Dim reportsOrdered As Boolean = False

        MyBase.AfterSaveOccurrs(args)

        Select Case Me.CurrentWorkFlow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                Me.ctlProperty_HOM.Populate() '8-4-14 Added to make sure property address is newest
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_
                Me.ctlCoverages_HOM.Populate() ' 10-24-14 Added to fix Todd's mining problem

                ' Added 10/10/16 for PCC (MGB)
                If Me.Quote Is Nothing OrElse (Me.Quote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso Me.Quote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage) Then 'added IF 8/2/2019 to only order PPC report for new business quoting; was previously happening every time
                    qqXML.CheckQuoteForProtectionClassInfoAndOrderVeriskReportIfNeeded(Me.Quote, reload, success, reportsOrdered)
                    CallHomIntegrationPreLoad()
                    If reload Then
                        'IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(QuoteId)
                        Me.ctlProperty_HOM.Populate()
                    End If
                End If

            Case Common.Workflow.Workflow.WorkflowSection.coverages
                'ctlCoverages_HOM.Populate()
                'This code is here to help fix an issue with Diamond not ordering the Verisk report properly on HO6 policies. If the report has not been pulled and Protection class Id is still nothing, we will attempt to order the report again and fill in the missing values.
                If Me.Quote Is Nothing OrElse (Me.Quote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso Me.Quote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage) Then 'added IF 8/2/2019 to only order PPC report for new business quoting; was previously happening every time
                    If Me.Quote IsNot Nothing AndAlso Quote.Locations.IsLoaded() AndAlso QQHelper.IsPositiveIntegerString(Quote.Locations(0).ProtectionClassSystemGeneratedId) = False Then
                        qqXML.CheckQuoteForProtectionClassInfoAndOrderVeriskReportIfNeeded(Me.Quote, reload, success, reportsOrdered)
                        CallHomIntegrationPreLoad()
                        If reload Then
                            'IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(QuoteId)
                            Me.ctlProperty_HOM.Populate()
                        End If
                    End If
                End If
        End Select
    End Sub

    Private Sub CallHomIntegrationPreLoad()
        'Used for preloading Cape Analytics, WillisTowersWatson and potentially other integration results which Diamond uses for Rating.
        'If Common.Helpers.HOM.AllHomeRatingHelper.IsAllHomeRatingAvailable(Me.Quote) Then
        '    If Me.Quote?.Locations?.IsLoaded() = True AndAlso Me.Quote?.Locations(0).Address IsNot Nothing Then
        '        Dim chc As New CommonHelperClass
        '        Dim apiKey As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_APIKey")
        '        Dim baseUrl As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_BaseURL")
        '        Dim myAddress = Me.Quote.Locations(0).Address
        '        Dim homRatingRequest = New IFI.Integrations.Request.AllHomeRating(baseUrl, apiKey) With {
        '            .Address1 = GetStreetAddress(myAddress),
        '            .Address2 = "",
        '            .City = myAddress.City,
        '            .State = myAddress.State,
        '            .Zip = myAddress.Zip.Substring(0, 5),
        '            .Country = "US"
        '        }
        '        'Dim response = homRatingRequest.PreLoadHomRatingScores()
        '        'updated 8/31/2023 for latest Integration Nuget package
        '        Dim response = homRatingRequest.PreLoadVendorData()
        '    End If
        'End If
        'updated 8/31/2023
        Dim ih As New IntegrationHelper
        ih.CallHomIntegrationPreLoad(Me.Quote)
    End Sub

    'Private Function GetStreetAddress(myAddress As QuickQuoteAddress) As String '8/31/2023: moved to IntegrationHelper
    '    Dim address As String = ""
    '    If myAddress IsNot Nothing Then
    '        If myAddress.HouseNum.IsNullEmptyorWhitespace() = False OrElse myAddress.StreetName.IsNullEmptyorWhitespace() = False Then
    '            address = myAddress.HouseNum + " " + myAddress.StreetName
    '            If myAddress.ApartmentNumber.IsNullEmptyorWhitespace() = False Then
    '                address = address + " " + myAddress.ApartmentNumber
    '            End If
    '        ElseIf myAddress.POBox.IsNullEmptyorWhitespace() = False Then
    '            address = "PO Box " + myAddress.POBox
    '        End If
    '    End If
    '    Return Regex.Replace(address, " {2,}", " ") 'replace any instance of consecutive spaces with a single space
    'End Function

    Private Sub ctlIsuredList_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlIsuredList.SaveRequested
        Me.ControlsToValidate_Custom.Add(Me.ctlIsuredList)
    End Sub

    Private Sub ctlProperty_HOM_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlProperty_HOM.SaveRequested
        ControlsToValidate_Custom.Add(Me.ctlProperty_HOM)
    End Sub

    Private Sub ctlCoverages_HOM_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlCoverages_HOM.SaveRequested
        ControlsToValidate_Custom.Add(Me.ctlCoverages_HOM)
    End Sub

    Protected Overrides Sub RateWasRequested()
        ControlsToValidate_Custom.Add(Me)

        Me.Save_FireSaveEvent()
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

            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                VR.Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                VR.Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
            Else
                IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId)
            End If

            ctlProperty_HOM.Populate()

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
                    RaiseEvent QuoteRated()
                    If ratedQuote.Success Then
                        If ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped OrElse ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped Then
                            'stay where you are - don't show summary - stop message will be contained in validation messages
                        Else
                            Me.ctlQuoteSummary_HOM.Populate()
                            SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                        End If
                    Else
                        'stay where you are - probably coverages
                        'Added 02/17/2020 for Home Endorsements Bug 43921 MLW
                        'If String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then 'added 11/11/2019
                        'updated 11/20/2020 (Interoperability) to show Route option on NewBusinessQuoting; note: may not need to check for Interoperability since the VR validation would've already been removed
                        If Me.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse (Me.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote AndAlso QuickQuoteHelperClass.AllowVRToUpdateFromDiamond_Interoperability(QuickQuoteObject.QuickQuoteLobType.HomePersonal) = True) Then
                            ctlCoverages_HOM.RouteToUwIsVisible = True
                        End If
                    End If

                End If
            End If
        End If
    End Sub


#End Region

#Region "Work Flows"

    Public Overrides Sub SetCurrentWorkFlow(workflow As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subWorkFlowIndex As String)
        HideAllControls()
        Me.VRScript.AddVariableLine("var DirtyFormException = false;")
        ' workflow = Common.Workflow.Workflow.WorkflowSection.printHistory
        'workflow = Common.Workflow.Workflow.WorkflowSection.policyHistory

        Select Case workflow
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders
                Me.ctlIsuredList.Visible = True
                Me.ctlIsuredList.ActiveInsuredIndex = subWorkFlowIndex
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_
                Me.ctlProperty_HOM.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages
                'ctlCoverages_HOM.ActiveRVWaterIndex = subWorkFlowIndex
                Me.ctlCoverages_HOM.Visible = True
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary
                Me.ctlQuoteSummary_HOM.Visible = True
            Case Common.Workflow.Workflow.WorkflowSection.billingInformation 'added 7/15/2019 for Home Endorsements Project Task 38926 MLW
                Me.ctlBillingInfo.Visible = True
                Me.ctlBillingInfo.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.printHistory 'added 7/17/2019 for Home Endorsements Project Task 38927 MLW
                Me.ctlPrintHistory.Visible = True
                Me.ctlPrintHistory.Populate()
                Me.VRScript.AddVariableLine("var DirtyFormException = true;")
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory 'added 7/17/2019 for Home Endorsements Project Task 38928 MLW
                Me.ctlPolicyHistory.Visible = True
                Me.ctlPolicyHistory.Populate()
            Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.fileUpload 'added 7/19/2019 for Home Endorsements Project Tasks 38922, 38929
                Me.ctl_AttachmentUpload.Visible = True
        End Select
        Me.CurrentWorkFlow = workflow
    End Sub

    Private Sub HideAllControls()
        Me.ctlIsuredList.Visible = False
        Me.ctlProperty_HOM.Visible = False
        Me.ctlCoverages_HOM.Visible = False
        Me.ctlQuoteSummary_HOM.Visible = False
        Me.ctlBillingInfo.Visible = False 'added 7/15/2019 for Home Endorsements Project Task 38926 MLW
        Me.ctlPrintHistory.Visible = False 'added 7/17/2019 for Home Endorsements Project Task 38927 MLW
        Me.ctlPolicyHistory.Visible = False 'added 7/17/2019 for Home Endorsements Project Task 38928 MLW
        Me.ctl_AttachmentUpload.Visible = False 'added 7/19/2019 for Home Endorsements Project Task 38929 MLW
    End Sub

#End Region

    Public Sub EffDateChanged(ByVal NewEffDate As String, ByVal OldEffDate As String)
        Helpers.EffectiveDateHelper.CheckDateCrossing(Quote, NewEffDate, OldEffDate, ValidationHelper.ValidationErrors)
        Me.EffectiveDateChanged(NewEffDate, OldEffDate)
    End Sub

    Private Sub ctlHomeQuote_BroadcastGenericEvent(type As BroadCastEventType) Handles Me.BroadcastGenericEvent
        Select Case type
            Case BroadCastEventType.DoHOMCreditRequest
                'do credit check  --- this will inform the treeview of any changes
                Me.ctl_OrderClueAndOrMVR.LoadReport(Web.ctl_OrderClueAndOrMVR.ReportRequestType.credit)
        End Select
    End Sub

    Private Sub ctl_OrderClueAndOrMVR_JustFinishedCreditOrder() Handles ctl_OrderClueAndOrMVR.JustFinishedCreditOrder 'added 9/27/2019
        Me.ctlIsuredList.SetClientIdTextboxFromQuoteIfNeeded()
    End Sub

    'added 2/18/2020
    Public Sub CheckForReRateAfterEffDateChange(Optional ByVal qqTranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.None, Optional ByVal newEffectiveDate As String = "", Optional ByVal oldEffectiveDate As String = "")
        Me.ctlQuoteSummary_HOM.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
    End Sub

End Class