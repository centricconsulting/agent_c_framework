Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports IFM.VR.Web.Helpers.WebHelper_Personal

Public Class ctl_PropertyUpdates_HOM_App
    Inherits VRControlBase

    Public ReadOnly Property RoofUpdateID() As String
        Get
            Return Me.txtRoofUpdateYear.ClientID
        End Get
    End Property

    Public ReadOnly Property RemarksID() As String
        Get
            Return Me.txtInspectionRemarks.ClientID
        End Get
    End Property

    Public Property MyLocationIndex As Int32
        Get
            If ViewState("vs_locationIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_locationIndex"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property IsHouse30OrMore As Boolean
        Get
            If MyLocation IsNot Nothing Then
                Dim yearBuilt As Int32 = 0
                If Int32.TryParse(MyLocation.YearBuilt, yearBuilt) Then
                    Dim houseAge As Int32 = DateTime.Now.Year - yearBuilt
                    Return houseAge >= 30
                End If
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property IsContentsOnlyPolicy As Boolean
        Get
            'Updated 12/5/17 for HOM Upgrade MLW
            If (MyLocation IsNot Nothing) Then
                If (MyLocation.FormTypeId.Equals("4")) Then '4 = HO-4 Contents policy
                    Return True
                Else
                    If (MyLocation.FormTypeId.Equals("25") AndAlso MyLocation.StructureTypeId <> "2") Then '25 = HO 0004 Contents policy, 2 = Mobile
                        Return True
                    Else
                        Return False
                    End If
                End If
            Else
                Return False
            End If
            'Return If(MyLocation IsNot Nothing, MyLocation.FormTypeId.Equals("4"), False) '4 = HO-4 Contents policy
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = Me.divUpdatesMain.ClientID
            'Updated 12/4/17 for HOM Upgrade MLW
            If IsHouse30OrMore Or MyLocation.FormTypeId = "6" Or MyLocation.FormTypeId = "7" Or Me.Quote.LobType = QuickQuoteLobType.Farm Or ((MyLocation.FormTypeId = "22" Or MyLocation.FormTypeId = "25") And MyLocation.StructureTypeId = "2") Then
                hiddenUpdatesAccordActive.Value = "0"
            Else
                hiddenUpdatesAccordActive.Value = "false"
            End If
        End If

        If Me.Quote IsNot Nothing Then
            Select Case Me.Quote.LobType
                Case QuickQuoteLobType.HomePersonal
                    If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                        If Me.Quote.Locations(0).FormTypeId = "4" Then
                            Me.Visible = False
                        Else
                            'Added 12/4/17 for HOM Upgrade MLW
                            If Me.Quote.Locations(0).FormTypeId = "25" AndAlso Me.Quote.Locations(0).StructureTypeId <> "2" Then
                                Me.Visible = False
                            End If
                        End If
                    Else
                        Me.Visible = False
                    End If
                Case QuickQuoteLobType.Farm
                    If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                        If MyLocation.FormTypeId = "13" Then
                            Me.Visible = False
                        End If
                    Else
                        Me.Visible = False
                    End If
            End Select

        End If

    End Sub

    Public Overrides Sub AddScriptAlways()
        Me.VRScript.AddVariableLine("var IsHouse30OrOver" + MyLocationIndex.ToString() + " = " + IsHouse30OrMore.ToString().ToLower() + ";")
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(MainAccordionDivId, hiddenUpdatesAccordActive, "")
        Me.VRScript.CreateConfirmDialog(Me.lnkClear.ClientID, "Clear Property Updates?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.CreateDatePicker(Me.txtInspectionDate.ClientID, True)
        Me.VRScript.AddVariableLine("var InspectiondateClientId = '" + Me.txtInspectionDate.ClientID + "';")

        If Me.Quote IsNot Nothing Then
            If Me.Quote.LobType = QuickQuoteLobType.Farm Then
                ' ************************** these are for occupant dropdown ***************************
                Me.VRScript.AddScriptLine("BindMedicalPaymentNametoOccupant('" + Me.ddOccupants.ClientID + "','" + Me.hdnOccupant.ClientID + "');", True)
                Me.VRScript.AddScriptLine("LoadFramLocationOccupantDropdown('" + Me.ddOccupants.ClientID + "','" + Me.hdnOccupant.ClientID + "');", True)
                Me.VRScript.CreateJSBinding(Me.ddOccupants.ClientID, "change", "$('#" + Me.hdnOccupant.ClientID + "').val($('#" + Me.ddOccupants.ClientID + "').val());")
                ' ************************** END ***************************
            End If
        End If



    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddRoofType.Items.Count = 0 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddRoofType, QuickQuoteClassName.QuickQuoteUpdatesRecord, QuickQuotePropertyName.RoofUpdateTypeId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddCentralAirType, QuickQuoteClassName.QuickQuoteUpdatesRecord, QuickQuotePropertyName.CentralHeatUpdateTypeId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddElectricType, QuickQuoteClassName.QuickQuoteUpdatesRecord, QuickQuotePropertyName.ElectricUpdateTypeId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddPlumbingType, QuickQuoteClassName.QuickQuoteUpdatesRecord, QuickQuotePropertyName.PlumbingUpdateTypeId, SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        If Me.Visible Then
            Me.ValidationHelper.GroupName = "Property Updates"
            If Me.Quote.LobType = QuickQuoteLobType.Farm Then
                Me.ValidationHelper.GroupName = String.Format("Location #{0} - Property Updates", (Me.MyLocationIndex + 1).ToString())
            End If

            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            Dim valItems = PropertyUpdatesValidator.ValidateHOMPropertyUpdates(Me.Quote, Me.MyLocationIndex, valArgs.ValidationType)
            If valItems.Any() Then

                For Each v In valItems
                    Select Case v.FieldId
                        Case PropertyUpdatesValidator.UpdatesRoofYear
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtRoofUpdateYear, v, accordList)
                        Case PropertyUpdatesValidator.UpadtesRoofUpdateType
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.radRoofPartial.ClientID + " parent", v, accordList)
                        Case PropertyUpdatesValidator.UpdatesRoofMaterialType
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddRoofType, v, accordList)

                        Case PropertyUpdatesValidator.UpdatesHeatYear
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCentralAirUpdated, v, accordList)
                        Case PropertyUpdatesValidator.UpdatesHeatUpdateType
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.radCentralPartial.ClientID + " parent", v, accordList)
                        Case PropertyUpdatesValidator.UpdatesHeatMaterialType
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddCentralAirType, v, accordList)

                        Case PropertyUpdatesValidator.UpdatesElectricYear
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtElectricUpdated, v, accordList)
                        Case PropertyUpdatesValidator.UpdatesElectricUpdateType
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.radElectricPartial.ClientID + " parent", v, accordList)
                        Case PropertyUpdatesValidator.UpdatesElectricMaterialType
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddElectricType, v, accordList)

                        Case PropertyUpdatesValidator.UpdatesPlumbingYear
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPlumbingUpdated, v, accordList)
                        Case PropertyUpdatesValidator.UpdatesPlumbingUpdateType
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.radPlumbingPartial.ClientID + " parent", v, accordList)
                        Case PropertyUpdatesValidator.UpdatesPlumbingMaterialType
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddPlumbingType, v, accordList)

                        Case PropertyUpdatesValidator.UpdatesInspectionDate
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtInspectionDate, v, accordList)
                        Case PropertyUpdatesValidator.UpdatesInspectionUpdateType
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.radInspectionPartial.ClientID + " parent", v, accordList)
                        Case PropertyUpdatesValidator.UpdatesInspectionRemarks
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtInspectionRemarks, v, accordList)

                        Case PropertyUpdatesValidator.MobileHomeLength
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtMobileHomeLength, v, accordList)
                        Case PropertyUpdatesValidator.MobileHomeWidth
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtMobileHomeWidth, v, accordList)

                        Case PropertyUpdatesValidator.Occupant
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddOccupants, v, accordList)

                    End Select
                Next
            End If
        End If
    End Sub

    Public Overrides Sub Populate()

        If Me.MyLocation IsNot Nothing AndAlso IsContentsOnlyPolicy = False Then
            LoadStaticData()

            Select Case Me.Quote.LobType
                Case QuickQuoteLobType.Farm
                    Me.lblMainAccord.Text = String.Format("Dwelling for Location #{0} - Updates", (Me.MyLocationIndex + 1).ToString())
                    Me.lblMainAccord.Text = IFM.Common.InputValidation.InputHelpers.EllipsisText(Me.lblMainAccord.Text, 60)

                Case Else
                    Me.lblMainAccord.Text = String.Format("Property Updates - {0} {1} {2} {3}", Me.MyLocation.Address.HouseNum, Me.MyLocation.Address.StreetName, Me.MyLocation.Address.City, Me.MyLocation.Address.State)
                    Me.lblMainAccord.Text = IFM.Common.InputValidation.InputHelpers.EllipsisText(Me.lblMainAccord.Text, 60)

            End Select

            'ROOF
            Me.txtRoofUpdateYear.Text = IFM.Common.InputValidation.InputHelpers.RemovePossibleDefaultedNumericValue(Me.MyLocation.Updates.RoofUpdateYear)
            If String.IsNullOrWhiteSpace(Me.MyLocation.Updates.RoofUpdateTypeId) = False Then
                Me.radRoofPartial.Checked = Me.MyLocation.Updates.RoofUpdateTypeId = "1" ' 1 = partial, 2 = complete
                Me.radRoofComplete.Checked = Me.MyLocation.Updates.RoofUpdateTypeId = "2" ' 1 = partial, 2 = complete
            End If

            If Me.MyLocation.Updates.RoofAsphaltShingle Then
                SetdropDownFromValue(Me.ddRoofType, "1")
            End If
            If Me.MyLocation.Updates.RoofWood Then
                SetdropDownFromValue(Me.ddRoofType, "2")
            End If
            If Me.MyLocation.Updates.RoofOther Then
                SetdropDownFromValue(Me.ddRoofType, "3")
            End If
            If Me.MyLocation.Updates.RoofSlate Then
                SetdropDownFromValue(Me.ddRoofType, "4")
            End If
            If Me.MyLocation.Updates.RoofMetal Then
                SetdropDownFromValue(Me.ddRoofType, "5")
            End If

            'Central Heat
            Me.txtCentralAirUpdated.Text = IFM.Common.InputValidation.InputHelpers.RemovePossibleDefaultedNumericValue(Me.MyLocation.Updates.CentralHeatUpdateYear)
            If String.IsNullOrWhiteSpace(Me.MyLocation.Updates.CentralHeatUpdateTypeId) = False Then
                Me.radCentralPartial.Checked = Me.MyLocation.Updates.CentralHeatUpdateTypeId = "1" ' 1 = partial, 2 = complete
                Me.radCentralComplete.Checked = Me.MyLocation.Updates.CentralHeatUpdateTypeId = "2" ' 1 = partial, 2 = complete
            End If

            If Me.MyLocation.Updates.CentralHeatGas Then
                SetdropDownFromValue(Me.ddCentralAirType, "1")
            End If
            If Me.MyLocation.Updates.CentralHeatElectric Then
                SetdropDownFromValue(Me.ddCentralAirType, "2")
            End If
            If Me.MyLocation.Updates.CentralHeatOil Then
                SetdropDownFromValue(Me.ddCentralAirType, "3")
            End If
            If Me.MyLocation.Updates.CentralHeatOther Then
                SetdropDownFromValue(Me.ddCentralAirType, "4")
            End If

            'Electric
            Me.txtElectricUpdated.Text = IFM.Common.InputValidation.InputHelpers.RemovePossibleDefaultedNumericValue(Me.MyLocation.Updates.ElectricUpdateYear)
            If String.IsNullOrWhiteSpace(Me.MyLocation.Updates.ElectricUpdateTypeId) = False Then
                Me.radElectricPartial.Checked = Me.MyLocation.Updates.ElectricUpdateTypeId = "1" ' 1 = partial, 2 = complete
                Me.radElectricComplete.Checked = Me.MyLocation.Updates.ElectricUpdateTypeId = "2" ' 1 = partial, 2 = complete
            End If

            If Me.MyLocation.Updates.ElectricCircuitBreaker Then
                SetdropDownFromValue(Me.ddElectricType, "1")
            End If
            If Me.MyLocation.Updates.ElectricFuses Then
                SetdropDownFromValue(Me.ddElectricType, "2")
            End If
            If Me.MyLocation.Updates.Electric60Amp Then
                SetdropDownFromValue(Me.ddElectricType, "3")
            End If
            If Me.MyLocation.Updates.Electric100Amp Then
                SetdropDownFromValue(Me.ddElectricType, "4")
            End If
            If Me.MyLocation.Updates.Electric200Amp Then
                SetdropDownFromValue(Me.ddElectricType, "5")
            End If

            'Plumbing
            Me.txtPlumbingUpdated.Text = IFM.Common.InputValidation.InputHelpers.RemovePossibleDefaultedNumericValue(Me.MyLocation.Updates.PlumbingUpdateYear)
            If String.IsNullOrWhiteSpace(Me.MyLocation.Updates.PlumbingUpdateTypeId) = False Then
                Me.radPlumbingPartial.Checked = Me.MyLocation.Updates.PlumbingUpdateTypeId = "1" ' 1 = partial, 2 = complete
                Me.radPlumbingComplete.Checked = Me.MyLocation.Updates.PlumbingUpdateTypeId = "2" ' 1 = partial, 2 = complete
            End If

            If Me.MyLocation.Updates.PlumbingPlastic Then
                SetdropDownFromValue(Me.ddPlumbingType, "1")
            End If
            If Me.MyLocation.Updates.PlumbingGalvanized Then
                SetdropDownFromValue(Me.ddPlumbingType, "2")
            End If
            If Me.MyLocation.Updates.PlumbingCopper Then
                SetdropDownFromValue(Me.ddPlumbingType, "3")
            End If

            'Inspection
            If Me.Quote.LobType <> QuickQuoteLobType.Farm Then
                Me.txtInspectionDate.Text = IFM.Common.InputValidation.InputHelpers.RemovePossibleDefaultedDateValue(Me.MyLocation.Updates.InspectionDate)
                If String.IsNullOrWhiteSpace(Me.MyLocation.Updates.InspectionUpdateTypeId) = False Then
                    Me.radInspectionPartial.Checked = Me.MyLocation.Updates.InspectionUpdateTypeId = "1" ' 1 = partial, 2 = complete
                    Me.radInspectionComplete.Checked = Me.MyLocation.Updates.InspectionUpdateTypeId = "2" ' 1 = partial, 2 = complete
                End If
                Me.txtInspectionRemarks.Text = Me.MyLocation.Updates.InspectionRemarks
            Else
                Me.trInspection.Visible = False
            End If

            'Updated 12/4/17 for HOM Upgrade
            If Me.MyLocation.FormTypeId = "6" Or Me.MyLocation.FormTypeId = "7" Or ((MyLocation.FormTypeId = "22" Or MyLocation.FormTypeId = "25") And MyLocation.StructureTypeId = "2") Then
                Me.divMobileWidthHeight.Visible = True
                Me.txtMobileHomeLength.Text = If(Me.MyLocation.MobileHomeLength <> "0", Me.MyLocation.MobileHomeLength, "")
                Me.txtMobileHomeWidth.Text = If(Me.MyLocation.MobileHomeWidth <> "0", Me.MyLocation.MobileHomeWidth, "")
            Else
                Me.divMobileWidthHeight.Visible = False
            End If

            'Farm Information
            If Me.Quote.LobType = QuickQuoteLobType.Farm Then
                Me.chkBoxCentralHeat.Checked = Me.MyLocation.Updates.CentralHeatUpdateTypeId.Equals("2")
                Me.divFarmAdditionalQuestions.Visible = True
                Me.radCentralPartial.Visible = False
                Me.radCentralComplete.Visible = False

                Me.hdnOccupant.Value = MyLocation.Remarks
            Else
                Me.divFarmAdditionalQuestions.Visible = False
                Me.radCentralPartial.Visible = True
                Me.radCentralComplete.Visible = True
            End If

        End If
    End Sub

    Private Sub ClearForm()
        LoadStaticData()
        Me.txtCentralAirUpdated.Text = ""
        Me.txtElectricUpdated.Text = ""
        Me.txtInspectionDate.Text = ""
        Me.txtInspectionRemarks.Text = ""
        Me.txtPlumbingUpdated.Text = ""
        Me.txtRoofUpdateYear.Text = ""

        Me.radCentralComplete.Checked = False
        Me.radCentralPartial.Checked = False
        Me.radElectricComplete.Checked = False
        Me.radElectricPartial.Checked = False
        Me.radInspectionComplete.Checked = False
        Me.radInspectionPartial.Checked = False
        Me.radPlumbingComplete.Checked = False
        Me.radPlumbingPartial.Checked = False
        Me.radRoofComplete.Checked = False
        Me.radRoofPartial.Checked = False

        Me.ddCentralAirType.SelectedIndex = 0
        Me.ddElectricType.SelectedIndex = 0
        Me.ddPlumbingType.SelectedIndex = 0
        Me.ddRoofType.SelectedIndex = 0

    End Sub

    Public Overrides Function Save() As Boolean

        If Me.Visible Then
            ' wipe all data from form if it is a contents only form type
            If IsContentsOnlyPolicy Then
                ClearForm()
            End If

            If Me.Quote IsNot Nothing AndAlso Me.MyLocation IsNot Nothing AndAlso Me.MyLocation.Updates IsNot Nothing Then
                'ROOF
                Me.MyLocation.Updates.RoofUpdateYear = Me.txtRoofUpdateYear.Text
                If radRoofPartial.Checked Or radRoofComplete.Checked Then
                    If radRoofPartial.Checked Then
                        Me.MyLocation.Updates.RoofUpdateTypeId = "1"
                    Else
                        Me.MyLocation.Updates.RoofUpdateTypeId = "2"
                    End If
                Else
                    Me.MyLocation.Updates.RoofUpdateTypeId = ""
                End If

                Me.MyLocation.Updates.RoofAsphaltShingle = False
                Me.MyLocation.Updates.RoofWood = False
                Me.MyLocation.Updates.RoofOther = False
                Me.MyLocation.Updates.RoofSlate = False
                Me.MyLocation.Updates.RoofMetal = False
                Select Case Me.ddRoofType.SelectedValue
                    Case "1"
                        Me.MyLocation.Updates.RoofAsphaltShingle = True
                    Case "2"
                        Me.MyLocation.Updates.RoofWood = True
                    Case "3"
                        Me.MyLocation.Updates.RoofOther = True
                    Case "4"
                        Me.MyLocation.Updates.RoofSlate = True
                    Case "5"
                        Me.MyLocation.Updates.RoofMetal = True
                End Select

                Select Case Me.Quote.LobType
                    Case QuickQuoteLobType.Farm
                        Me.MyLocation.Updates.CentralHeatUpdateYear = Me.txtCentralAirUpdated.Text
                        If Me.chkBoxCentralHeat.Checked Then
                            Me.MyLocation.Updates.CentralHeatUpdateTypeId = "2"
                        Else
                            If IsHouse30OrMore Then
                                Me.MyLocation.Updates.CentralHeatUpdateTypeId = "1"
                            Else
                                If String.IsNullOrWhiteSpace(Me.txtCentralAirUpdated.Text) = False AndAlso String.IsNullOrWhiteSpace(Me.ddCentralAirType.SelectedValue) = False Then
                                    Me.MyLocation.Updates.CentralHeatUpdateTypeId = "1"
                                Else
                                    Me.MyLocation.Updates.CentralHeatUpdateTypeId = ""
                                End If
                            End If
                        End If
                        Me.MyLocation.Updates.CentralHeatGas = False
                        Me.MyLocation.Updates.CentralHeatElectric = False
                        Me.MyLocation.Updates.CentralHeatOil = False
                        Me.MyLocation.Updates.CentralHeatOther = False
                        Select Case Me.ddCentralAirType.SelectedValue
                            Case "1"
                                Me.MyLocation.Updates.CentralHeatGas = True
                            Case "2"
                                Me.MyLocation.Updates.CentralHeatElectric = True
                            Case "3"
                                Me.MyLocation.Updates.CentralHeatOil = True
                            Case "4"
                                Me.MyLocation.Updates.CentralHeatOther = True

                        End Select

                        MyLocation.Remarks = Me.hdnOccupant.Value.Trim().ToUpper()

                    Case Else
                        'Central Heat
                        Me.MyLocation.Updates.CentralHeatUpdateYear = Me.txtCentralAirUpdated.Text
                        If Me.radCentralPartial.Checked Or Me.radCentralComplete.Checked Then
                            If Me.radCentralPartial.Checked Then
                                Me.MyLocation.Updates.CentralHeatUpdateTypeId = "1"
                            Else
                                Me.MyLocation.Updates.CentralHeatUpdateTypeId = "2"
                            End If
                        Else
                            Me.MyLocation.Updates.CentralHeatUpdateTypeId = ""
                        End If

                        Me.MyLocation.Updates.CentralHeatGas = False
                        Me.MyLocation.Updates.CentralHeatElectric = False
                        Me.MyLocation.Updates.CentralHeatOil = False
                        Me.MyLocation.Updates.CentralHeatOther = False
                        Select Case Me.ddCentralAirType.SelectedValue
                            Case "1"
                                Me.MyLocation.Updates.CentralHeatGas = True
                            Case "2"
                                Me.MyLocation.Updates.CentralHeatElectric = True
                            Case "3"
                                Me.MyLocation.Updates.CentralHeatOil = True
                            Case "4"
                                Me.MyLocation.Updates.CentralHeatOther = True

                        End Select
                End Select

                'Electric
                Me.MyLocation.Updates.ElectricUpdateYear = Me.txtElectricUpdated.Text
                If Me.radElectricPartial.Checked Or Me.radElectricComplete.Checked Then
                    If Me.radElectricPartial.Checked Then
                        Me.MyLocation.Updates.ElectricUpdateTypeId = "1"
                    Else
                        Me.MyLocation.Updates.ElectricUpdateTypeId = "2"
                    End If
                Else
                    Me.MyLocation.Updates.ElectricUpdateTypeId = ""
                End If

                Me.MyLocation.Updates.ElectricCircuitBreaker = False
                Me.MyLocation.Updates.ElectricFuses = False
                Me.MyLocation.Updates.Electric60Amp = False
                Me.MyLocation.Updates.Electric100Amp = False
                Me.MyLocation.Updates.Electric200Amp = False
                Select Case Me.ddElectricType.SelectedValue
                    Case "1"
                        Me.MyLocation.Updates.ElectricCircuitBreaker = True
                    Case "2"
                        Me.MyLocation.Updates.ElectricFuses = True
                    Case "3"
                        Me.MyLocation.Updates.Electric60Amp = True
                    Case "4"
                        Me.MyLocation.Updates.Electric100Amp = True
                    Case "5"
                        Me.MyLocation.Updates.Electric200Amp = True
                End Select

                'Plumbing
                Me.MyLocation.Updates.PlumbingUpdateYear = Me.txtPlumbingUpdated.Text
                If Me.radPlumbingPartial.Checked Or Me.radPlumbingComplete.Checked Then
                    If Me.radPlumbingPartial.Checked Then
                        Me.MyLocation.Updates.PlumbingUpdateTypeId = "1"
                    Else
                        Me.MyLocation.Updates.PlumbingUpdateTypeId = "2"
                    End If
                Else
                    Me.MyLocation.Updates.PlumbingUpdateTypeId = ""
                End If

                Me.MyLocation.Updates.PlumbingPlastic = False
                Me.MyLocation.Updates.PlumbingGalvanized = False
                Me.MyLocation.Updates.PlumbingCopper = False
                Select Case Me.ddPlumbingType.SelectedValue
                    Case "1"
                        Me.MyLocation.Updates.PlumbingPlastic = True
                    Case "2"
                        Me.MyLocation.Updates.PlumbingGalvanized = True
                    Case "3"
                        Me.MyLocation.Updates.PlumbingCopper = True
                End Select

                'Inspection
                If Me.Quote.LobType = QuickQuoteLobType.DwellingFirePersonal Then
                    'The If statement below is safeguarding against invalid dates however this leads to some funky things where the stored value doesn't necessarily match what is on the screen.
                    Me.MyLocation.Updates.InspectionDate = Me.txtInspectionDate.Text
                Else
                    If IsDate(Me.txtInspectionDate.Text) Then
                        Me.MyLocation.Updates.InspectionDate = Me.txtInspectionDate.Text
                    End If
                End If

                If Me.radInspectionPartial.Checked Or Me.radInspectionComplete.Checked Then
                    If Me.radInspectionPartial.Checked Then
                        Me.MyLocation.Updates.InspectionUpdateTypeId = "1"
                    Else
                        Me.MyLocation.Updates.InspectionUpdateTypeId = "2"
                    End If
                Else
                    Me.MyLocation.Updates.InspectionUpdateTypeId = ""
                End If

                If Me.txtInspectionRemarks.Text.Trim().Length > 300 Then ' table 1024 char max
                    Me.MyLocation.Updates.InspectionRemarks = Me.txtInspectionRemarks.Text.Trim().Substring(0, 300).Trim()
                Else
                    Me.MyLocation.Updates.InspectionRemarks = Me.txtInspectionRemarks.Text.Trim()
                End If

                Me.MyLocation.MobileHomeLength = Me.txtMobileHomeLength.Text
                Me.MyLocation.MobileHomeWidth = Me.txtMobileHomeWidth.Text

                If Me.Quote.LobType = QuickQuoteLobType.Farm Then
                    MyLocation.Remarks = Me.hdnOccupant.Value.Trim().ToUpper()
                End If
            End If

        End If
        Return True
    End Function

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Protected Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        Me.ClearControl()
        'force edit mode so they have to save at some point before leaving
        Me.LockTree()
    End Sub

    Public Overrides Sub ClearControl()
        Me.txtCentralAirUpdated.Text = ""
        Me.txtElectricUpdated.Text = ""
        Me.txtInspectionDate.Text = ""
        Me.txtInspectionRemarks.Text = ""
        Me.txtPlumbingUpdated.Text = ""
        Me.txtRoofUpdateYear.Text = ""

        Me.radCentralComplete.Checked = False
        Me.radCentralPartial.Checked = False
        Me.radElectricComplete.Checked = False
        Me.radElectricPartial.Checked = False
        Me.radInspectionComplete.Checked = False
        Me.radInspectionPartial.Checked = False
        Me.radPlumbingComplete.Checked = False
        Me.radPlumbingPartial.Checked = False
        Me.radRoofComplete.Checked = False
        Me.radRoofPartial.Checked = False

        Me.ddCentralAirType.SelectedIndex = 0
        Me.ddElectricType.SelectedIndex = 0
        Me.ddPlumbingType.SelectedIndex = 0
        Me.ddRoofType.SelectedIndex = 0

        'farm
        Me.chkBoxCentralHeat.Checked = False
        Me.hdnOccupant.Value = ""

        MyBase.ClearControl()
    End Sub

End Class