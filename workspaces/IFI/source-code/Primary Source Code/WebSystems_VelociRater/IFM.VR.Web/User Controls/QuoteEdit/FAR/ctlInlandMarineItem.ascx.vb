'Imports QuickQuote.CommonObjects
'Imports QuickQuote.CommonMethods
'Imports IFM.VR.Web.Helpers
'Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM

'Public Class ctlInlandMarineItem
'    Inherits VRControlBase

'    Public Event RecalculateCoverageTotal(optionalTotal As String)
'    Public Event RefreshBaseCoverage()

'    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
'        Get
'            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
'                Return Me.Quote.Locations(0)
'            End If
'            Return Nothing
'        End Get
'    End Property

'    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
'        If Not IsPostBack Then
'            LoadStaticData()
'            Populate()
'        End If
'    End Sub

'    Public Overrides Sub AddScriptAlways()

'    End Sub

'    Public Overrides Sub AddScriptWhenRendered()

'    End Sub

'    Public Overrides Sub LoadStaticData()
'        SetDefaultValues()
'    End Sub

'    Public Overrides Sub Populate()
'        If MyLocation IsNot Nothing Then
'            Dim imTotalInt As Integer = 0
'            Dim recalculateCount As Boolean = False

'            If MyLocation.InlandMarines IsNot Nothing AndAlso MyLocation.InlandMarines.Count > 0 Then
'                ' Inland Marines Jewelry
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry).Count > 0 Then
'                    Dim imJewelryDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry)

'                    ctlIMJewelry.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry
'                    ctlIMJewelry.Populate()
'                    chkIMJewelry.Checked = True
'                    chkIMJewelry.Enabled = False
'                    dvIMJewelryLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Jewelry In Vault
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault).Count > 0 Then
'                    Dim jewelryInVaultDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault)

'                    ctlJewelryVault.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault
'                    ctlJewelryVault.Populate()
'                    chkJewelInVault.Checked = True
'                    chkJewelInVault.Enabled = False
'                    dvJewelInVaultLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Cameras
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras).Count > 0 Then
'                    Dim CamerasDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras)

'                    ctlCameras.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras
'                    ctlCameras.Populate()
'                    chkCameras.Checked = True
'                    chkCameras.Enabled = False
'                    dvCamerasLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Computers
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer).Count > 0 Then
'                    Dim ComputersDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer)

'                    ctlComputers.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer
'                    ctlComputers.Populate()
'                    chkComputers.Checked = True
'                    chkComputers.Enabled = False
'                    dvComputersLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Farm Machinery - Scheduled
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled).Count > 0 Then
'                    Dim FarmDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled)

'                    ctlFarm.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled
'                    ctlFarm.Populate()
'                    chkFarm.Checked = True
'                    chkFarm.Enabled = False
'                    dvFarmLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Fine Arts - with breakage coverage
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage).Count > 0 Then
'                    Dim ArtsBreakDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage)

'                    ctlArtsBreak.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage
'                    ctlArtsBreak.Populate()
'                    chkFABreak.Checked = True
'                    chkFABreak.Enabled = False
'                    dvFABreakLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Fine Arts - without breakage coverage
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage).Count > 0 Then
'                    Dim ArtsNoBreakDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage)

'                    ctlArtsNoBreak.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage
'                    ctlArtsNoBreak.Populate()
'                    chkFANoBreak.Checked = True
'                    chkFANoBreak.Enabled = False
'                    dvFANoBreakLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Furs
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs).Count > 0 Then
'                    Dim FursDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs)

'                    ctlFurs.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs
'                    ctlFurs.Populate()
'                    chkFurs.Checked = True
'                    chkFurs.Enabled = False
'                    dvFursLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Garden Tractors
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors).Count > 0 Then
'                    Dim TractorsDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors)

'                    ctlTractors.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors
'                    ctlTractors.Populate()
'                    chkGarden.Checked = True
'                    chkGarden.Enabled = False
'                    dvGardenLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Golfers Equipment
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf).Count > 0 Then
'                    Dim GolfersDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf)

'                    ctlGolfEquip.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf
'                    ctlGolfEquip.Populate()
'                    chkGolfers.Checked = True
'                    chkGolfers.Enabled = False
'                    dvGolfersLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Guns
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns).Count > 0 Then
'                    Dim GunsDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns)

