Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.EndorsementStructures
Imports QuickQuote.CommonMethods


Public Class ctlPolicyInfo
    Inherits VRControlBase

    Private _pastDateDays As Integer

    Public Property pastDateDays() As Integer
        Get
            If _pastDateDays = 0 Then

                'Dim configSetting As Integer
                'Dim pastDate = System.Configuration.ConfigurationManager.AppSettings("VR_Endorsement_Past_CreateDateMin")
                'If String.IsNullOrWhiteSpace(pastDate) OrElse Integer.TryParse(pastDate, configSetting) = False Then
                '    _pastDateDays = -30
                'Else
                '    _pastDateDays = configSetting
                'End If
                _pastDateDays = Common.Helpers.Endorsements.EndorsementHelper.EndorsementDaysBack()
            End If
            Return _pastDateDays
        End Get
        Set(ByVal value As Integer)
            _pastDateDays = value
        End Set
    End Property

    Private _futureDateDays As Integer

    Public Property futureDateDays() As Integer
        Get
            If _futureDateDays = 0 Then

                'Dim configSetting As Integer
                'Dim futureDate = System.Configuration.ConfigurationManager.AppSettings("VR_Endorsement_Future_CreateDateMax")
                'If String.IsNullOrWhiteSpace(futureDate) OrElse Integer.TryParse(futureDate, configSetting) = False Then
                '    _futureDateDays = 25
                'Else
                '    _futureDateDays = configSetting
                'End If
                _futureDateDays = Common.Helpers.Endorsements.EndorsementHelper.EndorsementDaysForward()
            End If
            Return _futureDateDays
        End Get
        Set(ByVal value As Integer)
            _futureDateDays = value
        End Set
    End Property

    Private _pastDate As DateTime

    Public Property pastDate() As DateTime
        Get
            '_pastDate = DateTime.Now.AddDays(pastDateDays)
            'updated 7/27/2019
            _pastDate = Date.Today.AddDays(pastDateDays)
            Return _pastDate
        End Get
        Set(ByVal value As DateTime)
            _pastDate = value
        End Set
    End Property

    Private _futureDate As DateTime

    Public Property futureDate() As DateTime
        Get
            '_futureDate = DateTime.Now.AddDays(futureDateDays)
            'updated 7/27/2019
            _futureDate = Date.Today.AddDays(futureDateDays)
            Return _futureDate
        End Get
        Set(ByVal value As DateTime)
            _futureDate = value
        End Set
    End Property

    Private _ValPopupTitleText As String

    Public Property ValPopupTitleText As String
        Get
            If String.IsNullOrWhiteSpace(_ValPopupTitleText) Then
                Dim popTitle = System.Configuration.ConfigurationManager.AppSettings("VR_Endorsement_ValPopupTitle")
                If String.IsNullOrWhiteSpace(popTitle) Then
                    _ValPopupTitleText = "Endorsment Creation Issue:"
                Else
                    _ValPopupTitleText = popTitle
                End If
            End If
            Return _ValPopupTitleText
        End Get
        Set(ByVal value As String)
            _ValPopupTitleText = value
        End Set
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            LoadTransactionTypeDdl()
        End If
        Populate()
    End Sub

    Protected Sub LoadTransactionTypeDdl()
        ' Added Informational Section Updates. CAH 8/17/2021
        Dim options As Dictionary(Of String, String) = New Dictionary(Of String, String)

        Select Case Quote.LobType
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                options.Add("", "")
                options.Add(EndorsementTypeString.CAP_AmendMailing, EndorsementTypeString.CAP_AmendMailing)
                options.Add(EndorsementTypeString.CAP_AddDeleteVehicle, EndorsementTypeString.CAP_AddDeleteVehicle)
                options.Add(EndorsementTypeString.CAP_AddDeleteDriver, EndorsementTypeString.CAP_AddDeleteDriver)
                options.Add(EndorsementTypeString.CAP_AddDeleteAI, EndorsementTypeString.CAP_AddDeleteAI)
                lblAuthority.Text = "The following are changes that you have authority to issue."
                lblLobLimitText.Text = "For changes involving more than 3 vehicles, lienholders, and/or drivers please refer to underwriting for processing."
                lblOtherChangeText.Text = "If you have a change request not listed above, please send your request to <a href='mailto:changes@indianafarmers.com' style='color:blue;'>changes@indianafarmers.com</a> and we will handle accordingly."
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                options.Add("", "")
                options.Add(EndorsementTypeString.BOP_AmendMailing, EndorsementTypeString.BOP_AmendMailing)
                options.Add(EndorsementTypeString.BOP_AddDeleteLocationLienholder, EndorsementTypeString.BOP_AddDeleteLocationLienholder)
                'options.Add(EndorsementTypeString.BOP_AddDeleteContractorsEquipmentLienholder, EndorsementTypeString.BOP_AddDeleteContractorsEquipmentLienholder)
                'options.Add(EndorsementTypeString.BOP_AddDeleteLocation, EndorsementTypeString.BOP_AddDeleteLocation)
                'options.Add(EndorsementTypeString.BOP_AddDeleteContractorsEquipment, EndorsementTypeString.BOP_AddDeleteContractorsEquipment)
                lblAuthority.Text = "The following are changes that you have authority to issue."
                lblLobLimitText.Text = "For changes involving more than 3 locations or lienholders, please refer to underwriting for processing."
                lblOtherChangeText.Text = "If you have a change request not listed above, please send your request to <a href='mailto:changes@indianafarmers.com' style='color:blue;'>changes@indianafarmers.com</a> and we will handle accordingly."
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                options.Add("", "")
                options.Add(EndorsementTypeString.CPP_AmendMailing, EndorsementTypeString.CPP_AmendMailing)
                options.Add(EndorsementTypeString.CPP_AddDeleteLocationLienholder, EndorsementTypeString.CPP_AddDeleteLocationLienholder)
                'Only allow contractors' equipment edit if already exists on the policy.
                ' 04/29/2022 CAH - Removed Contractors' Equipment because of mismatch in VR and AI Manager in Diamond.
                'If GoverningStateQuote.ContractorsEquipmentScheduledCoverages IsNot Nothing AndAlso GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Any() Then
                '    options.Add(EndorsementTypeString.CPP_AddDeleteContractorsEquipmentLienholder, EndorsementTypeString.CPP_AddDeleteContractorsEquipmentLienholder)
                'End If
                lblAuthority.Text = "The following are changes that you have authority to issue."
                lblLobLimitText.Text = "For changes involving more than 3 locations or property lienholders (First Mortgagees or Loss Payees for Property coverage), please refer to underwriting for processing."
                lblOtherChangeText.Text = "If you have a change request not listed above, please send your request to <a href='mailto:changes@indianafarmers.com' style='color:blue;'>changes@indianafarmers.com</a> and we will handle accordingly."
            Case Else
                lblAuthority.Text = "The following are changes that you have authority to issue."
                lblLobLimitText.Text = "For changes involving more than 3 Items, please refer to underwriting for processing."
                lblOtherChangeText.Text = "If you have a change request not listed above, please send your request to <a href='mailto:changes@indianafarmers.com' style='color:blue;'>changes@indianafarmers.com</a> and we will handle accordingly."
        End Select

        If options.Count > 0 Then
            UpdateTransactionTypeDdl(options)

            For Each item In options
                If Not String.IsNullOrWhiteSpace(item.Key) Then
                    Dim newLi = New HtmlGenericControl("li")
                    newLi.InnerText = item.Value
                    Me.ListOfChanges.Controls.Add(newLi)
                End If
            Next
        End If

    End Sub

    Protected Sub UpdateTransactionTypeDdl(options As Dictionary(Of String, String))
        ddlTypeOfEndorsement.DataSource = options
        ddlTypeOfEndorsement.DataTextField = "Key"
        ddlTypeOfEndorsement.DataValueField = "Value"
        ddlTypeOfEndorsement.DataBind()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(divPolicyInfo.ClientID, hdnAccordGenInfo, "0")
        Me.VRScript.CreateConfirmDialog(btnCancel.ClientID, "Cancel this policy change?")
        Me.VRScript.AddScriptLine("$('#" + Me.txtTransEffectDate.ClientID + "').mask('00/00/0000');")
        Me.VRScript.AddScriptLine("$(document).ready(function () { $('#" + Me.txtTransEffectDate.ClientID + "').datepicker({ changeMonth: true, changeYear: true, minDate: '" + pastDate.ToShortDateString + "', maxDate: '" + futureDate.ToShortDateString + "', showButtonPanel: true, gotoCurrent: true });});")
        ' Use below to force Today button to enter the current day's date in the text box, original function is to jump the calendar to today only
        'Me.VRScript.AddScriptLine("$.datepicker._gotoToday = function(id) { $(id).datepicker('setDate', new Date());};")
        ' Theme Selection (billing query string?)
        If IsBillingUpdate() Then
            Me.VRScript.AddScriptLine("ifm.vr.theming.LoadThemeFromCookie(""PayplanChange"");")
        Else
            Me.VRScript.AddScriptLine("ifm.vr.theming.LoadThemeFromCookie(""Endorsement"");")
        End If

        'Updated 12/14/2020 For CAP Endorsements task 52969 MLW
        If Me.Quote.LobType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
            Me.txtRemarks.CreateWatermark("Please enter a valid description of the changes.")
        End If

    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            'Added 03/29/2021 for CAP Endorsements Task 52969 MLW
            divMaxTransactionsMessage.Visible = False

            LoadStaticData()

            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso IsBillingUpdate() AndAlso IsDate(Quote.ExpirationDate) AndAlso DateDiff(DateInterval.Day, Date.Today, CDate(Quote.ExpirationDate)) <= 27 Then
                txtTransEffectDate.Text = Quote.ExpirationDate
                txtTransEffectDate.Enabled = False
                lblTransEffDateRenewal.Visible = True
            End If

            'Added 11/13/2020 For CAP Endorsements task 52969 MLW

            If IsCommercialQuote() Then

                If PreventMultipleEndorsementsPerDay() = True Then
                    divMaxTransactionsMessage.Visible = True
                    btnSubmit.Enabled = False
                End If

                typeOfTransaction.Visible = True
                Me.lblPH1Name.Text = "Name:"
                Me.lblPH2Name.Text = "DBA Name:"
                Me.remarksArea.Visible = False
                Me.divRemarksPersonal.Visible = False
                Me.divRemarksCommercial.Visible = True

            Else
                typeOfTransaction.Visible = False
                Me.lblPH1Name.Text = "Policyholder 1 Name:"
                Me.lblPH2Name.Text = "Policyholder 2 Name:"
                Me.divRemarksPersonal.Visible = True
                Me.divRemarksCommercial.Visible = False
            End If

            Dim ph As QuickQuotePolicyholder = Nothing
            Dim ph2 As QuickQuotePolicyholder = Nothing
            Dim ph1Name As String = String.Empty
            Dim ph2Name As String = String.Empty
            Dim PolicyTerm As String = String.Empty

            If Quote.Policyholder.HasData Then
                ph = Quote.Policyholder
            End If
            If Quote.Policyholder2.HasData Then
                ph2 = Quote.Policyholder2
            End If

            PolicyTerm = Quote.EffectiveDate + " - " + Quote.ExpirationDate

            Me.txtPolicyTerm.Text = PolicyTerm

            If ph.Name IsNot Nothing Then
                If ph.Name.TypeId <> "2" Then
                    ph1Name = "{0} {1} {2} {3}".FormatIFM(ph.Name.FirstName.ToMaxLength(50), ph.Name.MiddleName.ToMaxLength(50), ph.Name.LastName.ToMaxLength(50), ph.Name.SuffixName)

                    'Title Name
                    Me.lblInsuredTitle.Text = "Policy Information - " + ph1Name
                    'Textbox Name
                    Me.txtPH1Name.Text = ph1Name

                    'Adjust Title and add to Textbox
                    If Quote.Policyholder2.HasData Then
                        If ph2.Name.HasData Then
                            ph2Name = "{0} {1} {2} {3}".FormatIFM(ph2.Name.FirstName.ToMaxLength(50), ph2.Name.MiddleName.ToMaxLength(50), ph2.Name.LastName.ToMaxLength(50), ph2.Name.SuffixName).ToMaxLength(150)
                            Me.txtPh2Name.Text = ph2Name
                            Me.lblInsuredTitle.Text = Me.lblInsuredTitle.Text + " and " + ph2Name
                        End If
                    Else
                        Me.txtPh2Name.Text = "N/A"
                    End If
                Else
                    ph1Name = "{0}".FormatIFM(ph.Name.CommercialName1.ToMaxLength(50))
                    'Title Name
                    Me.lblInsuredTitle.Text = "Policy Information - " + ph1Name
                    'Textbox Name
                    Me.txtPH1Name.Text = ph1Name
                    'Updated for BOP Endorsements task 65326 MLW
                    Select Case Me.Quote.LobType
                        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto _
                            , QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                            ph2Name = "{0}".FormatIFM(ph.Name.DoingBusinessAsName.ToMaxLength(50))
                            Me.txtPh2Name.Text = ph2Name
                    End Select
                    ''Added 11/13/2020 For CAP Endorsements task 52969 MLW
                    'If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                    '    ph2Name = "{0}".FormatIFM(ph.Name.DoingBusinessAsName.ToMaxLength(50))
                    '    Me.txtPh2Name.Text = ph2Name
                    'End If
                End If
                Me.lblInsuredTitle.Text = Me.lblInsuredTitle.Text.Ellipsis(55)


                ' Address Info is always pulled from Ph#1
                If Me.Quote.Policyholder.Address IsNot Nothing Then
                    Me.txtStreetNum.Text = Quote.Policyholder.Address.HouseNum
                    Me.txtStreetName.Text = Quote.Policyholder.Address.StreetName
                    Me.txtAptNum.Text = Quote.Policyholder.Address.ApartmentNumber
                    Me.txtPOBox.Text = Quote.Policyholder.Address.POBox
                    Me.txtCityName.Text = Quote.Policyholder.Address.City
                    Me.txtStateAbbrev.Text = Quote.Policyholder.Address.State

                    Me.txtZipCode.Text = Quote.Policyholder.Address.Zip.RemoveAny("00000")
                    Me.txtGaragedCounty.Text = Quote.Policyholder.Address.County
                End If

            End If

        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Me.ValidationHelper.GroupName = "Policy Infomation"

        ' Cut out Effective Date Validation - should be more testable.
        ValidateEffectiveDate(txtTransEffectDate.Text)

        'Updated 11/13/2020 For CAP Endorsements task 52969 MLW
        If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto _
           OrElse Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP _
           OrElse Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
            ValidateTypeOfEndorsement(ddlTypeOfEndorsement.SelectedValue)
        Else
            ' Cut out Remarks Validation - should be more testable.
            ValidateRemarks(txtRemarks.Text)
        End If

    End Sub

    Public Sub ValidateEffectiveDate(effectiveDate As String)
        Dim ValList = IFM.VR.Validation.ObjectValidation.AllLines.EndorsementValidator.ValidateEndorsementEffectiveDate(effectiveDate, pastDate, futureDate, Quote)

        For Each ValError In ValList
            Select Case ValError.FieldId
                Case IFM.VR.Validation.ObjectValidation.AllLines.EndorsementValidator.EndorsementEffectiveDateMissing
                    Me.ValidationHelper.AddError(ValError.Message, txtTransEffectDate.ClientID, True, ValPopupTitleText)
                Case IFM.VR.Validation.ObjectValidation.AllLines.EndorsementValidator.EndorsementEffectiveDateInvalid
                    Me.ValidationHelper.AddError(ValError.Message, txtTransEffectDate.ClientID, True, ValPopupTitleText)
                Case IFM.VR.Validation.ObjectValidation.AllLines.EndorsementValidator.EndorsementEffectiveDateOutOfPolicy
                    Me.ValidationHelper.AddError(ValError.Message, txtTransEffectDate.ClientID, True, ValPopupTitleText)
                Case IFM.VR.Validation.ObjectValidation.AllLines.EndorsementValidator.EndorsementEffectiveDateOutOfPolicyLongMessage 'added 7/25/2019; don't bind to textbox since it's too long
                    Me.ValidationHelper.AddError(ValError.Message, True, ValPopupTitleText)
            End Select
        Next
    End Sub

    'Added 11/13/2020 For CAP Endorsements task 52969 MLW
    Public Sub ValidateTypeOfEndorsement(typeOfEndorsement As String)
        Dim ValList = IFM.VR.Validation.ObjectValidation.AllLines.EndorsementValidator.ValidateTypeOfEndorsement(typeOfEndorsement)

        For Each ValError In ValList
            Select Case ValError.FieldId
                Case IFM.VR.Validation.ObjectValidation.AllLines.EndorsementValidator.EndorsementType
                    Me.ValidationHelper.AddError(ValError.Message, ddlTypeOfEndorsement.ClientID, True, ValPopupTitleText)
            End Select
        Next
    End Sub

    Public Sub ValidateRemarks(remarks As String)
        Dim ValList = IFM.VR.Validation.ObjectValidation.AllLines.EndorsementValidator.ValidateEndorsementRemarks(remarks)

        For Each ValError In ValList
            Select Case ValError.FieldId
                Case IFM.VR.Validation.ObjectValidation.AllLines.EndorsementValidator.EndorsementRemarks
                    Me.ValidationHelper.AddError(ValError.Message, txtRemarks.ClientID, True, ValPopupTitleText)
            End Select
        Next
    End Sub

    Public Overrides Function Save() As Boolean
        Me.ValidateControl(Nothing)
        If Me.ValidationSummmary.ValidationItems.Count = 0 Then
            'Do Creation
            Dim endPolicyId As Integer = 0
            Dim endPolicyImageNum As Integer = 0
            Dim endQQO As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
            Dim newEndorsementErrorMsg As String = ""
            Dim newEndorsementImageNum As Integer = 0
            Dim latestEndorsementImageNum As Integer = 0
            'Dim imgQueryString As String = String.Empty 'removed 6/11/2019; logic now to be performed by helper method
            Dim transEffectiveDate As DateTime
            Dim typeOfEndorsement As String = "" 'Added 12/14/2020 for CAP Endorsements Task 52969 MLW
            Dim remarks As String = "" 'Added 12/14/2020 for CAP Endorsements Task 52969 MLW
            Dim transactionReasonId As Integer = 0 'Added 04/27/2021 for CAP Endorsements Task 52969 MLW
            Dim devDictionaryKeys As List(Of QuickQuoteGenericObjectWithTwoStringProperties) = Nothing 'Added 04/27/2021 for CAP Endorsements Task 52969 MLW
            Dim objectDelimiter As String = "&&" 'Added 06/02/2021 for CAP Endorsements Task 52974 MLW
            Dim propDelimiter As String = "==" 'Added 06/02/2021 for CAP Endorsements Task 52974 MLW

            Dim lobTypeToUse As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.None

            If Quote.LobType Then
                lobTypeToUse = Quote.LobType
            End If

            If Not Date.TryParse(txtTransEffectDate.Text, transEffectiveDate) Then
                transEffectiveDate = Date.Today 'Today?
            End If

            'Added 12/14/2020 for CAP Endorsements Task 52969
            Select Case Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    typeOfEndorsement = ddlTypeOfEndorsement.SelectedValue
                    If typeOfEndorsement = EndorsementTypeString.CAP_AmendMailing Then
                        remarks = "AMENDING MAILING ADDRESS PER ABOVE."
                    End If
                    transactionReasonId = 10168 'Endorsement Change Dec Only - used on all CAP Endorsements except Add Vehicle and Add AI which use 10169, Endorsement Change Dec and Full Revised Dec.
                    Dim ddItem As New QuickQuoteGenericObjectWithTwoStringProperties With {
                        .Property1 = "Type_Of_Endorsement_Selected",
                        .Property2 = typeOfEndorsement
                    }
                    If devDictionaryKeys Is Nothing Then
                        devDictionaryKeys = New List(Of QuickQuoteGenericObjectWithTwoStringProperties)
                    End If
                    devDictionaryKeys.Add(ddItem)

                    'Added 06/02/2021 for CAP Endorsements Task 52974 MLW - need to save original vehicle AI assignments to DevDictionary
                    'If typeOfEndorsement = EndorsementTypeString.CAP_AddDeleteAI Then
                    '    Dim originalVehicleAIList As String = ""
                    '    If Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
                    '        For Each v As QuickQuoteVehicle In Quote.Vehicles
                    '            If v.AdditionalInterests IsNot Nothing AndAlso v.AdditionalInterests.Count > 0 Then
                    '                For Each vai As QuickQuoteAdditionalInterest In v.AdditionalInterests
                    '                    If vai.ListId IsNot Nothing Then
                    '                        If Not originalVehicleAIList.Contains(v.DisplayNum & "==") Then
                    '                            If originalVehicleAIList <> "" Then
                    '                                originalVehicleAIList &= objectDelimiter
                    '                            End If
                    '                            originalVehicleAIList &= v.DisplayNum & propDelimiter & vai.ListId
                    '                        End If
                    '                    End If
                    '                Next
                    '            End If
                    '        Next
                    '    End If
                    '    If originalVehicleAIList <> "" Then
                    '        Dim vehicleAIItem As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    '        .Property1 = "CAPEndorsements_Original_Vehicle_AIs",
                    '        .Property2 = originalVehicleAIList
                    '        }
                    '        devDictionaryKeys.Add(vehicleAIItem)
                    '    End If
                    'End If
                    'Added 06/29/2021 for CAP Endorsements Task 52974 MLW
                    Dim clearDevDictionary As New QuickQuoteGenericObjectWithTwoStringProperties With {
                                .Property1 = "CAPEndorsementsDetails",
                                .Property2 = ""
                            }
                    devDictionaryKeys.Add(clearDevDictionary)

                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP

                    typeOfEndorsement = ddlTypeOfEndorsement.SelectedValue
                    If typeOfEndorsement = EndorsementTypeString.BOP_AmendMailing Then
                        remarks = "AMENDING MAILING ADDRESS PER ABOVE."
                    End If
                    transactionReasonId = 10168 'Endorsement Change Dec Only - used on all BOP, Add AI which use 10169, Endorsement Change Dec and Full Revised Dec.
                    Dim ddItem As New QuickQuoteGenericObjectWithTwoStringProperties With {
                        .Property1 = "Type_Of_Endorsement_Selected",
                        .Property2 = typeOfEndorsement
                    }
                    If devDictionaryKeys Is Nothing Then
                        devDictionaryKeys = New List(Of QuickQuoteGenericObjectWithTwoStringProperties)
                    End If
                    devDictionaryKeys.Add(ddItem)

                    'Dim AllExisting = New DevDictionaryHelper.AllPreExistingItems()
                    'AllExisting.ConvertAllPreExistingIntoDevDictionary(Quote)

                    'Dim AddInterets As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    '    .Property1 = AllExisting.PreExisting_AdditionalInterests.LibraryName,
                    '    .Property2 = AllExisting.PreExisting_AdditionalInterests.ToString
                    '    }
                    'devDictionaryKeys.Add(AddInterets)
                    'Dim AppliedAddInterets As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    '    .Property1 = AllExisting.PreExisting_AssignedAdditionalInterests.LibraryName,
                    '    .Property2 = AllExisting.PreExisting_AssignedAdditionalInterests.ToString
                    '    }
                    'devDictionaryKeys.Add(AppliedAddInterets)
                    'Dim locations As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    '    .Property1 = AllExisting.PreExisting_Locations.LibraryName,
                    '    .Property2 = AllExisting.PreExisting_Locations.ToString
                    '    }
                    'devDictionaryKeys.Add(locations)

                Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage

                    typeOfEndorsement = ddlTypeOfEndorsement.SelectedValue
                    If typeOfEndorsement = EndorsementStructures.EndorsementTypeString.CPP_AmendMailing Then
                        remarks = "AMENDING MAILING ADDRESS PER ABOVE."
                    End If
                    transactionReasonId = 10168 'Endorsement Change Dec Only - used on all BOP, Add AI which use 10169, Endorsement Change Dec and Full Revised Dec.
                    Dim ddItem As New QuickQuoteGenericObjectWithTwoStringProperties With {
                        .Property1 = "Type_Of_Endorsement_Selected",
                        .Property2 = typeOfEndorsement
                    }
                    If devDictionaryKeys Is Nothing Then
                        devDictionaryKeys = New List(Of QuickQuoteGenericObjectWithTwoStringProperties)
                    End If
                    devDictionaryKeys.Add(ddItem)

                    'Dim AllExisting = New DevDictionaryHelper.AllPreExistingItems()
                    'AllExisting.ConvertAllPreExistingIntoDevDictionary(Quote)

                    'Dim AddInterets As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    '    .Property1 = AllExisting.PreExisting_AdditionalInterests.LibraryName,
                    '    .Property2 = AllExisting.PreExisting_AdditionalInterests.ToString
                    '    }
                    'devDictionaryKeys.Add(AddInterets)
                    'Dim AppliedAddInterets As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    '    .Property1 = AllExisting.PreExisting_AssignedAdditionalInterests.LibraryName,
                    '    .Property2 = AllExisting.PreExisting_AssignedAdditionalInterests.ToString
                    '    }
                    'devDictionaryKeys.Add(AppliedAddInterets)
                    'Dim locations As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    '    .Property1 = AllExisting.PreExisting_Locations.LibraryName,
                    '    .Property2 = AllExisting.PreExisting_Locations.ToString
                    '    }
                    'devDictionaryKeys.Add(locations)
                    'Dim CglClassCodes As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    '    .Property1 = AllExisting.PreExisting_CglClassCodes.LibraryName,
                    '    .Property2 = AllExisting.PreExisting_CglClassCodes.ToString
                    '    }
                    'devDictionaryKeys.Add(CglClassCodes)

                    'Dim IMContractorsEquip As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    '    .Property1 = AllExisting.PreExisting_InlandMarineCoveragesWithPremium.LibraryName,
                    '    .Property2 = AllExisting.PreExisting_InlandMarineCoveragesWithPremium.ToString
                    '    }
                    'devDictionaryKeys.Add(IMContractorsEquip)


                Case Else
                    remarks = txtRemarks.Text
            End Select



            'If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
            '    Dim HadWoodburning As Boolean = False
            '    If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Any() AndAlso Quote.Locations(0) IsNot Nothing Then
            '        HadWoodburning = Quote.Locations(0).WoodOrFuelBurningApplianceSurcharge
            '    End If
            '    If HadWoodburning Then
            '        Dim ddItem As New QuickQuoteGenericObjectWithTwoStringProperties With {
            '        .Property1 = "HadWoodburningSurchargeOnPreviousImage",
            '        .Property2 = HadWoodburning.ToString
            '    }
            '        If devDictionaryKeys Is Nothing Then
            '            devDictionaryKeys = New List(Of QuickQuoteGenericObjectWithTwoStringProperties)
            '        End If
            '        devDictionaryKeys.Add(ddItem)
            '    End If
            'End If

            'ProcessPreExistingAfterCreation(Quote)
            'Dim qqMethodToUse As QuickQuote.CommonMethods.QuickQuoteXML.CopyPreExistingItemsToDevDictMethod = Nothing
            Dim qqMethodToUse As QuickQuote.CommonMethods.QuickQuoteXML.DelegateMethod = AddressOf ProcessPreExistingAfterCreation

            'Updated 12/14/2020 and 04/27/2021 for CAP Endorsements Task 52969 MLW
            endQQO = VR.Common.QuoteSave.QuoteSaveHelpers.NewEndorsementQuoteForPolicyIdTransactionDate_TransactionReasonDevDictionaryKeys(Quote.PolicyId, transEffectiveDate.ToShortDateString, transactionReasonId:=transactionReasonId, endorsementRemarks:=remarks, devDictionaryKeys:=devDictionaryKeys, newPolicyImageNum:=newEndorsementImageNum, latestPendingEndorsementImageNum:=latestEndorsementImageNum, errorMessage:=newEndorsementErrorMsg, isBillingUpdate:=IsBillingUpdate, daysBack:=pastDateDays, daysForward:=futureDateDays, MethodToUse:=qqMethodToUse)
            ''endQQO = VR.Common.QuoteSave.QuoteSaveHelpers.NewEndorsementQuoteForPolicyIdTransactionDate(Quote.PolicyId, transEffectiveDate.ToShortDateString, endorsementRemarks:=txtRemarks.Text, newPolicyImageNum:=newEndorsementImageNum, latestPendingEndorsementImageNum:=latestEndorsementImageNum, errorMessage:=newEndorsementErrorMsg)
            ''updated 7/27/2019
            'endQQO = VR.Common.QuoteSave.QuoteSaveHelpers.NewEndorsementQuoteForPolicyIdTransactionDate(Quote.PolicyId, transEffectiveDate.ToShortDateString, endorsementRemarks:=txtRemarks.Text, newPolicyImageNum:=newEndorsementImageNum, latestPendingEndorsementImageNum:=latestEndorsementImageNum, errorMessage:=newEndorsementErrorMsg, isBillingUpdate:=IsBillingUpdate, daysBack:=pastDateDays, daysForward:=futureDateDays)
            If String.IsNullOrWhiteSpace(newEndorsementErrorMsg) = True AndAlso endQQO IsNot Nothing AndAlso newEndorsementImageNum > 0 Then
                endPolicyId = Quote.PolicyId
                endPolicyImageNum = newEndorsementImageNum
            ElseIf latestEndorsementImageNum > 0 Then
                endPolicyId = Quote.PolicyId
                endPolicyImageNum = latestEndorsementImageNum
            End If

            Dim quoteStatus As QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType = Nothing '6/11/2019 - added new variable for updated logic

            If endPolicyId > 0 AndAlso endPolicyImageNum > 0 Then
                If endQQO Is Nothing Then
                    endQQO = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteFromAnywhere(endPolicyId, endPolicyImageNum)
                End If
                If endQQO IsNot Nothing Then
                    'lobTypeToUse = endQQO.LobType
                    'imgQueryString = "?EndorsementPolicyIdAndImageNum=" & endPolicyId.ToString & "|" & endPolicyImageNum.ToString 'removed 6/11/2019; logic now to be performed by helper method

                    'added 3/25/2019; removed 6/11/2019; logic now to be performed by helper method
                    'If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType), endQQO.QuoteStatus) = True Then
                    '    Select Case endQQO.QuoteStatus
                    '        Case QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppGapRated, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppGapRatingFailed, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteRated, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteRatingFailed
                    '            imgQueryString &= "&" & IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs & "=" & IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                    '    End Select
                    'End If
                    '6/11/2019 - new logic for helper method
                    quoteStatus = endQQO.QuoteStatus
                    If System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteLobType), endQQO.LobType) = True AndAlso endQQO.LobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                        lobTypeToUse = endQQO.LobType
                    End If
                End If
            End If

            'Added 12/14/2020 and 04/29/2021 for CAP Endorsements Task 52969 MLW
            If endQQO IsNot Nothing AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                'Dim doEndorsementSave As Boolean = False
                'some policies from Diamond do not have a location, need to copy the policyholder address to quote.locations
                If endQQO.Locations Is Nothing OrElse endQQO.Locations.Count = 0 Then
                    copyPolicyholderAddressToQuoteLocations(endQQO)
                    'doEndorsementSave = True
                End If
                'some older policies from Diamond do not have a vehicle garaging address, copy location/policyholder address
                If endQQO.Vehicles IsNot Nothing AndAlso endQQO.Vehicles.Count > 0 Then
                    Dim garagingAddressChanged As Boolean = evaluateVehicleGaragingAddress(endQQO)
                    'If garagingAddressChanged = True Then
                    '    doEndorsementSave = True
                    'End If
                End If

            End If

            ' 04/08/2022 CAH - This Pulls PreExisting from latest image(endQQO) instead of Active Quote.
            'ProcessPreExistingAfterCreation(endQQO)

            'Dim savedSuccessfully As Boolean = False
            'Dim savedErrorMessage As String = ""
            'savedSuccessfully = IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(endQQO, errorMessage:=savedErrorMessage)


            'Redirect
            'If String.IsNullOrWhiteSpace(imgQueryString) = False AndAlso System.Enum.IsDefined(GetType(QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType), lobTypeToUse) = True AndAlso lobTypeToUse <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.None Then
            '    Dim useAppPage As Boolean = False
            '    If Request.QueryString("App") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("App").ToString) = False AndAlso (UCase(Request.QueryString("App").ToString) = "YES" OrElse QQHelper.BitToBoolean(Request.QueryString("App").ToString) = True) Then
            '        useAppPage = True
            '    End If

            '    Select Case Quote.LobType
            '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal
            '            If useAppPage = True Then
            '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_App") & imgQueryString)
            '            Else
            '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_Input") & imgQueryString, True)
            '            End If
            '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal
            '            If useAppPage = True Then
            '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_App") & imgQueryString)
            '            Else
            '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_Input") & imgQueryString, True)
            '            End If
            '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
            '            If useAppPage = True Then
            '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_App") & imgQueryString)
            '            Else
            '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_Input") & imgQueryString, True)
            '            End If
            '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm
            '            If useAppPage = True Then
            '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_App") & imgQueryString)
            '            Else
            '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_Input") & imgQueryString, True)
            '            End If
            '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
            '            If IFM.VR.Common.VRFeatures.NewLookBOPEnabled Then
            '                If useAppPage = True Then
            '                    Response.Redirect("VR3BOPApp.aspx" & imgQueryString)
            '                Else
            '                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_BOP_Quote_NewLook") & imgQueryString, True)
            '                End If
            '            End If
            '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
            '            If IFM.VR.Common.VRFeatures.NewLookWCPEnabled Then
            '                If useAppPage = True Then
            '                    Response.Redirect("VR3WCPApp.aspx" & imgQueryString)
            '                Else
            '                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_WCP_Quote_NewLook") & imgQueryString, True)
            '                End If
            '            End If
            '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
            '            If IFM.VR.Common.VRFeatures.NewLookCAPEnabled Then
            '                If useAppPage = True Then
            '                    Response.Redirect("VR3CAPApp.aspx" & imgQueryString)
            '                Else
            '                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CAP_Quote_NewLook") & imgQueryString, True)
            '                End If
            '            End If
            '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
            '            If IFM.VR.Common.VRFeatures.NewLookCGLEnabled Then
            '                If useAppPage = True Then
            '                    Response.Redirect("VR3CGLApp.aspx" & imgQueryString)
            '                Else
            '                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CGL_Quote_NewLook") & imgQueryString, True)
            '                End If
            '            End If
            '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
            '            If IFM.VR.Common.VRFeatures.NewLookCPREnabled Then
            '                If useAppPage = True Then
            '                    Response.Redirect("VR3CPRApp.aspx" & imgQueryString)
            '                Else
            '                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPR_Quote_NewLook") & imgQueryString, True)
            '                End If
            '            End If
            '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
            '            If IFM.VR.Common.VRFeatures.NewLookCPPEnabled Then
            '                If useAppPage = True Then
            '                    Response.Redirect("VR3CPPApp.aspx" & imgQueryString)
            '                Else
            '                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPP_Quote_NewLook") & imgQueryString, True)
            '                End If
            '            End If
            '    End Select
            'Else
            '    Me.ValidationHelper.AddError(newEndorsementErrorMsg, True, ValPopupTitleText)
            'End If
            'updated 6/11/2019 to use new helper method
            Dim showError As Boolean = False
            If endQQO IsNot Nothing Then
                If System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteLobType), lobTypeToUse) = False OrElse lobTypeToUse = QuickQuoteObject.QuickQuoteLobType.None Then
                    If String.IsNullOrWhiteSpace(newEndorsementErrorMsg) = True Then
                        newEndorsementErrorMsg = "Invalid LOB for VelociRater."
                    End If
                    showError = True
                Else
                    IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, lobTypeToUse, policyId:=endPolicyId, policyImageNum:=endPolicyImageNum, quoteStatus:=quoteStatus, goToApp:=False, isBillingUpdate:=IsBillingUpdate) 'updated 7/28/2019 for isBillingUpdate
                End If
            Else
                showError = True
            End If
            If showError = True Then
                Me.ValidationHelper.AddError(newEndorsementErrorMsg, True, ValPopupTitleText)
            End If
        End If

    End Function

    Public Sub ProcessPreExistingAfterCreation(ByRef myQuote As QuickQuoteObject)

        'Do Creation
        Dim typeOfEndorsement As String = ""
        Dim devDictionaryKeys As List(Of QuickQuoteGenericObjectWithTwoStringProperties) = New List(Of QuickQuoteGenericObjectWithTwoStringProperties)
        Dim objectDelimiter As String = "&&"
        Dim propDelimiter As String = "=="

        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                Dim originalPayplan As String = String.Empty
                'Reflects Current PayPlan ID of latest endorsement image
                'BillingPayplanId - only represents the Active Image
                If myQuote IsNot Nothing AndAlso myQuote.CurrentPayplanId IsNot Nothing Then
                    originalPayplan = myQuote.CurrentPayplanId
                End If
                If originalPayplan Then
                    Dim ddItem As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    .Property1 = "PpaPayplanOnPreviousImage",
                    .Property2 = originalPayplan.ToString
                }
                    If devDictionaryKeys Is Nothing Then
                        devDictionaryKeys = New List(Of QuickQuoteGenericObjectWithTwoStringProperties)
                    End If
                    devDictionaryKeys.Add(ddItem)
                End If
            Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                Dim HadWoodburning As Boolean = False
                If myQuote.Locations IsNot Nothing AndAlso myQuote.Locations.Any() AndAlso myQuote.Locations(0) IsNot Nothing Then
                    HadWoodburning = myQuote.Locations(0).WoodOrFuelBurningApplianceSurcharge
                End If
                If HadWoodburning Then
                    Dim ddItem As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    .Property1 = "HadWoodburningSurchargeOnPreviousImage",
                    .Property2 = HadWoodburning.ToString
                }
                    If devDictionaryKeys Is Nothing Then
                        devDictionaryKeys = New List(Of QuickQuoteGenericObjectWithTwoStringProperties)
                    End If
                    devDictionaryKeys.Add(ddItem)
                End If
            Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                Dim isUnderConstruction As Boolean = False
                If myQuote.Locations IsNot Nothing AndAlso myQuote.Locations.Any() AndAlso myQuote.Locations(0) IsNot Nothing Then
                    isUnderConstruction = myQuote.Locations(0).OccupancyCodeId = "7" 'Under Construction
                End If
                If isUnderConstruction Then
                    Dim ddItem As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    .Property1 = "UnderConstructionOnPreviousImage",
                    .Property2 = isUnderConstruction.ToString
                }
                    If devDictionaryKeys Is Nothing Then
                        devDictionaryKeys = New List(Of QuickQuoteGenericObjectWithTwoStringProperties)
                    End If
                    devDictionaryKeys.Add(ddItem)
                End If
            Case QuickQuoteObject.QuickQuoteLobType.Farm
                'Added 10/4/2022 for bug 75312 MLW
                Dim AllExisting = New DevDictionaryHelper.AllPreExistingItems()
                AllExisting.ConvertAllPreExistingIntoDevDictionary(myQuote)

                Dim locations As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    .Property1 = AllExisting.PreExisting_Locations.LibraryName,
                    .Property2 = AllExisting.PreExisting_Locations.ToString
                    }
                devDictionaryKeys.Add(locations)
            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                'Added 06/02/2021 for CAP Endorsements Task 52974 MLW - need to save original vehicle AI assignments to DevDictionary
                If typeOfEndorsement = EndorsementTypeString.CAP_AddDeleteAI Then
                    Dim originalVehicleAIList As String = ""
                    If myQuote.Vehicles IsNot Nothing AndAlso myQuote.Vehicles.Count > 0 Then
                        For Each v As QuickQuoteVehicle In myQuote.Vehicles
                            If v.AdditionalInterests IsNot Nothing AndAlso v.AdditionalInterests.Count > 0 Then
                                For Each vai As QuickQuoteAdditionalInterest In v.AdditionalInterests
                                    If vai.ListId IsNot Nothing Then
                                        If Not originalVehicleAIList.Contains(v.DisplayNum & "==") Then
                                            If originalVehicleAIList <> "" Then
                                                originalVehicleAIList &= objectDelimiter
                                            End If
                                            originalVehicleAIList &= v.DisplayNum & propDelimiter & vai.ListId
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If
                    If originalVehicleAIList <> "" Then
                        Dim vehicleAIItem As New QuickQuoteGenericObjectWithTwoStringProperties With {
                        .Property1 = "CAPEndorsements_Original_Vehicle_AIs",
                        .Property2 = originalVehicleAIList
                        }
                        devDictionaryKeys.Add(vehicleAIItem)
                    End If
                End If
                'Added 06/29/2021 for CAP Endorsements Task 52974 MLW
                Dim clearDevDictionary As New QuickQuoteGenericObjectWithTwoStringProperties With {
                            .Property1 = "CAPEndorsementsDetails",
                            .Property2 = ""
                        }
                devDictionaryKeys.Add(clearDevDictionary)

            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP

                Dim AllExisting = New DevDictionaryHelper.AllPreExistingItems()
                AllExisting.ConvertAllPreExistingIntoDevDictionary(myQuote)

                Dim AddInterests As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    .Property1 = AllExisting.PreExisting_AdditionalInterests.LibraryName,
                    .Property2 = AllExisting.PreExisting_AdditionalInterests.ToString
                    }
                devDictionaryKeys.Add(AddInterests)
                Dim AppliedAddInterests As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    .Property1 = AllExisting.PreExisting_AssignedAdditionalInterests.LibraryName,
                    .Property2 = AllExisting.PreExisting_AssignedAdditionalInterests.ToString
                    }
                devDictionaryKeys.Add(AppliedAddInterests)
                Dim locations As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    .Property1 = AllExisting.PreExisting_Locations.LibraryName,
                    .Property2 = AllExisting.PreExisting_Locations.ToString
                    }
                devDictionaryKeys.Add(locations)

            Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage

                Dim AllExisting = New DevDictionaryHelper.AllPreExistingItems()
                AllExisting.ConvertAllPreExistingIntoDevDictionary(myQuote)

                Dim AddInterests As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    .Property1 = AllExisting.PreExisting_AdditionalInterests.LibraryName,
                    .Property2 = AllExisting.PreExisting_AdditionalInterests.ToString
                    }
                devDictionaryKeys.Add(AddInterests)
                Dim AppliedAddInterests As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    .Property1 = AllExisting.PreExisting_AssignedAdditionalInterests.LibraryName,
                    .Property2 = AllExisting.PreExisting_AssignedAdditionalInterests.ToString
                    }
                devDictionaryKeys.Add(AppliedAddInterests)
                Dim locations As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    .Property1 = AllExisting.PreExisting_Locations.LibraryName,
                    .Property2 = AllExisting.PreExisting_Locations.ToString
                    }
                devDictionaryKeys.Add(locations)
                Dim CglClassCodes As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    .Property1 = AllExisting.PreExisting_CglClassCodes.LibraryName,
                    .Property2 = AllExisting.PreExisting_CglClassCodes.ToString
                    }
                devDictionaryKeys.Add(CglClassCodes)

                Dim IMContractorsEquip As New QuickQuoteGenericObjectWithTwoStringProperties With {
                    .Property1 = AllExisting.PreExisting_InlandMarineCoveragesWithPremium.LibraryName,
                    .Property2 = AllExisting.PreExisting_InlandMarineCoveragesWithPremium.ToString
                    }
                devDictionaryKeys.Add(IMContractorsEquip)
        End Select

        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal, QuickQuoteObject.QuickQuoteLobType.Farm 
                Dim billPayPlanIdsEFTMonthly As List(Of Integer) = QQHelper.BillingPayPlanIdsForPayPlanType(QuickQuoteHelperClass.PayPlanType.EftMonthly)
                If billPayPlanIdsEFTMonthly IsNot Nothing AndAlso billPayPlanIdsEFTMonthly.Count > 0 AndAlso qqHelper.IsPositiveIntegerString(quote.BillingPayPlanId) = True AndAlso billPayPlanIdsEFTMonthly.Contains(qqHelper.IntegerForString(quote.BillingPayPlanId)) = True Then
                    Dim isAgreeToEFTTermsChecked As String = "False"
                    If IsNullEmptyOrWhitespace(Quote.EFT_BankRoutingNumber) = False OrElse IsNullEmptyorWhitespace(Quote.EFT_BankAccountNumber) = False Then
                        isAgreeToEFTTermsChecked = "True"
                    End If
                    Dim ddItem As New QuickQuoteGenericObjectWithTwoStringProperties With {
                        .Property1 = "AgreeToEFTTerms",
                        .Property2 = isAgreeToEFTTermsChecked
                    }
                    If devDictionaryKeys Is Nothing Then
                        devDictionaryKeys = New List(Of QuickQuoteGenericObjectWithTwoStringProperties)
                    End If
                    devDictionaryKeys.Add(ddItem)
                End If
        End Select 

        For Each entry In devDictionaryKeys
            myQuote.SetDevDictionaryItem("global", entry.Property1, entry.Property2)
        Next

    End Sub
    'Added 01/21/2021 for CAP Endoresements Tasks 52969 and 52974 MLW
    Private Function evaluateVehicleGaragingAddress(endQQO As QuickQuoteObject) As Boolean
        Dim garagingAddressChanged As Boolean = False
        Dim hasGaragingAddress As Boolean = False
        For Each v In endQQO.Vehicles
            If v.GaragingAddress IsNot Nothing Then
                If v.GaragingAddress.Address IsNot Nothing Then
                    If String.IsNullOrWhiteSpace(v.GaragingAddress.Address.Zip) = False OrElse String.IsNullOrWhiteSpace(v.GaragingAddress.Address.City) = False OrElse String.IsNullOrWhiteSpace(v.GaragingAddress.Address.County) = False Then
                        hasGaragingAddress = True
                    End If
                Else
                    v.GaragingAddress.Address = New QuickQuoteAddress
                End If
            Else
                v.GaragingAddress = New QuickQuoteGaragingAddress
            End If
            If hasGaragingAddress = False Then
                populateGaragingAddress(endQQO, v.GaragingAddress.Address)
                garagingAddressChanged = True
            End If
        Next
        Return garagingAddressChanged
    End Function
    'Private Sub evaluateVehicleGaragingAddress(endQQO As QuickQuoteObject)
    '    Dim hasGaragingAddress As Boolean = False
    '    For Each v In endQQO.Vehicles
    '        If v.GaragingAddress IsNot Nothing Then
    '            If v.GaragingAddress.Address IsNot Nothing Then
    '                If String.IsNullOrWhiteSpace(v.GaragingAddress.Address.Zip) = False OrElse String.IsNullOrWhiteSpace(v.GaragingAddress.Address.City) = False OrElse String.IsNullOrWhiteSpace(v.GaragingAddress.Address.County) = False Then
    '                    hasGaragingAddress = True
    '                End If
    '            Else
    '                v.GaragingAddress.Address = New QuickQuoteAddress
    '            End If
    '        Else
    '            v.GaragingAddress = New QuickQuoteGaragingAddress
    '        End If
    '        If hasGaragingAddress = False Then
    '            populateGaragingAddress(endQQO, v.GaragingAddress.Address)
    '        End If
    '    Next
    'End Sub

    'Added 01/21/2021 for CAP Endoresements Tasks 52969 and 52974 MLW
    Private Sub populateGaragingAddress(endQQO As QuickQuoteObject, ByRef garagingAddress As QuickQuoteAddress)
        If endQQO IsNot Nothing AndAlso endQQO.Locations IsNot Nothing AndAlso endQQO.Locations.Count > 0 Then
            Dim loc As QuickQuote.CommonObjects.QuickQuoteLocation = endQQO.Locations(0)
            QQHelper.CopyQuickQuoteAddress(loc.Address, garagingAddress)
        Else
            If endQQO IsNot Nothing AndAlso endQQO.Policyholder IsNot Nothing AndAlso endQQO.Policyholder.Address IsNot Nothing Then
                Dim ph As QuickQuote.CommonObjects.QuickQuotePolicyholder = endQQO.Policyholder
                QQHelper.CopyQuickQuoteAddress(ph.Address, garagingAddress)
            End If
        End If
    End Sub

    'Added 12/30/2020 for CAP Endorsements Tasks 52969 and 52971 MLW
    Private Sub copyPolicyholderAddressToQuoteLocations(quote As QuickQuoteObject)
        If quote IsNot Nothing AndAlso quote.Policyholder IsNot Nothing AndAlso quote.Policyholder.Address IsNot Nothing Then
            If quote.Locations Is Nothing Then
                quote.Locations = New List(Of QuickQuoteLocation)
            End If
            Dim newLoc As New QuickQuote.CommonObjects.QuickQuoteLocation()
            newLoc.Address = New QuickQuote.CommonObjects.QuickQuoteAddress()
            QQHelper.CopyQuickQuoteAddress(quote.Policyholder.Address, newLoc.Address)
            quote.Locations.Add(newLoc)
        End If
    End Sub

    'Added 03/25/2021 for CAP Endorsements Task 52969 MLW
    Private Function PreventMultipleEndorsementsPerDay() As Boolean
        Dim maxNumber As Integer = 2
        Dim currCount As Integer = 0
        Dim polLookupInfo As New QuickQuotePolicyLookupInfo
        With polLookupInfo
            .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByImage
            .PolicyId = Me.Quote.PolicyId
            .TransTypeId = 3 'Endorsement
            .PolicyStatusCodeIds = New List(Of Integer)
            With .PolicyStatusCodeIds
                .Add(CInt(QuickQuotePolicyLookupInfo.DiamondPolicyStatusCode.InForce))
                .Add(CInt(QuickQuotePolicyLookupInfo.DiamondPolicyStatusCode.Future))
                .Add(CInt(QuickQuotePolicyLookupInfo.DiamondPolicyStatusCode.History))
            End With
        End With

        Dim lookupResults As List(Of QuickQuotePolicyLookupInfo) = QuickQuote.CommonMethods.QuickQuoteHelperClass.PolicyResultsForLookupInfo(polLookupInfo)
        If lookupResults IsNot Nothing AndAlso lookupResults.Count > 0 Then
            For Each r As QuickQuotePolicyLookupInfo In lookupResults
                If r IsNot Nothing Then
                    If QQHelper.DateForString(r.DateAdded).ToShortDateString = Date.Today.ToShortDateString Then
                        currCount += 1
                        If currCount >= maxNumber Then
                            Return True
                            Exit For
                        End If
                    End If
                End If
            Next
        End If
        Return False
    End Function



    Public Overrides Sub ClearControl()
        Me.txtTransEffectDate.Text = String.Empty
        Me.txtRemarks.Text = String.Empty
    End Sub

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage"))
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Save()
    End Sub

    Public Function isAlphaChar(val As Char) As Boolean
        Return Char.IsLetter(val)
    End Function

    Public Function isNumericChar(val As Char) As Boolean
        Return Char.IsNumber(val)
    End Function

    Public Function isAlphaNumericChar(val As Char) As Boolean
        Return (Char.IsNumber(val) OrElse Char.IsLetter(val))
    End Function

    Public Function isWhitespaceChar(val As Char) As Boolean
        Return Char.IsWhiteSpace(val)
    End Function

End Class