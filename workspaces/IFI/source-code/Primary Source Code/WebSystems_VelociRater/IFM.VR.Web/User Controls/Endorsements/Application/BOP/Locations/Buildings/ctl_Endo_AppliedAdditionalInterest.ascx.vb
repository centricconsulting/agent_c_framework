Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions
Imports IFM.VR.Web.EndorsementStructures

Public Class ctl_Endo_AppliedAdditionalInterest
    Inherits VRControlBase

    'Added 05/03/2021 for CAP Endorsements Task 52974 MLW
    Public Event AIListChanged()
    Public Event UpdateTransactionReasonType(ddh As DevDictionaryHelper.DevDictionaryHelper)

    Public Event RemoveAi(AiToRemove As Integer)
    Public Event CountTransactions()
    'Public Event RemoveAi(LocationIndex As Int32, BuildingIndex As Int32, AdditionalInterestIndex As Int32)

    Private Property _devDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
    Public ReadOnly Property ddh() As DevDictionaryHelper.DevDictionaryHelper
        Get
            If Quote IsNot Nothing Then
                Dim DictionaryName = String.Empty
                Select Case Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                        DictionaryName = "CAPEndorsementsDetails"
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                        DictionaryName = "BOPEndorsementDetails"
                    Case Else
                        DictionaryName = "Temp"
                End Select

                If _devDictionaryHelper Is Nothing Then
                    If String.IsNullOrWhiteSpace(DictionaryName) = False Then
                        _devDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, DictionaryName, Quote.LobType)
                    End If
                End If
            End If
            Return _devDictionaryHelper
        End Get
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

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return Me.AdditionalInterestIndex
        End Get
    End Property

    Private ReadOnly Property AllPreExisting As DevDictionaryHelper.AllPreExistingItems
        Get
            Dim _AllPreExisting As DevDictionaryHelper.AllPreExistingItems = Nothing
            If Quote IsNot Nothing Then
                _AllPreExisting = New DevDictionaryHelper.AllPreExistingItems()
                _AllPreExisting.GetAllPreExistingInDevDictionary(Quote)
            End If
            Return _AllPreExisting
        End Get
    End Property



    Public Property AdditionalInterestIndex As Int32
        Get
            If ViewState("vs_interestNum") Is Nothing Then
                ViewState("vs_interestNum") = -1
            End If
            Return CInt(ViewState("vs_interestNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_interestNum") = value
        End Set
    End Property

    Public Property MyBuildingAdditionalInterestIndex As Int32
        Get
            If (ViewState("vs_MyBuildingAdditionalInterestIndex") Is Nothing) Then
                ViewState("vs_MyBuildingAdditionalInterestIndex") = -1
            End If
            Return CInt(ViewState("vs_MyBuildingAdditionalInterestIndex"))
        End Get
        Set(value As Int32)
            ViewState("vs_MyBuildingAdditionalInterestIndex") = value
        End Set
    End Property

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Public Property BuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
        End Set
    End Property
    Public Property MyAdditionalInterest As QuickQuoteAdditionalInterest
        Get
            If AdditionalInterestIndex > -1 AndAlso LocationIndex > -1 AndAlso BuildingIndex > -1 Then
                If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(LocationIndex) Then
                    If Quote.Locations(LocationIndex).Buildings IsNot Nothing AndAlso Quote.Locations(LocationIndex).Buildings.HasItemAtIndex(BuildingIndex) Then
                        If Quote.Locations(LocationIndex).Buildings(BuildingIndex).AdditionalInterests IsNot Nothing AndAlso Quote.Locations(LocationIndex).Buildings(BuildingIndex).AdditionalInterests.HasItemAtIndex(MyBuildingAdditionalInterestIndex) Then
                            Return Quote.Locations(LocationIndex).Buildings(BuildingIndex).AdditionalInterests(MyBuildingAdditionalInterestIndex)
                        End If
                    End If
                End If
            End If
            Return New QuickQuoteAdditionalInterest()
        End Get
        Set(value As QuickQuoteAdditionalInterest)
            If AdditionalInterestIndex > -1 AndAlso LocationIndex > -1 AndAlso BuildingIndex > -1 Then
                If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(LocationIndex) Then
                    If Quote.Locations(LocationIndex).Buildings IsNot Nothing AndAlso Quote.Locations(LocationIndex).Buildings.HasItemAtIndex(BuildingIndex) Then
                        If Quote.Locations(LocationIndex).Buildings(BuildingIndex).AdditionalInterests Is Nothing Then
                            Quote.Locations(LocationIndex).Buildings(BuildingIndex).AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                        End If
                        If Quote.Locations(LocationIndex).Buildings(BuildingIndex).AdditionalInterests.HasItemAtIndex(MyBuildingAdditionalInterestIndex) Then
                            Quote.Locations(LocationIndex).Buildings(BuildingIndex).AdditionalInterests(MyBuildingAdditionalInterestIndex) = value
                        Else
                            Quote.Locations(LocationIndex).Buildings(BuildingIndex).AdditionalInterests.Add(value)
                        End If
                    End If
                End If
            End If
        End Set
    End Property

    Public ReadOnly Property MyAppliedAiList As List(Of LocationBuildingAIItem)
        Get
            Dim appAIs = New Helpers.FindAppliedAdditionalInterestList
            Return appAIs.FindAppliedAI(Quote)
        End Get
    End Property

    Public ReadOnly Property GetAdditionalInterestBuilding(locIndex As Int32, buildIndex As Int32) As QuickQuoteBuilding
        Get
            If locIndex > -1 AndAlso buildIndex > -1 Then
                If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(locIndex) Then
                    If Quote.Locations(locIndex).Buildings IsNot Nothing AndAlso Quote.Locations(locIndex).Buildings.HasItemAtIndex(buildIndex) Then
                        Return Quote.Locations(locIndex).Buildings(buildIndex)
                    End If
                End If
            End If
            Return New QuickQuoteBuilding
        End Get
    End Property
    Public ReadOnly Property TransactionLimitReached As Boolean
        Get
            'get from Direct Parent ViewState
            If Me.ParentVrControl IsNot Nothing AndAlso TypeOf Me.ParentVrControl Is ctl_Endo_AppliedAdditionalInterestList Then
                Dim Parent = CType(ParentVrControl, ctl_Endo_AppliedAdditionalInterestList)
                Return Parent.TransactionLimitReached
            End If
            Return False
        End Get
    End Property





    Dim flaggedForDelete As Boolean = False 'added 4/1/2020

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'If IsSingleAI(MyAdditionalInterest) Then
        '    Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "This Additional Interest is assigned to one or more items. If the Additional Interest is removed it will be removed from all items. Are you sure you want to remove? To remove the Additional Interest from a specific item, please do so from below.")
        'Else
        Me.VRScript.StopEventPropagation(Me.lnkRemove.ClientID)
        'End If
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)

        If String.IsNullOrWhiteSpace(TypeOfEndorsement()) = False Then
            Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
            AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
            Select Case TypeOfEndorsement()
                Case EndorsementTypeString.BOP_AddDeleteContractorsEquipment,
             EndorsementTypeString.BOP_AddDeleteContractorsEquipmentLienholder,
             EndorsementTypeString.BOP_AddDeleteLocationLienholder,
             EndorsementTypeString.BOP_AddDeleteLocation
                    If AllPreExistingItems.PreExisting_AssignedAdditionalInterests.isPreExistingLocationBuildingAIs(LocationIndex + 1, BuildingIndex + 1, MyAdditionalInterest.ListId, MyAdditionalInterest.Description) Then
                        DisableControls()
                        DisableHeaderLinks()
                    End If
                Case EndorsementTypeString.CPP_AddDeleteContractorsEquipmentLienholder,
            EndorsementTypeString.CPP_AddDeleteLocationLienholder
                    If AllPreExistingItems.PreExisting_AssignedAdditionalInterests.isPreExistingLocationBuildingAIs(LocationIndex + 1, BuildingIndex + 1, MyAdditionalInterest.ListId, MyAdditionalInterest.Description) Then
                        DisableControls()
                        DisableHeaderLinks()
                    End If
            End Select
        End If

    End Sub

    Public ReadOnly Property AdditionalInterstIdsCreatedThisSession As List(Of Int32)
        Get
            If Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List") Is Nothing Then
                Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List") = New List(Of Int32)
            End If

            Return Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List")
        End Get

    End Property

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        'Updated 12/1/2021 for CPP Endorsements task 66977 MLW
        'If MyAdditionalInterest IsNot Nothing Then
        If MyAdditionalInterest IsNot Nothing AndAlso (IsQuoteReadOnly() OrElse IsQuoteEndorsement()) Then
            LoadPayeeDDLs()
            SetPersonalPropertyAdditionalInterestFieldsFromAI(MyAdditionalInterest)

            Dim expanderDescription As String = MyAdditionalInterest?.Description
            If String.IsNullOrWhiteSpace(expanderDescription) Then
                If MyAdditionalInterest.TypeId = "42" Then
                    expanderDescription = "FIRST MORTGAGEE"
                Else
                    expanderDescription = "N/A"
                End If
            End If


            Me.lblExpanderText.Text = "AI - " + expanderDescription
            If Me.lblExpanderText.Text.Length > 38 Then
                Me.lblExpanderText.Text = Me.lblExpanderText.Text.Substring(0, 38) + "..."
            End If


            If IsNumeric(MyAdditionalInterest.ListId) Then
