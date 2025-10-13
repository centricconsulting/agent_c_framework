Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers.CIM
Imports IFM.PrimativeExtensions
Public Class cov_Computer_Item
    Inherits VRControlBase

    Private ReadOnly Property Buildings As List(Of CIM_Building)
        Get
            'Quote.Locations(locIndex).Address.State
            Dim list As New List(Of CIM_Building)
            If Quote IsNot Nothing Then
                If Quote.Locations IsNot Nothing Then
                    For locIndex As Int32 = 0 To Quote.Locations.Count - 1
                        Dim zip As String = Quote.Locations(locIndex).Address.Zip
                        If zip.Length > 5 Then
                            zip = zip.Substring(0, 5)
                        End If
                        Dim address As String = String.Format("{0} {1} {2} {3} {4} {5} {6}", Quote.Locations(locIndex).Address.HouseNum, Quote.Locations(locIndex).Address.StreetName, If(String.IsNullOrWhiteSpace(Quote.Locations(locIndex).Address.ApartmentNumber) = False, "Apt# " + Quote.Locations(locIndex).Address.ApartmentNumber, ""), Quote.Locations(locIndex).Address.POBox, Quote.Locations(locIndex).Address.City, Quote.Locations(locIndex).Address.State, zip).Replace("  ", " ").Trim()
                        If Quote.Locations(locIndex).Buildings IsNot Nothing Then
                            For bIndex As Int32 = 0 To Quote.Locations(locIndex).Buildings.Count - 1
                                list.Add(New CIM_Building(locIndex, bIndex, Quote.Locations(locIndex).Address.DisplayAddress, Quote.Locations(locIndex).Buildings(bIndex)))
                            Next
                        End If
                    Next
                End If
            End If
            Return list
        End Get
    End Property

    Private Property selectedLocations As List(Of String)

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        Me.cpRepeater.DataSource = Me.Buildings
        Me.cpRepeater.DataBind()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean

        Dim buildingIndex As Int32 = 0
        selectedLocations = New List(Of String)
        For Each itm In Me.cpRepeater.Items
            Dim LocationHasPackageParts = False
            Dim chkApply As CheckBox = itm.FindControl("chkApply")
            selectedLocations.Add(chkApply.Checked.ToString)
            If chkApply.Checked Then
                Dim txtHardwareLimit As TextBox = itm.FindControl("txtHardwareLimit")
                Dim txtProgramsLimit As TextBox = itm.FindControl("txtProgramsLimit")
                Dim txtBusinessIncomeLimit As TextBox = itm.FindControl("txtBusinessIncomeLimit")

                Dim chkEarthQuake As CheckBox = Parent.FindControl("chkEarthQuake")
                Dim chkMechanicalBreakdown As CheckBox = Parent.FindControl("chkMechanicalBreakdown")

                Dim eqLoadCalc As Func(Of String, Double) = Function(rate As String)
                                                                Return 0.06 'Math.Round(0.06 / (CDbl(rate) + 0.06), 3)
                                                            End Function

                Dim mbLoadCalc As Func(Of String, Double) = Function(rate As String)
                                                                Return 0.1 'Math.Round(0.1 / (CDbl(rate) + 0.1), 3)
                                                            End Function

                ' ----- Target States for Sub Quotes
                Dim stateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForLocation(Quote.Locations(Me.Buildings(buildingIndex).LocationIndex))



                If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtHardwareLimit.Text) > 0 Then
                    ' calc suggested rates here
                    Dim HardwareLimitRate As Double = CIMHelper.ComputerHardwareRateTable.GetRateForLimit(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtHardwareLimit.Text))
                    Me.Buildings(buildingIndex).Building.ComputerHardwareLimit = txtHardwareLimit.Text
                    Me.Buildings(buildingIndex).Building.ComputerHardwareRate = HardwareLimitRate
                    ' now that you know what rate will be used you need to apply the EQ and MB loads
                    Dim Hardware_EQLoad As Double = 0.0
                    Dim Hardware_MBLoad As Double = 0.0
                    If chkEarthQuake.Checked Then
                        Hardware_EQLoad = eqLoadCalc(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Buildings(buildingIndex).Building.ComputerHardwareRate))
                    End If
                    If chkMechanicalBreakdown.Checked Then
                        Hardware_MBLoad = mbLoadCalc(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Buildings(buildingIndex).Building.ComputerHardwareRate))
                    End If
                    Me.Buildings(buildingIndex).Building.ComputerHardwareRate = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Buildings(buildingIndex).Building.ComputerHardwareRate) + Hardware_EQLoad + Hardware_MBLoad
                    LocationHasPackageParts = True
                Else
                    Me.Buildings(buildingIndex).Building.ComputerHardwareLimit = "" 'txtHardwareLimit.Text
                    Me.Buildings(buildingIndex).Building.ComputerHardwareRate = ""
                End If

                If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtProgramsLimit.Text) > 0 Then
                    Dim ProgramsLimitRate As Double = CIMHelper.ComputerProgramsMediaRateTable.GetRateForLimit(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtProgramsLimit.Text))
                    Me.Buildings(buildingIndex).Building.ComputerProgramsApplicationsAndMediaLimit = txtProgramsLimit.Text
                    Me.Buildings(buildingIndex).Building.ComputerProgramsApplicationsAndMediaRate = ProgramsLimitRate
                    ' now that you know what rate will be used you need to apply the EQ and MB loads
                    Dim programs_EQLoad As Double = 0.0
                    Dim programs_MBLoad As Double = 0.0
                    If chkEarthQuake.Checked Then
                        programs_EQLoad = eqLoadCalc(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Buildings(buildingIndex).Building.ComputerProgramsApplicationsAndMediaRate))
                    End If
                    If chkMechanicalBreakdown.Checked Then
                        programs_MBLoad = mbLoadCalc(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Buildings(buildingIndex).Building.ComputerProgramsApplicationsAndMediaRate))
                    End If
                    Me.Buildings(buildingIndex).Building.ComputerProgramsApplicationsAndMediaRate = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Buildings(buildingIndex).Building.ComputerProgramsApplicationsAndMediaRate) + programs_EQLoad + programs_MBLoad
                    LocationHasPackageParts = True
                Else
                    Me.Buildings(buildingIndex).Building.ComputerProgramsApplicationsAndMediaLimit = "" 'txtProgramsLimit.Text
                    Me.Buildings(buildingIndex).Building.ComputerProgramsApplicationsAndMediaRate = ""
                End If

                If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtBusinessIncomeLimit.Text) > 0 Then
                    Dim BusinessLimitRate As Double = CIMHelper.ComputerBusinessIncomeRateTable.GetRateForLimit(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtBusinessIncomeLimit.Text))
                    Me.Buildings(buildingIndex).Building.ComputerBusinessIncomeLimit = txtBusinessIncomeLimit.Text
                    Me.Buildings(buildingIndex).Building.ComputerBusinessIncomeRate = BusinessLimitRate
                    ' now that you know what rate will be used you need to apply the EQ and MB loads
                    Dim bi_EqLoad As Double = 0.0
                    Dim bi_MBLoad As Double = 0.0
                    If chkEarthQuake.Checked Then
                        bi_EqLoad = eqLoadCalc(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Buildings(buildingIndex).Building.ComputerBusinessIncomeRate))
                    End If
                    If chkMechanicalBreakdown.Checked Then
                        bi_MBLoad = mbLoadCalc(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Buildings(buildingIndex).Building.ComputerBusinessIncomeRate))
                    End If
                    Me.Buildings(buildingIndex).Building.ComputerBusinessIncomeRate = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Buildings(buildingIndex).Building.ComputerBusinessIncomeRate) + bi_EqLoad + bi_MBLoad
                    LocationHasPackageParts = True
                Else
                    Me.Buildings(buildingIndex).Building.ComputerBusinessIncomeLimit = "" 'txtBusinessIncomeLimit.Text
                    Me.Buildings(buildingIndex).Building.ComputerBusinessIncomeRate = ""
                End If

                ' ----- Write HasPackagePart and Policysettings for Sub Quotes
                If LocationHasPackageParts Then
                    stateQuote.CPP_Has_InlandMarine_PackagePart = True

                    Dim CAPD As DropDownList = Me.Parent.FindControl("cpDeductible")
                    Dim CVMT As DropDownList = Me.Parent.FindControl("cpValuation")
                    Dim CCT As DropDownList = Me.Parent.FindControl("cpCoinsurance")
                    Dim CEQ As CheckBox = Me.Parent.FindControl("chkEarthQuake")
                    Dim CMB As CheckBox = Me.Parent.FindControl("chkMechanicalBreakdown")
                    Dim DED As DropDownList = Me.Parent.FindControl("ddEqDeductible")
                    Dim DMD As DropDownList = Me.Parent.FindControl("ddMechanicalDeductible")

                    stateQuote.ComputerAllPerilsDeductibleId = CAPD.SelectedValue
                    stateQuote.ComputerValuationMethodTypeId = CVMT.SelectedValue
                    stateQuote.ComputerCoinsuranceTypeId = CCT.SelectedValue
                    If CEQ.Checked Then
                        stateQuote.ComputerEarthquakeVolcanicEruptionDeductible = DED.SelectedValue
                    Else
                        stateQuote.ComputerEarthquakeVolcanicEruptionDeductible = ""
                    End If
                    If CMB.Checked Then
                        stateQuote.ComputerMechanicalBreakdownDeductible = DMD.SelectedValue
                    Else
                        stateQuote.ComputerMechanicalBreakdownDeductible = ""
                    End If
                End If

            Else
                Me.Buildings(buildingIndex).Building.ComputerHardwareLimit = ""
                Me.Buildings(buildingIndex).Building.ComputerHardwareRate = ""
                Me.Buildings(buildingIndex).Building.ComputerProgramsApplicationsAndMediaLimit = ""
                Me.Buildings(buildingIndex).Building.ComputerProgramsApplicationsAndMediaRate = ""
                Me.Buildings(buildingIndex).Building.ComputerBusinessIncomeLimit = ""
                Me.Buildings(buildingIndex).Building.ComputerBusinessIncomeRate = ""
            End If

            buildingIndex += 1
        Next
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = "Computer"
        Dim deductibleControl As DropDownList = Parent.FindControl("cpDeductible")
        Dim deductibleAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(deductibleControl.SelectedItem.Text)
        Dim totalLimit As Double = 0.0
        For Each ri As RepeaterItem In cpRepeater.Items
            ' Individual Limts
            Dim LocationChecked As CheckBox = ri.FindControl("chkApply")
            Dim hardwareLimit As TextBox = ri.FindControl("txtHardwareLimit")
            Dim programLimit As TextBox = ri.FindControl("txtProgramsLimit")
            Dim busIncomeLimit As TextBox = ri.FindControl("txtBusinessIncomeLimit")
            Dim hardwareLimitAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(hardwareLimit.Text)
            Dim programLimitAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(programLimit.Text)
            Dim busIncomeLimitAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(busIncomeLimit.Text)
            '3.8.21
            If String.IsNullOrEmpty(hardwareLimit.Text) AndAlso LocationChecked.Checked Then
                Me.ValidationHelper.AddError("Missing Hardware Limit", hardwareLimit.ClientID)
            End If
            'Hardware Limit - 3.8.115
            If hardwareLimitAmount > 500000 AndAlso LocationChecked.Checked Then
                Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", hardwareLimit.ClientID)
            End If
            'Program Limit - 3.8.116
            If programLimitAmount > 500000 AndAlso LocationChecked.Checked Then
                Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", programLimit.ClientID)
            End If
            If deductibleAmount >= hardwareLimitAmount AndAlso LocationChecked.Checked Then
                Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", hardwareLimit.ClientID)
            End If
            If String.IsNullOrEmpty(programLimit.Text) = False AndAlso deductibleAmount >= programLimitAmount AndAlso LocationChecked.Checked Then
                Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", programLimit.ClientID)
            End If
            If String.IsNullOrEmpty(busIncomeLimit.Text) = False AndAlso deductibleAmount >= busIncomeLimitAmount AndAlso LocationChecked.Checked Then
                Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", busIncomeLimit.ClientID)
            End If
            ' Total Limit
            totalLimit = totalLimit + hardwareLimitAmount + programLimitAmount
            If totalLimit > 500000 AndAlso LocationChecked.Checked Then
                Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", programLimit.ClientID)
            End If
        Next

        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Sub ClearControl()
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                For Each location In Quote.Locations
                    If location.Buildings IsNot Nothing Then
                        For Each building As QuickQuoteBuilding In location.Buildings
                            building.ComputerHardwareLimit = Nothing
                            building.ComputerProgramsApplicationsAndMediaLimit = Nothing
                            building.ComputerBusinessIncomeLimit = Nothing
                        Next
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub cpRepeater_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles cpRepeater.ItemDataBound

        Dim location As Object = e.Item.DataItem
        Dim hardwareLimit As TextBox = e.Item.FindControl("txtHardwareLimit")
        Dim programLimit As TextBox = e.Item.FindControl("txtProgramsLimit")
        Dim businessLimit As TextBox = e.Item.FindControl("txtBusinessIncomeLimit")
        Dim chkApply As CheckBox = e.Item.FindControl("chkApply")
        Dim index As Int32 = e.Item.ItemIndex

        If location.Building IsNot Nothing Then
            If location.LocationIndex = 0 And location.BuildingIndex = 0 Then
                Dim CopyButton As Button = e.Item.FindControl("btnCopyToOtherLocations")
                CopyButton.Visible = True
            End If

            If String.IsNullOrEmpty(hardwareLimit.Text) = False OrElse String.IsNullOrEmpty(programLimit.Text) = False OrElse String.IsNullOrEmpty(businessLimit.Text) = False Then
                chkApply.Checked = True
            Else
                If selectedLocations IsNot Nothing AndAlso selectedLocations.Count > 0 AndAlso selectedLocations.Count >= (index + 1) AndAlso selectedLocations(index) IsNot Nothing AndAlso CBool(selectedLocations(index)) Then
                    chkApply.Checked = CBool(selectedLocations(index))
                Else
                    chkApply.Checked = False
                End If
            End If

        End If
    End Sub

    Protected Sub cpRepeater_Functions(source As Object, e As RepeaterCommandEventArgs) Handles cpRepeater.ItemCommand
        If e.CommandName = "btnCopy" Then
            btncopyClick(e)
        End If
        Save_FireSaveEvent(False)
        Populate()
    End Sub

    Protected Sub btncopyClick(e As RepeaterCommandEventArgs)
        Dim hardwareLimitMaster As TextBox = e.Item.FindControl("txtHardwareLimit")
        Dim programLimitMaster As TextBox = e.Item.FindControl("txtProgramsLimit")
        Dim businessLimitMaster As TextBox = e.Item.FindControl("txtBusinessIncomeLimit")

        For Each ri As RepeaterItem In cpRepeater.Items
            ' Individual Limts
            Dim LocationChecked As CheckBox = ri.FindControl("chkApply")
            Dim hardwareLimit As TextBox = ri.FindControl("txtHardwareLimit")
            Dim programLimit As TextBox = ri.FindControl("txtProgramsLimit")
            Dim busIncomeLimit As TextBox = ri.FindControl("txtBusinessIncomeLimit")

            LocationChecked.Checked = True
            hardwareLimit.Text = hardwareLimitMaster.Text
            programLimit.Text = programLimitMaster.Text
            busIncomeLimit.Text = businessLimitMaster.Text

        Next

    End Sub

    Public Overrides Function hasSetting() As Boolean
        Dim hasPackagePart As Boolean = False
        For Each stateQuote As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            If stateQuote.CPP_Has_InlandMarine_PackagePart Then
                hasPackagePart = True
            End If
        Next
        Return hasPackagePart
    End Function


End Class