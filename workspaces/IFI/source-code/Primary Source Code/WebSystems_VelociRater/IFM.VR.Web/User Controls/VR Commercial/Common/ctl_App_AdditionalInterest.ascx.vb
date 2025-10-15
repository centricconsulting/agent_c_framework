Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Public Class ctl_App_AdditionalInterest
    Inherits VRControlBase

    Public Event RemoveAi(index As Int32)

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

    Public Property MyAiList As List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)
        Get
            Dim AiList As List(Of QuickQuoteAdditionalInterest) = Nothing
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    AiList = Quote.AdditionalInterests
                    Exit Select
            End Select
            Return AiList
        End Get
        Set(value As List(Of QuickQuoteAdditionalInterest))
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    Quote.AdditionalInterests = value
                    Exit Select
            End Select
        End Set
    End Property

    Public ReadOnly Property MyAdditionalInterest As QuickQuoteAdditionalInterest
        Get
            If MyAiList IsNot Nothing AndAlso MyAiList.Count > 0 Then
                Return MyAiList(Me.AdditionalInterestIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()


        Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Remove?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)

        Dim clearAiIdScript As String = "if ($(""#" + Me.txtIsEditable.ClientID + """).val().toString().toLowerCase() == 'false') {$(""#" + Me.txtAiId.ClientID + """).val('');}"

        'Dim setBindings As String = "AdditionalInterest.SetAdditionalInterestLookupBindings('" + Me.ClientID + "','" + Me.txtCommName.ClientID + "','" + Me.txtFirstName.ClientID + "','" + Me.txtMiddleName.ClientID + "','" + Me.txtLastName.ClientID + "','" + Me.txtPhoneNumber.ClientID + "','" + Me.txtPhoneExtension.ClientID + "','" + Me.txtPOBOX.ClientID + "','" + Me.txtAptSuiteNum.ClientID + "','" + Me.txtStreetNum.ClientID + "','" + Me.txtStreet.ClientID + "','" + Me.txtCity.ClientID + "','" + Me.ddStateAbbrev.ClientID + "','" + Me.txtZipCode.ClientID + "','" + Me.txtAiId.ClientID + "','" + Me.txtIsEditable.ClientID + "');"
        Dim setBindings As String = "AdditionalInterest.SetAdditionalInterestLookupBindings('" + Me.ClientID + "','" + Me.txtCommName.ClientID + "','" + Me.txtFirstName.ClientID + "','" + Me.txtMiddleName.ClientID + "','" + Me.txtLastName.ClientID + "','','','" + Me.txtPOBOX.ClientID + "','" + Me.txtAptSuiteNum.ClientID + "','" + Me.txtStreetNum.ClientID + "','" + Me.txtStreet.ClientID + "','" + Me.txtCity.ClientID + "','" + Me.ddStateAbbrev.ClientID + "','" + Me.txtZipCode.ClientID + "','" + Me.txtAiId.ClientID + "','" + Me.txtIsEditable.ClientID + "');"
        Me.VRScript.AddScriptLine(setBindings)

        Dim interestLookup As String = "AdditionalInterest.DoAdditionalInterestLookup('" + Me.ClientID + "'.toString(),'" + Me.txtCommName.ClientID + "','" + Me.txtCommName.ClientID + "','" + Me.txtFirstName.ClientID + "','" + Me.txtMiddleName.ClientID + "','" + Me.txtLastName.ClientID + "'); "
        Me.VRScript.CreateJSBinding(Me.txtCommName, ctlPageStartupScript.JsEventType.onkeyup, interestLookup + clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.txtFirstName, ctlPageStartupScript.JsEventType.onkeyup, interestLookup + clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.txtMiddleName, ctlPageStartupScript.JsEventType.onkeyup, interestLookup + clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.txtLastName, ctlPageStartupScript.JsEventType.onkeyup, interestLookup + clearAiIdScript)

        Me.VRScript.CreateJSBinding(Me.txtAptSuiteNum, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.txtCity, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.txtPOBOX, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.txtStreet, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        Me.VRScript.CreateJSBinding(Me.txtStreetNum, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript)
        'Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript + "DoCityCountyLookup('" + Me.txtZipCode.ClientID + "','" + Me.ddCityName.ClientID + "','" + Me.txtCity.ClientID + "','','" + Me.ddStateAbbrev.ClientID + "');")
        Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup, clearAiIdScript + "DoCityCountyLookup('" + Me.txtZipCode.ClientID + "','','" + Me.txtCity.ClientID + "','','" + Me.ddStateAbbrev.ClientID + "');")
        Me.VRScript.CreateTextBoxFormatter(Me.txtZipCode, ctlPageStartupScript.FormatterType.ZipCode, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateJSBinding(Me.ddStateAbbrev, ctlPageStartupScript.JsEventType.onchange, clearAiIdScript)

        Me.VRScript.AddScriptLine("$(""#" + Me.txtCity.ClientID + """).autocomplete({ source: INCities });")

        Dim addWarning_True As String = "DoAddressWarning(true,'" + Me.divAddressMessage.ClientID + "','" + Me.txtStreetNum.ClientID + "','" + txtStreet.ClientID + "','" + txtPOBOX.ClientID + "');"
        Dim addWarning_False As String = "DoAddressWarning(false,'" + Me.divAddressMessage.ClientID + "','" + Me.txtStreetNum.ClientID + "','" + txtStreet.ClientID + "','" + txtPOBOX.ClientID + "');"

        Me.VRScript.CreateJSBinding(Me.txtStreetNum, ctlPageStartupScript.JsEventType.onfocus, addWarning_True)
        Me.VRScript.CreateJSBinding(Me.txtStreetNum, ctlPageStartupScript.JsEventType.onblur, addWarning_False)

        Me.VRScript.CreateJSBinding(Me.txtStreet, ctlPageStartupScript.JsEventType.onfocus, addWarning_True)
        Me.VRScript.CreateJSBinding(Me.txtStreet, ctlPageStartupScript.JsEventType.onblur, addWarning_False)

        Me.VRScript.CreateJSBinding(Me.txtPOBOX, ctlPageStartupScript.JsEventType.onfocus, addWarning_True)
        Me.VRScript.CreateJSBinding(Me.txtPOBOX, ctlPageStartupScript.JsEventType.onblur, addWarning_False)

#If Not DEBUG Then
        Me.txtAiId.Attributes.Add("style","display:none;")
#End If

        'Me.VRScript.AddScriptLine("$(""#" + Me.ddCityName.ClientID + """).hide();") ' do at startup

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
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddStateAbbrev, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.TextAscending, Me.Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()

        If MyAdditionalInterest IsNot Nothing Then

            Me.txtAiId.Text = MyAdditionalInterest.ListId
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

            If Me.lblExpanderText.Text.Length > 45 Then
                Me.lblExpanderText.Text = Me.lblExpanderText.Text.Substring(0, 45) + "..."
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
        End If

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'MyBase.ValidateControl(valArgs) does it below in the DoValidationCheck
        If Me.Visible Then
            MyBase.ValidateControl(valArgs)
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    Me.ValidationHelper.GroupName = String.Format("Additional Interest #{1}", Me.AdditionalInterestIndex + 1)
                    Exit Select
            End Select

            Dim paneIndex = Me.AdditionalInterestIndex
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            Dim aiVals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.AdditionalInterestValidation(MyAdditionalInterest, valArgs.ValidationType)
            For Each v In aiVals
                Select Case v.FieldId
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
                    MyAiList = New List(Of QuickQuoteAdditionalInterest)
                End If
                While (MyAiList.Count < AdditionalInterestIndex + 1)
                    MyAiList.Add(New QuickQuoteAdditionalInterest())
                End While

                If MyAdditionalInterest IsNot Nothing Then
                    MyAdditionalInterest.AgencyId = DirectCast(Me.Page.Master, VelociRater).AgencyID.ToString()
                    MyAdditionalInterest.SingleEntry = False

                    If String.IsNullOrWhiteSpace(Me.txtAiId.Text) Or IsNumeric(Me.txtAiId.Text) = False Then
                        MyAdditionalInterest.ListId = "" ' basically lets this AI become something else
                        ' IS NEW AI
                        InjectFormIntoAI(MyAdditionalInterest) ' persist always
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
                                    Me.txtIsEditable.Text = "true" 'added 2/16/2021
                                End If
                            End If
                        End If
                    Else
                        ' already existed
                        Dim qqXml As New QuickQuoteXML()
                        Dim loadedAi As QuickQuoteAdditionalInterest = Nothing

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
                                ' save that loaded AI with any changes - well the changes will be saved soon
                                MyAiList(AdditionalInterestIndex) = loadedAi
                            End If
                        Else
                            'no changes to name or address allowed - but you need the loan number and it looks like that is all they changed or the ID would have been blank
                            If loadedAi IsNot Nothing Then
                                loadedAi.TypeId = "17"  ' Property = Addl Insured
                                'loadedAi.TypeId = Me.ddAiType.SelectedValue '"53"
                                MyAiList(AdditionalInterestIndex) = loadedAi
                                'loadedAi.OverwriteAdditionalInterestListInfoForDiamondId = True ' without this it will not commit the edits; 6/1/2017 note: Overwrite flag should be specific to list info (name/address/phones/emails), so it isn't necessary to set when updating other fields as normal Save will update them either way... should be okay to leave in here since LoadDiamondAdditionalInterestListIntoQuickQuoteAdditionalInterest has re-populated the info anyway; removed 2/16/2021 to prevent extra calls to DiamondService_CreateOrUpdateAdditionalInterestList from qqXml, which could cause duplicate address/name link records; note: OverwriteAdditionalInterestListInfoForDiamondId should only be set to True if the AiList record was created during the current session
                            Else
                                ' should not get here because the AI loaded above should not be returning nothing
                                'MyAdditionalInterest.TypeId = Me.ddAiType.SelectedValue '"53"
                                loadedAi.TypeId = "17" ' Property = Addl Insured
                                MyAdditionalInterest.ListId = Me.txtAiId.Text
                            End If
                        End If
                    End If
                    Return True
                End If
            End If
        End If

        Return False
    End Function

    Private Sub InjectFormIntoAI(interest As QuickQuoteAdditionalInterest)
        interest.TypeId = "17"

        'interest.LoanNumber = Me.txtLoanNumber.Text

        If String.IsNullOrWhiteSpace(Me.txtCommName.Text) = False Then
            ' use comm name
            interest.Name.CommercialName1 = Me.txtCommName.Text.Trim().ToUpper()
            interest.Name.CommercialDBAname = Me.txtCommName.Text.Trim().ToUpper()
            interest.Name.FirstName = ""
            interest.Name.LastName = ""
            interest.GroupTypeId = "1" 'Lending Institution
            'interest.Phones(0).TypeId = "2"
            interest.Name.TypeId = "2"
        Else
            If String.IsNullOrWhiteSpace(Me.txtFirstName.Text) = False OrElse String.IsNullOrWhiteSpace(Me.txtLastName.Text) = False Then
                ' use pers name
                interest.Name.FirstName = Me.txtFirstName.Text.Trim().ToUpper()
                interest.Name.LastName = Me.txtLastName.Text.Trim().ToUpper()
                interest.Name.CommercialName1 = ""
                interest.GroupTypeId = "3" 'Individual / Corporation / Trust
                'interest.Phones(0).TypeId = "1"
                interest.Name.TypeId = "1"
            Else
                ' use unknown  name
                interest.Name.FirstName = ""
                interest.Name.LastName = ""
                interest.Name.CommercialName1 = ""
                interest.GroupTypeId = "" 'none
                'interest.Phones(0).TypeId = "1"
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
        'interest.Description = Me.txtDescription.Text.Trim()

        'interest.ATIMA = Me.chkATIMA.Checked
        'interest.ISAOA = Me.chkISAOA.Checked
        'interest.BillTo = Me.chkBillToMe.Checked
    End Sub

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        RaiseEvent RemoveAi(Me.AdditionalInterestIndex)
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        'Me.Save_FireSaveEvent(True)
        Me.Save_FireSaveEvent(False)
    End Sub

    Public Overrides Sub ClearControl()
        Me.txtAiId.Text = ""
        Me.txtAptSuiteNum.Text = ""
        Me.txtCity.Text = ""
        Me.txtCommName.Text = ""
        Me.txtFirstName.Text = ""
        Me.txtIsEditable.Text = ""
        Me.txtLastName.Text = ""
        Me.txtMiddleName.Text = ""
        Me.txtPOBOX.Text = ""
        Me.txtStreet.Text = ""
        Me.txtStreetNum.Text = ""
        Me.txtZipCode.Text = ""
        'Me.ddAiType.SelectedIndex = -1
        'Me.ddCityName.SelectedIndex = -1
        Me.ddStateAbbrev.SelectedIndex = -1

        MyBase.ClearControl()
    End Sub


End Class