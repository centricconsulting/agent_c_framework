Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.PrimativeExtensions

Public Class ctlVehicleAdditionalInterestList
    Inherits VRControlBase

    Public Event AIChange()
    Public Event AddFakeAIIfNeeded()
    Public Event RemoveFakeAI()

    Public ReadOnly CAPEndorsementsDictionaryName = "CAPEndorsementsDetails" 'Added 04/29/2021 for CAP Endorsements Task 52974 MLW

    'Added 04/29/2021 for CAP Endorsements Task 52974 MLW
    Private Property _devDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
    Public ReadOnly Property ddh() As DevDictionaryHelper.DevDictionaryHelper
        Get
            If _devDictionaryHelper Is Nothing Then
                If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(CAPEndorsementsDictionaryName) = False Then
                    _devDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, CAPEndorsementsDictionaryName, Quote.LobType)
                End If
            End If
            Return _devDictionaryHelper
        End Get
    End Property

    Private Property NeedToShowAIPopup As Boolean
    Private PopupCounter As Integer = 0

    Public Property VehicleIndex As Int32
        Get
            If ViewState("vs_vehicleNum") Is Nothing Then
                ViewState("vs_vehicleNum") = -1
            End If
            Return CInt(ViewState("vs_vehicleNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_vehicleNum") = value
        End Set
    End Property

    Private ReadOnly Property QuoteVehicleAICount As Integer
        Get
            Dim AICount As Integer = 0
            If Quote IsNot Nothing Then
                If Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
                    For Each veh As QuickQuote.CommonObjects.QuickQuoteVehicle In Quote.Vehicles
                        If veh.AdditionalInterests IsNot Nothing AndAlso veh.AdditionalInterests.Count > 0 Then
                            AICount += veh.AdditionalInterests.Count
                        End If
                    Next
                End If
            End If
            Return AICount
        End Get
    End Property

    Public ReadOnly Property AdditionalInterstIdsCreatedThisSession As List(Of Int32)
        Get
            'If Session(Me.QuoteId + "_AI_Created_List") Is Nothing Then
            '    Session(Me.QuoteId + "_AI_Created_List") = New List(Of Int32)
            'End If

            'Return CType(Session(Me.QuoteId + "_AI_Created_List"), List(Of Int32))
            'updated 2/16/2021
            If Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List") Is Nothing Then
                Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List") = New List(Of Int32)
            End If

            Return CType(Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List"), List(Of Int32))
        End Get

    End Property

    Public Property MyAiList As List(Of QuickQuoteAdditionalInterest)
        Get
            Dim AiList As List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest) = Nothing
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
                    If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > Me.VehicleIndex Then
                        vehicle = Me.Quote.Vehicles(Me.VehicleIndex)
                        If vehicle IsNot Nothing Then
                            AiList = vehicle.AdditionalInterests
                        End If
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    If Quote?.Locations IsNot Nothing And Quote.Locations.HasItemAtIndex(0) Then
                        AiList = Me.Quote.Locations(0).AdditionalInterests
                    End If

                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    If Quote?.Locations IsNot Nothing And Quote.Locations.HasItemAtIndex(0) Then
                        AiList = Me.Quote.Locations(0).AdditionalInterests
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    AiList = Me.Quote.AdditionalInterests
                    ''Added 3/25/2021 for CAP Endorsements task 52974 MLW
                    'If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                    '    Dim aiListIds As List(Of Integer) = Nothing
                    '    If Me.Quote.AdditionalInterests IsNot Nothing AndAlso Me.Quote.AdditionalInterests.Count > 0 Then
                    '        For Each ai As QuickQuoteAdditionalInterest In Me.Quote.AdditionalInterests
                    '            If ai IsNot Nothing Then
                    '                QuickQuote.CommonMethods.QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(QQHelper.IntegerForString(ai.ListId), aiListIds, positiveOnly:=True)
                    '            End If
                    '        Next
                    '    End If
                    '    If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > 0 Then
                    '        For Each v As QuickQuoteVehicle In Me.Quote.Vehicles
                    '            If v IsNot Nothing AndAlso v.AdditionalInterests IsNot Nothing AndAlso v.AdditionalInterests.Count > 0 Then
                    '                For Each vai As QuickQuoteAdditionalInterest In v.AdditionalInterests
                    '                    If vai IsNot Nothing AndAlso vai.HasValidAdditionalInterestListId = True Then
                    '                        If aiListIds Is Nothing OrElse aiListIds.Count = 0 OrElse aiListIds.Contains(QQHelper.IntegerForString(vai.ListId)) = False Then
                    '                            'If AiList Is Nothing Then
                    '                            '    AiList = New List(Of QuickQuoteAdditionalInterest)
                    '                            'End If
                    '                            'AiList.Add(QQHelper.CloneObject(vai))
                    '                            'QuickQuote.CommonMethods.QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(QQHelper.IntegerForString(vai.ListId), aiListIds, positiveOnly:=True)
                    '                            Dim copiedAI As QuickQuoteAdditionalInterest = QQHelper.CloneObject(vai)
                    '                            If copiedAI IsNot Nothing Then
                    '                                If AiList Is Nothing Then
                    '                                    AiList = New List(Of QuickQuoteAdditionalInterest)
                    '                                End If
                    '                                copiedAI.Num = "" 'clearing out in case it came over from source AI
                    '                                AiList.Add(copiedAI)
                    '                                QuickQuote.CommonMethods.QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(QQHelper.IntegerForString(vai.ListId), aiListIds, positiveOnly:=True)
                    '                            End If
                    '                        End If
                    '                    End If
                    '                Next
                    '            End If
                    '        Next
                    '    End If
                    'End If
            End Select

            ' Do NOT return anything if the only ai is fake  Per Bug 32112 MGB 4-17-19
            'If AiList IsNot Nothing AndAlso AiList.Count = 1 Then
            '    If IFM.VR.Common.Helpers.AdditionalInterest.IsFakeAI(AiList(0)) Then Return Nothing
            'End If

            Return AiList
        End Get
        Set(value As List(Of QuickQuoteAdditionalInterest))
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
                    If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > Me.VehicleIndex Then
                        vehicle = Me.Quote.Vehicles(Me.VehicleIndex)
                        If vehicle IsNot Nothing Then
                            vehicle.AdditionalInterests = value
                        End If
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() AndAlso Me.Quote.Locations(0) IsNot Nothing Then
                        Me.Quote.Locations(0).AdditionalInterests = value
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() AndAlso Me.Quote.Locations(0) IsNot Nothing Then
                        Me.Quote.Locations(0).AdditionalInterests = value
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    Me.Quote.AdditionalInterests = value
            End Select
        End Set
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

            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Any() AndAlso Quote.Locations(0) IsNot Nothing AndAlso Quote.Locations(0).AdditionalInterests IsNot Nothing AndAlso Quote.Locations(0).AdditionalInterests.Any() Then
                        hasMortgageeType = (From a In Quote.Locations(0).AdditionalInterests Where a IsNot Nothing AndAlso a.TypeId = "42" Select a).Any()
                    End If
            End Select
            If Me.VRScript IsNot Nothing Then
                Me.VRScript.AddVariableLine("var hasMortgageeTypeAI = " + hasMortgageeType.ToString().ToLower() + ";")
                Me.VRScript.AddVariableLine("var AIBillToCheckBoxClientIdArray = new Array();")
            End If

        End If
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.Quote IsNot Nothing Then
            If MyAiList IsNot Nothing AndAlso MyAiList.Any() Then
                'Updated 12/16/2021 for CPP Task 67310 MLW
                If IsQuoteReadOnly() AndAlso Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage AndAlso (Me.ParentVrControl IsNot Nothing AndAlso TypeOf Me.ParentVrControl Is ctl_CPP_InlandMarine) Then
                    'if parent control ctl_CPP_InlandMarine for a view only CPP then collapse accordion, otherwise expand (expanded for property locations page)
                    Me.VRScript.CreateAccordion(Me.divAdditionalInterests.ClientID, Nothing, "1")
                Else
                    Me.VRScript.CreateAccordion(Me.divAdditionalInterests.ClientID, hiddenAdditionalInterest, "false")
                End If
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
            Dim aiControl As ctlVehicleAdditionalInterest = CType(cntrl.FindControl("ctlVehicleAdditionalInterest"), ctlVehicleAdditionalInterest)
            AddHandler aiControl.SaveRequested, AddressOf aiControlRequestedSave
            AddHandler aiControl.RemoveAi, AddressOf aiRemovalRequested
            AddHandler aiControl.AIListChanged, AddressOf AIListWasChanged
        Next
    End Sub

    Private Sub ShowAIPopup()
        ' Only show once per session
        If PopupCounter = 0 Then
            NeedToShowAIPopup = True
            ctlAIPopup.Visible = True
            ctlAIPopup.ShowPopup()
            PopupCounter += 1
        End If
        Exit Sub
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            If Not NeedToShowAIPopup Then ctlAIPopup.Visible = False
            divEndorsementButtons.Visible = False 'Added 04/29/2021 for CAP Endorsements Task 52974 MLW
            'Updated 12/23/2020 for CAP Endorsements Task 52974 MLW
            If AllowPopulate() Then
                Me.Repeater1.DataSource = MyAiList
                Me.Repeater1.DataBind()

                Me.FindChildVrControls() ' finds the just added controls do to the binding

                ' If we're on the quote side disable add/save  Bug 32112 MGB 4-17-19
                If IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
                    lnkBtnAdd.Visible = True
                    lnkBtnSave.Visible = True
                Else
                    lnkBtnAdd.Visible = False
                    lnkBtnSave.Visible = False
                End If

                If MyAiList IsNot Nothing AndAlso MyAiList.Any() Then
                    If Not IsPostBack Then
                        Me.OpenAccordionAtIndex("0") ' open when there are AIs on first load
                    End If

                    Select Case Me.Quote.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.Farm
                            Me.lblHeader.Text = String.Format("Additional Insureds ({0})", MyAiList.Count)
                            Me.lnkBtnAdd.Text = "Add Additional Insured"
                            Me.lnkBtnAdd.ToolTip = "Add Additional Insured"
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                            Me.lblHeader.Text = String.Format("Additional Interests ({0}) (Property/IM)", MyAiList.Count)
                        Case Else
                            Me.lblHeader.Text = String.Format("Additional Interests ({0})", MyAiList.Count)
                    End Select

                    Me.divAdditionalInterestItems.Visible = True
                    Dim index As Int32 = 0
                    For Each child In Me.ChildVrControls
                        If TypeOf child Is ctlVehicleAdditionalInterest Then
                            Dim c As ctlVehicleAdditionalInterest = CType(child, ctlVehicleAdditionalInterest)
                            'c.AIMainAccordID = Me.AIMainAccordID
                            'c.AIItemsAccordID = Me.AIItemsAccordID
                            c.VehicleIndex = Me.VehicleIndex
                            c.AdditionalInterestIndex = index
                            c.Populate()
                            index += 1
                        End If
                    Next
                Else
                    Select Case Me.Quote.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.Farm
                            Me.lblHeader.Text = "Additional Insureds (0)"
                            Me.lnkBtnAdd.Text = "Add Additional Insured"
                            Me.lnkBtnAdd.ToolTip = "Add Additional Insured"
                        Case Else
                            Me.lblHeader.Text = "Additional Interests (0)"
                    End Select
                    Me.divAdditionalInterestItems.Visible = False
                End If
                'End If               

                'Added 04/29/2021 for CAP Endorsements Task 52974 MLW
                If IsQuoteEndorsement() Then
                    If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                        divEndorsementButtons.Visible = True
                    End If
                    If TypeOfEndorsement() = "Add/Delete Additional Interest" Then
                        'Need to show the max 3 transaction message and disable the add AI button & link when the max AI transactions reach 3.
                        Dim transactionCount As Integer = ddh.GetEndorsementAdditionalInterestTransactionCount()
                        If transactionCount >= 3 Then
                            divEndorsementMaxTransactionsMessage.Visible = True
                            btnAddAdditionalInterest.Enabled = False
                            lnkBtnAdd.Visible = False
                        Else
                            divEndorsementMaxTransactionsMessage.Visible = False
                            btnAddAdditionalInterest.Enabled = True
                            lnkBtnAdd.Visible = True
                        End If
                    End If
                End If
            End If
        End If

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        If Me.Quote IsNot Nothing Then
            'Updated 12/29/2020 for CAP Endorsements Task 52974 MLW
            If AllowValidateAndSave() Then
                Select Case Me.Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                        Me.ValidationHelper.GroupName = String.Format("Vehicle #{0} Additional Interests", Me.VehicleIndex + 1)
                    Case Else
                        Me.ValidationHelper.GroupName = "Additional Interests"
                End Select

                Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

                Select Case Me.Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                        'added 8/1/2019
                        Dim currentWorkflow As Common.Workflow.Workflow.WorkflowSection = Common.Workflow.Workflow.WorkflowSection.na
                        If Me.Page IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Me.Page.AppRelativeVirtualPath) = False Then
                            If UCase(Me.Page.AppRelativeVirtualPath).Contains("VR3AUTO.ASPX") = True Then
                                currentWorkflow = DirectCast(Me.Page, VR3Auto).WorkFlowManager.CurrentWorkFlow()
                            End If
                        End If

                        Dim vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.ValidateAdditionalInterestList(Me.Quote, valArgs.ValidationType, CInt(Me.VehicleIndex))
                        For Each v In vals
                            'If IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
                            'updated 8/1/2019
                            If IsOnAppPage OrElse (IsQuoteEndorsement() AndAlso (currentWorkflow = Common.Workflow.Workflow.WorkflowSection.na OrElse currentWorkflow = Common.Workflow.Workflow.WorkflowSection.coverages)) Then
                                Select Case v.FieldId
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.VehicleRequiredAiButNone
                                        Me.ValidationHelper.AddError("Additional Interest is required when Loan/Lease coverage option is applied to vehicle.")
                                        With Me.ValidationHelper.GetLastError()
                                            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                                            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(Me.lnkBtnAdd.ClientID))
                                        End With
                                        'Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.CompCollRequiredWithAi
                                        '    Me.ValidationHelper.AddError(v.Message)
                                        '    With Me.ValidationHelper.GetLastError()
                                        '        .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(VehicleItemsAccordID, VehicleIndex.ToString()))
                                        '        .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(Me.lnkBtnAdd.ClientID))
                                        '    End With
                                End Select
                            End If
                        Next
                    Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                        Dim vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.ValidateAdditionalInterestList(Me.Quote, valArgs.ValidationType)
                        For Each v In vals
                            Select Case v.FieldId
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.MortgageeTypeUsedMultipleTimes
                                    Me.ValidationHelper.Val_BindValidationItemToControl("", v, accordList(0).AccordDivId, accordList(0).AccordIndex)
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.MultipleBillToFlagsSet
                                    Me.ValidationHelper.Val_BindValidationItemToControl("", v, accordList(0).AccordDivId, accordList(0).AccordIndex)
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.HasThirdMortgagee
                                    If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                                        Me.ValidationHelper.AddError(v.Message)
                                    End If
                            End Select
                        Next
                        ' make sure only one mortgagee AI has the billto flag as True/ also confirm that the AI Type is Mortgagee 1/2/or 3

                End Select
            End If
        End If
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            'Updated 12/29/2020 for CAP Endorsements Task 52974 MLW
            If AllowValidateAndSave() Then
                Me.SaveChildControls()
                Return True
            End If
        End If

        Return False
    End Function

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
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

        Me.Populate()
    End Sub

    'Updated 04/29/2021 for CAP Endorsements Task 52974 MLW
    'Public Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkBtnAdd.Click
    Public Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkBtnAdd.Click, btnAddAdditionalInterest.Click
        If Quote IsNot Nothing Then

            RemoveFakeOn_PPA()
            'Save event needs to happen before we attempt to add the additional interest. If it isn't, when someone attempts to add two AI's, without manually specifying a save inbetween, the MyAiList will not
            'be updated with the most recent attempt to add. This forces the use to have to click the "Add Additional Interest" button twice to actuall add one.
            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

            ' Show AI popup when the AI being added is the first on the quote - Bug 60188
            ' Only show on NB App side, do not show for endorsements.
            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal _
                AndAlso (Not Me.IsQuoteEndorsement()) _
                AndAlso QuoteVehicleAICount() = 0 Then _
                'AndAlso (Quote.Vehicles?(VehicleIndex).AdditionalInterests Is Nothing _
                'OrElse Quote.Vehicles(VehicleIndex).AdditionalInterests.Count <= 0) Then
                ShowAIPopup()
            End If

            If MyAiList Is Nothing Then
                MyAiList = New List(Of QuickQuoteAdditionalInterest)()
            End If

            Dim interest As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest
            MyAiList.Add(interest)

            LockTree()

            OpenAccordionAtIndex((MyAiList.Count() - 1).ToString())

            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                VRScript.AddScriptLine("alert('Changing number of AI on policy could change premium.');", True, False, True)
            End If

            Me.Populate()
        End If
    End Sub

    Private Sub aiControlRequestedSave(args As VrControlBaseSaveEventArgs)
        Me.Populate()
    End Sub

    Private Sub AIListWasChanged()
        RaiseEvent AIChange()
    End Sub

    Private Sub aiRemovalRequested(index As Integer)
        If Quote IsNot Nothing Then
            'Added 05/10/2021 for CAP Endorsements Task 52974 MLW; 5/22/2021: reverted to previous logic so we can 1st Save any un-saved info as-is before the AI removal (newer logic doesn't appear to be correct anyway as it would've also not saved for CAP NewBusiness, which was not likely the intention)
            'If Not IsQuoteEndorsement() AndAlso Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
            '    Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            'End If
            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                VRScript.AddScriptLine("alert('Changing number of AI on policy could change premium.');", True, False, True)
            End If

            If MyAiList IsNot Nothing Then
                If MyAiList.Count > index Then

                    If MyAiList(index).TypeId = "56" Then
                        If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() AndAlso Me.Quote.Locations(0).SectionIICoverages IsNot Nothing Then
                            Dim additionalInsuredList As List(Of QuickQuoteSectionIICoverage) = Me.Quote.Locations(0).SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Occupying_Residence_on_Premises)
                            If additionalInsuredList.Any() Then
                                Me.Quote.Locations(0).SectionIICoverages.Remove(additionalInsuredList(0))
                            End If
                        End If
                    End If

                    'added 5/22/2021; removed 5/23/2021
                    'Dim wouldAiGetCopiedBack As Boolean = WouldAdditionalInterestGetAddBackOnSuccessfulSave(index)
                    'added 5/23/2021 for CAP Endorsements Task 52974 MLW
                    Dim aiToRemove As QuickQuoteAdditionalInterest = MyAiList(index)

                    MyAiList.RemoveAt(index)

                    'If wouldAiGetCopiedBack = False Then 'added IF 5/22/2021 - there's no point to Save if the AI will be copied back; Save will happen again below anyway; see Matt's note below from 7/22/14... will this be a problem for whatever scenario the note was in reference to?; removed IF 5/23/2021... will just check to see if it was re-added and then re-remove if so
                    '5/22/2021 note: code below is doing an AppGap Save even though this control could be on the Quote side (as-is the case with Endorsements; may also be used on the Quote side for PPA NewBus?; Save logic should probably be updated to check IsOnAppPage - done)
                    ' this is weird but you need this database save now - otherwise the wrong AI get removed later '7-22-14 Matt A
                    If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/15/2019; original logic in ELSE
                        'note: could also check for Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                        'no Save needed
                    ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                        'note: could also check for Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                        Dim endorsementSaveError As String = ""
                        'Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError, saveTypeView:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
                        'updated 5/22/2021 to check IsOnAppPage
                        Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError, saveTypeView:=If(IsOnAppPage = True, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote))
                    Else
                        'VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, Nothing, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
                        'updated 5/22/2021 to check IsOnAppPage
                        VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, Nothing, saveType:=If(IsOnAppPage = True, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote))
                    End If
                    'End If

                    'added 5/23/2021 for CAP Endorsements Task 52974 MLW
                    Dim aiToRemoveFromCurrentList As QuickQuoteAdditionalInterest = Nothing
                    If WasAdditionalInterestCopiedBackOnSave(aiToRemove, aiFromCurrentList:=aiToRemoveFromCurrentList) = True AndAlso aiToRemoveFromCurrentList IsNot Nothing Then
                        MyAiList.Remove(aiToRemoveFromCurrentList)
                    End If
                    If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal _
                        AndAlso Quote.Vehicles IsNot Nothing _
                        AndAlso Quote.Vehicles.Count > VehicleIndex Then
                        If Quote.Vehicles(VehicleIndex).AdditionalInterests Is Nothing OrElse Quote.Vehicles(VehicleIndex).AdditionalInterests.Count <= 0 Then
                            ' Remove Loan/Lease
                            Quote.Vehicles(VehicleIndex).HasAutoLoanOrLease = False
                        End If
                    End If

                    Me.Populate() ' repopulates with the ai gone
                    Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType))) ' saves the rest of the form after the ai was rempved
                    RaiseEvent AIChange()
                End If
            End If
        End If
    End Sub
    'added 5/22/2021; removed 5/23/2021
    'Private Function WouldAdditionalInterestGetAddBackOnSuccessfulSave(ByVal index As Integer) As Boolean
    '    Dim wouldIt As Boolean = False

    '    '5/23/2021 note: this won't handle all scenarios as written since it could also be a top-level AI that is already assigned at a lower level
    '    If Me.Quote IsNot Nothing AndAlso MyAiList IsNot Nothing AndAlso MyAiList.Count > index AndAlso MyAiList(index) IsNot Nothing AndAlso MyAiList(index).OriginalSourceAI IsNot Nothing Then
    '        'it's an AI that was copied over from a lower-level (i.e. from one of the vehicles in the case of CAP)
    '        If QuickQuote.CommonMethods.QuickQuoteHelperClass.OkayToCopySourceAdditionalInterestsToTopLevelOnRetrieval_With_Optional_TransactionTypeText_And_Or_ThreeLetterLobAppreviation(tranType:=Me.Quote.QuoteTransactionType, lobType:=Me.Quote.LobType) = True Then
    '            wouldIt = True
    '        End If
    '    End If

    '    Return wouldIt
    'End Function
    'added 5/23/2021 for CAP Endorsements Task 52974 MLW
    Private Function WasAdditionalInterestCopiedBackOnSave(ByVal ai As QuickQuoteAdditionalInterest, Optional ByRef aiFromCurrentList As QuickQuoteAdditionalInterest = Nothing) As Boolean
        aiFromCurrentList = Nothing
        Dim wasIt As Boolean = False

        If ai IsNot Nothing AndAlso ai.HasValidAdditionalInterestListId() = True AndAlso MyAiList IsNot Nothing AndAlso MyAiList.Count > 0 Then
            aiFromCurrentList = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteAdditionalInterestForListId(MyAiList, ai.ListId, cloneAI:=False, returnNewIfNothing:=False)
            If aiFromCurrentList IsNot Nothing Then
                wasIt = True
            End If
        End If

        Return wasIt
    End Function

    Public Sub OpenAccordionAtIndex(index As String)
        Me.hiddenAdditionalInterest.Value = "0"
        Me.hiddenAdditionalInterestItems.Value = index
    End Sub

    Public Sub InsureProperFakeAI_PPA()
        If Quote?.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
            If MyAiList Is Nothing OrElse MyAiList.Count = 0 Then
                Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
                If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > Me.VehicleIndex Then
                    vehicle = Me.Quote.Vehicles(Me.VehicleIndex)
                    vehicle.AdditionalInterests = Nothing
                    RaiseEvent AddFakeAIIfNeeded()
                End If
            End If
        End If
    End Sub

    Public Sub RemoveFakeOn_PPA()
        If Quote?.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
            RaiseEvent RemoveFakeAI()
        End If
    End Sub

    'Added 02/01/2021 For CAP Endorsements Task 52974 MLW
    Private Function AllowPopulate() As Boolean
        Select Case Me.Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                If IsOnAppPage Then
                    Return True
                ElseIf IsQuoteReadOnly() Then
                    Return True
                ElseIf (IsQuoteEndorsement() AndAlso (TypeOfEndorsement() = "Add/Delete Vehicle" OrElse TypeOfEndorsement() = "Add/Delete Additional Interest")) Then
                    'lock down AIs - do not populate when on the Add/Delete Drivers or Amend Mailing Address type of endorsement. Only populate for Add/Delete AIs and Add/Delete Vehicles.
                    Return True
                Else
                    Return False
                End If
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP,
                 QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                'Added 09/13/2021 for BOP Endorsements Task 63912 MLW
                If IsOnAppPage OrElse IsQuoteReadOnly() Then
                    Return True
                Else
                    Return False
                End If
            Case Else
                Return True
        End Select
    End Function

    'Added 02/01/2021 For CAP Endorsements Task 52974 MLW
    Private Function AllowValidateAndSave() As Boolean
        Select Case Me.Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                If IsOnAppPage Then
                    Return True
                ElseIf (IsQuoteEndorsement() AndAlso (TypeOfEndorsement() = "Add/Delete Additional Interest" OrElse TypeOfEndorsement() = "Add/Delete Vehicle")) Then
                    'Do not validate and save when not on the vehicles page for endorsements.
                    Return True
                Else
                    Return False
                End If
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP,
                 QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                'Added 09/13/2021 for BOP Endorsements Task 63912 MLW
                If IsOnAppPage Then
                    Return True
                Else
                    Return False
                End If
            Case Else
                Return True
        End Select
        Return False
    End Function


End Class