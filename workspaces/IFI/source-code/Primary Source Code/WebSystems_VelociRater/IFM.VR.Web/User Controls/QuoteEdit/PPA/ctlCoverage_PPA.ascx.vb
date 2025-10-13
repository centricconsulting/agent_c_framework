Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
Imports IFM.VR.Web.ENUMHelper
Imports IFM.PrimativeExtensions

Public Class ctlCoverage_PPA
    Inherits VRControlBase

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW

    Protected PolicyPlan As ctlCoverage_PPA_Vehicle_List
    Protected UpdateDropDowns As ctlCoverage_PPA_Vehicle_List

    Public Event CoveragesChanged()
    Public Event ListRefreshRequested()
    Public Event LType(lType As String)
    Public Event SaveAllCoverages(validate As Boolean)
    Public Event QuoteRateRequested()
    ''' <summary>
    ''' not used elsewhere, may be deprecated
    ''' </summary>
    ''' <returns></returns> 
    Public Property VehicleNumber As Int32
        Get
            If ViewState("vs_vehicleNum") Is Nothing Then
                ViewState("vs_vehicleNum") = 0
            End If
            Return CInt(ViewState("vs_vehicleNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_vehicleNum") = value
        End Set
    End Property

    ''' <summary>
    ''' not used elsewhere, may be deprecated
    ''' </summary>
    ''' <returns></returns>
    Public Property IsVehicleNumber As Boolean
        Get
            If ViewState("vs_vehicleNum") Is Nothing Then
                ViewState("vs_vehicleNum") = False
            End If
            Return CBool(ViewState("vs_vehicleNum"))
        End Get
        Set(value As Boolean)
            ViewState("vs_vehicleNum") = value
        End Set
    End Property

    ''' <summary>
    ''' not used elsewhere, may be deprecated
    ''' </summary>
    ''' <returns></returns>
    Public Property SelectedLiabilityType() As String
        Get
            Return ViewState("vs_LiabType")?.ToString
        End Get
        Set(ByVal Value As String)
            ViewState("vs_LiabType") = Value
        End Set
    End Property

    Public Property SelectedPolicyPlan() As String
        Get
            Return ViewState("vs_PolicyPlan")?.ToString
        End Get
        Set(ByVal value As String)
            ViewState("vs_PolicyPlan") = value
        End Set
    End Property

    Private _childDriver As Boolean
    Public Property ChildDriver() As Boolean
        Get
            Return _childDriver
        End Get
        Set(ByVal value As Boolean)
            _childDriver = value
        End Set
    End Property

    Private _p1OverForty As Boolean
    Public Property P1OverForty() As Boolean
        Get
            Return _p1OverForty
        End Get
        Set(ByVal value As Boolean)
            _p1OverForty = value
        End Set
    End Property

    Private _p2OverForty As Boolean
    Public Property P2OverForty() As Boolean
        Get
            Return _p2OverForty
        End Get
        Set(ByVal value As Boolean)
            _p2OverForty = value
        End Set
    End Property

    Public ReadOnly Property ToggleCompOnlyPanel() As Boolean
        Get
            Return CBool(Session("sess_CompOnlyToggle"))
        End Get
    End Property

    Public ReadOnly Property isAutoPlusEnhancementChecked As Boolean
        Get
            Return chkAutoPlusEnhance.Checked
        End Get
    End Property
    Public ReadOnly Property chkAutoPlusEnhancementClientId As String
        Get
            Return chkAutoPlusEnhance.ClientID
        End Get
    End Property
    Public ReadOnly Property chkAutoEnhancementClientId As String
        Get
            Return chkAutoEnhance.ClientID
        End Get
    End Property

    Private ReadOnly Property isAutoPlusEnhancementAvailable As Boolean
        Get
            If IsValidEffectiveDateForAutoPlusEnhancement(Quote.EffectiveDate) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public Property RouteToUwIsVisible As Boolean 'added 11/11/2019
        Get
            Return Me.ctlCoverage_PPA_Vehicle_List.RouteToUwIsVisible
        End Get
        Set(value As Boolean)
            Me.ctlCoverage_PPA_Vehicle_List.RouteToUwIsVisible = value
        End Set
    End Property

#Region "Selected Values"
    Public ReadOnly Property SelectedLiabilityTypeValue() As String
        Get
            Return Me.ddLiabType.SelectedValue
        End Get
    End Property

    'TODO: need to expose all selcted values this way instead of using FindControl to decrease tight coupling

#End Region

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.CreateAccordion(MainAccordionDivId, hiddenPolicyCoverage, "0", True)

        VRScript.StopEventPropagation(lnkBtnClear.ClientID, False)
        VRScript.StopEventPropagation(lnkBtnSave.ClientID, False)

        Dim usedPopupIds As New List(Of String)
        Dim autoEnhacementPopupMessage As String = "<div>Auto Enhancement Endorsement</div><br /><div>The Auto Enhancement provides additional coverages to protect from unexpected losses:</div><div><ul><li>Replacement cost coverage (comp and collision) for current and previous model year vehicles.</li><li>For Rental Cars – diminution in value and ""Loss Or Use"" coverage ($40/$1,200 max)</li><li>For vehicle parts of the current model year and five previous, no depreciation applied.</li><li>$600 for emergency travel expenses 100+ miles from home.</li><li>$500 for Personal Property/audio visual equipment and media.</li><li>$500 for Fire Department Service Charges.</li><li>$1,000 for recharging unintended air bag deployment.</li><li>$500 for emergency locksmith services.</li><li>$1,500 coverage for ""non-owned"" trailers.</li></ul></div>"
        Dim autoPlusEnhancementPopupMessage As String = "<div>Auto Plus Enhancement Endorsement</div><br /><div>The Auto Plus Enhancement provides additional coverages to protect from unexpected losses:</div><div><ul><li>Replacement cost coverage (comp and collision) for current and previous model year vehicles.</li><li>For Rental Cars – diminution in value and ""Loss or Use"" coverage ($40/$1,200 max)</li><li>For vehicle parts of the current model year and five previous, no depreciation applied.</li><li>$600 for emergency travel expenses 100+ miles from home.</li><li>$500 for Personal Property/audio visual equipment and media.</li><li>$500 for Fire Department Service Charges.</li><li>$1,000 for recharging unintended air bag deployment.</li><li>$500 for emergency locksmith services</li><li>$1,500 coverage for ""non-owned"" trailers.</li><li>Comp coverage for repair/replacement of safety glass without application of a deductible.</li><li>Rental Reimbursement when owned auto is damaged ($40/$1,200 max)</li></ul></div>"

        usedPopupIds.Add(VRScript.CreatePopUpWindow_jQueryUi_Dialog_CreatesUniqueDiv("Auto Enhancement Endoresement Note:", autoEnhacementPopupMessage, 400, 375, False, True, True, lnkAutoEnhance.ClientID))
        usedPopupIds.Add(VRScript.CreatePopUpWindow_jQueryUi_Dialog_CreatesUniqueDiv("Auto Plus Enhancement Note:", autoPlusEnhancementPopupMessage, 400, 425, False, True, True, lnkAutoPlusEnhance.ClientID, Nothing, usedPopupIds))
        
        Dim IsTransportationAvailable As String = IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote).ToString
        Dim scriptSwitchChecksAutoPlusEnhanceAndAutoEnhance As String = "switchChecksAutoPlusEnhanceAndAutoEnhance(this, """ + chkAutoPlusEnhance.ClientID + """, """ + chkAutoEnhance.ClientID + """, """ + IsTransportationAvailable +""");"
        chkAutoEnhance.Attributes.Add("onclick", scriptSwitchChecksAutoPlusEnhanceAndAutoEnhance)

        Dim scriptChangeTransportationDDLValues As String = "changeTransportationExpenseDDLOptions(""" + chkAutoPlusEnhance.ClientID + """);"
        chkAutoPlusEnhance.Attributes.Add("onclick", scriptChangeTransportationDDLValues & scriptSwitchChecksAutoPlusEnhanceAndAutoEnhance)
        VRScript.AddScriptLine(scriptChangeTransportationDDLValues)

        Dim scriptShowHideAutoPlusEnhance As String = "showHideAutoPlusEnhancement(""" + dvAutoPlusEnhance.ClientID + """, """ + chkAutoPlusEnhance.ClientID + """, """ + chkAutoEnhance.ClientID + """, """ + IsTransportationAvailable + """);"
        VRScript.AddScriptLine(scriptShowHideAutoPlusEnhance)

        ' Change Liability Type
        'Updated 9/26/18 for multi state MLW
        Dim scriptLiabilityTypeToggle As String = "ToggleLiabilityTypeStateExpansion(this, """ + Quote.State + """, """ + dvR1C1.ClientID + """, """ + dvR1C2.ClientID + """, """ + dvR1C2SE.ClientID +
                        """, """ + dvR2C1.ClientID + """, """ + dvR2C2.ClientID + """, """ + dvR2C2SE.ClientID + """, """ + dvR3C2.ClientID + """, """ + dvR4C2.ClientID + """, """ + ddBodilyInjury.ClientID + """, """ + ddPropertyDamage.ClientID +
                        """, """ + ddSingleLimitLib.ClientID + """, """ + ddmedicalPayments.ClientID + """, """ + ddUmUimSSl.ClientID + """, """ + ddUMCSL.ClientID + """, """ + txtUIMCSL.ClientID +
                        """, """ + ddUmUmiBi.ClientID + """, """ + ddUMBI.ClientID + """, """ + txtUIMBI.ClientID + """, """ + ddUmPd.ClientID + """, """ + ddUmPdDeductible.ClientID + """, """ + chkMultiPolicyDiscount.ClientID + """, """ + PersonalLiabilitylimitTextPPASPLIT.ClientID + """, """ + PersonalLiabilitylimitTextPPACSL.ClientID +
                        """, """ + chkSelectMarketCredit.ClientID + """, """ + chkAutoEnhance.ClientID + """, """ + Quote.HasBusinessMasterEnhancement.ToString() + """, """ + dvMarketCredit.ClientID +
                        """, """ + ChildDriver.ToString() + """, """ + P1OverForty.ToString() + """, """ + P2OverForty.ToString() + """);"
        ddLiabType.Attributes.Add("onchange", scriptLiabilityTypeToggle)
        'Dim scriptLiabilityTypeToggle As String = "ToggleLiabilityType(this, """ + dvR1C1.ClientID + """, """ + dvR1C2.ClientID + """, """ + dvR2C1.ClientID + """, """ + dvR2C2.ClientID +
        '            """, """ + dvR3C2.ClientID + """, """ + ddBodilyInjury.ClientID + """, """ + ddPropertyDamage.ClientID + """, """ + ddSingleLimitLib.ClientID + """, """ + ddmedicalPayments.ClientID +
        '            """, """ + ddUmUimSSl.ClientID + """, """ + ddUmUmiBi.ClientID + """, """ + ddUmPd.ClientID + """, """ + ddUmPdDeductible.ClientID + """, """ + chkMultiPolicyDiscount.ClientID +
        '            """, """ + chkSelectMarketCredit.ClientID + """, """ + chkAutoEnhance.ClientID + """, """ + Quote.HasBusinessMasterEnhancement.ToString() + """, """ + dvMarketCredit.ClientID +
        '            """, """ + ChildDriver.ToString() + """, """ + P1OverForty.ToString() + """, """ + P2OverForty.ToString() + """);"
        'ddLiabType.Attributes.Add("onchange", scriptLiabilityTypeToggle)

        'Added 9/26/18 for multi state MLW, Updated 1/17/2022 for OH task 66101 MLW
        If Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois OrElse Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
            Dim scriptUMCSLToggle As String = "ToggleUMCSL(""" + ddUMCSL.ClientID + """, """ + txtUIMCSL.ClientID + """, """ + Quote.State + """);"
            ddUMCSL.Attributes.Add("onchange", scriptUMCSLToggle)
            Dim scriptUMBIToggle As String = "ToggleUMBI(""" + ddUMBI.ClientID + """, """ + txtUIMBI.ClientID + """, """ + Quote.State + """);"
            ddUMBI.Attributes.Add("onchange", scriptUMBIToggle)

            'Added 10/8/18 for multi state MLW
            Dim scriptRelatedHomeFarmToggle As String = "ToggleRelatedHomeFarm(""" + chkAutoHomeDiscount_Parachute.ClientID + """, """ + tr_RelatedHomeFarm.ClientID + """,""" + txtMoreInfo.ClientID + """);"
            chkAutoHomeDiscount_Parachute.Attributes.Add("onclick", scriptRelatedHomeFarmToggle)
            VRScript.AddScriptLine(scriptRelatedHomeFarmToggle)
        End If

        If Quote IsNot Nothing Then
            If Quote.Vehicles IsNot Nothing Then
                Dim bodilyInj As Integer = 0
                Dim propDamage As String = ""
                Dim SSL As String = ""
                Dim medPay As String = ""
                Dim umSSL As String = ""
                Dim umBI As String = ""
                Dim umPD As String = ""
                Dim umPDDeduct As String = ""
                Dim marketCredit As Boolean = False
                Dim autoEnhance As Boolean = False
                Dim multiPolicy As Boolean = False

                Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = (From v In Me.Quote.Vehicles Where (String.IsNullOrWhiteSpace(v.BodilyInjuryLiabilityLimitId) = False Or String.IsNullOrWhiteSpace(v.Liability_UM_UIM_LimitId) = False) And Not v.ComprehensiveCoverageOnly And v.BodyTypeId <> CInt(ENUMHelper.VehicleBodyType.bodyType_RecTrailer).ToString() And v.BodyTypeId <> CInt(ENUMHelper.VehicleBodyType.bodyType_OtherTrailer).ToString() Select v).FirstOrDefault()

                If vehicle Is Nothing Then
                    vehicle = New QuickQuote.CommonObjects.QuickQuoteVehicle
                    vehicle.BodilyInjuryLiabilityLimitId = "0"
                    vehicle.PropertyDamageLimitId = "0"
                    vehicle.Liability_UM_UIM_LimitId = "0"
                    vehicle.MedicalPaymentsLimitId = "0"
                    'Added 9/26/18 for multi state MLW
                    Select Case (Quote.QuickQuoteState)
                        Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                            vehicle.UninsuredCombinedSingleLimitId = "0"
                            vehicle.UnderinsuredCombinedSingleLimitId = "0"
                            vehicle.UninsuredBodilyInjuryLimitId = "0"
                            vehicle.UnderinsuredBodilyInjuryLimitId = "0"
                            vehicle.UninsuredMotoristPropertyDamageLimitId = "0"
                            vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ""
                        Case Else
                            vehicle.UninsuredCombinedSingleLimitId = "0"
                            vehicle.UninsuredMotoristLiabilityLimitId = "0"
                            vehicle.UninsuredMotoristPropertyDamageLimitId = "0"
                            vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = "0"
                    End Select
                End If

                'Dim nonCompCnt As String = Quote.Vehicles.FindAll(Function(p) p.BodyTypeId <> VehicleBodyType.bodyType_RecTrailer AndAlso p.BodyTypeId <> VehicleBodyType.bodyType_OtherTrailer AndAlso (Not p.ComprehensiveCoverageOnly)).Count.ToString()
                For idx = 0 To Quote.Vehicles.Count - 1
                    'Updated 9/27/18 for multi state MLW - added Quote.State, ddUMCSL.ClientID, txtUIMCSL.ClientID, ddUMBI.ClientID, txtUIMBI.ClientID, vehicle.UninsuredCombinedSingleLimitId, and vehicle.UninsuredBodilyInjuryLimitId 
                    Dim scriptCommonVehicle As String = "PopulateVehiclePolicyCoverage(this,""" + Quote.State + """,""" + idx.ToString() + """, """ + divPolicyCoverage.ClientID + """,""" + hiddenVehCovPlanList.ClientID +
                        """,""" + Quote.Vehicles(idx).BodyTypeId + """,""" + Quote.Vehicles(idx).NonOwnedNamed.ToString() + """,""" + Quote.Vehicles(idx).NonOwned.ToString() + """,""" + dvAutoEnhance.ClientID +
                        """,""" + chkAutoEnhance.ClientID + """,""" + ddBodilyInjury.ClientID + """,""" + ddPropertyDamage.ClientID + """,""" + ddSingleLimitLib.ClientID + """,""" + ddmedicalPayments.ClientID +
                        """,""" + ddUmUimSSl.ClientID + """,""" + ddUMCSL.ClientID + """,""" + txtUIMCSL.ClientID + """, """ + ddUmUmiBi.ClientID + """,""" + ddUMBI.ClientID + """, """ + txtUIMBI.ClientID +
                        """,""" + ddUmPd.ClientID + """,""" + ddUmPdDeductible.ClientID + """,""" + ddLiabType.ClientID +
                        """,""" + vehicle.BodilyInjuryLiabilityLimitId + """,""" + vehicle.PropertyDamageLimitId + """,""" + vehicle.Liability_UM_UIM_LimitId + """,""" + vehicle.MedicalPaymentsLimitId +
                        """,""" + vehicle.UninsuredCombinedSingleLimitId + """,""" + vehicle.UnderinsuredCombinedSingleLimitId + """,""" + vehicle.UninsuredMotoristLiabilityLimitId +
                        """,""" + vehicle.UninsuredBodilyInjuryLimitId + """,""" + vehicle.UninsuredMotoristPropertyDamageLimitId +
                        """,""" + vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId + """, """ + chkAutoPlusEnhance.ClientID + """);"
                    'VRScript.CreateJSBinding("pnlAccordHeader" + idx.ToString(), ctlPageStartupScript.JsEventType.onclick, scriptCommonVehicle)
                    VRScript.CreateJSBinding("ddPolicy" + idx.ToString(), ctlPageStartupScript.JsEventType.onchange, scriptCommonVehicle)
                Next
            End If
        End If
    End Sub

    Private Sub SetCSS(myControl As HtmlGenericControl, CSSSetting As String)
        Dim a As String = CSSSetting.Split(":"c)(0)
        If myControl.Attributes.Item("style") IsNot Nothing AndAlso myControl.Attributes.Item("style").Contains(a) Then
            myControl.Attributes.Item("style") = CSSSetting
        Else
            myControl.Attributes.Add("style", CSSSetting)
        End If
    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not IsPostBack Then
            MainAccordionDivId = dvCoverageType.ClientID
        End If

        Dim VehicleCoverageList As ctlCoverage_PPA_Vehicle_List = New ctlCoverage_PPA_Vehicle_List()
    End Sub

    Protected Sub GetPolicyPlan(plan As String)
        SelectedPolicyPlan = plan
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not IsPostBack Then
            MainAccordionDivId = dvCoverageType.ClientID
            LoadLiabilityDropDowns()
            LoadStaticData()
        End If

        hiddenAutoPlusEnhancementEffectiveDate.Value = AutoPlusEnhancement_EffectiveDate()

        If Me.Quote IsNot Nothing Then
            If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Any() Then
                If Not IsPostBack Then
                    LoadStaticData()
                End If

                For Each vehicle As QuickQuoteVehicle In Me.Quote.Vehicles
                    If Not IsPostBack Then
                        Populate()
                    Else
                        CheckPolicyBoxes(vehicle)
                    End If
                Next
            End If
        End If
        If isAutoPlusEnhancementAvailable Then
            SetCSS(dvAutoPlusEnhance, "display:block;")
        Else
            If Me.chkAutoPlusEnhance.Checked = True Then
                Me.ValidationHelper.AddWarning("Auto Plus Enhancement Endorsement requires the effective date to be on or after " & AutoPlusEnhancement_EffectiveDate() & ". Auto Plus Enhancement Endorsement has been removed.")
            End If
            Me.chkAutoPlusEnhance.Checked = False
            SetCSS(dvAutoPlusEnhance, "display:none;")
        End If

    End Sub

    Public Overrides Sub LoadStaticData()
        Dim qqHelper As New QuickQuoteHelperClass
        qqHelper.LoadStaticDataOptionsDropDown(Me.ddSingleLimitLib, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.Liability_UM_UIM_LimitId, SortBy.None, Me.Quote.LobType)
        qqHelper.LoadStaticDataOptionsDropDown(Me.ddmedicalPayments, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.MedicalPaymentsLimitId, SortBy.None, Me.Quote.LobType)
        qqHelper.LoadStaticDataOptionsDropDown(Me.ddBodilyInjury, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodilyInjuryLiabilityLimitId, SortBy.None, Me.Quote.LobType)
        qqHelper.LoadStaticDataOptionsDropDown(Me.ddPropertyDamage, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.PropertyDamageLimitId, SortBy.None, Me.Quote.LobType)
        'Updated 10/2/18 for multi state MLW
        Select Case (Quote.QuickQuoteState)
            Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                qqHelper.LoadStaticDataOptionsDropDownForState(Me.ddUMCSL, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredCombinedSingleLimitId, Quote.QuickQuoteState, SortBy.None, Me.Quote.LobType)
                qqHelper.LoadStaticDataOptionsDropDownForState(Me.ddUMBI, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, Quote.QuickQuoteState, SortBy.None, Me.Quote.LobType)
            Case Else
                qqHelper.LoadStaticDataOptionsDropDown(Me.ddUmUimSSl, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredCombinedSingleLimitId, SortBy.None, Me.Quote.LobType)
                qqHelper.LoadStaticDataOptionsDropDown(Me.ddUmUmiBi, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredMotoristLiabilityLimitId, SortBy.None, Me.Quote.LobType)
                qqHelper.LoadStaticDataOptionsDropDown(Me.ddUmPd, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, SortBy.None, Me.Quote.LobType)
                qqHelper.LoadStaticDataOptionsDropDown(Me.ddUmPdDeductible, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredMotoristPropertyDamageDeductibleLimitId, SortBy.None, Me.Quote.LobType)
        End Select
        qqHelper.LoadStaticDataOptionsDropDown(Me.ddPriorBiLimit, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.PriorBiCoverageCodeLimitId, SortBy.None, Me.Quote.LobType)
        'LoadLiabilityDropDowns() - Matt A removed 2-15-2017

        ' Matt A Bug 8161 & 8258 1-20-2017
        If IsDate(Me.Quote.EffectiveDate) AndAlso CDate(Me.Quote.EffectiveDate) >= CDate(System.Configuration.ConfigurationManager.AppSettings("VR_PPA_PropertyLiab_MinChange_StartDate")) Then

            Dim PD_TenKItem = (From i As ListItem In Me.ddPropertyDamage.Items Where i.Value = "7" Select i).FirstOrDefault()
            Dim PD_FifteenKItem = (From i As ListItem In Me.ddPropertyDamage.Items Where i.Value = "48" Select i).FirstOrDefault()

            If PD_TenKItem IsNot Nothing Then
                Me.ddPropertyDamage.Items.Remove(PD_TenKItem)
            End If

            If PD_FifteenKItem IsNot Nothing Then
                Me.ddPropertyDamage.Items.Remove(PD_FifteenKItem)
            End If

            Dim UMPD_TenKItem = (From i As ListItem In Me.ddUmPd.Items Where i.Value = "7" Select i).FirstOrDefault()
            If UMPD_TenKItem IsNot Nothing Then
                Me.ddUmPd.Items.Remove(UMPD_TenKItem)
            End If


            Dim SingleLimit_SixtyKItem = (From i As ListItem In Me.ddSingleLimitLib.Items Where i.Value = "122" Select i).FirstOrDefault()
            If SingleLimit_SixtyKItem IsNot Nothing Then
                Me.ddSingleLimitLib.Items.Remove(SingleLimit_SixtyKItem)
            End If

            'Updated 9/26/18 for multi state MLW
            Select Case (Quote.QuickQuoteState)
                Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                    'Uses XML ignoreForLists attribute
                Case Else
                    Dim UmUimSSl_SixtyKItem = (From i As ListItem In Me.ddUmUimSSl.Items Where i.Value = "122" Select i).FirstOrDefault()
                    If UmUimSSl_SixtyKItem IsNot Nothing Then
                        Me.ddUmUimSSl.Items.Remove(SingleLimit_SixtyKItem)
                    End If
            End Select


        End If

    End Sub

    Public Overrides Sub Populate()
        If Quote IsNot Nothing Then
            If Quote.Vehicles IsNot Nothing Then
                If Quote.Vehicles.Count > 0 Then
                    SetVisible(ddLiabType.SelectedValue)
                    txtUIMCSL.Enabled = False 'Added 9/26/18 for multi state MLW
                    txtUIMBI.Enabled = False 'Added 9/26/18 for multi state MLW
                    Dim vehicle As QuickQuoteVehicle = (From veh In Quote.Vehicles Where If(veh.BodyTypeId = "", "0", veh.BodyTypeId).TryToGetInt32 <> ENUMHelper.VehicleBodyType.bodyType_OtherTrailer AndAlso If(veh.BodyTypeId = "", "0", veh.BodyTypeId).TryToGetInt32 <> ENUMHelper.VehicleBodyType.bodyType_RecTrailer AndAlso Not veh.ComprehensiveCoverageOnly Select veh).FirstOrDefault()
                    'SetDefaultValues(Quote.Vehicles(VehicleNumber))

                    If vehicle IsNot Nothing Then
                        'Set Vehicle Policy Values

                        ' Matt A Bug 8161 & 8258 1-20-2017
                        If IsDate(Me.Quote.EffectiveDate) AndAlso CDate(Me.Quote.EffectiveDate) >= CDate(System.Configuration.ConfigurationManager.AppSettings("VR_PPA_PropertyLiab_MinChange_StartDate")) Then
                            ' remove the values that are no longer valid for this timeframe
                            Dim PD_TenKItem = (From i As ListItem In Me.ddPropertyDamage.Items Where i.Value = "7" Select i).FirstOrDefault()
                            Dim PD_FifteenKItem = (From i As ListItem In Me.ddPropertyDamage.Items Where i.Value = "48" Select i).FirstOrDefault()

                            If PD_TenKItem IsNot Nothing Then
                                Me.ddPropertyDamage.Items.Remove(PD_TenKItem)
                            End If

                            If PD_FifteenKItem IsNot Nothing Then
                                Me.ddPropertyDamage.Items.Remove(PD_FifteenKItem)
                            End If

                            'Updated 10/2/18 for multi state MLW, Updated 1/17/2022 for OH task 66101 MLW
                            If (Quote.QuickQuoteState <> QuickQuoteHelperClass.QuickQuoteState.Illinois) AndAlso (Quote.QuickQuoteState <> QuickQuoteHelperClass.QuickQuoteState.Ohio) Then
                                Dim UMPD_TenKItem = (From i As ListItem In Me.ddUmPd.Items Where i.Value = "7" Select i).FirstOrDefault()
                                If UMPD_TenKItem IsNot Nothing Then
                                    Me.ddUmPd.Items.Remove(UMPD_TenKItem)
                                End If
                            End If

                            Dim SingleLimit_SixtyKItem = (From i As ListItem In Me.ddSingleLimitLib.Items Where i.Value = "122" Select i).FirstOrDefault()
                            If SingleLimit_SixtyKItem IsNot Nothing Then
                                Me.ddSingleLimitLib.Items.Remove(SingleLimit_SixtyKItem)
                            End If

                            If Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois Then
                                Dim UMBI_NAItem = (From i As ListItem In Me.ddUMBI.Items Where i.Value = "0" Select i).FirstOrDefault()
                                If UMBI_NAItem IsNot Nothing Then
                                    UMBI_NAItem.Text = "25,000/50,000"
                                End If

                                Dim UmCSL_FixtyKItem = (From i As ListItem In Me.ddUMCSL.Items Where i.Value = "0" Select i).FirstOrDefault()
                                If UmCSL_FixtyKItem IsNot Nothing Then
                                    UmCSL_FixtyKItem.Text = "50,000"
                                End If
                            End If

                            'Updated 9/26/18 for multi state MLW
                            Select Case (Quote.QuickQuoteState)
                                Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                                    Dim UmCSL_SixtyKItem = (From i As ListItem In Me.ddUMCSL.Items Where i.Value = "122" Select i).FirstOrDefault()
                                    If UmCSL_SixtyKItem IsNot Nothing Then
                                        Me.ddUMCSL.Items.Remove(SingleLimit_SixtyKItem)
                                    End If
                                Case Else
                                    Dim UmUimSSl_SixtyKItem = (From i As ListItem In Me.ddUmUimSSl.Items Where i.Value = "122" Select i).FirstOrDefault()
                                    If UmUimSSl_SixtyKItem IsNot Nothing Then
                                        Me.ddUmUimSSl.Items.Remove(SingleLimit_SixtyKItem)
                                    End If
                            End Select


                            If Me.Visible Then ' set new limits if selected limit is no longer valid and add warnings

                                'Split LImits
                                If vehicle.PropertyDamageLimitId.EqualsAny("7", "48") Then
                                    'WebHelper_Personal.SetdropDownFromValue(ddPropertyDamage, "8")
                                    vehicle.PropertyDamageLimitId = "8"
                                    Me.ValidationHelper.GroupName = "Coverages"
                                    Me.ValidationHelper.AddWarning("Property Damage can not be less than 25,000. Value changed to 25,000.", Me.ddPropertyDamage.ClientID)
                                End If

                                'Updated 10/2/18 for multi state MLW, Updated 1/17/2022 for OH task 66101 MLW
                                If (Quote.QuickQuoteState <> QuickQuoteHelperClass.QuickQuoteState.Illinois) AndAlso (Quote.QuickQuoteState <> QuickQuoteHelperClass.QuickQuoteState.Ohio) Then
                                    If vehicle.UninsuredMotoristPropertyDamageLimitId.EqualsAny("7") Then
                                        'WebHelper_Personal.SetdropDownFromValue(ddUmPd, "8")
                                        vehicle.UninsuredMotoristPropertyDamageLimitId = "8"
                                        Me.ValidationHelper.GroupName = "Coverages"
                                        Me.ValidationHelper.AddWarning("UM PD can not be less than 25,000. Value changed to 25,000.", Me.ddUmPd.ClientID)
                                    End If
                                End If


                                'Combined Limits
                                If vehicle.Liability_UM_UIM_LimitId.EqualsAny("122") Then
                                    'WebHelper_Personal.SetdropDownFromValue(ddSingleLimitLib, "50")
                                    vehicle.Liability_UM_UIM_LimitId = "50"
                                    Me.ValidationHelper.GroupName = "Coverages"
                                    Me.ValidationHelper.AddWarning("Single Limit Liability can not be less than 75,000. Value changed to 75,000.", Me.ddSingleLimitLib.ClientID)
                                End If
                                'Updated 9/26/18 for multi state MLW
                                Select Case (Quote.QuickQuoteState)
                                    Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                                        If vehicle.UninsuredCombinedSingleLimitId.EqualsAny("122") Then
                                            'WebHelper_Personal.SetdropDownFromValue(ddUMCSL, "50")
                                            vehicle.UninsuredCombinedSingleLimitId = "50"
                                            Me.ValidationHelper.GroupName = "Coverages"
                                            Me.ValidationHelper.AddWarning("UM CSL and UIM CSL can not be less than 75,000. Values changed to 75,000.", Me.ddUMCSL.ClientID)
                                        End If
                                    Case Else
                                        If vehicle.UninsuredCombinedSingleLimitId.EqualsAny("122") Then
                                            'WebHelper_Personal.SetdropDownFromValue(ddUmUimSSl, "50")
                                            vehicle.UninsuredCombinedSingleLimitId = "50"
                                            Me.ValidationHelper.GroupName = "Coverages"
                                            Me.ValidationHelper.AddWarning("UM/UIM CSL can not be less than 75,000. Value changed to 75,000.", Me.ddUmPd.ClientID)
                                        End If
                                End Select


                            End If



                        Else
                            ' was it removed and now needs added back ?
                            Dim PD_TenKItem = (From i As ListItem In Me.ddPropertyDamage.Items Where i.Value = "7" Select i).FirstOrDefault()
                            Dim PD_FifteenKItem = (From i As ListItem In Me.ddPropertyDamage.Items Where i.Value = "48" Select i).FirstOrDefault()
                            Dim SingleLimit_SixtyKItem = (From i As ListItem In Me.ddSingleLimitLib.Items Where i.Value = "122" Select i).FirstOrDefault()
                            'Updated 10/2/18 for multi state MLW
                            Select Case (Quote.QuickQuoteState)
                                Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                                    If (PD_TenKItem.IsNull OrElse PD_FifteenKItem.IsNull OrElse SingleLimit_SixtyKItem.IsNull) Then
                                        LoadStaticData()
                                    End If
                                Case Else
                                    Dim UMPD_TenKItem = (From i As ListItem In Me.ddUmPd.Items Where i.Value = "7" Select i).FirstOrDefault()
                                    If (PD_TenKItem.IsNull OrElse PD_FifteenKItem.IsNull OrElse UMPD_TenKItem.IsNull OrElse SingleLimit_SixtyKItem.IsNull) Then
                                        LoadStaticData()
                                    End If
                            End Select
                        End If

                        WebHelper_Personal.SetdropDownFromValue(ddSingleLimitLib, vehicle.Liability_UM_UIM_LimitId)
                        WebHelper_Personal.SetdropDownFromValue(ddPropertyDamage, vehicle.PropertyDamageLimitId)
                        WebHelper_Personal.SetdropDownFromValue(ddBodilyInjury, vehicle.BodilyInjuryLiabilityLimitId)

                        'Updated 10/2/18 for multi state MLW
                        Select Case (Quote.QuickQuoteState)
                            Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                                WebHelper_Personal.SetdropDownFromValue(ddUMCSL, vehicle.UninsuredCombinedSingleLimitId)
                                txtUIMCSL.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredCombinedSingleLimitId, Quote.QuickQuoteState, ddUMCSL.SelectedValue)
                                WebHelper_Personal.SetdropDownFromValue(ddUMBI, vehicle.UninsuredBodilyInjuryLimitId)
                                txtUIMBI.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, Quote.QuickQuoteState, ddUMBI.SelectedValue)
                            Case Else
                                WebHelper_Personal.SetdropDownFromValue(ddUmUimSSl, vehicle.UninsuredCombinedSingleLimitId)
                                WebHelper_Personal.SetdropDownFromValue(ddUmUmiBi, vehicle.UninsuredMotoristLiabilityLimitId)
                                WebHelper_Personal.SetdropDownFromValue(ddUmPd, vehicle.UninsuredMotoristPropertyDamageLimitId)
                                WebHelper_Personal.SetdropDownFromValue(ddUmPdDeductible, vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId)
                        End Select
                        WebHelper_Personal.SetdropDownFromValue(ddmedicalPayments, vehicle.MedicalPaymentsLimitId)
                        If Not IsQuoteEndorsement() Then
                            If Me.ddLiabType.SelectedValue = "0" OrElse Me.ddLiabType.SelectedValue = "" Then
                                Me.PersonalLiabilitylimitTextPPASPLIT.Attributes.Add("style", "display:''")
                                Me.PersonalLiabilitylimitTextPPACSL.Attributes.Add("style", "display:none")
                            Else
                                Me.PersonalLiabilitylimitTextPPACSL.Attributes.Add("style", "display:''")
                                Me.PersonalLiabilitylimitTextPPASPLIT.Attributes.Add("style", "display:none")
                            End If
                        End If
                    Else
                        'Vehicle is NUll
                        WebHelper_Personal.SetdropDownFromValue(ddBodilyInjury, "0")
                        WebHelper_Personal.SetdropDownFromValue(ddPropertyDamage, "0")
                        WebHelper_Personal.SetdropDownFromValue(ddSingleLimitLib, "0")
                        'Updated 10/2/18 for multi state MLW
                        Select Case (Quote.QuickQuoteState)
                            Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                                WebHelper_Personal.SetdropDownFromValue(ddUMCSL, "0")
                                txtUIMCSL.Text = "N/A"
                                WebHelper_Personal.SetdropDownFromValue(ddUMBI, "0")
                                txtUIMBI.Text = "N/A"
                            Case Else
                                WebHelper_Personal.SetdropDownFromValue(ddUmUimSSl, "0")
                                WebHelper_Personal.SetdropDownFromValue(ddUmUmiBi, "0")
                                WebHelper_Personal.SetdropDownFromValue(ddUmPd, "0")
                                WebHelper_Personal.SetdropDownFromValue(ddUmPdDeductible, "0")
                        End Select
                        WebHelper_Personal.SetdropDownFromValue(ddmedicalPayments, "0")

                    End If

                    PopulateChildControls()

                    'If Quote.Vehicles(VehicleNumber).BodyTypeId IsNot Nothing AndAlso Quote.Vehicles(VehicleNumber).BodyTypeId <> "" Then
                    '    If Quote.Vehicles(VehicleNumber).BodyTypeId <> VehicleBodyType.bodyType_RecTrailer AndAlso Quote.Vehicles(VehicleNumber).BodyTypeId <> VehicleBodyType.bodyType_OtherTrailer Then
                    '        'If Quote.Vehicles(VehicleNumber).ComprehensiveCoverageOnly Then
                    '        If ToggleCompOnlyPanel Then
                    '            divPolicyCoverage.Attributes.Add("style", "display:none;")
                    '            dvCompOnlyPopup.Attributes.Add("style", "display:block;")
                    '        Else
                    '            divPolicyCoverage.Attributes.Add("style", "display:block;")
                    '            dvCompOnlyPopup.Attributes.Add("style", "display:none;")
                    '        End If
                    '    Else
                    '        divPolicyCoverage.Attributes.Add("style", "display:none;")
                    '        dvCompOnlyPopup.Attributes.Add("style", "display:block;")
                    '    End If
                    'End If

                    chkAutoEnhance.Checked = Quote.HasBusinessMasterEnhancement
                    chkAutoPlusEnhance.Checked = Quote.HasAutoPlusEnhancement

                    chkSelectMarketCredit.Checked = Quote.SelectMarketCredit
                    chkMultiPolicyDiscount.Checked = Quote.AutoHome
                End If
            End If
            'Parachute
            Me.divParachuteDiscounts.Visible = VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote)
            'Added 10/8/18 for multi state MLW
            Select Case (Quote.QuickQuoteState)
                Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                    lblAutoHomeDiscount.Text = "Related Homeowners/Farmowners Quote/Policy"
                    tr_RelatedHomeFarm.Visible = VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote)
                    Dim quotePolicyNumber As String = ""
                    'If SubQuoteFirst IsNot Nothing AndAlso SubQuoteFirst.PolicyUnderwritings IsNot Nothing AndAlso SubQuoteFirst.PolicyUnderwritings.Count > 0 Then
                    If Quote.PolicyUnderwritings IsNot Nothing AndAlso Quote.PolicyUnderwritings.Count > 0 Then
                        For Each item In SubQuoteFirst.PolicyUnderwritings
                            If item.PolicyUnderwritingCodeId = "9290" AndAlso item.PolicyUnderwritingAnswer = "1" Then
                                quotePolicyNumber = item.PolicyUnderwritingExtraAnswer
                                Exit For
                            End If
                        Next
                    End If
                    txtMoreInfo.Text = quotePolicyNumber
                Case Else
                    lblAutoHomeDiscount.Text = "Home/Auto discount (HOM or FAR w/primary residence)"
                    tr_RelatedHomeFarm.Visible = False
            End Select
            ' Bug 29563 commented out logic to hide the Market Credit question if the effective date is after the parachute start date per instructions in the bug MGB 3/28/19
            'Me.dvMarketCredit.Visible = Not VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote)
            Me.dvMultiPolicy.Visible = Not VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote)
            Me.dvPriorBi.Visible = VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote)
            ' can't control Comp Raters so can't garantee that priorbi is set just because it came back from prefill - so as backup always enable if no value is set
            Me.ddPriorBiLimit.Enabled = IFM.VR.Common.Helpers.PPA.PriorBiHelper.HasPrefillPriorBi(Me.Quote.PolicyId) = False Or Quote.PriorBodilyInjuryLimitId.TryToGetInt32() = 0
            ddPriorBiLimit.SetFromValue(Quote.PriorBodilyInjuryLimitId)
            chkAutoHomeDiscount_Parachute.Checked = Quote.AutoHome
            WebHelper_Personal.SetdropDownFromValue(ddMultiLineDiscount_Parachute, Quote.MultiLineDiscount)
        End If
    End Sub

    Private Sub CheckPolicyBoxes(vehicle As QuickQuoteVehicle)
        If Me.Quote IsNot Nothing Then
            'Determines if the Market Credit check box is displayed
            P1OverForty = False
            P2OverForty = False
            Dim p As QuickQuote.CommonObjects.QuickQuotePolicyholder = Me.Quote.Policyholder
            Dim p2 As QuickQuote.CommonObjects.QuickQuotePolicyholder = Me.Quote.Policyholder2
            ChildDriver = False

            ' MarketCredit is hidden if any child drivers are covered on this policy no matter where they actually live
            If Me.Quote.Drivers IsNot Nothing Then
                For Each driver As QuickQuote.CommonObjects.QuickQuoteDriver In Me.Quote.Drivers
                    If driver.RelationshipTypeId = DriverRelationshipType.PolicyholderChild.ToString() Then
                        ' NOTE: if you ever want this section of code (child-check) to work correctly, you'll need to replace the above line with the one
                        '       below.  Sherry T wanted to leave this code in place so that Diamond will remove the coverage automatically.  
                        '       MGB 3/28/19 Bug 29563
                        'If driver.RelationshipTypeId = Int(DriverRelationshipType.PolicyholderChild).ToString() Then
                        ChildDriver = True
                        chkSelectMarketCredit.Checked = False
                        dvMarketCredit.Attributes.Add("style", "display:none;")
                        Exit For
                    Else
                        ChildDriver = False
                    End If
                Next
            End If

            If Not ChildDriver Then
                ' Check Age of Policyholder 1
                If p IsNot Nothing Then
                    If p.Name.BirthDate = "" Or p.Name.BirthDate Is Nothing Then
                        Return
                    End If

                    Try
                        Dim birthDate As Date = CDate(p.Name.BirthDate)

                        Dim span As TimeSpan = IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().Subtract(birthDate)

                        If ((span.Duration().Days / 365) >= 40) Then
                            dvMarketCredit.Attributes.Add("style", "display:block;")
                            P1OverForty = True
                        Else
                            chkSelectMarketCredit.Checked = False
                            dvMarketCredit.Attributes.Add("style", "display:none;")
                            P1OverForty = False
                        End If
                    Catch ex As Exception

                    End Try
                End If

                If Not P1OverForty Then
                    ' Check Age of Policyholder 2
                    If p2 IsNot Nothing Then
                        If p2.Name.BirthDate = "" Or p2.Name.BirthDate Is Nothing Then
                            Return
                        End If

                        Try
                            Dim birthDate As Date = CDate(p2.Name.BirthDate)

                            Dim span As TimeSpan = IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().Subtract(birthDate)

                            If ((span.Duration().Days / 365) >= 40) Then
                                dvMarketCredit.Attributes.Add("style", "display:block;")
                                P2OverForty = True
                            Else
                                chkSelectMarketCredit.Checked = False
                                dvMarketCredit.Attributes.Add("style", "display:none;")
                                P2OverForty = False
                            End If
                        Catch ex As Exception

                        End Try
                    End If
                End If
            End If
        End If
    End Sub

    '
    ' If this is a new quote or no coverages have been saved yet, then the default values will display
    '
    Private Sub SetDefaultValues(vehicle As QuickQuoteVehicle)
        Dim setDefaults As Boolean = True

        If vehicle Is Nothing Then
            setDefaults = True
        ElseIf vehicle IsNot Nothing And ((ddLiabType.SelectedValue = "SPLIT LIMIT" And vehicle.BodilyInjuryLiabilityLimitId = "") Or (ddLiabType.SelectedValue = "CSL" And vehicle.UninsuredCombinedSingleLimitId = "")) And Not vehicle.ComprehensiveCoverageOnly Then 'Checks to see if any coverages have been saved yet
            setDefaults = True
        Else
            setDefaults = False
        End If

        If setDefaults Then
            SetPolicyDefaults()
        End If

        SetVisible(ddLiabType.SelectedValue)
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = "Policy Coverage"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        'Updated 10/8/18 for multi state MLW - added chkAutoHomeDiscount_Parachute.Checked, txtMoreInfo.Text
        Dim valList = PolicyCoverageValidator.ValidatePPAPolicyCoverage(Quote, ddLiabType.SelectedValue, ddSingleLimitLib.SelectedValue, ddUmUimSSl.SelectedValue, ddUMCSL.SelectedValue, ddUmPd.SelectedValue,
                                                                        ddUmPdDeductible.SelectedValue, ddUmUmiBi.SelectedValue, ddUMBI.SelectedValue, ddBodilyInjury.SelectedValue, ddPropertyDamage.SelectedValue,
                                                                        valArgs.ValidationType, hiddenCompOnly.Value, chkAutoEnhance.Checked, chkAutoPlusEnhance.Checked, chkAutoHomeDiscount_Parachute.Checked,
                                                                        txtMoreInfo.Text)

        If valList.Any() Then
            For Each v In valList
                ' *********************
                ' Base Policy Coverages
                ' *********************
                Select Case v.FieldId
                    Case PolicyCoverageValidator.MinLiab
                        ValidationHelper.Val_BindValidationItemToControl(ddSingleLimitLib, v, accordList)
                    Case PolicyCoverageValidator.CSLExceedLimit
                        ValidationHelper.Val_BindValidationItemToControl(ddUmUimSSl, v, accordList)
                    Case PolicyCoverageValidator.CSLExceedLimitStateExpansion
                        'Added 9/27/18 for multi state MLW
                        ValidationHelper.Val_BindValidationItemToControl(ddUMCSL, v, accordList)
                    Case PolicyCoverageValidator.UMPDDeductRequired
                        ValidationHelper.Val_BindValidationItemToControl(ddUmPdDeductible, v, accordList)
                    Case PolicyCoverageValidator.MinBI
                        ValidationHelper.Val_BindValidationItemToControl(ddBodilyInjury, v, accordList)
                    Case PolicyCoverageValidator.ExceedBI
                        ValidationHelper.Val_BindValidationItemToControl(ddUmUmiBi, v, accordList)
                    Case PolicyCoverageValidator.ExceedBIStateExpansion
                        'Added 9/27/18 for multi state MLW
                        ValidationHelper.Val_BindValidationItemToControl(ddUMBI, v, accordList)
                    Case PolicyCoverageValidator.UMBIRequired
                        ValidationHelper.Val_BindValidationItemToControl(ddUmUmiBi, v, accordList)
                    Case PolicyCoverageValidator.UMPDRequired
                        ValidationHelper.Val_BindValidationItemToControl(ddUmPd, v, accordList)
                    Case PolicyCoverageValidator.UMPDDeductRequired
                        ValidationHelper.Val_BindValidationItemToControl(ddUmPdDeductible, v, accordList)
                    Case PolicyCoverageValidator.ExceedPD
                        ValidationHelper.Val_BindValidationItemToControl(ddUmPd, v, accordList)
                    Case PolicyCoverageValidator.NotEligibleForAutoEnhance
                        ValidationHelper.AddError(v.Message)
                    Case PolicyCoverageValidator.AutoEnhanceRequiresGreaterTransportation
                        ValidationHelper.AddError(v.Message)
                    Case PolicyCoverageValidator.AutoPlusEnhanceAndAutoEnhanceCantBothBeSelected
                        ValidationHelper.AddError(v.Message)
                    Case PolicyCoverageValidator.PropertyLimit
                        ValidationHelper.Val_BindValidationItemToControl(ddPropertyDamage, v, accordList)
                    Case PolicyCoverageValidator.PrioBiLimit 'Parachute
                        ValidationHelper.Val_BindValidationItemToControl(ddPriorBiLimit, v, accordList)
                    Case PolicyCoverageValidator.QuotePolicyInfo 'Added 10/8/18 for multi state MLW
                        ValidationHelper.Val_BindValidationItemToControl(txtMoreInfo, v, accordList)
                End Select
            Next
        End If

        ' If Prior BI Limit is 0/0 show a validation warning - Bug 29097
        If VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote) Then
            ' The id for 0/0 is 410
            If ddPriorBiLimit.SelectedValue = "410" Then
                Me.ValidationHelper.AddWarning("The Prior BI field has a 0 value.  Please update if this is incorrect.")
            End If
        End If

        ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean

        If Quote IsNot Nothing Then
            If Not isAutoPlusEnhancementAvailable Then
                If chkAutoPlusEnhance.Checked = True Then
                    ValidationHelper.AddWarning("Auto Plus Enhancement Endorsement requires the effective date to be on or after " & AutoPlusEnhancement_EffectiveDate() & ". Auto Plus Enhancement Endorsement has been removed.")
                    chkAutoPlusEnhance.Checked = False
                End If
            End If

            SaveChildControls()

            Quote.AutoHome = chkMultiPolicyDiscount.Checked
            Quote.SelectMarketCredit = chkSelectMarketCredit.Checked

            If Quote.Vehicles.IsLoaded Then
                If Quote.Vehicles.FindAll(Function(p) p.BodyTypeId.TryToGetInt32 <> VehicleBodyType.bodyType_RecTrailer AndAlso p.BodyTypeId.TryToGetInt32 <> VehicleBodyType.bodyType_OtherTrailer AndAlso (Not p.ComprehensiveCoverageOnly)).Count > 0 Then
                    Quote.HasBusinessMasterEnhancement = chkAutoEnhance.Checked
                Else
                    Quote.HasBusinessMasterEnhancement = False
                End If
            End If


            Quote.HasAutoPlusEnhancement = chkAutoPlusEnhance.Checked

            'Parachute
            If VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote) Then

                Quote.PriorBodilyInjuryLimitId = ddPriorBiLimit.SelectedValue
                Quote.AutoHome = chkAutoHomeDiscount_Parachute.Checked
                'Added 10/8/18 for multi state MLW, Updated 1/17/2022 for OH task 66101 MLW
                If Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois OrElse Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                    Dim quotePolicyNumber As String = txtMoreInfo.Text
                    If Quote.PolicyUnderwritings IsNot Nothing AndAlso Quote.PolicyUnderwritings.Count > 0 Then
                        For Each item In Quote.PolicyUnderwritings
                            If item.PolicyUnderwritingCodeId = "9290" Then
                                If chkAutoHomeDiscount_Parachute.Checked = True Then
                                    item.PolicyUnderwritingAnswer = "1"
                                    item.PolicyUnderwritingExtraAnswer = quotePolicyNumber
                                Else
                                    item.PolicyUnderwritingAnswerTypeId = "0"
                                    item.PolicyUnderwritingAnswer = "-1"
                                End If
                                Exit For
                            End If
                        Next
                    End If
                End If
                Quote.MultiLineDiscount = ddMultiLineDiscount_Parachute.SelectedValue
            End If

            'SaveChildControls()
            Return True
        End If

        Return False
    End Function

    '
    ' Resets Policy Coverage back to defaults for the currently selected Policy Plan & Liability Type
    '
    Public Overrides Sub ClearControl()
        'MyBase.ClearControl()
        If Quote IsNot Nothing Then
            'Updated 4/4/2022 for bug 68773 MLW
            ddLiabType.SelectedIndex = 0
            'ddLiabType.SelectedValue = AutoLiabilityType.SplitLimit.ToString
            SetPolicyDefaults()
            PopulateChildControls()
        End If
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click ', btnSubmit.Click
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        Populate()
    End Sub

    Private Sub SaveAllCoverage(state As Boolean) Handles ctlCoverage_PPA_Vehicle_List.SaveAllCoverages
        RaiseEvent SaveAllCoverages(state)
    End Sub

    Private Sub LoadLiabilityDropDowns()
        ddLiabType.Items.Clear()
        'Load Liability Type DropDown
        ddLiabType.Items.Insert(0, New ListItem("SPLIT LIMIT", "0"))
        ddLiabType.Items.Insert(1, New ListItem("CSL", "1"))

        ddLiabType.SelectedValue = "0"
    End Sub

    Private Sub SetPolicyDefaults()
        'Updated 4/4/2022 for bug 68773 MLW
        'If ddLiabType.SelectedValue = AutoLiabilityType.CSL.ToString Then
        If ddLiabType.SelectedValue = 1 Then '1 CSL
            WebHelper_Personal.SetdropDownFromValue(Me.ddBodilyInjury, "0")
            WebHelper_Personal.SetdropDownFromValue(Me.ddPropertyDamage, "0")
            WebHelper_Personal.SetdropDownFromValue(Me.ddSingleLimitLib, "34") 'Updated 4/4/2022 for bug 68773 MLW
            'Updated 9/27/18 for multi state MLW
            Select Case (Quote.QuickQuoteState)
                Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Added 1/17/2022 for OH task 66101 MLW
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUMCSL, "34")
                    txtUIMCSL.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredCombinedSingleLimitId, Quote.QuickQuoteState, ddUMCSL.SelectedValue)
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUMBI, "0")
                    txtUIMBI.Text = "N/A"
                Case Else
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUmUimSSl, "34") 'Updated 4/4/2022 for bug 68773 MLW
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUmUmiBi, "0")
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUmPd, "0")
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUmPdDeductible, "120") 'Updated 4/4/2022 for bug 68773 MLW
            End Select
        Else
            'Split Limit
            WebHelper_Personal.SetdropDownFromValue(Me.ddBodilyInjury, "4")
            WebHelper_Personal.SetdropDownFromValue(Me.ddPropertyDamage, "10")
            WebHelper_Personal.SetdropDownFromValue(Me.ddSingleLimitLib, "0")
            'Updated 10/2/18 for multi state MLW 'Updated 4/4/2022 for bug 68773 MLW
            Select Case (Quote.QuickQuoteState)
                Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUMCSL, "0")
                    txtUIMCSL.Text = "N/A"
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUMBI, "4") 'Updated 4/4/2022 for bug 68773 MLW
                    txtUIMBI.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, Quote.QuickQuoteState, ddUMBI.SelectedValue)
                Case QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUMCSL, "0")
                    txtUIMCSL.Text = "N/A"
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUMBI, "4")
                    txtUIMBI.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, Quote.QuickQuoteState, ddUMBI.SelectedValue)
                Case Else
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUmUmiBi, "4")
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUmUimSSl, "0")
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUmPd, "10")
                    WebHelper_Personal.SetdropDownFromValue(Me.ddUmPdDeductible, "120")
            End Select
        End If

        WebHelper_Personal.SetdropDownFromValue(Me.ddmedicalPayments, "173")
        WebHelper_Personal.SetdropDownFromValue(Me.ddPriorBiLimit, "0") 'Added 1/19/2022 for OH task 66101 MLW

        If Quote IsNot Nothing Then
            If Me.Quote.Vehicles IsNot Nothing Then
                'Removed 4/4/2022 for bug 68773 MLW
                'If isAutoPlusEnhancementAvailable = True Then
                '    Me.chkAutoEnhance.Checked = False
                '    Me.chkAutoEnhance.Enabled = True
                '    Me.chkAutoPlusEnhance.Checked = True
                '    Me.chkAutoPlusEnhance.Enabled = True
                'Else
                Me.chkAutoEnhance.Checked = True
                Me.chkAutoEnhance.Enabled = True
                Me.chkAutoPlusEnhance.Checked = False
                Me.chkAutoPlusEnhance.Enabled = False
                'End If
            End If
        End If

        Me.chkMultiPolicyDiscount.Checked = False
        Me.chkSelectMarketCredit.Checked = False

        'Added 4/4/2022 for bug 68773 MLW
        WebHelper_Personal.SetdropDownFromValue(Me.ddMultiLineDiscount_Parachute, "0")

        SetVisible(ddLiabType.SelectedValue)
    End Sub

    Public Sub SetVisible(liabtype As String) Handles ctlCoverage_PPA_Vehicle_List.ToggleVehPolicyCoverage
        If liabtype <> "" Then
            If liabtype.TryToGetInt32 = AutoLiabilityType.CSL Then
                'Added 9/26/18 for multi state MLW
                Select Case (Quote.QuickQuoteState)
                    Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                        dvR1C1.Attributes.Add("style", "display:none;")
                        dvR2C1.Attributes.Add("style", "display:block;")
                        dvR1C2.Attributes.Add("style", "display:none;")
                        dvR1C2SE.Attributes.Add("style", "display:block;")
                        dvR2C2.Attributes.Add("style", "display:none;")
                        dvR2C2SE.Attributes.Add("style", "display:none;")
                        dvR3C2.Attributes.Add("style", "display:none;")
                        dvR4C2.Attributes.Add("style", "display:none;")
                    Case Else
                        dvR1C1.Attributes.Add("style", "display:none;")
                        dvR2C1.Attributes.Add("style", "display:block;")
                        dvR1C2.Attributes.Add("style", "display:block;")
                        dvR1C2SE.Attributes.Add("style", "display:none;")
                        dvR2C2.Attributes.Add("style", "display:none;")
                        dvR2C2SE.Attributes.Add("style", "display:none;")
                        dvR3C2.Attributes.Add("style", "display:none;")
                End Select
                'dvR1C1.Attributes.Add("style", "display:none;")
                'dvR2C1.Attributes.Add("style", "display:block;")
                'dvR1C2.Attributes.Add("style", "display:block;")
                'dvR2C2.Attributes.Add("style", "display:none;")
                'dvR3C2.Attributes.Add("style", "display:none;")
            Else
                dvR1C1.Attributes.Add("style", "display:block;")
                dvR2C1.Attributes.Add("style", "display:none;")
                dvR1C2.Attributes.Add("style", "display:none;")
                dvR1C2SE.Attributes.Add("style", "display:none;") 'Added 9/26/18 for multi state MLW
                'dvR2C2.Attributes.Add("style", "display:block;") 'moved to below for multi state 9/26/18 MLW
                'Added 9/26/18 for multi state MLW
                Select Case (Quote.QuickQuoteState)
                    Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                        dvR2C2.Attributes.Add("style", "display:none;")
                        dvR2C2SE.Attributes.Add("style", "display:block;")
                        dvR4C2.Attributes.Add("style", "display:none;")
                        dvR3C2.Attributes.Add("style", "display:none;")
                    Case Else
                        dvR2C2.Attributes.Add("style", "display:block;")
                        dvR2C2SE.Attributes.Add("style", "display:none;")
                        dvR4C2.Attributes.Add("style", "display:block;") 'Added 9/26/18 for multi state MLW
                        dvR3C2.Attributes.Add("style", "display:block;")
                End Select
            End If
        Else
            dvR1C1.Attributes.Add("style", "display:block;")
            dvR2C1.Attributes.Add("style", "display:none;")
            dvR1C2.Attributes.Add("style", "display:none;")
            dvR1C2SE.Attributes.Add("style", "display:none;") 'Added 9/26/18 for multi state MLW
            dvR2C2.Attributes.Add("style", "display:block;")
            dvR2C2SE.Attributes.Add("style", "display:none;") 'Added 9/26/18 for multi state MLW
            dvR3C2.Attributes.Add("style", "display:block;")
            dvR4C2.Attributes.Add("style", "display:block;") 'Added 9/26/18 for multi state MLW
        End If
    End Sub

    Protected Sub lnkBtnClear_Click(sender As Object, e As EventArgs) Handles lnkBtnClear.Click
        ClearControl()
    End Sub

    'Protected Sub lnkAutoEnhance_Click(sender As Object, e As EventArgs) Handles lnkAutoEnhance.Click
    '    'Response.Redirect("vrHelpMe.aspx?p=VR3Auto&s=coverages")
    'End Sub

    Private Sub RateAutoQuote() Handles ctlCoverage_PPA_Vehicle_List.QuoteRateRequested
        RaiseEvent QuoteRateRequested()
    End Sub

    Public Sub SetVehiclePolicyValues(vehCovList As String) Handles ctlCoverage_PPA_Vehicle_List.SetVehCovList
        hiddenVehCovPlanList.Value = vehCovList
    End Sub

    Private Sub ctlCoverage_PPA_Vehicle_List_SaveRequested(args As VrControlBaseSaveEventArgs) Handles ctlCoverage_PPA_Vehicle_List.SaveRequested

    End Sub

    Private Sub ctlCoverage_PPA_Vehicle_List_RequestPageRefresh() Handles ctlCoverage_PPA_Vehicle_List.RequestPageRefresh
        Me.Populate()
    End Sub
End Class