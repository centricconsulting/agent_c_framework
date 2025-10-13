Imports System.ServiceProcess
Imports Diamond.Business.ThirdParty.GeneralLedger.NetsuiteService
Imports Diamond.Common.Enums
Imports Diamond.Common.Objects.Claims.NotifyUnderwriting
Imports IFM.VR.Web.Helpers
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonObjects.QuickQuoteObject

Public Class ctlCommercialDataPrefillEntry
    Inherits VRControlBase

    Public Enum PrefillControlLastAction
        None = 0
        Populate = 1
        AddLocation = 2
        DeleteLocation = 3
        Save = 4
        Cancel = 5
        CopyPolicyholder = 6
        Validate = 7
        ReloadLocations = 8
    End Enum
    Dim lastAction As PrefillControlLastAction = PrefillControlLastAction.None
    Public Property CommercialDataPrefillPolicyId As Integer
        Get
            Dim pId As Integer = 0
            If ViewState("CommercialDataPrefillPolicyId") IsNot Nothing Then
                pId = CType(ViewState("CommercialDataPrefillPolicyId"), Integer)
            End If
            Return pId
        End Get
        Set(value As Integer)
            ViewState("CommercialDataPrefillPolicyId") = value
        End Set
    End Property
    Public Property CommercialDataPrefillPolicyImageNum As Integer
        Get
            Dim pImgNum As Integer = 0
            If ViewState("CommercialDataPrefillPolicyImageNum") IsNot Nothing Then
                pImgNum = CType(ViewState("CommercialDataPrefillPolicyImageNum"), Integer)
            End If
            Return pImgNum
        End Get
        Set(value As Integer)
            ViewState("CommercialDataPrefillPolicyImageNum") = value
        End Set
    End Property

    Public Overrides Sub LoadStaticData()
        'Throw New NotImplementedException()
        If Me.ddStateAbbrev IsNot Nothing AndAlso Me.ddStateAbbrev.Items IsNot Nothing AndAlso Me.ddStateAbbrev.Items.Count > 1 Then 'added to make sure it's only called when needed; original logic in ELSE
            'appears to have values
        Else
            'QQHelper.LoadStaticDataOptionsDropDown(Me.ddStateAbbrev, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddStateAbbrev, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId)

            'QQHelper.LoadStaticDataOptionsDropDown(Me.ddPhoneType, QuickQuoteClassName.QuickQuotePhone, QuickQuotePropertyName.TypeId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddPhoneType, QuickQuoteClassName.QuickQuotePhone, QuickQuotePropertyName.TypeId)

            'QQHelper.LoadStaticDataOptionsDropDown(Me.ddBusinessType, QuickQuoteClassName.QuickQuoteName, QuickQuotePropertyName.EntityTypeId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddBusinessType, QuickQuoteClassName.QuickQuoteName, QuickQuotePropertyName.EntityTypeId)
        End If
    End Sub

    Public Overrides Sub Populate()
        SetLastAction(PrefillControlLastAction.None)
        Me.Visible = False
        If IsPostBack = False AndAlso Me.Quote IsNot Nothing Then 'note: IsPostBack False check taken from ctl_Farm_Basic_Policy_Info
            If ShouldShow() = True Then
                SetLastAction(PrefillControlLastAction.Populate, shouldDefaultFocusId:=True)
                Me.Visible = True
                'Me.ShowPopup()
                LoadStaticData() 'added here to make sure it happens (since Populate can happen before LoadStaticData is called from Page_Load)
                With Me.Quote
                    Me.CommercialDataPrefillPolicyId = QQHelper.IntegerForString(.PolicyId)
                    Me.CommercialDataPrefillPolicyImageNum = QQHelper.IntegerForString(.PolicyImageNum)
                    Dim ih As New IntegrationHelper

                    Dim okayToCallFirm As Boolean = Not Me.HasAttemptedCommercialDataPrefillFirmographicsPreload
                    Dim okayToCallProp As Boolean = Not Me.HasAttemptedCommercialDataPrefillPropertyPreload
                    If okayToCallFirm = False OrElse okayToCallProp = False Then
                        'note: will just call preload now if needed so we know it will be called before the actual Prefill call and we don't have to wait until a button click
                        Dim attemptedServiceCall As Boolean = False
                        Dim attemptedServiceCallType As IntegrationHelper.CommercialDataPrefillServiceType = IntegrationHelper.CommercialDataPrefillServiceType.None
                        Dim caughtUnhandledException As Boolean = False
                        Dim unhandledExceptionToString As String = ""
                        Dim locNumsAttempted As List(Of Integer) = Nothing
                        If okayToCallProp = False Then
                            'just Firm
                            attemptedServiceCallType = IntegrationHelper.CommercialDataPrefillServiceType.FirmographicsOnly
                            ih.CallCommercialDataPrefill_FirmographicsOnly_Preload_IfNeeded(Me.Quote, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
                            Me.HasAttemptedCommercialDataPrefillFirmographicsPreload = True
                        ElseIf okayToCallFirm = False Then
                            'just Prop
                            attemptedServiceCallType = IntegrationHelper.CommercialDataPrefillServiceType.PropertyOnly
                            ih.CallCommercialDataPrefill_PropertyOnly_Preload_IfNeeded(Me.Quote, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, locNumsAttempted:=locNumsAttempted)
                            Me.HasAttemptedCommercialDataPrefillPropertyPreload = True
                        Else
                            'Both
                            ih.CallCommercialDataPrefill_Preload_IfNeeded(Me.Quote, attemptedServiceCall:=attemptedServiceCall, attemptedServiceCallType:=attemptedServiceCallType, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, locNumsAttempted:=locNumsAttempted)
                            Me.HasAttemptedCommercialDataPrefillFirmographicsPreload = True
                            Me.HasAttemptedCommercialDataPrefillPropertyPreload = True
                        End If
                        If caughtUnhandledException = True Then
                            Dim preloadError As New IFM.ErrLog_Parameters_Structure()
                            Dim preloadStackTrace As String = ""
                            If attemptedServiceCallType = IntegrationHelper.CommercialDataPrefillServiceType.FirmographicsOnly Then
                                preloadStackTrace = "at IntegrationHelper.CallCommercialDataPrefill_FirmographicsOnly_Preload at IntegrationHelper.CallCommercialDataPrefill_FirmographicsOnly_Preload_IfNeeded"
                            ElseIf attemptedServiceCallType = IntegrationHelper.CommercialDataPrefillServiceType.PropertyOnly Then
                                preloadStackTrace = "at IntegrationHelper.CallCommercialDataPrefill_PropertyOnly_Preload at IntegrationHelper.CallCommercialDataPrefill_PropertyOnly_Preload_IfNeeded"
                            Else
                                preloadStackTrace = "at IntegrationHelper.CallCommercialDataPrefill_Preload at IntegrationHelper.CallCommercialDataPrefill_Preload_IfNeeded"
                            End If
                            Dim addPreloadInfo As String = WebHelper_Personal.AdditionalInfoTextForCommercialDataPrefillError(Me.Quote, qqHelper:=QQHelper)
                            If locNumsAttempted IsNot Nothing AndAlso locNumsAttempted.Count > 0 Then
                                addPreloadInfo = QQHelper.appendText(addPreloadInfo, "locNumsAttempted: " & QuickQuoteHelperClass.StringForListOfInteger(locNumsAttempted, delimiter:=","), splitter:="; ")
                            End If
                            With preloadError
                                .ApplicationName = "Velocirater Personal"
                                .ClassName = "ctlCommercialDataPrefillEntry"
                                .ErrorMessage = unhandledExceptionToString
                                .LogDate = DateTime.Now
                                .RoutineName = "Populate"
                                .StackTrace = preloadStackTrace
                                .AdditionalInfo = addPreloadInfo
                            End With
                            WriteErrorLogRecord(preloadError, "")
                        End If
                    End If

                    Dim orderInfo As String = ""
                    Dim hasPrefill As Boolean = ih.HasPolicyCommercialDataPrefillOrder(Me.Quote, orderInformation:=orderInfo)
                    Me.hdnHasPrefill.Value = hasPrefill.ToString
                    Me.hdnPrefillOrderInfo.Value = orderInfo
                    'below flag no longer needs to be set w/ introduction of Preload call above
                    'If hasPrefill = False Then
                    '    Me.hdnHasChangedSinceLastCheck.Value = IntegrationHelper.PolicyHolderHasNameAndAddressData(.Policyholder, mustHaveAllRequiredFields:=True).ToString
                    'Else
                    '    'see if current info is valid and different than existing order info
                    '    Me.hdnHasChangedSinceLastCheck.Value = IntegrationHelper.StringIsDifferentThanFirmographicsPrefillNameAddressString(orderInfo, .Policyholder, mustHaveAllRequiredFields:=True).ToString
                    'End If

                    Dim hasClientId As Boolean = False
                    If .Client IsNot Nothing AndAlso .Client.HasValidClientId() = True Then
                        hasClientId = True
                    End If
                    If hasClientId = True Then
                        Me.txtClientId.Text = .Client.ClientId
                        IFM.VR.Common.QuoteSave.QuoteSaveHelpers.AddToClientIdsInSessionFromPopulate(QQHelper.IntegerForString(.Client.ClientId))
                    Else
                        Me.txtClientId.Text = ""
                        IFM.VR.Common.QuoteSave.QuoteSaveHelpers.AddToQuoteIdsInSessionWithoutClientId(QQHelper.IntegerForString(Me.QuoteId))
                    End If
                    Me.lblGovStateId.Text = .StateId
                    Dim addressStateId As String = ""
                    Dim hasAddressData As Boolean = False
                    If .Policyholder IsNot Nothing Then
                        With .Policyholder
                            If .Name IsNot Nothing Then
                                With .Name
                                    Me.txtBusinessName.Text = .CommercialName1
                                    Me.txtDBA.Text = .DoingBusinessAsName
                                    If QQHelper.IsPositiveIntegerString(.EntityTypeId) = True AndAlso Me.ddBusinessType.Items IsNot Nothing AndAlso Me.ddBusinessType.Items.Count > 0 AndAlso Me.ddBusinessType.Items.FindByValue(.EntityTypeId) IsNot Nothing Then
                                        Me.ddBusinessType.SelectedValue = .EntityTypeId
                                    End If
                                    Me.txtOtherEntity.Text = .OtherLegalEntityDescription
                                    If QQHelper.IsPositiveIntegerString(.TaxTypeId) = True AndAlso Me.ddTaxIDType.Items IsNot Nothing AndAlso Me.ddTaxIDType.Items.Count > 0 AndAlso Me.ddTaxIDType.Items.FindByValue(.TaxTypeId) IsNot Nothing Then
                                        Me.ddTaxIDType.SelectedValue = .TaxTypeId
                                    End If
                                    Me.txtFEIN.Text = .TaxNumber
                                    Me.txtDescriptionOfOperations.Text = .DescriptionOfOperations
                                    Me.txtBusinessStarted.Text = .DateBusinessStarted
                                    Me.txtYearsOfExperience.Text = .YearsOfExperience
                                End With
                            End If
                            If .Emails IsNot Nothing AndAlso .Emails.Count > 0 AndAlso .Emails(0) IsNot Nothing Then
                                Me.txtEmail.Text = .Emails(0).Address
                                Me.hdnEmailTypeId.Value = .Emails(0).TypeId
                            End If
                            If .Phones IsNot Nothing AndAlso .Phones.Count > 0 AndAlso .Phones(0) IsNot Nothing Then
                                Me.txtPhone.Text = .Phones(0).Number
                                Me.txtPhoneExt.Text = .Phones(0).Extension
                                If QQHelper.IsPositiveIntegerString(.Phones(0).TypeId) = True AndAlso Me.ddPhoneType.Items IsNot Nothing AndAlso Me.ddPhoneType.Items.Count > 0 AndAlso Me.ddPhoneType.Items.FindByValue(.Phones(0).TypeId) IsNot Nothing Then
                                    Me.ddPhoneType.SelectedValue = .Phones(0).TypeId
                                End If
                            End If
                            If .Address IsNot Nothing Then ' AndAlso .Address.HasData = True Then 'note: also checking HasData would prevent us from loading IN as the default state since that's what QQ defaults the Address to; ctlInsured works the same way, so I guess it's okay here
                                With .Address
                                    Me.txtStreetNum.Text = .HouseNum
                                    Me.txtStreetName.Text = .StreetName
                                    Me.txtAptNum.Text = .ApartmentNumber
                                    Me.txtPOBox.Text = .POBox
                                    Me.txtZipCode.Text = .Zip
                                    Me.txtCityName.Text = .City
                                    Me.txtGaragedCounty.Text = .County
                                    'If QQHelper.IsPositiveIntegerString(.StateId) = True AndAlso Me.ddStateAbbrev.Items IsNot Nothing AndAlso Me.ddStateAbbrev.Items.Count > 0 AndAlso Me.ddStateAbbrev.Items.FindByValue(.StateId) IsNot Nothing Then
                                    '    Me.ddStateAbbrev.SelectedValue = .StateId
                                    'End If
                                    addressStateId = .StateId
                                    hasAddressData = .HasData
                                    Me.txtGaragedCounty.Text = .County
                                    Me.txtOther.Text = .Other
                                End With
                            End If
                        End With
                    End If
                    If Me.ddStateAbbrev.Items IsNot Nothing AndAlso Me.ddStateAbbrev.Items.Count > 0 Then
                        Dim hasAddressIdInList As Boolean = False
                        If QQHelper.IsPositiveIntegerString(addressStateId) = True AndAlso Me.ddStateAbbrev.Items.FindByValue(addressStateId) IsNot Nothing Then
                            hasAddressIdInList = True
                        End If
                        If hasAddressData = True AndAlso hasAddressIdInList = True Then
                            Me.ddStateAbbrev.SelectedValue = addressStateId
                        ElseIf QQHelper.IsPositiveIntegerString(Me.lblGovStateId.Text) = True AndAlso Me.ddStateAbbrev.Items.FindByValue(Me.lblGovStateId.Text) IsNot Nothing Then
                            Me.ddStateAbbrev.SelectedValue = Me.lblGovStateId.Text
                        ElseIf hasAddressIdInList = True Then
                            Me.ddStateAbbrev.SelectedValue = addressStateId
                        End If
                    End If
                    If .Locations IsNot Nothing AndAlso .Locations.Count > 0 Then
                        Dim dt As New DataTable
                        dt.Columns.Add("locNum", System.Type.GetType("System.String"))
                        dt.Columns.Add("isPreExisting", System.Type.GetType("System.String"))
                        dt.Columns.Add("streetNum", System.Type.GetType("System.String"))
                        dt.Columns.Add("streetName", System.Type.GetType("System.String"))
                        dt.Columns.Add("aptNum", System.Type.GetType("System.String"))
                        'dt.Columns.Add("poBox", System.Type.GetType("System.String"))
                        dt.Columns.Add("zipCode", System.Type.GetType("System.String"))
                        dt.Columns.Add("city", System.Type.GetType("System.String"))
                        dt.Columns.Add("state", System.Type.GetType("System.String"))
                        dt.Columns.Add("county", System.Type.GetType("System.String"))
                        dt.Columns.Add("accordionFlag", System.Type.GetType("System.String"))
                        dt.Columns.Add("hasPrefill", System.Type.GetType("System.String"))
                        dt.Columns.Add("prefillOrderInfo", System.Type.GetType("System.String"))
                        dt.Columns.Add("hasChangedSinceLoad", System.Type.GetType("System.String"))
                        dt.Columns.Add("hasChangedSinceLastCheck", System.Type.GetType("System.String"))

                        Dim currLocNum As Integer = 0
                        For Each l As QuickQuoteLocation In .Locations
                            currLocNum += 1
                            Dim preExistingLocRow As DataRow = dt.NewRow
                            preExistingLocRow.Item("locNum") = (currLocNum).ToString
                            preExistingLocRow.Item("isPreExisting") = "true"
                            If l IsNot Nothing AndAlso l.Address IsNot Nothing Then
                                With l.Address
                                    preExistingLocRow.Item("streetNum") = .HouseNum
                                    preExistingLocRow.Item("streetName") = .StreetName
                                    preExistingLocRow.Item("aptNum") = .ApartmentNumber
                                    'preExistingLocRow.Item("poBox") = .POBox
                                    If .Zip = "00000-0000" Then
                                        preExistingLocRow.Item("zipCode") = ""
                                    Else
                                        preExistingLocRow.Item("zipCode") = .Zip
                                    End If
                                    preExistingLocRow.Item("city") = .City
                                    preExistingLocRow.Item("state") = .StateId
                                    preExistingLocRow.Item("county") = .County
                                End With
                            End If
                            Dim locOrderInfo As String = ""
                            Dim locHasPrefill As Boolean = ih.HasLocationCommercialDataPrefillOrder(l, orderInformation:=locOrderInfo)
                            preExistingLocRow.Item("hasPrefill") = locHasPrefill.ToString
                            preExistingLocRow.Item("prefillOrderInfo") = locOrderInfo
                            'below flag no longer needs to be set w/ introduction of Preload call above
                            'If locHasPrefill = False Then
                            '    preExistingLocRow.Item("hasChangedSinceLastCheck") = IntegrationHelper.LocationHasAddressData(l, mustHaveAllRequiredFields:=True).ToString
                            'Else
                            '    'see if current info is valid and different than existing order info
                            '    preExistingLocRow.Item("hasChangedSinceLastCheck") = IntegrationHelper.StringIsDifferentThanPropertyPrefillAddressString(locOrderInfo, l.Address, mustHaveAllRequiredFields:=True).ToString
                            'End If
                            dt.Rows.Add(preExistingLocRow)
                        Next
                        Me.rptLocations.DataSource = dt
                        Me.rptLocations.DataBind()
                    End If
                End With
            End If
        End If
    End Sub
    Public Function ShouldShow() As Boolean
        Dim showIt As Boolean = False

        ''If Me.Quote IsNot Nothing AndAlso (Me.Quote.LobType = QuickQuoteLobType.CommercialPackage OrElse Me.Quote.LobType = QuickQuoteLobType.CommercialProperty) AndAlso Helpers.WebHelper_Personal.UseCommercialDataPrefill() = True Then
        'If Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(Me.Quote) = True Then
        '    Dim commDataPrefillQuerystring As String = CommercialDataPrefillQuerystringParam()
        '    If Request.QueryString IsNot Nothing AndAlso Request.QueryString(commDataPrefillQuerystring) IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString(commDataPrefillQuerystring).ToString) = False AndAlso UCase(Request.QueryString(commDataPrefillQuerystring).ToString) = "YES" Then
        '        showIt = True
        '    ElseIf WebHelper_Personal.CommercialDataPrefill_OkayToAutoLaunchIfNeeded() = True Then
        '        'just auto-pop for newBus for now; maybe Endorsements eventually
        '        If Me.Quote.QuoteTransactionType = QuickQuoteTransactionType.NewBusinessQuote Then
        '            Dim ih As New IntegrationHelper
        '            If ih.HasAnyCommercialDataPrefillOrders(Me.Quote) = False Then
        '                'no prefill orders found at policy or location level
        '                'If IsSummaryWorkflow() = False Then
        '                If Me.IsSummaryWorkflow = False Then
        '                    'quote wasn't loaded to Summary screen
        '                    If WebHelper_Personal.IsQuoteIdOrPolicyImageInSessionFromCommDataPrefillError(Me.QuoteIdOrPolicyIdPipeImageNumber) = False Then
        '                        'this quote hasn't gotten a prefill error during this session
        '                        showIt = True
        '                    End If
        '                End If
        '            End If
        '        End If
        '    End If
        'End If
        If WebHelper_Personal.IsCommercialDataPrefillPopupAvailableForQuote(Me.Quote, expectedIsSummaryWorkflowValue:=False, request:=Request, qIdOrPIdAndImgNum:=Me.QuoteIdOrPolicyIdPipeImageNumber) = True Then
            showIt = True
        End If

        Return showIt
    End Function
    'Public Function IsSummaryWorkflow() As Boolean 'now property on VrControlBaseEssentials
    '    Dim isIt As Boolean = False

    '    Dim wkflowTxt As String = "Workflow"
    '    If Request.QueryString IsNot Nothing AndAlso Request.QueryString(wkflowTxt) IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString(wkflowTxt).ToString) = False AndAlso UCase(Request.QueryString(wkflowTxt).ToString) = "SUMMARY" Then
    '        isIt = True
    '    End If

    '    Return isIt
    'End Function

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.PolicyholderSection.ClientID, Me.hdnPolicyholderSection, "0")
        Me.VRScript.CreateAccordion(Me.LocationSection.ClientID, Me.hdnLocationSection, "0")

        Me.txtCityName.CreateAutoComplete("INCities")

        Me.VRScript.AddScriptLine("$(""#" + Me.ddCityName.ClientID + """).hide();")
        Me.VRScript.CreateJSBinding(Me.ddCityName.ClientID, "change", "if($(this).val() == '0'){$(""#" + Me.txtCityName.ClientID + """).show(); } else {$(""#" + Me.txtCityName.ClientID + """).val($(this).val()); $(""#" + Me.txtCityName.ClientID + """).hide();}")

        Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup, "DoCityCountyLookup('" + Me.txtZipCode.ClientID + "','" + Me.ddCityName.ClientID + "','" + Me.txtCityName.ClientID + "','" + Me.txtGaragedCounty.ClientID + "','" + Me.ddStateAbbrev.ClientID + "');")

        ' do this on focus
        Dim addresswarningScript As String = "DoAddressWarning(true,'" + Me.divAddressMessage.ClientID + "','" + Me.txtStreetNum.ClientID + "','" + txtStreetName.ClientID + "','" + txtPOBox.ClientID + "');"
        Me.VRScript.CreateJSBinding({Me.txtStreetNum, Me.txtStreetName, Me.txtPOBox}, ctlPageStartupScript.JsEventType.onfocus, addresswarningScript)

        'do this onblur
        Dim addresswarningScriptOff As String = "DoAddressWarning(false,'" + Me.divAddressMessage.ClientID + "','" + Me.txtStreetNum.ClientID + "','" + txtStreetName.ClientID + "','" + txtPOBox.ClientID + "');"
        Me.VRScript.CreateJSBinding({Me.txtStreetNum, Me.txtStreetName, Me.txtPOBox}, ctlPageStartupScript.JsEventType.onblur, addresswarningScriptOff)

        'client search
        Me.VRScript.AddScriptLine("ClientSearch.SetBindingsCommDataPrefill(""" + Me.txtBusinessName.ClientID + """,""" + Me.txtDBA.ClientID + """,""" + Me.ddBusinessType.ClientID + """,""" + Me.txtFEIN.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""" + Me.txtClientId.ClientID + """,""" + Me.ddPhoneType.ClientID + """,""" + Me.ddTaxIDType.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """,""" + Me.txtDescriptionOfOperations.ClientID + """,""" + Me.txtBusinessStarted.ClientID + """,""" + Me.txtYearsOfExperience.ClientID + """,""" + Me.txtOtherEntity.ClientID + """,""" + Me.txtOther.ClientID + """);")

        Dim clientSearchScript As String = "ClientSearch.DoSearchWithEleNamesComm(e.keyCode,$(this).attr('id'),""" + Me.txtBusinessName.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtFEIN.ClientID + """,true);"

        Me.VRScript.CreateJSBinding(Me.txtBusinessName, ctlPageStartupScript.JsEventType.onkeyup, clientSearchScript)
        Me.VRScript.CreateJSBinding({Me.txtDBA, Me.txtFEIN, Me.txtCityName, Me.txtZipCode}, ctlPageStartupScript.JsEventType.onkeyup, clientSearchScript)

        'track PH info changes
        Dim phInfoChangedScript As String = "$(""#" + Me.hdnHasChangedSinceLoad.ClientID + """).val('true'); $(""#" + Me.hdnHasChangedSinceLastCheck.ClientID + """).val('true');"
        Me.VRScript.CreateJSBinding({txtBusinessName, txtStreetNum, txtStreetName, txtAptNum, txtPOBox, txtZipCode, ddCityName, txtCityName, ddStateAbbrev}, ctlPageStartupScript.JsEventType.onkeyup, phInfoChangedScript)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStaticData()

            'Me.MainAccordionDivId = Me.divCommDataPrefillPopup.ClientID
        Else
            'moved to PreRender
            'If Me.Visible = True Then
            '    ShowPopup()
            'End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        SetLastAction(PrefillControlLastAction.None, shouldDefaultFocusId:=True)
        If Me.Quote IsNot Nothing AndAlso Me.Visible = True AndAlso passedValidation = True Then
            SetLastAction(PrefillControlLastAction.Save)
            'Dim phMissingPrefill As Boolean = Not QQHelper.BitToBoolean(Me.hdnHasPrefill.Value)
            'Dim phChangedSinceLoad As Boolean = QQHelper.BitToBoolean(Me.hdnHasChangedSinceLoad.Value)
            Dim firmIsMissingOrNeedsReOrder As Boolean = False
            Dim locNumsMissingPrefill As List(Of Integer) = Nothing
            Dim locNumsNeedingPrefillReOrder As List(Of Integer) = Nothing
            Dim locNumsChangedSinceLoad As List(Of Integer) = Nothing
            With Me.Quote
                If .Client Is Nothing Then
                    .Client = New QuickQuoteClient
                End If
                With .Client
                    WebHelper_Personal.SetValueIfNotSet_Local(.ClientId, Me.txtClientId.Text, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                End With
                'If .Policyholder Is Nothing Then
                '    .Policyholder = New QuickQuote.CommonObjects.QuickQuotePolicyholder
                'End If
                'With .Policyholder
                '    If .Name Is Nothing Then
                '        .Name = New QuickQuote.CommonObjects.QuickQuoteName
                '    End If
                '    With .Name
                '        QQHelper.SetValueIfNotSet(.CommercialName1, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtBusinessName.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '        QQHelper.SetValueIfNotSet(.TypeId, "2", onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.NumericType) 'note: looks like SetValueIfNotSet is missing some logic for some of the onlyValidIfSpecifiedType param types (PositiveIntegerType, PositiveDecimalType), so just use NumericType for number fields 
                '        QQHelper.SetValueIfNotSet(.DoingBusinessAsName, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtDBA.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '        WebHelper_Personal.SetValueIfNotSet_Local(.EntityTypeId, Me.ddBusinessType.SelectedValue, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                '        QQHelper.SetValueIfNotSet(.OtherLegalEntityDescription, Me.txtOtherEntity.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                '        WebHelper_Personal.SetValueIfNotSet_Local(.TaxTypeId, Me.ddTaxIDType.SelectedValue, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                '        QQHelper.SetValueIfNotSet(.TaxNumber, Me.txtFEIN.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                '        QQHelper.SetValueIfNotSet(.DescriptionOfOperations, Me.txtDescriptionOfOperations.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                '        WebHelper_Personal.SetValueIfNotSet_Local(.DateBusinessStarted, Me.txtBusinessStarted.Text, onlyValidIfSpecifiedType:=TypeToVerify.DateType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to empty string; note: could also check for isValidDataString to make sure it's greater than default date (1/1/1800)
                '        WebHelper_Personal.SetValueIfNotSet_Local(.YearsOfExperience, Me.txtYearsOfExperience.Text, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                '    End With
                '    If String.IsNullOrWhiteSpace(Me.txtPhone.Text) = False OrElse QQHelper.IsPositiveIntegerString(Me.ddPhoneType.SelectedValue) = True OrElse String.IsNullOrWhiteSpace(Me.txtPhoneExt.Text) = False Then
                '        If .Phones Is Nothing Then
                '            .Phones = New List(Of QuickQuotePhone)
                '        End If
                '        If .Phones.Count < 1 Then
                '            .Phones.Add(New QuickQuotePhone)
                '        End If
                '        If .Phones(0) Is Nothing Then
                '            .Phones(0) = New QuickQuotePhone
                '        End If
                '        With .Phones(0)
                '            WebHelper_Personal.SetValueIfNotSet_Local(.TypeId, Me.ddPhoneType.SelectedValue, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                '            QQHelper.SetValueIfNotSet(.Number, Me.txtPhone.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                '            QQHelper.SetValueIfNotSet(.Extension, Me.txtPhoneExt.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                '        End With
                '    End If
                '    If String.IsNullOrWhiteSpace(Me.txtEmail.Text) = False OrElse QQHelper.IsPositiveIntegerString(Me.hdnEmailTypeId.Value) = True Then
                '        If .Emails Is Nothing Then
                '            .Emails = New List(Of QuickQuoteEmail)
                '        End If
                '        If .Emails.Count < 1 Then
                '            .Emails.Add(New QuickQuoteEmail)
                '        End If
                '        If .Emails(0) Is Nothing Then
                '            .Emails(0) = New QuickQuoteEmail
                '        End If
                '        With .Emails(0)
                '            WebHelper_Personal.SetValueIfNotSet_Local(.TypeId, Me.hdnEmailTypeId.Value, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                '            QQHelper.SetValueIfNotSet(.Address, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtEmail.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '        End With
                '    End If
                '    If .Address Is Nothing Then
                '        .Address = New QuickQuote.CommonObjects.QuickQuoteAddress
                '    End If
                '    With .Address
                '        QQHelper.SetValueIfNotSet(.HouseNum, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtStreetNum.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '        QQHelper.SetValueIfNotSet(.StreetName, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtStreetName.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '        QQHelper.SetValueIfNotSet(.ApartmentNumber, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtAptNum.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '        QQHelper.SetValueIfNotSet(.POBox, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtPOBox.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '        QQHelper.SetValueIfNotSet(.Zip, Me.txtZipCode.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                '        QQHelper.SetValueIfNotSet(.City, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtCityName.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '        WebHelper_Personal.SetValueIfNotSet_Local(.StateId, Me.ddStateAbbrev.SelectedValue, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                '        QQHelper.SetValueIfNotSet(.County, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtGaragedCounty.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '        QQHelper.SetValueIfNotSet(.Other, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtOther.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '    End With
                'End With
                SavePolicyholderInfo(.Policyholder, firmIsMissingOrNeedsReOrder:=firmIsMissingOrNeedsReOrder)
                .CopyPolicyholdersToClients()

                'If rptLocations.Items IsNot Nothing AndAlso rptLocations.Items.Count > 0 Then
                '    If .Locations Is Nothing Then
                '        .Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
                '    End If
                '    Dim locCount As Integer = 0
                '    For Each item As RepeaterItem In rptLocations.Items
                '        locCount += 1

                '        Dim currLoc As QuickQuoteLocation = Nothing
                '        If .Locations.Count >= locCount Then
                '            currLoc = .Locations(locCount - 1)
                '            If currLoc Is Nothing Then
                '                currLoc = New QuickQuoteLocation
                '            End If
                '        Else
                '            currLoc = New QuickQuoteLocation
                '            .Locations.Add(currLoc)
                '        End If
                '        With currLoc
                '            If .Address Is Nothing Then
                '                .Address = New QuickQuoteAddress
                '            End If
                '            With .Address
                '                Dim txtLocStreetNum As TextBox = item.FindControl("txtLocStreetNum")
                '                If txtLocStreetNum IsNot Nothing Then
                '                    QQHelper.SetValueIfNotSet(.HouseNum, WebHelper_Personal.TextInUpperCaseAndTrimmed(txtLocStreetNum.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '                End If
                '                Dim txtLocStreetName As TextBox = item.FindControl("txtLocStreetName")
                '                If txtLocStreetName IsNot Nothing Then
                '                    QQHelper.SetValueIfNotSet(.StreetName, WebHelper_Personal.TextInUpperCaseAndTrimmed(txtLocStreetName.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '                End If
                '                Dim txtLocAptNum As TextBox = item.FindControl("txtLocAptNum")
                '                If txtLocAptNum IsNot Nothing Then
                '                    QQHelper.SetValueIfNotSet(.ApartmentNumber, WebHelper_Personal.TextInUpperCaseAndTrimmed(txtLocAptNum.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '                End If
                '                'Dim txtLocPOBox As TextBox = item.FindControl("txtLocPOBox")
                '                'If txtLocPOBox IsNot Nothing Then
                '                '    QQHelper.SetValueIfNotSet(.POBox, WebHelper_Personal.TextInUpperCaseAndTrimmed(txtLocPOBox.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '                'End If
                '                Dim txtLocZipCode As TextBox = item.FindControl("txtLocZipCode")
                '                If txtLocZipCode IsNot Nothing Then
                '                    QQHelper.SetValueIfNotSet(.Zip, txtLocZipCode.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                '                End If
                '                Dim txtLocCityName As TextBox = item.FindControl("txtLocCityName")
                '                If txtLocCityName IsNot Nothing Then
                '                    QQHelper.SetValueIfNotSet(.City, WebHelper_Personal.TextInUpperCaseAndTrimmed(txtLocCityName.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '                End If
                '                Dim ddStateAbbrev As DropDownList = item.FindControl("ddStateAbbrev")
                '                If ddStateAbbrev IsNot Nothing Then
                '                    WebHelper_Personal.SetValueIfNotSet_Local(.StateId, ddStateAbbrev.SelectedValue, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                '                End If
                '                Dim txtLocGaragedCounty As TextBox = item.FindControl("txtLocGaragedCounty")
                '                If txtLocGaragedCounty IsNot Nothing Then
                '                    QQHelper.SetValueIfNotSet(.County, WebHelper_Personal.TextInUpperCaseAndTrimmed(txtLocGaragedCounty.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                '                End If
                '            End With
                '        End With
                '    Next
                'End If
                SaveLocationInfo(.Locations, locNumsMissingPrefill:=locNumsMissingPrefill, locNumsNeedingPrefillReOrder:=locNumsNeedingPrefillReOrder, locNumsChangedSinceLoad:=locNumsChangedSinceLoad)
            End With

            Dim changedOrMissingLocNums As List(Of Integer) = Nothing
            QuickQuoteHelperClass.AddUniqueIntegersToIntegerList(locNumsMissingPrefill, changedOrMissingLocNums)
            'QuickQuoteHelperClass.AddUniqueIntegersToIntegerList(locNumsChangedSinceLoad, changedOrMissingLocNums)
            QuickQuoteHelperClass.AddUniqueIntegersToIntegerList(locNumsNeedingPrefillReOrder, changedOrMissingLocNums)
            CallCommDataPrefill(onlyChangedOrMissingInfo:=True, firmIsMissingOrNeedsReOrder:=firmIsMissingOrNeedsReOrder, changedOrMissingLocNums:=changedOrMissingLocNums)

            'note: this logic was borrowed from ctlInsured - added after prefill call since EntityType could be set by prefill results
            If Me.Quote.Policyholder IsNot Nothing AndAlso Me.Quote.Policyholder.Name IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(Me.Quote.Policyholder.Name.EntityTypeId) = True Then
                If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                    For Each sq As QuickQuoteObject In SubQuotes
                        If sq IsNot Nothing Then
                            WebHelper_Personal.SetValueIfNotSet_Local(sq.EntityTypeId, Me.Quote.Policyholder.Name.EntityTypeId, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                        End If
                    Next
                End If
            End If

            If Me.Quote.QuoteTransactionType = QuickQuoteTransactionType.ReadOnlyImage Then
                'no save
            ElseIf Me.Quote.QuoteTransactionType = QuickQuoteTransactionType.EndorsementQuote Then
                Dim endorsementSaveError As String = ""
                Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError)
            Else
                IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, Nothing, QuickQuoteXML.QuickQuoteSaveType.Quote)
            End If
            CallCapeComPreLoad()
        End If

        Return True
    End Function

    Private Sub rptLocations_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptLocations.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim lblLocStreetNum As HtmlGenericControl = e.Item.FindControl("lblLocStreetNum")
            Dim txtLocStreetNum As TextBox = e.Item.FindControl("txtLocStreetNum")
            If lblLocStreetNum IsNot Nothing AndAlso txtLocStreetNum IsNot Nothing Then
                lblLocStreetNum.Attributes.Add("for", txtLocStreetNum.ClientID)
            End If
            Dim lblLocStreetName As HtmlGenericControl = e.Item.FindControl("lblLocStreetName")
            Dim txtLocStreetName As TextBox = e.Item.FindControl("txtLocStreetName")
            If lblLocStreetName IsNot Nothing AndAlso txtLocStreetName IsNot Nothing Then
                lblLocStreetName.Attributes.Add("for", txtLocStreetName.ClientID)
            End If
            Dim lblLocAptNum As HtmlGenericControl = e.Item.FindControl("lblLocAptNum")
            Dim txtLocAptNum As TextBox = e.Item.FindControl("txtLocAptNum")
            If lblLocAptNum IsNot Nothing AndAlso txtLocAptNum IsNot Nothing Then
                lblLocAptNum.Attributes.Add("for", txtLocAptNum.ClientID)
            End If
            'Dim lblLocPOBox As HtmlGenericControl = e.Item.FindControl("lblLocPOBox")
            'Dim txtLocPOBox As TextBox = e.Item.FindControl("txtLocPOBox")
            'If lblLocPOBox IsNot Nothing AndAlso txtLocStreetNum IsNot Nothing Then
            '    lblLocPOBox.Attributes.Add("for", txtLocPOBox.ClientID)
            'End If
            Dim lblLocZipCode As HtmlGenericControl = e.Item.FindControl("lblLocZipCode")
            Dim txtLocZipCode As TextBox = e.Item.FindControl("txtLocZipCode")
            If lblLocZipCode IsNot Nothing AndAlso txtLocZipCode IsNot Nothing Then
                lblLocZipCode.Attributes.Add("for", txtLocZipCode.ClientID)
            End If
            Dim lblLocCityName As HtmlGenericControl = e.Item.FindControl("lblLocCityName")
            Dim ddLocCityName As DropDownList = e.Item.FindControl("ddLocCityName")
            If lblLocCityName IsNot Nothing AndAlso ddLocCityName IsNot Nothing Then
                lblLocCityName.Attributes.Add("for", ddLocCityName.ClientID)
            End If
            Dim lblLocStateAbbrev As HtmlGenericControl = e.Item.FindControl("lblLocStateAbbrev")
            Dim ddLocStateAbbrev As DropDownList = e.Item.FindControl("ddLocStateAbbrev")
            If lblLocStateAbbrev IsNot Nothing AndAlso ddLocStateAbbrev IsNot Nothing Then
                lblLocStateAbbrev.Attributes.Add("for", ddLocStateAbbrev.ClientID)
            End If

            If ddLocStateAbbrev IsNot Nothing Then
                QQHelper.LoadStaticDataOptionsDropDown(ddLocStateAbbrev, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId)
            End If

            Dim lblLocNum As Label = e.Item.FindControl("lblLocNum")

            Dim lbDeleteLocation As LinkButton = e.Item.FindControl("lbDeleteLocation")
            If lbDeleteLocation IsNot Nothing Then
                lbDeleteLocation.Visible = True

                If lblLocNum IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(lblLocNum.Text) = True AndAlso CInt(lblLocNum.Text) = 1 Then
                    lbDeleteLocation.Visible = False
                Else
                    Dim lblIsPreExistingLoc As Label = e.Item.FindControl("lblIsPreExistingLoc")
                    If lblIsPreExistingLoc IsNot Nothing AndAlso QQHelper.BitToBoolean(lblIsPreExistingLoc.Text) = True Then
                        lbDeleteLocation.Visible = False
                    End If
                End If
            End If

            Dim lblLocState As Label = e.Item.FindControl("lblLocState")
            If lblLocState IsNot Nothing AndAlso ddLocStateAbbrev IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(lblLocState.Text) = True AndAlso ddLocStateAbbrev.Items IsNot Nothing AndAlso ddLocStateAbbrev.Items.Count > 0 AndAlso ddLocStateAbbrev.Items.FindByValue(lblLocState.Text) IsNot Nothing Then
                ddLocStateAbbrev.SelectedValue = lblLocState.Text
            End If

            Dim hdnIndividualLocationRepeaterItemSection As HiddenField = Nothing

            Dim openCurrentLoc As Boolean = False
            If txtLocStreetNum IsNot Nothing AndAlso lblLocNum IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(lblLocNum.Text) = True Then
                Dim setFocus As Boolean = False
                If lastAction = PrefillControlLastAction.AddLocation OrElse lastAction = PrefillControlLastAction.CopyPolicyholder Then
                    If lblLocNum.Text = Me.lblLocNumForLastAction.Text Then
                        setFocus = True
                        openCurrentLoc = True
                    End If
                ElseIf lastAction = PrefillControlLastAction.DeleteLocation Then
                    'try to set focus to closest open Loc but don't open anything from here
                    If hdnIndividualLocationRepeaterItemSection Is Nothing Then
                        hdnIndividualLocationRepeaterItemSection = e.Item.FindControl("hdnIndividualLocationRepeaterItemSection")
                    End If
                    If hdnIndividualLocationRepeaterItemSection IsNot Nothing AndAlso hdnIndividualLocationRepeaterItemSection.Value = "0" Then
                        If QQHelper.IsPositiveIntegerString(Me.lblLocNumWithFocus.Text) = True Then
                            Dim locNumDeleted As Integer = QQHelper.IntegerForString(Me.lblLocNumForLastAction.Text)
                            Dim currNumWithFocus As Integer = CInt(Me.lblLocNumWithFocus.Text)
                            Dim currLocNum As Integer = CInt(lblLocNum.Text)
                            If currLocNum = locNumDeleted Then
                                setFocus = True
                            Else
                                Dim currTentativeDiff As Integer = locNumDeleted - currNumWithFocus
                                Dim currNumDiff As Integer = locNumDeleted - currLocNum
                                Dim currTentativeDiffNoNeg As Integer = If(currTentativeDiff < 0, QuickQuoteHelperClass.ReversePositiveOrNegativeInteger(currTentativeDiff), currTentativeDiff)
                                Dim currNumDiffDiffNoNeg As Integer = If(currNumDiff < 0, QuickQuoteHelperClass.ReversePositiveOrNegativeInteger(currNumDiff), currNumDiff)
                                If currTentativeDiffNoNeg = currNumDiffDiffNoNeg Then
                                    If currNumDiff < 0 Then
                                        setFocus = True
                                    End If
                                Else
                                    If currNumDiffDiffNoNeg < currTentativeDiffNoNeg Then
                                        setFocus = True
                                    End If
                                End If
                            End If
                        Else
                            setFocus = True
                        End If
                    End If
                End If
                If setFocus = True Then
                    Me.lblLocNumWithFocus.Text = lblLocNum.Text
                    Me.lblFocusClientId.Text = txtLocStreetNum.ClientID
                    Me.hdnLocationSection.Value = "0" 'make sure the Location Info section is open
                End If
            End If

            If openCurrentLoc = True Then
                If hdnIndividualLocationRepeaterItemSection Is Nothing Then
                    hdnIndividualLocationRepeaterItemSection = e.Item.FindControl("hdnIndividualLocationRepeaterItemSection")
                End If
                If hdnIndividualLocationRepeaterItemSection IsNot Nothing Then
                    hdnIndividualLocationRepeaterItemSection.Value = "0"
                End If
            End If

            RegisterLocationRepeaterItemScripts(e.Item, ddLocCityName:=ddLocCityName, txtLocZipCode:=txtLocZipCode, ddLocStateAbbrev:=ddLocStateAbbrev, lbDeleteLocation:=lbDeleteLocation, hdnIndividualLocationRepeaterItemSection:=hdnIndividualLocationRepeaterItemSection)
        End If
    End Sub
    Private Sub RegisterLocationRepeaterItemScripts(ByVal item As RepeaterItem, Optional ByRef ddLocCityName As DropDownList = Nothing, Optional ByRef txtLocCityName As TextBox = Nothing, Optional ByRef txtLocZipCode As TextBox = Nothing, Optional ByRef ddLocStateAbbrev As DropDownList = Nothing, Optional ByRef txtLocGaragedCounty As TextBox = Nothing, Optional ByRef lbCopyPolicyholder As LinkButton = Nothing, Optional ByRef lbDeleteLocation As LinkButton = Nothing, Optional ByRef IndividualLocationRepeaterItemSection As HtmlGenericControl = Nothing, Optional ByRef hdnIndividualLocationRepeaterItemSection As HiddenField = Nothing, Optional ByRef txtLocStreetNum As TextBox = Nothing, Optional ByRef txtLocStreetName As TextBox = Nothing, Optional ByRef txtLocAptNum As TextBox = Nothing, Optional ByRef hdnLocHasChangedSinceLoad As HiddenField = Nothing, Optional ByRef hdnLocHasChangedSinceLastCheck As HiddenField = Nothing)
        If item IsNot Nothing Then
            registeredRepeaterScripts = True

            If ddLocCityName Is Nothing Then
                ddLocCityName = item.FindControl("ddLocCityName")
            End If
            If ddLocCityName IsNot Nothing Then
                If txtLocCityName Is Nothing Then
                    txtLocCityName = item.FindControl("txtLocCityName")
                End If
                If txtLocCityName IsNot Nothing Then
                    txtLocCityName.CreateAutoComplete("INCities")

                    Me.VRScript.AddScriptLine("$(""#" + ddLocCityName.ClientID + """).hide();")
                    Me.VRScript.CreateJSBinding(ddLocCityName.ClientID, "change", "if($(this).val() == '0'){$(""#" + txtLocCityName.ClientID + """).show(); } else {$(""#" + txtLocCityName.ClientID + """).val($(this).val()); $(""#" + txtLocCityName.ClientID + """).hide();}")

                    If txtLocZipCode Is Nothing Then
                        txtLocZipCode = item.FindControl("txtLocZipCode")
                    End If
                    If ddLocStateAbbrev Is Nothing Then
                        ddLocStateAbbrev = item.FindControl("ddLocStateAbbrev")
                    End If
                    If txtLocZipCode IsNot Nothing AndAlso ddLocStateAbbrev IsNot Nothing Then
                        If txtLocGaragedCounty Is Nothing Then
                            txtLocGaragedCounty = item.FindControl("txtLocGaragedCounty")
                        End If
                        If txtLocGaragedCounty IsNot Nothing Then
                            Me.VRScript.CreateJSBinding(txtLocZipCode, ctlPageStartupScript.JsEventType.onkeyup, "DoCityCountyLookup('" + txtLocZipCode.ClientID + "','" + ddLocCityName.ClientID + "','" + txtLocCityName.ClientID + "','" + txtLocGaragedCounty.ClientID + "','" + ddLocStateAbbrev.ClientID + "');")
                        End If
                    End If
                End If
            End If

            If lbCopyPolicyholder Is Nothing Then
                lbCopyPolicyholder = item.FindControl("lbCopyPolicyholder")
            End If
            If lbCopyPolicyholder IsNot Nothing Then
                Me.VRScript.StopEventPropagation(lbCopyPolicyholder.ClientID)
            End If

            If lbDeleteLocation Is Nothing Then
                lbDeleteLocation = item.FindControl("lbDeleteLocation")
            End If
            If lbDeleteLocation IsNot Nothing Then
                Me.VRScript.StopEventPropagation(lbDeleteLocation.ClientID)
            End If

            If IndividualLocationRepeaterItemSection Is Nothing Then
                IndividualLocationRepeaterItemSection = item.FindControl("IndividualLocationRepeaterItemSection")
            End If
            If IndividualLocationRepeaterItemSection IsNot Nothing Then
                If hdnIndividualLocationRepeaterItemSection Is Nothing Then
                    hdnIndividualLocationRepeaterItemSection = item.FindControl("hdnIndividualLocationRepeaterItemSection")
                End If
                If hdnIndividualLocationRepeaterItemSection IsNot Nothing Then
                    Me.VRScript.CreateAccordion(IndividualLocationRepeaterItemSection.ClientID, hdnIndividualLocationRepeaterItemSection, "0")
                End If
            End If

            'track location info changes
            If ddLocCityName IsNot Nothing AndAlso txtLocCityName IsNot Nothing AndAlso txtLocZipCode IsNot Nothing AndAlso ddLocStateAbbrev IsNot Nothing Then
                If txtLocStreetNum Is Nothing Then
                    txtLocStreetNum = item.FindControl("txtLocStreetNum")
                End If
                If txtLocStreetName Is Nothing Then
                    txtLocStreetName = item.FindControl("txtLocStreetName")
                End If
                If txtLocAptNum Is Nothing Then
                    txtLocAptNum = item.FindControl("txtLocAptNum")
                End If
                If txtLocStreetNum IsNot Nothing AndAlso txtLocStreetName IsNot Nothing AndAlso txtLocAptNum IsNot Nothing Then
                    If hdnLocHasChangedSinceLoad Is Nothing Then
                        hdnLocHasChangedSinceLoad = item.FindControl("hdnLocHasChangedSinceLoad")
                    End If
                    If hdnLocHasChangedSinceLastCheck Is Nothing Then
                        hdnLocHasChangedSinceLastCheck = item.FindControl("hdnLocHasChangedSinceLastCheck")
                    End If
                    If hdnLocHasChangedSinceLoad IsNot Nothing AndAlso hdnLocHasChangedSinceLastCheck IsNot Nothing Then
                        Dim locInfoChangedScript As String = "$(""#" + hdnLocHasChangedSinceLoad.ClientID + """).val('true'); $(""#" + hdnLocHasChangedSinceLastCheck.ClientID + """).val('true');"
                        Me.VRScript.CreateJSBinding({txtLocStreetNum, txtLocStreetName, txtLocAptNum, txtLocZipCode, ddLocCityName, txtLocCityName, ddLocStateAbbrev}, ctlPageStartupScript.JsEventType.onkeyup, locInfoChangedScript)
                    End If
                End If
            End If

        End If
    End Sub
    Private Sub RegisterLocationRepeaterScripts()
        If rptLocations.Items IsNot Nothing AndAlso rptLocations.Items.Count > 0 Then
            Dim locCount As Integer = 0
            For Each item As RepeaterItem In rptLocations.Items
                RegisterLocationRepeaterItemScripts(item)
            Next
        End If
    End Sub
    Protected Sub lbDeleteLocation_Click(sender As Object, e As EventArgs)
        SetLastAction(PrefillControlLastAction.None, shouldDefaultFocusId:=True)
        If TypeOf sender Is LinkButton Then
            Dim lbDeleteLocation As LinkButton = CType(sender, LinkButton)
            Dim lblLocNum As Label = lbDeleteLocation.Parent.FindControl("lblLocNum")
            If lblLocNum IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(lblLocNum.Text) = True Then
                SetLastAction(PrefillControlLastAction.DeleteLocation)

                Dim locNumsChangedSinceLastCheck As List(Of Integer) = Nothing

                ReloadLocationsOnAction(PrefillControlLastAction.DeleteLocation, CInt(lblLocNum.Text), locNumsChangedSinceLastCheck:=locNumsChangedSinceLastCheck, resetChangedSinceLastCheck:=True)

                CallCommDataPrefillPreload(onlyChangedInfo:=True, changedLocNums:=locNumsChangedSinceLastCheck)
            End If
        End If
    End Sub
    Protected Sub lbCopyPolicyholder_Click(sender As Object, e As EventArgs)
        SetLastAction(PrefillControlLastAction.None, shouldDefaultFocusId:=True)
        If TypeOf sender Is LinkButton Then
            Dim lbCopyPolicyholder As LinkButton = CType(sender, LinkButton)
            Dim lblLocNum As Label = lbCopyPolicyholder.Parent.FindControl("lblLocNum")
            If lblLocNum IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(lblLocNum.Text) = True Then
                SetLastAction(PrefillControlLastAction.CopyPolicyholder)

                Dim locNumsChangedSinceLastCheck As List(Of Integer) = Nothing

                ReloadLocationsOnAction(PrefillControlLastAction.CopyPolicyholder, CInt(lblLocNum.Text), locNumsChangedSinceLastCheck:=locNumsChangedSinceLastCheck, resetChangedSinceLastCheck:=True)

                CallCommDataPrefillPreload(onlyChangedInfo:=True, changedLocNums:=locNumsChangedSinceLastCheck)
            End If
        End If
    End Sub

    Protected Sub btnAddLoc_Click(sender As Object, e As EventArgs) Handles btnAddLoc.Click
        SetLastAction(PrefillControlLastAction.AddLocation, shouldDefaultFocusId:=True)

        Dim locNumForAction As Integer = 0
        If Me.rptLocations.Items IsNot Nothing AndAlso Me.rptLocations.Items.Count > 0 Then
            locNumForAction = Me.rptLocations.Items.Count + 1
        End If
        Dim locNumsChangedSinceLastCheck As List(Of Integer) = Nothing

        ReloadLocationsOnAction(PrefillControlLastAction.AddLocation, locNumForAction:=locNumForAction, locNumsChangedSinceLastCheck:=locNumsChangedSinceLastCheck, resetChangedSinceLastCheck:=True)

        CallCommDataPrefillPreload(onlyChangedInfo:=True, changedLocNums:=locNumsChangedSinceLastCheck)
    End Sub
    Private Sub ReloadLocationsOnAction(ByVal actionToTake As PrefillControlLastAction, Optional ByVal locNumForAction As Integer = 0, Optional ByRef locNumsChangedSinceLastCheck As List(Of Integer) = Nothing, Optional ByVal resetChangedSinceLastCheck As Boolean = False)
        Me.lblLocNumForLastAction.Text = locNumForAction.ToString
        Me.lblLocNumWithFocus.Text = ""
        locNumsChangedSinceLastCheck = Nothing

        Dim dt As New DataTable
        dt.Columns.Add("locNum", System.Type.GetType("System.String"))
        dt.Columns.Add("isPreExisting", System.Type.GetType("System.String"))
        dt.Columns.Add("streetNum", System.Type.GetType("System.String"))
        dt.Columns.Add("streetName", System.Type.GetType("System.String"))
        dt.Columns.Add("aptNum", System.Type.GetType("System.String"))
        'dt.Columns.Add("poBox", System.Type.GetType("System.String"))
        dt.Columns.Add("zipCode", System.Type.GetType("System.String"))
        dt.Columns.Add("city", System.Type.GetType("System.String"))
        dt.Columns.Add("state", System.Type.GetType("System.String"))
        dt.Columns.Add("county", System.Type.GetType("System.String"))
        dt.Columns.Add("accordionFlag", System.Type.GetType("System.String"))
        dt.Columns.Add("hasPrefill", System.Type.GetType("System.String"))
        dt.Columns.Add("prefillOrderInfo", System.Type.GetType("System.String"))
        dt.Columns.Add("hasChangedSinceLoad", System.Type.GetType("System.String"))
        dt.Columns.Add("hasChangedSinceLastCheck", System.Type.GetType("System.String"))

        Dim currItemCount As Integer = 0
        Dim hasRemovedLoc As Boolean = False
        If rptLocations.Items IsNot Nothing AndAlso rptLocations.Items.Count > 0 Then
            For Each item As RepeaterItem In rptLocations.Items
                If actionToTake = PrefillControlLastAction.DeleteLocation AndAlso locNumForAction > 0 AndAlso hasRemovedLoc = False AndAlso currItemCount + 1 = locNumForAction Then
                    hasRemovedLoc = True
                Else
                    currItemCount += 1
                    Dim preExistingRow As DataRow = dt.NewRow
                    preExistingRow.Item("locNum") = (currItemCount).ToString
                    Dim lblIsPreExistingLoc As Label = item.FindControl("lblIsPreExistingLoc")
                    If lblIsPreExistingLoc IsNot Nothing Then
                        preExistingRow.Item("isPreExisting") = lblIsPreExistingLoc.Text
                    End If
                    Dim hdnIndividualLocationRepeaterItemSection As HiddenField = item.FindControl("hdnIndividualLocationRepeaterItemSection")
                    If hdnIndividualLocationRepeaterItemSection IsNot Nothing Then
                        preExistingRow.Item("accordionFlag") = hdnIndividualLocationRepeaterItemSection.Value
                    End If
                    Dim hdnLocHasPrefill As HiddenField = item.FindControl("hdnLocHasPrefill")
                    If hdnLocHasPrefill IsNot Nothing Then
                        preExistingRow.Item("hasPrefill") = hdnLocHasPrefill.Value
                    End If
                    Dim hdnLocPrefillOrderInfo As HiddenField = item.FindControl("hdnLocPrefillOrderInfo")
                    If hdnLocPrefillOrderInfo IsNot Nothing Then
                        preExistingRow.Item("prefillOrderInfo") = hdnLocPrefillOrderInfo.Value
                    End If
                    Dim hdnLocHasChangedSinceLoad As HiddenField = item.FindControl("hdnLocHasChangedSinceLoad")
                    If hdnLocHasChangedSinceLoad IsNot Nothing Then
                        preExistingRow.Item("hasChangedSinceLoad") = hdnLocHasChangedSinceLoad.Value
                    End If
                    Dim hdnLocHasChangedSinceLastCheck As HiddenField = item.FindControl("hdnLocHasChangedSinceLastCheck")
                    If hdnLocHasChangedSinceLastCheck IsNot Nothing Then
                        If QQHelper.BitToBoolean(hdnLocHasChangedSinceLastCheck.Value) = True Then
                            QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(currItemCount, locNumsChangedSinceLastCheck)
                        End If
                        preExistingRow.Item("hasChangedSinceLastCheck") = hdnLocHasChangedSinceLastCheck.Value
                        'preExistingRow.Item("hasChangedSinceLastCheck") = If(resetChangedSinceLastCheck = True, "", hdnLocHasChangedSinceLastCheck.Value)
                    End If
                    If actionToTake = PrefillControlLastAction.CopyPolicyholder AndAlso locNumForAction > 0 AndAlso currItemCount = locNumForAction Then
                        preExistingRow.Item("streetNum") = txtStreetNum.Text
                        preExistingRow.Item("streetName") = txtStreetName.Text
                        preExistingRow.Item("aptNum") = txtAptNum.Text
                        'preExistingRow.Item("poBox") = txtPOBox.Text
                        preExistingRow.Item("zipCode") = txtZipCode.Text
                        preExistingRow.Item("city") = txtCityName.Text
                        preExistingRow.Item("state") = ddStateAbbrev.SelectedValue
                        preExistingRow.Item("county") = txtGaragedCounty.Text
                        If hdnLocHasChangedSinceLoad IsNot Nothing Then
                            preExistingRow.Item("hasChangedSinceLoad") = "true"
                        End If
                        If hdnLocHasChangedSinceLastCheck IsNot Nothing Then
                            preExistingRow.Item("hasChangedSinceLastCheck") = "true"
                            QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(currItemCount, locNumsChangedSinceLastCheck)
                        End If
                    Else
                        Dim txtLocStreetNum As TextBox = item.FindControl("txtLocStreetNum")
                        If txtLocStreetNum IsNot Nothing Then
                            preExistingRow.Item("streetNum") = txtLocStreetNum.Text
                        End If
                        Dim txtLocStreetName As TextBox = item.FindControl("txtLocStreetName")
                        If txtLocStreetName IsNot Nothing Then
                            preExistingRow.Item("streetName") = txtLocStreetName.Text
                        End If
                        Dim txtLocAptNum As TextBox = item.FindControl("txtLocAptNum")
                        If txtLocAptNum IsNot Nothing Then
                            preExistingRow.Item("aptNum") = txtLocAptNum.Text
                        End If
                        'Dim txtLocPOBox As TextBox = item.FindControl("txtLocPOBox")
                        'If txtLocPOBox IsNot Nothing Then
                        '    preExistingRow.Item("poBox") = txtLocPOBox.Text
                        'End If
                        Dim txtLocZipCode As TextBox = item.FindControl("txtLocZipCode")
                        If txtLocZipCode IsNot Nothing Then
                            preExistingRow.Item("zipCode") = txtLocZipCode.Text
                        End If
                        Dim txtLocCityName As TextBox = item.FindControl("txtLocCityName")
                        If txtLocCityName IsNot Nothing Then
                            preExistingRow.Item("city") = txtLocCityName.Text
                        End If
                        'Dim lblLocState As Label = item.FindControl("lblLocState")
                        'If lblLocState IsNot Nothing Then
                        '    preExistingRow.Item("state") = lblLocState.Text
                        'End If
                        Dim ddLocStateAbbrev As DropDownList = item.FindControl("ddLocStateAbbrev")
                        If ddLocStateAbbrev IsNot Nothing Then
                            preExistingRow.Item("state") = ddLocStateAbbrev.SelectedValue
                        End If
                        Dim txtLocGaragedCounty As TextBox = item.FindControl("txtLocGaragedCounty")
                        If txtLocGaragedCounty IsNot Nothing Then
                            preExistingRow.Item("county") = txtLocGaragedCounty.Text
                        End If
                    End If
                    If resetChangedSinceLastCheck = True Then
                        preExistingRow.Item("hasChangedSinceLastCheck") = ""
                    End If
                    dt.Rows.Add(preExistingRow)
                End If
            Next
        End If

        If actionToTake = PrefillControlLastAction.AddLocation Then
            Dim newRow As DataRow = dt.NewRow
            newRow.Item("locNum") = (currItemCount + 1).ToString
            newRow.Item("isPreExisting") = "false"
            newRow.Item("hasPrefill") = "false"
            dt.Rows.Add(newRow)
        End If

        Me.rptLocations.DataSource = dt
        Me.rptLocations.DataBind()

    End Sub
    Private Function OpenLocationNums() As List(Of Integer)
        Dim numList As List(Of Integer) = Nothing

        If Me.rptLocations.Items IsNot Nothing AndAlso Me.rptLocations.Items.Count > 0 Then
            Dim currLocNum As Integer = 0
            For Each i As RepeaterItem In Me.rptLocations.Items
                currLocNum += 1
                Dim hdnIndividualLocationRepeaterItemSection As HiddenField = i.FindControl("hdnIndividualLocationRepeaterItemSection")
                If hdnIndividualLocationRepeaterItemSection IsNot Nothing AndAlso hdnIndividualLocationRepeaterItemSection.Value = "0" Then
                    QuickQuoteHelperClass.AddIntegerToIntegerList(currLocNum, numList)
                End If
            Next
        End If

        Return numList
    End Function

    Dim passedValidation As Boolean = False
    Protected Sub btnSavePrefillEntry_Click(sender As Object, e As EventArgs) Handles btnSavePrefillEntry.Click
        If PassesValidation() = True Then
            passedValidation = True
            Me.Save()
            Dim redirectUrl As String = Request.RawUrl
            Dim commDataPrefillQuerystring = CommercialDataPrefillQuerystringParam()
            commDataPrefillQuerystring = "&" & commDataPrefillQuerystring & "=Yes"

            redirectUrl = WebHelper_Personal.RemoveAllInstancesOfStringFromString_Local(redirectUrl, commDataPrefillQuerystring)
            Response.Redirect(redirectUrl, True)
        End If
    End Sub
    Private Function PassesValidation() As Boolean
        Dim isOkay As Boolean = True

        SetLastAction(PrefillControlLastAction.Validate, shouldDefaultFocusId:=True)

        If Me.ValidationSummmary IsNot Nothing Then
            Dim valSummaryTitle As String = Me.ValidationSummmary.Title
            Dim summaryTxtToRemove As String = "Quote was saved. "
            If String.IsNullOrWhiteSpace(valSummaryTitle) = False AndAlso valSummaryTitle.Contains(summaryTxtToRemove) = True Then
                Me.ValidationSummmary.Title = WebHelper_Personal.RemoveAllInstancesOfStringFromString_Local(valSummaryTitle, summaryTxtToRemove)
            End If
        End If

        If String.IsNullOrWhiteSpace(Me.txtBusinessName.Text) = True Then
            If isOkay = True Then
                Me.lblFocusClientId.Text = Me.txtBusinessName.ClientID
            End If
            isOkay = False
            Me.ValidationHelper.AddError("Business Name required", Me.txtBusinessName.ClientID)
        End If
        If String.IsNullOrWhiteSpace(Me.txtStreetNum.Text) = True AndAlso String.IsNullOrWhiteSpace(Me.txtStreetName.Text) = True AndAlso String.IsNullOrWhiteSpace(Me.txtPOBox.Text) = True Then
            If isOkay = True Then
                Me.lblFocusClientId.Text = Me.txtStreetNum.ClientID
            End If
            isOkay = False
            Me.ValidationHelper.AddError("Street Number / Street Name or P.O. Box required", Me.txtStreetNum.ClientID)
        ElseIf String.IsNullOrWhiteSpace(Me.txtStreetNum.Text) = False AndAlso String.IsNullOrWhiteSpace(Me.txtStreetName.Text) = True Then
            If isOkay = True Then
                Me.lblFocusClientId.Text = Me.txtStreetName.ClientID
            End If
            isOkay = False
            Me.ValidationHelper.AddError("Street Name required with Street Number", Me.txtStreetName.ClientID)
        ElseIf String.IsNullOrWhiteSpace(Me.txtStreetName.Text) = False AndAlso String.IsNullOrWhiteSpace(Me.txtStreetNum.Text) = True Then
            If isOkay = True Then
                Me.lblFocusClientId.Text = Me.txtStreetNum.ClientID
            End If
            isOkay = False
            Me.ValidationHelper.AddError("Street Number required with Street Name", Me.txtStreetNum.ClientID)
        End If
        If String.IsNullOrWhiteSpace(Me.txtZipCode.Text) = True Then
            If isOkay = True Then
                Me.lblFocusClientId.Text = Me.txtZipCode.ClientID
            End If
            isOkay = False
            Me.ValidationHelper.AddError("Zip Code required", Me.txtZipCode.ClientID)
        End If
        If String.IsNullOrWhiteSpace(Me.txtCityName.Text) = True Then
            If isOkay = True Then
                Me.lblFocusClientId.Text = Me.txtCityName.ClientID
            End If
            isOkay = False
            Me.ValidationHelper.AddError("City required", Me.txtCityName.ClientID)
        End If
        If QQHelper.IsPositiveIntegerString(Me.ddStateAbbrev.SelectedValue) = False Then
            If isOkay = True Then
                Me.lblFocusClientId.Text = Me.ddStateAbbrev.ClientID
            End If
            isOkay = False
            Me.ValidationHelper.AddError("State required", Me.ddStateAbbrev.ClientID)
        End If

        If isOkay = False Then
            Me.hdnPolicyholderSection.Value = "0"
        End If

        If rptLocations.Items IsNot Nothing AndAlso rptLocations.Items.Count > 0 Then
            Dim locationsAreOkay As Boolean = True
            Dim locCount As Integer = 0
            For Each item As RepeaterItem In rptLocations.Items
                locCount += 1

                ValidateLocationRepeaterItem(item, locCount, isOkay:=isOkay, locationsOkay:=locationsAreOkay)
            Next
            If locationsAreOkay = False Then
                Me.hdnLocationSection.Value = "0"
            End If
        Else
            isOkay = False
            Me.ValidationHelper.AddError("At least 1 Location required")
        End If

        Return isOkay
    End Function
    Private Sub ValidateLocationRepeaterItem(ByVal item As RepeaterItem, ByVal locNum As Integer, ByRef isOkay As Boolean, ByRef locationsOkay As Boolean) 'note: don't default isOkay... need to maintain it's current value
        If item IsNot Nothing Then
            If locNum < 1 Then
                Dim lblLocNum As Label = item.FindControl("lblLocNum")
                If lblLocNum IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(lblLocNum.Text) = True Then
                    locNum = CInt(lblLocNum.Text)
                End If
            End If

            Dim locText As String = "Location"
            If locNum > 0 Then
                locText &= " # " & locNum.ToString
            End If

            Dim currentLocHasIssue As Boolean = False

            Dim txtLocStreetNum As TextBox = item.FindControl("txtLocStreetNum")
            If txtLocStreetNum IsNot Nothing AndAlso String.IsNullOrWhiteSpace(txtLocStreetNum.Text) = True Then
                If isOkay = True Then
                    Me.lblFocusClientId.Text = txtLocStreetNum.ClientID
                End If
                isOkay = False
                locationsOkay = False
                currentLocHasIssue = True
                Me.ValidationHelper.AddError(locText & " Street Number required", txtLocStreetNum.ClientID)
            End If
            Dim txtLocStreetName As TextBox = item.FindControl("txtLocStreetName")
            If txtLocStreetName IsNot Nothing AndAlso String.IsNullOrWhiteSpace(txtLocStreetName.Text) = True Then
                If isOkay = True Then
                    Me.lblFocusClientId.Text = txtLocStreetName.ClientID
                End If
                isOkay = False
                locationsOkay = False
                currentLocHasIssue = True
                Me.ValidationHelper.AddError(locText & " Street Name required", txtLocStreetName.ClientID)
            End If
            Dim txtLocZipCode As TextBox = item.FindControl("txtLocZipCode")
            If txtLocZipCode IsNot Nothing AndAlso String.IsNullOrWhiteSpace(txtLocZipCode.Text) = True Then
                If isOkay = True Then
                    Me.lblFocusClientId.Text = txtLocZipCode.ClientID
                End If
                isOkay = False
                locationsOkay = False
                currentLocHasIssue = True
                Me.ValidationHelper.AddError(locText & " Zip Code required", txtLocZipCode.ClientID)
            End If
            Dim txtLocCityName As TextBox = item.FindControl("txtLocCityName")
            If txtLocCityName IsNot Nothing AndAlso String.IsNullOrWhiteSpace(txtLocCityName.Text) = True Then
                If isOkay = True Then
                    Me.lblFocusClientId.Text = txtLocCityName.ClientID
                End If
                isOkay = False
                locationsOkay = False
                currentLocHasIssue = True
                Me.ValidationHelper.AddError(locText & " City required", txtLocCityName.ClientID)
            End If
            Dim ddLocStateAbbrev As DropDownList = item.FindControl("ddLocStateAbbrev")
            If ddLocStateAbbrev IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(ddLocStateAbbrev.SelectedValue) = False Then
                If isOkay = True Then
                    Me.lblFocusClientId.Text = ddLocStateAbbrev.ClientID
                End If
                isOkay = False
                locationsOkay = False
                currentLocHasIssue = True
                Me.ValidationHelper.AddError(locText & " State required", ddLocStateAbbrev.ClientID)
            End If

            If currentLocHasIssue = True Then
                Dim hdnIndividualLocationRepeaterItemSection As HiddenField = item.FindControl("hdnIndividualLocationRepeaterItemSection")
                If hdnIndividualLocationRepeaterItemSection IsNot Nothing Then
                    hdnIndividualLocationRepeaterItemSection.Value = "0"
                End If
            End If
        End If
    End Sub
    Private Function CommercialDataPrefillQuerystringParam() As String
        Dim commDataPrefillQuerystring = Helpers.WebHelper_Personal.CommercialDataPrefillQuerystringParam()
        If String.IsNullOrWhiteSpace(commDataPrefillQuerystring) = True Then
            commDataPrefillQuerystring = "CommDataPrefill"
        End If
        Return commDataPrefillQuerystring
    End Function

    Public Sub ShowPopup()

        If DirectCast(Me.Page.Master, VelociRater).AgencyID > 0 Then
            If HasValidFocusClientId() = False Then
                DefaultFocusClientId()
            End If
            Me.VRScript.CreatePopupForm(Me.divCommDataPrefillPopup.ClientID, "Named Insured and Business Location Details", 520, 680, True, True, False, Me.lblFocusClientId.Text, String.Empty)
        Else
            Me.VRScript.AddScriptLine("alert('Choose an agency before creating a new quote.');")
        End If

    End Sub
    Public Sub DefaultFocusClientId()
        Me.lblFocusClientId.Text = Me.txtBusinessName.ClientID
    End Sub
    Private Function HasValidFocusClientId() As Boolean
        Dim isValid As Boolean = False

        Dim fc As Control = Nothing
        If String.IsNullOrWhiteSpace(Me.lblFocusClientId.Text) = False Then
            fc = RecursiveFindControl(Me.lblFocusClientId.Text, rootControl:=Me)
        End If

        If String.IsNullOrWhiteSpace(Me.lblFocusClientId.Text) = False AndAlso (fc IsNot Nothing) Then
            isValid = True
        End If

        Return isValid
    End Function
    Private Function RecursiveFindControl(ByVal clientId As String, Optional ByVal rootControl As Control = Nothing) As Control
        Dim c As Control = Nothing

        If String.IsNullOrEmpty(clientId) = False Then
            If rootControl Is Nothing Then
                rootControl = Page
            End If

            If rootControl IsNot Nothing Then
                If String.IsNullOrWhiteSpace(rootControl.ClientID) = False Then
                    If UCase(rootControl.ClientID) = UCase(clientId) Then
                        c = rootControl
                    End If
                End If
                If c Is Nothing AndAlso rootControl.Controls IsNot Nothing AndAlso rootControl.Controls.Count > 0 Then
                    For Each cc As Control In rootControl.Controls
                        If cc IsNot Nothing Then
                            c = RecursiveFindControl(clientId, rootControl:=cc)
                            If c IsNot Nothing Then
                                Exit For
                            End If
                        End If
                    Next
                End If
            End If
        End If

        Return c
    End Function
    Public Sub HidePopup()
        Me.Visible = False
        VRScript.AddScriptLine("$('#divASP_Popups').hide();")
        VRScript.AddScriptLine("EnableFormOnSaveRemoves();")
    End Sub

    Private Sub btnCancelPrefill_Click(sender As Object, e As EventArgs) Handles btnCancelPrefill.Click
        SetLastAction(PrefillControlLastAction.Cancel, shouldDefaultFocusId:=True)
        HidePopup()
    End Sub
    Private Sub SetLastAction(ByVal action As PrefillControlLastAction, Optional ByVal shouldDefaultFocusId As Boolean = False)
        lastAction = action
        If shouldDefaultFocusId = True Then
            DefaultFocusClientId()
        End If
    End Sub

    Private Sub ctlCommercialDataPrefillEntry_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        'If Page.IsPostBack = True AndAlso Me.Visible = True Then
        '    ShowPopup()
        '    If registeredRepeaterScripts = False Then
        '        RegisterLocationRepeaterScripts()
        '    End If
        'End If
        If Me.Visible = True Then
            If Me.RiskGradeSearchIsVisible = True Then
                Me.VRScript.AddScriptLine("$(""#" + Me.divCommDataPrefillPopup.ClientID + """).hide();")
            Else
                ShowPopup()
                Me.VRScript.AddScriptLine("$(""#" + Me.divCommDataPrefillPopup.ClientID + """).show();")
                If Page.IsPostBack = True Then
                    If registeredRepeaterScripts = False Then
                        RegisterLocationRepeaterScripts()
                    End If
                End If
            End If
        End If
    End Sub
    Dim registeredRepeaterScripts As Boolean = False

    Private Sub CallCommDataPrefill(Optional ByVal onlyChangedOrMissingInfo As Boolean = False, Optional ByVal firmIsMissingOrNeedsReOrder As Nullable(Of Boolean) = Nothing, Optional ByVal changedOrMissingLocNums As List(Of Integer) = Nothing)
        Dim ih As New IntegrationHelper
        Dim attemptedServiceCall As Boolean = False
        Dim serviceType As IntegrationHelper.CommercialDataPrefillServiceType = IntegrationHelper.CommercialDataPrefillServiceType.None
        Dim caughtUnhandledException As Boolean = False
        Dim unhandledExceptionToString As String = ""
        Dim setAnyMods As Boolean = False
        'ih.CallCommercialDataPrefill(Me.Quote, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
        Dim okayForPH As Boolean = False
        Dim okayForLocs As Boolean = False
        If onlyChangedOrMissingInfo = True Then
            'If QQHelper.BitToBoolean(Me.hdnHasPrefill.Value) = False OrElse QQHelper.BitToBoolean(Me.hdnHasChangedSinceLoad.Value) = True Then
            If firmIsMissingOrNeedsReOrder Is Nothing Then
                firmIsMissingOrNeedsReOrder = QQHelper.BitToBoolean(Me.hdnHasPrefill.Value) = False OrElse IntegrationHelper.StringIsDifferentThanFirmographicsPrefillNameAddressString(Me.hdnPrefillOrderInfo.Value, Me.Quote.Policyholder, mustHaveAllRequiredFields:=True) = True
            End If
            If firmIsMissingOrNeedsReOrder = True Then
                okayForPH = True
            End If
            If changedOrMissingLocNums IsNot Nothing AndAlso changedOrMissingLocNums.Count > 0 Then
                okayForLocs = True
            End If
        Else
            okayForPH = True
            okayForLocs = True
            changedOrMissingLocNums = Nothing
        End If

        If okayForPH = True OrElse okayForLocs = True Then
            If okayForLocs = False Then
                'Firmograhpics only
                serviceType = IntegrationHelper.CommercialDataPrefillServiceType.FirmographicsOnly
                ih.CallCommercialDataPrefill_FirmographicsOnly(Me.Quote, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, setAnyMods:=setAnyMods)
            ElseIf okayForPH = False Then
                'Property only
                serviceType = IntegrationHelper.CommercialDataPrefillServiceType.PropertyOnly
                ih.CallCommercialDataPrefill_PropertyOnly(Me.Quote, locationNums:=changedOrMissingLocNums, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, setAnyMods:=setAnyMods)
            Else 'okayForPH = True AndAlso okayForLocs = True
                'both Firmographics and Property
                ih.CallCommercialDataPrefill(Me.Quote, locationNums:=changedOrMissingLocNums, attemptedServiceCall:=attemptedServiceCall, attemptedServiceCallType:=serviceType, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, setAnyMods:=setAnyMods)
            End If

            If attemptedServiceCall = True Then

            End If
        End If

        If caughtUnhandledException = True Then
            WebHelper_Personal.AddToQuoteIdsOrPolicyImagesInSessionFromCommDataPrefillError(Me.QuoteIdOrPolicyIdPipeImageNumber)
            Dim prefillError As New IFM.ErrLog_Parameters_Structure()
            Dim prefillStackTrace As String = ""
            If serviceType = IntegrationHelper.CommercialDataPrefillServiceType.FirmographicsOnly Then
                prefillStackTrace = "at IntegrationHelper.CallCommercialDataPrefill_FirmographicsOnly"
            ElseIf serviceType = IntegrationHelper.CommercialDataPrefillServiceType.PropertyOnly Then
                prefillStackTrace = "at IntegrationHelper.CallCommercialDataPrefill_PropertyOnly"
            Else
                prefillStackTrace = "at IntegrationHelper.CallCommercialDataPrefill"
            End If
            Dim addPrefillInfo As String = WebHelper_Personal.AdditionalInfoTextForCommercialDataPrefillError(Me.Quote, qqHelper:=QQHelper)
            If changedOrMissingLocNums IsNot Nothing AndAlso changedOrMissingLocNums.Count > 0 Then
                addPrefillInfo = QQHelper.appendText(addPrefillInfo, "changedOrMissingLocNums: " & QuickQuoteHelperClass.StringForListOfInteger(changedOrMissingLocNums, delimiter:=","), splitter:="; ")
            End If
            With prefillError
                .ApplicationName = "Velocirater Personal"
                .ClassName = "ctlCommercialDataPrefillEntry"
                .ErrorMessage = unhandledExceptionToString
                .LogDate = DateTime.Now
                .RoutineName = "CallCommDataPrefill"
                .StackTrace = prefillStackTrace
                .AdditionalInfo = addPrefillInfo
            End With
            WriteErrorLogRecord(prefillError, "")
        End If
    End Sub
    Private Sub CallCommDataPrefillPreload(Optional ByVal onlyChangedInfo As Boolean = False, Optional ByVal changedLocNums As List(Of Integer) = Nothing)
        Dim ih As New IntegrationHelper
        Dim attemptedServiceCall As Boolean = False
        Dim serviceType As IntegrationHelper.CommercialDataPrefillServiceType = IntegrationHelper.CommercialDataPrefillServiceType.None
        Dim caughtUnhandledException As Boolean = False
        Dim unhandledExceptionToString As String = ""
        'ih.CallCommercialDataPrefill_Preload(Me.Quote, attemptedServiceCall:=attemptedServiceCall, attemptedServiceCallType:=serviceType, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
        Dim ph As QuickQuotePolicyholder = Nothing
        Dim locs As List(Of QuickQuoteLocation) = Nothing
        'note: see Save logic above
        Dim okayForPH As Boolean = False
        Dim okayForLocs As Boolean = False
        If onlyChangedInfo = True Then
            If QQHelper.BitToBoolean(Me.hdnHasChangedSinceLastCheck.Value) = True Then
                okayForPH = True
                Me.hdnHasChangedSinceLastCheck.Value = ""
            End If
            If changedLocNums IsNot Nothing AndAlso changedLocNums.Count > 0 Then
                okayForLocs = True
            End If
        Else
            okayForPH = True
            okayForLocs = True
            changedLocNums = Nothing
        End If

        If okayForPH = True OrElse okayForLocs = True Then
            'either something loaded that needed an order/reOrder or something changed; check specifically for missing orders or ones that are different than the previous order
            'or we're ordering on everything (not the case currently)
            Dim savedPHInfo As Boolean = False

            If okayForPH = True AndAlso onlyChangedInfo = True Then
                Dim firmIsMissingOrNeedsReOrder As Boolean = False
                SavePolicyholderInfo(ph, firmIsMissingOrNeedsReOrder:=firmIsMissingOrNeedsReOrder)
                savedPHInfo = True
                If firmIsMissingOrNeedsReOrder = False Then
                    okayForPH = False
                End If
            End If

            If okayForLocs = True Then
                Dim locNumsMissingPrefill As List(Of Integer) = Nothing
                Dim locNumsNeedingPrefillReOrder As List(Of Integer) = Nothing
                SaveLocationInfo(locs, locNumsMissingPrefill:=locNumsMissingPrefill, locNumsNeedingPrefillReOrder:=locNumsNeedingPrefillReOrder)
                If onlyChangedInfo = True AndAlso changedLocNums IsNot Nothing AndAlso changedLocNums.Count > 0 Then
                    Dim changedOrMissingLocNums As List(Of Integer) = Nothing
                    QuickQuoteHelperClass.AddUniqueIntegersToIntegerList(locNumsMissingPrefill, changedOrMissingLocNums)
                    QuickQuoteHelperClass.AddUniqueIntegersToIntegerList(locNumsNeedingPrefillReOrder, changedOrMissingLocNums)
                    If changedOrMissingLocNums Is Nothing OrElse changedOrMissingLocNums.Count = 0 Then
                        okayForLocs = False
                    Else
                        Dim changedLocNumsUpdated As List(Of Integer) = Nothing
                        For Each n As Integer In changedLocNums
                            If changedOrMissingLocNums.Contains(n) = True Then
                                QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(n, changedLocNumsUpdated)
                            End If
                        Next
                        If changedLocNumsUpdated Is Nothing OrElse changedLocNumsUpdated.Count = 0 Then
                            okayForLocs = False
                        Else
                            changedLocNums = changedLocNumsUpdated
                        End If
                    End If
                End If
            End If

            'now check again to make sure everything is good
            If okayForPH = True OrElse okayForLocs = True Then
                If savedPHInfo = False Then
                    SavePolicyholderInfo(ph)
                End If
                If okayForLocs = False Then
                    'Firmograhpics only
                    serviceType = IntegrationHelper.CommercialDataPrefillServiceType.FirmographicsOnly
                    ih.CallCommercialDataPrefill_FirmographicsOnly_Preload(ph, policyId:=Me.CommercialDataPrefillPolicyId, policyImageNum:=Me.CommercialDataPrefillPolicyImageNum, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
                ElseIf okayForPH = False Then
                    'Property only
                    serviceType = IntegrationHelper.CommercialDataPrefillServiceType.PropertyOnly
                    ih.CallCommercialDataPrefill_PropertyOnly_Preload(locs, ph:=ph, policyId:=Me.CommercialDataPrefillPolicyId, policyImageNum:=Me.CommercialDataPrefillPolicyImageNum, locationNums:=changedLocNums, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
                Else 'okayForPH = True AndAlso okayForLocs = True
                    'both Firmographics and Property
                    ih.CallCommercialDataPrefill_Preload(ph, locs, policyId:=Me.CommercialDataPrefillPolicyId, policyImageNum:=Me.CommercialDataPrefillPolicyImageNum, locationNums:=changedLocNums, attemptedServiceCall:=attemptedServiceCall, attemptedServiceCallType:=serviceType, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
                End If

                If attemptedServiceCall = True Then

                End If
                If caughtUnhandledException = True Then
                    WebHelper_Personal.AddToQuoteIdsOrPolicyImagesInSessionFromCommDataPrefillError(Me.QuoteIdOrPolicyIdPipeImageNumber)
                    Dim preloadError As New IFM.ErrLog_Parameters_Structure()
                    Dim preloadStackTrace As String = ""
                    If serviceType = IntegrationHelper.CommercialDataPrefillServiceType.FirmographicsOnly Then
                        preloadStackTrace = "at IntegrationHelper.CallCommercialDataPrefill_FirmographicsOnly_Preload"
                    ElseIf serviceType = IntegrationHelper.CommercialDataPrefillServiceType.PropertyOnly Then
                        preloadStackTrace = "at IntegrationHelper.CallCommercialDataPrefill_PropertyOnly_Preload"
                    Else
                        preloadStackTrace = "at IntegrationHelper.CallCommercialDataPrefill_Preload"
                    End If
                    Dim addPreloadInfo As String = "" 'WebHelper_Personal.AdditionalInfoTextForCommercialDataPrefillError(Me.Quote, qqHelper:=QQHelper)
                    addPreloadInfo = QQHelper.appendText(addPreloadInfo, "QuoteIdOrPolicyIdPipeImageNumber: " & Me.QuoteIdOrPolicyIdPipeImageNumber, splitter:="; ")
                    addPreloadInfo = QQHelper.appendText(addPreloadInfo, "CommercialDataPrefillPolicyId: " & Me.CommercialDataPrefillPolicyId, splitter:="; ")
                    addPreloadInfo = QQHelper.appendText(addPreloadInfo, "CommercialDataPrefillPolicyImageNum: " & Me.CommercialDataPrefillPolicyImageNum, splitter:="; ")
                    If changedLocNums IsNot Nothing AndAlso changedLocNums.Count > 0 Then
                        addPreloadInfo = QQHelper.appendText(addPreloadInfo, "changedLocNums: " & QuickQuoteHelperClass.StringForListOfInteger(changedLocNums, delimiter:=","), splitter:="; ")
                    End If
                    With preloadError
                        .ApplicationName = "Velocirater Personal"
                        .ClassName = "ctlCommercialDataPrefillEntry"
                        .ErrorMessage = unhandledExceptionToString
                        .LogDate = DateTime.Now
                        .RoutineName = "CallCommDataPrefillPreload"
                        .StackTrace = preloadStackTrace
                        .AdditionalInfo = addPreloadInfo
                    End With
                    WriteErrorLogRecord(preloadError, "")
                End If
            End If
        End If
    End Sub
    Private Sub SavePolicyholderInfo(ByRef ph As QuickQuotePolicyholder, Optional ByRef firmIsMissingOrNeedsReOrder As Boolean = False)
        firmIsMissingOrNeedsReOrder = False
        If ph Is Nothing Then
            ph = New QuickQuotePolicyholder
        End If
        With ph
            If .Name Is Nothing Then
                .Name = New QuickQuote.CommonObjects.QuickQuoteName
            End If
            With .Name
                QQHelper.SetValueIfNotSet(.CommercialName1, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtBusinessName.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                QQHelper.SetValueIfNotSet(.TypeId, "2", onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.NumericType) 'note: looks like SetValueIfNotSet is missing some logic for some of the onlyValidIfSpecifiedType param types (PositiveIntegerType, PositiveDecimalType), so just use NumericType for number fields 
                QQHelper.SetValueIfNotSet(.DoingBusinessAsName, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtDBA.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                WebHelper_Personal.SetValueIfNotSet_Local(.EntityTypeId, Me.ddBusinessType.SelectedValue, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                QQHelper.SetValueIfNotSet(.OtherLegalEntityDescription, Me.txtOtherEntity.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                WebHelper_Personal.SetValueIfNotSet_Local(.TaxTypeId, Me.ddTaxIDType.SelectedValue, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                QQHelper.SetValueIfNotSet(.TaxNumber, Me.txtFEIN.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                QQHelper.SetValueIfNotSet(.DescriptionOfOperations, Me.txtDescriptionOfOperations.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                WebHelper_Personal.SetValueIfNotSet_Local(.DateBusinessStarted, Me.txtBusinessStarted.Text, onlyValidIfSpecifiedType:=TypeToVerify.DateType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to empty string; note: could also check for isValidDataString to make sure it's greater than default date (1/1/1800)
                WebHelper_Personal.SetValueIfNotSet_Local(.YearsOfExperience, Me.txtYearsOfExperience.Text, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
            End With
            If String.IsNullOrWhiteSpace(Me.txtPhone.Text) = False OrElse QQHelper.IsPositiveIntegerString(Me.ddPhoneType.SelectedValue) = True OrElse String.IsNullOrWhiteSpace(Me.txtPhoneExt.Text) = False Then
                If .Phones Is Nothing Then
                    .Phones = New List(Of QuickQuotePhone)
                End If
                If .Phones.Count < 1 Then
                    .Phones.Add(New QuickQuotePhone)
                End If
                If .Phones(0) Is Nothing Then
                    .Phones(0) = New QuickQuotePhone
                End If
                With .Phones(0)
                    WebHelper_Personal.SetValueIfNotSet_Local(.TypeId, Me.ddPhoneType.SelectedValue, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                    QQHelper.SetValueIfNotSet(.Number, Me.txtPhone.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                    QQHelper.SetValueIfNotSet(.Extension, Me.txtPhoneExt.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                End With
            End If
            If String.IsNullOrWhiteSpace(Me.txtEmail.Text) = False OrElse QQHelper.IsPositiveIntegerString(Me.hdnEmailTypeId.Value) = True Then
                If .Emails Is Nothing Then
                    .Emails = New List(Of QuickQuoteEmail)
                End If
                If .Emails.Count < 1 Then
                    .Emails.Add(New QuickQuoteEmail)
                End If
                If .Emails(0) Is Nothing Then
                    .Emails(0) = New QuickQuoteEmail
                End If
                With .Emails(0)
                    WebHelper_Personal.SetValueIfNotSet_Local(.TypeId, Me.hdnEmailTypeId.Value, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                    QQHelper.SetValueIfNotSet(.Address, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtEmail.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                End With
            End If
            If .Address Is Nothing Then
                .Address = New QuickQuote.CommonObjects.QuickQuoteAddress
            End If
            With .Address
                'QQHelper.SetValueIfNotSet(.HouseNum, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtStreetNum.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                'QQHelper.SetValueIfNotSet(.StreetName, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtStreetName.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                'QQHelper.SetValueIfNotSet(.ApartmentNumber, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtAptNum.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                'QQHelper.SetValueIfNotSet(.POBox, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtPOBox.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                'updated 9/27/2023 to allow for change from streetAddress to poBox or vice versa - code will only get here if address passes validation
                If String.IsNullOrWhiteSpace(Me.txtStreetNum.Text) = False Then
                    QQHelper.SetValueIfNotSet(.HouseNum, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtStreetNum.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                ElseIf String.IsNullOrWhiteSpace(Me.txtPOBox.Text) = False Then 'probably not needed since address validation should catch it
                    .HouseNum = ""
                End If
                If String.IsNullOrWhiteSpace(Me.txtStreetName.Text) = False Then
                    QQHelper.SetValueIfNotSet(.StreetName, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtStreetName.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                ElseIf String.IsNullOrWhiteSpace(Me.txtPOBox.Text) = False Then 'probably not needed since address validation should catch it
                    .StreetName = ""
                End If
                If String.IsNullOrWhiteSpace(Me.txtAptNum.Text) = False Then
                    QQHelper.SetValueIfNotSet(.ApartmentNumber, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtAptNum.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                ElseIf String.IsNullOrWhiteSpace(Me.txtPOBox.Text) = False Then 'probably not needed since address validation should catch any problems... and aptNum would never be required anyway
                    'note: as written, this code will not let you wipe out aptNum on a street address unless you also have poBox
                    .ApartmentNumber = ""
                End If
                If String.IsNullOrWhiteSpace(Me.txtPOBox.Text) = False Then
                    QQHelper.SetValueIfNotSet(.POBox, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtPOBox.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                ElseIf String.IsNullOrWhiteSpace(Me.txtStreetNum.Text) = False AndAlso String.IsNullOrWhiteSpace(Me.txtStreetName.Text) = False Then 'probably not needed since address validation should catch it
                    .POBox = ""
                End If
                QQHelper.SetValueIfNotSet(.Zip, Me.txtZipCode.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                QQHelper.SetValueIfNotSet(.City, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtCityName.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                WebHelper_Personal.SetValueIfNotSet_Local(.StateId, Me.ddStateAbbrev.SelectedValue, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                QQHelper.SetValueIfNotSet(.County, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtGaragedCounty.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                QQHelper.SetValueIfNotSet(.Other, WebHelper_Personal.TextInUpperCaseAndTrimmed(Me.txtOther.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
            End With
        End With
        If QQHelper.BitToBoolean(Me.hdnHasPrefill.Value) = False OrElse IntegrationHelper.StringIsDifferentThanFirmographicsPrefillNameAddressString(Me.hdnPrefillOrderInfo.Value, ph, mustHaveAllRequiredFields:=True) = True Then
            firmIsMissingOrNeedsReOrder = True
        End If
    End Sub
    Private Sub SaveLocationInfo(ByRef locs As List(Of QuickQuoteLocation), Optional ByRef locNumsMissingPrefill As List(Of Integer) = Nothing, Optional ByRef locNumsNeedingPrefillReOrder As List(Of Integer) = Nothing, Optional ByRef locNumsChangedSinceLoad As List(Of Integer) = Nothing)
        locNumsMissingPrefill = Nothing
        locNumsNeedingPrefillReOrder = Nothing
        locNumsChangedSinceLoad = Nothing
        If rptLocations.Items IsNot Nothing AndAlso rptLocations.Items.Count > 0 Then
            If locs Is Nothing Then
                locs = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
            End If
            Dim locCount As Integer = 0
            For Each item As RepeaterItem In rptLocations.Items
                locCount += 1

                Dim currLoc As QuickQuoteLocation = Nothing
                If locs.Count >= locCount Then
                    currLoc = locs(locCount - 1)
                    If currLoc Is Nothing Then
                        currLoc = New QuickQuoteLocation
                    End If
                Else
                    currLoc = New QuickQuoteLocation
                    locs.Add(currLoc)
                End If
                With currLoc
                    If .Address Is Nothing Then
                        .Address = New QuickQuoteAddress
                    End If
                    With .Address
                        Dim txtLocStreetNum As TextBox = item.FindControl("txtLocStreetNum")
                        If txtLocStreetNum IsNot Nothing Then
                            QQHelper.SetValueIfNotSet(.HouseNum, WebHelper_Personal.TextInUpperCaseAndTrimmed(txtLocStreetNum.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                        End If
                        Dim txtLocStreetName As TextBox = item.FindControl("txtLocStreetName")
                        If txtLocStreetName IsNot Nothing Then
                            QQHelper.SetValueIfNotSet(.StreetName, WebHelper_Personal.TextInUpperCaseAndTrimmed(txtLocStreetName.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                        End If
                        Dim txtLocAptNum As TextBox = item.FindControl("txtLocAptNum")
                        If txtLocAptNum IsNot Nothing Then
                            QQHelper.SetValueIfNotSet(.ApartmentNumber, WebHelper_Personal.TextInUpperCaseAndTrimmed(txtLocAptNum.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                        End If
                        'Dim txtLocPOBox As TextBox = item.FindControl("txtLocPOBox")
                        'If txtLocPOBox IsNot Nothing Then
                        '    QQHelper.SetValueIfNotSet(.POBox, WebHelper_Personal.TextInUpperCaseAndTrimmed(txtLocPOBox.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                        'End If
                        Dim txtLocZipCode As TextBox = item.FindControl("txtLocZipCode")
                        If txtLocZipCode IsNot Nothing Then
                            QQHelper.SetValueIfNotSet(.Zip, txtLocZipCode.Text, okayToOverwrite:=True, neverSetItNotValid:=True)
                        End If
                        Dim txtLocCityName As TextBox = item.FindControl("txtLocCityName")
                        If txtLocCityName IsNot Nothing Then
                            QQHelper.SetValueIfNotSet(.City, WebHelper_Personal.TextInUpperCaseAndTrimmed(txtLocCityName.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                        End If
                        Dim ddLocStateAbbrev As DropDownList = item.FindControl("ddLocStateAbbrev")
                        If ddLocStateAbbrev IsNot Nothing Then
                            WebHelper_Personal.SetValueIfNotSet_Local(.StateId, ddLocStateAbbrev.SelectedValue, onlyValidIfSpecifiedType:=TypeToVerify.PositiveIntegerType, okayToOverwrite:=True, neverSetItNotValid:=True) 'note: neverSetItNotValid True means that it won't set to zero
                        End If
                        Dim txtLocGaragedCounty As TextBox = item.FindControl("txtLocGaragedCounty")
                        If txtLocGaragedCounty IsNot Nothing Then
                            QQHelper.SetValueIfNotSet(.County, WebHelper_Personal.TextInUpperCaseAndTrimmed(txtLocGaragedCounty.Text), okayToOverwrite:=True, neverSetItNotValid:=True)
                        End If

                        Dim hdnLocHasPrefill As HiddenField = item.FindControl("hdnLocHasPrefill")
                        If hdnLocHasPrefill IsNot Nothing Then
                            If QQHelper.BitToBoolean(hdnLocHasPrefill.Value) = False Then
                                QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(locCount, locNumsMissingPrefill)
                            Else
                                'has prefill; check for re-order
                                Dim hdnLocPrefillOrderInfo As HiddenField = item.FindControl("hdnLocPrefillOrderInfo")
                                If hdnLocPrefillOrderInfo IsNot Nothing Then
                                    If IntegrationHelper.StringIsDifferentThanPropertyPrefillAddressString(hdnLocPrefillOrderInfo.Value, currLoc.Address, mustHaveAllRequiredFields:=True) = True Then
                                        QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(locCount, locNumsNeedingPrefillReOrder)
                                    End If
                                End If
                            End If
                        End If

                        Dim hdnLocHasChangedSinceLoad As HiddenField = item.FindControl("hdnLocHasChangedSinceLoad")
                        If hdnLocHasChangedSinceLoad IsNot Nothing Then
                            If QQHelper.BitToBoolean(hdnLocHasChangedSinceLoad.Value) = True Then
                                QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(locCount, locNumsChangedSinceLoad)
                            End If
                        End If
                    End With
                End With
            Next
        End If
    End Sub
End Class