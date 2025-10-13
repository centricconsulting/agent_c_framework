Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.PPA

Public Class ctl_Billing_Info_PPA
    Inherits VRControlBase

    Public const AgreeToEFTTerms As String = "AgreeToEFTTerms"

    Public ReadOnly Property MethodID() As String
        Get
            Return Me.ddMethod.ClientID
        End Get
    End Property

    Public ReadOnly Property BillToID() As String
        Get
            Return Me.ddBillTo.ClientID
        End Get
    End Property

    Public ReadOnly Property IsBillingUpdate() As Boolean
        Get
            Dim result As Boolean = False
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("isBillingUpdate") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("isBillingUpdate").ToString) = False Then
                result = CBool(Request.QueryString("isBillingUpdate").ToString)
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("isBillingUpdate") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("isBillingUpdate").ToString) = False Then
                result = CBool(Page.RouteData.Values("isBillingUpdate").ToString)
            Else
                If Me.Quote IsNot Nothing AndAlso Me.Quote.IsBillingEndorsement = True Then
                    result = True
                End If
            End If
            Return result
        End Get

    End Property

    Public Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = Me.divBillingInfo.ClientID
        End If

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.Quote IsNot Nothing Then
            ' When this is a billing endorsement ('billing change' in the actions dropdown), open the Billing and EFT accordions.
            ' When it's a non billing endorsement ('process change' from the actions dropdown), close the billing and EFT accordions.
            Dim AccordOpen As Integer = 0
            Dim AccordClose As Integer = 1
            If Quote.IsBillingEndorsement Then
                ' Billing endorsement
                Me.VRScript.CreateAccordion(MainAccordionDivId, hiddenDivBillingActive, AccordOpen)
                Me.VRScript.CreateAccordion(Me.divEFTInfo.ClientID, Nothing, AccordOpen)
            ElseIf Quote.IsNonBillingEndorsement Then
                ' Non-billing endorsement
                Me.VRScript.CreateAccordion(MainAccordionDivId, hiddenDivBillingActive, AccordClose)
                Me.VRScript.CreateAccordion(Me.divEFTInfo.ClientID, Nothing, AccordOpen)
            Else
                ' Everything else - open accordions
                Me.VRScript.CreateAccordion(MainAccordionDivId, hiddenDivBillingActive, AccordOpen)
                Me.VRScript.CreateAccordion(Me.divEFTInfo.ClientID, Nothing, AccordOpen)
            End If

            Me.VRScript.CreateAccordion(Me.divBillingInfoAddress.ClientID, Nothing, 0)

            Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID, True)
            Me.VRScript.AddVariableLine(String.Format("var eftBillingDivId = '{0}';", Me.divEFTInfo.ClientID)) ' used in js method BilltoMethodChanged(...);
            Me.VRScript.CreateJSBinding(Me.txtEFTRouting, ctlPageStartupScript.JsEventType.onkeyup, "GetBankNameFromRoutingNumber('" + Me.lblBankName.ClientID + "',$(this).val());")

            Me.VRScript.AddScriptLine("GetBankNameFromRoutingNumber('" + Me.lblBankName.ClientID + "','" + Me.txtEFTRouting.Text + "');")

            Dim billTo_Other_id As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BillToId, "Other", Me.Quote.LobType)
            Dim billTo_Mortgagee_id As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BillToId, "Mortgagee", Me.Quote.LobType)
            Dim billTo_Insured_id As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BillToId, "Insured", Me.Quote.LobType)
            Dim billTo_Agent_id As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BillToId, "Agent", Me.Quote.LobType)

            Dim method_DirectBill_Id As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BillMethodId, "Direct Bill", Me.Quote.LobType)
            Dim method_AgencyBill_Id As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BillMethodId, "Agency Bill", Me.Quote.LobType)

            Dim ddMethodScript As String = "BilltoMethodChanged('" + Me.ddMethod.ClientID + "','" + Me.ddPayPlan.ClientID + "','" + Me.ddBillTo.ClientID + "','" + Me.divBillingInfoAddress.ClientID + "','" & Me.IsQuoteEndorsement & "');"

            Me.VRScript.CreateJSBinding(Me.ddMethod, ctlPageStartupScript.JsEventType.onchange, ddMethodScript)
            Me.VRScript.CreateJSBinding(Me.ddPayPlan, ctlPageStartupScript.JsEventType.onchange, ddMethodScript)

            If IsCommercialQuote() Then
                Me.VRScript.CreateJSBinding(Me.ddPayPlan, ctlPageStartupScript.JsEventType.onchange, "ShowAccountBillAvailText('" & Me.ddPayPlan.ClientID & "','" & Me.divAccountBillAvailText.ClientID & "');")
            End If

            If IsQuoteEndorsement() OrElse IsBillingUpdate() = True Then
                Dim eftDateValue As DateTime
                Dim eftDayValue As Integer = 0
                If String.IsNullOrWhiteSpace(Quote.EffectiveDate) = False Then
                    If DateTime.TryParse(Quote.EffectiveDate, eftDateValue) Then
                        eftDayValue = eftDateValue.Day
                    End If
                End If
                Dim effDayChangeWarning As String = $"EffDayChangeWarning('{Me.txtEftDeductionDate.ClientID}', '{eftDayValue}');"
                Me.VRScript.CreateJSBinding(Me.txtEftDeductionDate, ctlPageStartupScript.JsEventType.onchange, effDayChangeWarning)
            End If

            Me.VRScript.AddVariableLine("var eftAgreeCheckId = '" + Me.chkAgreeToEftTerms.ClientID + "';")
            Me.VRScript.AddVariableLine("var eftDeclineCheckId = '" + Me.chkDeclineEftTerms.ClientID + "';")
            Me.VRScript.CreateJSBinding(Me.chkAgreeToEftTerms, ctlPageStartupScript.JsEventType.onchange, "$(""#" + Me.chkDeclineEftTerms.ClientID + """).attr('checked', false);" + ddMethodScript)
            Me.VRScript.CreateJSBinding(Me.chkDeclineEftTerms, ctlPageStartupScript.JsEventType.onchange, "$(""#" + Me.ddPayPlan.ClientID + """).val(''); $(""#" + Me.chkAgreeToEftTerms.ClientID + """).attr('checked', false);" + ddMethodScript + "$(""#" + Me.chkDeclineEftTerms.ClientID + """).attr('checked', false);")

            Me.VRScript.AddScriptLine(ddMethodScript) ' run at startup

            Me.VRScript.CreateJSBinding(Me.ddBillTo, ctlPageStartupScript.JsEventType.onchange, "if ($(""#" + Me.ddBillTo.ClientID + """).val() == '" + billTo_Other_id + "'){$(""#" + Me.divBillingInfoAddress.ClientID + """).show();} else {$(""#" + Me.divBillingInfoAddress.ClientID + """).hide();} ")
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddMethod.Items.Count = 0 Then
            If Me.Quote IsNot Nothing Then
                ' Updated G:\ClassFiles\DiamondStaticData.xml to include BOP items for BillingPayPlanID and BillToId - 5/1/2017 CH and MA
                ' Updated G:\ClassFiles\DiamondStaticData.xml to include BOP items for BillingPayPlanID and BillToId - 7-19-17 MGB
                QQHelper.LoadStaticDataOptionsDropDown(Me.ddMethod, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BillMethodId, SortBy.None, Me.Quote.LobType)
                '9/29/2021 note: would need to be updated in order to start loading payplans based on effDate (would rely on VR_Default_PayPlanIds and VR_ConvertPayPlanIdsIfNeeded config keys until then)
                QQHelper.LoadStaticDataOptionsDropDown(Me.ddPayPlan, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BillingPayPlanId, SortBy.None, Me.Quote.LobType)
                QQHelper.LoadStaticDataOptionsDropDown(Me.ddBillTo, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BillToId, SortBy.None, Me.Quote.LobType)
                QQHelper.LoadStaticDataOptionsDropDown(Me.ddState, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
            End If

            ' CAH 8/10/2022 - Added to PPA in DSD.  We need to remove it here if PPA Shouldn't have it.
            If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal OrElse IsCommercialQuote()) AndAlso IFM.VR.Common.Helpers.PPA.RccOptionHelper.IsRccOptionAvailable(Quote) = False Then
                ddPayPlan.Items.Remove(New ListItem("RENEWAL CREDIT CARD MONTHLY", "18"))
            End If

            ' clean up the text of selections
            For Each li As ListItem In Me.ddPayPlan.Items
                li.Text = li.Text.Replace("2", "").Trim()
                li.Text = li.Text.Replace("RENEWAL", "").Trim() '12-2-14 Matt A
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                    li.Text = li.Text.Replace("RENEWAL", "").Trim()
                End If
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    If li.Text = "ANNUAL MTG" Then
                        li.Enabled = False
                    End If
                End If
                ' *** CAH 08/10/2022 - Business wants all lines to use the below RCC text
                If li.Text = "CREDIT CARD MONTHLY" Then li.Text = "RECURRING CREDIT CARD"
                ' For BOP, CAP, WCP, CGL: change "CREDIT CARD MONTHLY" to "RECURRING CREDIT CARD"
                'If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                '    If li.Text = "CREDIT CARD MONTHLY" Then li.Text = "RECURRING CREDIT CARD"
                'End If

            Next

            For Each li As ListItem In Me.ddBillTo.Items
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    If li.Text = "MORTGAGEE" Then
                        li.Enabled = False
                    End If
                End If
            Next
            'Added 8/20/2019 for Auto & Home Endorsements Tasks 32771 & 39096 MLW
            'If (Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly) AndAlso (Me.Quote.BillingPayPlanId = "18") Then
            '    Me.ddPayPlan.Items.Add(New ListItem("RECURRING CREDIT CARD", "18"))
            'End If
            'updated 9/29/2021... just in case the call is updated to return different ids at some point; still has fall-back logic in case; note: since this is for Endorsements/ReadOnly, should we be looking at CurrentPayPlanId instead?
            If Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly AndAlso QQHelper.IsPositiveIntegerString(Me.Quote.BillingPayPlanId) = True Then
                Dim rccOptions As List(Of QuickQuoteStaticDataOption) = QQHelper.StaticDataOptionsForPayPlanType(PayPlanType.RccMonthly)
                Dim rccIds As List(Of Integer) = QQHelper.BillingPayPlanIdsForStaticDataOptions(rccOptions)
                If rccIds Is Nothing Then
                    rccIds = New List(Of Integer)
                End If
                If rccIds.Count = 0 Then
                    rccIds.Add(18)
                End If
                If rccIds.Contains(CInt(Me.Quote.BillingPayPlanId)) = True Then
                    Dim rccIdToAdd As Integer = 18 'default for now

                    '1st make sure rcc option is not already in ddl; also find 1st positive value in ddl
                    Dim alreadyInList As Boolean = False
                    Dim liFirstValid As ListItem = Nothing
                    If Me.ddPayPlan IsNot Nothing AndAlso Me.ddPayPlan.Items IsNot Nothing AndAlso Me.ddPayPlan.Items.Count > 0 Then
                        For Each li As ListItem In Me.ddPayPlan.Items
                            If li IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(li.Value) = True Then
                                If liFirstValid Is Nothing Then
                                    liFirstValid = li
                                End If
                                If rccIds.Contains(CInt(li.Value)) = True Then
                                    alreadyInList = True
                                    Exit For
                                End If
                            End If
                        Next
                    End If

                    If alreadyInList = False Then
                        'find 1st positive value in ddl
                        If rccOptions IsNot Nothing AndAlso rccOptions.Count > 0 AndAlso liFirstValid IsNot Nothing Then
                            'get option to find date
                            Dim sdo As QuickQuoteStaticDataOption = QQHelper.GetStaticDataOptionForTextOrValue(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BillingPayPlanId, StaticDataOptionSearchType.ByValue, liFirstValid.Value)
                            If sdo IsNot Nothing AndAlso sdo.MiscellaneousAttributes IsNot Nothing AndAlso sdo.MiscellaneousAttributes.Count > 0 Then
                                Dim sdaStartDate As QuickQuoteStaticDataAttribute = QQHelper.StaticDataAttributeForName(sdo.MiscellaneousAttributes, "startDate")
                                Dim sdaEndDate As QuickQuoteStaticDataAttribute = QQHelper.StaticDataAttributeForName(sdo.MiscellaneousAttributes, "endDate")
                                If sdaStartDate IsNot Nothing AndAlso sdaEndDate IsNot Nothing AndAlso QQHelper.IsValidDateString(sdaStartDate.nvp_value, mustBeGreaterThanDefaultDate:=True) = True AndAlso QQHelper.IsValidDateString(sdaEndDate.nvp_value, mustBeGreaterThanDefaultDate:=True) = True Then
                                    'now find rcc option w/ same date
                                    For Each rccO As QuickQuoteStaticDataOption In rccOptions
                                        If rccO IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(rccO.Value) = True AndAlso sdo.MiscellaneousAttributes IsNot Nothing AndAlso sdo.MiscellaneousAttributes.Count > 0 Then
                                            Dim sdaRccStartDate As QuickQuoteStaticDataAttribute = QQHelper.StaticDataAttributeForName(rccO.MiscellaneousAttributes, "startDate")
                                            Dim sdaRccEndDate As QuickQuoteStaticDataAttribute = QQHelper.StaticDataAttributeForName(rccO.MiscellaneousAttributes, "endDate")
                                            If sdaRccStartDate IsNot Nothing AndAlso sdaRccEndDate IsNot Nothing AndAlso QQHelper.IsValidDateString(sdaRccStartDate.nvp_value, mustBeGreaterThanDefaultDate:=True) = True AndAlso QQHelper.IsValidDateString(sdaRccEndDate.nvp_value, mustBeGreaterThanDefaultDate:=True) = True Then
                                                If sdaRccStartDate.nvp_value = sdaStartDate.nvp_value AndAlso sdaRccEndDate.nvp_value = sdaEndDate.nvp_value Then
                                                    rccIdToAdd = CInt(rccO.Value)
                                                    Exit For
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                            End If
                        End If

                        Me.ddPayPlan.Items.Add(New ListItem("RECURRING CREDIT CARD", rccIdToAdd.ToString))
                    End If
                End If
            End If
        End If
    End Sub

    Public Overrides Sub Populate()
        'Added 7/25/2019 for Auto, Home, DFR Endorsements Tasks 32771, 39096, 40277 (09/17/2019), 40272 (09/17/2019) MLW
        If (IsOnAppPage OrElse Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly OrElse Me.Visible) Then
            If Me.Quote IsNot Nothing Then
                LoadStaticData()
                Me.ddMethod.SetFromValue(Me.Quote.BillMethodId) 'defaulted by Don on back end - 8-18-14

                'Updated 9/25/2019 for bug 40515 MLW
                'Added 7/24/2019 for Auto & Home Endorsements Tasks 32771 & 39096 MLW
                If (Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly) Then
                    Me.ddMethod.Enabled = False
                    '9/29/2021 note: would need to be updated if we ever start loading payplans based on effDate (would rely on VR_Default_PayPlanIds and VR_ConvertPayPlanIdsIfNeeded config keys until then)
                    If Me.Quote.BillMethodId <> "2" Then ' agency billed
                        'convert
                        Select Case Me.Quote.CurrentPayplanId
                            Case "20"
                                Me.ddPayPlan.SetFromValue("12")
                            Case "21"
                                Me.ddPayPlan.SetFromValue("13")
                            Case "22"
                                Me.ddPayPlan.SetFromValue("14")
                        End Select
                    Else
                        'direct bill
                        Me.ddPayPlan.SetFromValue(Me.Quote.CurrentPayplanId) 'defaulted by Don on back end - 8-18-14
                    End If

                    'Added 8/20/2019 for Auto & Home Endorsements Tasks 32771 & 39096 MLW
                    'If Me.Quote.BillingPayPlanId = "18" Then
                    'updated 9/24/2021 to handle for all RCC payplans (text should be "Credit Card Monthly" or "Renewal Credit Card Monthly"); note: would also include "Account Bill Credit Card Monthly"
                    Dim billingPayPlanTxt As String = ""
                    If QQHelper.IsPositiveIntegerString(Me.Quote.BillingPayPlanId) = True Then
                        billingPayPlanTxt = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, Me.Quote.BillingPayPlanId)
                    End If
                    If String.IsNullOrWhiteSpace(billingPayPlanTxt) = False AndAlso UCase(billingPayPlanTxt).Contains("CREDIT CARD") = True AndAlso IFM.VR.Common.Helpers.PPA.RccOptionHelper.IsRccOptionAvailable(Quote) = False Then
                        Me.ddPayPlan.Enabled = False
                        Me.ddPayPlan.SetFromValue(Me.Quote.BillingPayPlanId) 'do not allow switching from Policy RCC
                    End If

                    Me.ddBillTo.SetFromValue(Me.Quote.BillToId) 'defaulted by Don on back end - 8-18-14
                Else
                    '9/29/2021 note: would need to be updated if we ever start loading payplans based on effDate (would rely on VR_Default_PayPlanIds and VR_ConvertPayPlanIdsIfNeeded config keys until then)
                    If Me.Quote.BillMethodId <> "2" Then ' agency billed
                        'convert
                        Select Case Me.Quote.BillingPayPlanId
                            Case "20"
                                Me.ddPayPlan.SetFromValue("12")
                            Case "21"
                                Me.ddPayPlan.SetFromValue("13")
                            Case "22"
                                Me.ddPayPlan.SetFromValue("14")
                        End Select
                    Else
                        'direct bill
                        Me.ddPayPlan.SetFromValue(Me.Quote.BillingPayPlanId) 'defaulted by Don on back end - 8-18-14
                    End If

                    Me.ddBillTo.SetFromValue(Me.Quote.BillToId) 'defaulted by Don on back end - 8-18-14

                    PopulateAccountBillAvailableText()
                End If

                Dim billTo_Other_id As String = Me.QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BillToId, "Other", Me.Quote.LobType)
                If Me.Quote.BillToId <> billTo_Other_id Then
                    Me.divBillingInfoAddress.Attributes.Add("style", "display:none")
                End If

                'If Me.Quote.CurrentPayplanId <> "19" Then
                'updated 9/24/2021 to handle for all EFT payplans (text should be "EFT Monthly" or "Renewal EFT Monthly"); note: would also include "Account Bill EFT Monthly"; note2: not sure why we're checking CurrentPayplanId here but BillingPayPlanId above
                Dim currentPayPlanTxt As String = ""
                If QQHelper.IsPositiveIntegerString(Me.Quote.CurrentPayplanId) = True Then
                    currentPayPlanTxt = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.CurrentPayplanId, Me.Quote.CurrentPayplanId)
                End If
                Dim eftDateValue As DateTime
                Dim eftDayValue As Integer
                If String.IsNullOrWhiteSpace(Quote.EFT_DeductionDay) = True OrElse Quote.EFT_DeductionDay = "0" Then
                    If DateTime.TryParse(Quote.EffectiveDate, eftDateValue) Then
                        eftDayValue = eftDateValue.Day
                    End If
                Else
                    eftDayValue = Quote.EFT_DeductionDay
                End If

                Me.txtEftDeductionDate.Text = eftDayValue

                If String.IsNullOrWhiteSpace(currentPayPlanTxt) = True OrElse UCase(currentPayPlanTxt).Contains("EFT MONTHLY") = False Then
                    Me.txtEFTRouting.Text = ""
                    Me.txtEftAccount.Text = ""
                    Me.ddEftAcountType.SetFromValue("")
                    'Me.txtEftDeductionDate.Text = ""
                    Me.divEFTInfo.Attributes.Add("style", "display:none;")
                Else
                    Me.txtEFTRouting.Text = Me.Quote.EFT_BankRoutingNumber
                    Me.txtEftAccount.Text = Me.Quote.EFT_BankAccountNumber
                    Me.ddEftAcountType.SetFromValue(Me.Quote.EFT_BankAccountTypeId)
                    Me.txtEftDeductionDate.Text = eftDayValue

                    Dim isAgreeToEFTTermsChecked = QQDevDictionary_GetItem(AgreeToEFTTerms)
                    'If (String.IsNullOrWhiteSpace(Me.txtEFTRouting.Text) = False Or String.IsNullOrWhiteSpace(Me.txtEftAccount.Text) = False) Then
                    If isAgreeToEFTTermsChecked IsNot Nothing AndAlso isAgreeToEFTTermsChecked = "True" Then
                        Me.chkAgreeToEftTerms.Checked = True
                    Else
                        Me.chkAgreeToEftTerms.Checked = False
                    End If
                End If

                '  Go ahead and try to load no matter the bill to -   populate name and address info
                If Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal AndAlso Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                    ' DFR and CAP default to blank address
                    If Me.Quote.BillingAddressee IsNot Nothing Then
                        If Me.Quote.BillingAddressee.Name IsNot Nothing Then
                            Me.txtFirstName.Text = Me.Quote.BillingAddressee.Name.FirstName
                            Me.txtMiddleName.Text = Me.Quote.BillingAddressee.Name.MiddleName
                            Me.txtLastName.Text = Me.Quote.BillingAddressee.Name.LastName
                        End If
                        If Me.Quote.BillingAddressee.Address IsNot Nothing Then
                            Me.txtStreetNum.Text = Me.Quote.BillingAddressee.Address.HouseNum
                            Me.txtStreet.Text = Me.Quote.BillingAddressee.Address.StreetName
                            Me.txtAptSuiteNum.Text = Me.Quote.BillingAddressee.Address.ApartmentNumber
                            Me.txtPOBOX.Text = Me.Quote.BillingAddressee.Address.POBox
                            Me.txtCity.Text = Me.Quote.BillingAddressee.Address.City
                            Me.ddState.SetFromValue(Me.Quote.BillingAddressee.Address.StateId)
                            Me.txtZipCode.Text = Me.Quote.BillingAddressee.Address.Zip.RemoveAny("00000", "00000-0000")
                        End If
                    End If
                End If

                If IFM.VR.Common.Helpers.PPA.RccOptionHelper.IsRccOptionAvailable(Quote) Then
                    If IsQuoteEndorsement() Then
                        Dim hadRCC As Boolean
                        Me.divRccReminderText.InnerText = "Please be advised for Recurring Credit Card pay plan you will need to provide credit card information once your change is submitted."
                        Dim dictionaryVal As String = Quote.GetDevDictionaryItem("", "PpaPayplanOnPreviousImage")

                        If String.IsNullOrWhiteSpace(dictionaryVal) = False Then
                            hadRCC = (dictionaryVal = "18") 'RCC is 18
                        Else
                            hadRCC = (Quote.BillingPayPlanId = "18")
                        End If
                        If hadRcc Then
                            Me.divRccReminderText.Visible = False
                        End If
                        Me.divRccPayPlanText.Visible = False
                    Else
                        Me.divRccReminderText.InnerText = "Please be advised for the Recurring Credit Card pay plan you will need to submit credit card information once the quote has been successfully rated and issued."
                    End If
                End If

                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso IsQuoteEndorsement() AndAlso IsBillingUpdate() = False Then
                    Me.ddMethod.Enabled = False
                    Me.ddPayPlan.Enabled = False
                    Me.ddBillTo.Enabled = False
                    If Me.Quote.BillingPayPlanId = "19" Then
                        Me.chkAgreeToEftTerms.Enabled = False
                        Me.chkDeclineEftTerms.Enabled = False
                        Me.txtEFTRouting.Enabled = False
                        Me.txtEftAccount.Enabled = False
                        Me.ddEftAcountType.Enabled = False
                        Me.txtEftDeductionDate.Enabled = False
                    End If
                End If

            End If
        End If
    End Sub

    Private Sub PopulateAccountBillAvailableText()
        If IsCommercialQuote() AndAlso Quote.BillingPayPlanId.EqualsAny("15", "19") Then
            Me.divAccountBillAvailText.Attributes.Add("style", "display:'';")
        Else
            Me.divAccountBillAvailText.Attributes.Add("style", "display:none;")
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Added 7/25/2019 for Auto & Home Endorsements Tasks 32771 & 39096 MLW
        If (IsOnAppPage OrElse Me.Visible) Then
            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = "Billing Information"

            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            Dim accordListPlusEft As New List(Of VRAccordionTogglePair)
            accordListPlusEft.AddRange(accordList)
            accordListPlusEft.Add(New VRAccordionTogglePair(Me.divEFTInfo.ClientID, "0"))

            Dim accordListPlusAdress As New List(Of VRAccordionTogglePair)
            accordListPlusAdress.AddRange(accordList)
            accordListPlusAdress.Add(New VRAccordionTogglePair(Me.divBillingInfoAddress.ClientID, "0"))

            Dim isAgreeToEFTTermsChecked = QQDevDictionary_GetItem(AgreeToEFTTerms)

            Dim valItems = BillingInformationValidator.BillingInformationValidation(Me.Quote, valArgs.ValidationType, isAgreeToEFTTermsChecked)
            If valItems.Any() Then

                For Each v In valItems
                    Select Case v.FieldId
                        Case BillingInformationValidator.Billto_Method_Combination
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddBillTo, v, accordList)
                        Case BillingInformationValidator.Billto
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddBillTo, v, accordList)
                        Case BillingInformationValidator.Method
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddMethod, v, accordList)
                        Case BillingInformationValidator.PayPlan
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddPayPlan, v, accordList)

                        Case BillingInformationValidator.BillingFirstName
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstName, v, accordListPlusAdress)
                        Case BillingInformationValidator.BillingLastName
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLastName, v, accordListPlusAdress)
                        Case BillingInformationValidator.BillingAddressStreetAndPoBoxEmpty
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordListPlusAdress)
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreet, v, accordListPlusAdress)
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBOX, v, accordListPlusAdress)
                        Case BillingInformationValidator.BillingAddressStreetAndPoBoxAreSet
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordListPlusAdress)
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreet, v, accordListPlusAdress)
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBOX, v, accordListPlusAdress)
                        Case BillingInformationValidator.BillingAddressStreetNumber
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordListPlusAdress)
                        Case BillingInformationValidator.BillingAddressStreetName
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreet, v, accordListPlusAdress)
                        Case BillingInformationValidator.BillingAddressZipCode
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtZipCode, v, accordListPlusAdress)
                        Case BillingInformationValidator.BillingAddressCity
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCity, v, accordListPlusAdress)
                        Case BillingInformationValidator.BillingAddressState
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddState, v, accordListPlusAdress)

                        Case BillingInformationValidator.AgreeToEFTTerms
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.eftTerms, v, accordListPlusEft)
                        Case BillingInformationValidator.BillingEftRouting
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtEFTRouting, v, accordListPlusEft)
                        Case BillingInformationValidator.BillingEftAccount
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtEftAccount, v, accordListPlusEft)
                        Case BillingInformationValidator.BillingEftAccountType
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddEftAcountType, v, accordListPlusEft)
                        Case BillingInformationValidator.BillingEftDeduction
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtEftDeductionDate, v, accordListPlusEft)

                    End Select
                Next
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        'Added 7/25/2019 for Auto & Home Endorsements Tasks 32771 & 39096 MLW
        If (IsOnAppPage OrElse Me.Visible) Then
            'Currently, the lienholder option for a vehicle is the only known offender of this kind of save.
            If Me.Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Me.ddMethod.SelectedValue) = False AndAlso String.IsNullOrWhiteSpace(Me.ddPayPlan.SelectedValue) = False AndAlso String.IsNullOrWhiteSpace(Me.ddBillTo.SelectedValue) = False Then
                Me.Quote.BillMethodId = Me.ddMethod.SelectedValue

                If Me.Quote.BillMethodId = "2" Then
                    'direct bill Ids
                    'Updated 09/24/2019 for bug 40515 MLW
                    If (Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly) Then
                        'Added 8/20/2019 for Auto & Home Endorsements Tasks 32771 & 39096 MLW
                        'If Me.Quote.BillingPayPlanId = "18" Then
                        'updated 9/24/2021 to handle for all RCC payplans (text should be "Credit Card Monthly" or "Renewal Credit Card Monthly"); note: would also include "Account Bill Credit Card Monthly"
                        Dim billingPayPlanTxt As String = ""
                        If QQHelper.IsPositiveIntegerString(Me.Quote.BillingPayPlanId) = True Then
                            billingPayPlanTxt = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, Me.Quote.BillingPayPlanId)
                        End If
                        If RccOptionHelper.IsRccOptionAvailable(Quote) = False AndAlso (String.IsNullOrWhiteSpace(billingPayPlanTxt) = False AndAlso UCase(billingPayPlanTxt).Contains("CREDIT CARD") = True) Then
                            'do not change an rcc pay plan value
                            'Updated 09/26/2019 for bug 40515
                            Me.Quote.CurrentPayplanId = Me.Quote.BillingPayPlanId
                        Else
                            Me.Quote.OnlyUsePropertyToSetFieldWithSameName = True
                            Me.Quote.CurrentPayplanId = Me.ddPayPlan.SelectedValue
                        End If
                    Else
                        Me.Quote.BillingPayPlanId = Me.ddPayPlan.SelectedValue
                    End If
                Else
                    'agency bill has different ids for the same payplans - why? you got me??
                    'Updated 09/24/2019 for bug 40515 MLW
                    '9/29/2021 note: would need to be updated if we ever start loading payplans based on effDate (would rely on VR_Default_PayPlanIds and VR_ConvertPayPlanIdsIfNeeded config keys until then)
                    If (Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly) Then
                        Me.Quote.OnlyUsePropertyToSetFieldWithSameName = True
                        Select Case Me.ddPayPlan.SelectedValue
                            Case "12"
                                Me.Quote.CurrentPayplanId = "20"
                            Case "13"
                                Me.Quote.CurrentPayplanId = "21"
                            Case "14"
                                Me.Quote.CurrentPayplanId = "22"
                        End Select
                        'Me.Quote.OnlyUsePropertyToSetFieldWithSameName = False
                    Else
                        Select Case Me.ddPayPlan.SelectedValue
                            Case "12"
                                Me.Quote.BillingPayPlanId = "20"
                            Case "13"
                                Me.Quote.BillingPayPlanId = "21"
                            Case "14"
                                Me.Quote.BillingPayPlanId = "22"
                        End Select
                    End If
                End If

                'added 9/29/2021... just in case the call is updated to return different ids at some point; still has fall-back logic in case
                Dim hasEftSelected As Boolean = False
                If QQHelper.IsPositiveIntegerString(Me.ddPayPlan.SelectedValue) = True Then
                    Dim eftIds As List(Of Integer) = QQHelper.BillingPayPlanIdsForPayPlanType(PayPlanType.EftMonthly)
                    If eftIds Is Nothing Then
                        eftIds = New List(Of Integer)
                    End If
                    If eftIds.Count = 0 Then
                        eftIds.Add(19)
                    End If
                    If eftIds.Contains(CInt(Me.ddPayPlan.SelectedValue)) = True Then
                        hasEftSelected = True
                    End If
                End If

                'If Me.ddPayPlan.SelectedValue = "19" Then
                'updated 9/29/2021
                If hasEftSelected = True Then
                    Me.Quote.EFT_BankRoutingNumber = Me.txtEFTRouting.Text.Trim()
                    Me.Quote.EFT_BankAccountNumber = Me.txtEftAccount.Text.Trim()
                    Me.Quote.EFT_BankAccountTypeId = Me.ddEftAcountType.SelectedValue
                    Me.Quote.EFT_DeductionDay = Me.txtEftDeductionDate.Text.Trim()
                Else
                    Me.Quote.EFT_BankRoutingNumber = ""
                    Me.Quote.EFT_BankAccountNumber = ""
                    Me.Quote.EFT_BankAccountTypeId = ""
                    Me.Quote.EFT_DeductionDay = ""
                    chkAgreeToEftTerms.Checked = False
                End If
                QQDevDictionary_SetItem(AgreeToEFTTerms, chkAgreeToEftTerms.Checked)

                'commented 9/29/2021 since it's not being used
                'If Me.ddPayPlan.SelectedValue = "18" Then ' RCC

                'End If

                'Updated 09/24/2019 for bug 40515 MLW
                If (Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly) Then
                    Me.Quote.OnlyUsePropertyToSetFieldWithSameName = True
                    Me.Quote.BillToId = Me.ddBillTo.SelectedValue 'Should be BillToId and not CurrentBillToId, naming conventions are mixed up - MLW
                Else
                    Me.Quote.BillToId = Me.ddBillTo.SelectedValue
                End If

                'Dim qqHelper As New QuickQuoteHelperClass 'removed 9/24/2021 since we already have a property for it
                Dim billTo_Other_id As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BillToId, "Other", Me.Quote.LobType)
                If Me.ddBillTo.SelectedValue = billTo_Other_id Then
                    'save name and address info
                    If Me.Quote.BillingAddressee Is Nothing Then
                        Me.Quote.BillingAddressee = New QuickQuoteBillingAddressee()
                    End If
                    If Me.Quote.BillingAddressee.Name Is Nothing Then
                        Me.Quote.BillingAddressee.Name = New QuickQuoteName()
                    End If

                    If Me.Quote.BillingAddressee.Address Is Nothing Then
                        Me.Quote.BillingAddressee.Address = New QuickQuoteAddress()
                    End If

                    Me.Quote.BillingAddressee.Name.FirstName = Me.txtFirstName.Text.ToUpper().Trim()
                    Me.Quote.BillingAddressee.Name.MiddleName = Me.txtMiddleName.Text.ToUpper().Trim()
                    Me.Quote.BillingAddressee.Name.LastName = Me.txtLastName.Text.ToUpper().Trim()

                    Me.Quote.BillingAddressee.Address.HouseNum = Me.txtStreetNum.Text.Trim()
                    Me.Quote.BillingAddressee.Address.StreetName = Me.txtStreet.Text.ToUpper().Trim()
                    Me.Quote.BillingAddressee.Address.ApartmentNumber = Me.txtAptSuiteNum.Text.ToUpper().Trim()
                    Me.Quote.BillingAddressee.Address.POBox = Me.txtPOBOX.Text.ToUpper().Trim()
                    Me.Quote.BillingAddressee.Address.City = Me.txtCity.Text.ToUpper().Trim()
                    Me.Quote.BillingAddressee.Address.StateId = Me.ddState.SelectedValue
                    Me.Quote.BillingAddressee.Address.Zip = Me.txtZipCode.Text.ToUpper().Trim()
                Else
                    ' clear address info
                    Me.Quote.BillingAddressee = Nothing
                End If
            End If
            PopulateAccountBillAvailableText()
        End If

        Return True
    End Function

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Public Overrides Sub ClearControl()
        Me.txtAptSuiteNum.Text = ""
        Me.txtCity.Text = ""
        Me.txtFirstName.Text = ""
        Me.txtLastName.Text = ""
        Me.txtMiddleName.Text = ""
        Me.txtPOBOX.Text = ""
        Me.txtStreet.Text = ""
        Me.txtStreetNum.Text = ""
        Me.txtZipCode.Text = ""
        Me.ddBillTo.SelectedIndex = -1
        Me.ddMethod.SelectedIndex = -1
        Me.ddPayPlan.SelectedIndex = -1
        Me.ddState.SelectedIndex = -1
        MyBase.ClearControl()
    End Sub

End Class