#If DEBUG Then
                If Me.AdditionalInterstIdsCreatedThisSession.Contains(MyAdditionalInterest.ListId) Then
                    Me.lblExpanderText.ToolTip = Me.lblExpanderText.Text + " Created this Session EDITABLE AI"
                Else
                    Me.lblExpanderText.ToolTip = Me.lblExpanderText.Text + " READ-ONLY AI - Any changes will create a new AI record."
                End If
#End If
            End If

            Select Case Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    Exit Select
                Case Else
                    Exit Select
            End Select

            'Not limiting 10/08/2021 CAH
            If TransactionLimitReached Then
                Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
                AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
                If AllPreExistingItems.PreExisting_AssignedAdditionalInterests.isPreExistingLocationBuildingAIs(LocationIndex + 1, BuildingIndex + 1, MyAdditionalInterest.ListId, MyAdditionalInterest.Description) Then
                    lnkRemove.Visible = False
                End If
                'DisableHeaderLinks()
            End If

            DescriptionItemDisabling()

        End If
    End Sub

    Private Sub SetPersonalPropertyAdditionalInterestFieldsFromAI(ByVal persPropAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest, Optional ByVal setLossPayeeName As Boolean = True)
        If persPropAI IsNot Nothing Then
            If String.IsNullOrWhiteSpace(persPropAI.Description) = False Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddlGAIDescriptions, persPropAI.Description.ToUpper, persPropAI.Description.ToUpper)
                Me.hdnAiDescription.Value = persPropAI.Description.ToUpper
            End If
            If setLossPayeeName = True Then
                If QQHelper.IsPositiveIntegerString(persPropAI.ListId) = True Then
                    If Me.ddlGAILimitLossPayeeName.Items IsNot Nothing Then
                        If Me.ddlGAILimitLossPayeeName.Items.Count > 0 AndAlso Me.ddlGAILimitLossPayeeName.Items.FindByValue(persPropAI.ListId) IsNot Nothing Then
                            Me.ddlGAILimitLossPayeeName.SelectedValue = persPropAI.ListId
                        Else
                            'add name to dropdown and set

                        End If
                    End If
                End If
            End If
            If QQHelper.IsPositiveIntegerString(persPropAI.TypeId) = True Then
                If Me.ddlGAILimitLossPayeeType.Items IsNot Nothing Then
                    If Me.ddlGAILimitLossPayeeType.Items.Count > 0 AndAlso Me.ddlGAILimitLossPayeeType.Items.FindByValue(persPropAI.TypeId) IsNot Nothing Then
                        Me.ddlGAILimitLossPayeeType.SelectedValue = persPropAI.TypeId
                    Else
                        'add type to dropdown and set

                    End If
                End If
            End If
            If persPropAI.ATIMA = True AndAlso persPropAI.ISAOA = True Then
                Me.ddlGAILimitATMA.SelectedValue = "3"
            ElseIf persPropAI.ATIMA = True Then
                Me.ddlGAILimitATMA.SelectedValue = "1"
            ElseIf persPropAI.ISAOA = True Then
                Me.ddlGAILimitATMA.SelectedValue = "2"
            Else 'ElseIf persPropAI.ATIMA = False AndAlso persPropAI.ISAOA = False Then
                Me.ddlGAILimitATMA.SelectedValue = "0"
            End If

            Dim LocationSetValue As String = "L" + (LocationIndex + 1).ToString + "B" + (BuildingIndex + 1).ToString
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlGAILocation, LocationSetValue)
        End If
    End Sub

    Private Sub LoadPayeeDDLs()

        ddlGAILimitLossPayeeName.Items.Clear()

        If Quote IsNot Nothing AndAlso Quote.AdditionalInterests IsNot Nothing Then
            For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In Quote.AdditionalInterests
                IFM.VR.Web.Helpers.WebHelper_Personal.AddDropDownValueIfMissing(ddlGAILimitLossPayeeName, ai.ListId, ai.Name.DisplayName)
            Next
        End If

        If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing Then
            Dim locIndex As Integer = 0
            If Quote?.Locations.IsLoaded Then
                For Each location In Quote.Locations
                    Dim value As String = String.Empty
                    Dim text As String = String.Empty
                    Dim buildIndex As Integer = 0
                    locIndex += 1
                    If location.Buildings.IsLoaded Then
                        For Each building In location.Buildings
                            buildIndex += 1
                            value = "L" + locIndex.ToString + "B" + buildIndex.ToString
                            text = "Location: " + locIndex.ToString + " Building: " + buildIndex.ToString
                            IFM.VR.Web.Helpers.WebHelper_Personal.AddDropDownValueIfMissing(ddlGAILocation, value, text)
                        Next
                    End If
                Next
            End If
        End If

        DescriptionItemDisabling()

    End Sub

    Private Sub DescriptionItemDisabling()
        If TypeOfEndorsement() = EndorsementTypeString.BOP_AddDeleteLocationLienholder Then
            IFM.VR.Web.Helpers.WebHelper_Personal.AddDropDownValueIfMissing(ddlGAIDescriptions, "BUILDING", "BUILDING")
            IFM.VR.Web.Helpers.WebHelper_Personal.AddDropDownValueIfMissing(ddlGAIDescriptions, "PERSONAL PROPERTY", "PERSONAL PROPERTY")
            AddContractorsEquipmentToDropdown()
            For Each item As ListItem In ddlGAIDescriptions.Items
                If item.Value <> "" AndAlso item.Value.ToUpper <> "BUILDING" AndAlso item.Value.ToUpper <> "PERSONAL PROPERTY" Then
                    item.Attributes.Add("style", "color: gray")
                    item.Attributes.Add("disabled", "disabled")
                End If
            Next
        End If

        If TypeOfEndorsement() = EndorsementTypeString.CPP_AddDeleteLocationLienholder Then
            IFM.VR.Web.Helpers.WebHelper_Personal.AddDropDownValueIfMissing(ddlGAIDescriptions, "BUILDING", "BUILDING")
            IFM.VR.Web.Helpers.WebHelper_Personal.AddDropDownValueIfMissing(ddlGAIDescriptions, "BUSINESS INCOME LIMIT", "BUSINESS INCOME LIMIT")
            IFM.VR.Web.Helpers.WebHelper_Personal.AddDropDownValueIfMissing(ddlGAIDescriptions, "PERSONAL PROPERTY", "PERSONAL PROPERTY")
            IFM.VR.Web.Helpers.WebHelper_Personal.AddDropDownValueIfMissing(ddlGAIDescriptions, "PERSONAL PROPERTY OTHERS", "PERSONAL PROPERTY OTHERS")
            AddContractorsEquipmentToDropdown()
            For Each item As ListItem In ddlGAIDescriptions.Items
                If item.Value <> "" AndAlso item.Value.ToUpper <> "BUILDING" AndAlso item.Value.ToUpper <> "PERSONAL PROPERTY" AndAlso item.Value.ToUpper <> "BUSINESS INCOME LIMIT" AndAlso item.Value.ToUpper <> "PERSONAL PROPERTY" Then
                    item.Attributes.Add("style", "color: gray")
                    item.Attributes.Add("disabled", "disabled")
                End If
            Next
        End If

        If TypeOfEndorsement() = EndorsementTypeString.BOP_AddDeleteContractorsEquipmentLienholder Then
            IFM.VR.Web.Helpers.WebHelper_Personal.AddDropDownValueIfMissing(ddlGAIDescriptions, "BUILDING", "BUILDING")
            IFM.VR.Web.Helpers.WebHelper_Personal.AddDropDownValueIfMissing(ddlGAIDescriptions, "PERSONAL PROPERTY", "PERSONAL PROPERTY")
            AddContractorsEquipmentToDropdown()
            For Each item As ListItem In ddlGAIDescriptions.Items
                If item.Value = "BUILDING" AndAlso item.Value = "PERSONAL PROPERTY" Then
                    item.Attributes.Add("style", "color: gray")
                    item.Attributes.Add("disabled", "disabled")
                End If
            Next
        End If
    End Sub
    Private Sub AddContractorsEquipmentToDropdown()
        If GoverningStateQuote() IsNot Nothing AndAlso GoverningStateQuote.ContractorsEquipmentScheduledItems.IsLoaded Then
            For Each item As QuickQuoteContractorsEquipmentScheduledItem In GoverningStateQuote.ContractorsEquipmentScheduledItems
                Dim Desc As String = item.Description
                If String.IsNullOrWhiteSpace(Desc) Then
                    Desc = Desc.ToUpper
                End If
                IFM.VR.Web.Helpers.WebHelper_Personal.AddDropDownValueIfMissing(ddlGAIDescriptions, Desc, Desc)
            Next
        End If
    End Sub
    ''' <summary>
    ''' Disables all the controls on the page
    ''' </summary>
    Private Sub DisableControls()
        VRScript.AddScriptLine("$(document).ready(function () {ifm.vr.ui.SingleContainerContentDisable(['" + Me.divAiEntry.ClientID + "']);});")
    End Sub

    Private Sub DisableHeaderLinks()
        'Me.lnkRemove.Visible = False
        Me.lnkSave.Visible = False
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If Me.Visible Then
            MyBase.ValidateControl(valArgs)
            ValidationHelper.GroupName = "Assigned AI Description"
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    If Not AllPreExisting.PreExisting_AssignedAdditionalInterests.isPreExistingLocationBuildingAIs(LocationIndex + 1, BuildingIndex + 1, MyAdditionalInterest.ListId, MyAdditionalInterest.Description) Then
                        If ddlGAIDescriptions.SelectedIndex <= 0 Then
                            Me.ValidationHelper.Val_BindValidationItemToControl(ddlGAIDescriptions, New IFM.VR.Validation.ObjectValidation.ValidationItem("Description - N/A is not allowed in new Assignments"), MyAccordionList)
                        End If
                    End If
            End Select
            Dim paneIndex = Me.AdditionalInterestIndex
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it


        End If
    End Sub

    Public Overrides Function Save() As Boolean

        If Me.Visible Then
            If Quote IsNot Nothing Then

                Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
                AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
                If AllPreExistingItems.PreExisting_AssignedAdditionalInterests.isPreExistingLocationBuildingAIs(LocationIndex + 1, BuildingIndex + 1, MyAdditionalInterest.ListId, MyAdditionalInterest.Description) Then
                    Exit Function
                End If

                'Get AI Index change tracker "DiffList"
                Dim DiffList As AiDifferentialList = New AiDifferentialList
                If Me.ParentVrControl IsNot Nothing AndAlso TypeOf Me.ParentVrControl Is ctl_Endo_AppliedAdditionalInterestList Then
                    DiffList = ParentVrControl.VrViewState("vs_DiffList")
                End If

                'Check if current Index needs updated because we have deleted previous items from the list of AIs
                If DiffList IsNot Nothing Then
                    Dim DiffAdjustment = DiffList.GetAIDifferential(LocationIndex, BuildingIndex, MyBuildingAdditionalInterestIndex)
                    If DiffAdjustment <> 0 Then
                        MyBuildingAdditionalInterestIndex = MyBuildingAdditionalInterestIndex - DiffAdjustment
                    End If
                End If

                'Generated from Form - aka New AI info
                Dim FormItem = GetLocationBuildingAiInfo(ddlGAILocation, LoadAIInfoFromForm())
                FormItem.AI.Other = MyAdditionalInterest.Other

                'Prep BopDictItems
                BopDictItems.GetAllCommercialDictionary()

                If FormItem.LocationIndex = LocationIndex AndAlso FormItem.BuildingIndex = BuildingIndex Then
                    'Item is on the same building, so just update with possible new form info
                    BopDictItems.UpdateAssignedAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Update,
                        New DevDictionaryHelper.AssignedAI(LocationIndex, BuildingIndex, FormItem.AI.ListId, FormItem.AI.Description, DevDictionaryHelper.DevDictionaryHelper.addItem),
                        New DevDictionaryHelper.AssignedAI(LocationIndex, BuildingIndex, MyAdditionalInterest.ListId, MyAdditionalInterest.Description, DevDictionaryHelper.DevDictionaryHelper.addItem))

                    'Update the Quote
                    Dim Location = GetAdditionalInterestBuilding(FormItem.LocationIndex, FormItem.BuildingIndex)
                    If Location.AdditionalInterests Is Nothing Then
                        Location.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                    End If
                    If MyAdditionalInterest.HasValidAdditionalInterestListId = False Then
                        Location.AdditionalInterests.Add(FormItem.AI)
                    Else
                        Location.AdditionalInterests(MyBuildingAdditionalInterestIndex) = FormItem.AI
                    End If
                Else
                    'remove from old building
                    Dim OldLocation = GetAdditionalInterestBuilding(LocationIndex, BuildingIndex)
                    If OldLocation.AdditionalInterests IsNot Nothing Then
                        If OldLocation.AdditionalInterests.HasItemAtIndex(MyBuildingAdditionalInterestIndex) Then
                            'BopDictItems.AssignedAdditionalInterests.Remove(New DevDictionaryHelper.AssignedAI(LocationIndex, BuildingIndex, MyAdditionalInterest.ListId, MyAdditionalInterest.Description, DevDictionaryHelper.DevDictionaryHelper.addItem))
                            BopDictItems.UpdateAssignedAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, New DevDictionaryHelper.AssignedAI(LocationIndex, BuildingIndex, MyAdditionalInterest.ListId, MyAdditionalInterest.Description, DevDictionaryHelper.DevDictionaryHelper.addItem))
                            OldLocation.AdditionalInterests.RemoveAt(MyBuildingAdditionalInterestIndex)
                            DiffList.AddAiIndex(LocationIndex, BuildingIndex, MyBuildingAdditionalInterestIndex)

                        End If
                    End If

                    'add to New building, update with possible new form info
                    Dim NewLocation = GetAdditionalInterestBuilding(FormItem.LocationIndex, FormItem.BuildingIndex)
                    If NewLocation.AdditionalInterests Is Nothing Then
                        NewLocation.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                    End If
                    NewLocation.AdditionalInterests.Add(FormItem.AI)
                    BopDictItems.UpdateAssignedAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add,
                                                                   New DevDictionaryHelper.AssignedAI(FormItem.LocationIndex, FormItem.BuildingIndex, FormItem.AI.ListId, FormItem.AI.Description, DevDictionaryHelper.DevDictionaryHelper.addItem))

                End If
                'UpdateCurrentAIInfoFromForm(FormItem)
                'RaiseEvent AIListChanged() 'Removed 3/7/2022 for bug 73427 MLW

                'ParentVrControl.Populate()

                Return True
            End If
        End If

        Return False
    End Function


    Private Function LoadAIInfoFromForm() As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest
        Dim NewAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()

        If Me.Quote IsNot Nothing AndAlso Quote.AdditionalInterests IsNot Nothing Then

            Dim sourceAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteAdditionalInterestForListId(Me.Quote.AdditionalInterests, Me.ddlGAILimitLossPayeeName.SelectedValue, cloneAI:=False, firstOrLastItem:=QuickQuote.CommonMethods.QuickQuoteHelperClass.FirstOrLast.First, matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing, returnNewIfNothing:=False)
            If sourceAI IsNot Nothing Then
                QQHelper.CopyQuickQuoteAdditionalInterestNameAddressEmailsAndPhones(sourceAI, NewAI)
            End If
            If String.IsNullOrWhiteSpace(ddlGAIDescriptions.SelectedValue) Then
                NewAI.Description = Me.hdnAiDescription.Value.ToMaxLength(249).ToUpper
            Else
                NewAI.Description = ddlGAIDescriptions.SelectedValue.ToMaxLength(249).ToUpper
            End If
            'NewAI.Description = ddlGAIDescriptions.SelectedValue.ToMaxLength(249)
            NewAI.TypeId = ddlGAILimitLossPayeeType.SelectedValue
            NewAI.ListId = Me.ddlGAILimitLossPayeeName.SelectedValue
            Select Case ddlGAILimitATMA.SelectedValue
                Case 0
                    NewAI.ATIMA = False
                    NewAI.ISAOA = False
                Case 1
                    NewAI.ATIMA = True
                    NewAI.ISAOA = False
                Case 2
                    NewAI.ATIMA = False
                    NewAI.ISAOA = True
                Case 3
                    NewAI.ATIMA = True
                    NewAI.ISAOA = True
            End Select
            Return NewAI
        End If

        Return Nothing
    End Function

    Private Sub UpdateCurrentAIInfoFromForm(ByRef updateAi As LocationBuildingAIItem)

        If MyAdditionalInterest IsNot Nothing AndAlso updateAi IsNot Nothing Then

            LocationIndex = updateAi.LocationIndex
            BuildingIndex = updateAi.BuildingIndex

            MyAdditionalInterest = updateAi.AI
        End If
        If updateAi.LocationIndex <> LocationIndex OrElse updateAi.BuildingIndex <> BuildingIndex Then
            If Quote.Locations(updateAi.LocationIndex).Buildings(updateAi.BuildingIndex).AdditionalInterests Is Nothing Then
                Quote.Locations(updateAi.LocationIndex).Buildings(updateAi.BuildingIndex).AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            End If
            Dim updatedBuildingIndex = Quote.Locations(updateAi.LocationIndex).Buildings(updateAi.BuildingIndex).AdditionalInterests.Count
            Quote.Locations(updateAi.LocationIndex).Buildings(updateAi.BuildingIndex).AdditionalInterests.Add(MyAdditionalInterest)
            Quote.Locations(LocationIndex).Buildings(BuildingIndex).AdditionalInterests.RemoveAt(MyBuildingAdditionalInterestIndex)

            ParentVrControl.Populate()
        End If
    End Sub

    Private Function GetLocationBuildingAiInfo(LocationDDL As DropDownList, AI As QuickQuoteAdditionalInterest) As LocationBuildingAIItem
        Dim LocData = LocationDDL.SelectedValue
        Dim LocSplit = LocData.TrimStart("l").Split("b")
        Dim Location = 0
        Dim Building = 0
        If Integer.TryParse(LocSplit(0), Location) = False OrElse Integer.TryParse(LocSplit(1), Building) = False Then
            Return New LocationBuildingAIItem(LocationIndex, BuildingIndex, MyBuildingAdditionalInterestIndex, AI)
        End If
        Return New LocationBuildingAIItem(Location - 1, Building - 1, MyBuildingAdditionalInterestIndex, AI)
    End Function



    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        flaggedForDelete = True
        'RaiseEvent RemoveAi(GetLocationBuildingAiInfo(ddlGAILocation, LoadAIInfoFromForm()))
        RaiseEvent RemoveAi(AdditionalInterestIndex)
    End Sub

    Protected Sub checkTransactionReasonType(ddh As DevDictionaryHelper.DevDictionaryHelper)
        RaiseEvent UpdateTransactionReasonType(ddh)

    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        ' FOR COMMERCIAL LINES - Don't fire the validation because we're making the user save any additional interests before they can use them
        ' further down the page and we don't want to ding them for field validations when we save the additional interest
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                Me.Save_FireSaveEvent(False)
                Exit Select
            Case Else
                Me.Save_FireSaveEvent(True)
                Exit Select
        End Select
        RaiseEvent AIListChanged()
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
    End Sub

    Public Function IsSingleAI(MyAddInterest As QuickQuoteAdditionalInterest) As Boolean
        If MyAppliedAiList IsNot Nothing AndAlso String.IsNullOrWhiteSpace(MyAddInterest?.ListId) = False Then
            Dim count = MyAppliedAiList.FindAll(Function(x) x.AI.ListId = MyAddInterest.ListId).Count < 2
            Return count
        End If
        Return False
    End Function


End Class