Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.VR.Common.Helpers.PPA
Imports IFM.PrimativeExtensions

Public Class ctlQsummary_PPA
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Public Event RequestedApplicationSubmit()
    Public Event QuoteRateRequested()

    Const umpdDeductible As String = "250"

    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject Implements IVRUI_P.Quote
        Get
            Return DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache() '6-3-14 - Matt
        End Get
    End Property

    Protected ReadOnly Property QuoteId As String Implements IVRUI_P.QuoteId
        Get
            If Request.QueryString("quoteid") IsNot Nothing Then
                Return Request.QueryString("quoteid")
            End If
            If Page.RouteData.Values("quoteid") IsNot Nothing Then
                Return Page.RouteData.Values("quoteid").ToString()
            End If
            Return ""
        End Get
    End Property

    Public Property ValidationHelper As ControlValidationHelper Implements IVRUI_P.ValidationHelper
        Get
            If ViewState("vs_valHelp") Is Nothing Then
                ViewState("vs_valHelp") = New ControlValidationHelper
            End If
            Return DirectCast(ViewState("vs_valHelp"), ControlValidationHelper)
        End Get
        Set(value As ControlValidationHelper)
            ViewState("vs_valHelp") = value
        End Set
    End Property

    Public ReadOnly Property IsAppPageMode As Boolean
        Get
            If TypeOf Me.Page Is VR3AutoApp Then
                Return True
            End If
            Return False
        End Get
    End Property

    'added 5/8/2019
    Private _EndorsementPolicyId As Integer = 0
    Public ReadOnly Property EndorsementPolicyId As Integer
        Get
            If _EndorsementPolicyId <= 0 Then
                LoadEndorsementPolicyIdAndImageNum()
            End If
            Return _EndorsementPolicyId
        End Get
    End Property
    Private _EndorsementPolicyImageNum As Integer = 0
    Public ReadOnly Property EndorsementPolicyImageNum As Integer
        Get
            If _EndorsementPolicyImageNum <= 0 Then
                LoadEndorsementPolicyIdAndImageNum()
            End If
            Return _EndorsementPolicyImageNum
        End Get
    End Property
    Public ReadOnly Property EndorsementPolicyIdAndImageNum As String
        Get
            Dim strEndorsementPolicyIdAndImageNum As String = ""
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("EndorsementPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("EndorsementPolicyIdAndImageNum").ToString) = False Then
                strEndorsementPolicyIdAndImageNum = Request.QueryString("EndorsementPolicyIdAndImageNum").ToString
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("EndorsementPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("EndorsementPolicyIdAndImageNum").ToString) = False Then
                strEndorsementPolicyIdAndImageNum = Page.RouteData.Values("EndorsementPolicyIdAndImageNum").ToString
            End If
            Return strEndorsementPolicyIdAndImageNum
        End Get
    End Property
    Private Sub LoadEndorsementPolicyIdAndImageNum(Optional ByVal reset As Boolean = False)
        If reset = True Then
            _EndorsementPolicyId = 0
            _EndorsementPolicyImageNum = 0
        End If
        Dim strEndorsementPolicyIdAndImageNum As String = EndorsementPolicyIdAndImageNum
        If String.IsNullOrWhiteSpace(strEndorsementPolicyIdAndImageNum) = False Then
            Dim intList As List(Of Integer) = QuickQuoteHelperClass.ListOfIntegerFromString(strEndorsementPolicyIdAndImageNum, delimiter:="|", positiveOnly:=True)
            If intList IsNot Nothing AndAlso intList.Count > 0 Then
                _EndorsementPolicyId = intList(0)
                If intList.Count > 1 Then
                    _EndorsementPolicyImageNum = intList(1)
                End If
            End If
        End If
    End Sub
    Private _ReadOnlyPolicyId As Integer = 0
    Public ReadOnly Property ReadOnlyPolicyId As Integer
        Get
            If _ReadOnlyPolicyId <= 0 Then
                LoadReadOnlyPolicyIdAndImageNum()
            End If
            Return _ReadOnlyPolicyId
        End Get
    End Property
    Private _ReadOnlyPolicyImageNum As Integer = 0
    Public ReadOnly Property ReadOnlyPolicyImageNum As Integer
        Get
            If _ReadOnlyPolicyImageNum <= 0 Then
                LoadReadOnlyPolicyIdAndImageNum()
            End If
            Return _ReadOnlyPolicyImageNum
        End Get
    End Property
    Public ReadOnly Property ReadOnlyPolicyIdAndImageNum As String
        Get
            Dim strReadOnlyPolicyIdAndImageNum As String = ""
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("ReadOnlyPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString) = False Then
                strReadOnlyPolicyIdAndImageNum = Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum").ToString) = False Then
                strReadOnlyPolicyIdAndImageNum = Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum").ToString
            End If
            Return strReadOnlyPolicyIdAndImageNum
        End Get
    End Property
    Private Sub LoadReadOnlyPolicyIdAndImageNum(Optional ByVal reset As Boolean = False)
        If reset = True Then
            _ReadOnlyPolicyId = 0
            _ReadOnlyPolicyImageNum = 0
        End If
        Dim strReadOnlyPolicyIdAndImageNum As String = ReadOnlyPolicyIdAndImageNum
        If String.IsNullOrWhiteSpace(strReadOnlyPolicyIdAndImageNum) = False Then
            Dim intList As List(Of Integer) = QuickQuoteHelperClass.ListOfIntegerFromString(strReadOnlyPolicyIdAndImageNum, delimiter:="|", positiveOnly:=True)
            If intList IsNot Nothing AndAlso intList.Count > 0 Then
                _ReadOnlyPolicyId = intList(0)
                If intList.Count > 1 Then
                    _ReadOnlyPolicyImageNum = intList(1)
                End If
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        _script.AddScriptLine("$(""#" + Me.lnkPrint.ClientID + """).bind(""click"", function (e) { e.stopPropagation();return true; });")

        If Not IsPostBack Then
            Me.ValidationHelper.GroupName = String.Format("Quote Summary")
            If IsAppPageMode Then
                Session("SumType") = "App"
                ctlPayPlanOptions.Visible = False
            Else
                Session("SumType") = ""
                ctlPayPlanOptions.Visible = True
            End If
        End If
    End Sub

    Protected Sub lnkPrint_Click(sender As Object, e As EventArgs) Handles lnkPrint.Click
        'Response.Redirect(String.Format("~/Reports/PPA/PFQuoteSummary.aspx?quoteid={0}&summarytype={1}", Request.QueryString("QuoteId").ToString, Session("SumType")))
        'updated 5/8/2019
        Dim quoteOrPolicyInfo As String = ""
        If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
            quoteOrPolicyInfo = "ReadOnlyPolicyIdAndImageNum=" & Me.ReadOnlyPolicyId.ToString & "|" & Me.ReadOnlyPolicyImageNum.ToString
        ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
            quoteOrPolicyInfo = "EndorsementPolicyIdAndImageNum=" & Me.EndorsementPolicyId.ToString & "|" & Me.EndorsementPolicyImageNum.ToString
        ElseIf String.IsNullOrWhiteSpace(Me.QuoteId) = False Then
            quoteOrPolicyInfo = "quoteid=" & Me.QuoteId
        End If
        If String.IsNullOrWhiteSpace(quoteOrPolicyInfo) = False Then
            Dim sumType As String = ""
            If Session("SumType") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Session("SumType").ToString) = False AndAlso UCase(Session("SumType").ToString) = "APP" Then
                sumType = "App"
            End If
            Response.Redirect(String.Format("~/Reports/PPA/PFQuoteSummary.aspx?{0}&summarytype={1}", quoteOrPolicyInfo, sumType))
        End If
    End Sub

    Public Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate

        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        _script.AddScriptLine("ifm.vr.ui.UnLockTree(); ", True) 'never in edit mode on this pane

        If Quote IsNot Nothing Then
#If DEBUG Then
            Dim hgjghdj = Quote.PaymentOptions
            Dim test = VR.Common.Helpers.HOM.HOMCreditFactors.GetPolicyDiscountsAsListOfPercents(Quote, False)
#End If
            Dim qqHelper As New QuickQuoteHelperClass 'moved here from below 9/27/2021

            Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
            If Me.Quote.Vehicles IsNot Nothing Then
                Try
                    Dim errorMsg As String = ""
                    Dim rateType As QuickQuoteXML.QuickQuoteSaveType = Nothing

                    vehicle = (From v In Me.Quote.Vehicles Where (String.IsNullOrWhiteSpace(v.BodilyInjuryLiabilityLimitId) = False Or String.IsNullOrWhiteSpace(v.Liability_UM_UIM_LimitId) = False) And Not v.ComprehensiveCoverageOnly And v.BodyTypeId <> ENUMHelper.VehicleBodyType.bodyType_RecTrailer And v.BodyTypeId <> ENUMHelper.VehicleBodyType.bodyType_OtherTrailer Select v).FirstOrDefault()

                    If vehicle Is Nothing Then
                        vehicle = (From v In Me.Quote.Vehicles Where (String.IsNullOrWhiteSpace(v.BodilyInjuryLiabilityLimitId) = False Or String.IsNullOrWhiteSpace(v.Liability_UM_UIM_LimitId) = False) Select v).FirstOrDefault()
                    End If
                Catch ex As Exception
                End Try
                If vehicle IsNot Nothing Then
                    With Me.Quote
                        'Dim qqHelper As New QuickQuoteHelperClass 'moved above 9/27/2021

                        'Me.lblHeader.Text = String.Format("Quote Summary - Effective Date: {0} - {1}", .EffectiveDate, qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId))
                        'If IsAppPageMode Then
                        '    Me.lblHeader.Text = String.Format("{2} - Effective Date: {0} - {1}", .EffectiveDate, qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId), "Application Summary")
                        'Else
                        '    Me.lblHeader.Text = String.Format("{2} - Effective Date: {0} - {1}", .EffectiveDate, qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId), "Quote Summary")
                        'End If
                        'updated 5/8/2019
                        Select Case .QuoteTransactionType
                            Case QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                                Me.lblHeader.Text = If(.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, "Change", "Image") & " Summary - Updated " & qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, .PolicyTermId) & " Premium"
                                Me.ImageDateAndPremChangeLine.Visible = True
                                If qqHelper.IsDateString(.TransactionEffectiveDate) = True Then
                                    Me.lblTranEffDate.Text = "Effective Date: " & .TransactionEffectiveDate 'note: should already be in shortdate format
                                Else
                                    Me.lblTranEffDate.Text = ""
                                End If
                                If qqHelper.IsNumericString(.ChangeInFullTermPremium) = True Then 'note: was originally looking for positive decimal, but the change in prem could be zero or negative
                                    Me.lblAnnualPremChg.Text = "Annual Premium Change: " & .ChangeInFullTermPremium 'note: should already be in money format
                                Else
                                    Me.lblAnnualPremChg.Text = ""
                                End If
                            Case Else
                                Me.lblHeader.Text = String.Format("{2} - Effective Date: {0} - {1} Policy", .EffectiveDate, qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, .PolicyTermId), If(IsAppPageMode, "Application", "Quote") & " Summary")
                                Me.ImageDateAndPremChangeLine.Visible = False
                        End Select

                        Me.lblQuoteSummary.Text = .TotalQuotedPremium

                        Me.lblEnhancementPrem.Text = .BusinessMasterEnhancementQuotedPremium

                        If IsValidEffectiveDateForAutoPlusEnhancement(Quote.EffectiveDate) = True Then
                            Me.trAutoPlusEnhance.Visible = True
                            Me.lblAutoPlusEnhancePrem.Text = .AutoPlusEnhancement_QuotedPremium
                        Else
                            Me.trAutoPlusEnhance.Visible = False
                        End If

                        Dim policyType As String = Nothing

                        If (vehicle.BodilyInjuryLiabilityQuotedPremium = "$0.00" Or vehicle.BodilyInjuryLiabilityQuotedPremium = "") And (vehicle.Liability_UM_UIM_QuotedPremium = "$0.00" Or vehicle.Liability_UM_UIM_QuotedPremium = "") Then
                            For Each item As QuickQuote.CommonObjects.QuickQuoteVehicle In Quote.Vehicles
                                'If item.BodilyInjuryLiabilityQuotedPremium <> "$0.00" And item.BodilyInjuryLiabilityQuotedPremium <> "" Then
                                If item.UninsuredCombinedSingleQuotedPremium <> "$0.00" And item.UninsuredCombinedSingleQuotedPremium <> "" Then
                                    'policyType = "Split"
                                    policyType = "SLL"
                                    Exit For
                                Else
                                    'policyType = "SLL"
                                    policyType = "Split"
                                End If
                            Next
                        End If

                        If (.VehiclesTotal_BodilyInjuryLiabilityQuotedPremium <> "$0.00" And .VehiclesTotal_BodilyInjuryLiabilityQuotedPremium <> "") Or policyType = "Split" Then
                            Me.tblSplitLimit.Visible = True
                            Me.tblSLL.Visible = False
                            'split limit
                            Me.lbl_SL_BodiliyInjury.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodilyInjuryLiabilityLimitId, vehicle.BodilyInjuryLiabilityLimitId)
                            Me.lbl_SL_Prem_BodilyInjury.Text = .VehiclesTotal_BodilyInjuryLiabilityQuotedPremium

                            Me.lbl_SL_PropertyDamage.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.PropertyDamageLimitId, vehicle.PropertyDamageLimitId)
                            Me.lbl_SL_Prem_PropertyDamage.Text = .VehiclesTotal_PropertyDamageQuotedPremium
                            'Updated 10/9/18 for multi state MLW
                            Select Case (Quote.QuickQuoteState)
                                Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                                    Me.pnlUMPD.Visible = False
                                Case QuickQuoteHelperClass.QuickQuoteState.Ohio
                                    Me.lbl_SL_UMPD.Text = qqHelper.GetStaticDataTextForValueAndState(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, vehicle.UninsuredMotoristPropertyDamageLimitId)
                                    Me.lbl_SL_Prem_UMPD.Text = .VehiclesTotal_UninsuredMotoristPropertyDamageQuotedPremium
                                Case Else
                                    Me.lbl_SL_UMPD.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, vehicle.UninsuredMotoristPropertyDamageLimitId)
                                    Me.lbl_SL_Prem_UMPD.Text = .VehiclesTotal_UninsuredMotoristPropertyDamageQuotedPremium
                            End Select

                            'Updated 10/9/18 for multi state MLW
                            Select Case (Quote.QuickQuoteState)
                                Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio
                                    Me.lbl_SL_UMUIM_BI.Text = qqHelper.GetStaticDataTextForValueAndState(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredBodilyInjuryLimitId,
                                                                                                     Quote.QuickQuoteState, vehicle.UninsuredBodilyInjuryLimitId)
                                    Me.lbl_SL_Prem_UMUIM_BI.Text = .VehiclesTotal_UM_UIM_BodilyInjuryLiabilityQuotedPremium
                                Case Else
                                    Me.lbl_SL_UMUIM_BI.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredMotoristLiabilityLimitId,
                                                                                             vehicle.UninsuredMotoristLiabilityLimitId) 'vehicle.UninsuredMotoristLiabilityLimitId
                                    Me.lbl_SL_Prem_UMUIM_BI.Text = .VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium
                            End Select

                        Else
                            'single limit liability
                            Me.tblSplitLimit.Visible = False
                            Me.tblSLL.Visible = True

                            Me.lbl_SLL_SLLCov.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.Liability_UM_UIM_LimitId, vehicle.Liability_UM_UIM_LimitId, Quote.LobType) 'vehicle.UninsuredCombinedSingleLimitId
                            Me.lbl_SLL_SLLCov_Prem.Text = .VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium
                            'Updated 10/9/18 for multi state MLW
                            Select Case (Quote.QuickQuoteState)
                                Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio
                                    Me.lbl_SLL_UMUIM_CSL.Text = qqHelper.GetStaticDataTextForValueAndState(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredCombinedSingleLimitId,
                                                                                                       Quote.QuickQuoteState, vehicle.UninsuredCombinedSingleLimitId)
                                    Me.lbl_SLL_UMUIM_CSL_PREM.Text = .VehiclesTotal_UM_UIM_CombinedSingleLimitQuotedPremium
                                Case Else
                                    Me.lbl_SLL_UMUIM_CSL.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredCombinedSingleLimitId,
                                                                                                                                   vehicle.UninsuredCombinedSingleLimitId) 'vehicle.Liability_UM_UIM_LimitId
                                    Me.lbl_SLL_UMUIM_CSL_PREM.Text = .VehiclesTotal_UninsuredCombinedSingleQuotedPremium
                            End Select
                        End If

                        Me.lbl_Med_Pay.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.MedicalPaymentsLimitId, vehicle.MedicalPaymentsLimitId)
                        Me.lbl_Med_Pay_Prem.Text = .VehiclesTotal_MedicalPaymentsQuotedPremium

                        'Updated 10/9/18 for multi state MLW
                        Select Case (Quote.QuickQuoteState)
                            Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                                Me.pnlUMPDDed.Visible = False
                            Case QuickQuoteHelperClass.QuickQuoteState.Ohio
                                If vehicle.UninsuredMotoristPropertyDamageLimitId <> "0" AndAlso IsNullEmptyorWhitespace(vehicle.UninsuredMotoristPropertyDamageLimitId) = False Then
                                    Me.lbl_UMPD_Deduc.Text = umpdDeductible 'qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredMotoristPropertyDamageDeductibleLimitId, vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId) 'vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId
                                    Me.lbl_UMPD_Deduc_Prem.Text = ""
                                Else
                                     Me.lbl_UMPD_Deduc.Text = ""
                                    Me.lbl_UMPD_Deduc_Prem.Text = ""
                                End If
                            Case Else
                                Me.lbl_UMPD_Deduc.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredMotoristPropertyDamageDeductibleLimitId, vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId) 'vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId
                                Me.lbl_UMPD_Deduc_Prem.Text = .VehiclesTotal_UninsuredMotoristPropertyDamageDeductibleQuotedPremium
                        End Select
                    End With
                End If

            End If

            Me.ctlQsummary_PPA_Vehicle_List.Populate()
            'Updated 8/22/2019 for Auto Endorsements Task 39544 MLW - do not show pay plan options when RCC pay plan type
            'If Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso Me.Quote.CurrentPayplanId = "18" Then
            'updated 9/27/2021
            Dim okayToShowPayPlans As Boolean = True
            If IsAppPageMode Then
                okayToShowPayPlans = False
            Else
                If Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    Dim currentPayPlanTxt As String = ""
                    If qqHelper.IsPositiveIntegerString(Me.Quote.CurrentPayplanId) = True Then
                        currentPayPlanTxt = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.CurrentPayplanId, Me.Quote.CurrentPayplanId)
                    End If
                    If String.IsNullOrWhiteSpace(currentPayPlanTxt) = False AndAlso UCase(currentPayPlanTxt).Contains("CREDIT CARD") = True AndAlso RccOptionHelper.IsRccOptionAvailable(Quote) = False Then
                        okayToShowPayPlans = False
                    End If
                End If
            End If
            If okayToShowPayPlans = False Then
                Me.ctlPayPlanOptions.Visible = False
            Else
                Me.ctlPayPlanOptions.Visible = True
                Me.ctlPayPlanOptions.Populate()
            End If
            'Me.ctlPayPlanOptions.Populate()
            Me.ctlQuoteSummaryActions.Populate()

            If StatePoliceFundHelper.IsStatePoliceFundLabelAvailable(Quote) Then
                lblFeeText.Style.Remove("display")
                lblFeeText.Text = "The Total Premium shown above does not include the State Police Law Enforcement Administration Fund."
            Else
                lblFeeText.Style.Add("display", "none")
            End If
        End If
    End Sub

    Private Sub HandleRateRequest() Handles ctlPayPlanOptions.QuoteRateRequested
        RaiseEvent QuoteRateRequested()
    End Sub

    Public Function Save() As Boolean Implements IVRUI_P.Save
        Return True
    End Function

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm
        Me.ValidationHelper.Clear()
        Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
        valSum.RegisterValidationHelper(Me.ValidationHelper)
    End Sub

    'added 2/18/2020
    Public Sub CheckForReRateAfterEffDateChange(Optional ByVal qqTranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.None, Optional ByVal newEffectiveDate As String = "", Optional ByVal oldEffectiveDate As String = "")
        Me.ctlQuoteSummaryActions.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
    End Sub

End Class