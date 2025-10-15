Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports IFM.PrimativeExtensions

Public Class VR3QuoteSummaryVehicle
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Dim qqHelper As New QuickQuoteHelperClass 'added 5/29/2013 to check for zero premiums
    Dim QuickQuote As QuickQuoteObject

    Const umpdDeductible As String = "250"

    Dim _quote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject Implements IVRUI_P.Quote
        Get
            If _quote Is Nothing Then
                Dim errCreateQSO As String = ""

                If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 7/17/2019; original logic in ELSE
                    _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyQuickQuoteObjectForPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=If(VR.Common.Workflow.Workflow.isAppOrQuote(Me.ReadOnlyPolicyIdAndImageNum) = Common.Workflow.Workflow.WorkflowAppOrQuote.App, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), expectedLobType:=QuickQuoteObject.QuickQuoteLobType.AutoPersonal, ratedView:=True, errorMessage:=errCreateQSO)
                ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                    _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteForPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=If(VR.Common.Workflow.Workflow.isAppOrQuote(Me.EndorsementPolicyIdAndImageNum) = Common.Workflow.Workflow.WorkflowAppOrQuote.App, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), expectedLobType:=QuickQuoteObject.QuickQuoteLobType.AutoPersonal, ratedView:=True, errorMessage:=errCreateQSO)
                Else
                    If VR.Common.Workflow.Workflow.isAppOrQuote(Me.QuoteId) = Common.Workflow.Workflow.WorkflowAppOrQuote.App Then
                        _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.AutoPersonal, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                    Else
                        _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                    End If
                End If
            End If
            Return _quote
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

    'Public ReadOnly Property IsAppPageMode As Boolean
    '    Get
    '        If TypeOf Me.Page Is VR3AutoApp Then
    '            Return True
    '        End If
    '        Return False
    '    End Get
    'End Property

    Public Property VehicleNumber As Int32
        Get
            If ViewState("vs_vehicleNum") Is Nothing Then
                ViewState("vs_vehicleNum") = -1
            End If
            Return CInt(ViewState("vs_vehicleNum"))
        End Get
        Set(ByVal value As Int32)
            ViewState("vs_vehicleNum") = value
        End Set
    End Property

    'Vehicle Info
    Public Property Vehicle As String
        Get
            Return Me.lblVehicle.Text
        End Get
        Set(value As String)
            Me.lblVehicle.Text = value
        End Set
    End Property

    Public Property VehYear As String
        Get
            Return Me.lblYear.Text
        End Get
        Set(value As String)
            Me.lblYear.Text = value
        End Set
    End Property

    Public Property VehMake As String
        Get
            Return Me.lblMake.Text
        End Get
        Set(value As String)
            Me.lblMake.Text = value
        End Set
    End Property

    Public Property VehModel As String
        Get
            Return Me.lblModel.Text
        End Get
        Set(value As String)
            Me.lblModel.Text = value
        End Set
    End Property

    Public Property VehVIN As String
        Get
            Return Me.lblVIN.Text
        End Get
        Set(value As String)
            Me.lblVIN.Text = value
        End Set
    End Property

    Public Property VehTerritory As String
        Get
            Return Me.lblTerritory.Text
        End Get
        Set(value As String)
            Me.lblTerritory.Text = value
        End Set
    End Property

    Public Property VehSymbol As String
        Get
            Return Me.lblSymbol.Text
        End Get
        Set(value As String)
            Me.lblSymbol.Text = value
        End Set
    End Property

    Public Property VehClass As String
        Get
            Return Me.lblClass.Text
        End Get
        Set(value As String)
            Me.lblClass.Text = value
        End Set
    End Property

    Public Property VehType As String
        Get
            Return Me.lblType.Text
        End Get
        Set(value As String)
            Me.lblType.Text = value
        End Set
    End Property

    'added 7/17/2019
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

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then

        End If
    End Sub

    Protected Sub ValidateForm() Implements IVRUI_P.ValidateForm

    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate
        Dim trimModel As StringBuilder = New StringBuilder

        If Me.Quote IsNot Nothing Then
            Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
            If Me.Quote.Vehicles IsNot Nothing Then
                Try
                    vehicle = Me.Quote.Vehicles(Me.VehicleNumber)
                Catch ex As Exception
                End Try
                If vehicle IsNot Nothing Then
                    'Only displays row headings when the first column is being displayed
                    If (Me.VehicleNumber + 1) Mod 3 = 1 Then
                        VehicleHdrRow.Visible = True
                    Else
                        VehicleHdrRow.Visible = False
                    End If


                    Me.lblVehicle.Text = VehicleNumber + 1
                    Me.lblYear.Text = vehicle.Year
                    Me.lblMake.Text = vehicle.Make

                    If vehicle.Model.Length > 20 Then
                        trimModel.Append(vehicle.Model.Substring(0, 20))
                        trimModel.Append("...")
                        Me.lblModel.Text = trimModel.ToString
                    Else
                        Me.lblModel.Text = vehicle.Model
                    End If

                    Me.lblVIN.Text = vehicle.Vin
                    Me.lblTerritory.Text = vehicle.TerritoryNum

                    ''Assignes a 0 if the symbol is blank
                    'If vehicle.VehicleSymbols IsNot Nothing Then
                    '    Dim compSym As String = "0"
                    '    Dim collSym As String = "0"
                    '    Dim liabSym As String = "0"
                    '    Dim compSymbol = (From s In vehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "1" Select s).FirstOrDefault()
                    '    Dim colliSymbol = (From s In vehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "2" Select s).FirstOrDefault()
                    '    Dim liabSymbol = (From s In vehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "3" Select s).FirstOrDefault()

                    '    compSym = If(compSymbol IsNot Nothing AndAlso String.IsNullOrWhiteSpace(compSymbol.UserOverrideSymbol) = False, compSymbol.UserOverrideSymbol, "0")
                    '    collSym = If(colliSymbol IsNot Nothing AndAlso String.IsNullOrWhiteSpace(colliSymbol.UserOverrideSymbol) = False, colliSymbol.UserOverrideSymbol, "0")
                    '    liabSym = If(liabSymbol IsNot Nothing AndAlso String.IsNullOrWhiteSpace(liabSymbol.UserOverrideSymbol) = False, liabSymbol.UserOverrideSymbol, "0")

                    '    If liabSymbol IsNot Nothing AndAlso String.IsNullOrWhiteSpace(liabSymbol.UserOverrideSymbol) = False Then
                    '        Me.lblSymbol.Text = "Comp " & compSym & " Coll " & collSym & " Liab " + liabSym
                    '    Else
                    '        Me.lblSymbol.Text = "Comp " & compSym & " Coll " & collSym
                    '    End If


                    'End If


                    If IFM.VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote) Then
                        divCVehclass1.Visible = False
                        divCVehclass2.Visible = False

                    Else
                        lblClass.Text = vehicle.ClassCode
                    End If

                    Me.lblType.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId)

                    'Coverages
                    Dim policyType As String = Nothing

                    '527 Fix for blank values - TLB
                    If (vehicle.BodilyInjuryLiabilityQuotedPremium = "$0.00" Or vehicle.BodilyInjuryLiabilityQuotedPremium = "") And (vehicle.Liability_UM_UIM_QuotedPremium = "$0.00" Or vehicle.Liability_UM_UIM_QuotedPremium = "") Then
                        For Each item As QuickQuote.CommonObjects.QuickQuoteVehicle In Quote.Vehicles
                            If item.BodilyInjuryLiabilityQuotedPremium <> "$0.00" And item.BodilyInjuryLiabilityQuotedPremium <> "" Then
                                policyType = "Split"
                                Exit For
                            Else
                                policyType = "SLL"
                            End If
                        Next
                    End If

                    If (vehicle.BodilyInjuryLiabilityQuotedPremium <> "$0.00" And vehicle.BodilyInjuryLiabilityQuotedPremium <> "") Or policyType = "Split" Then
                        Me.pnlSplitRow.Visible = True
                        Me.pnlSplit.Visible = True
                        Me.pnlSplitPrem.Visible = True
                        Me.lblBodily.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodilyInjuryLiabilityLimitId, vehicle.BodilyInjuryLiabilityLimitId)
                        Me.lblBodilyPrem.Text = vehicle.BodilyInjuryLiabilityQuotedPremium
                        Me.lblProperty.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDamageLimitId, vehicle.PropertyDamageLimitId)
                        Me.lblPropertyPrem.Text = vehicle.PropertyDamageQuotedPremium
                        'Added 10/10/18 for multi state MLW
                        Select Case (Quote.QuickQuoteState)
                            Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio
                                Me.lblUMBodily.Text = qqHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, Quote.QuickQuoteState, vehicle.UninsuredBodilyInjuryLimitId)
                                Dim umuimPrem As String = qqHelper.getSum(vehicle.UninsuredBodilyInjuryQuotedPremium, vehicle.UnderinsuredBodilyInjuryQuotedPremium)
                                Me.lblUMBodilyPrem.Text = qqHelper.QuotedPremiumFormat(umuimPrem)
                            Case Else
                                Me.lblUMBodily.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristLiabilityLimitId, vehicle.UninsuredMotoristLiabilityLimitId)
                                Me.lblUMBodilyPrem.Text = vehicle.UninsuredMotoristLiabilityQuotedPremium
                        End Select

                        'Updated 10/22/18 for multi state MLW
                        Select Case (Quote.QuickQuoteState)
                            Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                                Me.lblUMProperty.Text = qqHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, vehicle.UninsuredMotoristPropertyDamageLimitId)
                                If IFM.VR.Common.Helpers.PPA.CollisionAndUMPD.IsILCollisionAndUMPDAvailable(Quote) AndAlso (vehicle.UninsuredMotoristPropertyDamageLimitId <> "0" AndAlso IsNullEmptyorWhitespace(vehicle.UninsuredMotoristPropertyDamageLimitId) = False) Then
                                    Me.lblUMProperty.Text = Me.lblUMProperty.Text & "/" & umpdDeductible
                                End If
                            Case QuickQuoteHelperClass.QuickQuoteState.Ohio
                                Me.lblUMProperty.Text = qqHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, vehicle.UninsuredMotoristPropertyDamageLimitId)
                                If vehicle.UninsuredMotoristPropertyDamageLimitId <> "0" AndAlso IsNullEmptyorWhitespace(vehicle.UninsuredMotoristPropertyDamageLimitId) = False Then
                                    Me.lblUMProperty.Text = Me.lblUMProperty.Text & "/" & umpdDeductible
                                End If
                            Case Else
                                Me.lblUMProperty.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, vehicle.UninsuredMotoristPropertyDamageLimitId)
                        End Select
                        Me.lblUMPropertyPrem.Text = vehicle.UninsuredMotoristPropertyDamageQuotedPremium
                    Else
                        Me.pnlSLLRow.Visible = True
                        Me.pnlSSL.Visible = True
                        Me.pnlSSLPrem.Visible = True
                        Me.lblSSL.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, vehicle.Liability_UM_UIM_LimitId, Quote.LobType) 'vehicle.UninsuredCombinedSingleLimitId
                        Me.lblSSLPrem.Text = vehicle.Liability_UM_UIM_QuotedPremium
                        'Added 10/10/18 for multi state MLW
                        Select Case (Quote.QuickQuoteState)
                            Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                                Me.lblUMUIMCSL.Text = qqHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredCombinedSingleLimitId, Quote.QuickQuoteState, vehicle.UninsuredCombinedSingleLimitId) 'vehicle.Liability_UM_UIM_LimitId
                                Dim umuimPrem As String = qqHelper.getSum(vehicle.UninsuredCombinedSingleQuotedPremium, vehicle.UnderinsuredCombinedSingleLimitQuotedPremium)
                                Me.lblUMUIMCSLPrem.Text = qqHelper.QuotedPremiumFormat(umuimPrem)
                                'Added 10/30/18 for multi state MLW
                                Me.trUMPDRow.Visible = True
                                Me.trUMProperty.Visible = True
                                Me.trUMPropertyPrem.Visible = True
                                Me.lblUMPD.Text = qqHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, vehicle.UninsuredMotoristPropertyDamageLimitId)
                                If IFM.VR.Common.Helpers.PPA.CollisionAndUMPD.IsILCollisionAndUMPDAvailable(Quote) AndAlso (vehicle.UninsuredMotoristPropertyDamageLimitId <> "0" AndAlso IsNullEmptyorWhitespace(vehicle.UninsuredMotoristPropertyDamageLimitId) = False) Then
                                    Me.lblUMPD.Text = Me.lblUMPD.Text & "/" & umpdDeductible
                                End If
                                Me.lblUMPDPrem.Text = vehicle.UninsuredMotoristPropertyDamageQuotedPremium
                            Case Else
                                'Added 10/30/18 for multi state MLW
                                Me.trUMPDRow.Visible = False
                                Me.trUMPD.Visible = False
                                Me.trUMPDPrem.Visible = False
                                Me.lblUMUIMCSL.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredCombinedSingleLimitId, vehicle.UninsuredCombinedSingleLimitId) 'vehicle.Liability_UM_UIM_LimitId
                                Me.lblUMUIMCSLPrem.Text = vehicle.UninsuredCombinedSingleQuotedPremium
                        End Select
                    End If

                    Me.lblMedical.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, vehicle.MedicalPaymentsLimitId)
                    '527 Fix for blank values - TLB
                    If vehicle.MedicalPaymentsQuotedPremium = "" Then
                        Me.lblMedicalPrem.Text = "$0.00"
                    Else
                        Me.lblMedicalPrem.Text = vehicle.MedicalPaymentsQuotedPremium
                    End If

                    Me.lblComp.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.ComprehensiveDeductibleLimitId, vehicle.ComprehensiveDeductibleLimitId, Quote.LobType)
                    '527 Fix for blank values - TLB
                    If vehicle.ComprehensiveQuotedPremium = "" Then
                        Me.lblCompPrem.Text = "$0.00"
                    Else
                        Me.lblCompPrem.Text = vehicle.ComprehensiveQuotedPremium
                    End If

                    Me.lblColl.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.CollisionDeductibleLimitId, vehicle.CollisionDeductibleLimitId, Quote.LobType)
                    '527 Fix for blank values - TLB
                    If vehicle.CollisionQuotedPremium = "" Then
                        Me.lblCollPrem.Text = "$0.00"
                    Else
                        Me.lblCollPrem.Text = vehicle.CollisionQuotedPremium
                    End If

                    If IFM.VR.Common.Helpers.PPA.StatePoliceFundHelper.IsStatePoliceFundLabelAvailable(Quote) Then
                        lblPoliceTrainingFeeRow.Text = "State Police Law Enforcement Administration Fund"
                    End If

                    'Illinois - Police Training Fee - "Illinois State Police Training and Academy Fund" - added by Diamond at rate
                    Select Case (Quote.QuickQuoteState)
                        Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                            If IFM.VR.Common.Helpers.PPA.PolicyTrainingFeeHelper.IsPoliceTrainingFeeAvailable(Quote) AndAlso anyVehiclesHavePoliceTrainingFee(Quote.Vehicles) = True Then
                                trPoliceTrainingFeeRow.Visible = True
                                trPoliceTrainingFee.Visible = True
                                trPoliceTrainingFeePrem.Visible = True
                                Dim policeTrainingFeeCoverage As QuickQuoteCoverage = (From cov In vehicle.Coverages Where cov.CoverageCodeId = 100014 Select cov).FirstOrDefault()
                                If policeTrainingFeeCoverage IsNot Nothing AndAlso qqHelper.IsPositiveDecimalString(policeTrainingFeeCoverage.FullTermPremium) = True Then
                                    lblPoliceTrainingFeePrem.Text = FormatCurrency(policeTrainingFeeCoverage.FullTermPremium)
                                Else
                                    lblPoliceTrainingFeePrem.Text = "$0.00"
                                End If
                            Else
                                trPoliceTrainingFeeRow.Visible = False
                                trPoliceTrainingFee.Visible = False
                                trPoliceTrainingFeePrem.Visible = False
                            End If
                        Case Else
                            trPoliceTrainingFeeRow.Visible = False
                            trPoliceTrainingFee.Visible = False
                            trPoliceTrainingFeePrem.Visible = False
                    End Select

                    'Optional Coverages
                    Me.lblTowing.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TowingAndLaborDeductibleLimitId, vehicle.TowingAndLaborDeductibleLimitId)
                    '527 Fix for blank values - TLB
                    If vehicle.TowingAndLaborQuotedPremium = "" Then
                        Me.lblTowPrem.Text = "$0.00"
                    Else
                        Me.lblTowPrem.Text = vehicle.TowingAndLaborQuotedPremium
                    End If

                    If vehicle.ComprehensiveCoverageOnly Or
                        (qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.ComprehensiveDeductibleLimitId, vehicle.ComprehensiveDeductibleLimitId, Quote.LobType) = "N/A" And
                         qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.CollisionDeductibleLimitId, vehicle.CollisionDeductibleLimitId, Quote.LobType) = "N/A") Or
                        qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Motorcycle" Or
                        qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Rec. Trailer" Or
                        qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Other Trailer" Then
                        Me.lblRental.Text = "N/A"
                    Else
                        '527 Fix for blank values - TLB
                       If IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote) = False Then
                            If vehicle.TransportationExpenseLimitId = 0 Or vehicle.TransportationExpenseLimitId = "" Then
                                Me.lblRental.Text = "$20 day/max $600"
                            Else
                                Dim rentalSplt() As String = Split(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TransportationExpenseLimitId, vehicle.TransportationExpenseLimitId), "/")
                                lblRental.Text = "$" & rentalSplt(0) & " day/max $" & rentalSplt(1)
                            End If
                        Else
                            If vehicle.TransportationExpenseLimitId = "0" OrElse vehicle.TransportationExpenseLimitId = "" OrElse vehicle.TransportationExpenseLimitId = "446" Then
                                Me.lblRental.Text = "40/1200 30 DAYS"
                            Else
                                Dim rentalSplt() As String = Split(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TransportationExpenseLimitId, vehicle.TransportationExpenseLimitId), "/")
                                lblRental.Text = rentalSplt(0) & "/" & rentalSplt(1)
                            End If
                        End If
                    End If

                    '527 Fix for blank values - TLB
                    If vehicle.TransportationExpenseQuotedPremium = "" Then
                        Me.lblRentPrem.Text = "$0.00"
                    Else
                        Me.lblRentPrem.Text = vehicle.TransportationExpenseQuotedPremium
                    End If

                    '527 Fix for blank values - TLB
                    Dim tripPrem As Double = 0

                    If vehicle.TripInterruptionQuotedPremium <> "" Then
                        tripPrem = vehicle.TripInterruptionQuotedPremium
                    End If

                    Dim tripInterrupt As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TripInterruptionLimitId, vehicle.TripInterruptionLimitId)
                    Me.lblTripPrem.Text = tripPrem.ToString("$0.00")
                    Me.lblTrip.Text = If(tripInterrupt <> "N/A" And tripInterrupt <> "", "inc", "excluded")

                    If vehicle.NonOwnedNamed Or
                        vehicle.ComprehensiveCoverageOnly Or
                        (qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.ComprehensiveDeductibleLimitId, vehicle.ComprehensiveDeductibleLimitId, Quote.LobType) = "N/A" And
                         qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.CollisionDeductibleLimitId, vehicle.CollisionDeductibleLimitId, Quote.LobType) = "N/A") Or
                        qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Motorcycle" Or
                        qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Rec. Trailer" Or
                        qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Other Trailer" Then
                        Me.lblSound.Text = "N/A"
                        '527 Fix for blank values - TLB
                        If vehicle.SoundEquipmentQuotedPremium = "" Then
                            Me.lblSoundPrem.Text = "$0.00"
                        Else
                            Me.lblSoundPrem.Text = vehicle.SoundEquipmentQuotedPremium
                        End If

                        Me.lblAudio.Text = "N/A"
                        '527 Fix for blank values - TLB
                        If vehicle.ElectronicEquipmentQuotedPremium = "" Then
                            Me.lblAudioPrem.Text = "$0.00"
                        Else
                            Me.lblAudioPrem.Text = vehicle.ElectronicEquipmentQuotedPremium
                        End If
                    Else
                        '527 Fix for blank values - TLB
                        If vehicle.SoundEquipmentQuotedPremium = "" Then
                            Me.lblSoundPrem.Text = "$0.00"
                        Else
                            Me.lblSoundPrem.Text = vehicle.SoundEquipmentQuotedPremium
                        End If

                        '527 Fix for blank values - TLB
                        If vehicle.SoundEquipmentQuotedPremium <> "$0.00" And vehicle.SoundEquipmentQuotedPremium <> "" Then
                            Me.lblSound.Text = vehicle.SoundEquipmentLimit
                        Else
                            Me.lblSound.Text = "[1,000 INC]"
                        End If

                        '527 Fix for blank values - TLB
                        If vehicle.ElectronicEquipmentQuotedPremium = "" Then
                            lblAudioPrem.Text = "$0.00"
                        Else
                            lblAudioPrem.Text = vehicle.ElectronicEquipmentQuotedPremium
                        End If

                        If vehicle.ElectronicEquipmentLimit = "0" Or vehicle.SoundEquipmentLimit = "" Then
                            lblAudio.Text = "N/A"
                        Else
                            lblAudio.Text = vehicle.ElectronicEquipmentLimit
                        End If
                        'If lblAudio.Text = "0" Or lblAudio.Text = "" Then
                        '    Me.lblAudio.Text = "N/A"
                        '    '527 Fix for blank values - TLB
                        '    If vehicle.ElectronicEquipmentQuotedPremium = "" Then
                        '        Me.lblAudioPrem.Text = "$0.00"
                        '    Else
                        '        Me.lblAudioPrem.Text = vehicle.ElectronicEquipmentQuotedPremium
                        '    End If
                        'Else
                        '    Me.lblAudio.Text = vehicle.ElectronicEquipmentLimit
                        '    Me.lblAudioPrem.Text = vehicle.ElectronicEquipmentQuotedPremium
                        'End If
                    End If

                    '527 Fix for blank values - TLB
                    'If (vehicle.SoundEquipmentLimit = "0" Or vehicle.SoundEquipmentLimit = "") And (vehicle.ElectronicEquipmentLimit = "0" Or vehicle.ElectronicEquipmentLimit = "") Then
                    '    lblTapes.Text = "N/A"
                    'ElseIf ((vehicle.SoundEquipmentLimit <> "0" And vehicle.SoundEquipmentLimit <> "") Or (vehicle.ElectronicEquipmentLimit <> "0" And vehicle.ElectronicEquipmentLimit <> "")) And (vehicle.TapesAndRecordsLimitId = "0" Or vehicle.TapesAndRecordsLimitId = "") Then
                    If ((vehicle.SoundEquipmentLimit <> "0" And vehicle.SoundEquipmentLimit <> "") Or (vehicle.ElectronicEquipmentLimit <> "0" And vehicle.ElectronicEquipmentLimit <> "")) And (vehicle.TapesAndRecordsLimitId = "0" Or vehicle.TapesAndRecordsLimitId = "") Then
                        lblTapes.Text = "[200 INC]"
                    Else
                        lblTapes.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TapesAndRecordsLimitId, vehicle.TapesAndRecordsLimitId)
                    End If
                    'Me.lblTapes.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TapesAndRecordsLimitId, vehicle.TapesAndRecordsLimitId)
                    'If Me.lblTapes.Text = "" Then
                    '    Me.lblTapes.Text = "N/A"
                    'End If

                    If vehicle.TapesAndRecordsQuotedPremium = "" Then
                        Me.lblTapesPrem.Text = "$0.00"
                    Else
                        Me.lblTapesPrem.Text = vehicle.TapesAndRecordsQuotedPremium
                    End If

                    '527 Fix for blank values - TLB
                    Dim leasePrem As Double = 0

                    If vehicle.AutoLoanOrLeaseQuotedPremium <> "" Then
                        leasePrem = vehicle.AutoLoanOrLeaseQuotedPremium
                    End If

                    Me.lblLoanPrem.Text = leasePrem.ToString("$0.00")
                    Me.lblLoan.Text = If(vehicle.HasAutoLoanOrLease, "applied", "excluded")

                    '527 Fix for blank values - TLB
                    Dim pollutionPrem As Double = 0

                    If vehicle.PollutionLiabilityBroadenedCoverageQuotedPremium <> "" Then
                        pollutionPrem = vehicle.PollutionLiabilityBroadenedCoverageQuotedPremium
                    End If

                    Me.lblPollutionPrem.Text = pollutionPrem.ToString("$0.00")
                    Me.lblPollution.Text = If(vehicle.HasPollutionLiabilityBroadenedCoverage, "applied", "excluded")

                    If vehicle.ScheduledItems.Count > 0 And vehicle.ScheduledItems IsNot Nothing Then
                        Dim motorEquipInt As Integer = 0
                        Dim motorPremInt As Integer = 0
                        Dim diamondEquip As Integer = 0
                        Dim diamondTotal As Integer = 0

                        For Each schedItem As QuickQuote.CommonObjects.QuickQuoteScheduledItem In vehicle.ScheduledItems
                            '527 - Diamond is not returning a blank instead of a 0. This is needed to prevent the application from blowing up - TLB
                            If schedItem.Amount = "" Then
                                diamondEquip = 0
                            Else
                                diamondEquip = CInt(schedItem.Amount.Substring(0, schedItem.Amount.Length - 3).Trim("$"))
                            End If

                            If schedItem.PremiumFullTerm = "" Then
                                diamondTotal = 0
                            Else
                                diamondTotal = CInt(schedItem.PremiumFullTerm.Substring(0, schedItem.PremiumFullTerm.Length - 3).Trim("$"))
                            End If

                            motorEquipInt = motorEquipInt + diamondEquip
                            motorPremInt = motorPremInt + diamondTotal
                        Next

                        Me.lblCustom.Text = String.Format("{0:C2}", motorEquipInt)
                        Me.lblCustomPrem.Text = String.Format("{0:C2}", motorPremInt)
                    Else
                        Me.lblCustom.Text = "$0.00"
                        Me.lblCustomPrem.Text = "$0.00"
                    End If
                    If IFM.VR.Common.Helpers.PPA.CustomEquipmentHelper.IsCustomEquipmentAvailable(Quote) Then
                        Dim customEquipmentCoverage As QuickQuoteCoverage = (From cov In vehicle.Coverages Where cov.CoverageCodeId = 47 Select cov).FirstOrDefault()
                        If customEquipmentCoverage IsNot Nothing Then
                            Me.lblCustomEquipmentPrem.Text = FormatCurrency(customEquipmentCoverage.FullTermPremium)
                            Me.lblCustomEquipment.Text = FormatCurrency(customEquipmentCoverage.ManualLimitAmount)
                        Else
                            Me.lblCustomEquipment.Text = "$0.00"
                            Me.lblCustomEquipmentPrem.Text = "$0.00"
                        End If
                    Else
                        trCustomEquipmentRow.Visible = False
                        trCustomEquipmentLabel.Visible = False
                        trCustomEquipmentPrem.Visible = False
                    End If
                    

                    'Discounts
                    '527 Fix for blank values - TLB
                    'If vehicle.RestraintTypeId = "0" Or vehicle.RestraintTypeId = "" Then
                    '    lblRestraint.Text = "N/A"
                    'Else
                    '    lblRestraint.Text = "applied"
                    'End If

                    '527 Fix for blank values - TLB
                    'If vehicle.AntiTheftTypeId = "0" Or vehicle.AntiTheftTypeId = "" Then
                    '    lblTheft.Text = "N/A"
                    'ElseIf vehicle.AntiTheftTypeId = 1 Then
                    '    lblTheft.Text = If(IFM.VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote), "applied", "5%")
                    'Else
                    '    lblTheft.Text = If(IFM.VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote), "applied", "15%")
                    'End If

                    Me.lblVehicleTotal.Text = vehicle.PremiumFullTerm
                End If
            End If
        End If
    End Sub

    Private Function anyVehiclesHavePoliceTrainingFee(vehicles As List(Of QuickQuoteVehicle)) As Boolean
	    'Illinois only - "Illinois State Police Training and Academy Fund" - added by Diamond at rate
        Dim showPoliceTrainingFee As Boolean = False
        If Quote IsNot Nothing AndAlso Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
            For each v In Quote.Vehicles
                Dim policeTrainingFeeCoverage As QuickQuoteCoverage = (From cov In v.Coverages Where cov.CoverageCodeId = 100014 Select cov).FirstOrDefault()
                If policeTrainingFeeCoverage IsNot Nothing AndAlso policeTrainingFeeCoverage.FullTermPremium IsNot Nothing AndAlso qqHelper.IsPositiveDecimalString(policeTrainingFeeCoverage.FullTermPremium) = True Then
                    showPoliceTrainingFee = True
                    Exit For
                End If
            Next
        End If
        Return showPoliceTrainingFee
    End Function

    Protected Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Protected Function Save() As Boolean Implements IVRUI_P.Save
        Return False
    End Function
End Class