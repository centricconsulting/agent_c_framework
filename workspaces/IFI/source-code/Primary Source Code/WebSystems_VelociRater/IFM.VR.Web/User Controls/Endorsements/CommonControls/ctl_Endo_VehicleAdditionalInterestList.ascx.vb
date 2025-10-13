Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.PrimativeExtensions

Public Class ctl_Endo_VehicleAdditionalInterestList
    Inherits VRControlBase

    Public Event AIChange()
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

    Public ReadOnly Property AdditionalInterstIdsCreatedThisSession As List(Of Int32)
        Get
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
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    AiList = Me.Quote.AdditionalInterests
            End Select


            Return AiList
        End Get
        Set(value As List(Of QuickQuoteAdditionalInterest))
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    Me.Quote.AdditionalInterests = value
            End Select
        End Set
    End Property



    Public ReadOnly Property TransactionLimitReached As Boolean
        Get
            If Me.ParentVrControl IsNot Nothing Then
                Select Case Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                        If TypeOf Me.ParentVrControl Is ctl_BOP_ENDO_LocationList Then
                            Dim Parent = CType(ParentVrControl, ctl_BOP_ENDO_LocationList)
                            Return Parent.TransactionLimitReached
                        End If
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        If TypeOf Me.ParentVrControl Is ctl_CPP_ENDO_InlandMarine Then
                            Dim Parent = CType(ParentVrControl, ctl_CPP_ENDO_InlandMarine)
                            Return Parent.TransactionLimitReached
                        End If
                End Select
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
                Me.VRScript.AddVariableLine("var AIBillToCheckBoxClientIdArray = new Array();")
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
            Dim aiControl As ctl_Endo_VehicleAdditionalInterest = CType(cntrl.FindControl("ctl_Endo_VehicleAdditionalInterest"), ctl_Endo_VehicleAdditionalInterest)
            AddHandler aiControl.SaveRequested, AddressOf aiControlRequestedSave
            AddHandler aiControl.RemoveAi, AddressOf aiRemovalRequested
            AddHandler aiControl.AIListChanged, AddressOf AIListWasChanged
            AddHandler aiControl.UpdateTransactionReasonType, AddressOf UpdateTransactionReasonTypeFloat
        Next
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            divEndorsementButtons.Visible = False 'Added 04/29/2021 for CAP Endorsements Task 52974 MLW
            Me.Repeater1.DataSource = MyAiList
            Me.Repeater1.DataBind()

            lnkBtnAdd.Visible = True
            lnkBtnSave.Visible = True

            If MyAiList IsNot Nothing AndAlso MyAiList.Any() Then
                If (IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage) = False Then
                    If Not IsPostBack Then
                        Me.OpenAccordionAtIndex("0") ' open when there are AIs on first load
                    End If
                End If

                Select Case Me.Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        Me.lblHeader.Text = String.Format("Additional Interests ({0}) (Property/IM)", MyAiList.Count)
                    Case Else
                        Me.lblHeader.Text = String.Format("Additional Interests ({0})", MyAiList.Count)
                End Select

                Me.divAdditionalInterestItems.Visible = True
            Else
                Select Case Me.Quote.LobType
                    Case Else
                        Me.lblHeader.Text = "Additional Interests (0)"
                End Select
                Me.divAdditionalInterestItems.Visible = False
            End If

            divEndorsementButtons.Visible = True

            lnkBtnAdd.Visible = True
            btnAddAdditionalInterest.Enabled = True
            lnkBtnSave.Visible = True

        End If
        PopulateChildControls()

    End Sub

    Private Sub repeater1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
        If e IsNot Nothing Then
            Dim AiControl As ctl_Endo_VehicleAdditionalInterest = e.Item.FindControl("ctl_Endo_VehicleAdditionalInterest")
            AiControl.VehicleIndex = Me.VehicleIndex
            AiControl.AdditionalInterestIndex = e.Item.ItemIndex
            AiControl.TransactionLimitReached = TransactionLimitReached
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

            Dim interest As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest

            If BopDictItems.UpdateAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, interest) Then
            End If

            'Save event needs to happen before we attempt to add the additional interest. If it isn't, when someone attempts to add two AI's, without manually specifying a save inbetween, the MyAiList will not
            'be updated with the most recent attempt to add. This forces the use to have to click the "Add Additional Interest" button twice to actuall add one.
            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

            If MyAiList Is Nothing Then
                MyAiList = New List(Of QuickQuoteAdditionalInterest)()
            End If


            MyAiList.Add(interest)

            LockTree()

            OpenAccordionAtIndex((MyAiList.Count() - 1).ToString())

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

            If MyAiList(index) IsNot Nothing Then
                If BopDictItems.UpdateAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, MyAiList(index)) Then
                End If
            End If

            If MyAiList IsNot Nothing Then
                If MyAiList.Count > index Then

                    If String.IsNullOrWhiteSpace(MyAiList(index).ListId) = False Then
                        'Removes Contractor's Equipment and AAI Too.
                        QQHelper.RemoveSpecificQuickQuoteAdditionalInterestFromQuoteBasedOnLob(Me.Quote, MyAiList(index).ListId, removeFromTopLevel:=True)
                    Else
                        MyAiList.RemoveAt(index)
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