'                    ctlGuns.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns
'                    ctlGuns.Populate()
'                    chkGuns.Checked = True
'                    chkGuns.Enabled = False
'                    dvGunsLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Hearing Aids
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids).Count > 0 Then
'                    Dim HearingDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids)

'                    ctlHearing.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids
'                    ctlHearing.Populate()
'                    chkHearing.Checked = True
'                    chkHearing.Enabled = False
'                    dvHearingLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Silverware
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I).Count > 0 Then
'                    Dim SilverwareDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I)

'                    ctlSilverware.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I
'                    ctlSilverware.Populate()
'                    chkSilverware.Checked = True
'                    chkSilverware.Enabled = False
'                    dvSilverwareLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Telephone - Car or Mobile
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile).Count > 0 Then
'                    Dim MobileDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile)

'                    ctlMobile.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile
'                    ctlMobile.Populate()
'                    chkMobile.Checked = True
'                    chkMobile.Enabled = False
'                    dvMobileLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Tools and Equipment
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment).Count > 0 Then
'                    Dim ToolsDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment)

'                    ctlTools.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment
'                    ctlTools.Populate()
'                    chkTools.Checked = True
'                    chkTools.Enabled = False
'                    dvToolsLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If

'                ' Musical Instruments (Non-Professional)
'                If MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional).Count > 0 Then
'                    Dim MusicDataSource As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional)

'                    ctlMusicInstr.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional
'                    ctlMusicInstr.Populate()
'                    chkMusic.Checked = True
'                    chkMusic.Enabled = False
'                    dvMusicLimit.Attributes.Add("style", "display:block;")
'                    imTotalInt += 1
'                    recalculateCount = True
'                End If
'            End If

'            If recalculateCount Then
'                hiddenSelectedIMCoverage.Value = imTotalInt.ToString
'                RaiseEvent RecalculateCoverageTotal(hiddenSelectedIMCoverage.Value)
'            End If
'        End If
'    End Sub

'    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
'        MyBase.ValidateControl(valArgs)
'        ValidationHelper.GroupName = "Inland Marine"
'        Dim divInlandMarine As String = dvInlandMarineInput.ClientID
'        Dim gunsError As Boolean = False

'        Dim valList = InlandMarineListValidator.ValidateHOMInlandMarine(Me.Quote, valArgs.ValidationType)

'        If valList.Any() Then
'            For Each v In valList
'                ' *************************
'                ' Optional Policy Coverages
'                ' *************************
'                Select Case v.FieldId
'                    Case InlandMarineListValidator.Combined_Jewelry_Exceeded_30000
'                        ValidationHelper.Val_BindValidationItemToControl(lblIMJewelry, v, divInlandMarine, "0")
'                    Case InlandMarineListValidator.Combined_JewelryVault_Exceeded_30000
'                        ValidationHelper.Val_BindValidationItemToControl(lblJewelInVault, v, divInlandMarine, "0")
'                    Case InlandMarineListValidator.Combined_ArtsBreak_Exceeded_30000
'                        ValidationHelper.Val_BindValidationItemToControl(lblFABreak, v, divInlandMarine, "0")
'                    Case InlandMarineListValidator.Combined_ArtsNoBreak_Exceeded_30000
'                        ValidationHelper.Val_BindValidationItemToControl(lblFANoBreak, v, divInlandMarine, "0")
'                    Case InlandMarineListValidator.Combined_Furs_Exceeded_30000
'                        ValidationHelper.Val_BindValidationItemToControl(lblFurs, v, divInlandMarine, "0")
'                    Case InlandMarineListValidator.Combined_Guns_Limit_Exceeded
'                        ValidationHelper.Val_BindValidationItemToControl(lblGuns, v, divInlandMarine, "0")
'                End Select
'            Next
'        End If

'        If gunsError Then
'            lblGuns.BorderColor = Drawing.Color.Red
'            lblGuns.BorderStyle = BorderStyle.Solid
'            lblGuns.BorderWidth = 1
'        Else
'            lblGuns.BorderStyle = BorderStyle.None
'        End If

'        ValidateChildControls(valArgs)
'    End Sub

'    Public Overrides Function Save() As Boolean
'        If MyLocation IsNot Nothing Then
'            Dim InlandMarineCoverage As QuickQuoteInlandMarine = Nothing

'            If MyLocation.InlandMarines Is Nothing Then
'                MyLocation.InlandMarines = New List(Of QuickQuoteInlandMarine)
'            End If

'            SaveChildControls()
'            Return True
'        End If

