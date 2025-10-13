Imports IFM.PrimativeExtensions
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctl_BOP_ENDO_LocationList
    Inherits VRControlBase

    Public Event CallTreeUpdate()

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

    Private Property _BopDictItems As DevDictionaryHelper.AllCommercialDictionary
    Public ReadOnly Property BopDictItems As DevDictionaryHelper.AllCommercialDictionary
        Get
            If Quote IsNot Nothing Then
                If _BopDictItems Is Nothing Then
                    _BopDictItems = New DevDictionaryHelper.AllCommercialDictionary(Quote)
                End If
            End If
            Return _BopDictItems
        End Get
    End Property

    Public Property TransactionLimitReached As Boolean
        Get
            If ViewState("vs_BopTransactionLimitReached") Is Nothing Then
                TransactionCountRequested()
            End If
            Return ViewState.GetBool("vs_BopTransactionLimitReached", False, True)
        End Get
        Set(value As Boolean)
            ViewState("vs_BopTransactionLimitReached") = value
        End Set
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


    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Protected Sub AttachLocationControlEvents()
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim LocControl As ctl_BOP_ENDO_Location = cntrl.FindControl("ctl_BOP_ENDO_Location")

            AddHandler LocControl.NewLocationRequested, AddressOf locationControlNewLocationRequested
            AddHandler LocControl.DeleteLocationRequested, AddressOf locationControlDeleteLocationRequested
            AddHandler LocControl.ClearLocationRequested, AddressOf locationControlClearLocationRequested
            AddHandler LocControl.AddLocationBuildingRequested, AddressOf locationAddBuildingRequested
            AddHandler LocControl.CountTransactions, AddressOf TransactionCountRequested
            AddHandler LocControl.UpdateRemarks, AddressOf UpdateRemarks
        Next

        AddHandler ctl_Endo_VehicleAdditionalInterestList.CountTransactions, AddressOf TransactionCountRequested
        AddHandler ctl_Endo_AppliedAdditionalInterestList.CountTransactions, AddressOf TransactionCountRequested
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
                        .ClassName = "ctl_BOP_ENDO_LocationList"
                        .ErrorMessage = unhandledExceptionToString
                        .LogDate = DateTime.Now
                        .RoutineName = "Populate"
                        .StackTrace = "at IntegrationHelper.CallCommercialDataPrefill_PropertyOnly_Preload at IntegrationHelper.CallCommercialDataPrefill_PropertyOnly_Preload_IfNeeded"
                        .AdditionalInfo = addPreloadInfo
                    End With
                    WriteErrorLogRecord(preloadError, "")
                End If
            End If
            'Added 10/28/2021 for BOP Endorsements Task 65882 MLW
            Quote.CopyProfessionalLiabilityCoveragesFromPolicyToBuildings_UseBuildingClassificationList() 'This is to move this to one central spot instead of being called for every building.
            Me.Repeater1.DataSource = Me.Quote.Locations
            Me.Repeater1.DataBind()

            FindChildVrControls()

            TransactionCountRequested()
            If TransactionLimitReached Then
                btnAddAnotherLocation.Enabled = False
            Else
                btnAddAnotherLocation.Enabled = True
            End If
        End If
        Me.PopulateChildControls()

    End Sub

    Private Sub repeater1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
        If e IsNot Nothing Then
            Dim LocControl As ctl_BOP_ENDO_Location = e.Item.FindControl("ctl_BOP_ENDO_Location")
            LocControl.LocationIndex = e.Item.ItemIndex
            LocControl.Populate()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divMainList.ClientID
        End If

        AttachLocationControlEvents()
    End Sub

    Public Overrides Function Save() As Boolean
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                'Updated 10/28/2021 for BOP Endorsements Task 65882 MLW - added two quote.copy for prof liab
                Quote.CopyProfessionalLiabilityCoveragesFromPolicyToBuildings_UseBuildingClassificationList() 'This is to make is so stuff doesn't fall off because the UI doesn't save preexisting locations.
                Me.SaveChildControls()
                'This is not needed for endorsements at this time because we cannot add or delete prof liab on endorsements. If in the future, they add back the ability to save the prof liab coverages on endorsements, we will need this copy added back in, but not always. Will need to raise the event in save of ctl_BOP_ENDO_BuildingCoverages. Add a flag here in the endorsement location list control before the save to set the flag to false. Then if the event is raised, the flag gets set to true. If flag true, then use the copy line below to copy prof liab from building up to policy level.
                Session("BOPCheckLimitEventTrigger") = String.Empty
        End Select


        If ActiveLocationIndex = "" Then
            ActiveLocationIndex = "false"
        End If

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
                    .ClassName = "ctl_BOP_ENDO_LocationList"
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
        Dim TransactionCount As Integer = 0
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Sub CheckTranactionLimitAtRate()
        Dim TransactionCount As Integer = 0
        TransactionCountRequested(TransactionCount)
        If TransactionLimitReached AndAlso TransactionCount > 3 Then
            Me.ValidationHelper.AddError("BOP Endorsements allow a maximum of 3 changes per transaction.  You currently have " & TransactionCount & ". Please reduce the number of changes or route to underwriting for completion.")
        End If
    End Sub


    Private Sub locationControlNewLocationRequested()
        btnAddAnotherLocation_Click(Me, New EventArgs())
    End Sub

    Private Sub locationControlDeleteLocationRequested(ByRef LocIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                If Quote.Locations.HasItemAtIndex(LocIndex) Then

                    Save_FireSaveEvent(False)

                    'Move Contractor's Equipment if necessary to Location 1 Building 1
                    'Move AI
                    'Location 1 cannot be deleted.
                    For Each AI In MyAiList
                        If AI.LocationIndex = LocIndex Then
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
                                If BopDictItems.DoesAssignedAiExist(New DevDictionaryHelper.AssignedAI(AI.LocationIndex, AI.BuildingIndex, AI.AI.ListId, AI.AI.Description, DevDictionaryHelper.DevDictionaryHelper.addItem), DevDictionaryHelper.DevDictionaryHelper.addItem) Then
                                    BopDictItems.UpdateAssignedAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Update,
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

                    'Remove AIs and AAIs
                    Dim LocationIndex = LocIndex
                    'Find all AAIs on this location
                    Dim AAIsOnLocation = MyAiList.FindAll(Function(x) x.LocationIndex = LocationIndex)

                    If AAIsOnLocation.Count > 0 Then
                        For Each AAI In AAIsOnLocation
                            Dim ListId = AAI.AI.ListId
                            'Check if AI.ListId appears on any other location and delete if not.  Leave it if it does exist on another location.
                            If MyAiList.FindAll(Function(x) x.AI.ListId = ListId AndAlso x.LocationIndex <> LocationIndex).Count = 0 Then
                                BopDictItems.UpdateAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, AAI.AI)
                                QQHelper.RemoveSpecificQuickQuoteAdditionalInterestFromQuoteBasedOnLob(Me.Quote, AAI.AI.ListId, removeFromTopLevel:=True)
                            End If
                            'Remove These AAI's from the dev Dictionary (They get removed from quote when the Location is removed below)
                        Next
                    End If

                    'Remove from ddh 
                    BopDictItems.UpdateLocations(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, Quote.Locations(LocIndex), LocIndex.ToString)

                    'Add Remarks
                    'Added 09/28/2021 for BOP Endorsements Task 61660 MLW
                    'Dim endRemarksHelper = New EndorsementsRemarksHelper(BopDictItems)
                    'Dim updatedRemarks As String = endRemarksHelper.UpdateRemarks(EndorsementsRemarksHelper.RemarksType.Location)
                    'Quote.TransactionRemark = updatedRemarks

                    'Remove from Quote
                    Quote.Locations.RemoveAt(LocIndex)
                    'Move these below the location remove so it's data is pulled, too.
                    UpdateRemarks()

                End If


                Dim endorsementSaveError As String = ""
                Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError, saveTypeView:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)

                Populate()
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

            Dim newLocationIndex As Integer = 0
            If Quote.Locations.Count > 0 Then
                newLocationIndex = Quote.Locations.Count - 1
            End If
            BopDictItems.UpdateLocations(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, New QuickQuote.CommonObjects.QuickQuoteLocation, newLocationIndex.ToString)
            Quote.TransactionReasonId = 10169 'Endorsement Change Dec and Full Revised Dec

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
                If Quote.Locations.HasItemAtIndex(LocIndex) Then
                    Quote.Locations.RemoveAt(LocIndex)
                    BopDictItems.UpdateLocations(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, Quote.Locations(LocIndex), LocIndex.ToString)
                    Quote.Locations.Insert(LocIndex, New QuickQuote.CommonObjects.QuickQuoteLocation())
                    BopDictItems.UpdateLocations(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, New QuickQuote.CommonObjects.QuickQuoteLocation, LocIndex.ToString)
                    Populate()
                End If

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

    'added 11/15/2017 for Equipment Breakdown MBR
    Public Sub PopulateLocationCoverages()
        If Me.Repeater1.Items IsNot Nothing AndAlso Me.Repeater1.Items.Count > 0 Then
            For Each cntrl As RepeaterItem In Me.Repeater1.Items
                Dim LocControl As ctl_BOP_ENDO_Location = cntrl.FindControl("ctl_BOP_ENDO_Location")
                If LocControl IsNot Nothing Then
                    LocControl.PopulateLocationCoverages()
                End If
            Next
        End If

    End Sub

    Public Sub EffectiveDateChanging(ByVal NewDate As String, ByVal OldDate As String)
        ' Put any code here to handle when the effective date changes
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctlLoc As ctl_BOP_ENDO_Location = ri.FindControl("ctl_BOP_ENDO_Location")
            If ctlLoc IsNot Nothing Then ctlLoc.EffectiveDateChanging(NewDate, OldDate)
        Next
        Exit Sub
    End Sub

    Private Sub PageChanged() Handles ctl_Endo_AppliedAdditionalInterestList.AIChange, ctl_Endo_VehicleAdditionalInterestList.AIChange 'added 9/22/2017
        'Populate()
        UpdateRemarks()

        TransactionCountRequested()
        If TransactionLimitReached Then
            btnAddAnotherLocation.Enabled = False
        Else
            btnAddAnotherLocation.Enabled = True
        End If
        'RaiseEvent CallTreeUpdate() 'Removed 3/7/2022 for bug 73427 MLW
        PopulateChildControls()
    End Sub

    Public Sub TransactionCountRequested(Optional ByRef count As Integer = 0)
        Dim TransactionCount = BopDictItems.GetTransactionCount()
        If TransactionCount >= 3 Then
            TransactionLimitReached = True
        Else
            TransactionLimitReached = False
        End If
        count = TransactionCount

    End Sub

    Public Sub UpdateRemarks()
        Dim endorsementsRemarksHelper = New EndorsementsRemarksHelper(Quote)
        Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(endorsementsRemarksHelper.RemarksType.AllBopRemarks)
        Quote.TransactionRemark = updatedRemarks
        RaiseEvent CallTreeUpdate()
    End Sub

End Class