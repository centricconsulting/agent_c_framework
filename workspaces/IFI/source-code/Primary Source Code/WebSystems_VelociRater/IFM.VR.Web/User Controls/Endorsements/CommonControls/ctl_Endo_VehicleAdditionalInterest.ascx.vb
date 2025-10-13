Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.VR.Web.EndorsementStructures

Public Class ctl_Endo_VehicleAdditionalInterest
    Inherits VRControlBase

    'Added 05/03/2021 for CAP Endorsements Task 52974 MLW
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
        'Get
        '    'get from Direct Parent ViewState
        '    Dim test = ParentVrControl
        '    If Me.ParentVrControl IsNot Nothing AndAlso TypeOf Me.ParentVrControl Is ctl_Endo_VehicleAdditionalInterestList Then
        '        Dim Parent = CType(ParentVrControl, ctl_Endo_VehicleAdditionalInterestList)
        '        Dim Parent2 = Parent.ParentVrControl
        '        Return Parent.TransactionLimitReached
        '    End If
        '    Return False
        'End Get

        Get
            Return ViewState.GetBool("vs_CppTransactionLimitReachedAi", False, True)
        End Get
        Set(value As Boolean)
            ViewState("vs_CppTransactionLimitReachedAi") = value
        End Set
    End Property


    Public Event AIListChanged()
    Public Event UpdateTransactionReasonType(ddh As DevDictionaryHelper.DevDictionaryHelper)
    Public Event CountTransactions()

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

    Public Property MyAiList As List(Of QuickQuoteAdditionalInterest)
        Get
            Dim AiList As List(Of QuickQuoteAdditionalInterest) = Nothing
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    AiList = Quote.AdditionalInterests
            End Select
            Return AiList
        End Get
        Set(value As List(Of QuickQuoteAdditionalInterest))
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    Quote.AdditionalInterests = value
            End Select
        End Set
    End Property
    Public ReadOnly Property MyAppliedAiList As List(Of LocationBuildingAIItem)
        Get
            Dim appAIs = New Helpers.FindAppliedAdditionalInterestList
            Return appAIs.FindAppliedAI(Quote)
        End Get
    End Property

    Public ReadOnly Property MyAdditionalInterest As QuickQuoteAdditionalInterest
        Get
            'Updated 04/29/2021 for CAP Endorsements Task 52974 MLW
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

    Dim flaggedForDelete As Boolean = False 'added 4/1/2020

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If IsSingleAI(MyAdditionalInterest) = False Then
            Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "This Additional Interest is assigned to one or more items. If the Additional Interest is removed it will be removed from all items. Are you sure you want to remove? To remove the Additional Interest from a specific item, please do so from below.")
        Else
            Me.VRScript.StopEventPropagation(Me.lnkRemove.ClientID)
        End If
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

        If String.IsNullOrWhiteSpace(TypeOfEndorsement()) = False Then
            Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
            AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
            Select Case TypeOfEndorsement()
                Case EndorsementTypeString.BOP_AddDeleteContractorsEquipment,
                 EndorsementTypeString.BOP_AddDeleteContractorsEquipmentLienholder,
                 EndorsementTypeString.BOP_AddDeleteLocationLienholder,
                 EndorsementTypeString.BOP_AddDeleteLocation
                    If MyAdditionalInterest IsNot Nothing AndAlso String.IsNullOrWhiteSpace(MyAdditionalInterest.ListId) = False Then
                        If AllPreExistingItems.PreExisting_AdditionalInterests.isPreExistingAI(MyAdditionalInterest.ListId) Then
                            DisableControls()
                            DisableHeaderLinks()
                        End If
                    End If
                Case EndorsementTypeString.CPP_AddDeleteContractorsEquipmentLienholder,
                    EndorsementTypeString.CPP_AddDeleteLocationLienholder
                    If AllPreExistingItems.PreExisting_AdditionalInterests.isPreExistingAI(MyAdditionalInterest.ListId) Then
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
        If Me.ddStateAbbrev.Items.Count = 0 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddAiType, QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuotePropertyName.TypeId, SortBy.TextAscending, Me.Quote.LobType)
            For Each item As ListItem In Me.ddAiType.Items
                item.Attributes.Add("title", item.Text)
            Next
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddStateAbbrev, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.TextAscending, Me.Quote.LobType)


        End If
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        Dim test = MyAiList
        Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
        AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)

        Me.trAppliesTo.Visible = False
        Me.trDescription.Visible = False
        Me.trATIMA.Visible = False
        Me.trISAOA.Visible = False
        Me.trBillTo.Visible = False
        Select Case Me.Quote.LobType
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

                    IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddAiType, MyAdditionalInterest.TypeId, QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuotePropertyName.TypeId)

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
            Me.txtStreetNum.Text = MyAdditionalInterest.Address.HouseNum
            Me.txtStreet.Text = MyAdditionalInterest.Address.StreetName
                Me.txtAptSuiteNum.Text = MyAdditionalInterest.Address.ApartmentNumber
            Me.txtPOBOX.Text = MyAdditionalInterest.Address.POBox
            Me.txtCity.Text = MyAdditionalInterest.Address.City
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, MyAdditionalInterest.Address.StateId)
                Me.txtZipCode.Text = MyAdditionalInterest.Address.Zip

            Select Case Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    Exit Select
                Case Else
                    If MyAdditionalInterest.Phones IsNot Nothing AndAlso MyAdditionalInterest.Phones.Any() Then
                        Me.txtPhoneNumber.Text = MyAdditionalInterest.Phones(0).Number
                        Me.txtPhoneExtension.Text = MyAdditionalInterest.Phones(0).Extension
                    End If

                    Me.txtDescription.Text = MyAdditionalInterest.Description
                    Me.chkATIMA.Checked = MyAdditionalInterest.ATIMA
                    Me.chkISAOA.Checked = MyAdditionalInterest.ISAOA
                    Me.chkBillToMe.Checked = MyAdditionalInterest.BillTo
                    Exit Select
            End Select

            'Total Transactions Counted and UI components removed. Not limiting 10/08/2021 CAH
            If TransactionLimitReached Then
                If AllPreExistingItems.PreExisting_AdditionalInterests.isPreExistingAI(MyAdditionalInterest.ListId) Then
                    lnkRemove.Visible = False
                End If
            End If

        End If
    End Sub

    ''' <summary>
    ''' Disables all the controls on the page
    ''' </summary>
    Private Sub DisableControls()
        VRScript.AddScriptLine("$(document).ready(function () {ifm.vr.ui.SingleContainerContentDisable(['" + Me.divAiEntry.ClientID + "']);});")
    End Sub

    Private Sub DisableHeaderLinks()
        Me.lnkSave.Visible = False
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'MyBase.ValidateControl(valArgs) does it below in the DoValidationCheck
        'Updated 12/29/2020 for CAP Endorsements Task 52974 MLW
        If Me.Visible Then
            MyBase.ValidateControl(valArgs)
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    Me.ValidationHelper.GroupName = String.Format("Additional Interest #{0}", Me.AdditionalInterestIndex + 1)
            End Select

            Dim paneIndex = Me.AdditionalInterestIndex
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            'Dim aiVals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.AdditionalInterestValidation(MyAdditionalInterest, valArgs.ValidationType)
            Dim aiVals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.AdditionalInterestValidation(MyAdditionalInterest, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.endorsement)
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
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreet, v, accordList)
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBOX, v, accordList)
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
                End Select
            Next

        End If
    End Sub

    Public Overrides Function Save() As Boolean


        If Me.Visible Then
            If Quote IsNot Nothing Then

                If MyAiList Is Nothing Then
                    MyAiList = New List(Of QuickQuoteAdditionalInterest)()
                End If
                While (MyAiList.Count < AdditionalInterestIndex + 1)
                    MyAiList.Add(New QuickQuoteAdditionalInterest())
                End While

                If MyAdditionalInterest IsNot Nothing AndAlso flaggedForDelete = False Then
                    MyAdditionalInterest.AgencyId = DirectCast(Me.Page.Master, VelociRater).AgencyID.ToString()
                    MyAdditionalInterest.SingleEntry = False

                    If (String.IsNullOrWhiteSpace(Me.txtAiId.Text) Or IsNumeric(Me.txtAiId.Text) = False) Then
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

                                    If TypeOfEndorsement() = EndorsementTypeString.BOP_AddDeleteLocationLienholder Then
                                    End If

                                    RaiseEvent AIListChanged()  ' MGB 5-9-17 for BOP
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

                        qqXml.LoadDiamondAdditionalInterestListIntoQuickQuoteAdditionalInterest_OptionallyUseExistingAdditionalInterestForBaseInfo(Me.txtAiId.Text.Trim(), loadedAi, existingQQAdditionalInterest:=MyAdditionalInterest)
                        If Me.AdditionalInterstIdsCreatedThisSession.Contains(CInt(Me.txtAiId.Text.Trim())) Then
                            ' was created this session it is probably safe to edit

                            ' ************* you don't want to pass in the 'interest' object above  ***********************
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
                                        Exit Select
                                End Select

                                'added 9/13/2017
                                If needsToRaiseChangedEvent = False AndAlso MyAdditionalInterest IsNot Nothing Then
                                    If QuickQuoteHelperClass.IsQuickQuoteObjectMatch_Name(MyAdditionalInterest.Name, loadedAi.Name) = False Then
                                        needsToRaiseChangedEvent = True
                                    End If
                                End If


                                MyAiList(AdditionalInterestIndex) = loadedAi
                            Else
                                ' should not get here because the AI loaded above should not be returning nothing
                                Select Case Quote.LobType
                                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
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

                        'added 9/13/2017
                        If needsToRaiseChangedEvent = True Then
                            RaiseEvent AIListChanged()
                        End If

                    End If
                    If TypeOfEndorsement() = EndorsementTypeString.BOP_AddDeleteLocationLienholder _
                        OrElse TypeOfEndorsement() = EndorsementTypeString.CPP_AddDeleteContractorsEquipmentLienholder Then
                        If BopDictItems.UpdateAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, MyAdditionalInterest) Then
                        End If
                    End If
                    Return True
                End If


            End If
        End If

        Return False
    End Function

    'Added 01/26/2021 for CAP Endorsements Task 52974 MLW
    Private Function AllowPopulate() As Boolean
        'Remove?  We're always on Endo Pages
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                If IsOnAppPage Then
                    Return True
                ElseIf IsQuoteReadOnly() Then
                    Return True
                ElseIf (IsQuoteEndorsement() AndAlso (TypeOfEndorsement() = "Add/Delete a lienholder on contractors' equipment" OrElse TypeOfEndorsement() = "Add/Delete contractors' equipment")) Then
        Return True
                Else
                    Return False
                End If
            Case Else
                Return True
        End Select
    End Function

    Private Function AllowValidateAndSave() As Boolean
        'Remove?  We're always on Endo Pages
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                If IsOnAppPage OrElse IsQuoteEndorsement() Then
                    Return True
                Else
                    Return False
                End If
            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                If IsOnAppPage Then
                    Return True
                    'ElseIf (IsQuoteEndorsement() AndAlso (TypeOfEndorsement() = "Add/Delete Additional Interest" OrElse TypeOfEndorsement() = "Add/Delete Vehicle")) Then
                    '    Return True
                ElseIf IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete contractors' equipment" Then 'AndAlso IsNewAdditionalInterestOnEndorsement(MyAdditionalInterest) ' AndAlso IsNewVehicleOnEndorsement(MyVehicle)
                    'TODO: Mary - Need to do something here so that it does not save the existing AI, which gives validation errors when an AI has both the street and PO box.
                    'The issue probably has to do with the AI not saving yet. Need to fix saving a new AI. Once it saves adding the isnewAI(myAI) should work when added back to this if stmt. (it throws an index out of range message for the AI index).
                    Return True
                ElseIf IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete a lienholder on contractors’ equipment" Then ' AndAlso IsNewAdditionalInterestOnEndorsement(MyAdditionalInterest)
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
                'updated 6/15/2017 for VR Commercial LOBs since personal names don't appear to show on the dec; note: leaving GroupType and PhoneType the same for all
                interest.GroupTypeId = "3" 'Individual / Corporation / Trust
                If interest.Phones IsNot Nothing AndAlso interest.Phones.Count > 0 Then interest.Phones(0).TypeId = "1"

                ' 7-10-17 MGB Per bug 20769 it appears that first, last name should be saved for commercial lines as well as personal
                interest.Name.FirstName = Me.txtFirstName.Text.Trim().ToUpper()
                interest.Name.LastName = Me.txtLastName.Text.Trim().ToUpper()
                interest.Name.CommercialName1 = ""

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


        RaiseEvent RemoveAi(Me.AdditionalInterestIndex)

    End Sub

    Protected Sub checkTransactionReasonType(ddh As DevDictionaryHelper.DevDictionaryHelper)
        RaiseEvent UpdateTransactionReasonType(ddh)

    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        ' FOR COMMERCIAL LINES - Don't fire the validation because we're making the user save any additional interests before they can use them
        ' further down the page and we don't want to ding them for field validations when we save the additional interest
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                Me.Save_FireSaveEvent(True)
                Exit Select
            Case Else
                Me.Save_FireSaveEvent(True)
                Exit Select
        End Select

        RaiseEvent AIListChanged()

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

    Public Function IsSingleAI(MyAddInterest As QuickQuoteAdditionalInterest) As Boolean
        If MyAppliedAiList IsNot Nothing AndAlso String.IsNullOrWhiteSpace(MyAddInterest?.ListId) = False Then
            Dim count = MyAppliedAiList.FindAll(Function(x) x.AI.ListId = MyAddInterest.ListId).Count < 2
            Return count
        End If
        Return False
    End Function


End Class