Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.PrimativeExtensions

Public Class ctl_Endo_AppliedAdditionalInterestList
    Inherits VRControlBase

    Public Event AIChange()
    Public Event AddFakeAIIfNeeded()
    Public Event UpdateTransactionReasonType(ddh As DevDictionaryHelper.DevDictionaryHelper)
    Public Event CountTransactions()

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

    Public ReadOnly Property AdditionalInterstIdsCreatedThisSession As List(Of Int32)
        Get
            If Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List") Is Nothing Then
                Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List") = New List(Of Int32)
            End If

            Return CType(Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List"), List(Of Int32))
        End Get

    End Property

    Public ReadOnly Property MyAiList As List(Of LocationBuildingAIItem)
        Get
            Dim appAIs = New Helpers.FindAppliedAdditionalInterestList
            Return appAIs.FindAppliedAI(Quote)
        End Get
    End Property

    Public Property DiffList As AiDifferentialList
        Get
            If (ViewState("vs_DiffList") Is Nothing) Then
                Dim Setup = New AiDifferentialList
                Setup.AiDiffList = New List(Of AiDifferentialItem)
                ViewState("vs_DiffList") = Setup
            End If
            Return CType(ViewState("vs_DiffList"), AiDifferentialList)
        End Get
        Set(value As AiDifferentialList)
            ViewState("vs_DiffList") = value
        End Set
    End Property

    Public ReadOnly Property TransactionLimitReached As Boolean
        Get
            If Me.ParentVrControl IsNot Nothing AndAlso TypeOf Me.ParentVrControl Is ctl_BOP_ENDO_LocationList Then
                Dim Parent = CType(ParentVrControl, ctl_BOP_ENDO_LocationList)
                Return Parent.TransactionLimitReached
            End If

            Return False
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStaticData()
        End If
        Me.MainAccordionDivId = Me.divAdditionalInterests.ClientID
        Me.ListAccordionDivId = Me.divAdditionalInterestItems.ClientID
        AttachDriverControlEvents()
    End Sub

    Public Overrides Sub AddScriptAlways()
        If Me.Quote IsNot Nothing Then
            Dim hasMortgageeType As Boolean = False

            If Me.VRScript IsNot Nothing Then
                Me.VRScript.AddVariableLine("var hasMortgageeTypeAI = " + hasMortgageeType.ToString().ToLower() + ";")
            End If

        End If
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.Quote IsNot Nothing Then
            If MyAiList IsNot Nothing AndAlso MyAiList.Any() Then
                'Updated 8/15/2022 for task 76303 MLW
                If IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    If IsPostBack Then
                        Me.VRScript.CreateAccordion(Me.divAdditionalInterests.ClientID, hiddenAdditionalInterest, "false")
                    Else
                        Me.VRScript.CreateAccordion(Me.divAdditionalInterests.ClientID, Nothing, "false")
                    End If
                Else
                    Me.VRScript.CreateAccordion(Me.divAdditionalInterests.ClientID, hiddenAdditionalInterest, "false")
                End if
                'Me.VRScript.CreateAccordion(Me.divAdditionalInterests.ClientID, hiddenAdditionalInterest, "false")
            Else
                Me.VRScript.CreateAccordion(Me.divAdditionalInterests.ClientID, hiddenAdditionalInterest, "false", True)
            End If
        End If

        Me.VRScript.CreateAccordion(Me.divAdditionalInterestItems.ClientID, hiddenAdditionalInterestItems, "0")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Protected Sub AttachDriverControlEvents()

        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim aiControl As ctl_Endo_AppliedAdditionalInterest = CType(cntrl.FindControl("ctl_Endo_AppliedAdditionalInterest"), ctl_Endo_AppliedAdditionalInterest)
            'AddHandler aiControl.SaveRequested, AddressOf aiControlRequestedSave
            AddHandler aiControl.RemoveAi, AddressOf aiRemovalRequested
            AddHandler aiControl.AIListChanged, AddressOf AIListWasChanged
            'AddHandler aiControl.UpdateTransactionReasonType, AddressOf UpdateTransactionReasonTypeFloat
        Next
    End Sub

    Public Overrides Sub Populate()
        'Updated 12/1/2021 for CPP Endorsements task 66977 MLW
        'If Me.Quote IsNot Nothing Then
        If Me.Quote IsNot Nothing AndAlso (IsQuoteReadOnly() OrElse IsQuoteEndorsement()) Then
            DiffList.AiDiffList = New List(Of AiDifferentialItem)
            Me.Repeater1.DataSource = MyAiList
            Me.Repeater1.DataBind()

            lnkBtnAdd.Visible = True
            lnkBtnSave.Visible = True

            If MyAiList IsNot Nothing AndAlso MyAiList.Any() Then
                'Updated 8/15/2022 for task 76303 MLW
                If (IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage) = False Then
                    If Not IsPostBack Then
                        Me.OpenAccordionAtIndex("0") ' open when there are AIs on first load
                    End If
                End If

                Select Case Me.Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        Me.lblHeader.Text = String.Format("Assigned Additional Interests ({0}) (Property)", MyAiList.Count)
                    Case Else
                        Me.lblHeader.Text = String.Format("Assigned Additional Interests ({0})", MyAiList.Count)
                End Select

                Me.divAdditionalInterestItems.Visible = True
            Else
                Select Case Me.Quote.LobType
                    Case Else
                        Me.lblHeader.Text = "Assigned Additional Interests (0)"
                End Select
                Me.divAdditionalInterestItems.Visible = False
            End If
            'End If

            divEndorsementButtons.Visible = True

        End If
        PopulateChildControls()

    End Sub

    Private Sub repeater1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
        If e IsNot Nothing Then
            Dim AiItem As LocationBuildingAIItem = e.Item.DataItem
            Dim AiControl As ctl_Endo_AppliedAdditionalInterest = e.Item.FindControl("ctl_Endo_AppliedAdditionalInterest")
            AiControl.AdditionalInterestIndex = e.Item.ItemIndex
            AiControl.MyBuildingAdditionalInterestIndex = AiItem.BuildingAiIndex
            AiControl.LocationIndex = AiItem.LocationIndex
            AiControl.BuildingIndex = AiItem.BuildingIndex
            AiControl.Populate()
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        If Me.Quote IsNot Nothing Then
            'Updated 12/29/2020 for CAP Endorsements Task 52974 MLW
            Select Case Me.Quote.LobType
                Case Else
                    Me.ValidationHelper.GroupName = "Additional Interests"
            End Select

            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        End If
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            Me.SaveChildControls()
            RaiseEvent AIChange() 'Added 3/7/2022 for bug 73427 MLW
        End If

        Return True
    End Function

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        ' FOR COMMERCIAL LINES - Don't fire the validation because we're making the user save any additional interests before they can use them
        ' further down the page and we don't want to ding them for field validations when we save the additional interest
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                Me.Save_FireSaveEvent(False)
                Exit Select
            Case Else
                Me.Save_FireSaveEvent(True)
                Exit Select
        End Select

        'Me.Populate()
        RaiseEvent AIChange()
    End Sub

    Public Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkBtnAdd.Click, btnAddAdditionalInterest.Click
        If Quote IsNot Nothing Then

            Dim interest As New DevDictionaryHelper.AssignedAI("1", "1", "", DevDictionaryHelper.DevDictionaryHelper.addItem)

            If BopDictItems.UpdateAssignedAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, interest) Then
            End If

            'Save event needs to happen before we attempt to add the additional interest. If it isn't, when someone attempts to add two AI's, without manually specifying a save inbetween, the MyAiList will not
            'be updated with the most recent attempt to add. This forces the use to have to click the "Add Additional Interest" button twice to actuall add one.
            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))


            If Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings.HasItemAtIndex(0) Then
                If Quote.Locations(0).Buildings(0).AdditionalInterests Is Nothing Then
                    Quote.Locations(0).Buildings(0).AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                End If
                Dim newAi As QuickQuoteAdditionalInterest = New QuickQuoteAdditionalInterest
                Quote.Locations(0).Buildings(0).AdditionalInterests.Add(newAi)
            End If


            LockTree()

            'Find Newly added item
            Dim aiIndex = MyAiList.FindIndex(Function(x) x.AI.ListId = String.Empty)
            'Open New Item
            OpenAccordionAtIndex(aiIndex.ToString())

            'Me.Populate()
            RaiseEvent AIChange()
        End If
    End Sub

    Private Sub UpdateTransactionReasonTypeFloat(ddh As DevDictionaryHelper.DevDictionaryHelper)
        RaiseEvent UpdateTransactionReasonType(ddh)
    End Sub

    Private Sub aiControlRequestedSave(args As VrControlBaseSaveEventArgs)
        Me.Populate()
    End Sub

    Private Sub AIListWasChanged()
        RaiseEvent AIChange()
    End Sub

    Private Sub aiRemovalRequested(index As Integer)
        If Quote IsNot Nothing Then
            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            If MyAiList IsNot Nothing Then
                If MyAiList.Count > index Then

                    Dim AiToRemove As LocationBuildingAIItem = MyAiList(index)

                    'Remove Master AI if This AppliedAI is the only thing attached to that Master
                    Dim AIs = Me.Quote.AdditionalInterests
                    'Dim isSingleAI = AIs.FindAll(Function(x) x.ListId = AiToRemove.AI.ListId).Count() = 1
                    Dim isSingleAI = MyAiList.FindAll(Function(x) x.AI.ListId = AiToRemove.AI.ListId).Count = 1
                    If isSingleAI Then
                        BopDictItems.UpdateAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, AiToRemove.AI)
                        QQHelper.RemoveSpecificQuickQuoteAdditionalInterestFromQuoteBasedOnLob(Me.Quote, AiToRemove.AI.ListId, removeFromTopLevel:=True)
                    Else

                        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP _
                            OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(AiToRemove.LocationIndex) Then
                                If Quote.Locations(AiToRemove.LocationIndex).Buildings IsNot Nothing AndAlso Quote.Locations(AiToRemove.LocationIndex).Buildings.HasItemAtIndex(AiToRemove.BuildingIndex) Then
                                    If Quote.Locations(AiToRemove.LocationIndex).Buildings(AiToRemove.BuildingIndex).AdditionalInterests IsNot Nothing AndAlso Quote.Locations(AiToRemove.LocationIndex).Buildings(AiToRemove.BuildingIndex).AdditionalInterests.IsLoaded Then
                                        Quote.Locations(AiToRemove.LocationIndex).Buildings(AiToRemove.BuildingIndex).AdditionalInterests.RemoveAt(AiToRemove.BuildingAiIndex)
                                    End If
                                End If
                            End If
                            BopDictItems.UpdateAssignedAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete,
                                                                   New DevDictionaryHelper.AssignedAI(AiToRemove.LocationIndex, AiToRemove.BuildingIndex, AiToRemove.AI.ListId, AiToRemove.AI.Description))
                        End If

                    End If

                    Dim endorsementSaveError As String = ""
                    Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError, saveTypeView:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
                    RaiseEvent AIChange()

                End If
            End If

        End If
    End Sub

    Public Sub OpenAccordionAtIndex(index As String)
        Me.hiddenAdditionalInterest.Value = "0"
        Me.hiddenAdditionalInterestItems.Value = index
    End Sub


