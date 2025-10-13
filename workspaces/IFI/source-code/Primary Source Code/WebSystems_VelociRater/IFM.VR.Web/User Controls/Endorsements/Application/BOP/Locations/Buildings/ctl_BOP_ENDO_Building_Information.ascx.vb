Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers
Imports System.Configuration.ConfigurationManager
Imports System.Data
Imports System.Data.SqlClient
Imports DevDictionaryHelper
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctl_BOP_ENDO_Building_Information
    Inherits VRControlBase


#Region "Declarations"

    Public Event BuildingClassificationChanged(ByVal LocIndex As Integer, ByVal BldgIndex As Integer, ByVal ClassificationIndex As Integer, ByVal NewClassCode As String)

    Public ReadOnly Property MyScrollToID
        Get
            Return txtDescription.ClientID
        End Get
    End Property
    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
            'Me.ctlProtectionClass_HOM.MyLocationIndex = value
            Me.ctlClassificationsList.LocationIndex = value
        End Set
    End Property

    Public Property BuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
            Me.ctlClassificationsList.BuildingIndex = value
        End Set
    End Property

    Private ReadOnly Property MyBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.GetItemAtIndex(Me.BuildingIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ScrollToControlId As String
        Get
            Return Me.txtDescription.ClientID
        End Get
    End Property

    Private Property _BopDictItems As DevDictionaryHelper.AllCommercialDictionary
    Public ReadOnly Property BopDictItems As DevDictionaryHelper.AllCommercialDictionary
        Get
            If Quote IsNot Nothing Then
                If _BopDictItems Is Nothing Then
                    _BopDictItems = New DevDictionaryHelper.AllCommercialDictionary(Quote)
                End If
            End If
            Return _BopDictItems
        End Get
    End Property

#End Region

#Region "Methods and Functions"

    Public Sub UpdateProtectionClasses()
        LoadProtectionClasses()
    End Sub

    Private Sub HandleError(ByVal RoutineName As String, ByVal ex As Exception)
        Dim str As String = RoutineName & ":  " & ex.Message
        If AppSettings("TestOrProd").ToUpper <> "PROD" Then lblMsg.Text = str Else Throw New Exception(ex.Message, ex)
    End Sub

    ''' <summary>
    ''' Handles building classification changes
    ''' </summary>
    ''' <param name="LocNdx"></param>
    ''' <param name="BldNdx"></param>
    ''' <param name="ClassNdx"></param>
    Private Sub HandleBuildingClassificationChange(ByVal LocNdx As Integer, ByVal BldNdx As Integer, ByVal ClassNdx As Integer, ByVal NewClassCode As String) Handles ctlClassificationsList.BuildingClassificationChanged
        ' OHIO MINE SUB
        ' Ohio mine sub layout may change depending on the classification
        ' Only repopulate if location and building match and the state is OH
        If Quote IsNot Nothing AndAlso IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) AndAlso LocationIndex = LocNdx AndAlso BuildingIndex = BldNdx AndAlso Quote.Locations(LocationIndex).Address.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio Then
            ' For Ohio, certain counties are required to have mine subsidence and it cannot be removed by the user.
            ' Only certain building class codes are eligible for mine sub.  Some class codes will require number of units.
            ' If mine sub is required, show the OH info row as well.
            trMineSubsidenceInfo_OH.Attributes.Add("style", "display:none")
            trMineSubsidenceNumberOfUnitsRow.Attributes.Add("style", "display:none")
            Dim NumUnitsReqd As Boolean = False
            trMineSubsidenceLimitInfo.Attributes.Add("style", "display:none")
            ' Only specific dwelling class codes are eligible for mine subsidence
            If (Not NewClassCode.IsNullEmptyorWhitespace) AndAlso IFM.VR.Common.Helpers.MineSubsidenceHelper.OhioBuildingClassCodeIsEligibleForMineSubsidence(NewClassCode, NumUnitsReqd) Then
                Select Case IFM.VR.Common.Helpers.MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(Quote.Locations(LocationIndex).Address.County)
                    Case MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleMandatory
                        ' Must have mine sub
                        trMineSubsidenceRow.Attributes.Add("style", "display:''")
                        trMineSubsidenceInfo_OH.Attributes.Add("style", "display:''")
                        If MyBuilding.Limit IsNot Nothing AndAlso IsNumeric(MyBuilding.Limit) Then
                            If CDec(MyBuilding.Limit) >= 300000 Then
                                trMineSubsidenceLimitInfo.Attributes.Add("style", "display:''")
                            End If
                        End If
                        chkMineSubsidence.Checked = True
                        chkMineSubsidence.Enabled = False
                        If NumUnitsReqd Then trMineSubsidenceNumberOfUnitsRow.Attributes.Add("style", "display:''")
                        Exit Select
                    Case Else
                        ' Hide Mine Sub for non mine sub req'd counties
                        trMineSubsidenceRow.Attributes.Add("style", "display:none")
                        trMineSubsidenceNumberOfUnitsRow.Attributes.Add("style", "display:none")
                        chkMineSubsidence.Checked = False
                End Select
            End If
        End If
        Exit Sub
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "0")
        chkLimitationsOnRoofSurfacing.Attributes.Add("onchange", "BopBuilding.LimitationsOnRoofSurfacingCheckboxChanged('" & chkLimitationsOnRoofSurfacing.ClientID & "','" & trRoofSettlementOptionsRow.ClientID & "');")
        Me.VRScript.CreateConfirmDialog(Me.lnkClear.ClientID, "Clear?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)

    End Sub

    Public Overrides Sub LoadStaticData()
        'Added 10/25/2021 for BOP Endorsements task 61276 MLW
        Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
        If Not endorsementsPreexistHelper.IsPreexistingLocation(LocationIndex) Then
            Dim removeItem As ListItem = Nothing
            For Each item As ListItem In Me.ddlAutomaticIncrease.Items
                If item.Value = "0" Then
                    removeItem = item
                End If
            Next
            If removeItem IsNot Nothing Then
                Me.ddlAutomaticIncrease.Items.Remove(removeItem)
            End If
        End If
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddlPropertyDeductible, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDeductibleId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            Dim Item250 = New ListItem("250", "21")
            ddlPropertyDeductible.Items.Remove(Item250)
        If Quote IsNot Nothing AndAlso Common.Helpers.BOP.RemovePropDedBelow1k.IsPropertyDeductibleBelow1kAvailable(Quote) Then
            Dim Item500 = New ListItem("500", "22")
            ddlPropertyDeductible.Items.Remove(Item500)
        End If
    End Sub

    Public Overrides Sub Populate()
        Try
            LoadStaticData()
            If MyBuilding IsNot Nothing Then
                txtDescription.Text = MyBuilding.Description
                ddlOccupancy.SetFromValue(MyBuilding.OccupancyId)
                ddlOccupancy_SelectedIndexChanged(Me, New EventArgs())
                ddlConstruction.SetFromValue(MyBuilding.ConstructionId)
                ddlAutomaticIncrease.SetFromValue(MyBuilding.AutoIncreaseId, 2)
                ' MGB 4-11-18
                'ddlPropertyDeductible.SelectedValue = Quote.Locations(LocationIndex).PropertyDeductibleId
                If Quote IsNot Nothing Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddlPropertyDeductible, Quote.Locations(LocationIndex).PropertyDeductibleId, QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDeductibleId, Quote.Locations(LocationIndex).PropertyDeductibleId))              
                End If
                If BuildingIndex = 0 Then
                    ddlPropertyDeductible.Enabled = True
                Else
                    ddlPropertyDeductible.Enabled = False
                End If
                txtBuildingLimit.Text = MyBuilding.Limit
                'Add Missing Diamond Options if selected.
                If MyBuilding.ValuationId IsNot Nothing And MyBuilding.ValuationId = "3" Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddDropDownValueIfMissing(ddlBuildingValuation, "3", "Functional Building Valuation")
                End If
                ddlBuildingValuation.SetFromValue(MyBuilding.ValuationId)
                chkBuildingValuationIncludedInBlanketRating.Checked = MyBuilding.IsBuildingValIncludedInBlanketRating
                chkSprinklered.Checked = MyBuilding.HasSprinklered
                txtPersonalPropertyLimit.Text = MyBuilding.PersonalPropertyLimit
                ddlValuationMethod.SetFromValue(MyBuilding.ValuationMethodId)
                chkValuationMethodIncludedInBlanketRating.Checked = MyBuilding.IsValMethodIncludedInBlanketRating

                ' LIMITATIONS ON ROOF SURFACING - Added for multistate MGB 10/30/2018
                ' When building is in ILLINOIS, hide 'ACV Roofing' and show 'Limitations on Roof Surfacing' instead
                ' Hide both coverages to start
                trACVRoofing.Attributes.Add("style", "display:none")
                trLimitationsOnRoofSurfacing.Attributes.Add("style", "display:none")
                trRoofSettlementOptionsRow.Attributes.Add("style", "display:none")

                ' Depending on the state the location is in hide or show the appropriate coverage
                Select Case Quote.Locations(LocationIndex).Address.QuickQuoteState
                    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                        ' Illinois & Ohio - show Limitations on Roof Surfacing
                        trLimitationsOnRoofSurfacing.Attributes.Add("style", "display:''")
                        chkLimitationsOnRoofSurfacing.Checked = MyBuilding.HasLimitationsOnRoofSurfacing
                        ' Load the roof surfacing options based on state
                        ddRoofSettlementOptions.Items.Clear()
                        ddRoofSettlementOptions.Items.Add(New ListItem("", ""))
                        ddRoofSettlementOptions.Items.Add(New ListItem("ACV Roof Surfacing", "ACV"))
                        ddRoofSettlementOptions.Items.Add(New ListItem("Exclude Cosmetic Damage", "EXCLUDE"))
                        ddRoofSettlementOptions.Items.Add(New ListItem("Both", "BOTH"))

                        If chkLimitationsOnRoofSurfacing.Checked Then
                            ' Show the roof settlement options row when checked
                            trRoofSettlementOptionsRow.Attributes.Add("style", "display:''")
                            ddRoofSettlementOptions.SelectedIndex = -1

                            If MyBuilding.HasACVRoofSurfacing AndAlso MyBuilding.ExcludeCosmeticDamage Then
                                ddRoofSettlementOptions.SelectedValue = "BOTH"
                            ElseIf MyBuilding.HasACVRoofSurfacing Then
                                ddRoofSettlementOptions.SelectedValue = "ACV"
                            ElseIf MyBuilding.ExcludeCosmeticDamage Then
                                ddRoofSettlementOptions.SelectedValue = "EXCLUDE"
                            End If

                        End If
                        Exit Select
                    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                        ' Indiana - Show ACV Roofing
                        trACVRoofing.Attributes.Add("style", "display:''")
                        chkACVRoofing.Checked = MyBuilding.HasACVRoofing
                        Exit Select
                    Case Else
                        Throw New Exception("Building Info Populate: unsupported state")
                End Select

                ' MINE SUBSIDENCE
                'chkMineSubsidence.Enabled = True
                If MyBuilding.HasMineSubsidence = True Then
                    chkMineSubsidence.Checked = True
                End If

                ' For Ohio, certain counties are required to have mine subsidence and it cannot be removed by the user.
                ' Only certain building class codes are eligible for mine sub.  Some class codes will require number of units.
                ' If mine sub is required, show the OH info row as well.
                trMineSubsidenceLimitInfo.Attributes.Add("style", "display:none")
                trMineSubsidenceInfo_OH.Attributes.Add("style", "display:none")
                trMineSubsidenceNumberOfUnitsRow.Attributes.Add("style", "display:none")
                txtMineSubNumberOfUnits.Text = ""
                If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) AndAlso Quote.Locations(LocationIndex).Address.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                    Dim NumUnitsReqd As Boolean = False
                    ' Only specific dwelling class codes are eligible for mine subsidence
                    If (Not MyBuilding.ClassCode.IsNullEmptyorWhitespace) AndAlso IFM.VR.Common.Helpers.MineSubsidenceHelper.OhioBuildingClassCodeIsEligibleForMineSubsidence(MyBuilding.ClassCode, NumUnitsReqd) Then
                        Select Case IFM.VR.Common.Helpers.MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(Quote.Locations(LocationIndex).Address.County)
                            Case MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleMandatory
                                trMineSubsidenceRow.Attributes.Add("style", "display:''")
                                trMineSubsidenceInfo_OH.Attributes.Add("style", "display:''")
                                chkMineSubsidence.Checked = True
                                chkMineSubsidence.Enabled = False
                                If NumUnitsReqd Then
                                    trMineSubsidenceNumberOfUnitsRow.Attributes.Add("style", "display:''")
                                    txtMineSubNumberOfUnits.Text = MyBuilding.NumberOfUnitsPerBuilding
                                End If
                                If MyBuilding.Limit IsNot Nothing AndAlso IsNumeric(MyBuilding.Limit) Then
                                    If CDec(MyBuilding.Limit) >= 300000 Then
                                        trMineSubsidenceLimitInfo.Attributes.Add("style", "display:''")
                                    End If
                                End If
                                Exit Select
                            Case Else
                                ' Hide Mine Sub for non mine sub req'd counties
                                trMineSubsidenceRow.Attributes.Add("style", "display:none")
                                trMineSubsidenceNumberOfUnitsRow.Attributes.Add("style", "display:none")
                                chkMineSubsidence.Checked = False
                        End Select
                    Else
                        ' Ohio building is not eligible for mine subsidence
                        trMineSubsidenceRow.Attributes.Add("style", "display:none")
                    End If
                End If

                'txtFeetToHydrant.Text = MyBuilding.FeetToFireHydrant
                'txtMilesToFireDepartment.Text = MyBuilding.MilesToFireDepartment

                'LoadProtectionClasses()

                'If ddlProtectionClass.Items.Count > 0 AndAlso IsNumeric(MyBuilding.ProtectionClassId) Then
                '    'ddlProtectionClass.SelectedValue = MyBuilding.ProtectionClassId
                '    ddlProtectionClass.SetFromValue(MyBuilding.ProtectionClassId)
                'End If

                If NewCoProtectionClassHelper.IsNewCoProtectionClassAvailable(Quote) Then
                    rowFeetToHydrant.Visible = False
                    rowMilesToFireDepartment.Visible = False
                    rowProtectionClass.Visible = False
                Else
                    txtFeetToHydrant.Text = MyBuilding.FeetToFireHydrant
                    txtMilesToFireDepartment.Text = MyBuilding.MilesToFireDepartment

                    LoadProtectionClasses()

                    If ddlProtectionClass.Items.Count > 0 AndAlso IsNumeric(MyBuilding.ProtectionClassId) Then
                        'ddlProtectionClass.SelectedValue = MyBuilding.ProtectionClassId
                        ddlProtectionClass.SetFromValue(MyBuilding.ProtectionClassId)
                    End If


                End If



                Me.PopulateChildControls()
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
        End Try
    End Sub

    Public Overrides Function Save() As Boolean
        Try
            'Added 10/18/2021 for BOP Endorsements task 61660 MLW
            Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
            If Not endorsementsPreexistHelper.IsPreexistingLocation(LocationIndex) Then
                If MyBuilding IsNot Nothing Then
                    MyBuilding.UseBuildingClassificationPropertiesToCreateOneItemInList = False ' Only needed when using comp rater - defaulted to false in Diamond services
                    MyBuilding.Description = txtDescription.Text
                    MyBuilding.OccupancyId = ddlOccupancy.SelectedValue
                    MyBuilding.ConstructionId = ddlConstruction.SelectedValue
                    MyBuilding.AutoIncreaseId = ddlAutomaticIncrease.SelectedValue
                    If BuildingIndex = 0 Then
                        ' Set the Location's property deductible to whatever the building is set to if this is Building 0 on the current location
                        Quote.Locations(LocationIndex).PropertyDeductibleId = ddlPropertyDeductible.SelectedValue
                    End If

                    ' Always set the building property deductible to whatever the location that the building is attached to is set to
                    MyBuilding.PropertyDeductibleId = Quote.Locations(LocationIndex).PropertyDeductibleId
                    MyBuilding.Limit = txtBuildingLimit.Text
                    MyBuilding.ValuationId = ddlBuildingValuation.SelectedValue
                    MyBuilding.HasACVRoofing = chkACVRoofing.Checked
                    MyBuilding.IsBuildingValIncludedInBlanketRating = chkBuildingValuationIncludedInBlanketRating.Checked
                    MyBuilding.HasSprinklered = chkSprinklered.Checked
                    MyBuilding.PersonalPropertyLimit = txtPersonalPropertyLimit.Text
                    MyBuilding.ValuationMethodId = ddlValuationMethod.SelectedValue
                    MyBuilding.IsValMethodIncludedInBlanketRating = chkValuationMethodIncludedInBlanketRating.Checked

                    If Not NewCoProtectionClassHelper.IsNewCoProtectionClassAvailable(Quote) Then
                        MyBuilding.FeetToFireHydrant = txtFeetToHydrant.Text
                        MyBuilding.MilesToFireDepartment = txtMilesToFireDepartment.Text

                        MyBuilding.ProtectionClassId = ddlProtectionClass.SelectedValue
                    End If


                    ' Set these roofing options all to false and then set them all to theor values below
                    MyBuilding.HasACVRoofing = False
                    MyBuilding.HasACVRoofSurfacing = False
                    MyBuilding.ExcludeCosmeticDamage = False
                    MyBuilding.HasLimitationsOnRoofSurfacing = False
                    Select Case Quote.Locations(LocationIndex).Address.QuickQuoteState
                        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                            ' Illinois & Ohio
                            ' Always set the mine subsidence to whatever the checkbox is set to
                            MyBuilding.HasMineSubsidence = chkMineSubsidence.Checked
                            MyBuilding.NumberOfUnitsPerBuilding = ""
                            If chkMineSubsidence.Checked Then
                                ' If this is an Ohio building and it has the correct class code (69145) and is mine sub mandatory
                                ' then set the number of units value
                                If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) AndAlso Quote.Locations(LocationIndex).Address.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                                    Dim NumUnitsReqd As Boolean = False
                                    If IFM.VR.Common.Helpers.MineSubsidenceHelper.OhioBuildingClassCodeIsEligibleForMineSubsidence(MyBuilding.ClassCode, NumUnitsReqd) Then
                                        If NumUnitsReqd Then MyBuilding.NumberOfUnitsPerBuilding = txtMineSubNumberOfUnits.Text
                                    End If
                                End If
                            End If
                            ' Set the Limitations on Roof Surfacing & ACV Roofing values
                            MyBuilding.HasLimitationsOnRoofSurfacing = chkLimitationsOnRoofSurfacing.Checked
                            Select Case ddRoofSettlementOptions.SelectedValue
                                Case "ACV"
                                    MyBuilding.HasACVRoofSurfacing = True
                                    Exit Select
                                Case "EXCLUDE"
                                    MyBuilding.ExcludeCosmeticDamage = True
                                    Exit Select
                                Case "BOTH"
                                    MyBuilding.HasACVRoofSurfacing = True
                                    MyBuilding.ExcludeCosmeticDamage = True
                                    Exit Select
                            End Select
                            Exit Select
                        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                            ' Indiana
                            'Only set the mine subsidence for eligible counties, otherwise turn it off
                            If IFM.VR.Common.Helpers.MineSubsidenceHelper.MineSubCountiesByStateAbbreviation("IN").Contains(Quote.Locations(LocationIndex).Address.County) Then
                                MyBuilding.HasMineSubsidence = chkMineSubsidence.Checked
                            Else
                                MyBuilding.HasMineSubsidence = False
                            End If
                            ' Set the ACV Roofing & Limitations on Roof Surfacing values
                            MyBuilding.HasACVRoofing = chkACVRoofing.Checked
                            Exit Select
                    End Select
                End If

                Me.SaveChildControls()

                'Populate() ' Never can populate inside the save! if you need to populate after a save then call it explicitly after the save_fireevent() returns - Matt A 5-18-17
            End If
            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Try
            'Added 10/18/2021 for BOP Endorsements task 61660 MLW
            Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
            If Not endorsementsPreexistHelper.IsPreexistingLocation(LocationIndex) Then
                MyBase.ValidateControl(valArgs)
                Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

                ValidationHelper.GroupName = "Location #" & LocationIndex + 1 & ", Building #" & BuildingIndex + 1 & " Building Information"


                If ddlOccupancy.SelectedIndex <= 0 Then
                    Me.ValidationHelper.AddError(ddlOccupancy, "Missing Occupancy", accordList)
                End If
                If Me.ddlConstruction.SelectedIndex <= 0 Then
                    Me.ValidationHelper.AddError(ddlConstruction, "Missing Construction", accordList)
                End If

                If lblBuildingLimit.Text.Substring(0, 1) = "*" Then
                    If txtBuildingLimit.Text = String.Empty Then
                        Me.ValidationHelper.AddError(txtBuildingLimit, "Missing Building Limit", accordList)
                    End If
                End If
                If txtBuildingLimit.Text <> String.Empty Then
                    If Not IsNumeric(txtBuildingLimit.Text) Then
                        Me.ValidationHelper.AddError(txtBuildingLimit, "Building Limit must be numeric", accordList)
                    ElseIf CDec(txtBuildingLimit.Text) <= 0 Then
                        Me.ValidationHelper.AddError(txtBuildingLimit, "Building Limit must be greater than 0", accordList)
                    End If
                End If

                If lblPersonalPropertyLimit.Text.Substring(0, 1) = "*" Then
                    If txtPersonalPropertyLimit.Text = String.Empty Then
                        Me.ValidationHelper.AddError(txtPersonalPropertyLimit, "Missing Personal Property Limit", accordList)
                    End If
                End If
                If txtPersonalPropertyLimit.Text <> String.Empty Then
                    If Not IsNumeric(txtPersonalPropertyLimit.Text) Then
                        Me.ValidationHelper.AddError(txtPersonalPropertyLimit, "Personal Property Limit must be numeric", accordList)
                    ElseIf CDec(txtPersonalPropertyLimit.Text) <= 0 Then
                        Me.ValidationHelper.AddError(txtPersonalPropertyLimit, "Personal Property Limit must be greater than 0", accordList)
                    End If
                End If
                'If ddlProtectionClass.SelectedIndex <= 0 Then
                '    Me.ValidationHelper.AddError(ddlProtectionClass, "Missing Protection Class", accordList)
                'End If

                If Not NewCoProtectionClassHelper.IsNewCoProtectionClassAvailable(Quote) Then
                    If ddlProtectionClass.SelectedIndex <= 0 Then
                        Me.ValidationHelper.AddError(ddlProtectionClass, "Missing Protection Class", accordList)
                    End If
                End If

                ' Limitations on Roof Surfacing validation MGB 10/30/2018
                If Quote.Locations(LocationIndex).Address.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois Then
                    If chkLimitationsOnRoofSurfacing.Checked Then
                        If ddRoofSettlementOptions.SelectedIndex = 0 Then
                            Me.ValidationHelper.AddError(ddRoofSettlementOptions, "Missing Roof Settlement Options", accordList)
                        End If
                    End If
                End If

                ' Mine sub number of units
                ' Only required on mine sub eligible buildings with specific class codes
                If IFM.VR.Common.Helpers.MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(Quote.Locations(LocationIndex).Address.County) = MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleMandatory Then
                    Dim NumUnitsReqd As Boolean = False
                    If IFM.VR.Common.Helpers.MineSubsidenceHelper.OhioBuildingClassCodeIsEligibleForMineSubsidence(MyBuilding.ClassCode, NumUnitsReqd) Then
                        If NumUnitsReqd Then
                            If txtMineSubNumberOfUnits.Text.IsNullEmptyorWhitespace Then
                                Me.ValidationHelper.AddError(txtMineSubNumberOfUnits, "Number Of Units per Building required for Class Code 69145", accordList)
                            Else
                                If Not txtMineSubNumberOfUnits.Text.IsNumeric Then
                                    Me.ValidationHelper.AddError(txtMineSubNumberOfUnits, "Invalid Number Of Units", accordList)
                                Else
                                    If CInt(txtMineSubNumberOfUnits.Text) <= 0 OrElse CInt(txtMineSubNumberOfUnits.Text) > 4 Then
                                        Me.ValidationHelper.AddError(txtMineSubNumberOfUnits, "Number of Units must be between 1 and 4", accordList)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                Me.ValidateChildControls(valArgs)
            End If
            Exit Sub

        Catch ex As Exception
            HandleError("ValidateControls", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub LoadProtectionClasses()
        Dim county As String = Nothing
        Dim city As String = Nothing
        Dim ds As DataSet = Nothing
        Dim FTH As String = Nothing
        Dim MTFD As String = Nothing
        Dim StID As String = Nothing

        Try
            ' Get the values to pass to the protection class lookup
            county = Quote.Locations(LocationIndex).Address.County
            city = Quote.Locations(LocationIndex).Address.City
            FTH = txtFeetToHydrant.Text
            MTFD = txtMilesToFireDepartment.Text
            StID = Quote.Locations(LocationIndex).Address.StateId


            ' 1-30-2013 MGB Pull protection class by the entered city. 
            ' If no results by city, pull protection classes by county
            ds = GetProtectionClasses("CITY", city, StID, FTH, MTFD)
            If ds Is Nothing OrElse ds.Tables.Count <= 0 OrElse ds.Tables(0).Rows.Count <= 0 Then
                ds = Nothing
                ds = GetProtectionClasses("COUNTY", county, StID, FTH, MTFD)
            End If

            ' If the protection class list is still empty, just exit (use the default protection classes only)
            If ds Is Nothing OrElse ds.Tables.Count <= 0 OrElse ds.Tables(0).Rows.Count <= 0 Then
                ddlProtectionClass.Items.Clear()
                LoadDefaultProtectionClassNumbers()
                Exit Sub
            End If

            ' Load the protection class list
            ddlProtectionClass.Items.Clear()
            Me.ddlProtectionClass.Items.Add(New ListItem("", ""))
            For Each dr As DataRow In ds.Tables(0).Rows
                Dim item As New ListItem()
                item.Text = dr.Item("county").ToString()
                'item.Text = dr.Item("community").ToString()
                item.Value = dr.Item("protectionclass").ToString()
                item.Attributes.Add("title", dr.Item("footnote").ToString())
                Me.ddlProtectionClass.Items.Add(item)
            Next
            ' Load the defaults at the END of the list BUG 964 MGB 5/1/2013
            LoadDefaultProtectionClassNumbers()

            If ddlProtectionClass.Items.Count = 1 Then
                ' no county selected or something went wrong
                LoadDefaultProtectionClassNumbers()
            End If

            Exit Sub
        Catch ex As Exception
            LoadDefaultProtectionClassNumbers()
        End Try
    End Sub

    Private Function GetProtectionClasses(ByVal CityOrCounty As String, ByVal CityOrCountyName As String, ByVal StateId As String, Optional ByVal FeetToHydrant As String = Nothing, Optional ByVal MilesToFireDept As String = Nothing) As DataSet
        Dim td As New DataTable()
        td.Columns.Add(New DataColumn("community"))
        td.Columns.Add(New DataColumn("county"))
        td.Columns.Add(New DataColumn("protectionclass"))
        td.Columns.Add(New DataColumn("footnote"))

        Try
            ' Set defaults MGB 8/21/17
            If MilesToFireDept Is Nothing OrElse MilesToFireDept.Trim = "" Then MilesToFireDept = "5"
            If FeetToHydrant Is Nothing OrElse FeetToHydrant.Trim = "" Then FeetToHydrant = "1000"

            Using conn As New SqlConnection(AppSettings("ConnDiamond"))
                Using cmd As New SqlCommand()
                    conn.Open()
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "assp_ProtectionClass_Search"
                    Select Case CityOrCounty.ToUpper()
                        Case "CITY"
                            cmd.Parameters.AddWithValue("@SearchType", 1)     ' 0 = County, 1 = Community
                            Exit Select
                        Case "COUNTY"
                            cmd.Parameters.AddWithValue("@SearchType", 0)     ' 0 = County, 1 = Community
                            Exit Select
                        Case Else
                            Return Nothing
                    End Select
                    cmd.Parameters.AddWithValue("@SearchText", CityOrCountyName)
                    cmd.Parameters.AddWithValue("@stateID", StateId)
                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                Dim township As String = reader.GetString(1)
                                Dim county As String = reader.GetString(2)
                                Dim returnedProtectionClass As String = reader.GetString(3)
                                Dim footnote As String = reader.GetString(5)

                                If returnedProtectionClass.Contains("*") Then
                                    Dim newRow As DataRow = td.NewRow()
                                    newRow("community") = township + "(10)"
                                    newRow("county") = county + "(10)"
                                    newRow("protectionclass") = GetProtectionClassIdFromProtectionClassNumber("10")
                                    newRow("footnote") = footnote
                                    td.Rows.Add(newRow)
                                Else
                                    If returnedProtectionClass.Contains("/") Then
                                        ' is split protection class
                                        Dim LeftVal As String = returnedProtectionClass.Split(CChar("/"))(0)
                                        Dim RightVal As String = returnedProtectionClass.Split(CChar("/"))(1)
                                        Dim newRow As DataRow = td.NewRow()
                                        'see if miles is < 5 but > 0 and feet is < 1000 but > 0
                                        If MilesToFireDept <= 5 And MilesToFireDept > 0 And FeetToHydrant <= 1000 And FeetToHydrant > 0 Then
                                            ' use lower val
                                            newRow("community") = String.Format("{0} ({1})", township, LeftVal.PadLeft(2, CChar("0")))
                                            newRow("county") = String.Format("{0} ({1})", county, LeftVal.PadLeft(2, CChar("0")))
                                            newRow("protectionclass") = GetProtectionClassIdFromProtectionClassNumber(LeftVal.PadLeft(2, CChar("0")))
                                        Else
                                            ' maybe miles < 5 but > 0  and Hydrant > 1000 or it doesn't have Hydrant at all
                                            If MilesToFireDept <= 5 And MilesToFireDept > 0 Then
                                                'use higher
                                                newRow("community") = String.Format("{0} ({1})", township, RightVal.PadLeft(2, CChar("0")))
                                                newRow("coounty") = String.Format("{0} ({1})", county, RightVal.PadLeft(2, CChar("0")))
                                                newRow("protectionclass") = GetProtectionClassIdFromProtectionClassNumber(RightVal.PadLeft(2, CChar("0")))
                                            Else
                                                ' miles is > 5 so it must return a 10 regardless of hydrant
                                                newRow("community") = String.Format("{0} ({1})", township, "10")
                                                newRow("county") = String.Format("{0} ({1})", county, "10")
                                                newRow("protectionclass") = GetProtectionClassIdFromProtectionClassNumber("10")
                                            End If

                                        End If
                                        newRow("footnote") = footnote
                                        td.Rows.Add(newRow)
                                    Else
                                        ' single selection hydrant and miles make no difference
                                        Dim newRow As DataRow = td.NewRow()
                                        newRow("community") = String.Format("{0} ({1})", township, returnedProtectionClass.PadLeft(2, CChar("0")))
                                        newRow("county") = String.Format("{0} ({1})", county, returnedProtectionClass.PadLeft(2, CChar("0")))
                                        newRow("protectionclass") = GetProtectionClassIdFromProtectionClassNumber(returnedProtectionClass.PadLeft(2, CChar("0")))
                                        newRow("footnote") = footnote
                                        td.Rows.Add(newRow)
                                    End If
                                End If
                            End While
                            Dim ds As New DataSet("ds1")
                            ds.Tables.Add(td)
                            Return ds
                        End If
                    End Using
                End Using
            End Using
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function GetProtectionClassIdFromProtectionClassNumber(ByVal num As String) As String
        Select Case num.ToLower()
            Case "01"
                Return "12"
            Case "02"
                Return "13"
            Case "03"
                Return "14"
            Case "04"
                Return "15"
            Case "05"
                Return "16"
            Case "06"
                Return "17"
            Case "07"
                Return "18"
            Case "08"
                Return "19"
            Case "8b"
                Return "20"
            Case "09"
                Return "21"
            Case "10"
                Return "22"
            Case Else
                Return ""
        End Select
    End Function

    Public Sub LoadDefaultProtectionClassNumbers()
        Me.ddlProtectionClass.Items.Add(New ListItem("01", "12"))
        Me.ddlProtectionClass.Items.Add(New ListItem("02", "13"))
        Me.ddlProtectionClass.Items.Add(New ListItem("03", "14"))
        Me.ddlProtectionClass.Items.Add(New ListItem("04", "15"))
        Me.ddlProtectionClass.Items.Add(New ListItem("05", "16"))
        Me.ddlProtectionClass.Items.Add(New ListItem("06", "17"))
        Me.ddlProtectionClass.Items.Add(New ListItem("07", "18"))
        Me.ddlProtectionClass.Items.Add(New ListItem("08", "19"))
        Me.ddlProtectionClass.Items.Add(New ListItem("8B", "20"))
        Me.ddlProtectionClass.Items.Add(New ListItem("09", "21"))
        Me.ddlProtectionClass.Items.Add(New ListItem("10", "22"))
        Me.ddlProtectionClass.ToolTip = "All Protection Classes"


        'Added 09/16/2021 for BOP Endorsements Task 61276 MLW
        Dim AllPreExistingItems = New AllPreExistingItems()
        Dim PreExistingFlag As Boolean
        AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
        If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) Then
            Dim MyLocation = Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex)
            PreExistingFlag = AllPreExistingItems.PreExisting_Locations.isPreExistingLocationByLocationObject(MyLocation)
        End If

        If PreExistingFlag Then
            Me.ddlProtectionClass.Items.Add(New ListItem("1X", "23"))
            Me.ddlProtectionClass.Items.Add(New ListItem("2X", "24"))
            Me.ddlProtectionClass.Items.Add(New ListItem("3X", "25"))
            Me.ddlProtectionClass.Items.Add(New ListItem("4X", "26"))
            Me.ddlProtectionClass.Items.Add(New ListItem("5X", "27"))
            Me.ddlProtectionClass.Items.Add(New ListItem("6X", "28"))
            Me.ddlProtectionClass.Items.Add(New ListItem("7X", "29"))
            Me.ddlProtectionClass.Items.Add(New ListItem("8X", "30"))
            Me.ddlProtectionClass.Items.Add(New ListItem("1Y", "31"))
            Me.ddlProtectionClass.Items.Add(New ListItem("2Y", "32"))
            Me.ddlProtectionClass.Items.Add(New ListItem("3Y", "33"))
            Me.ddlProtectionClass.Items.Add(New ListItem("4Y", "34"))
            Me.ddlProtectionClass.Items.Add(New ListItem("5Y", "35"))
            Me.ddlProtectionClass.Items.Add(New ListItem("6Y", "36"))
            Me.ddlProtectionClass.Items.Add(New ListItem("7Y", "37"))
            Me.ddlProtectionClass.Items.Add(New ListItem("8Y", "38"))
            Me.ddlProtectionClass.Items.Add(New ListItem("10W", "39"))
        End If

    End Sub

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            AddHandler Me.ctlClassificationsList.BuildingClassificationChanged, AddressOf ClassificationChanged
            Me.MainAccordionDivId = Me.divMain.ClientID
            If Not IsPostBack Then
            End If
        Catch ex As Exception
            HandleError("Page LOAD", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub ClassificationChanged(ByVal LocNdx As Integer, ByVal BldNdx As Integer, ByVal ClsNdx As Integer, ByVal NewClassCode As String)
        RaiseEvent BuildingClassificationChanged(LocNdx, BldNdx, ClsNdx, NewClassCode)
    End Sub

    Private Sub ddlOccupancy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlOccupancy.SelectedIndexChanged
        Try
            If ddlOccupancy.SelectedIndex < 0 Then Exit Sub

            lblBuildingLimit.Text = "Building Limit"
            lblPersonalPropertyLimit.Text = "Personal Property Limit"

            Select Case ddlOccupancy.SelectedValue
                Case "16" 'Non-Owner Occupied Bldg / Lessor's
                    ' Building Limit Only
                    lblBuildingLimit.Text = "*Building Limit"
                    Exit Select
                'Case "17" 'Insured owns and occupies 1-75% of the building
                '    ' Building Limit & Personal Property Limit
                '    lblBuildingLimit.Text = "*Building Limit"
                '    lblPersonalPropertyLimit.Text = "*Personal Property Limit"
                '    Exit Select
                'Case "18" 'Insured owns and occupies 76-100% of the building
                '    ' Building Limit & Personal Property Limit
                '    lblBuildingLimit.Text = "*Building Limit"
                '    lblPersonalPropertyLimit.Text = "*Personal Property Limit"
                '    Exit Select
                'Case "19" 'Insured is the tenant and does not own the building
                '    ' Personal Property Limit Only
                '    lblPersonalPropertyLimit.Text = "*Personal Property Limit"
                '    Exit Select
                Case 20 'Owner Occupied Bldg 10% or Less / Lessor's
                    ' Personal Property & Building Limit
                    lblPersonalPropertyLimit.Text = "*Personal Property Limit"
                    lblBuildingLimit.Text = "*Building Limit"
                    Exit Select
                Case 21 'Owner Occupied Bldg More than 10% / Occupant
                    ' Personal Property & Building Limit
                    lblPersonalPropertyLimit.Text = "*Personal Property Limit"
                    lblBuildingLimit.Text = "*Building Limit"
                    Exit Select
                Case 19 'Tenant / Occupant
                    ' Personal Property
                    lblPersonalPropertyLimit.Text = "*Personal Property Limit"
                    Exit Select
            End Select

            Exit Sub
        Catch ex As Exception
            HandleError("ddlOccupancy_SelectedIndexChanged", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
        Me.Populate() 'Added 12/31/18 for Bug 30676 MLW
    End Sub

    Private Sub lnkClear_Command(sender As Object, e As CommandEventArgs) Handles lnkClear.Command
        Try
            txtDescription.Text = String.Empty
            ddlOccupancy.SelectedIndex = -1
            ddlConstruction.SelectedIndex = -1
            ddlAutomaticIncrease.SelectedIndex = 1
            ddlPropertyDeductible.SelectedValue = "22" '500
            txtBuildingLimit.Text = String.Empty
            ddlBuildingValuation.SelectedIndex = -1
            chkACVRoofing.Checked = False
            chkBuildingValuationIncludedInBlanketRating.Checked = False
            If chkMineSubsidence.Enabled = True Then 'added IF 11/20/2018 so user can't remove mine sub if we've added it automatically
                chkMineSubsidence.Checked = False
            End If
            chkSprinklered.Checked = False
            txtPersonalPropertyLimit.Text = String.Empty
            ddlValuationMethod.SelectedIndex = -1
            chkValuationMethodIncludedInBlanketRating.Checked = False
            txtFeetToHydrant.Text = String.Empty
            txtMilesToFireDepartment.Text = String.Empty
            ddlProtectionClass.SelectedIndex = -1

            Exit Sub
        Catch ex As Exception
            HandleError("lnkClear_Command", ex)
            Exit Sub
        End Try
    End Sub
#End Region



End Class