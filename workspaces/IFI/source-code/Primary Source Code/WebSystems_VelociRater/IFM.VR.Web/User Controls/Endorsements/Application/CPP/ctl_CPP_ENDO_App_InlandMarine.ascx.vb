Imports DevDictionaryHelper
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CPP
Imports IFM.VR.Web.EndorsementStructures
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Public Class ctl_CPP_ENDO_App_InlandMarine
    Inherits VRControlBase

    Public Event AIChange()

#Region "Declarations"
    Public Structure Building_Struct
        Public LocIndex As Integer
        Public BldIndex As Integer
        Public building As QuickQuote.CommonObjects.QuickQuoteBuilding
    End Structure

    Public Structure FineArts_Struct
        Dim num As Integer
        Public LocIndex As Integer
        Public BldIndex As Integer
        Public FineArtsItem As QuickQuote.CommonObjects.QuickQuoteFineArtsScheduledItem
    End Structure

    Public Structure Signs_Struct
        Dim num As Integer
        Public LocIndex As Integer
        Public BldIndex As Integer
        Public SignsItem As QuickQuote.CommonObjects.QuickQuoteScheduledSign
    End Structure

    Private Enum IMType_enum
        BuildersRisk
        Computer
        Contractors
        FineArts
        InstallationFloater
        MotorTruckCargo
        OwnersCargo
        ScheduledProperty
        Signs
        Transportation
    End Enum

    Private ReadOnly Property HasBuildersRisk() As Boolean
        Get
            'If Not IsNullWhitespaceEmptyZeroOrNegative(GoverningStateQuote.BuildersRiskScheduledLocationsTotalLimit) Then Return True Else Return False
            Return False
        End Get
    End Property

    Private ReadOnly Property HasComputer() As Boolean
        Get
            'If Not IsNullWhitespaceEmptyZeroOrNegative(GoverningStateQuote.ComputerBuildingsTotalLimit) Then Return True Else Return False
            Return False
        End Get
    End Property

    Private ReadOnly Property HasContractors() As Boolean
        Get
            'If Not IsNullWhitespaceEmptyZeroOrNegative(GoverningStateQuote.ContractorsEquipmentCatastropheLimit) Then Return True Else Return False
            Return True
        End Get
    End Property

    Private ReadOnly Property HasFineArts() As Boolean
        Get
            'If Not IsNullWhitespaceEmptyZeroOrNegative(GoverningStateQuote.FineArtsBuildingsTotalLimit) Then Return True Else Return False
            Return False
        End Get
    End Property

    Private ReadOnly Property HasInstallationFloater() As Boolean
        Get
            'If Not IsNullWhitespaceEmptyZeroOrNegative(GoverningStateQuote.InstallationBlanketLimit) Then Return True Else Return False
            Return False
        End Get
    End Property

    Private ReadOnly Property HasMotorTruckCargo() As Boolean
        Get
            'If Not IsNullWhitespaceEmptyZeroOrNegative(GoverningStateQuote.MotorTruckCargoScheduledVehiclesTotalLimit) Then Return True Else Return False
            Return False
        End Get
    End Property

    Private ReadOnly Property HasOwnersCargo() As Boolean
        Get
            'If Not IsNullWhitespaceEmptyZeroOrNegative(GoverningStateQuote.OwnersCargoCatastropheLimit) Then Return True Else Return False
            Return False
        End Get
    End Property

    Private ReadOnly Property HasScheduledProperty() As Boolean
        Get
            'If Not IsNullWhitespaceEmptyZeroOrNegative(GoverningStateQuote.ScheduledPropertyItemsTotalLimit) Then Return True Else Return False
            Return False
        End Get
    End Property

    Private ReadOnly Property HasSigns() As Boolean
        Get
            'If Not IsNullWhitespaceEmptyZeroOrNegative(GoverningStateQuote.SignsBuildingTotalLimit) Then Return True Else Return False
            Return False
        End Get
    End Property

    Private ReadOnly Property HasTransportation() As Boolean
        Get
            'If Not IsNullWhitespaceEmptyZeroOrNegative(GoverningStateQuote.TransportationCatastropheLimit) Then Return True Else Return False
            Return False
        End Get
    End Property

    Private Enum OddOrEvenEnum
        Odd
        Even
    End Enum

    Private ReadOnly Property MyAIList As List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)
        Get
            Return Quote.AdditionalInterests
        End Get
    End Property
#End Region

    Private Property _CppDictItems As DevDictionaryHelper.AllCommercialDictionary
    Public ReadOnly Property CppDictItems As DevDictionaryHelper.AllCommercialDictionary
        Get
            If Quote IsNot Nothing Then
                If _CppDictItems Is Nothing Then
                    _CppDictItems = New DevDictionaryHelper.AllCommercialDictionary(Quote)
                End If
            End If
            Return _CppDictItems
        End Get
    End Property

    Public ReadOnly Property TransactionLimitReached As Boolean
        Get
            'get from Direct Parent ViewState
            If Me.ParentVrControl IsNot Nothing AndAlso TypeOf Me.ParentVrControl Is ctl_CPP_ENDO_InlandMarine Then
                Dim Parent = CType(ParentVrControl, ctl_CPP_ENDO_InlandMarine)
                Return Parent.TransactionLimitReached
            End If
            Return False
        End Get
    End Property

#Region "Methods and Functions"
    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkRemove.ClientID)
        Me.VRScript.CreateAccordion(divCPPAppIM.ClientID, hdnAccord, "0")
        'Me.VRScript.CreateAccordion(divBuildersRisk.ClientID, hdnBuildersRiskAccord, "0")
        'Me.VRScript.CreateAccordion(divComputers.ClientID, hdnComputersAccord, "0")
        Me.VRScript.CreateAccordion(divContractors.ClientID, hdnContractorsAccord, "0")
        'Me.VRScript.CreateAccordion(divFineArts.ClientID, hdnFineArtsAccord, "0")
        ''Me.VRScript.CreateAccordion(divInstallationFloater.ClientID, hdnInstallationAccord, "0")
        'Me.VRScript.CreateAccordion(divMotorTruckCargo.ClientID, hdnMotorTrucCargoAccord, "0")
        'Me.VRScript.CreateAccordion(divOwnersCargo.ClientID, hdnOwnersCargoAccord, "0")
        'Me.VRScript.CreateAccordion(divSchedulePropertyFloater.ClientID, hdnScheduledPropertyAccord, "0")
        'Me.VRScript.CreateAccordion(divSigns.ClientID, hdnSignsAccord, "0")
        'Me.VRScript.CreateAccordion(divTransportation.ClientID, hdnTransportationAccord, "0")

        'Me.VRScript.StopEventPropagation(Me.lbSaveBuildersRisk.ClientID)
        'Me.VRScript.StopEventPropagation(Me.lbSaveComputers.ClientID)
        Me.VRScript.StopEventPropagation(Me.lbSaveContractors.ClientID)
        'Me.VRScript.StopEventPropagation(Me.lbSaveFineArts.ClientID)
        ''Me.VRScript.StopEventPropagation(Me.lbSaveInstallation.ClientID)
        'Me.VRScript.StopEventPropagation(Me.lbSaveMotorTruckCargo.ClientID)
        'Me.VRScript.StopEventPropagation(Me.lbSaveOwnersCargo.ClientID)
        'Me.VRScript.StopEventPropagation(Me.lbSaveScheduledProperty.ClientID)
        'Me.VRScript.StopEventPropagation(Me.lbSaveSigns.ClientID)
        'Me.VRScript.StopEventPropagation(Me.lbSaveTransportation.ClientID)

        Exit Sub
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

