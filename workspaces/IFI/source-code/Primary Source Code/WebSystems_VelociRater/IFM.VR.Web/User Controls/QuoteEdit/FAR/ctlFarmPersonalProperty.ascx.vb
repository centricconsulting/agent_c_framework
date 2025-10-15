Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.FarmLines
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports IFM.PrimativeExtensions

Public Class ctlFarmPersonalProperty
    Inherits VRControlBase

    Public Event RequestIMNavigation()
    Public Event RatePersonalProperty()

    Private Const DefaultGlassBreakageForCabsIncludedLimit = "500"

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

    Public ReadOnly Property MyFarmLocation As List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations
            End If
            Return Nothing
        End Get
    End Property

    Public Property BlanketAccordionDivId As String
        Get
            If ViewState("vs_BlanketAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_BlanketAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_BlanketAccordionDivId_") = value
        End Set
    End Property

    Public Property OptionalAccordionDivId As String
        Get
            If ViewState("vs_OptionalAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_OptionalAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_OptionalAccordionDivId_") = value
        End Set
    End Property

    Public Property FarmIncidentalsAccordionDivId As String
        Get
            If ViewState("vs_FarmIncidentalsAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_FarmIncidentalsAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_FarmIncidentalsAccordionDivId_") = value
        End Set
    End Property

    Private _ControlToClear As String
    Public Property ControlToClear() As String
        Get
            Return _ControlToClear
        End Get
        Set(ByVal value As String)
            _ControlToClear = value
        End Set
    End Property

    Private _PeakButtonPress As String
    Public Property PeakSeasonButtonPress() As String
        Get
            Return _PeakButtonPress
        End Get
        Set(ByVal value As String)
            _PeakButtonPress = value
        End Set
    End Property

    Public Property RowNumber As Int32
        Get
            If ViewState("vs_rowNumber") Is Nothing Then
                ViewState("vs_rowNumber") = 0
            End If
            Return CInt(ViewState("vs_rowNumber"))
        End Get
        Set(value As Int32)
            ViewState("vs_rowNumber") = value
        End Set
    End Property

    Public ReadOnly Property FarmMachinerySpecialCoverageGClientID As String
        Get
            Return chkFarmMachinery.ClientID
        End Get
    End Property

    Public ReadOnly Property lblFarmMachinerySpecialCoverageGClientID As String
        Get
            Return lblFarmMachinery.ClientID
        End Get
    End Property

    Public ReadOnly Property MoreLessCoverages() As List(Of String)
        Get
            Dim moreLessCoverageList As List(Of String) = New List(Of String)(New String() _
                                        {QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.LocationF_MachineryNotDescribed,
                                         QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Grain,
                                         QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.GrainintheOpen,
                                         QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_Barn,
                                         QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_the_Open,
                                         QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.ReproductiveMaterials})
            Return moreLessCoverageList
        End Get
    End Property

    Private Sub SetCSS(myControl As HtmlGenericControl, CSSItem As String, CSSSetting As String)
        Dim a As String = CSSSetting.Split(":")(0)
        If myControl.Attributes.Item(CSSItem) IsNot Nothing AndAlso myControl.Attributes.Item(CSSItem).Contains(a) Then
            myControl.Attributes.Item(CSSItem) = CSSSetting
        Else
            myControl.Attributes.Add(CSSItem, CSSSetting)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            MainAccordionDivId = dvFarmPersPropDeduct.ClientID
            ListAccordionDivId = dvFarmPersPropSched.ClientID
            BlanketAccordionDivId = dvFarmBlanket.ClientID
            OptionalAccordionDivId = dvFarmOptional.ClientID
            FarmIncidentalsAccordionDivId = dvFarmIncidentalLimits.ClientID

            LoadStaticData()
            Populate()
            Me.divActionButtons.Visible = Not IsOnAppPage
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.CreateAccordion(MainAccordionDivId, hiddenFarmPPCoverage, "0", False)
        VRScript.CreateAccordion(ListAccordionDivId, hiddenPPSched, "0")
        VRScript.CreateAccordion(BlanketAccordionDivId, hiddenFarmBlanket, "0")
        VRScript.CreateAccordion(OptionalAccordionDivId, hiddenOptional, "0")
        VRScript.CreateAccordion(FarmIncidentalsAccordionDivId, HiddenFarmIncidentalLimits, "0")


        VRScript.StopEventPropagation(lnkClearPersProp.ClientID, False)
        VRScript.StopEventPropagation(lnkSavePersProp.ClientID, False)
        VRScript.StopEventPropagation(lnkClearPersPropSched.ClientID, False)
        VRScript.StopEventPropagation(lnkSavePersPropSched.ClientID, False)
        VRScript.StopEventPropagation(lnkSaveBlanket.ClientID, False)
        VRScript.StopEventPropagation(lnkClearOptional.ClientID, False)
        VRScript.StopEventPropagation(lnkSaveOptional.ClientID, False)
        VRScript.StopEventPropagation(lnkClearFarmIncidental.ClientID, False)
        VRScript.StopEventPropagation(lnkSaveFarmIncidental.ClientID, False)

        Me.VRScript.AddScriptLine("ifm.vr.ui.ElementDisabler($('#" + txtGlassBreakageIncludedLimit.ClientID + "'));", onlyAllowOnce:=True)
        Me.VRScript.AddScriptLine("ifm.vr.ui.ElementDisabler($('#" + txtGlassBreakageTotalLimit.ClientID + "'));", onlyAllowOnce:=True)
        Me.VRScript.AddScriptLine("ifm.vr.ui.ElementDisabler($('#" + chkGlassBreakageCabs.ClientID + "'));", onlyAllowOnce:=True)

        Dim scriptGlassCabMath As String = "UpdateGenericPersonalPropertyControl(""" + txtGlassBreakageIncludedLimit.ClientID + """,""" + txtGlassBreakageIncreaseLimit.ClientID + """,""" + txtGlassBreakageTotalLimit.ClientID + """);"
        Me.VRScript.CreateJSBinding(txtGlassBreakageIncreaseLimit.ClientID, "blur", scriptGlassCabMath)

        Dim scriptGlassCabFilter As String = "$(this).val(FormatAsNumberNoCommaFormatting($(this).val()));"
        Me.VRScript.CreateJSBinding(txtGlassBreakageIncreaseLimit.ClientID, "keyup", scriptGlassCabFilter)

        Dim scriptGlassCabCheck As String = "ToggleGenericPersonalPropertyControl(""" + tblGlassBreakageCabs.ClientID + """, """ + chkGlassBreakageCabs.ClientID + """, """ + txtGlassBreakageIncreaseLimit.ClientID + """);"
        Me.VRScript.CreateJSBinding(chkGlassBreakageCabs.ClientID, "change", scriptGlassCabCheck + scriptGlassCabMath)


        Dim scriptFarmMachinerySpecialConverageG As String = "ToggleFarmMachinerySpecialCoverageG(""" + ctlBlnktPersProp.GetBlanketLimitClientID + """, """ + chkFarmMachinery.ClientID + """, """ + lblFarmMachinery.ClientID + """);"
        VRScript.AddScriptLine(scriptFarmMachinerySpecialConverageG)

        ' Toggle More/Less
        Dim scriptMoreLessToggle As String = "ToggleFarmMoreLess(""" + dvMoreLess.ClientID + """, """ + lnkMoreLess.ClientID + """, """ + hiddenMoreLess.ClientID + """, """ + hiddenMoreLessCnt.ClientID + """); return false;"
        lnkMoreLess.Attributes.Add("onclick", scriptMoreLessToggle)

        ' Toggle Sheep Additional Perils Limit
        Dim scriptSheepPerilsToggle As String = "ToggleSheepPerilsLimit(""" + dvSheepPerilsLimit.ClientID + """, """ + chkSheepPerils.ClientID + """, """ + txtSheep_LimitData.ClientID + """);"
        chkSheepPerils.Attributes.Add("onclick", scriptSheepPerilsToggle)

        Dim scriptLimitData As String = "FarmRoundValue(""" + txtSheep_LimitData.ClientID + """);"
        txtSheep_LimitData.Attributes.Add("onblur", scriptLimitData)

        txtSheep_LimitData.Attributes.Add("onfocus", "this.select()")

        Dim peakExists As Boolean = False
        'Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
        'Updated 9/6/18 for multi state MLW - Quote to SubQuoteFirst
        'If SubQuoteFirst IsNot Nothing Then
        If GoverningStateQuote() IsNot Nothing Then
            'If SubQuoteFirst.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
            If GoverningStateQuote.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                'If SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
                If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
                    peakExists = True
                End If
            End If
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(ddlPPDeduct, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.Farm_F_and_G_DeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
    End Sub

    Private Sub ToggleMoreLess() Handles ctlLivestock.ToggleExpandMoreLess, ctlHayBuilding.ToggleExpandMoreLess, ctlHayOpen.ToggleExpandMoreLess, ctlGrainOpen.ToggleExpandMoreLess, ctlGrainBuilding.ToggleExpandMoreLess
        If hiddenMoreLess.Value <> "expanded" Then
            dvMoreLess.Attributes.Add("style", "display:none;")

            GetSelectedMoreLess()

            lnkMoreLess.Text = "+More (" & hiddenMoreLessCnt.Value & ")"
            hiddenMoreLess.Value = "collapsed"
        Else
            dvMoreLess.Attributes.Add("style", "display:block;")
            lnkMoreLess.Text = "-Less"
            hiddenMoreLess.Value = "expanded"
        End If
    End Sub

    Private Sub GetSelectedMoreLess()
        Dim distinctCoverageCnt = 0
        'Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
        If GoverningStateQuote() IsNot Nothing Then
            'Updated 9/6/18 for multi state MLW - Quote to SubQuoteFirst
            'If SubQuoteFirst.ScheduledPersonalPropertyCoverages IsNot Nothing Then
            If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                'Dim distinctCoverage = From coverage In SubQuoteFirst.ScheduledPersonalPropertyCoverages
                '                       Select coverage.CoverageType
                '                       Distinct
                Dim distinctCoverage = From coverage In GoverningStateQuote.ScheduledPersonalPropertyCoverages
                                       Select coverage.CoverageType
                                       Distinct

                For Each item In distinctCoverage
                    If MoreLessCoverages.Contains(item) Then
                        distinctCoverageCnt = distinctCoverageCnt + 1
                    End If
                Next
            End If
        End If

        hiddenMoreLessCnt.Value = distinctCoverageCnt
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        ToggleMoreLess()
        ToggleEndorsementRequirements()

        If Quote IsNot Nothing Then
            'Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
            If GoverningStateQuote() IsNot Nothing Then
                'Updated 9/6/18 for multi state MLW - Quote to SubQuoteFirst
                'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlPPDeduct, SubQuoteFirst.Farm_F_and_G_DeductibleLimitId)
                'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlPPDeduct, GoverningStateQuote.Farm_F_and_G_DeductibleLimitId)

                IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlPPDeduct, GoverningStateQuote.Farm_F_and_G_DeductibleLimitId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.Farm_F_and_G_DeductibleLimitId)

                'If QQHelper.IsPositiveIntegerString(GoverningStateQuote.Farm_F_and_G_DeductibleLimitId) AndAlso ddlPPDeduct.Items.FindByValue(GoverningStateQuote.Farm_F_and_G_DeductibleLimitId) Is Nothing Then
                '    Dim TypeDescription As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.Farm_F_and_G_DeductibleLimitId, GoverningStateQuote.Farm_F_and_G_DeductibleLimitId)
                '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(Me.ddlPPDeduct, GoverningStateQuote.Farm_F_and_G_DeductibleLimitId, TypeDescription)
                'Else
                '    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlPPDeduct, GoverningStateQuote.Farm_F_and_G_DeductibleLimitId)
                'End If



                'If SubQuoteFirst.OptionalCoverages IsNot Nothing Then
                If GoverningStateQuote.OptionalCoverages IsNot Nothing Then
                    ' 4H and FFA Animals
                    'If SubQuoteFirst.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_4_H_and_FFAAnimals).Count > 0 Then
                    If GoverningStateQuote.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_4_H_and_FFAAnimals).Count > 0 Then
                        ctlAnimals.PersPropType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_4_H_and_FFAAnimals
                        ctlAnimals.Populate()
                        chkAnimals.Checked = True
                        chkAnimals.Enabled = False
                        dvAnimalsLimit.Attributes.Add("style", "display:block;")
                    Else
                        chkAnimals.Checked = False
                        chkAnimals.Enabled = True
                        dvAnimalsLimit.Attributes.Add("style", "display:none;")
                    End If

                    ' Sheep Additional Perils
                    'If SubQuoteFirst.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep).Count > 0 Then
                    If GoverningStateQuote.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep).Count > 0 Then
                        'Dim sheepPerils As QuickQuoteOptionalCoverage = SubQuoteFirst.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep)
                        Dim sheepPerils As QuickQuoteOptionalCoverage = GoverningStateQuote.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep)
                        chkSheepPerils.Checked = True
                        txtSheep_LimitData.Text = sheepPerils.IncreasedLimit
                        dvSheepPerilsLimit.Attributes.Add("style", "display:block;")
                    Else
                        dvSheepPerilsLimit.Attributes.Add("style", "display:none;")
                    End If

                    ' Property In Transit
                    If GoverningStateQuote.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Property_in_Transit).Count > 0 Then
                        'Dim sheepPerils As QuickQuoteOptionalCoverage = SubQuoteFirst.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep)
                        chkPropertyInTransit.Checked = True
                    Else
                        chkPropertyInTransit.Checked = False
                    End If
                Else
                    chkAnimals.Checked = False
                    chkAnimals.Enabled = True
                    dvAnimalsLimit.Attributes.Add("style", "display:none;")
                    chkSheepPerils.Checked = False
                    chkSheepPerils.Enabled = True
                    dvSheepPerilsLimit.Attributes.Add("style", "display:none;")
                    chkFarmMachinery.Checked = False
                    chkPropertyInTransit.Checked = False
                End If

                'If SubQuoteFirst.ScheduledPersonalPropertyCoverages IsNot Nothing AndAlso SubQuoteFirst.ScheduledPersonalPropertyCoverages.Count > 0 Then
                If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing AndAlso GoverningStateQuote.ScheduledPersonalPropertyCoverages.Count > 0 Then
                    Try
                        If PeakSeasonButtonPress IsNot Nothing Then
                            Dim splitPeakButton As List(Of String) = New List(Of String)(PeakSeasonButtonPress.Split("!"c))
                            'Dim filterPeakSeason As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType.ToString() = splitPeakButton(1).ToString())
                            Dim filterPeakSeason As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType.ToString() = splitPeakButton(1).ToString())

                            If filterPeakSeason(RowNumber).PeakSeasons IsNot Nothing AndAlso filterPeakSeason(RowNumber).PeakSeasons.Count > 0 Then
                                'filterPeakSeason(RowNumber).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, filterPeakSeason, New QuickQuoteUnscheduledPersonalPropertyCoverage, RowNumber)
                                filterPeakSeason(RowNumber).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(filterPeakSeason, New QuickQuoteUnscheduledPersonalPropertyCoverage, RowNumber, Quote.QuoteTransactionType)
                            End If
                        Else

                            'If SubQuoteFirst.ScheduledPersonalPropertyCoverages(RowNumber).PeakSeasons IsNot Nothing AndAlso SubQuoteFirst.ScheduledPersonalPropertyCoverages(RowNumber).PeakSeasons.Count > 0 Then
                            If GoverningStateQuote.ScheduledPersonalPropertyCoverages(RowNumber).PeakSeasons IsNot Nothing AndAlso GoverningStateQuote.ScheduledPersonalPropertyCoverages(RowNumber).PeakSeasons.Count > 0 Then
                                'For covIdx As Integer = 0 To SubQuoteFirst.ScheduledPersonalPropertyCoverages.Count - 1
                                For covIdx As Integer = 0 To GoverningStateQuote.ScheduledPersonalPropertyCoverages.Count - 1
                                    If covIdx = RowNumber Then
                                        'SubQuoteFirst.ScheduledPersonalPropertyCoverages(covIdx).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, SubQuoteFirst.ScheduledPersonalPropertyCoverages, New QuickQuoteUnscheduledPersonalPropertyCoverage, covIdx)
                                        'GoverningStateQuote.ScheduledPersonalPropertyCoverages(covIdx).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, SubQuoteFirst.ScheduledPersonalPropertyCoverages, New QuickQuoteUnscheduledPersonalPropertyCoverage, covIdx)
                                        GoverningStateQuote.ScheduledPersonalPropertyCoverages(covIdx).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(SubQuoteFirst.ScheduledPersonalPropertyCoverages, New QuickQuoteUnscheduledPersonalPropertyCoverage, covIdx, Quote.QuoteTransactionType)
                                    End If
                                Next
                            End If
                        End If
                    Catch ex As Exception

                    End Try

                    ' Farm Machinery - Described
                    'If SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.LocationF_DescribedMachinery).Count > 0 Then
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.LocationF_DescribedMachinery).Count > 0 Then
                        ctlMachineDescribed.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.LocationF_DescribedMachinery
                        'ctlMachineDescribed.EarthquakeState = chkSchedEarth.Checked
                        ctlMachineDescribed.Populate()
                        chkMachineryDescribed.Checked = True
                        chkMachineryDescribed.Enabled = False
                        dvMachineryDescribedLimit.Attributes.Add("style", "display:block;")
                    End If

                    ' Farm Machinery - Described - Open Perils
                    'If SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.FarmMachineryDescribed_OpenPerils).Count > 0 Then
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.FarmMachineryDescribed_OpenPerils).Count > 0 Then
                        ctlMachineOpen.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.FarmMachineryDescribed_OpenPerils
                        ctlMachineOpen.Populate()
                        chkMachineryOpen.Checked = True
                        chkMachineryOpen.Enabled = False
                        dvMachineryOpenLimit.Attributes.Add("style", "display:block;")
                    End If

                    ' Irrigation
                    'If SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Irrigation_Equipment).Count > 0 Then
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Irrigation_Equipment).Count > 0 Then
                        ctlIrrigationEquip.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Irrigation_Equipment
                        ctlIrrigationEquip.Populate()
                        chkIrrigation.Checked = True
                        chkIrrigation.Enabled = False
                        dvIrrigationLimit.Attributes.Add("style", "display:block;")
                    End If

                    ' Livestock
                    'If SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Livestock).Count > 0 Then
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Livestock).Count > 0 Then
                        ctlLivestock.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Livestock
                        ctlLivestock.Populate()
                        chkLivestock.Checked = True
                        chkLivestock.Enabled = False
                        dvLivestockLimit.Attributes.Add("style", "display:block;")
                        chkSheepPerils.Enabled = True
                    End If

                    ' Miscellaneous Farm Personal Property
                    If IFM.VR.Common.Helpers.FARM.MiscFarmPersPropHelper.IsMiscFarmPersPropAvailable(Quote) Then
                        dvMiscFarmPersProperty.Attributes.Add("style", "display:block;")
                        If GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.MiscellaneousFarmPersonalProperty).Count > 0 Then
                            ctlMiscFarmPersonalProperty.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.MiscellaneousFarmPersonalProperty
                            ctlMiscFarmPersonalProperty.Populate()
                            chkMiscFarmPersProperty.Checked = True
                            chkMiscFarmPersProperty.Enabled = False
                            dvMiscFarmPersPropertyLimit.Attributes.Add("style", "display:block;")
                        End If
                    Else
                        dvMiscFarmPersProperty.Attributes.Add("style", "display:none;")
                        chkMiscFarmPersProperty.Checked = False
                        chkMiscFarmPersProperty.Enabled = True
                        dvMiscFarmPersPropertyLimit.Attributes.Add("style", "display:none;")
                    End If


                    ' Rented or Borrowed Equipment
                    'If SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_Rented_or_borrowed_Equipment).Count > 0 Then
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_Rented_or_borrowed_Equipment).Count > 0 Then
                        ctlBorrowed.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_Rented_or_borrowed_Equipment
                        ctlBorrowed.Populate()
                        chkBorrowed.Checked = True
                        chkBorrowed.Enabled = False
                        dvBorrowedLimit.Attributes.Add("style", "display:block;")
                        lnkAddBorrowed.Visible = False 'setting this always to be just one item, not multiple, and not just when Farm Extender applies
                    End If

                    ' Farm Machinery - Not Described
                    'If SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.LocationF_MachineryNotDescribed).Count > 0 Then
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.LocationF_MachineryNotDescribed).Count > 0 Then
                        ctlMachineNotDescribed.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.LocationF_MachineryNotDescribed
                        ctlMachineNotDescribed.Populate()
                        chkMachineryNotDescribed.Checked = True
                        chkMachineryNotDescribed.Enabled = False
                        dvMachineryNotDescribedLimit.Attributes.Add("style", "display:block;")
                    End If

                    ' Grain in Buildings
                    'If SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Grain).Count > 0 Then
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Grain).Count > 0 Then
                        ctlGrainBuilding.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Grain
                        ctlGrainBuilding.Populate()
                        chkGrainBuild.Checked = True
                        chkGrainBuild.Enabled = False
                        dvGrainBuildLimit.Attributes.Add("style", "display:block;")
                    End If

                    ' Grain in the Open
                    'If SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.GrainintheOpen).Count > 0 Then
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.GrainintheOpen).Count > 0 Then
                        ctlGrainOpen.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.GrainintheOpen
                        ctlGrainOpen.Populate()
                        chkGrainOpen.Checked = True
                        chkGrainOpen.Enabled = False
                        dvGrainOpenLimit.Attributes.Add("style", "display:block;")
                    End If

                    ' Hay in Buildings
                    'If SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_Barn).Count > 0 Then
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_Barn).Count > 0 Then
                        ctlHayBuilding.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_Barn
                        ctlHayBuilding.Populate()
                        chkHayBuild.Checked = True
                        chkHayBuild.Enabled = False
                        dvHayBuildLimit.Attributes.Add("style", "display:block;")
                    End If

                    ' Hay in the Open
                    'If SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_the_Open).Count > 0 Then
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_the_Open).Count > 0 Then
                        ctlHayOpen.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_the_Open
                        ctlHayOpen.Populate()
                        chkHayOpen.Checked = True
                        chkHayOpen.Enabled = False
                        dvHayOpenLimit.Attributes.Add("style", "display:block;")
                    End If

                    ' Reproductive Equipment
                    'If SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.ReproductiveMaterials).Count > 0 Then
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.ReproductiveMaterials).Count > 0 Then
                        ctlReproductiveEquip.PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.ReproductiveMaterials
                        ctlReproductiveEquip.Populate()
                        chkReproductive.Checked = True
                        chkReproductive.Enabled = False
                        dvReproductiveLimit.Attributes.Add("style", "display:block;")
                    End If
                Else
                    chkMachineryDescribed.Checked = False
                    chkMachineryDescribed.Enabled = True
                    dvMachineryDescribedLimit.Attributes.Add("style", "display:none;")
                    chkMachineryOpen.Checked = False
                    chkMachineryOpen.Enabled = True
                    dvMachineryOpenLimit.Attributes.Add("style", "display:none;")
                    chkIrrigation.Checked = False
                    chkIrrigation.Enabled = True
                    dvIrrigationLimit.Attributes.Add("style", "display:none;")
                    chkLivestock.Checked = False
                    chkLivestock.Enabled = True
                    dvLivestockLimit.Attributes.Add("style", "display:none;")
                    If IFM.VR.Common.Helpers.FARM.MiscFarmPersPropHelper.IsMiscFarmPersPropAvailable(Quote) = False Then
                        dvMiscFarmPersProperty.Attributes.Add("style", "display:none;")
                    End If
                    chkMiscFarmPersProperty.Checked = False
                    chkMiscFarmPersProperty.Enabled = True
                    dvMiscFarmPersPropertyLimit.Attributes.Add("style", "display:none;")
                    chkBorrowed.Checked = False
                    chkBorrowed.Enabled = True
                    dvBorrowedLimit.Attributes.Add("style", "display:none;")
                    chkMachineryNotDescribed.Checked = False
                    chkMachineryNotDescribed.Enabled = True
                    dvMachineryNotDescribedLimit.Attributes.Add("style", "display:none;")
                    chkGrainBuild.Checked = False
                    chkGrainBuild.Enabled = True
                    dvGrainBuildLimit.Attributes.Add("style", "display:none;")
                    chkGrainOpen.Checked = False
                    chkGrainOpen.Enabled = True
                    dvGrainOpenLimit.Attributes.Add("style", "display:none;")
                    chkHayBuild.Checked = False
                    chkHayBuild.Enabled = True
                    dvHayBuildLimit.Attributes.Add("style", "display:none;")
                    chkHayOpen.Checked = False
                    chkHayOpen.Enabled = True
                    dvHayOpenLimit.Attributes.Add("style", "display:none;")
                    chkReproductive.Checked = False
                    chkReproductive.Enabled = True
                    dvReproductiveLimit.Attributes.Add("style", "display:none;")

                    ''If SubQuoteFirst.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                    'If GoverningStateQuote.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                    '    'Farm Machinery - Special Coverage G
                    '    If QuickQuoteHelperClass.IsValidEffectiveDateForFarmMachinerySpecialCoverageG(Quote.EffectiveDate) = True Then
                    '        SetCSS(dvFarmMachinery, "style", "display:block;")
                    '        'If SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = True Then
                    '        If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = True Then
                    '            chkFarmMachinery.Checked = True
                    '        End If
                    '    Else
                    '        'If SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = True Then
                    '        If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = True Then
                    '            'SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = False
                    '            GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = False
                    '            'Me.ValidationHelper.AddWarning("Effective date must be " + ConfigurationManager.AppSettings("VR_FarmMachinerySpecialCoverageG_EffectiveDate") + " or later for the Farm Machinery - Special Coverage - Coverage G. Coverage was removed.")
                    '        End If
                    '        chkFarmMachinery.Checked = False
                    '        SetCSS(dvFarmMachinery, "style", "display:none;")
                    '    End If

                    '    If String.IsNullOrWhiteSpace(ctlBlnktPersProp.GetBlanketLimitValue) Then
                    '        If chkFarmMachinery.Checked = True Then
                    '            chkFarmMachinery.Checked = False
                    '        End If
                    '    End If

                    '    'If SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IncreasedLimit = "" Then
                    '    If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IncreasedLimit = "" Then
                    '        chkSchedEarth.Checked = False
                    '    End If
                    'Else
                    '    chkSchedEarth.Checked = False
                    'End If

                    'PopulateChildControls()
                End If

                If GoverningStateQuote.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                    'Farm Machinery - Special Coverage G
                    If QuickQuoteHelperClass.IsValidEffectiveDateForFarmMachinerySpecialCoverageG(Quote.EffectiveDate) = True Then
                        SetCSS(dvFarmMachinery, "style", "display:block;")
                        If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = True Then
                            chkFarmMachinery.Checked = True
                        End If
                    Else
                        If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = True Then
                            GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = False
                        End If
                        chkFarmMachinery.Checked = False
                        SetCSS(dvFarmMachinery, "style", "display:none;")
                    End If

                    If String.IsNullOrWhiteSpace(ctlBlnktPersProp.GetBlanketLimitValue) Then
                        chkFarmMachinery.Checked = False
                    End If
                End If


                dvFarmIncidentalLimits.Attributes.Add("style", "display:none;")
                tblGlassBreakageCabs.Attributes.Add("style", "display:none;")
                If Common.Helpers.FARM.GlassBreakageForCabs.IsGlassBreakageForCabsAvailable(Quote) Then
                    If SubQuoteFirst.ProgramTypeId = "6" OrElse SubQuoteFirst.ProgramTypeId = "7" Then
                        dvFarmIncidentalLimits.Attributes.Add("style", "display:block;")
                        txtGlassBreakageIncludedLimit.Text = DefaultGlassBreakageForCabsIncludedLimit
                        Dim GlassBreakCab As QuickQuoteFarmIncidentalLimit
                        If GoverningStateQuote.FarmIncidentalLimits.FindAll(Function(p) p.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Glass_Breakage_in_Cabs).Count > 0 Then
                            GlassBreakCab = GoverningStateQuote.FarmIncidentalLimits.Find(Function(p) p.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Glass_Breakage_in_Cabs)
                            If GlassBreakCab.IncreasedLimit = "0" Then
                                GlassBreakCab.IncreasedLimit = String.Empty
                            End If
                            If GlassBreakCab.TotalLimit = "0" Then
                                GlassBreakCab.TotalLimit = DefaultGlassBreakageForCabsIncludedLimit
                            End If
                            txtGlassBreakageIncreaseLimit.Text = Format(GlassBreakCab.IncreasedLimit.TryToGetInt32, "N0")
                            txtGlassBreakageTotalLimit.Text = Format(GlassBreakCab.TotalLimit.TryToGetInt32, "N0")
                        Else
                            GlassBreakCab = New QuickQuoteFarmIncidentalLimit()
                            GlassBreakCab.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Glass_Breakage_in_Cabs
                            GlassBreakCab.IncludedLimit = DefaultGlassBreakageForCabsIncludedLimit
                            GoverningStateQuote.FarmIncidentalLimits.Add(GlassBreakCab)

                            txtGlassBreakageIncreaseLimit.Text = String.Empty
                            txtGlassBreakageTotalLimit.Text = DefaultGlassBreakageForCabsIncludedLimit
                        End If
                        chkGlassBreakageCabs.Checked = True
                        chkGlassBreakageCabs.Enabled = False
                        tblGlassBreakageCabs.Attributes.Add("style", "display:block;")
                    End If
                End If
                PopulateChildControls()
            End If

            If IsQuoteReadOnly() Then
                divActionButtons.Visible = False
                divEndorsementButtons.Visible = True
                Dim policyNumber As String = Me.Quote.PolicyNumber
                Dim imageNum As Integer = 0
                Dim policyId As Integer = 0
                Dim toolTip As String = "Make a change to this policy"
                'Dim qqHelper As New QuickQuoteHelperClass
                Dim readOnlyViewPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                'QuickQuoteHelperClass.configAppSettingValueAsString("")  'Unused CAH 07/21/2020
                If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                    readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                End If

                btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                btnMakeAChange.ToolTip = toolTip
                btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
            Else
                divActionButtons.Visible = Not IsOnAppPage
                btnMakeAChange.Visible = False
            End If
        End If
    End Sub

    'Coverages should be on the Governing quote B31175 - CAH 04/08/2019
    'Public Overrides Function Save() As Boolean
    '    If Quote IsNot Nothing Then
    '        'Updated 9/6/18 for multi state MLW - Quote to sq
    '        If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
    '            For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
    '                Dim optionalCoverage As QuickQuoteOptionalCoverage

    '                If sq.ScheduledPersonalPropertyCoverages IsNot Nothing Then
    '                    ' If earthquake coverage is selected, it will be added to all coverages that have earthquake coverage available
    '                    For Each coverage As QuickQuoteScheduledPersonalPropertyCoverage In sq.ScheduledPersonalPropertyCoverages
    '                        coverage.HasEarthquakeCoverage = chkSchedEarth.Checked
    '                    Next
    '                End If

    '                ctlBlnktPersProp.SaveEarthquakeToggle(chkSchedEarth.Checked)

    '                If chkSheepPerils.Checked Then
    '                    If sq.OptionalCoverages Is Nothing Then
    '                        sq.OptionalCoverages = New List(Of QuickQuoteOptionalCoverage)
    '                    End If

    '                    If sq.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep).Count <= 0 Then
    '                        optionalCoverage = New QuickQuoteOptionalCoverage()
    '                        optionalCoverage.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep
    '                        optionalCoverage.IncreasedLimit = txtSheep_LimitData.Text
    '                        sq.OptionalCoverages.Add(optionalCoverage)
    '                    End If
    '                Else
    '                    If sq.OptionalCoverages IsNot Nothing Then
    '                        Dim sheepPerils As QuickQuoteOptionalCoverage = sq.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep)
    '                        If sheepPerils IsNot Nothing Then
    '                            dvSheepPerilsLimit.Attributes.Add("style", "display:none;")
    '                            sheepPerils.IncreasedLimit = ""
    '                            sq.OptionalCoverages.Remove(sheepPerils)
    '                        End If
    '                    End If
    '                End If

    '                If chkFarmMachinery.Checked = True AndAlso chkFarmMachinery.Enabled = True Then
    '                    If QuickQuoteHelperClass.IsValidEffectiveDateForFarmMachinerySpecialCoverageG(Quote.EffectiveDate) = True Then
    '                        If sq.UnscheduledPersonalPropertyCoverage Is Nothing Then
    '                            sq.UnscheduledPersonalPropertyCoverage = New QuickQuoteUnscheduledPersonalPropertyCoverage
    '                        End If
    '                        sq.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = True
    '                        sq.UnscheduledPersonalPropertyCoverage.Description = "" 'BUG 7431 Matt A 10-14-2016
    '                    Else
    '                        chkFarmMachinery.Checked = False
    '                        If sq.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
    '                            sq.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = False
    '                            sq.UnscheduledPersonalPropertyCoverage.Description = "" 'BUG 7431 Matt A 10-14-2016
    '                        End If
    '                        Me.ValidationHelper.AddWarning("Effective date must be " + ConfigurationManager.AppSettings("VR_FarmMachinerySpecialCoverageG_EffectiveDate") + " or later for the Farm Machinery - Special Coverage - Coverage G. Coverage was removed.")
    '                    End If
    '                Else
    '                    If sq.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
    '                        If sq.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = True Then
    '                            sq.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = False
    '                            If QuickQuoteHelperClass.IsValidEffectiveDateForFarmMachinerySpecialCoverageG(Quote.EffectiveDate) = False Then
    '                                Me.ValidationHelper.AddWarning("Effective date must be " + ConfigurationManager.AppSettings("VR_FarmMachinerySpecialCoverageG_EffectiveDate") + " or later for the Farm Machinery - Special Coverage - Coverage G. Coverage was removed.")
    '                            End If
    '                        End If
    '                        sq.UnscheduledPersonalPropertyCoverage.Description = "" 'BUG 7431 Matt A 10-14-2016
    '                    End If
    '                End If

    '                'Because the populate function isn't called after attempting to submit the app, this is used to show and hide the Farm Machinery Special Coverage div when needed.
    '                'This should only be needed until quotes can no longer be made effective before 6/1/2016
    '                If Me.IsOnAppPage = True Then
    '                    If QuickQuoteHelperClass.IsValidEffectiveDateForFarmMachinerySpecialCoverageG(Quote.EffectiveDate) = True Then
    '                        SetCSS(dvFarmMachinery, "style", "display:block;")
    '                    Else
    '                        SetCSS(dvFarmMachinery, "style", "display:none;")
    '                    End If
    '                End If


    '                SaveChildControls()

    '                If sq.ScheduledPersonalPropertyCoverages IsNot Nothing OrElse sq.OptionalCoverages IsNot Nothing OrElse sq.UnscheduledPersonalPropertyCoverage.IncreasedLimit <> "" Then
    '                    sq.Farm_F_and_G_DeductibleLimitId = ddlPPDeduct.SelectedValue
    '                Else
    '                    sq.Farm_F_and_G_DeductibleLimitId = "0"
    '                End If

    '                If PeakSeasonButtonPress IsNot Nothing Then
    '                    Dim splitPeakButton As List(Of String) = New List(Of String)(PeakSeasonButtonPress.Split("!"c))
    '                    Dim filterPeakSeason As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = sq.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType.ToString() = splitPeakButton(1).ToString())
    '                    filterPeakSeason(RowNumber).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, filterPeakSeason, New QuickQuoteUnscheduledPersonalPropertyCoverage, RowNumber)
    '                    filterPeakSeason(RowNumber).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.SplitPeak(Quote, filterPeakSeason, New QuickQuoteUnscheduledPersonalPropertyCoverage, PeakSeasonButtonPress, RowNumber, RowNumber)
    '                Else
    '                    If sq.ScheduledPersonalPropertyCoverages IsNot Nothing AndAlso sq.ScheduledPersonalPropertyCoverages.Count > 0 Then
    '                        For covIdx As Integer = 0 To sq.ScheduledPersonalPropertyCoverages.Count - 1
    '                            sq.ScheduledPersonalPropertyCoverages(covIdx).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, sq.ScheduledPersonalPropertyCoverages, New QuickQuoteUnscheduledPersonalPropertyCoverage, covIdx)
    '                            sq.ScheduledPersonalPropertyCoverages(covIdx).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.SplitPeak(Quote, sq.ScheduledPersonalPropertyCoverages, New QuickQuoteUnscheduledPersonalPropertyCoverage, PeakSeasonButtonPress, covIdx, RowNumber)
    '                        Next
    '                    End If
    '                End If
    '            Next
    '            Return True
    '        End If
    '    End If

    '    Return False
    'End Function

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            'Updated 9/6/18 for multi state MLW - Quote to sq
            If GoverningStateQuote() IsNot Nothing Then
                Dim optionalCoverage As QuickQuoteOptionalCoverage

                If chkSheepPerils.Checked Then
                    If GoverningStateQuote.OptionalCoverages Is Nothing Then
                        GoverningStateQuote.OptionalCoverages = New List(Of QuickQuoteOptionalCoverage)
                    End If

                    If GoverningStateQuote.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep).Count <= 0 Then
                        optionalCoverage = New QuickQuoteOptionalCoverage()
                        optionalCoverage.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep
                        optionalCoverage.IncreasedLimit = txtSheep_LimitData.Text
                        GoverningStateQuote.OptionalCoverages.Add(optionalCoverage)
                    End If
                Else
                    If GoverningStateQuote.OptionalCoverages IsNot Nothing Then
                        Dim sheepPerils As QuickQuoteOptionalCoverage = GoverningStateQuote.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep)
                        If sheepPerils IsNot Nothing Then
                            dvSheepPerilsLimit.Attributes.Add("style", "display:none;")
                            sheepPerils.IncreasedLimit = ""
                            GoverningStateQuote.OptionalCoverages.Remove(sheepPerils)
                        End If
                    End If
                End If

                If chkFarmMachinery.Checked = True AndAlso chkFarmMachinery.Enabled = True Then
                    If QuickQuoteHelperClass.IsValidEffectiveDateForFarmMachinerySpecialCoverageG(Quote.EffectiveDate) = True Then
                        If GoverningStateQuote.UnscheduledPersonalPropertyCoverage Is Nothing Then
                            GoverningStateQuote.UnscheduledPersonalPropertyCoverage = New QuickQuoteUnscheduledPersonalPropertyCoverage
                        End If
                        GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = True
                        GoverningStateQuote.UnscheduledPersonalPropertyCoverage.Description = "" 'BUG 7431 Matt A 10-14-2016
                    Else
                        chkFarmMachinery.Checked = False
                        If GoverningStateQuote.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                            GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = False
                            GoverningStateQuote.UnscheduledPersonalPropertyCoverage.Description = "" 'BUG 7431 Matt A 10-14-2016
                        End If
                        Me.ValidationHelper.AddWarning("Effective date must be " + ConfigurationManager.AppSettings("VR_FarmMachinerySpecialCoverageG_EffectiveDate") + " or later for the Farm Machinery - Special Coverage - Coverage G. Coverage was removed.")
                    End If
                Else
                    If GoverningStateQuote.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                        If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = True Then
                            GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = False
                            If QuickQuoteHelperClass.IsValidEffectiveDateForFarmMachinerySpecialCoverageG(Quote.EffectiveDate) = False Then
                                Me.ValidationHelper.AddWarning("Effective date must be " + ConfigurationManager.AppSettings("VR_FarmMachinerySpecialCoverageG_EffectiveDate") + " or later for the Farm Machinery - Special Coverage - Coverage G. Coverage was removed.")
                            End If
                        End If
                        GoverningStateQuote.UnscheduledPersonalPropertyCoverage.Description = "" 'BUG 7431 Matt A 10-14-2016
                    End If
                End If

                If chkPropertyInTransit.Checked = True Then
                    If GoverningStateQuote.OptionalCoverages Is Nothing Then
                        GoverningStateQuote.OptionalCoverages = New List(Of QuickQuoteOptionalCoverage)
                    End If

                    If GoverningStateQuote.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Property_in_Transit).Count <= 0 Then
                        optionalCoverage = New QuickQuoteOptionalCoverage()
                        optionalCoverage.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Property_in_Transit
                        optionalCoverage.IncludedLimit = "5000"
                        GoverningStateQuote.OptionalCoverages.Add(optionalCoverage)
                    End If
                Else
                    If GoverningStateQuote.OptionalCoverages IsNot Nothing Then
                        Dim propTransit As QuickQuoteOptionalCoverage = GoverningStateQuote.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Property_in_Transit)
                        If propTransit IsNot Nothing Then
                            GoverningStateQuote.OptionalCoverages.Remove(propTransit)
                        End If
                    End If
                End If

                If Common.Helpers.FARM.GlassBreakageForCabs.IsGlassBreakageForCabsAvailable(Quote) Then
                    If SubQuoteFirst.ProgramTypeId = "6" OrElse SubQuoteFirst.ProgramTypeId = "7" Then
                        If chkGlassBreakageCabs.Checked Then
                            Dim GlassBreakCab As QuickQuoteFarmIncidentalLimit
                            If GoverningStateQuote.FarmIncidentalLimits Is Nothing Then
                                GoverningStateQuote.FarmIncidentalLimits = New List(Of QuickQuoteFarmIncidentalLimit)
                            End If

                            If GoverningStateQuote.FarmIncidentalLimits.FindAll(Function(p) p.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Glass_Breakage_in_Cabs).Count > 0 Then
                                GlassBreakCab = GoverningStateQuote.FarmIncidentalLimits.Find(Function(p) p.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Glass_Breakage_in_Cabs)
                                GlassBreakCab.IncreasedLimit = txtGlassBreakageIncreaseLimit.Text
                                GlassBreakCab.IncludedLimit = txtGlassBreakageIncludedLimit.Text
                                GlassBreakCab.TotalLimit = txtGlassBreakageTotalLimit.Text
                            Else
                                GlassBreakCab = New QuickQuoteFarmIncidentalLimit()
                                GlassBreakCab.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Glass_Breakage_in_Cabs
                                GlassBreakCab.IncreasedLimit = txtGlassBreakageIncreaseLimit.Text
                                GlassBreakCab.IncludedLimit = txtGlassBreakageIncludedLimit.Text
                                GlassBreakCab.TotalLimit = txtGlassBreakageTotalLimit.Text
                                GoverningStateQuote.FarmIncidentalLimits.Add(GlassBreakCab)

                                chkGlassBreakageCabs.Checked = True
                                chkGlassBreakageCabs.Enabled = False
                                tblGlassBreakageCabs.Attributes.Add("style", "display:block;")

                                txtGlassBreakageIncreaseLimit.Text = Format(GlassBreakCab.IncreasedLimit.TryToGetInt32, "N0")
                                txtGlassBreakageTotalLimit.Text = Format(GlassBreakCab.TotalLimit.TryToGetInt32, "N0")


                            End If
                        Else
                            If GoverningStateQuote.FarmIncidentalLimits IsNot Nothing Then
                                Dim GlassBreakCab = GoverningStateQuote.FarmIncidentalLimits.Find(Function(p) p.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Glass_Breakage_in_Cabs)
                                If GlassBreakCab IsNot Nothing Then
                                    tblGlassBreakageCabs.Attributes.Add("style", "display:none;")
                                    txtGlassBreakageIncreaseLimit.Text = String.Empty
                                    GoverningStateQuote.FarmIncidentalLimits.Remove(GlassBreakCab)
                                End If
                            End If
                        End If
                    End If
                End If


                'Because the populate function isn't called after attempting to submit the app, this is used to show and hide the Farm Machinery Special Coverage div when needed.
                'This should only be needed until quotes can no longer be made effective before 6/1/2016
                If Me.IsOnAppPage = True Then
                    If QuickQuoteHelperClass.IsValidEffectiveDateForFarmMachinerySpecialCoverageG(Quote.EffectiveDate) = True Then
                        SetCSS(dvFarmMachinery, "style", "display:block;")
                    Else
                        SetCSS(dvFarmMachinery, "style", "display:none;")
                    End If
                End If


                SaveChildControls()

                If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing OrElse GoverningStateQuote.OptionalCoverages IsNot Nothing OrElse GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IncreasedLimit <> "" Then
                    GoverningStateQuote.Farm_F_and_G_DeductibleLimitId = ddlPPDeduct.SelectedValue
                Else
                    GoverningStateQuote.Farm_F_and_G_DeductibleLimitId = "0"
                End If

                If PeakSeasonButtonPress IsNot Nothing Then
                    Dim splitPeakButton As List(Of String) = New List(Of String)(PeakSeasonButtonPress.Split("!"c))
                    Dim filterPeakSeason As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType.ToString() = splitPeakButton(1).ToString())
                    'filterPeakSeason(RowNumber).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, filterPeakSeason, New QuickQuoteUnscheduledPersonalPropertyCoverage, RowNumber)
                    filterPeakSeason(RowNumber).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(filterPeakSeason, New QuickQuoteUnscheduledPersonalPropertyCoverage, RowNumber, Quote.QuoteTransactionType)
                    filterPeakSeason(RowNumber).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.SplitPeak_EndoReady(Quote, filterPeakSeason, New QuickQuoteUnscheduledPersonalPropertyCoverage, PeakSeasonButtonPress, RowNumber, RowNumber, Quote.QuoteTransactionType)
                Else
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing AndAlso GoverningStateQuote.ScheduledPersonalPropertyCoverages.Count > 0 Then
                        For covIdx As Integer = 0 To GoverningStateQuote.ScheduledPersonalPropertyCoverages.Count - 1
                            'GoverningStateQuote.ScheduledPersonalPropertyCoverages(covIdx).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable, GoverningStateQuote.ScheduledPersonalPropertyCoverages, New QuickQuoteUnscheduledPersonalPropertyCoverage, covIdx)
                            GoverningStateQuote.ScheduledPersonalPropertyCoverages(covIdx).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(GoverningStateQuote.ScheduledPersonalPropertyCoverages, New QuickQuoteUnscheduledPersonalPropertyCoverage, covIdx, Quote.QuoteTransactionType)
                            GoverningStateQuote.ScheduledPersonalPropertyCoverages(covIdx).PeakSeasons = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.SplitPeak_EndoReady(Quote, GoverningStateQuote.ScheduledPersonalPropertyCoverages, New QuickQuoteUnscheduledPersonalPropertyCoverage, PeakSeasonButtonPress, covIdx, RowNumber, Quote.QuoteTransactionType)
                        Next
                    End If
                End If
                Return True
            End If
        End If
        Populate()
        Return False
    End Function

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click, btnIMRVPage.Click, btnIMRVPageEndo.Click, lnkSavePersProp.Click, lnkSavePersPropSched.Click, lnkSaveBlanket.Click, lnkSaveOptional.Click, lnkSaveFarmIncidental.Click
        If IsQuoteReadOnly() AndAlso sender Is btnIMRVPageEndo Then RaiseEvent RequestIMNavigation() : Exit Sub
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New VRValidationArgs(DefaultValidationType)))

        If sender Is btnIMRVPage Then
            If ValidationSummmary.HasErrors = False Then
                RaiseEvent RequestIMNavigation()
            End If
        ElseIf sender Is btnIMRVPageEndo Then
            RaiseEvent RequestIMNavigation()
        Else
            Populate()
        End If
    End Sub

    Protected Sub btnRate_Click(sender As Object, e As EventArgs) Handles btnRate.Click
        'Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType))) 'remove because it caused duplicate validations - Matt A 8-6-15
        RaiseEvent RatePersonalProperty()
    End Sub

    Protected Sub CheckboxAdd(sender As Object, e As EventArgs) Handles chkAnimals.CheckedChanged, chkMachineryDescribed.CheckedChanged, chkMachineryOpen.CheckedChanged,
        chkIrrigation.CheckedChanged, chkLivestock.CheckedChanged, chkBorrowed.CheckedChanged, chkMachineryNotDescribed.CheckedChanged, chkGrainBuild.CheckedChanged,
        chkGrainOpen.CheckedChanged, chkHayBuild.CheckedChanged, chkHayOpen.CheckedChanged, chkReproductive.CheckedChanged, chkMiscFarmPersProperty.CheckedChanged
        If Quote IsNot Nothing Then
            AddNewItem(DirectCast(sender, CheckBox).ID)
        End If
    End Sub

    Private Sub AddNewItem(form As String)
        If Quote IsNot Nothing Then
            'Updated 11/05/2019 for Bug 33751 MLW - sq to GoverningStateQuote
            If GoverningStateQuote() IsNot Nothing Then
                'Updated 9/7/18 for multi state MLW - Quote to SubQuotes sq
                'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                'For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                'If sq.ScheduledPersonalPropertyCoverages Is Nothing Then
                If GoverningStateQuote.ScheduledPersonalPropertyCoverages Is Nothing Then
                    'sq.ScheduledPersonalPropertyCoverages = New List(Of QuickQuoteScheduledPersonalPropertyCoverage)()
                    GoverningStateQuote.ScheduledPersonalPropertyCoverages = New List(Of QuickQuoteScheduledPersonalPropertyCoverage)()
                End If

                Dim newSchedPPItem As New QuickQuoteScheduledPersonalPropertyCoverage()

                If form = "chkAnimals" Or form = "lnkAddAnimalsLimit" Then
                    'If sq.OptionalCoverages Is Nothing Then
                    If GoverningStateQuote.OptionalCoverages Is Nothing Then
                        'sq.OptionalCoverages = New List(Of QuickQuoteOptionalCoverage)()
                        GoverningStateQuote.OptionalCoverages = New List(Of QuickQuoteOptionalCoverage)()
                    End If
                End If

                Dim newOptionalItem As New QuickQuoteOptionalCoverage()

                Select Case form
                    Case "chkAnimals",
                            "lnkAddAnimalsLimit"
                        newOptionalItem.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_4_H_and_FFAAnimals
                        newOptionalItem.Description = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmOptionalCoverage(newOptionalItem.CoverageType)
                        'sq.OptionalCoverages.Add(newOptionalItem)
                        GoverningStateQuote.OptionalCoverages.Add(newOptionalItem)
                        chkAnimals.Enabled = False
                        dvAnimalsLimit.Attributes.Add("style", "display:block;")
                    Case "chkMachineryDescribed",
                            "lnkAddMachineryDescribed"
                        newSchedPPItem.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.LocationF_DescribedMachinery
                        newSchedPPItem.Description = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmPersonalCoverage(newSchedPPItem.CoverageType)
                        'sq.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        GoverningStateQuote.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        chkMachineryDescribed.Enabled = False
                        dvMachineryDescribedLimit.Attributes.Add("style", "display:block;")
                    Case "chkMachineryOpen",
                            "lnkAddMachineryOpen"
                        newSchedPPItem.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.FarmMachineryDescribed_OpenPerils
                        newSchedPPItem.Description = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmPersonalCoverage(newSchedPPItem.CoverageType)
                        'sq.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        GoverningStateQuote.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        chkMachineryOpen.Enabled = False
                        dvMachineryOpenLimit.Attributes.Add("style", "display:block;")
                    Case "chkIrrigation",
                            "lnkAddIrrigation"
                        newSchedPPItem.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Irrigation_Equipment
                        newSchedPPItem.Description = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmPersonalCoverage(newSchedPPItem.CoverageType)
                        'sq.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        GoverningStateQuote.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        chkIrrigation.Enabled = False
                        dvIrrigationLimit.Attributes.Add("style", "display:block;")
                    Case "chkLivestock",
                            "lnkAddLivestock"
                        newSchedPPItem.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Livestock
                        newSchedPPItem.Description = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmPersonalCoverage(newSchedPPItem.CoverageType)
                        'sq.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        GoverningStateQuote.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        chkLivestock.Enabled = False
                        ctlLivestock.TogglePeakSeason = True
                        dvLivestockLimit.Attributes.Add("style", "display:block;")
                        chkSheepPerils.Enabled = True
                    Case "chkMiscFarmPersProperty",
                            "lnkAddMiscFarmPersProperty"
                        If IFM.VR.Common.Helpers.FARM.MiscFarmPersPropHelper.IsMiscFarmPersPropAvailable(Quote) Then
                            newSchedPPItem.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.MiscellaneousFarmPersonalProperty
                            newSchedPPItem.Description = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmPersonalCoverage(newSchedPPItem.CoverageType)
                            'sq.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                            GoverningStateQuote.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                            chkMiscFarmPersProperty.Enabled = False
                            dvMiscFarmPersPropertyLimit.Attributes.Add("style", "display:block;")
                        End If
                    Case "chkBorrowed",
                            "lnkAddBorrowed"
                        newSchedPPItem.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_Rented_or_borrowed_Equipment
                        newSchedPPItem.Description = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmPersonalCoverage(newSchedPPItem.CoverageType)
                        'sq.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        GoverningStateQuote.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        chkBorrowed.Enabled = False
                        dvBorrowedLimit.Attributes.Add("style", "display:block;")
                    Case "chkMachineryNotDescribed",
                            "lnkAddMachineryNotDescribed"
                        newSchedPPItem.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.LocationF_MachineryNotDescribed
                        newSchedPPItem.Description = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmPersonalCoverage(newSchedPPItem.CoverageType)
                        'sq.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        GoverningStateQuote.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        chkMachineryNotDescribed.Enabled = False
                        dvMachineryNotDescribedLimit.Attributes.Add("style", "display:block;")
                        hiddenMoreLess.Value = "expanded"
                    Case "chkGrainBuild",
                            "lnkAddGrainBuildPeak"
                        newSchedPPItem.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Grain
                        newSchedPPItem.Description = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmPersonalCoverage(newSchedPPItem.CoverageType)
                        'sq.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        GoverningStateQuote.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        chkGrainBuild.Enabled = False
                        ctlGrainBuilding.TogglePeakSeason = True
                        dvGrainBuildLimit.Attributes.Add("style", "display:block;")
                        hiddenMoreLess.Value = "expanded"
                    Case "chkGrainOpen",
                            "lnkAddGrainOpen"
                        newSchedPPItem.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.GrainintheOpen
                        newSchedPPItem.Description = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmPersonalCoverage(newSchedPPItem.CoverageType)
                        'sq.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        GoverningStateQuote.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        chkGrainOpen.Enabled = False
                        ctlGrainOpen.TogglePeakSeason = True
                        dvGrainOpenLimit.Attributes.Add("style", "display:block;")
                        hiddenMoreLess.Value = "expanded"
                    Case "chkHayBuild",
                            "lnkAddHayBuilding"
                        newSchedPPItem.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_Barn
                        newSchedPPItem.Description = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmPersonalCoverage(newSchedPPItem.CoverageType)
                        'sq.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        GoverningStateQuote.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        chkHayBuild.Enabled = False
                        ctlHayBuilding.TogglePeakSeason = True
                        dvHayBuildLimit.Attributes.Add("style", "display:block;")
                        hiddenMoreLess.Value = "expanded"
                    Case "chkHayOpen",
                            "lnkAddHayOpen"
                        newSchedPPItem.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_the_Open
                        newSchedPPItem.Description = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmPersonalCoverage(newSchedPPItem.CoverageType)
                        'sq.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        GoverningStateQuote.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        chkHayOpen.Enabled = False
                        ctlHayOpen.TogglePeakSeason = True
                        dvHayOpenLimit.Attributes.Add("style", "display:block;")
                        hiddenMoreLess.Value = "expanded"
                    Case "chkReproductive",
                            "lnkAddReproductive"
                        newSchedPPItem.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.ReproductiveMaterials
                        newSchedPPItem.Description = IFM.VR.Common.Helpers.FARM.FarmPersonalPropertyHelper.GetDefaultTextForFarmPersonalCoverage(newSchedPPItem.CoverageType)
                        'sq.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        GoverningStateQuote.ScheduledPersonalPropertyCoverages.Add(newSchedPPItem)
                        chkReproductive.Enabled = False
                        dvReproductiveLimit.Attributes.Add("style", "display:block;")
                        hiddenMoreLess.Value = "expanded"
                End Select
                'Next
            End If

            GetSelectedMoreLess()
            'SaveChildControls()
            'PopulateChildControls()
            Save()
            Save_FireSaveEvent(False)
            Populate()
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = "Farm Personal Property"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim valList = PersonalPropertyValidator.ValidateFARPersProp(Quote, 0, 0, QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.None, QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.None, valArgs.ValidationType)

        If valList.Any() Then
            For Each v In valList
                ' *********************
                ' Base Policy Coverages
                ' *********************
                Select Case v.FieldId
                    Case PersonalPropertyValidator.MissingFGDeductible
                        ValidationHelper.Val_BindValidationItemToControl(ddlPPDeduct, v, accordList)
                    Case PersonalPropertyValidator.MissingSheepLimit
                        ValidationHelper.Val_BindValidationItemToControl(txtSheep_LimitData, v, accordList)
                End Select
            Next
        End If

        ValidateChildControls(valArgs)
    End Sub

    Private Sub RemoveItem(rowNumber As Integer, optCov As QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType,
                           persPropCov As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType) Handles ctlAnimals.RemoveItem, ctlMachineDescribed.RemoveItem, ctlMachineOpen.RemoveItem, ctlIrrigationEquip.RemoveItem,
        ctlLivestock.RemoveItem, ctlBorrowed.RemoveItem, ctlMachineNotDescribed.RemoveItem, ctlGrainBuilding.RemoveItem, ctlGrainOpen.RemoveItem, ctlHayBuilding.RemoveItem, ctlHayOpen.RemoveItem, ctlReproductiveEquip.RemoveItem, ctlMiscFarmPersonalProperty.RemoveItem
        If Quote IsNot Nothing Then
            'Updated 11/05/2019 for Bug 33751 MLW - sq to GoverningStateQuote
            If GoverningStateQuote() IsNot Nothing Then
                'Updated 9/7/18 for multi state MLW
                'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                'For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                If GoverningStateQuote.OptionalCoverages IsNot Nothing Then
                    Select Case optCov
                            ' 4H and FAA Animals
                        Case QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_4_H_and_FFAAnimals
                            Dim optPersPropList As List(Of QuickQuoteOptionalCoverage) = GoverningStateQuote.OptionalCoverages.FindAll(Function(p) p.CoverageType = optCov)
                            Dim optPersProp As QuickQuoteOptionalCoverage = optPersPropList(rowNumber)

                            GoverningStateQuote.OptionalCoverages.Remove(optPersProp)

                            If optPersPropList.Count <= 1 Then
                                chkAnimals.Checked = False
                                chkAnimals.Enabled = True
                                dvAnimalsLimit.Attributes.Add("style", "display:none;")
                            End If
                    End Select
                End If

                If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                    Select Case persPropCov
                        ' Farm Machinery Described
                        Case QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.LocationF_DescribedMachinery
                            Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = persPropCov)
                            Dim schedPersProp As QuickQuoteScheduledPersonalPropertyCoverage = schedPersPropList(rowNumber)

                            GoverningStateQuote.ScheduledPersonalPropertyCoverages.Remove(schedPersProp)

                            If schedPersPropList.Count <= 1 Then
                                chkMachineryDescribed.Checked = False
                                chkMachineryDescribed.Enabled = True
                                dvMachineryDescribedLimit.Attributes.Add("style", "display:none;")
                            End If
                            ' Farm Machinery Described - Open Perils
                        Case QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.FarmMachineryDescribed_OpenPerils
                            Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = persPropCov)
                            Dim schedPersProp As QuickQuoteScheduledPersonalPropertyCoverage = schedPersPropList(rowNumber)

                            GoverningStateQuote.ScheduledPersonalPropertyCoverages.Remove(schedPersProp)

                            If schedPersPropList.Count <= 1 Then
                                chkMachineryOpen.Checked = False
                                chkMachineryOpen.Enabled = True
                                dvMachineryOpenLimit.Attributes.Add("style", "display:none;")
                            End If
                            ' Irrigation Equipment
                        Case QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Irrigation_Equipment
                            Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = persPropCov)
                            Dim schedPersProp As QuickQuoteScheduledPersonalPropertyCoverage = schedPersPropList(rowNumber)

                            GoverningStateQuote.ScheduledPersonalPropertyCoverages.Remove(schedPersProp)

                            If schedPersPropList.Count <= 1 Then
                                chkIrrigation.Checked = False
                                chkIrrigation.Enabled = True
                                dvIrrigationLimit.Attributes.Add("style", "display:none;")
                            End If
                            ' Livestock
                        Case QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Livestock
                            Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = persPropCov)
                            Dim schedPersProp As QuickQuoteScheduledPersonalPropertyCoverage = schedPersPropList(rowNumber)

                            GoverningStateQuote.ScheduledPersonalPropertyCoverages.Remove(schedPersProp)

                            If schedPersPropList.Count <= 1 Then
                                chkLivestock.Checked = False
                                chkLivestock.Enabled = True
                                dvLivestockLimit.Attributes.Add("style", "display:none;")
                                chkSheepPerils.Checked = False
                                chkSheepPerils.Enabled = False
                                txtSheep_LimitData.Text = ""
                            End If
                        ' Miscellaneous Farm Personal Property
                        Case QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.MiscellaneousFarmPersonalProperty
                            If IFM.VR.Common.Helpers.FARM.MiscFarmPersPropHelper.IsMiscFarmPersPropAvailable(Quote) Then
                                Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = persPropCov)
                                Dim schedPersProp As QuickQuoteScheduledPersonalPropertyCoverage = schedPersPropList(rowNumber)

                                GoverningStateQuote.ScheduledPersonalPropertyCoverages.Remove(schedPersProp)

                                If schedPersPropList.Count <= 1 Then
                                    chkMiscFarmPersProperty.Checked = False
                                    chkMiscFarmPersProperty.Enabled = True
                                    dvMiscFarmPersPropertyLimit.Attributes.Add("style", "display:none;")
                                End If
                            End If
                            ' Rented or Borrowed Equipment
                        Case QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_Rented_or_borrowed_Equipment
                            Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = persPropCov)
                            Dim schedPersProp As QuickQuoteScheduledPersonalPropertyCoverage = schedPersPropList(rowNumber)

                            GoverningStateQuote.ScheduledPersonalPropertyCoverages.Remove(schedPersProp)

                            If schedPersPropList.Count <= 1 Then
                                chkBorrowed.Checked = False
                                chkBorrowed.Enabled = True
                                dvBorrowedLimit.Attributes.Add("style", "display:none;")
                            End If
                            ' Farm Machinery - Not Described
                        Case QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.LocationF_MachineryNotDescribed
                            Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = persPropCov)
                            Dim schedPersProp As QuickQuoteScheduledPersonalPropertyCoverage = schedPersPropList(rowNumber)

                            GoverningStateQuote.ScheduledPersonalPropertyCoverages.Remove(schedPersProp)

                            If schedPersPropList.Count <= 1 Then
                                chkMachineryNotDescribed.Checked = False
                                chkMachineryNotDescribed.Enabled = True
                                dvMachineryNotDescribedLimit.Attributes.Add("style", "display:none;")
                            End If
                            hiddenMoreLess.Value = "expanded"
                            ' Grain in Buildings
                        Case QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Grain
                            Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = persPropCov)
                            Dim schedPersProp As QuickQuoteScheduledPersonalPropertyCoverage = schedPersPropList(rowNumber)

                            GoverningStateQuote.ScheduledPersonalPropertyCoverages.Remove(schedPersProp)

                            If schedPersPropList.Count <= 1 Then
                                chkGrainBuild.Checked = False
                                chkGrainBuild.Enabled = True
                                dvGrainBuildLimit.Attributes.Add("style", "display:none;")
                            End If
                            hiddenMoreLess.Value = "expanded"
                            ' Grain in the Open
                        Case QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.GrainintheOpen
                            Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = persPropCov)
                            Dim schedPersProp As QuickQuoteScheduledPersonalPropertyCoverage = schedPersPropList(rowNumber)

                            GoverningStateQuote.ScheduledPersonalPropertyCoverages.Remove(schedPersProp)

                            If schedPersPropList.Count <= 1 Then
                                chkGrainOpen.Checked = False
                                chkGrainOpen.Enabled = True
                                dvGrainOpenLimit.Attributes.Add("style", "display:none;")
                            End If
                            hiddenMoreLess.Value = "expanded"
                            ' Hay in Buildings
                        Case QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_Barn
                            Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = persPropCov)
                            Dim schedPersProp As QuickQuoteScheduledPersonalPropertyCoverage = schedPersPropList(rowNumber)

                            GoverningStateQuote.ScheduledPersonalPropertyCoverages.Remove(schedPersProp)

                            If schedPersPropList.Count <= 1 Then
                                chkHayBuild.Checked = False
                                chkHayBuild.Enabled = True
                                dvHayBuildLimit.Attributes.Add("style", "display:none;")
                            End If
                            hiddenMoreLess.Value = "expanded"
                            ' Hay in the Open
                        Case QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Hay_in_the_Open
                            Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = persPropCov)
                            Dim schedPersProp As QuickQuoteScheduledPersonalPropertyCoverage = schedPersPropList(rowNumber)

                            GoverningStateQuote.ScheduledPersonalPropertyCoverages.Remove(schedPersProp)

                            If schedPersPropList.Count <= 1 Then
                                chkHayOpen.Checked = False
                                chkHayOpen.Enabled = True
                                dvHayOpenLimit.Attributes.Add("style", "display:none;")
                            End If
                            hiddenMoreLess.Value = "expanded"
                            ' Reproductive Equipment
                        Case QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.ReproductiveMaterials
                            Dim schedPersPropList As List(Of QuickQuoteScheduledPersonalPropertyCoverage) = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = persPropCov)
                            Dim schedPersProp As QuickQuoteScheduledPersonalPropertyCoverage = schedPersPropList(rowNumber)

                            GoverningStateQuote.ScheduledPersonalPropertyCoverages.Remove(schedPersProp)

                            If schedPersPropList.Count <= 1 Then
                                chkReproductive.Checked = False
                                chkReproductive.Enabled = True
                                dvReproductiveLimit.Attributes.Add("style", "display:none;")
                            End If
                            hiddenMoreLess.Value = "expanded"
                    End Select
                End If
                'Next
            End If
            GetSelectedMoreLess()
            PopulateChildControls()
            Save_FireSaveEvent(False)

            Populate()
            'PopulateChildControls()
        End If
    End Sub

    Protected Sub lnkAddAnimalsLimit_Click(sender As Object, e As EventArgs) Handles lnkAddAnimalsLimit.Click
        'Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
        'Updated 9/7/18 for multi state MLW
        'If SubQuoteFirst IsNot Nothing Then
        If GoverningStateQuote() IsNot Nothing Then
            'If SubQuoteFirst.OptionalCoverages IsNot Nothing Then
            If GoverningStateQuote.OptionalCoverages IsNot Nothing Then
                AddNewItem("chkAnimals")
                dvAnimalsLimit.Attributes.Add("style", "display:block;")
            End If
        End If
    End Sub

    Protected Sub lnkAddMachineryDescribed_Click(sender As Object, e As EventArgs) Handles lnkAddMachineryDescribed.Click, lnkAddMachineryOpen.Click, lnkAddIrrigation.Click, lnkAddLivestock.Click,
        lnkAddBorrowed.Click, lnkAddReproductive.Click, lnkAddMiscFarmPersProperty.Click
        'Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
        'Updated 9/7/18 for multi state MLW
        'If SubQuoteFirst IsNot Nothing Then
        If GoverningStateQuote() IsNot Nothing Then
            'If SubQuoteFirst.ScheduledPersonalPropertyCoverages IsNot Nothing Then
            If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                AddNewItem(CType(sender, LinkButton).ID)
                dvAnimalsLimit.Attributes.Add("style", "display:block;")
            End If
        End If
    End Sub

    Protected Sub ClearOptionalControls(sender As Object, e As EventArgs) Handles lnkClearOptional.Click
        'Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
        'Updated 9/7/18 for multi state MLW
        'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
        If GoverningStateQuote() IsNot Nothing Then
            'For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            'If sq.OptionalCoverages IsNot Nothing Then
            If GoverningStateQuote.OptionalCoverages IsNot Nothing Then
                'Dim sheepPerils As QuickQuoteOptionalCoverage = sq.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep)
                Dim sheepPerils As QuickQuoteOptionalCoverage = GoverningStateQuote.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep)

                If sheepPerils IsNot Nothing Then
                    'sq.OptionalCoverages.Remove(sheepPerils)
                    GoverningStateQuote.OptionalCoverages.Remove(sheepPerils)
                    txtSheep_LimitData.Text = ""
                    chkSheepPerils.Checked = False
                End If
            End If
            'Next
        End If

        Save_FireSaveEvent(False)
        Populate()
    End Sub

    Protected Sub ClearFarmIncidentalControls(sender As Object, e As EventArgs) Handles lnkClearFarmIncidental.Click

        txtGlassBreakageIncreaseLimit.Text = String.Empty
        txtGlassBreakageTotalLimit.Text = DefaultGlassBreakageForCabsIncludedLimit

        Save_FireSaveEvent(False)
        Populate()
    End Sub

    Protected Sub ClearDeductibleControls(sender As Object, e As EventArgs) Handles lnkClearPersProp.Click
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlPPDeduct, "")
        Save_FireSaveEvent(False)
        Populate()
    End Sub

    Protected Sub ClearScheduledControls(sender As Object, e As EventArgs) Handles lnkClearPersPropSched.Click
        'Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
        If GoverningStateQuote() IsNot Nothing Then
            GoverningStateQuote.OptionalCoverages = Nothing
            GoverningStateQuote.ScheduledPersonalPropertyCoverages = Nothing
        End If
        'Updated 9/7/18 for multi state MLW
        'If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
        '    For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
        '        sq.OptionalCoverages = Nothing
        '        sq.ScheduledPersonalPropertyCoverages = Nothing
        '    Next
        'End If
        Save_FireSaveEvent(False)
        hiddenMoreLess.Value = "collapsed"
        Populate()
    End Sub

    Private Sub RefreshAfterPeakSeasonAdd() Handles ctlLivestock.RaiseRefreshPeakSeason, ctlGrainBuilding.RaiseRefreshPeakSeason, ctlGrainOpen.RaiseRefreshPeakSeason, ctlHayBuilding.RaiseRefreshPeakSeason, ctlHayOpen.RaiseRefreshPeakSeason
        Populate()
    End Sub

    Private Sub SetPeakDateButtonPress(button As String) Handles ctlLivestock.RaisePeakButtonPress, ctlGrainBuilding.RaisePeakButtonPress, ctlGrainOpen.RaisePeakButtonPress, ctlHayBuilding.RaisePeakButtonPress, ctlHayOpen.RaisePeakButtonPress, ctlBlnktPersProp.RaisePeakButtonPress
        PeakSeasonButtonPress = button
    End Sub

    Private Sub SaveBeforePeakSeasonAdd() Handles ctlLivestock.SavePeakSeason, ctlGrainBuilding.SavePeakSeason, ctlGrainOpen.SavePeakSeason, ctlHayBuilding.SavePeakSeason, ctlHayOpen.SavePeakSeason
        Save()
    End Sub

    Private Sub SetCurrentRowNumber(rowNum As Integer) Handles ctlLivestock.RaiseRowNumber, ctlGrainBuilding.RaiseRowNumber, ctlGrainOpen.RaiseRowNumber, ctlHayBuilding.RaiseRowNumber, ctlHayOpen.RaiseRowNumber
        RowNumber = rowNum
    End Sub

    Private Sub ToggleEndorsementRequirements()
        If IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
            lblAnimalsDesc.CssClass = "showAsRequired"
            lblMachineryDescribedDesc.CssClass = "showAsRequired"
            lblMachineryOpenDesc.CssClass = "showAsRequired"
            lblIrrigationDesc.CssClass = "showAsRequired"
            lblLivestockDesc.CssClass = "showAsRequired"
            If IFM.VR.Common.Helpers.FARM.MiscFarmPersPropHelper.IsMiscFarmPersPropAvailable(Quote) Then
                lblMiscFarmPersPropertyDesc.CssClass = "showAsRequired"
            End If
            lblBorrowedDesc.CssClass = "showAsRequired"
            lblMachineryNotDescribedDesc.CssClass = "showAsRequired"
            lblGrainBuildDesc.CssClass = "showAsRequired"
            lblGrainOpenDesc.CssClass = "showAsRequired"
            lblHayBuildDesc.CssClass = "showAsRequired"
            lblHayOpenDesc.CssClass = "showAsRequired"
            lblReproductiveDesc.CssClass = "showAsRequired"
        End If
    End Sub

    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub
End Class