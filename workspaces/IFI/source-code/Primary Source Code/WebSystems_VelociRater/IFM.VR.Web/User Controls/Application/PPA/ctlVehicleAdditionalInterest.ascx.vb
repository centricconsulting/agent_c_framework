Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions


Public Class ctlVehicleAdditionalInterest
    Inherits VRControlBase

    Public ReadOnly CAPEndorsementsDictionaryName = "CAPEndorsementsDetails" 'Added 05/03/2021 for CAP Endorsements Task 52974 MLW

    'Added 05/03/2021 for CAP Endorsements Task 52974 MLW
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

    Public ReadOnly Property ReturnToQuoteEvent() As String
        Get
            Return "ReturnToQuoteEvent" + "_" + Quote.PolicyId + "|" + Quote.PolicyImageNum
        End Get
    End Property

    Public Property ReturnToQuoteSession() As Boolean
        Get
            Dim ReturnToQuoteEventBool As Boolean
            If Session IsNot Nothing AndAlso Session(ReturnToQuoteEvent) IsNot Nothing Then
                If Boolean.TryParse(Session(ReturnToQuoteEvent).ToString, ReturnToQuoteEventBool) = False Then
                    ReturnToQuoteEventBool = False
                End If
            End If
            Return ReturnToQuoteEventBool
        End Get
        Set(ByVal value As Boolean)
            If Session IsNot Nothing AndAlso Session(ReturnToQuoteEvent) IsNot Nothing Then
                Session(ReturnToQuoteEvent) = value
            Else
                Session.Add(ReturnToQuoteEvent, value)
            End If
        End Set
    End Property

    Public Event AIListChanged()

    Public Event RemoveAi(index As Int32)

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

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return Me.AdditionalInterestIndex
        End Get
    End Property

    'Public Property VehicleItemsAccordID As String
    '    Get
    '        If ViewState("vs_VehicleItemsAccordID") IsNot Nothing Then
    '            Return ViewState("vs_VehicleItemsAccordID").ToString()
    '        End If
    '        Return ""
    '    End Get
    '    Set(value As String)
    '        ViewState("vs_VehicleItemsAccordID") = value
    '    End Set
    'End Property

    Public Property MyAiList As List(Of QuickQuoteAdditionalInterest)
        Get
            Dim AiList As List(Of QuickQuoteAdditionalInterest) = Nothing
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
                    AiList = Me.Quote.Locations(0).AdditionalInterests
                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    AiList = Me.Quote.Locations(0).AdditionalInterests
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    AiList = Quote.AdditionalInterests
                    ''Added 3/25/2021 for CAP Endorsements task 52974 MLW
                    'If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                    '    Dim aiListIds As List(Of Integer) = Nothing
                    '    If Me.Quote.AdditionalInterests IsNot Nothing AndAlso Me.Quote.AdditionalInterests.Count > 0 Then
                    '        For Each ai As QuickQuoteAdditionalInterest In Me.Quote.AdditionalInterests
                    '            If ai IsNot Nothing Then
                    '                QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(QQHelper.IntegerForString(ai.ListId), aiListIds, positiveOnly:=True)
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
                    '                            'QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(QQHelper.IntegerForString(vai.ListId), aiListIds, positiveOnly:=True)
                    '                            Dim copiedAI As QuickQuoteAdditionalInterest = QQHelper.CloneObject(vai)
                    '                            If copiedAI IsNot Nothing Then
                    '                                If AiList Is Nothing Then
                    '                                    AiList = New List(Of QuickQuoteAdditionalInterest)
                    '                                End If
                    '                                copiedAI.Num = "" 'clearing out in case it came over from source AI
                    '                                AiList.Add(copiedAI)
                    '                                QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(QQHelper.IntegerForString(vai.ListId), aiListIds, positiveOnly:=True)
                    '                            End If
                    '                        End If
                    '                    End If
                    '                Next
                    '            End If
                    '        Next
                    '    End If
                    'End If
            End Select
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
                    Me.Quote.Locations(0).AdditionalInterests = value
                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    Me.Quote.Locations(0).AdditionalInterests = value
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    Quote.AdditionalInterests = value
            End Select
        End Set
    End Property

    Public ReadOnly Property MyAdditionalInterest As QuickQuoteAdditionalInterest
        Get
            'Updated 04/29/2021 for CAP Endorsements Task 52974 MLW
            'If MyAiList IsNot Nothing Then
            If MyAiList IsNot Nothing AndAlso MyAiList.Count > 0 AndAlso Me.AdditionalInterestIndex < MyAiList.Count Then
                Return MyAiList(Me.AdditionalInterestIndex)
            End If
            Return Nothing
        End Get
    End Property


    Public ReadOnly Property MyVehicle As QuickQuote.CommonObjects.QuickQuoteVehicle
        Get
            If Me.Quote IsNot Nothing Then
                If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > Me.VehicleIndex Then
                    Return Me.Quote.Vehicles(Me.VehicleIndex)
                End If
            End If

            Return Nothing
        End Get
    End Property

    Public ReadOnly Property EnforceAppliesDescription() As Boolean
        Get
            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                Dim VrTypeIdText As String = QQHelper.GetStaticDataTextForValue_ForceLob(True, QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuotePropertyName.TypeId, MyAdditionalInterest.TypeId, lob:=QuickQuoteObject.QuickQuoteLobType.Farm)
                If String.IsNullOrWhiteSpace(VrTypeIdText) = False AndAlso (IsEndorsementRelated() = False OrElse IsNewAdditionalInterestOnEndorsement(MyAdditionalInterest)) Then
                    Return True
                End If
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
        'Updated 05/24/2021 for CAP Endorsements Task 52974 MLW
        If IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
            Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "This Additional Interest is assigned to one or more vehicles on the policy. If the Additional Interest is removed it will be removed from all assigned vehicles. Are you sure you want to remove?<br>To remove the Additional Interest from a specific vehicle, please do so from the vehicle below.")
        Else
            Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Remove?")
        End If
        'Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Remove?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)

        Dim clearAiIdScript As String = "if ($(""#" + Me.txtIsEditable.ClientID + """).val().toString().toLowerCase() == 'false') {$(""#" + Me.txtAiId.ClientID + """).val('');}"

        'Updated 01/26/2021 for CAP Endorsements Task 52974 MLW
        Dim setBindings As String = "AdditionalInterest.SetAdditionalInterestLookupBindings('" + Me.ClientID + "','" + Me.txtCommName.ClientID + "','" + Me.txtFirstName.ClientID + "','" + Me.txtMiddleName.ClientID + "','" + Me.txtLastName.ClientID + "','" + Me.txtPhoneNumber.ClientID + "','" + Me.txtPhoneExtension.ClientID + "','" + Me.txtPOBOX.ClientID + "','" + Me.txtAptSuiteNum.ClientID + "','" + Me.txtStreetNum.ClientID + "','" + Me.txtStreet.ClientID + "','" + Me.txtCity.ClientID + "','" + Me.ddStateAbbrev.ClientID + "','" + Me.txtZipCode.ClientID + "','" + Me.txtAiId.ClientID + "','" + Me.txtIsEditable.ClientID + "');"
        Me.VRScript.AddScriptLine(setBindings)

        ' Don't bind the lookup if we're on the quote side and the LOB is PPA.  Bug 32112 MGB 4-19-19
        If IsOnAppPage OrElse IsQuoteEndorsement() Then
            Dim interestLookup As String = "AdditionalInterest.DoAdditionalInterestLookup('" + Me.ClientID + "'.toString(),'" + Me.txtCommName.ClientID + "','" + Me.txtCommName.ClientID + "','" + Me.txtFirstName.ClientID + "','" + Me.txtMiddleName.ClientID + "','" + Me.txtLastName.ClientID + "'); "
            Me.VRScript.CreateJSBinding(Me.txtCommName, ctlPageStartupScript.JsEventType.onkeyup, interestLookup + clearAiIdScript)
            Me.VRScript.CreateJSBinding(Me.txtFirstName, ctlPageStartupScript.JsEventType.onkeyup, interestLookup + clearAiIdScript)
            Me.VRScript.CreateJSBinding(Me.txtMiddleName, ctlPageStartupScript.JsEventType.onkeyup, interestLookup + clearAiIdScript)
            Me.VRScript.CreateJSBinding(Me.txtLastName, ctlPageStartupScript.JsEventType.onkeyup, interestLookup + clearAiIdScript)
        End If

        Me.VRScript.CreateJSBinding(Me.txtAptSuiteNum, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.txtCity, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.txtPhoneExtension, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateTextBoxFormatter(Me.txtPhoneExtension, ctlPageStartupScript.FormatterType.PositiveNumberNoCommas, ctlPageStartupScript.JsEventType.onkeyup)
        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            Me.VRScript.CreateJSBinding(chkAppliesTo, ctlPageStartupScript.JsEventType.onclick, "if ($('#" + Me.chkAppliesTo.ClientID + "').is(':checked') ) { $('#" + Me.trDescription.ClientID + "').show();} else {$('#" + Me.trDescription.ClientID + "').hide(); $('#" + Me.txtDescription.ClientID + "').val(''); }", True)
        End If
        Me.VRScript.CreateJSBinding(Me.txtPhoneNumber, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.txtPOBOX, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.txtStreet, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.txtStreetNum, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript + "DoCityCountyLookup('" + Me.txtZipCode.ClientID + "','" + Me.ddCityName.ClientID + "','" + Me.txtCity.ClientID + "','','" + Me.ddStateAbbrev.ClientID + "');")
        Me.VRScript.CreateTextBoxFormatter(Me.txtZipCode, ctlPageStartupScript.FormatterType.ZipCode, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateJSBinding(Me.ddStateAbbrev, ctlPageStartupScript.JsEventType.onchange, clearAiIdScript)

        Me.VRScript.CreateTextboxMask(Me.txtPhoneNumber, "(000)000-0000")
        Me.VRScript.CreateTextboxWaterMark(Me.txtPhoneNumber, "(000)000-0000")

        Me.VRScript.AddScriptLine("$(""#" + Me.txtCity.ClientID + """).autocomplete({ source: INCities });")

        Dim addWarning_True As String = "DoAddressWarning(true,'" + Me.divAddressMessage.ClientID + "','" + Me.txtStreetNum.ClientID + "','" + txtStreet.ClientID + "','" + txtPOBOX.ClientID + "');"
        Dim addWarning_False As String = "DoAddressWarning(false,'" + Me.divAddressMessage.ClientID + "','" + Me.txtStreetNum.ClientID + "','" + txtStreet.ClientID + "','" + txtPOBOX.ClientID + "');"

        Me.VRScript.CreateJSBinding(Me.txtStreetNum, ctlPageStartupScript.JsEventType.onfocus, addWarning_True)
        Me.VRScript.CreateJSBinding(Me.txtStreetNum, ctlPageStartupScript.JsEventType.onblur, addWarning_False)



        Me.VRScript.CreateJSBinding(Me.txtStreet, ctlPageStartupScript.JsEventType.onfocus, addWarning_True)
        Me.VRScript.CreateJSBinding(Me.txtStreet, ctlPageStartupScript.JsEventType.onblur, addWarning_False)

        Me.VRScript.CreateJSBinding(Me.txtPOBOX, ctlPageStartupScript.JsEventType.onfocus, addWarning_True)
        Me.VRScript.CreateJSBinding(Me.txtPOBOX, ctlPageStartupScript.JsEventType.onblur, addWarning_False)

        '#If Not DEBUG Then
        Me.txtAiId.Attributes.Add("style", "display:none;")
        '        Me.txtIsEditable.Attributes.Add("style", "display:none;")
        '#End If

        ' need to use the is dirty script to clear the ai id if anyone edits a readonly AI
        ' so need to know if an AI is editable???

        Me.VRScript.AddVariableLine("AIBillToCheckBoxClientIdArray.push('" + Me.chkBillToMe.ClientID + "');")
        'Updated 02/12/2020 for bug 43977 MLW
        Me.VRScript.CreateJSBinding(Me.chkBillToMe, ctlPageStartupScript.JsEventType.onchange, "AiBillToToggled('" + Me.chkBillToMe.ClientID + "','" + Me.IsQuoteEndorsement.ToString() + "');")
        Me.VRScript.AddScriptLine("AiTypeChanged('" + Me.ddAiType.ClientID + "','" + Me.chkBillToMe.ClientID + "');") ' do at startup
        Me.VRScript.AddScriptLine("$(""#" + Me.ddCityName.ClientID + """).hide();") ' do at startup

    End Sub

    Public ReadOnly Property AdditionalInterstIdsCreatedThisSession As List(Of Int32)
        Get
            'If Session(Me.QuoteId + "_AI_Created_List") Is Nothing Then
            '    Session(Me.QuoteId + "_AI_Created_List") = New List(Of Int32)
            'End If

            'Return Session(Me.QuoteId + "_AI_Created_List")
            'updated 2/16/2021
            If Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List") Is Nothing Then
                Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List") = New List(Of Int32)
            End If

            Return Session(Me.QuoteIdOrPolicyIdPipeImageNumber + "_AI_Created_List")
        End Get

    End Property

    Public Overrides Sub LoadStaticData()
        If Me.ddStateAbbrev.Items.Count = 0 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddAiType, QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuotePropertyName.TypeId, SortBy.TextAscending, Me.Quote.LobType)
            For Each item As ListItem In Me.ddAiType.Items
                item.Attributes.Add("title", item.Text)
            Next
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddStateAbbrev, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.TextAscending, Me.Quote.LobType)

            ' on Farm quotes exclude Additionl Insured - Commercial (43) unless it is a commercial Farm Quote
            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                If Me.Quote.Policyholder.Name.TypeId <> "2" Then
                    'personal
                    Dim removeItem As ListItem = Nothing
                    For Each item As ListItem In Me.ddAiType.Items
                        If item.Value = "43" Then
                            removeItem = item
                        End If
                    Next
                    If removeItem IsNot Nothing Then
                        Me.ddAiType.Items.Remove(removeItem)
                    End If
                Else
                    'commercial
                    '56 ADDITIONAL INSURED - PARTNERS, CORPORATE OFFICERS OR CO-OWNERS
                    Dim removeAIPartners As ListItem = ddAiType.Items.FindByValue("56")
                    If removeAIPartners IsNot Nothing Then
                        ddAiType.Items.Remove(removeAIPartners)
                    End If

                    '57 ADDITIONAL INSURED - PERSONS OR ORGANIZATIONS
                    Dim removeAIPersons As ListItem = ddAiType.Items.FindByValue("57")
                    If removeAIPersons IsNot Nothing Then
                        ddAiType.Items.Remove(removeAIPersons)
                    End If
                End If
            End If

        End If
    End Sub

    Public Overrides Sub Populate()

        'Updated 12/29/2020 for CAP Endorsements Task 52974 MLW
        If AllowPopulate() Then
            LoadStaticData()



            Me.trAppliesTo.Visible = False
            Me.trDescription.Visible = False
            Me.trATIMA.Visible = False
            Me.trISAOA.Visible = False
            Me.trBillTo.Visible = False
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    ' If we're on the quote side disable Remove/Save  Bug 32112 MGB 4-17-19
                    If IsOnAppPage OrElse IsQuoteEndorsement() Then
                        lnkRemove.Visible = True
                        lnkSave.Visible = True
                    Else
                        lnkRemove.Visible = False
                        lnkSave.Visible = False
                    End If

                    ' Don't populate the control if the ai is fake
                    If IFM.VR.Common.Helpers.AdditionalInterest.IsFakeAI(MyAdditionalInterest) Then Exit Sub

                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    trSaveMsgRow.Visible = False
                    Me.trDescription.Visible = True
                    Me.trATIMA.Visible = True
                    Me.trISAOA.Visible = True
                    Me.trBillTo.Visible = True
                    Me.ddAiType.Attributes.Add("onchange", "AiTypeChanged($(this).attr('id'),'" + Me.chkBillToMe.ClientID + "');")
                    If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                        Me.txtZipCode.TabIndex = 0
                        Me.txtCity.TabIndex = 0
                        Me.ddStateAbbrev.TabIndex = 0
                    End If
                    If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                        ' Don't populate the control if the ai is fake
                        If IFM.VR.Common.Helpers.AdditionalInterest.IsFakeAI(MyAdditionalInterest) Then Exit Sub
                    End If

                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    trSaveMsgRow.Visible = False
                    Me.trAppliesTo.Visible = True
                    Me.trDescription.Visible = True
                    Me.txtDescription.Visible = True
                    Me.trATIMA.Visible = True
                    Me.trISAOA.Visible = True
                    Me.trBillTo.Visible = True
                    Me.ddAiType.Attributes.Add("onchange", "AiTypeChanged($(this).attr('id'),'" + Me.chkBillToMe.ClientID + "');")
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    trSaveMsgRow.Visible = True
                    Me.trAppliesTo.Visible = False
                    Me.trDescription.Visible = False
                    Me.txtDescription.Visible = False
                    Me.trBillTo.Visible = False
                    Me.trLoanNumber.Visible = False
                    Me.trPhoneNumber.Visible = False
                    Me.trPhoneExtension.Visible = False
                    Me.trAIType.Visible = False
                    'Updated 02/01/2021 for CAP Endorsements Task 52981 MLW
                    If IsQuoteReadOnly() Then
                        spnMsg.Visible = False
                    Else
                        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                            spnMsg.InnerHtml = "Additional Interests must be saved before they can be selected as Loss Payees in the Building section"
                        ElseIf Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                            spnMsg.InnerHtml = "Additional Interests must be saved before they can be selected as Loss Payees in the Building section or in the Inland Marine Coverages section."

                        Else
                            spnMsg.InnerHtml = "You must add and <u>save</u> the Additional Interest. Once saved, select the vehicle to <u>assign</u> the Loss Payee or Interest."
                        End If
                    End If
                    'Added 12/30/2020 for CAP Endorsements Task 52974 MLW
                    If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso IsQuoteEndorsement() Then
                        If TypeOfEndorsement() = "Add/Delete Vehicle" Then
                            If Not IsNewAdditionalInterestOnEndorsement(MyAdditionalInterest) Then
                                'Do not allow AI changes or deletions on existing AIs when in the add/delete vehicle type of endorsement. Only allow changes and deletions for new AIs. 
                                lnkRemove.Visible = False
                                DisableControls()
                            End If
                        ElseIf TypeOfEndorsement() = "Add/Delete Additional Interest" Then
                            If Not IsNewAdditionalInterestOnEndorsement(MyAdditionalInterest) Then
                                'Do not allow AI changes on existing AIs when in the add/delete additional interest type of endorsement.
                                DisableControls()
                            End If
                            Dim transactionCount As Integer = ddh.GetEndorsementAdditionalInterestTransactionCount()
                            If transactionCount >= 3 Then
                                'When we reach the max 3 AI transactions, we do not allow existing AIs to be removed since it would cause the transaction count to increase.
                                If Not IsNewAdditionalInterestOnEndorsement(MyAdditionalInterest) Then
                                    lnkRemove.Visible = False
                                End If
                                lnkSave.Visible = False
                            End If
                        End If
                    End If
                    Exit Select
            End Select

            If MyAdditionalInterest IsNot Nothing Then

                Select Case Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        Me.txtAiId.Text = MyAdditionalInterest.ListId 'added 6/1/2017 so new AIs aren't created on every Save
                        Exit Select
                    Case Else
                        Me.txtAiId.Text = MyAdditionalInterest.ListId
                        'Updated 03/05/2020 for bug 44465 MLW
                        'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAiType, MyAdditionalInterest.TypeId)

                        IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddAiType, MyAdditionalInterest.TypeId, QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuotePropertyName.TypeId)

                        'If QQHelper.IsPositiveIntegerString(MyAdditionalInterest.TypeId) AndAlso ddAiType.Items.FindByValue(MyAdditionalInterest.TypeId) Is Nothing Then
                        '    Dim AITypeDescription As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuotePropertyName.TypeId, MyAdditionalInterest.TypeId)
                        '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(Me.ddAiType, MyAdditionalInterest.TypeId, AITypeDescription)
                        'Else
                        '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAiType, MyAdditionalInterest.TypeId)
                        'End If
                        Me.txtLoanNumber.Text = MyAdditionalInterest.LoanNumber
                        Exit Select
                End Select

                If MyAdditionalInterest.Name IsNot Nothing Then
                    If MyAdditionalInterest.Name.TypeId <> "2" Then
                        Me.txtFirstName.Text = MyAdditionalInterest.Name.FirstName
                        Me.txtMiddleName.Text = MyAdditionalInterest.Name.MiddleName
                        Me.txtLastName.Text = MyAdditionalInterest.Name.LastName
                        Dim displayName As String = MyAdditionalInterest.Name.FirstName + " " + MyAdditionalInterest.Name.MiddleName + " " + MyAdditionalInterest.Name.LastName
                        displayName = displayName.Replace("  ", " ").Trim()
                        Me.lblExpanderText.Text = String.Format("Additional {2} #{0} - {1}", AdditionalInterestIndex + 1, displayName, If(Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm, "Insured", "Interest"))
                    Else
                        Me.txtCommName.Text = MyAdditionalInterest.Name.CommercialName1
                        Me.lblExpanderText.Text = String.Format("Additional {2} #{0} - {1}", AdditionalInterestIndex + 1, MyAdditionalInterest.Name.CommercialName1, If(Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm, "Insured", "Interest"))
                    End If
                End If
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm And IsQuoteEndorsement() Or IsQuoteReadOnly() Then
                    If Me.lblExpanderText.Text.Length > 38 Then
                        Me.lblExpanderText.Text = Me.lblExpanderText.Text.Substring(0, 38) + "..."
                    End If
                Else
                    If Me.lblExpanderText.Text.Length > 45 Then
                        Me.lblExpanderText.Text = Me.lblExpanderText.Text.Substring(0, 45) + "..."
                    End If
                End If

                If IsNumeric(MyAdditionalInterest.ListId) Then
