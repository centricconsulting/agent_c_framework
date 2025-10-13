Imports IFM.VR.Validation.ObjectValidation.FarmLines.Buildings
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers.MultiState.General
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Common.Helpers.FARM

Public Class ctl_FarmBuilding_Coverages
    Inherits VRControlBase


    Private ReadOnly Property MySuffocationDescriptionIndicator As String
        Get
            Return String.Format("{0}.{1}", Me.MyLocationIndex + 1, Me.MyBuildingIndex + 1)
        End Get
    End Property

    Private Enum AnimalType_enum
        None
        Swine
        Poultry
        Cattle
        Equine
        All
    End Enum

    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public Property MyBuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
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

    Public ReadOnly Property MyBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding
        Get
            If Me.MyLocation IsNot Nothing Then
                Return Me.MyLocation.Buildings.GetItemAtIndex(MyBuildingIndex)
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property SuffocationAndCustomFeedingCutoffDate As DateTime
        Get
            Return CDate("7/1/2020")
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.HiddenField1, "0")
        Me.VRScript.CreateConfirmDialog(Me.lnkBtnClear.ClientID, "Clear")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
        Me.VRScript.CreateTextBoxFormatter(Me.txtLossIncomeLimit, ctlPageStartupScript.FormatterType.NumericWithCommas, ctlPageStartupScript.JsEventType.onblur)
        Me.VRScript.CreateTextBoxFormatter(Me.txtSuffocationLimit, ctlPageStartupScript.FormatterType.NumericWithCommas, ctlPageStartupScript.JsEventType.onblur)

        ' these js variable are defined here but not used here they are used by javascript that is located on the ctl_FarmBuilding_Property control
        Me.VRScript.AddVariableLine(String.Format("var location_{0}_building_{1}_Covs_TrAdditionalPerils = '{2}';", Me.MyLocationIndex, MyBuildingIndex, dvAddlPerils.ClientID))
        Me.VRScript.AddVariableLine(String.Format("var location_{0}_building_{1}_Covs_TrEqContent = '{2}';", Me.MyLocationIndex, MyBuildingIndex, dvEarthContents.ClientID))
        Me.VRScript.AddVariableLine(String.Format("var location_{0}_building_{1}_Covs_chkAdditionalPerils = '{2}';", Me.MyLocationIndex, MyBuildingIndex, Me.chkAdditionalPerils.ClientID))
        Me.VRScript.AddVariableLine(String.Format("var location_{0}_building_{1}_Covs_chkEqContent = '{2}';", Me.MyLocationIndex, MyBuildingIndex, Me.chkEarthQuake_contents.ClientID))
        Me.VRScript.AddVariableLine(String.Format("var location_{0}_building_{1}_Covs_lblLossIncome = '{2}';", Me.MyLocationIndex, MyBuildingIndex, Me.lblLossIncome.ClientID))
        VRScript.AddVariableLine(String.Format("var location_{0}_building_{1}_Covs_chkLossIncome = '{2}';", Me.MyLocationIndex, MyBuildingIndex, chkLossIncome.ClientID))
        VRScript.AddVariableLine(String.Format("var location_{0}_building_{1}_Covs_dvLossIncomeLimit = '{2}';", Me.MyLocationIndex, MyBuildingIndex, dvLossIncomeLimit.ClientID))
        VRScript.AddVariableLine(String.Format("var location_{0}_building_{1}_Covs_dvLossIncomeData = '{2}';", Me.MyLocationIndex, MyBuildingIndex, dvLossIncomeData.ClientID))
        VRScript.AddVariableLine(String.Format("var location_{0}_building_{1}_Covs_txtLossIncomeLimit = '{2}';", Me.MyLocationIndex, MyBuildingIndex, txtLossIncomeLimit.ClientID))
        VRScript.AddVariableLine(String.Format("var location_{0}_building_{1}_Covs_chkReplacement = '{2}';", Me.MyLocationIndex, MyBuildingIndex, chkReplacement.ClientID))
        Dim ddlBuldingType As String = String.Format("location_{0}_building_{1}_BuildingType", MyLocationIndex, MyBuildingIndex)

        'These run at startup also
        'Me.VRScript.CreateJSBinding(Me.chkContractGrower, ctlPageStartupScript.JsEventType.onchange, "if($('#" + chkContractGrower.ClientID + "').prop('checked')){$('#" + dvContractGrowersLimit.ClientID + "').show();} else{$('#" + Me.ddContractGrowerLimit.ClientID + "').val('');$('#" + dvContractGrowersLimit.ClientID + "').hide();}", True)
        'Me.VRScript.CreateJSBinding(Me.chkLossIncome, ctlPageStartupScript.JsEventType.onchange, "if($('#" + chkLossIncome.ClientID + "').prop('checked')){$('#" + Me.txtLossIncomeLimit.ClientID + "').show();} else{$('#" + Me.txtLossIncomeLimit.ClientID + "').val('');$('#" + Me.txtLossIncomeLimit.ClientID + "').hide();}", True)
        'Me.VRScript.CreateJSBinding(Me.chkSuffocation, ctlPageStartupScript.JsEventType.onchange, "if($('#" + chkSuffocation.ClientID + "').prop('checked')){$('#" + dvSufficationLimit.ClientID + "').show();} else{$('#" + Me.txtSuffocationLimit.ClientID + "').val('');$('#" + dvSufficationLimit.ClientID + "').hide();}", True)
        Dim scriptToggleAddlLossIncome As String = "ToggleExpandAddlLossIncome(this, """ + chkLossIncome.ClientID + """, """ + dvLossIncomeData.ClientID + """, """ + dvLossIncomeLimit.ClientID +
            """, """ + txtLossIncomeLimit.ClientID + """, """ + ddlCoInsurance.ClientID + """, """ + ddlLossExt.ClientID + """);"
        VRScript.CreateJSBinding(String.Format("location_{0}_building_{1}", MyLocationIndex, MyBuildingIndex), ctlPageStartupScript.JsEventType.onchange, scriptToggleAddlLossIncome)

        Dim scriptContractGrowers As String = "ToggleContractGrowers(""" + chkContractGrower.ClientID + """, """ + dvContractGrowersLimit.ClientID + """, """ + ddContractGrowerLimit.ClientID + """);"
        chkContractGrower.Attributes.Add("onchange", scriptContractGrowers)

        Dim scriptAddlPerils As String = "ToggleCheckboxOnly(""" + chkAdditionalPerils.ClientID + """);"
        chkAdditionalPerils.Attributes.Add("onclick", scriptAddlPerils)

        Dim scriptEarthContents As String = "ToggleCheckboxOnly(""" + chkEarthQuake_contents.ClientID + """);"
        chkEarthQuake_contents.Attributes.Add("onclick", scriptEarthContents)

        Dim scriptEarthStruct As String = "ToggleCheckboxOnly(""" + chkEarthQuake_structure.ClientID + """);"
        chkEarthQuake_structure.Attributes.Add("onclick", scriptEarthStruct)

        Dim scriptReplacement As String = "ToggleCheckboxOnly(""" + chkReplacement.ClientID + """);"
        chkReplacement.Attributes.Add("onclick", scriptReplacement)

        Dim scriptSpecialForm As String = "ToggleCheckboxOnly(""" + chkSpecialForm.ClientID + """);"
        chkSpecialForm.Attributes.Add("onclick", scriptSpecialForm)

        Dim scriptLossIncome As String = "ToggleLossIncome(""" + chkLossIncome.ClientID + """, """ + txtLossIncomeLimit.ClientID + """, """ + dvLossIncomeData.ClientID + """, """ + dvLossIncomeLimit.ClientID +
            """, """ + ddlCoInsurance.ClientID + """, """ + ddlLossExt.ClientID + """, $('#' + " + ddlBuldingType + ").val());"
        chkLossIncome.Attributes.Add("onchange", scriptLossIncome)

        Dim scriptMaterials As String = "ToggleCheckboxOnly(""" + chkBuildingMaterials.ClientID + """);"
        chkBuildingMaterials.Attributes.Add("onclick", scriptMaterials)

        Dim scriptSuffocation As String = "ToggleSuffocation(""" + chkSuffocation.ClientID + """, """ + dvSufficationLimit.ClientID + """, """ + txtSuffocationLimit.ClientID + """);"
        chkSuffocation.Attributes.Add("onchange", scriptSuffocation)

        'Updated 11/3/2022 for task 60749 MLW
        If IFM.VR.Common.Helpers.FARM.FarmCustomFeeding.IsFARCustomFeedingAvailable(Quote) = False Then
            Me.VRScript.CreateJSBinding(chkCustomFeeding.ClientID, "click", "HandleCustomFeedingCheckboxClicks('" & chkCustomFeeding.ClientID & "','" & dvCustomFeedingData.ClientID & "');")
        End If
        Me.VRScript.CreateJSBinding(chkSuffocationOfLivestockOrPoultry.ClientID, "click", "HandleSuffocationOfLivestockCheckboxClicks('" & chkSuffocationOfLivestockOrPoultry.ClientID & "');")

        ' Handle Cosmetic Damage Exclusion clicks
        If CosmeticDamageExHelper.IsCosmeticDamageExAvailable(Quote) Then
            Dim scriptCDE As String = "ToggleCheckboxOnly(""" + chkCosmeticDamageExclusion.ClientID + """);"
            chkCosmeticDamageExclusion.Attributes.Add("onclick", scriptCDE)
        Else
            ' Handle Cosmetic Damage Exclusion clicks
            VRScript.CreateJSBinding(chkCosmeticDamageExclusion.ClientID, "onclick", "HandleCosmeticDamageClicks('" & chkCosmeticDamageExclusion.ClientID & "','" & dvCosmeticDamageExclusionData.ClientID & "');")
        End If

        ' Handle OH Mine Subsidence clicks
        If MyLocation.Address.QuickQuoteState = QuickQuoteState.Ohio Then
            If IFM.VR.Common.Helpers.MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(MyLocation.Address.County) = MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleOptional Then
                VRScript.CreateJSBinding(chkMineBuilding.ClientID, "onclick", "HandleOHMineSubClicks('" & chkMineBuilding.ClientID & "','1','" & MyLocationIndex.ToString & "');")
            Else
                VRScript.CreateJSBinding(chkMineBuilding.ClientID, "onclick", "HandleOHMineSubClicks('" & chkMineBuilding.ClientID & "','0','" & MyLocationIndex.ToString & "');")
            End If
        End If

        ' Handle IL Mine Subsidence clicks
        If IFM.VR.Common.Helpers.FARM.ILMineSubsidenceHelper.IsILMineSubsidenceAvailable(Quote) = True Then
            If MyLocation.Address.QuickQuoteState = QuickQuoteState.Illinois Then
                VRScript.CreateJSBinding(chkMineBuilding.ClientID, "onclick", "HandleILMineSubClicks('" & chkMineBuilding.ClientID & "','" & MyLocationIndex.ToString & "');")
            End If
        End If

        Exit Sub
    End Sub

    Public Overrides Sub LoadStaticData()
        'Updated 11/3/2022 for task 60749 MLW
        If Quote IsNot Nothing Then
            If IFM.VR.Common.Helpers.FARM.FarmCustomFeeding.IsFARCustomFeedingAvailable(Quote) = False Then
                ' Load the custom feeding limit dropdowns
                If ddCFSwineLimit.Items Is Nothing OrElse ddCFSwineLimit.Items.Count = 0 Then
                    QQHelper.LoadStaticDataOptionsDropDown(ddCFSwineLimit, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FarmCustomFeedingSwineLimitId)
                    QQHelper.LoadStaticDataOptionsDropDown(ddCFPoultryLimit, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FarmCustomFeedingPoultryLimitId)
                    QQHelper.LoadStaticDataOptionsDropDown(ddCFCattleLimit, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FarmCustomFeedingCattleLimitId)
                    QQHelper.LoadStaticDataOptionsDropDown(ddCFEquineLimit, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FarmCustomFeedingEquineLimitId)
                End If
            End If
        End If
    End Sub

    Private Sub PopulatePolicyLevelCoverages()

        ' CUSTOM FEEDING/CONTRACT GROWERS & SUFFOCATION COVERAGES
        ' Format and populate the Custom Feeding/Contract Growers and Suffocation sections based on cutoff date.
        '   - Before the cutoff date we use the Contract Growers and Suffocation of Livestock coverages.
        '   - After the cutoff date we use the Custom Feeding and Suffocation of Livestock and Poultry coverages.
        If CDate(SubQuoteFirst.EffectiveDate) >= SuffocationAndCustomFeedingCutoffDate Then
            ' Effective date is after cutoff date
            ' Use Custom Feeding and Suffocation of Livestock and Poultry coverages

            'Updated 11/3/2022 for task 60749 MLW
            ' CUSTOM FEEDING
            If IFM.VR.Common.Helpers.FARM.FarmCustomFeeding.IsFARCustomFeedingAvailable(Quote) = False Then
                lblOptLiability.Attributes.Add("style", "display:''")
                dvCustomFeeding.Attributes.Add("style", "display:''")
                dvContractGrowers.Attributes.Add("style", "display:none")
                chkCustomFeeding.Checked = QuoteHasCustomFeedingCoverage()
                If chkCustomFeeding.Checked Then
                    dvCustomFeedingData.Attributes.Add("style", "display:''")
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddCFSwineLimit, SubQuoteFirst.FarmCustomFeedingSwineLimitId)
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddCFPoultryLimit, SubQuoteFirst.FarmCustomFeedingPoultryLimitId)
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddCFCattleLimit, SubQuoteFirst.FarmCustomFeedingCattleLimitId)
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddCFEquineLimit, SubQuoteFirst.FarmCustomFeedingEquineLimitId)
                    txtCFSwineDesc.Text = SubQuoteFirst.FarmCustomFeedingSwineDescription
                    txtCFPoultryDesc.Text = SubQuoteFirst.FarmCustomFeedingPoultryDescription
                    txtCFCattleDesc.Text = SubQuoteFirst.FarmCustomFeedingCattleDescription
                    txtCFEquineDesc.Text = SubQuoteFirst.FarmCustomFeedingEquineDescription
                    If MyLocationIndex > 0 Then
                        'txtCFCattleDesc.Attributes.Add("style", "disabled:true")
                        'txtCFEquineDesc.Attributes.Add("style", "disabled:true")
                        'txtCFPoultryDesc.Attributes.Add("style", "disabled:true")
                        'txtCFSwineDesc.Attributes.Add("style", "disabled:true")
                        'ddCFCattleLimit.Attributes.Add("style", "disabled:true")
                        'ddCFEquineLimit.Attributes.Add("style", "disabled:true")
                        'ddCFPoultryLimit.Attributes.Add("style", "disabled:true")
                        'ddCFSwineLimit.Attributes.Add("style", "disabled:true")
                        txtCFCattleDesc.Enabled = False
                        txtCFEquineDesc.Enabled = False
                        txtCFPoultryDesc.Enabled = False
                        txtCFSwineDesc.Enabled = False
                        ddCFCattleLimit.Enabled = False
                        ddCFEquineLimit.Enabled = False
                        ddCFPoultryLimit.Enabled = False
                        ddCFSwineLimit.Enabled = False
                    End If
                Else
                    dvCustomFeedingData.Attributes.Add("style", "display:none")
                    ddCFSwineLimit.SelectedIndex = -1
                    ddCFPoultryLimit.SelectedIndex = -1
                    ddCFCattleLimit.SelectedIndex = -1
                    ddCFEquineLimit.SelectedIndex = -1
                    txtCFSwineDesc.Text = ""
                    txtCFPoultryDesc.Text = ""
                    txtCFCattleDesc.Text = ""
                    txtCFEquineDesc.Text = ""
                    'txtCFCattleDesc.Attributes.Add("style", "disabled:false")
                    'txtCFEquineDesc.Attributes.Add("style", "disabled:false")
                    'txtCFPoultryDesc.Attributes.Add("style", "disabled:false")
                    'txtCFSwineDesc.Attributes.Add("style", "disabled:false")
                    'ddCFCattleLimit.Attributes.Add("style", "disabled:false")
                    'ddCFEquineLimit.Attributes.Add("style", "disabled:false")
                    'ddCFPoultryLimit.Attributes.Add("style", "disabled:false")
                    'ddCFSwineLimit.Attributes.Add("style", "disabled:false")
                    txtCFCattleDesc.Enabled = True
                    txtCFEquineDesc.Enabled = True
                    txtCFPoultryDesc.Enabled = True
                    txtCFSwineDesc.Enabled = True
                    ddCFCattleLimit.Enabled = True
                    ddCFEquineLimit.Enabled = True
                    ddCFPoultryLimit.Enabled = True
                    ddCFSwineLimit.Enabled = True
                End If
            Else
                lblOptLiability.Attributes.Add("style", "display:none")
                dvCustomFeeding.Attributes.Add("style", "display:none")
                dvContractGrowers.Attributes.Add("style", "display:none")
            End If

            ' SUFFOCATION OF LIVESTOCK AND POULTRY
            dvSuffocationNEW.Attributes.Add("style", "display:''")
            dvSuffocationOLD.Attributes.Add("style", "display:none")

            If SubQuoteFirst IsNot Nothing Then
                If SubQuoteFirst.OptionalCoverages IsNot Nothing Then
                    ' use suffocation of livestock and poultry when eff date >= cutoff date
                    If QuoteHAsSuffocationOfLivestockAndPoultry() Then
                        dvSuffocationOfLivestockOrPoultryData.Attributes.Add("style", "display:block;")
                        Me.chkSuffocationOfLivestockOrPoultry.Checked = True

                        ' Cattle
                        Dim suffocationCovCattle = (From cov In SubQuoteFirst.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Cattle Select cov).FirstOrDefault()
                        If suffocationCovCattle IsNot Nothing Then
                            Me.txtSuffCattleLimit.Text = suffocationCovCattle.IncreasedLimit
                            Me.txtSuffCattleDesc.Text = suffocationCovCattle.Description
                            If MyLocationIndex > 0 Then
                                'txtSuffCattleDesc.ReadOnly = True
                                'txtSuffCattleLimit.ReadOnly = True
                                txtSuffCattleLimit.Enabled = False
                                txtSuffCattleDesc.Enabled = False
                            End If
                        End If
                        ' Equine
                        Dim suffocationCovEquine = (From cov In SubQuoteFirst.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Equine Select cov).FirstOrDefault()
                        If suffocationCovEquine IsNot Nothing Then
                            Me.txtSuffEquineLimit.Text = suffocationCovEquine.IncreasedLimit
                            Me.txtSuffEquineDesc.Text = suffocationCovEquine.Description
                            If MyLocationIndex > 0 Then
                                'txtSuffEquineDesc.ReadOnly = True
                                'txtSuffEquineLimit.ReadOnly = True
                                txtSuffEquineDesc.Enabled = False
                                txtSuffEquineLimit.Enabled = False
                            End If
                        End If
                        ' Poultry
                        Dim suffocationCovPoultry = (From cov In SubQuoteFirst.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Poultry Select cov).FirstOrDefault()
                        If suffocationCovPoultry IsNot Nothing Then
                            Me.txtSuffPoultryLimit.Text = suffocationCovPoultry.IncreasedLimit
                            Me.txtSuffPoultryDesc.Text = suffocationCovPoultry.Description
                            If MyLocationIndex > 0 Then
                                'txtSuffPoultryDesc.ReadOnly = True
                                'txtSuffPoultryLimit.ReadOnly = True
                                txtSuffPoultryDesc.Enabled = False
                                txtSuffPoultryLimit.Enabled = False
                            End If
                        End If
                        ' Swine
                        Dim suffocationCovSwine = (From cov In SubQuoteFirst.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Swine Select cov).FirstOrDefault()
                        If suffocationCovSwine IsNot Nothing Then
                            Me.txtSuffSwineLimit.Text = suffocationCovSwine.IncreasedLimit
                            Me.txtSuffSwineDesc.Text = suffocationCovSwine.Description
                            If MyLocationIndex > 0 Then
                                'txtSuffSwineDesc.ReadOnly = True
                                'txtSuffSwineLimit.ReadOnly = True
                                txtSuffSwineDesc.Enabled = False
                                txtSuffSwineLimit.Enabled = False
                            End If
                        End If
                    Else
                        chkSuffocationOfLivestockOrPoultry.Checked = False
                        dvSuffocationOfLivestockOrPoultryData.Attributes.Add("style", "display:none;")
                        txtSuffCattleLimit.Text = ""
                        txtSuffCattleDesc.Text = ""
                        txtSuffEquineLimit.Text = ""
                        txtSuffEquineDesc.Text = ""
                        txtSuffPoultryLimit.Text = ""
                        txtSuffPoultryDesc.Text = ""
                        txtSuffSwineLimit.Text = ""
                        txtSuffSwineDesc.Text = ""
                        'txtSuffCattleLimit.ReadOnly = True
                        'txtSuffCattleDesc.ReadOnly = True
                        'txtSuffEquineLimit.ReadOnly = True
                        'txtSuffEquineDesc.ReadOnly = True
                        'txtSuffPoultryLimit.ReadOnly = True
                        'txtSuffPoultryDesc.ReadOnly = True
                        'txtSuffSwineLimit.ReadOnly = True
                        'txtSuffSwineDesc.ReadOnly = True
                        txtSuffCattleLimit.Enabled = True
                        txtSuffCattleDesc.Enabled = True
                        txtSuffEquineLimit.Enabled = True
                        txtSuffEquineDesc.Enabled = True
                        txtSuffPoultryLimit.Enabled = True
                        txtSuffPoultryDesc.Enabled = True
                        txtSuffSwineLimit.Enabled = True
                        txtSuffSwineDesc.Enabled = True
                    End If
                End If
            End If
        Else
            'NOTE: Since cutoff date was 7/1/2020, should never get here anymore - 11/7/2022 MLW - CustomFeeding replaced ContractGrowers in 2020
            ' Effective date is before cutoff date
            ' Use Contract Growers and Suffocation of Livestock coverages
            dvCustomFeeding.Attributes.Add("style", "display:none")
            dvContractGrowers.Attributes.Add("style", "display:''")

            dvSuffocationNEW.Attributes.Add("style", "display:none")
            dvSuffocationOLD.Attributes.Add("style", "display:''")

            ' CONTRACT GROWERS
            If SubQuoteFirst IsNot Nothing Then
                Me.chkContractGrower.Checked = Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmContractGrowersCareCustodyControlLimitId)
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddContractGrowerLimit, SubQuoteFirst.FarmContractGrowersCareCustodyControlLimitId)
            End If
            Me.chkContractGrower.Enabled = Me.MyBuilding.Equals(IFM.VR.Common.Helpers.BuildingsHelper.FindFirstBuilding(Me.Quote))
            Me.ddContractGrowerLimit.Enabled = Me.MyBuilding.Equals(IFM.VR.Common.Helpers.BuildingsHelper.FindFirstBuilding(Me.Quote))
            Me.chkContractGrower.ToolTip = If(Me.chkContractGrower.Enabled = False, "This coverage can only be toggled on the first building of the lowest numbered location.", "")

            If chkContractGrower.Checked Then
                dvContractGrowersLimit.Attributes.Add("style", "display:block;")
            Else
                dvContractGrowersLimit.Attributes.Add("style", "display:none;")
            End If

            ' SUFFOCATION OF LIVESTOCK
            'Updated 9/10/18 for multi state MLW - Quote to SubQuoteFirst
            If SubQuoteFirst IsNot Nothing Then
                If SubQuoteFirst.OptionalCoverages IsNot Nothing Then
                    Dim suffocationCov = (From cov In SubQuoteFirst.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock And cov.Description = Me.MySuffocationDescriptionIndicator Select cov).FirstOrDefault()
                    If suffocationCov IsNot Nothing Then
                        Me.chkSuffocation.Checked = True
                        Me.txtSuffocationLimit.Text = suffocationCov.IncreasedLimit
                        dvSufficationLimit.Attributes.Add("style", "display:block;")
                    End If
                End If
            End If
        End If

        ' Check for Farm Dwelling And Mobile Home Dwelling
        If Quote.Locations IsNot Nothing Then
            If MyLocation.Buildings IsNot Nothing Then
                If MyBuilding.FarmStructureTypeId <> "18" AndAlso MyBuilding.FarmStructureTypeId <> "17" Then
                    lblLossIncome.Text = "Loss of Income"
                    dvAddlPerils.Attributes.Add("style", "display:none;")

                    If MyBuilding.FarmStructureTypeId = "37" Then
                        dvEarthContents.Attributes.Add("style", "display:block; margin-left: 20px;")
                    Else
                        dvEarthContents.Attributes.Add("style", "display:none;")
                    End If


                    If MyLocation.IncomeLosses IsNot Nothing Then
                        If MyLocation.IncomeLosses.Count > 0 Then
                            Dim incomeLoss = MyLocation.IncomeLosses.Find(Function(p) p.Description.Trim() = String.Format("LOC{0}BLD{1}", (MyLocationIndex + 1), (MyBuildingIndex + 1)))

                            If incomeLoss IsNot Nothing Then
                                chkLossIncome.Checked = True
                                dvLossIncomeLimit.Attributes.Add("style", "display:block;")
                                dvLossIncomeData.Attributes.Add("style", "display:block;")

                                'For Each incomeLoss In incomeLossList
                                'txtLossIncomeLimit.Text = incomeLossList(0).Coverage.ManualLimitAmount
                                txtLossIncomeLimit.Text = incomeLoss.Limit
                                ddlCoInsurance.SelectedValue = incomeLoss.CoinsuranceTypeId
                                ddlLossExt.SelectedValue = incomeLoss.ExtendFarmIncomeOptionId
                                'Next
                            End If
                        Else
                            chkLossIncome.Checked = False
                            dvLossIncomeData.Attributes.Add("style", "display:none;")
                            dvLossIncomeLimit.Attributes.Add("style", "display:none;")
                        End If
                    Else
                        chkLossIncome.Checked = False
                        dvLossIncomeData.Attributes.Add("style", "display:none;")
                        dvLossIncomeLimit.Attributes.Add("style", "display:none;")
                    End If
                Else
                    dvAddlPerils.Attributes.Add("style", "display:block; margin-left: 20px;")
                    dvEarthContents.Attributes.Add("style", "display:block; margin-left: 20px;")
                    lblLossIncome.Text = "Loss of Income - Rents"
                End If
            End If
        End If

        ' OH Mine Subsidence
        ' For OH, set mine sub based on county
        ' Set classes on the controls so we can manipulate them easily in jquery
        chkMineBuilding.Attributes.Remove("class")
        If MyLocation.Address.QuickQuoteState = QuickQuoteState.Ohio Then
            If MyBuilding.FarmStructureTypeId = "17" OrElse MyBuilding.FarmStructureTypeId = "18" Then
                divMineSubReqHelpInfo_OH.Attributes.Add("style", "display:none;")
                divMineSubLimitInfo.Attributes.Add("style", "display:none;")
                ' Mine sub is only allowed on Farm Dwelling or Mobile Home
                Select Case IFM.VR.Common.Helpers.MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(MyLocation.Address.County)
                    Case MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleMandatory
                        divMineSub.Attributes.Add("style", "display:block;margin-left:20px;")
                        divMineSubReqHelpInfo_OH.Attributes.Add("style", "display:block;margin-left:20px;")
                        If MyBuilding.E_Farm_Limit IsNot Nothing AndAlso IsNumeric(MyBuilding.E_Farm_Limit) Then
                            If CDec(MyBuilding.E_Farm_Limit) >= 300000 Then
                                divMineSubLimitInfo.Attributes.Add("style", "display:block;margin-left:20px;")
                            End If
                        End If
                        chkMineBuilding.Attributes.Add("class", "chkOHMineSubMandatory_" & MyLocationIndex.ToString)
                        chkMineBuilding.Checked = True
                        chkMineBuilding.Enabled = False
                        Exit Select
                    Case MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleOptional
                        divMineSub.Attributes.Add("style", "display:block;margin-left:20px;")
                        chkMineBuilding.Attributes.Add("class", "chkOHMineSubOptional_" & MyLocationIndex.ToString)
                        chkMineBuilding.Enabled = True
                        chkMineBuilding.Checked = False
                        Dim mineCov = (From cov In MyBuilding.OptionalCoverageEs Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Farm_Mine_Subsidence Select cov).FirstOrDefault()
                        ' Check to see if the location dwelling has mine sub, if so this bulding must as well (if it's a dwelling)
                        ' So if the building or it's containing location have mine sub check the box
                        If mineCov IsNot Nothing OrElse IFM.VR.Common.Helpers.MultiState.General.QuoteHasOHMineSubOnLocationDwelling(MyLocation) Then
                            chkMineBuilding.Checked = True
                        End If
                        If chkMineBuilding.Checked Then
                            If MyBuilding.E_Farm_Limit IsNot Nothing AndAlso IsNumeric(MyBuilding.E_Farm_Limit) Then
                                If CDec(MyBuilding.E_Farm_Limit) >= 300000 Then
                                    divMineSubLimitInfo.Attributes.Add("style", "display:block;margin-left:20px;")
                                End If
                            End If
                        End If
                        Exit Select
                    Case MineSubsidenceHelper.OhioMineSubsidenceType_enum.Ineligible
                        divMineSub.Attributes.Add("style", "display:none;")
                        divMineSubLimitInfo.Attributes.Add("style", "display:none;")
                        chkMineBuilding.Checked = False
                        chkMineBuilding.Enabled = False
                        Exit Select
                End Select
            Else
                divMineSub.Attributes.Add("style", "display:none;")
                divMineSubReqHelpInfo_OH.Attributes.Add("style", "display:none;")
                chkMineBuilding.Checked = False
                chkMineBuilding.Enabled = False
            End If
        End If

        ' IL Mine Subsidence
        ' Set classes on the controls so we can manipulate them easily in jquery
        If IFM.VR.Common.Helpers.FARM.ILMineSubsidenceHelper.IsILMineSubsidenceAvailable(Quote) = True Then
            chkMineBuilding.Attributes.Remove("class")
            If MyLocation.Address.QuickQuoteState = QuickQuoteState.Illinois Then
                divMineSub.Attributes.Add("style", "display:block;margin-left:20px;")
                chkMineBuilding.Attributes.Add("class", "chkILMineSubsidence_" & MyLocationIndex.ToString)
                divMineSubReqHelpInfo.Attributes.Add("style", "display:none;")
                chkMineBuilding.Enabled = True
                Select Case IFM.VR.Common.Helpers.MineSubsidenceHelper.GetIllinoisMineSubsidenceTypeByCounty(MyLocation.Address.County)
                    Case MineSubsidenceHelper.IllinoisMineSubsidenceType_enum.EligibleMandatory
                        divMineSubReqHelpInfo.Attributes.Add("style", "display:block;margin-left:20px;")
                        ' When mandatory the coverage is checked by default 
                        chkMineBuilding.Checked = True
                        'Dim mineCov = (From cov In MyBuilding.OptionalCoverageEs Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Farm_Mine_Subsidence Select cov).FirstOrDefault()
                        ' Check to see if the location dwelling has mine sub, if so this bulding must as well (if it's a dwelling)
                        ' So if the building or it's containing location have mine sub check the box
                        'If mineCov IsNot Nothing OrElse IFM.VR.Common.Helpers.MultiState.General.QuoteHasILMineSubOnLocationDwelling(MyLocation) Then
                        '    chkMineBuilding.Checked = True
                        'End If
                        Exit Select
                    Case MineSubsidenceHelper.IllinoisMineSubsidenceType_enum.EligibleOptional
                        ' When optional, the coverage is shown and enabled.  We'll check it later
                        Dim mineCov = (From cov In MyBuilding.OptionalCoverageEs Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Farm_Mine_Subsidence Select cov).FirstOrDefault()
                        ' Check to see if the location dwelling has mine sub, if so this bulding must as well (if it's a dwelling)
                        ' So if the building or it's containing location have mine sub check the box
                        If mineCov IsNot Nothing OrElse IFM.VR.Common.Helpers.MultiState.General.QuoteHasILMineSubOnLocationDwelling(MyLocation) Then
                            chkMineBuilding.Checked = True
                        End If
                        Exit Select
                End Select
            End If
        End If
#If DEBUG Then
        Me.txtSuffocationLimit.ToolTip = String.Format("Description: {0}", Me.MySuffocationDescriptionIndicator)
#End If

        Exit Sub
    End Sub

    Private Sub RemoveAllCosmeticDamageExclusionCoverages()
        If MyLocation.SectionICoverages IsNot Nothing AndAlso MyLocation.SectionICoverages.Count > 0 Then
remloop:
            For Each sc As QuickQuoteSectionICoverage In MyLocation.SectionICoverages
                If sc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Cosmetic_Damage_Exclusion Then
                    MyLocation.SectionICoverages.Remove(sc)
                    GoTo remloop
                End If
            Next
        End If
    End Sub

    Public Overrides Sub Populate()
        If Me.MyBuilding IsNot Nothing Then

            LoadStaticData()

            PopulatePolicyLevelCoverages()

            ' OHIO
            ' Show or hide Cosmetic Damage Exclusion based on Effective Date and State
            ' Only display after OH eff date and if state is IL or OH

            If CosmeticDamageExHelper.IsCosmeticDamageExAvailable(Quote) Then
                dvCosmeticDamageExclusion.Attributes.Add("style", "display:block;margin-left:20px;")
                dvCosmeticDamageExclusionData.Visible = False
            Else
                ' Show or hide Cosmetic Damage Exclusion based on Effective Date and State
                ' Only display after OH eff date and if state is IL or OH
                If IsOhioEffective(Quote) AndAlso MyLocation.Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois OrElse MyLocation.Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                    dvCosmeticDamageExclusion.Attributes.Add("style", "display:block;margin-left:20px;")
                Else
                    dvCosmeticDamageExclusion.Attributes.Add("style", "display:none;") 'Added 8/9/2022 for task 74881 MLW
                    ' if we're before the OH effective date, need to remove any existing cosmetic damage coverages
                    RemoveAllCosmeticDamageExclusionCoverages()
                End If
            End If



            If Me.MyBuilding.OptionalCoverageEs IsNot Nothing Then
                For Each cov In Me.MyBuilding.OptionalCoverageEs
                    Select Case cov.CoverageType
                        Case QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Barn_Additional_Perils
                            Me.chkAdditionalPerils.Checked = True
                        Case QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Earthquake_Contents
                            Me.chkEarthQuake_contents.Checked = True
                        Case QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Barn_Earthquake
                            Me.chkEarthQuake_structure.Checked = True
                        Case QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Barn_Special_Form
                            Me.chkSpecialForm.Checked = True
                        Case QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Barn_Theft_of_Building_Materials
                            Me.chkBuildingMaterials.Checked = True
                        Case QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.LossOfIncome_Rents
                            dvLossIncomeLimit.Attributes.Add("style", "display:block;")
                            dvLossIncomeData.Attributes.Add("style", "display:none;")
                            lblLossIncome.Text = "Loss of Income - Rent"
                            Me.chkLossIncome.Checked = True
                            Me.txtLossIncomeLimit.Text = cov.IncreasedLimit
                        Case QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Barn_Replacement_Cost
                            Me.chkReplacement.Checked = True
                        Case QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Farm_Mine_Subsidence
                            Me.chkMineBuilding.Checked = True
                        Case QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Cosmetic_Damage_Exclusion_Coverage_E
                            'Updated 8/9/2022 for task 74881 MLW
                            'If MyLocation.Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois OrElse MyLocation.Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                            '    chkCosmeticDamageExclusion.Checked = True
                            '    dvCosmeticDamageExclusionData.Attributes.Add("style", "display:block;text-indent:25px;")
                            If CosmeticDamageExHelper.IsCosmeticDamageExAvailable(Quote) Then
                                chkCosmeticDamageExclusion.Checked = True
                                dvCosmeticDamageExclusionData.Attributes.Add("style", "display:block;text-indent:25px;")
                            Else
                                If MyLocation.Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois OrElse MyLocation.Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                                    chkCosmeticDamageExclusion.Checked = True
                                    dvCosmeticDamageExclusionData.Attributes.Add("style", "display:block;text-indent:25px;")
                                    If cov.ExteriorDoorWindowSurfacing Then chkCDEExteriorDoorAndWindowSurfacing.Checked = True
                                    If cov.ExteriorWallSurfacing Then chkCDEExteriorWallSurfacing.Checked = True
                                    If cov.RoofSurfacing Then chkCDERoofSurfacing.Checked = True
                                End If
                            End If

                            'End If
                            Exit Select
                    End Select
                Next
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divMain.ClientID
        'LoadStaticData()
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.MyBuilding IsNot Nothing Then
            If Me.MyBuilding.Equals(IFM.VR.Common.Helpers.BuildingsHelper.FindFirstBuilding(Me.Quote)) Then
                ' CONTRACT GROWERS/CUSTOM FEEDING
                If CDate(Quote.EffectiveDate) < SuffocationAndCustomFeedingCutoffDate Then
                    ' Effective date less than cutoff date, use Contract Growers coverage
                    If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                            'Policy Level Coverage - Farm Property - Contract Growers CCC
                            sq.FarmContractGrowersCareCustodyControlLimitId = If(Me.chkContractGrower.Checked, Me.ddContractGrowerLimit.SelectedValue, "")
                        Next
                    End If
                Else
                    'Updated 11/3/2022 for task 60749 MLW
                    If IFM.VR.Common.Helpers.FARM.FarmCustomFeeding.IsFARCustomFeedingAvailable(Quote) = False Then
                        ' Effective date greater than cutoff date, use Custom Feeding coverage
                        If chkCustomFeeding.Checked Then
                            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                                For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                                    'Policy Level Coverage - Custom Feeding
                                    ' Swine
                                    If ddCFSwineLimit.SelectedValue <> "" AndAlso ddCFSwineLimit.SelectedValue <> "0" Then
                                        sq.FarmCustomFeedingSwineDescription = txtCFSwineDesc.Text
                                        sq.FarmCustomFeedingSwineLimitId = ddCFSwineLimit.SelectedValue
                                    Else
                                        sq.FarmCustomFeedingSwineDescription = ""
                                        sq.FarmCustomFeedingSwineLimitId = ""
                                    End If
                                    ' Poultry
                                    If ddCFPoultryLimit.SelectedValue <> "" AndAlso ddCFPoultryLimit.SelectedValue <> "0" Then
                                        sq.FarmCustomFeedingPoultryDescription = txtCFPoultryDesc.Text
                                        sq.FarmCustomFeedingPoultryLimitId = ddCFPoultryLimit.SelectedValue
                                    Else
                                        sq.FarmCustomFeedingPoultryDescription = ""
                                        sq.FarmCustomFeedingPoultryLimitId = ""
                                    End If
                                    ' Cattle
                                    If ddCFCattleLimit.SelectedValue <> "" AndAlso ddCFCattleLimit.SelectedValue <> "0" Then
                                        sq.FarmCustomFeedingCattleDescription = txtCFCattleDesc.Text
                                        sq.FarmCustomFeedingCattleLimitId = ddCFCattleLimit.SelectedValue
                                    Else
                                        sq.FarmCustomFeedingCattleDescription = ""
                                        sq.FarmCustomFeedingCattleLimitId = ""
                                    End If
                                    ' Equine
                                    If ddCFEquineLimit.SelectedValue <> "" And ddCFEquineLimit.SelectedValue <> "0" Then
                                        sq.FarmCustomFeedingEquineDescription = txtCFEquineDesc.Text
                                        sq.FarmCustomFeedingEquineLimitId = ddCFEquineLimit.SelectedValue
                                    Else
                                        sq.FarmCustomFeedingEquineDescription = ""
                                        sq.FarmCustomFeedingEquineLimitId = ""
                                    End If
                                Next
                            End If
                        Else
                            ' Custom feeding NOT checked - remove any existing
                            For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                                sq.FarmCustomFeedingSwineDescription = txtCFSwineDesc.Text
                                sq.FarmCustomFeedingSwineLimitId = ddCFSwineLimit.SelectedValue
                                sq.FarmCustomFeedingPoultryDescription = txtCFPoultryDesc.Text
                                sq.FarmCustomFeedingPoultryLimitId = ddCFPoultryLimit.SelectedValue
                                sq.FarmCustomFeedingCattleDescription = txtCFCattleDesc.Text
                                sq.FarmCustomFeedingCattleLimitId = ddCFCattleLimit.SelectedValue
                                sq.FarmCustomFeedingEquineDescription = txtCFEquineDesc.Text
                                sq.FarmCustomFeedingEquineLimitId = ddCFEquineLimit.SelectedValue
                            Next
                        End If
                    End If
                End If
            End If

            ' SUFFOCATION OF LIVESTOCK/SUFFOCATION OF LIVESTOCK AND POULTRY
            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                    If CDate(Quote.EffectiveDate) < SuffocationAndCustomFeedingCutoffDate Then
                        ' Effective date less than cutoff date, use Suffocation of Livestock coverage
                        'Policy Level Coverage - Optional Liability - Suffocation of Livestock
                        'Updated 9/10/18 for multi state MLW - Quote to SubQuotes sq
                        If sq.OptionalCoverages Is Nothing Then
                            sq.OptionalCoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteOptionalCoverage)()
                        End If
                        Dim suffocationCov = (From cov In sq.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock And cov.Description = Me.MySuffocationDescriptionIndicator Select cov).FirstOrDefault()
                        If Me.chkSuffocation.Checked Then
                            If suffocationCov Is Nothing Then
                                'create it
                                suffocationCov = New QuickQuote.CommonObjects.QuickQuoteOptionalCoverage() With {.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock}
                                sq.OptionalCoverages.Add(suffocationCov)
                            End If
                            'set description
                            suffocationCov.Description = Me.MySuffocationDescriptionIndicator
                            'set limit
                            suffocationCov.IncreasedLimit = Me.txtSuffocationLimit.Text
                        Else
                            If suffocationCov IsNot Nothing Then
                                'remove it
                                sq.OptionalCoverages.Remove(suffocationCov)
                            End If
                        End If
                    Else
                        ' Effective date >= than cutoff date, use Suffocation of Livestock and Poultry coverage
                        'Policy Level Coverage - Optional Liability - Suffocation of Livestock and Poultry
                        If sq.OptionalCoverages Is Nothing Then
                            sq.OptionalCoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteOptionalCoverage)()
                        End If

                        ' Cattle
                        If txtSuffCattleLimit.Enabled Then
                            Dim suffocationCov = (From cov In sq.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Cattle Select cov).FirstOrDefault()
                            If chkSuffocationOfLivestockOrPoultry.Checked AndAlso Me.txtSuffCattleLimit.Text <> "" Then
                                If suffocationCov Is Nothing Then
                                    'create it
                                    suffocationCov = New QuickQuote.CommonObjects.QuickQuoteOptionalCoverage() With {.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Cattle}
                                    sq.OptionalCoverages.Add(suffocationCov)
                                End If
                                'set description
                                suffocationCov.Description = Me.txtSuffCattleDesc.Text
                                'set limit
                                suffocationCov.IncreasedLimit = Me.txtSuffCattleLimit.Text
                            Else
                                If suffocationCov IsNot Nothing Then
                                    'remove it
                                    sq.OptionalCoverages.Remove(suffocationCov)
                                End If
                            End If
                        End If

                        ' Equine
                        If txtSuffEquineLimit.Enabled Then
                            Dim suffocationCov = (From cov In sq.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Equine Select cov).FirstOrDefault()
                            If chkSuffocationOfLivestockOrPoultry.Checked AndAlso Me.txtSuffEquineLimit.Text <> "" Then
                                If suffocationCov Is Nothing Then
                                    'create it
                                    suffocationCov = New QuickQuote.CommonObjects.QuickQuoteOptionalCoverage() With {.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Equine}
                                    sq.OptionalCoverages.Add(suffocationCov)
                                End If
                                'set description
                                suffocationCov.Description = Me.txtSuffEquineDesc.Text
                                'set limit
                                suffocationCov.IncreasedLimit = Me.txtSuffEquineLimit.Text
                            Else
                                If suffocationCov IsNot Nothing Then
                                    'remove it
                                    sq.OptionalCoverages.Remove(suffocationCov)
                                End If
                            End If
                        End If

                        ' Poultry
                        If txtSuffPoultryLimit.Enabled Then
                            Dim suffocationCov = (From cov In sq.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Poultry Select cov).FirstOrDefault()
                            If chkSuffocationOfLivestockOrPoultry.Checked AndAlso Me.txtSuffPoultryLimit.Text <> "" Then
                                If suffocationCov Is Nothing Then
                                    'create it
                                    suffocationCov = New QuickQuote.CommonObjects.QuickQuoteOptionalCoverage() With {.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Poultry}
                                    sq.OptionalCoverages.Add(suffocationCov)
                                End If
                                'set description
                                suffocationCov.Description = Me.txtSuffPoultryDesc.Text
                                'set limit
                                suffocationCov.IncreasedLimit = Me.txtSuffPoultryLimit.Text
                            Else
                                If suffocationCov IsNot Nothing Then
                                    'remove it
                                    sq.OptionalCoverages.Remove(suffocationCov)
                                End If
                            End If
                        End If

                        ' Swine
                        If txtSuffSwineLimit.Enabled Then
                            Dim suffocationCov = (From cov In sq.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Swine Select cov).FirstOrDefault()
                            If chkSuffocationOfLivestockOrPoultry.Checked AndAlso Me.txtSuffSwineLimit.Text <> "" Then
                                If suffocationCov Is Nothing Then
                                    'create it
                                    suffocationCov = New QuickQuote.CommonObjects.QuickQuoteOptionalCoverage() With {.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Swine}
                                    sq.OptionalCoverages.Add(suffocationCov)
                                End If
                                'set description
                                suffocationCov.Description = Me.txtSuffSwineDesc.Text
                                'set limit
                                suffocationCov.IncreasedLimit = Me.txtSuffSwineLimit.Text
                            Else
                                If suffocationCov IsNot Nothing Then
                                    'remove it
                                    sq.OptionalCoverages.Remove(suffocationCov)
                                End If
                            End If
                        End If
                    End If
                Next
            End If

            'Building Level - Optional Coverages
            If Me.MyBuilding.OptionalCoverageEs Is Nothing Then
                Me.MyBuilding.OptionalCoverageEs = New List(Of QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE)()
            End If

            ' this creates an empty list of an anonymous type to hold the bindings
            Dim bindingsList = {(New With {.CheckBox = New CheckBox(), .CovType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Barn_Additional_Perils, .Limit = ""})}.Take(0).ToList()
            bindingsList.Add(New With {.CheckBox = Me.chkAdditionalPerils, .CovType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Barn_Additional_Perils, .Limit = ""})
            bindingsList.Add(New With {.CheckBox = Me.chkEarthQuake_contents, .CovType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Earthquake_Contents, .Limit = ""})
            bindingsList.Add(New With {.CheckBox = Me.chkEarthQuake_structure, .CovType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Barn_Earthquake, .Limit = ""})
            bindingsList.Add(New With {.CheckBox = Me.chkReplacement, .CovType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Barn_Replacement_Cost, .Limit = ""})
            bindingsList.Add(New With {.CheckBox = Me.chkBuildingMaterials, .CovType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Barn_Theft_of_Building_Materials, .Limit = ""})
            'bindingsList.Add(New With {.CheckBox = Me.chkLossIncome, .CovType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.LossOfIncome_Rents, .Limit = Me.txtLossIncomeLimit.Text})
            bindingsList.Add(New With {.CheckBox = Me.chkSpecialForm, .CovType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Barn_Special_Form, .Limit = ""})
            'bindingsList.Add(New With {.CheckBox = Me.chkMine, .CovType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Farm_Mine_Subsidence, .Limit = ""})
            bindingsList.Add(New With {.CheckBox = Me.chkMineBuilding, .CovType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Farm_Mine_Subsidence, .Limit = ""})
            bindingsList.Add(New With {.CheckBox = Me.chkCosmeticDamageExclusion, .CovType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Cosmetic_Damage_Exclusion_Coverage_E, .Limit = ""})

            ' Check for Farm Dwelling and Mobile Home Dwelling
            If MyBuilding.FarmStructureTypeId <> "18" And MyBuilding.FarmStructureTypeId <> "17" Then
                If MyLocation.IncomeLosses Is Nothing Then
                    MyLocation.IncomeLosses = New List(Of QuickQuote.CommonObjects.QuickQuoteIncomeLoss)
                End If
                If chkLossIncome.Checked Then
                    Dim lossIncome = MyLocation.IncomeLosses.Find(Function(p) p.Description.Trim() = String.Format("LOC{0}BLD{1}", (MyLocationIndex + 1), (MyBuildingIndex + 1)))

                    If lossIncome Is Nothing Then
                        Dim incomeLoss = New QuickQuote.CommonObjects.QuickQuoteIncomeLoss
                        incomeLoss.Limit = txtLossIncomeLimit.Text
                        incomeLoss.Description = String.Format("LOC{0}BLD{1}", (MyLocationIndex + 1), (MyBuildingIndex + 1))
                        incomeLoss.CoinsuranceTypeId = ddlCoInsurance.SelectedValue
                        incomeLoss.ExtendFarmIncomeOptionId = ddlLossExt.SelectedValue
                        MyLocation.IncomeLosses.Add(incomeLoss)
                    Else
                        lossIncome.Limit = txtLossIncomeLimit.Text
                        lossIncome.Description = String.Format("LOC{0}BLD{1}", (MyLocationIndex + 1), (MyBuildingIndex + 1))
                        lossIncome.CoinsuranceTypeId = ddlCoInsurance.SelectedValue
                        lossIncome.ExtendFarmIncomeOptionId = ddlLossExt.SelectedValue
                    End If
                Else
                    Dim lossInc = MyLocation.IncomeLosses.Find(Function(p) p.Description.Trim() = String.Format("LOC{0}BLD{1}", (MyLocationIndex + 1), (MyBuildingIndex + 1)))

                    If lossInc IsNot Nothing Then
                        Dim buldingNum = lossInc.Description.Trim()
                        MyLocation.IncomeLosses.Remove(lossInc)
                    End If

                    Dim lossIncomeRents = MyBuilding.OptionalCoverageEs.Find(Function(p) p.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.LossOfIncome_Rents)
                    If lossIncomeRents IsNot Nothing Then
                        MyBuilding.OptionalCoverageEs.Remove(lossIncomeRents)
                    End If

                    dvLossIncomeData.Attributes.Add("style", "display:none;")
                    dvLossIncomeLimit.Attributes.Add("style", "display:none;")
                End If
            Else
                If chkLossIncome.Checked Then
                    If MyLocation.IncomeLosses IsNot Nothing Then
                        Dim lossIncome = MyLocation.IncomeLosses.Find(Function(p) p.Description.Trim() = String.Format("LOC{0}BLD{1}", (MyLocationIndex + 1), (MyBuildingIndex + 1)))

                        If lossIncome IsNot Nothing Then
                            MyLocation.IncomeLosses.Remove(lossIncome)
                        End If
                    End If
                Else
                    dvLossIncomeData.Attributes.Add("style", "display:none;")
                    dvLossIncomeLimit.Attributes.Add("style", "display:none;")
                End If

                bindingsList.Add(New With {.CheckBox = Me.chkLossIncome, .CovType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.LossOfIncome_Rents, .Limit = Me.txtLossIncomeLimit.Text})
            End If
            'Dim buildingNum As Integer = 0

            For Each binding In bindingsList
                'If MyBuilding.FarmStructureTypeId = "18" Then
                Dim cov = (From c In Me.MyBuilding.OptionalCoverageEs Where c.CoverageType = binding.CovType Select c).FirstOrDefault()
                If binding.CheckBox.Checked Then
                    ' CHECKED
                    'need to add?
                    Dim newcov As Boolean = False
                    If cov Is Nothing Then
                        cov = New QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE() With {.CoverageType = binding.CovType}
                        newcov = True
                    End If
                    'set limit value - most of the time it is blank
                    cov.IncreasedLimit = binding.Limit

                    If binding.CovType = QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Cosmetic_Damage_Exclusion_Coverage_E Then
                        If CosmeticDamageExHelper.IsCosmeticDamageExAvailable(Quote) Then
                            'do nothing
                        Else
                            cov.ExteriorDoorWindowSurfacing = chkCDEExteriorDoorAndWindowSurfacing.Checked
                            cov.ExteriorWallSurfacing = chkCDEExteriorWallSurfacing.Checked
                            cov.RoofSurfacing = chkCDERoofSurfacing.Checked
                        End If

                    End If
                    If newcov Then Me.MyBuilding.OptionalCoverageEs.Add(cov)
                Else
                    ' NOT CHECKED
                    ' if cov isnot nothing then it exists and you need to remove it
                    If cov IsNot Nothing Then
                        Me.MyBuilding.OptionalCoverageEs.Remove(cov)
                    End If
                End If
            Next

            PopulatePolicyLevelCoverages() ' here so you don't have to do a full repopulate
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = String.Format("Location #{0} Building #{1} - Coverage", MyLocationIndex + 1, MyBuildingIndex + 1)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim valList = FarmBuildingCoveragesValidator.ValidateFARBuildingCoverages(Me.Quote, Me.MyLocationIndex, Me.MyBuildingIndex, valArgs.ValidationType)
        trCDEValidationMessage.Attributes.Add("style", "display:none")

        If valList.Any() Then
            For Each v In valList
                Select Case v.FieldId
                    Case FarmBuildingCoveragesValidator.additionalPerils
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.chkAdditionalPerils, v, accordList)
                    Case FarmBuildingCoveragesValidator.eq_contents
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.chkEarthQuake_contents, v, accordList)
                    Case FarmBuildingCoveragesValidator.mineSubsidence
                        If MyLocation.Address.QuickQuoteState <> QuickQuoteState.Illinois Then
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.chkMineBuilding, v, accordList)
                        End If
                    Case FarmBuildingCoveragesValidator.businessIncome
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLossIncomeLimit, v, accordList)
                    Case FarmBuildingCoveragesValidator.sufficationCov
                        ' Use the old validation logic for suffication of livestock if eff date is before the cutoff date
                        If CDate(Quote.EffectiveDate) < SuffocationAndCustomFeedingCutoffDate Then
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSuffocationLimit, v, accordList)
                        End If
                    Case FarmBuildingCoveragesValidator.coInsuranceIsNull
                        Me.ValidationHelper.Val_BindValidationItemToControl(ddlCoInsurance, v, accordList)
                    Case FarmBuildingCoveragesValidator.lossExtensionIsNull
                        Me.ValidationHelper.Val_BindValidationItemToControl(ddlLossExt, v, accordList)
                    Case FarmBuildingCoveragesValidator.CosmeticDamageExclusionOptionRequired
                        Me.ValidationHelper.Val_BindValidationItemToControl(New Control(), v, accordList)
                        trCDEValidationMessage.Attributes.Add("style", "display:block")
                        Exit Select
                End Select
            Next
        End If

        ' Validate the new Custom Feeding and Suffocation of Livestock coverages if eff date > cutoff date
        If Quote.EffectiveDate >= SuffocationAndCustomFeedingCutoffDate Then
            'Updated 11/3/2022 for task 60749 MLW
            ' Custom Feeding
            If IFM.VR.Common.Helpers.FARM.FarmCustomFeeding.IsFARCustomFeedingAvailable(Quote) = False Then
                If chkCustomFeeding.Checked Then
                    If (ddCFCattleLimit.SelectedValue = "" OrElse ddCFCattleLimit.SelectedValue = "0") AndAlso (ddCFEquineLimit.SelectedValue = "" OrElse ddCFEquineLimit.SelectedValue = "0") AndAlso (ddCFPoultryLimit.SelectedValue = "" OrElse ddCFPoultryLimit.SelectedValue = "0") AndAlso (ddCFSwineLimit.SelectedValue = "" OrElse ddCFSwineLimit.SelectedValue = "0") Then
                        Me.ValidationHelper.AddError("When Custom Feeding is selected you must select at least one limit for Cattle, Equine, Poultry or Swine and enter a description for your selection(s).")
                    End If
                    If IsQuoteEndorsement() = False Then
                        If ddCFCattleLimit.SelectedValue <> "" AndAlso ddCFCattleLimit.SelectedValue <> "0" Then
                            If txtCFCattleDesc.Text.Trim = "" Then Me.ValidationHelper.AddError("Cattle description is required", txtCFCattleDesc.ClientID)
                        End If
                        If ddCFEquineLimit.SelectedValue <> "" AndAlso ddCFEquineLimit.SelectedValue <> "0" Then
                            If txtCFEquineDesc.Text.Trim = "" Then Me.ValidationHelper.AddError("Equine description is required", txtCFEquineDesc.ClientID)
                        End If
                        If ddCFPoultryLimit.SelectedValue <> "" AndAlso ddCFPoultryLimit.SelectedValue <> "0" Then
                            If txtCFPoultryDesc.Text.Trim = "" Then Me.ValidationHelper.AddError("Poultry description is required", txtCFPoultryDesc.ClientID)
                        End If
                        If ddCFSwineLimit.SelectedValue <> "" AndAlso ddCFSwineLimit.SelectedValue <> "0" Then
                            If txtCFSwineDesc.Text.Trim = "" Then Me.ValidationHelper.AddError("Swine description is required", txtCFSwineDesc.ClientID)
                        End If
                    End If
                End If
            End If

            ' Suffocation of Livestock
            If chkSuffocationOfLivestockOrPoultry.Checked Then
                If txtSuffCattleLimit.Text.Trim = "" AndAlso txtSuffEquineLimit.Text.Trim = "" AndAlso txtSuffPoultryLimit.Text.Trim = "" AndAlso txtSuffSwineLimit.Text.Trim = "" Then
                    Me.ValidationHelper.AddError("When Suffocation of Livestock or Poultry is selected you must enter at least one limit for Cattle, Equine, Poultry or Swine and enter a description for your selection(s).")
                End If
                If txtSuffCattleLimit.Text.Trim <> "" Then
                    If txtSuffCattleDesc.Text.Trim = "" Then Me.ValidationHelper.AddError("Cattle description is required", txtSuffCattleDesc.ClientID)
                End If
                If txtSuffEquineLimit.Text.Trim <> "" Then
                    If txtSuffEquineDesc.Text.Trim = "" Then Me.ValidationHelper.AddError("Equine description is required", txtSuffEquineDesc.ClientID)
                End If
                If txtSuffPoultryLimit.Text.Trim <> "" Then
                    If txtSuffPoultryDesc.Text.Trim = "" Then Me.ValidationHelper.AddError("Poultry description is required", txtSuffPoultryDesc.ClientID)
                End If
                If txtSuffSwineLimit.Text.Trim <> "" Then
                    If txtSuffSwineDesc.Text.Trim = "" Then Me.ValidationHelper.AddError("Swine description is required", txtSuffSwineDesc.ClientID)
                End If
            End If
        End If
    End Sub

    Private Function QuoteHasContractGrowersCoverage() As Boolean
        If SubQuoteFirst IsNot Nothing Then
            If Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmContractGrowersCareCustodyControlLimitId) Then Return True
        End If

        Return False
    End Function

    Private Function QuoteHasCustomFeedingCoverage(Optional ByVal CustomFeedingType As AnimalType_enum = AnimalType_enum.All) As Boolean
        If CustomFeedingType = AnimalType_enum.All Then
            If ((Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmCustomFeedingSwineLimitId) AndAlso SubQuoteFirst.FarmCustomFeedingSwineLimitId <> "0")) OrElse ((Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmCustomFeedingPoultryLimitId) AndAlso SubQuoteFirst.FarmCustomFeedingPoultryLimitId <> "0")) OrElse ((Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmCustomFeedingCattleLimitId)) AndAlso SubQuoteFirst.FarmCustomFeedingCattleLimitId <> "0") OrElse ((Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmCustomFeedingEquineLimitId)) AndAlso SubQuoteFirst.FarmCustomFeedingEquineLimitId <> "0") Then
                Return True
            End If
        Else
            Select Case CustomFeedingType
                Case AnimalType_enum.Swine
                    If Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmCustomFeedingSwineLimitId) Then Return True
                    Exit Select
                Case AnimalType_enum.Poultry
                    If Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmCustomFeedingPoultryLimitId) Then Return True
                    Exit Select
                Case AnimalType_enum.Cattle
                    If Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmCustomFeedingCattleLimitId) Then Return True
                    Exit Select
                Case AnimalType_enum.Equine
                    If Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmCustomFeedingEquineLimitId) Then Return True
                    Exit Select
            End Select
        End If

        Return False
    End Function

    Private Function QuoteHAsSuffocationOfLivestockAndPoultry(Optional SuffType As AnimalType_enum = AnimalType_enum.All) As Boolean
        If SuffType = AnimalType_enum.All Then
            If SubQuoteFirst.OptionalCoverages IsNot Nothing Then
                Dim suffocationCov = (From cov In SubQuoteFirst.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Cattle Select cov).FirstOrDefault()
                If suffocationCov IsNot Nothing Then Return True
                suffocationCov = (From cov In SubQuoteFirst.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Equine Select cov).FirstOrDefault()
                If suffocationCov IsNot Nothing Then Return True
                suffocationCov = (From cov In SubQuoteFirst.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Poultry Select cov).FirstOrDefault()
                If suffocationCov IsNot Nothing Then Return True
                suffocationCov = (From cov In SubQuoteFirst.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Swine Select cov).FirstOrDefault()
                If suffocationCov IsNot Nothing Then Return True
            End If
        Else
            Select Case SuffType
                Case AnimalType_enum.Cattle
                    Dim suffocationCov = (From cov In SubQuoteFirst.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Cattle Select cov).FirstOrDefault()
                    If suffocationCov IsNot Nothing Then Return True
                    Exit Select
                Case AnimalType_enum.Equine
                    Dim suffocationCov = (From cov In SubQuoteFirst.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Equine Select cov).FirstOrDefault()
                    If suffocationCov IsNot Nothing Then Return True
                    Exit Select
                Case AnimalType_enum.Poultry
                    Dim suffocationCov = (From cov In SubQuoteFirst.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Poultry Select cov).FirstOrDefault()
                    If suffocationCov IsNot Nothing Then Return True
                    Exit Select
                Case AnimalType_enum.Swine
                    Dim suffocationCov = (From cov In SubQuoteFirst.OptionalCoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Swine Select cov).FirstOrDefault()
                    If suffocationCov IsNot Nothing Then Return True
                    Exit Select
            End Select
        End If

        Return False
    End Function

    Private Function QuoteHasSuffocationOfLivestock(ByRef rtnFirstCoverage As QuickQuoteOptionalCoverage) As Boolean
        If SubQuoteFirst.OptionalCoverages IsNot Nothing Then
            For Each oc As QuickQuoteOptionalCoverage In SubQuoteFirst.OptionalCoverages
                If oc.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock Then
                    If oc.IncreasedLimit <> "" AndAlso oc.IncreasedLimit <> "0" AndAlso oc.IncreasedLimit <> "0.00" Then
                        rtnFirstCoverage = oc
                        Return True
                    End If
                End If
            Next
        End If

        rtnFirstCoverage = Nothing
        Return False
    End Function

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Session("valuationValue") = "False"
        Me.Save_FireSaveEvent()
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        Me.chkAdditionalPerils.Checked = False
        Me.chkBuildingMaterials.Checked = False
        Me.chkContractGrower.Checked = False
        Me.chkEarthQuake_contents.Checked = False
        Me.chkEarthQuake_structure.Checked = False
        Me.chkLossIncome.Checked = False
        'Updated 11/6/18 for multi state MLW
        Select Case (MyLocation.Address.StateId)
            Case States.Abbreviations.IL
                If MineSubsidenceHelper.MineSubCountiesByStateAbbreviation(MyLocation.Address.State).Contains(MyLocation.Address.County.Trim.ToUpper()) Then
                    'keep the same, do not uncheck                    
                Else
                    chkMineBuilding.Checked = False
                End If
            Case States.Abbreviations.IN
                chkMineBuilding.Checked = False
            Case Else
                chkMineBuilding.Checked = False
        End Select
        Me.chkReplacement.Checked = False
        Me.chkSpecialForm.Checked = False
        Me.chkSuffocation.Checked = False
        Me.txtLossIncomeLimit.Text = ""
        Me.txtSuffocationLimit.Text = ""
        Me.chkCustomFeeding.Checked = False
        Me.dvCustomFeedingData.Attributes.Add("style", "display:none")
        Me.chkSuffocationOfLivestockOrPoultry.Checked = False
        Me.dvSuffocationOfLivestockOrPoultryData.Attributes.Add("style", "display:none")
        Me.ddCFCattleLimit.SelectedIndex = -1
        Me.ddCFEquineLimit.SelectedIndex = -1
        Me.ddCFPoultryLimit.SelectedIndex = -1
        Me.ddCFSwineLimit.SelectedIndex = -1
        Me.txtCFCattleDesc.Text = ""
        Me.txtCFEquineDesc.Text = ""
        Me.txtCFPoultryDesc.Text = ""
        Me.txtCFSwineDesc.Text = ""
        Me.txtSuffCattleDesc.Text = ""
        Me.txtSuffCattleLimit.Text = ""
        Me.txtSuffEquineDesc.Text = ""
        Me.txtSuffEquineLimit.Text = ""
        Me.txtSuffPoultryDesc.Text = ""
        Me.txtSuffPoultryLimit.Text = ""
        Me.txtSuffSwineDesc.Text = ""
        Me.txtSuffSwineLimit.Text = ""

        Exit Sub
    End Sub

    Protected Sub lnkBtnClear_Click(sender As Object, e As EventArgs) Handles lnkBtnClear.Click
        Session("valuationValue") = "False"
        ClearControl()
        Me.Save_FireSaveEvent(False)
    End Sub

    '11/7/2022 - No longer used as the effective date crossing line was 7/1/2020
    '    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
    '        ' THIS EVENT FIRES BEFORE POPULATE!

    '        ' Check to see if custom feeding/suffocation cutoff date was crossed
    '        ' CUSTOM FEEDING
    '        If CDate(OldEffectiveDate) < SuffocationAndCustomFeedingCutoffDate And CDate(NewEffectiveDate) >= SuffocationAndCustomFeedingCutoffDate Then
    '            ' Crossed from old to new
    '            If QuoteHasContractGrowersCoverage() Then
    '                ' Add custom feeding swine (new coverage) using the contract growers info
    '                For Each sq As QuickQuoteObject In SubQuotes
    '                    sq.FarmCustomFeedingSwineDescription = sq.FarmContractGrowersCareCustodyControlDescription
    '                    Select Case sq.FarmContractGrowersCareCustodyControlLimitId
    '                        Case "55"   ' 250,0000 converts to 250k/1,000k
    '                            sq.FarmCustomFeedingSwineLimitId = "434"
    '                            Exit Select
    '                        Case "34"   ' 500,000 converts to 500k/1,000k
    '                            sq.FarmCustomFeedingSwineLimitId = "435"
    '                            Exit Select
    '                        Case "56"   ' 1,000,000 converts to 1,000k/1,000k
    '                            sq.FarmCustomFeedingSwineLimitId = "437"
    '                            Exit Select
    '                    End Select

    '                    ' Remove the contract growers coverage
    '                    sq.FarmContractGrowersCareCustodyControlDescription = ""
    '                    sq.FarmContractGrowersCareCustodyControlLimitId = ""
    '                Next
    '            End If
    '        ElseIf CDate(OldEffectiveDate) >= SuffocationAndCustomFeedingCutoffDate And CDate(NewEffectiveDate) < SuffocationAndCustomFeedingCutoffDate Then
    '            ' Crossed from new to old
    '            If QuoteHasCustomFeedingCoverage(AnimalType_enum.Swine) Then
    '                For Each sq As QuickQuoteObject In SubQuotes
    '                    ' Add Contract Growers coverage using the Custom feeding - swine values
    '                    sq.FarmContractGrowersCareCustodyControlDescription = sq.FarmCustomFeedingSwineDescription
    '                    Select Case sq.FarmCustomFeedingSwineLimitId
    '                        Case "431", "432", "433", "434"  ' 25k/100k, 50k/250k, 100k/500k, 250k/1,000k convert to 250k
    '                            sq.FarmContractGrowersCareCustodyControlLimitId = "55"
    '                            Exit Select
    '                        Case "435"  ' 500k/1,000k converts to 500k
    '                            sq.FarmContractGrowersCareCustodyControlLimitId = "34"
    '                            Exit Select
    '                        Case "436"  ' 750k/1,000k and 1,000k/1,000k converts to 1,000k
    '                            sq.FarmContractGrowersCareCustodyControlLimitId = "56"
    '                            Exit Select
    '                    End Select

    '                    ' Remove all Custom Feeding coverages
    '                    sq.FarmCustomFeedingSwineDescription = ""
    '                    sq.FarmCustomFeedingSwineLimitId = ""
    '                    sq.FarmCustomFeedingSwineQuotedPremium = ""

    '                    sq.FarmCustomFeedingPoultryDescription = ""
    '                    sq.FarmCustomFeedingPoultryLimitId = ""
    '                    sq.FarmCustomFeedingPoultryQuotedPremium = ""

    '                    sq.FarmCustomFeedingCattleDescription = ""
    '                    sq.FarmCustomFeedingCattleLimitId = ""
    '                    sq.FarmCustomFeedingCattleQuotedPremium = ""

    '                    sq.FarmCustomFeedingEquineDescription = ""
    '                    sq.FarmCustomFeedingEquineLimitId = ""
    '                    sq.FarmCustomFeedingEquineQuotedPremium = ""
    '                Next
    '            End If
    '        Else
    '            ' Did not cross cutoff date - do nothing
    '        End If

    '        ' SUFFOCATION OF LIVESTOCK/POULTRY
    '        If CDate(OldEffectiveDate) < SuffocationAndCustomFeedingCutoffDate And CDate(NewEffectiveDate) >= SuffocationAndCustomFeedingCutoffDate Then
    '            ' Crossed from old to new
    '            Dim OriginalCoverage As New QuickQuoteOptionalCoverage()
    '            If QuoteHasSuffocationOfLivestock(OriginalCoverage) Then
    '                For Each sq As QuickQuoteObject In SubQuotes
    'Remove1:
    '                    ' Remove any existing Suffocation of Livestock or Poultry coverages
    '                    If sq.OptionalCoverages IsNot Nothing Then
    '                        For Each oc As QuickQuoteOptionalCoverage In sq.OptionalCoverages
    '                            If oc.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Cattle OrElse
    '                                oc.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Equine OrElse
    '                                oc.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Poultry OrElse
    '                                oc.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Swine Then
    '                                sq.OptionalCoverages.Remove(oc)
    '                                GoTo Remove1 ' restart the enumeration after each remove
    '                            End If
    '                        Next
    '                    End If

    '                    ' Create new Suffocation of Livestock and Poultry - swine coverage and populate with the values from the old coverage
    '                    Dim newcov As New QuickQuoteOptionalCoverage()
    '                    newcov.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Swine
    '                    'newcov.Description = OriginalCoverage.Description ' Description is not stored in the old coverage
    '                    newcov.IncreasedLimit = OriginalCoverage.IncreasedLimit
    '                    sq.OptionalCoverages.Add(newcov)

    'Remove2:
    '                    ' Remove any old Suffocation of Livestock coverages
    '                    For Each oc As QuickQuoteOptionalCoverage In sq.OptionalCoverages
    '                        If oc.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock Then
    '                            sq.OptionalCoverages.Remove(oc)
    '                            GoTo Remove2 ' restart the enumeration after each remove
    '                        End If
    '                    Next
    '                Next
    '            End If
    '        ElseIf CDate(OldEffectiveDate) >= SuffocationAndCustomFeedingCutoffDate And CDate(NewEffectiveDate) < SuffocationAndCustomFeedingCutoffDate Then
    '            ' Crossed from new to old 
    '            If QuoteHAsSuffocationOfLivestockAndPoultry(AnimalType_enum.Swine) Then
    '                For Each sq As QuickQuoteObject In SubQuotes
    'Remove3:
    '                    ' Remove any existing Suffocation of Livestock coverage(s)
    '                    If sq.OptionalCoverages IsNot Nothing Then
    '                        For Each oc As QuickQuoteOptionalCoverage In sq.OptionalCoverages
    '                            If oc.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock Then
    '                                sq.OptionalCoverages.Remove(oc)
    '                                GoTo Remove3 ' restart the enumeration after each remove
    '                            End If
    '                        Next
    '                    End If

    '                    Dim OriginalCoverage As QuickQuoteOptionalCoverage = Nothing
    '                    If sq.OptionalCoverages IsNot Nothing Then
    '                        For Each oc As QuickQuoteOptionalCoverage In sq.OptionalCoverages
    '                            If oc.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Swine Then
    '                                If oc.IncreasedLimit IsNot Nothing AndAlso oc.IncreasedLimit <> "" AndAlso IsNumeric(oc.IncreasedLimit) AndAlso CDec(oc.IncreasedLimit) > 0 Then
    '                                    OriginalCoverage = oc
    '                                    Exit For
    '                                End If
    '                            End If
    '                        Next
    '                    End If

    '                    If OriginalCoverage IsNot Nothing Then
    '                        ' Create new Suffocation of Livestock coverage and populate with the values from the original coverage
    '                        Dim newcov As New QuickQuoteOptionalCoverage()
    '                        newcov.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock
    '                        newcov.Description = "1.1" ' Description is used to store the location and building indexes
    '                        newcov.IncreasedLimit = OriginalCoverage.IncreasedLimit
    '                        sq.OptionalCoverages.Add(newcov)

    'Remove4:
    '                        ' Remove all old Suffocation of Livestock - (cattle, equine, poultry, swine) coverages
    '                        For Each oc As QuickQuoteOptionalCoverage In sq.OptionalCoverages
    '                            If oc.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Cattle OrElse
    '                                    oc.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Equine OrElse
    '                                    oc.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Poultry OrElse
    '                                    oc.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Swine Then
    '                                sq.OptionalCoverages.Remove(oc)
    '                                GoTo Remove4 ' restart the enumeration after each remove
    '                            End If
    '                        Next
    '                    End If
    '                Next
    '            Else
    '                ' Did not cross cutoff date - do nothing
    '            End If
    '        End If

    '        Exit Sub
    '    End Sub
End Class