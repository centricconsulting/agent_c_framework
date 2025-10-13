Imports QuickQuote.CommonMethods
Imports IFM.VR.Common.QuoteSearch
Imports IFM.VR.Validation.ObjectValidation
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
Imports System.Configuration.ConfigurationManager
Imports IFM.VR.Common.Helpers
Imports IFM.PrimativeExtensions
Imports IFM.VR.Web.ctlPageStartupScript
Imports IFM.ControlFlags
Imports IFM.VR.Flags
Imports IFM.VR.Common.Helpers.DFR
Imports Newtonsoft.Json
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports IFM.VR.Common.Helpers.CAP

Public Class ctlShowACCORD
    Inherits VRControlBase

    Public DisableIRPMValue As Integer = 500
    Public Event RequestNavigationToIRPM()

    Dim reRateMessage As String = "Effective date change requires re-rate." 'added 2/19/2020
    Private _flags As IEnumerable(Of IFeatureFlag)

    Public Sub New()
        _flags = New List(Of IFeatureFlag) From {New LOB.PPA}
    End Sub

    Public ReadOnly Property AcordAppText As String
        Get
            If Me.Quote IsNot Nothing Then
                If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm Then
                    Return "Farm App"
                End If
            End If
            Return "ACORD App"
        End Get
    End Property

    Public Property MyLocationIndex As Int32
        Get
            If ViewState("vs_locationIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_locationIndex"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyFarmLocation As List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property LinkedApplications As List(Of QQSearchResult)
        Get
            If ViewState("vs_linkedApplicationlist") Is Nothing Then
                ViewState("vs_linkedApplicationlist") = GetLinkedApplications()
            End If
            Return DirectCast(ViewState("vs_linkedApplicationlist"), List(Of QQSearchResult))
        End Get
    End Property

    Public ReadOnly Property LinkedQuotes As List(Of QQSearchResult)
        Get
            If ViewState("vs_linkedQuotelist") Is Nothing Then
                Dim lq = GetLinkedQuotes()
                If lq IsNot Nothing AndAlso lq.Any() Then
                    Dim count As Int32 = 3
                    If lq.Count < 3 Then
                        count = lq.Count
                    End If
                    ViewState("vs_linkedQuotelist") = GetLinkedQuotes().GetRange(0, count)
                    Return DirectCast(ViewState("vs_linkedQuotelist"), List(Of QQSearchResult))
                Else
                    Return Nothing
                End If

            Else
                Return DirectCast(ViewState("vs_linkedQuotelist"), List(Of QQSearchResult))
            End If

        End Get
    End Property

    Public ReadOnly Property HasLinkedApplications As Boolean
        Get
            If LinkedApplications IsNot Nothing Then
                Return LinkedApplications.Any()
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property HasLinkedQuotes As Boolean
        Get
            If LinkedQuotes IsNot Nothing Then
                Return LinkedQuotes.Any()
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property HasLinkedQuotesOrLinkedApps As Boolean
        Get
            Return (HasLinkedApplications Or HasLinkedQuotes) And CBool(System.Configuration.ConfigurationManager.AppSettings("VR_Personal_AllowMultipleApplicationIssuance"))
        End Get
    End Property

    Private _EndorsementDictionaryName As String
    Public Property EndorsementDictionaryName() As String
        Get
            Return _EndorsementDictionaryName
        End Get
        Set(ByVal value As String)
            _EndorsementDictionaryName = value
            _devDictionaryHelper = Nothing
        End Set
    End Property

    Private Property _devDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
    Public ReadOnly Property ddh() As DevDictionaryHelper.DevDictionaryHelper
        Get
            If _devDictionaryHelper Is Nothing Then
                If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(EndorsementDictionaryName) = False Then
                    _devDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, EndorsementDictionaryName, Quote.LobType)
                End If
            End If
            Return _devDictionaryHelper
        End Get
    End Property

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.UseRatedQuoteImage = True
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MyBase.ValidateControl(New VRValidationArgs()) ' Matt A 10-14-14 ' Just attaches the validationhelper to the validation summary
        Me.ValidationHelper.GroupName = "Quote Summary"

        Me.VRScript.AddVariableLine("var allowAccordQuestion = true;") '7-22-14 Matt A
        If Not IsPostBack Then
            If Request.QueryString("printAccord") IsNot Nothing Then '7-22-14 Matt A
                PrintAcord()
                Me.VRScript.AddScriptLine("DisableFormOnSaveRemoves();")
            End If
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Me.VRScript.StopEventPropagation(Me.btnEmailForUWAssistance.ClientID)
        btnEmailForUWAssistance.Attributes.Add("onclick", "InitEmailToUW();")
        'Me.VRScript.CreateJSBinding(Me.btnCommStartNewQuote.ClientID, "click", "alert('Your quote was created successfully.  You will now be redirected to the LOB Selection page.');")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub


    Public Overrides Sub Populate()
        ' ***********************************************************************************************************************
        ' FYI: This is a place that you can do some post final rate checks to determine if you need to show route to UW is shown
        ' So find or create LOB case, Check to see if you are on APPSide, then set ValType to Issuance then do checks - Examples below
        ' ***********************************************************************************************************************
        '
        'Me.btnContinueToApp.Visible = True 'Matt A 12/30/14
        Dim ShowNewCoMessage As Boolean = False
        Me.NewCoMessage.Text = "Your quote is rated in Indiana Farmers Indemnity due to the effective date, rates and premiums have been adjusted. Please confirm coverage and limit selections before completing the application."
        Me.divNewCoMessage.Visible = False

        'Me.NewCoRedirectMessage.Text = "To ensure accurate coverage and premium charges, we need to re-rate your quote.  Please click [here] to begin the process."
        Me.divNewCoRedirectMessage.Visible = False


        Me.DivFarmMessage.Visible = False
        Me.btnDeleteEndorsement.Visible = False
        Me.divTopMessage.Visible = False
        If Me.Quote IsNot Nothing Then

            If IsNewCo() Then
                If Context.Session("ShowNewCoLockMessage") IsNot Nothing Then
                    Boolean.TryParse(Context.Session("ShowNewCoLockMessage"), ShowNewCoMessage)
                End If

                If Not IsOnAppPage AndAlso Not IsEndorsementRelated() _
                    AndAlso ShowNewCoMessage Then
                    Me.divNewCoMessage.Visible = True
                End If
            End If

            ' Show the correct button group (Personal or Commercial)
            Me.ctl_RouteToUw.Visible = False 'moved to include personal lines & farm
            If IsCommercialQuote() Then
                If IsOnAppPage Then
                    ' Use Personal buttons Lines
                    divCommButtons.Visible = False
                    divPersButtons.Visible = True
                    'Me.btnCreateHomeQuote.Visible = False
                    Me.btnPolicyholderStartNewQuote.Visible = False

                Else
                    ' Commercial Lines
                    divCommButtons.Visible = True
                    divPersButtons.Visible = False
                    Me.btnCommContinueToApplication.Visible = True
                End If
            Else
                ' Personal Lines & Farm
                divCommButtons.Visible = False
                divPersButtons.Visible = True
                Me.btnContinueToApp.Visible = True 'Matt A 12/30/14
            End If

            Me.btnCapVehicleListApp.Visible = False
            Me.btnCapVehicleList.Visible = False

            'Dim hasDiamondError As Boolean = (From v In Me.Quote.ValidationItems Where v.ValidationSeverityType = QuickQuote.CommonObjects.QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationError Select v).Any() ' added for bug 6493
            'updated 2/18/2019 for Endorsements/ReadOnly images; current newBus quoting code expects ValidationItems to always be something when RatedQuote is returned
            Dim hasDiamondError As Boolean = False ' added for bug 6493
            If Me.Quote.ValidationItems IsNot Nothing AndAlso Me.Quote.ValidationItems.Count > 0 Then
                hasDiamondError = (From v In Me.Quote.ValidationItems Where v.ValidationSeverityType = QuickQuote.CommonObjects.QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationError Select v).Any()
            End If

            Select Case Me.Quote.QuoteTransactionType'added CASE 5/21/2019; original logic in ELSE for New Business Quoting
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                    btnShowIRPM.Visible = False
                    'Me.btnCreateHomeQuote.Visible = False
                    Me.btnPolicyholderStartNewQuote.Visible = False

                    Me.btnViewAccord.Visible = False
                    Me.btnCommViewWorksheet.Visible = False

                    If Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                        'Updated 01/12/2021 for CAP Endorsements Task 52976 MLW
                        Select Case Quote.LobType
                            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                                divCommButtons.Visible = False
                                'Updated 04/28/2020 for Bug 46352 MLW
                                If QQHelper.BitToBoolean(QuickQuoteHelperClass.configAppSettingValueAsString("VR_Endorsements_AllowDFRSTP")) = True Then
                                    divPersButtons.Visible = True
                                    Me.btnContinueToApp.Visible = True
                                Else
                                    Me.ctl_RouteToUw.Visible = True
                                    Me.btnContinueToApp.Visible = False
                                    Me.divPersButtons.Visible = False
                                End If
                            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto _
                                , QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP _
                                , QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                                divPersButtons.Visible = False
                                divCommButtons.Visible = True
                                Me.btnCommIRPM.Visible = False
                                Me.btnShowIRPM.Visible = False
                                'Updated  09/20/2021 for BOP Endorsements Task 61506 MLW
                                If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                                    Me.btnCapVehicleList.Visible = True
                                    ToggleSubmitButtonEnabledStatusAndMessage()
                                Else
                                    Me.btnCapVehicleList.Visible = False
                                    Me.ctl_RouteToUw.Visible = True
                                End If
                                Me.btnEmailForUWAssistance.Visible = False
                                Me.btnCommStartNewQuote.Visible = False
                                Me.btnCommPrepareProposal.Visible = False
                                Me.btnCommDeleteEndorsement.Visible = True
                                Me.btnCommSubmitEndorsement.Visible = True
                                Me.btnCommContinueToApplication.Visible = False

                                'Added 04/09/2021 for CAP Endorsements Tasks 60344, 52969 MLW
                                Dim hasDiamondEndorsementMsg As Boolean = False
                                If Me.Quote.ValidationItems IsNot Nothing AndAlso Me.Quote.ValidationItems.Count > 0 Then
                                    Dim diaWarnings = (From v In Me.Quote.ValidationItems Where v.ValidationSeverityType = QuickQuote.CommonObjects.QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationWarning Select v)
                                    If diaWarnings IsNot Nothing AndAlso diaWarnings.Count > 0 Then
                                        For Each ve In diaWarnings
                                            'Updated 05/24/2021 for CAP Endorsements Task 52969 (see codev task 52653) MLW
                                            'If ve.Message.Contains("Out of sequence transactions need to be reviewed by underwriting") OrElse ve.Message.Contains("Change the policyholder state needs to be reviewed by underwriting") OrElse ve.Message.Contains("Amending mailing address state needs to be reviewed by underwriting") Then
                                            If String.IsNullOrWhiteSpace(ve.Message) = False AndAlso (ve.Message.ToUpper.Contains("Out of sequence transactions need to be reviewed by underwriting".ToUpper) OrElse ve.Message.ToUpper.Contains("Change the policyholder state needs to be reviewed by underwriting".ToUpper) OrElse ve.Message.ToUpper.Contains("Changing the policyholder state needs to be reviewed by underwriting".ToUpper)) Then
                                                hasDiamondEndorsementMsg = True
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If

                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Or hasDiamondEndorsementMsg Then
                                    HideAccordFinalizeButtons()
                                Else
                                    If Quote.LobType = QuickQuoteLobType.CommercialAuto AndAlso Quote.HasMultipleQuoteStates AndAlso UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                                        HideAccordFinalizeButtons()
                                    Else
                                        Me.btnCommViewWorksheet.Visible = True
                                        Me.btnCommSubmitEndorsement.OnClientClick = "DisableFormOnSaveRemoves();"
                                    End If
                                End If
                                'Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                                '    'Added 09/20/2021 for BOP Endorsements Task 61506 MLW
                                '    If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                '        HideAccordFinalizeButtons()
                                '    Else
                                '        Me.btnCommViewWorksheet.Visible = True
                                '        Me.btnCommSubmitEndorsement.OnClientClick = "DisableFormOnSaveRemoves();"
                                '    End If
                            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm
                                'Added 07/11/2022 for tasks 65975 MLW
                                Dim hasDiamondEndorsementMsg As Boolean = False
                                If Me.Quote.ValidationItems IsNot Nothing AndAlso Me.Quote.ValidationItems.Count > 0 Then
                                    Dim diaWarnings = (From v In Me.Quote.ValidationItems Where v.ValidationSeverityType = QuickQuote.CommonObjects.QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationWarning Select v)
                                    If diaWarnings IsNot Nothing AndAlso diaWarnings.Count > 0 Then
                                        For Each ve In diaWarnings
                                            If String.IsNullOrWhiteSpace(ve.Message) = False AndAlso ve.Message.ToUpper.Contains("Out of sequence transactions need to be reviewed by underwriting".ToUpper) Then
                                                hasDiamondEndorsementMsg = True
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If

                                If hasDiamondEndorsementMsg Then
                                    HideAccordFinalizeButtons()
                                Else
                                    divCommButtons.Visible = False
                                    divPersButtons.Visible = True
                                    Me.btnContinueToApp.Visible = True
                                End If
                            Case Else
                                divCommButtons.Visible = False
                                divPersButtons.Visible = True
                                Me.btnContinueToApp.Visible = True
                        End Select
                        ''Updated 09/18/2019 for DFR Endorsements tasks 40285 and 40274 MLW - was just else statement
                        'divCommButtons.Visible = False
                        'If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                        '    'Updated 04/28/2020 for Bug 46352 MLW
                        '    If QQHelper.BitToBoolean(QuickQuoteHelperClass.configAppSettingValueAsString("VR_Endorsements_AllowDFRSTP")) = True Then
                        '        divPersButtons.Visible = True
                        '        Me.btnContinueToApp.Visible = True
                        '    Else
                        '        Me.ctl_RouteToUw.Visible = True
                        '        Me.btnContinueToApp.Visible = False
                        '        Me.divPersButtons.Visible = False
                        '    End If
                        'Else

                        '    divPersButtons.Visible = True
                        '    Me.btnContinueToApp.Visible = True
                        'End If
                        ''divPersButtons.Visible = True
                        ''Me.btnContinueToApp.Visible = True

                        'Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary

                        'Added 7/16/2019 for Home Endorsements Project Task 38921 MLW
                        If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                            Dim valType = Validation.ObjectValidation.ValidationItem.ValidationType.issuance
                            Dim aiList_Vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.ValidateAdditionalInterestList(Me.Quote, valType)
                            For Each v In aiList_Vals
                                Select Case v.FieldId
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.HasThirdMortgagee
                                        Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.CompCollRequiredWithAi '4-13-15 Matt A - Bug 4717
                                        Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                End Select
                            Next

                            For Each v In IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(Me.Quote, valType)
                                ' just show all
                                Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                            Next

                            If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                HideAccordFinalizeButtons()
                            Else
                                Me.btnContinueToApp.Text = "Submit"
                                Me.btnContinueToApp.OnClientClick = "DisableFormOnSaveRemoves();"
                            End If
                        Else

                            If valSum.HasErrors OrElse hasDiamondError Then 'note: this matches PPA, but other LOBs may have additional things to check per validation
                                HideAccordFinalizeButtons()
                            Else
                                'Me.btnContinueToApp.Text = "Submit"
                                'Me.btnContinueToApp.OnClientClick = "DisableFormOnSaveRemoves();"
                                'updated 5/24/2019
                                Dim endorsementValidationErrorMsg As String = ""

                                If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
                                    Dim hasMvrError As Boolean = False
                                    Dim drvs As List(Of QuickQuote.CommonObjects.QuickQuoteDriver) = Nothing
                                    If Me.GoverningStateQuote IsNot Nothing Then
                                        drvs = Me.GoverningStateQuote.Drivers
                                    Else
                                        drvs = Me.Quote.Drivers
                                    End If
                                    If drvs IsNot Nothing AndAlso drvs.Count > 0 AndAlso QQHelper.IsPositiveIntegerString(Me.Quote.PolicyId) = True Then
                                        For Each d As QuickQuote.CommonObjects.QuickQuoteDriver In drvs
                                            'If d IsNot Nothing AndAlso d.HasValidDriverNum() = True AndAlso QQHelper.IsQuickQuoteDriverNewToImage(d, Me.Quote.TransactionEffectiveDate, Me.Quote.EffectiveDate, Me.Quote.PCAdded_Date) = True Then
                                            'updated 7/25/2019 to use new IsQuickQuoteDriverNewToImage method
                                            If d IsNot Nothing AndAlso d.HasValidDriverNum() = True AndAlso QQHelper.IsQuickQuoteDriverNewToImage(d, Me.Quote) = True Then
                                                Dim caughtDbErrorOnExistingMvrRecordCheck As Boolean = False
                                                Dim lastChoicePointTransmissionMvr As QuickQuote.CommonObjects.QuickQuoteChoicePointTransmission = Nothing
                                                If QuickQuote.CommonMethods.QuickQuoteHelperClass.HasMvrReport(QQHelper.IntegerForString(Me.Quote.PolicyId), unitNumber:=QQHelper.IntegerForString(d.DriverNum), caughtDatabaseError:=caughtDbErrorOnExistingMvrRecordCheck, returnRecords:=True, firstLastOrAllRecords:=QuickQuote.CommonMethods.QuickQuoteHelperClass.FirstLastOrAll.LastOnly, choicePointTransmission:=lastChoicePointTransmissionMvr) = True Then
                                                    If lastChoicePointTransmissionMvr IsNot Nothing AndAlso QQHelper.IsNoHitChoicePointTransmission(lastChoicePointTransmissionMvr) = True Then
                                                        hasMvrError = True
                                                    End If
                                                Else
                                                    If caughtDbErrorOnExistingMvrRecordCheck = False Then
                                                        hasMvrError = True
                                                    End If
                                                End If
                                                If hasMvrError = True Then
                                                    endorsementValidationErrorMsg = "MVR failed to pull on one or more drivers. Endorsement cannot be submitted without Underwriting review."
                                                    Exit For
                                                End If
                                            End If
                                        Next
                                    End If
                                End If

                                If String.IsNullOrWhiteSpace(endorsementValidationErrorMsg) = False Then
                                    HideAccordFinalizeButtons()
                                    Me.ValidationHelper.AddError(endorsementValidationErrorMsg)
                                Else
                                    Me.btnContinueToApp.Text = "Submit"
                                    Me.btnContinueToApp.OnClientClick = "DisableFormOnSaveRemoves();"
                                End If
                            End If
                        End If

                        'note: similar logic is also in place for New Business Quoting, but the Route button will be visible for Endorsements; may not need Date validation here or may need to update it
                        Dim valItems = PolicyLevelValidator.PolicyValidation(Me.Quote, Me.DefaultValidationType)
                        If valItems.ListHasValidationId(PolicyLevelValidator.EffectiveDate) Then
                            'effective date is not valid
                            Me.HideAccordFinalizeButtons()
                            'Me.ctl_RouteToUw.Visible = False
                            lblInvalidEffectiveDate.Visible = True
                            'added 8/1/2019 to change message for Endorsements
                            If String.IsNullOrWhiteSpace(lblInvalidEffectiveDate.Text) = False AndAlso lblInvalidEffectiveDate.Text.Contains(" Please update the effective date and rerate.") Then
                                lblInvalidEffectiveDate.Text = lblInvalidEffectiveDate.Text.Replace(" Please update the effective date and rerate.", " Please send to Underwriting for approval.")
                            End If
                        Else
                            lblInvalidEffectiveDate.Visible = False
                        End If

                        'B46262 CAH - Endorsement Delete Button Visibility
                        Select Case Quote.LobType
                            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                                If GoverningStateQuote.State.ToUpper().EqualsAny("IN", "IL") Then
                                    Me.btnDeleteEndorsement.Visible = True
                                End If
                            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal
                                If GoverningStateQuote.State.ToUpper().EqualsAny("IN") Then
                                    Me.btnDeleteEndorsement.Visible = True
                                End If
                            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                                If GoverningStateQuote.State.ToUpper().EqualsAny("IN") Then
                                    Me.btnDeleteEndorsement.Visible = True
                                End If
                            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm
                                If GoverningStateQuote.State.ToUpper().EqualsAny("IN", "IL", "OH") Then
                                    Me.btnDeleteEndorsement.Visible = True
                                End If
                        End Select

                    End If
                Case Else
                    Me.btnCommDeleteEndorsement.Visible = False 'Added 01/12/2021 for CAP Endorsements Task 52976 MLW
                    Me.btnCommSubmitEndorsement.Visible = False 'Added 01/12/2021 for CAP Endorsements Task 52976 MLW
                    Select Case Me.Quote.LobId
                        Case "1" 'PPA
                            btnShowIRPM.Visible = False
                            'Matt A 9-11-2014
                            If IsOnAppPage Then
                                'Me.btnCreateHomeQuote.Visible = False '10-6-14 Matt A
                                Me.btnPolicyholderStartNewQuote.Visible = False

                                If Session(Me.QuoteId + "_HasNoHitMVR") IsNot Nothing AndAlso CBool(Session(Me.QuoteId + "_HasNoHitMVR")) = True Then
                                    If Me.Visible Then
                                        Me.ValidationHelper.AddError("MVR failed to pull on one or more drivers. Application can not be finalized without Underwriting review.")
                                    End If
                                    HideAccordFinalizeButtons()
                                Else
                                    Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                    If valSum.HasErrors Or hasDiamondError Then ' added hasDiamonderror for bug 6493
                                        HideAccordFinalizeButtons()
                                    Else
                                        'Updated 10/5/18 for multi state MLW
                                        Select Case (Quote.QuickQuoteState)
                                            'KLJ 7/29/22
                                            'commented out as introductory effective date has past and
                                            'if OH PPA is disabled in the future, a quote should not reach this point
                                            'Case QuickQuoteHelperClass.QuickQuoteState.Ohio
                                            '    If _flags.WithFlags(Of LOB.PPA).OhioEnabled Then
                                            '        Me.btnViewAccord.Visible = False

                                            '        LoadLinkedQuotes()
                                            '        LoadLinkedApplications()
                                            '        Me.btnContinueToApp.Text = "Finalize Application"
                                            '    End If
                                            Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio
                                                'Check to see if validation message has already been added - do not want duplicates
                                                Dim hasCorrespondingFarmMsg As Boolean = False
                                                Dim vhErrors As List(Of WebValidationItem) = Me.ValidationHelper.GetErrors()
                                                If vhErrors IsNot Nothing AndAlso vhErrors.Count > 0 Then
                                                    For Each ve In vhErrors
                                                        '''TODO: these messages should be immutable constants or loaded from an external source
                                                        If ve.Message.Contains($"This PPA quote is an {Quote.State} quote, UW must verify the corresponding Farm policy. Please contact your UW for additional information.") Then
                                                            hasCorrespondingFarmMsg = True
                                                            Exit For
                                                        End If
                                                    Next
                                                End If
                                                If hasCorrespondingFarmMsg = False Then
                                                    Me.ValidationHelper.AddError($"This PPA quote is an {Quote.State} quote, UW must verify the corresponding Farm policy. Please contact your UW for additional information.")
                                                End If
                                                If Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois OrElse Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                                                    Me.btnViewAccord.Visible = True
                                                    Me.ctl_RouteToUw.Visible = True
                                                    divPersButtons.Attributes.Add("style", "display:inline;")
                                                    ctl_RouteToUw.Attributes.Add("style", "display:inline;")
                                                    Me.linkPrintAccord.NavigateUrl = Request.RawUrl + "&printAccord=1"
                                                    Me.btnContinueToApp.Visible = False
                                                Else
                                                    HideAccordFinalizeButtons()
                                                End If
                                            Case Else
                                                LoadLinkedQuotes()
                                                LoadLinkedApplications()
                                                Me.btnContinueToApp.Text = "Finalize Application"
                                                Me.btnViewAccord.Visible = True
                                                Me.btnContinueToApp.OnClientClick = "if (allowAccordQuestion){OpenACORDPopup();} DisableFormOnSaveRemoves();return false;" '7-22-14 Matt A
                                                Me.linkPrintAccord.NavigateUrl = Request.RawUrl + "&printAccord=1"
                                        End Select

                                    End If

                                End If

                            Else
                                Me.btnContinueToApp.Text = "Continue to Application"
                                Me.btnContinueToApp.OnClientClick = "DisableFormOnSaveRemoves();" '7-22-14 Matt A

                                If Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(2) Then
                                    'Me.btnCreateHomeQuote.Visible = True '10-6-14 Matt A
                                    Me.btnPolicyholderStartNewQuote.Visible = True '10-6-14 Matt A
                                    'Updated 10/9/18 for multi state MLW
                                    Select Case (Quote.QuickQuoteState)
                                        Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                                            'Me.btnCreateHomeQuote.Visible = False
                                            Me.btnPolicyholderStartNewQuote.Visible = False
                                        Case Else
                                            'Me.btnCreateHomeQuote.Visible = False
                                            Me.btnPolicyholderStartNewQuote.Visible = True
                                            'Me.btnCreateHomeQuote.Text = "Begin Home Quote with Same Policyholder"
                                    End Select
                                    'Me.btnCreateHomeQuote.OnClientClick = "DisableFormOnSaveRemoves();" '10-6-14 Matt A
                                Else
                                    'Me.btnCreateHomeQuote.Visible = False '10-6-14 Matt A
                                    Me.btnPolicyholderStartNewQuote.Visible = False
                                End If

                                Me.btnViewAccord.Visible = False
                            End If
                ' End Matt A Change 9-11-2014
                        Case "2" ' HOM
                            btnShowIRPM.Visible = False
                            'Matt A 9-11-2014
                            If IsOnAppPage Then
                                'Me.btnCreateHomeQuote.Visible = False '10-6-14 Matt A
                                Me.btnPolicyholderStartNewQuote.Visible = False

                                Dim hasInlandErrrosThatAreNotShown As Boolean = False
                                ' MATT -  Do a series of pre-issuance checks for HOM
                                Dim valType = Validation.ObjectValidation.ValidationItem.ValidationType.issuance
                                Dim aiList_Vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.ValidateAdditionalInterestList(Me.Quote, valType)
                                For Each v In aiList_Vals
                                    Select Case v.FieldId
                                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.HasThirdMortgagee
                                            Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.CompCollRequiredWithAi '4-13-15 Matt A - Bug 4717
                                            Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                    End Select
                                Next

                                For Each v In IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(Me.Quote, valType)
                                    ' just show all
                                    Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                Next

                                For Each v In IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(Me.Quote, valType)
                                    ' just show all
                                    Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                    hasInlandErrrosThatAreNotShown = True
                                Next

                                'Added 11/25/2019 for bug 27286 MLW
                                If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count > 0 Then 'added IF 3/11/2021
                                    For Each location In Quote.Locations
                                        If location IsNot Nothing AndAlso location.InlandMarines IsNot Nothing AndAlso location.InlandMarines.Count > 0 Then 'added IF 3/11/2021
                                            For Each im In location.InlandMarines
                                                'For Each v In IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineValidator.ValidateHOMInlandMarine(Me.Quote, valType, im.InlandMarineType)
                                                'updated 3/11/2021
                                                For Each v In IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineValidator.ValidateHOMInlandMarine(Me.Quote, im, valType)
                                                    ' just show all
                                                    Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                                    hasInlandErrrosThatAreNotShown = True
                                                Next
                                            Next
                                        End If
                                    Next
                                End If

                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasInlandErrrosThatAreNotShown Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    LoadLinkedQuotes()
                                    LoadLinkedApplications()
                                    Me.btnContinueToApp.Text = "Finalize Application"
                                    Me.btnViewAccord.Visible = True
                                    Me.btnContinueToApp.OnClientClick = "if (allowAccordQuestion){OpenACORDPopup();} DisableFormOnSaveRemoves();return false;" '7-22-14 Matt A
                                    Me.linkPrintAccord.NavigateUrl = Request.RawUrl + "&printAccord=1"
                                End If

                            Else
                                Me.btnContinueToApp.Text = "Continue to Application"
                                Me.btnContinueToApp.OnClientClick = "DisableFormOnSaveRemoves();" '7-22-14 Matt A

                                If Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(1) Then
                                    Me.btnPolicyholderStartNewQuote.Visible = True

                                    'Me.btnCreateHomeQuote.Visible = True '10-6-14 Matt A
                                    'Me.btnCreateHomeQuote.Text = "Begin Auto Quote with Same Policyholder"
                                    'Me.btnCreateHomeQuote.OnClientClick = "DisableFormOnSaveRemoves();" '10-6-14 Matt A
                                Else
                                    'Me.btnCreateHomeQuote.Visible = False '10-6-14 Matt A
                                    Me.btnPolicyholderStartNewQuote.Visible = False

                                End If

                                Me.btnViewAccord.Visible = False
                            End If
                ' End Matt A Change 9-11-2014
                        Case "17" 'Farm
                            'Me.btnCreateHomeQuote.Visible = False 'Matt A 6-16-15
                            Me.btnPolicyholderStartNewQuote.Visible = False

                            Me.DivFarmMessage.Visible = True
                            If IsOnAppPage Then
                                Me.btnViewAccord.Text = "Print Farm App" ' Matt A Bug#5143
                                linkPrintAccord.Text = "Print Farm App" ' Matt A Bug#5143
                                Dim hasInlandErrrosThatAreNotShown As Boolean = False
                                ' MATT -  Do a series of pre-issuance checks for HOM
                                Dim valType = Validation.ObjectValidation.ValidationItem.ValidationType.issuance

                                Dim aiList_Vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.ValidateAdditionalInterestList(Me.Quote, valType)
                                For Each v In aiList_Vals
                                    Select Case v.FieldId
                                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.HasThirdMortgagee
                                            Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.CompCollRequiredWithAi '4-13-15 Matt A - Bug 4717
                                            Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                    End Select
                                Next

                                For Each v In IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(Me.Quote, valType)
                                    ' just show all
                                    Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                    hasInlandErrrosThatAreNotShown = True
                                Next

                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasInlandErrrosThatAreNotShown Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    LoadLinkedQuotes()
                                    LoadLinkedApplications()
                                    Me.btnContinueToApp.Text = "Finalize Application"
                                    Me.btnViewAccord.Visible = True
                                    Me.btnContinueToApp.OnClientClick = "if (allowAccordQuestion){OpenACORDPopup();} DisableFormOnSaveRemoves();return false;" '7-22-14 Matt A
                                    Me.linkPrintAccord.NavigateUrl = Request.RawUrl + "&printAccord=1"
                                End If
                            Else
                                Me.btnStartNewQuote.Visible = True
                                Me.btnViewAccord.Visible = False
                                Me.btnContinueToApp.Text = "Continue to Application"
                                Me.btnContinueToApp.OnClientClick = "DisableFormOnSaveRemoves();" '7-22-14 Matt A
                            End If

                            '**********************
                            'IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Quote.TotalQuotedPremium) > 500 ' highly recommend using this helper function instead
                            ' i would also check for null on MyFarmLocation(MyLocationIndex) before you dot into it to see if it is hobby farm
                            'like so....
                            '      If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Quote.TotalQuotedPremium) > 500 And Quote.ProgramTypeId <> "8" AndAlso MyFarmLocation(MyLocationIndex) IsNot Nothing And Not MyFarmLocation(MyLocationIndex).FarmTypeHobby Then
                            '************************
                            'Updated 9/11/18 for multi state MLW - Quote to SubQuoteFirst
                            If SubQuoteFirst IsNot Nothing Then
                                If Double.Parse((Quote.TotalQuotedPremium.Replace("$", "")).Replace(",", "")) > 500 And SubQuoteFirst.ProgramTypeId <> "8" And Not MyFarmLocation(MyLocationIndex).FarmTypeHobby Then
                                    btnShowIRPM.Visible = True
                                Else
                                    btnShowIRPM.Visible = False
                                End If
                            Else
                                btnShowIRPM.Visible = False
                            End If

                        Case "3" 'dfr
                            btnShowIRPM.Visible = False
                            'Matt A 9-11-2014
                            If IsOnAppPage Then
                                'Me.btnCreateHomeQuote.Visible = False '10-6-14 Matt A
                                Me.btnPolicyholderStartNewQuote.Visible = False


                                ' MATT -  Do a series of pre-issuance checks for DFR
                                Dim valType = Validation.ObjectValidation.ValidationItem.ValidationType.issuance
                                Dim aiList_Vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.ValidateAdditionalInterestList(Me.Quote, valType)
                                For Each v In aiList_Vals
                                    Select Case v.FieldId
                                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.HasThirdMortgagee
                                            'Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "") 'This was giving me a javascript error. I think it is because the control does not exist on the app summary page.
                                            Me.ValidationHelper.AddError(v.Message)
                                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.CompCollRequiredWithAi '4-13-15 Matt A - Bug 4717
                                            'Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                            Me.ValidationHelper.AddError(v.Message)
                                    End Select
                                Next

                                Dim qqXml As New QuickQuoteXML()
                                Dim Clue_ThirdPartyData As Diamond.Common.Objects.ThirdParty.ThirdPartyData = Nothing
                                qqXml.LoadExistingCluePropertyReportForQuote(Me.Quote, Clue_ThirdPartyData)

                                For Each v In LossListValidator.ValidateLossList(0, Clue_ThirdPartyData, Me.Quote, valType)
                                    Select Case v.FieldId
                                        Case LossListValidator.LossDateDuplicatedInList
                                            Me.ValidationHelper.AddError(v.Message)
                                        Case LossListValidator.MultipleLossesIn5Years
                                            Me.ValidationHelper.AddError(v.Message)
                                            'Case LossListValidator.ChoicePointProcessingStatusCodeIs2 'BU Decided they did not want this validation - Daniel Gugenheim - 11/20/2015
                                            'Me.ValidationHelper.AddError(v.Message)
                                    End Select
                                Next

                                'Matt A 6-20-2017 - Need to show route to UW if prior carrier is IFM
                                Dim valItems_Prior = PriorCarrierValidator.ValidatePriorCarrier(Me.Quote, valType)
                                If valItems_Prior.Any() Then
                                    For Each v In valItems_Prior
                                        Select Case v.FieldId
                                            Case PriorCarrierValidator.PriorcarrierPreviousInsurer
                                                Me.ValidationHelper.AddError(v.Message)
                                        End Select
                                    Next
                                End If

                                'added by 9/1/2022 KLJ for 76771,76772,76777
                                If DFRStandaloneHelper.isDFRStandaloneAvailable(Me.Quote) Then
                                    Dim valList = IFM.VR.Validation.ObjectValidation.PersLines.LOB.DFR.UnderwritingQuestionValidator.ValidateRouteToUnderwriting(Me.Quote, valType)

                                    If valList.Any() Then
                                        For Each v In valList
                                            Me.ValidationHelper.AddError(v.Message)
                                        Next
                                    End If
                                End If

                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    LoadLinkedQuotes()
                                    LoadLinkedApplications()
                                    Me.btnContinueToApp.Text = "Finalize Application"
                                    Me.btnViewAccord.Visible = True
                                    Me.btnContinueToApp.OnClientClick = "if (allowAccordQuestion){OpenACORDPopup();} DisableFormOnSaveRemoves();return false;" '7-22-14 Matt A
                                    Me.linkPrintAccord.NavigateUrl = Request.RawUrl + "&printAccord=1"
                                End If

                            Else
                                Me.btnContinueToApp.Text = "Continue to Application"
                                Me.btnContinueToApp.OnClientClick = "DisableFormOnSaveRemoves();" '7-22-14 Matt A

                                If Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(1) Then
                                    Me.btnPolicyholderStartNewQuote.Visible = True

                                    'Me.btnCreateHomeQuote.Visible = True '10-6-14 Matt A
                                    'Me.btnCreateHomeQuote.Text = "Begin Auto Quote with Same Policyholder"
                                    'Me.btnCreateHomeQuote.OnClientClick = "DisableFormOnSaveRemoves();" '10-6-14 Matt A
                                Else
                                    'Me.btnCreateHomeQuote.Visible = False '10-6-14 Matt A
                                    Me.btnPolicyholderStartNewQuote.Visible = False

                                End If

                                Me.btnViewAccord.Visible = False
                            End If

                        Case "9" ' CGL
                            Me.topMessage.Text = "Credits require underwriting approval for Hotel/Motel classifications.  Please contact your underwriter."
                            Me.btnCommIRPM.Text = "Credits/Debits"
                            Me.btnShowIRPM.Text = "Credits/Debits"
                            'Me.btnCreateHomeQuote.Visible = False 'Matt A 6-16-15
                            'Me.DivFarmMessage.Visible = False
                            If IsOnAppPage Then
                                Me.btnViewAccord.Text = "Print Documents"
                                linkPrintAccord.Text = "Print App"

                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    LoadLinkedQuotes()
                                    LoadLinkedApplications()
                                    Me.btnContinueToApp.Text = "Submit"
                                    ' Me.btnCommViewWorksheet.Visible = True
                                    Me.btnCommViewWorksheet.Visible = False
                                    Me.linkPrintAccord.NavigateUrl = Request.RawUrl + "&printAccord=1"
                                End If
                            Else
                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    Me.btnCommViewWorksheet.Visible = True
                                    Me.btnCommContinueToApplication.OnClientClick = "DisableFormOnSaveRemoves();"

                                End If
                                ' View Worksheet should be visible on Quote Summary and not Application Summary
                                ' Me.btnCommViewWorksheet.Visible = False
                                ' Me.btnCommViewWorksheet.Visible = True
                                ' Me.btnCommContinueToApplication.OnClientClick = "DisableFormOnSaveRemoves();"
                            End If
                            'Hide IRPM if premium is less than $500 (set above) and show IRPM if there is a current IRPM adjustment.
                            Dim amount As Integer
                            If Integer.TryParse(Me.Quote.TotalQuotedPremium, Globalization.NumberStyles.Currency, Globalization.CultureInfo.CurrentCulture, amount) Then
                                DisableIRPMValue = 500
                                If amount >= DisableIRPMValue Or hasIRPMAdjustment() Then
                                    Me.btnCommIRPM.Enabled = True
                                    Me.btnShowIRPM.Enabled = True
                                Else
                                    Me.btnCommIRPM.Enabled = False
                                    Me.btnShowIRPM.Enabled = False
                                End If
                            End If
                        Case "25" ' BOP
                            Me.topMessage.Text = "IRPM requires underwriting approval for Motel classifications.  Please contact your underwriter."
                            If IsOnAppPage Then
                                Me.btnViewAccord.Text = "Print Documents"
                                Me.btnShowIRPM.Text = "IRPM"
                                linkPrintAccord.Text = "Print App"
                                'Dim hasInlandErrrosThatAreNotShown As Boolean = False
                                ' MATT -  Do a series of pre-issuance checks for HOM
                                'Dim valType = Validation.ObjectValidation.ValidationItem.ValidationType.issuance
                                'Dim aiList_Vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.ValidateAdditionalInterestList(Me.Quote, valType)
                                'For Each v In aiList_Vals
                                '    Select Case v.FieldId
                                '        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.HasThirdMortgagee
                                '            Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                '        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.CompCollRequiredWithAi '4-13-15 Matt A - Bug 4717
                                '            Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                '    End Select
                                'Next

                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    LoadLinkedQuotes()
                                    LoadLinkedApplications()
                                    Me.btnContinueToApp.Text = "Submit"
                                    ' Me.btnCommViewWorksheet.Visible = True
                                    Me.btnCommViewWorksheet.Visible = False
                                    'Me.btnCommContinueToApplication.OnClientClick = "<%= lnkFinalize_Click('', '') %>"
                                    Me.linkPrintAccord.NavigateUrl = Request.RawUrl + "&printAccord=1"
                                End If
                            Else
                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    Me.btnCommViewWorksheet.Visible = True
                                    Me.btnCommContinueToApplication.OnClientClick = "DisableFormOnSaveRemoves();"

                                End If
                                ' View Worksheet should be visible on Quote Summary and not Application Summary
                                ' Me.btnCommViewWorksheet.Visible = False
                                ' Me.btnCommViewWorksheet.Visible = True
                                ' Me.btnCommContinueToApplication.OnClientClick = "DisableFormOnSaveRemoves();"
                            End If
                            'Hide IRPM if premium is less than $500 (set above) and show IRPM if there is a current IRPM adjustment.
                            Dim amount As Integer
                            If Integer.TryParse(Me.Quote.TotalQuotedPremium, Globalization.NumberStyles.Currency, Globalization.CultureInfo.CurrentCulture, amount) Then
                                DisableIRPMValue = 500
                                If amount >= DisableIRPMValue Or hasIRPMAdjustment() Then
                                    Me.btnCommIRPM.Enabled = True
                                    Me.btnShowIRPM.Enabled = True
                                Else
                                    Me.btnCommIRPM.Enabled = False
                                    Me.btnShowIRPM.Enabled = False
                                End If
                            End If
                        Case "20" ' CAP
                            Me.btnCommIRPM.Text = "Credits/Debits"
                            Me.btnShowIRPM.Text = "Credits/Debits"
                            If IsOnAppPage Then
                                Me.btnCapVehicleListApp.Visible = True
                                Me.btnViewAccord.Text = "Print Documents"
                                linkPrintAccord.Text = "Print App"

                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    If IsOnAppPage AndAlso Quote.HasMultipleQuoteStates AndAlso UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                                        HideAccordFinalizeButtons()
                                    Else
                                        LoadLinkedQuotes()
                                        LoadLinkedApplications()
                                        Me.btnContinueToApp.Text = "Submit"
                                        ' Me.btnCommViewWorksheet.Visible = True
                                        Me.btnCommViewWorksheet.Visible = False
                                        Me.linkPrintAccord.NavigateUrl = Request.RawUrl + "&printAccord=1"
                                    End If
                                End If
                            Else
                                Me.btnCapVehicleList.Visible = True
                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    Me.btnCommViewWorksheet.Visible = True
                                    Me.btnCommContinueToApplication.OnClientClick = "DisableFormOnSaveRemoves();"

                                End If
                                ' View Worksheet should be visible on Quote Summary and not Application Summary
                                ' Me.btnCommViewWorksheet.Visible = False
                                ' Me.btnCommViewWorksheet.Visible = True
                                ' Me.btnCommContinueToApplication.OnClientClick = "DisableFormOnSaveRemoves();"
                            End If
                            'Hide IRPM if < 2 motorized vehicles and show IRPM if there is a current IRPM adjustment.
                            If (Quote.Vehicles IsNot Nothing And QuickQuoteObjectHelper.GetMotorizedVehicleCount(Quote.Vehicles) > 1) Or hasIRPMAdjustment() Then
                                Me.btnCommIRPM.Enabled = True
                                Me.btnShowIRPM.Enabled = True
                                Me.btnCommIRPM.ToolTip = ""
                                Me.btnShowIRPM.ToolTip = ""
                            Else
                                Me.btnCommIRPM.Enabled = False
                                Me.btnShowIRPM.Enabled = False
                                Me.btnCommIRPM.ToolTip = "You cannot edit CAP Credits/Debits factors unless there are at least 2 vehicles."
                                Me.btnShowIRPM.ToolTip = "You cannot edit CAP Credits/Debits factors unless there are at least 2 vehicles."
                            End If
                        Case "21" ' WCP
                            Me.btnCommIRPM.Text = "Credits/Debits"
                            Me.btnShowIRPM.Text = "Credits/Debits"
                            If IsOnAppPage Then
                                Me.btnViewAccord.Text = "Print Documents"
                                linkPrintAccord.Text = "Print App"

                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    LoadLinkedQuotes()
                                    LoadLinkedApplications()
                                    Me.btnContinueToApp.Text = "Submit"
                                    ' Me.btnCommViewWorksheet.Visible = True
                                    Me.btnCommViewWorksheet.Visible = False
                                    Me.linkPrintAccord.NavigateUrl = Request.RawUrl + "&printAccord=1"
                                End If
                            Else
                                If GoverningStateQuote.State.ToUpper() = "KY" Then
                                    btnCommStartNewQuote.Visible = False
                                End If
                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    Me.btnCommViewWorksheet.Visible = True
                                    Me.btnCommContinueToApplication.OnClientClick = "DisableFormOnSaveRemoves();"

                                End If
                                ' View Worksheet should be visible on Quote Summary and not Application Summary
                                ' Me.btnCommViewWorksheet.Visible = False
                                ' Me.btnCommViewWorksheet.Visible = True
                                ' Me.btnCommContinueToApplication.OnClientClick = "DisableFormOnSaveRemoves();"
                            End If
                            'Hide IRPM if premium is less than $2500 (set above) and show IRPM if there is a current IRPM adjustment.
                            Dim amount As Integer
                            DisableIRPMValue = 2500
                            If Integer.TryParse(Me.Quote.TotalQuotedPremium, Globalization.NumberStyles.Currency, Globalization.CultureInfo.CurrentCulture, amount) Then
                                If amount >= DisableIRPMValue Or hasIRPMAdjustment() Then
                                    Me.btnCommIRPM.Enabled = True
                                    Me.btnShowIRPM.Enabled = True
                                Else
                                    Me.btnCommIRPM.Enabled = False
                                    Me.btnShowIRPM.Enabled = False
                                End If
                            End If
                        Case "28" ' CPR
                            Me.topMessage.Text = "IRPM requires underwriting approval for Hotel/Motel classifications.  Please contact your underwriter."
                            If IsOnAppPage Then
                                Me.btnViewAccord.Text = "Print Documents"
                                Me.btnShowIRPM.Text = "IRPM"
                                linkPrintAccord.Text = "Print App"

                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    LoadLinkedQuotes()
                                    LoadLinkedApplications()
                                    Me.btnContinueToApp.Text = "Submit"
                                    Me.btnCommViewWorksheet.Visible = False
                                    Me.linkPrintAccord.NavigateUrl = Request.RawUrl + "&printAccord=1"
                                End If
                            Else
                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    Me.btnCommViewWorksheet.Visible = True
                                    Me.btnCommContinueToApplication.OnClientClick = "DisableFormOnSaveRemoves();"

                                End If
                            End If

                            Dim amount As Integer
                            If Integer.TryParse(Me.Quote.TotalQuotedPremium, Globalization.NumberStyles.Currency, Globalization.CultureInfo.CurrentCulture, amount) Then
                                DisableIRPMValue = 500
                                If amount >= DisableIRPMValue Or hasIRPMAdjustment() Then
                                    Me.btnCommIRPM.Enabled = True
                                    Me.btnShowIRPM.Enabled = True
                                Else
                                    Me.btnCommIRPM.Enabled = False
                                    Me.btnShowIRPM.Enabled = False
                                End If
                            End If

                        Case "23" ' CPP
                            Me.topMessage.Text = "Credits require underwriting approval for Hotel/Motel classifications.  Please contact your underwriter."
                            Me.btnCommIRPM.Text = "Credits/Debits"
                            Me.btnShowIRPM.Text = "Credits/Debits"
                            If IsOnAppPage Then
                                Me.btnViewAccord.Text = "Print Documents"
                                linkPrintAccord.Text = "Print App"

                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    LoadLinkedQuotes()
                                    LoadLinkedApplications()
                                    Me.btnContinueToApp.Text = "Submit"
                                    Me.btnCommViewWorksheet.Visible = False
                                    Me.linkPrintAccord.NavigateUrl = Request.RawUrl + "&printAccord=1"
                                End If
                            Else
                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    Me.btnCommViewWorksheet.Visible = True
                                    Me.btnCommContinueToApplication.OnClientClick = "DisableFormOnSaveRemoves();"

                                End If
                            End If

                            'Hide IRPM if premium is less than $500 (set above) and show IRPM if there is a current IRPM adjustment.
                            ' NOTE PER BUG 63093:  For CPP the premium amount is calculated from ONLY the CGL and CPR parts, NOT the total quote premium.
                            Dim amount As Integer
                            Dim CGLAmount As Integer = 0
                            Dim CPRAmount As Integer = 0
                            If Integer.TryParse(Me.SubQuoteFirst.CPP_GL_PackagePart_QuotedPremium, Globalization.NumberStyles.Currency, Globalization.CultureInfo.CurrentCulture, CGLAmount) _
                                AndAlso Integer.TryParse(Me.SubQuoteFirst.CPP_CPR_PackagePart_QuotedPremium, Globalization.NumberStyles.Currency, Globalization.CultureInfo.CurrentCulture, CPRAmount) Then
                                amount = CPRAmount + CGLAmount
                            Else
                                amount = 0
                            End If

                            DisableIRPMValue = 500
                            If amount >= DisableIRPMValue Or hasIRPMAdjustment() Then
                                Me.btnCommIRPM.Enabled = True
                                Me.btnShowIRPM.Enabled = True
                            Else
                                Me.btnCommIRPM.Enabled = False
                                Me.btnShowIRPM.Enabled = False
                            End If

                ' Old code
                            'If Integer.TryParse(Me.Quote.TotalQuotedPremium, Globalization.NumberStyles.Currency, Globalization.CultureInfo.CurrentCulture, amount) Then
                            '    DisableIRPMValue = 500
                            '    If amount >= DisableIRPMValue Or hasIRPMAdjustment() Then
                            '        Me.btnCommIRPM.Enabled = True
                            '        Me.btnShowIRPM.Enabled = True
                            '    Else
                            '        Me.btnCommIRPM.Enabled = False
                            '        Me.btnShowIRPM.Enabled = False
                            '    End If
                            'End If
                        Case "14" ' PUP/FUP
                            btnShowIRPM.Visible = False
                            'Matt A 9-11-2014
                            If IsOnAppPage Then
                                'Me.btnCreateHomeQuote.Visible = False '10-6-14 Matt A
                                Me.btnPolicyholderStartNewQuote.Visible = False

                                ' MATT -  Do a series of pre-issuance checks for HOM
                                'Dim valType = Validation.ObjectValidation.ValidationItem.ValidationType.issuance
                                'Dim aiList_Vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.ValidateAdditionalInterestList(Me.Quote, valType)
                                'For Each v In aiList_Vals
                                '    Select Case v.FieldId
                                '        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.HasThirdMortgagee
                                '            Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                '        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestListValidator.CompCollRequiredWithAi '4-13-15 Matt A - Bug 4717
                                '            Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                '    End Select
                                'Next

                                'For Each v In IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(Me.Quote, valType)
                                '    ' just show all
                                '    Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "")
                                'Next


                                Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
                                If Me.ValidationHelper.HasErrros Or valSum.HasErrors Or hasDiamondError Then
                                    HideAccordFinalizeButtons()
                                Else
                                    'LoadLinkedQuotes()
                                    'LoadLinkedApplications()
                                    Me.btnContinueToApp.Text = "Finalize Application"
                                    Me.btnViewAccord.Visible = True
                                    Me.btnContinueToApp.OnClientClick = "if (allowAccordQuestion){OpenACORDPopup();} DisableFormOnSaveRemoves();return false;" '7-22-14 Matt A
                                    Me.linkPrintAccord.NavigateUrl = Request.RawUrl + "&printAccord=1"
                                End If

                            Else
                                Me.btnContinueToApp.Text = "Continue to Application"
                                Me.btnContinueToApp.OnClientClick = "DisableFormOnSaveRemoves();" '7-22-14 Matt A
                                'Me.btnCreateHomeQuote.Visible = False '10-6-14 Matt A
                                Me.btnPolicyholderStartNewQuote.Visible = False

                                Me.btnViewAccord.Visible = False
                            End If

                    End Select

                    If IrpmVisibilityByClasscodeHelper.IsIrpmVisibilityAvailable(Quote) Then
                        If AllLines.IRPM_ClasscodeCheck.IsUnwantedClassCodePresent(Quote) _
                            AndAlso Me.btnCommIRPM.Enabled _
                            AndAlso Me.btnShowIRPM.Enabled Then
                            Me.btnCommIRPM.Enabled = False
                            Me.btnShowIRPM.Enabled = False
                            Me.divTopMessage.Visible = True
                        End If
                    End If

                    '  Matt A - 12-31-14 Hides finalize buttons when the effective date is not valid. This can happen when opening an old application that had been rated.
                    Dim valItems = PolicyLevelValidator.PolicyValidation(Me.Quote, Me.DefaultValidationType)
                    If valItems.ListHasValidationId(PolicyLevelValidator.EffectiveDate) Then
                        'effective date is not valid
                        Me.HideAccordFinalizeButtons()
                        Me.ctl_RouteToUw.Visible = False
                        lblInvalidEffectiveDate.Visible = True
                    Else
                        lblInvalidEffectiveDate.Visible = False
                    End If
            End Select

            If Helpers.NewCo_Prefill_Helper.ShouldReturnToQuoteForPreFillOrNewCo(Me.Quote, Me.DefaultValidationType, Me.QuoteIdOrPolicyIdPipeImageNumber) Then
                Me.HideAccordFinalizeButtons()
                divNewCoRedirectMessage.Visible = True
                btnCommIRPM.Visible = False
                btnShowIRPM.Visible = False
                btnViewAccord.Visible = False
                Me.VRScript.AddScriptLine("var hdnInEditModeFlag = document.getElementById(hdnInEditModeFlagId);hdnInEditModeFlag.value = 'true';DoEditModeOverlay();")
            End If




        End If
        Me.QuoteSummaryActionsValidationHelper = Me.ValidationHelper 'added 5/22/2023


    End Sub

    Private Sub ToggleSubmitButtonEnabledStatusAndMessage()
        Dim transactionCount As Integer
        Dim transactionMsg As String = ""
        Dim strTypeOfEndorsement = TypeOfEndorsement()
        const CAPEndorsementsDictionaryName = "CAPEndorsementsDetails"

        Select Case strTypeOfEndorsement
            Case EndorsementStructures.EndorsementTypeString.CAP_AmendMailing
                'We are not stopping users from submitting address changes even if they do not change anything
                Exit Sub
            Case EndorsementStructures.EndorsementTypeString.CAP_AddDeleteVehicle
                EndorsementDictionaryName = CAPEndorsementsDictionaryName
                transactionCount = ddh.GetEndorsementVehicleTransactionCount()
                transactionMsg = "Before submitting, you must add or delete a vehicle to complete your endorsement transaction."
            Case EndorsementStructures.EndorsementTypeString.CAP_AddDeleteDriver
                EndorsementDictionaryName = CAPEndorsementsDictionaryName
                transactionCount = ddh.GetEndorsementTransactionCount()
                transactionMsg = "Before submitting, you must add or delete a driver to complete your endorsement transaction."
            Case EndorsementStructures.EndorsementTypeString.CAP_AddDeleteAI
                EndorsementDictionaryName = CAPEndorsementsDictionaryName
                transactionCount = ddh.GetEndorsementAdditionalInterestTransactionCount()
                transactionMsg = "Before submitting, you must add or delete an Additional Interest to complete your endorsement transaction."
        End Select

        Dim enableSubmit = GetSubmitButtonEnabledStatus(transactionCount)
        SetSubmitButtonEnabledStatusAndMessage(enableSubmit, transactionMsg)
    End Sub

    Private Function GetSubmitButtonEnabledStatus(transactionCount As Integer) As Boolean
        Dim enableSubmit As Boolean = True
        If transactionCount <= 0 Then
            enableSubmit = False
        End If
        Return enableSubmit
    End Function

    Private Sub SetSubmitButtonEnabledStatusAndMessage(enableSubmit As Boolean, transactionMsg As String)
        Me.btnCommSubmitEndorsement.Enabled = enableSubmit
        divSubmitDisabledMessage.Visible = Not enableSubmit
        lblSubmitDisabledMsg.Text = transactionMsg
    End Sub

    Private Sub HideAccordFinalizeButtons()
        If IsCommercialQuote() Then
            If IsOnAppPage Then
                btnContinueToApp.Visible = False
            End If
            Me.btnCommContinueToApplication.Visible = False
            Me.btnCommViewWorksheet.Visible = False
            'Added 01/12/2021 for CAP Endorsements Task 52976 MLW
            Me.btnCommSubmitEndorsement.Visible = False
        Else
            Me.btnContinueToApp.Visible = False
            Me.btnViewAccord.Visible = False
        End If
        Me.ctl_RouteToUw.Visible = True

        'added 2/19/2020
        If Me.ValidationHelper.HasErrros = True Then
            Dim valErrors As List(Of WebValidationItem) = Me.ValidationHelper.GetErrors
            If valErrors IsNot Nothing AndAlso valErrors.Count = 1 AndAlso valErrors(0) IsNot Nothing AndAlso valErrors(0).Message = reRateMessage Then
                Me.ctl_RouteToUw.Visible = False
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Protected Sub btnContinueToApp_Click(sender As Object, e As EventArgs) Handles btnContinueToApp.Click, btnCommContinueToApplication.Click, btnCommSubmitEndorsement.Click
        If IsOnAppPage Then
            ' never happens
        Else
            If Me.Quote IsNot Nothing Then
                'If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then 'added IF 2/27/2019; original logic in ELSE
                '    Dim imgQueryString As String = ""
                '    If String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                '        imgQueryString = "?EndorsementPolicyIdAndImageNum=" & Me.EndorsementPolicyId.ToString & "|" & Me.EndorsementPolicyImageNum.ToString

                '        'added 5/21/2019 to Submit Endorsements from Quote side
                '        If sender IsNot Nothing AndAlso TypeOf sender Is Button AndAlso sender.Equals(Me.btnContinueToApp) = True AndAlso UCase(Me.btnContinueToApp.Text) = "SUBMIT" Then
                '            FinalizeEndorsement()
                '            Exit Sub
                '        End If
                '    Else
                '        imgQueryString = "?ReadOnlyPolicyIdAndImageNum=" & Me.ReadOnlyPolicyId.ToString & "|" & Me.ReadOnlyPolicyImageNum.ToString
                '    End If
                '    Select Case Me.Quote.LobId
                '        Case "1"
                '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_App") & imgQueryString)
                '        Case "2"
                '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_App") & imgQueryString)
                '        Case "17"
                '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_App") & imgQueryString)
                '        Case "3"
                '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_App") & imgQueryString)
                '        Case "9" ' CGL
                '            Response.Redirect("VR3CGLApp.aspx" & imgQueryString)
                '        Case "25" ' BOP
                '            Response.Redirect("VR3BOPApp.aspx" & imgQueryString)
                '        Case "20" ' CAP
                '            Response.Redirect("VR3CAPApp.aspx" & imgQueryString)
                '        Case "21" ' WCP
                '            Response.Redirect("VR3WCPApp.aspx" & imgQueryString)
                '        Case "28" 'CPR
                '            Response.Redirect("VR3CPRApp.aspx" & imgQueryString)
                '        Case "23"  ' CPP
                '            Response.Redirect("VR3CPPApp.aspx" & imgQueryString)
                '    End Select
                'Else
                '    Select Case Me.Quote.LobId
                '        Case "1"
                '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_App") + "?quoteid=" + QuoteId.ToString())
                '        Case "2"
                '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_App") + "?quoteid=" + QuoteId.ToString())
                '        Case "17"
                '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_App") + "?quoteid=" + QuoteId.ToString())
                '        Case "3"
                '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_App") + "?quoteid=" + QuoteId.ToString())
                '        Case "9" ' CGL
                '            Response.Redirect("VR3CGLApp.aspx?quoteid=" + QuoteId.ToString())
                '        Case "25" ' BOP
                '            Response.Redirect("VR3BOPApp.aspx?quoteid=" + QuoteId.ToString())
                '        Case "20" ' CAP
                '            Response.Redirect("VR3CAPApp.aspx?quoteid=" + QuoteId.ToString())
                '        Case "21" ' WCP
                '            Response.Redirect("VR3WCPApp.aspx?quoteid=" + QuoteId.ToString())
                '        Case "28" 'CPR
                '            Response.Redirect("VR3CPRApp.aspx?quoteid=" + QuoteId.ToString())
                '        Case "23"  ' CPP
                '            Response.Redirect("VR3CPPApp.aspx?quoteid=" + QuoteId.ToString())
                '    End Select
                'End If
                'updated 6/11/2019 to use new helper method
                Dim polIdToUse As Integer = 0
                Dim polImgNumToUse As Integer = 0
                Dim quoteIdToUse As Integer = 0
                Dim tranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.None
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    If String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                        'Updated 01/13/2021 for CAP Endorsements Task 52976 MLW
                        If sender IsNot Nothing AndAlso TypeOf sender Is Button AndAlso (sender.Equals(Me.btnContinueToApp) = True OrElse sender.Equals(Me.btnCommSubmitEndorsement) = True) AndAlso UCase(Me.btnContinueToApp.Text) = "SUBMIT" Then
                            'If sender IsNot Nothing AndAlso TypeOf sender Is Button AndAlso sender.Equals(Me.btnContinueToApp) = True AndAlso UCase(Me.btnContinueToApp.Text) = "SUBMIT" Then
                            FinalizeEndorsement()
                            Exit Sub
                        End If
                        polIdToUse = Me.EndorsementPolicyId
                        polImgNumToUse = Me.EndorsementPolicyImageNum
                        tranType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                    Else
                        polIdToUse = Me.ReadOnlyPolicyId
                        polImgNumToUse = Me.ReadOnlyPolicyImageNum
                        tranType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                    End If
                Else
                    quoteIdToUse = QQHelper.IntegerForString(Me.QuoteId)
                    tranType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
                End If
                If quoteIdToUse > 0 OrElse (polIdToUse > 0 AndAlso polImgNumToUse > 0) Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType, Me.Quote.LobType, quoteId:=quoteIdToUse, policyId:=polIdToUse, policyImageNum:=polImgNumToUse, goToApp:=True)
                End If
            Else
                Me.VRScript.AddScriptLine("alert('Please re-rate quote before continuing.');", True)
            End If

        End If
    End Sub

    'Protected Sub btnCreateHomeQuote_Click(sender As Object, e As EventArgs) Handles btnCreateHomeQuote.Click
    '    ' create the new quote
    '    If Me.Quote IsNot Nothing Then
    '        Dim newQuoteLOBID As Int32 = 0
    '        Select Case Me.Quote.LobId
    '            Case "1"
    '                newQuoteLOBID = 2
    '            Case "2", "3" 'Hom,DFR
    '                newQuoteLOBID = 1
    '        End Select

    '        If Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then 'added IF 2/15/2019; original logic in ELSE
    '            'shouldn't be valid for ReadOnly or Endorsement images
    '        Else
    '            Dim newQuoteId As Int32 = Me.CreateQuote(newQuoteLOBID)
    '            If newQuoteId > 0 Then
    '                ' attach client id and policyholder data
    '                Dim newQuote As QuickQuote.CommonObjects.QuickQuoteObject = Common.QuoteSave.QuoteSaveHelpers.GetQuoteById_NOSESSION(newQuoteId.ToString)
    '                If newQuote IsNot Nothing Then
    '                    newQuote.Client.ClientId = Me.Quote.Client.ClientId
    '                    newQuote.Policyholder = Me.Quote.Policyholder
    '                    newQuote.Policyholder2 = Me.Quote.Policyholder2
    '                    newQuote.CopyPolicyholdersToClients()

    '                    Select Case newQuoteLOBID
    '                        Case 2
    '                            newQuote.CopyPolicyholdersToApplicants()
    '                            ' it will get a location created on the UW Question popup - Matt A
    '                    End Select

    '                    Dim errMsg As String = ""
    '                    If Common.QuoteSave.QuoteSaveHelpers.SaveQuote(newQuoteId.ToString, newQuote, errMsg, QuickQuoteXML.QuickQuoteSaveType.Quote) Then
    '                        'load quote to show kill questions
    '                        Select Case newQuoteLOBID
    '                            Case 1
    '                                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_Input") + "?quoteid=" + newQuoteId.ToString())
    '                            Case 2
    '                                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_Input") + "?quoteid=" + newQuoteId.ToString())
    '                        End Select

    '                    Else
    '                        Me.ValidationHelper.AddError(errMsg)
    '                    End If
    '                End If
    '            Else
    '                Select Case newQuoteLOBID
    '                    Case 1
    '                        Me.ValidationHelper.AddError("Creating new auto quote from this quote failed.")
    '                    Case 2
    '                        Me.ValidationHelper.AddError("Creating new home quote from this quote failed.")
    '                End Select

    '            End If
    '        End If
    '    Else
    '        Me.VRScript.AddScriptLine("alert('Please re-rate quote before continuing.');", True)
    '    End If

    'End Sub

    Private Function CreateQuote(lobID As Int32) As Int32
        Dim agencyId As Int32 = (DirectCast(Me.Page.Master, VelociRater)).AgencyID
        Dim agencyCode As String = (DirectCast(Me.Page.Master, VelociRater)).AgencyCode
        Dim newQuoteId As String = ""
        Dim errorMsg As String = ""
        If IFM.VR.Common.QuoteSave.QuoteSaveHelpers.CreateNewQuote(agencyId, agencyCode, lobID, newQuoteId, errorMsg) = False Then
            'error
        End If
        Dim qID As Int32 = 0
        Int32.TryParse(newQuoteId, qID)
        Return qID
    End Function

    ''' <summary>
    ''' This only occurrs after the client side script runs that asks if they want to print the ACCORD prior to finalizing.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub lnkFinalize_Click(sender As Object, e As EventArgs) Handles lnkFinalize.Click, btnContinueToApp.Click

        If IsOnAppPage Then
            If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/15/2019; original logic in ELSE
                'nothing to Finalize for ReadOnly; maybe redirect back to MyVelociRater
            ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                'nothing to do for Endorsements but redirect to MakeAPayment for Issuance
                'If Me.Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Me.Quote.PolicyNumber) = False Then
                '    Dim makeAPaymentLink As String = QuickQuoteHelperClass.configAppSettingValueAsString("MakeAPaymentLink")
                '    If String.IsNullOrWhiteSpace(makeAPaymentLink) = True Then
                '        Dim agentsLink As String = QuickQuoteHelperClass.configAppSettingValueAsString("AgentsLink")
                '        If String.IsNullOrWhiteSpace(agentsLink) = False AndAlso UCase(agentsLink).Contains("AGENTS.ASPX") AndAlso Len(agentsLink) > 11 Then 'has full url
                '            makeAPaymentLink = UCase(agentsLink).Replace("AGENTS.ASPX", "MakeAPayment.aspx?polNum=")
                '        Else
                '            makeAPaymentLink = "MakeAPayment.aspx?polNum="
                '        End If
                '    End If
                '    makeAPaymentLink &= Me.Quote.PolicyNumber & "&Quote=Yes"
                '    Response.Redirect(makeAPaymentLink)
                'Else
                '    Me.ValidationHelper.AddError("Unable to load information needed for issuance.")
                'End If
                'updated 5/21/2019 to use new common method
                FinalizeEndorsement()
            Else
                Dim QQxml As New QuickQuoteXML()
                Dim policyNumber As String = ""
                Dim errorMsg As String = ""
                'If QQxml.DiamondService_SuccessfullyPromotedQuote(Me.Quote, policyNumber, "", errorMsg) Then
                'updated 5/2/2023
                Dim valItems As List(Of QuickQuoteValidationItem) = Nothing
                Dim promoteCaughtUnhandledException As Boolean = False
                If QQxml.DiamondService_SuccessfullyPromotedQuote_ReturnValidationItems(Me.Quote, newPolicyNumber:=policyNumber, errorMsg:=errorMsg, valItems:=valItems, promoteCaughtUnhandledException:=promoteCaughtUnhandledException) Then
                    ' remove this quote from recent quote log
                    Helpers.WebHelper_Personal.RemoveQuoteIdFromSessionHistory(Session, Me.QuoteId)
                    ' get current policy number -  "polnum="
                    ' goto make a payment
                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("MakeAPaymentLink") + policyNumber + "&PolicyId=" + Me.Quote.PolicyId + "&quote=Yes&VrQuoteId=" + Me.QuoteId)
                Else
                    ' show route to uw button
                    'Me.ValidationHelper.AddError("Application finalization failed. " + errorMsg)
                    'updated 5/9/2023
                    Dim valMsg As String = "Application finalization failed."
                    If String.IsNullOrWhiteSpace(errorMsg) = False Then
                        valMsg &= " " & errorMsg
                    End If
                    Me.ValidationHelper.AddError(valMsg)
                    If promoteCaughtUnhandledException = False AndAlso valItems IsNot Nothing AndAlso valItems.Count > 0 Then 'added 5/2/2023
                        For Each vi As QuickQuoteValidationItem In valItems
                            If vi IsNot Nothing AndAlso vi.ValidationSeverityType = QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationError AndAlso String.IsNullOrWhiteSpace(vi.Message) = False Then
                                Me.ValidationHelper.AddError(vi.Message)
                            End If
                        Next
                    End If
                    Me.ctl_RouteToUw.Visible = True
                End If
            End If
            Me.QuoteSummaryActionsValidationHelper = Me.ValidationHelper 'added 5/19/2023
        End If
    End Sub

    Protected Sub btnViewAccord_Click(sender As Object, e As EventArgs) Handles btnViewAccord.Click, btnCommViewWorksheet.Click
        If IsCommercialQuote() And IsOnAppPage() Then
            Me.Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.documentPrinting, "")
        Else
            PrintAcord()
        End If
    End Sub

    Private Sub PrintAcord()
        If Me.Quote IsNot Nothing Then
            If IsNumeric(Me.Quote.PolicyId) Then
                Dim loginName As String = ""
                Dim loginPassword As String = ""
                If System.Web.HttpContext.Current.Session("DiamondUsername") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondUsername").ToString <> "" Then
                    loginName = System.Web.HttpContext.Current.Session("DiamondUsername").ToString
                ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                    loginName = ConfigurationManager.AppSettings("QuickQuoteTestUsername").ToString
                End If
                If System.Web.HttpContext.Current.Session("DiamondPassword") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondPassword").ToString <> "" Then
                    loginPassword = System.Web.HttpContext.Current.Session("DiamondPassword").ToString
                ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                    loginPassword = ConfigurationManager.AppSettings("QuickQuoteTestPassword").ToString
                End If


                'Change PrintType by LOB
                Dim PrintType As Common.PrintHistories.PrintHistories.PrintType

                Select Case Me.Quote.LobType
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        PrintType = Common.PrintHistories.PrintHistories.PrintType.JustWorksheet
                    Case Else
                        PrintType = Common.PrintHistories.PrintHistories.PrintType.JustApplication
                End Select

                Dim bytes As Byte() = Common.PrintHistories.PrintHistories.GetPrintHistories(Me.Quote.PolicyId,
                                                                                             loginName,
                                                                                             loginPassword,
                                                                                             PrintType,
                                                                                             "",
                                                                                             Common.PrintHistories.PrintHistories.PrintFormDescriptionEvaluationType.OnlyUniqueFormDescriptions)
                If bytes IsNot Nothing Then
                    Response.ContentType = "application/pdf"
                    If PrintType = Common.PrintHistories.PrintHistories.PrintType.JustApplication Then
                        Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("ACCORD{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
                    Else
                        Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
                    End If
                    Response.BinaryWrite(bytes)
                Else

                    Me.VRScript.AddScriptLine("alert('Could not find Acord.');")
                End If

            End If
        End If
    End Sub

    Private Sub LoadLinkedApplications()
        Dim html As String = "<div>Linked Applications<br />"
        html += "Select rated application(s) with a common insured to finalize with current application.<br />"
        html += "<table class=""linkedQuotesAppTable"" id=""tblLinkedApps"">"
        html += "<tr>"
        html += "<td>"
        html += ""
        html += "</td>"
        html += "<th>"
        html += "Quote Number"
        html += "</th>"
        html += "<th>"
        html += "ACORD"
        html += "</th>"
        html += "<th>"
        html += "Line of Business"
        html += "</th>"
        html += "<th>"
        html += "Premium"
        html += "</th>"
        html += "<th>"
        html += "Effective Date"
        html += "</th>"
        html += "<th>"
        html += "Last Modified"
        html += "</th>"
        html += "</tr>"
        If LinkedApplications IsNot Nothing Then
            ' seed quote
            html += "<tr title=""Current Quote"">"
            html += "<td>"
            html += "<input checked=""checked"" disabled=""disabled"" id=""a_" + Me.QuoteId + """ type=""checkbox"" quoteid=""" + Me.QuoteId + """ />"
            html += "</td>"
            html += "<td>"
            html += Me.Quote.QuoteNumber
            html += "</td>"
            html += "<td>"
            html += "<a target=""_blank"" title=""View ACORD"" href=""VR3ViewAccord.aspx?quoteid=" + Me.QuoteId + """>View</a>"
            html += "</td>"
            html += "<td>"
            html += IFM.VR.Common.QuoteSearch.QuoteSearch.GetLobNameFromId(CInt(Me.Quote.LobId))
            html += "</td>"
            html += "<td>"
            html += Me.Quote.TotalQuotedPremium
            html += "</td>"
            html += "<td>"
            html += Me.Quote.EffectiveDate
            html += "</td>"
            html += "<td>"
            html += "n/a"
            html += "</td>"
            html += "</tr>"
            ' seed quote

            For Each q In LinkedApplications
                Dim dbQuote = IFM.VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById_NOSESSION(q.QuoteId.ToString)
                If dbQuote IsNot Nothing Then
                    html += "<tr>"
                    html += "<td>"
                    Dim valItems = PolicyLevelValidator.PolicyValidation(dbQuote, Me.DefaultValidationType)
                    If valItems.ListHasValidationId(PolicyLevelValidator.EffectiveDate) Then
                        'has a effective date that is not valid
                        html += "<a href=""" + q.ViewUrl + """ title=""Open application. Requires effective date change."">Resume</a>"
                        'html += "<input id=""q" + q.QuoteId.ToString() + """ type=""checkbox"" disabled=""disabled"" title=""Effective Date is invalid. Requires re-rating."" quoteid=""" + q.QuoteId.ToString() + """ />"
                    Else
                        html += "<input id=""q" + q.QuoteId.ToString() + """ type=""checkbox"" quoteid=""" + q.QuoteId.ToString() + """ />"
                    End If
                    html += "</td>"
                    html += "<td>"
                    html += "<label title=""" + q.Description + """>" + q.QuoteNumber + "</label>"
                    html += "</td>"
                    html += "<td>"
                    html += "<a target=""_blank"" title=""View ACORD"" href=""VR3ViewAccord.aspx?quoteid=" + Me.QuoteId + """>View</a>"
                    html += "</td>"
                    html += "<td>"
                    html += q.LobName
                    html += "</td>"
                    html += "<td>"
                    html += q.FormatedPremium
                    html += "</td>"
                    html += "<td>"
                    If valItems.ListHasValidationId(PolicyLevelValidator.EffectiveDate) Then
                        html += "<span title=""The effective date is not is not within an acceptable time frame of todays date."" style=""color:red"">" + dbQuote.EffectiveDate + "</span>"
                    Else
                        html += dbQuote.EffectiveDate
                    End If

                    html += "</td>"
                    html += "<td>"
                    html += q.LastModified_FriendlyDate
                    html += "</td>"

                    html += "</tr>"
                End If

            Next
        End If
        html += "</table>"

        html += "<center>"
        html += "<div style=""margin-top: 20px;"">"
        html += "<input id=""btnFinalizeLinkedApps"" class=""StandardSaveButton"" onclick='DobtnFinalizeLinkedApps_click();' type=""button"" value=""Finalize Linked Applications"" />"
        html += "</div>"
        html += "</center>"
        html += "</div>"
        Me.tblLinedApps.Text = html
    End Sub

    Private Function GetLinkedApplications() As List(Of QQSearchResult)
        Dim results As List(Of QQSearchResult) = Nothing
        If Me.Quote IsNot Nothing Then
            Dim masterPage As VelociRater = DirectCast(Me.Page.Master, VR.Web.VelociRater)

            'look at this quotes lob and decide what other lobs you want to search for
            'make sure the lob is supported by VR
            Dim lobIdList As String = ""
            Select Case Me.Quote.LobId
                Case "1"
                    If IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(2) Then
                        lobIdList = "2"
                    End If
                Case "2"
                    If IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(1) Then
                        lobIdList = "1"
                    End If
            End Select

            'need to specify specific LOBs that are allowed or else you get nothing
            If String.IsNullOrWhiteSpace(lobIdList) = False Then
                ' search by agencyid, lobids, clientid, app rated status, exclude this quotes lobid - do not give staff any more search rights than agents
                ' default sort is by 'last modified date' desc
                results = IFM.VR.Common.QuoteSearch.QuoteSearch.SearchForQuotes(0, "", masterPage.AgencyID, "", "", "8", lobIdList, Me.Quote.LobId, False, True, 0, False, QQHelper.CanUserAccessEmployeePolicies(), CInt(System.Web.HttpContext.Current.Session("DiamondUserId")), clientId:=CInt(Me.Quote.Client.ClientId))
            End If
        End If
        Return results
    End Function

    Private Sub LoadLinkedQuotes()
        Dim html As String = "<div style=""margin-top:20px;"">Linked Quotes<br />"
        html += "Select a quote with a common insured to resume.<br />"
        html += "<table class=""linkedQuotesAppTable"" id=""tblLinkedQuotes"">"
        html += "<tr>"
        html += "<td>"
        html += ""
        html += "</td>"
        html += "<th>"
        html += "Quote Number"
        html += "</th>"
        html += "<th>"
        html += "Line of Business"
        html += "</th>"
        html += "<th>"
        html += "Premium"
        html += "</th>"
        html += "<th>"
        html += "Status"
        html += "</th>"
        html += "<th>"
        html += "Last Modified"
        html += "</th>"
        html += "</tr>"
        If LinkedQuotes IsNot Nothing Then

            For Each q In LinkedQuotes
                html += "<tr>"
                html += "<td>"
                'html += "<input id=""q_" + q.QuoteId.ToString() + """ onchange=""UncheckAllButSender(this,'tblLinkedQuotes');"" type=""checkbox"" quoteid=""" + q.QuoteId.ToString() + """ quotelobid=""" + q.LobId.ToString() + """ quotestatusid=""" + q.StatusId.ToString() + """ />"
                html += "<a href=""" + q.ViewUrl + """ title=""Resume quote."">Resume</a>"
                html += "</td>"
                html += "<td>"
                html += "<label title=""" + q.Description + """>" + q.QuoteNumber + "</label>"
                html += "</td>"
                html += "<td>"
                html += q.LobName
                html += "</td>"
                html += "<td>"
                html += q.FormatedPremium
                html += "</td>"
                html += "<td>"
                html += q.FriendlyStatus
                html += "</td>"
                html += "<td>"
                html += q.LastModified_FriendlyDate
                html += "</td>"

                html += "</tr>"
            Next
        End If
        html += "</table>"

        'html += "<center>"
        'html += "<div style=""margin-top: 20px;"">"
        'html += "<input id=""btnLoadLinkedQuotes"" onclick='DobtnLoadLinkedQuotes_click();' class=""StandardSaveButton"" type=""button"" value=""Resume Selected Quote"" />"
        'html += "</div>"
        'html += "</center>"
        html += "</div>"
        Me.tblLinkedQuotes.Text = html
    End Sub

    Private Function GetLinkedQuotes() As List(Of QQSearchResult)
        Dim results As List(Of QQSearchResult) = Nothing
        If Me.Quote IsNot Nothing Then
            Dim masterPage As VelociRater = DirectCast(Me.Page.Master, VR.Web.VelociRater)

            'look at this quotes lob and decide what other lobs you want to search for
            'make sure the lob is supported by VR
            Dim lobIdList As String = ""
            Select Case Me.Quote.LobId
                Case "1"
                    If IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(2) Then
                        lobIdList = "2"
                    End If
                Case "2"
                    If IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(1) Then
                        lobIdList = "1"
                    End If
            End Select

            'need to specify specific LOBs that are allowed or else you get nothing
            If String.IsNullOrWhiteSpace(lobIdList) = False Then
                ' search by agencyid, lobids, clientid, app rated status, exclude this quotes lobid - do not give staff any more search rights than agents
                ' default sort is by 'last modified date' desc
                results = IFM.VR.Common.QuoteSearch.QuoteSearch.SearchForQuotes(0, "", masterPage.AgencyID, "", "", "2,3,4,5,6,7,9,10,11", lobIdList, Me.Quote.LobId, False, True, 0, False, QQHelper.CanUserAccessEmployeePolicies(), CInt(System.Web.HttpContext.Current.Session("DiamondUserId")), clientId:=CInt(Me.Quote.Client.ClientId))
            End If
        End If
        Return results
    End Function

    Private Function hasIRPMAdjustment() As Boolean
        'Updated 9/7/18 for multi state MLW - Quote to SubQuoteFirst
        If Quote IsNot Nothing AndAlso SubQuoteFirst IsNot Nothing AndAlso SubQuoteFirst.ScheduledRatings IsNot Nothing Then
            For Each item In Me.SubQuoteFirst.ScheduledRatings
                If item.RiskFactor IsNot Nothing AndAlso item.RiskFactor <> "1.000" Then
                    Return True
                End If
            Next
        End If
        Return False
    End Function

    Protected Sub btnShowIRPM_Click(sender As Object, e As EventArgs) Handles btnShowIRPM.Click, btnCommIRPM.Click
        'Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New VRValidationArgs(DefaultValidationType)))
        'RaiseEvent RequestNavigationToIRPM()
        Me.Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.farmIRPM, "") 'Matt A 8-19-15
    End Sub

    'Private Sub btnCommEmailForUWAssistance_Click(sender As Object, e As EventArgs) Handles btnCommEmailForUWAssistance.Click
    '    'Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
    '    '_script.AddScriptLine("InitEmailToUW();")
    'End Sub

    Private Sub btnCommStartNewQuote_Click(sender As Object, e As EventArgs) Handles btnCommStartNewQuote.Click, btnPolicyholderStartNewQuote.Click
        'This is now its own control within Velocirater.  The control is added to the workflows of all commercial lines.
        'a javascript call on the button calls a new popup.
        'CAH 4/24/2018


        'If Me.Quote IsNot Nothing AndAlso Me.Quote.Client IsNot Nothing AndAlso Me.Quote.AgencyId <> "" AndAlso Me.Quote.AgencyCode <> "" Then
        '    Dim qq As New QuickQuote.CommonObjects.QuickQuoteObject
        '    qq.AgencyId = Me.Quote.AgencyId
        '    qq.AgencyCode = Me.Quote.AgencyCode
        '    Dim qqhelper = New QuickQuoteHelperClass
        '    qq.Client = qqhelper.CloneObject(Me.Quote.Client)
        '    Dim qId As String = ""
        '    Dim errMsg As String = ""
        '    Dim QQxml = New QuickQuoteXML
        '    QQxml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qId, errMsg)
        '    If errMsg = "" Then
        '        'okay
        '        If qId <> "" Then
        '            'ShowError("Your quote was created successfully.  You will now be redirected to the LOB Selection page.", True, ConfigurationManager.AppSettings("QuickQuote_LOB_Selection").ToString & "?QuoteId=" & qId)
        '            Response.Redirect(ConfigurationManager.AppSettings("QuickQuote_LOB_Selection").ToString & "?QuoteId=" & qId)
        '        Else
        '            'problem
        '            'ShowError("There was a problem creating your new quote.  Please try again later.")
        '        End If
        '    Else
        '        'ShowError("There was a problem creating your new quote:  " & errMsg)
        '    End If
        'Else
        '    'ShowError("A new quote cannot be created for this client right now.  Please try again later.")
        'End If
    End Sub

    Private Sub btnCommPrepareProposal_Click(sender As Object, e As EventArgs) Handles btnCommPrepareProposal.Click
        If IsCommercialQuote() Then
            Me.Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.proposal, "")
        Else
            Response.Redirect(AppSettings("QuickQuote_QuoteProposalSelection") + "?quoteid=" + Me.QuoteId.ToString())
        End If

    End Sub

    Private Sub btnDeleteEndorsement_Click(sender As Object, e As EventArgs) Handles btnDeleteEndorsement.Click, btnCommDeleteEndorsement.Click
        Dim EndoPandI = Quote.PolicyId + "|" + Quote.PolicyImageNum
        Dim redirect As String = "MyVelociRater.aspx?EndorsementPolicyImageToDelete=" + EndoPandI
        Response.Redirect(redirect, True)

    End Sub

    'added 5/21/2019
    Private Sub FinalizeEndorsement()
        If String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
            'nothing to do for Endorsements but redirect to MakeAPayment for Issuance
            If Me.Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Me.Quote.PolicyNumber) = False Then
                Dim endorsementValidationErrorMsg As String = ""

                '5/24/2019 - added Vehicle VIN check on new vehicles
                If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
                    If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > 0 Then
                        Dim vehCounter As Integer = 0
                        For Each v As QuickQuote.CommonObjects.QuickQuoteVehicle In Me.Quote.Vehicles
                            vehCounter += 1
                            'If v IsNot Nothing AndAlso QQHelper.IsQuickQuoteVehicleNewToImage(v, Me.Quote.TransactionEffectiveDate, Me.Quote.EffectiveDate, Me.Quote.PCAdded_Date) = True Then
                            'updated 7/25/2019 to use new IsQuickQuoteVehicleNewToImage method
                            If v IsNot Nothing AndAlso QQHelper.IsQuickQuoteVehicleNewToImage(v, Me.Quote) = True Then
                                Dim valItems = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.VehicleValidator.VehicleValidation(vehCounter - 1, Me.Quote, ValidationItem.ValidationType.appRate)
                                If valItems.Any() Then
                                    For Each vi In valItems
                                        If vi.IsWarning = False Then 'if statement added 04/27/2020 for bug 45376 MLW
                                            Select Case vi.FieldId
                                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.VehicleValidator.VehicleVIN
                                                    endorsementValidationErrorMsg = "Full VIN # is required. Return to the Vehicle Page to add the incomplete information."
                                                    Exit For
                                            End Select
                                        End If
                                    Next
                                End If
                            End If
                            If String.IsNullOrWhiteSpace(endorsementValidationErrorMsg) = False Then
                                Exit For
                            End If
                        Next
                    End If
                End If

                If String.IsNullOrWhiteSpace(endorsementValidationErrorMsg) = True Then
                    'no validation errors
                    'Dim makeAPaymentLink As String = QuickQuoteHelperClass.configAppSettingValueAsString("MakeAPaymentLink")
                    'If String.IsNullOrWhiteSpace(makeAPaymentLink) = True Then
                    '    Dim agentsLink As String = QuickQuoteHelperClass.configAppSettingValueAsString("AgentsLink")
                    '    If String.IsNullOrWhiteSpace(agentsLink) = False AndAlso UCase(agentsLink).Contains("AGENTS.ASPX") AndAlso Len(agentsLink) > 11 Then 'has full url
                    '        makeAPaymentLink = UCase(agentsLink).Replace("AGENTS.ASPX", "MakeAPayment.aspx?polNum=")
                    '    Else
                    '        makeAPaymentLink = "MakeAPayment.aspx?polNum="
                    '    End If
                    'End If
                    'makeAPaymentLink &= Me.Quote.PolicyNumber & "&Quote=Yes"
                    'makeAPaymentLink &= "&VrQuoteId=" & EndorsementPolicyIdAndImageNum 'added 6/20/2019

                    '5/23/2019 - added logic to update status
                    'Dim qqXml As New QuickQuoteXML
                    'Dim updateStatusErrorMsg As String = ""
                    'qqXml.UpdateQuoteStatus(Me.Quote, QuickQuoteXML.QuickQuoteStatusType.Finalized, updateStatusErrorMsg)

                    'Response.Redirect(makeAPaymentLink)

                    'updated 10/11/2019
                    Dim qqXml As New QuickQuoteXML
                    Dim finalizeErrorMsg As String = ""
                    Dim successfullyFinalized As Boolean = False
                    Dim attemptedPromote As Boolean = False
                    Dim successfullyPromoted As Boolean = False
                    'qqXml.FinalizeEndorsement(Me.Quote, onlyFinalizeWhenNoFailureOnPromotion:=True, errorMsg:=finalizeErrorMsg, successfullyFinalized:=successfullyFinalized, attemptedToPromote:=attemptedPromote, successfullyPromoted:=successfullyPromoted)
                    'updated 5/2/2023
                    Dim valItems As List(Of QuickQuoteValidationItem) = Nothing
                    Dim promoteCaughtUnhandledException As Boolean = False
                    qqXml.FinalizeEndorsement_ReturnValidationItems(Me.Quote, onlyFinalizeWhenNoFailureOnPromotion:=True, errorMsg:=finalizeErrorMsg, successfullyFinalized:=successfullyFinalized, attemptedToPromote:=attemptedPromote, successfullyPromoted:=successfullyPromoted, valItems:=valItems, promoteCaughtUnhandledException:=promoteCaughtUnhandledException)
                    If successfullyFinalized = True OrElse attemptedPromote = False OrElse successfullyPromoted = True Then 'we won't worry if it fails to update our table w/ the Finalized status; we just don't want to send the user to MakeAPayment if it needs to be Promoted and that has failed
                        Dim makeAPaymentLink As String = QuickQuoteHelperClass.configAppSettingValueAsString("MakeAPaymentLink")
                        If String.IsNullOrWhiteSpace(makeAPaymentLink) = True Then
                            Dim agentsLink As String = QuickQuoteHelperClass.configAppSettingValueAsString("AgentsLink")
                            If String.IsNullOrWhiteSpace(agentsLink) = False AndAlso UCase(agentsLink).Contains("AGENTS.ASPX") AndAlso Len(agentsLink) > 11 Then 'has full url
                                makeAPaymentLink = UCase(agentsLink).Replace("AGENTS.ASPX", "MakeAPayment.aspx?polNum=")
                            Else
                                makeAPaymentLink = "MakeAPayment.aspx?polNum="
                            End If
                        End If
                        'makeAPaymentLink &= Me.Quote.PolicyNumber & "&Quote=Yes"
                        'ws-2347 CAH 3/20/2024
                        makeAPaymentLink &= Me.Quote.PolicyNumber & "&PolicyId="
                        makeAPaymentLink &= Me.Quote.PolicyId & "&Quote=Yes"
                        makeAPaymentLink &= "&VrQuoteId=" & EndorsementPolicyIdAndImageNum
                        Response.Redirect(makeAPaymentLink)
                    Else
                        Dim friendlyFinalizeErrorMsg As String = ""
                        Dim failedPromote As Boolean = False 'added 5/2/2023
                        If attemptedPromote = True AndAlso successfullyPromoted = False Then
                            failedPromote = True 'added 5/2/2023
                            friendlyFinalizeErrorMsg = "Problem promoting Endorsement Quote to Pending Endorsement"
                        Else
                            friendlyFinalizeErrorMsg = "Problem finalizing Endorsement"
                            friendlyFinalizeErrorMsg &= "; please try again later." 'added 5/2/2023; will only append on non-promote failure
                        End If
                        'friendlyFinalizeErrorMsg &= "; please try again later." 'removed 5/2/2023; will only append on non-promote failure

                        Me.ValidationHelper.AddError(friendlyFinalizeErrorMsg)
                        If failedPromote = True AndAlso promoteCaughtUnhandledException = False AndAlso valItems IsNot Nothing AndAlso valItems.Count > 0 Then 'added 5/2/2023
                            For Each vi As QuickQuoteValidationItem In valItems
                                If vi IsNot Nothing AndAlso vi.ValidationSeverityType = QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationError AndAlso String.IsNullOrWhiteSpace(vi.Message) = False Then
                                    Me.ValidationHelper.AddError(vi.Message)
                                End If
                            Next
                        End If
                    End If
                Else
                    'show validation error
                    Me.ValidationHelper.AddError(endorsementValidationErrorMsg)
                End If
            Else
                Me.ValidationHelper.AddError("Unable to load information needed for issuance.")
            End If
        End If
    End Sub

    'added 2/18/2020
    Public Sub CheckForReRateAfterEffDateChange(Optional ByVal qqTranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.None, Optional ByVal newEffectiveDate As String = "", Optional ByVal oldEffectiveDate As String = "")
        If Me.Visible = True Then
            Dim showReRateMsg As Boolean = False
            If Me.btnContinueToApp.Visible = True Then
                Me.btnContinueToApp.Visible = False
                showReRateMsg = True
            ElseIf Me.btnCommContinueToApplication.Visible = True Then
                Me.btnCommContinueToApplication.Visible = False
                showReRateMsg = True
            End If
            If showReRateMsg = True Then
                'Me.ValidationHelper.AddError("Effective date change requires re-rate.")
                Me.ValidationHelper.AddError(reRateMessage)
            End If
        End If
    End Sub

    Public Sub NewCoRedirectLink_Click(sender As Object, e As EventArgs) Handles NewCoRedirectLink.ServerClick
        'Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders, "0")

        WebHelper_Personal.RedirectToQuotePage(Me.Quote.QuoteTransactionType, Me.Quote.LobType, quoteId:=QQHelper.IntegerForString(Me.QuoteId), policyId:=Me.EndorsementPolicyId, policyImageNum:=Me.EndorsementPolicyImageNum)
    End Sub

End Class