'        Return False
'    End Function

'    Private Sub SetDefaultValues()
'        Dim imTotalInt As Integer = 0
'        dvIMJewelryLimit.Attributes.Add("style", "display:none;")
'        dvJewelInVaultLimit.Attributes.Add("style", "display:none;")
'        dvCamerasLimit.Attributes.Add("style", "display:none;")
'        dvComputersLimit.Attributes.Add("style", "display:none;")
'        dvFarmLimit.Attributes.Add("style", "display:none;")
'        dvFABreakLimit.Attributes.Add("style", "display:none;")
'        dvFANoBreakLimit.Attributes.Add("style", "display:none;")
'        dvFursLimit.Attributes.Add("style", "display:none;")
'        dvGardenLimit.Attributes.Add("style", "display:none;")
'        dvGolfersLimit.Attributes.Add("style", "display:none;")
'        dvGunsLimit.Attributes.Add("style", "display:none;")
'        dvHearingLimit.Attributes.Add("style", "display:none;")
'        dvSilverwareLimit.Attributes.Add("style", "display:none;")
'        dvMobileLimit.Attributes.Add("style", "display:none;")
'        dvToolsLimit.Attributes.Add("style", "display:none;")
'        dvMusicLimit.Attributes.Add("style", "display:none;")

'        hiddenSelectedIMCoverage.Value = imTotalInt.ToString
'        RaiseEvent RecalculateCoverageTotal(hiddenSelectedIMCoverage.Value)
'    End Sub

'    Private Sub AddNewItem(form As String)
'        If MyLocation IsNot Nothing Then
'            If MyLocation.InlandMarines Is Nothing Then
'                MyLocation.InlandMarines = New List(Of QuickQuoteInlandMarine)()
'            End If

'            Dim newIMItem As New QuickQuoteInlandMarine()

'            Select Case form
'                Case "IMJewelry"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry
'                    newIMItem.Description = "Jewelry"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkIMJewelry.Enabled = False
'                Case "JewelryVault"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault
'                    newIMItem.Description = "Jewelry in Vault"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkJewelInVault.Enabled = False
'                Case "Cameras"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras
'                    newIMItem.Description = "Cameras"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkCameras.Enabled = False
'                Case "Computers"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer
'                    newIMItem.Description = "Computers"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkComputers.Enabled = False
'                Case "Farm"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled
'                    newIMItem.Description = "Farm Machinery - Scheduled"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkFarm.Enabled = False
'                Case "ArtsBreak"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage
'                    newIMItem.Description = "Fine Arts - with breakage coverage"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkFABreak.Enabled = False
'                Case "ArtsNoBreak"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage
'                    newIMItem.Description = "Fine Arts - without breakage coverage"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkFANoBreak.Enabled = False
'                Case "Tractors"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors
'                    newIMItem.Description = "Garden Tractors"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkGarden.Enabled = False
'                Case "Furs"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs
'                    newIMItem.Description = "Furs"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkFurs.Enabled = False
'                Case "GolfEquip"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf
'                    newIMItem.Description = "Golfers Equipment"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkGolfers.Enabled = False
'                Case "Guns"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns
'                    newIMItem.Description = "Guns"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkGuns.Enabled = False
'                Case "Hearing"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids
'                    newIMItem.Description = "Hearing Aids"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkHearing.Enabled = False
'                Case "Silverware"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I
'                    newIMItem.Description = "Silverware"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkSilverware.Enabled = False
'                Case "Mobile"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile
'                    newIMItem.Description = "Telephone - Car or Mobile"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkMobile.Enabled = False
'                Case "Tools"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment
'                    newIMItem.Description = "Tools and Equipment"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkTools.Enabled = False
'                Case "Music"
'                    newIMItem.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional
'                    newIMItem.Description = "Musical Instrument (Non-Professional)"
'                    MyLocation.InlandMarines.Add(newIMItem)
'                    chkMusic.Enabled = False
'            End Select

'            SaveChildControls()
'            PopulateChildControls()
'            Save()
'            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
'            Populate()
'            RaiseEvent RefreshBaseCoverage()
'        End If
'    End Sub

