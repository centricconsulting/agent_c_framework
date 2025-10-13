Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports PopupMessageClass



Public Class ctl_BOP_BuildingCoverages
    Inherits VRControlBase

#Region "Declarations"

    ''' <summary>
    ''' Returns the index of the location ON THE TOP QUOTE
    ''' </summary>
    ''' <returns></returns>
    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    ''' <summary>
    ''' Returns the index of the location ON THE STATE QUOTE
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property StateLocationIndex As Int32
        Get
            Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(MyLocation.Address.QuickQuoteState)
            Dim ndx As Int32 = -1

            If StateQuote IsNot Nothing Then
                If StateQuote.Locations IsNot Nothing Then
                    For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In StateQuote.Locations
                        ndx += 1
                        If LOC.Equals(MyLocation) Then Return ndx
                    Next
                End If
            End If
            Return -1
        End Get
    End Property

    ''' <summary>
    ''' If this is the first building on a state quote then this will return true otherwise returns false
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsFirstBuildingOnStateQuote() As Boolean
        Get
            Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(MyLocation.Address.QuickQuoteState)
            If StateQuote IsNot Nothing Then
                If StateQuote.Locations IsNot Nothing AndAlso StateQuote.Locations.HasItemAtIndex(0) Then
                    If StateQuote.Locations(0).Buildings IsNot Nothing AndAlso StateQuote.Locations(0).Buildings.Count > 0 Then
                        If StateQuote.Locations(0).Buildings(0).Equals(MyBuilding) Then Return True
                    End If
                End If
            End If
            Return False
        End Get
    End Property

    Public Property BuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
        End Set
    End Property
    Private ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote.IsNotNull Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property MyBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations(LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) Then
                Return Quote.Locations(LocationIndex).Buildings(BuildingIndex)
            End If
            Return Nothing
        End Get
    End Property

    ''' <summary>
    ''' Returns the number of the building as it appears on the page
    ''' Zero-based
    ''' 
    ''' If thebuilding is not found returns -1
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property BuildingNumber As Integer
        Get
            Dim ndx As Integer = -1
            For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                For Each BLD As QuickQuote.CommonObjects.QuickQuoteBuilding In LOC.Buildings
                    ndx += 1
                    If BLD.Equals(MyBuilding) Then Return ndx
                Next
            Next
            Return -1
        End Get
    End Property

    Private Enum ClassCoverage_Enum
        Apartments
        Veterinarians
        OpticalAndHearingAid
        Barbers
        Beauticians
        FuneralDirectors
        Printers
        SelfStorage
        Motel
        LiquorLiability
        FineArts
        Photography
        Photography_MakeupAndHair
        Photography_SheduledEquipment
        Pharmacist
        Restaurant
        ResidentialCleaning
    End Enum

#End Region

#Region "Methods and Functions"

    Private Sub HandleError(ByVal RoutineName As String, ByVal ex As Exception)
        Dim str As String = RoutineName & ":  " & ex.Message
        If AppSettings("TestOrProd").ToUpper <> "PROD" Then lblMsg.Text = str Else Throw New Exception(ex.Message, ex)
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Sub ClassificationChanged()
        Populate()
    End Sub

    ''' <summary>
    ''' Code to execute when effective date changes
    ''' </summary>
    ''' <param name="NewDate"></param>
    ''' <param name="OldDate"></param>
    Public Sub EffectiveDateChanging(ByVal NewDate As String, ByVal OldDate As String)
        Dim hascov As Boolean = False
        Dim err As String = False

        ' Liquor Liability - if effective date is on or after 4/1/2020 don't show the numeric limits for Illinois  Task 40485
        If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.LiquorLiability, hascov, err) Then
            trLiquorLiabilityCheckboxRow.Visible = True
            trLiquorLiabilityCheckboxRow.Attributes.Add("style", "display:''")

            If MyLocation?.Address IsNot Nothing Then
                Select Case MyLocation.Address.QuickQuoteState
                    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                        If CDate(NewDate) >= CDate("4/1/2020") Then
                            lblLiquorLiabilityLimit.Text = "STATE STATUTORY LIMITS WILL APPLY"
                        Else
                            lblLiquorLiabilityLimit.Text = "69/69/85" 'diamond Id 409
                        End If
                    Case Else
                        lblLiquorLiabilityLimit.Text = "$1,000,000" 'diamond Id ???
                End Select
            Else
                lblLiquorLiabilityLimit.Text = "$1,000,000"
            End If
        End If
        Exit Sub
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "0")
        Me.VRScript.CreateConfirmDialog(Me.lnkClear.ClientID, "Clear?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)

        ' Load the PL coverage control ID's into a javascript array for easier handling
        Me.VRScript.AddVariableLine("Bop.BOPPLCoverageBindings.push(new Bop.BOPPLCoveragUIBinding('" &
            Me.chkApartments.ClientID & "','" &
            Me.txtAptsNumberOfLocationsWithApts.ClientID & "','" &
            Me.trApartmentsDataRow.ClientID & "','" &
            Me.ddlAptAutoLiabilityLimit.ClientID & "','" &
            Me.ddlAptDeductible.ClientID & "','" &
            Me.chkBarbersLiability.ClientID & "','" &
            Me.trBarbersLiabilityDataRow.ClientID & "','" &
            Me.txtBarberFullTimeEmployees.ClientID & "','" &
            Me.txtBarberPartTimeEmployees.ClientID & "','" &
            Me.chkBeautyShops.ClientID & "','" &
            Me.trBeautyShopsDataRow.ClientID & "','" &
            Me.txtBeautyShopsNumFullTimeEmployees.ClientID & "','" &
            Me.txtBeautyShopsNumPartTimeEmployees.ClientID & "','" &
            Me.chkFineArts.ClientID & "','" &
            Me.chkFuneralDirectors.ClientID & "','" &
            Me.trFuneralDataRow.ClientID & "','" &
            Me.txtFuneralNumOfEmployees.ClientID & "','" &
            Me.chkLiquorLiability.ClientID & "','" &
            Me.trLiquorLiabilityDataRow.ClientID & "','" &
            Me.lblLiquorLiabilityLimit.ClientID & "','" &
            Me.lblLiquorLiabilityClassCode.ClientID & "','" &
            Me.txtLiquorLiabiltyAnnualGrossAlcoholSales.ClientID & "','" &
            Me.chkMotel.ClientID & "','" &
            Me.trMotelDataRow.ClientID & "','" &
            Me.ddlMotelLiabilityLimit.ClientID & "','" &
            Me.ddlMotelSafeDepositBoxLimit.ClientID & "','" &
            Me.ddlMotelSafeDepositBoxDeductible.ClientID & "','" &
            Me.chkOpticalAndHearingAid.ClientID & "','" &
            Me.trOpticalAndHearingDataRow.ClientID & "','" &
            Me.txtOpticalAndHearingNumOfEmployees.ClientID & "','" &
            Me.chkPharmacists.ClientID & "','" &
            Me.trPharmacistsDataRow.ClientID & "','" &
            Me.txtPharmacistsReceipts.ClientID & "','" &
            Me.chkPhotography.ClientID & "','" &
            Me.trPhotographyDataRow.ClientID & "','" &
            Me.chkPhotographyScheduledEquipment.ClientID & "','" &
            Me.trPhotogScheduledEquipmentRow.ClientID & "','" &
            Me.txtPhotogScheduledEquipmentLimit.ClientID & "','" &
            Me.trPhotographyMakeupAndHairCheckboxRow.ClientID & "','" &
            Me.chkPhotogMakeupAndHair.ClientID & "','" &
            Me.chkPrinters.ClientID & "','" &
            Me.trPrintersDataRow.ClientID & "','" &
            Me.txtPrintersNumOfLocations.ClientID & "','" &
            Me.chkResidentialCleaning.ClientID & "','" &
            Me.chkRestaurant.ClientID & "','" &
            Me.trRestaurantDataRow.ClientID & "','" &
            Me.ddlRestaurantsLimitOfLiability.ClientID & "','" &
            Me.ddlRestaurantsDeductible.ClientID & "','" &
            Me.chkSelfStorage.ClientID & "','" &
            Me.trSelfStorageDataRow.ClientID & "','" &
            Me.txtStorageLimit.ClientID & "','" &
            Me.chkVeterinarians.ClientID & "','" &
            Me.trVeterinariansDataRow.ClientID & "','" &
            Me.txtVeterinariansNumOfEmployees.ClientID &
            "'));")

        ' Add the functionality to disable/enable the contractors fields based on what's entered
        txtDemolitionCostLimit.Attributes.Add("onchange", "Bop.SetBOPOrdOrLawFields('" & txtDemolitionCostLimit.ClientID & "', '" & txtIncreasedCostOfConstructionLimit.ClientID & "', '" & txtDemolitionAndIncreasedCostCombinedLimit.ClientID & "');")
        txtIncreasedCostOfConstructionLimit.Attributes.Add("onchange", "Bop.SetBOPOrdOrLawFields('" & txtDemolitionCostLimit.ClientID & "', '" & txtIncreasedCostOfConstructionLimit.ClientID & "', '" & txtDemolitionAndIncreasedCostCombinedLimit.ClientID & "');")
        txtDemolitionAndIncreasedCostCombinedLimit.Attributes.Add("onchange", "Bop.SetBOPOrdOrLawFields('" & txtDemolitionCostLimit.ClientID & "', '" & txtIncreasedCostOfConstructionLimit.ClientID & "', '" & txtDemolitionAndIncreasedCostCombinedLimit.ClientID & "');")

        ' Add functionality to the optional coverages checkboxes to show or hide their divs
        ' Building-specific
        chkAcctsReceivableONPremises.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkAcctsReceivableONPremises.ClientID & "','" & trAcctsReceivableONPremisesDataRow.ClientID & "');")
        chkValuablePapersOnPremises.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkValuablePapersOnPremises.ClientID & "','" & trValuablePapersOnPremisesDataRow.ClientID & "');")
        chkOrdinanceOrLaw.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkOrdinanceOrLaw.ClientID & "','" & trOrdinanceOrLawDataRow.ClientID & "');")
        chkSpoilage.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkSpoilage.ClientID & "','" & trSpoilageDataRow.ClientID & "');")
        chkBreakdownOrContamination.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkBreakdownOrContamination.ClientID & "');")

        ' NEW PL COVERAGE SCRIPT CALLS
        chkApartments.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('APTS','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkBarbersLiability.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('BARBER','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkBeautyShops.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('BEAUTY','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkFineArts.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('FINE','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkFuneralDirectors.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('FUNERAL','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkLiquorLiability.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('LIQUOR'," & Me.BuildingNumber & ",'" & Me.MyLocation.Address.State & "');")
        chkMotel.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('MOTEL','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkOpticalAndHearingAid.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('OPTICAL','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkPharmacists.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('PHARMACIST','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkPhotography.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('PHOTOG','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkPhotographyScheduledEquipment.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('PHOTOG_SCHED','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkPhotogMakeupAndHair.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('PHOTOG_MAKEUP','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkPrinters.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('PRINTERS','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkResidentialCleaning.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('RESCLEAN','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkRestaurant.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('REST','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkSelfStorage.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('SELFSTORAGE','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")
        chkVeterinarians.Attributes.Add("onchange", "Bop.ProfessionalLiabilityCheckboxChanged('VET','" & Me.BuildingNumber & "','" & Me.MyLocation.Address.State & "');")

        Exit Sub
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Dim err As String = Nothing
        Dim helper As New CommonHelperClass()

        Try
            If Me.Quote IsNot Nothing Then
                LoadStaticData()

                Quote.CopyProfessionalLiabilityCoveragesFromPolicyToBuildings_UseBuildingClassificationList()

                If Me.Quote.Locations.HasItemAtIndex(LocationIndex) AndAlso Me.Quote.Locations(LocationIndex).Buildings.HasItemAtIndex(BuildingIndex) Then
                    ' Accounts Receivable - On Premises
                    If MyBuilding.AccountsReceivableOnPremisesExcessLimit.Trim <> "" Then
                        chkAcctsReceivableONPremises.Checked = True
                        If MyBuilding.AccountsReceivableOnPremisesExcessLimit <> "0" Then
                            txtAcctsReceivableOnPremisesTotalLimit.Text = MyBuilding.AccountsReceivableOnPremisesExcessLimit
                        Else
                            txtAcctsReceivableOnPremisesTotalLimit.Text = ""
                        End If
                        chkAcctsReceivableONPremises_CheckedChanged(Me, New EventArgs())
                    Else
                        chkAcctsReceivableONPremises.Checked = False
                    End If

                    ' Valuable Papers - On Premises
                    If MyBuilding.ValuablePapersOnPremises.Trim <> "" Then
                        chkValuablePapersOnPremises.Checked = True
                        If MyBuilding.ValuablePapersOnPremises <> "0" Then
                            txtValuablePapersOnPremisesTotalLimit.Text = MyBuilding.ValuablePapersOnPremises
                        Else
                            txtValuablePapersOnPremisesTotalLimit.Text = ""
                        End If
                        chkValuablePapersOnPremises_CheckedChanged(Me, New EventArgs())
                    Else
                        chkValuablePapersOnPremises.Checked = False
                    End If

                    ' Ordinance Or Law
                    If MyBuilding.HasOrdinanceOrLaw Then
                        chkOrdinanceOrLaw.Checked = True
                        chkOrdinanceOrLaw_CheckedChanged(Me, New EventArgs())
                        If MyBuilding.HasOrdOrLawUndamagedPortion Then chkOrdinanceOrLawUndamaged.Checked = True Else chkOrdinanceOrLawUndamaged.Checked = False
                        txtDemolitionCostLimit.Text = MyBuilding.OrdOrLawDemoCostLimit
                        txtIncreasedCostOfConstructionLimit.Text = MyBuilding.OrdOrLawIncreasedCostLimit
                        txtDemolitionAndIncreasedCostCombinedLimit.Text = MyBuilding.OrdOrLawDemoAndIncreasedCostLimit
                        If txtDemolitionAndIncreasedCostCombinedLimit.Text.Trim <> String.Empty Then
                            txtDemolitionAndIncreasedCostCombinedLimit.Enabled = True
                            txtDemolitionCostLimit.Enabled = False
                            txtIncreasedCostOfConstructionLimit.Enabled = False
                        Else
                            If txtDemolitionCostLimit.Text.Trim <> String.Empty OrElse txtIncreasedCostOfConstructionLimit.Text.Trim <> String.Empty Then
                                txtDemolitionAndIncreasedCostCombinedLimit.Enabled = False
                                txtDemolitionCostLimit.Enabled = True
                                txtIncreasedCostOfConstructionLimit.Enabled = True
                            Else
                                txtDemolitionAndIncreasedCostCombinedLimit.Enabled = True
                                txtDemolitionCostLimit.Enabled = True
                                txtIncreasedCostOfConstructionLimit.Enabled = True
                            End If
                        End If

                    Else
                        chkOrdinanceOrLaw.Checked = False
                    End If

                    ' Spoilage
                    If MyBuilding.HasSpoilage Then
                        chkSpoilage.Checked = True
                        chkSpoilage_CheckedChanged(Me, New EventArgs())
                        SetFromValue(ddlSpoilagePropertyClassification, MyBuilding.SpoilagePropertyClassificationId)
                        'ddlSpoilagePropertyClassification.SelectedValue = MyBuilding.SpoilagePropertyClassificationId
                        txtSpoilageTotalLimit.Text = MyBuilding.SpoilageTotalLimit
                        If MyBuilding.IsSpoilageRefrigerationMaintenanceAgreement Then chkRefrigeratorMaintenanceAgreement.Checked = True Else chkRefrigeratorMaintenanceAgreement.Checked = False
                        If MyBuilding.IsSpoilageBreakdownOrContamination Then chkBreakdownOrContamination.Checked = True Else chkBreakdownOrContamination.Checked = False
                        If MyBuilding.IsSpoilagePowerOutage Then chkPowerOutage.Checked = True Else chkPowerOutage.Checked = False
                    Else
                        chkSpoilage.Checked = False
                    End If

                    ' PROFESSIONAL LIABILITY COVERAGES
                    '--------------------------------------
                    Dim hascov As Boolean = False

                    ' LIQUOR LIABILITY (Motel, Restaurant, Retail)
                    hascov = False
                    ' Remove the classes from the liquor checkbox and sales textbox
                    chkLiquorLiability.Attributes("class") = ""
                    txtLiquorLiabiltyAnnualGrossAlcoholSales.Attributes("class") = ""
                    trLiquorLiabilityDataRow.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.LiquorLiability, hascov, err) Then
                        trLiquorLiabilityCheckboxRow.Visible = True
                        trLiquorLiabilityCheckboxRow.Attributes.Add("style", "display:''") 'Added 12/31/18 for Bug 30677 MLW

                        If MyLocation?.Address IsNot Nothing Then
                            Select Case MyLocation.Address.QuickQuoteState
                                Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                    If CDate(Quote.EffectiveDate) >= CDate("4/1/2020") Then
                                        lblLiquorLiabilityLimit.Text = "STATE STATUTORY LIMITS WILL APPLY"
                                    Else
                                        lblLiquorLiabilityLimit.Text = "69/69/85" 'diamond Id 409
                                    End If
                                Case Else
                                    lblLiquorLiabilityLimit.Text = "$1,000,000" 'diamond Id ???
                            End Select
                        Else
                            lblLiquorLiabilityLimit.Text = "$1,000,000"
                        End If

                        If hascov Then
                            ' IS ELIGIBLE & HAS LIQUOR LIABILITY COVERAGE
                            chkLiquorLiability.Checked = True
                            trLiquorLiabilityDataRow.Attributes.Add("style", "display:''")

                            ' Only one of these will be populated
                            If MyBuilding.LiquorLiabilityAnnualGrossAlcoholSalesReceipts.Trim <> String.Empty AndAlso IsNumeric(MyBuilding.LiquorLiabilityAnnualGrossAlcoholSalesReceipts) Then
                                txtLiquorLiabiltyAnnualGrossAlcoholSales.Text = MyBuilding.LiquorLiabilityAnnualGrossAlcoholSalesReceipts
                            ElseIf MyBuilding.LiquorLiabilityAnnualGrossPackageSalesReceipts.Trim <> String.Empty AndAlso IsNumeric(MyBuilding.LiquorLiabilityAnnualGrossPackageSalesReceipts) Then
                                txtLiquorLiabiltyAnnualGrossAlcoholSales.Text = MyBuilding.LiquorLiabilityAnnualGrossPackageSalesReceipts
                            Else
                                txtLiquorLiabiltyAnnualGrossAlcoholSales.Text = ""
                            End If

                            If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.LiquorLiability, err) Then
                                'chkLiquorLiability.Attributes.Add("disabled", "false")
                                'txtLiquorLiabiltyAnnualGrossAlcoholSales.Attributes.Add("disabled", "false")
                                chkLiquorLiability.Enabled = True
                                txtLiquorLiabiltyAnnualGrossAlcoholSales.Enabled = True
                            Else
                                'chkLiquorLiability.Attributes.Add("disabled", "true")
                                'txtLiquorLiabiltyAnnualGrossAlcoholSales.Attributes.Add("disabled", "true")
                                chkLiquorLiability.Enabled = False
                                txtLiquorLiabiltyAnnualGrossAlcoholSales.Enabled = False
                            End If
                        ElseIf AnyBuildingOnStateHasPLCoverage(ClassCoverage_Enum.LiquorLiability) Then
                            ' IS NOT ELIGIBLE FOR LIQUOR LIABILITY BUT DOES NOT HAVE IT, BUT ANOTHER BUILDING ON THE STATE DOES HAVE IT
                            ' - Show, check & disable the Liquor Liability checkbox
                            ' - Show & disable the data rows and populate with whatever is on the first building in the same state with the coverage

                            ' Show, check & disable the checkbox
                            chkLiquorLiability.Checked = True
                            chkLiquorLiability.Checked = True
                            chkLiquorLiability.Enabled = False

                            ' Show the data rows
                            trLiquorLiabilityDataRow.Attributes.Add("style", "display:''")

                            ' Populate the data from the first building in the state that has the coverage
                            Dim fb As QuickQuote.CommonObjects.QuickQuoteBuilding = GetFirstBuildingOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.LiquorLiability)
                            If fb IsNot Nothing Then
                                ' Sales
                                If fb.LiquorLiabilityAnnualGrossAlcoholSalesReceipts IsNot Nothing AndAlso fb.LiquorLiabilityAnnualGrossAlcoholSalesReceipts <> "" AndAlso IsNumeric(fb.LiquorLiabilityAnnualGrossAlcoholSalesReceipts) AndAlso CDec(fb.LiquorLiabilityAnnualGrossAlcoholSalesReceipts) > 0 Then
                                    txtLiquorLiabiltyAnnualGrossAlcoholSales.Text = fb.LiquorLiabilityAnnualGrossAlcoholSalesReceipts
                                ElseIf fb.LiquorLiabilityAnnualGrossPackageSalesReceipts IsNot Nothing AndAlso fb.LiquorLiabilityAnnualGrossPackageSalesReceipts <> "" AndAlso IsNumeric(fb.LiquorLiabilityAnnualGrossPackageSalesReceipts) AndAlso CDec(fb.LiquorLiabilityAnnualGrossPackageSalesReceipts) > 0 Then
                                    txtLiquorLiabiltyAnnualGrossAlcoholSales.Text = fb.LiquorLiabilityAnnualGrossPackageSalesReceipts
                                End If
                            End If

                            ' Enable/disable the coverage controls 
                            If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.LiquorLiability, err) Then
                                ' First building in state with this coverage - enable the checkbox and sales textbox
                                chkLiquorLiability.Enabled = True
                                txtLiquorLiabiltyAnnualGrossAlcoholSales.Enabled = True
                            Else
                                ' Not the first building on the state with this coverage - disable the checkbox and sales textbox
                                chkLiquorLiability.Attributes.Add("disabled", "true")
                                txtLiquorLiabiltyAnnualGrossAlcoholSales.Enabled = False
                            End If

                            'txtLiquorLiabiltyAnnualGrossAlcoholSales.Text = Quote.LiquorLiabilityAnnualGrossAlcoholSalesReceipts
                            'If Quote.LiquorLiabilityAnnualGrossAlcoholSalesReceipts.Trim <> String.Empty AndAlso IsNumeric(Quote.LiquorLiabilityAnnualGrossAlcoholSalesReceipts) Then
                            '    txtLiquorLiabiltyAnnualGrossAlcoholSales.Text = Quote.LiquorLiabilityAnnualGrossAlcoholSalesReceipts
                            'ElseIf Quote.LiquorLiabilityAnnualGrossPackageSalesReceipts.Trim <> String.Empty AndAlso IsNumeric(Quote.LiquorLiabilityAnnualGrossPackageSalesReceipts) Then
                            '    txtLiquorLiabiltyAnnualGrossAlcoholSales.Text = Quote.LiquorLiabilityAnnualGrossPackageSalesReceipts
                            'Else
                            '    txtLiquorLiabiltyAnnualGrossAlcoholSales.Text = ""
                            'End If
                            'If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.LiquorLiability, err) Then
                            '    chkLiquorLiability.Enabled = True
                            '    txtLiquorLiabiltyAnnualGrossAlcoholSales.Enabled = True
                            'Else
                            '    chkLiquorLiability.Attributes.Add("disabled", "true")
                            '    txtLiquorLiabiltyAnnualGrossAlcoholSales.Enabled = False
                            'End If
                        Else
                            ' Is Eligible for Liquor Liability but does not have it and no other buildings in the state have it
                            ' - Show the Liquor Liability checkbox
                            ' - Hide the data rows
                            ' - Clear out the sales textbox
                            chkLiquorLiability.Checked = False
                            chkLiquorLiability.Enabled = True
                            trLiquorLiabilityDataRow.Attributes.Add("style", "display: none")
                            trLiquorLiabilityCheckboxRow.Attributes.Add("style", "display:''")
                            txtLiquorLiabiltyAnnualGrossAlcoholSales.Text = String.Empty
                        End If

                        ' Assign classes to the liquor checkbox and sales textbox based on state
                        ' We'll use these classes in the script to format the elements
                        ' NOTE: Needed to add the classes last because when you add a css class to a checkbox it is rendered within a span which breaks the javasscript
                        '       that sets the checkbox.
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                trLiquorLiabilityDataRow.Attributes.Add("class", "trLiquor_IL")
                                helper.AddCSSClassToControl(chkLiquorLiability, "chkLiquorSelector_IL")
                                helper.AddCSSClassToControl(txtLiquorLiabiltyAnnualGrossAlcoholSales, "txtLiquorSales_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                trLiquorLiabilityDataRow.Attributes.Add("class", "trLiquor_IN")
                                helper.AddCSSClassToControl(chkLiquorLiability, "chkLiquorSelector_IN")
                                helper.AddCSSClassToControl(txtLiquorLiabiltyAnnualGrossAlcoholSales, "txtLiquorSales_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                trLiquorLiabilityDataRow.Attributes.Add("class", "trLiquor_OH")
                                helper.AddCSSClassToControl(chkLiquorLiability, "chkLiquorSelector_OH")
                                helper.AddCSSClassToControl(txtLiquorLiabiltyAnnualGrossAlcoholSales, "txtLiquorSales_OH")
                                Exit Select
                        End Select
                    Else
                        ' Building is not eligible for Liquor Liability
                        ' - Hide the Liquor Liability checkbox
                        ' - Hide the data rows
                        chkLiquorLiability.Checked = False
                        'chkLiquorLiability.Attributes.Add("disabled", "false")
                        chkLiquorLiability.Enabled = True
                        trLiquorLiabilityCheckboxRow.Attributes.Add("style", "display:none")
                        'trLiquorLiabilityCheckboxRow.Visible = False
                        trLiquorLiabilityDataRow.Attributes.Add("style", "display:none")
                        txtLiquorLiabiltyAnnualGrossAlcoholSales.Text = String.Empty
                    End If

                    ' FINE ARTS  (Apartment, Restaurant)
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkFineArts.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.FineArts, hascov, err) Then
                        trFineArtsCheckboxRow.Visible = True
                        If hascov Then
                            chkFineArts.Checked = True
                            If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.FineArts, err) Then
                                chkFineArts.Enabled = True
                            Else
                                chkFineArts.Enabled = False
                            End If
                        ElseIf AnyBuildingOnStateHasPLCoverage(ClassCoverage_Enum.FineArts) Then
                            ' This else section of code needs to be here for any coverage that can be added as the result of another coverage
                            chkFineArts.Checked = True
                            If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.FineArts, err) Then
                                chkFineArts.Enabled = True
                            Else
                                chkFineArts.Enabled = False
                            End If
                        Else
                            chkFineArts.Enabled = True
                            chkFineArts.Checked = False
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                helper.AddCSSClassToControl(chkFineArts, "chkFineArts_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                helper.AddCSSClassToControl(chkFineArts, "chkFineArts_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                helper.AddCSSClassToControl(chkFineArts, "chkFineArts_OH")
                                Exit Select
                        End Select
                    Else
                        chkFineArts.Checked = False
                        trFineArtsCheckboxRow.Visible = False
                    End If

                    ' RESTAURANT (Motel, Restaurant)
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkRestaurant.Attributes("class") = ""
                    ddlRestaurantsLimitOfLiability.Attributes("class") = ""
                    ddlRestaurantsDeductible.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.Restaurant, hascov, err) Then
                        trRestaurantCheckboxRow.Visible = True
                        If hascov Then
                            chkRestaurant.Checked = True
                            trRestaurantDataRow.Attributes.Add("style", "display:''")
                            SetFromValue(ddlRestaurantsLimitOfLiability, MyBuilding.CustomerAutoLegalLiabilityLimitOfLiabilityId)
                            'ddlRestaurantsLimitOfLiability.SelectedValue = MyBuilding.CustomerAutoLegalLiabilityLimitOfLiabilityId
                            SetFromValue(ddlRestaurantsDeductible, MyBuilding.CustomerAutoLegalLiabilityDeductibleId)
                            'ddlRestaurantsDeductible.SelectedValue = MyBuilding.CustomerAutoLegalLiabilityDeductibleId
                        ElseIf AnyBuildingOnStateHasPLCoverage(ClassCoverage_Enum.Restaurant) Then
                            ' This else section of code needs to be here for any coverage that can be added as the result of another coverage
                            chkRestaurant.Checked = True
                            trRestaurantDataRow.Attributes.Add("style", "display:''")
                            SetFromValue(ddlRestaurantsLimitOfLiability, Quote.Locations(LocationIndex).CustomerAutoLegalLiabilityLimitOfLiabilityId)
                            'ddlRestaurantsLimitOfLiability.SelectedValue = Quote.Locations(LocationIndex).CustomerAutoLegalLiabilityLimitOfLiabilityId
                            SetFromValue(ddlRestaurantsDeductible, Quote.Locations(LocationIndex).CustomerAutoLegalLiabilityDeductibleId)
                            'ddlRestaurantsDeductible.SelectedValue = Quote.Locations(LocationIndex).CustomerAutoLegalLiabilityDeductibleId
                            If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Restaurant, err) Then
                                chkRestaurant.Enabled = True
                                ddlRestaurantsLimitOfLiability.Enabled = True
                                ddlRestaurantsDeductible.Enabled = True
                            Else
                                chkRestaurant.Enabled = False
                                ddlRestaurantsLimitOfLiability.Enabled = False
                                ddlRestaurantsDeductible.Enabled = False
                            End If
                        Else
                            trRestaurantDataRow.Attributes.Add("style", "display:none")
                            chkRestaurant.Checked = False
                            chkRestaurant.Enabled = True
                            ddlRestaurantsDeductible.SelectedIndex = -1
                            ddlRestaurantsLimitOfLiability.SelectedIndex = -1
                        End If

                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.LiquorLiability, err) Then
                            chkRestaurant.Enabled = True
                            ddlRestaurantsLimitOfLiability.Enabled = True
                            ddlRestaurantsDeductible.Enabled = True
                        Else
                            chkRestaurant.Enabled = False
                            ddlRestaurantsLimitOfLiability.Enabled = False
                            ddlRestaurantsDeductible.Enabled = False
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                helper.AddCSSClassToControl(chkRestaurant, "chkRestaurant_IL")
                                helper.AddCSSClassToControl(ddlRestaurantsLimitOfLiability, "ddlRestaurantLimitOfLiability_IL")
                                helper.AddCSSClassToControl(ddlRestaurantsDeductible, "ddlRestaurantDeductible_IL")
                                trRestaurantDataRow.Attributes.Add("class", "trRestaurantDataRow_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                helper.AddCSSClassToControl(chkRestaurant, "chkRestaurant_IN")
                                helper.AddCSSClassToControl(ddlRestaurantsLimitOfLiability, "ddlRestaurantLimitOfLiability_IN")
                                helper.AddCSSClassToControl(ddlRestaurantsDeductible, "ddlRestaurantDeductible_IN")
                                trRestaurantDataRow.Attributes.Add("class", "trRestaurantDataRow_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                helper.AddCSSClassToControl(chkRestaurant, "chkRestaurant_OH")
                                helper.AddCSSClassToControl(ddlRestaurantsLimitOfLiability, "ddlRestaurantLimitOfLiability_OH")
                                helper.AddCSSClassToControl(ddlRestaurantsDeductible, "ddlRestaurantDeductible_OH")
                                trRestaurantDataRow.Attributes.Add("class", "trRestaurantDataRow_OH")
                                Exit Select
                        End Select
                    Else
                        chkRestaurant.Checked = False
                        trRestaurantCheckboxRow.Visible = False
                        trRestaurantDataRow.Attributes.Add("style", "display:none")
                        ddlRestaurantsDeductible.SelectedIndex = -1
                        ddlRestaurantsLimitOfLiability.SelectedIndex = -1
                    End If

                    ' PHARMACISTS
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkPharmacists.Attributes("class") = ""
                    txtPharmacistsReceipts.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.Pharmacist, hascov, err) Then
                        trPharmacistsCheckBoxRow.Visible = True
                        If hascov Then
                            chkPharmacists.Checked = True
                            trPharmacistsDataRow.Attributes.Add("style", "display:''")
                            txtPharmacistsReceipts.Text = MyBuilding.PharmacistAnnualGrossSales
                        Else
                            trPharmacistsDataRow.Attributes.Add("style", "display:none")
                            chkPharmacists.Checked = False
                            txtPharmacistsReceipts.Text = ""
                        End If
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Pharmacist, err) Then
                            chkPharmacists.Enabled = True
                            txtPharmacistsReceipts.Enabled = True
                        Else
                            chkPharmacists.Enabled = False
                            txtPharmacistsReceipts.Enabled = False
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                helper.AddCSSClassToControl(chkPharmacists, "chkPharmacists_IL")
                                helper.AddCSSClassToControl(txtPharmacistsReceipts, "txtPharmacistsReceipts_IL")
                                trPharmacistsDataRow.Attributes.Add("class", "trPharmacistsDataRow_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                helper.AddCSSClassToControl(chkPharmacists, "chkPharmacists_IN")
                                helper.AddCSSClassToControl(txtPharmacistsReceipts, "txtPharmacistsReceipts_IN")
                                trPharmacistsDataRow.Attributes.Add("class", "trPharmacistsDataRow_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                helper.AddCSSClassToControl(chkPharmacists, "chkPharmacists_OH")
                                helper.AddCSSClassToControl(txtPharmacistsReceipts, "txtPharmacistsReceipts_OH")
                                trPharmacistsDataRow.Attributes.Add("class", "trPharmacistsDataRow_OH")
                                Exit Select
                        End Select
                    Else
                        chkPharmacists.Checked = False
                        trPharmacistsCheckBoxRow.Visible = False
                        trPharmacistsDataRow.Attributes.Add("style", "display:none")
                        txtPharmacistsReceipts.Text = ""
                    End If

                    ' OPTICAL & HEARING AID
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkOpticalAndHearingAid.Attributes("class") = ""
                    txtOpticalAndHearingNumOfEmployees.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.OpticalAndHearingAid, hascov, err) Then
                        trOpticalAndHearingAidCheckboxRow.Visible = True
                        If hascov Then
                            chkOpticalAndHearingAid.Checked = True
                            trOpticalAndHearingDataRow.Attributes.Add("style", "display:''")
                            txtOpticalAndHearingNumOfEmployees.Text = MyBuilding.OpticalAndHearingAidProfessionalLiabilityEmpNum
                        Else
                            trOpticalAndHearingDataRow.Attributes.Add("style", "display:none")
                            chkOpticalAndHearingAid.Checked = False
                            txtOpticalAndHearingNumOfEmployees.Text = ""
                        End If
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.OpticalAndHearingAid, err) Then
                            chkOpticalAndHearingAid.Enabled = True
                            txtOpticalAndHearingNumOfEmployees.Enabled = True
                        Else
                            chkOpticalAndHearingAid.Enabled = False
                            txtOpticalAndHearingNumOfEmployees.Enabled = False
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                helper.AddCSSClassToControl(chkOpticalAndHearingAid, "chkOptical_IL")
                                helper.AddCSSClassToControl(txtOpticalAndHearingNumOfEmployees, "txtOpticalNumEmp_IL")
                                trOpticalAndHearingDataRow.Attributes.Add("class", "trOpticalDataRow_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                helper.AddCSSClassToControl(chkOpticalAndHearingAid, "chkOptical_IN")
                                helper.AddCSSClassToControl(txtOpticalAndHearingNumOfEmployees, "txtOpticalNumEmp_IN")
                                trOpticalAndHearingDataRow.Attributes.Add("class", "trOpticalDataRow_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                helper.AddCSSClassToControl(chkOpticalAndHearingAid, "chkOptical_OH")
                                helper.AddCSSClassToControl(txtOpticalAndHearingNumOfEmployees, "txtOpticalNumEmp_OH")
                                trOpticalAndHearingDataRow.Attributes.Add("class", "trOpticalDataRow_OH")
                                Exit Select
                        End Select
                    Else
                        chkOpticalAndHearingAid.Checked = False
                        trOpticalAndHearingAidCheckboxRow.Visible = False
                        trOpticalAndHearingDataRow.Attributes.Add("style", "display:none")
                        txtOpticalAndHearingNumOfEmployees.Text = ""
                    End If

                    ' BARBERS
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkBarbersLiability.Attributes("class") = ""
                    txtBarberFullTimeEmployees.Attributes("class") = ""
                    txtBarberPartTimeEmployees.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.Barbers, hascov, err) Then
                        trBarbersLiabilityCheckBoxRow.Visible = True
                        If hascov Then
                            chkBarbersLiability.Checked = True
                            trBarbersLiabilityDataRow.Attributes.Add("style", "display:''")
                            txtBarberFullTimeEmployees.Text = MyBuilding.BarbersProfessionalLiabilityFullTimeEmpNum
                            txtBarberPartTimeEmployees.Text = MyBuilding.BarbersProfessionalLiabilityPartTimeEmpNum
                        Else
                            trBarbersLiabilityDataRow.Attributes.Add("style", "display:none")
                            chkBarbersLiability.Checked = False
                            txtBarberFullTimeEmployees.Text = String.Empty
                            txtBarberPartTimeEmployees.Text = String.Empty
                        End If
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Barbers, err) Then
                            chkBarbersLiability.Enabled = True
                            txtBarberFullTimeEmployees.Enabled = True
                            txtBarberPartTimeEmployees.Enabled = True
                        Else
                            chkBarbersLiability.Enabled = False
                            txtBarberFullTimeEmployees.Enabled = False
                            txtBarberPartTimeEmployees.Enabled = False
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                helper.AddCSSClassToControl(chkBarbersLiability, "chkBarbers_IL")
                                trBarbersLiabilityDataRow.Attributes.Add("class", "trBarbersDataRow_IL")
                                helper.AddCSSClassToControl(txtBarberFullTimeEmployees, "txtBarbersFullTimeEmp_IL")
                                helper.AddCSSClassToControl(txtBarberPartTimeEmployees, "txtBarbersPartTimeEmp_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                helper.AddCSSClassToControl(chkBarbersLiability, "chkBarbers_IN")
                                trBarbersLiabilityDataRow.Attributes.Add("class", "trBarbersDataRow_IN")
                                helper.AddCSSClassToControl(txtBarberFullTimeEmployees, "txtBarbersFullTimeEmp_IN")
                                helper.AddCSSClassToControl(txtBarberPartTimeEmployees, "txtBarbersPartTimeEmp_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                helper.AddCSSClassToControl(chkBarbersLiability, "chkBarbers_OH")
                                trBarbersLiabilityDataRow.Attributes.Add("class", "trBarbersDataRow_OH")
                                helper.AddCSSClassToControl(txtBarberFullTimeEmployees, "txtBarbersFullTimeEmp_OH")
                                helper.AddCSSClassToControl(txtBarberPartTimeEmployees, "txtBarbersPartTimeEmp_OH")
                                Exit Select
                        End Select
                    Else
                        chkBarbersLiability.Checked = False
                        trBarbersLiabilityCheckBoxRow.Visible = False
                        trBarbersLiabilityDataRow.Attributes.Add("style", "display:none")
                        txtBarberFullTimeEmployees.Text = String.Empty
                        txtBarberPartTimeEmployees.Text = String.Empty
                    End If

                    ' BEAUTICIANS
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkBeautyShops.Attributes("class") = ""
                    txtBeautyShopsNumFullTimeEmployees.Attributes("class") = ""
                    txtBeautyShopsNumPartTimeEmployees.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.Beauticians, hascov, err) Then
                        trBeautyShopsCheckBoxRow.Visible = True
                        If hascov Then
                            chkBeautyShops.Checked = True
                            trBeautyShopsDataRow.Attributes.Add("style", "display:''")
                            txtBeautyShopsNumFullTimeEmployees.Text = MyBuilding.BeauticiansProfessionalLiabilityFullTimeEmpNum
                            txtBeautyShopsNumPartTimeEmployees.Text = MyBuilding.BeauticiansProfessionalLiabilityPartTimeEmpNum
                        Else
                            chkBeautyShops.Checked = False
                            trBeautyShopsDataRow.Attributes.Add("style", "display:none")
                            txtBeautyShopsNumPartTimeEmployees.Text = ""
                            txtBeautyShopsNumFullTimeEmployees.Text = ""
                        End If
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Beauticians, err) Then
                            chkBeautyShops.Enabled = True
                            txtBeautyShopsNumFullTimeEmployees.Enabled = True
                            txtBeautyShopsNumPartTimeEmployees.Enabled = True
                        Else
                            chkBeautyShops.Enabled = False
                            txtBeautyShopsNumFullTimeEmployees.Enabled = False
                            txtBeautyShopsNumPartTimeEmployees.Enabled = False
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                helper.AddCSSClassToControl(chkBeautyShops, "chkBeauty_IL")
                                helper.AddCSSClassToControl(txtBeautyShopsNumFullTimeEmployees, "txtBeautyFullTimeEmp_IL")
                                helper.AddCSSClassToControl(txtBeautyShopsNumPartTimeEmployees, "txtBeautyPartTimeEmp_IL")
                                trBeautyShopsDataRow.Attributes.Add("class", "trBeautyDataRow_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                helper.AddCSSClassToControl(chkBeautyShops, "chkBeauty_IN")
                                helper.AddCSSClassToControl(txtBeautyShopsNumFullTimeEmployees, "txtBeautyFullTimeEmp_IN")
                                helper.AddCSSClassToControl(txtBeautyShopsNumPartTimeEmployees, "txtBeautyPartTimeEmp_IN")
                                trBeautyShopsDataRow.Attributes.Add("class", "trBeautyDataRow_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                helper.AddCSSClassToControl(chkBeautyShops, "chkBeauty_OH")
                                helper.AddCSSClassToControl(txtBeautyShopsNumFullTimeEmployees, "txtBeautyFullTimeEmp_OH")
                                helper.AddCSSClassToControl(txtBeautyShopsNumPartTimeEmployees, "txtBeautyPartTimeEmp_OH")
                                trBeautyShopsDataRow.Attributes.Add("class", "trBeautyDataRow_OH")
                                Exit Select
                        End Select
                    Else
                        chkBeautyShops.Checked = False
                        trBeautyShopsCheckBoxRow.Visible = False
                        trBeautyShopsDataRow.Attributes.Add("style", "display:none")
                        txtBeautyShopsNumFullTimeEmployees.Text = ""
                        txtBeautyShopsNumPartTimeEmployees.Text = ""
                    End If

                    ' PRINTERS
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkPrinters.Attributes("class") = ""
                    txtPrintersNumOfLocations.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.Printers, hascov, err) Then
                        trPrintersCheckBoxRow.Visible = True
                        If hascov Then
                            chkPrinters.Checked = True
                            trPrintersDataRow.Attributes.Add("style", "display:''")
                            txtPrintersNumOfLocations.Text = MyBuilding.PrintersProfessionalLiabilityLocNum
                        Else
                            chkPrinters.Checked = False
                            trPrintersDataRow.Attributes.Add("style", "display:none")
                            txtPrintersNumOfLocations.Text = ""
                        End If
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Printers, err) Then
                            chkPrinters.Enabled = True
                            txtPrintersNumOfLocations.Enabled = True
                        Else
                            chkPrinters.Enabled = False
                            txtPrintersNumOfLocations.Enabled = False
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                helper.AddCSSClassToControl(chkPrinters, "chkPrinters_IL")
                                helper.AddCSSClassToControl(txtPrintersNumOfLocations, "txtPrintersNumLocs_IL")
                                trPrintersDataRow.Attributes.Add("class", "trPrintersDataRow_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                helper.AddCSSClassToControl(chkPrinters, "chkPrinters_IN")
                                helper.AddCSSClassToControl(txtPrintersNumOfLocations, "txtPrintersNumLocs_IN")
                                trPrintersDataRow.Attributes.Add("class", "trPrintersDataRow_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                helper.AddCSSClassToControl(chkPrinters, "chkPrinters_OH")
                                helper.AddCSSClassToControl(txtPrintersNumOfLocations, "txtPrintersNumLocs_OH")
                                trPrintersDataRow.Attributes.Add("class", "trPrintersDataRow_OH")
                                Exit Select
                        End Select
                    Else
                        chkPrinters.Checked = False
                        trPrintersDataRow.Attributes.Add("style", "display:none")
                        trPrintersCheckBoxRow.Visible = False
                        txtPrintersNumOfLocations.Text = ""
                    End If

                    ' FUNERAL DIRECTORS
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkFuneralDirectors.Attributes("class") = ""
                    txtFuneralNumOfEmployees.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.FuneralDirectors, hascov, err) Then
                        trFuneralCheckBoxRow.Visible = True
                        If hascov Then
                            chkFuneralDirectors.Checked = True
                            trFuneralDataRow.Attributes.Add("style", "display:''")
                            txtFuneralNumOfEmployees.Text = MyBuilding.FuneralDirectorsProfessionalLiabilityEmpNum
                        Else
                            chkFuneralDirectors.Checked = False
                            trFuneralDataRow.Attributes.Add("style", "display:none")
                            txtFuneralNumOfEmployees.Text = ""
                        End If
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.FuneralDirectors, err) Then
                            chkFuneralDirectors.Enabled = True
                            txtFuneralNumOfEmployees.Enabled = True
                        Else
                            chkFuneralDirectors.Enabled = False
                            txtFuneralNumOfEmployees.Enabled = False
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                trFuneralDataRow.Attributes.Add("class", "trFuneralDataRow_IL")
                                helper.AddCSSClassToControl(chkFuneralDirectors, "chkFuneral_IL")
                                helper.AddCSSClassToControl(txtFuneralNumOfEmployees, "txtFuneralNumEmp_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                trFuneralDataRow.Attributes.Add("class", "trFuneralDataRow_IN")
                                helper.AddCSSClassToControl(chkFuneralDirectors, "chkFuneral_IN")
                                helper.AddCSSClassToControl(txtFuneralNumOfEmployees, "txtFuneralNumEmp_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                trFuneralDataRow.Attributes.Add("class", "trFuneralDataRow_OH")
                                helper.AddCSSClassToControl(chkFuneralDirectors, "chkFuneral_OH")
                                helper.AddCSSClassToControl(txtFuneralNumOfEmployees, "txtFuneralNumEmp_OH")
                                Exit Select
                        End Select
                    Else
                        chkFuneralDirectors.Checked = False
                        trFuneralCheckBoxRow.Visible = False
                        trFuneralDataRow.Attributes.Add("style", "display:none")
                        txtFuneralNumOfEmployees.Text = ""
                    End If

                    ' PHOTOGRAPHY (NEW)
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkPhotography.Attributes("class") = ""
                    chkPhotographyScheduledEquipment.Attributes("class") = ""
                    chkPhotogMakeupAndHair.Attributes("class") = ""
                    txtPhotogScheduledEquipmentLimit.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.Photography, hascov, err) Then
                        trPhotographyCheckboxRow.Visible = True
                        If hascov Then
                            chkPhotography.Checked = True
                            trPhotographyDataRow.Attributes.Add("style", "display:''")
                            trPhotographyMakeupAndHairCheckboxRow.Attributes.Add("style", "display:'';")
                            chkPhotogMakeupAndHair.Checked = MyBuilding.HasPhotographyMakeupAndHair
                            If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Photography, err) Then
                                chkPhotography.Enabled = True
                                chkPhotographyScheduledEquipment.Enabled = True
                                chkPhotogMakeupAndHair.Enabled = True
                            Else
                                chkPhotography.Enabled = False
                                chkPhotographyScheduledEquipment.Enabled = False
                                chkPhotogMakeupAndHair.Enabled = False
                            End If

                            If MyBuilding.HasPhotographyCoverageScheduledCoverages Then
                                chkPhotographyScheduledEquipment.Checked = True
                                trPhotogScheduledEquipmentRow.Attributes.Add("style", "display:''")
                                ' Add up the amounts for all of the scheduled items
                                Dim tot As Decimal = 0
                                For Each itm As QuickQuote.CommonObjects.QuickQuoteCoverage In MyBuilding.PhotographyScheduledCoverages
                                    If IsNumeric(itm.ManualLimitAmount) Then tot += CDec(itm.ManualLimitAmount)
                                Next
                                'Updated 12/23/2021 for task 65795 MLW
                                txtPhotogScheduledEquipmentLimit.Text = Format(tot, "0")
                                'txtPhotogScheduledEquipmentLimit.Text = Format(tot, "g")

                                If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Photography, err) Then
                                    chkPhotographyScheduledEquipment.Enabled = True
                                    txtPhotogScheduledEquipmentLimit.Enabled = True
                                Else
                                    chkPhotographyScheduledEquipment.Enabled = False
                                    txtPhotogScheduledEquipmentLimit.Enabled = False
                                End If
                            Else
                                chkPhotographyScheduledEquipment.Checked = False
                                trPhotogScheduledEquipmentRow.Attributes.Add("style", "display:none")
                                txtPhotogScheduledEquipmentLimit.Text = ""
                            End If
                        Else  ' HasCov = False  
                            chkPhotography.Checked = False
                            trPhotographyDataRow.Attributes.Add("style", "display:none")
                            trPhotographyMakeupAndHairCheckboxRow.Attributes.Add("style", "display:none")
                            chkPhotography.Checked = False
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                helper.AddCSSClassToControl(chkPhotography, "chkPhotog_IL")
                                helper.AddCSSClassToControl(chkPhotogMakeupAndHair, "chkPhotogMakeup_IL")
                                helper.AddCSSClassToControl(chkPhotographyScheduledEquipment, "chkPhotogSchedEquip_IL")
                                helper.AddCSSClassToControl(txtPhotogScheduledEquipmentLimit, "txtPhotogLimit_IL")
                                trPhotographyDataRow.Attributes.Add("class", "trPhotogDataRow_IL")
                                trPhotogScheduledEquipmentRow.Attributes.Add("class", "trPhotogScheduleEquipDataRow_IL")
                                trPhotographyMakeupAndHairCheckboxRow.Attributes.Add("class", "trPhotogMakeupAndHairCheckboxRow_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                helper.AddCSSClassToControl(chkPhotography, "chkPhotog_IN")
                                helper.AddCSSClassToControl(chkPhotogMakeupAndHair, "chkPhotogMakeup_IN")
                                helper.AddCSSClassToControl(chkPhotographyScheduledEquipment, "chkPhotogSchedEquip_IN")
                                helper.AddCSSClassToControl(txtPhotogScheduledEquipmentLimit, "txtPhotogLimit_IN")
                                trPhotographyDataRow.Attributes.Add("class", "trPhotogDataRow_IN")
                                trPhotogScheduledEquipmentRow.Attributes.Add("class", "trPhotogScheduleEquipDataRow_IN")
                                trPhotographyMakeupAndHairCheckboxRow.Attributes.Add("class", "trPhotogMakeupAndHairCheckboxRow_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                helper.AddCSSClassToControl(chkPhotography, "chkPhotog_OH")
                                helper.AddCSSClassToControl(chkPhotogMakeupAndHair, "chkPhotogMakeup_OH")
                                helper.AddCSSClassToControl(chkPhotographyScheduledEquipment, "chkPhotogSchedEquip_OH")
                                helper.AddCSSClassToControl(txtPhotogScheduledEquipmentLimit, "txtPhotogLimit_OH")
                                trPhotographyDataRow.Attributes.Add("class", "trPhotogDataRow_OH")
                                trPhotogScheduledEquipmentRow.Attributes.Add("class", "trPhotogScheduleEquipDataRow_OH")
                                trPhotographyMakeupAndHairCheckboxRow.Attributes.Add("class", "trPhotogMakeupAndHairCheckboxRow_OH")
                                Exit Select
                        End Select
                    Else
                        chkPhotography.Checked = False
                        trPhotographyCheckboxRow.Visible = False
                        trPhotographyDataRow.Attributes.Add("style", "display:none")
                        trPhotographyMakeupAndHairCheckboxRow.Attributes.Add("style", "display:none")
                        chkPhotographyScheduledEquipment.Checked = False
                        trPhotographyDataRow.Attributes.Add("style", "display:none")
                        chkPhotogMakeupAndHair.Checked = False
                    End If

                    ' SELF-STORAGE
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkSelfStorage.Attributes("class") = ""
                    txtStorageLimit.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.SelfStorage, hascov, err) Then
                        trSelfStorageCheckBoxRow.Visible = True
                        If hascov Then
                            chkSelfStorage.Checked = True
                            trSelfStorageDataRow.Attributes.Add("style", "display:''")
                            If MyBuilding.SelfStorageFacilityLimit IsNot Nothing AndAlso IsNumeric(MyBuilding.SelfStorageFacilityLimit) Then
                                'Updated 10/13/2021 for BOP Endorsements Task 65794 MLW
                                If IsQuoteReadOnly() Then
                                    txtStorageLimit.Width = "100"
                                    txtStorageLimit.Text = Format(CDec(MyBuilding.SelfStorageFacilityLimit), "0")
                                Else
                                    Dim amt As Decimal = 0
                                    ' Calculate and display the amount over 50,000
                                    If CDec(MyBuilding.SelfStorageFacilityLimit) > 50000 Then
                                        amt = CDec(MyBuilding.SelfStorageFacilityLimit) - 50000
                                        txtStorageLimit.Text = Format(amt, "0")
                                    Else
                                        txtStorageLimit.Text = ""
                                    End If
                                End If
                            End If
                        Else
                            chkSelfStorage.Checked = False
                            trSelfStorageDataRow.Attributes.Add("style", "display:none")
                            txtStorageLimit.Text = ""
                        End If
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.SelfStorage, err) Then
                            chkSelfStorage.Enabled = True
                            txtStorageLimit.Enabled = True
                        Else
                            chkSelfStorage.Enabled = False
                            txtStorageLimit.Enabled = False
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                helper.AddCSSClassToControl(chkSelfStorage, "chkSelfStorage_IL")
                                helper.AddCSSClassToControl(txtStorageLimit, "txtSelfStorageLimit_IL")
                                trSelfStorageDataRow.Attributes.Add("class", "trSelfStorageDataRow_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                helper.AddCSSClassToControl(chkSelfStorage, "chkSelfStorage_IN")
                                helper.AddCSSClassToControl(txtStorageLimit, "txtSelfStorageLimit_IN")
                                trSelfStorageDataRow.Attributes.Add("class", "trSelfStorageDataRow_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                helper.AddCSSClassToControl(chkSelfStorage, "chkSelfStorage_OH")
                                helper.AddCSSClassToControl(txtStorageLimit, "txtSelfStorageLimit_OH")
                                trSelfStorageDataRow.Attributes.Add("class", "trSelfStorageDataRow_OH")
                                Exit Select
                        End Select
                    Else
                        chkSelfStorage.Checked = False
                        trSelfStorageCheckBoxRow.Visible = False
                        trSelfStorageDataRow.Attributes.Add("style", "display:none")
                        txtStorageLimit.Text = ""
                    End If

                    ' VETERINARIANS
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkVeterinarians.Attributes("class") = ""
                    txtVeterinariansNumOfEmployees.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.Veterinarians, hascov, err) Then
                        trVeterinariansCheckBoxRow.Visible = True
                        If hascov Then
                            chkVeterinarians.Checked = True
                            trVeterinariansDataRow.Attributes.Add("style", "display:''")
                            txtVeterinariansNumOfEmployees.Text = MyBuilding.VeterinariansProfessionalLiabilityEmpNum
                        Else
                            chkVeterinarians.Checked = False
                            trVeterinariansDataRow.Attributes.Add("style", "display:none")
                            txtVeterinariansNumOfEmployees.Text = ""
                        End If
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Veterinarians, err) Then
                            chkVeterinarians.Enabled = True
                            txtVeterinariansNumOfEmployees.Enabled = True
                        Else
                            chkVeterinarians.Enabled = False
                            txtVeterinariansNumOfEmployees.Enabled = False
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                helper.AddCSSClassToControl(chkVeterinarians, "chkVet_IL")
                                helper.AddCSSClassToControl(txtVeterinariansNumOfEmployees, "txtVetNumEmp_IL")
                                trVeterinariansDataRow.Attributes.Add("class", "trVetDataRow_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                helper.AddCSSClassToControl(chkVeterinarians, "chkVet_IN")
                                helper.AddCSSClassToControl(txtVeterinariansNumOfEmployees, "txtVetNumEmp_IN")
                                trVeterinariansDataRow.Attributes.Add("class", "trVetDataRow_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                helper.AddCSSClassToControl(chkVeterinarians, "chkVet_OH")
                                helper.AddCSSClassToControl(txtVeterinariansNumOfEmployees, "txtVetNumEmp_OH")
                                trVeterinariansDataRow.Attributes.Add("class", "trVetDataRow_OH")
                                Exit Select
                        End Select
                    Else
                        chkVeterinarians.Checked = False
                        trVeterinariansCheckBoxRow.Visible = False
                        trVeterinariansDataRow.Attributes.Add("style", "display:none")
                        txtVeterinariansNumOfEmployees.Text = ""
                    End If

                    ' MOTEL
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkMotel.Attributes("class") = ""
                    ddlMotelLiabilityLimit.Attributes("class") = ""
                    ddlMotelSafeDepositBoxLimit.Attributes("class") = ""
                    ddlMotelSafeDepositBoxDeductible.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.Motel, hascov, err) Then
                        trMotelCheckboxRow.Visible = True
                        If hascov Then
                            chkMotel.Checked = True
                            trMotelDataRow.Attributes.Add("style", "display:''")
                            SetFromValue(ddlMotelLiabilityLimit, MyBuilding.MotelCoveragePerGuestLimitId)
                            'ddlMotelLiabilityLimit.SelectedValue = MyBuilding.MotelCoveragePerGuestLimitId
                            SetFromValue(ddlMotelSafeDepositBoxLimit, MyBuilding.MotelCoverageSafeDepositLimitId)
                            'ddlMotelSafeDepositBoxLimit.SelectedValue = MyBuilding.MotelCoverageSafeDepositLimitId
                            SetFromValue(ddlMotelSafeDepositBoxDeductible, MyBuilding.MotelCoverageSafeDepositDeductibleId)
                            'ddlMotelSafeDepositBoxDeductible.SelectedValue = MyBuilding.MotelCoverageSafeDepositDeductibleId
                        Else
                            chkMotel.Checked = False
                            trMotelDataRow.Attributes.Add("style", "display:none")
                            ddlMotelLiabilityLimit.SelectedIndex = -1
                            ddlMotelSafeDepositBoxDeductible.SelectedIndex = -1
                            ddlMotelSafeDepositBoxLimit.SelectedIndex = -1
                        End If
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Motel, err) Then
                            chkMotel.Enabled = True
                            ddlMotelLiabilityLimit.Enabled = True
                            ddlMotelSafeDepositBoxDeductible.Enabled = True
                            ddlMotelSafeDepositBoxLimit.Enabled = True
                        Else
                            chkMotel.Enabled = False
                            ddlMotelLiabilityLimit.Enabled = False
                            ddlMotelSafeDepositBoxDeductible.Enabled = False
                            ddlMotelSafeDepositBoxLimit.Enabled = False
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                helper.AddCSSClassToControl(chkMotel, "chkMotel_IL")
                                helper.AddCSSClassToControl(ddlMotelLiabilityLimit, "ddMotelLiabLimit_IL")
                                helper.AddCSSClassToControl(ddlMotelSafeDepositBoxLimit, "ddMotelSafeLimit_IL")
                                helper.AddCSSClassToControl(ddlMotelSafeDepositBoxDeductible, "ddMotelSafeDed_IL")
                                trMotelDataRow.Attributes.Add("class", "trMotelDataRow_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                helper.AddCSSClassToControl(chkMotel, "chkMotel_IN")
                                helper.AddCSSClassToControl(ddlMotelLiabilityLimit, "ddMotelLiabLimit_IN")
                                helper.AddCSSClassToControl(ddlMotelSafeDepositBoxLimit, "ddMotelSafeLimit_IN")
                                helper.AddCSSClassToControl(ddlMotelSafeDepositBoxDeductible, "ddMotelSafeDed_IN")
                                trMotelDataRow.Attributes.Add("class", "trMotelDataRow_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                helper.AddCSSClassToControl(chkMotel, "chkMotel_OH")
                                helper.AddCSSClassToControl(ddlMotelLiabilityLimit, "ddMotelLiabLimit_OH")
                                helper.AddCSSClassToControl(ddlMotelSafeDepositBoxLimit, "ddMotelSafeLimit_OH")
                                helper.AddCSSClassToControl(ddlMotelSafeDepositBoxDeductible, "ddMotelSafeDed_OH")
                                trMotelDataRow.Attributes.Add("class", "trMotelDataRow_OH")
                                Exit Select
                        End Select
                    Else
                        chkMotel.Checked = False
                        trMotelCheckboxRow.Visible = False
                        trMotelDataRow.Attributes.Add("style", "display:none")
                        ddlMotelLiabilityLimit.SelectedIndex = -1
                        ddlMotelSafeDepositBoxDeductible.SelectedIndex = -1
                        ddlMotelSafeDepositBoxLimit.SelectedIndex = -1
                    End If

                    ' APARTMENTS
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkApartments.Attributes("class") = ""
                    txtAptsNumberOfLocationsWithApts.Attributes("class") = ""
                    ddlAptAutoLiabilityLimit.Attributes("class") = ""
                    ddlAptDeductible.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.Apartments, hascov, err) Then
                        trApartmentsCheckboxRow.Visible = True
                        If hascov Then
                            chkApartments.Checked = True
                            trApartmentsDataRow.Attributes.Add("style", "display:''")
                            trApartmentsDataRow.Visible = True

                            ' Use the values at the QUOTE & LOCATION levels, not the building level
                            txtAptsNumberOfLocationsWithApts.Text = MyBuilding.NumberOfLocationsWithApartments   ' Use the building level property MGB 12/10/18
                            'txtAptsNumberOfLocationsWithApts.Text = Quote.NumberOfLocationsWithApartments
                            If Quote.Locations(LocationIndex).HasTenantAutoLegalLiability Then
                                SetFromValue(ddlAptAutoLiabilityLimit, Quote.Locations(LocationIndex).TenantAutoLegalLiabilityLimitOfLiabilityId)
                                'ddlAptAutoLiabilityLimit.SelectedValue = Quote.Locations(LocationIndex).TenantAutoLegalLiabilityLimitOfLiabilityId
                                SetFromValue(ddlAptDeductible, Quote.Locations(LocationIndex).TenantAutoLegalLiabilityDeductibleId)
                                'ddlAptDeductible.SelectedValue = Quote.Locations(LocationIndex).TenantAutoLegalLiabilityDeductibleId
                            Else
                                ddlAptAutoLiabilityLimit.SelectedIndex = 0
                                ddlAptDeductible.SelectedIndex = 0
                            End If

                            ' Disable the inputs on all but the first building with the coverage
                            If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Apartments, err) Then
                                chkApartments.Enabled = True
                                txtAptsNumberOfLocationsWithApts.Enabled = True
                                ddlAptAutoLiabilityLimit.Enabled = True
                                ddlAptDeductible.Enabled = True
                            Else
                                chkApartments.Enabled = False
                                txtAptsNumberOfLocationsWithApts.Enabled = False
                                ddlAptAutoLiabilityLimit.Enabled = False
                                ddlAptDeductible.Enabled = False
                            End If
                        Else
                            chkApartments.Checked = False
                            trApartmentsDataRow.Attributes.Add("style", "display:none")
                            txtAptsNumberOfLocationsWithApts.Text = ""
                            ddlAptAutoLiabilityLimit.SelectedIndex = -1
                            ddlAptDeductible.SelectedIndex = -1
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                helper.AddCSSClassToControl(chkApartments, "chkApt_IL")
                                trApartmentsDataRow.Attributes.Add("class", "trAptDataRow_IL")
                                helper.AddCSSClassToControl(txtAptsNumberOfLocationsWithApts, "txtAptNumLocs_IL")
                                helper.AddCSSClassToControl(ddlAptAutoLiabilityLimit, "ddAptLimit_IL")
                                helper.AddCSSClassToControl(ddlAptDeductible, "ddAptDed_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                helper.AddCSSClassToControl(chkApartments, "chkApt_IN")
                                trApartmentsDataRow.Attributes.Add("class", "trAptDataRow_IN")
                                helper.AddCSSClassToControl(txtAptsNumberOfLocationsWithApts, "txtAptNumLocs_IN")
                                helper.AddCSSClassToControl(ddlAptAutoLiabilityLimit, "ddAptLimit_IN")
                                helper.AddCSSClassToControl(ddlAptDeductible, "ddAptDed_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                helper.AddCSSClassToControl(chkApartments, "chkApt_OH")
                                trApartmentsDataRow.Attributes.Add("class", "trAptDataRow_OH")
                                helper.AddCSSClassToControl(txtAptsNumberOfLocationsWithApts, "txtAptNumLocs_OH")
                                helper.AddCSSClassToControl(ddlAptAutoLiabilityLimit, "ddAptLimit_OH")
                                helper.AddCSSClassToControl(ddlAptDeductible, "ddAptDed_OH")
                                Exit Select
                        End Select
                    Else
                        chkApartments.Checked = False
                        trApartmentsCheckboxRow.Visible = False
                        trApartmentsDataRow.Attributes.Add("style", "display:none")
                        txtAptsNumberOfLocationsWithApts.Text = ""
                        ddlAptAutoLiabilityLimit.SelectedIndex = -1
                        ddlAptDeductible.SelectedIndex = -1
                    End If

                    ' RESIDENTIAL CLEANING
                    hascov = False

                    ' Remove the classes from the coverage checkbox and controls
                    chkResidentialCleaning.Attributes("class") = ""

                    If BuildingCanHaveOptionalClassCoverage(ClassCoverage_Enum.ResidentialCleaning, hascov, err) Then
                        trResidentialCleaningCheckboxRow.Visible = True
                        If hascov Then
                            chkResidentialCleaning.Checked = True
                        Else
                            chkResidentialCleaning.Checked = False
                        End If

                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.ResidentialCleaning, err) Then
                            chkResidentialCleaning.Enabled = True
                        Else
                            chkResidentialCleaning.Enabled = False
                        End If

                        ' Assign classes to the coverage checkbox and data controls
                        ' We'll use these classes in the script to format the elements
                        Select Case MyLocation.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                helper.AddCSSClassToControl(chkResidentialCleaning, "chkResCleaning_IL")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                helper.AddCSSClassToControl(chkResidentialCleaning, "chkResCleaning_IN")
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                                helper.AddCSSClassToControl(chkResidentialCleaning, "chkResCleaning_OH")
                                Exit Select
                        End Select
                    Else
                        chkResidentialCleaning.Checked = False
                        trResidentialCleaningCheckboxRow.Visible = False
                    End If

                End If

                trPhotogScheduledItems.Visible = False 'Added 10/18/2021 for BOP Endorsements Task 65807 MLW
                'Added 09/09/2021 for BOP Endorsements Task 63912 MLW
                If IsQuoteReadOnly() Then
                    trAccountsReceivableOnPremInfoText.Attributes.Add("style", "display:none")
                    trValuablePapersOnPremInfoText.Attributes.Add("style", "display:none")
                    trOrdinanceOrLawInfoText.Attributes.Add("style", "display:none")
                    trSpoilageInfoText.Attributes.Add("style", "display:none")
                    trApartmentsInfoText.Attributes.Add("style", "display:none")
                    trBarbersInfoText.Attributes.Add("style", "display:none")
                    trBeautyShopsInfoText.Attributes.Add("style", "display:none")
                    trFuneralDirectorsInfoText.Attributes.Add("style", "display:none")
                    trLiquorLiabilityInfoText.Attributes.Add("style", "display:none")
                    trMotelInfoText.Attributes.Add("style", "display:none")
                    trOpticalAndHearingInfoText.Attributes.Add("style", "display:none")
                    trPharmacistsInfoText.Attributes.Add("style", "display:none")
                    trPhotographyInfoText.Attributes.Add("style", "display:none")
                    trPrintersInfoText.Attributes.Add("style", "display:none")
                    trRestaurantsInfoText.Attributes.Add("style", "display:none")
                    trSelfStorageInfoText.Attributes.Add("style", "display:none")
                    trVeterinariansInfoText.Attributes.Add("style", "display:none")
                    trOtherOptionalCoveragesInfoText.Attributes.Add("style", "display:none")
                    'Added 10/18/2021 for BOP Endorsements Task 65807 MLW
                    If chkPhotographyScheduledEquipment.Checked = True Then
                        trPhotogScheduledEquipmentRow.Visible = False
                        trPhotogScheduledItems.Visible = True
                    End If
                End If
            End If

            Me.PopulateChildControls()

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
            Exit Sub
        End Try
    End Sub

    Private Function BuildingHasOptionalCoverage(ByVal Building As QuickQuote.CommonObjects.QuickQuoteBuilding, ByVal CovType As ClassCoverage_Enum) As Boolean
        Dim ClassCodes As New List(Of String)
        Try
            If Building IsNot Nothing Then
                If Building.BuildingClassifications IsNot Nothing AndAlso Building.BuildingClassifications.Count > 0 Then
                    For Each cl As QuickQuote.CommonObjects.QuickQuoteClassification In Building.BuildingClassifications
                        ' Get the class codes we want to check for based on the passed coverage type
                        Select Case CovType
                            Case ClassCoverage_Enum.Apartments
                                ' Apartment Class Codes
                                ClassCodes.Add("65132")
                                ClassCodes.Add("65133")
                                ClassCodes.Add("65141")
                                ClassCodes.Add("65142")
                                ClassCodes.Add("65146")
                                ClassCodes.Add("65147")
                                ClassCodes.Add("69145")
                                Exit Select
                            Case ClassCoverage_Enum.Barbers
                                ClassCodes.Add("71332")
                                Exit Select
                            Case ClassCoverage_Enum.Beauticians
                                ClassCodes.Add("71952")
                                Exit Select
                            Case ClassCoverage_Enum.FineArts
                                ' Apartment Class Codes
                                ClassCodes.Add("65132")
                                ClassCodes.Add("65133")
                                ClassCodes.Add("65141")
                                ClassCodes.Add("65142")
                                ClassCodes.Add("65146")
                                ClassCodes.Add("65147")
                                ClassCodes.Add("69145")
                                ' Restaurant Class Codes
                                ClassCodes.Add("9001")
                                ClassCodes.Add("9011")
                                ClassCodes.Add("9021")
                                ClassCodes.Add("9031")
                                ClassCodes.Add("9041")
                                ClassCodes.Add("9051")
                                ClassCodes.Add("9061")
                                ClassCodes.Add("9071")
                                ClassCodes.Add("9081")
                                ClassCodes.Add("9091")
                                ClassCodes.Add("9101")
                                ClassCodes.Add("9111")
                                ClassCodes.Add("9121")
                                ClassCodes.Add("9131")
                                ClassCodes.Add("9141")
                                ClassCodes.Add("9151")
                                ClassCodes.Add("9161")
                                ClassCodes.Add("9171")
                                ClassCodes.Add("9181")
                                ClassCodes.Add("9191")
                                ClassCodes.Add("9201")
                                ClassCodes.Add("9211")
                                ClassCodes.Add("9221")
                                ClassCodes.Add("9231")
                                ClassCodes.Add("9241")
                                ClassCodes.Add("9251")
                                ClassCodes.Add("9261")
                                ClassCodes.Add("9331")
                                ClassCodes.Add("9341")
                                ClassCodes.Add("9351")
                                ClassCodes.Add("9361")
                                ClassCodes.Add("9421")
                                ClassCodes.Add("9431")
                                ClassCodes.Add("9441")
                                ClassCodes.Add("9611")
                                ClassCodes.Add("9621")
                                ClassCodes.Add("9631")
                                ClassCodes.Add("9641")
                                ClassCodes.Add("9651")
                                ClassCodes.Add("9661")
                                Exit Select
                            Case ClassCoverage_Enum.FuneralDirectors
                                ClassCodes.Add("71865")
                                Exit Select
                            Case ClassCoverage_Enum.LiquorLiability
                                ' Motel Class Codes (minus the one without restaurants)
                                ClassCodes.Add("69161")
                                ClassCodes.Add("69171")
                                ' Restaurant Class Codes
                                ClassCodes.Add("9001")
                                ClassCodes.Add("9011")
                                ClassCodes.Add("9021")
                                ClassCodes.Add("9031")
                                ClassCodes.Add("9041")
                                ClassCodes.Add("9051")
                                ClassCodes.Add("9061")
                                ClassCodes.Add("9071")
                                ClassCodes.Add("9081")
                                ClassCodes.Add("9091")
                                ClassCodes.Add("9101")
                                ClassCodes.Add("9111")
                                ClassCodes.Add("9121")
                                ClassCodes.Add("9131")
                                ClassCodes.Add("9141")
                                ClassCodes.Add("9151")
                                ClassCodes.Add("9161")
                                ClassCodes.Add("9171")
                                ClassCodes.Add("9181")
                                ClassCodes.Add("9191")
                                ClassCodes.Add("9201")
                                ClassCodes.Add("9211")
                                ClassCodes.Add("9221")
                                ClassCodes.Add("9231")
                                ClassCodes.Add("9241")
                                ClassCodes.Add("9251")
                                ClassCodes.Add("9261")
                                ClassCodes.Add("9331")
                                ClassCodes.Add("9341")
                                ClassCodes.Add("9351")
                                ClassCodes.Add("9361")
                                ClassCodes.Add("9421")
                                ClassCodes.Add("9431")
                                ClassCodes.Add("9441")
                                ClassCodes.Add("9611")
                                ClassCodes.Add("9621")
                                ClassCodes.Add("9631")
                                ClassCodes.Add("9641")
                                ClassCodes.Add("9651")
                                ClassCodes.Add("9661")
                                ' Grocery Store Class Codes
                                ClassCodes.Add("54321")
                                ClassCodes.Add("54331")
                                ClassCodes.Add("54341")
                                ClassCodes.Add("54351")
                                ClassCodes.Add("54136")
                                ' Convenience Store Class Codes
                                ClassCodes.Add("51436")
                                ClassCodes.Add("9321")
                                ClassCodes.Add("9331")
                                ClassCodes.Add("9341")
                                ClassCodes.Add("9351")
                                ClassCodes.Add("9361")
                                ' Supermarkets Class Codes
                                ClassCodes.Add("54221")
                                ClassCodes.Add("54231")
                                ClassCodes.Add("54241")
                                ClassCodes.Add("54251")
                                Exit Select
                            Case ClassCoverage_Enum.Motel
                                ' Motel Class Codes (minus the one without restaurants)
                                ClassCodes.Add("69151")
                                ClassCodes.Add("69161")
                                ClassCodes.Add("69171")
                                Exit Select
                            Case ClassCoverage_Enum.OpticalAndHearingAid
                                ClassCodes.Add("50721")
                                ClassCodes.Add("59954")
                                ClassCodes.Add("50571")
                                ClassCodes.Add("59974")
                                Exit Select
                            Case ClassCoverage_Enum.Pharmacist
                                ClassCodes.Add("59116")
                                Exit Select
                            Case ClassCoverage_Enum.Photography
                                ClassCodes.Add("71899")
                                Exit Select
                            Case ClassCoverage_Enum.Printers
                                ClassCodes.Add("71877")
                                ClassCodes.Add("71842")
                                ClassCodes.Add("71888")
                                ClassCodes.Add("71912")
                                Exit Select
                            Case ClassCoverage_Enum.ResidentialCleaning
                                ClassCodes.Add("76221")
                                ClassCodes.Add("76231")
                                Exit Select
                            Case ClassCoverage_Enum.Restaurant
                                ' Restaurant Class Codes
                                ClassCodes.Add("9001")
                                ClassCodes.Add("9011")
                                ClassCodes.Add("9021")
                                ClassCodes.Add("9031")
                                ClassCodes.Add("9041")
                                ClassCodes.Add("9051")
                                ClassCodes.Add("9061")
                                ClassCodes.Add("9071")
                                ClassCodes.Add("9081")
                                ClassCodes.Add("9091")
                                ClassCodes.Add("9101")
                                ClassCodes.Add("9111")
                                ClassCodes.Add("9121")
                                ClassCodes.Add("9131")
                                ClassCodes.Add("9141")
                                ClassCodes.Add("9151")
                                ClassCodes.Add("9161")
                                ClassCodes.Add("9171")
                                ClassCodes.Add("9181")
                                ClassCodes.Add("9191")
                                ClassCodes.Add("9201")
                                ClassCodes.Add("9211")
                                ClassCodes.Add("9221")
                                ClassCodes.Add("9231")
                                ClassCodes.Add("9241")
                                ClassCodes.Add("9251")
                                ClassCodes.Add("9261")
                                ClassCodes.Add("9331")
                                ClassCodes.Add("9341")
                                ClassCodes.Add("9351")
                                ClassCodes.Add("9361")
                                ClassCodes.Add("9421")
                                ClassCodes.Add("9431")
                                ClassCodes.Add("9441")
                                ClassCodes.Add("9611")
                                ClassCodes.Add("9621")
                                ClassCodes.Add("9631")
                                ClassCodes.Add("9641")
                                ClassCodes.Add("9651")
                                ClassCodes.Add("9661")
                                ' Motel Class Codes (minus the one without restaurants)
                                ClassCodes.Add("69161")
                                ClassCodes.Add("69171")
                                Exit Select
                            Case ClassCoverage_Enum.SelfStorage
                                ClassCodes.Add("9411")
                                Exit Select
                            Case ClassCoverage_Enum.Veterinarians
                                ClassCodes.Add("64181")
                                ClassCodes.Add("64191")
                                ClassCodes.Add("60999")
                                ClassCodes.Add("65198")
                                ClassCodes.Add("65121")
                                Exit Select
                            Case Else
                                Return False
                        End Select

                        ' Loop thru all the building's classifications and see if the class code we're looking for is on the building
                        For Each cls As QuickQuote.CommonObjects.QuickQuoteClassification In Building.BuildingClassifications
                            For Each cc As String In ClassCodes
                                If IsNumeric(cc) AndAlso IsNumeric(cls.ClassCode) Then
                                    If CDec(cc) = CDec(cls.ClassCode) Then
                                        Return True
                                    End If
                                Else
                                    If cls.ClassCode.Trim <> "" Then
                                        If cc = cls.ClassCode Then
                                            Return True
                                        End If
                                    End If
                                End If
                            Next
                        Next


                    Next
                End If
            End If

            Return False
        Catch ex As Exception
            HandleError("BuildingHasCoverage", ex)
            Return False
        End Try
    End Function

    '' ORIGINAL 
    'Private Function BuildingHasOptionalCoverage(ByVal Building As QuickQuote.CommonObjects.QuickQuoteBuilding, ByVal CovType As ClassCoverage_Enum) As Boolean
    '    Dim ClassCodes As New List(Of String)
    '    Try
    '        If Building IsNot Nothing Then
    '            If Building.BuildingClassifications IsNot Nothing AndAlso Building.BuildingClassifications.Count > 0 Then
    '                For Each cl As QuickQuote.CommonObjects.QuickQuoteClassification In Building.BuildingClassifications
    '                    ' Get the class codes we want to check for based on the passed coverage type
    '                    Select Case CovType
    '                        Case ClassCoverage_Enum.Apartments
    '                            ' Apartment Class Codes
    '                            ClassCodes.Add("65132")
    '                            ClassCodes.Add("65133")
    '                            ClassCodes.Add("65141")
    '                            ClassCodes.Add("65142")
    '                            ClassCodes.Add("65146")
    '                            ClassCodes.Add("65147")
    '                            ClassCodes.Add("69145")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.Barbers
    '                            ClassCodes.Add("71332")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.Beauticians
    '                            ClassCodes.Add("71952")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.FineArts
    '                            ' Apartment Class Codes
    '                            ClassCodes.Add("65132")
    '                            ClassCodes.Add("65133")
    '                            ClassCodes.Add("65141")
    '                            ClassCodes.Add("65142")
    '                            ClassCodes.Add("65146")
    '                            ClassCodes.Add("65147")
    '                            ClassCodes.Add("69145")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.FuneralDirectors
    '                            ClassCodes.Add("71865")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.LiquorLiability
    '                            ' Motel Class Codes (minus the one without restaurants)
    '                            ClassCodes.Add("69161")
    '                            ClassCodes.Add("69171")
    '                            ' Restaurant Class Codes
    '                            ClassCodes.Add("9001")
    '                            ClassCodes.Add("9011")
    '                            ClassCodes.Add("9021")
    '                            ClassCodes.Add("9031")
    '                            ClassCodes.Add("9041")
    '                            ClassCodes.Add("9051")
    '                            ClassCodes.Add("9061")
    '                            ClassCodes.Add("9071")
    '                            ClassCodes.Add("9081")
    '                            ClassCodes.Add("9091")
    '                            ClassCodes.Add("9101")
    '                            ClassCodes.Add("9111")
    '                            ClassCodes.Add("9121")
    '                            ClassCodes.Add("9131")
    '                            ClassCodes.Add("9141")
    '                            ClassCodes.Add("9151")
    '                            ClassCodes.Add("9161")
    '                            ClassCodes.Add("9171")
    '                            ClassCodes.Add("9181")
    '                            ClassCodes.Add("9191")
    '                            ClassCodes.Add("9201")
    '                            ClassCodes.Add("9211")
    '                            ClassCodes.Add("9221")
    '                            ClassCodes.Add("9231")
    '                            ClassCodes.Add("9241")
    '                            ClassCodes.Add("9251")
    '                            ClassCodes.Add("9261")
    '                            ClassCodes.Add("9331")
    '                            ClassCodes.Add("9341")
    '                            ClassCodes.Add("9351")
    '                            ClassCodes.Add("9361")
    '                            ClassCodes.Add("9421")
    '                            ClassCodes.Add("9431")
    '                            ClassCodes.Add("9441")
    '                            ClassCodes.Add("9611")
    '                            ClassCodes.Add("9621")
    '                            ClassCodes.Add("9631")
    '                            ClassCodes.Add("9641")
    '                            ClassCodes.Add("9651")
    '                            ClassCodes.Add("9661")
    '                            ' Grocery Store Class Codes
    '                            ClassCodes.Add("54321")
    '                            ClassCodes.Add("54331")
    '                            ClassCodes.Add("54341")
    '                            ClassCodes.Add("54351")
    '                            ClassCodes.Add("54136")
    '                            ' Convenience Store Class Codes
    '                            ClassCodes.Add("51436")
    '                            ClassCodes.Add("9321")
    '                            ClassCodes.Add("9331")
    '                            ClassCodes.Add("9341")
    '                            ClassCodes.Add("9351")
    '                            ClassCodes.Add("9361")
    '                            ' Supermarkets Class Codes
    '                            ClassCodes.Add("54221")
    '                            ClassCodes.Add("54231")
    '                            ClassCodes.Add("54241")
    '                            ClassCodes.Add("54251")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.Motel
    '                            ' Motel Class Codes (minus the one without restaurants)
    '                            ClassCodes.Add("69151")
    '                            ClassCodes.Add("69161")
    '                            ClassCodes.Add("69171")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.OpticalAndHearingAid
    '                            ClassCodes.Add("50721")
    '                            ClassCodes.Add("59954")
    '                            ClassCodes.Add("50571")
    '                            ClassCodes.Add("59974")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.Pharmacist
    '                            ClassCodes.Add("59116")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.Photography
    '                            ClassCodes.Add("71899")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.Printers
    '                            ClassCodes.Add("71877")
    '                            ClassCodes.Add("71842")
    '                            ClassCodes.Add("71888")
    '                            ClassCodes.Add("71912")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.ResidentialCleaning
    '                            ClassCodes.Add("76221")
    '                            ClassCodes.Add("76231")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.Restaurant
    '                            ' Restaurant Class Codes
    '                            ClassCodes.Add("9001")
    '                            ClassCodes.Add("9011")
    '                            ClassCodes.Add("9021")
    '                            ClassCodes.Add("9031")
    '                            ClassCodes.Add("9041")
    '                            ClassCodes.Add("9051")
    '                            ClassCodes.Add("9061")
    '                            ClassCodes.Add("9071")
    '                            ClassCodes.Add("9081")
    '                            ClassCodes.Add("9091")
    '                            ClassCodes.Add("9101")
    '                            ClassCodes.Add("9111")
    '                            ClassCodes.Add("9121")
    '                            ClassCodes.Add("9131")
    '                            ClassCodes.Add("9141")
    '                            ClassCodes.Add("9151")
    '                            ClassCodes.Add("9161")
    '                            ClassCodes.Add("9171")
    '                            ClassCodes.Add("9181")
    '                            ClassCodes.Add("9191")
    '                            ClassCodes.Add("9201")
    '                            ClassCodes.Add("9211")
    '                            ClassCodes.Add("9221")
    '                            ClassCodes.Add("9231")
    '                            ClassCodes.Add("9241")
    '                            ClassCodes.Add("9251")
    '                            ClassCodes.Add("9261")
    '                            ClassCodes.Add("9331")
    '                            ClassCodes.Add("9341")
    '                            ClassCodes.Add("9351")
    '                            ClassCodes.Add("9361")
    '                            ClassCodes.Add("9421")
    '                            ClassCodes.Add("9431")
    '                            ClassCodes.Add("9441")
    '                            ClassCodes.Add("9611")
    '                            ClassCodes.Add("9621")
    '                            ClassCodes.Add("9631")
    '                            ClassCodes.Add("9641")
    '                            ClassCodes.Add("9651")
    '                            ClassCodes.Add("9661")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.SelfStorage
    '                            ClassCodes.Add("9411")
    '                            Exit Select
    '                        Case ClassCoverage_Enum.Veterinarians
    '                            ClassCodes.Add("64181")
    '                            ClassCodes.Add("64191")
    '                            ClassCodes.Add("60999")
    '                            ClassCodes.Add("65198")
    '                            ClassCodes.Add("65121")
    '                            Exit Select
    '                        Case Else
    '                            Return False
    '                    End Select

    '                    ' Loop thru all the building's classifications and see if the class code we're looking for is on the building
    '                    For Each cls As QuickQuote.CommonObjects.QuickQuoteClassification In Building.BuildingClassifications
    '                        For Each cc As String In ClassCodes
    '                            If IsNumeric(cc) AndAlso IsNumeric(cls.ClassCode) Then
    '                                If CDec(cc) = CDec(cls.ClassCode) Then
    '                                    Return True
    '                                End If
    '                            Else
    '                                If cls.ClassCode.Trim <> "" Then
    '                                    If cc = cls.ClassCode Then
    '                                        Return True
    '                                    End If
    '                                End If
    '                            End If
    '                        Next
    '                    Next


    '                Next
    '            End If
    '        End If

    '        Return False
    '    Catch ex As Exception
    '        HandleError("BuildingHasCoverage", ex)
    '        Return False
    '    End Try
    'End Function



    'Private Function BuildingHasOptionalCoverage(ByVal Building As QuickQuote.CommonObjects.QuickQuoteBuilding, ByVal CovType As ClassCoverage_Enum) As Boolean
    '    Dim ClassCodes As New List(Of String)
    '    Try
    '        If Building IsNot Nothing Then

    '            Select Case CovType
    '                Case ClassCoverage_Enum.Apartments
    '                    If Building.HasApartmentBuildings Then Return True
    '                Case ClassCoverage_Enum.Barbers
    '                    If Building.HasBarbersProfessionalLiability Then Return True
    '                Case ClassCoverage_Enum.Beauticians
    '                    If Building.HasBeauticiansProfessionalLiability Then Return True
    '                Case ClassCoverage_Enum.FineArts
    '                    If Building.HasFineArts Then Return True
    '                Case ClassCoverage_Enum.FuneralDirectors
    '                    ClassCodes.Add("71865")
    '                Case ClassCoverage_Enum.LiquorLiability
    '                    If Building.HasLiquorLiability Then Return True
    '                Case ClassCoverage_Enum.Motel
    '                    If Building.HasMotelCoverage Then Return True
    '                Case ClassCoverage_Enum.OpticalAndHearingAid
    '                    If Building.HasOpticalAndHearingAidProfessionalLiability Then Return True
    '                Case ClassCoverage_Enum.Pharmacist
    '                    If Building.HasPharmacistProfessionalLiability Then Return True
    '                Case ClassCoverage_Enum.Photography
    '                    If Building.HasPhotographyCoverage Then Return True
    '                Case ClassCoverage_Enum.Printers
    '                    If Building.HasPrintersProfessionalLiability Then Return True
    '                Case ClassCoverage_Enum.ResidentialCleaning
    '                    If Building.HasResidentialCleaning Then Return True
    '                Case ClassCoverage_Enum.Restaurant
    '                    If Building.HasRestaurantEndorsement Then Return True
    '                Case ClassCoverage_Enum.SelfStorage
    '                    If Building.HasSelfStorageFacility Then Return True
    '                Case ClassCoverage_Enum.Veterinarians
    '                    If Building.HasVeterinariansProfessionalLiability Then Return True
    '                Case Else
    '                    Return False
    '            End Select
    '        End If

    '        Return False
    '    Catch ex As Exception
    '        HandleError("BuildingHasCoverage", ex)
    '        Return False
    '    End Try
    'End Function


    ''' <summary>
    ''' Pass in the coverage you want to see if a building is eligible for and this function will tell you if the building is eligible for that coverage. 
    ''' Also, if the building HAS the coverage on it, the HasCoverage flag will be set.
    ''' </summary>
    ''' <param name="CovType"></param>
    ''' <param name="HasCoverage"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    Private Function BuildingCanHaveOptionalClassCoverage(ByVal CovType As ClassCoverage_Enum, ByRef HasCoverage As Boolean, ByRef err As String) As Boolean
        Dim ClassCodes As New List(Of String)
        Dim BldgCanHaveCoverage As Boolean = False

        Try
            HasCoverage = False

            If MyBuilding IsNot Nothing Then
                Quote.CopyProfessionalLiabilityCoveragesFromPolicyToBuildings_UseBuildingClassificationList()
                ' Get the class codes we want to check for based on the passed coverage type
                Select Case CovType
                    Case ClassCoverage_Enum.Apartments
                        ' Apartment Class Codes
                        ClassCodes.Add("65132")
                        ClassCodes.Add("65133")
                        ClassCodes.Add("65141")
                        ClassCodes.Add("65142")
                        ClassCodes.Add("65146")
                        ClassCodes.Add("65147")
                        ClassCodes.Add("69145")
                        Exit Select
                    Case ClassCoverage_Enum.Barbers
                        ClassCodes.Add("71332")
                        Exit Select
                    Case ClassCoverage_Enum.Beauticians
                        ClassCodes.Add("71952")
                        Exit Select
                    Case ClassCoverage_Enum.FineArts
                        ' Apartment Class Codes
                        ClassCodes.Add("65132")
                        ClassCodes.Add("65133")
                        ClassCodes.Add("65141")
                        ClassCodes.Add("65142")
                        ClassCodes.Add("65146")
                        ClassCodes.Add("65147")
                        ClassCodes.Add("69145")
                        ' Restaurant Class Codes
                        ClassCodes.Add("9001")
                        ClassCodes.Add("9011")
                        ClassCodes.Add("9021")
                        ClassCodes.Add("9031")
                        ClassCodes.Add("9041")
                        ClassCodes.Add("9051")
                        ClassCodes.Add("9061")
                        ClassCodes.Add("9071")
                        ClassCodes.Add("9081")
                        ClassCodes.Add("9091")
                        ClassCodes.Add("9101")
                        ClassCodes.Add("9111")
                        ClassCodes.Add("9121")
                        ClassCodes.Add("9131")
                        ClassCodes.Add("9141")
                        ClassCodes.Add("9151")
                        ClassCodes.Add("9161")
                        ClassCodes.Add("9171")
                        ClassCodes.Add("9181")
                        ClassCodes.Add("9191")
                        ClassCodes.Add("9201")
                        ClassCodes.Add("9211")
                        ClassCodes.Add("9221")
                        ClassCodes.Add("9231")
                        ClassCodes.Add("9241")
                        ClassCodes.Add("9251")
                        ClassCodes.Add("9261")
                        ClassCodes.Add("9331")
                        ClassCodes.Add("9341")
                        ClassCodes.Add("9351")
                        ClassCodes.Add("9361")
                        ClassCodes.Add("9421")
                        ClassCodes.Add("9431")
                        ClassCodes.Add("9441")
                        ClassCodes.Add("9611")
                        ClassCodes.Add("9621")
                        ClassCodes.Add("9631")
                        ClassCodes.Add("9641")
                        ClassCodes.Add("9651")
                        ClassCodes.Add("9661")
                        Exit Select
                    Case ClassCoverage_Enum.FuneralDirectors
                        ClassCodes.Add("71865")
                        Exit Select
                    Case ClassCoverage_Enum.LiquorLiability
                        ' Motel Class Codes (minus the one without restaurants)
                        ClassCodes.Add("69161")
                        ClassCodes.Add("69171")
                        ' Restaurant Class Codes
                        ClassCodes.Add("9001")
                        ClassCodes.Add("9011")
                        ClassCodes.Add("9021")
                        ClassCodes.Add("9031")
                        ClassCodes.Add("9041")
                        ClassCodes.Add("9051")
                        ClassCodes.Add("9061")
                        ClassCodes.Add("9071")
                        ClassCodes.Add("9081")
                        ClassCodes.Add("9091")
                        ClassCodes.Add("9101")
                        ClassCodes.Add("9111")
                        ClassCodes.Add("9121")
                        ClassCodes.Add("9131")
                        ClassCodes.Add("9141")
                        ClassCodes.Add("9151")
                        ClassCodes.Add("9161")
                        ClassCodes.Add("9171")
                        ClassCodes.Add("9181")
                        ClassCodes.Add("9191")
                        ClassCodes.Add("9201")
                        ClassCodes.Add("9211")
                        ClassCodes.Add("9221")
                        ClassCodes.Add("9231")
                        ClassCodes.Add("9241")
                        ClassCodes.Add("9251")
                        ClassCodes.Add("9261")
                        ClassCodes.Add("9331")
                        ClassCodes.Add("9341")
                        ClassCodes.Add("9351")
                        ClassCodes.Add("9361")
                        ClassCodes.Add("9421")
                        ClassCodes.Add("9431")
                        ClassCodes.Add("9441")
                        ClassCodes.Add("9611")
                        ClassCodes.Add("9621")
                        ClassCodes.Add("9631")
                        ClassCodes.Add("9641")
                        ClassCodes.Add("9651")
                        ClassCodes.Add("9661")
                        ' Grocery Store Class Codes
                        ClassCodes.Add("54321")
                        ClassCodes.Add("54331")
                        ClassCodes.Add("54341")
                        ClassCodes.Add("54351")
                        ClassCodes.Add("54136")
                        ' Convenience Store Class Codes
                        ClassCodes.Add("51436")
                        ClassCodes.Add("9321")
                        ClassCodes.Add("9331")
                        ClassCodes.Add("9341")
                        ClassCodes.Add("9351")
                        ClassCodes.Add("9361")
                        ' Supermarkets Class Codes
                        ClassCodes.Add("54221")
                        ClassCodes.Add("54231")
                        ClassCodes.Add("54241")
                        ClassCodes.Add("54251")
                        Exit Select
                    Case ClassCoverage_Enum.Motel
                        ' Motel Class Codes (minus the one without restaurants)
                        ClassCodes.Add("69151")
                        ClassCodes.Add("69161")
                        ClassCodes.Add("69171")
                        Exit Select
                    Case ClassCoverage_Enum.OpticalAndHearingAid
                        ClassCodes.Add("50721")
                        ClassCodes.Add("59954")
                        ClassCodes.Add("50571")
                        ClassCodes.Add("59974")
                        Exit Select
                    Case ClassCoverage_Enum.Pharmacist
                        ClassCodes.Add("59116")
                        Exit Select
                    Case ClassCoverage_Enum.Photography
                        ClassCodes.Add("71899")
                        Exit Select
                    Case ClassCoverage_Enum.Printers
                        ClassCodes.Add("71877")
                        ClassCodes.Add("71842")
                        ClassCodes.Add("71888")
                        ClassCodes.Add("71912")
                        Exit Select
                    Case ClassCoverage_Enum.ResidentialCleaning
                        ClassCodes.Add("76221")
                        ClassCodes.Add("76231")
                        Exit Select
                    Case ClassCoverage_Enum.Restaurant
                        ' Restaurant Class Codes
                        ClassCodes.Add("9001")
                        ClassCodes.Add("9011")
                        ClassCodes.Add("9021")
                        ClassCodes.Add("9031")
                        ClassCodes.Add("9041")
                        ClassCodes.Add("9051")
                        ClassCodes.Add("9061")
                        ClassCodes.Add("9071")
                        ClassCodes.Add("9081")
                        ClassCodes.Add("9091")
                        ClassCodes.Add("9101")
                        ClassCodes.Add("9111")
                        ClassCodes.Add("9121")
                        ClassCodes.Add("9131")
                        ClassCodes.Add("9141")
                        ClassCodes.Add("9151")
                        ClassCodes.Add("9161")
                        ClassCodes.Add("9171")
                        ClassCodes.Add("9181")
                        ClassCodes.Add("9191")
                        ClassCodes.Add("9201")
                        ClassCodes.Add("9211")
                        ClassCodes.Add("9221")
                        ClassCodes.Add("9231")
                        ClassCodes.Add("9241")
                        ClassCodes.Add("9251")
                        ClassCodes.Add("9261")
                        ClassCodes.Add("9331")
                        ClassCodes.Add("9341")
                        ClassCodes.Add("9351")
                        ClassCodes.Add("9361")
                        ClassCodes.Add("9421")
                        ClassCodes.Add("9431")
                        ClassCodes.Add("9441")
                        ClassCodes.Add("9611")
                        ClassCodes.Add("9621")
                        ClassCodes.Add("9631")
                        ClassCodes.Add("9641")
                        ClassCodes.Add("9651")
                        ClassCodes.Add("9661")
                        ' Motel Class Codes (minus the one without restaurants)
                        ClassCodes.Add("69161")
                        ClassCodes.Add("69171")
                        Exit Select
                    Case ClassCoverage_Enum.SelfStorage
                        ClassCodes.Add("9411")
                        Exit Select
                    Case ClassCoverage_Enum.Veterinarians
                        ClassCodes.Add("64181")
                        ClassCodes.Add("64191")
                        ClassCodes.Add("60999")
                        ClassCodes.Add("65198")
                        ClassCodes.Add("65121")
                        Exit Select
                    Case Else
                        Return False
                End Select

                ' Loop thru all the building's classifications and see if the class code we're looking for is on the building
                If MyBuilding.BuildingClassifications IsNot Nothing AndAlso MyBuilding.BuildingClassifications.Count > 0 Then
                    For Each cls As QuickQuote.CommonObjects.QuickQuoteClassification In MyBuilding.BuildingClassifications
                        For Each cc As String In ClassCodes
                            If IsNumeric(cc) AndAlso IsNumeric(cls.ClassCode) Then
                                If CDec(cc) = CDec(cls.ClassCode) Then
                                    BldgCanHaveCoverage = True
                                End If
                            Else
                                If cls.ClassCode.Trim <> "" Then
                                    If cc = cls.ClassCode Then
                                        BldgCanHaveCoverage = True
                                    End If
                                End If
                            End If
                        Next
                    Next
                End If
            End If

            ' Now check to see if the building has the coverage
            If BldgCanHaveCoverage Then
                Select Case CovType
                    Case ClassCoverage_Enum.Apartments
                        HasCoverage = MyBuilding.HasApartmentBuildings
                        Exit Select
                    Case ClassCoverage_Enum.Barbers
                        HasCoverage = MyBuilding.HasBarbersProfessionalLiability
                        Exit Select
                    Case ClassCoverage_Enum.Beauticians
                        HasCoverage = MyBuilding.HasBeauticiansProfessionalLiability
                        Exit Select
                    Case ClassCoverage_Enum.FineArts
                        HasCoverage = MyBuilding.HasFineArts
                        Exit Select
                    Case ClassCoverage_Enum.FuneralDirectors
                        HasCoverage = MyBuilding.HasFuneralDirectorsProfessionalLiability
                        Exit Select
                    Case ClassCoverage_Enum.LiquorLiability
                        HasCoverage = MyBuilding.HasLiquorLiability
                        Exit Select
                    Case ClassCoverage_Enum.Motel
                        HasCoverage = MyBuilding.HasMotelCoverage
                        Exit Select
                    Case ClassCoverage_Enum.OpticalAndHearingAid
                        HasCoverage = MyBuilding.HasOpticalAndHearingAidProfessionalLiability
                        Exit Select
                    Case ClassCoverage_Enum.Pharmacist
                        HasCoverage = MyBuilding.HasPharmacistProfessionalLiability
                        Exit Select
                    Case ClassCoverage_Enum.Photography
                        HasCoverage = MyBuilding.HasPhotographyCoverage
                        Exit Select
                    Case ClassCoverage_Enum.Printers
                        HasCoverage = MyBuilding.HasPrintersProfessionalLiability
                        Exit Select
                    Case ClassCoverage_Enum.ResidentialCleaning
                        HasCoverage = MyBuilding.HasResidentialCleaning
                        Exit Select
                    Case ClassCoverage_Enum.Restaurant
                        HasCoverage = MyBuilding.HasRestaurantEndorsement
                        Exit Select
                    Case ClassCoverage_Enum.SelfStorage
                        HasCoverage = MyBuilding.HasSelfStorageFacility
                        Exit Select
                    Case ClassCoverage_Enum.Veterinarians
                        HasCoverage = MyBuilding.HasVeterinariansProfessionalLiability
                        Exit Select
                End Select
            End If

            Return BldgCanHaveCoverage
        Catch ex As Exception
            err = ex.Message
            HandleError("BuildingCanHaveOptionalClassCoverage", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Checks to see if any building on the same state as the building on the control has the passed PL coverage
    ''' </summary>
    ''' <param name="CovType"></param>
    ''' <returns></returns>
    Private Function AnyBuildingOnStateHasPLCoverage(ByVal CovType As ClassCoverage_Enum) As Boolean
        Try
            Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(MyLocation.Address.QuickQuoteState)
            If StateQuote IsNot Nothing AndAlso StateQuote.Locations IsNot Nothing Then
                For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In StateQuote.Locations
                    If LOC.Buildings IsNot Nothing Then
                        For Each BLD As QuickQuote.CommonObjects.QuickQuoteBuilding In LOC.Buildings
                            Select Case CovType
                                Case ClassCoverage_Enum.Apartments
                                    If BLD.HasApartmentBuildings Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.Barbers
                                    If BLD.HasBarbersProfessionalLiability Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.Beauticians
                                    If BLD.HasBeauticiansProfessionalLiability Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.FineArts
                                    If BLD.HasFineArts Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.FuneralDirectors
                                    If BLD.HasFuneralDirectorsProfessionalLiability Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.LiquorLiability
                                    If BLD.HasLiquorLiability Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.Motel
                                    If BLD.HasMotelCoverage Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.OpticalAndHearingAid
                                    If BLD.HasOpticalAndHearingAidProfessionalLiability Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.Pharmacist
                                    If BLD.HasPharmacistProfessionalLiability Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.Photography
                                    If BLD.HasPhotographyCoverage Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.Printers
                                    If BLD.HasPrintersProfessionalLiability Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.ResidentialCleaning
                                    If BLD.HasResidentialCleaning Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.Restaurant
                                    If BLD.HasRestaurantEndorsement Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.SelfStorage
                                    If BLD.HasSelfStorageFacility Then Return True
                                    Exit Select
                                Case ClassCoverage_Enum.Veterinarians
                                    If BLD.HasVeterinariansProfessionalLiability Then Return True
                                    Exit Select
                                Case Else
                                    Return False
                            End Select
                        Next
                    End If
                Next
            End If

            Return False
        Catch ex As Exception
            HandleError("AnyBuildingOnStateHasPLCoverage", ex)
            Return False
        End Try
    End Function

    ' Original
    'Private Function AnyBuildingHasPLCoverage(ByVal CovType As ClassCoverage_Enum) As Boolean
    '    Try
    '        If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing Then
    '            For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
    '                If LOC.Buildings IsNot Nothing Then
    '                    For Each BLD As QuickQuote.CommonObjects.QuickQuoteBuilding In LOC.Buildings
    '                        Select Case CovType
    '                            Case ClassCoverage_Enum.Apartments
    '                                If BLD.HasApartmentBuildings Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.Barbers
    '                                If BLD.HasBarbersProfessionalLiability Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.Beauticians
    '                                If BLD.HasBeauticiansProfessionalLiability Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.FineArts
    '                                If BLD.HasFineArts Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.FuneralDirectors
    '                                If BLD.HasFuneralDirectorsProfessionalLiability Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.LiquorLiability
    '                                If BLD.HasLiquorLiability Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.Motel
    '                                If BLD.HasMotelCoverage Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.OpticalAndHearingAid
    '                                If BLD.HasOpticalAndHearingAidProfessionalLiability Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.Pharmacist
    '                                If BLD.HasPharmacistProfessionalLiability Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.Photography
    '                                If BLD.HasPhotographyCoverage Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.Printers
    '                                If BLD.HasPrintersProfessionalLiability Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.ResidentialCleaning
    '                                If BLD.HasResidentialCleaning Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.Restaurant
    '                                If BLD.HasRestaurantEndorsement Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.SelfStorage
    '                                If BLD.HasSelfStorageFacility Then Return True
    '                                Exit Select
    '                            Case ClassCoverage_Enum.Veterinarians
    '                                If BLD.HasVeterinariansProfessionalLiability Then Return True
    '                                Exit Select
    '                            Case Else
    '                                Return False
    '                        End Select
    '                    Next
    '                End If
    '            Next
    '        End If

    '        Return False
    '    Catch ex As Exception
    '        HandleError("AnyBuildingHasPLCoverage", ex)
    '        Return False
    '    End Try
    'End Function

    Private Function GetFirstBuildingOnStateQuote() As QuickQuote.CommonObjects.QuickQuoteBuilding
        Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(MyLocation.Address.QuickQuoteState)
        If StateQuote IsNot Nothing Then
            If StateQuote.Locations IsNot Nothing AndAlso StateQuote.Locations.HasItemAtIndex(0) Then
                If StateQuote.Locations(0).Buildings IsNot Nothing AndAlso StateQuote.Locations(0).Buildings.Count > 0 Then Return StateQuote.Locations(0).Buildings(0)
            End If
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Returns the first building on the quote with the passed professional liability coverage
    ''' </summary>
    ''' <param name="cov"></param>
    ''' <returns></returns>
    Private Function GetFirstBuildingOnStateQuoteWithProfessionalLiabilityCoverage(cov As ClassCoverage_Enum) As QuickQuote.CommonObjects.QuickQuoteBuilding
        Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(MyLocation.Address.QuickQuoteState)
        If StateQuote IsNot Nothing AndAlso StateQuote.Locations IsNot Nothing Then
            For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In StateQuote.Locations
                If LOC.Buildings IsNot Nothing Then
                    For Each BLD As QuickQuote.CommonObjects.QuickQuoteBuilding In LOC.Buildings
                        Select Case cov
                            Case ClassCoverage_Enum.Apartments
                                If BLD.HasApartmentBuildings Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.Barbers
                                If BLD.HasBarbersProfessionalLiability Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.Beauticians
                                If BLD.HasBeauticiansProfessionalLiability Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.FineArts
                                If BLD.HasFineArts Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.FuneralDirectors
                                If BLD.HasFuneralDirectorsProfessionalLiability Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.LiquorLiability
                                If BLD.HasLiquorLiability Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.Motel
                                If BLD.HasMotelCoverage Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.OpticalAndHearingAid
                                If BLD.HasOpticalAndHearingAidProfessionalLiability Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.Pharmacist
                                If BLD.HasPharmacistProfessionalLiability Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.Photography
                                If BLD.HasPhotographyCoverage Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.Photography_MakeupAndHair
                                If BLD.HasPhotographyMakeupAndHair Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.Photography_SheduledEquipment
                                If BLD.HasPhotographyCoverageScheduledCoverages Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.Printers
                                If BLD.HasPrintersProfessionalLiability Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.ResidentialCleaning
                                If BLD.HasResidentialCleaning Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.Restaurant
                                If BLD.HasRestaurantEndorsement Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.SelfStorage
                                If BLD.HasSelfStorageFacility Then Return BLD
                                Exit Select
                            Case ClassCoverage_Enum.Veterinarians
                                If BLD.HasVeterinariansProfessionalLiability Then Return BLD
                                Exit Select
                            Case Else
                                Return Nothing
                        End Select
                    Next
                End If
            Next
        End If
        ' if we got here then the professional liability coverage was not found in any building on the current state - return nothing
        Return Nothing
    End Function


    Public Overrides Function Save() As Boolean
        Dim err As String = Nothing

        Try
            If Me.Quote IsNot Nothing Then
                If Me.MyBuilding IsNot Nothing Then
                    If chkAcctsReceivableONPremises.Checked Then
                        If txtAcctsReceivableOnPremisesTotalLimit.Text.Trim <> "" Then
                            MyBuilding.AccountsReceivableOnPremisesExcessLimit = txtAcctsReceivableOnPremisesTotalLimit.Text
                        Else
                            MyBuilding.AccountsReceivableOnPremisesExcessLimit = "0"
                        End If
                    Else
                        ' Bug 52258 - must set both values to clear coverage  MGB 11/11/2020
                        MyBuilding.AccountsReceivableOnPremisesExcessLimit = ""
                        MyBuilding.AccountsReceivableOnPremises = ""
                    End If

                    If chkValuablePapersOnPremises.Checked Then
                        If txtValuablePapersOnPremisesTotalLimit.Text.Trim <> "" Then
                            MyBuilding.ValuablePapersOnPremises = txtValuablePapersOnPremisesTotalLimit.Text
                        Else
                            MyBuilding.ValuablePapersOnPremises = "0"
                        End If
                    Else
                        MyBuilding.ValuablePapersOnPremises = String.Empty
                    End If

                    If chkOrdinanceOrLaw.Checked Then
                        MyBuilding.HasOrdinanceOrLaw = True
                        MyBuilding.HasOrdOrLawUndamagedPortion = chkOrdinanceOrLawUndamaged.Checked
                        MyBuilding.OrdOrLawDemoCostLimit = txtDemolitionCostLimit.Text
                        MyBuilding.OrdOrLawIncreasedCostLimit = txtIncreasedCostOfConstructionLimit.Text
                        MyBuilding.OrdOrLawDemoAndIncreasedCostLimit = txtDemolitionAndIncreasedCostCombinedLimit.Text
                    Else
                        MyBuilding.HasOrdinanceOrLaw = False
                        MyBuilding.HasOrdOrLawUndamagedPortion = False
                        MyBuilding.OrdOrLawDemoCostLimit = String.Empty
                        MyBuilding.OrdOrLawIncreasedCostLimit = String.Empty
                        MyBuilding.OrdOrLawDemoAndIncreasedCostLimit = String.Empty
                        MyBuilding.OrdOrLawDemoAndIncreasedCostLimitQuotedPremium = String.Empty
                        MyBuilding.OrdOrLawDemoCostLimitQuotedPremium = String.Empty
                        MyBuilding.OrdOrLawIncreaseCostLimitQuotedPremium = String.Empty
                        MyBuilding.OrdOrLawUndamagedPortionQuotedPremium = String.Empty
                    End If

                    If chkSpoilage.Checked Then
                        MyBuilding.HasSpoilage = True
                        MyBuilding.SpoilagePropertyClassificationId = ddlSpoilagePropertyClassification.SelectedValue
                        MyBuilding.SpoilageTotalLimit = txtSpoilageTotalLimit.Text
                        MyBuilding.IsSpoilageRefrigerationMaintenanceAgreement = chkRefrigeratorMaintenanceAgreement.Checked
                        MyBuilding.IsSpoilageBreakdownOrContamination = chkBreakdownOrContamination.Checked
                        MyBuilding.IsSpoilagePowerOutage = chkPowerOutage.Checked
                    Else
                        MyBuilding.HasSpoilage = False
                        MyBuilding.SpoilagePropertyClassificationId = String.Empty
                        MyBuilding.SpoilageTotalLimit = String.Empty
                        MyBuilding.IsSpoilageRefrigerationMaintenanceAgreement = False
                        MyBuilding.IsSpoilageBreakdownOrContamination = False
                        MyBuilding.IsSpoilagePowerOutage = False
                        MyBuilding.SpoilagePropertyClassification = String.Empty
                        MyBuilding.SpoilageQuotedPremium = String.Empty
                    End If

                    ' CLASS BASED OPTIONAL BUILDING COVERAGES
                    ' NOTE THAT ON ALL PROFESSIONAL LIABILITY IF WE CHANGE THE VALUE OF THE COVERAGE STUFF, WE NEED TO CHANGE
                    ' THE VALUES ON ALL OF THE BUILDINGS
                    ' Liquor
                    If chkLiquorLiability.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.LiquorLiability, err) Then
                            MyBuilding.HasLiquorLiability = True

                            If MyLocation?.Address IsNot Nothing Then
                                Select Case MyLocation.Address.QuickQuoteState
                                    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                        If Quote.EffectiveDate >= "4/1/2020" Then
                                            ' We don't send the limit now, Diamond figures it out.  MGB 2020-02-27 Task 40487
                                            MyBuilding.LiquorLiabilityAggregateLimit = ""
                                        Else
                                            MyBuilding.LiquorLiabilityAggregateLimit = lblLiquorLiabilityLimit.Text
                                        End If
                                    Case Else
                                        ' IN 1000000
                                        MyBuilding.LiquorLiabilityAggregateLimit = lblLiquorLiabilityLimit.Text.TryToGetInt32().ToString()
                                End Select
                            Else
                                'default to IN 1000000
                                MyBuilding.LiquorLiabilityAggregateLimit = lblLiquorLiabilityLimit.Text.TryToGetInt32().ToString()
                            End If


                            ' If Restaurant is on the building use the Liquor Sales field, otherwise use the package sales field
                            If BuildingHasOptionalCoverage(MyBuilding, ClassCoverage_Enum.Restaurant) Then
                                MyBuilding.LiquorLiabilityClassCodeTypeId = "12"
                                MyBuilding.LiquorLiabilityAnnualGrossAlcoholSalesReceipts = txtLiquorLiabiltyAnnualGrossAlcoholSales.Text
                                MyBuilding.LiquorLiabilityAnnualGrossPackageSalesReceipts = ""
                            Else
                                MyBuilding.LiquorLiabilityClassCodeTypeId = "13"
                                MyBuilding.LiquorLiabilityAnnualGrossPackageSalesReceipts = txtLiquorLiabiltyAnnualGrossAlcoholSales.Text
                                MyBuilding.LiquorLiabilityAnnualGrossAlcoholSalesReceipts = ""
                            End If
                            ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                            SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.LiquorLiability)
                        End If
                    Else
                        MyBuilding.HasLiquorLiability = False
                        MyBuilding.LiquorLiabilityAggregateLimit = ""
                        MyBuilding.LiquorLiabilityClassCodeTypeId = ""
                        MyBuilding.LiquorLiabilityAnnualGrossAlcoholSalesReceipts = ""
                        MyBuilding.LiquorLiabilityAnnualGrossPackageSalesReceipts = ""

                    End If

                    ' Fine Arts
                    If chkFineArts.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.FineArts, err) Then
                            MyBuilding.HasFineArts = True
                        End If
                    Else
                        MyBuilding.HasFineArts = False
                    End If

                    ' Restaurant
                    If chkRestaurant.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Restaurant, err) Then
                            MyBuilding.HasRestaurantEndorsement = True
                            MyBuilding.HasCustomerAutoLegalLiability = True
                            MyBuilding.CustomerAutoLegalLiabilityLimitOfLiabilityId = ddlRestaurantsLimitOfLiability.SelectedValue
                            MyBuilding.CustomerAutoLegalLiabilityDeductibleId = ddlRestaurantsDeductible.SelectedValue
                            ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                            SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.Restaurant)
                        End If
                    Else
                        MyBuilding.HasRestaurantEndorsement = False
                        MyBuilding.HasCustomerAutoLegalLiability = False
                        MyBuilding.CustomerAutoLegalLiabilityLimitOfLiabilityId = ""
                        MyBuilding.CustomerAutoLegalLiabilityDeductibleId = ""
                    End If

                    ' Pharmacists
                    If chkPharmacists.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Pharmacist, err) Then
                            MyBuilding.HasPharmacistProfessionalLiability = True
                            MyBuilding.PharmacistAnnualGrossSales = txtPharmacistsReceipts.Text
                            ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                            SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.Pharmacist)
                        End If
                    Else
                        MyBuilding.HasPharmacistProfessionalLiability = False
                        MyBuilding.PharmacistAnnualGrossSales = ""
                    End If

                    ' Optical & Hearing Aid
                    If chkOpticalAndHearingAid.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.OpticalAndHearingAid, err) Then
                            MyBuilding.HasOpticalAndHearingAidProfessionalLiability = True
                            MyBuilding.OpticalAndHearingAidProfessionalLiabilityEmpNum = txtOpticalAndHearingNumOfEmployees.Text
                            ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                            SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.OpticalAndHearingAid)
                        End If
                    Else
                        MyBuilding.HasOpticalAndHearingAidProfessionalLiability = False
                        MyBuilding.OpticalAndHearingAidProfessionalLiabilityEmpNum = ""
                    End If

                    ' Barbers
                    If chkBarbersLiability.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Barbers, err) Then
                            MyBuilding.HasBarbersProfessionalLiability = True
                            MyBuilding.BarbersProfessionalLiabilityFullTimeEmpNum = txtBarberFullTimeEmployees.Text
                            MyBuilding.BarbersProfessionalLiabilityPartTimeEmpNum = txtBarberPartTimeEmployees.Text
                            ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                            SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.Barbers)
                        End If
                    Else
                        MyBuilding.HasBarbersProfessionalLiability = False
                        MyBuilding.BarbersProfessionalLiabilityFullTimeEmpNum = String.Empty
                        MyBuilding.BarbersProfessionalLiabilityPartTimeEmpNum = String.Empty
                    End If

                    ' Beauticians
                    If chkBeautyShops.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Beauticians, err) Then
                            MyBuilding.HasBeauticiansProfessionalLiability = True
                            MyBuilding.BeauticiansProfessionalLiabilityFullTimeEmpNum = txtBeautyShopsNumFullTimeEmployees.Text
                            MyBuilding.BeauticiansProfessionalLiabilityPartTimeEmpNum = txtBeautyShopsNumPartTimeEmployees.Text
                            ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                            SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.Beauticians)
                        End If
                    Else
                        MyBuilding.HasBeauticiansProfessionalLiability = False
                        MyBuilding.BeauticiansProfessionalLiabilityFullTimeEmpNum = ""
                        MyBuilding.BeauticiansProfessionalLiabilityPartTimeEmpNum = ""
                    End If

                    ' Printers
                    If chkPrinters.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Printers, err) Then
                            MyBuilding.HasPrintersProfessionalLiability = True
                            MyBuilding.PrintersProfessionalLiabilityLocNum = txtPrintersNumOfLocations.Text
                            ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                            SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.Printers)
                        End If
                    Else
                        MyBuilding.HasPrintersProfessionalLiability = False
                        MyBuilding.PrintersProfessionalLiabilityLocNum = ""
                    End If

                    ' Funeral Directors
                    If chkFuneralDirectors.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.FuneralDirectors, err) Then
                            MyBuilding.HasFuneralDirectorsProfessionalLiability = True
                            MyBuilding.FuneralDirectorsProfessionalLiabilityEmpNum = txtFuneralNumOfEmployees.Text
                            ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                            SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.FuneralDirectors)
                        End If
                    Else
                        MyBuilding.HasFuneralDirectorsProfessionalLiability = False
                        MyBuilding.FuneralDirectorsProfessionalLiabilityEmpNum = ""
                    End If

                    ' Photography (NEW)
                    If chkPhotography.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Photography, err) Then
                            MyBuilding.HasPhotographyCoverage = True
                            If chkPhotographyScheduledEquipment.Checked Then
                                MyBuilding.HasPhotographyCoverageScheduledCoverages = True
                                ' Create one item and add it to the scheduled equipment list
                                Dim NewSchedEquip As New QuickQuote.CommonObjects.QuickQuoteCoverage
                                NewSchedEquip.Description = "Total Limit"
                                NewSchedEquip.ManualLimitAmount = txtPhotogScheduledEquipmentLimit.Text
                                MyBuilding.PhotographyScheduledCoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteCoverage)
                                MyBuilding.PhotographyScheduledCoverages.Add(NewSchedEquip)
                            Else
                                MyBuilding.HasPhotographyCoverageScheduledCoverages = False
                                MyBuilding.PhotographyScheduledCoverages = Nothing
                            End If
                            ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                            SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.Photography)
                        End If

                        If chkPhotogMakeupAndHair.Checked Then
                            If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Photography_MakeupAndHair, err) Then
                                MyBuilding.HasPhotographyMakeupAndHair = True
                                ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                                SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.Photography_MakeupAndHair)
                            End If
                        Else
                            MyBuilding.HasPhotographyMakeupAndHair = False
                        End If
                    Else
                        MyBuilding.HasPhotographyCoverage = False
                        MyBuilding.HasPhotographyCoverageScheduledCoverages = False
                        MyBuilding.PhotographyScheduledCoverages = Nothing
                        MyBuilding.HasPhotographyMakeupAndHair = False
                        MyBuilding.PhotographyScheduledCoverages = Nothing
                    End If

                    ' Self Storage
                    If chkSelfStorage.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.SelfStorage, err) Then
                            MyBuilding.HasSelfStorageFacility = True
                            If txtStorageLimit.Text.Trim <> "" AndAlso IsNumeric(txtStorageLimit.Text) Then
                                MyBuilding.SelfStorageFacilityLimit = 50000 + CDec(txtStorageLimit.Text)
                            Else
                                MyBuilding.SelfStorageFacilityLimit = ""
                            End If
                            ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                            SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.SelfStorage)
                        End If
                    Else
                        MyBuilding.HasSelfStorageFacility = False
                        MyBuilding.SelfStorageFacilityLimit = ""
                    End If

                    ' Veterinarians
                    If chkVeterinarians.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Veterinarians, err) Then
                            MyBuilding.HasVeterinariansProfessionalLiability = True
                            MyBuilding.VeterinariansProfessionalLiabilityEmpNum = txtVeterinariansNumOfEmployees.Text
                            ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                            SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.Veterinarians)
                        End If
                    Else
                        MyBuilding.HasVeterinariansProfessionalLiability = False
                        MyBuilding.VeterinariansProfessionalLiabilityEmpNum = ""
                    End If

                    ' Motel
                    If chkMotel.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Motel, err) Then
                            MyBuilding.HasMotelCoverage = True
                            MyBuilding.MotelCoveragePerGuestLimitId = ddlMotelLiabilityLimit.SelectedValue
                            MyBuilding.MotelCoverageSafeDepositLimitId = ddlMotelSafeDepositBoxLimit.SelectedValue
                            MyBuilding.MotelCoverageSafeDepositDeductibleId = ddlMotelSafeDepositBoxDeductible.SelectedValue
                            ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                            SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.Motel)
                        End If
                    Else
                        MyBuilding.HasMotelCoverage = False
                        MyBuilding.MotelCoveragePerGuestLimitId = ""
                        MyBuilding.MotelCoveragePerGuestLimit = ""
                        MyBuilding.MotelCoverageSafeDepositLimitId = ""
                        MyBuilding.MotelCoverageSafeDepositLimit = ""
                        MyBuilding.MotelCoverageSafeDepositDeductibleId = ""
                        MyBuilding.MotelCoverageSafeDepositDeductible = ""
                    End If

                    ' Apartments
                    If chkApartments.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Apartments, err) Then
                            MyBuilding.HasApartmentBuildings = True
                            MyBuilding.NumberOfLocationsWithApartments = txtAptsNumberOfLocationsWithApts.Text
                            ' The limit of liability ddl is what determines whether or not the building has Tenant Auto Legal Liability
                            If ddlAptAutoLiabilityLimit.SelectedIndex > 0 Then MyBuilding.HasTenantAutoLegalLiability = True Else MyBuilding.HasTenantAutoLegalLiability = False
                            If MyBuilding.HasTenantAutoLegalLiability Then
                                If ddlAptAutoLiabilityLimit.SelectedIndex > 0 Then
                                    MyBuilding.TenantAutoLegalLiabilityLimitOfLiabilityId = ddlAptAutoLiabilityLimit.SelectedValue
                                    MyBuilding.TenantAutoLegalLiabilityLimitOfLiability = ddlAptAutoLiabilityLimit.SelectedItem.Text
                                Else
                                    MyBuilding.TenantAutoLegalLiabilityLimitOfLiabilityId = ""
                                    MyBuilding.TenantAutoLegalLiabilityLimitOfLiability = ""
                                End If
                                If ddlAptDeductible.SelectedIndex > 0 Then
                                    MyBuilding.TenantAutoLegalLiabilityDeductibleId = ddlAptDeductible.SelectedValue
                                    MyBuilding.TenantAutoLegalLiabilityDeductible = ddlAptDeductible.SelectedItem.Text
                                Else
                                    MyBuilding.TenantAutoLegalLiabilityDeductibleId = ""
                                    MyBuilding.TenantAutoLegalLiabilityDeductible = ""
                                End If
                            End If
                            ' SAVE THE COVERAGE INFO TO ALL BUILDINGS ON THE POLICY THAT CAN HAVE THE COVERAGE
                            SetProfessionalLiabilityValuesOnBuildings(ClassCoverage_Enum.Apartments)
                        End If
                    Else
                        MyBuilding.HasApartmentBuildings = False
                        MyBuilding.HasTenantAutoLegalLiability = False
                        MyBuilding.NumberOfLocationsWithApartments = ""
                        MyBuilding.TenantAutoLegalLiabilityLimitOfLiabilityId = ""
                        MyBuilding.TenantAutoLegalLiabilityLimitOfLiability = ""
                        MyBuilding.TenantAutoLegalLiabilityDeductibleId = ""
                        MyBuilding.TenantAutoLegalLiabilityDeductible = ""
                    End If

                    ' Residential Cleaning
                    If chkResidentialCleaning.Checked Then
                        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.ResidentialCleaning, err) Then
                            MyBuilding.HasResidentialCleaning = True
                        End If
                    Else
                        MyBuilding.HasResidentialCleaning = False
                    End If

                    Quote.CopyProfessionalLiabilityCoveragesFromBuildingsToPolicy_UseBuildingClassificationList()
                End If

            End If

            Populate()  ' Watch the values each time this gets hit
            Me.SaveChildControls()

            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Checks to see if the current building is the first building on it's state quote
    ''' </summary>
    ''' <param name="CovType"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    Private Function BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ByVal CovType As ClassCoverage_Enum, ByRef err As String) As Boolean
        Try
            ' This function was rewritten for Multistate MGB 11/27/18
            Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(MyLocation.Address.QuickQuoteState)
            If StateQuote IsNot Nothing AndAlso StateQuote.Locations IsNot Nothing AndAlso StateQuote.Locations.Count > 0 Then
                ' Loop thru all the locations on the state quote
                For LocIndex As Integer = 0 To StateQuote.Locations.Count - 1
                    Dim loc As QuickQuote.CommonObjects.QuickQuoteLocation = StateQuote.Locations(LocIndex)
                    If loc.Buildings IsNot Nothing Then
                        ' Loop thru all the buildings in the locations
                        For BldIndex As Integer = 0 To loc.Buildings.Count - 1
                            Dim bldg As QuickQuote.CommonObjects.QuickQuoteBuilding = loc.Buildings(BldIndex)
                            If BuildingHasOptionalCoverage(bldg, CovType) Then
                                If LocIndex = StateLocationIndex AndAlso BldIndex = BuildingIndex Then
                                    Return True
                                Else
                                    Return False
                                End If
                            End If
                        Next
                    End If
                Next
            End If
            ' If we got here then none of the buildings had the coverage so this is the first one
            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError("BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage", ex)
            Return False
        End Try
    End Function

    '(pre-multistate)
    'Private Function BuildingIsFirstWithProfessionalLiabilityCoverage(ByVal CovType As ClassCoverage_Enum, ByRef err As String) As Boolean
    '    Try

    '        'find all buildings in state part
    '        '


    '        ' Need to check all of the buildings on the policy and to see if this one is the first with the passed professional liability
    '        If Quote IsNot Nothing Then
    '            If Quote.Locations IsNot Nothing Then
    '                ' Loop thru all the locations on the quote
    '                For LocIndex As Integer = 0 To Quote.Locations.Count - 1
    '                    Dim loc As QuickQuote.CommonObjects.QuickQuoteLocation = Quote.Locations(LocIndex)
    '                    If loc.Buildings IsNot Nothing Then ' loc.Address.QuickQuoteState = Me.MyLocation.Address.QuickQuoteState AndAlso
    '                        ' Loop thru all the buildings in the locations
    '                        For BldIndex As Integer = 0 To loc.Buildings.Count - 1
    '                            Dim bldg As QuickQuote.CommonObjects.QuickQuoteBuilding = loc.Buildings(BldIndex)
    '                            If BuildingHasOptionalCoverage(bldg, CovType) Then
    '                                If LocIndex = LocationIndex AndAlso BldIndex = BuildingIndex Then
    '                                    Return True
    '                                Else
    '                                    Return False
    '                                End If
    '                            End If
    '                        Next
    '                    End If
    '                Next
    '            End If
    '        End If

    '        ' If we got here then none of the buildings had the coverage so this is the first one
    '        Return True
    '    Catch ex As Exception
    '        err = ex.Message
    '        HandleError("BuildingIsFirstWithProfessionalLiabilityCoverage", ex)
    '        Return False
    '    End Try
    'End Function

    ''' <summary>
    ''' This function will set the values on 2nd and subsequent buildings with a specific Optional Class-Based Building Coverage
    ''' You only want to call this guy on the first building with the coverage...
    ''' </summary>
    ''' <param name="CovType"></param>
    Private Sub SetProfessionalLiabilityValuesOnBuildings(ByVal CovType As ClassCoverage_Enum)
        Dim ndx As Integer = 0
        Dim hascov As Boolean = False
        Dim err As String = Nothing
        Dim Lndx As Integer = 0
        Dim Bndx As Integer = 0

        Try
            Lndx = -1
            Bndx = -1

            Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(MyLocation.Address.QuickQuoteState)

            If StateQuote IsNot Nothing AndAlso StateQuote.Locations IsNot Nothing Then
                For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In StateQuote.Locations
                    Lndx += 1
                    If LOC.Buildings IsNot Nothing Then
                        For Each BLD As QuickQuote.CommonObjects.QuickQuoteBuilding In LOC.Buildings
                            Bndx += 1
                            If Not (Lndx = 0 AndAlso Bndx = 0) Then  ' Do not update the first bulding on each state - we did that in populate
                                'If Not (Lndx = LocationIndex And Bndx = BuildingIndex) Then
                                Select Case CovType
                                    Case ClassCoverage_Enum.Apartments
                                        ' APARTMENTS
                                        BLD.HasApartmentBuildings = True
                                        BLD.NumberOfLocationsWithApartments = txtAptsNumberOfLocationsWithApts.Text
                                        ' The limit of liability ddl is what determines whether or not the building has Tenant Auto Legal Liability
                                        If ddlAptAutoLiabilityLimit.SelectedIndex > 0 Then BLD.HasTenantAutoLegalLiability = True Else BLD.HasTenantAutoLegalLiability = False
                                        If BLD.HasTenantAutoLegalLiability Then
                                            If ddlAptAutoLiabilityLimit.SelectedIndex > 0 Then
                                                BLD.TenantAutoLegalLiabilityLimitOfLiabilityId = ddlAptAutoLiabilityLimit.SelectedValue
                                            Else
                                                BLD.TenantAutoLegalLiabilityLimitOfLiabilityId = ""
                                            End If
                                            If ddlAptDeductible.SelectedIndex > 0 Then
                                                BLD.TenantAutoLegalLiabilityDeductibleId = ddlAptDeductible.SelectedValue
                                            Else
                                                BLD.TenantAutoLegalLiabilityDeductibleId = ""
                                            End If
                                        End If
                                        Exit Select
                                    Case ClassCoverage_Enum.Barbers
                                        ' BARBERS
                                        BLD.HasBarbersProfessionalLiability = True
                                        BLD.BarbersProfessionalLiabilityFullTimeEmpNum = txtBarberFullTimeEmployees.Text
                                        BLD.BarbersProfessionalLiabilityPartTimeEmpNum = txtBarberPartTimeEmployees.Text
                                        Exit Select
                                    Case ClassCoverage_Enum.Beauticians
                                        ' BEAUTICIANS
                                        BLD.HasBeauticiansProfessionalLiability = True
                                        BLD.BeauticiansProfessionalLiabilityFullTimeEmpNum = txtBeautyShopsNumFullTimeEmployees.Text
                                        BLD.BeauticiansProfessionalLiabilityPartTimeEmpNum = txtBeautyShopsNumPartTimeEmployees.Text
                                        Exit Select
                                    Case ClassCoverage_Enum.FineArts
                                        ' FINE ARTS
                                        BLD.HasFineArts = True
                                        Exit Select
                                    Case ClassCoverage_Enum.FuneralDirectors
                                        ' FUNERAL DIRECTORS
                                        BLD.HasFuneralDirectorsProfessionalLiability = True
                                        BLD.FuneralDirectorsProfessionalLiabilityEmpNum = txtFuneralNumOfEmployees.Text
                                        Exit Select
                                    Case ClassCoverage_Enum.LiquorLiability
                                        ' LIQUOR LIABILITY
                                        ' Get the first building on the state quote - we will use this to populat the rest of the buildings
                                        Dim FirstBuildingOnStateQuote As QuickQuote.CommonObjects.QuickQuoteBuilding = GetFirstBuildingOnStateQuote()
                                        If FirstBuildingOnStateQuote IsNot Nothing Then
                                            Dim sales As String = ""

                                            ' HasLiquor switch
                                            BLD.HasLiquorLiability = True

                                            ' Liability Limit
                                            BLD.LiquorLiabilityAggregateLimit = FirstBuildingOnStateQuote.LiquorLiabilityAggregateLimit

                                            ' Sales
                                            ' Only one of these should be set
                                            If FirstBuildingOnStateQuote.LiquorLiabilityAnnualGrossAlcoholSalesReceipts <> "" Then sales = FirstBuildingOnStateQuote.LiquorLiabilityAnnualGrossAlcoholSalesReceipts
                                            If FirstBuildingOnStateQuote.LiquorLiabilityAnnualGrossPackageSalesReceipts <> "" Then sales = FirstBuildingOnStateQuote.LiquorLiabilityAnnualGrossPackageSalesReceipts

                                            ' If Restaurant is on the building use the Liquor Sales field, otherwise use the package sales field
                                            If BuildingHasOptionalCoverage(BLD, ClassCoverage_Enum.Restaurant) Then
                                                BLD.LiquorLiabilityClassCodeTypeId = "12"
                                                BLD.LiquorLiabilityAnnualGrossAlcoholSalesReceipts = sales
                                                BLD.LiquorLiabilityAnnualGrossPackageSalesReceipts = ""
                                            Else
                                                BLD.LiquorLiabilityClassCodeTypeId = "13"
                                                BLD.LiquorLiabilityAnnualGrossPackageSalesReceipts = sales
                                                BLD.LiquorLiabilityAnnualGrossAlcoholSalesReceipts = ""
                                            End If
                                        End If

                                        '' only set other buildings on the same state
                                        ''Me.SubQuotesHasAnyState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                                        'If MyLocation?.Address IsNot Nothing AndAlso LOC?.Address IsNot Nothing Then
                                        '    If MyLocation.Address.QuickQuoteState = LOC.Address.QuickQuoteState Then
                                        '        BLD.HasLiquorLiability = True
                                        '        Select Case LOC.Address.QuickQuoteState
                                        '            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                        '                BLD.LiquorLiabilityAggregateLimit = lblLiquorLiabilityLimit.Text
                                        '            Case Else
                                        '                ' IN 1000000
                                        '                BLD.LiquorLiabilityAggregateLimit = lblLiquorLiabilityLimit.Text.TryToGetInt32().ToString()
                                        '        End Select
                                        '        ' If Restaurant is on the building use the Liquor Sales field, otherwise use the package sales field
                                        '        If BuildingHasOptionalCoverage(BLD, ClassCoverage_Enum.Restaurant) Then
                                        '            BLD.LiquorLiabilityClassCodeTypeId = "12"
                                        '            BLD.LiquorLiabilityAnnualGrossAlcoholSalesReceipts = txtLiquorLiabiltyAnnualGrossAlcoholSales.Text
                                        '            BLD.LiquorLiabilityAnnualGrossPackageSalesReceipts = ""
                                        '        Else
                                        '            BLD.LiquorLiabilityClassCodeTypeId = "13"
                                        '            BLD.LiquorLiabilityAnnualGrossPackageSalesReceipts = txtLiquorLiabiltyAnnualGrossAlcoholSales.Text
                                        '            BLD.LiquorLiabilityAnnualGrossAlcoholSalesReceipts = ""
                                        '        End If
                                        '    End If
                                        'End If
                                        Exit Select
                                    Case ClassCoverage_Enum.Motel
                                        ' MOTEL
                                        BLD.HasMotelCoverage = True
                                        BLD.MotelCoveragePerGuestLimitId = ddlMotelLiabilityLimit.SelectedValue
                                        BLD.MotelCoverageSafeDepositLimitId = ddlMotelSafeDepositBoxLimit.SelectedValue
                                        BLD.MotelCoverageSafeDepositDeductibleId = ddlMotelSafeDepositBoxDeductible.SelectedValue
                                        Exit Select
                                    Case ClassCoverage_Enum.OpticalAndHearingAid
                                        ' OPTICAL & HEARING AID
                                        BLD.HasOpticalAndHearingAidProfessionalLiability = True
                                        BLD.OpticalAndHearingAidProfessionalLiabilityEmpNum = txtOpticalAndHearingNumOfEmployees.Text
                                        Exit Select
                                    Case ClassCoverage_Enum.Pharmacist
                                        ' PHARMACISTS
                                        BLD.HasPharmacistProfessionalLiability = True
                                        BLD.PharmacistAnnualGrossSales = txtPharmacistsReceipts.Text
                                        Exit Select
                                    Case ClassCoverage_Enum.Pharmacist
                                        ' PRINTERS
                                        BLD.HasPrintersProfessionalLiability = True
                                        BLD.PrintersProfessionalLiabilityLocNum = txtPrintersNumOfLocations.Text
                                        Exit Select
                                    Case ClassCoverage_Enum.ResidentialCleaning
                                        ' RESIDENTIAL CLEANING
                                        BLD.HasResidentialCleaning = True
                                        Exit Select
                                    Case ClassCoverage_Enum.Restaurant
                                        ' RESTAURANT
                                        BLD.HasRestaurantEndorsement = True
                                        BLD.HasCustomerAutoLegalLiability = True
                                        BLD.CustomerAutoLegalLiabilityLimitOfLiabilityId = ddlRestaurantsLimitOfLiability.SelectedValue
                                        BLD.CustomerAutoLegalLiabilityDeductibleId = ddlRestaurantsDeductible.SelectedValue
                                        Exit Select
                                    Case ClassCoverage_Enum.SelfStorage
                                        ' SELF-STORAGE
                                        BLD.HasSelfStorageFacility = True
                                        BLD.SelfStorageFacilityLimit = txtStorageLimit.Text
                                        Exit Select
                                    Case ClassCoverage_Enum.Veterinarians
                                        ' VETERINARIANS
                                        BLD.HasVeterinariansProfessionalLiability = True
                                        BLD.VeterinariansProfessionalLiabilityEmpNum = txtVeterinariansNumOfEmployees.Text
                                        Exit Select
                                    Case ClassCoverage_Enum.Photography
                                        ' PHOTOGRAPHY (NEW)
                                        BLD.HasPhotographyCoverage = True
                                        If chkPhotographyScheduledEquipment.Checked Then
                                            BLD.HasPhotographyCoverageScheduledCoverages = True
                                            ' Create one item and add it to the scheduled equipment list
                                            Dim NewSchedEquip As New QuickQuote.CommonObjects.QuickQuoteCoverage
                                            NewSchedEquip.Description = "Total Limit"
                                            NewSchedEquip.ManualLimitAmount = txtPhotogScheduledEquipmentLimit.Text
                                            BLD.PhotographyScheduledCoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteCoverage)
                                            BLD.PhotographyScheduledCoverages.Add(NewSchedEquip)
                                        End If
                                        Exit Select
                                    Case ClassCoverage_Enum.Photography_MakeupAndHair
                                        ' PHOTOGRAPHY - MAKEUP AND HAIR
                                        BLD.HasPhotographyMakeupAndHair = True
                                        Exit Select
                                    Case ClassCoverage_Enum.Photography_SheduledEquipment
                                        ' PHOTOGRAPHY - SCHEDULED EQUIPMENT
                                        BLD.HasPhotographyCoverageScheduledCoverages = True
                                        BLD.PhotographyScheduledCoverages = MyBuilding.PhotographyScheduledCoverages
                                        Exit Select
                                End Select
                            End If  '***
                        Next
                    End If
                Next
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("SetProfessionalLiabilityValuesOnBuildings", ex)
        End Try
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Dim Undamaged_1 As Boolean = False
        Dim Demolition_2 As Boolean = False
        Dim IncreasedCost_3 As Boolean = False
        Dim err As String = Nothing

        Try
            MyBase.ValidateControl(valArgs)
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
            ValidationHelper.GroupName = "Location #" & LocationIndex + 1 & ", Building #" & BuildingIndex + 1 & " Optional Building Coverages"

            If chkAcctsReceivableONPremises.Checked Then
                If txtAcctsReceivableOnPremisesTotalLimit.Text = String.Empty Then
                    Me.ValidationHelper.AddError(txtAcctsReceivableOnPremisesTotalLimit, "Missing Total Limit", accordList)
                Else
                    If Not IsNumeric(txtAcctsReceivableOnPremisesTotalLimit.Text) Then
                        Me.ValidationHelper.AddError(txtAcctsReceivableOnPremisesTotalLimit, "Total Limit is invalid", accordList)
                    ElseIf CDec(txtAcctsReceivableOnPremisesTotalLimit.Text) <= 50000 Then
                        Me.ValidationHelper.AddError(txtAcctsReceivableOnPremisesTotalLimit, "Total Limit must be greater than $50,000", accordList)
                    End If
                End If
            End If

            If chkValuablePapersOnPremises.Checked Then
                If txtValuablePapersOnPremisesTotalLimit.Text = String.Empty Then
                    Me.ValidationHelper.AddError(txtValuablePapersOnPremisesTotalLimit, "Missing Total Limit", accordList)
                Else
                    If Not IsNumeric(txtValuablePapersOnPremisesTotalLimit.Text) Then
                        Me.ValidationHelper.AddError(txtValuablePapersOnPremisesTotalLimit, "Total Limit is invalid", accordList)
                    ElseIf CDec(txtValuablePapersOnPremisesTotalLimit.Text) <= 25000 Then
                        Me.ValidationHelper.AddError(txtValuablePapersOnPremisesTotalLimit, "Total Limit must be greater than $25,000", accordList)
                    End If
                End If
            End If

            If chkOrdinanceOrLaw.Checked Then
                ' 1 Loss to the undamaged portion of the building
                ' 2 Demolition Cost Limit OR Increased Cost of Construction Limit (but not both)
                ' 3 Increased Cost of Construction Limit (but not both)

                ' First off, either Demolition Cost & Increased Cost of Construction OR Demolition & Increased Cost Combined MUST have value(s)
                If (txtDemolitionCostLimit.Text.Trim = "" And txtIncreasedCostOfConstructionLimit.Text.Trim = "") And txtDemolitionAndIncreasedCostCombinedLimit.Text.Trim = "" Then
                    Me.ValidationHelper.AddError("When Ordinance or Law coverage is selected you must enter amounts for 1) Demolition Cost & Increased Cost of Construction or 2) Demolition and Increased Cost Combined.", Nothing)
                End If

                Undamaged_1 = chkOrdinanceOrLawUndamaged.Checked
                If txtDemolitionCostLimit.Text <> String.Empty AndAlso IsNumeric(txtDemolitionCostLimit.Text) Then Demolition_2 = True
                If txtIncreasedCostOfConstructionLimit.Text <> String.Empty AndAlso IsNumeric(txtIncreasedCostOfConstructionLimit.Text) Then IncreasedCost_3 = True

                ' 1 Only - OK  Undamaged Only
                ' 3 Only - OK  Increased Cost Only
                ' 1+2 - OK  Undamaged + Demolition
                ' 1+2+3 - OK  Undamaged + Demolition + Increased Cost

                ' 2 Only - NO
                If Demolition_2 And (Not Undamaged_1) And (Not IncreasedCost_3) Then
                    Me.ValidationHelper.AddError("Ordinance or Law Demolition Cost (Coverage 2) cannot be added without Undamaged Portion of the Building (Coverage 1)", Nothing)
                End If

                ' 1+3 - NO
                If Undamaged_1 And (Not Demolition_2) And IncreasedCost_3 Then
                    Me.ValidationHelper.AddError("Ordinance or Law Undamaged Portion of the Building (Coverage 1) cannot be combined with Increased Cost of Construction (Coverage 3)", Nothing)
                End If

                ' 2+3 - NO
                If (Not Undamaged_1) And Demolition_2 And IncreasedCost_3 Then
                    Me.ValidationHelper.AddError("Ordinance or Law Demolition Cost (Coverage 2) and Increased Cost of Construction (Coverage 3) cannot be added without Undamaged Portion of the Building (Coverage 1)", Nothing)
                End If
            End If

            If chkSpoilage.Checked Then
                If ddlSpoilagePropertyClassification.SelectedIndex < 0 Then
                    Me.ValidationHelper.AddError(ddlSpoilagePropertyClassification, "Missing Property Classification", accordList)
                End If
                If txtSpoilageTotalLimit.Text = String.Empty Then
                    Me.ValidationHelper.AddError(txtSpoilageTotalLimit, "Missing Total Limit", accordList)
                Else
                    If Not IsNumeric(txtSpoilageTotalLimit.Text) Then
                        Me.ValidationHelper.AddError(txtSpoilageTotalLimit, "Total Limit is invalid", accordList)
                    ElseIf CDec(txtSpoilageTotalLimit.Text) <= 10000 Then
                        Me.ValidationHelper.AddError(txtSpoilageTotalLimit, "Total Limit must be greater than 10000", accordList)
                    End If
                End If
            End If

            ' CLASS BASED OPTIONAL BUILDING COVERAGES
            ' Liquor Liability
            If chkLiquorLiability.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.LiquorLiability, err) Then
                If chkLiquorLiability.Checked Then
                    If txtLiquorLiabiltyAnnualGrossAlcoholSales.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError(txtLiquorLiabiltyAnnualGrossAlcoholSales, "Missing Annual Gross Alcohol Sales", accordList)
                    Else
                        If Not IsNumeric(txtLiquorLiabiltyAnnualGrossAlcoholSales.Text) OrElse CDec(txtLiquorLiabiltyAnnualGrossAlcoholSales.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtLiquorLiabiltyAnnualGrossAlcoholSales, "Invalid Value", accordList)
                        End If
                    End If
                End If
            End If

            ' Fine Arts

            ' Restaurant
            If chkRestaurant.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Restaurant, err) Then
                If chkRestaurant.Checked Then
                    ' If a limit is selected the deductible is required
                    If ddlRestaurantsLimitOfLiability.SelectedIndex > 0 Then
                        If ddlRestaurantsDeductible.SelectedIndex <= 0 Then
                            Me.ValidationHelper.AddError(ddlRestaurantsDeductible, "Missing Deductible", accordList)
                        End If
                    End If
                End If
            End If

            ' Pharmacists
            If chkPharmacists.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Pharmacist, err) Then
                If chkPharmacists.Checked Then
                    If txtPharmacistsReceipts.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError(txtPharmacistsReceipts, "Missing Receipts", accordList)
                    Else
                        If Not IsNumeric(txtPharmacistsReceipts.Text) OrElse CDec(txtPharmacistsReceipts.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtPharmacistsReceipts, "Invalid Value", accordList)
                        End If
                    End If
                End If
            End If

            ' Optical and Hearing Aid
            If chkOpticalAndHearingAid.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.OpticalAndHearingAid, err) Then
                If chkOpticalAndHearingAid.Checked Then
                    If txtOpticalAndHearingNumOfEmployees.Text.Trim = "" Then
                        Me.ValidationHelper.AddError(txtOpticalAndHearingNumOfEmployees, "Missing Number of Employees", accordList)
                    Else
                        If Not IsNumeric(txtOpticalAndHearingNumOfEmployees.Text) OrElse CDec(txtOpticalAndHearingNumOfEmployees.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtOpticalAndHearingNumOfEmployees, "Invalid Value", accordList)
                        End If
                    End If
                End If
            End If

            ' Barbers
            If chkBarbersLiability.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Barbers, err) Then
                If chkBarbersLiability.Checked Then
                    If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Barbers, err) Then
                        If txtBarberFullTimeEmployees.Text = String.Empty Then
                            Me.ValidationHelper.AddError(txtBarberFullTimeEmployees, "Missing Number of Full Time Employees", accordList)
                        Else
                            If Not IsNumeric(txtBarberFullTimeEmployees.Text) OrElse CDec(txtBarberFullTimeEmployees.Text) < 0 Then
                                Me.ValidationHelper.AddError(txtBarberFullTimeEmployees, "Invalid Value", accordList)
                            End If
                            If CInt(txtBarberFullTimeEmployees.Text) = 0 Then
                                If IsNumeric(txtBarberPartTimeEmployees.Text) AndAlso CInt(txtBarberPartTimeEmployees.Text) = 0 Then
                                    Me.ValidationHelper.AddError(txtBarberFullTimeEmployees, "You must have at least one employee", accordList)
                                    'Me.ValidationHelper.AddError(txtBarberPartTimeEmployees, "You must have at least one employee", accordList)
                                End If
                            End If
                        End If

                        If txtBarberPartTimeEmployees.Text = String.Empty Then
                            Me.ValidationHelper.AddError(txtBarberPartTimeEmployees, "Missing Number of Part Time Employees", accordList)
                        Else
                            If Not IsNumeric(txtBarberPartTimeEmployees.Text) OrElse CDec(txtBarberPartTimeEmployees.Text) < 0 Then
                                Me.ValidationHelper.AddError(txtBarberPartTimeEmployees, "Invalid Value", accordList)
                            End If
                            If CInt(txtBarberPartTimeEmployees.Text) = 0 Then
                                If IsNumeric(txtBarberFullTimeEmployees.Text) AndAlso CInt(txtBarberFullTimeEmployees.Text) = 0 Then
                                    'Me.ValidationHelper.AddError(txtBarberFullTimeEmployees, "You must have at least one employee", accordList)
                                    Me.ValidationHelper.AddError(txtBarberPartTimeEmployees, "You must have at least one employee", accordList)
                                End If
                            End If
                        End If
                    End If
                End If
            End If
            'If chkBarbersLiability.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Barbers, err) Then
            '    If chkBarbersLiability.Checked Then
            '        If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Barbers, err) Then
            '            If txtBarberFullTimeEmployees.Text = String.Empty Then
            '                Me.ValidationHelper.AddError(txtBarberFullTimeEmployees, "Missing Number of Full Time Employees", accordList)
            '            Else
            '                If Not IsNumeric(txtBarberFullTimeEmployees.Text) OrElse CDec(txtBarberFullTimeEmployees.Text) < 0 Then
            '                    Me.ValidationHelper.AddError(txtBarberFullTimeEmployees, "Invalid Value", accordList)
            '                End If
            '            End If
            '            If txtBarberPartTimeEmployees.Text = String.Empty Then
            '                Me.ValidationHelper.AddError(txtBarberPartTimeEmployees, "Missing Number of Part Time Employees", accordList)
            '            Else
            '                If Not IsNumeric(txtBarberPartTimeEmployees.Text) OrElse CDec(txtBarberPartTimeEmployees.Text) < 0 Then
            '                    Me.ValidationHelper.AddError(txtBarberPartTimeEmployees, "Invalid Value", accordList)
            '                End If
            '            End If
            '        End If
            '    End If
            'End If

            ' Beauticians
            If chkBeautyShops.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Beauticians, err) Then
                If chkBeautyShops.Checked Then
                    If txtBeautyShopsNumFullTimeEmployees.Text = String.Empty Then
                        Me.ValidationHelper.AddError(txtBeautyShopsNumFullTimeEmployees, "Missing Number of Full Time Employees", accordList)
                    Else
                        If Not IsNumeric(txtBeautyShopsNumFullTimeEmployees.Text) OrElse CDec(txtBeautyShopsNumFullTimeEmployees.Text) < 0 Then
                            Me.ValidationHelper.AddError(txtBeautyShopsNumFullTimeEmployees, "Invalid Value", accordList)
                        End If
                        If CInt(txtBeautyShopsNumFullTimeEmployees.Text) = 0 Then
                            If IsNumeric(txtBeautyShopsNumPartTimeEmployees.Text) AndAlso CInt(txtBeautyShopsNumPartTimeEmployees.Text) = 0 Then
                                Me.ValidationHelper.AddError(txtBeautyShopsNumFullTimeEmployees, "You must have at least one employee", accordList)
                                'Me.ValidationHelper.AddError(txtBeautyShopsNumPartTimeEmployees, "You must have at least one employee", accordList)
                            End If
                        End If
                    End If
                    If txtBeautyShopsNumPartTimeEmployees.Text = String.Empty Then
                        Me.ValidationHelper.AddError(txtBeautyShopsNumPartTimeEmployees, "Missing Number of Part Time Employees", accordList)
                    Else
                        If Not IsNumeric(txtBeautyShopsNumPartTimeEmployees.Text) OrElse CDec(txtBeautyShopsNumPartTimeEmployees.Text) < 0 Then
                            Me.ValidationHelper.AddError(txtBeautyShopsNumPartTimeEmployees, "Invalid Value", accordList)
                        End If
                        If CInt(txtBeautyShopsNumPartTimeEmployees.Text) = 0 Then
                            If IsNumeric(txtBeautyShopsNumFullTimeEmployees.Text) AndAlso CInt(txtBeautyShopsNumFullTimeEmployees.Text) = 0 Then
                                'Me.ValidationHelper.AddError(txtBeautyShopsNumFullTimeEmployees, "You must have at least one employee", accordList)
                                Me.ValidationHelper.AddError(txtBeautyShopsNumPartTimeEmployees, "You must have at least one employee", accordList)
                            End If
                        End If
                    End If
                End If
            End If
            'If chkBeautyShops.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Beauticians, err) Then
            '    If chkBeautyShops.Checked Then
            '        If txtBeautyShopsNumFullTimeEmployees.Text = String.Empty Then
            '            Me.ValidationHelper.AddError(txtBeautyShopsNumFullTimeEmployees, "Missing Number of Full Time Employees", accordList)
            '        Else
            '            If Not IsNumeric(txtBeautyShopsNumFullTimeEmployees.Text) OrElse CDec(txtBeautyShopsNumFullTimeEmployees.Text) <= 0 Then
            '                Me.ValidationHelper.AddError(txtBeautyShopsNumFullTimeEmployees, "Invalid Value", accordList)
            '            End If
            '        End If
            '        If txtBeautyShopsNumPartTimeEmployees.Text = String.Empty Then
            '            Me.ValidationHelper.AddError(txtBeautyShopsNumPartTimeEmployees, "Missing Number of Part Time Employees", accordList)
            '        Else
            '            If Not IsNumeric(txtBeautyShopsNumPartTimeEmployees.Text) OrElse CDec(txtBeautyShopsNumPartTimeEmployees.Text) <= 0 Then
            '                Me.ValidationHelper.AddError(txtBeautyShopsNumPartTimeEmployees, "Invalid Value", accordList)
            '            End If
            '        End If
            '    End If
            'End If

            ' Printers
            If chkPrinters.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Printers, err) Then
                If chkPrinters.Checked Then
                    If txtPrintersNumOfLocations.Text = String.Empty Then
                        Me.ValidationHelper.AddError(txtPrintersNumOfLocations, "Missing Number of Locations", accordList)
                    Else
                        If Not IsNumeric(txtPrintersNumOfLocations.Text) OrElse CDec(txtPrintersNumOfLocations.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtPrintersNumOfLocations, "Invalid Value", accordList)
                        End If
                    End If
                End If
            End If

            ' Funeral Directors
            If chkFuneralDirectors.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.FuneralDirectors, err) Then
                If chkFuneralDirectors.Checked Then
                    If txtFuneralNumOfEmployees.Text = String.Empty Then
                        Me.ValidationHelper.AddError(txtFuneralNumOfEmployees, "Missing Number of Employees", accordList)
                    Else
                        If Not IsNumeric(txtFuneralNumOfEmployees.Text) OrElse CDec(txtFuneralNumOfEmployees.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtFuneralNumOfEmployees, "Invalid Value", accordList)
                        End If
                    End If
                End If
            End If

            ' Photography (NEW)
            If chkPhotography.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Photography, err) Then
                If Not chkPhotogMakeupAndHair.Checked AndAlso Not chkPhotographyScheduledEquipment.Checked Then
                    Me.ValidationHelper.AddError("When 'Photography' is checked, you must add at least one of 'Makeup and Hair' or 'Scheduled Equipment' coverages", Nothing)
                End If
                If chkPhotographyScheduledEquipment.Checked Then
                    If txtPhotogScheduledEquipmentLimit.Text = String.Empty Then
                        Me.ValidationHelper.AddError(txtPhotogScheduledEquipmentLimit, "Missing Limit", accordList)
                    Else
                        If Not IsNumeric(txtPhotogScheduledEquipmentLimit.Text) Then
                            Me.ValidationHelper.AddError(txtPhotogScheduledEquipmentLimit, "Invalid Value", accordList)
                        Else
                            If CDec(txtPhotogScheduledEquipmentLimit.Text) <= 0 OrElse CDec(txtPhotogScheduledEquipmentLimit.Text) > 1000000 Then
                                Me.ValidationHelper.AddError(txtPhotogScheduledEquipmentLimit, "Invalid Amount", accordList)
                            End If
                        End If
                    End If
                End If
            End If

            '' Photography
            'If BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Photography, err) Then
            '    If chkPhotography.Checked Then
            '        If Not chkPhotogMakeupAndHair.Checked AndAlso Not chkPhotographyScheduledEquipment.Checked Then
            '            Me.ValidationHelper.AddError("When 'Photography' is checked, you must add at least one of 'Makeup and Hair' or 'Scheduled Equipment' coverages", Nothing)
            '        End If
            '        If chkPhotographyScheduledEquipment.Checked Then
            '            If rptPhotogScheduledItems.Items.Count <= 0 Then
            '                Me.ValidationHelper.AddError("When 'Photography Scheduled Equipment' is checked, you must add at least one scheduled item", Nothing)
            '            Else
            '                For Each ri As RepeaterItem In rptPhotogScheduledItems.Items
            '                    Dim Amt As TextBox = ri.FindControl("txtPhotogItemLimit")
            '                    Dim Dscr As TextBox = ri.FindControl("txtPhotogItemDesc")
            '                    If Amt Is Nothing OrElse Dscr Is Nothing Then Throw New Exception("controls not found")
            '                    If Amt.Text.Trim = "" Then
            '                        Me.ValidationHelper.AddError(Amt, "Missing Limit", accordList)
            '                    Else
            '                        If Not IsNumeric(Amt.Text) OrElse CInt(Amt.Text) <= 0 Then
            '                            Me.ValidationHelper.AddError(Amt, "Limit is invalid", accordList)
            '                        End If
            '                    End If
            '                    If Dscr.Text.Trim = "" Then
            '                        Me.ValidationHelper.AddError(Dscr, "Missing Description", accordList)
            '                    End If
            '                Next
            '            End If
            '        End If
            '    End If
            'End If

            ' Residential Cleaning

            ' Self Storage
            If chkSelfStorage.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.SelfStorage, err) Then
                If chkSelfStorage.Checked Then
                    If txtStorageLimit.Text.Trim <> "" AndAlso (Not IsNumeric(txtStorageLimit.Text)) Then
                        Me.ValidationHelper.AddError(txtStorageLimit, "Invalid Value", accordList)
                    End If
                End If
            End If

            ' Veterinarians
            If chkVeterinarians.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Veterinarians, err) Then
                If chkVeterinarians.Checked Then
                    If txtVeterinariansNumOfEmployees.Text = String.Empty Then
                        Me.ValidationHelper.AddError(txtVeterinariansNumOfEmployees, "Missing Number of Employees", accordList)
                    Else
                        If Not IsNumeric(txtVeterinariansNumOfEmployees.Text) OrElse CDec(txtVeterinariansNumOfEmployees.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtVeterinariansNumOfEmployees, "Invalid Value", accordList)
                        End If
                    End If
                End If
            End If

            ' Motel
            If chkMotel.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Motel, err) Then
                If chkMotel.Checked Then
                    ' When a limit is selected deductible is required
                    If ddlMotelSafeDepositBoxLimit.SelectedIndex > 0 Then
                        If ddlMotelSafeDepositBoxDeductible.SelectedIndex <= 0 Then
                            Me.ValidationHelper.AddError(ddlMotelSafeDepositBoxDeductible, "Missing Safe Deposit Box Deductible", accordList)
                        End If
                    End If
                End If
            End If

            ' Apartments
            ' Only validate the building where the coverage fields are enabled
            If chkApartments.Checked AndAlso BuildingIsFirstOnStateQuoteWithProfessionalLiabilityCoverage(ClassCoverage_Enum.Apartments, err) Then
                If chkApartments.Checked Then
                    If txtAptsNumberOfLocationsWithApts.Text = String.Empty Then
                        Me.ValidationHelper.AddError(txtAptsNumberOfLocationsWithApts, "Missing Number of Locations with Apartments", accordList)
                    Else
                        If Not IsNumeric(txtAptsNumberOfLocationsWithApts.Text) OrElse CDec(txtAptsNumberOfLocationsWithApts.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtAptsNumberOfLocationsWithApts, vbBack, accordList)
                        End If
                    End If
                    ' If a limit of liability is selected then the deductible is required
                    If ddlAptAutoLiabilityLimit.SelectedIndex > 0 Then
                        If ddlAptDeductible.SelectedIndex <= 0 Then
                            Me.ValidationHelper.AddError(ddlAptDeductible, "Missing Deductible", accordList)
                        End If
                    End If
                End If
            End If

            Me.ValidateChildControls(valArgs)

            Exit Sub
        Catch ex As Exception
            HandleError("ValidateControl", ex)
            Exit Sub
        End Try
    End Sub

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim js As String = "if(this.checked == false) { if (confirm(""Are you sure you want to delete this coverage?"")) {__doPostBack('');return true} else {return false;}}"

        Me.MainAccordionDivId = Me.divMain.ClientID

        If Not IsPostBack Then

        End If
    End Sub

    Private Sub chkAcctsReceivableONPremises_CheckedChanged(sender As Object, e As EventArgs) Handles chkAcctsReceivableONPremises.CheckedChanged
        Try
            If chkAcctsReceivableONPremises.Checked Then
                trAcctsReceivableONPremisesDataRow.Attributes.Add("style", "display:''")
            Else
                trAcctsReceivableONPremisesDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkAcctsReceivableONPremises_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub chkValuablePapersOnPremises_CheckedChanged(sender As Object, e As EventArgs) Handles chkValuablePapersOnPremises.CheckedChanged
        Try
            If chkValuablePapersOnPremises.Checked Then
                trValuablePapersOnPremisesDataRow.Attributes.Add("style", "display:''")
            Else
                trValuablePapersOnPremisesDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkValuablePapersOnPremises_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub chkOrdinanceOrLaw_CheckedChanged(sender As Object, e As EventArgs) Handles chkOrdinanceOrLaw.CheckedChanged
        Try
            If chkOrdinanceOrLaw.Checked Then
                trOrdinanceOrLawDataRow.Attributes.Add("style", "display:''")
            Else
                trOrdinanceOrLawDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkOrdinanceOrLaw_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub chkSpoilage_CheckedChanged(sender As Object, e As EventArgs) Handles chkSpoilage.CheckedChanged
        Try
            If chkSpoilage.Checked Then
                trSpoilageDataRow.Attributes.Add("style", "display:''")
            Else
                trSpoilageDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkSpoilage_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Private Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        Try
            chkAcctsReceivableONPremises.Checked = False
            txtAcctsReceivableOnPremisesTotalLimit.Text = String.Empty
            chkAcctsReceivableONPremises_CheckedChanged(Me, New EventArgs())

            chkValuablePapersOnPremises.Checked = False
            txtValuablePapersOnPremisesTotalLimit.Text = String.Empty
            chkValuablePapersOnPremises_CheckedChanged(Me, New EventArgs())

            chkOrdinanceOrLaw.Checked = False
            chkOrdinanceOrLawUndamaged.Checked = False
            txtDemolitionCostLimit.Text = String.Empty
            txtIncreasedCostOfConstructionLimit.Text = String.Empty
            txtDemolitionAndIncreasedCostCombinedLimit.Text = String.Empty
            chkOrdinanceOrLaw_CheckedChanged(Me, New EventArgs())

            chkSpoilage.Checked = False
            ddlSpoilagePropertyClassification.SelectedIndex = 0
            chkRefrigeratorMaintenanceAgreement.Checked = False
            chkBreakdownOrContamination.Checked = False
            chkPowerOutage.Checked = False
            chkSpoilage_CheckedChanged(Me, New EventArgs())

            ' CLASS-BASED PL COVERAGES
            ' Apartments
            chkApartments.Checked = False
            txtAptsNumberOfLocationsWithApts.Text = ""
            ddlAptAutoLiabilityLimit.SelectedIndex = 0
            ddlAptDeductible.SelectedIndex = 0
            chkApartments_CheckedChanged(Me, New EventArgs())

            ' Barbers
            chkBarbersLiability.Checked = False
            txtBarberFullTimeEmployees.Text = ""
            txtBarberPartTimeEmployees.Text = ""
            chkBarbersLiability_CheckedChanged(Me, New EventArgs())

            ' Beauticians
            chkBeautyShops.Checked = False
            txtBeautyShopsNumFullTimeEmployees.Text = ""
            txtBeautyShopsNumPartTimeEmployees.Text = ""
            chkBeautyShops_CheckedChanged(Me, New EventArgs())

            ' Fine Arts
            chkFineArts.Checked = False

            ' Funeral Directors
            chkFuneralDirectors.Checked = False
            txtFuneralNumOfEmployees.Text = ""
            chkFuneralDirectors_CheckedChanged(Me, New EventArgs())

            ' Liquor Liability
            chkLiquorLiability.Checked = False
            txtLiquorLiabiltyAnnualGrossAlcoholSales.Text = ""
            chkLiquorLiability_CheckedChanged(Me, New EventArgs())

            ' Motel
            chkMotel.Checked = False
            ddlMotelLiabilityLimit.SelectedIndex = 0
            ddlMotelSafeDepositBoxDeductible.SelectedIndex = 0
            ddlMotelSafeDepositBoxLimit.SelectedIndex = 0
            chkMotel_CheckedChanged(Me, New EventArgs())

            ' Optical & Hearing Aid
            chkOpticalAndHearingAid.Checked = False
            txtOpticalAndHearingNumOfEmployees.Text = ""
            chkOpticalAndHearingAid_CheckedChanged(Me, New EventArgs())

            ' Pharmacists
            chkPharmacists.Checked = False
            txtPharmacistsReceipts.Text = ""
            chkPharmacists_CheckedChanged(Me, New EventArgs())

            ' Photography
            chkPhotography.Checked = False
            chkPhotographyScheduledEquipment.Checked = False
            chkPhotogMakeupAndHair.Checked = False
            txtPhotogScheduledEquipmentLimit.Text = ""
            chkPhotography_CheckedChanged(Me, New EventArgs())

            ' Printers
            chkPrinters.Checked = False
            txtPrintersNumOfLocations.Text = ""
            chkPrinters_CheckedChanged(Me, New EventArgs())


            ' Residential Cleaning
            chkResidentialCleaning.Checked = False

            ' Restaurants
            chkRestaurant.Checked = False
            ddlRestaurantsDeductible.SelectedIndex = 0
            ddlRestaurantsLimitOfLiability.SelectedIndex = 0
            chkRestaurant_CheckedChanged(Me, New EventArgs())

            ' Self Storage
            chkSelfStorage.Checked = False
            txtStorageLimit.Text = ""
            chkSelfStorage_CheckedChanged(Me, New EventArgs())

            ' Veterinarians
            chkVeterinarians.Checked = False
            txtVeterinariansNumOfEmployees.Text = ""
            chkVeterinarians_CheckedChanged(Me, New EventArgs())

            Exit Sub
        Catch ex As Exception
            HandleError("lnkClear_Click", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub chkBarbersLiability_CheckedChanged(sender As Object, e As EventArgs) Handles chkBarbersLiability.CheckedChanged
        Try
            If chkBarbersLiability.Checked Then
                trBarbersLiabilityDataRow.Attributes.Add("style", "display:''")
            Else
                trBarbersLiabilityDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkBarbersLiability_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub chkLiquorLiability_CheckedChanged(sender As Object, e As EventArgs) Handles chkLiquorLiability.CheckedChanged
        Try
            If chkLiquorLiability.Checked Then
                trLiquorLiabilityDataRow.Attributes.Add("style", "display:''")
            Else
                trLiquorLiabilityDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkLiquorLiability_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub chkRestaurant_CheckedChanged(sender As Object, e As EventArgs) Handles chkRestaurant.CheckedChanged
        Try
            If chkRestaurant.Checked Then
                trRestaurantDataRow.Attributes.Add("style", "display:''")
            Else
                trRestaurantDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkRestaurant_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub chkPharmacists_CheckedChanged(sender As Object, e As EventArgs) Handles chkPharmacists.CheckedChanged
        Try
            If chkPharmacists.Checked Then
                trPharmacistsDataRow.Attributes.Add("style", "display:''")
            Else
                trPharmacistsDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkPharmacists_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub chkOpticalAndHearingAid_CheckedChanged(sender As Object, e As EventArgs) Handles chkOpticalAndHearingAid.CheckedChanged
        Try
            If chkOpticalAndHearingAid.Checked Then
                trOpticalAndHearingDataRow.Attributes.Add("style", "display:''")
            Else
                trOpticalAndHearingDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkOpticalAndHearingAid_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub chkBeautyShops_CheckedChanged(sender As Object, e As EventArgs) Handles chkBeautyShops.CheckedChanged
        Try
            If chkBeautyShops.Checked Then
                trBeautyShopsDataRow.Attributes.Add("style", "display:''")
            Else
                trBeautyShopsDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkBeautyShops_CheckedChanged", ex)
        End Try
    End Sub

    'Private Sub chkPrinters_CheckedChanged(sender As Object, e As EventArgs) Handles chkPrinters.CheckedChanged
    '    Try
    '        If chkPrinters.Checked Then
    '            trPrintersDataRow.Visible = True
    '        Else
    '            trPrintersDataRow.Visible = False
    '        End If

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("chkPrinters_CheckedChanged", ex)
    '    End Try
    'End Sub

    Private Sub chkFuneralDirectors_CheckedChanged(sender As Object, e As EventArgs) Handles chkFuneralDirectors.CheckedChanged
        Try
            If chkFuneralDirectors.Checked Then
                trFuneralDataRow.Attributes.Add("style", "display:''")
            Else
                trFuneralDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkFuneralDirectors_CheckedChanged", ex)
        End Try
    End Sub

    'Private Sub chkPhotography_CheckedChanged(sender As Object, e As EventArgs) Handles chkPhotography.CheckedChanged
    '    Try
    '        If chkPhotography.Checked Then
    '            trPhotographyDataRow.Attributes.Add("style", "display:''")
    '            trPhotographyMakeupAndHairCheckboxRow.Attributes.Add("style", "display:''")
    '        Else
    '            MyBuilding.HasPhotographyCoverage = False
    '            trPhotographyDataRow.Attributes.Add("style", "display:none")
    '            trPhotographyMakeupAndHairCheckboxRow.Attributes.Add("style", "display:none")
    '        End If

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("chkPhotography_CheckedChanged", ex)
    '    End Try
    'End Sub

    Private Sub chkSelfStorage_CheckedChanged(sender As Object, e As EventArgs) Handles chkSelfStorage.CheckedChanged
        Try
            If chkSelfStorage.Checked Then
                trSelfStorageDataRow.Attributes.Add("style", "display:''")
            Else
                trSelfStorageDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkSelfStorage_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub chkVeterinarians_CheckedChanged(sender As Object, e As EventArgs) Handles chkVeterinarians.CheckedChanged
        Try
            If chkVeterinarians.Checked Then
                trVeterinariansDataRow.Attributes.Add("style", "display:''")
            Else
                trVeterinariansDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkVeterinarians_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub chkMotel_CheckedChanged(sender As Object, e As EventArgs) Handles chkMotel.CheckedChanged
        Try
            If chkMotel.Checked Then
                trMotelDataRow.Attributes.Add("style", "display:''")
            Else
                trMotelDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkMotel_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub chkApartments_CheckedChanged(sender As Object, e As EventArgs) Handles chkApartments.CheckedChanged
        Try
            If chkApartments.Checked Then
                trApartmentsDataRow.Attributes.Add("style", "display:''")
            Else
                trApartmentsDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkApartments_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub chkPhotography_CheckedChanged(sender As Object, e As EventArgs) Handles chkPhotography.CheckedChanged
        Try
            If chkPhotography.Checked Then
                trPhotographyDataRow.Attributes.Add("style", "display:''")
            Else
                trPhotographyDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkPhotography_CheckedChanged", ex)
        End Try
    End Sub

    Private Sub chkPrinters_CheckedChanged(sender As Object, e As EventArgs) Handles chkPrinters.CheckedChanged
        Try
            If chkPrinters.Checked Then
                trPrintersDataRow.Attributes.Add("style", "display:''")
            Else
                trPrintersDataRow.Attributes.Add("style", "display:none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("chkPrinters_CheckedChanged", ex)
        End Try
    End Sub

    'Private Sub chkPhotographyScheduledEquipment_CheckedChanged(sender As Object, e As EventArgs) Handles chkPhotographyScheduledEquipment.CheckedChanged
    '    Dim err As String = Nothing
    '    Try
    '        'If chkPhotographyScheduledEquipment.Checked Then
    '        '    trPhotogItemsTotalRow.Attributes.Add("style", "display:''")
    '        '    trPhotogScheduledItemsListRow.Attributes.Add("style", "display:''")
    '        '    trPhotogAddItemRow.Attributes.Add("style", "display:''")
    '        'Else
    '        '    lblPhotographerItemsTotal.Text = "Total of All Scheduled Limits: "
    '        '    trPhotogItemsTotalRow.Attributes.Add("style", "display:none")
    '        '    trPhotogScheduledItemsListRow.Attributes.Add("style", "display:none")
    '        '    trPhotogAddItemRow.Attributes.Add("style", "display:none")
    '        '    rptPhotogScheduledItems.DataSource = Nothing
    '        '    rptPhotogScheduledItems.DataBind()
    '        'End If

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("chkPhotographyScheduledEquipment_CheckedChanged", ex)
    '        Exit Sub
    '    End Try
    'End Sub

    'Private Sub rptPhotogScheduledItems_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPhotogScheduledItems.ItemDataBound
    '    Dim desc As TextBox = Nothing
    '    Dim amt As TextBox = Nothing
    '    Dim drec As QuickQuote.CommonObjects.QuickQuoteCoverage = Nothing

    '    Try
    '        desc = e.Item.FindControl("txtPhotogItemDesc")
    '        amt = e.Item.FindControl("txtPhotogItemLimit")
    '        If desc Is Nothing OrElse amt Is Nothing Then
    '            Throw New Exception("Control not found")
    '        End If
    '        drec = e.Item.DataItem
    '        desc.Text = drec.Description
    '        amt.Text = drec.ManualLimitAmount

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("rptPhotogScheduledItems_ItemDataBound", ex)
    '        Exit Sub
    '    End Try
    'End Sub

    'Private Sub lbPhotogAddItem_Click(sender As Object, e As EventArgs) Handles lbPhotogAddItem.Click
    '    Dim NewItem As New QuickQuote.CommonObjects.QuickQuoteCoverage()
    '    Try
    '        If Not Quote.HasPhotographyCoverageScheduledCoverages Then Quote.HasPhotographyCoverageScheduledCoverages = True
    '        Quote.CopyProfessionalLiabilityCoveragesFromPolicyToBuildings_UseBuildingClassificationList()
    '        Save_FireSaveEvent(False) ' Save any previously entered unsaved items
    '        If Not MyBuilding.HasPhotographyCoverageScheduledCoverages Then MyBuilding.HasPhotographyCoverageScheduledCoverages = True
    '        If MyBuilding.PhotographyScheduledCoverages Is Nothing Then MyBuilding.PhotographyScheduledCoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteCoverage)
    '        NewItem.Description = "Item " & MyBuilding.PhotographyScheduledCoverages.Count + 1.ToString
    '        NewItem.ManualLimitAmount = ""
    '        MyBuilding.PhotographyScheduledCoverages.Add(NewItem)
    '        Quote.CopyProfessionalLiabilityCoveragesFromBuildingsToPolicy_UseBuildingClassificationList()
    '        Populate()
    '        Save_FireSaveEvent(False)
    '        Populate_FirePopulateEvent()

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("lbPhotogAddItem_Click", ex)
    '        Exit Sub
    '    End Try
    'End Sub

    'Protected Sub rptPhotogScheduledItems_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
    '    Dim itemIndex As Integer = e.Item.ItemIndex
    '    Try
    '        Select Case e.CommandName
    '            Case "DELETE"
    '                Quote.CopyProfessionalLiabilityCoveragesFromPolicyToBuildings_UseBuildingClassificationList()
    '                MyBuilding.PhotographyScheduledCoverages.RemoveAt(itemIndex)
    '                Quote.CopyProfessionalLiabilityCoveragesFromBuildingsToPolicy_UseBuildingClassificationList()
    '                Populate()
    '                Save_FireSaveEvent(False)
    '                Populate_FirePopulateEvent()
    '                Exit Select
    '        End Select

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("rptPhotogScheduledItems_ItemCommand", ex)
    '        Exit Sub
    '    End Try
    'End Sub


#End Region

End Class