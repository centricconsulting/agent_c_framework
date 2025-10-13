Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.FarmLines
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Common.Helpers.MultiState.General
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.FARM

Public Class ctlResidenceCoverages
    Inherits VRControlBase

    Public Event GetYearBult()
    Public Event GetDwellingClassification()

    Public Event GetParentControlIds() 'aded 3/3/2021

    Dim pvHelper As New QuickQuotePropertyValuationHelperClass
    Private Const FO2ANDFO3DefaultPrivatePowerPolesIncludedLimit = "1,500"
    Private Const FO5DefaultPrivatePowerPolesIncludedLimit = "2,500"


    Public Property ResidenceCoverageLiabAccordionDivId As String
        Get
            If ViewState("vs_DwellingCoverageAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_DwellingCoverageAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_DwellingCoverageAccordionDivId_") = value
        End Set
    End Property

    Public Property ResidenceCoveragePropAccordionDivId As String
        Get
            If ViewState("vs_ResidenceCoveragePropAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_ResidenceCoveragePropAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_ResidenceCoveragePropAccordionDivId_") = value
        End Set
    End Property

    Public Property ResidenceCoverageAccordionDivId As String
        Get
            If ViewState("vs_ResidenceCoverageAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_ResidenceCoverageAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_ResidenceCoverageAccordionDivId_") = value
        End Set
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

    Public Property SetHiddenCovALimit() As String
        Get
            Return hiddenCovALimit.Value
        End Get
        Set(ByVal value As String)
            hiddenCovALimit.Value = value
        End Set
    End Property

    Public Property SetHiddenCovATotal() As String
        Get
            Return hiddenCovATotal.Value
        End Get
        Set(ByVal value As String)
            hiddenCovATotal.Value = value
        End Set
    End Property

    Public Property SetReplacementCostCovC() As Boolean
        Get
            Return chkReplaceCost.Checked
        End Get
        Set(ByVal value As Boolean)
            chkReplaceCost.Checked = value
        End Set
    End Property

    Private _deleteResidence As Boolean
    Public Property DeleteResidence() As Boolean
        Get
            Return _deleteResidence
        End Get
        Set(ByVal value As Boolean)
            _deleteResidence = value
        End Set
    End Property

    Public ReadOnly Property MyFarmLocation As List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ProgramType() As String
        Get
            If MyFarmLocation IsNot Nothing Then
                hiddenProgramType.Value = MyFarmLocation(MyLocationIndex).ProgramTypeId
                Return MyFarmLocation(MyLocationIndex).ProgramTypeId
            Else
                hiddenProgramType.Value = "6"
                Return "6"
            End If
        End Get
    End Property

    'Public Property PrimaryLocDeduct() As String
    '    Get
    '        Return Session("sess_PrimLocDeduct")
    '    End Get
    '    Set(ByVal value As String)
    '        Session("sess_PrimLocDeduct") = value
    '    End Set
    'End Property

    Public Property ResidenceExists() As Boolean
        Get
            Return ViewState("vs_ResidenceExists")
        End Get
        Set(ByVal value As Boolean)
            ViewState("vs_ResidenceExists") = value
        End Set
    End Property

    Public ReadOnly Property dvReplacementCC_ClientId() As String
        Get
            Return dvReplacementCC.ClientID
        End Get
    End Property

    Public ReadOnly Property txtPrivatePowerPolesIncludedLimit_ClientId() As String
        Get
            Return txtPrivatePowerPolesIncludedLimit.ClientID
        End Get
    End Property

    Public ReadOnly Property txtPrivatePowerPolesIncreaseLimit_ClientId() As String
        Get
            Return txtPrivatePowerPolesIncreaseLimit.ClientID
        End Get
    End Property

    Public ReadOnly Property txtPrivatePowerPolesTotalLimit_ClientId() As String
        Get
            Return txtPrivatePowerPolesTotalLimit.ClientID
        End Get
    End Property


    Public ReadOnly Property chkPrivatePowerPoles_ClientId() As String
        Get
            Return chkPrivatePowerPoles.ClientID
        End Get
    End Property

    Public ReadOnly Property dvPrivatePowerPoles_ClientId() As String
        Get
            Return dvPrivatePowerPoles.ClientID
        End Get
    End Property

    Public ReadOnly Property tblPrivatePowerPoles_ClientId() As String
        Get
            Return tblPrivatePowerPoles.ClientID
        End Get
    End Property

    ''Removed 10/11/18 for multi state MLW
    'Public ReadOnly Property MineSubCounties(stateId As String) As List(Of String)
    '    Get
    '        Dim msCounties As List(Of String) = Nothing
    '        'Select Case (MyFarmLocation(MyLocationIndex).Address.StateId)
    '        Select Case (stateId)
    '            Case States.Abbreviations.IL
    '                msCounties = New List(Of String)(New String() _
    '                                    {"BOND", "BUREAU", "CHRISTIAN", "CLINTON", "DOUGLAS", "FRANKLIN", "FULTON", "GALLATIN", "GRUNDY", "JACKSON", "JEFFERSON", "KNOX", "LASALLE",
    '                                     "LOGAN", "MCDONOUGH", "MACOUPIN", "MADISON", "MARION", "MARSHALL", "MENARD", "MERCER", "MONTGOMERY", "PEORIA", "PERRY", "PUTNAM",
    '                                     "RANDOLPH", "ROCK ISLAND", "ST. CLAIR", "SALINE", "SANGAMON", "TAZEWELL", "VERMILION", "WASHINGTON", "WILLIAMSON"})
    '            Case Else
    '                msCounties = New List(Of String)(New String() _
    '                                    {"CLAY", "CRAWFORD", "DAVIESS", "DUBOIS", "FOUNTAIN", "GIBSON", "GREENE", "KNOX", "LAWRENCE", "MARTIN", "MONROE", "MONTGOMERY", "ORANGE", "OWEN",
    '                                     "PARKE", "PERRY", "PIKE", "POSEY", "PUTNAM", "SPENCER", "SULLIVAN", "VANDERBURGH", "VERMILLION", "VIGO", "WARREN", "WARRICK"})
    '        End Select

    '        Return msCounties
    '    End Get
    'End Property
    ''Public ReadOnly Property MineSubCounties() As List(Of String)
    ''    Get
    ''        Dim msCounties As List(Of String) = New List(Of String)(New String() _
    ''                                    {"CLAY", "CRAWFORD", "DAVIESS", "DUBOIS", "FOUNTAIN", "GIBSON", "GREENE", "KNOX", "LAWRENCE", "MARTIN", "MONROE", "MONTGOMERY", "ORANGE", "OWEN",
    ''                                     "PARKE", "PERRY", "PIKE", "POSEY", "PUTNAM", "SPENCER", "SULLIVAN", "VANDERBURGH", "VERMILLION", "VIGO", "WARREN", "WARRICK"})
    ''        Return msCounties
    ''    End Get
    ''End Property

    Private _YearBuilt As String
    Public Property YearBuilt() As String
        Get
            Return _YearBuilt
        End Get
        Set(ByVal value As String)
            _YearBuilt = value
        End Set
    End Property

    Private _DwellingClassification As String
    Public Property DwellingClassification() As String
        Get
            Return _DwellingClassification
        End Get
        Set(ByVal value As String)
            _DwellingClassification = value
        End Set
    End Property

    'Removed 12/3/18 for new jQuery mine sub code MLW
    ''Added 10/25/18 for multi state MLW
    'Private Property MineSubsidenceCheckboxIsEnabled As Boolean
    '    Get
    '        If ViewState("vs_MineSubChecked") IsNot Nothing Then
    '            Return ViewState("vs_MineSubChecked")
    '        Else
    '            Return True
    '        End If
    '    End Get
    '    Set(value As Boolean)
    '        ViewState("vs_MineSubChecked") = value
    '    End Set
    'End Property

    'added 3/3/2021 so we can stop using a static ids for ctlResidence.txtYearBuilt and ctlResidence.dd_Residence_CoverageForm
    Public Property YearBuiltTextboxClientId As String
        Get
            Dim yrBuiltClientId As String = Me.hdnYearBuiltTextboxClientId.Value
            If String.IsNullOrWhiteSpace(yrBuiltClientId) = True Then
                RaiseEvent GetParentControlIds()
                yrBuiltClientId = Me.hdnYearBuiltTextboxClientId.Value
            End If
            Return yrBuiltClientId
        End Get
        Set(value As String)
            Me.hdnYearBuiltTextboxClientId.Value = value
        End Set
    End Property
    Public Property CoverageFormDropdownClientId As String
        Get
            Dim covFormClientId As String = Me.hdnCoverageFormDropdownClientId.Value
            If String.IsNullOrWhiteSpace(covFormClientId) = True Then
                RaiseEvent GetParentControlIds()
                covFormClientId = Me.hdnCoverageFormDropdownClientId.Value
            End If
            Return covFormClientId
        End Get
        Set(value As String)
            Me.hdnCoverageFormDropdownClientId.Value = value
        End Set
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ResidenceCoverageAccordionDivId = dvResidenceCoverage.ClientID
        ResidenceCoverageLiabAccordionDivId = dvResidenceCovLiab.ClientID
        ResidenceCoveragePropAccordionDivId = dvResidenceCovProp.ClientID

        If Not IsPostBack Then
            LoadStaticData()
            Populate()
            If ProgramType = "6" Then
                DisplayCoverages(True)
            Else
                DisplayCoverages(False)
            End If
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.CreateAccordion(ResidenceCoverageLiabAccordionDivId, hiddenResCovLiab, "0", False)
        VRScript.CreateAccordion(ResidenceCoveragePropAccordionDivId, hiddenResCovProp, "0", False)
        VRScript.CreateAccordion(ResidenceCoverageAccordionDivId, hiddenResCov, "0", False)
        VRScript.CreateConfirmDialog(lnkDCClear.ClientID, "Clear Dwelling Liability Coverages?")
        VRScript.StopEventPropagation(lnkDCSave.ClientID, True)
        VRScript.CreateConfirmDialog(lnkRCPClear.ClientID, "Clear Dwelling Property Coverages?")
        VRScript.StopEventPropagation(lnkRCPSave.ClientID, True)
        VRScript.CreateConfirmDialog(lnkRCClear.ClientID, "Clear Dwelling Coverage?")
        VRScript.StopEventPropagation(lnkRCSave.ClientID, True)

        ' Add a state script variable so we can process OH mine sub correctly
        Dim st As String = QuickQuoteHelperClass.StateAbbreviationForDiamondStateId(MyFarmLocation(MyLocationIndex).Address.StateId)
        VRScript.AddVariableLine("var FAR_MineSubState = '" & st & "';")

        'Removed 12/3/18 for new jQuery changes for mine sub MLW
        ''Added 11/2/18 for multi state MLW
        'Dim hiddenMineSubIsChecked As Boolean = False
        'Dim hiddenMineSubIsEnabled As Boolean = True
        'If MultiState.General.IsMultistateCapableEffectiveDate(Me.Quote.EffectiveDate) Then
        '    Select Case (MyFarmLocation(MyLocationIndex).Address.StateId)
        '        Case States.Abbreviations.IL
        '            If MineSubsidenceHelper.MineSubCountiesByStateAbbreviation(MyFarmLocation(MyLocationIndex).Address.State).Contains(MyFarmLocation(MyLocationIndex).Address.County.Trim.ToUpper()) Then
        '                hiddenMineSubIsChecked = True
        '                hiddenMineSubIsEnabled = False
        '            Else
        '                Dim indexForFirstMineSubCheckboxEnabled As String = FindIndexForFirstMineSubCheckboxEnabled(Quote)
        '                If indexForFirstMineSubCheckboxEnabled = -1 Then
        '                    hiddenMineSubIsChecked = False
        '                Else
        '                    hiddenMineSubIsChecked = True
        '                    If MyLocationIndex = indexForFirstMineSubCheckboxEnabled Then
        '                        hiddenMineSubIsEnabled = True
        '                    Else
        '                        hiddenMineSubIsEnabled = False
        '                    End If
        '                End If
        '            End If
        '    End Select
        'End If
        'hiddenMineSubDwellingChecked.Value = hiddenMineSubIsChecked
        'hiddenMineSubDwellingEnabled.Value = hiddenMineSubIsEnabled

        ''Updated 10/17/18 for multi state MLW - added dvMineSubsidence.ClientID, chkMineSubsidence.ClientID, dvMineSubsidenceNotReqHelpInfo.ClientID, ddState and txtCounty
        'Dim scriptToggleCoverageLimit As String = "ToggleCoverageByForm(this, """ + txtDwLimit.ClientID + """,""" + txtDwellingChangeInLimit.ClientID +
        '    """,""" + txtDwellingTotalLimit.ClientID + """, """ + txtRPSLimit.ClientID + """, """ + txtRPSChgInLimit.ClientID +
        '    """, """ + txtRPSTotalLimit.ClientID + """, """ + txtPPLimit.ClientID + """, """ + txtPPChgInLimit.ClientID +
        '    """, """ + txtPPTotalLimit.ClientID + """, """ + txtLossLimit.ClientID + """, """ + txtLossChgInLimit.ClientID +
        '    """, """ + txtLossTotalLimit.ClientID + """, """ + chkReplaceCost.ClientID + """, """ + hiddenCovALimit.ClientID +
        '    """, """ + hiddenCovATotal.ClientID + """, """ + hiddenCovBLimit.ClientID + """, """ + hiddenCovBChange.ClientID +
        '    """, """ + hiddenCovBTotal.ClientID + """, """ + hiddenCovCLimit.ClientID + """, """ + hiddenCovCChange.ClientID +
        '    """, """ + hiddenCovCTotal.ClientID + """, """ + hiddenCovDLimit.ClientID + """, """ + hiddenCovDChange.ClientID +
        '    """, """ + hiddenCovDTotal.ClientID + """, """ + dvCovA.ClientID + """, """ + dvCovB.ClientID + """, """ + lblPPReq.ClientID +
        '    """, """ + hiddenProgramType.ClientID + """, """ + hiddenFormType.ClientID + """, """ + dvExpandReplacement.ClientID +
        '    """, """ + chkExpandReplacement.ClientID + """, """ + hiddenBuiltYear.ClientID + """, """ + chkACV.ClientID + """, """ + dvReplacementCC.ClientID +
        '    """, """ + dvReplaceCost.ClientID + """, """ + dvReplaceCostIncl.ClientID + """, """ + dvReplacement.ClientID + """, """ + chkReplacement.ClientID +
        '    """, """ + dvMineSubsidence.ClientID + """, """ + chkMineSubsidence.ClientID + """, """ + dvMineSubsidenceReqHelpInfo.ClientID +
        '    """, """ + dvMineSubsidenceNotReqHelpInfo.ClientID + """,$('#' + " + String.Format("location_{0}_ddstateId", Me.MyLocationIndex.ToString()) + ").val().toUpperCase(), 
        '    $('#' + " + String.Format("location_{0}_txtcountyId", Me.MyLocationIndex.ToString()) + ").val().toUpperCase(), """ + hiddenMineSubDwellingChecked.ClientID +
        '    """, """ + hiddenMineSubDwellingEnabled.ClientID + """);"
        Dim ddlStructureList As DropDownList = Me.Parent.FindControl("ddlStructure")

        Dim chc As New CommonHelperClass
        Dim isVerisk360Enabled As String = chc.ConfigurationAppSettingValueAsBoolean("VR_AllLines_Site360Valuation_Settings")

        Dim scriptToggleCoverageLimit As String = "ToggleCoverageByForm(this, """ + txtDwLimit.ClientID + """,""" + txtDwellingChangeInLimit.ClientID +
            """,""" + txtDwellingTotalLimit.ClientID + """, """ + txtRPSLimit.ClientID + """, """ + txtRPSChgInLimit.ClientID +
            """, """ + txtRPSTotalLimit.ClientID + """, """ + txtPPLimit.ClientID + """, """ + txtPPChgInLimit.ClientID +
            """, """ + txtPPTotalLimit.ClientID + """, """ + txtLossLimit.ClientID + """, """ + txtLossChgInLimit.ClientID +
            """, """ + txtLossTotalLimit.ClientID + """, """ + chkReplaceCost.ClientID + """, """ + hiddenCovALimit.ClientID +
            """, """ + hiddenCovATotal.ClientID + """, """ + hiddenCovBLimit.ClientID + """, """ + hiddenCovBChange.ClientID +
            """, """ + hiddenCovBTotal.ClientID + """, """ + hiddenCovCLimit.ClientID + """, """ + hiddenCovCChange.ClientID +
            """, """ + hiddenCovCTotal.ClientID + """, """ + hiddenCovDLimit.ClientID + """, """ + hiddenCovDChange.ClientID +
            """, """ + hiddenCovDTotal.ClientID + """, """ + dvCovA.ClientID + """, """ + dvCovB.ClientID + """, """ + lblPPReq.ClientID +
            """, """ + hiddenProgramType.ClientID + """, """ + hiddenFormType.ClientID + """, """ + dvExpandReplacement.ClientID +
            """, """ + chkExpandReplacement.ClientID + """, """ + hiddenBuiltYear.ClientID + """, """ + chkACV.ClientID + """, """ + dvReplacementCC.ClientID +
            """, """ + dvReplaceCost.ClientID + """, """ + dvReplaceCostIncl.ClientID + """, """ + dvReplacement.ClientID + """, """ + chkReplacement.ClientID + """, """ + chkRPS.ClientID + """, """ + ddlStructureList.ClientID + """, """ + isVerisk360Enabled + """);"
        'VRScript.CreateJSBinding("dd_Residence_CoverageForm" + MyLocationIndex.ToString(), ctlPageStartupScript.JsEventType.onchange, scriptToggleCoverageLimit)
        'updated 3/3/2021 to use new property to get clientId for CoverageForm dropdown
        VRScript.CreateJSBinding(Me.CoverageFormDropdownClientId, ctlPageStartupScript.JsEventType.onchange, scriptToggleCoverageLimit)

        'Removed 12/3/18 for new jQuery mine sub code MLW
        ''Added 11/2/18 for multi state MLW
        'Dim scriptToggleMineSubsidence As String = "ToggleMineSubsidenceDwelling($('#' + " + String.Format("location_{0}_ddstateId", Me.MyLocationIndex.ToString()) + ").val().toUpperCase(), 
        '    $('#' + " + String.Format("location_{0}_txtcountyId", Me.MyLocationIndex.ToString()) + ").val().toUpperCase(), """ + hiddenFormType.ClientID + """, """ + dvMineSubsidence.ClientID +
        '    """, """ + chkMineSubsidence.ClientID + """, """ + dvMineSubsidenceReqHelpInfo.ClientID + """, """ + dvMineSubsidenceNotReqHelpInfo.ClientID +
        '    """, """ + hiddenMineSubDwellingChecked.ClientID + """, """ + hiddenMineSubDwellingEnabled.ClientID + """);"
        'Me.VRScript.CreateJSBinding_CustomSelector(String.Format("'#' + location_{0}_txtzipId", Me.MyLocationIndex.ToString()), ctlPageStartupScript.JsEventType.onblur, scriptToggleMineSubsidence)
        'Me.VRScript.CreateJSBinding_CustomSelector(String.Format("'#' + location_{0}_txtcountyId", Me.MyLocationIndex.ToString()), ctlPageStartupScript.JsEventType.onblur, scriptToggleMineSubsidence, True)
        'Me.VRScript.CreateJSBinding_CustomSelector(String.Format("'#' + location_{0}_ddstateId", Me.MyLocationIndex.ToString()), ctlPageStartupScript.JsEventType.onchange, scriptToggleMineSubsidence)

        ' Calculate coverage from Dwelling A Limit
        Dim scriptCalcDwellingLimit As String = "CalculateFromDwellingLimit(""" + txtDwLimit.ClientID + """, """ + txtDwellingTotalLimit.ClientID +
            """, """ + txtRPSLimit.ClientID + """, """ + txtRPSChgInLimit.ClientID + """, """ + txtRPSTotalLimit.ClientID +
            """, """ + txtPPLimit.ClientID + """, """ + txtPPChgInLimit.ClientID + """, """ + txtPPTotalLimit.ClientID +
            """, """ + txtLossLimit.ClientID + """, """ + txtLossChgInLimit.ClientID + """, """ + txtLossTotalLimit.ClientID +
            """, """ + chkReplaceCost.ClientID + """, """ + hiddenCovALimit.ClientID + """, """ + hiddenCovATotal.ClientID +
            """, """ + hiddenCovBLimit.ClientID + """, """ + hiddenCovBChange.ClientID + """, """ + hiddenCovBTotal.ClientID +
            """, """ + hiddenCovCLimit.ClientID + """, """ + hiddenCovCChange.ClientID + """, """ + hiddenCovCTotal.ClientID +
            """, """ + hiddenCovDLimit.ClientID + """, """ + hiddenCovDChange.ClientID + """, """ + hiddenCovDTotal.ClientID +
            """, """ + hiddenFormType.ClientID + """, """ + chkRPS.ClientID + """);"
        txtDwLimit.Attributes.Add("onblur", scriptCalcDwellingLimit)
        txtRPSChgInLimit.Attributes.Add("onblur", scriptCalcDwellingLimit)
        txtPPChgInLimit.Attributes.Add("onblur", scriptCalcDwellingLimit)
        txtLossChgInLimit.Attributes.Add("onblur", scriptCalcDwellingLimit)
        VRScript.AddScriptLine(scriptCalcDwellingLimit)

        ' Calculate coverage from Personal Property C Limit (ONLY for FO4)
        Dim scriptCalcLimitC As String = "CalculateFromPPLimit(""" + txtPPLimit.ClientID + """, """ + txtPPTotalLimit.ClientID +
            """, """ + txtLossLimit.ClientID + """, """ + txtLossChgInLimit.ClientID + """, """ + txtLossTotalLimit.ClientID +
            """, """ + hiddenCovALimit.ClientID + """, """ + hiddenCovATotal.ClientID +
            """, """ + hiddenCovBLimit.ClientID + """, """ + hiddenCovBChange.ClientID + """, """ + hiddenCovBTotal.ClientID +
            """, """ + hiddenCovCLimit.ClientID + """, """ + hiddenCovCChange.ClientID + """, """ + hiddenCovCTotal.ClientID +
            """, """ + hiddenCovDLimit.ClientID + """, """ + hiddenCovDChange.ClientID + """, """ + hiddenCovDTotal.ClientID + """);"
        txtPPLimit.Attributes.Add("onblur", scriptCalcLimitC)

        ' Toggle Expanded Replacement Cost Terms Coverage based on Year Built
        Dim scriptToggleExpandReplaceByYear As String = "ToggleExpandReplaceByYear(this, """ + hiddenFormType.ClientID +
            """,""" + dvExpandReplacement.ClientID + """,""" + chkExpandReplacement.ClientID + """,""" + chkACV.ClientID + """,""" + hiddenBuiltYear.ClientID +
            """, """ + dvReplacement.ClientID + """, """ + chkReplacement.ClientID + """);"
        'VRScript.CreateJSBinding("txtYearBuilt" + MyLocationIndex.ToString(), ctlPageStartupScript.JsEventType.onblur, scriptToggleExpandReplaceByYear)
        'updated 3/3/2021 to use new property to get clientId for YearBuilt textbox
        VRScript.CreateJSBinding(Me.YearBuiltTextboxClientId, ctlPageStartupScript.JsEventType.onblur, scriptToggleExpandReplaceByYear)

        'VRScript.CreateJSBinding_CustomSelector("'.MyClass'", ctlPageStartupScript.JsEventType.onchange, "alert('test')")

        Dim scriptReplaceCostCovC As String = "CalculateCovCReplaceCost(""" + chkReplaceCost.ClientID + """, """ + txtDwLimit.ClientID +
        """, """ + txtPPLimit.ClientID + """, """ + txtPPChgInLimit.ClientID + """, """ + txtPPTotalLimit.ClientID +
        """, """ + hiddenCovALimit.ClientID + """, """ + hiddenCovATotal.ClientID +
        """, """ + hiddenCovBLimit.ClientID + """, """ + hiddenCovBChange.ClientID + """, """ + hiddenCovBTotal.ClientID +
        """, """ + hiddenCovCLimit.ClientID + """, """ + hiddenCovCChange.ClientID + """, """ + hiddenCovCTotal.ClientID +
        """, """ + hiddenCovDLimit.ClientID + """, """ + hiddenCovDChange.ClientID + """, """ + hiddenCovDTotal.ClientID +
        """, """ + hiddenProgramType.ClientID + """);"
        chkReplaceCost.Attributes.Add("onclick", scriptReplaceCostCovC)

        ' Added 10/14/22 for task 68166 BD Motorized Incresed Limit
        Dim script As String = "$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas(($('#{1}').val().toFloat() + $('#{2}').val().toFloat()).toString()));".FormatIFM(Me.txtMotorizedTotalLimit.ClientID, Me.txtMotorizedIncludedLimit.ClientID, Me.txtMotorizedIncreaseLimit.ClientID)
        Me.VRScript.CreateJSBinding(Me.txtMotorizedIncreaseLimit, ctlPageStartupScript.JsEventType.onkeyup, script)
        Dim scriptFDSC As String = "$('#{2}').val(ifm.vr.stringFormating.asRoundToNextNumber100($('#{2}').val()));$('#{2}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{2}').val()));$('#{0}').val(ifm.vr.stringFormating.asRoundToNextNumber100(($('#{1}').val().toFloat() + $('#{2}').val().toFloat()).toString()));$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{0}').val()));".FormatIFM(Me.txtMotorizedTotalLimit.ClientID, Me.txtMotorizedIncludedLimit.ClientID, Me.txtMotorizedIncreaseLimit.ClientID)
        Me.VRScript.CreateJSBinding(Me.txtMotorizedIncreaseLimit, ctlPageStartupScript.JsEventType.onblur, scriptFDSC)
        Me.VRScript.AddScriptLine("ifm.vr.ui.ElementDisabler($('#" + txtMotorizedIncludedLimit.ClientID + "'));", onlyAllowOnce:=True)
        Me.VRScript.AddScriptLine("ifm.vr.ui.ElementDisabler($('#" + txtMotorizedTotalLimit.ClientID + "'));", onlyAllowOnce:=True)

        ' added 70145 - Private Power Pole
        Me.VRScript.AddScriptLine("ifm.vr.ui.ElementDisabler($('#" + txtPrivatePowerPolesIncludedLimit.ClientID + "'));", onlyAllowOnce:=True)
        Me.VRScript.AddScriptLine("ifm.vr.ui.ElementDisabler($('#" + txtPrivatePowerPolesTotalLimit.ClientID + "'));", onlyAllowOnce:=True)
        Me.VRScript.AddScriptLine("ifm.vr.ui.ElementDisabler($('#" + chkPrivatePowerPoles.ClientID + "'));", onlyAllowOnce:=True)

        Dim scriptPrivatePowerPolesMath As String = "UpdateGenericPersonalPropertyControl(""" + txtPrivatePowerPolesIncludedLimit.ClientID + """,""" + txtPrivatePowerPolesIncreaseLimit.ClientID + """,""" + txtPrivatePowerPolesTotalLimit.ClientID + """);"
        Me.VRScript.CreateJSBinding(txtPrivatePowerPolesIncreaseLimit.ClientID, "blur", scriptPrivatePowerPolesMath)

        Dim scriptPrivatePowerPolesFilter As String = "$(this).val(FormatAsNumberNoCommaFormatting($(this).val()));"
        Me.VRScript.CreateJSBinding(txtPrivatePowerPolesIncreaseLimit.ClientID, "keyup", scriptPrivatePowerPolesFilter)

        Dim scriptPrivatePowerPolesCheck As String = "ToggleGenericPersonalPropertyControl(""" + tblPrivatePowerPoles.ClientID + """, """ + chkPrivatePowerPoles.ClientID + """, """ + txtPrivatePowerPolesIncreaseLimit.ClientID + """);"
        Me.VRScript.CreateJSBinding(chkPrivatePowerPoles.ClientID, "change", scriptPrivatePowerPolesCheck + scriptPrivatePowerPolesMath)

        ' Toggle Theft of Building Materials
        Dim scriptTheft As String = "ToggleTheft(""" + chkTheft.ClientID + """, """ + dvTheftLimit.ClientID + """, """ + txtTheftLimit.ClientID + """);"
        chkTheft.Attributes.Add("onclick", scriptTheft)

        'Toggle Underground Service Line - 80507
        Dim scriptUSL As String = "ToggleGenTextboxInput(""" + chkUndergroundServiceLine.ClientID + """, """ + dvUndergroundServiceLineLimit.ClientID + """);"
        chkUndergroundServiceLine.Attributes.Add("onclick", scriptUSL)

        ' Toggle Additional Residence
        Dim scriptCheckBox As String = "ToggleCheckboxOnly(""" + chkAddlResidence.ClientID + """);"
        chkAddlResidence.Attributes.Add("onclick", scriptCheckBox)

        ' Toggle Actual Cash Value
        Dim scriptACV As String = "ToggleExtendReplace(this, """ + dvExpandReplacement.ClientID + """, """ + chkExpandReplacement.ClientID + """, """ + hiddenFormType.ClientID +
            """, """ + hiddenBuiltYear.ClientID + """);"
        'Added for Bug 20405
        scriptACV += "ToggleFuncReplaceCost(this, """ + dvReplacement.ClientID + """, """ + chkReplacement.ClientID + """, """ + hiddenDwellingClassification.ClientID +
            """, """ + hiddenBuiltYear.ClientID + """);"
        chkACV.Attributes.Add("onclick", scriptACV)

        ' Toggle Actual Cash Value Wind
        Dim scriptACVWind As String = "ToggleCheckboxOnly(""" + chkACVWind.ClientID + """);"
        chkACVWind.Attributes.Add("onclick", scriptACVWind)

        ' Toggle Earthquake
        Dim scriptEarth As String = "ToggleCheckboxOnly(""" + chkEarthquake.ClientID + """);"
        chkEarthquake.Attributes.Add("onclick", scriptEarth)

        ' Toggle Expanded Replacement Cost Terms
        Dim ToggleExpandReplaceByChkbox As String = "ToggleExpandReplaceByChkbox(this, """ + hiddenFormType.ClientID +
            """,""" + dvExpandReplacement.ClientID + """,""" + chkExpandReplacement.ClientID + """,""" + chkACV.ClientID + """,""" + hiddenBuiltYear.ClientID +
            """, """ + dvReplacement.ClientID + """, """ + chkReplacement.ClientID + """);"

        Dim scriptExpandReplace As String = "ToggleCheckboxOnly(""" + chkExpandReplacement.ClientID + """);"
        chkExpandReplacement.Attributes.Add("onclick", scriptExpandReplace & ToggleExpandReplaceByChkbox)

        ' Toggle Functional Replacement Cost
        Dim scriptReplacement As String = "ToggleCheckboxOnly(""" + chkReplacement.ClientID + """);"
        chkReplacement.Attributes.Add("onclick", scriptReplacement)

        ' Toggle Related Private Structures
        Dim scriptRPS As String = "ToggleCheckboxOnly(""" + chkRPS.ClientID + """);"
        chkRPS.Attributes.Add("onclick", scriptRPS)

        If CosmeticDamageExHelper.IsCosmeticDamageExAvailable(Quote) Then
            Dim scriptCDE As String = "ToggleCheckboxOnly(""" + chkCosmeticDamageExclusion.ClientID + """);"
            chkCosmeticDamageExclusion.Attributes.Add("onclick", scriptCDE)
        Else
            'Handle Cosmetic Damage Exclusion clicks
            VRScript.CreateJSBinding(chkCosmeticDamageExclusion.ClientID, "onclick", "HandleCosmeticDamageClicks('" & chkCosmeticDamageExclusion.ClientID & "','" & dvCosmeticDamageExclusionData.ClientID & "');")
        End If


        ' Handle OH Mine Subsidence clicks
        If MyFarmLocation(MyLocationIndex).Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
            If IFM.VR.Common.Helpers.MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(MyFarmLocation(MyLocationIndex).Address.County) = MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleOptional Then
                VRScript.CreateJSBinding(chkMineSubsidence.ClientID, "onclick", "HandleOHMineSubClicks('" & chkMineSubsidence.ClientID & "','1','" & MyLocationIndex.ToString & "');")
            Else
                VRScript.CreateJSBinding(chkMineSubsidence.ClientID, "onclick", "HandleOHMineSubClicks('" & chkMineSubsidence.ClientID & "','0','" & MyLocationIndex.ToString & "');")
            End If
        End If

        ' Handle IL Mine Subsidence clicks
        If IFM.VR.Common.Helpers.FARM.ILMineSubsidenceHelper.IsILMineSubsidenceAvailable(Quote) = True Then
            If MyFarmLocation(MyLocationIndex).Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois Then
                VRScript.CreateJSBinding(chkMineSubsidence.ClientID, "onclick", "HandleILMineSubClicks('" & chkMineSubsidence.ClientID & "','" & MyLocationIndex.ToString & "');")
            End If
        End If

        'Removed 12/3/18 for new jQuery mine sub code MLW
        '' Toggle Mine Subsidence
        ''Added 10/29/18 for multi state MLW
        'If MultiState.General.IsMultistateCapableEffectiveDate(Me.Quote.EffectiveDate) Then
        '    Select Case (MyFarmLocation(MyLocationIndex).Address.StateId)
        '        Case States.Abbreviations.IL
        '            If MyFarmLocation(MyLocationIndex).FormTypeId <> "17" Then
        '                'toggle checkbox only
        '                Dim scriptMS As String = "ToggleCheckboxOnly(""" + chkMineSubsidence.ClientID + """);"
        '                chkMineSubsidence.Attributes.Add("onclick", scriptMS)
        '            Else
        '            End If
        '        Case States.Abbreviations.IN
        '            'delete message not turned of for IN - UA decided to not include in the bug 29623
        '        Case Else
        '            'state not yet supported
        '    End Select
        'Else
        '    'prior to multi state project - delete message not turned of for IN - UA decided to not include in the bug 29623
        'End If
        ''Removed 12/3/18 for new jQuery mine sub code MLW
        ''UpdateMineSubsidenceCheckboxScript()

        ' Round Number to nearest whole number
        Dim scriptRoundTheftLimit As String = "RoundToNearestNumberGT1000(""" + txtTheftLimit.ClientID + """);"
        txtTheftLimit.Attributes.Add("onblur", scriptRoundTheftLimit)

        txtDwLimit.Attributes.Add("onfocus", "this.select()")
        txtRPSChgInLimit.Attributes.Add("onfocus", "this.select()")
        txtPPChgInLimit.Attributes.Add("onfocus", "this.select()")
        txtLossChgInLimit.Attributes.Add("onfocus", "this.select()")
        txtPPLimit.Attributes.Add("onfocus", "this.select()")
        txtTheftLimit.Attributes.Add("onfocus", "this.select()")
    End Sub

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(ddlDeductible, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
    End Sub

    'Removed 12/3/18 for new jQuery mine sub code MLW
    ''Added 10/25/18 for multi state MLW
    'Public Sub UpdateMineSubsidenceCheckboxScript()
    '    Dim enbl As String = "FALSE"
    '    If MineSubsidenceCheckboxIsEnabled Then enbl = "TRUE"
    '    UpdateMineSubsidence()
    '    Me.chkMineSubsidence.Attributes.Add("onchange", "MineSubsidenceCheckboxChanged('" & chkMineSubsidence.ClientID & "','" & MyFarmLocation(MyLocationIndex).Address.State & "','" & enbl & "','" & dvMineSubsidenceNotReqHelpInfo.ClientID & "','" & MyFarmLocation(MyLocationIndex).FormTypeId & "');")
    'End Sub

    'Removed 12/3/18 for new jQuery mine sub code MLW
    ''Added 10/25/18 for multi state MLW
    'Private Sub UpdateMineSubsidence()

    '    'Added 10/25/18 for multi state MLW
    '    Dim helper As New CommonHelperClass()
    '    helper.RemoveCSSClassFromControl(chkMineSubsidence, "chkMine_IL_Enabled_NotReqd")
    '    helper.RemoveCSSClassFromControl(chkMineSubsidence, "chkMine_IL_Reqd")
    '    helper.RemoveCSSClassFromControl(chkMineSubsidence, "chkMine_IN")

    '    'Added 11/2/18 for multi state MLW
    '    Dim indexForFirstMineSubCheckboxEnabled As Int32 = FindIndexForFirstMineSubCheckboxEnabled(Quote)

    '    'Updated 10/11/18 for multi state MLW
    '    If MultiState.General.IsMultistateCapableEffectiveDate(Me.Quote.EffectiveDate) Then
    '        Select Case (MyFarmLocation(MyLocationIndex).Address.StateId)
    '            Case States.Abbreviations.IL
    '                dvMineSubsidence.Attributes.Add("style", "display:block;")
    '                If MineSubsidenceHelper.MineSubCountiesByStateAbbreviation(MyFarmLocation(MyLocationIndex).Address.State).Contains(MyFarmLocation(MyLocationIndex).Address.County.Trim.ToUpper()) Then
    '                    chkMineSubsidence.Checked = True
    '                    'chkMineSubsidence.Enabled = False '11/21/18 now handled in JS, because a disabled checkbox value will not get sent to the server - once checked it won't be able to be unchecked MLW
    '                    dvMineSubsidenceReqHelpInfo.Attributes.Add("style", "display:block;")
    '                    helper.AddCSSClassToControl(chkMineSubsidence, "chkMine_IL_Reqd")
    '                    dvMineSubsidenceReqHelpInfo.Attributes.Remove("class")
    '                    dvMineSubsidenceReqHelpInfo.Attributes.Add("class", "dvInfo_IL_Reqd informationalText")
    '                    MineSubsidenceCheckboxIsEnabled = False
    '                Else
    '                    Dim MineSub As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB)
    '                    If MineSub IsNot Nothing Then
    '                        chkMineSubsidence.Checked = True
    '                        'Added 10/29/18 for multi state MLW
    '                        If MyLocationIndex = indexForFirstMineSubCheckboxEnabled Then
    '                            chkMineSubsidence.Enabled = True
    '                            MineSubsidenceCheckboxIsEnabled = True
    '                        Else
    '                            'chkMineSubsidence.Enabled = False '11/21/18 now handled in JS, because a disabled checkbox value will not get sent to the server - once checked it won't be able to be unchecked MLW
    '                            MineSubsidenceCheckboxIsEnabled = False
    '                        End If
    '                        dvMineSubsidenceNotReqHelpInfo.Attributes.Add("style", "display:block;")
    '                    Else
    '                        chkMineSubsidence.Checked = False
    '                        chkMineSubsidence.Enabled = True
    '                        MineSubsidenceCheckboxIsEnabled = True
    '                    End If
    '                    dvMineSubsidenceNotReqHelpInfo.Attributes.Remove("class")
    '                    dvMineSubsidenceNotReqHelpInfo.Attributes.Add("class", "dvInfo_IL_NotReqd informationalText")
    '                    helper.AddCSSClassToControl(chkMineSubsidence, "chkMine_IL_Enabled_NotReqd")
    '                End If
    '            Case States.Abbreviations.IN
    '                If Not MineSubsidenceHelper.MineSubCountiesByStateAbbreviation(MyFarmLocation(MyLocationIndex).Address.State).Contains(MyFarmLocation(MyLocationIndex).Address.County.Trim.ToUpper()) Then
    '                    dvMineSubsidence.Attributes.Add("style", "display:none;")
    '                    chkMineSubsidence.Checked = False
    '                    MineSubsidenceCheckboxIsEnabled = False
    '                Else
    '                    dvMineSubsidence.Attributes.Add("style", "display:block;")
    '                    MineSubsidenceCheckboxIsEnabled = True
    '                    Dim MineSub As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA)
    '                    If MineSub IsNot Nothing Then
    '                        chkMineSubsidence.Checked = True
    '                    Else
    '                        chkMineSubsidence.Checked = False
    '                    End If
    '                End If
    '                helper.AddCSSClassToControl(chkMineSubsidence, "chkMine_IN")
    '            Case Else
    '                dvMineSubsidence.Attributes.Add("style", "display:none;")
    '                chkMineSubsidence.Checked = False
    '                MineSubsidenceCheckboxIsEnabled = False
    '        End Select
    '    Else
    '        If Not MineSubsidenceHelper.MineSubCountiesByStateAbbreviation(MyFarmLocation(MyLocationIndex).Address.State).Contains(MyFarmLocation(MyLocationIndex).Address.County.Trim.ToUpper()) Then
    '            dvMineSubsidence.Attributes.Add("style", "display:none;")
    '            chkMineSubsidence.Checked = False
    '            MineSubsidenceCheckboxIsEnabled = False
    '        Else
    '            dvMineSubsidence.Attributes.Add("style", "display:block;")
    '            MineSubsidenceCheckboxIsEnabled = True
    '        End If
    '        helper.AddCSSClassToControl(chkMineSubsidence, "chkMine_IN")
    '    End If
    'End Sub

    'Removed 12/3/18 for new jQuery mine sub code MLW
    ''Added 11/2/18 for multi state MLW
    'Private Function FindIndexForFirstMineSubCheckboxEnabled(Quote As QuickQuoteObject) As Int32
    '    'Added 10/29/18 for multi state MLW
    '    'find first location index that has mine sub for a non-req county
    '    Dim indexForFirstMineSubCheckboxEnabled As Int32 = -1

    '    If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing Then
    '        Dim quoteLocIndex As Int32 = 0
    '        For Each Loc As QuickQuoteLocation In Quote.Locations
    '            If Loc.Address.State = "IL" Then
    '                If MineSubsidenceHelper.MineSubCountiesByStateAbbreviation(Loc.Address.State).Contains(Loc.Address.County.Trim.ToUpper()) Then
    '                    'mine county - required, not it
    '                Else
    '                    'non mine county, find first
    '                    If Loc.SectionICoverages IsNot Nothing Then
    '                        Dim MineSub As QuickQuoteSectionICoverage = Loc.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB)
    '                        If MineSub IsNot Nothing Then
    '                            indexForFirstMineSubCheckboxEnabled = quoteLocIndex
    '                            Exit For
    '                        End If
    '                    End If
    '                End If
    '            End If
    '            quoteLocIndex += 1
    '        Next
    '    End If
    '    Return indexForFirstMineSubCheckboxEnabled
    'End Function

    Private Sub ToggleCoverageByForm()
        If MyFarmLocation IsNot Nothing Then
            RaiseEvent GetYearBult()
            hiddenBuiltYear.Value = YearBuilt

            'Get Dwelling Classification
            RaiseEvent GetDwellingClassification()
            hiddenDwellingClassification.Value = DwellingClassification


            If YearBuilt >= "1947" Then
                dvReplacement.Attributes.Add("style", "display:none;")
                chkReplacement.Checked = False
            Else
                dvReplacement.Attributes.Add("style", "display:block;")
            End If
            ' Removed until IS fixes issue
            'dvResidenceCovLiab.Attributes.Add("style", "display:block;")
            dvResidenceCovProp.Attributes.Add("style", "display:block;")

            If MyFarmLocation(MyLocationIndex).FormTypeId <> "17" Then  ' Check for FO-4
                dvReplacementCC.Attributes.Add("style", "display:block;") 'Fix for BUG 79779 BB
                dvCovA.Attributes.Add("style", "display:block;")
                dvCovB.Attributes.Add("style", "display:block;")
                lblPPReq.Text = ""
                txtPPLimit.Enabled = False
                txtPPLimit.BackColor = Drawing.Color.LightGray
                txtPPLimit.ForeColor = Drawing.Color.Gray
                txtPPChgInLimit.Enabled = True
                txtPPChgInLimit.BackColor = Drawing.Color.White
                txtPPChgInLimit.ForeColor = Drawing.Color.Black
                'Added 10/25/18 for multi state MLW
                dvMineSubsidenceReqHelpInfo.Attributes.Add("style", "display:none;")
                dvMineSubsidenceNotReqHelpInfo.Attributes.Add("style", "display:none;")
                'Removed 12/3/18 for new jQuery mine sub code MLW
                'UpdateMineSubsidence()

                chkExpandReplacement.Enabled = True
                ' Form Type MUST be FO-3 or FO-5
                If MyFarmLocation(MyLocationIndex).FormTypeId <> "16" And MyFarmLocation(MyLocationIndex).FormTypeId <> "18" Then
                    If MyFarmLocation(MyLocationIndex).FormTypeId = "-1" Then
                        chkExpandReplacement.Enabled = True
                    Else
                        chkExpandReplacement.Enabled = False
                        chkExpandReplacement.Checked = False
                    End If
                Else
                    If MyFarmLocation(MyLocationIndex).FormTypeId = "18" Then 'FO-5
                        dvReplaceCost.Attributes.Add("style", "display:none;")
                        chkReplaceCost.Checked = False
                        dvReplaceCostIncl.Attributes.Add("style", "display:block;")
                    End If
                    ' Year Built MUST be newer than 1950
                    If YearBuilt <> "" Then
                        'If Integer.Parse(YearBuilt) < 1951 Then
                        If Integer.Parse(YearBuilt) < DateTime.Now.Year - 40 Then 'ws-655 CAH
                            chkExpandReplacement.Enabled = False
                            chkExpandReplacement.Checked = False
                        Else
                            If MyFarmLocation(MyLocationIndex).SectionICoverages IsNot Nothing Then
                                If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlement).Count > 0 Then
                                    chkExpandReplacement.Enabled = False
                                    chkExpandReplacement.Checked = False
                                End If
                            Else
                                chkExpandReplacement.Enabled = True
                            End If
                        End If
                    End If

                End If
            Else    ' FO-4
                dvReplacementCC.Attributes.Add("style", "display:none;") 'Fix for BUG 79779 BB
                dvCovA.Attributes.Add("style", "display:none;")
                dvCovB.Attributes.Add("style", "display:none;")
                txtPPLimit.BackColor = Drawing.Color.LightGray
                txtPPLimit.ForeColor = Drawing.Color.Gray
                txtPPLimit.Enabled = False
                'lblPPReq.Text = "*"
                'txtPPLimit.Enabled = True
                'txtPPLimit.BackColor = Drawing.Color.White
                'txtPPLimit.ForeColor = Drawing.Color.Black
                'txtPPChgInLimit.Enabled = False
                'txtPPChgInLimit.BackColor = Drawing.Color.LightGray
                'txtPPChgInLimit.ForeColor = Drawing.Color.Gray
                chkReplacement.Checked = False
                dvReplacement.Attributes.Add("style", "display:none;")
                chkExpandReplacement.Enabled = False
                chkExpandReplacement.Checked = False
                'Removed 12/3/18 for new jQuery mine sub code MLW
                ''Added 10/26/18 for multi state MLW
                'If MultiState.General.IsMultistateCapableEffectiveDate(Me.Quote.EffectiveDate) Then
                '    Select Case (MyFarmLocation(MyLocationIndex).Address.StateId)
                '        Case States.Abbreviations.IL
                '            'Added 10/25/18 for multi state MLW - dwelling mine sub not available on FO-4
                '            dvMineSubsidence.Attributes.Add("style", "display:none;")
                '            chkMineSubsidence.Checked = False
                '            dvMineSubsidenceNotReqHelpInfo.Attributes.Remove("class")
                '            dvMineSubsidenceReqHelpInfo.Attributes.Add("style", "display:none;")
                '            dvMineSubsidenceNotReqHelpInfo.Attributes.Add("style", "display:none;")
                '        Case States.Abbreviations.IN
                '            'Added 10/31/18 for bug 29623 MLW - dwelling mine sub not available on FO-4
                '            dvMineSubsidence.Attributes.Add("style", "display:none;")
                '            chkMineSubsidence.Checked = False
                '            dvMineSubsidenceNotReqHelpInfo.Attributes.Remove("class")
                '            dvMineSubsidenceReqHelpInfo.Attributes.Add("style", "display:none;")
                '            dvMineSubsidenceNotReqHelpInfo.Attributes.Add("style", "display:none;")
                '        Case Else
                '            'not yet supported
                '            dvMineSubsidence.Attributes.Add("style", "display:none;")
                '            chkMineSubsidence.Checked = False
                '            dvMineSubsidenceNotReqHelpInfo.Attributes.Remove("class")
                '            dvMineSubsidenceReqHelpInfo.Attributes.Add("style", "display:none;")
                '            dvMineSubsidenceNotReqHelpInfo.Attributes.Add("style", "display:none;")
                '    End Select
                'End If
            End If
        End If
    End Sub

    Private Sub RemoveAllCosmeticDamageExclusionCoverages()
        If MyFarmLocation(MyLocationIndex).SectionICoverages IsNot Nothing AndAlso MyFarmLocation(MyLocationIndex).SectionICoverages.Count > 0 Then
remloop:
            For Each sc As QuickQuoteSectionICoverage In MyFarmLocation(MyLocationIndex).SectionICoverages
                If sc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Cosmetic_Damage_Exclusion Then
                    MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(sc)
                    GoTo remloop
                End If
            Next
        End If
    End Sub

    Public Overrides Sub Populate()
        If MyFarmLocation IsNot Nothing Then
            If ProgramType = "6" Then  ' Farm Owner
                LoadStaticData()
                hiddenFormType.Value = MyFarmLocation(MyLocationIndex).FormTypeId
                ToggleCoverageByForm()

                ctlFARAddlResidence.MyLocationIndex = MyLocationIndex

                ' Moved this to ToggleCoverage by form as that's where it belongs.  MGB
                ' Hide E2Value Button for an FO-4
                'If MyFarmLocation(MyLocationIndex).FormTypeId = "17" Then
                '    dvReplacementCC.Attributes.Add("style", "display:none;")
                'Else
                '    dvReplacementCC.Attributes.Add("style", "display:block;")
                'End If

                'If CosmeticDamageExHelper.IsCosmeticDamageExAvailable(Quote) Then
                '    dvCosmeticDamageExclusion.Attributes.Add("style", "display:block")
                '    dvCosmeticDamageExclusionData.Visible = False
                'Else
                ' Show or hide Cosmetic Damage Exclusion based on Effective Date and State
                ' Only display after OH eff date and if state is IL or OH
                If IsOhioEffective(Quote) AndAlso MyFarmLocation(MyLocationIndex).Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois OrElse MyFarmLocation(MyLocationIndex).Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                    dvCosmeticDamageExclusion.Attributes.Add("style", "display:block")
                Else
                    dvCosmeticDamageExclusion.Attributes.Add("style", "display:none;") 'Added 8/9/2022 for task 74881 MLW
                    RemoveAllCosmeticDamageExclusionCoverages()
                End If
                'End If


                'Show or Hide Underground Service Line
                If FARM.UndergroundServiceLineHelper.isUndergroundServiceLineAvailable(Quote) Then
                    dvUndergroundServiceLine.Attributes.Add("style", "display:block")
                    txtUndergroundServiceLineLimit.Text = "10,000"
                Else
                    MyFarmLocation(MyLocationIndex).SectionICoverages?.RemoveAll(Function(sc) sc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.UndergroundServiceLine)
                    dvUndergroundServiceLine.Attributes.Add("style", "display:none;")
                    chkUndergroundServiceLine.Checked = False
                    txtUndergroundServiceLineLimit.Text = "0"
                End If

                'Show or Hide Motorized Increased Limit
                If MyFarmLocation(0).FormTypeId.EqualsAny("13", "14", "15", "16", "17", "18") Then
                    dvMotorizedVehiclesLimit.Attributes.Add("style", "display:block")
                    chkMotorizedVehiclesIncreasedLimit.Checked = True
                    chkMotorizedVehiclesIncreasedLimit.Enabled = False
                    txtMotorizedIncludedLimit.Text = "5,000"
                    txtMotorizedTotalLimit.Text = "5,000"
                    txtMotorizedIncreaseLimit.Text = "0"
                Else
                    MyFarmLocation(MyLocationIndex).SectionICoverages?.RemoveAll(Function(sc) sc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Motorized_Vehicles_Increased_Limits)
                    dvMotorizedVehiclesLimit.Attributes.Add("style", "display:none;")
                    chkMotorizedVehiclesIncreasedLimit.Checked = False
                End If

                ' Show or HidePrivate Power/Light Poles 70145
                tblPrivatePowerPoles.Attributes.Add("style", "display:none;")
                dvPrivatePowerPoles.Attributes.Add("style", "display:none;")
                If Common.Helpers.FARM.PrivatePowerPolesHelper.IsPrivatePowerPolesAvailable(Quote) Then
                    If MyFarmLocation(MyLocationIndex).FormTypeId <> "17" Then
                        If MyFarmLocation(MyLocationIndex).FormTypeId = "13" OrElse MyFarmLocation(MyLocationIndex).FormTypeId = "15" OrElse MyFarmLocation(MyLocationIndex).FormTypeId = "16" Then
                            txtPrivatePowerPolesIncludedLimit.Text = FO2ANDFO3DefaultPrivatePowerPolesIncludedLimit
                        Else
                            txtPrivatePowerPolesIncludedLimit.Text = FO5DefaultPrivatePowerPolesIncludedLimit
                        End If
                        Dim PrivatePowerPoles As QuickQuoteCoverage
                        If MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages Is Nothing Then
                            MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages = New List(Of QuickQuoteCoverage)
                        End If
                        If MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.FindAll(Function(p) p.CoverageCodeId = "70145").Count > 0 Then
                            PrivatePowerPoles = MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.Find(Function(p) p.CoverageCodeId = "70145")
                            If txtPrivatePowerPolesIncreaseLimit.Text = "0" Then
                                txtPrivatePowerPolesIncreaseLimit.Text = String.Empty
                            End If
                            If PrivatePowerPoles.ManualLimitIncreased = "0" OrElse PrivatePowerPoles.ManualLimitIncreased = String.Empty Then
                                If MyFarmLocation(MyLocationIndex).FormTypeId = "13" OrElse MyFarmLocation(MyLocationIndex).FormTypeId = "15" OrElse MyFarmLocation(MyLocationIndex).FormTypeId = "16" Then
                                    PrivatePowerPoles.ManualLimitAmount = FO2ANDFO3DefaultPrivatePowerPolesIncludedLimit
                                Else
                                    PrivatePowerPoles.ManualLimitAmount = FO5DefaultPrivatePowerPolesIncludedLimit
                                End If
                            End If
                            txtPrivatePowerPolesIncreaseLimit.Text = Format(PrivatePowerPoles.ManualLimitIncreased.TryToGetInt32, "N0")
                            txtPrivatePowerPolesTotalLimit.Text = Format(PrivatePowerPoles.ManualLimitAmount.TryToGetInt32, "N0")
                        Else
                            PrivatePowerPoles = New QuickQuoteCoverage()
                            PrivatePowerPoles.CoverageCodeId = "70145"
                            If MyFarmLocation(MyLocationIndex).FormTypeId = "13" OrElse MyFarmLocation(MyLocationIndex).FormTypeId = "15" OrElse MyFarmLocation(MyLocationIndex).FormTypeId = "16" Then
                                PrivatePowerPoles.ManualLimitIncluded = FO2ANDFO3DefaultPrivatePowerPolesIncludedLimit
                                PrivatePowerPoles.ManualLimitAmount = FO2ANDFO3DefaultPrivatePowerPolesIncludedLimit
                            Else
                                PrivatePowerPoles.ManualLimitIncluded = FO5DefaultPrivatePowerPolesIncludedLimit
                                PrivatePowerPoles.ManualLimitAmount = FO5DefaultPrivatePowerPolesIncludedLimit
                            End If
                                MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.Add(PrivatePowerPoles)
                                txtPrivatePowerPolesIncreaseLimit.Text = String.Empty
                            If MyFarmLocation(MyLocationIndex).FormTypeId = "13" OrElse MyFarmLocation(MyLocationIndex).FormTypeId = "15" OrElse MyFarmLocation(MyLocationIndex).FormTypeId = "16" Then
                                txtPrivatePowerPolesTotalLimit.Text = FO2ANDFO3DefaultPrivatePowerPolesIncludedLimit
                            Else
                                txtPrivatePowerPolesTotalLimit.Text = FO5DefaultPrivatePowerPolesIncludedLimit
                            End If
                        End If
                        chkPrivatePowerPoles.Checked = True
                        chkPrivatePowerPoles.Enabled = False
                        tblPrivatePowerPoles.Attributes.Add("style", "display:block;")
                        dvPrivatePowerPoles.Attributes.Add("style", "display:block;")
                    End If
                End If

                ' OHIO Mine Subsidence - display if eligible
                ' Add classes to the controls so we can easily manipulate them with jquery

                If MyFarmLocation(MyLocationIndex).Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                    chkMineSubsidence.Attributes.Remove("class")
                    dvMineSubsidenceReqHelpInfo_OH.Attributes.Add("style", "display:none;")
                    dvMineSubLimitInfo.Attributes.Add("style", "display:none;")
                    Select Case IFM.VR.Common.Helpers.MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(MyFarmLocation(MyLocationIndex).Address.County)
                        Case MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleMandatory
                            ' When mandatory the coverage is always checked and disabled
                            dvMineSubsidence.Attributes.Add("style", "display:block")
                            dvMineSubsidenceReqHelpInfo_OH.Attributes.Add("style", "display:block;margin-left:20px;")
                            If MyFarmLocation(MyLocationIndex).A_Dwelling_Limit IsNot Nothing AndAlso IsNumeric(MyFarmLocation(MyLocationIndex).A_Dwelling_Limit) Then
                                If CDec(MyFarmLocation(MyLocationIndex).A_Dwelling_Limit) >= 300000 Then
                                    dvMineSubLimitInfo.Attributes.Add("style", "display:block;margin-left:20px;")
                                End If
                            End If
                            chkMineSubsidence.Attributes.Add("class", "chkOHMineSubMandatory_" & MyLocationIndex.ToString)
                            chkMineSubsidence.Checked = True
                            chkMineSubsidence.Enabled = False
                            Exit Select
                        Case MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleOptional
                            ' When optional, the coverage is shown and enabled.  We'll check it later
                            dvMineSubsidence.Attributes.Add("style", "display:block")
                            chkMineSubsidence.Attributes.Add("class", "chkOHMineSubOptional_" & MyLocationIndex.ToString)
                            chkMineSubsidence.Checked = False
                            chkMineSubsidence.Enabled = True
                        Case Else
                            ' Not eligible.  Don't show it
                            dvMineSubsidence.Attributes.Add("style", "display:none")
                            chkMineSubsidence.Checked = False
                            Exit Select
                    End Select
                End If

                If IFM.VR.Common.Helpers.FARM.ILMineSubsidenceHelper.IsILMineSubsidenceAvailable(Quote) = True Then
                    If MyFarmLocation(MyLocationIndex).Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois Then
                        lblMineSubsidenceReqHelpInfo_OH.Attributes.Add("style", "display:none;")
                        dvMineSubsidenceReqHelpInfo.Attributes.Add("style", "display:none;")
                        chkMineSubsidence.Enabled = True
                        Select Case IFM.VR.Common.Helpers.MineSubsidenceHelper.GetIllinoisMineSubsidenceTypeByCounty(MyFarmLocation(MyLocationIndex).Address.County)
                            Case MineSubsidenceHelper.IllinoisMineSubsidenceType_enum.EligibleMandatory
                                ' When mandatory the coverage is always checked and enabled
                                dvMineSubsidence.Attributes.Add("style", "display:block")
                                dvMineSubsidenceReqHelpInfo.Attributes.Add("style", "display:block;margin-left:20px;")
                                chkMineSubsidence.Attributes.Add("class", "chkILMineSubsidence_" & MyLocationIndex.ToString)
                                Exit Select
                            Case MineSubsidenceHelper.IllinoisMineSubsidenceType_enum.EligibleOptional
                                ' When optional, the coverage is shown and enabled.  We'll check it later
                                dvMineSubsidence.Attributes.Add("style", "display:block")
                                chkMineSubsidence.Attributes.Add("class", "chkILMineSubsidence_" & MyLocationIndex.ToString)
                                Exit Select
                        End Select
                    End If
                End If

                If MyLocationIndex > 0 AndAlso IsQuoteEndorsement() = False Then
                    ddlDeductible.Visible = False
                    lblDeductible.Visible = True

                    hiddenFormType.Value = MyFarmLocation(MyLocationIndex).FormTypeId

                    ' Removed until IS fixes issue
                    'dvResidenceCovLiab.Visible = False
                    'chkAddlResidence.Checked = False

                    lblDeductible.Text = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, MyFarmLocation(0).DeductibleLimitId)
                    lblFarmLocDeduct.ToolTip = "Dwelling Deductible set on Primary Dwelling"
                Else
                    ddlDeductible.Visible = True
                    lblDeductible.Visible = False
                    ' Removed until IS fixes issue
                    'dvResidenceCovLiab.Visible = True

                    'Changed for Farm Endo 20201120 CAH
                    'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlDeductible, MyFarmLocation(MyLocationIndex).DeductibleLimitId)

                    IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlDeductible, MyFarmLocation(MyLocationIndex).DeductibleLimitId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleLimitId)

                    'If QQHelper.IsPositiveIntegerString(MyFarmLocation(MyLocationIndex).DeductibleLimitId) AndAlso ddlDeductible.Items.FindByValue(MyFarmLocation(MyLocationIndex).DeductibleLimitId) Is Nothing Then
                    '    Dim TypeDescription As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleLimitId, MyFarmLocation(MyLocationIndex).DeductibleLimitId)
                    '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(Me.ddlDeductible, MyFarmLocation(MyLocationIndex).DeductibleLimitId, TypeDescription)
                    'Else
                    '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddlDeductible, MyFarmLocation(MyLocationIndex).DeductibleLimitId)
                    'End If
                End If

                ' Coverage A
                If MyFarmLocation(MyLocationIndex).A_Dwelling_LimitIncreased <> "" Then
                    txtDwLimit.Text = MyFarmLocation(MyLocationIndex).A_Dwelling_LimitIncreased
                    hiddenCovALimit.Value = MyFarmLocation(MyLocationIndex).A_Dwelling_LimitIncreased
                Else
                    If MyLocationIndex = 0 Then
                        txtDwLimit.Text = "0"
                        hiddenCovALimit.Value = "0"
                    End If
                End If

                If MyFarmLocation(MyLocationIndex).A_Dwelling_LimitIncreased <> "" Then
                    txtDwellingTotalLimit.Text = MyFarmLocation(MyLocationIndex).A_Dwelling_Limit
                    hiddenCovATotal.Value = MyFarmLocation(MyLocationIndex).A_Dwelling_Limit
                Else
                    If MyLocationIndex = 0 Then
                        txtDwellingTotalLimit.Text = "0"
                        hiddenCovATotal.Value = "0"
                    End If
                End If

                ' Coverage B
                If MyFarmLocation(MyLocationIndex).B_OtherStructures_LimitIncluded <> "" Then
                    txtRPSLimit.Text = MyFarmLocation(MyLocationIndex).B_OtherStructures_LimitIncluded
                    hiddenCovBLimit.Value = MyFarmLocation(MyLocationIndex).B_OtherStructures_LimitIncluded
                    hiddenCovBTotal.Value = MyFarmLocation(MyLocationIndex).B_OtherStructures_Limit
                Else
                    If hiddenCovATotal.Value = "0" Then
                        txtRPSLimit.Text = "0"
                        hiddenCovBLimit.Value = "0"
                    End If
                End If

                'txtRPSChgInLimit.Text = "0"
                'txtRPSTotalLimit.Text = MyFarmLocation(MyLocationIndex).B_OtherStructures_LimitIncluded

                If MyFarmLocation(MyLocationIndex).B_OtherStructures_LimitIncreased <> "" Then
                    txtRPSChgInLimit.Text = MyFarmLocation(MyLocationIndex).B_OtherStructures_LimitIncreased
                    hiddenCovBChange.Value = MyFarmLocation(MyLocationIndex).B_OtherStructures_LimitIncreased
                Else
                    If hiddenCovATotal.Value = "0" Then
                        txtRPSChgInLimit.Text = "0"
                        hiddenCovBChange.Value = "0"
                    End If
                End If

                If MyFarmLocation(MyLocationIndex).B_OtherStructures_LimitIncreased <> "" Then
                    txtRPSTotalLimit.Text = MyFarmLocation(MyLocationIndex).B_OtherStructures_Limit
                    hiddenCovBTotal.Value = MyFarmLocation(MyLocationIndex).B_OtherStructures_Limit
                Else
                    If hiddenCovATotal.Value = "0" Then
                        txtRPSTotalLimit.Text = "0"
                        hiddenCovBTotal.Value = "0"
                    End If
                End If

                ' Coverage C
                If MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncluded <> "" Then
                    txtPPLimit.Text = MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncluded
                    hiddenCovCLimit.Value = MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncluded
                Else
                    If hiddenCovATotal.Value = "0" Then
                        txtPPLimit.Text = "0"
                        hiddenCovCLimit.Value = "0"
                    End If
                End If

                If MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncreased <> "" Then
                    txtPPChgInLimit.Text = MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncreased
                    hiddenCovCChange.Value = MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncreased
                Else
                    If hiddenCovATotal.Value = "0" Then
                        txtPPChgInLimit.Text = "0"
                        hiddenCovCChange.Value = "0"
                    End If
                End If

                If MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncreased <> "" Then
                    txtPPTotalLimit.Text = MyFarmLocation(MyLocationIndex).C_PersonalProperty_Limit
                    hiddenCovCTotal.Value = MyFarmLocation(MyLocationIndex).C_PersonalProperty_Limit
                Else
                    If hiddenCovATotal.Value = "0" Then
                        txtPPTotalLimit.Text = "0"
                        hiddenCovCTotal.Value = "0"
                    End If
                End If

                ' Coverage D
                If MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncreased <> "" Then
                    txtLossLimit.Text = MyFarmLocation(MyLocationIndex).D_LossOfUse_LimitIncluded
                    hiddenCovDLimit.Value = MyFarmLocation(MyLocationIndex).D_LossOfUse_LimitIncluded
                Else
                    If hiddenCovATotal.Value = "0" Then
                        txtLossLimit.Text = "0"
                        hiddenCovDLimit.Value = "0"
                    End If
                End If

                If MyFarmLocation(MyLocationIndex).D_LossOfUse_LimitIncreased <> "" Then
                    txtLossChgInLimit.Text = MyFarmLocation(MyLocationIndex).D_LossOfUse_LimitIncreased
                    hiddenCovDChange.Value = MyFarmLocation(MyLocationIndex).D_LossOfUse_LimitIncreased
                Else
                    If hiddenCovATotal.Value = "0" Then
                        txtLossChgInLimit.Text = "0"
                        hiddenCovDLimit.Value = "0"
                    End If
                End If

                If MyFarmLocation(MyLocationIndex).D_LossOfUse_LimitIncreased <> "" Then
                    txtLossTotalLimit.Text = MyFarmLocation(MyLocationIndex).D_LossOfUse_Limit
                    hiddenCovDTotal.Value = MyFarmLocation(MyLocationIndex).D_LossOfUse_Limit
                Else
                    If hiddenCovATotal.Value = "0" Then
                        txtLossTotalLimit.Text = "0"
                        hiddenCovDTotal.Value = "0"
                    End If
                End If

                If MyFarmLocation(MyLocationIndex).SectionICoverages IsNot Nothing Then
                    dvTheftLimit.Attributes.Add("style", "display:none;")
                    For Each sc As QuickQuoteSectionICoverage In MyFarmLocation(MyLocationIndex).SectionICoverages
                        Select Case sc.CoverageType
                            ' 70117 - Replacement Cost - Cov C
                            Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Replacement_Value_Personal_Property_Cov_C_
                                chkReplaceCost.Checked = True
                            ' 40173 - Actual Cash Value Loss Settlement
                            Case QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlement
                                chkACV.Checked = True
                            ' 40142 - Actual Cash Value Loss Settlement - Windstorm or Hail Losses to Roof Surfacing
                            Case QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing
                                chkACVWind.Checked = True
                            ' 50095 - Theft of Building Materials
                            Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Dwelling_Under_Construction_Theft
                                dvTheftLimit.Attributes.Add("style", "display:block;")
                                txtTheftLimit.Text = sc.IncreasedLimit
                                chkTheft.Checked = True
                            ' 80137 - Earthquake
                            Case QuickQuoteSectionICoverage.SectionICoverageType.Earthquake_Location
                                chkEarthquake.Checked = True
                            ' 70110 Expanded Replacement Cost Terms
                            Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Expanded_Replacement_Cost
                                chkExpandReplacement.Checked = True
                                chkExpandReplacement.Enabled = True
                            ' 40172 - Functional Replacement Cost Loss Settlement
                            Case QuickQuoteSectionICoverage.SectionICoverageType.FunctionalReplacementCostLossAssessment
                                chkReplacement.Checked = True
                            ' 80102 - Mine Subsidence Cov A
                            Case QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA
                                chkMineSubsidence.Checked = True
                            Case QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB
                                ' OH
                                chkMineSubsidence.Checked = True
                                ' Control the mine sub info messages
                                dvMineSubsidenceReqHelpInfo_OH.Attributes.Add("style", "display:none;")
                                dvMineSubLimitInfo.Attributes.Add("style", "display:none;")
                                Select Case IFM.VR.Common.Helpers.MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(MyFarmLocation(MyLocationIndex).Address.County)
                                    Case MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleMandatory
                                        ' When mandatory, show the appropriate messsage.
                                        dvMineSubsidenceReqHelpInfo_OH.Attributes.Add("style", "display:block;margin-left:20px;")
                                        ' When dwelling limit is 300k or larger, show the limit message
                                        If MyFarmLocation(MyLocationIndex).A_Dwelling_Limit IsNot Nothing AndAlso IsNumeric(MyFarmLocation(MyLocationIndex).A_Dwelling_Limit) Then
                                            If CDec(MyFarmLocation(MyLocationIndex).A_Dwelling_Limit) >= 300000 Then
                                                dvMineSubLimitInfo.Attributes.Add("style", "display:block;margin-left:20px;")
                                            End If
                                        End If
                                        Exit Select
                                    Case MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleOptional
                                        ' When dwelling limit is 300k or larger, show the limit message
                                        If MyFarmLocation(MyLocationIndex).A_Dwelling_Limit IsNot Nothing AndAlso IsNumeric(MyFarmLocation(MyLocationIndex).A_Dwelling_Limit) Then
                                            If CDec(MyFarmLocation(MyLocationIndex).A_Dwelling_Limit) >= 300000 Then
                                                dvMineSubLimitInfo.Attributes.Add("style", "display:block;margin-left:20px;")
                                            End If
                                        End If
                                        Exit Select
                                    Case Else
                                        Exit Select
                                End Select

                                Exit Select
                            ' 554 - Related Private Structures
                            Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_PrivateStructures_IncreasedLimitsForSpecificStructures
                                chkRPS.Checked = True
                                txtRPSChgInLimit.Text = sc.IncreasedLimit
                                hiddenCovBChange.Value = sc.IncreasedLimit

                                txtRPSTotalLimit.Text = MyFarmLocation(MyLocationIndex).B_OtherStructures_Limit
                                hiddenCovBTotal.Value = txtRPSTotalLimit.Text
                            Case QuickQuoteSectionICoverage.SectionICoverageType.Cosmetic_Damage_Exclusion

                                'Updated 8/9/2022 for task 74881 MLW
                                If MyFarmLocation(MyLocationIndex).Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois OrElse MyFarmLocation(MyLocationIndex).Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                                    chkCosmeticDamageExclusion.Checked = True
                                    If CosmeticDamageExHelper.IsCosmeticDamageExAvailable(Quote) Then
                                        dvCosmeticDamageExclusionData.Visible = False
                                    Else
                                        dvCosmeticDamageExclusionData.Attributes.Add("style", "display:block;text-indent:25px;")
                                        If sc.ExteriorDoorWindowSurfacing Then chkCDEExteriorDoorAndWindowSurfacing.Checked = True
                                        If sc.ExteriorWallSurfacing Then chkCDEExteriorWallSurfacing.Checked = True
                                        If sc.RoofSurfacing Then chkCDERoofSurfacing.Checked = True

                                    End If

                                End If

                                Exit Select
                            Case QuickQuoteSectionICoverage.SectionICoverageType.UndergroundServiceLine
                                chkUndergroundServiceLine.Checked = True
                                dvUndergroundServiceLineLimit.Attributes.Add("style", "display:block;")
                                Exit Select
                            Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Motorized_Vehicles_Increased_Limits
                                chkMotorizedVehiclesIncreasedLimit.Checked = True
                                dvMotorizedVehiclesLimit.Attributes.Add("style", "display:block")
                                txtMotorizedIncludedLimit.Text = sc.IncludedLimit
                                txtMotorizedIncreaseLimit.Text = sc.IncreasedLimit
                                txtMotorizedTotalLimit.Text = sc.TotalLimit
                                Exit Select
                        End Select
                    Next
                Else
                    If MyLocationIndex = 0 Then
                        If MyFarmLocation(MyLocationIndex).Acreages(0).Acreage <> "" Then
                            If MyFarmLocation(MyLocationIndex).SectionICoverages IsNot Nothing Then
                                Dim replaceCost As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Replacement_Value_Personal_Property_Cov_C_)
                                If replaceCost IsNot Nothing Then
                                    chkReplaceCost.Checked = True
                                Else
                                    chkReplaceCost.Checked = False
                                End If
                            Else
                                chkReplaceCost.Checked = False
                            End If
                        Else
                            chkReplaceCost.Checked = True   ' Default
                        End If
                    End If
                End If

                'If MyFarmLocation(MyLocationIndex).DwellingTypeId = " Then
                If MyFarmLocation(MyLocationIndex).SectionIICoverages IsNot Nothing Then
                    For Each sc As QuickQuoteSectionIICoverage In MyFarmLocation(MyLocationIndex).SectionIICoverages
                        Select Case sc.CoverageType
                            ' 40044 - Additional Residence - Occupied by insured
                            Case QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencePremisesOccupiedbyanInsured
                                chkAddlResidence.Checked = True
                        End Select
                    Next
                    Me.ctlFARAddlResidence.Populate()
                End If
                'End If

                'E2Value
                If MyFarmLocation(MyLocationIndex).PropertyValuation IsNot Nothing AndAlso MyFarmLocation(MyLocationIndex).PropertyValuation.db_propertyValuationId <> "" _
                    AndAlso MyFarmLocation(MyLocationIndex).PropertyValuation.VendorValuationId <> "" AndAlso MyFarmLocation(MyLocationIndex).RebuildCost <> "" Then
                    ' Set E2Value
                    lblCalValue.Visible = True
                    lblReplacementCCValue.Visible = True
                    lblReplacementCCValue.Text = MyFarmLocation(MyLocationIndex).RebuildCost.Substring(1).Split(".")(0)

                    ' Set Coverage A
                    If Session("valuationValue") = "True" Then
                        txtDwLimit.Text = lblReplacementCCValue.Text
                    End If

                    txtDwellingTotalLimit.Text = (Integer.Parse(txtDwLimit.Text.Replace(",", "")) + Integer.Parse(txtDwellingChangeInLimit.Text.Replace(",", ""))).ToString("N0")
                    hiddenCovALimit.Value = txtDwLimit.Text
                    hiddenCovAChange.Value = txtDwellingChangeInLimit.Text
                    hiddenCovATotal.Value = txtDwellingTotalLimit.Text

                    ' Set Coverage B
                    txtRPSLimit.Text = (Integer.Parse(txtDwellingTotalLimit.Text.Replace(",", "")) * 0.1).ToString("N0")
                    txtRPSTotalLimit.Text = (Integer.Parse(txtRPSLimit.Text.Replace(",", "")) + Integer.Parse(txtRPSChgInLimit.Text.Replace(",", ""))).ToString("N0")
                    hiddenCovBLimit.Value = txtRPSLimit.Text
                    hiddenCovBTotal.Value = txtRPSTotalLimit.Text

                    ' Set Coverage C
                    txtPPLimit.Text = (Integer.Parse(txtDwellingTotalLimit.Text.Replace(",", "")) * 0.5).ToString("N0")

                    If chkReplaceCost.Checked Then
                        txtPPChgInLimit.Text = (Integer.Parse(txtDwellingTotalLimit.Text.Replace(",", "")) * 0.2).ToString("N0")
                    End If

                    txtPPTotalLimit.Text = (Integer.Parse(txtPPLimit.Text.Replace(",", "")) + Integer.Parse(txtPPChgInLimit.Text.Replace(",", ""))).ToString("N0")
                    hiddenCovCLimit.Value = txtPPLimit.Text
                    hiddenCovCChange.Value = txtPPChgInLimit.Text
                    hiddenCovCTotal.Value = txtPPTotalLimit.Text

                    ' Shows date
                    If IsDate(MyFarmLocation(MyLocationIndex).LastCostEstimatorDate) Then

                    End If
                Else
                    lblCalValue.Visible = False
                    lblReplacementCCValue.Visible = False
                    lblReplacementCCValue.Text = ""
                End If
            End If
            PopulateChildControls()
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If MyFarmLocation IsNot Nothing Then
            Dim SectionICoverage As QuickQuoteSectionICoverage = Nothing
            Dim SectionIICoverage As QuickQuoteSectionIICoverage = Nothing

            If MyFarmLocation(MyLocationIndex).SectionICoverages Is Nothing Then
                MyFarmLocation(MyLocationIndex).SectionICoverages = New List(Of QuickQuoteSectionICoverage)
            End If

            If MyFarmLocation(MyLocationIndex).SectionIICoverages Is Nothing Then
                MyFarmLocation(MyLocationIndex).SectionIICoverages = New List(Of QuickQuoteSectionIICoverage)
            End If

            ' Farm owner
            If ProgramType = "6" Then
                If MyLocationIndex > 0 AndAlso IsQuoteEndorsement() = False Then
                    If ResidenceExists Then
                        MyFarmLocation(MyLocationIndex).DeductibleLimitId = MyFarmLocation(0).DeductibleLimitId 'PrimaryLocDeduct
                        MyFarmLocation(MyLocationIndex).ProtectionClassId = "10"
                    Else
                        MyFarmLocation(MyLocationIndex).DeductibleLimitId = "0"
                        'PrimaryLocDeduct = "0"
                        'chkReplaceCost.Checked = False
                        MyFarmLocation(MyLocationIndex).ProtectionClassId = "-1"
                    End If
                Else
                    MyFarmLocation(MyLocationIndex).DeductibleLimitId = ddlDeductible.SelectedValue
                    'PrimaryLocDeduct = ddlDeductible.SelectedValue
                    MyFarmLocation(MyLocationIndex).ProtectionClassId = "10"

                    If (MyFarmLocation(0).FormTypeId <> "15" AndAlso MyFarmLocation(0).FormTypeId <> "16" AndAlso MyFarmLocation(0).FormTypeId <> "18" AndAlso MyFarmLocation(0).FormTypeId <> "13") AndAlso IsQuoteEndorsement() = False Then
                        'Quote.HasFarmEquipmentBreakdown = False
                        'updated 8/21/2018 for multi-state
                        'would be best in this case to Save on all states unless we update functionality to allow different formTypes on different states
                        For Each stateQuote As QuickQuoteObject In SubQuotes
                            stateQuote.HasFarmEquipmentBreakdown = False
                        Next
                    End If
                End If

                'MyFarmLocation(MyLocationIndex).A_Dwelling_LimitIncluded = hiddenCovALimit.Value
                MyFarmLocation(MyLocationIndex).A_Dwelling_LimitIncreased = hiddenCovALimit.Value
                MyFarmLocation(MyLocationIndex).A_Dwelling_Limit = hiddenCovATotal.Value
                MyFarmLocation(MyLocationIndex).B_OtherStructures_LimitIncluded = hiddenCovBLimit.Value

                ' 554 - Related Private Structures
                If hiddenCovBChange.Value <> "" And hiddenCovBChange.Value <> "0" Then
                    If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_PrivateStructures_IncreasedLimitsForSpecificStructures).Count <= 0 Then
                        SectionICoverage = New QuickQuoteSectionICoverage()
                        SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_PrivateStructures_IncreasedLimitsForSpecificStructures
                        SectionICoverage.IncreasedLimit = hiddenCovBChange.Value
                        SectionICoverage.TotalLimit = hiddenCovBChange.Value
                        SectionICoverage.Description = "increase coverage B"
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                        chkRPS.Checked = True
                    Else
                        Dim editRPSChg As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_PrivateStructures_IncreasedLimitsForSpecificStructures)
                        editRPSChg.IncreasedLimit = hiddenCovBChange.Value
                        editRPSChg.TotalLimit = hiddenCovBChange.Value
                    End If

                    'MyFarmLocation(MyLocationIndex).B_OtherStructures_LimitIncreased = hiddenCovBChange.Value
                Else
                    Dim rpsChange As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_PrivateStructures_IncreasedLimitsForSpecificStructures)
                    If rpsChange IsNot Nothing Then
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(rpsChange)
                        chkRPS.Checked = False
                    End If
                End If

                ' 03/25/2021 CAH B60603
                'MyFarmLocation(MyLocationIndex).B_OtherStructures_LimitIncreased = hiddenCovBChange.Value
                MyFarmLocation(MyLocationIndex).B_OtherStructures_Limit = hiddenCovBTotal.Value
                'MyFarmLocation(MyLocationIndex).B_OtherStructures_Limit = hiddenCovBLimit.Value
                MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncluded = hiddenCovCLimit.Value
                MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncreased = hiddenCovCChange.Value
                MyFarmLocation(MyLocationIndex).C_PersonalProperty_Limit = hiddenCovCTotal.Value
                MyFarmLocation(MyLocationIndex).D_LossOfUse_LimitIncluded = hiddenCovDLimit.Value
                MyFarmLocation(MyLocationIndex).D_LossOfUse_LimitIncreased = hiddenCovDChange.Value
                MyFarmLocation(MyLocationIndex).D_LossOfUse_Limit = hiddenCovDTotal.Value

                ' Section II Coverages
                ' 40044 - Additional Residence - Occupied by insured
                If chkAddlResidence.Checked Then
                    If MyFarmLocation(MyLocationIndex).SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencePremisesOccupiedbyanInsured).Count <= 0 Then
                        SectionIICoverage = New QuickQuoteSectionIICoverage()
                        SectionIICoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencePremisesOccupiedbyanInsured
                        SectionIICoverage.Address.AddressNum = "1111"
                        SectionIICoverage.Address.StreetName = "ANY"
                        SectionIICoverage.Address.City = "ANY"
                        SectionIICoverage.Address.State = "IN"
                        SectionIICoverage.Address.StateId = "16"
                        SectionIICoverage.Address.Zip = "11111-0000"
                        MyFarmLocation(MyLocationIndex).SectionIICoverages.Add(SectionIICoverage)
                    End If
                Else
                    Dim ResInsured As QuickQuoteSectionIICoverage = MyFarmLocation(MyLocationIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencePremisesOccupiedbyanInsured)
                    If ResInsured IsNot Nothing Then
                        MyFarmLocation(MyLocationIndex).SectionIICoverages.Remove(ResInsured)
                    End If
                End If

                ' Section I Coverages
                ' 70117 - Replacement Cost - Cov C
                'If chkReplaceCost.Checked And MyFarmLocation(MyLocationIndex).FormTypeId <> "13" Then
                'updated 10/1/2021 to make sure replacement cost is never saved for formTypeId 18 (FO5)
                If chkReplaceCost.Checked AndAlso MyFarmLocation(MyLocationIndex).FormTypeId <> "13" AndAlso MyFarmLocation(MyLocationIndex).FormTypeId <> "18" Then
                    If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Replacement_Value_Personal_Property_Cov_C_).Count <= 0 Then
                        SectionICoverage = New QuickQuoteSectionICoverage()
                        SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Replacement_Value_Personal_Property_Cov_C_
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                    End If

                    MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncluded = hiddenCovCLimit.Value
                    MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncreased = hiddenCovCChange.Value
                    MyFarmLocation(MyLocationIndex).C_PersonalProperty_Limit = hiddenCovCTotal.Value
                Else
                    Dim ReplaceCost As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Replacement_Value_Personal_Property_Cov_C_)
                    If ReplaceCost IsNot Nothing Then
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(ReplaceCost)
                    End If

                    MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncluded = hiddenCovCLimit.Value
                    MyFarmLocation(MyLocationIndex).C_PersonalProperty_LimitIncreased = hiddenCovCChange.Value
                    MyFarmLocation(MyLocationIndex).C_PersonalProperty_Limit = hiddenCovCTotal.Value
                End If

                ' 40173 - Actual Cash Value Loss Settlement
                If chkACV.Checked Then
                    If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlement).Count <= 0 Then
                        SectionICoverage = New QuickQuoteSectionICoverage()
                        SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlement
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                    End If
                Else
                    Dim ACV As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlement)
                    If ACV IsNot Nothing Then
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(ACV)
                    End If
                End If

                ' 40142 - Actual Cash Value Loss Settlement - Windstorm or Hail Losses to Roof Surfacing
                If chkACVWind.Checked Then
                    If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing).Count <= 0 Then
                        SectionICoverage = New QuickQuoteSectionICoverage()
                        SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                    End If
                Else
                    Dim ACVWind As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing)
                    If ACVWind IsNot Nothing Then
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(ACVWind)
                    End If
                End If

                ' 70108 - Theft of Building Materials
                If chkTheft.Checked Then
                    If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Dwelling_Under_Construction_Theft).Count <= 0 Then
                        SectionICoverage = New QuickQuoteSectionICoverage()
                        SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Dwelling_Under_Construction_Theft
                        SectionICoverage.IncreasedLimit = txtTheftLimit.Text
                        SectionICoverage.TotalLimit = txtTheftLimit.Text ' Matt A 5-16-2016 - Didn't show a premium unless this was set
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                    Else
                        Dim matrlTheft As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Dwelling_Under_Construction_Theft)
                        matrlTheft.IncreasedLimit = txtTheftLimit.Text
                        matrlTheft.TotalLimit = txtTheftLimit.Text ' Matt A 5-16-2016 - Didn't show a premium unless this was set
                    End If
                Else
                    Dim Theft As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Dwelling_Under_Construction_Theft)
                    If Theft IsNot Nothing Then
                        txtTheftLimit.Text = ""
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(Theft)
                    End If
                End If

                ' 80137 - Earthquake
                If chkEarthquake.Checked Then
                    If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Earthquake_Location).Count <= 0 Then
                        SectionICoverage = New QuickQuoteSectionICoverage()
                        SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Earthquake_Location
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                    End If
                Else
                    Dim earthQuake As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Earthquake_Location)
                    If earthQuake IsNot Nothing Then
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(earthQuake)
                    End If
                End If

                ' 70110 - Expanded Replacement Cost Terms
                If chkExpandReplacement.Checked Then
                    If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Expanded_Replacement_Cost).Count <= 0 Then
                        SectionICoverage = New QuickQuoteSectionICoverage()
                        SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Expanded_Replacement_Cost
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                    End If
                Else
                    Dim expandReplace As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Expanded_Replacement_Cost)
                    If expandReplace IsNot Nothing Then
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(expandReplace)
                    End If
                End If

                ' 40172 - Functional Replacement Cost Loss Settlement
                If chkReplacement.Checked Then
                    If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.FunctionalReplacementCostLossAssessment).Count <= 0 Then
                        SectionICoverage = New QuickQuoteSectionICoverage()
                        SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.FunctionalReplacementCostLossAssessment
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                    End If
                Else
                    Dim Replacement As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.FunctionalReplacementCostLossAssessment)
                    If Replacement IsNot Nothing Then
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(Replacement)
                    End If
                End If
                ' 80102 - Mine Subsidence Cov A / Cov AB (OH)
                Dim hasMineSub As Boolean = chkMineSubsidence.Checked
                ' Logic for mine sub Ohio
                If MyFarmLocation(MyLocationIndex).Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                    Select Case MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(MyFarmLocation(MyLocationIndex).Address.County)
                        Case MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleMandatory
                            ' Mandatory MUST have mine sub
                            hasMineSub = True
                            Exit Select
                        Case MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleOptional
                            ' Eligible - set to the checkbox value
                            Exit Select
                        Case MineSubsidenceHelper.OhioMineSubsidenceType_enum.Ineligible
                            ' Ineligible cannot have mine sub
                            hasMineSub = False
                            Exit Select
                    End Select
                End If

                ' Logic for mine sub IL
                If IFM.VR.Common.Helpers.FARM.ILMineSubsidenceHelper.IsILMineSubsidenceAvailable(Quote) = True Then
                    If MyFarmLocation(MyLocationIndex).Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois Then
                        Select Case MineSubsidenceHelper.GetIllinoisMineSubsidenceTypeByCounty(MyFarmLocation(MyLocationIndex).Address.County)
                            Case MineSubsidenceHelper.IllinoisMineSubsidenceType_enum.EligibleMandatory
                                ' Default has mine sub
                                'hasMineSub = True
                                hasMineSub = chkMineSubsidence.Checked
                                Exit Select
                            Case MineSubsidenceHelper.IllinoisMineSubsidenceType_enum.EligibleOptional
                                ' Eligible - set to the checkbox value
                                ' hasMineSub = chkMineSubsidence.Checked
                                hasMineSub = chkMineSubsidence.Checked
                                Exit Select
                        End Select
                    End If
                End If

                If hasMineSub Then
                    'If chkMineSubsidence.Checked Then
                    'Moved 11/13/18 for multi state since IN uses MineSubsidenceCovA and IL uses MineSubsidenceCovAAndB
                    'If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA).Count <= 0 Then
                    '    SectionICoverage = New QuickQuoteSectionICoverage()
                    '    SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA
                    '    MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                    'End If
                    'Added 10/17/18 for multi state MLW
                    'find if location is IL with non-mine county that has mine subsidence coverage. If found, all IL locations will need mine subsidence coverage
                    If MultiState.General.IsMultistateCapableEffectiveDate(Me.Quote.EffectiveDate) Then
                        Select Case (MyFarmLocation(MyLocationIndex).Address.StateId)
                            Case States.Abbreviations.IL
                                If MineSubsidenceHelper.MineSubCountiesByStateAbbreviation(MyFarmLocation(MyLocationIndex).Address.State).Contains(MyFarmLocation(MyLocationIndex).Address.County.Trim.ToUpper()) Then
                                    'mine county, do not add coverage to all IL counties
                                    If MyFarmLocation(MyLocationIndex).FormTypeId <> "17" Then
                                        Dim LocHasMineSub As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB)
                                        If LocHasMineSub Is Nothing Then
                                            'add coverage
                                            SectionICoverage = New QuickQuoteSectionICoverage()
                                            SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB
                                            MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                                        End If
                                    Else
                                        Dim LocHasMineSub As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB)
                                        If LocHasMineSub IsNot Nothing Then
                                            'remove coverage
                                            SectionICoverage = New QuickQuoteSectionICoverage()
                                            SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB
                                            MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(SectionICoverage)
                                        End If
                                    End If
                                Else
                                    'do not evaluate FO-4 (FormTypeId = 17) forms since they should not have mine sub cov checked ever
                                    If MyFarmLocation(MyLocationIndex).FormTypeId <> "17" Then
                                        Dim LocHasMineSub As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB)
                                        If LocHasMineSub Is Nothing Then
                                            'add coverage
                                            SectionICoverage = New QuickQuoteSectionICoverage()
                                            SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB
                                            MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                                        End If
                                        ''non-mine county with mine subsidence coverage checked, need all IL counties to have mine subsidence coverage
                                        'If SubQuotes IsNot Nothing Then
                                        '    For Each stateQuote As QuickQuoteObject In SubQuotes
                                        '        If stateQuote.Locations IsNot Nothing Then
                                        '            For Each Loc As QuickQuoteLocation In stateQuote.Locations
                                        '                If Loc.Address.State = "IL" Then
                                        '                    'since mine sub cov required for IL mine counties and we are adding to non-mine counties here, might as well add to mine counties as a catch-all in case they *somehow* unchecked the mine county mine sub cov
                                        '                    If Loc.SectionICoverages Is Nothing Then
                                        '                        'need for when the user clicks the clear link on the location, otherwise an exception would be thrown since it clears out the section 1 coverages
                                        '                        Loc.SectionICoverages = New List(Of QuickQuoteSectionICoverage)
                                        '                    End If
                                        '                    Dim LocHasMineSub As QuickQuoteSectionICoverage = Loc.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB)
                                        '                    If Loc.FormTypeId <> "17" Then
                                        '                        'update coverage, find if coverage already exists
                                        '                        If LocHasMineSub Is Nothing Then
                                        '                            'add coverage
                                        '                            SectionICoverage = New QuickQuoteSectionICoverage()
                                        '                            SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB
                                        '                            Loc.SectionICoverages.Add(SectionICoverage)
                                        '                        End If
                                        '                    Else
                                        '                        'not available on FO-4
                                        '                        If LocHasMineSub IsNot Nothing Then
                                        '                            'remove coverage                                                              
                                        '                            Loc.SectionICoverages.Remove(LocHasMineSub)
                                        '                        End If
                                        '                    End If
                                        '                End If
                                        '            Next
                                        '        End If
                                        '    Next
                                        'End If
                                    End If
                                End If
                            Case States.Abbreviations.IN
                                If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA).Count <= 0 Then
                                    SectionICoverage = New QuickQuoteSectionICoverage()
                                    SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA
                                    MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                                End If
                            Case States.Abbreviations.OH
                                ' Ohio uses the A&B mine sub
                                If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB).Count <= 0 Then
                                    SectionICoverage = New QuickQuoteSectionICoverage()
                                    SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB
                                    MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                                End If
                        End Select
                    Else
                        If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA).Count <= 0 Then
                            SectionICoverage = New QuickQuoteSectionICoverage()
                            SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA
                            MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                        End If
                    End If
                Else
                    'Moved 10/19/18 for multi state MLW - moved to case statement because we do not want to remove a mine sub cov from an IL mine county - it's required
                    'Dim MineSub As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA)
                    'If MineSub IsNot Nothing Then
                    '    MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(MineSub)
                    'End If
                    'Added 10/17/18 for multi state MLW
                    'find if location is IL with non-mine county that has mine subsidence coverage. If found, all IL non-mine county locations will need mine subsidence coverage removed
                    If MultiState.General.IsMultistateCapableEffectiveDate(Me.Quote.EffectiveDate) Then
                        Select Case (MyFarmLocation(MyLocationIndex).Address.StateId)
                            Case States.Abbreviations.IL
                                Dim MineSub As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB)
                                If MineSub IsNot Nothing Then
                                    MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(MineSub)
                                End If
                                'End If
                                'Else
                                'Dim LocHasMineSub As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB)
                                'If MineSubsidenceHelper.MineSubCountiesByStateAbbreviation(MyFarmLocation(MyLocationIndex).Address.State).Contains(MyFarmLocation(MyLocationIndex).Address.County.Trim.ToUpper()) Then
                                'mine county - required for IL, do not remove
                                'Else
                                'non-mine county in IL
                                'remove coverage if it already exists
                                'If LocHasMineSub IsNot Nothing Then
                                ''remove coverage                                                              
                                'MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(LocHasMineSub)
                                'End If
                                'End If
                                'do not evaluate FO-4 (FormTypeId = 17) forms since they should not have mine sub cov checked ever
                                'If MyFarmLocation(MyLocationIndex).FormTypeId <> "17" Then
                                'non-mine county with mine subsidence coverage UNchecked, need all IL counties to NOT have mine subsidence coverage
                                'If SubQuotes IsNot Nothing Then
                                '    For Each stateQuote As QuickQuoteObject In SubQuotes
                                '        If stateQuote.Locations IsNot Nothing Then
                                '            For Each Loc As QuickQuoteLocation In stateQuote.Locations
                                '                If Loc.Address.State = "IL" Then
                                '                    If Loc.SectionICoverages Is Nothing Then
                                '                        'need for when the user clicks the clear link on the location, otherwise an exception would be thrown since it clears out the section 1 coverages
                                '                        Loc.SectionICoverages = New List(Of QuickQuoteSectionICoverage)
                                '                    End If
                                '                    Dim LocHasMineSub As QuickQuoteSectionICoverage = Loc.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB)
                                '                    If Loc.FormTypeId <> "17" Then
                                '                        If MineSubsidenceHelper.MineSubCountiesByStateAbbreviation(Loc.Address.State).Contains(Loc.Address.County.Trim.ToUpper()) Then
                                '                            'mine county - required for IL, do not remove
                                '                        Else
                                '                            'non-mine county in IL
                                '                            'remove coverage if it already exists
                                '                            If LocHasMineSub IsNot Nothing Then
                                '                                'remove coverage                                                              
                                '                                Loc.SectionICoverages.Remove(LocHasMineSub)
                                '                            End If
                                '                        End If
                                '                    Else
                                '                        'not available on any FO-4
                                '                        If LocHasMineSub IsNot Nothing Then
                                '                            'remove coverage                                                              
                                '                            Loc.SectionICoverages.Remove(LocHasMineSub)
                                '                        End If
                                '                    End If
                                '                End If
                                '            Next
                                '        End If
                                '    Next
                                'End If
                                'End If
                                'End If
                                Exit Select
                            Case States.Abbreviations.OH
                                ' OH uses mine sub A&B
                                Dim MineSub As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB)
                                If MineSub IsNot Nothing Then
                                    MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(MineSub)
                                End If
                                Exit Select
                            Case Else
                                Dim MineSub As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA)
                                If MineSub IsNot Nothing Then
                                    MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(MineSub)
                                End If
                        End Select
                    Else
                        ' Pre-multistate effective date
                        Dim MineSub As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA)
                        If MineSub IsNot Nothing Then
                            MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(MineSub)
                        End If
                    End If
                End If

                ' 554 - Related Private Structures
                'If chkRPS.Checked Then
                '    If MyFarmLocation(MyLocationIndex).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_PrivateStructures_IncreasedLimitsForSpecificStructures).Count <= 0 Then
                '        SectionICoverage = New QuickQuoteSectionICoverage()
                '        SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_PrivateStructures_IncreasedLimitsForSpecificStructures
                '        SectionICoverage.Description = "Description"
                '        MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                '    End If
                'Else
                '    Dim RPS As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_PrivateStructures_IncreasedLimitsForSpecificStructures)
                '    If RPS IsNot Nothing Then
                '        MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(RPS)
                '    End If
                'End If

                ' Cosmetic Damage Exclusion
                If chkCosmeticDamageExclusion.Checked Then
                    
                    SectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Cosmetic_Damage_Exclusion)
                    If SectionICoverage Is Nothing Then
                        SectionICoverage = New QuickQuoteSectionICoverage()
                        SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Cosmetic_Damage_Exclusion
                        SectionICoverage.Description = "Cosmetic Damage Exclusion"
                        If CosmeticDamageExHelper.IsCosmeticDamageExAvailable(Quote) Then
                            'Do Nothing to keep current settings

                            'SectionICoverage.ExteriorDoorWindowSurfacing = False
                            'SectionICoverage.ExteriorWallSurfacing = False
                            'SectionICoverage.RoofSurfacing = False
                        Else
                            SectionICoverage.ExteriorDoorWindowSurfacing = chkCDEExteriorDoorAndWindowSurfacing.Checked
                            SectionICoverage.ExteriorWallSurfacing = chkCDEExteriorWallSurfacing.Checked
                            SectionICoverage.RoofSurfacing = chkCDERoofSurfacing.Checked
                        End If
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                    Else
                        If CosmeticDamageExHelper.IsCosmeticDamageExAvailable(Quote) Then
                            'Do Nothing to keep current settings

                            'SectionICoverage.ExteriorDoorWindowSurfacing = False
                            'SectionICoverage.ExteriorWallSurfacing = False
                            'SectionICoverage.RoofSurfacing = False
                        Else
                            SectionICoverage.ExteriorDoorWindowSurfacing = chkCDEExteriorDoorAndWindowSurfacing.Checked
                            SectionICoverage.ExteriorWallSurfacing = chkCDEExteriorWallSurfacing.Checked
                            SectionICoverage.RoofSurfacing = chkCDERoofSurfacing.Checked
                        End If
                    End If
                Else
                    Dim RPS As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Cosmetic_Damage_Exclusion)
                    If RPS IsNot Nothing Then
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(RPS)
                    End If
                End If

                ' 80507 - Underground Service Line
                If chkUndergroundServiceLine.Checked Then
                    SectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.UndergroundServiceLine)
                    If SectionICoverage Is Nothing Then
                        SectionICoverage = New QuickQuoteSectionICoverage()
                        SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.UndergroundServiceLine
                        SectionICoverage.Description = "Underground Service Line"
                        SectionICoverage.IncludedLimit = txtUndergroundServiceLineLimit.Text
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                    Else
                        SectionICoverage.IncludedLimit = txtUndergroundServiceLineLimit.Text
                    End If
                Else
                    Dim RPS As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.UndergroundServiceLine)
                    If RPS IsNot Nothing Then
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(RPS)
                    End If
                End If

                ' Motorized Vehicles Increased Limits
                If chkMotorizedVehiclesIncreasedLimit.Checked Then
                    SectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Motorized_Vehicles_Increased_Limits)
                    If SectionICoverage Is Nothing Then
                        SectionICoverage = New QuickQuoteSectionICoverage()
                        SectionICoverage.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Motorized_Vehicles_Increased_Limits
                        SectionICoverage.Description = "Motorized Vehicle Limits"
                        SectionICoverage.IncludedLimit = txtMotorizedIncludedLimit.Text
                        SectionICoverage.IncreasedLimit = txtMotorizedIncreaseLimit.Text
                        SectionICoverage.TotalLimit = txtMotorizedTotalLimit.Text
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Add(SectionICoverage)
                    Else
                        SectionICoverage.IncludedLimit = txtMotorizedIncludedLimit.Text
                        SectionICoverage.IncreasedLimit = txtMotorizedIncreaseLimit.Text
                        SectionICoverage.TotalLimit = txtMotorizedTotalLimit.Text
                    End If
                Else
                    Dim motorizedVehLim As QuickQuoteSectionICoverage = MyFarmLocation(MyLocationIndex).SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Farm_Motorized_Vehicles_Increased_Limits)
                    If motorizedVehLim IsNot Nothing Then
                        MyFarmLocation(MyLocationIndex).SectionICoverages.Remove(motorizedVehLim)
                    End If
                End If

                ' Private Power Poles
                If Common.Helpers.FARM.PrivatePowerPolesHelper.IsPrivatePowerPolesAvailable(Quote) Then
                    If chkPrivatePowerPoles.Checked Then
                        ' 70145 - Private Power Poles
                        Dim PrivatePowerPoles As QuickQuoteCoverage
                        If MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages Is Nothing Then
                            MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages = New List(Of QuickQuoteCoverage)
                        End If
                        If MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.FindAll(Function(p) p.CoverageCodeId = "70145").Count > 0 Then
                            PrivatePowerPoles = MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.Find(Function(p) p.CoverageCodeId = "70145")
                            PrivatePowerPoles.ManualLimitIncreased = txtPrivatePowerPolesIncreaseLimit.Text
                            PrivatePowerPoles.ManualLimitIncluded = txtPrivatePowerPolesIncludedLimit.Text
                            PrivatePowerPoles.ManualLimitAmount = txtPrivatePowerPolesTotalLimit.Text
                        Else
                            PrivatePowerPoles = New QuickQuoteCoverage()
                            PrivatePowerPoles.CoverageCodeId = "70145"
                            PrivatePowerPoles.ManualLimitIncreased = txtPrivatePowerPolesIncreaseLimit.Text
                            PrivatePowerPoles.ManualLimitIncluded = txtPrivatePowerPolesIncludedLimit.Text
                            PrivatePowerPoles.ManualLimitAmount = txtPrivatePowerPolesTotalLimit.Text
                            MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.Add(PrivatePowerPoles)

                            txtPrivatePowerPolesIncreaseLimit.Text = Format(PrivatePowerPoles.ManualLimitIncreased.TryToGetInt32, "N0")
                            txtPrivatePowerPolesTotalLimit.Text = Format(PrivatePowerPoles.ManualLimitAmount.TryToGetInt32, "N0")
                        End If
                    End If
                    If MyFarmLocation(MyLocationIndex).FormTypeId = "17" Then
                        Dim privatePowerPoles As QuickQuoteCoverage = MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.Find(Function(p) p.CoverageCodeId = "70145")
                        If privatePowerPoles IsNot Nothing Then
                            MyFarmLocation(MyLocationIndex).IncidentalDwellingCoverages.Remove(privatePowerPoles)
                        End If
                    End If
                End If

                If MyFarmLocation(0).SectionICoverages Is Nothing Then
                    MyFarmLocation(0).SectionICoverages = New List(Of QuickQuoteSectionICoverage)
                End If

                SaveChildControls()
                Populate()
            End If
        End If

        Return True
    End Function

    Protected Sub lnkDCSave_Click(sender As Object, e As EventArgs) Handles lnkDCSave.Click, lnkRCPSave.Click, lnkRCSave.Click
        'Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        Session("valuationValue") = "False"
        SaveQuote(True)
    End Sub

    Protected Sub lnkDCClear_Click(sender As Object, e As EventArgs) Handles lnkDCClear.Click
        chkAddlResidence.Checked = False
        Session("valuationValue") = "False"

        Me.ctlFARAddlResidence.ClearControl()

        Try
            Dim checkButton = CType(sender, LinkButton).ID
            'Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            SaveQuote(False)
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub lnkRCPClear_Click(sender As Object, e As EventArgs) Handles lnkRCPClear.Click
        Session("valuationValue") = "False"
        If Not chkReplaceCost.Checked Then
            chkReplaceCost.Checked = True
        End If

        chkACV.Checked = False
        chkACVWind.Checked = False
        chkTheft.Checked = False
        txtTheftLimit.Text = ""
        chkEarthquake.Checked = False
        chkExpandReplacement.Checked = False
        chkReplacement.Checked = False
        'Updated 11/6/18 for multi state MLW
        Select Case (MyFarmLocation(MyLocationIndex).Address.StateId)
            Case States.Abbreviations.IL
                If MineSubsidenceHelper.MineSubCountiesByStateAbbreviation(MyFarmLocation(MyLocationIndex).Address.State).Contains(MyFarmLocation(MyLocationIndex).Address.County.Trim.ToUpper()) Then
                    'keep the same, do not uncheck
                    chkMineSubsidence.Checked = False
                End If
            Case States.Abbreviations.IN
                chkMineSubsidence.Checked = False
            Case Else
                chkMineSubsidence.Checked = False
        End Select
        chkRPS.Checked = False

        Try
            Dim checkButton = CType(sender, LinkButton).ID
            'Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            SaveQuote(False)
        Catch ex As Exception

        End Try
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        lnkDCClear_Click(Me, New EventArgs)
        lnkRCPClear_Click(Me, New EventArgs)
        lnkRCClear_Click(Me, New EventArgs)
    End Sub

    Public Sub DisplayCoverages(show As Boolean)
        If show Then
            dvResidenceCoverage.Attributes.Add("style", "display:block;")
        Else
            dvResidenceCoverage.Attributes.Add("style", "display:none;")
        End If
    End Sub

    Protected Sub lnkRCClear_Click(sender As Object, e As EventArgs) Handles lnkRCClear.Click
        Session("valuationValue") = "False"
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlDeductible, "0")
        txtDwLimit.Text = "0"
        txtDwellingChangeInLimit.Text = "0"
        txtDwellingTotalLimit.Text = "0"
        txtRPSLimit.Text = "0"
        txtRPSChgInLimit.Text = "0"
        txtRPSTotalLimit.Text = "0"
        txtPPLimit.Text = "0"
        txtPPChgInLimit.Text = "0"
        txtPPTotalLimit.Text = "0"
        txtLossLimit.Text = "0"
        txtLossChgInLimit.Text = "0"
        txtLossTotalLimit.Text = "0"
        txtMotorizedIncreaseLimit.Text = "0"

        hiddenCovALimit.Value = "0"
        hiddenCovAChange.Value = "0"
        hiddenCovATotal.Value = "0"
        hiddenCovBLimit.Value = "0"
        hiddenCovBChange.Value = "0"
        hiddenCovBTotal.Value = "0"
        hiddenCovCLimit.Value = "0"
        hiddenCovCChange.Value = "0"
        hiddenCovCTotal.Value = "0"
        hiddenCovDLimit.Value = "0"
        hiddenCovDChange.Value = "0"
        hiddenCovDTotal.Value = "0"

        lnkDCClear_Click(Me, New EventArgs)
        lnkRCPClear_Click(Me, New EventArgs)

        MyFarmLocation(MyLocationIndex).SectionICoverages = Nothing
        'Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        SaveQuote(False)
        ToggleCoverageByForm()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = "Farm Location Dwellings Coverage"
        Dim divCoverages As String = dvResidenceCoverage.ClientID

        Dim valList = LocationCoverageValidator.ValidateFARLocation(Quote, MyLocationIndex, valArgs.ValidationType)
        trCDEValidationMessage.Attributes.Add("style", "display:none")

        If valList.Any() Then
            For Each v In valList
                ' *********************
                ' Base Policy Coverages
                ' *********************
                Select Case v.FieldId
                    Case LocationCoverageValidator.TheftLimitRequired
                        ValidationHelper.Val_BindValidationItemToControl(txtTheftLimit, v, divCoverages, "0")
                    Case LocationCoverageValidator.MissingDeductibleAD
                        ValidationHelper.Val_BindValidationItemToControl(ddlDeductible, v, divCoverages, "0")
                    Case LocationCoverageValidator.CovARequired
                        ValidationHelper.Val_BindValidationItemToControl(txtDwLimit, v, divCoverages, "0")
                    Case LocationCoverageValidator.CovCRequired
                        ValidationHelper.Val_BindValidationItemToControl(txtPPLimit, v, divCoverages, "0")
                    Case LocationCoverageValidator.CosmeticDamageExclusionOptionRequired
                        ValidationHelper.Val_BindValidationItemToControl(New Control, v, divCoverages, "0")
                        trCDEValidationMessage.Attributes.Add("style", "display:block")
                    Case LocationCoverageValidator.FRCandType1Dwelling
                        If IsQuoteEndorsement() = False OrElse (IsQuoteEndorsement() AndAlso IsPreexistingLocationOnEndorsement(MyLocationIndex) = False) Then
                            ValidationHelper.Val_BindValidationItemToControl(txtTheftLimit, v, divCoverages, "0")
                        End If
                        Exit Select
                End Select
            Next
        End If

        ValidateChildControls(valArgs)
        PopulateChildControls()
    End Sub

    Protected Sub btnReplacementCC_Click(sender As Object, e As EventArgs) Handles btnReplacementCC.Click
        Dim errMsg As String = ""
        Dim wasSaveSuccessful As Boolean = False
        Dim valuationUrl As String = ""

        'Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        SaveQuote(False)
        Dim loc As Integer = 1

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then

            loc = Me.MyLocationIndex + 1
        End If

        Dim chc As New CommonHelperClass
        If chc.ConfigurationAppSettingValueAsBoolean("VR_AllLines_Site360Valuation_Settings") = True Then
            pvHelper.PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(Quote, valuationUrl, loc, True, wasSaveSuccessful, errMsg, valuationVendor:=QuickQuotePropertyValuation.ValuationVendor.Verisk360)
        Else
            pvHelper.PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(Quote, valuationUrl, loc, True, wasSaveSuccessful, errMsg, valuationVendor:=QuickQuotePropertyValuation.ValuationVendor.e2Value)
        End If

        If valuationUrl <> "" Then

            If Not wasSaveSuccessful Then
                'Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
                SaveQuote(False)
            End If

            Session("valuationValue") = "True"
            ' Need to clear session quote because it is going to change while on e2value side (basically) so you will want to reload session variable once you get back
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
            Else
                VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId) ' Matt A - 12-9-14
            End If

            Response.Redirect(valuationUrl, False)
        Else
            If errMsg = "" Then 'should have something if it gets here... ELSE will have already redirected to e2Value
                errMsg = "Problem Initiating Valuation Request"
            End If

            ShowError(errMsg)
        End If
    End Sub

    Private Sub ShowError(ByVal message As String, Optional ByVal redirect As Boolean = False, Optional ByVal redirectPage As String = "")
        message = Replace(message, "\", "\\")
        message = Replace(message, "<br>", "\n")
        message = Replace(message, vbCrLf, "\n")

        Dim strScript As String = "<script language=JavaScript>"
        strScript &= "alert(""" & message & """);"
        If redirect = True Then
            If redirectPage = "" Then
                redirectPage = "MyVelociRater.aspx" 'use config key if available
            End If
            strScript &= " window.location.href='" & redirectPage & "';"
        End If
        strScript &= "</script>"

        Page.RegisterStartupScript("clientScript", strScript)

    End Sub

    Private Sub SaveQuote(validate As Boolean)
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, validate, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub

End Class