Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
Imports IFM.VR.Web.ENUMHelper
Imports IFM.PrimativeExtensions
Imports IFM.Common.InputValidation.InputHelpers

Public Class ctlCoverage_PPA_VehicleSpecific
    Inherits VRControlBase

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW

    'Updated 10/2/18 for multi state MLW - added umCSL and umBI
    Public Event SetVehPolicy(vehCovPlan As String, vehicleNum As Integer, bodilyInj As String, propDamage As String, ssl As String, medPay As String, umumiSSL As String, umCSL As String,
                              umumiBI As String, umBI As String, umPD As String, umpdDeduct As String, autoEnhance As Boolean, multiPolicy As Boolean, marketCredit As Boolean, vehPolCov As String)
    Public Event SaveVehPolicy(ByRef vehicle As QuickQuoteVehicle, hiddenVehCovPlan As String)

    Public Event RequestPageRefresh()

    Public Event SavePreviousVehicle(vehicleNum As Integer)

    Private _compOnlyWarningShown As Boolean = False

    Private ReadOnly Property MotorcycleCustomEquipmentTotalDictionaryName As String
        Get
            Return QuoteId & "_" & "MCCustomEquipTotal_" & VehicleNumber
        End Get
    End Property

    Private _policyPlan As String

    Public Property VehicleIndex As Int32
        Get
            Return ViewState.GetInt32("vs_vehicleNum", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_vehicleNum") = value
            Me.ctlCoverage_PPA_ScheduledItemsList.VehicleIndex = Me.VehicleIndex
        End Set

    End Property
    Public Property policyPlan() As String
        Get
            Return _policyPlan
        End Get
        Set(ByVal value As String)
            _policyPlan = value
        End Set
    End Property

    Public Property IsVehicleNumber As Boolean
        Get
            If ViewState("vs_vehicleNum") Is Nothing Then
                ViewState("vs_vehicleNum") = False
            End If
            Return CInt(ViewState("vs_vehicleNum"))
        End Get
        Set(value As Boolean)
            ViewState("vs_vehicleNum") = value
        End Set
    End Property

    Public Property VehicleNumber As Int32
        Get
            If ViewState("vs_vehicleNum") Is Nothing Then
                ViewState("vs_vehicleNum") = 0
            End If
            Return CInt(ViewState("vs_vehicleNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_vehicleNum") = value
            Me.ctlCoverage_PPA_ScheduledItemsList.VehicleIndex = value
            'Added so vehicle will increment properly 76467 DLG 9/21/22
        End Set
    End Property

    'Private Shared selectLiabType As String
    Public Property SelectedLiabilityType() As String
        Get
            Return ViewState("vs_LiabType")
        End Get
        Set(ByVal Value As String)
            ViewState("vs_LiabType") = Value
        End Set
    End Property

    'Private _selectedPolicyPlan As String
    Public Property SelectedPolicyPlan() As String
        Get
            Return ViewState("vs_PolicyPlan")
        End Get
        Set(ByVal value As String)
            ViewState("vs_PolicyPlan") = value
        End Set
    End Property

    Public Property VehicleYear() As String
        Get
            Return hiddenVehicleYear.Value
        End Get
        Set(ByVal value As String)
            hiddenVehicleYear.Value = value
        End Set
    End Property

    Public Property VehicleType() As String
        Get
            Return hiddenVehicleType.Value
        End Get
        Set(ByVal value As String)
            hiddenVehicleType.Value = value
        End Set
    End Property

    Public Property VehicleUse() As String
        Get
            Return hiddenVehicleUse.Value
        End Get
        Set(ByVal value As String)
            hiddenVehicleUse.Value = value
        End Set
    End Property

    Public Property NamedNonOwned() As String
        Get
            Return hiddenNNO.Value
        End Get
        Set(ByVal value As String)
            hiddenNNO.Value = value
        End Set
    End Property

    Public Property DupZeroVehicle() As Boolean
        Get
            Return Session("sess_DupZeroVehicle")
        End Get
        Set(ByVal value As Boolean)
            Session("sess_DupZeroVehicle") = value
        End Set
    End Property

    Private ReadOnly Property ScheduledItemsTotalAmount() As Integer
        Get
            Dim tot As Integer = 0
            If Quote IsNot Nothing AndAlso Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
                Dim veh As QuickQuote.CommonObjects.QuickQuoteVehicle = Quote.Vehicles(VehicleNumber)
                If veh.ScheduledItems IsNot Nothing AndAlso veh.ScheduledItems.Count > 0 Then
                    For Each SI As QuickQuote.CommonObjects.QuickQuoteScheduledItem In veh.ScheduledItems
                        If Not IsNullEmptyorWhitespace(SI.Amount) Then
                            tot += QQHelper.IntegerForString(SI.Amount)
                        End If
                    Next
                End If
            End If
            Return tot
        End Get
    End Property


    Protected ReadOnly Property isAutoPlusEnhancementEndorsementChecked As Boolean
        Get
            Return DirectCast(Me.ParentVrControl.ParentVrControl, ctlCoverage_PPA).isAutoPlusEnhancementChecked
        End Get
    End Property

    Protected ReadOnly Property AutoPlusEnhancementClientId As String
        Get
            Return DirectCast(Me.ParentVrControl.ParentVrControl, ctlCoverage_PPA).chkAutoPlusEnhancementClientId
        End Get
    End Property

    Protected ReadOnly Property AutoEnhancementClientId As String
        Get
            Return DirectCast(Me.ParentVrControl.ParentVrControl, ctlCoverage_PPA).chkAutoEnhancementClientId
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStaticData()

            Try
                If Me.Quote IsNot Nothing Then
                    If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Any() Then
                        Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Me.Quote.Vehicles(Me.VehicleNumber)
                        OptionalCoverageVisibility(vehicle)
                        Populate()
                    End If
                End If
            Catch ex As Exception

            End Try
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.StopEventPropagation(lnkBtnClear.ClientID, False)
        VRScript.StopEventPropagation(lnkBtnSave.ClientID, False)

        ' Vehicle Plan
        'Updated 9/27/18 for multi state MLW - added dvUMPD, chkUMPD, txtUMPDLimit
        Dim collisionAndUMPDAvail As String = "False"
        Dim UMPDLimitsAvail As String = "False"
        If Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois Then
            collisionAndUMPDAvail = IFM.VR.Common.Helpers.PPA.CollisionAndUMPD.IsILCollisionAndUMPDAvailable(quote)
            UMPDLimitsAvail = IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(quote).ToString()
        End If 

        Dim IsTransportationAvailable As String = IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote).ToString
        Dim scriptVehiclePolicy As String = "ToggleVehiclePolicy(this, """ + Quote.State + """, """ + dvComp.ClientID + """, """ + dvColl.ClientID + """, """ + dvTowing.ClientID + """, """ + dvTransportation.ClientID +
            """, """ + dvTravel.ClientID + """, """ + dvRadio.ClientID + """, """ + dvAVEquip.ClientID + """, """ + dvMedia.ClientID + """, """ + dvCostNew.ClientID + """, """ + dvLoanLease.ClientID + """, """ + dvUMPD.ClientID + """, """ + dvPolution.ClientID +
            """, """ + dvMotorcycle.ClientID + """, """ + dvDisclaim.ClientID + """, """ + ddComprehensive.ClientID + """, """ + ddCollision.ClientID + """, """ + ddTowing.ClientID + """, """ + ddTransportation.ClientID +
            """, """ + chkInterruptionOfTravel.ClientID + """, """ + ddRadio.ClientID + """, """ + ddAudioVisual.ClientID + """, """ + ddMedia.ClientID + """, """ + chkAutoLoanLease.ClientID + """, """ + chkUMPD.ClientID + """, """ + txtUMPDLimit.ClientID +
            """, """ + chkPollution.ClientID + """, """ + txtMotorEquip.ClientID + """, """ + VehicleYear.ToString() + """, """ + hiddenVehicleType.ClientID + """, """ + hiddenVehicleUse.ClientID +
            """, """ + hiddenNNO.ClientID + """, """ + hiddenVehicleCoveragePlan.ClientID + """, """ + dvCompDisclaim.ClientID + """, """ + AutoPlusEnhancementClientId + """, """ + AutoEnhancementClientId + """,""" + collisionAndUMPDAvail + 
            """, """ + UMPDLimitsAvail + """,""" + ddUMPD.ClientID + """,""" + trUMPDLimitTB.ClientID + """,""" + trUMPDLimitDD.ClientID + """,""" + trUMPDLimitMsg.ClientID + """, """ + IsTransportationAvailable + """);"

        'CAH - 11/01/2017 - The Policy ID change after the binding would cause the call to fail (looking at the wrong id for change);
        ' -- Also, the script was being called on postback causing the hidden items to appear when unwanted because ddPolicy was
        ' -- set to page ('this' = page instead of 'this' = ddPolicy Element)
        'Me.VRScript.CreateJSBinding(Me.ddPolicy, ctlPageStartupScript.JsEventType.onchange, scriptVehiclePolicy, True)

        ddPolicy.Attributes.Add("onchange", scriptVehiclePolicy)
        ddPolicy.ID = "ddPolicy" + VehicleNumber.ToString()
        ddPolicy.ClientIDMode = UI.ClientIDMode.Static

        ' 8/26/19 for bug 32399 ZTS - CostNew onchange logic
        VRScript.CreateJSBinding(ddCollision, ctlPageStartupScript.JsEventType.onchange, "ToggleVehicleCostNewOnCoverages('" + ddCollision.ClientID + "','" + ddComprehensive.ClientID + "','" + dvCostNew.ClientID + "');")
        VRScript.CreateJSBinding(ddComprehensive, ctlPageStartupScript.JsEventType.onchange, "ToggleVehicleCostNewOnCoverages('" + ddCollision.ClientID + "','" + ddComprehensive.ClientID + "','" + dvCostNew.ClientID + "');")
        VRScript.AddScriptLine("ToggleVehicleCostNewOnCoverages('" + ddCollision.ClientID + "','" + ddComprehensive.ClientID + "','" + dvCostNew.ClientID + "');")
        ' 8/22/2023 Added for task WS-1339 BD
        Dim scriptAddTitleToTransportation As String = "addTitleToTransportation('" + ddTransportation.ClientID + "','" + IsTransportationAvailable + "');"
        VRScript.CreateJSBinding(ddTransportation, ctlPageStartupScript.JsEventType.onchange, scriptAddTitleToTransportation)
        VRScript.AddScriptLine(scriptAddTitleToTransportation)

        ' Comp Dropdownlist
        Dim scriptVehicleComp As String = "ToggleVehicleComp(this, """ + dvTransportation.ClientID + """, """ + ddTransportation.ClientID + """, """ + dvColl.ClientID + """, """ + ddCollision.ClientID +
            """, """ + dvTowing.ClientID + """, """ + ddTowing.ClientID + """, """ + dvRadio.ClientID + """, """ + ddRadio.ClientID + """, """ + dvAVEquip.ClientID + """, """ + ddAudioVisual.ClientID +
            """, """ + dvMedia.ClientID + """, """ + ddMedia.ClientID + """,  """ + chkInterruptionOfTravel.ClientID + """, """ + dvMotorcycle.ClientID + """, """ + txtMotorEquip.ClientID + """, """ + AutoPlusEnhancementClientId + """, """ + IsTransportationAvailable + """);"
        ddComprehensive.Attributes.Add("onchange", scriptVehicleComp)

        ' Sound Equipment Dropdownlist
        Dim scriptSoundEquipment As String = "UpdateSoundEquipList(this, """ + ddMedia.ClientID + """, """ + ddAudioVisual.ClientID + """);"
        ddRadio.Attributes.Add("onchange", scriptSoundEquipment)

        ' Electronic Equiupment Dropdownlist
        Dim scriptElectronicEquipment As String = "UpdateElectronicEquipList(this, """ + ddMedia.ClientID + """, """ + ddRadio.ClientID + """);"
        ddAudioVisual.Attributes.Add("onchange", scriptElectronicEquipment)

        'Added 9/27/18 for multi state MLW, Updated 1/17/2022 for OH task 66101 MLW
        If Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois OrElse Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
            Dim scriptCollisionToggle As String = "ToggleVehicleUMPDCollision(""" + ddCollision.ClientID + """,""" + chkUMPD.ClientID + """,""" + txtUMPDLimit.ClientID + """,""" + Quote.State + """,""" + collisionAndUMPDAvail + """, """ + UMPDLimitsAvail + """,""" + ddUMPD.ClientID + """,""" + trUMPDLimitTB.ClientID + """,""" + trUMPDLimitDD.ClientID + """,""" + trUMPDLimitMsg.ClientID + """);"
            ddCollision.Attributes.Add("onchange", scriptCollisionToggle)
            chkUMPD.Attributes.Add("onchange", scriptCollisionToggle)
        End If
        If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
            ddUMPD.Attributes.Add("onchange", "SetILUMPDLimits(""" & ddUMPD.ClientID & """);")
        End If

        pnlAccordHeader.ID = "pnlAccordHeader" + VehicleNumber.ToString()
        pnlAccordHeader.ClientIDMode = UI.ClientIDMode.Static

        txtMotorEquip.Attributes.Add("onfocus", "this.select()")
    End Sub

    Private Sub CheckVehicleAge(ByRef vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle)
        If vehicle IsNot Nothing Then
            If (Not vehicle.NonOwnedNamed) AndAlso
                vehicle.BodyTypeId <> VehicleBodyType.bodyType_RecTrailer AndAlso
                vehicle.BodyTypeId <> VehicleBodyType.bodyType_OtherTrailer AndAlso
                vehicle.BodyTypeId <> VehicleBodyType.bodyType_Motorcycle Then
                Dim todayDate As Long = DateTime.Now.Year

                If DateTime.Now.Month >= 10 Then
                    todayDate += 1
                End If
                Dim vYear As Int32 = 0

                If IsNumeric(vehicle.Year) Then
                    VehicleYear = vehicle.Year
                    If (todayDate - Integer.Parse(VehicleYear)) < 5 Then
                        dvLoanLease.Attributes.Add("style", "display:block;")
                    Else
                        dvLoanLease.Attributes.Add("style", "display:none;")
                        chkAutoLoanLease.Checked = False
                    End If
                Else
                    dvLoanLease.Attributes.Add("style", "display:none;")
                    chkAutoLoanLease.Checked = False
                End If
            Else
                dvLoanLease.Attributes.Add("style", "display:none;")
            End If
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
        If ddComprehensive.Items.Count = 0 Then
            Dim qqHelper As New QuickQuoteHelperClass
            qqHelper.LoadStaticDataOptionsDropDown(ddComprehensive, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.ComprehensiveDeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            qqHelper.LoadStaticDataOptionsDropDown(ddCollision, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.CollisionDeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            qqHelper.LoadStaticDataOptionsDropDown(ddTowing, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TowingAndLaborDeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            qqHelper.LoadStaticDataOptionsDropDown(ddTransportation, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TransportationExpenseLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            qqHelper.LoadStaticDataOptionsDropDown(ddMedia, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TapesAndRecordsLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)

            If IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote) Then
                If Me.ddTransportation.Items IsNot Nothing Then
                    Dim ItemsToRemove As New List(Of ListItem)
                    For Each item As ListItem In Me.ddTransportation.Items
                        If item.Value.TryToGetInt32 < 446 Then
                            ItemsToRemove.Add(item)
                        End If
                    Next
                    For Each LItem As ListItem In ItemsToRemove
                        Me.ddTransportation.Items.Remove(LItem)
                    Next
                    For Each item As ListItem In Me.ddTransportation.Items
                            item.Attributes("title") = item.Text
                    Next
                End If
            Else
                If Me.ddTransportation.Items IsNot Nothing Then
                    Dim ItemsToRemove As New List(Of ListItem)
                    For Each item As ListItem In Me.ddTransportation.Items
                        If item.Value.TryToGetInt32 > 445 Then
                            ItemsToRemove.Add(item)
                        End If
                    Next
                    For Each LItem As ListItem In ItemsToRemove
                        Me.ddTransportation.Items.Remove(LItem)
                    Next
                End If
            End If

            If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                qqHelper.LoadStaticDataOptionsDropDownForState(ddUMPD, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
                Dim removeNA As ListItem = ddUMPD.Items.FindByText("N/A")
                If removeNA IsNot Nothing Then
                    Me.ddUMPD.Items.Remove(removeNA)
                End If
            End If

            'LoadPolicyDropDowns()
            ' Load Policy Plan DropDown
            ddPolicy.Items.Clear()
            ddPolicy.Items.Insert(0, New ListItem("FULL COVERAGE", "0"))
            ddPolicy.Items.Insert(1, New ListItem("LIABILITY ONLY", "1"))
            ''ddPolicy.Items.Insert(2, New ListItem("COMP ONLY", "2"))

            ' On endorsements Comp Only is allowed on pre-existing vehicles that already have it
            If IsEndorsementRelated() AndAlso Quote.Vehicles(Me.VehicleNumber).ComprehensiveCoverageOnly Then
                ddPolicy.Items.Insert(2, New ListItem("PHYSICAL DAMAGE ONLY (PARKED CAR)", "2")) ' no longer valid with BUg 8418 - Matt A 3-27-17
                'ddPolicy.SelectedValue = "2"
            End If

            'Populate Radio Drop down
            ddRadio.Items.Insert(0, New ListItem("[1,000 INC]", "0"))
            ddRadio.Items.Insert(1, New ListItem("1,500", "1"))
            ddRadio.Items.Insert(2, New ListItem("2,000", "2"))
            ddRadio.Items.Insert(3, New ListItem("2,500", "3"))
            ddRadio.Items.Insert(4, New ListItem("3,000", "4"))

            ddRadio.SelectedValue = "0"

            'Populate A/V/D Equipment
            ddAudioVisual.Items.Insert(0, New ListItem("N/A", "0"))
            ddAudioVisual.Items.Insert(1, New ListItem("500", "1"))
            ddAudioVisual.Items.Insert(2, New ListItem("1,000", "2"))
            ddAudioVisual.Items.Insert(3, New ListItem("1,500", "3"))
            ddAudioVisual.Items.Insert(4, New ListItem("2,000", "4"))
            ddAudioVisual.Items.Insert(5, New ListItem("2,500", "5"))
            ddAudioVisual.Items.Insert(6, New ListItem("3,000", "6"))

            ' Added items here so they could be added & removed from the dropdown list
            ddMedia.Items.Insert(0, New ListItem("N/A", "0"))
            ddMedia.Items.Insert(1, New ListItem("200", "212"))
        End If
    End Sub

    Public Overrides Sub Populate()
        If Quote IsNot Nothing Then
            If Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Any() Then
                LoadStaticData()
                Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
                Dim prevVehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing

                Try
                    vehicle = Quote.Vehicles(Me.VehicleNumber)

                    If VehicleNumber = 0 Then
                        prevVehicle = Quote.Vehicles(Me.VehicleNumber)
                    Else
                        prevVehicle = Quote.Vehicles(Me.VehicleNumber - 1)
                    End If
                Catch ex As Exception

                End Try

                ''
                ' Vehicle Policy Level Coverages
                ''
                If IsEndorsementRelated() Then
                    ' On Endorsements we should allow comp only to remain on vehicles that already have it.
                    If vehicle.ComprehensiveCoverageOnly Then
                        ' Vehicle has comp only.  This is OK on endorsement transactions
                        ' Force the value for comp only in the dropdown
                        ddPolicy.SetFromValue_Force("2", "PHYSICAL DAMAGE ONLY (PARKED CAR)")
                        hiddenVehicleCoveragePlan.Value = ddPolicy.SelectedValue
                    End If
                Else
                    If vehicle.ComprehensiveCoverageOnly Then
                        'ddPolicy.SelectedValue = VehiclePolicyType.compOnly ' no longer valid with BUg 8418 - Matt A 3-27-17
                        ddPolicy.SelectedValue = VehiclePolicyType.fullCoverage
                        'hiddenVehicleCoveragePlan.Value = VehiclePolicyType.compOnly ' no longer valid with BUg 8418 - Matt A 3-27-17
                        hiddenVehicleCoveragePlan.Value = VehiclePolicyType.fullCoverage
                    End If
                End If

                Dim vehPolicyCoverage As String = "0"
                If vehicle.Liability_UM_UIM_LimitId <> "0" And vehicle.BodilyInjuryLiabilityLimitId = "0" Then
                    vehPolicyCoverage = "1"
                End If

                'Updated 10/2/18 for multi state MLW - added vehicle.UninsuredBodilyInjuryLimitId (umBI) and vehicle.UninsuredCombinedSingleLimitId (umCSL) for IL
                RaiseEvent SetVehPolicy(ddPolicy.SelectedValue, VehicleNumber, vehicle.BodilyInjuryLiabilityLimitId, vehicle.PropertyDamageLimitId, vehicle.Liability_UM_UIM_LimitId,
                                        vehicle.MedicalPaymentsLimitId, vehicle.UninsuredCombinedSingleLimitId, vehicle.UninsuredCombinedSingleLimitId,
                                        vehicle.UninsuredMotoristLiabilityLimitId, vehicle.UninsuredBodilyInjuryLimitId, vehicle.UninsuredMotoristPropertyDamageLimitId,
                                        vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId, Quote.HasBusinessMasterEnhancement, Quote.AutoHome, Quote.HasBusinessMasterEnhancement, vehPolicyCoverage)

                ''
                ' Vehicle Level Coverages
                ''

                'Hides copy previous vehicle button if there is no previous vehicle
                If VehicleNumber = 0 Or
                   (QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Rec. Trailer" Or
                   QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Other Trailer" Or
                   QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Motorcycle") Or
                   (QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, prevVehicle.BodyTypeId) = "Rec. Trailer" Or
                   QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, prevVehicle.BodyTypeId) = "Other Trailer" Or
                   QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, prevVehicle.BodyTypeId) = "Motorcycle") Then
                    btnCopyCoverage.Visible = False
                End If

                If vehicle IsNot Nothing Then
                    hiddenVehicleType.Value = vehicle.BodyTypeId
                    hiddenVehicleUse.Value = vehicle.VehicleUseTypeId
                    hiddenNNO.Value = vehicle.NonOwnedNamed

                    If ddPolicy.SelectedValue <> "" Then
                        ddPolicy.Items.Remove(ddPolicy.Items.FindByValue(ENUMHelper.VehiclePolicyType.namedNonOwner))
                        ddPolicy.Items.Remove(ddPolicy.Items.FindByValue(ENUMHelper.VehiclePolicyType.trailer))
                        ddPolicy.Enabled = True

                        If vehicle.NonOwnedNamed Then
                            ddPolicy.Items.Clear()
                            ddPolicy.Items.Insert(0, New ListItem("NAMED NON-OWNER", "3"))
                            ddPolicy.Enabled = False
                        Else
                            If QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Rec. Trailer" Or
                               QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Other Trailer" Then
                                ddPolicy.Items.Clear()
                                ddPolicy.Items.Insert(0, New ListItem("TRAILER", "4"))
                                ddPolicy.Enabled = False
                            End If
                        End If
                    End If

                    Dim vehVin As String = Nothing

                    ' Makes sure that vin has a value
                    If vehicle.Vin <> "" Then
                        vehVin = Right(vehicle.Vin, 4)
                    End If

                    If vehicle.Vin.ToLower.Trim().StartsWith("none") Then
                        vehVin = "NONE"
                    End If

                    Me.lblAccordHeader.Text = String.Format("Vehicle #{0} - {1} {2} {3} ...{4}", VehicleNumber + 1, vehicle.Year, vehicle.Make, vehicle.Model, vehVin)
                    If Me.lblAccordHeader.Text.Length > 65 Then
                        Me.lblAccordHeader.Text = Me.lblAccordHeader.Text.Substring(0, 60) + "..."
                    End If

                    If vehicle.ComprehensiveDeductibleLimitId = "0" AndAlso vehicle.CollisionDeductibleLimitId = "0" AndAlso vehicle.SoundEquipmentLimit = "0" AndAlso Not vehicle.NonOwnedNamed Then
                        ddPolicy.SelectedValue = "1"
                    End If

                    If IsEndorsementRelated() Then
                        ' ENDORSEMENTS
                        If vehicle.ComprehensiveCoverageOnly Then
                            'ddPolicy.SelectedValue = "2"
                            ddPolicy.SetFromValue_Force("2", "PHYSICAL DAMAGE ONLY (PARKED CAR)")
                        End If
                    Else
                        ' NEW BUSINESS
                        If vehicle.ComprehensiveCoverageOnly Then ' no longer valid with BUg 8418 - Matt A 3-27-17
                            ddPolicy.SelectedValue = VehiclePolicyType.fullCoverage
                            'ddPolicy.SelectedValue = "2" ' no longer valid with BUg 8418 - Matt A 3-27-17
                        End If
                    End If

                    If IsQuoteEndorsement() AndAlso Not IsQuoteReadOnly() AndAlso QQHelper.IsQuickQuoteVehicleNewToImage(vehicle, Quote) Then
                        ' New vehicles' Loan/Lease checkbox should be checked if the vehicle is new the image,
                        ' and 5 years or newer AND there are any AI's
                        Dim todayDate As Long = DateTime.Now.Year
                        If DateTime.Now.Month >= 10 Then
                            todayDate += 1
                        End If
                        Dim vYear As Int32 = 0
                        Dim VehicleYear As String = ""
                        If IsNumeric(vehicle.Year) Then
                            VehicleYear = vehicle.Year
                            If (todayDate - Integer.Parse(VehicleYear)) <= 5 Then
                                If vehicle.AdditionalInterests IsNot Nothing Then
                                    ' updated chkAutoLoanLease to use vehicle.HasAutoLoanOrLease instead of vehicle.AdditionalInterests, per bug WS-1383
                                    chkAutoLoanLease.Checked = vehicle.HasAutoLoanOrLease 'vehicle.AdditionalInterests.Any
                                Else
                                    chkAutoLoanLease.Checked = False
                                End If
                            Else
                                chkAutoLoanLease.Checked = False
                            End If
                        Else
                            chkAutoLoanLease.Checked = vehicle.HasAutoLoanOrLease
                        End If
                    Else
                        ' Existing vehicles set the loan/lease checkbox based on the vehicle value
                        Me.chkAutoLoanLease.Checked = vehicle.HasAutoLoanOrLease
                    End If

                    Me.chkPollution.Checked = vehicle.HasPollutionLiabilityBroadenedCoverage
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddComprehensive, vehicle.ComprehensiveDeductibleLimitId)
                    'Updated 5/11/2020 for task 45389 ZTS
                    'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddCollision, vehicle.CollisionDeductibleLimitId)
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddCollision, vehicle.CollisionDeductibleLimitId, QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.CollisionDeductibleLimitId, vehicle.CollisionDeductibleLimitId))
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddTowing, vehicle.TowingAndLaborDeductibleLimitId)
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddTransportation, vehicle.TransportationExpenseLimitId)
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddMedia, vehicle.TapesAndRecordsLimitId)
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddAudioVisual, vehicle.ElectronicEquipmentLimit) ' Changed from Value to Text 5-5-16
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddRadio, vehicle.SoundEquipmentLimit)

                    ' adding CostNew for bug 32399 - ZTS 8/28/19
                    If vehicle.CostNew.Trim() = "0" Or vehicle.CostNew.Trim() = "$0" Or vehicle.CostNew.Trim() = "$0.00" Then
                        Me.txtCostNew.Text = ""
                    Else
                        Me.txtCostNew.Text = TryToFormatAsCurrency(vehicle.CostNew, False)
                    End If

                    If vehicle.VehicleSymbols IsNot Nothing AndAlso vehicle.VehicleSymbols.Count > 1 Then
                        dvCostNew.Attributes.Add("style", "display:none;")
                    Else
                        If vehicle.ComprehensiveDeductibleLimitId = "0" AndAlso vehicle.CollisionDeductibleLimitId = "0" Then
                            dvCostNew.Attributes.Add("style", "display:none;")
                        Else
                            dvCostNew.Attributes.Add("style", "display:block;")
                        End If
                    End If

                    If QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Motorcycle" Then
                        chkInterruptionOfTravel.Checked = True

                        ' New code to use dev dictionary instead of session.  Task 60835 MGB 
                        If vehicle.ScheduledItems IsNot Nothing Then
                            If vehicle.ScheduledItems.Count > 0 Then
                                ' Scheduled items exist.  Get the amount sum for all scheduled items
                                ' and display it in the equipment field.
                                Dim motorEquipInt As Integer = ScheduledItemsTotalAmount()

                                txtMotorEquip.Text = motorEquipInt.ToString
                                QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, txtMotorEquip.Text)
                            Else
                                ' No scheduled items, display what we stored in the dev dictionary
                                ' Will be 0 or empty string if nothing is stored
                                Me.txtMotorEquip.Text = QQDevDictionary_GetItem(MotorcycleCustomEquipmentTotalDictionaryName)
                            End If
                        Else
                            ' Scheduled items is nothing.  Set equipment to 0
                            txtMotorEquip.Text = "0"
                            QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, txtMotorEquip.Text)
                        End If

                        If (IsQuoteEndorsement() OrElse IsQuoteReadOnly()) AndAlso IFM.Common.InputValidation.InputHelpers.TryToGetInt32(Me.txtMotorEquip.Text) > 0 Then
                            ctlCoverage_PPA_ScheduledItemsList.Visible = True
                            'Removed to properly increment vehicleindex in vehicle index prop above 76467 DLG 9/21/22
                            'Me.ctlCoverage_PPA_ScheduledItemsList.VehicleIndex = VehicleNumber
                        End If
                    End If
                    'Determines if disclaimer is displayed
                    If vehicle.NonOwnedNamed Then
                        dvDisclaim.Attributes.Add("style", "display:block;")
                        lnkBtnClear.Enabled = False
                    Else
                        dvDisclaim.Attributes.Add("style", "display:none;")
                        lnkBtnClear.Enabled = True
                    End If
                End If

                PopulateMediaDropDownList()
                OptionalCoverageVisibility(vehicle)
                Me.PopulateChildControls()
            End If
        End If
    End Sub

    Private Sub SetVehicleDefaults(vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle)
        If vehicle IsNot Nothing Then
            vehicle.ComprehensiveCoverageOnly = False

            If QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Rec. Trailer" Or
            QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Other Trailer" Then
                ddPolicy.SelectedValue = "4"
            Else
                If QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Motorcycle" Then
                    dvTravel.Attributes.Add("style", "display:block;")
                    chkInterruptionOfTravel.Checked = True
                    dvMotorcycle.Attributes.Add("style", "display:block;")
                Else
                    dvTravel.Attributes.Add("style", "display:none;")
                    chkInterruptionOfTravel.Checked = False
                    dvMotorcycle.Attributes.Add("style", "display:none;")
                End If

                dvTowing.Attributes.Add("style", "display:block;")
                vehicle.TowingAndLaborDeductibleLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TowingAndLaborDeductibleLimitId, "25", Me.Quote.LobType)
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddTowing, "25")

                If QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Motorcycle" Then
                    dvTransportation.Attributes.Add("style", "display:none;")
                    If IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote) Then
                        If Quote.HasAutoPlusEnhancement Then
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddTransportation, "40/1200")
                        Else
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddTransportation, "[40/1200 INC] 30 DAYS")
                        End If
                    Else
                        If Quote.HasAutoPlusEnhancement Then
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddTransportation, "40/1200")
                        Else
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddTransportation, "[20/600 INC]")
                        End If
                    End If
                    dvRadio.Attributes.Add("style", "display:none;")
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddRadio, "[1,000 INC]")
                    dvAVEquip.Attributes.Add("style", "display:none;")
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddAudioVisual, "N/A")
                Else
                    dvTransportation.Attributes.Add("style", "display:block;")
                    'ddTransportation.Enabled = True
                    dvTransportation.Disabled = False
                    If IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote) = False Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddTransportation, "30/900")
                    Else
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddTransportation, "[40/1200 INC] 30 DAYS")
                    End If
                    dvRadio.Attributes.Add("style", "display:block;")
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddRadio, "[1,000 INC]")
                    dvAVEquip.Attributes.Add("style", "display:block;")
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddAudioVisual, "N/A")
                    txtMotorEquip.Enabled = True
                End If
                'If Quote.HasBusinessMasterEnhancement Then
                'ddTransportation.Items.FindByText("")
                'End If

                dvMedia.Attributes.Add("style", "display:block;")
                If ddMedia.Items.Count > 2 Then
                    ddMedia.Items.RemoveAt(0)
                    ddMedia.Items.RemoveAt(1)
                Else
                    ddMedia.Items.RemoveAt(0)
                End If
                ddMedia.Items.Insert(0, New ListItem("N/A", "0"))
                ddMedia.Items.Insert(1, New ListItem("200", "212"))
                vehicle.TapesAndRecordsLimitId = "0"
            End If

            dvComp.Attributes.Add("style", "display:block;")
            vehicle.ComprehensiveDeductibleLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.ComprehensiveDeductibleLimitId, "500", Me.Quote.LobType)
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddComprehensive, "500")
            dvColl.Attributes.Add("style", "display:block;")
            vehicle.CollisionDeductibleLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.CollisionDeductibleLimitId, "500", Me.Quote.LobType)
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddCollision, "500")

            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddRadio, "[1,000 INC]")
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddAudioVisual, "N/A")
            vehicle.ComprehensiveCoverageOnly = False
            dvTransportation.Disabled = False
            CheckVehicleAge(vehicle)

            dvCostNew.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Private Sub OptionalCoverageVisibility(ByRef vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle)
        dvComp.Attributes.Add("style", "display:none;")
        dvColl.Attributes.Add("style", "display:none;")
        dvTowing.Attributes.Add("style", "display:none;")
        dvTransportation.Attributes.Add("style", "display:none;")
        dvTravel.Attributes.Add("style", "display:none;")
        dvRadio.Attributes.Add("style", "display:none;")
        dvAVEquip.Attributes.Add("style", "display:none;")
        dvMedia.Attributes.Add("style", "display:none;")
        dvCostNew.Attributes.Add("style", "display:none;")
        dvLoanLease.Attributes.Add("style", "display:none;")
        dvUMPD.Attributes.Add("style", "display:none;") 'Added 9/28/18 For multi state MLW
        dvPolution.Attributes.Add("style", "display:none;")
        dvMotorcycle.Attributes.Add("style", "display:none;")
        trUMPDLimitDD.Attributes.Add("style", "display:none;")
        trUMPDLimitMsg.Attributes.Add("style", "display:none;")
        If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
            trUMPDLimitTB.Attributes.Add("style", "display:none;")
        End If

        If IsEndorsementRelated() = False AndAlso vehicle.ComprehensiveCoverageOnly AndAlso _compOnlyWarningShown = False Then
            _compOnlyWarningShown = True
            Me.ValidationHelper.AddWarning($"Vehicle #{VehicleNumber + 1} - PHYSICAL DAMAGE ONLY (PARKED CAR) is no longer supported. Coverage changed to FULL COVERAGE for the affected vehicle. ")
            vehicle.ComprehensiveCoverageOnly = False ' no longer valid with BUg 8418 - Matt A 3-27-17
        End If


        If vehicle IsNot Nothing Then
            'Checks to see if one of the following is selected. Comp Only, Liability Only, Named Non-Owner
            If vehicle.NonOwnedNamed Or
            vehicle.ComprehensiveCoverageOnly Or
            ((vehicle.ComprehensiveDeductibleLimitId = "0" AndAlso vehicle.CollisionDeductibleLimitId = "0" AndAlso vehicle.SoundEquipmentLimit = "0") Or
             ddPolicy.SelectedValue = ENUMHelper.VehiclePolicyType.liabilityOnly) Then
                'Set Values
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddTowing, "N/A")
                If IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote) = False Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddTransportation, "0")
                Else
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddTransportation, "446")
                End If
                chkInterruptionOfTravel.Checked = False
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddRadio, "0")
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAudioVisual, "0")
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddMedia, "0")
                chkAutoLoanLease.Checked = False
                'Added 9/28/18 for multi state MLW
                Select Case (Quote.QuickQuoteState)
                    Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                        'Updated 12/17/18 for multi state bug 30381 MLW
                        If ddPolicy.SelectedValue = ENUMHelper.VehiclePolicyType.liabilityOnly Then
                            dvUMPD.Attributes.Add("style", "display:block;")
                            chkUMPD.Enabled = True
                            If vehicle.UninsuredMotoristPropertyDamageLimitId IsNot Nothing AndAlso vehicle.UninsuredMotoristPropertyDamageLimitId <> "0" AndAlso vehicle.UninsuredMotoristPropertyDamageLimitId <> "" AndAlso QQHelper.IsPositiveIntegerString(vehicle.UninsuredMotoristPropertyDamageLimitId) Then
                                chkUMPD.Checked = True
                                If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                                    trUMPDLimitDD.Attributes.Add("style", "display:block;")
                                    trUMPDLimitMsg.Attributes.Add("style", "display:block;width:100%;")
                                    If vehicle.UninsuredMotoristPropertyDamageLimitId IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(vehicle.UninsuredMotoristPropertyDamageLimitId) Then
                                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddUMPD, vehicle.UninsuredMotoristPropertyDamageLimitId, QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, vehicle.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState))
                                    Else
                                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddUMPD, "15,000")
                                    End If
                                Else
                                    txtUMPDLimit.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, vehicle.UninsuredMotoristPropertyDamageLimitId, QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                                End If
                            Else
                                chkUMPD.Checked = False
                                If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                                    ddUMPD.SelectedIndex = -1
                                Else
                                    txtUMPDLimit.Text = ""
                                End If
                            End If
                        Else
                            dvUMPD.Attributes.Add("style", "display:none;") 'Added 9/28/18 for multi state MLW
                            Me.chkUMPD.Checked = False
                            chkUMPD.Enabled = False
                            If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                                ddUMPD.SelectedIndex = -1
                            Else
                                txtUMPDLimit.Text = ""
                            End If
                            ddCollision.Enabled = True
                        End If
                        'Me.chkUMPD.Checked = False
                        'chkUMPD.Enabled = False
                        'txtUMPDLimit.Text = ""
                        'ddCollision.Enabled = True
                    Case Else
                        Me.chkUMPD.Checked = False
                        chkUMPD.Enabled = False
                        If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                            ddUMPD.SelectedIndex = -1
                        Else
                            txtUMPDLimit.Text = ""
                        End If
                End Select
                ddCollision.Enabled = True
                chkPollution.Checked = False

                If ddPolicy.SelectedValue = ENUMHelper.VehiclePolicyType.liabilityOnly Or vehicle.NonOwnedNamed Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddComprehensive, "0")
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddCollision, "0")
                End If

                If vehicle.NonOwnedNamed Then
                    dvDisclaim.Attributes.Add("style", "display:block;")
                End If

                'Set Visibility
                If vehicle.ComprehensiveCoverageOnly Then
                    dvComp.Attributes.Add("style", "display:block;")
                    dvCompDisclaim.Attributes.Add("style", "display:block;")
                End If
                'dvUMPD.Attributes.Add("style", "display:none;") 'Added 9/28/18 for multi state MLW 'Moved above for bug 30381 MLW               
            Else
                dvComp.Attributes.Add("style", "display:block;")
                dvColl.Attributes.Add("style", "display:block;")
                dvTowing.Attributes.Add("style", "display:block;")
                dvTransportation.Attributes.Add("style", "display:block;")
                dvRadio.Attributes.Add("style", "display:block;")
                dvAVEquip.Attributes.Add("style", "display:block;")
                dvMedia.Attributes.Add("style", "display:block;")
                dvCostNew.Attributes.Add("style", "display:block;")

                'Added 9/27/18 For multi state MLW
                Select Case (Quote.QuickQuoteState)
                    Case QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteHelperClass.QuickQuoteState.Ohio 'Updated 1/17/2022 for OH task 66101 MLW
                        If ddPolicy.SelectedValue <> ENUMHelper.VehiclePolicyType.fullCoverage Then
                            'Added 10/4/18 for multi state MLW
                            dvUMPD.Attributes.Add("style", "display:none;")
                            Me.chkUMPD.Checked = False
                            chkUMPD.Enabled = False
                            If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                                ddUMPD.SelectedIndex = -1
                            Else
                                txtUMPDLimit.Text = ""
                            End If
                            ddCollision.Enabled = True
                        Else
                            dvUMPD.Attributes.Add("style", "display:block;")
                            If vehicle.CollisionDeductibleLimitId = 0 Then
                                If vehicle.UninsuredMotoristPropertyDamageLimitId IsNot Nothing AndAlso vehicle.UninsuredMotoristPropertyDamageLimitId <> "0" AndAlso vehicle.UninsuredMotoristPropertyDamageLimitId <> "" AndAlso QQHelper.IsPositiveIntegerString(vehicle.UninsuredMotoristPropertyDamageLimitId) Then
                                    chkUMPD.Checked = True
                                    chkUMPD.Enabled = True
                                    If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                                        trUMPDLimitDD.Attributes.Add("style", "display:block;")
                                        trUMPDLimitMsg.Attributes.Add("style", "display:block;width:100%;")
                                        If vehicle.UninsuredMotoristPropertyDamageLimitId IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(vehicle.UninsuredMotoristPropertyDamageLimitId) Then
                                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddUMPD, vehicle.UninsuredMotoristPropertyDamageLimitId, QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, vehicle.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState))
                                        Else
                                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddUMPD, "15,000")
                                        End If
                                    Else
                                        txtUMPDLimit.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, vehicle.UninsuredMotoristPropertyDamageLimitId, QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                                    End If
                                    If Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois AndAlso IFM.VR.Common.Helpers.PPA.CollisionAndUMPD.IsILCollisionAndUMPDAvailable(Quote) Then
                                        ddCollision.Enabled = True
                                    Else
                                        ddCollision.Enabled = False
                                    End If
                                    'ddCollision.Enabled = False
                                Else
                                    chkUMPD.Checked = False
                                    chkUMPD.Enabled = True
                                    If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                                        ddUMPD.SelectedIndex = -1
                                    Else
                                        txtUMPDLimit.Text = ""
                                    End If
                                    ddCollision.Enabled = True
                                End If
                            Else
                                If Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois AndAlso IFM.VR.Common.Helpers.PPA.CollisionAndUMPD.IsILCollisionAndUMPDAvailable(Quote) Then
                                    chkUMPD.Enabled = True
                                    If vehicle.UninsuredMotoristPropertyDamageLimitId IsNot Nothing AndAlso vehicle.UninsuredMotoristPropertyDamageLimitId <> "0" AndAlso vehicle.UninsuredMotoristPropertyDamageLimitId <> "" AndAlso QQHelper.IsPositiveIntegerString(vehicle.UninsuredMotoristPropertyDamageLimitId) Then
                                        chkUMPD.Checked = True
                                        If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                                            trUMPDLimitDD.Attributes.Add("style", "display:block;")
                                            trUMPDLimitMsg.Attributes.Add("style", "display:block;width:100%;")
                                            If vehicle.UninsuredMotoristPropertyDamageLimitId IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(vehicle.UninsuredMotoristPropertyDamageLimitId) Then
                                                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddUMPD, vehicle.UninsuredMotoristPropertyDamageLimitId, QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, vehicle.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState))
                                            Else
                                                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddUMPD, "15,000")
                                            End If
                                        Else
                                            txtUMPDLimit.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, vehicle.UninsuredMotoristPropertyDamageLimitId, QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                                        End If
                                    Else
                                        chkUMPD.Checked = False
                                        If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                                            ddUMPD.SelectedIndex = -1
                                        Else
                                            txtUMPDLimit.Text = ""
                                        End If
                                    End If
                                Else
                                    chkUMPD.Checked = False
                                    chkUMPD.Enabled = False
                                    If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                                        ddUMPD.SelectedIndex = -1
                                    Else
                                        txtUMPDLimit.Text = ""
                                    End If
                                End If
                                ddCollision.Enabled = True
                                'chkUMPD.Checked = False
                                'chkUMPD.Enabled = False
                                'txtUMPDLimit.Text = ""
                                'ddCollision.Enabled = True
                            End If
                        End If
                    Case Else
                        dvUMPD.Attributes.Add("style", "display:none;") 'Added 9/28/18 for multi state MLW
                        Me.chkUMPD.Checked = False
                        chkUMPD.Enabled = False
                        txtUMPDLimit.Text = ""
                        ddCollision.Enabled = True
                End Select

                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddMedia, vehicle.TapesAndRecordsLimitId)

                'Is this what was intended??? I wrote some code below that seems like it would do what this code wants to do... but I need to verify - Daniel Gugenheim - 8/12/2016
                If ddComprehensive.SelectedValue = "0" Then
                    ddTransportation.Items.Add("N/A")
                    dvTransportation.Disabled = True
                Else
                    'ddTransportation.Enabled = True
                    dvTransportation.Disabled = False
                    Dim NA As WebControls.ListItem = New WebControls.ListItem()
                    NA.Text = "N/A"

                    If ddTransportation.Items.Contains(NA) Then
                        ddTransportation.Items.Remove(NA.Text)
                    End If
                End If


                'Dim NA As WebControls.ListItem = New WebControls.ListItem()
                'NA.Text = "N/A"
                'NA.Value = 0

                'If ddComprehensive.SelectedValue = "0" Then
                '    If ddTransportation.Items.FindByText("N/A") Is Nothing Then
                '        ddTransportation.Items.Add(NA)
                '    End If
                '    ddTransportation.Items.FindByText("N/A").Selected = True
                '    For Each item As ListItem In ddTransportation.Items
                '        If item.Text.Equals("N/A", StringComparison.OrdinalIgnoreCase) = False Then
                '            item.Selected = False
                '        End If
                '    Next
                '    'dvTransportation.Disabled = True
                '    ddTransportation.Enabled = False
                'Else
                '    'ddTransportation.Enabled = True
                '    'dvTransportation.Disabled = False
                '    ddTransportation.Enabled = True
                '    If ddTransportation.Items.Contains(NA) Then
                '        ddTransportation.Items.Remove(NA.Text)
                '    End If
                'End If

                If (QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Pickup w/Camper" Or
                    QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Pickup w/o Camper") And
                    QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.VehicleUseTypeId, vehicle.VehicleUseTypeId) = "Farm" Then
                    dvPolution.Attributes.Add("style", "display:block;")
                Else
                    chkPollution.Checked = False
                End If

                ' BodyTypeId = Motorcycle
                If QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Motorcycle" Then
                    dvTravel.Attributes.Add("style", "display:block;")
                    dvRadio.Attributes.Add("style", "display:none;")
                    dvTransportation.Attributes.Add("style", "display:none;")
                    If IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote) = False Then
                        ddTransportation.SelectedValue = "0"
                    Else
                        ddTransportation.SelectedValue = "446"
                    End If
                    ddRadio.SelectedValue = "0"
                    dvLoanLease.Attributes.Add("style", "display:none;")
                    chkAutoLoanLease.Checked = False
                    dvMotorcycle.Attributes.Add("style", "display:block;")
                    dvAVEquip.Attributes.Add("style", "display:none;")
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAudioVisual, "0")

                    If ddComprehensive.SelectedValue = "0" Then
                        chkInterruptionOfTravel.Checked = False
                        txtMotorEquip.Enabled = False
                    Else
                        txtMotorEquip.Enabled = True
                        chkInterruptionOfTravel.Checked = True
                    End If
                Else
                    dvMotorcycle.Attributes.Add("style", "display:none;")
                    dvTravel.Attributes.Add("style", "display:none;")
                    chkInterruptionOfTravel.Checked = False

                    If vehicle.NonOwnedNamed Then
                        dvRadio.Attributes.Add("style", "display:none;")
                        dvAVEquip.Attributes.Add("style", "display:none;")
                        dvTransportation.Attributes.Add("style", "display:none;")

                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddRadio, "0")
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAudioVisual, "0")
                        If IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote) = False Then
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddTransportation, "0")
                        Else
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddTransportation, "446")
                        End If
                        chkAutoLoanLease.Checked = False
                    Else
                        ' Checks to see if vehicle type is a trailer
                        If QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Rec. Trailer" Or
                            QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vehicle.BodyTypeId) = "Other Trailer" Then
                            dvLiabDisclaim.Attributes.Add("style", "display:block;")
                            dvRadio.Attributes.Add("style", "display:none;")
                            dvAVEquip.Attributes.Add("style", "display:none;")
                            dvTransportation.Attributes.Add("style", "display:none;")
                            dvTowing.Attributes.Add("style", "display:none;")
                            dvLoanLease.Attributes.Add("style", "display:none;")
                            dvMedia.Attributes.Add("style", "display:none;")

                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddTowing, "0")
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddRadio, "0")
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAudioVisual, "0")
                            If IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote) = False Then
                                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddTransportation, "0")
                            Else
                                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddTransportation, "446")
                            End If
                            chkAutoLoanLease.Checked = False
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddMedia, "0")
                        Else
                            Dim todayDate As Long = DateTime.Now.Year

                            If DateTime.Now.Month >= 10 Then
                                todayDate += 1
                            End If
                            Dim vYear As Int32 = 0
                            'Int32.TryParse(vehicle.Year, VehicleYear)
                            If IsNumeric(vehicle.Year) Then
                                VehicleYear = vehicle.Year
                                If (todayDate - Integer.Parse(VehicleYear)) <= 5 OrElse (IsQuoteEndorsement() AndAlso vehicle.HasAutoLoanOrLease) Then
                                    dvLoanLease.Attributes.Add("style", "display:block;")
                                    If IsQuoteEndorsement() AndAlso vehicle.HasAutoLoanOrLease Then
                                        If vehicle.HasAutoLoanOrLease Then chkAutoLoanLease.Checked = True
                                    End If
                                Else
                                    dvLoanLease.Attributes.Add("style", "display:none;")
                                    ' Do NOT uncheck the loan/lease checkbox on endorsements!!  Bug 60188
                                    If Not IsQuoteEndorsement() Then chkAutoLoanLease.Checked = False
                                End If
                            Else
                                dvLoanLease.Attributes.Add("style", "display:none;")
                                ' Do NOT uncheck the loan/lease checkbox on endorsements!!  Bug 60188
                                If Not IsQuoteEndorsement() Then chkAutoLoanLease.Checked = False
                            End If

                            dvLiabDisclaim.Attributes.Add("style", "display:none;")
                            dvCompDisclaim.Attributes.Add("style", "display:none;")
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Quote.Vehicles(VehicleNumber)
        ValidationHelper.GroupName = String.Format("Vehicle #{0}", VehicleNumber + 1)
        Dim accordList As List(Of VRAccordionTogglePair) = MyAccordionList ' just to cache it

        Dim valList = VehicleCoverageValidator.ValidatePPAVehicleCoverage(VehicleNumber, Quote, valArgs.ValidationType)

        If valList.Any() Then
            For Each v In valList
                ' *********************
                ' Base Policy Coverages
                ' *********************
                Select Case v.FieldId
                    Case VehicleCoverageValidator.VehicleCoverageRadio
                        ValidationHelper.Val_BindValidationItemToControl(ddRadio, v, accordList)
                    Case VehicleCoverageValidator.VehicleHasCompOnly
                        ValidationHelper.Val_BindValidationItemToControl(ddPolicy, v, accordList)
                        'Case PolicyCoverageValidator.CSLExceedLimit
                        '    ValidationHelper.Val_BindValidationItemToControl(ddUmUimSSl, v, accordList)
                        'Case PolicyCoverageValidator.UMPDDeductRequired
                        '    ValidationHelper.Val_BindValidationItemToControl(ddUmPdDeductible, v, accordList)
                        'Case PolicyCoverageValidator.MinBI
                        '    ValidationHelper.Val_BindValidationItemToControl(ddBodilyInjury, v, accordList)
                        'Case PolicyCoverageValidator.ExceedBI
                        '    ValidationHelper.Val_BindValidationItemToControl(ddUmUmiBi, v, accordList)
                        'Case PolicyCoverageValidator.UMBIRequired
                        '    ValidationHelper.Val_BindValidationItemToControl(ddUmUmiBi, v, accordList)
                        'Case PolicyCoverageValidator.UMPDRequired
                        '    ValidationHelper.Val_BindValidationItemToControl(ddUmPd, v, accordList)
                        'Case PolicyCoverageValidator.UMPDDeductRequired
                        '    ValidationHelper.Val_BindValidationItemToControl(ddUmPdDeductible, v, accordList)
                        'Case PolicyCoverageValidator.ExceedPD
                        '    ValidationHelper.Val_BindValidationItemToControl(ddUmPd, v, accordList)
                End Select
            Next
        End If
        ValidateChildControls(valArgs)
    End Sub

    Private Sub CreateScheduledItemStub(description As String)
        If Quote IsNot Nothing Then
            Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing

            If Me.Quote.Vehicles IsNot Nothing Then
                Try
                    vehicle = Me.Quote.Vehicles(Me.VehicleNumber)
                Catch ex As Exception
                End Try
                If vehicle IsNot Nothing Then
                    vehicle.ScheduledItems = New List(Of QuickQuote.CommonObjects.QuickQuoteScheduledItem)
                    Dim schedItem As QuickQuote.CommonObjects.QuickQuoteScheduledItem = New QuickQuote.CommonObjects.QuickQuoteScheduledItem()

                    schedItem.Description = description
                    schedItem.ItemDate = DateTime.Now
                    schedItem.Amount = txtMotorEquip.Text
                    schedItem.ScheduledItemsCategoryId = "2"
                    QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, txtMotorEquip.Text)

                    If txtMotorEquip.Text = "0" Then
                        Exit Sub
                    Else
                        vehicle.ScheduledItems.Add(schedItem)
                    End If
                End If
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            If IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
                ctlCoverage_PPA_ScheduledItemsList.Save()
            End If
            Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
            If Me.Quote.Vehicles IsNot Nothing Then
                Try
                    vehicle = Me.Quote.Vehicles(VehicleNumber)
                Catch ex As Exception
                End Try

                If vehicle IsNot Nothing Then
                    ''
                    ' Vehicle Level Coverage
                    ''
                    vehicle.ComprehensiveDeductibleLimitId = Me.ddComprehensive.SelectedValue
                    vehicle.CollisionDeductibleLimitId = Me.ddCollision.SelectedValue

                    vehicle.HasAutoLoanOrLease = Me.chkAutoLoanLease.Checked

                    'Updated 1/17/2022 for OH task 66101 MLW
                    Select Case Quote.QuickQuoteState
                        Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                            If IFM.VR.Common.Helpers.PPA.UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                                vehicle.UninsuredMotoristPropertyDamageLimitId = IIf(Me.chkUMPD.Checked, Me.ddUMPD.SelectedValue, "")
                            Else
                                vehicle.UninsuredMotoristPropertyDamageLimitId = IIf(Me.chkUMPD.Checked,
                                                                                     QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, "15,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal),
                                                                                     "")
                            End If
                        Case QuickQuoteHelperClass.QuickQuoteState.Ohio
                            vehicle.UninsuredMotoristPropertyDamageLimitId = IIf(Me.chkUMPD.Checked,
                                                                                 QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, "7,500", QuickQuoteObject.QuickQuoteLobType.AutoPersonal),
                                                                                 "")
                    End Select
                    ''Added 10/2/18 for multi state MLW
                    'If (Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois) Then
                    '    If Me.chkUMPD.Checked Then
                    '        vehicle.UninsuredMotoristPropertyDamageLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, "15,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                    '    Else
                    '        vehicle.UninsuredMotoristPropertyDamageLimitId = ""
                    '    End If
                    '    ''10/04/18 Keeping this in because we might need to update for future state use MLW
                    '    ''txtUMPDLimit.Text is not passing through its value when set to ReadOnly="true" on the aspx page. This work around works when ReadOnly is removed and onKeyDown="return false;" onFocus="blur();" onpaste="return false;" are used. This has not be tested cross-browser.
                    '    'If Me.txtUMPDLimit.Text IsNot Nothing AndAlso Me.txtUMPDLimit.Text <> "" AndAlso QQHelper.IsPositiveIntegerString(Me.txtUMPDLimit.Text) Then
                    '    '    vehicle.UninsuredMotoristPropertyDamageLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, Quote.QuickQuoteState, txtUMPDLimit.Text, QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                    '    'Else
                    '    '    vehicle.UninsuredMotoristPropertyDamageLimitId = ""
                    '    'End If
                    'End If                   

                    ' Saves "0" if N/A or [200 INC] is selected and saves "212" if 200 is selected
                    vehicle.TapesAndRecordsLimitId = ddMedia.SelectedValue
                        vehicle.HasPollutionLiabilityBroadenedCoverage = Me.chkPollution.Checked
                        vehicle.TowingAndLaborDeductibleLimitId = Me.ddTowing.SelectedValue


                    'TFS Bug 20129 Can not have Transportation Expense with these body types
                    'Task 66103 - modified because vehicle.BodyTypeId can be an empty string and this was not compensated for
                    If Not String.IsNullOrEmpty(vehicle.BodyTypeId) AndAlso
                            {VehicleBodyType.bodyType_AntiaueAuto, VehicleBodyType.bodyType_ClassicAuto, VehicleBodyType.bodyType_Motorcycle,
                        VehicleBodyType.bodyType_MotorHome, VehicleBodyType.bodyType_OtherTrailer, VehicleBodyType.bodyType_RecTrailer}.Contains(vehicle.BodyTypeId) Then
                        If IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote) = False Then
                            vehicle.TransportationExpenseLimitId = "0"
                        Else
                            vehicle.TransportationExpenseLimitId = "0"
                        End If
                    Else
                        If vehicle.ComprehensiveDeductibleLimitId = "0" Then
                            If IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote) = False Then
                                vehicle.TransportationExpenseLimitId = "0"
                            Else
                                vehicle.TransportationExpenseLimitId = "0"
                            End If
                        Else
                            vehicle.TransportationExpenseLimitId = Me.ddTransportation.SelectedValue
                        End If
                    End If

                    If (If(hiddenVehicleCoveragePlan.Value = "", "0", hiddenVehicleCoveragePlan.Value)) <> VehiclePolicyType.compOnly Then
                            vehicle.ComprehensiveCoverageOnly = False
                        Else
                            vehicle.ComprehensiveCoverageOnly = True
                        End If

                        ' Removed scheduled items if body type is changed from motorcycle
                        If vehicle.BodyTypeId <> VehicleBodyType.bodyType_Motorcycle Then
                            vehicle.ScheduledItems = Nothing
                            QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, "0")
                        Else
                            If IsQuoteEndorsement() = False AndAlso IsQuoteReadOnly() = False Then
                                If ddPolicy.SelectedValue <> VehiclePolicyType.liabilityOnly Then
                                    If vehicle.ScheduledItems IsNot Nothing Then
                                        If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtMotorEquip.Text) <> 0 And vehicle.ScheduledItems.Count <= 1 Then
                                            QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, txtMotorEquip.Text)
                                            'Updated 6/26/18 for Bug 25238 MLW
                                            If vehicle.ScheduledItems.Count < 1 Then
                                                CreateScheduledItemStub("DESCRIPTION # 1")
                                            Else
                                                CreateScheduledItemStub(vehicle.ScheduledItems(0).Description)
                                            End If
                                            'CreateScheduledItemStub(vehicle.ScheduledItems(0).Description)
                                        Else
                                            QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, txtMotorEquip.Text)
                                            'Added 6/26/18 for Bug 25238 MLW
                                            If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtMotorEquip.Text) = 0 OrElse txtMotorEquip.Text = "") AndAlso vehicle.ScheduledItems.Count >= 1 Then
                                                vehicle.ScheduledItems.Remove(vehicle.ScheduledItems(0))
                                            End If
                                        End If
                                    Else
                                        CreateScheduledItemStub("DESCRIPTION # 1")
                                    End If
                                Else
                                    vehicle.ScheduledItems = Nothing
                                    QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, "0")
                                End If
                            Else
                                QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, txtMotorEquip.Text)

                                If ddPolicy.SelectedValue <> VehiclePolicyType.liabilityOnly Then
                                    If vehicle.ScheduledItems IsNot Nothing Then
                                        If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtMotorEquip.Text) <> 0 And vehicle.ScheduledItems.Count < 1 Then
                                            QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, txtMotorEquip.Text)
                                            CreateScheduledItemStub("DESCRIPTION # 1")
                                        Else
                                            QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, txtMotorEquip.Text)
                                            'Added 6/26/18 for Bug 25238 MLW
                                            If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtMotorEquip.Text) = 0 OrElse txtMotorEquip.Text = "") AndAlso vehicle.ScheduledItems.Count >= 1 Then
                                                vehicle.ScheduledItems.Clear()
                                            End If
                                        End If
                                    Else
                                        CreateScheduledItemStub("DESCRIPTION # 1")
                                    End If
                                Else
                                    vehicle.ScheduledItems = Nothing
                                    QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, "0")
                                End If
                            End If

                        End If

                        If chkInterruptionOfTravel.Checked Then
                            vehicle.TripInterruptionLimitId = "25"
                        Else
                            vehicle.TripInterruptionLimitId = "0"
                        End If

                        ' Switched this back to the old original code since my update seems to have broken comp & collision  MGB 2/21/19 Bug 31733
                        If ddRadio.SelectedValue = "[1,000 INC]" Then
                            vehicle.SoundEquipmentLimit = "0"
                        Else
                            vehicle.SoundEquipmentLimit = ddRadio.SelectedValue
                        End If

                        If ddAudioVisual.SelectedValue = "N/A" Then
                            vehicle.ElectronicEquipmentLimit = "0"
                        Else
                            vehicle.ElectronicEquipmentLimit = ddAudioVisual.SelectedItem.Text.Replace(",", "")
                        End If

                        ' adding CostNew for bug 32399 - ZTS 8/28/19
                        If vehicle.VehicleSymbols IsNot Nothing AndAlso vehicle.VehicleSymbols.Count > 1 OrElse ddComprehensive.SelectedValue = 0 AndAlso ddCollision.SelectedValue = 0 Then

                        Else
                            vehicle.CostNew = txtCostNew.Text
                        End If
                    End If

                    RaiseEvent SaveVehPolicy(vehicle, hiddenVehicleCoveragePlan.Value)
            End If

            Return True
        End If

        Return False
    End Function

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        If Me.Quote IsNot Nothing Then
            Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
            If Me.Quote.Vehicles IsNot Nothing Then
                Try
                    vehicle = Me.Quote.Vehicles(Me.VehicleNumber)
                Catch ex As Exception
                End Try

                ddPolicy.SelectedValue = "0"
                policyPlan = ddPolicy.SelectedValue
                SetVehicleDefaults(vehicle)
                OptionalCoverageVisibility(vehicle)
                Save_FireSaveEvent(False)
            End If
        End If
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Save_FireSaveEvent(True)
    End Sub

    Protected Sub lnkBtnClear_Click(sender As Object, e As EventArgs) Handles lnkBtnClear.Click
        ClearControl()
    End Sub

    'Private Sub LoadPolicyDropDowns()
    '    ' Load Policy Plan DropDown
    '    ddPolicy.Items.Clear()
    '    ddPolicy.Items.Insert(0, New ListItem("FULL COVERAGE", "0")) 'Index 0
    '    ddPolicy.Items.Insert(1, New ListItem("LIABILITY ONLY", "1"))    'Index 1
    '    'ddPolicy.Items.Insert(2, New ListItem("PHYSICAL DAMAGE ONLY (PARKED CAR)", "2")) 'Index 2 ' no longer valid with BUg 8418 - Matt A 3-27-17

    '    ddPolicy.SelectedValue = "0"
    'End Sub

    Private Sub PopulateMediaDropDownList()
        If (ddMedia.Items.Count > 1) Then
            ddMedia.Items.RemoveAt("1")
            ddMedia.Items.RemoveAt("0")
        End If


        If ddRadio.SelectedValue <> "[1,000 INC]" Then
            ddMedia.Items.Insert(0, New ListItem("[200 INC]", "0"))

            If ddMedia.SelectedValue <> "219" Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddMedia, "0")
            End If
        ElseIf ddAudioVisual.SelectedValue = "N/A" Then
            ddMedia.Items.Insert(0, New ListItem("N/A", "0"))
            ddMedia.Items.Insert(1, New ListItem("200", "212"))

            If ddMedia.SelectedValue <> "219" Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddMedia, "0")
            End If
        Else
            ddMedia.Items.Insert(0, New ListItem("[200 INC]", "0"))

            If ddMedia.SelectedValue <> "219" Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddMedia, "0")
            End If
        End If
    End Sub

    Protected Sub btnCopyCoverage_Click(sender As Object, e As EventArgs) Handles btnCopyCoverage.Click
        If Quote IsNot Nothing Then
            Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
            If Me.Quote.Vehicles IsNot Nothing Then
                RaiseEvent SavePreviousVehicle(Me.VehicleNumber - 1)
                Try
                    vehicle = Me.Quote.Vehicles(Me.VehicleNumber - 1)
                Catch ex As Exception
                End Try

                'Copy coverages from previous vehicle
                Me.chkAutoLoanLease.Checked = vehicle.HasAutoLoanOrLease
                Me.chkPollution.Checked = vehicle.HasPollutionLiabilityBroadenedCoverage
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddComprehensive, vehicle.ComprehensiveDeductibleLimitId)
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddCollision, vehicle.CollisionDeductibleLimitId)
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddTowing, vehicle.TowingAndLaborDeductibleLimitId)
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddTransportation, vehicle.TransportationExpenseLimitId)
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAudioVisual, vehicle.ElectronicEquipmentLimit)
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddRadio, vehicle.SoundEquipmentLimit)
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddMedia, vehicle.TapesAndRecordsLimitId)
            End If
        End If
    End Sub

    Private Sub ctlCoverage_PPA_ScheduledItemsList_RequestPageRefresh() Handles ctlCoverage_PPA_ScheduledItemsList.RequestPageRefresh
        RaiseEvent RequestPageRefresh()
    End Sub

End Class