'    Private Sub RemoveItem(rowNumber As Integer, form As String) Handles ctlArtsBreak.RemoveItem, ctlArtsNoBreak.RemoveItem, ctlCameras.RemoveItem, ctlComputers.RemoveItem, ctlFarm.RemoveItem, ctlFurs.RemoveItem, ctlGolfEquip.RemoveItem, ctlGuns.RemoveItem, ctlHearing.RemoveItem, ctlIMJewelry.RemoveItem, ctlJewelryVault.RemoveItem, ctlMobile.RemoveItem, ctlSilverware.RemoveItem, ctlTools.RemoveItem, ctlTractors.RemoveItem, ctlMusicInstr.RemoveItem
'        If MyLocation IsNot Nothing And MyLocation.InlandMarines IsNot Nothing Then
'            Dim imTotalInt As Integer = 0

'            Select Case form
'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry
'                    Dim imJewelryList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry)
'                    Dim imJewelry As QuickQuoteInlandMarine = imJewelryList(rowNumber)

'                    MyLocation.InlandMarines.Remove(imJewelry)

'                    If imJewelryList.Count <= 1 Then
'                        chkIMJewelry.Checked = False
'                        chkIMJewelry.Enabled = True
'                        dvIMJewelryLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If imJewelryList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault
'                    Dim jewelryInVaultList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault)
'                    Dim jewelryInVault As QuickQuoteInlandMarine = jewelryInVaultList(rowNumber)

'                    MyLocation.InlandMarines.Remove(jewelryInVault)

'                    If jewelryInVaultList.Count <= 1 Then
'                        chkJewelInVault.Checked = False
'                        chkJewelInVault.Enabled = True
'                        dvJewelInVaultLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If jewelryInVaultList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras
'                    Dim CamerasList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras)
'                    Dim Cameras As QuickQuoteInlandMarine = CamerasList(rowNumber)

'                    MyLocation.InlandMarines.Remove(Cameras)

'                    If CamerasList.Count <= 1 Then
'                        chkCameras.Checked = False
'                        chkCameras.Enabled = True
'                        dvCamerasLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If CamerasList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer
'                    Dim ComputersList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer)
'                    Dim Computers As QuickQuoteInlandMarine = ComputersList(rowNumber)

'                    MyLocation.InlandMarines.Remove(Computers)

'                    If ComputersList.Count <= 1 Then
'                        chkComputers.Checked = False
'                        chkComputers.Enabled = True
'                        dvComputersLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If ComputersList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled
'                    Dim FarmList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled)
'                    Dim Farm As QuickQuoteInlandMarine = FarmList(rowNumber)

'                    MyLocation.InlandMarines.Remove(Farm)

'                    If FarmList.Count <= 1 Then
'                        chkFarm.Checked = False
'                        chkFarm.Enabled = True
'                        dvFarmLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If FarmList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage
'                    Dim ArtsBreakList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage)
'                    Dim ArtsBreak As QuickQuoteInlandMarine = ArtsBreakList(rowNumber)

'                    MyLocation.InlandMarines.Remove(ArtsBreak)

'                    If ArtsBreakList.Count <= 1 Then
'                        chkFABreak.Checked = False
'                        chkFABreak.Enabled = True
'                        dvFABreakLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If ArtsBreakList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage
'                    Dim ArtsNoBreakList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage)
'                    Dim ArtsNoBreak As QuickQuoteInlandMarine = ArtsNoBreakList(rowNumber)

'                    MyLocation.InlandMarines.Remove(ArtsNoBreak)

'                    If ArtsNoBreakList.Count <= 1 Then
'                        chkFANoBreak.Checked = False
'                        chkFANoBreak.Enabled = True
'                        dvFANoBreakLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If ArtsNoBreakList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs
'                    Dim FursList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs)
'                    Dim Furs As QuickQuoteInlandMarine = FursList(rowNumber)

'                    MyLocation.InlandMarines.Remove(Furs)

'                    If FursList.Count <= 1 Then
'                        chkFurs.Checked = False
'                        chkFurs.Enabled = True
'                        dvFursLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If FursList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors
'                    Dim TractorsList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors)
'                    Dim Tractors As QuickQuoteInlandMarine = TractorsList(rowNumber)

'                    MyLocation.InlandMarines.Remove(Tractors)

'                    If TractorsList.Count <= 1 Then
'                        chkGarden.Checked = False
'                        chkGarden.Enabled = True
'                        dvGardenLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If TractorsList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf
'                    Dim GolfEquipList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf)
'                    Dim GolfEquip As QuickQuoteInlandMarine = GolfEquipList(rowNumber)

'                    MyLocation.InlandMarines.Remove(GolfEquip)

