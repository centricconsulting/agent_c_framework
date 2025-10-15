Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CPR
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctl_CPR_LocationList
    Inherits VRControlBase

    Public Event LocationChanged(ByVal LocIndex As Integer)
    Public Event BuildingZeroDeductibleChanged()

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

    Private Enum CPRBuildingCoverageType
        BuildingCoverage
        BusinessIncomeCoverage
        PersonalPropertyCoverage
        PersonalPropertyOfOthersCoverage
    End Enum

    Public Overrides Sub AddScriptWhenRendered()
        If Me.divNewLocation.Visible Then
            Me.VRScript.CreateAccordion(Me.divNewLocation.ClientID, Nothing, "false", True)
        Else
            If Me.divLocationList.Visible Then
                Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
            End If
        End If
    End Sub
    Private Sub AddHandlers()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim LocControl As ctl_CPR_Location = cntrl.FindControl("ctl_CPR_Location")
            AddHandler LocControl.LocationChanged, AddressOf HandleAddressChange
            AddHandler LocControl.AddLocationRequested, AddressOf AddNewLocation
            AddHandler LocControl.CopyLocationRequested, AddressOf CopyLocation
            AddHandler LocControl.DeleteLocationRequested, AddressOf DeleteLocation
            AddHandler LocControl.ClearLocationRequested, AddressOf ClearLocation
            AddHandler LocControl.BuildingZeroDeductibleChanged, AddressOf HandleBuildingZeroDeductibleChange
            index += 1
        Next
    End Sub

    Public Sub HandleBuildingZeroDeductibleChange()
        RaiseEvent BuildingZeroDeductibleChanged()
    End Sub

    Public Sub HandleBlanketDeductibleChange()
        Dim myLocations As List(Of ctl_CPR_Location) = Me.GatherChildrenOfType(Of ctl_CPR_Location)()
        For Each L As ctl_CPR_Location In myLocations
            L.HandleBlanketDeductibleChange()
        Next
    End Sub

    Public Sub HandleAgreedAmountChange(ByVal newvalue As Boolean)
        Dim myLocations As List(Of ctl_CPR_Location) = Me.GatherChildrenOfType(Of ctl_CPR_Location)()
        For Each L As ctl_CPR_Location In myLocations
            L.HandleAgreedAmountChange(newvalue)
        Next
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Protected Sub HandleAddressChange(ByVal LocIndex As Integer)
        RaiseEvent LocationChanged(LocIndex)
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
                        .ClassName = "ctl_CPR_LocationList"
                        .ErrorMessage = unhandledExceptionToString
                        .LogDate = DateTime.Now
                        .RoutineName = "Populate"
                        .StackTrace = "at IntegrationHelper.CallCommercialDataPrefill_PropertyOnly_Preload at IntegrationHelper.CallCommercialDataPrefill_PropertyOnly_Preload_IfNeeded"
                        .AdditionalInfo = addPreloadInfo
                    End With
                    WriteErrorLogRecord(preloadError, "")
                End If
            End If

            ' Show the correct buttons based on LOB
            Select Case Quote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                    btnRate.Visible = True
                    btnEmailForUWAssistance.Visible = True
                    btnContinue.Visible = False
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    btnRate.Visible = False
                    btnEmailForUWAssistance.Visible = False
                    btnContinue.Visible = True
                    Exit Select
            End Select

            Me.divNewLocation.Visible = False
            Me.divLocationList.Visible = False
            If Me.Quote.Locations.IsLoaded() Then
                Me.divLocationList.Visible = True
                Me.Repeater1.DataSource = Me.Quote.Locations
                Me.Repeater1.DataBind()
                Me.FindChildVrControls()

                Dim index As Int32 = 0
                For Each Loc As ctl_CPR_Location In Me.GatherChildrenOfType(Of ctl_CPR_Location)
                    Loc.MyLocationIndex = index
                    'Loc.Populate()
                    index += 1
                Next
            Else
                Me.divNewLocation.Visible = True
                Me.Repeater1.DataSource = Nothing
                Me.Repeater1.DataBind()
            End If

            'Added 11/24/2021 for CPP Endorsements Task 66977 MLW
            If IsQuoteReadOnly() Then
                divActionButtons.Visible = False
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

                'Added 12/01/2021 for CPP Endorsements task 67055 MLW
                ctl_AdditionalInterestList.Visible = True
                'ctl_AdditionalInterestList.Populate()
                'Added 12/01/2021 for CPP Endorsements task 67073 MLW
                ctl_Endo_AppliedAdditionalInterestList.Visible = True
                'ctl_Endo_AppliedAdditionalInterestList.Populate()
            Else
                divActionButtons.Visible = True
                divEndorsementButtons.Visible = False
                'Added 12/01/2021 for CPP Endorsements task 67055 MLW
                ctl_AdditionalInterestList.Visible = False
                'Added 12/01/2021 for CPP Endorsements task 67073 MLW
                ctl_Endo_AppliedAdditionalInterestList.Visible = False
            End If
        End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandlers()
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divLocationList.ClientID
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Session("CPPCPRCheckACVEventTrigger") = String.Empty
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
                    .ClassName = "ctl_CPR_LocationList"
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

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctl As ctl_CPR_Location = ri.FindControl("ctl_CPR_Location")
            If ctl IsNot Nothing Then ctl.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        Next
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Dim found As Boolean = False

        MyBase.ValidateControl(valArgs)

        ' There must be at least one building with a building or property limit
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                    If L.Buildings IsNot Nothing Then
                        For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                            If B.Limit IsNot Nothing AndAlso IsNumeric(B.Limit) AndAlso CDec(B.Limit) > 0 Then
                                found = True
                                Exit For
                            End If
                            If B.PersPropCov_PersonalPropertyLimit IsNot Nothing AndAlso IsNumeric(B.PersPropCov_PersonalPropertyLimit) AndAlso CDec(B.PersPropCov_PersonalPropertyLimit) > 0 Then
                                found = True
                                Exit For
                            End If
                            If B.PersPropOfOthers_PersonalPropertyLimit IsNot Nothing AndAlso IsNumeric(B.PersPropOfOthers_PersonalPropertyLimit) AndAlso CDec(B.PersPropOfOthers_PersonalPropertyLimit) > 0 Then
                                found = True
                                Exit For
                            End If
                        Next
                    End If
                    If found Then Exit For
                Next
            End If
        End If

        If Not found Then
            Me.ValidationHelper.AddError("There must be at least one building with a building limit or a personal property limit to proceed")
        End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Private Function HasCoverage(ByVal MyBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding, ByVal CovType As CPRBuildingCoverageType) As Boolean
        Select Case CovType
            Case CPRBuildingCoverageType.BuildingCoverage
                If MyBuilding.Limit <> "" Then
                    Return True
                End If
                Exit Select
            Case CPRBuildingCoverageType.BusinessIncomeCoverage
                If MyBuilding.BusinessIncomeCov_Limit IsNot Nothing AndAlso MyBuilding.BusinessIncomeCov_Limit <> "" AndAlso MyBuilding.BusinessIncomeCov_Limit <> "0" Then Return True
                Exit Select
            Case CPRBuildingCoverageType.PersonalPropertyCoverage
                If MyBuilding.PersPropCov_PersonalPropertyLimit <> "" OrElse
                        MyBuilding.PersPropCov_PropertyTypeId <> "" Then
                    Return True
                End If
                Exit Select
            Case CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage
                If MyBuilding.PersPropOfOthers_PersonalPropertyLimit <> "" Then
                    Return True
                End If
                Exit Select
        End Select

        Return False
    End Function


    Public Sub AddNewLocation()
        If Me.Quote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.Quote.Locations.AddNew()
            If PropertyAddressProtectionClassHelper.ispaProtectionClassUnitsAvailable(Quote) Then
                Dim MyLocation = Me.Quote.Locations?.LastOrDefault
                If MyLocation IsNot Nothing Then
                    MyLocation.FeetToFireHydrant = "1000"
                    MyLocation.MilesToFireDepartment = "5"
                End If
            End If
            Me.ParentVrControl.Populate()
            Me.Save_FireSaveEvent(False)
            hdnAccord.Value = (Quote.Locations.Count - 1).ToString()
        End If
    End Sub

    Protected Sub DeleteLocation(LocationIndex)
        If Me.Quote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.Quote.Locations.RemoveAt(LocationIndex)
            Me.ParentVrControl.Populate()
            Me.Save_FireSaveEvent(False)
            hdnAccord.Value = (Quote.Locations.Count - 1).ToString()
        End If
    End Sub

    Protected Sub CopyLocation(LocationIndex)
        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(LocationIndex) Then
            Dim newloc As QuickQuote.CommonObjects.QuickQuoteLocation = Quote.Locations(LocationIndex)
            Quote.Locations.Add(newloc)
            Save_FireSaveEvent()
            Populate_FirePopulateEvent()
            hdnAccord.Value = (Quote.Locations.Count - 1).ToString()
        End If
    End Sub

    Protected Sub ClearLocation(LocationIndex)
        ' Clear the Wind/Hail, Property In the Open, and Building controls
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                ' Remove the location with the data in it then add a new location
                If Quote.Locations.HasItemAtIndex(LocationIndex) Then Quote.Locations.RemoveAt(LocationIndex)
                Quote.Locations.Insert(LocationIndex, New QuickQuote.CommonObjects.QuickQuoteLocation())
                If PropertyAddressProtectionClassHelper.ispaProtectionClassUnitsAvailable(Quote) Then
                    Dim MyLocation = Me.Quote.Locations(LocationIndex)
                    If MyLocation IsNot Nothing Then
                        MyLocation.FeetToFireHydrant = "1000"
                        MyLocation.MilesToFireDepartment = "5"
                    End If
                End If
                Populate()
                Me.hdnAccord.Value = LocationIndex
            End If
        End If
    End Sub

    Private Sub btnAddLocation_Click(sender As Object, e As EventArgs) Handles btnAddLocation.Click
        Session("CPPCPRCheckACVEventTrigger") = "AddNewLocationButton"
        AddNewLocation()
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnRate.Click
        Session("CPPCPRCheckACVEventTrigger") = "SaveOrRateButton" 'save is only available on CPP (rate is on liability location page), both save and rate are on CPR
        Dim QuoteAdjusted As Boolean = False

        ' MGB 2/20/18
        ' Check to see if any PIO items or Building Coverages have agreed amount and if so their Coinsurance amounts must be 100% and if they are not, adjust them
        ' to 100% and show a message
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                    ' PIO
                    If L.PropertyInTheOpenRecords IsNot Nothing Then
                        For Each PIO As QuickQuote.CommonObjects.QuickQuotePropertyInTheOpenRecord In L.PropertyInTheOpenRecords
                            If PIO.IsAgreedValue Then
                                If PIO.CoinsuranceTypeId <> "7" Then    ' 7 = 100%
                                    ' Adjust PIO Coinsurance to 100%
                                    PIO.CoinsuranceTypeId = "7"
                                    QuoteAdjusted = True
                                End If
                            End If
                        Next
                    End If
                    ' BUILDING COVERAGES
                    If L.Buildings IsNot Nothing Then
                        For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                            ' Check each coverage
                            ' Building Coverage
                            If HasCoverage(B, CPRBuildingCoverageType.BuildingCoverage) Then
                                If B.IsAgreedValue Then
                                    If B.CoinsuranceTypeId <> "7" Then
                                        ' Adjust Building Coverage Coinsurance
                                        B.CoinsuranceTypeId = "7"
                                        QuoteAdjusted = True
                                    End If
                                End If
                            End If
                            ' Business Income Coverage - does not apply
                            ' Personal Property Coverage
                            If HasCoverage(B, CPRBuildingCoverageType.PersonalPropertyCoverage) Then
                                If B.PersPropCov_IsAgreedValue Then
                                    If B.PersPropCov_CoinsuranceTypeId <> "7" Then
                                        ' Adjust Building Coverage Coinsurance
                                        B.PersPropCov_CoinsuranceTypeId = "7"
                                        QuoteAdjusted = True
                                    End If
                                End If
                            End If
                            ' Personal Property of Others Coverage - does not apply
                        Next
                    End If
                Next
            End If

            ' If we adjusted the quote then we need to save it show a message
            If QuoteAdjusted Then
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "adjmsg", "alert('Agreed Amount requires 100% Co-Insurance.  Co-Insurance has been adjusted accordingly.');", True)
                'Populate()
            End If
        End If

        Me.Save_FireSaveEvent(True)
        Populate()

        If sender.Equals(btnRate) Then
            If Not Me.ValidationSummmary.HasErrors Then
                Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
            End If
        End If
    End Sub

    Private Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnContinue.Click
        Session("CPPCPRCheckACVEventTrigger") = "ContinueButton"
        Me.Save_FireSaveEvent()
        If Me.ValidationSummmary.HasErrors = False Then
            Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Coverages, "")
        End If
    End Sub

    'Added 11/24/2021 for CPP Endorsements task 66977 MLW
    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    'Added 11/24/2021 for CPP Endorsements task 66977 MLW
    Protected Sub btnViewGenLiab_Click(sender As Object, e As EventArgs) Handles btnViewGenLiab.Click
        Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Coverages, "")
    End Sub

    'Added 10/20/2022 for task 77527 MLW
    Public Sub PopulateInflationGuard()
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctl As ctl_CPR_Location = ri.FindControl("ctl_CPR_Location")
            If ctl IsNot Nothing Then ctl.PopulateInflationGuard()
        Next
    End Sub

    Public Sub PopulateCPRBuildingInformation()
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctl As ctl_CPR_Location = ri.FindControl("ctl_CPR_Location")
            If ctl IsNot Nothing Then ctl.PopulateCPRBuildingInformation()
        Next
    End Sub

    Public Sub RemoveFunctionalReplacementCost()
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctl As ctl_CPR_Location = ri.FindControl("ctl_CPR_Location")
            If ctl IsNot Nothing Then ctl.RemoveFunctionalReplacementCost()
        Next
    End Sub
End Class