End Class

Public Class LocationBuildingAIItem
    Property LocationIndex As Int32
    Property BuildingIndex As Int32
    Property BuildingAiIndex As Int32
    Property AI As QuickQuoteAdditionalInterest

    Public Sub New(locIndex, buildIndex, buildingAiIndex, ai)
        Me.LocationIndex = locIndex
        Me.BuildingIndex = buildIndex
        Me.BuildingAiIndex = buildingAiIndex
        Me.AI = ai
    End Sub

End Class

<Serializable()> Public Class AiDifferentialItem
    Property LocationIndex As Int32
    Property BuildingIndex As Int32
    Property BuildingAiIndexes As List(Of Int32)

    Public Sub New()
        BuildingAiIndexes = New List(Of Integer)
        Me.LocationIndex = 0
        Me.BuildingIndex = 0

    End Sub

    Public Sub New(locIndex, buildIndex)
        BuildingAiIndexes = New List(Of Integer)
        Me.LocationIndex = locIndex
        Me.BuildingIndex = buildIndex

    End Sub

    ''' <summary>
    ''' Add's a Building's Ai to a list to track items removed from a list of indexes
    ''' </summary>
    ''' <param name="index">Index that was removed from a list of indexes</param>
    Public Sub AddAiIndex(index As Int32)
        BuildingAiIndexes.Add(index)
    End Sub

    ''' <summary>
    ''' Amount The building's Ai index should be reduced do to removing items from the list of Ais infront of this Ai's index.
    ''' </summary>
    ''' <param name="index">Current index of AI for a specific building</param>
    ''' <returns></returns>
    Public Function GetAIDifferential(index As Int32) As Integer
        Dim Diff As Integer = 0
        For Each item As Int32 In BuildingAiIndexes
            If index > item Then
                Diff += 1
            End If
        Next
        Return Diff
    End Function

End Class

<Serializable()> Public Class AiDifferentialList
    Property AiDiffList As List(Of AiDifferentialItem)

    Public Sub New()
    End Sub

    ''' <summary>
    ''' Add an index of an AI that was removed from a building.
    ''' </summary>
    ''' <param name="locindex">Location to Check</param>
    ''' <param name="buildIndex">Building to Check</param>
    ''' <param name="index">Current Ai index being removed from the list</param>
    Public Sub AddAiIndex(locindex As Int32, buildIndex As Int32, index As Int32)
        Dim BuildingAiIndexes = GetDiffGroup(locindex, buildIndex)
        BuildingAiIndexes.AddAiIndex(index)
    End Sub

    ''' <summary>
    ''' How much an index should be adjusted because of previous list removals
    ''' </summary>
    ''' <param name="locindex">Location to Check</param>
    ''' <param name="buildIndex">Building to Check</param>
    ''' <param name="inputIndex">Current Ai index being checked to see if an adjustment is needed</param>
    ''' <returns></returns>
    Public Function GetAIDifferential(locindex As Int32, buildIndex As Int32, inputIndex As Int32) As Int32
        Dim BuildingAiIndexes = GetDiffGroup(locindex, buildIndex)
        Return BuildingAiIndexes.GetAIDifferential(inputIndex)
    End Function

    ''' <summary>
    ''' Get the Specific Building's list of Ai's that have been removed
    ''' </summary>
    ''' <param name="locIndex">Location to Check</param>
    ''' <param name="buildIndex">Building to Check</param>
    ''' <returns></returns>
    Private Function GetDiffGroup(locIndex As Int32, buildIndex As Int32) As AiDifferentialItem
        If AiDiffList Is Nothing Then
            AiDiffList = New List(Of AiDifferentialItem)
        End If
        Dim DiffGroup = AiDiffList.Find(Function(x) x.LocationIndex = locIndex AndAlso x.BuildingIndex = buildIndex)
        If DiffGroup Is Nothing Then
            AiDiffList.Add(New AiDifferentialItem(locIndex, buildIndex))
            DiffGroup = AiDiffList.Find(Function(x) x.LocationIndex = locIndex AndAlso x.BuildingIndex = buildIndex)
        End If

        Return DiffGroup
    End Function

End Class



