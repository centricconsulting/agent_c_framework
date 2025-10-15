Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
Imports IFM.PrimativeExtensions

Public Class ctlVehicleList
    Inherits VRControlBase

    Private ReadOnly Property MotorcycleCustomEquipmentTotalDictionaryName As String
        Get
            Return QuoteId & "_" & "MCCustomEquipTotal_" & ActiveVehicleIndex
        End Get
    End Property

    Public Property ActiveVehicleIndex As String
        Get
            Return Me.hidden_VehicleListActive.Value
        End Get
        Set(value As String)
            Me.hidden_VehicleListActive.Value = value
        End Set
    End Property

    Public Property LstOfNewVehicleIndexes As List(Of Integer)

    Private Property HasReplacedVehicle As Boolean
        Get
            If Not IsNullEmptyorWhitespace(hdnHasReplacedVehicle.Value) Then
                hdnHasReplacedVehicle.Value = False
            End If
            Return CBool(hdnHasReplacedVehicle.Value)
        End Get
        Set(value As Boolean)
            hdnHasReplacedVehicle.Value = value
        End Set
    End Property

    Public Property HasShownRecentDialogOnce As Boolean
        Get
            If Session(EndorsementPolicyIdAndImageNum + "_LstOfNewVehicleIndexes") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Session(EndorsementPolicyIdAndImageNum + "_LstOfNewVehicleIndexes").ToString) <> True Then
                Return CBool(Session(EndorsementPolicyIdAndImageNum + "_LstOfNewVehicleIndexes"))
            End If
            Return False
        End Get
        Set(ByVal value As Boolean)
            Session(EndorsementPolicyIdAndImageNum + "_LstOfNewVehicleIndexes") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Quote?.LobType <> QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
            Me.ctl_AdditionalInterest_MiniSerach.Visible = False
        End If

        If IsQuoteEndorsement() Then
            btnEndorsementReplaceVehicle.Visible = True
            btnEndorsementRemoveVehicle.Visible = True
        End If

        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divVehicles.ClientID
            LoadStaticData()
            Populate()
        End If

        AttachVehicleControlEvents()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(ListAccordionDivId, Me.hidden_VehicleListActive, "0")
        Me.VRScript.AddVariableLine("var VehicleListControlDivTopMost = '" + Me.VehicleListControlDivTopMost.ClientID + "';")
        Me.VRScript.AddVariableLine("var garaged_copy = new Array();") '7-21-14 copy garaging address
        Me.VRScript.AddVariableLine("function ShowVehicle(index){SetActiveAccordionIndex('" + ListAccordionDivId + "',index);}")
    End Sub

    Protected Sub AttachVehicleControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim vehicleControl As ctlVehicle_PPA = cntrl.FindControl("ctlVehicle_PPAControl")
            AddHandler vehicleControl.VehicleControlRemoving, AddressOf vehicleControlRemoving
            AddHandler vehicleControl.NewVehcileRequested, AddressOf vehicleControlNewVehicleReqested
            AddHandler vehicleControl.ReplaceVehicleTitleBar, AddressOf vehicleControlReplaceVehicleTitleBar
            index += 1
        Next
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        ctlEndorsementReplaceVehicle.Visible = False
        ctlEndorsementRemoveVehicle.Visible = False
        ctlRecentManufacturedVehicle.Visible = False
        Me.Repeater1.DataSource = Nothing
        If Me.Quote IsNot Nothing Then
            Me.Repeater1.DataSource = Me.Quote.Vehicles
            Me.Repeater1.DataBind()

            Me.FindChildVrControls() ' finds the just added controls do to the binding
            Dim index As Int32 = 0
            For Each child In Me.ChildVrControls
                If TypeOf child Is ctlVehicle_PPA Then
                    Dim c As ctlVehicle_PPA = child
                    c.VehicleIndex = index
                    c.Populate()
                    index += 1
                End If
            Next

        End If

        If Me.Quote IsNot Nothing Then
            If IsQuoteReadOnly() Then
                Dim policyNumber As String = Me.Quote.PolicyNumber
                Dim imageNum As Integer = 0
                Dim policyId As Integer = 0
                Dim toolTip As String = "Make a change to this policy"
                'Dim qqHelper As New QuickQuoteHelperClass
                Dim readOnlyViewPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                    readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                End If

                divActionButtons.Visible = False
                divEndorsementButtons.Visible = True

                btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                btnMakeAChange.ToolTip = toolTip
                btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
            End If

        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Vehicle List"

        Dim valItems = VehicleListValidator.ValidateVehicleList(Me.Quote, valArgs.ValidationType)
        If valItems.Any() Then

            For Each v In valItems
                Select Case v.FieldId
                    Case VehicleListValidator.VehicleListNoHasMotorCycleVehicles
                        If v.IsWarning Then
                            Me.ValidationHelper.AddWarning(v.Message)
                        Else
                            Me.ValidationHelper.AddError(v.Message)
                        End If
                    Case VehicleListValidator.VehicleListNotAllDriversAreAssignedToAVehicle
                        If v.IsWarning Then
                            Me.ValidationHelper.AddWarning(v.Message)
                        Else
                            Me.ValidationHelper.AddError(v.Message)
                        End If
                    Case Else
                        If v.IsWarning Then
                            Me.ValidationHelper.AddWarning(v.Message)
                        Else
                            Me.ValidationHelper.AddError(v.Message)
                        End If
                End Select
            Next
        End If

        Me.ValidateChildControls(valArgs)

    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Private Sub vehicleControlRemoving(index As Integer)
        Dim activePan As Int32 = 0
        If Int32.TryParse(Me.hidden_VehicleListActive.Value, activePan) Then
            If activePan >= index Then
                Me.hidden_VehicleListActive.Value = (activePan - 1).ToString()
            End If
        End If
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSaveandGotoCoverages.Click, btnSubmit.Click
        Me.Save_FireSaveEvent(True)
        If sender Is btnSaveandGotoCoverages Then
            If Me.ValidationSummmary.HasErrors = False Then
                If IsQuoteEndorsement() AndAlso (HasShownRecentDialogOnce = False OrElse HasReplacedVehicle) Then
                    If hasRecentManufacturedVehicleWithoutAI() Then
                        ctlRecentManufacturedVehicle.Visible = True
                        HasShownRecentDialogOnce = True
                        HasReplacedVehicle = False
                        Return
                    End If
                End If

                Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
            End If
        End If
    End Sub

    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    Protected Sub btnViewGotoCoverages_Click(sender As Object, e As EventArgs) Handles btnViewGotoCoverages.Click
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
    End Sub


    Protected Sub btnAddvehicle_Click(sender As Object, e As EventArgs) Handles btnAddvehicle.Click
        If Me.Quote IsNot Nothing Then
            If Me.Quote.Vehicles Is Nothing Then
                Me.Quote.Vehicles = New List(Of QuickQuoteVehicle)()
            End If

            Dim newVehicle As New QuickQuoteVehicle()

            If Quote.Vehicles.Count = 0 Then
                Quote.HasBusinessMasterEnhancement = True
            End If

            Dim vehicleWithCoveragesToCopy As QuickQuoteVehicle = (From veh In Quote.Vehicles Where If(veh.BodyTypeId = "", "0", veh.BodyTypeId).TryToGetInt32 <> ENUMHelper.VehicleBodyType.bodyType_OtherTrailer AndAlso If(veh.BodyTypeId = "", "0", veh.BodyTypeId).TryToGetInt32 <> ENUMHelper.VehicleBodyType.bodyType_RecTrailer AndAlso Not veh.ComprehensiveCoverageOnly Select veh).FirstOrDefault()
            IFM.VR.Common.Helpers.PPA.PrefillHelper.SetNewVehicleDefaults_RetainTopLevelCoverages(newVehicle, Quote.QuickQuoteState, vehicleWithCoveragesToCopy)

            Me.Quote.Vehicles.Add(newVehicle)
            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            Me.Populate()
            Me.hidden_VehicleListActive.Value = (Me.Quote.Vehicles.Count() - 1).ToString()
        End If
    End Sub

    Protected Sub btnEndorsementReplaceVehicle_Click(sender As Object, e As EventArgs) Handles btnEndorsementReplaceVehicle.Click
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        ctlEndorsementReplaceVehicle.ReplacementIndex = -1
        ctlEndorsementReplaceVehicle.Populate()
        ctlEndorsementReplaceVehicle.Visible = True
    End Sub

    Protected Sub btnEndorsementRemoveVehicle_Click(sender As Object, e As EventArgs) Handles btnEndorsementRemoveVehicle.Click
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        ctlEndorsementRemoveVehicle.Populate()
        ctlEndorsementRemoveVehicle.Visible = True
    End Sub


    Private Sub vehicleControlNewVehicleReqested()
        btnAddvehicle_Click(Nothing, Nothing)
    End Sub

    Private Sub vehicleControlReplaceVehicleTitleBar(index As Integer)
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        ctlEndorsementReplaceVehicle.ReplacementIndex = index
        ctlEndorsementReplaceVehicle.Populate()
        ctlEndorsementReplaceVehicle.Visible = True
    End Sub

    Protected Sub ReplaceSelectedVehicle(CoverageRequest As CoverageRequestType, VehicleIndexNum As Integer) Handles ctlEndorsementReplaceVehicle.ReplaceSelectedVehicle
        Dim CoverageTypeRequest As CoverageRequestType = CoverageRequest
        Dim vehicleIndex = VehicleIndexNum
        Dim saveErr As String = String.Empty
        If Me.Quote IsNot Nothing Then

            If Me.Quote.Vehicles Is Nothing Then
                Me.Quote.Vehicles = New List(Of QuickQuoteVehicle)()
            End If

            Dim VehicleList As List(Of QuickQuoteVehicle) = Me.Quote.Vehicles
            Dim newVehicle As New QuickQuoteVehicle()

            Select Case CoverageRequest
                Case CoverageRequestType.RemoveCurrentCoverages
                    Me.Quote.Vehicles.RemoveAt(vehicleIndex)
                    IFM.VR.Common.Helpers.PPA.PrefillHelper.SetNewVehicleDefaults(newVehicle, Quote.QuickQuoteState)
                Case CoverageRequestType.RetainCurrentCoverages
                    newVehicle.Coverages = QQHelper.CloneObject(Me.Quote.Vehicles(vehicleIndex).Coverages)
                    Me.Quote.Vehicles.RemoveAt(vehicleIndex)
                    newVehicle.ParseThruCoverages()
            End Select

            Me.Quote.Vehicles.Add(newVehicle)
            Me.Populate()
            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))


            Me.hidden_VehicleListActive.Value = (Me.Quote.Vehicles.Count() - 1).ToString()
            HasReplacedVehicle = True
        End If
    End Sub

    Protected Sub RemoveSelectedVehicle(VehicleIndexNum As Integer) Handles ctlEndorsementRemoveVehicle.RemoveSelectedVehicle
        If Me.Quote IsNot Nothing Then
            If Me.Quote.Vehicles IsNot Nothing Then
                If Me.Quote.Vehicles.HasItemAtIndex(VehicleIndexNum) Then
                    Me.Quote.Vehicles.RemoveAt(VehicleIndexNum)
                    Me.Populate()
                    Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
                    Me.hidden_VehicleListActive.Value = (Me.Quote.Vehicles.Count() - 1).ToString()
                End If
            End If
        End If
    End Sub

    Protected Sub RecentManufacturedVehicle_OK() Handles ctlRecentManufacturedVehicle.RecentDateVehicleOK
        ' OK Button – user Is navigated to theCoverages page And the Lease/Loan checkbox Is Not checked
        ' on vehicles that are 5 years are newer (and new to the policy). 
        If Me.ValidationSummmary.HasErrors = False Then
            If hasRecentManufacturedVehicleWithoutAI() Then
                For Each index As Integer In LstOfNewVehicleIndexes
                    If Quote IsNot Nothing AndAlso Quote.Vehicles.HasItemAtIndex(index) Then
                        Quote.Vehicles(index).HasAutoLoanOrLease = False
                    End If
                Next
                Me.Save_FireSaveEvent(False)
            End If
            Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
        End If
    End Sub

    Protected Sub RecentManufacturedVehicle_Cancel() Handles ctlRecentManufacturedVehicle.RecentDateVehicleCancel
        ' Cancel button – user remains on theVehicle Page and the Additional Interest subsection is
        ' expanded and ready foruser input (no changes to required info and functionality of those
        ' fields: AI still must be Lienholder, Loss Payee or Lessor; Loan Lease coverage is only
        ' availablefor these vehicle body types: Car, SUV, Pickup, Pickup w/Camper,VanMotorcycle). 
        If hasRecentManufacturedVehicleWithoutAI() Then
            For Each index As Integer In LstOfNewVehicleIndexes
                Dim cntrl As RepeaterItem = Repeater1.Items(index)
                Dim vehicleControl As ctlVehicle_PPA = cntrl.FindControl("ctlVehicle_PPAControl")
                Dim AILoL As CheckBox = vehicleControl.FindControl("chkVehicleHasALienholderOrLease")
                AILoL.Checked = True
                vehicleControl.chkVehicleHasALienholderOrLease_CheckedChanged(Me, New EventArgs)
                Quote.Vehicles(index).HasAutoLoanOrLease = True
            Next
            hidden_VehicleListActive.Value = LstOfNewVehicleIndexes.First()
        End If

        Return
    End Sub

    Protected Function hasRecentManufacturedVehicleWithoutAI() As Boolean
        Dim HasNewVehicle As Boolean
        Dim index As Integer = 0
        LstOfNewVehicleIndexes = New List(Of Integer)
        For Each vehicle As QuickQuoteVehicle In Quote.Vehicles
            If QQHelper.IsQuickQuoteVehicleNewToImage(vehicle, Quote) Then
                'If CInt(vehicle.Year) > Date.Today.Year - 5 Then
                If CInt(vehicle.Year) > Date.Today.Year - 15 Then   ' Changed to 15 years per Bug 60188
                    If vehicle?.AdditionalInterests?.Count < 1 Then
                        HasNewVehicle = True
                        LstOfNewVehicleIndexes.Add(index)
                    End If
                End If
            End If
            index = index + 1
        Next
        Return HasNewVehicle
    End Function

    Protected Sub ClearVehicle(vehicle As QuickQuoteVehicle)
        vehicle.ActualCashValue = String.Empty
        vehicle.AdditionalInterests.Clear()
        vehicle.AnnualMiles = 0
        vehicle.AntiTheftTypeId = String.Empty
        vehicle.BodyTypeId = 0
        vehicle.CostNew = String.Empty
        vehicle.CubicCentimeters = String.Empty
        QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, "")
        vehicle.DriverOutOfStateSurcharge = False
        vehicle.GaragingAddress = New QuickQuoteGaragingAddress
        vehicle.HasAutoLoanOrLease = False
        vehicle.Make = String.Empty
        vehicle.Model = String.Empty
        vehicle.NonOwned = False
        vehicle.NonOwnedNamed = False
        vehicle.OccasionalDriver1Num = String.Empty
        vehicle.OccasionalDriver2Num = String.Empty
        vehicle.OccasionalDriver3Num = String.Empty
        vehicle.PerformanceTypeId = String.Empty
        vehicle.RestraintTypeId = String.Empty
        vehicle.ScheduledItems.Clear() '? will this clear coverages?
        vehicle.StatedAmount = String.Empty
        vehicle.UseCodeTypeId = 0
        vehicle.VehicleSymbols.Clear()
        vehicle.VehicleTypeId = String.Empty
        vehicle.VehicleUseTypeId = String.Empty
        vehicle.Vin = String.Empty
        vehicle.Year = String.Empty

        If vehicle.PrincipalDriverNum <> "" AndAlso vehicle.PrincipalDriverNum IsNot Nothing Then
            Dim enoDriver As QuickQuoteDriver = Quote.Drivers(Integer.Parse(vehicle.PrincipalDriverNum) - 1)
            enoDriver.ExtendedNonOwned = False
        End If

        vehicle.PrincipalDriverNum = String.Empty

        'MyVehicle.GaragingAddress.Address.HouseNum = String.Empty
        'MyVehicle.GaragingAddress.Address.StreetName = String.Empty
        'MyVehicle.GaragingAddress.Address.ApartmentNumber = String.Empty
        'MyVehicle.GaragingAddress.Address.Other = String.Empty
        'MyVehicle.GaragingAddress.Address.City = String.Empty
        'MyVehicle.GaragingAddress.Address.StateId = String.Empty
        'MyVehicle.GaragingAddress.Address.Zip = String.Empty
        'MyVehicle.GaragingAddress.Address.County = String.Empty

    End Sub

End Class