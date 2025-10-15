Imports System.Linq
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CGL
Imports IFM.VR.Common.Helpers.CPR
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctl_CPR_ENDO_LocationList
    Inherits VRControlBase

    Public Event CallTreeUpdate()

    Public Event LocationChanged(ByVal LocIndex As Integer)
    Public Event BuildingZeroDeductibleChanged()
    Public Event TriggerTransactionCount(ByRef count As Integer)
    Public Event TriggerUpdateRemarks()

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

    Private Property _CppDictItems As DevDictionaryHelper.AllCommercialDictionary
    Public ReadOnly Property CppDictItems As DevDictionaryHelper.AllCommercialDictionary
        Get
            If Quote IsNot Nothing Then
                If _CppDictItems Is Nothing Then
                    _CppDictItems = New DevDictionaryHelper.AllCommercialDictionary(Quote)
                End If
            End If
            Return _CppDictItems
        End Get
    End Property

    'Public Property TransactionLimitReached As Boolean
    '    Get
    '        If ViewState("vs_CppTransactionLimitReached") Is Nothing Then
    '            TransactionCountRequested()
    '        End If
    '        Return ViewState.GetBool("vs_CppTransactionLimitReached", False, True)
    '    End Get
    '    Set(value As Boolean)
    '        ViewState("vs_CppTransactionLimitReached") = value
    '    End Set
    'End Property

    Public ReadOnly Property TransactionLimitReached As Boolean
        Get
            If Me.ParentVrControl IsNot Nothing AndAlso TypeOf Me.ParentVrControl Is ctl_WorkflowManager_CPP_ENDO Then
                Dim Parent = CType(ParentVrControl, ctl_WorkflowManager_CPP_ENDO)
                Return Parent.TransactionLimitReached
            End If

            Return False
        End Get
    End Property

    Public ReadOnly Property MyAiList As List(Of LocationBuildingAIItem)
        Get
            Dim AIs As List(Of LocationBuildingAIItem) = New List(Of LocationBuildingAIItem)
            Dim locationIndex As Int32 = 0
            For Each location As QuickQuoteLocation In Me.Quote.Locations
                Dim buildingIndex As Int32 = 0
                If location.Buildings IsNot Nothing Then
                    For Each building As QuickQuoteBuilding In location.Buildings
                        If building.AdditionalInterests IsNot Nothing Then
                            Dim buildingAiIndex As Int32 = 0
                            For Each ai As QuickQuoteAdditionalInterest In building.AdditionalInterests
                                AIs.Add(New LocationBuildingAIItem(locationIndex, buildingIndex, buildingAiIndex, ai))
                                buildingAiIndex += 1
                            Next
                        End If
                        buildingIndex += 1
                    Next
                    locationIndex += 1
                End If

            Next
            Return AIs
        End Get
    End Property

    Const CoverageMissingMessage As String = " - Newly added locations require property coverage. Please add coverage or route to your underwriter for assistance."

    Public Overrides Sub AddScriptWhenRendered()
        If Me.divNewLocation.Visible Then
            Me.VRScript.CreateAccordion(Me.divNewLocation.ClientID, Nothing, "false", True)
        Else
            If Me.divLocationList.Visible Then
                'Updated 8/15/2022 for task 76303 MLW
                If IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    If hdnAccord.Value <> "" Then
                        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
                    Else
                        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Nothing, "false")
                    End If
                Else
                    Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
                End If
                'Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
            End If
        End If
    End Sub
    Private Sub AddHandlers()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim LocControl As ctl_CPR_ENDO_Location = cntrl.FindControl("ctl_CPR_ENDO_Location")
            AddHandler LocControl.LocationChanged, AddressOf HandleAddressChange
            AddHandler LocControl.AddLocationRequested, AddressOf AddNewLocation
            AddHandler LocControl.CopyLocationRequested, AddressOf CopyLocation
            AddHandler LocControl.DeleteLocationRequested, AddressOf DeleteLocation
            AddHandler LocControl.ClearLocationRequested, AddressOf ClearLocation
            AddHandler LocControl.BuildingZeroDeductibleChanged, AddressOf HandleBuildingZeroDeductibleChange
            AddHandler LocControl.CountTransactions, AddressOf TransactionCountRequested
            AddHandler LocControl.UpdateRemarks, AddressOf UpdateRemarks
            index += 1
        Next
        AddHandler ctl_Endo_VehicleAdditionalInterestList.CountTransactions, AddressOf TransactionCountRequested
        AddHandler ctl_Endo_AppliedAdditionalInterestList.CountTransactions, AddressOf TransactionCountRequested
    End Sub

    Public Sub HandleBuildingZeroDeductibleChange()
        RaiseEvent BuildingZeroDeductibleChanged()
    End Sub

    Public Sub HandleBlanketDeductibleChange()
        Dim myLocations As List(Of ctl_CPR_ENDO_Location) = Me.GatherChildrenOfType(Of ctl_CPR_ENDO_Location)()
        For Each L As ctl_CPR_ENDO_Location In myLocations
            L.HandleBlanketDeductibleChange()
        Next
    End Sub

    Public Sub HandleAgreedAmountChange(ByVal newvalue As Boolean)
        Dim myLocations As List(Of ctl_CPR_ENDO_Location) = Me.GatherChildrenOfType(Of ctl_CPR_ENDO_Location)()
        For Each L As ctl_CPR_ENDO_Location In myLocations
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
                        .ClassName = "ctl_CPR_ENDO_LocationList"
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
                    'btnEmailForUWAssistance.Visible = True
                    btnContinue.Visible = False
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    btnRate.Visible = False
                    'btnEmailForUWAssistance.Visible = False
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
                For Each Loc As ctl_CPR_ENDO_Location In Me.GatherChildrenOfType(Of ctl_CPR_ENDO_Location)
                    Loc.MyLocationIndex = index
                    'Loc.Populate()
                    index += 1
                Next
            Else
                Me.divNewLocation.Visible = True
                Me.Repeater1.DataSource = Nothing
                Me.Repeater1.DataBind()
            End If

            TransactionCountRequested()
            If TransactionLimitReached Then
                btnAddLocation.Enabled = False
            Else
                btnAddLocation.Enabled = True
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
                    .ClassName = "ctl_CPR_ENDO_LocationList"
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
        If Not IsQuoteReadOnly() Then
            CGLMedicalExpensesExcludedClassCodesHelper.UpdateAndShowMessagesForMedicalExpensesDropdownForExcludedGLCodes(Quote, Me.Page)
        End If
        Return True
    End Function

    Public Overrides Sub AfterSaveOccurrs(args As VrControlBaseSaveEventArgs)
        MyBase.AfterSaveOccurrs(args)

    End Sub

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctl As ctl_CPR_ENDO_Location = ri.FindControl("ctl_CPR_ENDO_Location")
            If ctl IsNot Nothing Then ctl.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        Next
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Dim found As Boolean = False
        Dim LocationNumber As Integer = 0
        Dim BuildingNumber As Integer = 0
        MyBase.ValidateControl(valArgs)




        ' There must be at least one building with a building or property limit
        If Quote IsNot Nothing Then
            Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
            AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)

            If Quote.Locations IsNot Nothing Then
                For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                    LocationNumber += 1
                    BuildingNumber = 0
                    If Not AllPreExistingItems.PreExisting_Locations.isPreExistingLocationByLocationObject(L) AndAlso L.Buildings IsNot Nothing Then
                        'If L.Buildings IsNot Nothing Then
                        For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                            BuildingNumber += 1
                            found = False
                            If B.Limit IsNot Nothing AndAlso IsNumeric(B.Limit) AndAlso CDec(B.Limit) > 0 Then
                                found = True
                                'Exit For
                            End If
                            If B.PersPropCov_PersonalPropertyLimit IsNot Nothing AndAlso IsNumeric(B.PersPropCov_PersonalPropertyLimit) AndAlso CDec(B.PersPropCov_PersonalPropertyLimit) > 0 Then
                                found = True
                                'Exit For
                            End If
                            If B.PersPropOfOthers_PersonalPropertyLimit IsNot Nothing AndAlso IsNumeric(B.PersPropOfOthers_PersonalPropertyLimit) AndAlso CDec(B.PersPropOfOthers_PersonalPropertyLimit) > 0 Then
                                found = True
                                'Exit For
                            End If
                            If Not found Then
                                Me.ValidationHelper.AddError("Location: " & LocationNumber & " Building: " & BuildingNumber & CoverageMissingMessage)
                            End If
                        Next
                    End If
                    'If found Then Exit For
                Next
            End If

            'CAH Bug 76305: Test CPP Endo to make sure all AIs are Assigned
            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                Dim PreExistAIList = AllPreExistingItems.PreExisting_AdditionalInterests.ListIds()
                Dim CurrentAIList = (From x In Quote.AdditionalInterests Select x.ListId)

                'Get list of AI's ListId that do not PreExist
                Dim AiResults = CurrentAIList.Except(PreExistAIList)

                Dim AppliedAIs = New Helpers.FindAppliedAdditionalInterestList
                Dim CurrentAAIList = AppliedAIs.FindAppliedAI(Quote)

                'Get list of AI's ListId that are applied
                Dim AppliedAiResults = (From x In CurrentAAIList Select x.AI.ListId)

                'Count AI's that are not Applied
                If AiResults.Except(AppliedAiResults).Count > 0 Then
                    Me.ValidationHelper.AddError("All newly added additional interests must be assigned to a property coverage (building, business personal property, property of others, etc.) at a location on the policy.  Select Assign Additional Interest to assign the corresponding location and coverage.")
                End If
            End If
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

            Dim newLocationIndex As Integer = 0
            If Quote.Locations.Count > 0 Then
                newLocationIndex = Quote.Locations.Count - 1
            End If

            Quote.TransactionReasonId = 10169 'Endorsement Change Dec and Full Revised Dec

            Me.ParentVrControl.Populate()
            Me.Save_FireSaveEvent(False)
            hdnAccord.Value = (Quote.Locations.Count - 1).ToString()
        End If
    End Sub

    Protected Sub DeleteLocation(LocationIndex)
        If Me.Quote.IsNotNull Then
            Me.Save_FireSaveEvent(False)

            'Move Contractor's Equipment if necessary to Location 1 Building 1
            'Move AI
            'Location 1 cannot be deleted.
            For Each AI In MyAiList
                If AI.LocationIndex = LocationIndex Then
                    If String.IsNullOrWhiteSpace(AI.AI?.Other) = False AndAlso AI.AI.Other.ToUpper.Contains("CE") Then
                        'Contractor's Equipment AI, move to 0,0
                        Dim Location = Me.Quote.Locations(0).Buildings(0)
                        If Location.AdditionalInterests Is Nothing Then
                            Location.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                        End If
                        Location.AdditionalInterests.Add(AI.AI)

                        Dim OldAILocationAssignedAI As DevDictionaryHelper.AssignedAI = New DevDictionaryHelper.AssignedAI(AI.LocationIndex, AI.BuildingIndex, AI.AI.ListId, AI.AI.Description, DevDictionaryHelper.DevDictionaryHelper.addItem)
                        Dim NewAILocationAssignedAI As DevDictionaryHelper.AssignedAI = New DevDictionaryHelper.AssignedAI(0, 0, AI.AI.ListId, AI.AI.Description, DevDictionaryHelper.DevDictionaryHelper.addItem)

                        'Dev Dictionary move items if they are new
                        If CppDictItems.DoesAssignedAiExist(New DevDictionaryHelper.AssignedAI(AI.LocationIndex, AI.BuildingIndex, AI.AI.ListId, AI.AI.Description, DevDictionaryHelper.DevDictionaryHelper.addItem), DevDictionaryHelper.DevDictionaryHelper.addItem) Then
                            CppDictItems.UpdateAssignedAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Update,
                                NewAILocationAssignedAI,
                                OldAILocationAssignedAI)
                        End If

                        'Move Pre Existing so we retain these.
                        Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
                        AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
                        If AllPreExistingItems.PreExisting_AssignedAdditionalInterests.isPreExistingLocationBuildingAIs(OldAILocationAssignedAI) Then
                            AllPreExistingItems.PreExisting_AssignedAdditionalInterests.movePreExistingAssignedAiToDiffLocation(NewAILocationAssignedAI, OldAILocationAssignedAI)
                        End If
                    End If
                End If
            Next

            'Find all AAIs on this location
            Dim AAIsOnLocation = MyAiList.FindAll(Function(x) x.LocationIndex = LocationIndex)

            If AAIsOnLocation.Count > 0 Then
                For Each AAI In AAIsOnLocation
                    Dim ListId = AAI.AI.ListId
                    'Check if AI.ListId appears on any other location and delete if not.  Leave it if it does exist on another location.
                    If MyAiList.FindAll(Function(x) x.AI.ListId = ListId AndAlso x.LocationIndex <> LocationIndex).Count = 0 Then
                        CppDictItems.UpdateAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, AAI.AI)
                        CppDictItems.UpdateAssignedAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, New DevDictionaryHelper.AssignedAI(AAI.LocationIndex, AAI.BuildingIndex, AAI.AI.ListId, AAI.AI.Description))
                        QQHelper.RemoveSpecificQuickQuoteAdditionalInterestFromQuoteBasedOnLob(Me.Quote, AAI.AI.ListId, removeFromTopLevel:=True)
                    End If
                    'Remove These AAI's from the dev Dictionary (They get removed from quote when the Location is removed below)
                Next
            End If

            'Remove from ddh 
            CppDictItems.UpdateLocations(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, Quote.Locations(LocationIndex), LocationIndex.ToString)

            'Add Remarks
            'Added 09/28/2021 for BOP Endorsements Task 61660 MLW
            Dim endRemarksHelper = New EndorsementsRemarksHelper(CppDictItems)
            Dim updatedRemarks As String = endRemarksHelper.UpdateRemarks(EndorsementsRemarksHelper.RemarksType.Location)
            Quote.TransactionRemark = updatedRemarks

            'Remove from Quote
            Me.Quote.Locations.RemoveAt(LocationIndex)
            Me.Populate()
            'Me.Save_FireSaveEvent(False)
            ' 05/04/2022 CAH - We need to save changes, not reprocess all save events again.  The above will break.
            Dim endorsementSaveError As String = ""
            Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError, saveTypeView:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
            hdnAccord.Value = (Quote.Locations.Count - 1).ToString()
        End If
    End Sub

    Protected Sub CopyLocation(LocationIndex)
        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(LocationIndex) Then
            Dim newloc As QuickQuote.CommonObjects.QuickQuoteLocation = Quote.Locations(LocationIndex)
            Quote.Locations.Add(newloc)

            Dim newLocationIndex As Integer = 0
            If Quote.Locations.Count > 0 Then
                newLocationIndex = Quote.Locations.Count - 1
            End If
            CppDictItems.UpdateLocations(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, Quote.Locations(newLocationIndex), newLocationIndex.ToString)
            Quote.TransactionReasonId = 10169 'Endorsement Change Dec and Full Revised Dec

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
                CppDictItems.UpdateLocations(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, Quote.Locations(LocationIndex), LocationIndex.ToString)
                Quote.Locations.Insert(LocationIndex, New QuickQuote.CommonObjects.QuickQuoteLocation())
                If PropertyAddressProtectionClassHelper.ispaProtectionClassUnitsAvailable(Quote) Then
                    Dim MyLocation = Me.Quote.Locations(LocationIndex)
                    If MyLocation IsNot Nothing Then
                        MyLocation.FeetToFireHydrant = "1000"
                        MyLocation.MilesToFireDepartment = "5"
                    End If
                End If
                CppDictItems.UpdateLocations(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, New QuickQuote.CommonObjects.QuickQuoteLocation, LocationIndex.ToString)
                Populate()
                Me.hdnAccord.Value = LocationIndex
            End If
        End If
    End Sub

    Private Sub btnAddLocation_Click(sender As Object, e As EventArgs) Handles btnAddLocation.Click
        Session("CPPCPREventTrigger") = "AddNewLocationButton"
        AddNewLocation()
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnRate.Click
        Session("CPPCPREventTrigger") = "SaveOrRateButton"
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
        Session("CPPCPREventTrigger") = "ContinueButton"
        Me.Save_FireSaveEvent()
        If Me.ValidationSummmary.HasErrors = False Then
            Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Locations, "")
        End If
    End Sub

    Private Sub PageChanged() Handles ctl_Endo_AppliedAdditionalInterestList.AIChange, ctl_Endo_VehicleAdditionalInterestList.AIChange 'added 9/22/2017
        'Populate()
        UpdateRemarks()

        TransactionCountRequested()
        If TransactionLimitReached Then
            btnAddLocation.Enabled = False
        Else
            btnAddLocation.Enabled = True
        End If
        RaiseEvent CallTreeUpdate()
        PopulateChildControls()
    End Sub

    Public Sub TransactionCountRequested(Optional ByRef count As Integer = 0)
        'Dim TransactionCount = CppDictItems.GetTransactionCount()
        'If TransactionCount >= 3 Then
        '    TransactionLimitReached = True
        'Else
        '    TransactionLimitReached = False
        'End If
        'count = TransactionCount

        RaiseEvent TriggerTransactionCount(count)

    End Sub

    Public Sub UpdateRemarks()
        'Dim endorsementsRemarksHelper = New EndorsementsRemarksHelper(Quote)
        'Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(EndorsementsRemarksHelper.RemarksType.AllBopRemarks)
        'If String.IsNullOrWhiteSpace(updatedRemarks) = False Then
        '    Quote.TransactionRemark = updatedRemarks
        'End If

        RaiseEvent TriggerUpdateRemarks()
    End Sub

End Class