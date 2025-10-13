Imports IFM.PrimativeExtensions
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctl_BOP_LocationList
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Property ActiveLocationIndex As String
        Get
            Return hdnAccord.Value
        End Get
        Set(value As String)
            hdnAccord.Value = value
        End Set
    End Property

    'Private Sub SetActivePanel(activePanel As String) Handles ctlFarmLocationList.ActivePanel
    '    ActiveLocationIndex = activePanel
    'End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Protected Sub AttachLocationControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim LocControl As ctl_BOP_Location = cntrl.FindControl("ctl_BOP_Location")
            AddHandler LocControl.NewLocationRequested, AddressOf locationControlNewLocationRequested
            AddHandler LocControl.DeleteLocationRequested, AddressOf locationControlDeleteLocationRequested
            AddHandler LocControl.ClearLocationRequested, AddressOf locationControlClearLocationRequested
            AddHandler LocControl.AddLocationBuildingRequested, AddressOf locationAddBuildingRequested
            index += 1
        Next
    End Sub


    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            If WebHelper_Personal.ControlVisibilityIsOkayForCommercialDataPrefillPropertyPreload(Me.Visible) = True AndAlso Me.HasAttemptedCommercialDataPrefillPropertyPreload = False AndAlso WebHelper_Personal.WorkflowIsOkayForCommercialDataPrefillCalls(Me.Quote, request:=Request) = True Then
                'note: will just call preload now if needed so we know it will be called before the actual Prefill call and we don't have to wait until a button click
                Dim ih As New IntegrationHelper
                Dim attemptedServiceCall As Boolean = False
                Dim caughtUnhandledException As Boolean = False
                Dim unhandledExceptionToString As String = ""
                Dim locNumsAttempted As List(Of Integer) = Nothing
                ih.CallCommercialDataPrefill_PropertyOnly_Preload_IfNeeded(Me.Quote, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, locNumsAttempted:=locNumsAttempted)
                Me.HasAttemptedCommercialDataPrefillPropertyPreload = True
                If caughtUnhandledException = True Then
                    Dim preloadError As New IFM.ErrLog_Parameters_Structure()
                    Dim addPreloadInfo As String = WebHelper_Personal.AdditionalInfoTextForCommercialDataPrefillError(Me.Quote, qqHelper:=QQHelper)
                    If locNumsAttempted IsNot Nothing AndAlso locNumsAttempted.Count > 0 Then
                        addPreloadInfo = QQHelper.appendText(addPreloadInfo, "locNumsAttempted: " & QuickQuoteHelperClass.StringForListOfInteger(locNumsAttempted, delimiter:=","), splitter:="; ")
                    End If
                    With preloadError
                        .ApplicationName = "Velocirater Personal"
                        .ClassName = "ctl_BOP_LocationList"
                        .ErrorMessage = unhandledExceptionToString
                        .LogDate = DateTime.Now
                        .RoutineName = "Populate"
                        .StackTrace = "at IntegrationHelper.CallCommercialDataPrefill_PropertyOnly_Preload at IntegrationHelper.CallCommercialDataPrefill_PropertyOnly_Preload_IfNeeded"
                        .AdditionalInfo = addPreloadInfo
                    End With
                    WriteErrorLogRecord(preloadError, "")
                End If
            End If

            Me.Repeater1.DataSource = Me.Quote.Locations
            Me.Repeater1.DataBind()

            Me.FindChildVrControls()

            Dim lIndex As Int32 = 0
            For Each cnt In Me.GatherChildrenOfType(Of ctl_BOP_Location)
                cnt.LocationIndex = lIndex
                cnt.Populate()
                lIndex += 1
            Next

            'Added 09/09/2021 for BOP Endorsements task 63912 MLW
            If IsQuoteReadOnly() Then
                divBOPLocationButtons.Visible = False
                divEndorsementButtons.Visible = True

                Dim policyNumber As String = Me.Quote.PolicyNumber
                Dim imageNum As Integer = 0
                Dim policyId As Integer = 0
                Dim toolTip As String = "Make a change to this policy"
                Dim readOnlyViewPageUrl As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                    readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                End If

                btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                btnMakeAChange.ToolTip = toolTip
                btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl

                ctl_AdditionalInterestList.Visible = True
                ctl_AdditionalInterestList.Populate()
                ctl_Endo_AppliedAdditionalInterestList.Visible = True
                ctl_Endo_AppliedAdditionalInterestList.Populate()
            Else
                divBOPLocationButtons.Visible = True
                divEndorsementButtons.Visible = False
                ctl_AdditionalInterestList.Visible = False
                ctl_Endo_AppliedAdditionalInterestList.Visible = False
            End If
        End If

    End Sub

    'Public Event PopupRequest(ByVal DialogMessage As String, ByVal DialogTitle As String)

    'Private Sub HandlePopupRequest(ByVal DialogMsg As String, ByVal DialogTitle As String)
    '    RaiseEvent PopupRequest(DialogMsg, DialogTitle)
    'End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divMainList.ClientID
        End If

        AttachLocationControlEvents()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Session("BOPCheckLimitEventTrigger") = String.Empty
        If ActiveLocationIndex = "" Then
            ActiveLocationIndex = "false"
        End If

        'SetActivePanel(ActiveLocationIndex)

        If WebHelper_Personal.ControlVisibilityIsOkayForCommercialDataPrefillPropertyPrefill(Me.Visible) = True AndAlso WebHelper_Personal.WorkflowIsOkayForCommercialDataPrefillCalls(Me.Quote, request:=Request) = True Then
            'note: may need to check for unhandledException info stored in session 1st... so we don't try the same thing over and over if LN/SnapLogic is down
            Dim ih As New IntegrationHelper
            Dim attemptedServiceCall As Boolean = False
            Dim caughtUnhandledException As Boolean = False
            Dim unhandledExceptionToString As String = ""
            Dim locNumsAttempted As List(Of Integer) = Nothing
            Dim setAnyMods As Boolean = False
            ih.CallCommercialDataPrefill_PropertyOnly_IfNeeded(Me.Quote, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, locNumsAttempted:=locNumsAttempted, setAnyMods:=setAnyMods)
            If caughtUnhandledException = True Then
                'maybe do something like store flag in session (maybe by location?) similar to what we do from ctlCommercialDataPrefillEntry
                Dim prefillError As New IFM.ErrLog_Parameters_Structure()
                Dim addPrefillInfo As String = WebHelper_Personal.AdditionalInfoTextForCommercialDataPrefillError(Me.Quote, qqHelper:=QQHelper)
                If locNumsAttempted IsNot Nothing AndAlso locNumsAttempted.Count > 0 Then
                    addPrefillInfo = QQHelper.appendText(addPrefillInfo, "locNumsAttempted: " & QuickQuoteHelperClass.StringForListOfInteger(locNumsAttempted, delimiter:=","), splitter:="; ")
                End If
                With prefillError
                    .ApplicationName = "Velocirater Personal"
                    .ClassName = "ctl_BOP_LocationList"
                    .ErrorMessage = unhandledExceptionToString
                    .LogDate = DateTime.Now
                    .RoutineName = "Save"
                    .StackTrace = "at IntegrationHelper.CallCommercialDataPrefill_PropertyOnly at IntegrationHelper.CallCommercialDataPrefill_PropertyOnly_IfNeeded"
                    .AdditionalInfo = addPrefillInfo
                End With
                WriteErrorLogRecord(prefillError, "")
            ElseIf setAnyMods = True Then
                'should re-populate pertinent fields
                Me.PopulateChildControls() 'this will hopefully suffice since all of the fields we'd update are on child controls
            End If
        End If

        Return True
    End Function

    Public Overrides Sub AfterSaveOccurrs(args As VrControlBaseSaveEventArgs)
        MyBase.AfterSaveOccurrs(args)

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    'Private Sub Repeater1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
    '    Dim ctl As ctl_BOP_Location = e.Item.FindControl("ctl_BOP_Location")
    '    If ctl IsNot Nothing Then
    '        AddHandler ctl.NewLocationRequested, AddressOf locationControlNewLocationRequested
    '    End If
    'End Sub

    Private Sub locationControlNewLocationRequested()
        btnAddAnotherLocation_Click(Me, New EventArgs())
    End Sub

    Private Sub locationControlDeleteLocationRequested(ByRef LocIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                If Quote.Locations.HasItemAtIndex(LocIndex) Then Quote.Locations.RemoveAt(LocIndex)
                Populate()
                Save_FireSaveEvent(False)
                'Populate_FirePopulateEvent()
                Me.hdnAccord.Value = (Me.Quote.Locations.Count - 1).ToString
            End If
        End If
    End Sub

    Private Sub btnAddAnotherLocation_Click(sender As Object, e As EventArgs) Handles btnAddAnotherLocation.Click
        Session("BOPCheckLimitEventTrigger") = "AddNewLocation"
        If Quote IsNot Nothing Then
            Save_FireSaveEvent(False)  ' Added 9/15/17 MGB
            If Quote.Locations Is Nothing Then
                Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
            End If
            Quote.Locations.AddNew()
            Populate()
            Save_FireSaveEvent(False)
            'Populate_FirePopulateEvent()
            Me.hdnAccord.Value = (Me.Quote.Locations.Count - 1).ToString
        End If
    End Sub

    Private Sub locationControlClearLocationRequested(ByRef LocIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                ' Remove the location with the data in it then add a new location
                If Quote.Locations.HasItemAtIndex(LocIndex) Then Quote.Locations.RemoveAt(LocIndex)
                Quote.Locations.Insert(LocIndex, New QuickQuote.CommonObjects.QuickQuoteLocation())
                Populate()
                Me.hdnAccord.Value = LocIndex
            End If
        End If
    End Sub

    Private Sub locationAddBuildingRequested(ByRef LocIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                ' Add a new building to the location at the passed index
                If Quote.Locations.HasItemAtIndex(LocIndex) Then
                    If Quote.Locations(LocIndex).Buildings Is Nothing Then
                        Quote.Locations(LocIndex).Buildings = New List(Of QuickQuote.CommonObjects.QuickQuoteBuilding)
                    End If
                    Quote.Locations(LocIndex).Buildings.AddNew()
                    Populate()
                    Save_FireSaveEvent(False)
                    'Populate_FirePopulateEvent()
                    Me.hdnAccord.Value = (LocIndex).ToString
                End If
            End If
        End If
    End Sub

    Private Sub btnSaveAndRate_Click(sender As Object, e As EventArgs) Handles btnSaveAndRate.Click
        Session("BOPCheckLimitEventTrigger") = "RateButton"
        Session("valuationValue") = "False"
        Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
    End Sub

    Private Sub btnSaveLocation_Click(sender As Object, e As EventArgs) Handles btnSaveLocation.Click
        Session("BOPCheckLimitEventTrigger") = "SaveLocButton"
        Save_FireSaveEvent()
        Populate() 'Added 12/31/18 for Bug 30676 MLW
    End Sub

    'Added 09/09/2021 for BOP Endorsements task 63912 MLW
    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    'Added 09/09/2021 for BOP Endorsements task 63912 MLW
    Protected Sub btnViewBillingInformation_Click(sender As Object, e As EventArgs) Handles btnViewBillingInfo.Click
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub

    'added 11/15/2017 for Equipment Breakdown MBR
    Public Sub PopulateLocationCoverages()
        If Me.Repeater1.Items IsNot Nothing AndAlso Me.Repeater1.Items.Count > 0 Then
            For Each cntrl As RepeaterItem In Me.Repeater1.Items
                Dim LocControl As ctl_BOP_Location = cntrl.FindControl("ctl_BOP_Location")
                If LocControl IsNot Nothing Then
                    LocControl.PopulateLocationCoverages()
                End If
            Next
        End If

    End Sub

    Public Sub EffectiveDateChanging(ByVal NewDate As String, ByVal OldDate As String)
        ' Put any code here to handle when the effective date changes
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctlLoc As ctl_BOP_Location = ri.FindControl("ctl_BOP_Location")
            If ctlLoc IsNot Nothing Then ctlLoc.EffectiveDateChanging(NewDate, OldDate)
        Next
        Exit Sub
    End Sub

    Public Sub PopulateBOPBuildingInformation()
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctlLoc As ctl_BOP_Location = ri.FindControl("ctl_BOP_Location")
            If ctlLoc IsNot Nothing Then ctlLoc.PopulateBOPBuildingInformation()
        Next
    End Sub

End Class