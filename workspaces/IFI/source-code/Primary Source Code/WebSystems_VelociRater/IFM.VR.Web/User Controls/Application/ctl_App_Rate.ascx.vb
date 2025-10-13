
Imports Diamond.Common.Objects.ThirdParty.Metro
Imports IFM.ControlFlags
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Common.Helpers.CGL
Imports IFM.VR.Common.Helpers.DFR
Imports IFM.VR.Flags
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects

Public Class ctl_App_Rate
    Inherits VRControlBase

    Public Event ApplicationRated()
    Public Event ApplicationRatedSuccessfully()

    Public Event UpdateUWQuestions() 'Added 8/2/2022 for task 75911 MLW
    Public Event UpdateCAPUMUIMSymbols()
    Public Event UpdateRACASymbols()

    Public Const NewCoAppRedirectToQuoteMessage = "Your quote requires additional details. Please click Return to Quote to ensure all coverages and required information are present and correct."
    Public Const NewCoAppRedirectToQuoteTitle = "Attention:"

    Public ReadOnly Property SaveBtnID() As String
        Get
            Return Me.btnsave.ClientID
        End Get
    End Property
    Public Property RouteToUwIsVisible As Boolean
        Get
            Return Me.ctl_RouteToUw.Visible
        End Get
        Set(value As Boolean)
            Me.ctl_RouteToUw.Visible = value
        End Set
    End Property
    'added 9/17/2015 for effDate validation; originally taken from treeview
    Private _minDate As Integer = -1000
    Private _maxDate As Integer = -1000
    Public ReadOnly Property MinimumEffectiveDateDaysFromToday As Integer
        Get
            If _minDate = -1000 Then
                _minDate = QuickQuoteHelperClass.MinimumEffectiveDateDaysFromToday()
            End If

            Return _minDate
        End Get
    End Property
    Public ReadOnly Property MaximumEffectiveDateDaysFromToday As Integer
        Get
            If _maxDate = -1000 Then
                _maxDate = QuickQuoteHelperClass.MaximumEffectiveDateDaysFromToday()
            End If

            Return _maxDate
        End Get
    End Property
    Public Property MinimumEffectiveDate As String
        Get
            If QQHelper.IsDateString(Me.hdnAppMinimumEffectiveDate.Value) = True Then
                Return CDate(Me.hdnAppMinimumEffectiveDate.Value).ToShortDateString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If QQHelper.IsDateString(value) = True Then
                Me.hdnAppMinimumEffectiveDate.Value = CDate(value).ToShortDateString
            Else
                Me.hdnAppMinimumEffectiveDate.Value = ""
            End If
        End Set
    End Property
    Public Property MaximumEffectiveDate As String
        Get
            If QQHelper.IsDateString(Me.hdnAppMaximumEffectiveDate.Value) = True Then
                Return CDate(Me.hdnAppMaximumEffectiveDate.Value).ToShortDateString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If QQHelper.IsDateString(value) = True Then
                Me.hdnAppMaximumEffectiveDate.Value = CDate(value).ToShortDateString
            Else
                Me.hdnAppMaximumEffectiveDate.Value = ""
            End If
        End Set
    End Property
    Public Property MinimumEffectiveDateAllQuotes As String
        Get
            If QQHelper.IsDateString(Me.hdnAppMinimumEffectiveDateAllQuotes.Value) = True Then
                Return CDate(Me.hdnAppMinimumEffectiveDateAllQuotes.Value).ToShortDateString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If QQHelper.IsDateString(value) = True Then
                Me.hdnAppMinimumEffectiveDateAllQuotes.Value = CDate(value).ToShortDateString
            Else
                Me.hdnAppMinimumEffectiveDateAllQuotes.Value = ""
            End If
        End Set
    End Property
    Public Property MaximumEffectiveDateAllQuotes As String
        Get
            If QQHelper.IsDateString(Me.hdnAppMaximumEffectiveDateAllQuotes.Value) = True Then
                Return CDate(Me.hdnAppMaximumEffectiveDateAllQuotes.Value).ToShortDateString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If QQHelper.IsDateString(value) = True Then
                Me.hdnAppMaximumEffectiveDateAllQuotes.Value = CDate(value).ToShortDateString
            Else
                Me.hdnAppMaximumEffectiveDateAllQuotes.Value = ""
            End If
        End Set
    End Property
    Public Property QuoteHasMinimumEffectiveDate As Boolean
        Get
            Return QQHelper.BitToBoolean(Me.hdnAppQuoteHasMinimumEffectiveDate.Value)
        End Get
        Set(value As Boolean)
            Me.hdnAppQuoteHasMinimumEffectiveDate.Value = value.ToString
        End Set
    End Property
    Public Property MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes As Boolean
        Get
            Return QQHelper.BitToBoolean(Me.hdnAppMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes.Value)
        End Get
        Set(value As Boolean)
            Me.hdnAppMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes.Value = value.ToString
        End Set
    End Property
    'added 5/16/2023
    Public Property BeforeDateMsg As String
        Get
            Return Me.hdnAppBeforeDateMsg.Value
        End Get
        Set(value As String)
            Me.hdnAppBeforeDateMsg.Value = value
        End Set
    End Property
    Public Property AfterDateMsg As String
        Get
            Return Me.hdnAppAfterDateMsg.Value
        End Get
        Set(value As String)
            Me.hdnAppAfterDateMsg.Value = value
        End Set
    End Property

    Private _previousDate As String
    Private Property PreviousDate As String
        Get
            Return _previousDate
        End Get
        Set(value As String)
            _previousDate = value
        End Set
    End Property

    Private _flags As IEnumerable(Of IFeatureFlag)
    Public Sub New()
        _flags = New List(Of IFeatureFlag) From {New LOB.PPA}
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub AddScriptAlways()
        ' 6/5/2019 MGB Added the following IF statement to handle when the Minimum & Maximum effective dates have not been set. 
        ' This was occurring on new quotes occasionally and this code fixes it.
        If (String.IsNullOrWhiteSpace(MinimumEffectiveDate) OrElse (Not IsDate(MinimumEffectiveDate))) OrElse (String.IsNullOrWhiteSpace(MaximumEffectiveDate) OrElse (Not IsDate(MaximumEffectiveDate))) Then
            ' Minimum and/or maximum effective dates are NOT set, attempt to set them
            SetMinimumAndMaximumEffectiveDates()
        End If

        ' Date picker initial value needs to default to quote effective date.  Task 52940.  6/3/21 MGB
        Me.btnShowEffectiveDate.Attributes.Add("onclick", "AppRateEffectiveDate.OpenEffectiveDatePopup('" & Quote.EffectiveDate & "');")

        If String.IsNullOrWhiteSpace(MinimumEffectiveDate) = False AndAlso IsDate(MinimumEffectiveDate) AndAlso String.IsNullOrWhiteSpace(MaximumEffectiveDate) = False AndAlso IsDate(MaximumEffectiveDate) Then
            Me.VRScript.CreateDatePicker(Me.txtEffectiveDate.ClientID, CDate(MinimumEffectiveDate), CDate(MaximumEffectiveDate))
        Else
            Me.VRScript.CreateDatePicker(Me.txtEffectiveDate.ClientID, Math.Abs(MinimumEffectiveDateDaysFromToday), MaximumEffectiveDateDaysFromToday)
        End If
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Added 08/03/2021 for CAP Endorsements Tasks 53028 and 53030 MLW 
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
            Me.VRScript.AddScriptLine("Cap.ToggleValidVIN(true);") 'Passing isOnAppPage = True/False because app gap has differently named buttons than quote side & endorsments that we are enabling/disabling and showing/hiding
        Else
            Me.VRScript.AddScriptLine("$('[id*=""spanRouteToUWContainer""]').css(""display"", """");")
        End If
    End Sub

    Public ReadOnly Property PrimaryLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        'do not populate the effective date we will gather it every time app rate is attempted

        '9/17/2015 - added logic to store different dates for validation purposes
        If Quote IsNot Nothing Then
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then 'added IF 3/7/2019; original logic in ELSE; note: could also use Quote.QuoteTransactionType
                'use tEffDate for ReadOnly and Endorsements; may not need to do anything here since nothing will be Saved
                Me.hdnAppOriginalEffectiveDate.Value = Quote.TransactionEffectiveDate

                'use button that doesn't bring up DatePicker
                IFM.VR.Web.Helpers.WebHelper_Personal.RemoveStyleFromWebControl(Me.btnFinalRate, "display") 'make visible
                IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToHtmlControl(Me.btnShowEffectiveDate, "display", "none") 'hide
            Else
                Me.hdnAppOriginalEffectiveDate.Value = Quote.EffectiveDate
            End If
            Me.hdnHasBlanketAcreage.Value = "false"
            If (Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso PrimaryLocation IsNot Nothing AndAlso PrimaryLocation.Acreages.Any(Function(a) a.LocationAcreageTypeId = "4")) Then
                Me.hdnHasBlanketAcreage.Value = "true"
                Me.hdnBlanketAcreageAvailableDate.Value = IFM.VR.Common.Helpers.FARM.FarmBlanketAcreageHelper.FarmBlanketAcreageEffDate
                lblBlanketAcreageWarning.Text = "Blanket Acreage coverage is only available for quotes with an effective date of " + Me.hdnBlanketAcreageAvailableDate.Value + " or later. Due to the change in the effective date, this coverage has been removed from your quote."
            End If
        End If
        SetMinimumAndMaximumEffectiveDates() '3/7/2019 note: may not be needed for ReadOnly or Endorsements

        'Added 08/04/2021 for CAP Endorsements Task 53030 MLW
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
            ctl_RouteToUw.Visible = True
        End If

    End Sub

    Public Overrides Function Save() As Boolean
        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 3/7/2019; original logic in ELSE; note: could also use Quote.QuoteTransactionType
            'nothing to Save for ReadOnly
        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            'don't change effDate or tEffDate for Endorsements
        Else
            If Quote IsNot Nothing Then
                If String.IsNullOrWhiteSpace(Me.txtEffectiveDate_Copy.Text) = False Then
                    If IsDate(Me.txtEffectiveDate_Copy.Text) Then
                        Dim ReturnValues As DateCrossingReturnValues = Nothing
                        Helpers.EffectiveDateHelper.CheckDateCrossing(Quote, Me.txtEffectiveDate_Copy.Text, Quote.EffectiveDate, ValidationHelper.ValidationErrors, ReturnValues:=ReturnValues) 'Added 6/2/2022 for task 74147 MLW
                        Me.Quote.EffectiveDate = Me.txtEffectiveDate_Copy.Text
                        If ReturnValues IsNot Nothing Then
                            If ReturnValues.DFRStandaloneDateCrossed Then
                                RaiseEvent UpdateUWQuestions()
                            End If
                            If ReturnValues.CAPUMUIMUMPDDateCrossed Then
                                RaiseEvent UpdateCAPUMUIMSymbols()
                            End If
                            If ReturnValues.RACASymbolsDateCrossed Then
                                RaiseEvent UpdateRACASymbols()
                            End If
                        End If
                    End If
                End If
                Return True
            End If
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Me.ValidationHelper.GroupName = "Application Rate"
        MyBase.ValidateControl(valArgs)

        If validateEffectiveDate Then ' you check this because you only want to validate if it is coming from the btnFinalRate_Click() below - all other times you do no validate the effective date because it isn't even on the screen
            Dim valList = IFM.VR.Validation.ObjectValidation.PolicyLevelValidator.PolicyValidation(Me.Quote, Me.DefaultValidationType, Me.hdnAppOriginalEffectiveDate.Value)
            If valList.Any() Then
                For Each v In valList
                    Select Case v.FieldId
                        Case IFM.VR.Validation.ObjectValidation.PolicyLevelValidator.EffectiveDate
                            Me.ValidationHelper.AddError(v.Message)
                    End Select
                Next
            End If
            ' Ohio Effective Date Validation - can be removed after 3/1/2021
            If SubQuotesContainsState("OH") Then
                If CDate(SubQuoteFirst.EffectiveDate) < CDate("2/1/2021") Then
                    Me.ValidationHelper.AddError("You have selected Ohio as your state.  We cannot write in Ohio prior to 2-1-21.  Please change your effective date to 2-1-21 or later.")
                End If
            End If
            'New Company Date Validation - can be removed after 2/1/2024
            If Me.Quote.Database_DiaCompany = QuickQuoteCompany.IndianaFarmersIndemnity AndAlso CDate(GoverningStateQuote.EffectiveDate) < NewCompanyIdHelper.NewCoGoverningStateEffDate(Quote) Then
                Dim earliestNewCoEffectiveDate = NewCompanyIdHelper.NewCoGoverningStateEffDate(Quote)
                Me.ValidationHelper.AddError($"The effective date for rated Indiana Farmers Indemnity Quotes must be on or after {earliestNewCoEffectiveDate:MM/dd/yyyy}")
            End If
            If NewCompanyIdHelper.isNewCompanyLocked(Quote) Then
                Dim earliestNewCoEffectiveDate = NewCompanyIdHelper.GetEarliestEffectiveDatePossible(Quote)
                If CDate(SubQuoteFirst.EffectiveDate) < earliestNewCoEffectiveDate Then
                    Me.ValidationHelper.AddError($"The effective date to rate this Indiana Farmers Indemnity Quote must be on or after {earliestNewCoEffectiveDate:MM/dd/yyyy}")
                End If
            End If
        End If
    End Sub

    Dim validateEffectiveDate As Boolean = False
    Protected Sub btnFinalRate_Click(sender As Object, e As EventArgs) Handles btnFinalRate.Click

        Me.Save() 'added 5/17/2016 to get effective date to save earlier
        Me.ctl_RouteToUw.Visible = False
        validateEffectiveDate = True ' needed above in ValidateControl()
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(If(Me.IsOnAppPage, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate, VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate))))
        validateEffectiveDate = False ' needed above in ValidateControl()

        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal Then
            Dim UmbrellaHelp = New UmbrellaUnderlyingValidation
            UmbrellaHelp.ValidateForSaveRateUnderlyingPolicies(Quote, Me.ValidationHelper)
        End If

        ' if on Quote Summary or App side, OldCo does not require Pre-Fill.  Since we are worried
        ' about NewCo only at this point, this code covers the pre-fill requirements, also.
        'Dim DiaRateCompanyId As Integer = 1
        'Dim VrRateCompanyId As Integer = 1
        'If Quote IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(Quote.Database_DiaCompanyId) Then
        '    DiaRateCompanyId = CInt(Quote.Database_DiaCompanyId)
        'End If
        'If QQHelper.IsPositiveIntegerString(Quote.CompanyId) Then
        '    VrRateCompanyId = CInt(Quote.CompanyId)
        '    If VrRateCompanyId > DiaRateCompanyId Then
        '        ParentVrControl.Populate()
        '        ValidationHelper.AddError(NewCoAppRedirectToQuoteMessage, True, NewCoAppRedirectToQuoteTitle)
        '    End If
        'End If


        If Not IsQuoteReadOnly() Then
            CGLMedicalExpensesExcludedClassCodesHelper.UpdateAndShowMessagesForMedicalExpensesDropdownForExcludedGLCodes(Quote, Me.Page)
        End If

        If Helpers.NewCo_Prefill_Helper.ShouldReturnToQuoteForPreFillOrNewCo(Quote, Me.DefaultValidationType, Me.QuoteIdOrPolicyIdPipeImageNumber) Then
            ParentVrControl.Populate()
            ValidationHelper.AddError(NewCoAppRedirectToQuoteMessage, True, NewCoAppRedirectToQuoteTitle)
        End If



        If Me.ValidationSummmary.HasErrors() = False Then
            ' do final rate
            Dim saveErr As String = Nothing
            Dim loadErr As String = Nothing

            Dim q = Me.Quote
            'Dim ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            'updated 2/18/2019
            Dim ratedQuote As QuickQuoteObject = Nothing
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
                'no rate
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Dim successfulEndorsementRate As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedEndorsementQuoteFromContext(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, qqEndorsementResults:=ratedQuote, errorMessage:=saveErr, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
            Else
                ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(Me.QuoteId, saveErr, loadErr, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            End If

            ' Check for quote stop or kill - DM 8/30/2017
            If Me.Quote IsNot Nothing AndAlso (Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled OrElse Me.Quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled) Then
                IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Session)
            End If
            If ratedQuote IsNot Nothing Then
                DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(False, ratedQuote) 'sets the rated quote cache
            Else
                ' you can't set a Nothing quote with this method you'll just have to let it find out for itself that the last rated quote was nothing - should never happen
                DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(True)
            End If
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                VR.Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                VR.Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
            Else
                Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            End If

            If String.IsNullOrWhiteSpace(saveErr) = False Or String.IsNullOrWhiteSpace(loadErr) = False Then
                'failed
                If String.IsNullOrWhiteSpace(saveErr) = False Then
                    Me.ValidationHelper.AddError(saveErr)
                End If
                If String.IsNullOrWhiteSpace(loadErr) = False Then
                    Me.ValidationHelper.AddError(loadErr)
                End If

            Else
                ' did not fail to call service but may have validation Items
                If ratedQuote IsNot Nothing Then
                    WebHelper_Personal.GatherRatingErrorsAndWarnings(ratedQuote, Me.ValidationHelper)



                    RaiseEvent ApplicationRated() ' always fire so tree gets even attempt rates 4-14-14

                    'If ratedQuote.Success Then
                    '    RaiseEvent ApplicationRatedSuccessfully()
                    '    ' if successful then got quote summary
                    '    'Me.ctlQsummary_PPA.Populate()
                    '    Me.txtEffectiveDate.Text = "" ' clear on rate
                    '    Me.txtEffectiveDate_Copy.Text = "" ' clear on rate
                    'Else
                    '    'else so 'route to IW' button
                    '    Me.ctl_RouteToUw.Visible = True
                    'End If

                    If ratedQuote.Success Then
                        If ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped OrElse ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped Then
                            'stay where you are - don't show summary - stop message will be contained in validation messages
                        Else
                            RaiseEvent ApplicationRatedSuccessfully()
                            Me.txtEffectiveDate.Text = "" ' clear on rate
                            Me.txtEffectiveDate_Copy.Text = "" ' clear on rate
                        End If
                    Else
                        Me.ctl_RouteToUw.Visible = True
                    End If

                End If
            End If

        End If
    End Sub

    Protected Sub btnsave_Click(sender As Object, e As EventArgs) Handles btnsave.Click
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(If(Me.IsOnAppPage, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate, VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate))))
    End Sub

    'added 9/17/2015 for effDate validation; originally taken from treeview
    Public Sub SetMinimumAndMaximumEffectiveDates()
        Dim minEffDateAllQuotes As String = DateAdd(DateInterval.Day, MinimumEffectiveDateDaysFromToday, Date.Today).ToShortDateString
        Dim maxEffDateAllQuotes As String = DateAdd(DateInterval.Day, MaximumEffectiveDateDaysFromToday, Date.Today).ToShortDateString

        'added 5/17/2023
        If Quote IsNot Nothing AndAlso Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
            Dim beforeDtMsg As String = ""
            Dim afterDtMsg As String = ""
            Helpers.WebHelper_Personal.Check_NewBusiness_Min_and_Max_Dates(minEffDateAllQuotes, maxEffDateAllQuotes, lob:=Quote.LobType, beforeMinDateMsg:=beforeDtMsg, afterMaxDateMsg:=afterDtMsg)
            BeforeDateMsg = beforeDtMsg
            AfterDateMsg = afterDtMsg
        End If

        Dim minEffDate As String = minEffDateAllQuotes
        Dim maxEffDate As String = maxEffDateAllQuotes

        Dim _QuoteHasMinimumEffectiveDate As Boolean = False
        Dim _MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes As Boolean = False


        If String.IsNullOrWhiteSpace(Quote.EffectiveDate) = False AndAlso IsDate(Quote.EffectiveDate) Then
            Dim uncrossableDateLineDict As New Dictionary(Of QuickQuoteHelperClass.LOBIFMVersions, String)
            If QQHelper.HasIFMLOBVersionUncrossableDateLineWithinRangeOfToday(Quote, uncrossableDateLineDict) Then
                For Each uncrossableKeyValuePair As KeyValuePair(Of QuickQuoteHelperClass.LOBIFMVersions, String) In uncrossableDateLineDict
                    If IsDate(uncrossableKeyValuePair.Value) Then
                        Dim uncrossableDate As Date = CDate(uncrossableKeyValuePair.Value).Date
                        If CDate(Quote.EffectiveDate).Date >= uncrossableDate Then
                            _QuoteHasMinimumEffectiveDate = True
                            minEffDate = uncrossableDate
                            _MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes = True
                        Else
                            maxEffDate = uncrossableDate.AddDays(-1)  ' Updated MGB 5/14/19
                        End If
                    End If
                Next
            End If
        End If
        Dim lobHelper As New IFM.VR.Common.Helpers.LOBHelper(Me.Quote.LobType)
        If lobHelper.IsMultistateCapableLob Then
            If lobHelper.IsMultistateCapableLob Then
                If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Me.Quote.EffectiveDate) Then
                    If minEffDate < IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate Then
                        minEffDate = IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate
                    End If
                Else
                    If maxEffDate > IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate Then
                        maxEffDate = IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate.AddDays(-1)
                    End If
                End If
            End If
        End If



        QuoteHasMinimumEffectiveDate = _QuoteHasMinimumEffectiveDate
        MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes = _MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes
        MinimumEffectiveDateAllQuotes = minEffDateAllQuotes
        MaximumEffectiveDateAllQuotes = maxEffDateAllQuotes

        _flags.With(Of LOB.PPA) _
              .When(Function(ppa) ppa.OhioEnabled AndAlso
                                  Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso
                                  Me.Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio) _
              .Do(Sub()
                      Dim gsLobMinEffDate = LOBHelper.GetEarliesttEffectiveDateForLobAndGoverningState(Me.Quote.LobType, Me.Quote.QuickQuoteState)
                      MinimumEffectiveDate = {CDate(minEffDate), gsLobMinEffDate}.Max()
                  End Sub) _
              .Else(Sub()
                        MinimumEffectiveDate = minEffDate
                    End Sub)

        'Added 3/11/2022 for KY WCP Task 73261 MLW
        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation AndAlso Me.Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Kentucky Then
            Dim earliestKYWCPGovStateEffectiveDate As DateTime = IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPGovStateEffectiveDate()
            MinimumEffectiveDate = {CDate(minEffDate), earliestKYWCPGovStateEffectiveDate}.Max()
        Else
            MinimumEffectiveDate = minEffDate
        End If

        MaximumEffectiveDate = maxEffDate
    End Sub
End Class