#Region "Save Routines"

    Public Overrides Function Save() As Boolean
        'SaveBuildersRisk()
        'SaveComputers()
        SaveContractors()
        'SaveFineArts()
        ''SaveInstallationFloater()
        'SaveMotorTruckCargo()
        'SaveOwnersCargo()
        'SaveScheduledProperty()
        'SaveSigns()
        'SaveTransportation()

        Me.SaveChildControls()

        Return True
    End Function

    Private Sub SaveBuildersRisk()
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim ndx As Integer = -1

        If rptBuildersRisk.Items IsNot Nothing AndAlso rptBuildersRisk.Items.Count > 0 Then
            GoverningStateQuote.BuildersRiskAdditionalInterests = Nothing   ' Clear the AI list in case the coverage isn't on the quote
            If Not HasBuildersRisk() Then Exit Sub  ' No need to continue if the coverage isn't on the quote
            GoverningStateQuote.BuildersRiskAdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)   ' New the AI list

            For Each ri As RepeaterItem In rptBuildersRisk.Items
                ndx += 1
                ' Get the controls
                ddAIName = ri.FindControl("ddBuildersRiskAddlInterestName")
                ddAIType = ri.FindControl("ddBuildersRiskAddlInterestType")
                ddATIMA = ri.FindControl("ddBuildersRiskATIMA")
                If ddAIName.SelectedValue <> "" Then
                    ' Save the AI info to each item
                    Dim newAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
                    newAI.TypeId = ddAIType.SelectedValue
                    SetATIMAValueFromDropdown(ddATIMA, newAI)
                    newAI.ListId = ddAIName.SelectedValue
                    CopyQuoteAIValuesToInlandMarineAIItem(newAI, newAI.ListId, IMType_enum.BuildersRisk, ndx)
                    GoverningStateQuote.BuildersRiskAdditionalInterests.Add(newAI)
                End If
            Next
        End If

        Exit Sub
    End Sub

    Private Sub SaveComputers()
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim ndx As Integer = -1

        If rptComputers.Items IsNot Nothing AndAlso rptComputers.Items.Count > 0 Then
            GoverningStateQuote.ComputerAdditionalInterests = Nothing   ' Clear the AI list in case the coverage isn't on the quote
            If Not HasComputer() Then Exit Sub  ' No need to continue if the coverage isn't on the quote
            GoverningStateQuote.ComputerAdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)   ' New the AI list

            For Each ri As RepeaterItem In rptComputers.Items
                ndx += 1
                ' Get the controls
                ddAIName = ri.FindControl("ddComputersAddlInterestName")
                ddAIType = ri.FindControl("ddComputersAddlInterestType")
                ddATIMA = ri.FindControl("ddComputersATIMA")
                If ddAIName.SelectedValue <> "" Then
                    ' Save the AI info to each item
                    Dim newAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
                    newAI.TypeId = ddAIType.SelectedValue
                    SetATIMAValueFromDropdown(ddATIMA, newAI)
                    newAI.ListId = ddAIName.SelectedValue
                    CopyQuoteAIValuesToInlandMarineAIItem(newAI, newAI.ListId, IMType_enum.Computer, ndx)
                    GoverningStateQuote.ComputerAdditionalInterests.Add(newAI)
                End If
            Next
        End If

        Exit Sub
    End Sub

    Private Sub SaveContractors()
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim ddValuation As DropDownList = Nothing
        Dim txtLimit As TextBox = Nothing
        Dim txtDscr As TextBox = Nothing
        Dim ContractorsCoverages As List(Of QuickQuote.CommonObjects.QuickQuoteContractorsEquipmentScheduledCoverage) = Nothing
        Dim ndx As Integer = -1
        Dim ContractorsTotal As Decimal = 0
        Dim ContractorsCoverage As QuickQuote.CommonObjects.QuickQuoteContractorsEquipmentScheduledCoverage = Nothing

        If rptContractors.Items IsNot Nothing AndAlso rptContractors.Items.Count > 0 Then
            ' Get the contractors coverages list
            ContractorsCoverages = GoverningStateQuote.ContractorsEquipmentScheduledCoverages

            If Not HasContractors() Then Exit Sub  ' No need to continue if the coverage isn't on the quote

            ' Loop thru the repeater items and save
            For Each ri As RepeaterItem In rptContractors.Items
                ndx += 1
                ContractorsCoverage = ContractorsCoverages(ndx)

                ' Get the controls
                ddAIName = ri.FindControl("ddContractorsAddlInterestName")
                ddAIType = ri.FindControl("ddContractorsAddlInterestType")
                ddATIMA = ri.FindControl("ddContractorsATIMA")
                ddValuation = ri.FindControl("ddContractorsValuation")
                txtLimit = ri.FindControl("txtContractorsLimit")
                txtDscr = ri.FindControl("txtContractorsDescription")

                ' Limit
                ContractorsCoverage.ManualLimitAmount = txtLimit.Text
                If IsNumeric(txtLimit.Text) Then ContractorsTotal += CDec(txtLimit.Text)
                ' Description - Don't ever save a blank description
                If txtDscr.Text <> "" Then ContractorsCoverage.Description = txtDscr.Text.ToUpper
                ' Valuation
                ContractorsCoverage.ValuationMethodTypeId = ddValuation.SelectedValue

                ' Additional Interest Info
                ContractorsCoverage.AdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)
                If ddAIName.SelectedValue <> "" Then
                    ' Save the AI info to each item
                    Dim newAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
                    newAI.TypeId = ddAIType.SelectedValue
                    SetATIMAValueFromDropdown(ddATIMA, newAI)
                    newAI.ListId = ddAIName.SelectedValue
                    CopyQuoteAIValuesToInlandMarineAIItem(newAI, newAI.ListId, IMType_enum.Contractors, ndx)
                    ContractorsCoverage.AdditionalInterests.Add(newAI)
                End If

                Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
                AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
                Dim coverage As InlandMarineCoverageWithPremium = New InlandMarineCoverageWithPremium(ContractorsCoverage.Description, ContractorsCoverage.ManualLimitAmount, ContractorsCoverage.ScheduledCoverageNum, ddAIName.SelectedValue)
                Dim DoesPreExist As Boolean = AllPreExistingItems.PreExisting_InlandMarineCoveragesWithPremium.isPreExistingCoverage(coverage)
                If DoesPreExist = False Then
                    CppDictItems.UpdateInlaneMarineCoverageWithPremium(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, coverage)
                Else
                    AllPreExistingItems.PreExisting_InlandMarineCoveragesWithPremium.updatePreExistingCoverage(coverage)
                    AllPreExistingItems.SetAllPreExistingInDevDictionary(Quote)
                End If

            Next
        End If

        ' Update the header with the total amount for the items
        lblContractorsSectionTitle.Text = "Contractors Scheduled Equipment: " & String.Format("{0:C0}", ContractorsTotal)


        RaiseEvent AIChange()


        Exit Sub
    End Sub

    Private Sub SaveFineArts()
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim txtDesc As TextBox = Nothing
        Dim ndx As Integer = -1

        If rptFineArts.Items IsNot Nothing AndAlso rptFineArts.Items.Count > 0 Then
            GoverningStateQuote.FineArtsAdditionalInterests = Nothing   ' Clear the AI list in case the coverage isn't on the quote
            If Not HasFineArts() Then Exit Sub  ' No need to continue if the coverage isn't on the quote
            GoverningStateQuote.FineArtsAdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)   ' New the AI list

            For Each ri As RepeaterItem In rptFineArts.Items
                ndx += 1
                Dim fa As FineArts_Struct = ri.DataItem
                Dim fa_item As QuickQuote.CommonObjects.QuickQuoteFineArtsScheduledItem = GetFineArtsItem(fa.LocIndex, fa.BldIndex, ndx)

                ' Get the controls
                ddAIName = ri.FindControl("ddFineArtsAddlInterestName")
                ddAIType = ri.FindControl("ddFineArtsAddlInterestType")
                ddATIMA = ri.FindControl("ddFineArtsATIMA")
                txtDesc = ri.FindControl("txtFineArtsDescription")

                ' Description - Don't ever save a blank description
                If txtDesc.Text.Trim <> "" AndAlso fa_item IsNot Nothing Then fa_item.Description = txtDesc.Text

                ' Additional Interest Info
                If ddAIName.SelectedValue <> "" Then
                    ' Save the AI info to each item
                    Dim newAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
                    newAI.TypeId = ddAIType.SelectedValue
                    SetATIMAValueFromDropdown(ddATIMA, newAI)
                    newAI.ListId = ddAIName.SelectedValue
                    CopyQuoteAIValuesToInlandMarineAIItem(newAI, newAI.ListId, IMType_enum.FineArts, ndx)
                    GoverningStateQuote.FineArtsAdditionalInterests.Add(newAI)
                End If
            Next
        End If

        Exit Sub
    End Sub

    'Private Sub SaveInstallationFloater()
    '    Dim ddAIName As DropDownList = Nothing
    '    Dim ddAIType As DropDownList = Nothing
    '    Dim ddATIMA As DropDownList = Nothing
    '    Dim ndx As Integer = -1

    '    ' (NO REPEATER)

    '    If Not HasInstallationFloater() Then Exit Sub  ' No need to continue if the coverage isn't on the quote
    '    ndx = 0

    '    ' Get the controls
    '    ddAIName = ddInstallationFloaterAddlInterestName
    '    ddAIType = ddInstallationFloaterAddlInterestType
    '    ddATIMA = ddInstallationFloaterATIMA
    '    If ddAIName.SelectedValue <> "" Then
    '        ' Only going to be one ai for installation so NEW the list before saving
    '        Quote.InstallationAdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)
    '        Dim newAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
    '        newAI.TypeId = ddAIType.SelectedValue
    '        SetATIMAValueFromDropdown(ddATIMA, newAI)
    '        newAI.ListId = ddAIName.SelectedValue
    '        CopyQuoteAIValuesToInlandMarineAIItem(newAI, newAI.ListId, IMType_enum.InstallationFloater, ndx)
    '        Quote.InstallationAdditionalInterests.Add(newAI)
    '    End If

    '    Exit Sub
    'End Sub

    Private Sub SaveMotorTruckCargo()
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim txtYear As TextBox = Nothing
        Dim txtMake As TextBox = Nothing
        Dim txtModel As TextBox = Nothing
        Dim txtVIN As TextBox = Nothing
        Dim ndx As Integer = -1
        If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
            ' (NO REPEATER)
            If Not HasMotorTruckCargo() Then Exit Sub  ' No need to continue if the coverage isn't on the quote
            ' Get the controls
            ddAIName = ddUnScheduledMotorTruckCargoAddlInterestName
            ddAIType = ddUnScheduledMotorTruckCargoAddlInterestType
            ddATIMA = ddUnScheduledMotorTruckCargoATIMA

            ' Additional Interest info
            If ddAIName.SelectedValue <> "" Then
                GoverningStateQuote.MotorTruckCargoUnScheduledVehicleAdditionalInterests = New List(Of QuickQuoteAdditionalInterest)   ' New the AI list
                Dim newAI As New QuickQuoteAdditionalInterest()
                newAI.TypeId = ddAIType.SelectedValue
                SetATIMAValueFromDropdown(ddATIMA, newAI)
                newAI.ListId = ddAIName.SelectedValue
                CopyQuoteAIValuesToInlandMarineAIItem(newAI, newAI.ListId, IMType_enum.MotorTruckCargo, 0)
                GoverningStateQuote.MotorTruckCargoUnScheduledVehicleAdditionalInterests.Add(newAI)
            End If
        Else
            If rptMotorTruckCargo.Items IsNot Nothing AndAlso rptMotorTruckCargo.Items.Count > 0 Then
                GoverningStateQuote.MotorTruckCargoScheduledVehicleAdditionalInterests = Nothing   ' Clear the AI list in case the coverage isn't on the quote
                If Not HasMotorTruckCargo() Then Exit Sub  ' No need to continue if the coverage isn't on the quote
                GoverningStateQuote.MotorTruckCargoScheduledVehicleAdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)   ' New the AI list

                For Each ri As RepeaterItem In rptMotorTruckCargo.Items
                    ndx += 1
                    ' Get the controls
                    ddAIName = ri.FindControl("ddMotorTruckCargoAddlInterestName")
                    ddAIType = ri.FindControl("ddMotorTruckCargoAddlInterestType")
                    ddATIMA = ri.FindControl("ddMotorTruckCargoATIMA")
                    txtYear = ri.FindControl("txtYear")
                    txtMake = ri.FindControl("txtMake")
                    txtModel = ri.FindControl("txtModel")
                    txtVIN = ri.FindControl("txtVIN")

                    ' Vehicle Info
                    GoverningStateQuote.MotorTruckCargoScheduledVehicles(ndx).Year = txtYear.Text
                    GoverningStateQuote.MotorTruckCargoScheduledVehicles(ndx).Make = txtMake.Text
                    GoverningStateQuote.MotorTruckCargoScheduledVehicles(ndx).Model = txtModel.Text
                    GoverningStateQuote.MotorTruckCargoScheduledVehicles(ndx).VIN = txtVIN.Text

                    ' Additional Interest info
                    If ddAIName.SelectedValue <> "" Then
                        ' Save the AI info to each item
                        Dim newAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
                        newAI.TypeId = ddAIType.SelectedValue
                        SetATIMAValueFromDropdown(ddATIMA, newAI)
                        newAI.ListId = ddAIName.SelectedValue
                        CopyQuoteAIValuesToInlandMarineAIItem(newAI, newAI.ListId, IMType_enum.MotorTruckCargo, ndx)
                        GoverningStateQuote.MotorTruckCargoScheduledVehicleAdditionalInterests.Add(newAI)
                    End If
                Next
            End If
        End If

        Exit Sub
    End Sub

    Private Sub SaveOwnersCargo()
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim ndx As Integer = -1

        ' (NO REPEATER)

        If Not HasOwnersCargo() Then Exit Sub  ' No need to continue if the coverage isn't on the quote

        ' Get the controls
        ddAIName = ddOwnersCargoAddlInterestName
        ddAIType = ddOwnersCargoAddlInterestType
        ddATIMA = ddOwnersCargoATIMA
        If ddAIName.SelectedValue <> "" Then
            ' Only going to be one ai for owners cargo so NEW the list before saving
            GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleAdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)
            Dim newAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
            newAI.TypeId = ddAIType.SelectedValue
            SetATIMAValueFromDropdown(ddATIMA, newAI)
            newAI.ListId = ddAIName.SelectedValue
            CopyQuoteAIValuesToInlandMarineAIItem(newAI, newAI.ListId, IMType_enum.OwnersCargo, 0)
            GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleAdditionalInterests.Add(newAI)
        End If

        Exit Sub
    End Sub

    Private Sub SaveScheduledProperty()
        Dim txtDesc As TextBox = Nothing
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim ndx As Integer = -1
        Dim PropertyItem As QuickQuote.CommonObjects.QuickQuoteScheduledPropertyItem = Nothing

        If rptScheduledPropertyFloater.Items IsNot Nothing AndAlso rptScheduledPropertyFloater.Items.Count > 0 Then
            GoverningStateQuote.ScheduledPropertyAdditionalInterests = Nothing   ' Clear the AI list in case the coverage isn't on the quote
            If Not HasScheduledProperty() Then Exit Sub  ' No need to continue if the coverage isn't on the quote
            GoverningStateQuote.ScheduledPropertyAdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)   ' New the AI list

            For Each ri As RepeaterItem In rptScheduledPropertyFloater.Items
                ndx += 1

                PropertyItem = GoverningStateQuote.ScheduledPropertyItems(ndx)
                'PropertyItem = ri.DataItem

                ' Get the controls
                ddAIName = ri.FindControl("ddScheduledPropertyAddlInterestName")
                ddAIType = ri.FindControl("ddScheduledPropertyInterestType")
                ddATIMA = ri.FindControl("ddScheduledPropertyATIMA")
                txtDesc = ri.FindControl("txtScheduledPropertyDescription")

                ' Description - Don't ever save an empty description
                If txtDesc.Text.Trim <> "" AndAlso PropertyItem IsNot Nothing Then PropertyItem.Description = txtDesc.Text

                If ddAIName.SelectedValue <> "" Then
                    ' Save the AI info to each item
                    Dim newAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
                    newAI.TypeId = ddAIType.SelectedValue
                    SetATIMAValueFromDropdown(ddATIMA, newAI)
                    newAI.ListId = ddAIName.SelectedValue
                    CopyQuoteAIValuesToInlandMarineAIItem(newAI, newAI.ListId, IMType_enum.ScheduledProperty, ndx)
                    GoverningStateQuote.ScheduledPropertyAdditionalInterests.Add(newAI)
                End If
            Next
        End If

        Exit Sub
    End Sub

    Private Sub SaveSigns()
        Dim txtDesc As TextBox = Nothing
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim ndx As Integer = -1
        Dim SignsItem As Signs_Struct = Nothing
        Dim Sign As QuickQuote.CommonObjects.QuickQuoteScheduledSign = Nothing

        If rptSigns.Items IsNot Nothing AndAlso rptSigns.Items.Count > 0 Then
            GoverningStateQuote.SignsAdditionalInterests = Nothing   ' Clear the AI list in case the coverage isn't on the quote
            If Not HasSigns() Then Exit Sub  ' No need to continue if the coverage isn't on the quote
            GoverningStateQuote.SignsAdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)   ' New the AI list

            For Each ri As RepeaterItem In rptSigns.Items
                ndx += 1

                SignsItem = ri.DataItem
                Sign = GetSignsItem(SignsItem.LocIndex, SignsItem.BldIndex, ndx)

                ' Get the controls
                txtDesc = ri.FindControl("txtSignsDescription")
                ddAIName = ri.FindControl("ddSignsAddlInterestName")
                ddAIType = ri.FindControl("ddSignsAddlInterestType")
                ddATIMA = ri.FindControl("ddSignsATIMA")

                ' Description - Don't ever save a blank description
                If txtDesc.Text.Trim <> "" AndAlso Sign IsNot Nothing Then Sign.Description = txtDesc.Text

                If ddAIName.SelectedValue <> "" Then
                    ' Save the AI info to each item
                    Dim newAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
                    newAI.TypeId = ddAIType.SelectedValue
                    SetATIMAValueFromDropdown(ddATIMA, newAI)
                    newAI.ListId = ddAIName.SelectedValue
                    CopyQuoteAIValuesToInlandMarineAIItem(newAI, newAI.ListId, IMType_enum.Signs, ndx)
                    GoverningStateQuote.SignsAdditionalInterests.Add(newAI)
                End If
            Next
        End If

        Exit Sub
    End Sub

    Private Sub SaveTransportation()
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim txtDesc As TextBox = Nothing
        Dim ndx As Integer = -1

        ' (NO REPEATER)

        If Not HasTransportation() Then Exit Sub  ' No need to continue if the coverage isn't on the quote

        ' Get the controls
        ddAIName = ddTransportationAddlInterestName
        ddAIType = ddTransportationAddlInterestType
        ddATIMA = ddTransportationATIMA

        ' Description - Don't ever save an empty description
        If txtTransportationDescription.Text.Trim <> "" Then GoverningStateQuote.TransportationCatastropheDescription = txtTransportationDescription.Text

        If ddAIName.SelectedValue <> "" Then
            ' Only going to be one ai for owners cargo so NEW the list before saving
            GoverningStateQuote.TransportationCatastropheAdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)
            Dim newAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
            newAI.TypeId = ddAIType.SelectedValue
            SetATIMAValueFromDropdown(ddATIMA, newAI)
            newAI.ListId = ddAIName.SelectedValue
            CopyQuoteAIValuesToInlandMarineAIItem(newAI, newAI.ListId, IMType_enum.Transportation, 0)
            GoverningStateQuote.TransportationCatastropheAdditionalInterests.Add(newAI)
        End If

        Exit Sub
    End Sub

