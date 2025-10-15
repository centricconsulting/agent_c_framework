Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM

Public Class ctlInlandMarine
    Inherits VRControlBase

    Public Event RecalculateCoverageTotal(optionalTotal As String)
    Public Event RefreshBaseCoverage()
    Public Event CommonRateIM()

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Public Property ActiveIMHeader() As Boolean
        Get
            Return ViewState("vs_ToggleIMHeader")
        End Get
        Set(ByVal value As Boolean)
            ViewState("vs_ToggleIMHeader") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MainAccordionDivId = dvInlandMarineInput.ClientID

        If Not IsPostBack Then
            If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Me.lblGolfers.Text = " Golfers Equipment (Excluding Golf Carts)"
            End If
            divSaveRateButtons.Visible = Not Me.IsOnAppPage ' Matt A - 8/3/15
            LoadStaticData()
            Populate()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Not ActiveIMHeader Then
            VRScript.CreateAccordion(MainAccordionDivId, hiddenIM, "false", True)
            Dim imTotalInt As Integer = 0
            ToggleIMCoverages(ActiveIMHeader)
            lblIMChosen.Text = imTotalInt
        Else
            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                VRScript.CreateAccordion(MainAccordionDivId, hiddenIM, "0", False)
                ToggleIMCoverages(ActiveIMHeader)
            Else
                VRScript.CreateAccordion(MainAccordionDivId, hiddenIM, "false")
            End If
        End If

        VRScript.StopEventPropagation(lnkSaveinland.ClientID, True)
        VRScript.CreateConfirmDialog(lnkClearInland.ClientID, "Clear Inland Marine Items?")
    End Sub

    Private Sub ToggleIMCoverages(state As Boolean)
        lnkClearInland.Visible = state
        lnkSaveinland.Visible = state
        dvIMCoverages.Visible = state
        btnSaveIM.Visible = state
        btnRateIM.Visible = state
    End Sub

    Public Overrides Sub LoadStaticData()
        SetDefaultValues()
    End Sub

    Public Overrides Sub Populate()
        If MyLocation IsNot Nothing Then
            Dim imTotalInt As Integer = 0
            Dim recalculateCount As Boolean = False

            If MyLocation.InlandMarines IsNot Nothing AndAlso MyLocation.InlandMarines.Count > 0 Then
                ' Inland Marines Jewelry
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry).Count > 0 Then
                    Dim imJewelryDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry)

                    ctlJewelry.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry
                    ctlJewelry.Populate()
                    chkIMJewelry.Checked = True
                    chkIMJewelry.Enabled = False
                    dvIMJewelryLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Jewelry In Vault
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault).Count > 0 Then
                    Dim jewelryInVaultDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault)

                    ctlJewelInVault.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault
                    ctlJewelInVault.Populate()
                    chkJewelInVault.Checked = True
                    chkJewelInVault.Enabled = False
                    dvJewelInVaultLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                'Bicycles added 3-21-16 Matt for Comparative Rater project
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Bicycles).Count > 0 Then
                    Dim bicyclesDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Bicycles)

                    ctlBikeList.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Bicycles
                    ctlBikeList.Populate()
                    chkBike.Checked = True
                    chkBike.Enabled = False
                    dvBikeLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                'Coins added 3-21-16 Matt for Comparative Rater project
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Coins).Count > 0 Then
                    Dim coinsDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Coins)

                    CtlCoinsList.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Coins
                    CtlCoinsList.Populate()
                    chkCoins.Checked = True
                    chkCoins.Enabled = False
                    dvCoinLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If


                ' Antiques - with breakage coverage
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithBreakage).Count > 0 Then
                    Dim AntiqueBreakDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithBreakage)

                    ctlAntiquesBreak.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithBreakage
                    ctlAntiquesBreak.Populate()
                    chkAntiquesBreak.Checked = True
                    chkAntiquesBreak.Enabled = False
                    dvAntiquesBreakLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Antiques - without breakage coverage
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithoutBreakage).Count > 0 Then
                    Dim AntiquesNoBreakDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithoutBreakage)

                    ctlAntiquesNoBreak.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithoutBreakage
                    ctlAntiquesNoBreak.Populate()
                    chkAntiquesNoBreak.Checked = True
                    chkAntiquesNoBreak.Enabled = False
                    dvAntiquesNoBreakLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Collector Items Hobby - with breakage coverage
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithBreakage).Count > 0 Then
                    Dim CollectBreakDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithBreakage)

                    ctlCollectBreak.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithBreakage
                    ctlCollectBreak.Populate()
                    chkCollectBreak.Checked = True
                    chkCollectBreak.Enabled = False
                    dvCollectBreakLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Collector Items Hobby - without breakage coverage
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithoutBreakage).Count > 0 Then
                    Dim CollectNoBreakDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithoutBreakage)

                    ctlCollectNoBreak.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithoutBreakage
                    ctlCollectNoBreak.Populate()
                    chkCollectNoBreak.Checked = True
                    chkCollectNoBreak.Enabled = False
                    dvCollectNoBreakLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Cameras
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras).Count > 0 Then
                    Dim CamerasDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras)

                    ctlCameras.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras
                    ctlCameras.Populate()
                    chkCameras.Checked = True
                    chkCameras.Enabled = False
                    dvCamerasLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Computers
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer).Count > 0 Then
                    Dim ComputersDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer)

                    ctlComputers.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer
                    ctlComputers.Populate()
                    chkComputers.Checked = True
                    chkComputers.Enabled = False
                    dvComputersLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Farm Machinery - Scheduled
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled).Count > 0 Then
                    Dim FarmMachineryDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled)

                    ctlFarmMachineSched.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled
                    ctlFarmMachineSched.Populate()
                    chkFarmMachineSched.Checked = True
                    chkFarmMachineSched.Enabled = False
                    dvFarmMachineSchedLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Fine Arts - with breakage coverage
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage).Count > 0 Then
                    Dim ArtsBreakDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage)

                    ctlArtsBreak.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage
                    ctlArtsBreak.Populate()
                    chkFABreak.Checked = True
                    chkFABreak.Enabled = False
                    dvFABreakLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Fine Arts - without breakage coverage
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage).Count > 0 Then
                    Dim ArtsNoBreakDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage)

                    ctlArtsNoBreak.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage
                    ctlArtsNoBreak.Populate()
                    chkFANoBreak.Checked = True
                    chkFANoBreak.Enabled = False
                    dvFANoBreakLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Furs
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs).Count > 0 Then
                    Dim FursDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs)

                    ctlFurs.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs
                    ctlFurs.Populate()
                    chkFurs.Checked = True
                    chkFurs.Enabled = False
                    dvFursLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Garden Tractors
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors).Count > 0 Then
                    Dim GardenDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors)

                    ctlGarden.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors
                    ctlGarden.Populate()
                    chkGarden.Checked = True
                    chkGarden.Enabled = False
                    dvGardenLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Golfers Equipment
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf).Count > 0 Then
                    Dim GolfersDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf)

                    ctlGolfers.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf
                    ctlGolfers.Populate()
                    chkGolfers.Checked = True
                    chkGolfers.Enabled = False
                    dvGolfersLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                'Grave Markers
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GraveMarkers).Count > 0 Then

                    ctlGraveMarkers.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GraveMarkers
                    ctlGraveMarkers.Populate()
                    chkGraveMarkers.Checked = True
                    chkGraveMarkers.Enabled = False
                    dvGraveMarkersLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Guns
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns).Count > 0 Then
                    Dim GunsDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns)

                    ctlGuns.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns
                    ctlGuns.Populate()
                    chkGuns.Checked = True
                    chkGuns.Enabled = False
                    dvGunsLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Hearing Aids
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids).Count > 0 Then
                    Dim HearingDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids)

                    ctlHearing.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids
                    ctlHearing.Populate()
                    chkHearing.Checked = True
                    chkHearing.Enabled = False
                    dvHearingLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Medical Items and Equipment
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.MedicalItemsAndEquipment).Count > 0 Then
                    Dim MedicalItemsAndEquipmentDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.MedicalItemsAndEquipment)

                    ctlMedicalItemsAndEquipment.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.MedicalItemsAndEquipment
                    ctlMedicalItemsAndEquipment.Populate()
                    chkMedicalItemsAndEquipment.Checked = True
                    chkMedicalItemsAndEquipment.Enabled = False
                    dvMedicalItemsAndEquipmentLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Silverware
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I).Count > 0 Then
                    Dim SilverwareDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I)

                    ctlSilverware.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I
                    ctlSilverware.Populate()
                    chkSilverware.Checked = True
                    chkSilverware.Enabled = False
                    dvSilverwareLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Sports Equipment
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineSportsEquipment).Count > 0 Then
                    Dim SportsEquipmentDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineSportsEquipment)

                    ctlSportsEquipment.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineSportsEquipment
                    ctlSportsEquipment.Populate()
                    chkSportsEquipment.Checked = True
                    chkSportsEquipment.Enabled = False
                    dvSportsEquipmentLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Irrigation Equipment - Named Perils
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentNamedPerils).Count > 0 Then
                    Dim IrrigationNamedDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentNamedPerils)

                    ctlIrrigationNamed.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentNamedPerils
                    ctlIrrigationNamed.Populate()
                    chkIrrigationNamed.Checked = True
                    chkIrrigationNamed.Enabled = False
                    dvIrrigationNamedLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Irrigation Equipment - Special Form
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentSpecialCoverage).Count > 0 Then
                    Dim IrrigationSpecialDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentSpecialCoverage)

                    ctlIrrigationSpecial.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentSpecialCoverage
                    ctlIrrigationSpecial.Populate()
                    chkIrrigationSpecial.Checked = True
                    chkIrrigationSpecial.Enabled = False
                    dvIrrigationSpecialLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Radios - CB
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_CB).Count > 0 Then
                    Dim RadioCBDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_CB)

                    ctlRadioCB.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_CB
                    ctlRadioCB.Populate()
                    chkRadioCB.Checked = True
                    chkRadioCB.Enabled = False
                    dvRadioCBLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Radios - FM
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_FM).Count > 0 Then
                    Dim RadioFMDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_FM)

                    ctlRadiosFM.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_FM
                    ctlRadiosFM.Populate()
                    chkRadiosFM.Checked = True
                    chkRadiosFM.Enabled = False
                    dvRadiosFMLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Reproductive Materials - Named Perils
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsNamedPerils).Count > 0 Then
                    Dim ReproductiveNamedDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsNamedPerils)

                    ctlReproductiveNamed.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsNamedPerils
                    ctlReproductiveNamed.Populate()
                    chkReproductiveNamed.Checked = True
                    chkReproductiveNamed.Enabled = False
                    dvReproductiveNamedLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Reproductive Materials - Special Form
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsSpecialCoverage).Count > 0 Then
                    Dim ReproductiveSpecialDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsSpecialCoverage)

                    ctlReproductiveSpecial.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsSpecialCoverage
                    ctlReproductiveSpecial.Populate()
                    chkReproductiveSpecial.Checked = True
                    chkReproductiveSpecial.Enabled = False
                    dvReproductiveSpecialLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Telephone - Car or Mobile
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile).Count > 0 Then
                    Dim MobileDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile)

                    ctlMobile.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile
                    ctlMobile.Populate()
                    chkMobile.Checked = True
                    chkMobile.Enabled = False
                    dvMobileLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Tools and Equipment
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment).Count > 0 Then
                    Dim ToolsDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment)

                    ctlTools.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment
                    ctlTools.Populate()
                    chkTools.Checked = True
                    chkTools.Enabled = False
                    dvToolsLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If

                ' Musical Instrument (Non-Professional)
                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional).Count > 0 Then
                    Dim MusicDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional)

                    ctlMusicInstr.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional
                    ctlMusicInstr.Populate()
                    chkMusic.Checked = True
                    chkMusic.Enabled = False
                    dvMusicLimit.Attributes.Add("style", "display:block;")
                    imTotalInt += 1
                    recalculateCount = True
                End If
            End If

            If recalculateCount Then
                lblIMChosen.Text = imTotalInt
                'hiddenSelectedIMCoverage.Value = imTotalInt.ToString
                'RaiseEvent RecalculateCoverageTotal(hiddenSelectedIMCoverage.Value)
            End If
            'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
            If Me.IsQuoteReadOnly Then
                Me.lnkAddIMJewelryLimit.Visible = False
                Me.lnkAddJewleryInVault.Visible = False
                Me.lnkAddAntiquesBreak.Visible = False
                Me.lnkAddAntiquesNoBreak.Visible = False
                Me.lnkAddBike.Visible = False
                Me.lnkAddCameras.Visible = False
                Me.lnkAddCollectBreak.Visible = False
                Me.lnkAddCollectNoBreak.Visible = False
                Me.lnkAddCoins.Visible = False
                Me.lnkAddComputers.Visible = False
                Me.lnkAddFarmMachineSched.Visible = False
                Me.lnkAddFABreak.Visible = False
                Me.lnkAddFANoBreak.Visible = False
                Me.lnkAddFurs.Visible = False
                Me.lnkAddGarden.Visible = False
                Me.lnkAddGolfers.Visible = False
                Me.lnkAddGraveMarkers.Visible = False
                Me.lnkAddGuns.Visible = False
                Me.lnkAddHearing.Visible = False
                Me.lnkAddIrrigationNamed.Visible = False
                Me.lnkAddIrrigationSpecial.Visible = False
                Me.lnkAddMedicalItemsAndEquipment.Visible = False
                Me.lnkAddMusic.Visible = False
                Me.lnkAddRadioCB.Visible = False
                Me.lnkAddRadiosFM.Visible = False
                Me.lnkAddReproductiveNamed.Visible = False
                Me.lnkAddReproductiveSpecial.Visible = False
                Me.lnkAddSilverware.Visible = False
                Me.lnkAddSportsEquipment.Visible = False
                Me.lnkAddMobile.Visible = False
                Me.lnkAddTools.Visible = False
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = "Inland Marine"
        Dim divInlandMarine As String = dvInlandMarineInput.ClientID
        Dim gunsError As Boolean = False

        'Updated 02/10/2020 for Home Endorsemenst Task 43872 MLW
        If Not Me.IsQuoteEndorsement And Not Me.IsOnAppPage Then
            Dim valList = InlandMarineListValidator.ValidateHOMInlandMarine(Me.Quote, valArgs.ValidationType)

            If valList.Any() Then
                For Each v In valList
                    ' *************************
                    ' Optional Policy Coverages
                    ' *************************
                    Select Case v.FieldId
                        Case InlandMarineListValidator.Combined_Jewelry_Exceeded_30000
                            ValidationHelper.Val_BindValidationItemToControl(lblIMJewelry, v, divInlandMarine, "0")
                        Case InlandMarineListValidator.Combined_JewelryVault_Exceeded_30000
                            ValidationHelper.Val_BindValidationItemToControl(lblJewelInVault, v, divInlandMarine, "0")
                        Case InlandMarineListValidator.Combined_ArtsBreak_Exceeded_30000
                            ValidationHelper.Val_BindValidationItemToControl(lblFABreak, v, divInlandMarine, "0")
                        Case InlandMarineListValidator.Combined_ArtsNoBreak_Exceeded_30000
                            ValidationHelper.Val_BindValidationItemToControl(lblFANoBreak, v, divInlandMarine, "0")
                        Case InlandMarineListValidator.Combined_Furs_Exceeded_30000
                            ValidationHelper.Val_BindValidationItemToControl(lblFurs, v, divInlandMarine, "0")
                        Case InlandMarineListValidator.Combined_Guns_Limit_Exceeded
                            ValidationHelper.Val_BindValidationItemToControl(lblGuns, v, divInlandMarine, "0")
                        Case Else
                            ValidationHelper.Val_BindValidationItemToControl(ctlCollectNoBreak, v, divInlandMarine, "")
                    End Select
                Next
            End If
        End If

        If gunsError Then
            lblGuns.BorderColor = Drawing.Color.Red
            lblGuns.BorderStyle = BorderStyle.Solid
            lblGuns.BorderWidth = 1
        Else
            lblGuns.BorderStyle = BorderStyle.None
        End If

        ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean
        If MyLocation IsNot Nothing Then
            Dim InlandMarineCoverage As QuickQuoteInlandMarine = Nothing

            If MyLocation.InlandMarines Is Nothing Then
                MyLocation.InlandMarines = New List(Of QuickQuoteInlandMarine)
            End If

            SaveChildControls()
            Return True
        End If

        Return False
    End Function

    Private Sub SetDefaultValues()
        Dim imTotalInt As Integer = 0
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.Farm
                ' Hide Home specific coverages
                pnlBike.Visible = False ' added 3-21-16 Matt for Comparative Rater project
                pnlCoins.Visible = False ' added 3-21-16 Matt for Comparative Rater project
                pnlFarmMachineSched.Visible = False
                pnlGarden.Visible = False
                pnlGolfers.Visible = False
                pnlMedicalItemsAndEquipment.Visible = False
                pnlSilverware.Visible = False
                pnlSportsEquipment.Visible = False
                pnlTools.Visible = False
                pnlMusic.Visible = False
                pnlGraveMarkers.Visible = False
            Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                ' Hide Farm specific coverages
                pnlAntiquesBreak.Visible = False
                pnlAntiquesNoBreak.Visible = False
                'pnlCollectBreak.Visible = False 'added to HOM 1/11/2024 CAH
                'pnlCollectNoBreak.Visible = False 'added to HOM 1/11/2024 CAH
                pnlIrrigationNamed.Visible = False
                pnlIrrigationSpecial.Visible = False
                pnlRadioCB.Visible = False
                pnlRadiosFM.Visible = False
                pnlReproductiveNamed.Visible = False
                pnlReproductiveSpecial.Visible = False
        End Select

        dvIMJewelryLimit.Attributes.Add("style", "display:none;")
        dvJewelInVaultLimit.Attributes.Add("style", "display:none;")
        dvCamerasLimit.Attributes.Add("style", "display:none;")
        dvComputersLimit.Attributes.Add("style", "display:none;")
        dvFABreakLimit.Attributes.Add("style", "display:none;")
        dvFANoBreakLimit.Attributes.Add("style", "display:none;")
        dvFursLimit.Attributes.Add("style", "display:none;")
        dvGunsLimit.Attributes.Add("style", "display:none;")
        dvHearingLimit.Attributes.Add("style", "display:none;")
        dvMobileLimit.Attributes.Add("style", "display:none;")
        dvCollectBreakLimit.Attributes.Add("style", "display:none;")
        dvCollectNoBreakLimit.Attributes.Add("style", "display:none;")

        'Farm
        dvAntiquesBreakLimit.Attributes.Add("style", "display:none;")
        dvAntiquesNoBreakLimit.Attributes.Add("style", "display:none;")
        'dvCollectBreakLimit.Attributes.Add("style", "display:none;")
        'dvCollectNoBreakLimit.Attributes.Add("style", "display:none;")
        dvIrrigationNamedLimit.Attributes.Add("style", "display:none;")
        dvIrrigationSpecialLimit.Attributes.Add("style", "display:none;")
        dvRadioCBLimit.Attributes.Add("style", "display:none;")
        dvRadiosFMLimit.Attributes.Add("style", "display:none;")
        dvReproductiveNamedLimit.Attributes.Add("style", "display:none;")
        dvReproductiveSpecialLimit.Attributes.Add("style", "display:none;")

        ' Home
        dvCoinLimit.Attributes.Add("style", "display:none;") ' added 3-21-16 Matt for Comparative Rater project
        dvBikeLimit.Attributes.Add("style", "display:none;") ' added 3-21-16 Matt for Comparative Rater project
        dvFarmMachineSchedLimit.Attributes.Add("style", "display:none;")
        dvGardenLimit.Attributes.Add("style", "display:none;")
        dvGolfersLimit.Attributes.Add("style", "display:none;")
        dvMedicalItemsAndEquipmentLimit.Attributes.Add("style", "display:none;")
        dvSilverwareLimit.Attributes.Add("style", "display:none;")
        dvSportsEquipmentLimit.Attributes.Add("style", "display:none;")
        dvGraveMarkersLimit.Attributes.Add("style", "display:none;")
        dvToolsLimit.Attributes.Add("style", "display:none;")
        dvMusicLimit.Attributes.Add("style", "display:none;")


        lblIMChosen.Text = imTotalInt
        'hiddenSelectedIMCoverage.Value = imTotalInt.ToString
        'RaiseEvent RecalculateCoverageTotal(hiddenSelectedIMCoverage.Value)
    End Sub

    Private Sub AddNewItem(form As String)
        If MyLocation IsNot Nothing Then
            If MyLocation.InlandMarines Is Nothing Then
                MyLocation.InlandMarines = New List(Of QuickQuoteInlandMarine)()
            End If

            Dim newIMItem As New QuickQuoteInlandMarine()

            Select Case form
                Case "IMJewelry"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry
                    newIMItem.Description = "Jewelry"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkIMJewelry.Enabled = False
                Case "JewelryVault"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault
                    newIMItem.Description = "Jewelry in Vault"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkJewelInVault.Enabled = False
                Case "IMBicycle" ' added 3-21-16 Matt for Comparative Rater project
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Bicycles
                    newIMItem.Description = "Bicycles"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkBike.Enabled = False
                Case "IMCoin" ' added 3-21-16 Matt for Comparative Rater project
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Coins
                    newIMItem.Description = "Coins"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkCoins.Enabled = False
                Case "AntiqueBreak"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithBreakage
                    newIMItem.Description = "Antiques - with breakage coverage"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkAntiquesBreak.Enabled = False
                Case "AntiqueNoBreak"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithoutBreakage
                    newIMItem.Description = "Antiques - without breakage coverage"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkAntiquesNoBreak.Enabled = False
                Case "CollectBreak"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithBreakage
                    newIMItem.Description = "Collector Items Hobby - with breakage coverage"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkCollectBreak.Enabled = False
                Case "CollectNoBreak"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithoutBreakage
                    newIMItem.Description = "Collector Items Hobby - without breakage coverage"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkCollectNoBreak.Enabled = False
                Case "Cameras"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras
                    newIMItem.Description = "Cameras"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkCameras.Enabled = False
                Case "Computers"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer
                    newIMItem.Description = "Computers"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkComputers.Enabled = False
                Case "FarmMachinery"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled
                    newIMItem.Description = "Farm Machinery - Scheduled"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkFarmMachineSched.Enabled = False
                Case "ArtsBreak"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage
                    newIMItem.Description = "Fine Arts - with breakage coverage"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkFABreak.Enabled = False
                Case "ArtsNoBreak"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage
                    newIMItem.Description = "Fine Arts - without breakage coverage"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkFANoBreak.Enabled = False
                Case "Furs"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs
                    newIMItem.Description = "Furs"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkFurs.Enabled = False
                Case "Garden"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors
                    newIMItem.Description = "Garden Tractors"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkGarden.Enabled = False
                Case "Golfers"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf
                    newIMItem.Description = "Golfers Equipment"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkGolfers.Enabled = False
                Case "Guns"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns
                    newIMItem.Description = "Guns"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkGuns.Enabled = False
                Case "Hearing"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids
                    newIMItem.Description = "Hearing Aids"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkHearing.Enabled = False
                Case "MedicalItemsAndEquipment"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.MedicalItemsAndEquipment
                    newIMItem.Description = "Medical Items and Equipment"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkMedicalItemsAndEquipment.Enabled = False
                Case "Silverware"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I
                    newIMItem.Description = "Silverware"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkSilverware.Enabled = False
                Case "SportsEquipment"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineSportsEquipment
                    newIMItem.Description = "Sports Equipment"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkSportsEquipment.Enabled = False
                Case "IrrigateNamed"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentNamedPerils
                    newIMItem.Description = "Irrigation Equipment - Named Perils"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkIrrigationNamed.Enabled = False
                Case "IrrigateSpecial"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentSpecialCoverage
                    newIMItem.Description = "Irrigation Equipment - Special Form"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkIrrigationSpecial.Enabled = False
                Case "CB"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_CB
                    newIMItem.Description = "Radios - CB"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkRadioCB.Enabled = False
                Case "FM"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_FM
                    newIMItem.Description = "Radios - FM"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkRadiosFM.Enabled = False
                Case "ReproduceNamed"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsNamedPerils
                    newIMItem.Description = "Reproductive Materials - Named Perils"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkReproductiveNamed.Enabled = False
                Case "ReproduceSpecial"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsSpecialCoverage
                    newIMItem.Description = "Reproductive Materials - Special Form"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkReproductiveSpecial.Enabled = False
                Case "Mobile"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile
                    newIMItem.Description = "Telephone - Car or Mobile"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkMobile.Enabled = False
                Case "Tools"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment
                    newIMItem.Description = "Tools and Equipment"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkTools.Enabled = False
                Case "Music"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional
                    newIMItem.Description = "Musical Instrument (Non-Professional)"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkMusic.Enabled = False
                Case "GraveMarkers"
                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GraveMarkers
                    newIMItem.Description = "Grave Markers"
                    MyLocation.InlandMarines.Add(newIMItem)
                    chkGraveMarkers.Enabled = False
            End Select

            If Me.IsQuoteEndorsement Then
                newIMItem.Description = String.Empty
            End If

            SaveChildControls()
            PopulateChildControls()
            Save()
            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            Populate()
            'RaiseEvent RefreshBaseCoverage()
        End If
    End Sub

    Private Sub RemoveItem(rowNumber As Integer, form As String) Handles ctlJewelry.RemoveFarmItem, ctlJewelInVault.RemoveFarmItem, ctlAntiquesBreak.RemoveFarmItem,
        ctlAntiquesNoBreak.RemoveFarmItem, ctlCollectBreak.RemoveFarmItem, ctlCollectNoBreak.RemoveFarmItem, ctlCameras.RemoveFarmItem, ctlComputers.RemoveFarmItem,
        ctlArtsBreak.RemoveFarmItem, ctlArtsNoBreak.RemoveFarmItem, ctlFurs.RemoveFarmItem, ctlGuns.RemoveFarmItem, ctlHearing.RemoveFarmItem, ctlIrrigationNamed.RemoveFarmItem,
        ctlIrrigationSpecial.RemoveFarmItem, ctlRadioCB.RemoveFarmItem, ctlRadiosFM.RemoveFarmItem, ctlReproductiveNamed.RemoveFarmItem, ctlReproductiveSpecial.RemoveFarmItem,
        ctlMobile.RemoveFarmItem, ctlFarmMachineSched.RemoveFarmItem, ctlGarden.RemoveFarmItem, ctlGolfers.RemoveFarmItem, ctlSilverware.RemoveFarmItem, ctlTools.RemoveFarmItem,
        ctlMusicInstr.RemoveFarmItem, ctlBikeList.RemoveFarmItem, CtlCoinsList.RemoveFarmItem, ctlSportsEquipment.RemoveFarmItem, ctlMedicalItemsAndEquipment.RemoveFarmItem, ctlGraveMarkers.RemoveFarmItem
        If MyLocation IsNot Nothing And MyLocation.InlandMarines IsNot Nothing Then
            Dim imTotalInt As Integer = 0

            Select Case form
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry
                    Dim imJewelryList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry)
                    Dim imJewelry As QuickQuoteInlandMarine = imJewelryList(rowNumber)

                    MyLocation.InlandMarines.Remove(imJewelry)

                    If imJewelryList.Count <= 1 Then
                        chkIMJewelry.Checked = False
                        chkIMJewelry.Enabled = True
                        dvIMJewelryLimit.Attributes.Add("style", "display:none;")
                    End If

                    If imJewelryList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault
                    Dim jewelryInVaultList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault)
                    Dim jewelryInVault As QuickQuoteInlandMarine = jewelryInVaultList(rowNumber)

                    MyLocation.InlandMarines.Remove(jewelryInVault)

                    If jewelryInVaultList.Count <= 1 Then
                        chkJewelInVault.Checked = False
                        chkJewelInVault.Enabled = True
                        dvJewelInVaultLimit.Attributes.Add("style", "display:none;")
                    End If

                    If jewelryInVaultList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Bicycles ' added 3-21-16 Matt for Comparative Rater project
                    Dim bicycleList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Bicycles)
                    Dim bicycle As QuickQuoteInlandMarine = bicycleList(rowNumber)

                    MyLocation.InlandMarines.Remove(bicycle)

                    If bicycleList.Count <= 1 Then
                        chkBike.Checked = False
                        chkBike.Enabled = True
                        dvBikeLimit.Attributes.Add("style", "display:none;")
                    End If

                    If bicycleList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If


                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Coins ' added 3-21-16 Matt for Comparative Rater project
                    Dim coinList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Coins)
                    Dim coin As QuickQuoteInlandMarine = coinList(rowNumber)

                    MyLocation.InlandMarines.Remove(coin)

                    If coinList.Count <= 1 Then
                        chkCoins.Checked = False
                        chkCoins.Enabled = True
                        dvCoinLimit.Attributes.Add("style", "display:none;")
                    End If

                    If coinList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If


                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithBreakage
                    Dim imList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithBreakage)
                    Dim imItem As QuickQuoteInlandMarine = imList(rowNumber)

                    MyLocation.InlandMarines.Remove(imItem)

                    If imList.Count <= 1 Then
                        chkAntiquesBreak.Checked = False
                        chkAntiquesBreak.Enabled = True
                        dvAntiquesBreakLimit.Attributes.Add("style", "display:none;")
                    End If

                    If imList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithoutBreakage
                    Dim imList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithoutBreakage)
                    Dim imItem As QuickQuoteInlandMarine = imList(rowNumber)

                    MyLocation.InlandMarines.Remove(imItem)

                    If imList.Count <= 1 Then
                        chkAntiquesNoBreak.Checked = False
                        chkAntiquesNoBreak.Enabled = True
                        dvAntiquesNoBreakLimit.Attributes.Add("style", "display:none;")
                    End If

                    If imList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithBreakage
                    Dim imList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithBreakage)
                    Dim imItem As QuickQuoteInlandMarine = imList(rowNumber)

                    MyLocation.InlandMarines.Remove(imItem)

                    If imList.Count <= 1 Then
                        chkCollectBreak.Checked = False
                        chkCollectBreak.Enabled = True
                        dvCollectBreakLimit.Attributes.Add("style", "display:none;")
                    End If

                    If imList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithoutBreakage
                    Dim imList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithoutBreakage)
                    Dim imItem As QuickQuoteInlandMarine = imList(rowNumber)

                    MyLocation.InlandMarines.Remove(imItem)

                    If imList.Count <= 1 Then
                        chkCollectNoBreak.Checked = False
                        chkCollectNoBreak.Enabled = True
                        dvCollectNoBreakLimit.Attributes.Add("style", "display:none;")
                    End If

                    If imList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras
                    Dim CamerasList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras)
                    Dim Cameras As QuickQuoteInlandMarine = CamerasList(rowNumber)

                    MyLocation.InlandMarines.Remove(Cameras)

                    If CamerasList.Count <= 1 Then
                        chkCameras.Checked = False
                        chkCameras.Enabled = True
                        dvCamerasLimit.Attributes.Add("style", "display:none;")
                    End If

                    If CamerasList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer
                    Dim ComputersList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer)
                    Dim Computers As QuickQuoteInlandMarine = ComputersList(rowNumber)

                    MyLocation.InlandMarines.Remove(Computers)

                    If ComputersList.Count <= 1 Then
                        chkComputers.Checked = False
                        chkComputers.Enabled = True
                        dvComputersLimit.Attributes.Add("style", "display:none;")
                    End If

                    If ComputersList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled
                    Dim farmMachineryList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled)
                    Dim farmMachinery As QuickQuoteInlandMarine = farmMachineryList(rowNumber)

                    MyLocation.InlandMarines.Remove(farmMachinery)

                    If farmMachineryList.Count <= 1 Then
                        chkFarmMachineSched.Checked = False
                        chkFarmMachineSched.Enabled = True
                        dvFarmMachineSchedLimit.Attributes.Add("style", "display:none;")
                    End If

                    If farmMachineryList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage
                    Dim ArtsBreakList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage)
                    Dim ArtsBreak As QuickQuoteInlandMarine = ArtsBreakList(rowNumber)

                    MyLocation.InlandMarines.Remove(ArtsBreak)

                    If ArtsBreakList.Count <= 1 Then
                        chkFABreak.Checked = False
                        chkFABreak.Enabled = True
                        dvFABreakLimit.Attributes.Add("style", "display:none;")
                    End If

                    If ArtsBreakList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage
                    Dim ArtsNoBreakList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage)
                    Dim ArtsNoBreak As QuickQuoteInlandMarine = ArtsNoBreakList(rowNumber)

                    MyLocation.InlandMarines.Remove(ArtsNoBreak)

                    If ArtsNoBreakList.Count <= 1 Then
                        chkFANoBreak.Checked = False
                        chkFANoBreak.Enabled = True
                        dvFANoBreakLimit.Attributes.Add("style", "display:none;")
                    End If

                    If ArtsNoBreakList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs
                    Dim FursList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs)
                    Dim Furs As QuickQuoteInlandMarine = FursList(rowNumber)

                    MyLocation.InlandMarines.Remove(Furs)

                    If FursList.Count <= 1 Then
                        chkFurs.Checked = False
                        chkFurs.Enabled = True
                        dvFursLimit.Attributes.Add("style", "display:none;")
                    End If

                    If FursList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors
                    Dim gardenList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors)
                    Dim garden As QuickQuoteInlandMarine = gardenList(rowNumber)

                    MyLocation.InlandMarines.Remove(garden)

                    If gardenList.Count <= 1 Then
                        chkGarden.Checked = False
                        chkGarden.Enabled = True
                        dvGardenLimit.Attributes.Add("style", "display:none;")
                    End If

                    If gardenList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf
                    Dim golfersList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf)
                    Dim golfers As QuickQuoteInlandMarine = golfersList(rowNumber)

                    MyLocation.InlandMarines.Remove(golfers)

                    If golfersList.Count <= 1 Then
                        chkGolfers.Checked = False
                        chkGolfers.Enabled = True
                        dvGolfersLimit.Attributes.Add("style", "display:none;")
                    End If

                    If golfersList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns
                    Dim GunsList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns)
                    Dim Guns As QuickQuoteInlandMarine = GunsList(rowNumber)

                    MyLocation.InlandMarines.Remove(Guns)

                    If GunsList.Count <= 1 Then
                        chkGuns.Checked = False
                        chkGuns.Enabled = True
                        dvGunsLimit.Attributes.Add("style", "display:none;")
                    End If

                    If GunsList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids
                    Dim HearingList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids)
                    Dim Hearing As QuickQuoteInlandMarine = HearingList(rowNumber)

                    MyLocation.InlandMarines.Remove(Hearing)

                    If HearingList.Count <= 1 Then
                        chkHearing.Checked = False
                        chkHearing.Enabled = True
                        dvHearingLimit.Attributes.Add("style", "display:none;")
                    End If

                    If HearingList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I
                    Dim silverwareList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I)
                    Dim silverware As QuickQuoteInlandMarine = silverwareList(rowNumber)

                    MyLocation.InlandMarines.Remove(silverware)

                    If silverwareList.Count <= 1 Then
                        chkSilverware.Checked = False
                        chkSilverware.Enabled = True
                        dvSilverwareLimit.Attributes.Add("style", "display:none;")
                    End If

                    If silverwareList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.MedicalItemsAndEquipment
                    Dim medicalItemsAndEquipmentList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.MedicalItemsAndEquipment)
                    Dim medicalItemsAndEquipment As QuickQuoteInlandMarine = medicalItemsAndEquipmentList(rowNumber)

                    MyLocation.InlandMarines.Remove(medicalItemsAndEquipment)

                    If medicalItemsAndEquipmentList.Count <= 1 Then
                        chkMedicalItemsAndEquipment.Checked = False
                        chkMedicalItemsAndEquipment.Enabled = True
                        dvMedicalItemsAndEquipmentLimit.Attributes.Add("style", "display:none;")
                    End If

                    If medicalItemsAndEquipmentList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineSportsEquipment
                    Dim sportsEquipmentList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineSportsEquipment)
                    Dim sportsEquipment As QuickQuoteInlandMarine = sportsEquipmentList(rowNumber)

                    MyLocation.InlandMarines.Remove(sportsEquipment)

                    If sportsEquipmentList.Count <= 1 Then
                        chkSportsEquipment.Checked = False
                        chkSportsEquipment.Enabled = True
                        dvSportsEquipmentLimit.Attributes.Add("style", "display:none;")
                    End If

                    If sportsEquipmentList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentNamedPerils
                    Dim imList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentNamedPerils)
                    Dim imItem As QuickQuoteInlandMarine = imList(rowNumber)

                    MyLocation.InlandMarines.Remove(imItem)

                    If imList.Count <= 1 Then
                        chkIrrigationNamed.Checked = False
                        chkIrrigationNamed.Enabled = True
                        dvIrrigationNamedLimit.Attributes.Add("style", "display:none;")
                    End If

                    If imList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentSpecialCoverage
                    Dim imList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentSpecialCoverage)
                    Dim imItem As QuickQuoteInlandMarine = imList(rowNumber)

                    MyLocation.InlandMarines.Remove(imItem)

                    If imList.Count <= 1 Then
                        chkIrrigationSpecial.Checked = False
                        chkIrrigationSpecial.Enabled = True
                        dvIrrigationSpecialLimit.Attributes.Add("style", "display:none;")
                    End If

                    If imList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_CB
                    Dim imList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_CB)
                    Dim imItem As QuickQuoteInlandMarine = imList(rowNumber)

                    MyLocation.InlandMarines.Remove(imItem)

                    If imList.Count <= 1 Then
                        chkRadioCB.Checked = False
                        chkRadioCB.Enabled = True
                        dvRadioCBLimit.Attributes.Add("style", "display:none;")
                    End If

                    If imList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_FM
                    Dim imList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_FM)
                    Dim imItem As QuickQuoteInlandMarine = imList(rowNumber)

                    MyLocation.InlandMarines.Remove(imItem)

                    If imList.Count <= 1 Then
                        chkRadiosFM.Checked = False
                        chkRadiosFM.Enabled = True
                        dvRadiosFMLimit.Attributes.Add("style", "display:none;")
                    End If

                    If imList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsNamedPerils
                    Dim imList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsNamedPerils)
                    Dim imItem As QuickQuoteInlandMarine = imList(rowNumber)

                    MyLocation.InlandMarines.Remove(imItem)

                    If imList.Count <= 1 Then
                        chkReproductiveNamed.Checked = False
                        chkReproductiveNamed.Enabled = True
                        dvReproductiveNamedLimit.Attributes.Add("style", "display:none;")
                    End If

                    If imList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsSpecialCoverage
                    Dim imList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsSpecialCoverage)
                    Dim imItem As QuickQuoteInlandMarine = imList(rowNumber)

                    MyLocation.InlandMarines.Remove(imItem)

                    If imList.Count <= 1 Then
                        chkReproductiveSpecial.Checked = False
                        chkReproductiveSpecial.Enabled = True
                        dvReproductiveSpecialLimit.Attributes.Add("style", "display:none;")
                    End If

                    If imList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile
                    Dim MobileList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile)
                    Dim Mobile As QuickQuoteInlandMarine = MobileList(rowNumber)

                    MyLocation.InlandMarines.Remove(Mobile)

                    If MobileList.Count <= 1 Then
                        chkMobile.Checked = False
                        chkMobile.Enabled = True
                        dvMobileLimit.Attributes.Add("style", "display:none;")
                    End If

                    If MobileList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment
                    Dim toolsList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment)
                    Dim tools As QuickQuoteInlandMarine = toolsList(rowNumber)

                    MyLocation.InlandMarines.Remove(tools)

                    If toolsList.Count <= 1 Then
                        chkTools.Checked = False
                        chkTools.Enabled = True
                        dvToolsLimit.Attributes.Add("style", "display:none;")
                    End If

                    If toolsList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If

                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional
                    Dim musicList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional)
                    Dim music As QuickQuoteInlandMarine = musicList(rowNumber)

                    MyLocation.InlandMarines.Remove(music)

                    If musicList.Count <= 1 Then
                        chkMusic.Checked = False
                        chkMusic.Enabled = True
                        dvMusicLimit.Attributes.Add("style", "display:none;")
                    End If

                    If musicList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GraveMarkers
                    Dim graveMarketsList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GraveMarkers)
                    Dim graveMarkets As QuickQuoteInlandMarine = graveMarketsList(rowNumber)

                    MyLocation.InlandMarines.Remove(graveMarkets)

                    If graveMarketsList.Count <= 1 Then
                        chkGraveMarkers.Checked = False
                        chkGraveMarkers.Enabled = True
                        dvGraveMarkersLimit.Attributes.Add("style", "display:none;")
                    End If

                    If graveMarketsList.Count <= 1 Then
                        imTotalInt -= 1

                        If imTotalInt < 0 Then
                            imTotalInt = 0
                        End If
                    Else
                        imTotalInt += 1
                    End If
            End Select

            lblIMChosen.Text = imTotalInt
            'hiddenSelectedIMCoverage.Value = imTotalInt.ToString
            'RaiseEvent RecalculateCoverageTotal(hiddenSelectedIMCoverage.Value)
            PopulateChildControls()
            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            Populate()
        End If
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        If MyLocation IsNot Nothing Then
            If MyLocation.InlandMarines IsNot Nothing Then
                MyLocation.InlandMarines = New List(Of QuickQuoteInlandMarine)
            End If

            dvIMJewelryLimit.Attributes.Add("style", "display:none;")
            chkIMJewelry.Enabled = True
            chkIMJewelry.Checked = False
            ctlJewelry.ClearControl()

            dvJewelInVaultLimit.Attributes.Add("style", "display:none;")
            chkJewelInVault.Enabled = True
            chkJewelInVault.Checked = False
            ctlJewelInVault.ClearControl()

            dvAntiquesBreakLimit.Attributes.Add("style", "display:none;")
            chkAntiquesBreak.Enabled = True
            chkAntiquesBreak.Checked = False
            ctlAntiquesBreak.ClearControl()

            dvBikeLimit.Attributes.Add("style", "display:none;") ' added 3-21-16 Matt for Comparative Rater project
            chkBike.Enabled = True
            chkBike.Checked = False
            ctlBikeList.ClearControl()

            dvCoinLimit.Attributes.Add("style", "display:none;") ' added 3-21-16 Matt for Comparative Rater project
            chkCoins.Enabled = True
            chkCoins.Checked = False
            CtlCoinsList.ClearControl()

            dvAntiquesNoBreakLimit.Attributes.Add("style", "display:none;")
            chkAntiquesNoBreak.Enabled = True
            chkAntiquesNoBreak.Checked = False
            ctlAntiquesNoBreak.ClearControl()

            dvCollectBreakLimit.Attributes.Add("style", "display:none;")
            chkCollectBreak.Enabled = True
            chkCollectBreak.Checked = False
            ctlCollectBreak.ClearControl()

            dvCollectNoBreakLimit.Attributes.Add("style", "display:none;")
            chkCollectNoBreak.Enabled = True
            chkCollectNoBreak.Checked = False
            ctlCollectNoBreak.ClearControl()

            dvCamerasLimit.Attributes.Add("style", "display:none;")
            chkCameras.Enabled = True
            chkCameras.Checked = False
            ctlCameras.ClearControl()

            dvComputersLimit.Attributes.Add("style", "display:none;")
            chkComputers.Enabled = True
            chkComputers.Checked = False
            ctlComputers.ClearControl()

            dvFarmMachineSchedLimit.Attributes.Add("style", "display:none;")
            chkFarmMachineSched.Enabled = True
            chkFarmMachineSched.Checked = False
            ctlFarmMachineSched.ClearControl()

            dvFABreakLimit.Attributes.Add("style", "display:none;")
            chkFABreak.Enabled = True
            chkFABreak.Checked = False
            ctlArtsBreak.ClearControl()

            dvFANoBreakLimit.Attributes.Add("style", "display:none;")
            chkFANoBreak.Enabled = True
            chkFANoBreak.Checked = False
            ctlArtsNoBreak.ClearControl()

            dvFursLimit.Attributes.Add("style", "display:none;")
            chkFurs.Enabled = True
            chkFurs.Checked = False
            ctlFurs.ClearControl()

            dvGardenLimit.Attributes.Add("style", "display:none;")
            chkGarden.Enabled = True
            chkGarden.Checked = False
            ctlGarden.ClearControl()

            dvGolfersLimit.Attributes.Add("style", "display:none;")
            chkGolfers.Enabled = True
            chkGolfers.Checked = False
            ctlGolfers.ClearControl()

            dvGunsLimit.Attributes.Add("style", "display:none;")
            chkGuns.Enabled = True
            chkGuns.Checked = False
            ctlGuns.ClearControl()

            dvHearingLimit.Attributes.Add("style", "display:none;")
            chkHearing.Enabled = True
            chkHearing.Checked = False
            ctlHearing.ClearControl()

            dvMedicalItemsAndEquipmentLimit.Attributes.Add("style", "display:none;")
            chkMedicalItemsAndEquipment.Enabled = True
            chkMedicalItemsAndEquipment.Checked = False
            ctlMedicalItemsAndEquipment.ClearControl()

            dvSilverwareLimit.Attributes.Add("style", "display:none;")
            chkSilverware.Enabled = True
            chkSilverware.Checked = False
            ctlSilverware.ClearControl()

            dvSportsEquipmentLimit.Attributes.Add("style", "display:none;")
            chkSportsEquipment.Enabled = True
            chkSportsEquipment.Checked = False
            ctlSportsEquipment.ClearControl()

            dvIrrigationNamedLimit.Attributes.Add("style", "display:none;")
            chkIrrigationNamed.Enabled = True
            chkIrrigationNamed.Checked = False
            ctlIrrigationNamed.ClearControl()

            dvIrrigationSpecialLimit.Attributes.Add("style", "display:none;")
            chkIrrigationSpecial.Enabled = True
            chkIrrigationSpecial.Checked = False
            ctlIrrigationSpecial.ClearControl()

            dvRadioCBLimit.Attributes.Add("style", "display:none;")
            chkRadioCB.Enabled = True
            chkRadioCB.Checked = False
            ctlRadioCB.ClearControl()

            dvRadiosFMLimit.Attributes.Add("style", "display:none;")
            chkRadiosFM.Enabled = True
            chkRadiosFM.Checked = False
            ctlRadiosFM.ClearControl()

            dvReproductiveNamedLimit.Attributes.Add("style", "display:none;")
            chkReproductiveNamed.Enabled = True
            chkReproductiveNamed.Checked = False
            ctlReproductiveNamed.ClearControl()

            dvReproductiveSpecialLimit.Attributes.Add("style", "display:none;")
            chkReproductiveSpecial.Enabled = True
            chkReproductiveSpecial.Checked = False
            ctlReproductiveSpecial.ClearControl()

            dvMobileLimit.Attributes.Add("style", "display:none;")
            chkMobile.Enabled = True
            chkMobile.Checked = False
            ctlMobile.ClearControl()

            dvToolsLimit.Attributes.Add("style", "display:none;")
            chkTools.Enabled = True
            chkTools.Checked = False
            ctlTools.ClearControl()

            dvMusicLimit.Attributes.Add("style", "display:none;")
            chkMusic.Enabled = True
            chkMusic.Checked = False
            ctlMusicInstr.ClearControl()

            dvGraveMarkersLimit.Attributes.Add("style", "display:none;")
            chkGraveMarkers.Enabled = True
            chkGraveMarkers.Checked = False
            ctlGraveMarkers.ClearControl()

            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            lblIMChosen.Text = "0"
            'RaiseEvent RecalculateCoverageTotal("0")
        End If
    End Sub

    Protected Sub chkJewelInVault_CheckedChanged(sender As Object, e As EventArgs) Handles chkJewelInVault.CheckedChanged, lnkAddJewleryInVault.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("JewelryVault")
            dvJewelInVaultLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkIMJewelry_CheckedChanged(sender As Object, e As EventArgs) Handles chkIMJewelry.CheckedChanged, lnkAddIMJewelryLimit.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("IMJewelry")
            dvIMJewelryLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkBike_CheckedChanged(sender As Object, e As EventArgs) Handles chkBike.CheckedChanged, lnkAddBike.Click ' added 3-21-16 Matt for Comparative Rater project
        If MyLocation IsNot Nothing Then
            AddNewItem("IMBicycle")
            dvBikeLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkCoins_CheckedChanged(sender As Object, e As EventArgs) Handles chkCoins.CheckedChanged, lnkAddCoins.Click ' added 3-21-16 Matt for Comparative Rater project
        If MyLocation IsNot Nothing Then
            AddNewItem("IMCoin")
            dvCoinLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkAntiquesBreak_CheckedChanged(sender As Object, e As EventArgs) Handles chkAntiquesBreak.CheckedChanged, lnkAddAntiquesBreak.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("AntiqueBreak")
            dvAntiquesBreakLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkAntiquesNoBreak_CheckedChanged(sender As Object, e As EventArgs) Handles chkAntiquesNoBreak.CheckedChanged, lnkAddAntiquesNoBreak.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("AntiqueNoBreak")
            dvAntiquesNoBreakLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkCollectBreak_CheckedChanged(sender As Object, e As EventArgs) Handles chkCollectBreak.CheckedChanged, lnkAddCollectBreak.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("CollectBreak")
            dvCollectBreakLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkCollectNoBreak_CheckedChanged(sender As Object, e As EventArgs) Handles chkCollectNoBreak.CheckedChanged, lnkAddCollectNoBreak.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("CollectNoBreak")
            dvCollectNoBreakLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkCameras_CheckedChanged(sender As Object, e As EventArgs) Handles chkCameras.CheckedChanged, lnkAddCameras.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("Cameras")
            dvCamerasLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkComputers_CheckedChanged(sender As Object, e As EventArgs) Handles chkComputers.CheckedChanged, lnkAddComputers.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("Computers")
            dvComputersLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkFarmMachineSched_CheckedChanged(sender As Object, e As EventArgs) Handles chkFarmMachineSched.CheckedChanged, lnkAddFarmMachineSched.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("FarmMachinery")
            dvFarmMachineSchedLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkFABreak_CheckedChanged(sender As Object, e As EventArgs) Handles chkFABreak.CheckedChanged, lnkAddFABreak.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("ArtsBreak")
            dvFABreakLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkFANoBreak_CheckedChanged(sender As Object, e As EventArgs) Handles chkFANoBreak.CheckedChanged, lnkAddFANoBreak.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("ArtsNoBreak")
            dvFANoBreakLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkFurs_CheckedChanged(sender As Object, e As EventArgs) Handles chkFurs.CheckedChanged, lnkAddFurs.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("Furs")
            dvFursLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkGarden_CheckedChanged(sender As Object, e As EventArgs) Handles chkGarden.CheckedChanged, lnkAddGarden.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("Garden")
            dvGardenLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkGolfers_CheckedChanged(sender As Object, e As EventArgs) Handles chkGolfers.CheckedChanged, lnkAddGolfers.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("Golfers")
            dvGolfersLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkGuns_CheckedChanged(sender As Object, e As EventArgs) Handles chkGuns.CheckedChanged, lnkAddGuns.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("Guns")
            dvGunsLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkHearing_CheckedChanged(sender As Object, e As EventArgs) Handles chkHearing.CheckedChanged, lnkAddHearing.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("Hearing")
            dvHearingLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkMedicalItemsAndEquipment_CheckedChanged(sender As Object, e As EventArgs) Handles chkMedicalItemsAndEquipment.CheckedChanged, lnkAddMedicalItemsAndEquipment.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("MedicalItemsAndEquipment")
            dvMedicalItemsAndEquipmentLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkSilverware_CheckedChanged(sender As Object, e As EventArgs) Handles chkSilverware.CheckedChanged, lnkAddSilverware.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("Silverware")
            dvSilverwareLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkSportsEquipment_CheckedChanged(sender As Object, e As EventArgs) Handles chkSportsEquipment.CheckedChanged, lnkAddSportsEquipment.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("SportsEquipment")
            dvSportsEquipmentLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkIrrigationNamed_CheckedChanged(sender As Object, e As EventArgs) Handles chkIrrigationNamed.CheckedChanged, lnkAddIrrigationNamed.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("IrrigateNamed")
            dvIrrigationNamedLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkIrrigationSpecial_CheckedChanged(sender As Object, e As EventArgs) Handles chkIrrigationSpecial.CheckedChanged, lnkAddIrrigationSpecial.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("IrrigateSpecial")
            dvIrrigationSpecialLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkRadioCB_CheckedChanged(sender As Object, e As EventArgs) Handles chkRadioCB.CheckedChanged, lnkAddRadioCB.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("CB")
            dvRadioCBLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkRadiosFM_CheckedChanged(sender As Object, e As EventArgs) Handles chkRadiosFM.CheckedChanged, lnkAddRadiosFM.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("FM")
            dvRadiosFMLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkReproductiveNamed_CheckedChanged(sender As Object, e As EventArgs) Handles chkReproductiveNamed.CheckedChanged, lnkAddReproductiveNamed.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("ReproduceNamed")
            dvReproductiveNamedLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkReproductiveSpecial_CheckedChanged(sender As Object, e As EventArgs) Handles chkReproductiveSpecial.CheckedChanged, lnkAddReproductiveSpecial.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("ReproduceSpecial")
            dvReproductiveSpecialLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkMobile_CheckedChanged(sender As Object, e As EventArgs) Handles chkMobile.CheckedChanged, lnkAddMobile.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("Mobile")
            dvMobileLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkTools_CheckedChanged(sender As Object, e As EventArgs) Handles chkTools.CheckedChanged, lnkAddTools.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("Tools")
            dvToolsLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub chkMusic_CheckedChanged(sender As Object, e As EventArgs) Handles chkMusic.CheckedChanged, lnkAddMusic.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("Music")
            dvMusicLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub
    Protected Sub chkGraveMarkers_CheckedChanged(sender As Object, e As EventArgs) Handles chkGraveMarkers.CheckedChanged, lnkAddGraveMarkers.Click
        If MyLocation IsNot Nothing Then
            AddNewItem("GraveMarkers")
            dvGraveMarkersLimit.Attributes.Add("style", "display:block;")
        End If
    End Sub

    Protected Sub lnkClearInland_Click(sender As Object, e As EventArgs) Handles lnkClearInland.Click
        ClearControl()
    End Sub

    Protected Sub btnSaveIM_Click(sender As Object, e As EventArgs) Handles btnSaveIM.Click
        Save_FireSaveEvent()
    End Sub

    Protected Sub btnRateIM_Click(sender As Object, e As EventArgs) Handles btnRateIM.Click
        RaiseEvent CommonRateIM()
    End Sub

    Protected Sub lnkSaveinland_Click(sender As Object, e As EventArgs) Handles lnkSaveinland.Click
        Save_FireSaveEvent()
    End Sub


End Class