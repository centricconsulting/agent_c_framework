Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers

Public Class ctlResidence
    Inherits VRControlBase

    Public Event HideDwelling()
    Public Event RaiseResidenceExists(state As Boolean)

    Public ReadOnly Property YearBuiltID() As String
        Get
            Return Me.txtYearBuilt.ClientID
        End Get
    End Property

    Public ReadOnly Property UsageTypeID() As String
        Get
            Return Me.ddlUsageType.ClientID
        End Get
    End Property

    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public Property SetCoverageALimit() As String
        Get
            Return ctlResidenceCoverages.SetHiddenCovALimit
        End Get
        Set(ByVal value As String)
            ctlResidenceCoverages.SetHiddenCovALimit = value
        End Set
    End Property

    Public Property SetCoverageATotal() As String
        Get
            Return ctlResidenceCoverages.SetHiddenCovATotal
        End Get
        Set(ByVal value As String)
            ctlResidenceCoverages.SetHiddenCovATotal = value
        End Set
    End Property

    Public Property SetCovCReplaceCost() As String
        Get
            Return ctlResidenceCoverages.SetReplacementCostCovC
        End Get
        Set(ByVal value As String)
            ctlResidenceCoverages.SetReplacementCostCovC = value
        End Set
    End Property

    Public Property ResidenceExists() As Boolean
        Get
            Return ViewState.GetBool("vs_ResidenceExists", False, True)
        End Get
        Set(ByVal value As Boolean)
            ViewState("vs_ResidenceExists") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Locations.GetItemAtIndex(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    'added 11/22/17 for HOM Upgrade - MLW
    Dim _qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Protected ReadOnly Property HomeVersion As String
        Get
            Dim effectiveDate As DateTime
            If Me.Quote IsNot Nothing Then
                If Me.Quote.EffectiveDate IsNot Nothing AndAlso Me.Quote.EffectiveDate <> String.Empty Then
                    effectiveDate = Me.Quote.EffectiveDate
                Else
                    effectiveDate = Now()
                End If
            Else
                effectiveDate = Now()
            End If
            If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property

    Private Sub GetYearBuilt() Handles ctlResidenceCoverages.GetYearBult
        ctlResidenceCoverages.YearBuilt = txtYearBuilt.Text

    End Sub

    Private Sub GetDwellingClassification() Handles ctlResidenceCoverages.GetDwellingClassification
        ctlResidenceCoverages.DwellingClassification = ddDwellingClass.SelectedValue
    End Sub

    'Removed 12/3/18 for new jQuery mine sub code MLW
    ''Added 10/25/18 for multi state MLW
    'Public Sub LoadMineSubCoverages()
    '    If MultiState.General.IsMultistateCapableEffectiveDate(Me.Quote.EffectiveDate) Then
    '        If Me.MyLocation IsNot Nothing AndAlso Me.MyLocation.Address IsNot Nothing Then
    '            Select Case (Me.MyLocation.Address.StateId)
    '                Case States.Abbreviations.IL
    '                    ctlResidenceCoverages.UpdateMineSubsidenceCheckboxScript()
    '                Case Else
    '            End Select
    '        End If
    '    End If
    'End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'dd_Residence_CoverageForm.ID = "dd_Residence_CoverageForm" + Me.MyLocationIndex.ToString()
        'dd_Residence_CoverageForm.ClientIDMode = UI.ClientIDMode.Static

        'txtYearBuilt.ID = "txtYearBuilt" + Me.MyLocationIndex.ToString()
        'txtYearBuilt.ClientIDMode = UI.ClientIDMode.Static
        'updated 3/3/2021
        SetControlIdsOnChild()

        'removed 3/3/2021; not sure why this would be needed here since it's being done at Populate... may have been needed previously w/ static ids?
        'If Me.Quote IsNot Nothing Then
        '    If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
        '        If Me.MyLocation IsNot Nothing Then
        '            Helpers.WebHelper_Personal.SetdropDownFromValue(Me.dd_Residence_CoverageForm, Me.MyLocation.FormTypeId)
        '            txtYearBuilt.Text = MyLocation.YearBuilt
        '        End If
        '    End If
        'End If


        Me.MainAccordionDivId = Me.ResidenceDiv.ClientID
        ctlResidenceCoverages.ResidenceExists = ResidenceExists
        RaiseEvent RaiseResidenceExists(ResidenceExists)

        If Not IsPostBack Then

        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(MainAccordionDivId, HiddenField1, "0")
        Me.VRScript.StopEventPropagation(Me.lnkDeleteResidence.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkClearResidence.ClientID, False)

        Select Case Me.Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.Farm
                Me.VRScript.CreateConfirmDialog(Me.lnkClearResidence.ClientID, "Clear Dwelling?")
            Case Else
                Me.VRScript.CreateConfirmDialog(Me.lnkClearResidence.ClientID, "Clear Residence?")
        End Select

        Me.VRScript.StopEventPropagation(Me.lnkSaveResidence.ClientID)

        Me.VRScript.CreateTextBoxFormatter(Me.txtYearBuilt.ClientID, ctlPageStartupScript.FormatterType.PositiveNumberNoCommas, ctlPageStartupScript.JsEventType.onblur)
        Me.VRScript.CreateTextBoxFormatter(Me.txtSqrFeet.ClientID, ctlPageStartupScript.FormatterType.NumericWithCommas, ctlPageStartupScript.JsEventType.onkeyup)

        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then

            Dim chc As New CommonHelperClass
            Dim isVerisk360Enabled As String = chc.ConfigurationAppSettingValueAsBoolean("VR_AllLines_Site360Valuation_Settings")
            Dim scriptSetDwellingClass As String = "SetDwellingClass(""" + dd_Residence_CoverageForm.ClientID + """, """ + ddlStructure.ClientID + """,""" + ddDwellingClass.ClientID + """ , """ + ctlResidenceCoverages.dvReplacementCC_ClientId + """, """ + isVerisk360Enabled + """);"
            'ddlStructure.Attributes.Add("onchange", scriptSetDwellingClass)

            Me.VRScript.AddScriptLine(scriptSetDwellingClass)
            Me.VRScript.CreateJSBinding(Me.ddlStructure, ctlPageStartupScript.JsEventType.onchange, scriptSetDwellingClass, True)

            Dim scriptSetPrivateLightPolesIncludedLimit As String = "SetPrivateLightPolesIncludedLimit(""" + dd_Residence_CoverageForm.ClientID + """,""" + ctlResidenceCoverages.txtPrivatePowerPolesIncludedLimit_ClientId + """,""" + ctlResidenceCoverages.txtPrivatePowerPolesIncreaseLimit_ClientId + """,""" + ctlResidenceCoverages.txtPrivatePowerPolesTotalLimit_ClientId + """);TogglePrivateLightPoles(""" + dd_Residence_CoverageForm.ClientID + """,""" + ctlResidenceCoverages.dvPrivatePowerPoles_ClientId + """,""" + ctlResidenceCoverages.chkPrivatePowerPoles_ClientId + """,""" + ctlResidenceCoverages.tblPrivatePowerPoles_ClientId + """);"
            dd_Residence_CoverageForm.Attributes.Add("onchange", scriptSetPrivateLightPolesIncludedLimit)

            'Added 10/5/2022 for bug 75312 MLW
            If Quote.Locations(0).HobbyFarmCredit = True Then
                Dim disableOption As Boolean = False
                If IsEndorsementRelated() Then
                    If IsQuoteEndorsement() = True AndAlso IsPreexistingLocationOnEndorsement(MyLocationIndex) = False Then
                        'Disable for endorsement change for new (added) locations
                        disableOption = True
                    ElseIf IsQuoteEndorsement() = True AndAlso IsPreexistingLocationOnEndorsement(MyLocationIndex) = True AndAlso MyLocation.FormTypeId <> "17" Then
                        'disable for endorsement change for existing locations that do not have FO-4 already selected
                        disableOption = True
                    End If
                Else
                    'Disable for new business
                    disableOption = True
                End If
                If disableOption = True Then
                    Dim scriptDisableFO4OnHobbyFarm As String = "DisableCoverageFormFO4(""" + dd_Residence_CoverageForm.ClientID + """);"
                    dd_Residence_CoverageForm.Attributes.Add("onclick", scriptDisableFO4OnHobbyFarm)
                End If
            End If
        End If

        'added 10/11/17 for HOM Upgrade MLW - updated 12/13/17 for HOM Upgrade MLW
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
            Dim scriptUnitsInFireDivision As String = "var ddStr = document.getElementById('" & ddlStructureLeft.ClientID & "');var ddStrText = ddStr.options[ddStr.selectedIndex].text; var ddUnits = document.getElementById('" & trUnitsInFireDivision.ClientID & "');if (ddStrText == ""TOWNHOUSE"" || ddStrText == 'ROWHOUSE') {ddUnits.style.display = '';}else {ddUnits.style.display = 'none';}"
            ddlStructureLeft.Attributes.Add("onchange", scriptUnitsInFireDivision)

            'Dim scriptRelatedPolicyNumber As String = "var ddOcc = document.getElementById('" & ddlOccupancy.ClientID & "'); var ddOccText = ddOcc.options[ddOcc.selectedIndex].text; var txtNum = document.getElementById('" & trRelatedPolicyNumber.ClientID & "');if (ddOccText == 'SECONDARY' || ddOccText == 'SEASONAL') {txtNum.style.display = '';}else {txtNum.style.display = 'none';}"
            Dim scriptOccChangeMessage As String = String.Empty
            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal And HOM.FamilyCyberDateHelper.IsPostCyberDate(Quote) Then
                scriptOccChangeMessage = "alert('Please note we do not provide Family Cyber Coverage or include our Home Plus Enhancement Endorsement for seasonal or secondary homes.  Extend the liability coverage to seasonal and secondary homes by endorsing the homeowner’s primary policy to include seasonal and secondary homes also occupied by the insured.');"
            End If

            Dim scriptRelatedPolicyNumber As String = "var ddOcc = document.getElementById('" & ddlOccupancy.ClientID & "');  var ddOccText = ddOcc.options[ddOcc.selectedIndex].text;  var txtNum = document.getElementById('" & trRelatedPolicyNumber.ClientID & "'); if (ddOccText == 'SECONDARY' || ddOccText == 'SEASONAL')  { txtNum.style.display = ''; " & scriptOccChangeMessage & " } else  { txtNum.style.display = 'none'; }"
            ddlOccupancy.Attributes.Add("onchange", scriptRelatedPolicyNumber)

        End If
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal AndAlso IsEndorsementRelated() = False Then
            Dim scriptToggleOccupancy As String = "ToggleOccupancy('" & ddlStructureLeft.ClientID & "','" & ddlOccupancy.ClientID & "');"
            ddlStructureLeft.Attributes.Add("onchange", scriptToggleOccupancy)
            Dim scriptToggleStructure As String = "ToggleStructure('" & ddlStructureLeft.ClientID & "','" & ddlOccupancy.ClientID & "');"
            ddlOccupancy.Attributes.Add("onchange", scriptToggleStructure)
        End If
        'Added 07/11/2023 for task WS-1286
        If IsEndorsementRelated() = False AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso Me.MyLocation.FormTypeId.EqualsAny("22", "23", "24") AndAlso IFM.VR.Common.Helpers.HOM.HOMDwellingAgeHelper.IsHomeDwellingTextAvailable(Quote) = True  Then
            Dim scriptCalculateStructureAge As String = "calculateStructureAge(""" + txtYearBuilt.ClientID + """,""" + divDwellingAgeText.ClientID + """);"
            txtYearBuilt.Attributes.Add("onblur", scriptCalculateStructureAge)
        End If

        txtYearBuilt.Attributes.Add("onfocus", "this.select()")
        txtSqrFeet.Attributes.Add("onfocus", "this.select()")
    End Sub

    Public Overrides Sub LoadStaticData()
        'residence
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddlNumberOfFamilies, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.NumberOfFamiliesId, SortBy.None, Me.Quote.LobType)
        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddlStructureLeft, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.StructureTypeId, SortBy.None, Me.Quote.LobType)
        Else
            'Updated 12/13/17 for HOM Upgrade MLW
            If (Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                'uses left structure drop down and new values - townhouse and rowhouse - MLW
                QQHelper.LoadStaticDataOptionsDropDown(Me.ddlStructureLeft, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.StructureTypeId, SortBy.None, Me.Quote.LobType)
                Select Case Me.Quote.Locations(0).FormTypeId
                    Case "22"
                        'only show townhouse and rowhouse options on new HO-2 home, not mobile - MLW
                        If Me.Quote.Locations(0).StructureTypeId = "2" Then
                            Dim removeTownhouse As ListItem = ddlStructureLeft.Items.FindByText("TOWNHOUSE")
                            If removeTownhouse IsNot Nothing Then
                                Me.ddlStructureLeft.Items.Remove(removeTownhouse)
                            End If
                            Dim removeRowhouse As ListItem = ddlStructureLeft.Items.FindByText("ROWHOUSE")
                            If removeRowhouse IsNot Nothing Then
                                Me.ddlStructureLeft.Items.Remove(removeRowhouse)
                            End If
                        End If
                    Case "23", "24" 'only show townhouse and rowhouse options on new HO-3 (HO 0003 and HO 0005) - MLW
                    Case Else
                        'remove townhouse and rowhouse on all other forms - MLW
                        Dim removeTownhouse As ListItem = ddlStructureLeft.Items.FindByText("TOWNHOUSE")
                        If removeTownhouse IsNot Nothing Then
                            Me.ddlStructureLeft.Items.Remove(removeTownhouse)
                        End If
                        Dim removeRowhouse As ListItem = ddlStructureLeft.Items.FindByText("ROWHOUSE")
                        If removeRowhouse IsNot Nothing Then
                            Me.ddlStructureLeft.Items.Remove(removeRowhouse)
                        End If
                End Select

                'Added 10/10/2022 for bug 76006 MLW                
                If HOM.StructureTypeManufactureHelper.IsStructureTypeManufacturedAvailable(Quote) Then
                    Dim removeMobileManufactured As ListItem = ddlStructureLeft.Items.FindByText("MOBILE MANUFACTURED")
                    If removeMobileManufactured IsNot Nothing Then
                        Me.ddlStructureLeft.Items.Remove(removeMobileManufactured)
                    End If
                Else
                    Dim removeManufactured As ListItem = ddlStructureLeft.Items.FindByText("MANUFACTURED")
                    If removeManufactured IsNot Nothing Then
                        Me.ddlStructureLeft.Items.Remove(removeManufactured)
                    End If
                End If

            Else
                QQHelper.LoadStaticDataOptionsDropDown(Me.ddlStructure, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.StructureTypeId, SortBy.None, Me.Quote.LobType)
                'remove Townhouse and Rowhouse from Structure ddl - 11/2/17 MLW
                Dim removeTownhouse As ListItem = ddlStructure.Items.FindByText("TOWNHOUSE")
                If removeTownhouse IsNot Nothing Then
                    Me.ddlStructure.Items.Remove(removeTownhouse)
                End If
                Dim removeRowhouse As ListItem = ddlStructure.Items.FindByText("ROWHOUSE")
                If removeRowhouse IsNot Nothing Then
                    Me.ddlStructure.Items.Remove(removeRowhouse)
                End If
            End If
        End If

        QQHelper.LoadStaticDataOptionsDropDown(Me.ddlOccupancy, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.OccupancyCodeId, SortBy.None, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddlConstruction, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.ConstructionTypeId, SortBy.None, Me.Quote.LobType)
        Select Case Me.Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                Dim OccupanyAllowedList As New List(Of String)
                Select Case Me.MyLocation.FormTypeId
                    Case "1", "2", "3"
                        OccupanyAllowedList.Add("")
                        OccupanyAllowedList.Add("1") 'Owner
                        OccupanyAllowedList.Add("4") 'Seasonal
                        OccupanyAllowedList.Add("5") 'Secondary
                        OccupanyAllowedList.Add("7") ' Under Construction

                    Case "4"
                        OccupanyAllowedList.Add("")
                        OccupanyAllowedList.Add("1") 'Owner
                        OccupanyAllowedList.Add("9") 'Tenant
                    Case "5"
                        'OccupanyAllowedList.Add("")
                        OccupanyAllowedList.Add("1") 'Owner
                    Case "6"
                        OccupanyAllowedList.Add("1") 'Owner
                    Case "7"
                        OccupanyAllowedList.Add("9") 'Tenant
                    Case "8", "9", "10", "11", "12"
                        'Dwelling Fire
                        OccupanyAllowedList.Add("")
                        OccupanyAllowedList.Add("14") 'Owner Occupied
                        OccupanyAllowedList.Add("15") 'Tenant Occupied
                        'Added 03/23/2020 for Home Endorsements task 45028 MLW
                        If Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly Then
                            OccupanyAllowedList.Add("6") 'Vacant
                            OccupanyAllowedList.Add("13") 'Unoccupied
                            OccupanyAllowedList.Add("7") 'Under Construction
                        End If

                        'Case "13", "15", "16", "17", "18"
                        '    OccupanyAllowedList.Add("")
                        '    If MyLocationIndex > 0 Then
                        '        OccupanyAllowedList.Add("1") 'Owner
                        '    End If
                        '    OccupanyAllowedList.Add("4") 'Seasonal
                        '    OccupanyAllowedList.Add("7") ' Under Construction


                        'added 11/14/17 Case 22 - 26 for HOM Upgrade MLW
                    Case "22" 'HO 0002
                        If Me.MyLocation.StructureTypeId = "2" Then
                            OccupanyAllowedList.Add("1") 'Owner
                        Else
                            OccupanyAllowedList.Add("")
                            OccupanyAllowedList.Add("1") 'Owner
                            OccupanyAllowedList.Add("4") 'Seasonal
                            OccupanyAllowedList.Add("5") 'Secondary
                            OccupanyAllowedList.Add("7") ' Under Construction
                        End If
                    Case "23", "24" 'HO 0003 and HO 0005
                        OccupanyAllowedList.Add("")
                        OccupanyAllowedList.Add("1") 'Owner
                        OccupanyAllowedList.Add("4") 'Seasonal
                        OccupanyAllowedList.Add("5") 'Secondary
                        OccupanyAllowedList.Add("7") ' Under Construction
                    Case "25" 'HO 0004 home and mobile
                        If Me.MyLocation.StructureTypeId = "2" Then
                            OccupanyAllowedList.Add("9") 'Tenant
                        Else
                            OccupanyAllowedList.Add("")
                            OccupanyAllowedList.Add("1") 'Owner
                            OccupanyAllowedList.Add("9") 'Tenant
                        End If
                    Case "26" 'HO 0006
                        'OccupanyAllowedList.Add("")
                        OccupanyAllowedList.Add("1") 'Owner
                        If IFM.VR.Common.Helpers.HOM.UnitOwnersRentalToOthers.IsUnitOwnersRentalToOthersAvailable(quote) Then
                            OccupanyAllowedList.Add("9") 'Tenant
                        End If
                End Select
                Dim occupancyRemoveIndex As New List(Of Int32)
                'find items that should not be in the drop down
                For i As Int32 = 0 To Me.ddlOccupancy.Items.Count - 1
                    If OccupanyAllowedList.Contains(Me.ddlOccupancy.Items(i).Value) = False Then
                        occupancyRemoveIndex.Add(i)
                    End If
                Next
                ' now remove them
                occupancyRemoveIndex.Reverse()
                For Each i As Int32 In occupancyRemoveIndex
                    Me.ddlOccupancy.Items.RemoveAt(i)
                Next

                Select Case Me.MyLocation.FormTypeId
                    Case "6", "7"
                        Me.ddlStructure.Items.Clear()
                        Me.ddlStructure.Items.Add(New ListItem("Mobile Home".ToUpper(), "2"))
                        If Not IsQuoteEndorsement() AndAlso Not IsQuoteReadOnly() Then
                            Me.MyLocation.StructureTypeId = 2
                        End If
                    Case "22", "25"
                        '11/14/17 added for HOM Upgrade MLW - updated 12/13/17 for HOM Upgrade MLW
                        If Me.MyLocation.StructureTypeId = "2" Then
                            If HomeVersion = "After20180701" Then
                                Me.ddlStructureLeft.Items.Clear()
                                Me.ddlStructureLeft.Items.Add(New ListItem("Mobile Home".ToUpper(), "2"))
                            Else
                                Me.ddlStructure.Items.Clear()
                                Me.ddlStructure.Items.Add(New ListItem("Mobile Home".ToUpper(), "2"))
                            End If
                        End If
                    Case Else
                End Select
                Select Case Me.MyLocation.FormTypeId
                    Case "6", "7"
                        Me.ddlConstruction.Items.Clear()
                        Me.ddlConstruction.Items.Add(New ListItem("Frame".ToUpper(), "1"))
                        If Not IsQuoteEndorsement() AndAlso Not IsQuoteReadOnly() Then
                            MyLocation.ConstructionTypeId = "1"
                        End If
                    Case "22", "25"
                        '11/14/17 added for HOM Upgrade MLW
                        If Me.MyLocation.StructureTypeId = "2" Then
                            Me.ddlConstruction.Items.Clear()
                            Me.ddlConstruction.Items.Add(New ListItem("Frame".ToUpper(), "1"))
                            If Not IsQuoteEndorsement() AndAlso Not IsQuoteReadOnly() Then
                                MyLocation.ConstructionTypeId = "1"
                            End If
                        End If
                    Case Else
                End Select

                If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                    QQHelper.LoadStaticDataOptionsDropDown(Me.ddlUsageType, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.UsageTypeId, SortBy.None, Me.Quote.LobType)
                End If

            Case QuickQuoteObject.QuickQuoteLobType.Farm
                Dim OccupanyAllowedList As New List(Of String)

                OccupanyAllowedList.Add("")
                'If MyLocationIndex = 0 Then
                OccupanyAllowedList.Add("1") 'Owner
                'Else
                OccupanyAllowedList.Add("4") 'Seasonal
                OccupanyAllowedList.Add("7") ' Under Construction
                'End If

                Dim occupancyRemoveIndex As New List(Of Int32)
                'find items that should not be in the drop down
                For i As Int32 = 0 To Me.ddlOccupancy.Items.Count - 1
                    If OccupanyAllowedList.Contains(Me.ddlOccupancy.Items(i).Value) = False Then
                        occupancyRemoveIndex.Add(i)
                    End If
                Next
                ' now remove them
                occupancyRemoveIndex.Reverse()
                For Each i As Int32 In occupancyRemoveIndex
                    Me.ddlOccupancy.Items.RemoveAt(i)
                Next
        End Select

        Me.ddlOccupancy.Enabled = Me.ddlOccupancy.Items.Count > 1

        If IFM.VR.Common.Helpers.HOM.UnitOwnersRentalToOthers.IsUnitOwnersRentalToOthersAvailable(quote) AndAlso Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso IsQuoteEndorsement() AndAlso Me.Quote.Locations(0).FormTypeId = "26" Then
            'Cannot change Occupancy drop down on an Endorsement for HO-6
            Me.ddlOccupancy.Enabled = False
        End If

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
            Me.ddlStructureLeft.Enabled = Me.ddlStructureLeft.Items.Count > 1
        Else
            'Updated 12/13/17 for HOM Upgrade MLW
            If (Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                Me.ddlStructureLeft.Enabled = Me.ddlStructureLeft.Items.Count > 1
            Else
                Me.ddlStructure.Enabled = Me.ddlStructure.Items.Count > 1
            End If
        End If

        Me.ddlConstruction.Enabled = Me.ddlConstruction.Items.Count > 1

        QQHelper.LoadStaticDataOptionsDropDown(Me.ddStyle, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.ArchitecturalStyle, SortBy.None, Me.Quote.LobType)
        'QQHelper.LoadStaticDataOptionsDropDown(Me.ddlNumberOfFamilies, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.NumberOfFamiliesId, SortBy.None, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.dd_Residence_CoverageForm, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.FormTypeId, SortBy.None, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddDwellingClass, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DwellingTypeId, SortBy.None, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()

        If Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.Farm Then
            Me.trUsageType.Visible = False
            Me.trStructureLeft.Visible = False
            Me.trCoverageForm.Visible = False
            Me.trDwellingClass.Visible = False
            tblResidenceCoverage.Visible = False
            ctlResidenceCoverages.ResidenceExists = ResidenceExists
        Else
            ' is a farm quote
            Me.trUsageType.Visible = False
            Me.trStructureLeft.Visible = False
            Me.trUnitsInFireDivision.Visible = False 'Added 12/13/17 for HOM Upgrade - MLW
            Me.trRelatedPolicyNumber.Visible = False 'Added 12/15/17 for HOM Upgrade - MLW
            ctlResidenceCoverages.MyLocationIndex = MyLocationIndex
            lblResidence.Text = "Dwelling"

            'Added 05/04/2020 for Task 46480 MLW
            Me.lnkFarDwellingTypeClassification.NavigateUrl = ConfigurationManager.AppSettings("VRHelpDoc_FAR_DwellingTypeClassification")

            If MyLocation.PrimaryResidence Or MyLocationIndex = 0 Then
                lblResidence.Text = lblResidence.Text & " (primary)"
            End If

            If MyLocationIndex > 0 Then
                lnkDeleteResidence.Visible = True
            End If

            'Updated 8/23/18 for multi state MLW
            If SubQuoteFirst IsNot Nothing Then
                If Me.SubQuoteFirst.ProgramTypeId <> "6" Then ' if not FO type then hide this whole control
                    Me.Visible = False
                    Return
                End If
            End If
        End If

        'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlOccupancy, MyLocation.OccupancyCodeId)
        IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlOccupancy, MyLocation.OccupancyCodeId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.OccupancyCodeId, Quote.LobType)

        txtYearBuilt.Text = Me.MyLocation.YearBuilt.ReturnEmptyIfEqualsAny("0")
        Me.txtSqrFeet.Text = Me.MyLocation.SquareFeet.ReturnEmptyIfEqualsAny("0")

        'Updated 4/25/2022 for bug 66929 MLW
        'If Me.MyLocation.NumberOfFamiliesId = "" Or Me.MyLocation.NumberOfFamiliesId = "0" Then
        '   Me.ddlNumberOfFamilies.SetFromValue("1")
        If (Me.MyLocation.NumberOfFamiliesId = "" OrElse Me.MyLocation.NumberOfFamiliesId = "0") Then
            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                If MyLocation.FormTypeId <> "13" Then '13-N/A (dwelling was removed)
                    Me.ddlNumberOfFamilies.SetFromValue("1")
                End If
            Else
                Me.ddlNumberOfFamilies.SetFromValue("1")
            End If
        Else
            'Me.ddlNumberOfFamilies.SetFromValue(Me.MyLocation.NumberOfFamiliesId)
            IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlNumberOfFamilies, Me.MyLocation.NumberOfFamiliesId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.NumberOfFamiliesId)
        End If



        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
            ddlStructureLeft.SetFromValue(Me.MyLocation.StructureTypeId)
        Else
            'Updated 12/13/17 for HOM Upgrade MLW
            If (Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                ddlStructureLeft.SetFromValue(Me.MyLocation.StructureTypeId)
            Else
                'ddlStructure.SetFromValue(Me.MyLocation.StructureTypeId)
                IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlStructure, Me.MyLocation.StructureTypeId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.StructureTypeId)
            End If
        End If

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal AndAlso IsEndorsementRelated() = False Then
            'Both MOBILE MANUFACTURED and OWNER OCCUPIED ids are 14, not a copy-paste error
            If Me.MyLocation.StructureTypeId = "14" Then
                'StructureTypeId = "14" MOBILE MANUFACTURED
                MyLocation.OccupancyCodeId = "14" 'OWNER OCCUPIED
                ddlOccupancy.Enabled = False
            Else
                ddlOccupancy.Enabled = True
            End If
            If MyLocation.OccupancyCodeId = "15" Then
                'OccupancyCodeId = "15" TENANT OCCUPIED
                If ddlStructureLeft.Items.Contains(New ListItem("MOBILE MANUFACTURED", "14")) Then
                    ddlStructureLeft.Items.FindByValue("14").Attributes.Add("disabled", "disabled")
                End If
            Else
                If ddlStructureLeft.Items.Contains(New ListItem("MOBILE MANUFACTURED", "14")) Then
                    ddlStructureLeft.Items.FindByValue("14").Attributes.Remove("disabled")
                End If
            End If
        End If

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal OrElse Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then 'this needs happen after line #6t78567
            ddlOccupancy.SetFromValue(Me.MyLocation.OccupancyCodeId) 'line #6t78567
            If String.IsNullOrWhiteSpace(OriginalOccCode.Value) Then
                OriginalOccCode.Value = (Me.MyLocation.OccupancyCodeId)
            End If


        End If

        'ddlConstruction.SetFromValue(Me.MyLocation.ConstructionTypeId)
        IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlConstruction, MyLocation.ConstructionTypeId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.ConstructionTypeId, Quote.LobType, forcedLOB:=True)


        If Me.MyLocation IsNot Nothing Then
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal

                    If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                        Me.trUsageType.Visible = True
                        Me.trStructureRight.Visible = False
                        Me.trStructureLeft.Visible = True
                        Me.trUnitsInFireDivision.Visible = False 'added 12/14/17 for HOM Upgrade
                        ddlUsageType.SetFromValue(Me.MyLocation.UsageTypeId)
                    End If

                    'Added 12/13/17 for HOM Upgrade MLW
                    If (Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        Me.trStructureRight.Visible = False
                        Me.trStructureLeft.Visible = True
                        'Updated 12/14/17 for HOM Upgrade MLW
                        If Me.MyLocation.StructureTypeId = "" Then
                            Me.trUnitsInFireDivision.Attributes.Add("style", "display:none")
                        Else
                            If Me.MyLocation.StructureTypeId = "4" Or Me.MyLocation.StructureTypeId = "5" Then
                                'townhouse or rowhouse
                                Me.trUnitsInFireDivision.Attributes.Add("style", "display:''")
                                ddlUnitsInFireDivision.SetFromValue(Me.MyLocation.NumberOfUnitsInFireDivision)
                            Else
                                Me.trUnitsInFireDivision.Attributes.Add("style", "display:none")
                            End If
                        End If
                        'Updated 12/15/17 for HOM Upgrade MLW
                        If Me.MyLocation.OccupancyCodeId = "" Then
                            Me.trRelatedPolicyNumber.Attributes.Add("style", "display:none")
                        Else
                            If Me.MyLocation.OccupancyCodeId = "4" Or Me.MyLocation.OccupancyCodeId = "5" Then
                                'seasonal or secondary
                                Me.trRelatedPolicyNumber.Attributes.Add("style", "display:''")
                                Me.txtRelatedPolicyNumber.Text = Me.MyLocation.PrimaryPolicyNumber
                            Else
                                Me.trRelatedPolicyNumber.Attributes.Add("style", "display:none")
                            End If
                        End If
                    Else
                        Me.trUnitsInFireDivision.Visible = False
                        Me.trRelatedPolicyNumber.Visible = False
                    End If

                    If Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                        Me.txtYearBuilt.Attributes.Add("autofocus", "")
                    End If

                    Select Case Me.MyLocation.FormTypeId
                        Case "4", "5", "26" 'HO 4-6
                            '11/14/17 added 26 for HOM Upgrade MLW
                            Me.trStyle.Visible = False
                        Case "6", "7" ' mobile home
                            Me.trStyle.Visible = False
                        Case "22"
                            '11/14/17 added for HOM Upgrade MLW
                            If HomeVersion = "After20180701" Then
                                If Me.MyLocation.StructureTypeId = "2" Then
                                    Me.trStyle.Visible = False
                                Else
                                    Me.trStyle.Visible = True
                                End If
                            End If
                        Case "25"
                            '11/14/17 added for HOM Upgrade MLW
                            If HomeVersion = "After20180701" Then
                                Me.trStyle.Visible = False
                            End If
                        Case Else
                            Me.trStyle.Visible = True
                    End Select
                    Select Case Me.MyLocation.FormTypeId
                        Case "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "22", "23", "24", "25", "26" 'Required - Added Ho-4(4) because diamond error said it was required
                            '11/14/17 added 22 - 26 for new form types on HOM Upgrade MLW
                            Me.lblYearBuilt.Text = "* Year Built"
                        Case Else
                            Me.lblYearBuilt.Text = "Year Built"
                    End Select
                    Select Case Me.MyLocation.FormTypeId
                        Case "1", "2", "3", "8", "9", "10", "11", "12", "23", "24" 'Required
                            '11/14/17 added 22-25 for new form types on HOM Upgrade MLW
                            Me.lblSqrFeet.Text = "* Square Feet"
                        Case "22" 'Added 12/6/17 For HOM Upgrade MLW
                            If Me.MyLocation.StructureTypeId = "2" Then
                                Me.lblSqrFeet.Text = "Square Feet"
                            Else
                                Me.lblSqrFeet.Text = "* Square Feet"
                            End If
                        Case Else
                            Me.lblSqrFeet.Text = "Square Feet"
                    End Select
                    Select Case Me.MyLocation.FormTypeId
                        Case "4"
                            Me.lblConstruction.Text = "Construction"
                        Case "25"
                            '11/14/17 added for HOM Upgrade MLW
                            If Me.MyLocation.StructureTypeId = "2" Then
                                Me.lblConstruction.Text = "*Construction"
                            Else
                                Me.lblConstruction.Text = "Construction"
                            End If
                        Case Else
                            Me.lblConstruction.Text = "*Construction"
                    End Select
                    Select Case Me.MyLocation.FormTypeId
                        Case "1", "2", "3", "8", "9", "10", "11", "12", "13", "15", "16", "17", "18", "22", "23", "24"
                            '11/14/17 added 22-24 for new form types on HOM Upgrade MLW
                            Me.ddStyle.SetFromValue(Me.MyLocation.ArchitecturalStyle)
                    End Select
                    Select Case Me.MyLocation.FormTypeId
                        Case "4" ' ho-4
                            If Me.MyLocation.OccupancyCodeId.EqualsAny("", "0") Then
                                ddlOccupancy.SetFromValue("9")
                            End If
                        Case "25" 'new HO 0004 and HO 0004 Mobile
                            '11/14/17 added for HOM Upgrade MLW
                            If Me.MyLocation.StructureTypeId <> "2" Then
                                If Me.MyLocation.OccupancyCodeId.EqualsAny("", "0") Then
                                    ddlOccupancy.SetFromValue("9")
                                End If
                            End If
                        Case "5" ' ho-6
                            If Me.MyLocation.OccupancyCodeId.EqualsAny("", "0") Then
                                ddlOccupancy.SetFromValue("1")
                            End If
                    End Select



                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    Me.dd_Residence_CoverageForm.SetFromValue(Me.MyLocation.FormTypeId)
                    'Me.ddDwellingClass.SetFromValue(Me.MyLocation.DwellingTypeId)
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddDwellingClass, Me.MyLocation.DwellingTypeId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DwellingTypeId)

                    'If MyLocation.AcreageOnly Then
                    '    txtSqrFeet.Enabled = False
                    '    ddlNumberOfFamilies.Enabled = False
                    '    ddlStructure.Enabled = False
                    '    ddlOccupancy.Enabled = False
                    '    ddlConstruction.Enabled = False
                    '    ddStyle.Enabled = False
                    '    ddDwellingClass.Enabled = False
                    '    txtYearBuilt.Enabled = False
                    'Else
                    '    txtSqrFeet.Enabled = True
                    '    ddlNumberOfFamilies.Enabled = True
                    '    ddlStructure.Enabled = True
                    '    ddlOccupancy.Enabled = True
                    '    ddlConstruction.Enabled = True
                    '    ddStyle.Enabled = True
                    '    ddDwellingClass.Enabled = True
                    '    txtYearBuilt.Enabled = True
                    'End If

                    Me.ddStyle.SetFromValue(Me.MyLocation.ArchitecturalStyle)
                    Me.trStyle.Visible = True
                    Me.lblYearBuilt.Text = "* Year Built"
                    Me.lblSqrFeet.Text = "* Square Feet"
                    Me.lblConstruction.Text = "*Construction"
            End Select

            'Added 07/11/2023 for task WS-1286
            If IsEndorsementRelated() = False AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso Me.MyLocation.FormTypeId.EqualsAny("22", "23", "24") AndAlso IFM.VR.Common.Helpers.HOM.HOMDwellingAgeHelper.IsHomeDwellingTextAvailable(Quote) = True Then
                'Message should only display for HO 0002, HO 0003, HO 0005
                Dim effectiveDateYear As Integer = DateTime.Now.Year
                Me.divDwellingAgeText.Attributes.Add("style", "display:none")
                If Me.txtYearBuilt.Text IsNot Nothing AndAlso Me.txtYearBuilt.Text <> String.Empty AndAlso QQHelper.IsPositiveIntegerString(Me.txtYearBuilt.Text) Then
                    Dim yearBuilt As Integer = Convert.ToInt32(Me.txtYearBuilt.Text)
                    If effectiveDateYear - yearBuilt >= 75 Then
                        Me.divDwellingAgeText.Attributes.Add("style", "display:''")
                    End If
                End If
            End If

            PopulateChildControls()
            End If
    End Sub

    Protected Sub OnConfirm(sender As Object, e As EventArgs) Handles lnkDeleteResidence.Click
        Session("valuationValue") = "False"
        Dim confirmValue As String = Request.Form("confirmValue")

        If confirmValue = "Yes" Then
            ctlResidenceCoverages.DeleteResidence = True
            'Added 4/25/2022 for bug 66929 MLW
            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                MyLocation.NumberOfFamiliesId = "" 'need to reset to blank particularly for locations that have barns but no residence, other scenarios may apply
                RemoveAdditionalResidenceRentedToOthersCov()
            End If
            RaiseEvent HideDwelling()
            ClearControl()
            ClearChildControls()
        End If
    End Sub

    Public Sub DeleteResidence()
        ctlResidenceCoverages.DeleteResidence = True
        'Added 4/25/2022 for bug 66929 MLW
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            MyLocation.NumberOfFamiliesId = "" 'need to reset to blank particularly for locations that have barns but no residence, other scenarios may apply
            RemoveAdditionalResidenceRentedToOthersCov()
        End If
        RaiseEvent HideDwelling()
        ClearControl()
        ClearChildControls()
    End Sub

    Private Sub RemoveAdditionalResidenceRentedToOthersCov()
        'Remove Additional Residence coverages - there can be multiple per location
        If MyLocation.SectionIICoverages IsNot Nothing AndAlso MyLocation.SectionIICoverages.FindAll(Function(sc) sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers).IsLoaded Then
            MyLocation.SectionIICoverages.RemoveAll(Function(sc) sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers)
        End If
    End Sub

    Public Overrides Function Save() As Boolean

        If Not IsNullEmptyorWhitespace(MyLocation.OccupancyCodeId) AndAlso MyLocation.OccupancyCodeId <> "0" AndAlso MyLocation.OccupancyCodeId <> ddlOccupancy.SelectedValue Then
            ' User changed occupancy
            ' -------------------------------------------------------------------------------------
            ' SECONDARY/SEASONAL/PRIMARYU OCCUPANCY CHANGE LOGIC - MGB 6/25/2020
            ' Secondary/Seasonal is not allowed to have HO+   1020(new) 1017(old) & WATER DAMAGE
            ' Occupancy Id's: 1=primary 4=seasonal 5=secondary 6=under construction
            Dim oldOcc As String = MyLocation.OccupancyCodeId
            Dim newOcc As String = ddlOccupancy.SelectedValue
            Dim isOnOrAfterCyberCutoff As Boolean = False
            Dim isMobileHome As Boolean = False
            Dim isHO4 As Boolean = False
            Dim isSecondaryOrSeasonal As Boolean = False

            If CDate(Quote.EffectiveDate) >= CDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate")) Then isOnOrAfterCyberCutoff = True
            If Quote.Locations(0).StructureTypeId = "2" Then isMobileHome = True
            If Quote.Locations(0).FormTypeId = "25" Then isHO4 = True
            If ddlOccupancy.SelectedValue.EqualsAny("4", "5") Then isSecondaryOrSeasonal = True

            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                If oldOcc = "1" AndAlso newOcc.EqualsAny("4", "5") Then
                    ' CHANGED FROM PRIMARY TO SECONDARY/SEASONAL
                    If isOnOrAfterCyberCutoff Then
                        ' Remove Cyber if the quote has it - not allowed on secondary/seasonal
                        Dim rtn As Integer = Quote.Locations(0).SectionICoverages.RemoveAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Family_Cyber_Protection)
                        ' Check for HO
                        ' Post cyber HO will be 1010 for HO-4,Mobile Home and Seasonal and 1019 for the rest -- Not Seasonal - Seasonal stays 1019 - CAH
                        ' If it has 1010 do nothing 
                        ' If it has 1019 remove it and replace it with the old enhancement (1010)
                        If isMobileHome OrElse isHO4 Then
                            rtn = Quote.Locations(0).SectionICoverages.RemoveAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement1019)
                            If rtn > 0 Then
                                ' We removed a 1019, replace it with 1010
                                Dim newsc As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
                                newsc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement
                                newsc.Address.StateId = "0"
                                Me.MyLocation.SectionICoverages.CreateIfNull()
                                Quote.Locations(0).SectionICoverages.Add(newsc)
                            End If
                        End If


                        ' Check for HO PLUS
                        ' Post cyber HO PLUS will be 1017 for HO-4 and Mobile Home and 1020 for the rest
                        ' If it has 1020 or 1017, remove it and Water Damage and, if it is NOT a HO-4 or Mobile Home, add the NEW Home Enhancement (1019) and Sewer Backup - Home plus is not available on seasonal/secondary
                        ' Remove any water damage coverage
                        rtn = Quote.Locations(0).SectionICoverages.RemoveAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.WaterDamage)
                        ' Remove 1017 or 1020
                        If isMobileHome Or isHO4 Or isSecondaryOrSeasonal Then
                            rtn = Quote.Locations(0).SectionICoverages.RemoveAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement)
                        Else
                            rtn = Quote.Locations(0).SectionICoverages.RemoveAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement1020)
                        End If
                        If rtn > 0 Then
                            ' We found and deleted a 1020 or 1017 so now we need to add a 1019 or 1010
                            Dim newsc As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
                            If isMobileHome Or isHO4 Or isSecondaryOrSeasonal Then
                                ' Mobile home, HO4, seasonal all get the old enhancement
                                newsc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement
                            Else
                                ' Non-mobile home/ho4 get the new enhancement
                                newsc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement1019
                            End If
                            newsc.Address.StateId = "0"
                            Me.MyLocation.SectionICoverages.CreateIfNull()
                            Quote.Locations(0).SectionICoverages.Add(newsc)

                            ' Add Sewer Backup
                            newsc = New QuickQuoteSectionICoverage()
                            newsc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.BackupSewersAndDrains
                            newsc.Address = New QuickQuoteAddress()
                            newsc.Address.StateId = "0"
                            Me.MyLocation.SectionICoverages.CreateIfNull()
                            Quote.Locations(0).SectionICoverages.Add(newsc)

                            '!! DISPLAY MESSAGE FOR REMOVAL OF CYBER AND HO PLUS !!
                        End If
                    Else
                        ' PRE-CYBER CUTOFF
                        ' Check for HO PLUS
                        ' Pre-cyber will be 1017
                        ' Remove any water damage coverage
                        Dim rtn As Integer = Quote.Locations(0).SectionICoverages.RemoveAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.WaterDamage)
                        ' If it has 1017, remove it and add the OLD Home Enhancement (1010) - Home plus is not available on seasonal/secondary
                        rtn = Quote.Locations(0).SectionICoverages.RemoveAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement)
                        If rtn > 0 Then
                            ' We found and deleted a 1017 so now we need to add a 1010 and Sewer Backup
                            Dim newsc As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
                            newsc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement
                            newsc.Address.StateId = "0"
                            Me.MyLocation.SectionICoverages.CreateIfNull()
                            Quote.Locations(0).SectionICoverages.Add(newsc)
                            newsc = New QuickQuoteSectionICoverage()
                            newsc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.BackupSewersAndDrains
                            newsc.Address.StateId = "0"
                            Me.MyLocation.SectionICoverages.CreateIfNull()
                            Quote.Locations(0).SectionICoverages.Add(newsc)
                        End If
                    End If
                ElseIf oldOcc.EqualsAny("4", "5") AndAlso newOcc = "1" Then
                    ' CHANGED FROM SECONDARY/SEASONAL TO PRIMARY
                    ' Remove any Section II Secondary Liability Credit coverages - these only apply to Secondary/Seasonal occupancy and will trigger a rule if present on Primary Occupancy
s2loop:
                    If MyLocation.SectionIICoverages IsNot Nothing AndAlso MyLocation.SectionIICoverages.Count > 0 Then
                        For Each s2c As QuickQuoteSectionIICoverage In MyLocation.SectionIICoverages
                            If s2c.CoverageCodeId = "80510" Then
                                MyLocation.SectionIICoverages.Remove(s2c)
                                GoTo s2loop
                            End If
                        Next
                    End If


                    If isOnOrAfterCyberCutoff Then
                        ' Add Cyber by default for primary location (if not mobile home, HO-4, or seasonal)
                        'Updated 10/6/2022 for task 51260 MLW
                        'If isMobileHome = False AndAlso isHO4 = False AndAlso isSecondaryOrSeasonal = False Then
                        If (isMobileHome = False OrElse (isMobileHome = True AndAlso Quote.Locations(0).FormTypeId = "22")) AndAlso isHO4 = False AndAlso isSecondaryOrSeasonal = False Then 'FormTypeId 22 = FO2
                            'Dim sc As New QuickQuoteSectionICoverage()
                            'sc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Family_Cyber_Protection
                            'sc.Address.StateId = "0"
                            'Me.MyLocation.SectionICoverages.CreateIfNull()
                            'Quote.Locations(0).SectionICoverages.Add(sc)


                            Dim newcov As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
                            newcov.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Family_Cyber_Protection
                            newcov.Address = New QuickQuoteAddress()
                            newcov.IncludedLimit = "50,000"
                            newcov.IncreasedLimitId = "22"
                            newcov.Address.StateId = "0"
                            Me.MyLocation.SectionICoverages.CreateIfNull()
                            Me.MyLocation.SectionICoverages.Add(newcov)

                        End If

                        ' Check for HO ENH
                        ' Post cyber HO ENH will be 1019
                        ' If it has 1019, do nothing
                        ' if it has 1010, switch it to 1019 if not HO-4, mobile home or secondary.
                        If isMobileHome = False AndAlso isHO4 = False AndAlso isSecondaryOrSeasonal = False Then
                            ' Remove 1010 if any
                            Dim rtn As Integer = Quote.Locations(0).SectionICoverages.RemoveAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement)
                            If rtn > 0 Then
                                ' We found and deleted a 1010 so now we need to add a 1019
                                Dim newsc As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
                                newsc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement1019
                                newsc.Address.StateId = "0"
                                Me.MyLocation.SectionICoverages.CreateIfNull()
                                Quote.Locations(0).SectionICoverages.Add(newsc)
                                ' No need to add sewer backup as it should already be there
                            End If
                        End If
                    Else
                        ' PRE-CYBER CUTOFF
                        ' Check for HO PLUS
                        ' Pre-cyber will be 1017
                        ' If it has 1010 do nothing
                        ' If it has 1017, remove it and water damage and add the OLD Home Enhancement (1010) - Home plus is not available on seasonal/secondary
                        Dim rtn As Integer = Quote.Locations(0).SectionICoverages.RemoveAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.WaterDamage)
                        rtn = Quote.Locations(0).SectionICoverages.RemoveAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement)
                        If rtn > 0 Then
                            ' We found and deleted a 1017 so now we need to add a 1010 and sewer backup
                            Dim newsc As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
                            newsc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement
                            Quote.Locations(0).SectionICoverages.Add(newsc)
                            ' Add sewer backup
                            newsc = New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
                            newsc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.BackupSewersAndDrains
                            newsc.Address.StateId = "0"
                            Me.MyLocation.SectionICoverages.CreateIfNull()
                            Quote.Locations(0).SectionICoverages.Add(newsc)
                        End If
                    End If
                End If
            End If
        End If
        ' --------------------------------------------------------------------

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            'Farm SOM endorsement policies with SOM-1 and SOM-2 FormTypes can have a residence.  We want to ignore the save and allow it to persist.
            ' 02/11/2021 CAH - Task 59590
            If IsQuoteEndorsement() Then
                If MyLocation.ProgramTypeId = "7" AndAlso (MyLocation.FormTypeId = "19" OrElse MyLocation.FormTypeId = "20") Then
                    Return False
                End If
            End If


            'Updated 8/23/18 for multi state MLW
            If SubQuoteFirst IsNot Nothing Then
                If Me.SubQuoteFirst.ProgramTypeId <> "6" Then ' if not FO type then don't save when farm quote
                    If SubQuoteFirst.ProgramTypeId = "7" Then
                        MyLocation.FormTypeId = "13"
                    End If
                    Return False
                End If
            End If
        End If

        Select Case Me.Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                If MyLocation.FormTypeId = "4" Then
                    Me.MyLocation.PrimaryResidence = True
                Else
                    '11/14/17 added for HOM Upgrade MLW, was just else statement
                    If MyLocation.FormTypeId = "25" Then
                        'Updated 2/5/18 must be set to primary for HO 0004 home and mobile
                        Me.MyLocation.PrimaryResidence = True
                    Else
                        'automatically set PrimaryResidence if the occupancy is set to Owner
                        Me.MyLocation.PrimaryResidence = If(Me.ddlOccupancy.SelectedValue = "1", True, False) 'Owner
                    End If
                End If
            Case Else
                'automatically set PrimaryResidence if the occupancy is set to Owner
                Me.MyLocation.PrimaryResidence = If(Me.ddlOccupancy.SelectedValue = "1", True, False) 'Owner

        End Select

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            If MyLocationIndex = 0 Then
                MyLocation.PrimaryResidence = True
            Else
                MyLocation.PrimaryResidence = False
            End If

            If ResidenceExists Then
                Me.MyLocation.FormTypeId = Me.dd_Residence_CoverageForm.SelectedValue
            Else
                Me.MyLocation.FormTypeId = "13"
            End If

            ctlResidenceCoverages.ResidenceExists = ResidenceExists
            Me.MyLocation.DwellingTypeId = Me.ddDwellingClass.SelectedValue
        End If

        Me.MyLocation.YearBuilt = txtYearBuilt.Text

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
            If String.IsNullOrWhiteSpace(txtSqrFeet.Text) Then
                Me.MyLocation.SquareFeet = ""
            Else
                Me.MyLocation.SquareFeet = Me.txtSqrFeet.Text.TryToGetInt32().ToString()
            End If
        Else
            Me.MyLocation.SquareFeet = Me.txtSqrFeet.Text.TryToGetInt32().ToString()
        End If

        'Updated 4/26/2022 for bug 66929 MLW
        'Me.MyLocation.NumberOfFamiliesId = ddlNumberOfFamilies.SelectedValue
        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            If MyLocation.FormTypeId <> 13 Then '13-N/A (dwelling was removed)
                Me.MyLocation.NumberOfFamiliesId = ddlNumberOfFamilies.SelectedValue
            End If
        Else
            Me.MyLocation.NumberOfFamiliesId = ddlNumberOfFamilies.SelectedValue
        End If
        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
            Me.MyLocation.StructureTypeId = ddlStructureLeft.SelectedValue
            Me.trUnitsInFireDivision.Attributes.Add("style", "display:none") 'Added 12/19/17 for HOM Upgrade
        Else
            'Updated 12/13/17 for HOM Upgrade MLW
            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
                Me.MyLocation.StructureTypeId = ddlStructureLeft.SelectedValue
                'Update 12/14/17 for HOM Upgrade MLW
                If Me.MyLocation.StructureTypeId = "4" Or Me.MyLocation.StructureTypeId = "5" Then
                    Me.MyLocation.NumberOfUnitsInFireDivision = ddlUnitsInFireDivision.SelectedValue
                    Me.trUnitsInFireDivision.Attributes.Add("style", "display:''")
                Else
                    Me.trUnitsInFireDivision.Attributes.Add("style", "display:none")
                End If
            Else
                Me.MyLocation.StructureTypeId = ddlStructure.SelectedValue
                Me.trUnitsInFireDivision.Attributes.Add("style", "display:none") 'Added 12/19/17 for HOM Upgrade
            End If
        End If



        If ddlOccupancy.SelectedValue <> "" Then
            Me.MyLocation.OccupancyCodeId = ddlOccupancy.SelectedValue
            'Added 12/15/17 for HOM Upgrade MLW
            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
                '8/21/18 No updates needed for multi-state since this is for HOM only
                If Me.MyLocation.OccupancyCodeId = "4" Or Me.MyLocation.OccupancyCodeId = "5" Then
                    Me.MyLocation.PrimaryPolicyNumber = txtRelatedPolicyNumber.Text
                    Me.trRelatedPolicyNumber.Attributes.Add("style", "display:''")
                    'Added 1/11/18 for HOM Upgrade MLW
                    Me.Quote.PersonalLiabilityLimitId = 0
                    Me.Quote.MedicalPaymentsLimitid = 0
                    'Added 5/4/18 for Bug 26572 MLW - cannot have coverages in these sections if Occupancy Type is Secondary or Seasonal
                    Me.MyLocation.SectionIICoverages = Nothing
                    Me.MyLocation.SectionIAndIICoverages = Nothing 'BU wants Loss Assessment included still, but cannot add Loss Assessment here, because Diamond removes it at rate anyway MLW
                    If MyLocation.RvWatercrafts IsNot Nothing Then
                        Dim rvsToRemove As New List(Of QuickQuoteRvWatercraft)
                        For Each rv As QuickQuoteRvWatercraft In MyLocation.RvWatercrafts
                            If rv.HasLiability Then
                                rvsToRemove.Add(rv)
                            End If
                        Next

                        For Each removeThis As QuickQuoteRvWatercraft In rvsToRemove
                            If MyLocation.RvWatercrafts.Contains(removeThis) Then
                                MyLocation.RvWatercrafts.Remove(removeThis)
                            End If
                        Next
                        If MyLocation.RvWatercrafts.Count = 0 Then
                            MyLocation.RvWatercrafts = Nothing
                        End If
                    End If
                Else
                    Me.trRelatedPolicyNumber.Attributes.Add("style", "display:none")
                    If  IFM.VR.Common.Helpers.HOM.UnitOwnersRentalToOthers.IsUnitOwnersRentalToOthersAvailable(quote) AndAlso Me.Quote.Locations(0).FormTypeId = "26" AndAlso Me.MyLocation.OccupancyCodeId = "1" AndAlso MyLocation.SectionIAndIICoverages IsNot Nothing AndAlso IsQuoteEndorsement() = False Then 
                        'HO-6 and Primary Occupancy and New Business (Cannot change occupancy on Endorsement, its drop down is disabled)
                        'Make sure UnitOwnersRentaltoOthers is not a SectionIAndII Cov
                        Dim unitOwnersRentalToOthers = (From c In MyLocation.SectionIAndIICoverages Where c.MainCoverageType = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers Select c).FirstOrDefault()
                        If unitOwnersRentalToOthers IsNot Nothing Then
                            MyLocation.SectionIAndIICoverages.Remove(unitOwnersRentalToOthers)
                        End If
                    Else If  IFM.VR.Common.Helpers.HOM.UnitOwnersRentalToOthers.IsUnitOwnersRentalToOthersAvailable(quote) AndAlso Me.Quote.Locations(0).FormTypeId = "26" AndAlso Me.MyLocation.OccupancyCodeId = "9" AndAlso MyLocation.SectionIAndIICoverages IsNot Nothing AndAlso IsQuoteEndorsement() = False Then 
                        Dim unitOwnersRentalToOthers = (From c In MyLocation.SectionIAndIICoverages Where c.MainCoverageType = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers Select c).FirstOrDefault()
                        If unitOwnersRentalToOthers Is Nothing Then
                            Dim unitOwnersRentalToOthersCoverage = New QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage()
                            unitOwnersRentalToOthersCoverage.MainCoverageType = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers
                            Me.MyLocation.SectionIAndIICoverages.CreateIfNull()
                            Me.MyLocation.SectionIAndIICoverages.Add(unitOwnersRentalToOthersCoverage)
                        End If
                    End If
                End If
                'Added 5/10/18 for Bug 26705 MLW
                If Me.MyLocation.OccupancyCodeId = "7" Then
                    Me.MyLocation.PrimaryResidence = True
                End If
            Else
                Me.trRelatedPolicyNumber.Attributes.Add("style", "display:none")
            End If
        Else
            Me.MyLocation.OccupancyCodeId = ""
            Me.trRelatedPolicyNumber.Attributes.Add("style", "display:none") 'added 12/19/17 for HOM Upgrade MLW
        End If

        Me.MyLocation.ConstructionTypeId = ddlConstruction.SelectedValue

        '9-9-14
        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal OrElse Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
            Select Case Me.MyLocation.FormTypeId
                Case "1", "2", "3", "8", "9", "10", "11", "12", "23", "24" ', "13", "15", "16", "17", "18"
                    '11/14/17 added 23 & 24 for HOM Upgrade MLW
                    Me.MyLocation.ArchitecturalStyle = Me.ddStyle.SelectedValue
                Case "22" 'HO 0002 home and mobile
                    'added 11/14/17 for HOM Upgrade MLW
                    If Me.MyLocation.StructureTypeId <> "2" Then
                        Me.MyLocation.ArchitecturalStyle = Me.ddStyle.SelectedValue
                    End If
            End Select
        End If

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
            Me.MyLocation.UsageTypeId = Me.ddlUsageType.SelectedValue
        End If

        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            Me.MyLocation.ArchitecturalStyle = Me.ddStyle.SelectedValue
        End If

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            SaveChildControls()
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Property Residence"

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            'Updated 8/23/18 for multi state MLW
            If SubQuoteFirst IsNot Nothing Then
                If Me.SubQuoteFirst.ProgramTypeId <> "6" Then ' if not FO type then don't validate
                    Return
                End If
            End If

            If MyLocationIndex > 0 And MyLocation.FormTypeId = "13" Then ' If no residence exists then don't validate -- This is only for location #2 and higher
                Return
            End If

        End If

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim valList = LocationResidenceValidator.ValidateHOMLocationResidence(Me.Quote, Me.MyLocationIndex, valArgs.ValidationType)
        If valList.Any() Then
            For Each v In valList
                Select Case v.FieldId

                    Case LocationResidenceValidator.LocationYearBuilt
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtYearBuilt, v, accordList)
                    Case LocationResidenceValidator.LocationSquareFeet
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSqrFeet, v, accordList)
                    Case LocationResidenceValidator.LocationStructure
                        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlStructureLeft, v, accordList)
                        Else
                            'Updated 12/13/17 for HOM Upgrade MLW
                            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlStructureLeft, v, accordList)
                            Else
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlStructure, v, accordList)
                            End If
                        End If
                    Case LocationResidenceValidator.LocationUnitsInFireDivision
                        'Added 12/15/17 for HOM Upgrade MLW
                        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
                            If Me.MyLocation.StructureTypeId = "4" OrElse Me.MyLocation.StructureTypeId = "5" Then
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlUnitsInFireDivision, v, accordList)
                            End If
                        End If
                    Case LocationResidenceValidator.LocationUsageType
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlUsageType, v, accordList)
                    Case LocationResidenceValidator.LocationOccupancy
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlOccupancy, v, accordList)
                    Case LocationResidenceValidator.LocationRelatedPolicyNumber
                        'Addded 10/12/17 and updated 12/15/17 for HOM Upgrade - Need Related Policy Number for seasonal and secondary occupancy types - MLW
                        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
                            If Me.MyLocation.OccupancyCodeId = 4 Or Me.MyLocation.OccupancyCodeId = 5 Then
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtRelatedPolicyNumber, v, accordList)
                            End If
                        End If
                    Case LocationResidenceValidator.LocationConstruction
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlConstruction, v, accordList)
                    Case LocationResidenceValidator.LocationStyle
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStyle, v, accordList)
                    Case LocationResidenceValidator.MissingFormType
                        Me.ValidationHelper.Val_BindValidationItemToControl(dd_Residence_CoverageForm, v, accordList)
                    Case LocationResidenceValidator.MissingDwellingClass
                        Me.ValidationHelper.Val_BindValidationItemToControl(ddDwellingClass, v, accordList)
                    Case LocationResidenceValidator.PreFabNewHomeDiscountRemovedWarning
                        Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "0")
                    Case LocationResidenceValidator.HO4NewHomeDiscountRemovedWarning
                        Me.ValidationHelper.Val_BindValidationItemToControl("", v, "", "0")
                End Select
            Next
        End If

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            ValidateChildControls(valArgs)
        End If
    End Sub

    Protected Sub lnkClearResidence_Click(sender As Object, e As EventArgs) Handles lnkClearResidence.Click
        Session("valuationValue") = "False"
        Me.ClearControl()
        'force edit mode so they have to save at some point before leaving
        Me.LockTree()
    End Sub

    Public Overrides Sub ClearControl()
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            dd_Residence_CoverageForm.SelectedValue = "13"
            ddDwellingClass.SelectedValue = ""
        End If

        txtYearBuilt.Text = ""
        Me.txtSqrFeet.Text = ""

        Me.ddlNumberOfFamilies.SetFromValue("")

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
            ddlStructureLeft.SetFromValue("")
            ddlUsageType.SetFromValue("")
        Else
            'Updated 12/13/17 for HOM Upgrade MLW
            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlStructureLeft, "")
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlUnitsInFireDivision, "") 'Added 12/15/17 for HOM Upgrade MLW
                Me.txtRelatedPolicyNumber.Text = ""
            Else
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlStructure, "")
            End If
        End If

        If Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.Farm Then
            ddlOccupancy.SetFromValue("")
        Else
            ddlOccupancy.SetFromValue("1")
        End If

        ddlConstruction.SetFromValue("")
        ddStyle.SetFromValue("")

        MyBase.ClearControl()
    End Sub

    Protected Sub lnkSaveResidence_Click(sender As Object, e As EventArgs) Handles lnkSaveResidence.Click
        Session("valuationValue") = "False"
        Me.Save_FireSaveEvent(True)
    End Sub

    'added 3/3/2021
    Private Sub ctlResidenceCoverages_GetParentControlIds() Handles ctlResidenceCoverages.GetParentControlIds
        SetControlIdsOnChild()
    End Sub
    Private Sub SetControlIdsOnChild()
        Me.ctlResidenceCoverages.YearBuiltTextboxClientId = Me.txtYearBuilt.ClientID
        Me.ctlResidenceCoverages.CoverageFormDropdownClientId = Me.dd_Residence_CoverageForm.ClientID
    End Sub
End Class