#End Region

#Region "Validate Routines"

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        ValidationHelper.GroupName = "Inland Marine"

        'If HasBuildersRisk() Then ValidateBuildersRisk(valArgs, accordList)
        'If HasComputer() Then ValidateComputers(valArgs, accordList)
        If HasContractors() Then ValidateContractors(valArgs, accordList)
        'If HasFineArts() Then ValidateFineArts(valArgs, accordList)
        ''If HasInstallationFloater() Then ValidateInstallationFloater(valArgs, accordList)
        'If HasMotorTruckCargo() Then ValidateMotorTruckCargo(valArgs, accordList)
        'If HasOwnersCargo() Then ValidateOwnersCargo(valArgs, accordList)
        'If HasScheduledProperty() Then ValidateScheduledProperty(valArgs, accordList)
        'If HasSigns() Then ValidateSigns(valArgs, accordList)
        'If HasTransportation() Then ValidateTransportation(valArgs, accordList)

        Me.ValidateChildControls(valArgs)
    End Sub

    ''' <summary>
    ''' Validates the passed AI fields
    ''' </summary>
    ''' <param name="ValArgs"></param>
    ''' <param name="accordList"></param>
    ''' <param name="ddAIName"></param>
    ''' <param name="ddAIType"></param>
    ''' <param name="ddATIMA"></param>
    Private Sub ValidateAI(ByRef newValidationHelper As ControlValidationHelper, ByRef ValArgs As VRValidationArgs, ByRef accordList As List(Of VRAccordionTogglePair), ByVal ddAIName As DropDownList, ByVal ddAIType As DropDownList, ByVal ddATIMA As DropDownList)
        If ddAIName.SelectedValue <> "" Then
            ' If N/A is not selected in the name dropdown then the AI type is required
            If ddAIType.SelectedValue = "" Then
                newValidationHelper.AddError(ddAIType, "Missing AI Type", accordList)
                'Me.ValidationHelper.AddError(ddAIType, "Missing AI Type", accordList)
            End If
        End If

        Exit Sub
    End Sub

    Private Sub ValidateBuildersRisk(ByRef valArgs As VRValidationArgs, ByRef accordList As List(Of VRAccordionTogglePair))
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim ndx As Integer = -1

        If rptBuildersRisk.Items IsNot Nothing AndAlso rptBuildersRisk.Items.Count > 0 Then
            For Each ri As RepeaterItem In rptBuildersRisk.Items
                ndx += 1

                Dim newValidationHelper As New ControlValidationHelper()
                newValidationHelper.GroupName = "Builders Risk Location #" & (ndx + 1).ToString
                Me.ValidationSummmary.RegisterValidationHelper(newValidationHelper)
                'ValidationHelper.GroupName = "Builders Risk Location #" & (ndx + 1).ToString

                ' Get the controls
                ddAIName = ri.FindControl("ddBuildersRiskAddlInterestName")
                ddAIType = ri.FindControl("ddBuildersRiskAddlInterestType")
                ddATIMA = ri.FindControl("ddBuildersRiskATIMA")

                ' Validate AI info
                'ValidateAI(valArgs, accordList, ddAIName, ddAIType, ddATIMA)
                ValidateAI(newValidationHelper, valArgs, accordList, ddAIName, ddAIType, ddATIMA)
            Next
        End If

        Exit Sub
    End Sub

    Private Sub ValidateComputers(ByRef valArgs As VRValidationArgs, ByRef accordList As List(Of VRAccordionTogglePair))
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim ndx As Integer = -1

        If rptComputers.Items IsNot Nothing AndAlso rptComputers.Items.Count > 0 Then
            For Each ri As RepeaterItem In rptComputers.Items
                ndx += 1

                Dim newValidationHelper As New ControlValidationHelper()
                newValidationHelper.GroupName = "Computers #" & (ndx + 1).ToString
                Me.ValidationSummmary.RegisterValidationHelper(newValidationHelper)
                'ValidationHelper.GroupName = "Computers #" & (ndx + 1).ToString

                ' Get the controls
                ddAIName = ri.FindControl("ddComputersAddlInterestName")
                ddAIType = ri.FindControl("ddComputersAddlInterestType")
                ddATIMA = ri.FindControl("ddComputersATIMA")

                ' VALIDATE USER INPUT FIELDS

                ' Validate AI info
                ValidateAI(newValidationHelper, valArgs, accordList, ddAIName, ddAIType, ddATIMA)
            Next
        End If

        Exit Sub
    End Sub

    Private Sub ValidateContractors(ByRef valArgs As VRValidationArgs, ByRef accordList As List(Of VRAccordionTogglePair))
        Dim txtLimit As TextBox = Nothing
        Dim ddValuation As DropDownList = Nothing
        Dim txtDscr As TextBox = Nothing
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing

        Dim ndx As Integer = -1

        Dim scheduledItemsTotalLimit As Double = 0.0 'added 10/21/2020 (Interoperability)

        If rptContractors.Items IsNot Nothing AndAlso rptContractors.Items.Count > 0 Then
            For Each ri As RepeaterItem In rptContractors.Items
                ndx += 1

                Dim newValidationHelper As New ControlValidationHelper()
                newValidationHelper.GroupName = "Contractors Item #" & (ndx + 1).ToString
                Me.ValidationSummmary.RegisterValidationHelper(newValidationHelper)
                'ValidationHelper.GroupName = "Contractors Item #" & (ndx + 1).ToString

                ' Get the controls
                txtLimit = ri.FindControl("txtContractorsLimit")
                ddValuation = ri.FindControl("ddContractorsValuation")
                txtDscr = ri.FindControl("txtContractorsDescription")
                ddAIName = ri.FindControl("ddContractorsAddlInterestName")
                ddAIType = ri.FindControl("ddContractorsAddlInterestType")
                ddATIMA = ri.FindControl("ddContractorsATIMA")

                ' VALIDATE USER INPUT FIELDS
                ' Limit
                If txtLimit.Text = "" Then
                    'Me.ValidationHelper.AddError(txtLimit, "Missing Limit", accordList)
                    'updated 10/21/2020 (Interoperability)
                    newValidationHelper.AddError(txtLimit, "Missing Limit", accordList)
                ElseIf Not IsNumeric(txtLimit.Text) Then
                    'Me.ValidationHelper.AddError(txtLimit, "Invalid Limit", accordList)
                    'updated 10/21/2020 (Interoperability)
                    newValidationHelper.AddError(txtLimit, "Invalid Limit", accordList)
                ElseIf CDec(txtLimit.Text) <= 0 Then
                    'Me.ValidationHelper.AddError(txtLimit, "Invalid Limit", accordList)
                    'updated 10/21/2020 (Interoperability)
                    newValidationHelper.AddError(txtLimit, "Invalid Limit", accordList)
                Else 'added 10/21/2020 for CE
                    Dim itemLimit As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtLimit.Text)
                    If itemLimit > 500000 Then
                        'newValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtLimit.ClientID)
                        newValidationHelper.AddError(txtLimit, "You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", accordList)
                    End If
                    scheduledItemsTotalLimit += itemLimit
                End If
                ' Valuation - will always have a value
                ' Description
                If txtDscr.Text = "" Then
                    'Me.ValidationHelper.AddError(txtDscr, "Missing Description", accordList)
                    'updated 10/21/2020 (Interoperability)
                    newValidationHelper.AddError(txtDscr, "Missing Description", accordList)
                End If

                ' Validate AI info
                ValidateAI(newValidationHelper, valArgs, accordList, ddAIName, ddAIType, ddATIMA)
            Next

            'added 10/21/2020
            Dim govStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = Me.GoverningStateQuote
            If govStateQuote IsNot Nothing Then
                Dim totalLimit As Double = 0.0
                totalLimit += scheduledItemsTotalLimit
                totalLimit += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(govStateQuote.ContractorsEquipmentLeasedRentedFromOthersLimit)
                totalLimit += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(govStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit)
                If totalLimit > 1000000 Then
                    'Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.") 'would show Inland Marine as the section title
                    'Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", changeTitle:=True, titleText:="Contractor Equipment") 'don't use this... changes the popup title
                    Dim totalLimitValidationHelper As New ControlValidationHelper()
                    totalLimitValidationHelper.GroupName = "Contractor Equipment"
                    Me.ValidationSummmary.RegisterValidationHelper(totalLimitValidationHelper)
                    totalLimitValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.")
                End If
            End If
        End If

        Exit Sub
    End Sub

    Private Sub ValidateFineArts(ByRef valArgs As VRValidationArgs, ByRef accordList As List(Of VRAccordionTogglePair))
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim txtDesc As TextBox = Nothing
        Dim ndx As Integer = -1

        If rptFineArts IsNot Nothing AndAlso rptFineArts.Items.Count > 0 Then
            For Each fa As RepeaterItem In rptFineArts.Items
                ndx += 1

                Dim newValidationHelper As New ControlValidationHelper()
                newValidationHelper.GroupName = "Fine Arts Item #" & (ndx + 1).ToString
                Me.ValidationSummmary.RegisterValidationHelper(newValidationHelper)
                'ValidationHelper.GroupName = "Fine Arts Item #" & (ndx + 1).ToString

                txtDesc = fa.FindControl("txtFineArtsDescription")
                ddAIName = fa.FindControl("ddFineArtsAddlInterestName")
                ddAIType = fa.FindControl("ddFineArtsAddlInterestType")
                ddATIMA = fa.FindControl("ddFineArtsATIMA")

                If txtDesc.Text.Trim = "" Then
                    'Me.ValidationHelper.AddError(txtDesc, "Missing Description", accordList)
                    'updated 10/22/2020 (Interoperability)
                    newValidationHelper.AddError(txtDesc, "Missing Description", accordList)
                End If

                ' Validate AI info
                ValidateAI(newValidationHelper, valArgs, accordList, ddAIName, ddAIType, ddATIMA)
            Next
        End If

        Exit Sub
    End Sub

    'Private Sub ValidateInstallationFloater(ByRef valArgs As VRValidationArgs, ByRef accordList As List(Of VRAccordionTogglePair))
    '    Dim ddAIName As DropDownList = Nothing
    '    Dim ddAIType As DropDownList = Nothing
    '    Dim ddATIMA As DropDownList = Nothing

    '    Dim newValidationHelper As New ControlValidationHelper()
    '    newValidationHelper.GroupName = "Installation Floater"
    '    'ValidationHelper.GroupName = "Installation Floater"

    '    ddAIName = ddInstallationFloaterAddlInterestName
    '    ddAIType = ddInstallationFloaterAddlInterestType
    '    ddATIMA = ddInstallationFloaterATIMA

    '    ' Validate AI info
    '    ValidateAI(newValidationHelper, valArgs, accordList, ddAIName, ddAIType, ddATIMA)

    '    Me.ValidationSummmary.InsertValidationControl(newValidationHelper)

    '    Exit Sub
    'End Sub

    Private Sub ValidateMotorTruckCargo(ByRef valArgs As VRValidationArgs, ByRef accordList As List(Of VRAccordionTogglePair))
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim txtYear As TextBox = Nothing
        Dim txtMake As TextBox = Nothing
        Dim txtModel As TextBox = Nothing
        Dim txtVIN As TextBox = Nothing
        Dim ndx As Integer = -1
        Dim newValidationHelper As New ControlValidationHelper()
        If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then

            newValidationHelper.GroupName = "Motor Truck Cargo"
            Me.ValidationSummmary.RegisterValidationHelper(newValidationHelper)

            ddAIName = ddUnScheduledMotorTruckCargoAddlInterestName
            ddAIType = ddUnScheduledMotorTruckCargoAddlInterestType
            ddATIMA = ddUnScheduledMotorTruckCargoATIMA

            ' Validate AI info
            ValidateAI(newValidationHelper, valArgs, accordList, ddAIName, ddAIType, ddATIMA)
        Else
            If rptMotorTruckCargo IsNot Nothing AndAlso rptMotorTruckCargo.Items.Count > 0 Then
                For Each ri As RepeaterItem In rptMotorTruckCargo.Items
                    ndx += 1
                    newValidationHelper.GroupName = "Motor Truck Cargo Vehicle #" & (ndx + 1).ToString
                    Me.ValidationSummmary.RegisterValidationHelper(newValidationHelper)
                    'ValidationHelper.GroupName = "Motor Truck Cargo Vehicle #" & (ndx + 1).ToString

                    txtYear = ri.FindControl("txtYear")
                    txtMake = ri.FindControl("txtmake")
                    txtModel = ri.FindControl("txtModel")
                    txtVIN = ri.FindControl("txtVIN")
                    ddAIName = ri.FindControl("ddMotorTruckCargoAddlInterestName")
                    ddAIType = ri.FindControl("ddMotorTruckCargoAddlInterestType")
                    ddATIMA = ri.FindControl("ddMotorTruckCargoATIMA")

                    ' Year
                    If txtYear.Text.Trim = "" Then
                        'Me.ValidationHelper.AddError(txtYear, "Missing Year", accordList)
                        'updated 10/22/2020 (Interoperability)
                        newValidationHelper.AddError(txtYear, "Missing Year", accordList)
                    Else
                        If Not YearOK(txtYear.Text) Then
                            'Me.ValidationHelper.AddError(txtYear, "Invalid Year", accordList)
                            'updated 10/22/2020 (Interoperability)
                            newValidationHelper.AddError(txtYear, "Invalid Year", accordList)
                        End If
                    End If

                    ' Make
                    If txtMake.Text.Trim = "" Then
                        'Me.ValidationHelper.AddError(txtMake, "Missing Make", accordList)
                        'updated 10/22/2020 (Interoperability)
                        newValidationHelper.AddError(txtMake, "Missing Make", accordList)
                    End If

                    ' Model
                    If txtModel.Text.Trim = "" Then
                        'Me.ValidationHelper.AddError(txtModel, "Missing Model", accordList)
                        'updated 10/22/2020 (Interoperability)
                        newValidationHelper.AddError(txtModel, "Missing Model", accordList)
                    End If

                    ' VIN
                    If txtVIN.Text.Trim = "" Then
                        'Me.ValidationHelper.AddError(txtVIN, "Missing VIN", accordList)
                        'updated 10/22/2020 (Interoperability)
                        newValidationHelper.AddError(txtVIN, "Missing VIN", accordList)
                    End If
                    ' Validate AI info
                    ValidateAI(newValidationHelper, valArgs, accordList, ddAIName, ddAIType, ddATIMA)
                Next
            End If
        End If

        Exit Sub
    End Sub

    Private Sub ValidateOwnersCargo(ByRef valArgs As VRValidationArgs, ByRef accordList As List(Of VRAccordionTogglePair))
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing

        Dim newValidationHelper As New ControlValidationHelper()
        newValidationHelper.GroupName = "Owners Cargo"
        Me.ValidationSummmary.RegisterValidationHelper(newValidationHelper)
        'ValidationHelper.GroupName = "Owners Cargo"

        ddAIName = ddOwnersCargoAddlInterestName
        ddAIType = ddOwnersCargoAddlInterestType
        ddATIMA = ddOwnersCargoATIMA

        ' Validate AI info
        ValidateAI(newValidationHelper, valArgs, accordList, ddAIName, ddAIType, ddATIMA)

        Exit Sub
    End Sub

    Private Sub ValidateScheduledProperty(ByRef valArgs As VRValidationArgs, ByRef accordList As List(Of VRAccordionTogglePair))
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim txtDesc As TextBox = Nothing
        Dim ndx As Integer = -1

        If rptScheduledPropertyFloater IsNot Nothing AndAlso rptScheduledPropertyFloater.Items.Count > 0 Then
            For Each sp As RepeaterItem In rptScheduledPropertyFloater.Items
                ndx += 1

                Dim newValidationHelper As New ControlValidationHelper()
                newValidationHelper.GroupName = "Scheduled Property Item #" & (ndx + 1).ToString
                Me.ValidationSummmary.RegisterValidationHelper(newValidationHelper)
                'ValidationHelper.GroupName = "Scheduled Property Item #" & (ndx + 1).ToString

                txtDesc = sp.FindControl("txtScheduledPropertyDescription")
                ddAIName = sp.FindControl("ddScheduledPropertyAddlInterestName")
                ddAIType = sp.FindControl("ddScheduledPropertyInterestType")
                ddATIMA = sp.FindControl("ddScheduledPropertyATIMA")

                If txtDesc.Text.Trim = "" Then
                    'Me.ValidationHelper.AddError(txtDesc, "Missing Description", accordList)
                    'updated 10/22/2020 (Interoperability)
                    newValidationHelper.AddError(txtDesc, "Missing Description", accordList)
                End If

                ' Validate AI info
                ValidateAI(newValidationHelper, valArgs, accordList, ddAIName, ddAIType, ddATIMA)
            Next
        End If

        Exit Sub
    End Sub

    Private Sub ValidateSigns(ByRef valArgs As VRValidationArgs, ByRef accordList As List(Of VRAccordionTogglePair))
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim txtDesc As TextBox = Nothing
        Dim ndx As Integer = -1

        If rptSigns IsNot Nothing AndAlso rptSigns.Items.Count > 0 Then
            For Each sp As RepeaterItem In rptSigns.Items
                ndx += 1

                Dim newValidationHelper As New ControlValidationHelper()
                newValidationHelper.GroupName = "Signs #" & (ndx + 1).ToString
                Me.ValidationSummmary.RegisterValidationHelper(newValidationHelper)
                'ValidationHelper.GroupName = "Signs #" & (ndx + 1).ToString

                txtDesc = sp.FindControl("txtSignsDescription")
                ddAIName = sp.FindControl("ddSignsAddlInterestName")
                ddAIType = sp.FindControl("ddSignsAddlInterestType")
                ddATIMA = sp.FindControl("ddSignsATIMA")

                If txtDesc.Text.Trim = "" Then
                    'Me.ValidationHelper.AddError(txtDesc, "Missing Description", accordList)
                    'updated 10/22/2020 (Interoperability)
                    newValidationHelper.AddError(txtDesc, "Missing Description", accordList)
                End If

                ' Validate AI info
                ValidateAI(newValidationHelper, valArgs, accordList, ddAIName, ddAIType, ddATIMA)
            Next
        End If

        Exit Sub
    End Sub

    Private Sub ValidateTransportation(ByRef valArgs As VRValidationArgs, ByRef accordList As List(Of VRAccordionTogglePair))
        Dim ddAIName As DropDownList = Nothing
        Dim ddAIType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing

        Dim newValidationHelper As New ControlValidationHelper()
        newValidationHelper.GroupName = "Transportation"
        Me.ValidationSummmary.RegisterValidationHelper(newValidationHelper)
        'ValidationHelper.GroupName = "Transportation"

        ddAIName = ddTransportationAddlInterestName
        ddAIType = ddTransportationAddlInterestType
        ddATIMA = ddTransportationATIMA

        ' Description
        If txtTransportationDescription.Text.Trim = "" Then
            'Me.ValidationHelper.AddError(txtTransportationDescription, "Missing Description", accordList)
            'updated 10/22/2020 (Interoperability)
            newValidationHelper.AddError(txtTransportationDescription, "Missing Description", accordList)
        End If

        ' Validate AI info
        ValidateAI(newValidationHelper, valArgs, accordList, ddAIName, ddAIType, ddATIMA)

        Exit Sub

    End Sub

