Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions

Public Class ctlQsummary_PPA_Vehicle
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

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

    Public Property VehicleNumber As Int32
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

    Public ReadOnly Property IsAppPageMode As Boolean
        Get
            If TypeOf Me.Page Is VR3AutoApp Then
                Return True
            End If
            Return False
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        '_script.AddScriptLine("$(""#" + Me.lnkPrint.ClientID + """).bind(""click"", function (e) { e.stopPropagation();return true; });")

    End Sub

    Public Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate
        If Me.Quote IsNot Nothing Then
            Dim quickQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing

            Me.LoadStaticData()
            Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
            If Me.Quote.Vehicles IsNot Nothing Then
                Try
                    vehicle = Me.Quote.Vehicles(Me.VehicleNumber)
                Catch ex As Exception
                End Try
                If vehicle IsNot Nothing Then
                    Dim qqHelper As New QuickQuoteHelperClass

                    Me.lblHeader.Text = String.Format("Vehicle #{0} - {1} {2} {3}", Me.VehicleNumber + 1, vehicle.Year, vehicle.Make, vehicle.Model)
                    If Me.lblHeader.Text.Length > 50 Then
                        Me.lblHeader.Text = Me.lblHeader.Text.Substring(0, 50) + "..."
                    End If

                    If Not IFM.VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote) Then
                        lblVehClass.Text = "(CLASS: " + vehicle.ClassCode + ")"
                    Else
                        lblVehClass.Visible = False
                    End If


                    If vehicle.NonOwnedNamed Or
                    (qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.ComprehensiveDeductibleLimitId, vehicle.ComprehensiveDeductibleLimitId, Quote.LobType) = "N/A" And
                    qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.CollisionDeductibleLimitId, vehicle.CollisionDeductibleLimitId, Quote.LobType) = "N/A") Then
                        pnlComp.Visible = False
                        pnlCollision.Visible = False
                        pnlTowing.Visible = False
                        pnlRental.Visible = False
                        pnlTrip.Visible = False
                        pnlSound.Visible = False
                        pnlAV.Visible = False
                        pnlMedia.Visible = False
                        pnlLoan.Visible = False
                        pnlPollution.Visible = False
                        pnlCustom.Visible = False
                        'Updated 12/17/18 for multi state bug 30381 MLW
                        Select Case (Quote.QuickQuoteState)
                            Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                                If vehicle.UninsuredMotoristPropertyDamageLimitId IsNot Nothing AndAlso vehicle.UninsuredMotoristPropertyDamageLimitId <> "0" AndAlso vehicle.UninsuredMotoristPropertyDamageLimitId <> "" AndAlso qqHelper.IsPositiveIntegerString(vehicle.UninsuredMotoristPropertyDamageLimitId) Then
                                    pnlUMPD.Visible = True
                                    If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                                        lbl_UMPD_Limit.Visible = True
	                                    lbl_UMPD_Limit.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, vehicle.UninsuredMotoristPropertyDamageLimitId, QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                                    End If
                                    If IFM.VR.Common.Helpers.PPA.CollisionAndUMPD.IsILCollisionAndUMPDAvailable(Quote) Then
                                        lbl_UMPD_Val.Text = umpdDeductible
                                    Else
                                        lbl_UMPD_Val.Text = ""
                                    End If
                                    'lbl_UMPD_Val.Text = ""
                                    lbl_UMPD_Prem.Text = vehicle.UninsuredMotoristPropertyDamageQuotedPremium
                                Else
                                    pnlUMPD.Visible = False
                                End If
                            Case Else
                                pnlUMPD.Visible = False 'Added 10/10/18 for multi state MLW
                        End Select

                    Else
                        Me.lbl_Comp_Prem.Text = vehicle.ComprehensiveQuotedPremium
                        Me.lbl_Comp_Val.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.ComprehensiveDeductibleLimitId, vehicle.ComprehensiveDeductibleLimitId, Quote.LobType)

                        If vehicle.ComprehensiveCoverageOnly Then
                            pnlCollision.Visible = False
                            pnlTowing.Visible = False
                            pnlRental.Visible = False
                            pnlTrip.Visible = False
                            pnlSound.Visible = False
                            pnlAV.Visible = False
                            pnlMedia.Visible = False
                            pnlLoan.Visible = False
                            pnlPollution.Visible = False
                            pnlCustom.Visible = False
                            pnlUMPD.Visible = False 'Added 10/10/18 for multi state MLW
                        Else
                            Me.lbl_Collision_Prem.Text = vehicle.CollisionQuotedPremium
                            Me.lbl_Collision_Val.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.CollisionDeductibleLimitId, vehicle.CollisionDeductibleLimitId, Quote.LobType)

                            If qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Rec. Trailer" Or
                            qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Other Trailer" Then
                                pnlTowing.Visible = False
                                pnlRental.Visible = False
                                pnlTrip.Visible = False
                                pnlSound.Visible = False
                                pnlAV.Visible = False
                                pnlMedia.Visible = False
                                pnlLoan.Visible = False
                                pnlPollution.Visible = False
                                pnlCustom.Visible = False
                                pnlUMPD.Visible = False 'Added 10/10/18 for multi state MLW
                            Else
                                Me.lbl_Towing_Prem.Text = vehicle.TowingAndLaborQuotedPremium
                                Me.lbl_Towing_Val.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.TowingAndLaborDeductibleLimitId, vehicle.TowingAndLaborDeductibleLimitId)

                                Me.lbl_Transportation_Prem.Text = vehicle.TransportationExpenseQuotedPremium
                                If IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote) = False Then
                                    If vehicle.TransportationExpenseLimitId = "0" Or vehicle.TransportationExpenseLimitId = "30" Then
                                        Me.lbl_Transportation_Val.Text = "[20/600 INC]"
                                    Else
                                        Me.lbl_Transportation_Val.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.TransportationExpenseLimitId, vehicle.TransportationExpenseLimitId)
                                    End If
                                Else
                                    If vehicle.TransportationExpenseLimitId = "446" Then
                                        Me.lbl_Transportation_Val.Text = "[40/1200 INC]"
                                    Else
                                        Me.lbl_Transportation_Val.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.TransportationExpenseLimitId, vehicle.TransportationExpenseLimitId)
                                    End If
                                End If

                                '527 Fix for blank values - TLB
                                If vehicle.TapesAndRecordsQuotedPremium = "" Then
                                    Me.lbl_Media_Prem.Text = "$0.00"
                                Else
                                    Me.lbl_Media_Prem.Text = vehicle.TapesAndRecordsQuotedPremium
                                End If

                                'If (vehicle.SoundEquipmentLimit = "0" Or vehicle.SoundEquipmentLimit = "") And (vehicle.ElectronicEquipmentLimit = "0" Or vehicle.ElectronicEquipmentLimit = "") Then
                                'Me.lbl_Media_Val.Text = "N/A"
                                'ElseIf ((vehicle.SoundEquipmentLimit <> "0" And vehicle.SoundEquipmentLimit <> "") Or (vehicle.ElectronicEquipmentLimit <> "0" And vehicle.ElectronicEquipmentLimit <> "")) And (vehicle.TapesAndRecordsLimitId = "0" Or vehicle.TapesAndRecordsLimitId = "") Then
                                If ((vehicle.SoundEquipmentLimit <> "0" And vehicle.SoundEquipmentLimit <> "") Or (vehicle.ElectronicEquipmentLimit <> "0" And vehicle.ElectronicEquipmentLimit <> "")) And (vehicle.TapesAndRecordsLimitId = "0" Or vehicle.TapesAndRecordsLimitId = "") Then
                                    Me.lbl_Media_Val.Text = "[200 INC]"
                                Else
                                    Me.lbl_Media_Val.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.TapesAndRecordsLimitId, vehicle.TapesAndRecordsLimitId)
                                End If

                                '527 Fix for blank values - TLB
                                Dim leasePrem As Double = 0

                                If vehicle.AutoLoanOrLeaseQuotedPremium <> "" Then
                                    leasePrem = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(vehicle.AutoLoanOrLeaseQuotedPremium)
                                End If

                                Me.lbl_Loan_Prem.Text = leasePrem.ToString("$0.00")
                                Me.lbl_Loan_Val.Text = If(vehicle.HasAutoLoanOrLease, "applied", "excluded")

                                'Added 10/10/18 for multi state MLW
                                Select Case (Quote.QuickQuoteState)
                                    Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                                        pnlUMPD.Visible = True
                                        If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                                            lbl_UMPD_Limit.Visible = True
	                                        lbl_UMPD_Limit.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, vehicle.UninsuredMotoristPropertyDamageLimitId, QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                                        End If
                                        If IFM.VR.Common.Helpers.PPA.CollisionAndUMPD.IsILCollisionAndUMPDAvailable(Quote) AndAlso (vehicle.UninsuredMotoristPropertyDamageLimitId <> "0" AndAlso IsNullEmptyorWhitespace(vehicle.UninsuredMotoristPropertyDamageLimitId) = False) Then
                                            lbl_UMPD_Val.Text = umpdDeductible
                                        Else
                                            lbl_UMPD_Val.Text = ""
                                        End If
                                        'lbl_UMPD_Val.Text = ""
                                        lbl_UMPD_Prem.Text = vehicle.UninsuredMotoristPropertyDamageQuotedPremium
                                    Case Else
                                        pnlUMPD.Visible = False
                                End Select

                                If qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Motorcycle" Then
                                    'Dim tripPrem As Double = Helpers.WebHelper_Personal.TryToGetDouble(vehicle.TripInterruptionQuotedPremium)
                                    Dim tripInterrupt As String = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.TripInterruptionLimitId, vehicle.TripInterruptionLimitId)
                                    Me.lblTripPrem.Text = "" ' tripPrem.ToString("$0.00")
                                    Me.lblTripVal.Text = If(tripInterrupt <> "N/A", "inc", "excluded")
                                    Me.pnlRental.Visible = False

                                    If vehicle.ScheduledItems IsNot Nothing AndAlso vehicle.ScheduledItems.Any() Then
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

                                        Me.lblCustomVal.Text = String.Format("{0:C2}", motorEquipInt)
                                        Me.lblCustomPrem.Text = String.Format("{0:C2}", motorPremInt)
                                    Else
                                        Me.lblCustomVal.Text = "$0.00"
                                        Me.lblCustomPrem.Text = "$0.00"
                                    End If

                                    pnlSound.Visible = False
                                    pnlAV.Visible = False
                                    pnlLoan.Visible = False
                                Else
                                    pnlTrip.Visible = False
                                    pnlCustom.Visible = False

                                    '527 Fix for blank values - TLB
                                    If vehicle.SoundEquipmentQuotedPremium = "" Then
                                        Me.lblSoundPrem.Text = "$0.00"
                                    Else
                                        Me.lblSoundPrem.Text = vehicle.SoundEquipmentQuotedPremium
                                    End If

                                    If vehicle.SoundEquipmentLimit = "0" Or vehicle.SoundEquipmentLimit = "" Then
                                        Me.lblSoundVal.Text = "[1,000 INC]"
                                    Else
                                        Me.lblSoundVal.Text = vehicle.SoundEquipmentLimit
                                    End If

                                    '527 Fix for blank values - TLB
                                    If vehicle.ElectronicEquipmentQuotedPremium = "" Then
                                        Me.lbl_AV_Prem.Text = "$0.00"
                                    Else
                                        Me.lbl_AV_Prem.Text = vehicle.ElectronicEquipmentQuotedPremium
                                    End If

                                    If vehicle.ElectronicEquipmentLimit = "0" Or vehicle.SoundEquipmentLimit = "" Then
                                        Me.lbl_AV_Val.Text = "N/A"
                                    Else
                                        Me.lbl_AV_Val.Text = vehicle.ElectronicEquipmentLimit
                                    End If
                                End If

                                Dim todayDate As Long = DateTime.Now.Year

                                If DateTime.Now.Month >= 10 Then
                                    todayDate += 1
                                End If
                                Dim vYear As Int32 = 0
                                Int32.TryParse(vehicle.Year, vYear)
                                If (todayDate - vYear) <= 5 Then
                                    pnlLoan.Visible = True
                                Else
                                    pnlLoan.Visible = False
                                End If

                                If vehicle.VehicleUseTypeId = "4" And (vehicle.BodyTypeId = "39" Or vehicle.BodyTypeId = "40") Then
                                    '527 Fix for blank values - TLB
                                    Dim pollutionPrem As Double = 0

                                    If vehicle.PollutionLiabilityBroadenedCoverageQuotedPremium <> "" Then
                                        pollutionPrem = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(vehicle.PollutionLiabilityBroadenedCoverageQuotedPremium)
                                    End If

                                    Me.lblPollutionPrem.Text = pollutionPrem.ToString("$0.00")
                                    Me.lblPollutionVal.Text = If(vehicle.HasPollutionLiabilityBroadenedCoverage, "applied", "excluded")
                                Else
                                    pnlPollution.Visible = False
                                End If
                            End If
                        End If
                    End If

                    If IFM.VR.Common.Helpers.PPA.CustomEquipmentHelper.IsCustomEquipmentAvailable(Quote) Then
                        Dim customEquipmentCoverage As QuickQuoteCoverage = (From cov In vehicle.Coverages Where cov.CoverageCodeId = 47 Select cov).FirstOrDefault()
                        If customEquipmentCoverage IsNot Nothing Then
                            Me.lblCustomEquipPrem.Text = FormatCurrency(customEquipmentCoverage.FullTermPremium)
                            Me.lblCustomEquipVal.Text = FormatCurrency(customEquipmentCoverage.ManualLimitAmount)
                        Else
                            Me.lblCustomEquipVal.Text = "$0.00"
                            Me.lblCustomEquipPrem.Text = "$0.00"
                        End If
                    Else
                        pnlCustomEquip.Visible = False
                    End If

	                'Illinois only - "Illinois State Police Training and Academy Fund" - added by Diamond at rate
                    If IFM.VR.Common.Helpers.PPA.PolicyTrainingFeeHelper.IsPoliceTrainingFeeAvailable(Quote) Then
                        Dim policeTrainingFeeCoverage As QuickQuoteCoverage = (From cov In vehicle.Coverages Where cov.CoverageCodeId = 100014 Select cov).FirstOrDefault()
                        If policeTrainingFeeCoverage IsNot Nothing AndAlso qqHelper.IsPositiveDecimalString(policeTrainingFeeCoverage.FullTermPremium) = True Then
                            pnlPoliceTrainingFee.Visible = True
                            Me.lblPoliceTrainingFeePrem.Text = FormatCurrency(policeTrainingFeeCoverage.FullTermPremium)
                            If IFM.VR.Common.Helpers.PPA.StatePoliceFundHelper.IsStatePoliceFundLabelAvailable(Quote) Then
                                tdCovName.InnerHtml = "State Police Law Enforcement Administration Fund"
                            End If
                        End If
                    End If

                    Me.lblVehicleSummary.Text = vehicle.PremiumFullTerm
                End If
            End If
        End If
    End Sub

    Public Function Save() As Boolean Implements IVRUI_P.Save
        Return True
    End Function

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm
        Me.ValidationHelper.GroupName = String.Format("Quote Summary Vehicle #{0}", Me.VehicleNumber + 1)

        Me.ValidationHelper.Clear()

        Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
        valSum.RegisterValidationHelper(Me.ValidationHelper)
    End Sub

End Class