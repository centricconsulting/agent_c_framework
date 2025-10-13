Imports IFM.PrimativeExtensions
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects

Public Class ctl_CAP_VehicleList
    Inherits VRControlBase

    Public Event AIChanged() 'Added 05/10/2021 for CAP Endorsements 52974 MLW

    Public ReadOnly CAPEndorsementsDictionaryName = "CAPEndorsementsDetails" 'Added 04/01/2021 for CAP Endorsements Task 52974 MLW

    'Added 04/01/2021 for CAP Endorsements Task 52974 MLW
    Private Property _devDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
    Public ReadOnly Property ddh() As DevDictionaryHelper.DevDictionaryHelper
        Get
            If _devDictionaryHelper Is Nothing Then
                If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(CAPEndorsementsDictionaryName) = False Then
                    _devDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, CAPEndorsementsDictionaryName, Quote.LobType)
                End If
            End If
            Return _devDictionaryHelper
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Property ActiveVehicleIndex As String
        Get
            Return hdnAccord.Value
        End Get
        Set(value As String)
            hdnAccord.Value = value
        End Set
    End Property

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
        'Added 08/02/2021 for CAP Endorsements Task 53028 MLW 
        If ((Not IsQuoteReadOnly() AndAlso Not IsQuoteEndorsement()) OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Vehicle")) Then
            Me.VRScript.AddScriptLine("Cap.ToggleValidVIN(false);") 'Passing isOnAppPage = True/False because app gap has differently named buttons than quote side & endorsments that we are enabling/disabling and showing/hiding
        End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Protected Sub AttachVehicleControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim VehControl As ctl_CAP_Vehicle = cntrl.FindControl("ctl_CAP_Vehicle")
            AddHandler VehControl.NewVehicleRequested, AddressOf AddVehicleRequested
            AddHandler VehControl.DeleteVehicleRequested, AddressOf DeleteVehicleRequested
            AddHandler VehControl.CopyVehicleRequested, AddressOf CopyVehicleRequested
            AddHandler VehControl.NeedToRepopulateTopLevelAIs, AddressOf RepopulateTopLevelAIs 'added 6/14/2021 for CAP Endorsements Task 52974 MLW
            index += 1
        Next
    End Sub


    Public Overrides Sub Populate()
        divEndorsementMaxTransactionsMessage.Visible = False 'Added 04/01/2021 for CAP Endorsements task 52974 MLW
        'Added 12/23/2020 for CAP Endorsements Task 52974 MLW
        ctlVehicleAdditionalInterestList.Visible = False
        If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso (TypeOfEndorsement() = "Add/Delete Vehicle" OrElse TypeOfEndorsement() = "Add/Delete Additional Interest")) Then
            If Me.Quote.IsNotNull AndAlso Quote.Vehicles IsNot Nothing Then

                Me.Repeater1.DataSource = Me.Quote.Vehicles
                Me.Repeater1.DataBind()

                Me.FindChildVrControls()

                Dim lIndex As Int32 = 0
                For Each cnt In Me.GatherChildrenOfType(Of ctl_CAP_Vehicle)
                    cnt.VehicleIndex = lIndex
                    cnt.Populate()
                    lIndex += 1
                Next
                'Added 02/04/2021 for CAP Endorsements Task 52974 MLW
                If IsQuoteReadOnly() OrElse IsQuoteEndorsement() Then
                    ctlVehicleAdditionalInterestList.Visible = True

                    Dim aIndex As Int32 = 0
                    For Each cntl In Me.GatherChildrenOfType(Of ctlVehicleAdditionalInterestList)
                        cntl.VehicleIndex = aIndex
                        cntl.Populate()
                        aIndex += 1
                    Next
                End If
            End If

            'Added 11/24/2020 for CAP Endorsements Task 52981 MLW
            If Me.Quote IsNot Nothing Then
                If IsQuoteReadOnly() Then
                    Dim policyNumber As String = Me.Quote.PolicyNumber
                    Dim imageNum As Integer = 0
                    Dim policyId As Integer = 0
                    Dim toolTip As String = "Make a change to this policy"
                    Dim readOnlyViewPageUrl As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                    If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                        readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                    End If

                    divActionButtons.Visible = False
                    divEndorsementButtons.Visible = True

                    btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                    readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                    btnMakeAChange.ToolTip = toolTip
                    btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
                Else
                    'Added 04/01/2021 for CAP Endorsements Task 52974 MLW
                    If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Vehicle" Then
                        Dim transactionCount As Integer = ddh.GetEndorsementVehicleTransactionCount()
                        If transactionCount >= 3 Then
                            divEndorsementMaxTransactionsMessage.Visible = True
                            btnAddVehicle.Visible = False
                        Else
                            divEndorsementMaxTransactionsMessage.Visible = False
                            btnAddVehicle.Visible = True
                        End If
                    ElseIf IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Additional Interest" Then
                        btnAddVehicle.Visible = False
                    End If
                End If

                'Added 08/04/2021 for CAP Endorsements Task 53030 MLW
                If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Vehicle" Then
                    ctl_RouteToUw.Visible = True
                ElseIf (IsQuoteReadOnly() OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() <> "Add/Delete Vehicle")) Then
                    divUseVINLookupMessage.Visible = False
                End If
            End If
        End If
    End Sub

    'Added 07/29/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
    Private Function HasAllValidatedVINs() As Boolean
        Dim validatedVINs As Boolean = True
        If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Vehicle") Then
            Dim vinDDH As DevDictionaryHelper.DevDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, "ValidVIN", Quote.LobType)
            Dim validVINList As Dictionary(Of String, String) = vinDDH.GetMasterValueAsDictionary()
            If validVINList IsNot Nothing AndAlso validVINList.Count > 0 Then
                For Each vv In validVINList
                    If vv.Value = "False" Then
                        validatedVINs = False
                        Exit For
                    End If
                Next
            End If
        End If
        Return validatedVINs
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divMainList.ClientID
            Me.hdnAccord.Value = "0"
        End If

        'Added 05/20/2021 for CAP Endorsements 52974 MLW
        AddHandler Me.ctlVehicleAdditionalInterestList.AIChange, AddressOf HandleAIChange

        AttachVehicleControlEvents()
    End Sub

    Public Overrides Function Save() As Boolean
        'Updated 12/22/2020 for CAP Endorsements Task 52974 MLW
        If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso (TypeOfEndorsement() = "Add/Delete Vehicle" OrElse TypeOfEndorsement() = "Add/Delete Additional Interest")) Then
            Me.SaveChildControls()
            'Added 06/28/2021 for CAP Endorsements Task 52974 MLW
            If IsQuoteEndorsement() Then
                PopulateChildControls()
            End If

            ' Remove any incomplete drivers so we can rate.  This can happen if the user went to the app side then returned to the quote 
            ' side without completing a driver.  Bug 33646  MGB 7-29-2019
            Dim rems As Integer = IFM.VR.Common.Helpers.CAP.CAPDriverListHelper.RemoveInvalidDriversFromQuote(QuoteId, Quote)

            'after child control save so that you have the latest garaging address information
            IFM.VR.Common.Helpers.CAP.Locations.EnsureLocationExistsForEachSubQuote(Me.Quote, Me.SubQuotes)

            If ActiveVehicleIndex = "" Then
                ActiveVehicleIndex = "false"
            End If

            'added 6/14/2021 for CAP Endorsements Task 52974 MLW
            If needToRepopulateTopLevelAIs = True Then
                Me.ctlVehicleAdditionalInterestList.Populate()
                needToRepopulateTopLevelAIs = False
            End If
        End If

        If Not IsQuoteEndorsement() Then
            Dim IsFarmIndicator As Boolean = False
            If Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
                Dim capClassCodesToFind As New List(Of String) From {"61", "62", "69"}
                For Each v As QuickQuoteVehicle In Quote.Vehicles
                    If v IsNot Nothing AndAlso String.IsNullOrWhiteSpace(v.ClassCode) = False AndAlso v.ClassCode.Length = 5 Then
                        Dim stringToCompare As String = v.ClassCode.Substring(3, 2)
                        If capClassCodesToFind.Contains(stringToCompare) Then
                            IsFarmIndicator = True
                            Exit For ' Exit the loop once the target value is found
                        End If
                    End If
                Next
            End If
            For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In Me.SubQuotes
                sq.HasFarmIndicator = IsFarmIndicator
            Next
            Me.Quote.HasFarmIndicator = IsFarmIndicator
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Updated 12/22/2020 for CAP Endorsements Task 52974 MLW
        If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso (TypeOfEndorsement() = "Add/Delete Vehicle" OrElse TypeOfEndorsement() = "Add/Delete Additional Interest")) Then
            MyBase.ValidateControl(valArgs)
            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Private Sub CopyVehicleRequested(ByRef VehIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.HasItemAtIndex(VehIndex) Then
                'Dim veh As New QuickQuote.CommonObjects.QuickQuoteVehicle()
                'veh = Quote.Vehicles(VehIndex)
                'updated 7/9/2021 for Bug 62070 MLW
                Dim veh As QuickQuote.CommonObjects.QuickQuoteVehicle = QQHelper.CloneObject(Quote.Vehicles(VehIndex))
                If veh IsNot Nothing Then 'added IF 7/9/2021  for Bug 62070 MLW
                    'added 7/9/2021 to clear out specific fields from previous vehicle -  for Bug 62070 MLW
                    With veh
                        .AddedDate = ""
                        .AddedImageNum = ""
                        .EffectiveDate = ""
                        .LastModifiedDate = ""
                        .PCAdded_Date = ""
                        .VehicleNum = ""
                        .VehicleNum_CGLPart = ""
                        .VehicleNum_CIMPart = ""
                        .VehicleNum_CPRPart = ""
                        .VehicleNum_CRMPart = ""
                        .VehicleNum_GARPart = ""
                        .VehicleNum_MasterPart = ""
                        'Updated 2/7/2022 for bug 67033 MLW
                        If QQHelper.BitToBoolean(ConfigurationManager.AppSettings("Task67033_CAPNewBusinessCopyVehicle_DoNotCopyVIN")) = True Then
                            .Vin = ""
                        Else
                            'Added 08/24/2021 for Bug 64541 MLW
                            If IsQuoteEndorsement() Then
                                .Vin = ""
                            End If
                        End If
                        ''Added 08/24/2021 for Bug 64541 MLW
                        'If IsQuoteEndorsement() Then
                        '    .Vin = ""
                        'End If
                    End With

                    Quote.Vehicles.Add(veh)
                    'Added 04/02/2021 for CAP Endorsements Task 52974 MLW
                    If IsQuoteEndorsement() Then
                        Dim newVehIndex As Integer = 0
                        If Quote.Vehicles.Count > 0 Then
                            newVehIndex = Quote.Vehicles.Count - 1
                        End If
                        ddh.UpdateDevDictionaryVehicleList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, newVehIndex, veh)
                        Quote.TransactionReasonId = 10169 'Endorsement Change Dec and Full Revised Dec
                        Dim endorsementsRemarksHelper = New EndorsementsRemarksHelper(ddh)
                        Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(EndorsementsRemarksHelper.RemarksType.Vehicle)
                        Quote.TransactionRemark = updatedRemarks
                    End If
                    Populate()
                    Save_FireSaveEvent(False)
                    Me.hdnAccord.Value = Quote.Vehicles.Count - 1.ToString
                End If
            End If
        End If
    End Sub

    Private Sub DeleteVehicleRequested(ByRef VehIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.Vehicles IsNot Nothing Then
                If Quote.Vehicles.HasItemAtIndex(VehIndex) Then Quote.Vehicles.RemoveAt(VehIndex)
                Populate()
                Save_FireSaveEvent(False)
                'Updated 05/24/2021 for bug 62071 MLW
                Dim lastVehicleIndex As String = "0"
                If Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
                    lastVehicleIndex = (Me.Quote.Vehicles.Count - 1).ToString
                End If
                Me.hdnAccord.Value = lastVehicleIndex
                'Me.hdnAccord.Value = (Me.Quote.Vehicles.Count - 1).ToString
            End If
        End If
    End Sub

    Private Sub btnAddAddVehicle_Click(sender As Object, e As EventArgs) Handles btnAddVehicle.Click
        AddVehicleRequested()
    End Sub


    Private Sub AddVehicleRequested()
        Dim newVeh As New QuickQuote.CommonObjects.QuickQuoteVehicle()

        ' Make sure we save any existing data...
        Save_FireSaveEvent(False)

        If Quote IsNot Nothing Then
            If Quote.Vehicles Is Nothing Then Quote.Vehicles = New List(Of QuickQuote.CommonObjects.QuickQuoteVehicle)
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                ' If there's a location then it contains the Primary Garaging Address - set the vehicle garaging address to it
                newVeh.GaragingAddress.Address = Quote.Locations(0).Address
            Else
                ' There's no location - set garaging address to policyholder address
                newVeh.GaragingAddress.Address = Quote.Policyholder.Address
            End If
            Quote.Vehicles.Add(newVeh)
            'Added 04/02/2021 for CAP Endorsements Task 52974 MLW
            Dim newVehIndex As Integer = 0
            If Quote.Vehicles.Count > 0 Then
                newVehIndex = Quote.Vehicles.Count - 1
            End If
            If IsQuoteEndorsement() Then
                ddh.UpdateDevDictionaryVehicleList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, newVehIndex, newVeh)
                Quote.TransactionReasonId = 10169 'Endorsement Change Dec and Full Revised Dec
            End If
            'Added 08/02/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
            AddValidVINToDevDictionary(newVehIndex)

            Populate()
            Save_FireSaveEvent(False)
            Me.hdnAccord.Value = Quote.Vehicles.Count - 1.ToString

            ' Start Focus on vehicle - CH 09/15/2017
            Dim lastVehicle = Me.GatherChildrenOfType(Of ctl_CAP_Vehicle)().LastOrDefault() ' added 10-1-2015
            If lastVehicle IsNot Nothing Then
                lastVehicle.OpenAllParentAccordionsOnNextLoad(lastVehicle.ScrollToControlId)
            End If
        End If
    End Sub



    Private Sub btnSaveAndRate_Click(sender As Object, e As EventArgs) Handles btnSaveAndRate.Click
        'Session("valuationValue") = "False"
        'Save_FireSaveEvent(False) 'Removed 01/28/2021 MLW - Causes duplicate validation messages
        Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
    End Sub

    Private Sub btnSaveVehicles_Click(sender As Object, e As EventArgs) Handles btnSaveVehicles.Click
        Save_FireSaveEvent()
    End Sub

    'Added 11/24/2020 for CAP Endorsements task 52981 MLW
    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    'Added 11/24/2020 for CAP Endorsements task 52981 MLW
    Protected Sub btnViewBillingInformation_Click(sender As Object, e As EventArgs) Handles btnViewBillingInfo.Click
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub

    'Added 05/10/2021 for CAP Endorsements 52974 MLW
    Private Sub HandleAIChange()
        RaiseEvent AIChanged()
    End Sub

    'added 5/19/2021 for CAP Endorsements Task 52974 MLW
    Public Sub PopulateAppVehicleInfo()
        For Each cntl In Me.GatherChildrenOfType(Of ctl_CAP_Vehicle)
            cntl.PopulateAppVehicleInfo()
        Next
    End Sub

    'added 6/14/2021 for CAP Endorsements Task 52974 MLW
    Dim needToRepopulateTopLevelAIs As Boolean = False
    Private Sub RepopulateTopLevelAIs()
        needToRepopulateTopLevelAIs = True
    End Sub

    'Added 08/02/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
    Private Sub AddValidVINToDevDictionary(vehicleIndex As Integer)
        Dim vinDDH As DevDictionaryHelper.DevDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, "ValidVIN", Quote.LobType)
        vinDDH.AddToMasterValueDictionary(vehicleIndex + 1, "False") 'Add as false becaue we haven't given the user a chance to use the VIN lookup yet.
    End Sub

    Public Sub PopulateVehicleCoverages()
        For Each cntl In Me.GatherChildrenOfType(Of ctl_CAP_Vehicle)
            cntl.LoadCompCollStaticData()
            cntl.PopulateRACASymbols()
            Dim quoteToUse As QuickQuote.CommonObjects.QuickQuoteObject = cntl.GetQuoteToUse() 'should always return something if Me.Quote is something
            cntl.PopulateVehicleCoverages(quoteToUse)
        Next
    End Sub

End Class