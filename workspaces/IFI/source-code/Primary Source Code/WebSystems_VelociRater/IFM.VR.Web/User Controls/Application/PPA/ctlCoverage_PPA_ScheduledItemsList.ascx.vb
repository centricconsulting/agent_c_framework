Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Public Class ctlCoverage_PPA_ScheduledItemsList
    Inherits VRControlBase

    'Public Event SaveRequested(invokeValidations As Boolean)
    Public Event RequestPageRefresh()

    Private hasSIScriptVars As Boolean = False ' makes sure that these vars are only added once per page load
    Public Property VehicleIndex As Int32
        Get
            If ViewState("vs_vehicleNum") Is Nothing Then
                ViewState("vs_vehicleNum") = 0
            End If
            Return CInt(ViewState("vs_vehicleNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_vehicleNum") = value
            If IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
                DoSiCalcScriptSetup()
            End If
        End Set
    End Property

    Private ReadOnly Property MotorcycleCustomEquipmentTotalDictionaryName As String
        Get
            Return QuoteId & "_" & "MCCustomEquipTotal_" & VehicleIndex
        End Get
    End Property

    Public Property OriginalScheduledItemSum As Int32
        Get
            ' Changed to use dev dictionary instead of session var
            If Quote IsNot Nothing Then
                Dim val As String = QQDevDictionary_GetItem(MotorcycleCustomEquipmentTotalDictionaryName)
                If val IsNot Nothing AndAlso IsNumeric(val) Then Return CInt(val)
            End If
            Return 0
        End Get
        Set(value As Int32)
            QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, value.ToString)
        End Set
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.ListAccordionDivId = Me.divScheduledItemsList.ClientID

        If Not IsPostBack Then
            LoadStaticData()
        End If

        If IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
            'sometimes the vehicle number hasn't been set yet in that case the vars will be set when the vehicle num is set
            DoSiCalcScriptSetup()
        End If
        AttachCoverageControlEvents()

    End Sub

    Public Overrides Sub AddScriptAlways()

        If Me.Quote IsNot Nothing Then
            If Me.Quote.Vehicles IsNot Nothing Then
                Dim v As QuickQuote.CommonObjects.QuickQuoteVehicle = Me.Quote.Vehicles.GetItemAtIndex(Me.VehicleIndex)
                If v IsNot Nothing Then
                    If IsQuoteEndorsement() Then
                        Me.VRScript.CreateAccordion(Me.divScheduledItems.ClientID, HiddenField1, "0")
                    Else
                        If v.ScheduledItems.IsLoaded Then
                            Me.VRScript.CreateAccordion(Me.divScheduledItems.ClientID, HiddenField1, "0")
                        Else
                            Me.VRScript.CreateAccordion(Me.divScheduledItems.ClientID, Nothing, "", True)
                        End If
                    End If

                End If
            End If
        End If

        'add schedule event
        Me.VRScript.AddScriptLine("$(""#" + Me.lnkBtnAdd.ClientID + """).bind(""click"", function (e) { e.stopPropagation(); DisableFormOnSaveRemoves();});")

        'save schedule event
        Me.VRScript.AddScriptLine("$(""#" + Me.lnkBtnSave.ClientID + """).bind(""click"", function (e) { e.stopPropagation(); DisableFormOnSaveRemoves();});")
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.divScheduledItemsList.ClientID, HiddenFieldMainAccord, "0")
    End Sub

    Private Sub DoSiCalcScriptSetup()
        If IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then 'double check
            If hasSIScriptVars = False And Me.VehicleIndex >= 0 Then
                hasSIScriptVars = True
                'CAH 8/14/2019 - Changed Me.QuoteId to Me.QuoteIdOrPolicyIdPipeImageNumber - Endo's will not have QuoteId.  The new prop resolve to QuoteID if not Endo
                Me.VRScript.AddVariableLine("var originalSISum_" + Me.VehicleIndex.ToString() + "_" + Me.QuoteIdOrPolicyIdPipeImageNumberJavaScriptSafe + " = " + OriginalScheduledItemSum.ToString() + ";")
                Me.VRScript.AddVariableLine("var currentSISum_" + Me.VehicleIndex.ToString() + "_" + Me.QuoteIdOrPolicyIdPipeImageNumberJavaScriptSafe + " = " + GetCurrentSI_Sum().ToString() + ";")
                Me.VRScript.AddVariableLine("var txtAmountID_Array_" + Me.VehicleIndex.ToString() + "_" + Me.QuoteIdOrPolicyIdPipeImageNumberJavaScriptSafe + " = new Array();")
                Me.VRScript.AddVariableLine("var txtReaminingAmountID_" + Me.VehicleIndex.ToString() + "_" + Me.QuoteIdOrPolicyIdPipeImageNumberJavaScriptSafe + " = '" + Me.txtRemaining.ClientID + "';")
            End If

        End If
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()

        Dim bodyType_MotorCycle As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Motorcycle", Me.Quote.LobType)
        Dim bodyType_MotorHome As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Motor Home", Me.Quote.LobType)
        Dim bodyType_PICKUPWCAMPER As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "PICKUP W/CAMPER", Me.Quote.LobType)
        Dim bodyType_RecTrailer As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Rec. Trailer", Me.Quote.LobType)
        Dim bodyType_ClassicAuto As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Classic Auto", Me.Quote.LobType)
        Dim bodyType_OtherTrailer As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Other Trailer", Me.Quote.LobType)
        Dim bodyType_AntiqueAuto As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Antique Auto", Me.Quote.LobType)

        Me.Repeater1.DataSource = Nothing
        Me.Repeater1.DataBind()

        If Me.Quote IsNot Nothing Then
            If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > Me.VehicleIndex Then
                Dim v As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
                v = Me.Quote.Vehicles(Me.VehicleIndex)
                If v IsNot Nothing Then
                    ' only show if the vehicle is a motorcycle and a custom equipment value is > $0
                    If v.BodyTypeId = bodyType_MotorCycle Then ' And Helpers.WebHelper_Personal.IsNonNegativeNumber(v.CustomEquipmentAmount) Then
                        If v.ScheduledItems IsNot Nothing AndAlso v.ScheduledItems.Any() Then
                            Me.Repeater1.DataSource = v.ScheduledItems
                            Me.Repeater1.DataBind()

                            Me.txtCustomEquipmentAmount.Text = String.Format("{0:C0}", OriginalScheduledItemSum)
                            Me.txtRemaining.Text = String.Format("{0:C0}", OriginalScheduledItemSum - GetCurrentSI_Sum())
                            If IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then ' if on quote side the coverage control will decide if this is visible
                                Me.Visible = True
                            End If

                        End If

                    Else
                        ' if somehow scheduled items get in here just remove them
                        Me.Visible = False
                        'Removed with bug 72753 - Now saving CustomEquipmentAmount for all vehicle types
                        If IFM.VR.Common.Helpers.PPA.CustomEquipmentHelper.IsCustomEquipmentAvailable(Quote) = False Then
                            If IsQuoteEndorsement() = False then
                                v.CustomEquipmentAmount = ""
                            End If
                        End If
                        
                        If v.ScheduledItems IsNot Nothing Then
                            v.ScheduledItems.Clear()
                        End If
                        ' raise save requested                       
                    End If
                End If
            End If
        End If
    End Sub

    Private Function GetCurrentSI_Sum() As Int32
        If Me.Quote IsNot Nothing Then
            If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > Me.VehicleIndex Then
                Dim v As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
                v = Me.Quote.Vehicles(Me.VehicleIndex)
                If v IsNot Nothing Then
                    Dim bodyType_MotorCycle As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Motorcycle", Me.Quote.LobType)

                    ' only show if the vehicle is a motorcycle and a custom equipment value is > $0
                    If v.BodyTypeId = bodyType_MotorCycle Then ' And Helpers.WebHelper_Personal.IsNonNegativeNumber(v.CustomEquipmentAmount) Then
                        If v.ScheduledItems IsNot Nothing AndAlso v.ScheduledItems.Any() Then
                            Dim scheduledAmount As Integer
                            If v.ScheduledItems IsNot Nothing Then
                                For Each si In v.ScheduledItems
                                    If IsNumeric(si.Amount) Then
                                        scheduledAmount += CInt(si.Amount)
                                    End If
                                Next
                            End If
                            Return scheduledAmount
                        End If
                    End If
                End If
            End If
        End If

        Return 0
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = String.Format("Custom Equipment - Vehicle #{0}", Me.VehicleIndex + 1)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        If Me.Quote IsNot Nothing Then
            If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > Me.VehicleIndex Then
                Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
                vehicle = Me.Quote.Vehicles(Me.VehicleIndex)
                Dim siSum As Double = 0.0
                If vehicle IsNot Nothing Then
                    Dim bodyType_MotorCycle As String = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Motorcycle")
                    If vehicle.BodyTypeId = bodyType_MotorCycle Then
                        If vehicle.ScheduledItems IsNot Nothing Then
                            For Each si In vehicle.ScheduledItems
                                siSum += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(si.Amount)
                            Next
                            If siSum <> IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtCustomEquipmentAmount.Text) Then
                                Me.ValidationHelper.AddWarning("App Custom Equipment sum did not match sum from quote.", Me.ClientID)
                                With Me.ValidationHelper.GetLastWarning()
                                    .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                                    .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                                End With
                            End If
                        End If
                    End If
                End If

            End If

        End If

        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim vehicleScheduleControl As ctlCoverage_PPA_ScheduledItem = cntrl.FindControl("ctlCoverage_PPA_ScheduledItem")
            vehicleScheduleControl.ValidateControl(valArgs)
        Next

    End Sub

    Public Overrides Function Save() As Boolean
        If IsOnAppPage = False AndAlso IsQuoteEndorsement() = False AndAlso IsQuoteReadOnly() = False Then
            ' on quote side so just wipe it out and let it set it if it makes it to app again
            OriginalScheduledItemSum = "0"
        Else
            If Quote IsNot Nothing Then
                For Each cntrl As RepeaterItem In Me.Repeater1.Items
                    Dim vehicleSheduledItem As ctlCoverage_PPA_ScheduledItem = cntrl.FindControl("ctlCoverage_PPA_ScheduledItem")
                    vehicleSheduledItem.Save()
                Next
                Me.Populate()
                Return True
            End If
        End If
        Return False
    End Function

    Private Sub Repeater1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
        Dim vehicleControl As ctlCoverage_PPA_ScheduledItem = e.Item.FindControl("ctlCoverage_PPA_ScheduledItem")
        'vehicleControl.VehicleItemsAccordID = Me.VehicleItemsAccordID
        vehicleControl.VehicleIndex = Me.VehicleIndex
        vehicleControl.ItemIndex = e.Item.ItemIndex
        vehicleControl.Populate()
    End Sub

    Protected Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkBtnAdd.Click
        Me.AddScheduleItem()
    End Sub

    Private Sub AddScheduleItem()
        If Me.Quote IsNot Nothing Then
            If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Any() Then
                Dim v As QuickQuote.CommonObjects.QuickQuoteVehicle = Me.Quote.Vehicles(Me.VehicleIndex)
                Dim newSI As New QuickQuote.CommonObjects.QuickQuoteScheduledItem()
                If v.ScheduledItems Is Nothing Then
                    v.ScheduledItems = New List(Of QuickQuoteScheduledItem)()
                End If
                newSI.ScheduledItemsComboId = 0
                newSI.ScheduledItemsCategoryId = 0
                v.ScheduledItems.Add(newSI)
                Me.HiddenFieldMainAccord.Value = "0"
                Me.HiddenField1.Value = (v.ScheduledItems.Count() - 1).ToString()

                'these will overwrite the ones created by control markup
                Me.VRScript.AddScriptLine("$(""#" + Me.divScheduledItemsList.ClientID + """).accordion({heightStyle: ""content"", active: " + Me.HiddenFieldMainAccord.Value + ", collapsible: true, activate: function(event, ui) { $(""#" + Me.HiddenFieldMainAccord.ClientID + """).val($(""#" + Me.divScheduledItemsList.ClientID + """).accordion('option','active'));    } });", True)
                Me.VRScript.AddScriptLine("$(""#" + Me.divScheduledItems.ClientID + """).accordion({heightStyle: ""content"", active: " + Me.HiddenField1.Value + ", collapsible: true, activate: function(event, ui) { $(""#" + Me.HiddenField1.ClientID + """).val($(""#" + Me.divScheduledItems.ClientID + """).accordion('option','active'));    } });", True)

                Me.Save_FireSaveEvent(New IFM.VR.Web.VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            End If
        End If
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent(New IFM.VR.Web.VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub

    Protected Sub AttachCoverageControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim vehicleScheduleControl As ctlCoverage_PPA_ScheduledItem = cntrl.FindControl("ctlCoverage_PPA_ScheduledItem")
            'AddHandler vehicleScheduleControl.SaveRequested, AddressOf ScheduledControlRequestedSave
            AddHandler vehicleScheduleControl.ItemRemoved, AddressOf ScheduledItemRemoved
            index += 1
        Next
    End Sub

    Private Sub ScheduledControlRequestedSave(invokeValidations As Boolean)
        Me.Save_FireSaveEvent(New IFM.VR.Web.VrControlBaseSaveEventArgs(Me, invokeValidations, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        Me.Populate()
    End Sub

    Private Sub ScheduledItemRemoved(itemIndex As Integer)

        If Quote IsNot Nothing Then
            Me.Save_FireSaveEvent(New IFM.VR.Web.VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > Me.VehicleIndex Then
                Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
                vehicle = Quote.Vehicles(Me.VehicleIndex)
                If vehicle IsNot Nothing AndAlso vehicle.ScheduledItems IsNot Nothing AndAlso vehicle.ScheduledItems.Count > itemIndex Then
                    vehicle.ScheduledItems.RemoveAt(itemIndex) ' removes
                    ' do database save here because I had to -  7-1-2014 Matt A
                    If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/15/2019; original logic in ELSE
                        'note: could also check for Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                        'no Save needed
                        'note: could also get policyId/imageNum from qqo; may note need to reload ReadOnly
                        VR.Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
                    ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                        'note: could also check for Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                        Dim endorsementSaveError As String = ""
                        Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
                        'note: could also get policyId/imageNum from qqo
                        VR.Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
                    Else
                        VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, Nothing, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
                        VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                    End If
                    RaiseEvent RequestPageRefresh()
#If DEBUG Then
                Else
                    Debugger.Break()
#End If
                End If
#If DEBUG Then
            Else
                Debugger.Break()
#End If
            End If

        End If
    End Sub

End Class