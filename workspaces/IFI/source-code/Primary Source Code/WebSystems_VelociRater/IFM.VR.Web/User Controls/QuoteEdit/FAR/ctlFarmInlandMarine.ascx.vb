Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.FarmLines

Public Class ctlFarmInlandMarine
    Inherits VRControlBase

    Public Event RecalculateCoverageTotal(optionalTotal As String)
    Public Event RefreshBaseCoverage()

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MainAccordionDivId = dvInlandMarineInput.ClientID
        If Not IsPostBack Then
            LoadStaticData()
            Populate()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.CreateAccordion(MainAccordionDivId, hiddenIM, "0", False)
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
                    ctlJewelInVault.Populate()
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
                    dvCamerasLimit.Attributes.Add("style", "display:block;")
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
            End If

            If recalculateCount Then
                hiddenSelectedIMCoverage.Value = imTotalInt.ToString
                RaiseEvent RecalculateCoverageTotal(hiddenSelectedIMCoverage.Value)
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = "Inland Marine"
        Dim divInlandMarine As String = dvInlandMarineInput.ClientID
        Dim gunsError As Boolean = False

        'Dim valList = InlandMarineListValidator.ValidateHOMInlandMarine(Me.Quote, valArgs.ValidationType)

        'If valList.Any() Then
        '    For Each v In valList
        '        ' *************************
        '        ' Optional Policy Coverages
        '        ' *************************
        '        Select Case v.FieldId
        '            Case InlandMarineListValidator.Combined_Jewelry_Exceeded_30000
        '                ValidationHelper.Val_BindValidationItemToControl(lblIMJewelry, v, divInlandMarine, "0")
        '            Case InlandMarineListValidator.Combined_JewelryVault_Exceeded_30000
        '                ValidationHelper.Val_BindValidationItemToControl(lblJewelInVault, v, divInlandMarine, "0")
        '            Case InlandMarineListValidator.Combined_ArtsBreak_Exceeded_30000
        '                ValidationHelper.Val_BindValidationItemToControl(lblFABreak, v, divInlandMarine, "0")
        '            Case InlandMarineListValidator.Combined_ArtsNoBreak_Exceeded_30000
        '                ValidationHelper.Val_BindValidationItemToControl(lblFANoBreak, v, divInlandMarine, "0")
        '            Case InlandMarineListValidator.Combined_Furs_Exceeded_30000
        '                ValidationHelper.Val_BindValidationItemToControl(lblFurs, v, divInlandMarine, "0")
        '            Case InlandMarineListValidator.Combined_Guns_Limit_Exceeded
        '                ValidationHelper.Val_BindValidationItemToControl(lblGuns, v, divInlandMarine, "0")
        '        End Select
        '    Next
        'End If

        'If gunsError Then
        '    lblGuns.BorderColor = Drawing.Color.Red
        '    lblGuns.BorderStyle = BorderStyle.Solid
        '    lblGuns.BorderWidth = 1
        'Else
        '    lblGuns.BorderStyle = BorderStyle.None
        'End If

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
        dvIMJewelryLimit.Attributes.Add("style", "display:none;")
        dvJewelInVaultLimit.Attributes.Add("style", "display:none;")
        dvAntiquesBreakLimit.Attributes.Add("style", "display:none;")
        dvAntiquesNoBreakLimit.Attributes.Add("style", "display:none;")
        dvCollectBreakLimit.Attributes.Add("style", "display:none;")
        dvCollectNoBreakLimit.Attributes.Add("style", "display:none;")
        dvCamerasLimit.Attributes.Add("style", "display:none;")
        dvComputersLimit.Attributes.Add("style", "display:none;")
        dvFABreakLimit.Attributes.Add("style", "display:none;")
        dvFANoBreakLimit.Attributes.Add("style", "display:none;")
        dvFursLimit.Attributes.Add("style", "display:none;")
        dvGunsLimit.Attributes.Add("style", "display:none;")
        dvHearingLimit.Attributes.Add("style", "display:none;")
        dvIrrigationNamedLimit.Attributes.Add("style", "display:none;")
        dvIrrigationSpecialLimit.Attributes.Add("style", "display:none;")
        dvRadioCBLimit.Attributes.Add("style", "display:none;")
        dvRadiosFMLimit.Attributes.Add("style", "display:none;")
        dvReproductiveNamedLimit.Attributes.Add("style", "display:none;")
        dvReproductiveSpecialLimit.Attributes.Add("style", "display:none;")
        dvMobileLimit.Attributes.Add("style", "display:none;")

        hiddenSelectedIMCoverage.Value = imTotalInt.ToString
        RaiseEvent RecalculateCoverageTotal(hiddenSelectedIMCoverage.Value)
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
                    chkComputers.Enabled = False
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
            End Select

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
        ctlMobile.RemoveFarmItem
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
            End Select

            hiddenSelectedIMCoverage.Value = imTotalInt.ToString
            RaiseEvent RecalculateCoverageTotal(hiddenSelectedIMCoverage.Value)
            PopulateChildControls()
            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            Populate()
        End If
    End Sub

    Public Sub ClearAllCoverages()
        If MyLocation IsNot Nothing Then
            If MyLocation.InlandMarines IsNot Nothing Then
                MyLocation.InlandMarines = New List(Of QuickQuoteInlandMarine)
            End If

            dvIMJewelryLimit.Attributes.Add("style", "display:none;")
            chkIMJewelry.Enabled = True
            chkIMJewelry.Checked = False

            dvJewelInVaultLimit.Attributes.Add("style", "display:none;")
            chkJewelInVault.Enabled = True
            chkJewelInVault.Checked = False

            dvAntiquesBreakLimit.Attributes.Add("style", "display:none;")
            chkAntiquesBreak.Enabled = True
            chkAntiquesBreak.Checked = False

            dvAntiquesNoBreakLimit.Attributes.Add("style", "display:none;")
            chkAntiquesNoBreak.Enabled = True
            chkAntiquesNoBreak.Checked = False

            dvCollectBreakLimit.Attributes.Add("style", "display:none;")
            chkCollectNoBreak.Enabled = True
            chkCollectNoBreak.Checked = False

            dvCollectNoBreakLimit.Attributes.Add("style", "display:none;")
            chkCameras.Enabled = True
            chkCameras.Checked = False

            dvCamerasLimit.Attributes.Add("style", "display:none;")
            chkCameras.Enabled = True
            chkCameras.Checked = False

            dvComputersLimit.Attributes.Add("style", "display:none;")
            chkComputers.Enabled = True
            chkComputers.Checked = False

            dvFABreakLimit.Attributes.Add("style", "display:none;")
            chkFABreak.Enabled = True
            chkFABreak.Checked = False

            dvFANoBreakLimit.Attributes.Add("style", "display:none;")
            chkFANoBreak.Enabled = True
            chkFANoBreak.Checked = False

            dvFursLimit.Attributes.Add("style", "display:none;")
            chkFurs.Enabled = True
            chkFurs.Checked = False

            dvGunsLimit.Attributes.Add("style", "display:none;")
            chkGuns.Enabled = True
            chkGuns.Checked = False

            dvHearingLimit.Attributes.Add("style", "display:none;")
            chkHearing.Enabled = True
            chkHearing.Checked = False

            dvIrrigationNamedLimit.Attributes.Add("style", "display:none;")
            chkIrrigationNamed.Enabled = True
            chkIrrigationNamed.Checked = False

            dvIrrigationSpecialLimit.Attributes.Add("style", "display:none;")
            chkIrrigationSpecial.Enabled = True
            chkIrrigationSpecial.Checked = False

            dvRadioCBLimit.Attributes.Add("style", "display:none;")
            chkRadioCB.Enabled = True
            chkRadioCB.Checked = False

            dvRadiosFMLimit.Attributes.Add("style", "display:none;")
            chkRadiosFM.Enabled = True
            chkRadiosFM.Checked = False

            dvReproductiveNamedLimit.Attributes.Add("style", "display:none;")
            chkReproductiveNamed.Enabled = True
            chkReproductiveNamed.Checked = False

            dvReproductiveSpecialLimit.Attributes.Add("style", "display:none;")
            chkReproductiveSpecial.Enabled = True
            chkReproductiveSpecial.Checked = False

            dvMobileLimit.Attributes.Add("style", "display:none;")
            chkMobile.Enabled = True
            chkMobile.Checked = False

            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            RaiseEvent RecalculateCoverageTotal("0")
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
End Class