#If DEBUG Then
                    If Me.AdditionalInterstIdsCreatedThisSession.Contains(MyAdditionalInterest.ListId) Then
                        Me.lblExpanderText.ToolTip = Me.lblExpanderText.Text + " Created this Session EDITABLE AI"
                    Else
                        Me.lblExpanderText.ToolTip = Me.lblExpanderText.Text + " READ-ONLY AI - Any changes will create a new AI record."
                    End If
#End If
                    ' lets js know if the Ai is updateable
                    If Me.AdditionalInterstIdsCreatedThisSession.Contains(MyAdditionalInterest.ListId) Then
                        Me.txtIsEditable.Text = "true"
                    Else
                        Me.txtIsEditable.Text = "false"
                    End If
                End If
                If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                    ' might have defaults from quote side so you need to check and remove if needed
                    If MyAdditionalInterest.Address.HouseNum.Trim() = "123" And MyAdditionalInterest.Address.StreetName.Trim().ToUpper() = "ANY" And MyAdditionalInterest.Address.City.Trim().ToUpper() = "ANY" Then
                        Me.txtStreetNum.Text = ""
                        Me.txtStreet.Text = ""
                        Me.txtAptSuiteNum.Text = ""
                        Me.txtPOBOX.Text = ""
                        Me.txtCity.Text = ""
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, MyAdditionalInterest.Address.StateId)
                        Me.txtZipCode.Text = ""
                        Me.txtAiId.Text = "" ' so that is saves and validates correctly
                    Else
                        Me.txtStreetNum.Text = MyAdditionalInterest.Address.HouseNum
                        Me.txtStreet.Text = MyAdditionalInterest.Address.StreetName
                        Me.txtAptSuiteNum.Text = MyAdditionalInterest.Address.ApartmentNumber
                        Me.txtPOBOX.Text = MyAdditionalInterest.Address.POBox
                        Me.txtCity.Text = MyAdditionalInterest.Address.City
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, MyAdditionalInterest.Address.StateId)
                        Me.txtZipCode.Text = MyAdditionalInterest.Address.Zip
                    End If


                Else
                    Me.txtStreetNum.Text = MyAdditionalInterest.Address.HouseNum
                    Me.txtStreet.Text = MyAdditionalInterest.Address.StreetName
                    Me.txtAptSuiteNum.Text = MyAdditionalInterest.Address.ApartmentNumber
                    'Me.txtOtherInfo.Text = MyAdditionalInterest.Address.Other
                    Me.txtPOBOX.Text = MyAdditionalInterest.Address.POBox
                    Me.txtCity.Text = MyAdditionalInterest.Address.City
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, MyAdditionalInterest.Address.StateId)
                    Me.txtZipCode.Text = MyAdditionalInterest.Address.Zip
                End If

                Select Case Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        Exit Select
                    Case Else
                        If MyAdditionalInterest.Phones IsNot Nothing AndAlso MyAdditionalInterest.Phones.Any() Then
                            Me.txtPhoneNumber.Text = MyAdditionalInterest.Phones(0).Number
                            Me.txtPhoneExtension.Text = MyAdditionalInterest.Phones(0).Extension
                        End If

                        Me.txtDescription.Text = MyAdditionalInterest.Description

                        ' 04/19/2023 CAH - WS-543
                        If EnforceAppliesDescription Then
                            Me.chkAppliesTo.Checked = True
                            Me.chkAppliesTo.Enabled = False
                        Else
                            Me.chkAppliesTo.Checked = Not String.IsNullOrWhiteSpace(Me.txtDescription.Text)
                            Me.chkAppliesTo.Enabled = True
                        End If

                        Me.chkATIMA.Checked = MyAdditionalInterest.ATIMA
                        Me.chkISAOA.Checked = MyAdditionalInterest.ISAOA
                        Me.chkBillToMe.Checked = MyAdditionalInterest.BillTo
                        Exit Select
                End Select

                ' If on the quote side disable all of the controls on the form
                If Not IsOnAppPage AndAlso Not IsQuoteEndorsement() Then DisableControls()

                'Me.chkVerified.Checked = MyAdditionalInterest.is
                'Me.chkVerifyAttemped.Checked = MyAdditionalInterest.att

            End If
        End If
    End Sub

    ''' <summary>
    ''' Disables all the controls on the page
    ''' </summary>
    Private Sub DisableControls()
        ddAiType.Enabled = False
        txtCommName.ReadOnly = True
        txtFirstName.ReadOnly = True
        txtMiddleName.ReadOnly = True
        txtLastName.ReadOnly = True
        txtLoanNumber.ReadOnly = True
        txtPhoneNumber.ReadOnly = True
        txtPhoneExtension.ReadOnly = True
        chkAppliesTo.Enabled = False
        txtDescription.ReadOnly = True
        txtStreetNum.ReadOnly = True
        txtStreet.ReadOnly = True
        txtAptSuiteNum.ReadOnly = True
        txtPOBOX.ReadOnly = True
        txtZipCode.ReadOnly = True
        ddCityName.Enabled = False
        txtCity.ReadOnly = True
        ddStateAbbrev.Enabled = False
        chkATIMA.Enabled = False
        chkISAOA.Enabled = False
        chkBillToMe.Enabled = False
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'MyBase.ValidateControl(valArgs) does it below in the DoValidationCheck
        'Updated 12/29/2020 for CAP Endorsements Task 52974 MLW
        If AllowValidateAndSave() Then
            'If IsOnAppPage OrElse IsQuoteEndorsement() Then  ' DO NOT VALIDATE ON QUOTE SIDE!  Bug 32112 MGB 4-17-19
            If Me.Visible Then
                MyBase.ValidateControl(valArgs)
                Select Case Me.Quote.LobType
                    'Updated 06/02/2021 for CAP Endorsements Task 52974 MLW
                    'Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal, QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    '    Me.ValidationHelper.GroupName = String.Format("Vehicle #{0} Additional Interest #{1}", Me.VehicleIndex + 1, Me.AdditionalInterestIndex + 1)
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                        If IsQuoteEndorsement() Then
                            Me.ValidationHelper.GroupName = String.Format("Additional Interest #{0}", Me.AdditionalInterestIndex + 1)
                        Else
                            Me.ValidationHelper.GroupName = String.Format("Vehicle #{0} Additional Interest #{1}", Me.VehicleIndex + 1, Me.AdditionalInterestIndex + 1)
                        End If
                    Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                        Me.ValidationHelper.GroupName = String.Format("Vehicle #{0} Additional Interest #{1}", Me.VehicleIndex + 1, Me.AdditionalInterestIndex + 1)
                    Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal, QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        Me.ValidationHelper.GroupName = String.Format("Additional Interest #{0}", Me.AdditionalInterestIndex + 1)
                    Case QuickQuoteObject.QuickQuoteLobType.Farm
                        Me.ValidationHelper.GroupName = String.Format("Additional Insured #{0}", Me.AdditionalInterestIndex + 1)
                End Select

                Dim paneIndex = Me.AdditionalInterestIndex
                Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

                Dim aiVals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.AdditionalInterestValidation(MyAdditionalInterest, valArgs.ValidationType)
                For Each v In aiVals
                    Select Case v.FieldId

                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.AIType
                            Select Case Quote.LobType
                                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                                    Exit Select
                                Case Else
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddAiType, v, accordList)
                                    Exit Select
                            End Select

                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.CommAndPersNameComponentsEmpty
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCommName, v, accordList)
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstName, v, accordList)
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLastName, v, accordList)

                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.CommAndPersNameComponentsAllSet
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCommName, v, accordList)
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstName, v, accordList)
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLastName, v, accordList)

                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.CommercialName
                            Try
                                v.Message = v.Message.Split(" ")(0) + " Business Name"
                            Catch ex As Exception
#If DEBUG Then
                                Debugger.Break()
#End If
                            End Try


                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCommName, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.FirstNameID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstName, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.LastNameID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLastName, v, accordList)

                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.PhoneNumberInvalid
                            Select Case Quote.LobType
                                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                                    Exit Select
                                Case Else
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPhoneNumber, v, accordList)
                                    Exit Select
                            End Select
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.PhoneExtensionInvalid
                            Select Case Quote.LobType
                                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                                    Exit Select
                                Case Else
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPhoneExtension, v, accordList)
                                    Exit Select
                            End Select

                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.StreetAndPoBoxEmpty
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreet, v, accordList)
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBOX, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.StreetAndPoxBoxAreSet
                            If IsQuoteEndorsement() = False OrElse (IsQuoteEndorsement() AndAlso IsNewAdditionalInterestOnEndorsement(MyAdditionalInterest)) Then
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreet, v, accordList)
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBOX, v, accordList)
                            End If
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.HouseNumberID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.StreetNameID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreet, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.POBOXID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBOX, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.ZipCodeID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtZipCode, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.CityID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCity, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.StateID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, accordList)


                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.Description
                            If EnforceAppliesDescription Then
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescription, v, accordList)
                            End If

                    End Select
                Next

            End If
        End If
    End Sub

    ''' <summary>
    ''' Adds Loan/Lease to the current vehicle if it meets the coverage requirements
    ''' </summary>
    Private Sub AddLoanLeaseToVehicleIfRequired()
        ' Don't add on:
        ' Endorsements
        ' Any LOB except PPA
        ' Vehicles older than 5 years
        If Quote IsNot Nothing _
            AndAlso (Not IsQuoteEndorsement()) _
            AndAlso IsOnAppPage _
            AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal _
            AndAlso Quote.Vehicles IsNot Nothing _
            AndAlso Quote.Vehicles.Count >= VehicleIndex - 1 Then
            Dim MyVehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Quote.Vehicles(VehicleIndex)
            If IsNumeric(MyVehicle.Year) AndAlso CInt(MyVehicle.Year) >= DateTime.Now.AddYears(-5).Year Then
                MyVehicle.HasAutoLoanOrLease = True
            End If
        End If


        Exit Sub
    End Sub

    Public Overrides Function Save() As Boolean
        Dim StartedWithNoAIs As Boolean = False
        If Me.Visible Then
            If Quote IsNot Nothing Then
                'Updated 12/29/2020 for CAP Endorsements Task 52974 MLW
                If AllowValidateAndSave() Then
                    'Moved to AllowValidateAndSave() 02/01/2021 MLW
                    '' If we're PPA and on the quote side do not ever save - control is read only MGB 4-22-19 Bug 32112
                    'If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso (Not IsOnAppPage AndAlso Not IsQuoteEndorsement()) Then Return True

                    If MyAiList Is Nothing OrElse MyAiList.Count <= 0 Then
                        StartedWithNoAIs = True
                    End If

                    If MyAiList Is Nothing Then
                        MyAiList = New List(Of QuickQuoteAdditionalInterest)()
                    End If
                    While (MyAiList.Count < AdditionalInterestIndex + 1)
                        MyAiList.Add(New QuickQuoteAdditionalInterest())
                    End While

                    If MyAdditionalInterest IsNot Nothing AndAlso flaggedForDelete = False Then
                        MyAdditionalInterest.AgencyId = DirectCast(Me.Page.Master, VelociRater).AgencyID.ToString()
                        MyAdditionalInterest.SingleEntry = False

                        If ReturnToQuoteSession = False Then ' Don't pull from form on a Return To Quote Event (this has been previously saved) CAH
                            If (String.IsNullOrWhiteSpace(Me.txtAiId.Text) Or IsNumeric(Me.txtAiId.Text) = False) Then
                                Dim originalAiListId As String = MyAdditionalInterest.ListId 'added 5/26/2021 so we can keep track if it changes
                                MyAdditionalInterest.ListId = "" ' basically lets this AI become something else
                                ' IS NEW AI
                                InjectFormIntoAI(MyAdditionalInterest) ' persist always
                                CheckForFarmCoverage(MyAdditionalInterest)

                                If Me.ValidationHelper.HasErrros() = False Then
                                    ' create then set new AI Id to this object
                                    ' ideally we wouldn't add this until rate but it is possible that this new ai is used twice on this one app and
                                    ' had we not created it then it would create two basically identical AIs at rate
                                    Dim qqXml As New QuickQuoteXML()
                                    Dim diamondList As Diamond.Common.Objects.Policy.AdditionalInterestList = Nothing
                                    qqXml.DiamondService_CreateAdditionalInterestList(MyAdditionalInterest, diamondList)
                                    If diamondList IsNot Nothing Then
                                        If IsNumeric(MyAdditionalInterest.ListId) Then
                                            Me.txtAiId.Text = MyAdditionalInterest.ListId ' just in case you don't repopulate the form it will know it was already created
                                            AdditionalInterstIdsCreatedThisSession.Add(CInt(MyAdditionalInterest.ListId))
                                            Me.txtIsEditable.Text = "true" 'added 2/3/2021

                                            'Added 05/03/2021 for CAP Endorsements Task 52974 MLW
                                            'Adds the AI Add entry to the DevDictionary when the AI is newly entered in (does not exist in lookup list, not selected from lookup list, is manually entered)
                                            If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Additional Interest" AndAlso IsNewAdditionalInterestOnEndorsement(MyAdditionalInterest) Then
                                                Dim aiVehicleList As List(Of Integer) = New List(Of Integer)
                                                ddh.UpdateDevDictionaryAdditionalInterestList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, MyAdditionalInterest, aiVehicleList)
                                                'For AI Adds, the TransactionReasonId needs to be 10169 to show the full dec in the print history.
                                                Quote.TransactionReasonId = 10169 'Endorsement Change Dec and Full Revised Dec
                                                Dim endorsementsRemarksHelper = New EndorsementsRemarksHelper(ddh)
                                                Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(endorsementsRemarksHelper.RemarksType.AdditionalInterest)
                                                Quote.TransactionRemark = updatedRemarks
                                            End If

                                            RaiseEvent AIListChanged()  ' MGB 5-9-17 for BOP
                                            If StartedWithNoAIs Then AddLoanLeaseToVehicleIfRequired()
                                        End If
                                    End If
                                End If
                            Else
                                ' already existed
                                Dim qqXml As New QuickQuoteXML()
                                Dim loadedAi As QuickQuoteAdditionalInterest = Nothing

                                'added 9/13/2017
                                Dim needsToRaiseChangedEvent As Boolean = False
                                If MyAdditionalInterest IsNot Nothing Then
                                    If MyAdditionalInterest.HasValidAdditionalInterestListId = False Then
                                        needsToRaiseChangedEvent = True
                                    End If
                                End If

                                ' ************* you don't want to pass in the 'interest' object above  ***********************
                                'qqXml.LoadDiamondAdditionalInterestListIntoQuickQuoteAdditionalInterest(Me.txtAiId.Text.Trim(), loadedAi)
                                ''If loadedAi IsNot Nothing AndAlso MyAdditionalInterest IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(MyAdditionalInterest.Num) = True Then
                                ''    loadedAi.Num = CInt(MyAdditionalInterest.Num).ToString
                                ''End If
                                'If loadedAi IsNot Nothing AndAlso MyAdditionalInterest IsNot Nothing Then
                                '    If QQHelper.IsPositiveIntegerString(MyAdditionalInterest.Num) = True Then
                                '        loadedAi.Num = CInt(MyAdditionalInterest.Num).ToString
                                '    ElseIf QQHelper.IsPositiveIntegerString(loadedAi.Num) = False AndAlso String.IsNullOrWhiteSpace(MyAdditionalInterest.Num) = False Then
                                '        loadedAi.Num = MyAdditionalInterest.Num
                                '    End If
                                'End If
                                'updated 4/3/2020 to use new method that loads all base information in from MyAdditionalInterest to avoid dropping any information that may not be on VR form
                                qqXml.LoadDiamondAdditionalInterestListIntoQuickQuoteAdditionalInterest_OptionallyUseExistingAdditionalInterestForBaseInfo(Me.txtAiId.Text.Trim(), loadedAi, existingQQAdditionalInterest:=MyAdditionalInterest)
                                If Me.AdditionalInterstIdsCreatedThisSession.Contains(CInt(Me.txtAiId.Text.Trim())) Then
                                    ' was created this session it is probably safe to edit

                                    ' ************* you don't want to pass in the 'interest' object above  ***********************
                                    'qqXml.LoadDiamondAdditionalInterestListIntoQuickQuoteAdditionalInterest(Me.txtAiId.Text.Trim(), loadedAi)
                                    If loadedAi IsNot Nothing Then
                                        InjectFormIntoAI(loadedAi)
                                        loadedAi.OverwriteAdditionalInterestListInfoForDiamondId = True ' without this it will not commit the edits

                                        'added 9/13/2017
                                        If needsToRaiseChangedEvent = False AndAlso MyAdditionalInterest IsNot Nothing Then
                                            If QuickQuoteHelperClass.IsQuickQuoteObjectMatch_Name(MyAdditionalInterest.Name, loadedAi.Name) = False Then
                                                needsToRaiseChangedEvent = True
                                            End If
                                        End If

                                        ' save that loaded AI with any changes - well the changes will be saved soon
                                        MyAiList(AdditionalInterestIndex) = loadedAi
                                        CheckForFarmCoverage(loadedAi)
                                    End If
                                Else
                                    'no changes to name or address allowed - but you need the loan number and it looks like that is all they changed or the ID would have been blank
                                    If loadedAi IsNot Nothing Then
                                        Select Case Quote.LobType
                                            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                                                'MyAdditionalInterest.TypeId = "17"
                                                'updated 6/9/2017 to set variable on correct object... MyAdditionalInterest could have already had a value, but since loadedAi didn't have anything set for TypeId and TypeId wasn't being set on correct object, MyAdditionalInterest was being overwritten w/ loadedAi below and wiping out TypeId... which makes it drop out of the list in the backend library
                                                'loadedAi.TypeId = "17"
                                                '6/9/2017 - updated again to try to maintain current value before defaulting
                                                If QQHelper.IsPositiveIntegerString(loadedAi.TypeId) = False Then
                                                    If MyAdditionalInterest IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(MyAdditionalInterest.TypeId) = True Then
                                                        loadedAi.TypeId = MyAdditionalInterest.TypeId
                                                    Else
                                                        loadedAi.TypeId = "17"
                                                    End If
                                                End If
                                                Exit Select
                                            Case Else
                                                loadedAi.TypeId = Me.ddAiType.SelectedValue '"53"
                                                loadedAi.LoanNumber = Me.txtLoanNumber.Text
                                                loadedAi.ATIMA = Me.chkATIMA.Checked
                                                loadedAi.ISAOA = Me.chkISAOA.Checked
                                                loadedAi.BillTo = Me.chkBillToMe.Checked
                                                loadedAi.Description = Me.txtDescription.Text.Trim()
                                                Exit Select
                                        End Select

                                        'added 9/13/2017
                                        If needsToRaiseChangedEvent = False AndAlso MyAdditionalInterest IsNot Nothing Then
                                            If QuickQuoteHelperClass.IsQuickQuoteObjectMatch_Name(MyAdditionalInterest.Name, loadedAi.Name) = False Then
                                                needsToRaiseChangedEvent = True
                                            End If
                                        End If


                                        MyAiList(AdditionalInterestIndex) = loadedAi
                                        'loadedAi.OverwriteAdditionalInterestListInfoForDiamondId = True ' without this it will not commit the edits; 6/1/2017 note: Overwrite flag should be specific to list info (name/address/phones/emails), so it isn't necessary to set when updating other fields as normal Save will update them either way... should be okay to leave in here since LoadDiamondAdditionalInterestListIntoQuickQuoteAdditionalInterest has re-populated the info anyway; removed 2/1/2021 to prevent extra calls to DiamondService_CreateOrUpdateAdditionalInterestList from qqXml, which could cause duplicate address/name link records; note: OverwriteAdditionalInterestListInfoForDiamondId should only be set to True if the AiList record was created during the current session
                                        CheckForFarmCoverage(loadedAi)
                                    Else
                                        ' should not get here because the AI loaded above should not be returning nothing
                                        Select Case Quote.LobType
                                            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                                                'MyAdditionalInterest.TypeId = "17"
                                                '6/9/2017 - updated to try to maintain current value before defaulting
                                                If QQHelper.IsPositiveIntegerString(MyAdditionalInterest.TypeId) = False Then
                                                    MyAdditionalInterest.TypeId = "17"
                                                End If
                                                Exit Select
                                            Case Else
                                                MyAdditionalInterest.LoanNumber = Me.txtLoanNumber.Text
                                                MyAdditionalInterest.TypeId = Me.ddAiType.SelectedValue '"53"
                                                Exit Select
                                        End Select
                                        MyAdditionalInterest.ListId = Me.txtAiId.Text
                                        CheckForFarmCoverage(MyAdditionalInterest)
                                    End If
                                End If

                                'Added 05/03/2021 for CAP Endorsements Task 52974 MLW
                                If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Additional Interest" AndAlso IsNewAdditionalInterestOnEndorsement(MyAdditionalInterest) Then
                                    'Updates the AI Add entry in the DevDictionary when the AI exists in the lookup list and is selected from the lookup list.
                                    'Need to preserve the vehicle number list for the AI. Otherwise it would get wiped out when saved.
                                    Dim aiVehicleList As List(Of Integer) = New List(Of Integer)
                                    'get current list of vehicles if AI add record exists
                                    Dim aiExistsInAddList As Boolean = False
                                    Dim strAddedVehicleNumsForAIList As String = ""
                                    If ddh.GetAdditionalInterestDictionary IsNot Nothing AndAlso ddh.GetAdditionalInterestDictionary.Count > 0 Then
                                        For Each additionalInterest As DevDictionaryHelper.AdditionalInterestInfo In ddh.GetAdditionalInterestDictionary
                                            If additionalInterest.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem AndAlso additionalInterest.diaAINumber = MyAdditionalInterest.ListId Then
                                                aiExistsInAddList = True
                                                strAddedVehicleNumsForAIList = additionalInterest.VehicleNumList
                                                Exit For
                                            End If
                                        Next
                                    End If
                                    If aiExistsInAddList = True AndAlso Not IsNullEmptyorWhitespace(strAddedVehicleNumsForAIList) Then
                                        aiVehicleList = strAddedVehicleNumsForAIList.Split(","c).[Select](Function(n) Integer.Parse(n)).ToList()
                                    End If
                                    ddh.UpdateDevDictionaryAdditionalInterestList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, MyAdditionalInterest, aiVehicleList)
                                    'For AI Adds, the TransactionReasonId needs to be 10169 to show the full dec in the print history.
                                    Quote.TransactionReasonId = 10169 'Endorsement Change Dec and Full Revised Dec
                                    Dim endorsementsRemarksHelper = New EndorsementsRemarksHelper(ddh)
                                    Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(endorsementsRemarksHelper.RemarksType.AdditionalInterest)
                                    Quote.TransactionRemark = updatedRemarks
                                End If

                                'added 9/13/2017
                                If needsToRaiseChangedEvent = True Then
                                    RaiseEvent AIListChanged()
                                    If StartedWithNoAIs Then AddLoanLeaseToVehicleIfRequired()
                                End If

                            End If

                        End If
                        Return True
                    End If
                End If
            End If
        End If

        Return False
    End Function

    'Added 01/26/2021 for CAP Endorsements Task 52974 MLW
    Private Function AllowPopulate() As Boolean
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                If IsOnAppPage Then
                    Return True
                ElseIf IsQuoteReadOnly() Then
                    Return True
                ElseIf (IsQuoteEndorsement() AndAlso (TypeOfEndorsement() = "Add/Delete Additional Interest" OrElse TypeOfEndorsement() = "Add/Delete Vehicle")) Then
                    'lock down AIs - do not populate when on the Add/Delete Drivers or Amend Mailing Address type of endorsement. Only populate for Add/Delete AIs and Add/Delete Vehicles.
                    Return True
                Else
                    Return False
                End If
            Case Else
                Return True
        End Select
    End Function

    'Added 01/26/2021 for CAP Endorsements Task 52974 MLW
    Private Function AllowValidateAndSave() As Boolean
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                If IsOnAppPage OrElse IsQuoteEndorsement() Then
                    Return True
                Else
                    Return False
                End If
            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                'Do not validate and save for endorsements unless they are in the Add/Delete Vehicle type of endorsement and the AI is new or in the Add/Delete Vehicle type of endorsement and the AI is new.
                'Do not validate and save existing AIs on endorsements.
                If IsOnAppPage Then
                    Return True
                ElseIf IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Vehicle" AndAlso (MyAdditionalInterest Is Nothing OrElse IsNewAdditionalInterestOnEndorsement(MyAdditionalInterest)) Then
                    Return True
                ElseIf IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Additional Interest" AndAlso (MyAdditionalInterest Is Nothing OrElse IsNewAdditionalInterestOnEndorsement(MyAdditionalInterest)) Then
                    Return True
                Else
                    Return False
                End If
            Case Else
                Return True
        End Select
    End Function
    Private Sub CheckForFarmCoverage(ai As QuickQuoteAdditionalInterest)
        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                If Me.Quote.Locations(0).AdditionalInterests.FindAll(Function(p) p.TypeId = "56").Count > 0 Then
                    If Me.Quote.Locations(0).SectionIICoverages Is Nothing Then
                        Me.Quote.Locations(0).SectionIICoverages = New List(Of QuickQuoteSectionIICoverage)()
                    End If

                    ' Add as Optional Liability
                    If Me.Quote.Locations(0).SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Occupying_Residence_on_Premises).Count = 0 Then
                        Dim sectionIICoverage As New QuickQuoteSectionIICoverage()
                        sectionIICoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Occupying_Residence_on_Premises
                        Me.Quote.Locations(0).SectionIICoverages.Add(sectionIICoverage)
                    End If
                Else
                    ' Make sure that Optional Liability coverage is removed
                    If Me.Quote.Locations(0).SectionIICoverages IsNot Nothing Then
                        Dim optLiabCov As QuickQuoteSectionIICoverage = Me.Quote.Locations(0).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Occupying_Residence_on_Premises)
                        Me.Quote.Locations(0).SectionIICoverages.Remove(optLiabCov)
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub InjectFormIntoAI(interest As QuickQuoteAdditionalInterest)
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                'interest.TypeId = "17"
                '6/9/2017 - updated again to try to maintain current value before defaulting
                If QQHelper.IsPositiveIntegerString(interest.TypeId) = False Then
                    If MyAdditionalInterest IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(MyAdditionalInterest.TypeId) = True Then
                        interest.TypeId = MyAdditionalInterest.TypeId
                    Else
                        interest.TypeId = "17"
                    End If
                End If
                Exit Select
            Case Else
                interest.TypeId = Me.ddAiType.SelectedValue ' "53" 'First Lienholder
                interest.LoanNumber = Me.txtLoanNumber.Text

                If interest.Phones Is Nothing Then
                    interest.Phones = New List(Of QuickQuotePhone)()
                End If
                If interest.Phones.Any() = False Then
                    interest.Phones.Add(New QuickQuotePhone)
                End If
                interest.Phones(0).Number = Me.txtPhoneNumber.Text.Trim()
                interest.Phones(0).Extension = Me.txtPhoneExtension.Text.Trim()
                Exit Select
        End Select

        If String.IsNullOrWhiteSpace(Me.txtCommName.Text) = False Then
            ' use comm name
            interest.Name.CommercialName1 = Me.txtCommName.Text.Trim().ToUpper()
            'interest.Name.CommercialDBAname = Me.txtCommName.Text.Trim().ToUpper() 'removed 6/15/2017 since it sets the same private variable as CommercialName1
            interest.Name.FirstName = ""
            interest.Name.LastName = ""
            interest.GroupTypeId = "1" 'Lending Institution
            If interest.Phones IsNot Nothing AndAlso interest.Phones.Count > 0 Then interest.Phones(0).TypeId = "2"
            interest.Name.TypeId = "2"

            '6/15/2017 note: may need to maintain GroupType and PhoneType if already set... in case it was entered as Personal name (first/last) and our code updated it to Commercial name by combining first/last... not sure if we can determine if the user intentionally changed or we did... might need to flag on populate

        Else
            If String.IsNullOrWhiteSpace(Me.txtFirstName.Text) = False OrElse String.IsNullOrWhiteSpace(Me.txtLastName.Text) = False Then
                ' use pers name
                'interest.Name.FirstName = Me.txtFirstName.Text.Trim().ToUpper()
                'interest.Name.LastName = Me.txtLastName.Text.Trim().ToUpper()
                'interest.Name.CommercialName1 = ""
                'interest.GroupTypeId = "3" 'Individual / Corporation / Trust
                'If interest.Phones IsNot Nothing AndAlso interest.Phones.Count > 0 Then interest.Phones(0).TypeId = "1"
                'interest.Name.TypeId = "1"

                'updated 6/15/2017 for VR Commercial LOBs since personal names don't appear to show on the dec; note: leaving GroupType and PhoneType the same for all
                interest.GroupTypeId = "3" 'Individual / Corporation / Trust
                If interest.Phones IsNot Nothing AndAlso interest.Phones.Count > 0 Then interest.Phones(0).TypeId = "1"

                ' 7-10-17 MGB Per bug 20769 it appears that first, last name should be saved for commercial lines as well as personal
                interest.Name.FirstName = Me.txtFirstName.Text.Trim().ToUpper()
                interest.Name.LastName = Me.txtLastName.Text.Trim().ToUpper()
                interest.Name.CommercialName1 = ""

                'Select Case Quote.LobType
                '    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP 'add other Comm LOBs as needed
                '        interest.Name.CommercialName1 = QQHelper.appendText(Me.txtFirstName.Text.Trim().ToUpper(), Me.txtLastName.Text.Trim().ToUpper(), splitter:=" ")
                '        'interest.Name.CommercialDBAname = QQHelper.appendText(Me.txtFirstName.Text.Trim().ToUpper(), Me.txtLastName.Text.Trim().ToUpper(), splitter:=" ") 'removed 6/15/2017 since it sets the same private variable as CommercialName1
                '        interest.Name.FirstName = ""
                '        interest.Name.LastName = ""
                '        interest.Name.TypeId = "2"
                '    Case Else
                '        interest.Name.FirstName = Me.txtFirstName.Text.Trim().ToUpper()
                '        interest.Name.LastName = Me.txtLastName.Text.Trim().ToUpper()
                '        interest.Name.CommercialName1 = ""
                '        interest.Name.TypeId = "1"
                'End Select

                interest.Name.TypeId = "1"

            Else
                ' SHOULD NOT GET HERE use unknown  name
                interest.Name.FirstName = ""
                interest.Name.LastName = ""
                interest.Name.CommercialName1 = ""
                interest.GroupTypeId = "" 'none
                If interest.Phones IsNot Nothing AndAlso interest.Phones.Count > 0 Then interest.Phones(0).TypeId = "1"
                interest.Name.TypeId = ""
            End If
        End If

        'interest.LoanNumber = Me.txtLoanNumber.Text.Trim()
        interest.Address.HouseNum = Me.txtStreetNum.Text.Trim().ToUpper()
        interest.Address.StreetName = Me.txtStreet.Text.Trim().ToUpper()
        interest.Address.ApartmentNumber = Me.txtAptSuiteNum.Text.Trim().ToUpper()
        interest.Address.POBox = Me.txtPOBOX.Text.Trim().ToUpper()
        interest.Address.City = Me.txtCity.Text.Trim().ToUpper()
        interest.Address.StateId = Me.ddStateAbbrev.SelectedValue
        interest.Address.Zip = Me.txtZipCode.Text.Trim()

        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                interest.ATIMA = False
                interest.ISAOA = False
                interest.BillTo = False
                Exit Select
            Case Else
                interest.Description = Me.txtDescription.Text.Trim()
                interest.ATIMA = Me.chkATIMA.Checked
                interest.ISAOA = Me.chkISAOA.Checked
                interest.BillTo = Me.chkBillToMe.Checked
                Exit Select
        End Select
    End Sub

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        flaggedForDelete = True

        'Added 05/03/2021 for CAP Endorsements Task 52974 MLW
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Additional Interest" AndAlso MyAdditionalInterest IsNot Nothing Then
            UpdateEndorsementDevDictionaryForAIRemoval(MyAdditionalInterest)
        End If

        RaiseEvent RemoveAi(Me.AdditionalInterestIndex)
    End Sub

    'Added 06/21/2021 for CAP Endorsements Task 52974 MLW
    Private Sub UpdateEndorsementDevDictionaryForAIRemoval(myAdditionalInterest As QuickQuoteAdditionalInterest)
        Dim aiVehicleList As List(Of Integer) = New List(Of Integer)
        If IsNewAdditionalInterestOnEndorsement(myAdditionalInterest) Then
            'If an AI that was added to the endorsement is deleted from the master list, we only remove the DevDictionary AI Add entry. We do not count it as a delete.
            RemovePrevioulsyAddedAI(myAdditionalInterest, aiVehicleList)
        Else
            'When an existing AI is deleted at the master list level, we need to get the vehicle numbers that were originally assigned to this AI when the endorsement was created to include in the DevDictionary AI Delete entry.
            'First we get a list of vehicles that are associated to the AI. Remove any vehicles that were assigned to the AI during the endorsement change. 
            'Then, we include any vehicle numbers that were deleted by unassigning the vehicle in the ctl_CAP_App_Vehicle control.
            'Finally, we record the AIs Delete entry to the DevDictionary.

            'Check AI add list for vehicle numbers that might have been added prior to delete. Do not count those in the delete. Remove them from the add, well remove the AI add, then check for delete list
            Dim aiVehicleAddList As List(Of Integer) = GetVehicleNumListForAdditionalInterestInDevDictionary(DevDictionaryHelper.DevDictionaryHelper.addItem) 'need the added vehicle list before we remove the AI
            RemovePrevioulsyAddedAI(myAdditionalInterest, aiVehicleList)
            aiVehicleList = GetVehicleNumListForAdditionalInterestInDevDictionary(DevDictionaryHelper.DevDictionaryHelper.deleteItem)

            'look up what vehicles have this AI. Add all vehicle numbers to the vehicle number list, send list in Delete.
            Dim identifiedVehiclesAndAIs As List(Of QuickQuoteVehicleAndAdditionalInterests) = Nothing
            QQHelper.IdentifySpecificQuickQuoteAdditionalInterestFromVehicles(Me.Quote.Vehicles, QQHelper.IntegerForString(myAdditionalInterest.ListId), identifiedResults:=identifiedVehiclesAndAIs, shouldRemove:=False)
            If identifiedVehiclesAndAIs IsNot Nothing AndAlso identifiedVehiclesAndAIs.Count > 0 Then
                For Each vai As QuickQuoteVehicleAndAdditionalInterests In identifiedVehiclesAndAIs
                    If vai IsNot Nothing AndAlso vai.Vehicle IsNot Nothing AndAlso vai.AdditionalInterests IsNot Nothing AndAlso vai.AdditionalInterests.Count > 0 Then
                        Dim diaVehicleNum As String = vai.Vehicle.DisplayNum
                        If aiVehicleAddList IsNot Nothing AndAlso aiVehicleAddList.Count > 0 Then
                            If Not aiVehicleAddList.Contains(diaVehicleNum) Then
                                aiVehicleList.Add(diaVehicleNum)
                            End If
                        Else
                            aiVehicleList.Add(diaVehicleNum)
                        End If
                    End If
                Next
            End If
            If aiVehicleList IsNot Nothing AndAlso aiVehicleList.Count > 1 Then
                aiVehicleList.Sort()
            End If
            'Dim identifiedResults As QuickQuoteAdditionalInterestRelatedResults = Nothing
            'QQHelper.IdentifySpecificQuickQuoteAdditionalInterestFromQuoteBasedOnLob(Me.Quote, QQHelper.IntegerForString(MyAdditionalInterest.ListId), identifiedResults:=identifiedResults, shouldRemove:=False, includeTopLevel:=False)
            'If identifiedResults IsNot Nothing Then
            '    Select Case Me.Quote.LobType
            '        Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
            '            If identifiedResults.VehiclesAndAdditionalInterests IsNot Nothing AndAlso identifiedResults.VehiclesAndAdditionalInterests.Count > 0 Then
            '                For Each vai As QuickQuoteVehicleAndAdditionalInterests In identifiedResults.VehiclesAndAdditionalInterests
            '                    If vai IsNot Nothing AndAlso vai.Vehicle IsNot Nothing AndAlso vai.AdditionalInterests IsNot Nothing AndAlso vai.AdditionalInterests.Count > 0 Then
            '                        'Dim diaVehicleNum As String = vai.Vehicle.OriginalDisplayNum
            '                        Dim diaVehicleNum As String = vai.Vehicle.DisplayNum
            '                    End If
            '                Next
            '            End If
            '    End Select
            'End If

            ddh.UpdateDevDictionaryAdditionalInterestList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, myAdditionalInterest, aiVehicleList)
        End If
        Dim endorsementsRemarksHelper = New EndorsementsRemarksHelper(ddh)
        Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(endorsementsRemarksHelper.RemarksType.AdditionalInterest)
        Quote.TransactionRemark = updatedRemarks
    End Sub

    'Added 06/21/2021 for CAP Endorsements task 52974 MLW
    Private Function GetVehicleNumListForAdditionalInterestInDevDictionary(dictionaryItemType As String) As List(Of Integer)
        'This will look up the DevDictionary list for the AI and return it back to use in the add or delete AI entry update.
        Dim strVehicleNumsForAIList As String = ""
        Dim aiVehicleList As List(Of Integer) = New List(Of Integer)
        Dim aiExistsInList As Boolean = False
        If ddh.GetAdditionalInterestDictionary IsNot Nothing AndAlso ddh.GetAdditionalInterestDictionary.Count > 0 Then
            For Each additionalInterest As DevDictionaryHelper.AdditionalInterestInfo In ddh.GetAdditionalInterestDictionary
                If additionalInterest.addOrDelete = dictionaryItemType Then
                    If additionalInterest.diaAINumber = MyAdditionalInterest.ListId Then
                        aiExistsInList = True
                        strVehicleNumsForAIList = additionalInterest.VehicleNumList
                        Exit For
                    End If
                End If
            Next
        End If
        If aiExistsInList = True Then
            aiVehicleList = ddh.ConvertStringToAIVehicleList(strVehicleNumsForAIList)
        End If
        Return aiVehicleList
    End Function

    'Added 06/21/2021 for CAP Endorsements task 52974 MLW
    Private Sub RemovePrevioulsyAddedAI(myAdditionalInterest As QuickQuoteAdditionalInterest, aiVehicleList As List(Of Integer))
        'Used when we delete an AI from the master list. Want to remove any AI Adds that happened during the endorsement change before we record the Delete entry for the AI.
        ddh.UpdateDevDictionaryAdditionalInterestList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.RemoveAdd, myAdditionalInterest, aiVehicleList)
        'After we remove the entry, we need to know if the Quote.TransactionReasonId needs to be updated. 
        'Below Is used to determine if any Add entries exist in the DevDictionary for any AIs. If so, we keep the transaction reason id to 10169. Otherwise, switch it to 10168.
        'Any add AIs need Quote.TransactionReasonId = 10169. All other AI transactions are 10168. This is so the correct dec is in the print history.
        Dim hasAddedAI As Boolean = False
        Dim aiList = ddh.GetAdditionalInterestDictionary
        For Each ai In aiList
            If ai.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem Then
                hasAddedAI = True
                Exit For
            End If
        Next
        If hasAddedAI = False Then
            Quote.TransactionReasonId = 10168 'Endorsement Change Dec Only
        End If
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
    End Sub

    Public Overrides Sub ClearControl()
        Me.txtAiId.Text = ""
        Me.txtAptSuiteNum.Text = ""
        Me.txtCity.Text = ""
        Me.txtCommName.Text = ""
        Me.txtDescription.Text = ""
        Me.txtFirstName.Text = ""
        Me.txtIsEditable.Text = ""
        Me.txtLastName.Text = ""
        Me.txtLoanNumber.Text = ""
        Me.txtMiddleName.Text = ""
        Me.txtPhoneExtension.Text = ""
        Me.txtPhoneNumber.Text = ""
        Me.txtPOBOX.Text = ""
        Me.txtStreet.Text = ""
        Me.txtStreetNum.Text = ""
        Me.txtZipCode.Text = ""
        Me.ddAiType.SelectedIndex = -1
        Me.ddCityName.SelectedIndex = -1
        Me.ddStateAbbrev.SelectedIndex = -1
        Me.chkATIMA.Checked = False
        Me.chkBillToMe.Checked = False
        Me.chkAppliesTo.Checked = False
        Me.chkISAOA.Checked = False

        MyBase.ClearControl()
    End Sub


End Class