Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.FarmLines.Buildings
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.FARM

Public Class ctl_FarmBuilding_Property
    Inherits VRControlBase

    Public Const HayStorageText As String = "HAY STORAGE - "
    Public Const VR_FAR_Quote_NewBusiness_Date_Text As String = "VR_FAR_Quote_NewBusiness_Date"
    Public Const VR_FAR_Quote_Renewal_Date_Text As String = "VR_FAR_Quote_Renewal_Date"

    Public ReadOnly Property ScrollToControlId As String
        Get
            Return Me.ddBuilding.ClientID
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

    Public Property MyBuildingIndex As Int32
        Get
            If ViewState("vs_BuildingIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_BuildingIndex"))
            End If
            Return 0
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

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.HiddenField1, "0")
        Me.VRScript.CreateConfirmDialog(Me.lnkBtnClear.ClientID, "Clear?")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
        Me.VRScript.CreateTextBoxFormatter(Me.txtLimit, ctlPageStartupScript.FormatterType.PositiveWholeNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtYear, ctlPageStartupScript.FormatterType.PositiveNumberNoCommas, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtsqrFeet, ctlPageStartupScript.FormatterType.NumericWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtDwellingContentsLimit, ctlPageStartupScript.FormatterType.PositiveWholeNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup) 'added 10/26/2020 (Interoperability)

        'only show the heated table row when the building is a specific type
        Me.VRScript.AddVariableLine("var buildingTypes_displayHeat = new Array('18','10','11','13','14','15','17','20','29', '37');")

        Dim js_showhide_heated As String = String.Format("if(buildingTypes_displayHeat.contains($('#{1}').val())){{$('#{0}').show();}}else{{$('#{0}').hide(); $('#{2}').prop('checked', false);  }}", Me.trHeated.ClientID, ddBuilding.ClientID, Me.chkHeated.ClientID)
        Me.VRScript.CreateJSBinding(Me.ddBuilding, ctlPageStartupScript.JsEventType.onchange, js_showhide_heated)
        Me.VRScript.AddScriptLine(js_showhide_heated)

        Me.VRScript.CreateJSBinding(Me.ddBuilding, ctlPageStartupScript.JsEventType.onchange, "ChangeBuildingDimensionLabel('" + Me.ddBuilding.ClientID + "','" + Me.lblDimensions.ClientID + "','" + Me.lblSquareFeet.ClientID + "');", True)

        'enable all building types
        'then disable specific types based on building(farmstructuretypeid) and limit
        'Dim js_buildingLimitType As String = "SuggestFarm_BuildingType('" + Me.ddBuilding.ClientID + "','" + Me.txtLimit.ClientID + "','" + Me.ddBuildingType.ClientID + "');"
        'updated 10/26/2020 (Interoperability)
        'Dim js_buildingLimitType As String = "SuggestFarm_BuildingType('" + Me.ddBuilding.ClientID + "','" + Me.txtLimit.ClientID + "','" + Me.ddBuildingType.ClientID + "','" + Me.DwellingContentsRow.ClientID + "','" + Me.ReplacementCostSection.ClientID + "');"

        Dim isFARMBuildingTypeForBuildingsAvailable As Boolean = FarmBuildingtypeforbuildingHelper.IsFARMBuildingTypeForBuildingsAvailable(Quote)

        Dim chc As New CommonHelperClass
        Dim IsVerisk360Enabled = chc.ConfigurationAppSettingValueAsBoolean("VR_AllLines_Site360Valuation_Settings")

        'Break out BuildingType Code and Dwelling/Replacement Cost Code for Interop.  This function calls the proper new code if necessary.  BuildingTypes are not used in Endorsements and are ignored in the script.
        'Dim js_BuildingType_DwellingReplacementDisplay As String = "BuildingType_DwellingDisplay('" + Me.ddBuilding.ClientID + "','" + Me.txtLimit.ClientID + "','" + Me.ddBuildingType.ClientID + "','" + Me.DwellingContentsRow.ClientID + "','" + Me.ReplacementCostSection.ClientID + "', '" + isCoverageEstructureAvailable.ToString() + "' );"
        Dim js_BuildingType_DwellingReplacementDisplay As String = "BuildingType_DwellingDisplay('" + Me.ddBuilding.ClientID + "','" + Me.txtLimit.ClientID + "','" + Me.ddBuildingType.ClientID + "','" + Me.DwellingContentsRow.ClientID + "','" + Me.ReplacementCostSection.ClientID + "', '" + isFARMBuildingTypeForBuildingsAvailable.ToString() + "', '" + IsVerisk360Enabled.ToString() + "' );"

        Me.VRScript.AddScriptLine(js_BuildingType_DwellingReplacementDisplay)
        Me.VRScript.CreateJSBinding(Me.ddBuilding, ctlPageStartupScript.JsEventType.onchange, js_BuildingType_DwellingReplacementDisplay, True) 'uncommented 5/21/18 for Bug 20410 and Bug 20411 MLW
        'Me.VRScript.CreateJSBinding(Me.txtLimit, ctlPageStartupScript.JsEventType.onkeyup, js_buildingLimitType)
        Me.VRScript.CreateJSBinding(Me.txtLimit, ctlPageStartupScript.JsEventType.onblur, js_BuildingType_DwellingReplacementDisplay)
        'Me.VRScript.AddVariableLine(String.Format("var location_{0}_building_{1}_Prop_ddBuilding = '{2}';", Me.MyLocationIndex, MyBuildingIndex, ddBuilding.ClientID))

        If CosmeticDamageExHelper.IsCosmeticDamageExAvailable(Quote) Then
            Dim isCosmeticDamageHidden As Boolean = CosDamHiddenHelper.IsCosmeticDamageHiddenAvailable(Quote)
            Dim isCosmeticDamagePreexistingOnBuilding As Boolean = False
            If IsQuoteEndorsement() AndAlso UCase(hdnHasCosmeticDamagePreexisting.Value) = "TRUE" Then
                isCosmeticDamagePreexistingOnBuilding = True
            End If
            Dim CosScript As String = "HandleCosmeticDamageBuildings('" & Me.ddBuilding.ClientID & "','" & isCosmeticDamageHidden & "','" & isCosmeticDamagePreexistingOnBuilding.ToString() & "','" & hdnHasCosmeticDamagePreexisting.ClientID & "');"
            Me.VRScript.CreateJSBinding(Me.ddBuilding, ctlPageStartupScript.JsEventType.onchange, CosScript)
            Me.VRScript.AddScriptLine(CosScript)
        End If

        ' Only Show Additional Perils and Eq Content (both under Building Coverages) when farmstructuretype id is 17 or 18
        Dim tr_additional As String = String.Format("location_{0}_building_{1}_Covs_TrAdditionalPerils", Me.MyLocationIndex, MyBuildingIndex) ' js variable defined on ctl_FarmBuilding_Coverages
        Dim tr_eqContents As String = String.Format("location_{0}_building_{1}_Covs_TrEqContent", Me.MyLocationIndex, MyBuildingIndex) ' js variable defined on ctl_FarmBuilding_Coverages
        Dim chkadditional As String = String.Format("location_{0}_building_{1}_Covs_chkAdditionalPerils", Me.MyLocationIndex, MyBuildingIndex) ' js variable defined on ctl_FarmBuilding_Coverages
        Dim chkeqContents As String = String.Format("location_{0}_building_{1}_Covs_chkEqContent", Me.MyLocationIndex, MyBuildingIndex) ' js variable defined on ctl_FarmBuilding_Coverages
        Dim txtLossIncomeLabel As String = String.Format("location_{0}_building_{1}_Covs_lblLossIncome", MyLocationIndex, MyBuildingIndex)
        Dim chkLossIncome As String = String.Format("location_{0}_building_{1}_Covs_chkLossIncome", MyLocationIndex, MyBuildingIndex)
        Dim dvLossIncomeLimit As String = String.Format("location_{0}_building_{1}_Covs_dvLossIncomeLimit", MyLocationIndex, MyBuildingIndex)
        Dim dvLossIncomeData As String = String.Format("location_{0}_building_{1}_Covs_dvLossIncomeData", MyLocationIndex, MyBuildingIndex)
        Dim txtLossIncomeLimit As String = String.Format("location_{0}_building_{1}_Covs_txtLossIncomeLimit", MyLocationIndex, MyBuildingIndex)
        Dim chkReplacement As String = String.Format("location_{0}_building_{1}_Covs_chkReplacement", MyLocationIndex, MyBuildingIndex)

        VRScript.AddVariableLine(String.Format("var location_{0}_building_{1}_BuildingType = '{2}';", MyLocationIndex, MyBuildingIndex, ddBuilding.ClientID))

        'WS-3738 - Farmstructure type 37 (Outbuilding with Living Quarters) needs the option for EQ Contents - lSchwieterman - 07/16/2025
        Dim js_show_hide_covs As String = String.Format("if($('#{0}').val() == '17' | $('#{0}').val() == '18') {{$('#' + " + tr_additional + ").show(); $('#' + " + tr_eqContents + ").show(); $('#' + " + tr_additional + ").css('marginLeft', '20px'); $('#' + " + tr_eqContents + ").css('marginLeft', '20px'); $('#' + " + txtLossIncomeLabel + ").text('Loss of Income - Rents');}} else {{$('#' + " + tr_additional + ").hide(); if($('#{0}').val() == '37') {{$('#' + " + tr_eqContents + ").show(); $('#' + " + tr_eqContents + ").css('marginLeft', '20px'); }} else {{$('#' + " + tr_eqContents + ").hide(); $('#' + " + chkeqContents + ").prop('checked', false);}}  $('#' + " + chkadditional + ").prop('checked', false);  $('#' + " + txtLossIncomeLabel + ").text('Loss of Income') }} $('#' + " + chkLossIncome + ").prop('checked', false); $('#' + " + dvLossIncomeData + ").hide(); $('#' + " + dvLossIncomeLimit + ").hide(); $('#' + " + txtLossIncomeLimit + ").val('');", Me.ddBuilding.ClientID)
        If Not IsEndorsementRelated() Then
            Dim js_toggle_cov_Replacement As String = String.Format("if ($('#{0}').val() == '17') {{$('#' + " & chkReplacement & ").prop('checked', false);$('#' + " & chkReplacement & ").attr('disabled', true);}} else {{$('#' + " & chkReplacement & ").removeAttr('disabled');}};", Me.ddBuilding.ClientID)
            Me.VRScript.CreateJSBinding(Me.ddBuilding, ctlPageStartupScript.JsEventType.onchange, js_show_hide_covs & js_toggle_cov_Replacement)
            Me.VRScript.AddScriptLine(js_toggle_cov_Replacement)
        Else
            Me.VRScript.CreateJSBinding(Me.ddBuilding, ctlPageStartupScript.JsEventType.onchange, js_show_hide_covs)
        End If
        'Me.VRScript.CreateJSBinding(Me.ddDeducible, ctlPageStartupScript.JsEventType.onchange, "confirm(Are you sure you want to change Deductible?);")

        'Me.VRScript.LockFormOnEvent(Me.ddDeducible, ctlPageStartupScript.JsEventType.onchange)

        txtLimit.Attributes.Add("onfocus", "this.select()")
        txtYear.Attributes.Add("onfocus", "this.select()")
        txtYear.Attributes.Add("onfocus", "this.select()")
        txtDimensions.Attributes.Add("onfocus", "this.select()")
        txtDescription.Attributes.Add("onfocus", "this.select()")
        txtDwellingContentsLimit.Attributes.Add("onfocus", "this.select()") 'added 10/26/2020 (Interoperability)
    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddBuilding.Items.Count <= 0 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddBuilding, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.FarmStructureTypeId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddBuildingType, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.FarmTypeId, SortBy.None, Me.Quote.LobType)


            If Not OutbuildingLivingQuartersHelper.isOutbuildingLivingQuartersAvailable(Quote) Then
                Dim itemToRemove As ListItem = Me.ddBuilding.Items.FindByValue("37")
                Me.ddBuilding.Items.Remove(itemToRemove)
            End If

            ''handle Building Structure type "Outbuilding with Living Quarters" based on date
            'Dim newBusinessDateString As String = QQHelper.configAppSettingValueAsString(VR_FAR_Quote_NewBusiness_Date_Text)
            'Dim renewalDateString As String = QQHelper.configAppSettingValueAsString(VR_FAR_Quote_Renewal_Date_Text)

            'If Me.ddBuilding.Items.Count > 0 Then
            '    Dim itemToRemove As ListItem = Me.ddBuilding.Items.FindByValue("37")
            '    If itemToRemove IsNot Nothing AndAlso
            '    Not ((Me.Quote.TransactionTypeId = "2" AndAlso Helpers.EffectiveDateHelper.isQuoteEffectiveDatePastDate(Quote.TransactionEffectiveDate, newBusinessDateString) = True) OrElse
            '     (Me.Quote.TransactionTypeId = "3" AndAlso Helpers.EffectiveDateHelper.isQuoteEffectiveDatePastDate(Quote.TransactionEffectiveDate, renewalDateString) = True)) Then
            '        Me.ddBuilding.Items.Remove(itemToRemove)
            '    End If
            'End If

        End If
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddDeducible, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.E_Farm_DeductibleLimitId, SortBy.None, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If Me.MyBuilding IsNot Nothing Then
            If String.IsNullOrWhiteSpace(Me.MyBuilding.FarmStructureTypeId) = False Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddBuilding, Me.MyBuilding.FarmStructureTypeId)
            End If
            If String.IsNullOrWhiteSpace(Me.MyBuilding.ConstructionId) = False Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddConstruction, Me.MyBuilding.ConstructionId)
            End If
            Me.chkHeated.Checked = Me.MyBuilding.HeatedBuildingSurchargeOther
            Me.chkHay.Checked = Me.MyBuilding.Description.StartsWith(HayStorageText)

            'Me.txtLimit.Text = Me.MyBuilding.E_Farm_Limit

            If Me.MyBuilding.PropertyValuation IsNot Nothing Then
                If Me.MyBuilding.PropertyValuation.Response IsNot Nothing Then
                    If String.IsNullOrWhiteSpace(Me.MyBuilding.PropertyValuation.Response.ReplacementCostValue) = False Then
                        If String.IsNullOrWhiteSpace(Me.MyBuilding.E_Farm_Limit) OrElse Me.MyBuilding.E_Farm_Limit.Trim() = "0" Then
                            Me.txtLimit.Text = If(Me.MyBuilding.PropertyValuation.Response.ReplacementCostValue.Replace("$", "") = "0.00", "0", MyBuilding.PropertyValuation.Response.ReplacementCostValue.Replace("$", ""))
                        Else
                            Me.txtLimit.Text = Me.MyBuilding.E_Farm_Limit
                        End If
                    Else
                        Me.txtLimit.Text = Me.MyBuilding.E_Farm_Limit
                    End If
                End If
            Else
                Me.txtLimit.Text = Me.MyBuilding.E_Farm_Limit
            End If

            'added 10/26/2020 (Interoperability)
            Me.txtDwellingContentsLimit.Text = MyBuilding.HouseholdContentsLimit
            IFM.VR.Web.Helpers.WebHelper_Personal.RemoveStyleFromHtmlControl(Me.DwellingContentsRow, "display")
            IFM.VR.Web.Helpers.WebHelper_Personal.RemoveStyleFromGenericControl(Me.ReplacementCostSection, "top")
            If QQHelper.IntegerForString(MyBuilding.FarmStructureTypeId) = 17 OrElse QQHelper.IntegerForString(MyBuilding.FarmStructureTypeId) = 18 OrElse QQHelper.IntegerForString(MyBuilding.FarmStructureTypeId) = 37 Then 'Farm Dwelling Contents
                IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToHtmlControl(Me.DwellingContentsRow, "display", "table-row")
                IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(Me.ReplacementCostSection, "top", "-260px")
            Else
                IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToHtmlControl(Me.DwellingContentsRow, "display", "none")
                IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(Me.ReplacementCostSection, "top", "-250px")
            End If

            ' Endorsement check removed per task 77651 BD
            'If IsQuoteEndorsement() = False Then
            Dim FirstBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding = IFM.VR.Common.Helpers.BuildingsHelper.FindFirstBuilding(Me.Quote)
            Me.ddDeducible.Enabled = MyBuilding.Equals(FirstBuilding) ' only enabled on first Building
            If ddDeducible.Enabled = False Then
                ddDeducible.ToolTip = "Deductible can only be changed on the first building on the quote."
            Else
                ddDeducible.ToolTip = ""
            End If
            If String.IsNullOrWhiteSpace(If(FirstBuilding IsNot Nothing, FirstBuilding.E_Farm_DeductibleLimitId, "")) = False Then
                'Me.ddDeducible.SetFromValue(FirstBuilding.E_Farm_DeductibleLimitId) 'always the value in loc1 building 1

                IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddDeducible, FirstBuilding.E_Farm_DeductibleLimitId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.E_Farm_DeductibleLimitId)

                'If QQHelper.IsPositiveIntegerString(FirstBuilding.E_Farm_DeductibleLimitId) AndAlso ddDeducible.Items.FindByValue(FirstBuilding.E_Farm_DeductibleLimitId) Is Nothing Then
                '    Dim TypeDescription As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.E_Farm_DeductibleLimitId, FirstBuilding.E_Farm_DeductibleLimitId)
                '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(Me.ddDeducible, FirstBuilding.E_Farm_DeductibleLimitId, TypeDescription)
                'Else
                '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddDeducible, FirstBuilding.E_Farm_DeductibleLimitId)
                'End If
            End If
            'Else
            '    ' Except on Endorsements where all can be different.  CAH Task 59589
            '    Me.ddDeducible.Enabled = True
            '    IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddDeducible, MyBuilding.E_Farm_DeductibleLimitId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.E_Farm_DeductibleLimitId)
            'End If
            If IsQuoteEndorsement() AndAlso MyBuilding.Equals(FirstBuilding) Then
                Me.trFarmCoverageEInfoMsg.Visible = True
            Else
                Me.trFarmCoverageEInfoMsg.Visible = False
            End If
            Me.txtYear.Text = MyBuilding.YearBuilt
            Me.txtsqrFeet.Text = If(MyBuilding.SquareFeet.Trim() <> "0", MyBuilding.SquareFeet, "")
            Me.txtDimensions.Text = MyBuilding.Dimensions
            If String.IsNullOrWhiteSpace(MyBuilding.FarmTypeId) = False Then
                Me.ddBuildingType.SetFromValue(Me.MyBuilding.FarmTypeId)
            End If
            Me.txtDescription.Text = Me.MyBuilding.Description.Replace(HayStorageText, "")

            If IsQuoteEndorsement() AndAlso CosDamHiddenHelper.IsCosmeticDamageHiddenAvailable(Quote) AndAlso MyBuilding.FarmStructureTypeId.EqualsAny("17", "18", "26", "27", "28", "33", "34", "35", "36", "37") Then
                Dim cosmeticDamageCov = (From cov In MyBuilding.OptionalCoverageEs Where cov.CoverageType = QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Cosmetic_Damage_Exclusion_Coverage_E Select cov).FirstOrDefault()
                If cosmeticDamageCov IsNot Nothing Then
                    hdnHasCosmeticDamagePreexisting.Value = "True"
                Else
                    hdnHasCosmeticDamagePreexisting.Value = "False"
                End If
            Else
                hdnHasCosmeticDamagePreexisting.Value = "False"
            End If
        End If

        If IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
            lblDescriptionRequired.Visible = True
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divMain.ClientID
        Me.lnkBuildingTypeHelp.NavigateUrl = ConfigurationManager.AppSettings("VRHelpDoc_BuildingType_HelpText")

        'ddBuilding.ID = String.Format("location_{0}_building_{1}", MyLocationIndex, MyBuildingIndex)
        'ddBuilding.ClientIDMode = UI.ClientIDMode.Static
    End Sub

    Public Overrides Function Save() As Boolean
        Me.ddBuildingType.Enabled = True
        If Me.MyBuilding IsNot Nothing Then
            'ddBuilding.ID = String.Format("location_{0}_building_{1}", MyLocationIndex, MyBuildingIndex)
            Me.MyBuilding.FarmStructureTypeId = Me.ddBuilding.SelectedValue
            Me.MyBuilding.ConstructionId = Me.ddConstruction.SelectedValue
            Me.MyBuilding.HeatedBuildingSurchargeOther = Me.chkHeated.Checked
            Me.MyBuilding.E_Farm_Limit = Me.txtLimit.Text.Trim()

            'added 10/26/2020 (Interoperability)
            ' commented out per bug WS-2947
            'If QQHelper.IntegerForString(MyBuilding.FarmStructureTypeId) = 17 OrElse QQHelper.IntegerForString(MyBuilding.FarmStructureTypeId) = 18 OrElse QQHelper.IntegerForString(MyBuilding.FarmStructureTypeId) = 37 AndAlso QQHelper.IsPositiveDecimalString(Me.txtDwellingContentsLimit.Text.Trim()) = True Then 'only valid for Farm Dwelling
            Me.MyBuilding.HouseholdContentsLimit = Me.txtDwellingContentsLimit.Text.Trim()
            'Else
            '    MyBuilding.HouseholdContentsLimit = ""
            'End If

            'Changed for bug 6483
            ' Endorsement check removed per task 77651 BD
            'If IsQuoteEndorsement() = False Then
            Dim FirstBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding = IFM.VR.Common.Helpers.BuildingsHelper.FindFirstBuilding(Me.Quote)
            If FirstBuilding IsNot Nothing Then ' should not be possible to be Nothing
                If MyBuilding.Equals(FirstBuilding) Then
                    MyBuilding.E_Farm_DeductibleLimitId = ddDeducible.SelectedValue ' this is the first building so set it's deductible
                Else
                    MyBuilding.E_Farm_DeductibleLimitId = FirstBuilding.E_Farm_DeductibleLimitId ' copy whatever the first building's deductible is
                End If
                'If MyBuildingIndex = 0 Then
                '    MyBuilding.E_Farm_DeductibleLimitId = ddDeducible.SelectedValue
                'Else
                '    MyBuilding.E_Farm_DeductibleLimitId = MyLocation.Buildings(0).E_Farm_DeductibleLimitId
                'End If
            End If
            'Else
            '    ' Except on Endorsements where all can be different.  CAH Task 59589
            '    MyBuilding.E_Farm_DeductibleLimitId = ddDeducible.SelectedValue
            'End If



            MyBuilding.YearBuilt = Me.txtYear.Text.Trim()
            MyBuilding.SquareFeet = Me.txtsqrFeet.Text.TryToGetInt32().ToString()
            MyBuilding.Dimensions = Me.txtDimensions.Text.Trim()
            MyBuilding.Description = Me.txtDescription.Text.Trim()
            If Me.chkHay.Checked Then ' you use this specific string to indicate if this was checked
                If Me.MyBuilding.Description.ToUpper().Contains(HayStorageText) = False Then
                    Me.MyBuilding.Description = HayStorageText + Me.MyBuilding.Description
                End If
            Else
                Me.MyBuilding.Description = Me.MyBuilding.Description.ToUpper().Replace(HayStorageText, "")
            End If

            Me.MyBuilding.FarmTypeId = Me.ddBuildingType.SelectedValue

        End If
        Populate()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = String.Format("Location #{0} Building #{1} - Property", MyLocationIndex + 1, MyBuildingIndex + 1)

        ' logic when this validation runs after a rate but the control never saves so the default 0 doesn't go back to zero after a rate
        If MyBuilding IsNot Nothing Then
            If MyBuilding.SquareFeet = "0" And Me.txtsqrFeet.Text <> "0" Then 'fixes issue with blank becoming zero after a rate Matt A 8-25-2015
                MyBuilding.SquareFeet = ""
            End If
        End If

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Dim valList = FarmBuildingValidator.ValidateFARBuildingProperty(Me.Quote, Me.MyLocationIndex, Me.MyBuildingIndex, valArgs.ValidationType)
        If valList.Any() Then
            For Each v In valList
                Select Case v.FieldId
                    Case FarmBuildingValidator.buildingType
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddBuildingType, v, accordList)
                    Case FarmBuildingValidator.construction
                        If Not IsQuoteEndorsement() Then 'CAH 10/8/2020 Task52116
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddConstruction, v, accordList)
                        End If
                    Case FarmBuildingValidator.deductible
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddDeducible, v, accordList)
                    Case FarmBuildingValidator.dimensions
                        If Not IsQuoteEndorsement() Then 'CAH 10/8/2020 Task52116
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDimensions, v, accordList)
                        End If

                    Case FarmBuildingValidator.farmStructureId
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddBuilding, v, accordList)
                    Case FarmBuildingValidator.heatIsNotSupportedOnThisFarmStructureType
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.chkHeated, v, accordList)
                    Case FarmBuildingValidator.limit_val
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLimit, v, accordList)
                    Case FarmBuildingValidator.dwellingContentsLimit_val 'added 10/26/2020 (Interoperability)
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDwellingContentsLimit, v, accordList)
                    Case FarmBuildingValidator.squareFeet
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtsqrFeet, v, accordList)
                    Case FarmBuildingValidator.yearConstructed
                        If Not IsQuoteEndorsement() Then 'CAH 10/8/2020 Task52116
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtYear, v, accordList)
                        End If
                    Case FarmBuildingValidator.description
                        If IsQuoteEndorsement() Then
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescription, v, accordList)
                        End If
                End Select
            Next
        End If

    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Session("valuationValue") = "False"
        Me.Save_FireSaveEvent()
    End Sub

    Protected Sub btnReplacementCC_Click(sender As Object, e As EventArgs) Handles btnReplacementCC.Click
        Session("valuationValue") = "False"
        Me.Save_FireSaveEvent(False)
        'Dim valList = FarmBuildingValidator.ValidateFARBuildingProperty(Me.Quote, Me.MyLocationIndex, Me.MyBuildingIndex, Validation.ObjectValidation.ValidationItem.ValidationType.quote)
        'If valList.ListHasValidationId(FarmBuildingValidator.farmStructureId) Then
        '    ' don't goto e2Value until you have a valid farmstructuretypeid
        'Else
        '    'go to e2value

        'End If
        Dim pvHelper As New QuickQuotePropertyValuationHelperClass
        Dim errMsg As String = ""
        Dim wasSaveSuccessful As Boolean = False
        Dim valuationUrl As String = ""
        'pvHelper.PopulateE2ValuePropertyValuationFromQuoteAndSetUrl(Quote, e2ValueUrl, 1, True, wasSaveSuccessful, errMsg)
        Dim propertyType As QuickQuotePropertyValuationHelperClass.ValuationPropertyType = QuickQuotePropertyValuationHelperClass.ValuationPropertyType.DefaultByInfo

        Dim chc As New CommonHelperClass
        If chc.ConfigurationAppSettingValueAsBoolean("VR_AllLines_Site360Valuation_Settings") = True Then
            pvHelper.PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(Me.Quote, valuationUrl, Me.MyLocationIndex + 1, MyBuildingIndex + 1, True, wasSaveSuccessful, errMsg, propertyType, valuationVendor:=QuickQuotePropertyValuation.ValuationVendor.Verisk360)
        Else
            pvHelper.PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(Me.Quote, valuationUrl, Me.MyLocationIndex + 1, MyBuildingIndex + 1, True, wasSaveSuccessful, errMsg, propertyType, valuationVendor:=QuickQuotePropertyValuation.ValuationVendor.e2Value)

        End If

        'pvHelper.PopulateVendorPropertyValuationFromQuoteAndSetUrl(Me.Quote, e2ValueUrl, Me.MyLocationIndex + 1, MyBuildingIndex + 1, True, wasSaveSuccessful, errMsg, propertyType, QuickQuotePropertyValuation.ValuationVendor.e2Value)
        If valuationUrl <> "" Then

            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
            Else
                VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId) ' Matt A - 12-9-14
            End If

            Response.Redirect(valuationUrl, False)
        Else
            Me.ValidationHelper.AddError("Problem Initiating Valuation Request")
        End If
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        Me.txtDescription.Text = ""
        Me.txtDimensions.Text = ""
        Me.txtLimit.Text = ""
        Me.txtDwellingContentsLimit.Text = "" 'added 10/26/2020 (Interoperability)
        Me.txtsqrFeet.Text = ""
        Me.txtYear.Text = ""
        Me.ddBuilding.SelectedIndex = -1
        Me.ddBuildingType.SelectedIndex = -1
        Me.ddConstruction.SelectedIndex = -1
        Me.ddDeducible.SelectedIndex = -1
        Me.chkHay.Checked = False
        Me.chkHeated.Checked = False

        Me.Save_FireSaveEvent(False)
    End Sub

    Protected Sub lnkBtnClear_Click(sender As Object, e As EventArgs) Handles lnkBtnClear.Click
        Session("valuationValue") = "False"
        ClearControl()
    End Sub

    'Protected Sub ddDeducible_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddDeducible.SelectedIndexChanged
    '    Me.Save_FireSaveEvent(False)
    '    Me.Populate_FirePopulateEvent() ' refresh whole page
    '    Me.VRScript.AddScriptLine("$('#" + ddDeducible.ClientID + "').focus();", True)
    'End Sub

End Class