'                    If GolfEquipList.Count <= 1 Then
'                        chkGolfers.Checked = False
'                        chkGolfers.Enabled = True
'                        dvGolfersLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If GolfEquipList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns
'                    Dim GunsList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns)
'                    Dim Guns As QuickQuoteInlandMarine = GunsList(rowNumber)

'                    MyLocation.InlandMarines.Remove(Guns)

'                    If GunsList.Count <= 1 Then
'                        chkGuns.Checked = False
'                        chkGuns.Enabled = True
'                        dvGunsLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If GunsList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids
'                    Dim HearingList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids)
'                    Dim Hearing As QuickQuoteInlandMarine = HearingList(rowNumber)

'                    MyLocation.InlandMarines.Remove(Hearing)

'                    If HearingList.Count <= 1 Then
'                        chkHearing.Checked = False
'                        chkHearing.Enabled = True
'                        dvHearingLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If HearingList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I
'                    Dim SilverwareList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I)
'                    Dim Silverware As QuickQuoteInlandMarine = SilverwareList(rowNumber)

'                    MyLocation.InlandMarines.Remove(Silverware)

'                    If SilverwareList.Count <= 1 Then
'                        chkSilverware.Checked = False
'                        chkSilverware.Enabled = True
'                        dvSilverwareLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If SilverwareList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile
'                    Dim MobileList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile)
'                    Dim Mobile As QuickQuoteInlandMarine = MobileList(rowNumber)

'                    MyLocation.InlandMarines.Remove(Mobile)

'                    If MobileList.Count <= 1 Then
'                        chkMobile.Checked = False
'                        chkMobile.Enabled = True
'                        dvMobileLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If MobileList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment
'                    Dim ToolsList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment)
'                    Dim Tools As QuickQuoteInlandMarine = ToolsList(rowNumber)

'                    MyLocation.InlandMarines.Remove(Tools)

'                    If ToolsList.Count <= 1 Then
'                        chkTools.Checked = False
'                        chkTools.Enabled = True
'                        dvToolsLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If ToolsList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If

'                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional
'                    Dim MusicList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional)
'                    Dim Music As QuickQuoteInlandMarine = MusicList(rowNumber)

'                    MyLocation.InlandMarines.Remove(Music)

'                    If MusicList.Count <= 1 Then
'                        chkMusic.Checked = False
'                        chkMusic.Enabled = True
'                        dvMusicLimit.Attributes.Add("style", "display:none;")
'                    End If

'                    If MusicList.Count <= 1 Then
'                        imTotalInt -= 1

'                        If imTotalInt < 0 Then
'                            imTotalInt = 0
'                        End If
'                    Else
'                        imTotalInt += 1
'                    End If
'            End Select

'            hiddenSelectedIMCoverage.Value = imTotalInt.ToString
'            RaiseEvent RecalculateCoverageTotal(hiddenSelectedIMCoverage.Value)
'            PopulateChildControls()
'            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
'            Populate()
'        End If
'    End Sub

'    Public Sub ClearAllCoverages()
'        If MyLocation IsNot Nothing Then
'            If MyLocation.InlandMarines IsNot Nothing Then
'                MyLocation.InlandMarines = New List(Of QuickQuoteInlandMarine)
'            End If

'            dvIMJewelryLimit.Attributes.Add("style", "display:none;")
'            chkIMJewelry.Enabled = True
'            chkIMJewelry.Checked = False

'            dvJewelInVaultLimit.Attributes.Add("style", "display:none;")
'            chkJewelInVault.Enabled = True
'            chkJewelInVault.Checked = False

'            dvCamerasLimit.Attributes.Add("style", "display:none;")
'            chkCameras.Enabled = True
'            chkCameras.Checked = False

'            dvComputersLimit.Attributes.Add("style", "display:none;")
'            chkComputers.Enabled = True
'            chkComputers.Checked = False

'            dvFarmLimit.Attributes.Add("style", "display:none;")
'            chkFarm.Enabled = True
'            chkFarm.Checked = False

'            dvFABreakLimit.Attributes.Add("style", "display:none;")
'            chkFABreak.Enabled = True
'            chkFABreak.Checked = False

'            dvFANoBreakLimit.Attributes.Add("style", "display:none;")
'            chkFANoBreak.Enabled = True
'            chkFANoBreak.Checked = False

'            dvFursLimit.Attributes.Add("style", "display:none;")
'            chkFurs.Enabled = True
'            chkFurs.Checked = False