#End Region

#Region "Populate Routines"

    Public Overrides Sub Populate()
        If HasContractors() Then
            lnkRemove.Attributes.Add("style", "display:none")
        Else
            lnkRemove.Attributes.Add("style", "display:''")
        End If

        PopulateBuildersRisk()
        PopulateComputers()
        PopulateContractors()
        PopulateFineArts()
        'PopulateInstallationFloater()
        PopulateMotorTruckCargo()
        PopulateOwnersCargo()
        PopulateScheduledPropertyFloater()
        PopulateSigns()
        PopulateTransportation()

        Me.PopulateChildControls()



        Exit Sub
    End Sub

    Private Sub PopulateBuildersRisk()
        Me.divBuildersRisk.Attributes.Add("style", "display:none")

        If HasBuildersRisk() Then
            Me.divBuildersRisk.Attributes.Add("style", "display:''")
            Me.rptBuildersRisk.DataSource = GoverningStateQuote.BuildersRiskScheduledLocations
        Else
            rptBuildersRisk.DataSource = Nothing
        End If
        rptBuildersRisk.DataBind()

        Exit Sub
    End Sub

    Private Sub PopulateComputers()
        Dim BuildingList As List(Of Building_Struct) = Nothing

        If Quote Is Nothing Then Exit Sub

        Me.divComputers.Attributes.Add("style", "display:none")
        If HasComputer() Then
            BuildingList = GetAllBuildings(IMType_enum.Computer)
            Me.divComputers.Attributes.Add("style", "display:''")
            Me.rptComputers.DataSource = BuildingList
        Else
            rptComputers.DataSource = Nothing
        End If
        rptComputers.DataBind()

        Exit Sub
    End Sub

    Private Sub PopulateContractors()

        Me.divContractors.Attributes.Add("style", "display:none")
        Me.lblContractorsSectionTitle.Text = "Contractors Scheduled Equipment"
        If HasContractors() Then
            Me.divContractors.Attributes.Add("style", "display:''")
            'Me.lblContractorsSectionTitle.Text = "Contractors Scheduled Equipment: " & GoverningStateQuote.ContractorsEquipmentCatastropheLimit
            'Me.rptContractors.DataSource = GoverningStateQuote.ContractorsEquipmentScheduledCoverages
            'updated 10/22/2020 to only show the amount for CE scheduled items (Interoperability); note: this may never actually show since the 1st populate calls is when the user sees the UW questions screen; the user would then need to Save to see the App screen, at which point the label is fixed, and Populate is not called again
            Dim govStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = GoverningStateQuote()
            Dim ceSchedCovs As List(Of QuickQuote.CommonObjects.QuickQuoteContractorsEquipmentScheduledCoverage) = Nothing
            Dim scheduledItemsTotalLimit As Double = 0.0
            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            If govStateQuote IsNot Nothing Then
                ceSchedCovs = govStateQuote.ContractorsEquipmentScheduledCoverages
                If ceSchedCovs IsNot Nothing AndAlso ceSchedCovs.Count > 0 Then
                    For Each sc As QuickQuote.CommonObjects.QuickQuoteContractorsEquipmentScheduledCoverage In ceSchedCovs
                        If sc IsNot Nothing AndAlso qqHelper.IsPositiveDecimalString(sc.ManualLimitAmount) = True Then
                            scheduledItemsTotalLimit += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(sc.ManualLimitAmount)
                        End If

                    Next
                End If
            End If
            Me.lblContractorsSectionTitle.Text = "Contractors Scheduled Equipment: " & qqHelper.LimitFormat(scheduledItemsTotalLimit.ToString)
            Me.rptContractors.DataSource = ceSchedCovs

            'If TransactionLimitReached Then
            '    btnContractorsAdd.Enabled = False
            'End If


        Else
            rptContractors.DataSource = Nothing
        End If
        rptContractors.DataBind()
        Exit Sub
    End Sub

    Private Sub PopulateFineArts()
        Me.divFineArts.Attributes.Add("style", "display:none")

        If HasFineArts() Then
            Dim fas As List(Of FineArts_Struct) = GetAllFineArtsItems()
            Me.divFineArts.Attributes.Add("style", "display:''")
            Me.rptFineArts.DataSource = fas
        Else
            rptFineArts.DataSource = Nothing
        End If
        rptFineArts.DataBind()

        Exit Sub
    End Sub

    'Private Sub PopulateInstallationFloater()
    '    ' The installation floater section does not have a repeater
    '    Me.divInstallationFloater.Attributes.Add("style", "display:none")

    '    If HasInstallationFloater() Then
    '        Me.divInstallationFloater.Attributes.Add("style", "display:''")

    '        ' I think this section only applies if the contractors enhancement is selected - need to check with group
    '        Me.txtInstallationFloaterIncludedLimit.Text = "$10,000"   ' Included Limit is $10,000
    '        Dim IncLim As Decimal = 0
    '        If Quote.InstallationBlanketLimit IsNot Nothing AndAlso Quote.InstallationBlanketLimit <> "0" Then
    '            IncLim = CDec(Quote.InstallationBlanketLimit) - 10000
    '            If IncLim < 0 Then IncLim = 0
    '        End If
    '        Me.txtInstallationFloaterIncreasedLimit.Text = String.Format("{0:C0}", IncLim)

    '        ' Additional interest Info
    '        PopulateInlandMarineItemAdditionalInterest(ddInstallationFloaterAddlInterestName, ddInstallationFloaterAddlInterestType, ddInstallationFloaterATIMA, 0, IMType_enum.InstallationFloater, -1)
    '    End If

    '    Exit Sub
    'End Sub

    Private Sub PopulateMotorTruckCargo()
        Me.divMotorTruckCargo.Attributes.Add("style", "display:none")
        Me.divUnScheduledMotorTruckCargo.Attributes.Add("style", "display:none")

        If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
            If HasMotorTruckCargo() Then
                Me.divUnScheduledMotorTruckCargo.Attributes.Add("style", "display:''")
                Me.txtUnScheduledMotorTruckCargoLimit.Text = GoverningStateQuote.MotorTruckCargoUnScheduledVehicleCatastropheLimit
            End If
            ' Additional interest Info
            PopulateInlandMarineItemAdditionalInterest(ddUnScheduledMotorTruckCargoAddlInterestName, ddUnScheduledMotorTruckCargoAddlInterestType, ddUnScheduledMotorTruckCargoATIMA, 0, IMType_enum.MotorTruckCargo, -1)
        Else
            If HasMotorTruckCargo() Then
                Me.divMotorTruckCargo.Attributes.Add("style", "display:''")
                Me.rptMotorTruckCargo.DataSource = GoverningStateQuote.MotorTruckCargoScheduledVehicles
            Else
                rptMotorTruckCargo.DataSource = Nothing
            End If
            rptMotorTruckCargo.DataBind()
        End If

        Exit Sub
    End Sub

    Private Sub PopulateOwnersCargo()
        ' The Owners Cargo section does not have a repeater
        Me.divOwnersCargo.Attributes.Add("style", "display:none")

        If HasOwnersCargo() Then
            Me.divOwnersCargo.Attributes.Add("style", "display:''")
            Me.txtOwnersCargoLimit.Text = GoverningStateQuote.OwnersCargoCatastropheLimit
        End If

        ' Additional interest Info
        PopulateInlandMarineItemAdditionalInterest(ddOwnersCargoAddlInterestName, ddOwnersCargoAddlInterestType, ddOwnersCargoATIMA, 0, IMType_enum.OwnersCargo, -1)

        Exit Sub
    End Sub

    Private Sub PopulateScheduledPropertyFloater()
        Me.divSchedulePropertyFloater.Attributes.Add("style", "display:none")

        If HasScheduledProperty() Then
            Me.divSchedulePropertyFloater.Attributes.Add("style", "display:''")
            Me.rptScheduledPropertyFloater.DataSource = GoverningStateQuote.ScheduledPropertyItems
        Else
            rptScheduledPropertyFloater.DataSource = Nothing
        End If
        rptScheduledPropertyFloater.DataBind()

        Exit Sub
    End Sub

    Private Sub PopulateSigns()
        Me.divSigns.Attributes.Add("style", "display:none")

        If HasSigns() Then
            Dim signs As List(Of Signs_Struct) = GetAllSignsItems()
            Me.divSigns.Attributes.Add("style", "display:''")
            Me.rptSigns.DataSource = signs
        Else
            rptSigns.DataSource = Nothing
        End If
        rptSigns.DataBind()

        Exit Sub
    End Sub

    Private Sub PopulateTransportation()
        ' Transportation does not have a repeater
        Me.divTransportation.Attributes.Add("style", "display:none")

        If HasTransportation() Then
            Me.divTransportation.Attributes.Add("style", "display:''")
            Me.txtTransportationDescription.Text = GoverningStateQuote.TransportationCatastropheDescription

            ' Additional interest Info
            PopulateInlandMarineItemAdditionalInterest(ddTransportationAddlInterestName, ddTransportationAddlInterestType, ddTransportationATIMA, 0, IMType_enum.Transportation, -1)
        End If
        Exit Sub
    End Sub