'            dvGardenLimit.Attributes.Add("style", "display:none;")
'            chkGarden.Enabled = True
'            chkGarden.Checked = False

'            dvGolfersLimit.Attributes.Add("style", "display:none;")
'            chkGolfers.Enabled = True
'            chkGolfers.Checked = False

'            dvGunsLimit.Attributes.Add("style", "display:none;")
'            chkGuns.Enabled = True
'            chkGuns.Checked = False

'            dvHearingLimit.Attributes.Add("style", "display:none;")
'            chkHearing.Enabled = True
'            chkHearing.Checked = False

'            dvSilverwareLimit.Attributes.Add("style", "display:none;")
'            chkSilverware.Enabled = True
'            chkSilverware.Checked = False

'            dvMobileLimit.Attributes.Add("style", "display:none;")
'            chkMobile.Enabled = True
'            chkMobile.Checked = False

'            dvToolsLimit.Attributes.Add("style", "display:none;")
'            chkTools.Enabled = True
'            chkTools.Checked = False

'            dvMusicLimit.Attributes.Add("style", "display:none;")
'            chkMusic.Enabled = True
'            chkMusic.Checked = False

'            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
'            RaiseEvent RecalculateCoverageTotal("0")
'        End If
'    End Sub

'    Protected Sub chkJewelInVault_CheckedChanged(sender As Object, e As EventArgs) Handles chkJewelInVault.CheckedChanged, lnkAddJewleryInVault.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("JewelryVault")
'            dvJewelInVaultLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkIMJewelry_CheckedChanged(sender As Object, e As EventArgs) Handles chkIMJewelry.CheckedChanged, lnkAddIMJewelryLimit.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("IMJewelry")
'            dvIMJewelryLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkCameras_CheckedChanged(sender As Object, e As EventArgs) Handles chkCameras.CheckedChanged, lnkAddCameras.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("Cameras")
'            dvCamerasLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkComputers_CheckedChanged(sender As Object, e As EventArgs) Handles chkComputers.CheckedChanged, lnkAddComputers.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("Computers")
'            dvComputersLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkFarm_CheckedChanged(sender As Object, e As EventArgs) Handles chkFarm.CheckedChanged, lnkAddFarm.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("Farm")
'            dvFarmLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkFABreak_CheckedChanged(sender As Object, e As EventArgs) Handles chkFABreak.CheckedChanged, lnkAddFABreak.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("ArtsBreak")
'            dvFABreakLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkFANoBreak_CheckedChanged(sender As Object, e As EventArgs) Handles chkFANoBreak.CheckedChanged, lnkAddFANoBreak.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("ArtsNoBreak")
'            dvFANoBreakLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkGarden_CheckedChanged(sender As Object, e As EventArgs) Handles chkGarden.CheckedChanged, lnkAddGarden.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("Tractors")
'            dvGardenLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkFurs_CheckedChanged(sender As Object, e As EventArgs) Handles chkFurs.CheckedChanged, lnkAddFurs.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("Furs")
'            dvFursLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkGolfers_CheckedChanged(sender As Object, e As EventArgs) Handles chkGolfers.CheckedChanged, lnkAddGolfers.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("GolfEquip")
'            dvGolfersLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkGuns_CheckedChanged(sender As Object, e As EventArgs) Handles chkGuns.CheckedChanged, lnkAddGuns.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("Guns")
'            dvGunsLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkHearing_CheckedChanged(sender As Object, e As EventArgs) Handles chkHearing.CheckedChanged, lnkAddHearing.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("Hearing")
'            dvHearingLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkSilverware_CheckedChanged(sender As Object, e As EventArgs) Handles chkSilverware.CheckedChanged, lnkAddSilverware.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("Silverware")
'            dvSilverwareLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkMobile_CheckedChanged(sender As Object, e As EventArgs) Handles chkMobile.CheckedChanged, lnkAddMobile.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("Mobile")
'            dvMobileLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkTools_CheckedChanged(sender As Object, e As EventArgs) Handles chkTools.CheckedChanged, lnkAddTools.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("Tools")
'            dvToolsLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub

'    Protected Sub chkMusic_CheckedChanged(sender As Object, e As EventArgs) Handles chkMusic.CheckedChanged, lnkAddMusic.Click
'        If MyLocation IsNot Nothing Then
'            AddNewItem("Music")
'            dvMusicLimit.Attributes.Add("style", "display:block;")
'        End If
'    End Sub
'End Class