#End Region

#Region "Get All data functions"

    ''' <summary>
    ''' Returns a list of all buildings on the quote along with their location and building indexes
    ''' </summary>
    ''' <returns></returns>
    Private Function GetAllBuildings(ByVal IMType As IMType_enum) As List(Of Building_Struct)
        Dim Bldgs As New List(Of Building_Struct)
        Dim LocNdx As Integer = -1
        Dim BldNdx As Integer = -1
        Dim AddBuildingToList As Boolean = False

        If Quote.Locations IsNot Nothing Then
            For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                LocNdx += 1
                BldNdx = -1
                If L.Buildings IsNot Nothing Then
                    For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                        AddBuildingToList = False
                        BldNdx += 1
                        Dim newrec As New Building_Struct
                        newrec.LocIndex = LocNdx
                        newrec.BldIndex = BldNdx
                        newrec.building = B

                        Select Case IMType
                            Case IMType_enum.Computer
                                If NumericFieldHasValidValue(B.ComputerHardwareLimit) OrElse NumericFieldHasValidValue(B.ComputerBusinessIncomeLimit) _
                                    OrElse NumericFieldHasValidValue(B.ComputerProgramsApplicationsAndMediaLimit) Then
                                    AddBuildingToList = True
                                End If
                                Exit Select
                            Case Else
                                AddBuildingToList = True
                        End Select

                        If AddBuildingToList Then Bldgs.Add(newrec)
                    Next
                End If
            Next
        End If

        Return Bldgs
    End Function

    ''' <summary>
    ''' Returns a list of all fine arts items on the quote along with their location and building indexes
    ''' </summary>
    ''' <returns></returns>
    Private Function GetAllFineArtsItems() As List(Of FineArts_Struct)
        Dim fas As New List(Of FineArts_Struct)
        Dim LocNdx As Integer = -1
        Dim BldNdx As Integer = -1
        Dim fanum As Integer = 0

        If Quote.Locations IsNot Nothing Then
            For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                LocNdx += 1
                BldNdx = -1
                If L.Buildings IsNot Nothing Then
                    For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                        BldNdx += 1
                        If B.FineArtsScheduledItems IsNot Nothing Then
                            For Each far As QuickQuote.CommonObjects.QuickQuoteFineArtsScheduledItem In B.FineArtsScheduledItems
                                fanum += 1
                                Dim newrec As New FineArts_Struct()
                                newrec.num = fanum
                                newrec.LocIndex = LocNdx
                                newrec.BldIndex = BldNdx
                                newrec.FineArtsItem = far
                                fas.Add(newrec)
                            Next
                        End If
                    Next
                End If
            Next
        End If

        Return fas
    End Function

    ''' <summary>
    ''' Gets a specific fine arts item based on Location, Building, and Item indexes
    ''' </summary>
    ''' <param name="LocIndex"></param>
    ''' <param name="BldIndex"></param>
    ''' <param name="FaIndex"></param>
    ''' <returns></returns>
    Private Function GetFineArtsItem(ByVal LocIndex As Integer, ByVal BldIndex As Integer, ByVal FaIndex As Integer) As QuickQuote.CommonObjects.QuickQuoteFineArtsScheduledItem
        If Quote Is Nothing Then Return Nothing
        If Quote.Locations Is Nothing Then Return Nothing
        If Not Quote.Locations.HasItemAtIndex(LocIndex) Then Return Nothing
        If Quote.Locations(LocIndex).Buildings Is Nothing Then Return Nothing
        If Not Quote.Locations(LocIndex).Buildings.HasItemAtIndex(BldIndex) Then Return Nothing
        If Quote.Locations(LocIndex).Buildings(BldIndex).FineArtsScheduledItems Is Nothing Then Return Nothing
        If Not Quote.Locations(LocIndex).Buildings(BldIndex).FineArtsScheduledItems.HasItemAtIndex(FaIndex) Then Return Nothing
        Return Quote.Locations(LocIndex).Buildings(BldIndex).FineArtsScheduledItems(FaIndex)
    End Function

    ''' <summary>
    ''' Returns a list of all signs items on the quote along with their location and building indexes
    ''' </summary>
    ''' <returns></returns>
    Private Function GetAllSignsItems() As List(Of Signs_Struct)
        Dim signs As New List(Of Signs_Struct)
        Dim LocNdx As Integer = -1
        Dim BldNdx As Integer = -1
        Dim snum As Integer = 0

        If Quote.Locations IsNot Nothing Then
            For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                LocNdx += 1
                BldNdx = -1
                If L.Buildings IsNot Nothing Then
                    For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                        BldNdx += 1
                        If B.ScheduledSigns IsNot Nothing Then
                            For Each s As QuickQuote.CommonObjects.QuickQuoteScheduledSign In B.ScheduledSigns
                                snum += 1
                                Dim newrec As New Signs_Struct()
                                newrec.num = snum
                                newrec.LocIndex = LocNdx
                                newrec.BldIndex = BldNdx
                                newrec.SignsItem = s
                                signs.Add(newrec)
                            Next
                        End If
                    Next
                End If
            Next
        End If

        Return signs
    End Function

    ''' <summary>
    ''' Gets a specific signs item based on Location, Building, and Item indexes
    ''' </summary>
    ''' <param name="LocIndex"></param>
    ''' <param name="BldIndex"></param>
    ''' <param name="FaIndex"></param>
    ''' <returns></returns>
    Private Function GetSignsItem(ByVal LocIndex As Integer, ByVal BldIndex As Integer, ByVal SignsIndex As Integer) As QuickQuote.CommonObjects.QuickQuoteScheduledSign
        If Quote Is Nothing Then Return Nothing
        If Quote.Locations Is Nothing Then Return Nothing
        If Not Quote.Locations.HasItemAtIndex(LocIndex) Then Return Nothing
        If Quote.Locations(LocIndex).Buildings Is Nothing Then Return Nothing
        If Not Quote.Locations(LocIndex).Buildings.HasItemAtIndex(BldIndex) Then Return Nothing
        If Quote.Locations(LocIndex).Buildings(BldIndex).ScheduledSigns Is Nothing Then Return Nothing
        If Not Quote.Locations(LocIndex).Buildings(BldIndex).ScheduledSigns.HasItemAtIndex(SignsIndex) Then Return Nothing
        Return Quote.Locations(LocIndex).Buildings(BldIndex).ScheduledSigns(SignsIndex)
    End Function

    Private Function IsNullWhitespaceEmptyZeroOrNegative(ByVal fieldval As String) As Boolean
        If fieldval Is Nothing Then Return True   ' check for nothing
        If fieldval.Equals(DBNull.Value) Then Return True ' check for null
        If fieldval = "" Then Return True   ' check for empty string
        If IsNumeric(fieldval) Then
            If CDec(fieldval) = 0 Then Return True  ' check for zero
            If CDec(fieldval) < 0 Then Return True  ' check for negative number
        End If
        Return False
    End Function

#End Region

    Private Function NumericFieldHasValidValue(ByVal field As String)
        If field Is Nothing Then Return False
        If field.Trim = "" Then Return False
        If Not IsNumeric(field) Then Return False

        Return True
    End Function

    Private Function GetQuoteAIItem(ByVal listid As String) As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest
        If MyAIList IsNot Nothing Then
            For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In MyAIList
                If ai.ListId = listid Then
                    Return ai
                End If
            Next
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Copies the values from the AI on the quote (based on the passed listid) to the passed Inland Marine AI 
    ''' </summary>
    ''' <param name="InlandMarineAI"></param>
    ''' <param name="listId"></param>
    ''' <param name="IMType"></param>
    Private Sub CopyQuoteAIValuesToInlandMarineAIItem(ByRef InlandMarineAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest, ByVal listId As String, ByVal IMType As IMType_enum, ByVal Index As Integer)
        Dim identifier As String = ""
        Dim ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = GetQuoteAIItem(listId)

        If ai IsNot Nothing Then
            InlandMarineAI.Address = ai.Address
            InlandMarineAI.AgencyId = ai.AgencyId
            InlandMarineAI.BillTo = ai.BillTo
            InlandMarineAI.Description = ai.Description
            InlandMarineAI.Emails = ai.Emails
            InlandMarineAI.GroupTypeId = ai.GroupTypeId
            InlandMarineAI.HasAdditionalInterestListIdChanged = ai.HasAdditionalInterestListIdChanged
            InlandMarineAI.HasAdditionalInterestListNameChanged = ai.HasAdditionalInterestListNameChanged
            InlandMarineAI.HasWaiverOfSubrogation = ai.HasWaiverOfSubrogation
            InlandMarineAI.InterestInProperty = ai.InterestInProperty
            InlandMarineAI.LoanAmount = ai.LoanAmount
            InlandMarineAI.LoanNumber = ai.LoanNumber
            InlandMarineAI.Name = ai.Name
            InlandMarineAI.NameAddressNum = ai.NameAddressNum
            InlandMarineAI.Num = ai.Num
            InlandMarineAI.OverwriteAdditionalInterestListInfoForDiamondId = ai.OverwriteAdditionalInterestListInfoForDiamondId
            InlandMarineAI.Phones = ai.Phones
            InlandMarineAI.PolicyId = ai.PolicyId
            InlandMarineAI.PolicyImageNum = ai.PolicyImageNum
            InlandMarineAI.SingleEntry = ai.SingleEntry
            InlandMarineAI.StatusCode = ai.StatusCode
            InlandMarineAI.TrustAgreementDate = ai.TrustAgreementDate

            ' Set the Other field to an identifier that will tell us which IM Coverage Item the AI belongs to.
            ' For example: Builders Risk item 0 would have an identifier of BR|0
            Select Case IMType
                Case IMType_enum.BuildersRisk
                    identifier = "BR|" & Index.ToString
                    Exit Select
                Case IMType_enum.Computer
                    identifier = "COM|" & Index.ToString
                    Exit Select
                Case IMType_enum.Contractors
                    identifier = "CNT|" & Index.ToString
                    Exit Select
                Case IMType_enum.FineArts
                    identifier = "FIN|" & Index.ToString
                    Exit Select
                Case IMType_enum.InstallationFloater
                    identifier = "IF|" & Index.ToString
                    Exit Select
                Case IMType_enum.MotorTruckCargo
                    identifier = "MTC|" & Index.ToString
                    Exit Select
                Case IMType_enum.OwnersCargo
                    identifier = "OC|" & Index.ToString
                    Exit Select
                Case IMType_enum.ScheduledProperty
                    identifier = "SPF|" & Index.ToString
                    Exit Select
                Case IMType_enum.Signs
                    identifier = "SIGN|" & Index.ToString
                    Exit Select
                Case IMType_enum.Transportation
                    identifier = "TR|" & Index.ToString
                    Exit Select
                Case Else
                    identifier = ""
                    Exit Select
            End Select
            InlandMarineAI.Other = identifier
        End If
    End Sub

    ''' <summary>
    ''' Determines whether the passed number is odd or even
    ''' </summary>
    ''' <param name="num"></param>
    ''' <returns></returns>
    Private Function OddOrEven(ByVal num As Integer) As OddOrEvenEnum
        If num = 0 Then Return OddOrEvenEnum.Even
        If CLng(num) Mod 2 > 0 Then
            Return OddOrEvenEnum.Odd
        Else
            Return OddOrEvenEnum.Even
        End If
    End Function

    ''' <summary>
    ''' Loads each Inland Marine coverage's additional interest dropdown
    ''' </summary>
    ''' <param name="ddlDropdownToLoad"></param>
    Private Sub LoadPayeeDDL(ByRef ddlDropdownToLoad As DropDownList)
        If ddlDropdownToLoad Is Nothing Then Exit Sub
        ddlDropdownToLoad.Items.Clear()

        Dim li As New ListItem()
        li.Text = "N/A"
        li.Value = ""
        ddlDropdownToLoad.Items.Add(li)

        If MyAIList IsNot Nothing Then
            For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In MyAIList
                If ai.Name.DisplayName.Trim <> "" Then  ' Do not add empty AI's
                    li = New ListItem
                    li.Text = ai.Name.DisplayName
                    li.Value = ai.ListId
                    ddlDropdownToLoad.Items.Add(li)
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' Shades a repeater row
    ''' </summary>
    ''' <param name="trRow"></param>
    ''' <param name="trRow2"></param>
    Private Sub ShadeRow(ByRef trRow As HtmlTableRow, Optional ByRef trRow2 As HtmlTableRow = Nothing)
        If trRow Is Nothing Then Exit Sub
        trRow.Attributes.Add("style", "background-color:lightblue")
        If trRow2 IsNot Nothing Then trRow2.Attributes.Add("style", "background-color:lightblue")
        Exit Sub
    End Sub

    Private Function AINameSelected(ByVal ddAIName As DropDownList) As Boolean
        If ddAIName Is Nothing Then Return False
        If ddAIName.SelectedValue = "" Then Return False Else Return True
    End Function

    Private Function GetInlandMarineAIItem(ByVal IMType As IMType_enum, ByVal index As Integer, ByVal ItemIndex As Integer) As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest
        Dim AIList As List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest) = Nothing
        Dim identifier As String = Nothing

        Select Case IMType
            Case IMType_enum.BuildersRisk
                AIList = GoverningStateQuote.BuildersRiskAdditionalInterests
                identifier = "BR|" & index.ToString()
                Exit Select
            Case IMType_enum.Computer
                AIList = GoverningStateQuote.ComputerAdditionalInterests
                identifier = "COM|" & index.ToString()
                Exit Select
            Case IMType_enum.Contractors
                If ItemIndex < 0 Then Return Nothing
                AIList = GoverningStateQuote.ContractorsEquipmentScheduledCoverages(ItemIndex).AdditionalInterests
                identifier = "CNT|" & index.ToString()
                Exit Select
            Case IMType_enum.FineArts
                AIList = GoverningStateQuote.FineArtsAdditionalInterests
                identifier = "FIN|" & index.ToString()
                Exit Select
            Case IMType_enum.InstallationFloater
                AIList = GoverningStateQuote.InstallationAdditionalInterests
                identifier = "IF|" & index.ToString()
                Exit Select
            Case IMType_enum.MotorTruckCargo
                If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
                    AIList = GoverningStateQuote.MotorTruckCargoUnScheduledVehicleAdditionalInterests
                    identifier = "MTC|" & index.ToString()
                Else
                    AIList = GoverningStateQuote.MotorTruckCargoScheduledVehicleAdditionalInterests
                    identifier = "MTC|" & index.ToString()
                End If
                Exit Select
            Case IMType_enum.OwnersCargo
                AIList = GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleAdditionalInterests
                identifier = "OC|" & index.ToString()
                Exit Select
            Case IMType_enum.ScheduledProperty
                AIList = GoverningStateQuote.ScheduledPropertyAdditionalInterests
                identifier = "SPF|" & index.ToString()
                Exit Select
            Case IMType_enum.Signs
                AIList = GoverningStateQuote.SignsAdditionalInterests
                identifier = "SIGN|" & index.ToString()
                Exit Select
            Case IMType_enum.Transportation
                AIList = GoverningStateQuote.TransportationCatastropheAdditionalInterests
                identifier = "TR|" & index.ToString()
                Exit Select
        End Select

        If AIList IsNot Nothing Then
            For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In AIList
                If ai.Other = identifier Then Return ai
            Next
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Pupulates the passed ATIMA dropdown based on the value in the passed add'l interest item
    ''' </summary>
    ''' <param name="ai"></param>
    ''' <param name="ddATIMA"></param>
    Private Sub SetATIMADropdownFromAIItem(ByVal ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest, ByVal ddATIMA As DropDownList)
        If ai Is Nothing Then Exit Sub
        If ai.ATIMA AndAlso ai.ISAOA Then
            SetFromValue(ddATIMA, "3")
        ElseIf ai.ATIMA Then
            SetFromValue(ddATIMA, "1")
        ElseIf ai.ISAOA Then
            SetFromValue(ddATIMA, "2")
        Else
            SetFromValue(ddATIMA, "0")
        End If
    End Sub

    ''' <summary>
    ''' Sets the ATIMA/ISAOA values on the passed Add'l Interest item (ai) 
    ''' based on what's selected in the passed dropdown (ddATIMA)
    ''' </summary>
    ''' <param name="ddATIMA"></param>
    ''' <param name="ai"></param>
    Private Sub SetATIMAValueFromDropdown(ByVal ddATIMA As DropDownList, ByRef ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)
        ai.ATIMA = False
        ai.ISAOA = False
        Select Case ddATIMA.SelectedValue
            Case "0"  ' None
                Exit Select
            Case "1"  ' ATIMA
                ai.ATIMA = True
                Exit Select
            Case "2"  ' ISAOA
                ai.ISAOA = True
                Exit Select
            Case "3"  ' ATIMA/ISAOA
                ai.ATIMA = True
                ai.ISAOA = True
                Exit Select
        End Select
        Return
    End Sub

    ''' <summary>
    ''' Populates an Inland Marine item with it's Ai info
    ''' </summary>
    ''' <param name="ddAddlIntName"></param>
    ''' <param name="ddAddlIntType"></param>
    ''' <param name="ddATIMA"></param>
    ''' <param name="ndx"></param>
    ''' <param name="IMType"></param>
    Private Sub PopulateInlandMarineItemAdditionalInterest(ByRef ddAddlIntName As DropDownList, ByRef ddAddlIntType As DropDownList, ByRef ddATIMA As DropDownList, ByVal ndx As Integer, ByVal IMType As IMType_enum, ByVal ItemIndex As Integer)
        Dim found As Boolean = False
        Dim aiItem As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = Nothing

        LoadPayeeDDL(ddAddlIntName)
        aiItem = GetInlandMarineAIItem(IMType, ndx, ItemIndex)
        If aiItem IsNot Nothing Then
            ' Look for the ai in the existing list
            For Each li As ListItem In ddAddlIntName.Items
                If li.Value = aiItem.ListId Then
                    ddAddlIntName.SelectedIndex = -1
                    li.Selected = True
                    found = True
                    Exit For
                End If
            Next
            ' if the ai was not in the existing list, add it then select it
            If Not found Then
                Dim li As New ListItem()
                li.Text = aiItem.Name.DisplayName
                li.Value = aiItem.ListId
                ddAddlIntName.Items.Add(li)
                ddAddlIntName.SelectedIndex = -1
                li.Selected = True
            End If
            SetFromValue(ddAddlIntType, aiItem.TypeId)
            SetATIMADropdownFromAIItem(aiItem, ddATIMA)
        End If

    End Sub

    Private Function YearOK(ByVal yr As String) As Boolean
        Dim intYear As Integer = 0

        ' Year must be numeric
        If Not IsNumeric(yr) Then Return False

        intYear = CInt(yr)

        ' Year must be between 1970 and current year + 1
        If intYear < 1970 OrElse intYear > DateTime.Now.Year + 1 Then Return False

        Return True
    End Function

    Private Sub DeleteContractorsItem(ByVal ndx As Integer)
        If ndx < 0 Then Exit Sub

        Dim govStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing 'GoverningStateQuote()

        If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), Me.Quote.QuickQuoteState) = True AndAlso Me.Quote.QuickQuoteState <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
            govStateQuote = SubQuoteForState(Me.Quote.QuickQuoteState)
        Else
            govStateQuote = SubQuoteFirst
        End If

        If govStateQuote IsNot Nothing Then
            If govStateQuote.ContractorsEquipmentScheduledCoverages.HasItemAtIndex(ndx) Then
                If govStateQuote.ContractorsEquipmentScheduledCoverages.Count = 1 Then
                    Me.ValidationHelper.AddError("You cannot delete all of your Contractors Scheduled Equipment from here.  Please contact your underwriter if you wish to delete this coverage.")
                Else
                    Save_FireSaveEvent(False)
                    Dim appAIs = New Helpers.FindAppliedAdditionalInterestList

                    Dim sc = govStateQuote.ContractorsEquipmentScheduledCoverages(ndx)

                    Dim AiToRemove = sc.AdditionalInterests?.FirstOrDefault

                    If AiToRemove IsNot Nothing Then
                        'At this point we have at least 1 of this AI (on the CE for example)

                        'Check Location/Buildings for AI's
                        Dim AIs = appAIs.FindAppliedAI(Quote)
                        'Check Contractors' Equipment for AI's
                        Dim ContEquipAIs = appAIs.FindAppliedInlandMarineAI(govStateQuote)

                        'Check That this AI appears only once on CE and not in Building AI's at all
                        Dim hasZeroBuildingAI = AIs?.FindAll(Function(x) x.AI.ListId = AiToRemove.ListId).Count = 0
                        Dim hasOneContEquipAI = ContEquipAIs?.FindAll(Function(x) x.AI.ListId = AiToRemove.ListId).Count = 1

                        'If this AI exists only on this piece of equipment then delete the AI, 
                        'because the item that uses it has been deleted.
                        If hasZeroBuildingAI AndAlso hasOneContEquipAI Then
                            CppDictItems.UpdateAdditionalInterests(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, AiToRemove)
                            QQHelper.RemoveSpecificQuickQuoteAdditionalInterestFromQuoteBasedOnLob(Me.Quote, AiToRemove.ListId, removeFromTopLevel:=True)
                        End If

                    End If

                    'Remove Equipment
                    CppDictItems.UpdateInlaneMarineCoverageWithPremium(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, New InlandMarineCoverageWithPremium(sc.Description, sc.ManualLimitAmount, sc.ScheduledCoverageNum))
                    'govStateQuote.ContractorsEquipmentScheduledCoverages.RemoveAt(ndx)



                    If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), Me.Quote.QuickQuoteState) = True AndAlso Me.Quote.QuickQuoteState <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                        SubQuoteForState(Me.Quote.QuickQuoteState).ContractorsEquipmentScheduledCoverages.RemoveAt(ndx)
                    Else
                        SubQuoteFirst.ContractorsEquipmentScheduledCoverages.RemoveAt(ndx)
                    End If



                    Dim endorsementSaveError As String = ""
                    Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError, saveTypeView:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
                    RaiseEvent AIChange()

                End If
            End If
        End If


        Exit Sub
    End Sub

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Exit Sub
    End Sub

    'Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click, lbSaveBuildersRisk.Click, lbSaveComputers.Click, lbSaveContractors.Click, lbSaveFineArts.Click, lbSaveInstallation.Click, lbSaveMotorTruckCargo.Click, lbSaveOwnersCargo.Click, lbSaveScheduledProperty.Click, lbSaveSigns.Click, lbSaveTransportation.Click
    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click, lbSaveBuildersRisk.Click, lbSaveComputers.Click, lbSaveContractors.Click, lbSaveFineArts.Click, lbSaveMotorTruckCargo.Click, lbSaveOwnersCargo.Click, lbSaveScheduledProperty.Click, lbSaveSigns.Click, lbSaveTransportation.Click
        'RaiseEvent AIChange()
        Save_FireSaveEvent()
        Exit Sub
    End Sub

    Private Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        Exit Sub
    End Sub

#Region "Section Repeater ItemDataBound Events"

    ''' <summary>
    ''' Populate each builders risk row
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub rptBuildersRisk_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptBuildersRisk.ItemDataBound
        Dim txtLimit As TextBox = Nothing
        Dim txtDesc As TextBox = Nothing
        Dim ddAddlIntName As DropDownList = Nothing
        Dim ddAddlIntType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim dItem As QuickQuote.CommonObjects.QuickQuoteBuildersRiskScheduledLocation = Nothing
        Dim ndx As Integer = 0
        Dim trRow1 As HtmlTableRow = Nothing
        Dim trRow2 As HtmlTableRow = Nothing
        Dim OddEven As OddOrEvenEnum = OddOrEvenEnum.Even
        Dim found As Boolean = False

        ' Get the data index
        ndx = e.Item.ItemIndex
        OddEven = OddOrEven(ndx)

        ' Get the controls for this item
        trRow1 = e.Item.FindControl("trBuildersRiskRow1")
        trRow2 = e.Item.FindControl("trBuildersRiskRow2")
        txtLimit = e.Item.FindControl("txtBuildersRiskLimit")
        txtDesc = e.Item.FindControl("txtBuildersRiskJobsiteDescription")
        ddAddlIntName = e.Item.FindControl("ddBuildersRiskAddlInterestName")
        ddAddlIntType = e.Item.FindControl("ddBuildersRiskAddlInterestType")
        ddATIMA = e.Item.FindControl("ddBuildersRiskATIMA")

        ' Shade even rows
        If OddEven = OddOrEvenEnum.Even Then ShadeRow(trRow1, trRow2)

        ' Get the data item
        dItem = e.Item.DataItem

        ' Populate the data fields
        If txtLimit IsNot Nothing Then txtLimit.Text = dItem.Limit
        If txtDesc IsNot Nothing Then txtDesc.Text = dItem.AddressInfo

        ' Populate the AI fields
        PopulateInlandMarineItemAdditionalInterest(ddAddlIntName, ddAddlIntType, ddATIMA, ndx, IMType_enum.BuildersRisk, -1)

        Exit Sub
    End Sub

    ''' <summary>
    ''' Populate each computers row
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub rptComputers_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptComputers.ItemDataBound
        Dim txtLocBldg As TextBox = Nothing
        Dim txtHardwareLimit As TextBox = Nothing
        Dim txtProgramsLimit As TextBox = Nothing
        Dim txtBusinessIncomeLimit As TextBox = Nothing
        Dim ddAddlIntName As DropDownList = Nothing
        Dim ddAddlIntType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim BldNum As Integer = -1
        Dim locbldg As String = ""
        Dim ndx As Integer = 0
        Dim LocNum As Integer = 0
        Dim trRow1 As HtmlTableRow = Nothing
        Dim trRow2 As HtmlTableRow = Nothing
        Dim OddEven As OddOrEvenEnum = OddOrEvenEnum.Even

        Dim BLD As Building_Struct = e.Item.DataItem

        ' Get the data index
        ndx = e.Item.ItemIndex
        LocNum = ndx + 1
        OddEven = OddOrEven(ndx)

        ' Get the controls for this item
        txtLocBldg = e.Item.FindControl("txtComputersLocBldgNum")
        txtHardwareLimit = e.Item.FindControl("txtComputersHardwareLimit")
        txtProgramsLimit = e.Item.FindControl("txtComputersProgramsAppAndMediaLimit")
        txtBusinessIncomeLimit = e.Item.FindControl("txtComputersBusinessIncomeLimit")
        ddAddlIntName = e.Item.FindControl("ddComputersAddlInterestName")
        ddAddlIntType = e.Item.FindControl("ddComputersAddlInterestType")
        ddATIMA = e.Item.FindControl("ddComputersATIMA")
        trRow1 = e.Item.FindControl("trComputersRow1")
        trRow2 = e.Item.FindControl("trComputersRow2")

        ' Shade even rows
        If OddEven = OddOrEvenEnum.Even Then ShadeRow(trRow1, trRow2)

        If txtLocBldg IsNot Nothing Then
            locbldg = BLD.LocIndex + 1.ToString & "-" & BLD.BldIndex + 1.ToString
            txtLocBldg.Text = locbldg
        End If
        If txtHardwareLimit IsNot Nothing Then txtHardwareLimit.Text = BLD.building.ComputerHardwareLimit
        If txtProgramsLimit IsNot Nothing Then txtProgramsLimit.Text = BLD.building.ComputerProgramsApplicationsAndMediaLimit
        If txtBusinessIncomeLimit IsNot Nothing Then txtBusinessIncomeLimit.Text = BLD.building.ComputerBusinessIncomeLimit

        ' Populate the AI fields
        PopulateInlandMarineItemAdditionalInterest(ddAddlIntName, ddAddlIntType, ddATIMA, ndx, IMType_enum.Computer, -1)

        Exit Sub
    End Sub

    ''' <summary>
    ''' Populate each Contractors Scheduled Equipment row
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub rptContractors_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptContractors.ItemDataBound
        Dim txtLimit As TextBox = Nothing
        Dim ddValuation As DropDownList = Nothing
        Dim txtDescription As TextBox = Nothing
        Dim ddAddlIntName As DropDownList = Nothing
        Dim ddAddlIntType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim btnDelete As Button = Nothing
        Dim locbldg As String = ""
        Dim ndx As Integer = 0
        Dim LocNum As Integer = 0
        Dim trRow1 As HtmlTableRow = Nothing
        Dim trRow2 As HtmlTableRow = Nothing
        Dim OddEven As OddOrEvenEnum = OddOrEvenEnum.Even
        Dim CONT As QuickQuote.CommonObjects.QuickQuoteContractorsEquipmentScheduledCoverage = e.Item.DataItem

        OddEven = OddOrEven(e.Item.ItemIndex)

        ' Get the data index
        ndx = e.Item.ItemIndex
        LocNum = ndx + 1

        ' Get the controls for this item
        txtLimit = e.Item.FindControl("txtContractorsLimit")
        ddValuation = e.Item.FindControl("ddContractorsValuation")
        txtDescription = e.Item.FindControl("txtContractorsDescription")
        ddAddlIntName = e.Item.FindControl("ddContractorsAddlInterestName")
        ddAddlIntType = e.Item.FindControl("ddContractorsAddlInterestType")
        ddATIMA = e.Item.FindControl("ddContractorsATIMA")
        trRow1 = e.Item.FindControl("trContractorsRow1")
        trRow2 = e.Item.FindControl("trContractorsRow2")
        btnDelete = e.Item.FindControl("btnContractorsDelete")

        ' Shade even rows
        If OddEven = OddOrEvenEnum.Even Then ShadeRow(trRow1, trRow2)

        ' Set the values on the page controls
        If txtLimit IsNot Nothing Then txtLimit.Text = CONT.ManualLimitAmount
        'If ddValuation IsNot Nothing Then SetFromValue(ddValuation, CONT.ValuationMethodTypeId)
        If txtDescription IsNot Nothing Then
            ' We only want to show the scheduled tools description if it's not what was defaulted on the quote side
            ' The defaulted description text is "SCHEDULED TOOLS #n"
            If CONT.Description.Length >= 15 AndAlso CONT.Description.Substring(0, 15).ToUpper = "SCHEDULED TOOLS" Then
                txtDescription.Text = ""
            Else
                txtDescription.Text = CONT.Description
            End If
        End If

        ' Valuation
        '   SetFromValue(ddValuation, CONT.ValuationMethodTypeId)

        If ddValuation IsNot Nothing Then
            QQHelper.LoadStaticDataOptionsDropDown(ddValuation, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerValuationMethodTypeId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
        End If

        'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
        If IsEndorsementRelated() Then
            WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddValuation, CONT.ValuationMethodTypeId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerValuationMethodTypeId)
        Else
            If CONT.ValuationMethodTypeId.IsNullEmptyorWhitespace() Then
                ddValuation.SelectedIndex = 0
            Else
                ddValuation.SelectedIndex = CONT.ValuationMethodTypeId

            End If
        End If



        ' Additional interest Info
        PopulateInlandMarineItemAdditionalInterest(ddAddlIntName, ddAddlIntType, ddATIMA, ndx, IMType_enum.Contractors, ndx)

        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
            Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
            AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
            Dim DoesPreExist As Boolean = AllPreExistingItems.PreExisting_InlandMarineCoveragesWithPremium.isPreExistingCoverage(CONT.Description, CONT.ManualLimitAmount, CONT.ScheduledCoverageNum)
            'check preexist and count, then disable else enable (below)
            Select Case TypeOfEndorsement()
                Case EndorsementTypeString.CPP_AddDeleteContractorsEquipmentLienholder
                    If DoesPreExist Then
                        VRScript.FakeDisableSingleElement(trRow1)
                        'VRScript.FakeDisableSingleElement(trRow2)
                    End If

                    If TransactionLimitReached Then
                        If DoesPreExist Then
                            btnDelete.Enabled = False
                        End If
                    Else
                        btnDelete.Enabled = True
                    End If
            End Select
        End If

        Exit Sub
    End Sub

    ''' <summary>
    ''' Populate each Fine Arts row
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub rptFineArts_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptFineArts.ItemDataBound
        Dim txtLocBldg As TextBox = Nothing
        Dim txtLimit As TextBox = Nothing
        Dim txtDesc As TextBox = Nothing
        Dim ddAddlIntName As DropDownList = Nothing
        Dim ddAddlIntType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim locbldg As String = ""
        Dim ndx As Integer = 0
        Dim LocNum As Integer = 0
        Dim trRow1 As HtmlTableRow = Nothing
        Dim trRow2 As HtmlTableRow = Nothing
        Dim OddEven As OddOrEvenEnum = OddOrEvenEnum.Even

        Dim FA As FineArts_Struct = e.Item.DataItem

        ' Get the data index
        ndx = e.Item.ItemIndex
        LocNum = ndx + 1
        OddEven = OddOrEven(ndx)

        ' Get the controls for this item
        txtLocBldg = e.Item.FindControl("txtFineArtsLocBldgNum")
        txtLimit = e.Item.FindControl("txtFineArtsLimit")
        txtDesc = e.Item.FindControl("txtFineArtsDescription")
        ddAddlIntName = e.Item.FindControl("ddFineArtsAddlInterestName")
        ddAddlIntType = e.Item.FindControl("ddFineArtsAddlInterestType")
        ddATIMA = e.Item.FindControl("ddFineArtsATIMA")
        trRow1 = e.Item.FindControl("trFineArtsRow1")
        trRow2 = e.Item.FindControl("trFineArtsRow2")

        ' Shade even rows
        If OddEven = OddOrEvenEnum.Even Then ShadeRow(trRow1, trRow2)

        If txtLocBldg IsNot Nothing Then
            locbldg = FA.LocIndex + 1.ToString & "-" & FA.BldIndex + 1.ToString
            txtLocBldg.Text = locbldg
        End If
        If txtLimit IsNot Nothing Then txtLimit.Text = FA.FineArtsItem.Limit

        If txtDesc IsNot Nothing Then
            ' We only want to show the fine arts description if it's not what was defaulted on the quote side
            ' The defaulted description text is "FINE ARTS #n"
            If FA.FineArtsItem.Description.Length >= 9 AndAlso FA.FineArtsItem.Description.Substring(0, 9).ToUpper = "FINE ARTS" Then
                txtDesc.Text = ""
            Else
                txtDesc.Text = FA.FineArtsItem.Description
            End If
        End If

        ' Additional interest Info
        PopulateInlandMarineItemAdditionalInterest(ddAddlIntName, ddAddlIntType, ddATIMA, ndx, IMType_enum.FineArts, -1)
    End Sub

    ''' <summary>
    ''' Populate each Motor Truck Cargo row
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub rptMotorTruckCargo_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptMotorTruckCargo.ItemDataBound
        Dim txtLimit As TextBox = Nothing
        Dim txtYear As TextBox = Nothing
        Dim txtMake As TextBox = Nothing
        Dim txtModel As TextBox = Nothing
        Dim txtVIN As TextBox = Nothing
        Dim ddAddlIntName As DropDownList = Nothing
        Dim ddAddlIntType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim trRow1 As HtmlTableRow = Nothing
        Dim trRow2 As HtmlTableRow = Nothing
        Dim OddEven As OddOrEvenEnum = OddOrEvenEnum.Even

        Dim MTC As QuickQuote.CommonObjects.QuickQuoteScheduledVehicle = e.Item.DataItem

        OddEven = OddOrEven(e.Item.ItemIndex)

        ' Get the controls for this item
        txtLimit = e.Item.FindControl("txtMotorTruckCargoLimit")
        txtYear = e.Item.FindControl("txtYear")
        txtMake = e.Item.FindControl("txtMake")
        txtModel = e.Item.FindControl("txtModel")
        txtVIN = e.Item.FindControl("txtVIN")
        ddAddlIntName = e.Item.FindControl("ddMotorTruckCargoAddlInterestName")
        ddAddlIntType = e.Item.FindControl("ddMotorTruckCargoAddlInterestType")
        ddATIMA = e.Item.FindControl("ddMotorTruckCargoATIMA")
        trRow1 = e.Item.FindControl("trMotorTruckCargoRow1")
        trRow2 = e.Item.FindControl("trMotorTruckCargoRow2")

        ' Shade even rows
        If OddEven = OddOrEvenEnum.Even Then ShadeRow(trRow1, trRow2)

        If txtLimit IsNot Nothing Then txtLimit.Text = MTC.Limit
        If txtYear IsNot Nothing Then
            If YearOK(MTC.Year) Then txtYear.Text = MTC.Year Else txtYear.Text = ""
        End If
        If txtMake IsNot Nothing Then txtMake.Text = MTC.Make
        If txtModel IsNot Nothing Then txtModel.Text = MTC.Model
        If txtVIN IsNot Nothing Then txtVIN.Text = MTC.VIN

        ' Additional interest Info
        PopulateInlandMarineItemAdditionalInterest(ddAddlIntName, ddAddlIntType, ddATIMA, e.Item.ItemIndex, IMType_enum.MotorTruckCargo, -1)

        Exit Sub
    End Sub

    ''' <summary>
    ''' Populate each Scheduled Property Floater row
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub rptScheduledPropertyFloater_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptScheduledPropertyFloater.ItemDataBound
        Dim txtLimit As TextBox = Nothing
        Dim txtDesc As TextBox = Nothing
        Dim ddAddlIntName As DropDownList = Nothing
        Dim ddAddlIntType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim ndx As Integer = 0
        Dim LocNum As Integer = 0
        Dim trRow1 As HtmlTableRow = Nothing
        Dim trRow2 As HtmlTableRow = Nothing
        Dim OddEven As OddOrEvenEnum = OddOrEvenEnum.Even

        Dim SP As QuickQuote.CommonObjects.QuickQuoteScheduledPropertyItem = e.Item.DataItem

        ' Get the data index
        ndx = e.Item.ItemIndex
        LocNum = ndx + 1
        OddEven = OddOrEven(ndx)

        ' Get the controls for this item
        txtLimit = e.Item.FindControl("txtScheduledPropertyLimit")
        txtDesc = e.Item.FindControl("txtScheduledPropertyDescription")
        ddAddlIntName = e.Item.FindControl("ddScheduledPropertyAddlInterestName")
        ddAddlIntType = e.Item.FindControl("ddScheduledPropertyInterestType")
        ddATIMA = e.Item.FindControl("ddScheduledPropertyATIMA")
        trRow1 = e.Item.FindControl("trScheduledPropertyRow1")
        trRow2 = e.Item.FindControl("trScheduledPropertyRow2")

        ' Shade even rows
        If OddEven = OddOrEvenEnum.Even Then ShadeRow(trRow1, trRow2)

        ' Set the field values on the page
        If txtLimit IsNot Nothing Then txtLimit.Text = SP.Limit

        If txtDesc IsNot Nothing Then
            ' We only want to show the scheduled property description if it's not what was defaulted on the quote side
            ' The defaulted description text is "SCHEDULED PROPERTY #n"
            If SP.Description.Length >= 18 AndAlso SP.Description.Substring(0, 18).ToUpper = "SCHEDULED PROPERTY" Then
                txtDesc.Text = ""
            Else
                txtDesc.Text = SP.Description
            End If
        End If

        ' Additional interest Info
        PopulateInlandMarineItemAdditionalInterest(ddAddlIntName, ddAddlIntType, ddATIMA, ndx, IMType_enum.ScheduledProperty, -1)

        Exit Sub
    End Sub

    ''' <summary>
    ''' Populate each signs row
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub rptSigns_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptSigns.ItemDataBound
        Dim txtLocBldg As TextBox = Nothing
        Dim txtLimit As TextBox = Nothing
        Dim txtDesc As TextBox = Nothing
        Dim ddAddlIntName As DropDownList = Nothing
        Dim ddAddlIntType As DropDownList = Nothing
        Dim ddATIMA As DropDownList = Nothing
        Dim locbldg As String = ""
        Dim ndx As Integer = 0
        Dim LocNum As Integer = 0
        Dim trRow1 As HtmlTableRow = Nothing
        Dim trRow2 As HtmlTableRow = Nothing
        Dim OddEven As OddOrEvenEnum = OddOrEvenEnum.Even

        Dim sign As Signs_Struct = e.Item.DataItem

        ' Get the data index
        ndx = e.Item.ItemIndex
        LocNum = ndx + 1
        OddEven = OddOrEven(ndx)

        ' Get the controls for this item
        txtLocBldg = e.Item.FindControl("txtSignsLocBldgNum")
        txtLimit = e.Item.FindControl("txtSignsLimit")
        txtDesc = e.Item.FindControl("txtSignsDescription")
        ddAddlIntName = e.Item.FindControl("ddSignsAddlInterestName")
        ddAddlIntType = e.Item.FindControl("ddSignsAddlInterestType")
        ddATIMA = e.Item.FindControl("ddSignsATIMA")
        trRow1 = e.Item.FindControl("trSignsRow1")
        trRow2 = e.Item.FindControl("trSignsRow2")

        ' Shade even rows
        If OddEven = OddOrEvenEnum.Even Then ShadeRow(trRow1, trRow2)

        If txtLocBldg IsNot Nothing Then
            locbldg = sign.LocIndex + 1.ToString & "-" & sign.BldIndex + 1.ToString
            txtLocBldg.Text = locbldg
        End If
        If txtLimit IsNot Nothing Then txtLimit.Text = sign.SignsItem.Limit

        If txtDesc IsNot Nothing Then
            ' We only want to show the signs description if it's not what was defaulted on the quote side
            ' The defaulted description text is "SIGNS #n"
            If sign.SignsItem.Description.Length >= 5 AndAlso sign.SignsItem.Description.Substring(0, 5).ToUpper = "SIGNS" Then
                txtDesc.Text = ""
            Else
                txtDesc.Text = sign.SignsItem.Description
            End If
        End If

        ' Additional interest Info
        PopulateInlandMarineItemAdditionalInterest(ddAddlIntName, ddAddlIntType, ddATIMA, ndx, IMType_enum.Signs, -1)

        Exit Sub
    End Sub

    Private Sub btnContractorsAdd_Click(sender As Object, e As EventArgs) Handles btnContractorsAdd.Click
        Dim newItem As New QuickQuote.CommonObjects.QuickQuoteContractorsEquipmentScheduledCoverage

        If GoverningStateQuote.ContractorsEquipmentScheduledCoverages Is Nothing Then GoverningStateQuote.ContractorsEquipmentScheduledCoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteContractorsEquipmentScheduledCoverage)

        GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Add(New QuickQuote.CommonObjects.QuickQuoteContractorsEquipmentScheduledCoverage())

        'Capture current form data + Add new coverage
        Save_FireSaveEvent(False)

        ' Get the recently saved "new" Coverage so we have the diamond assigned ScheduledCoverageNum for tracking
        newItem = GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Last

        CppDictItems.UpdateInlaneMarineCoverageWithPremium(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, New InlandMarineCoverageWithPremium(newItem.Description, newItem.ManualLimitAmount, newItem.ScheduledCoverageNum))

        'Me.Populate()
        RaiseEvent AIChange()
        Save_FireSaveEvent(False)

        Exit Sub
    End Sub

    Private Sub btnContractorsDelete_Click(sender As Object, e As EventArgs)
        Dim ri As RepeaterItem = sender.parent.parent.parent.parent   ' Gets the repeater item that the linkbutton belongs to
        DeleteContractorsItem(ri.ItemIndex)
        Exit Sub
    End Sub

    Protected Sub rptContractors_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        Select Case e.CommandName
            Case "DELETE"
                Dim ndx As Integer = e.Item.ItemIndex
                DeleteContractorsItem(ndx)
        End Select

        Exit Sub
    End Sub

#End Region

#End Region

End Class