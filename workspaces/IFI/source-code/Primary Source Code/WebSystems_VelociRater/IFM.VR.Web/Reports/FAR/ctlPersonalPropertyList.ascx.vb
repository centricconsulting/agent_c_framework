Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.FARM

Public Class ctlPersonalPropertyList
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Public Event FarmPropertyExist(state As Boolean)

    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject Implements IVRUI_P.Quote
        Get
            'Dim errCreateQSO As String = ""
            'If IsAppPageMode Then
            '    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.Farm, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            'Else
            '    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.Farm)
            'End If
            Return New QuickQuote.CommonObjects.QuickQuoteObject
        End Get
    End Property

    Protected ReadOnly Property QuoteId As String Implements IVRUI_P.QuoteId
        Get
            If Request.QueryString("quoteid") IsNot Nothing Then
                Return Request.QueryString("quoteid")
            End If
            If Page.RouteData.Values("quoteid") IsNot Nothing Then
                Return Page.RouteData.Values("quoteid").ToString()
            End If
            Return ""
        End Get
    End Property

    Public Property LocalQuickQuote() As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            Return Session("sess_LocalQuickQuote")
        End Get
        Set(ByVal value As QuickQuote.CommonObjects.QuickQuoteObject)
            Session("sess_LocalQuickQuote") = value
        End Set
    End Property

    Public Property ValidationHelper As ControlValidationHelper Implements IVRUI_P.ValidationHelper
        Get
            If ViewState("vs_valHelp") Is Nothing Then
                ViewState("vs_valHelp") = New ControlValidationHelper
            End If
            Return DirectCast(ViewState("vs_valHelp"), ControlValidationHelper)
        End Get
        Set(value As ControlValidationHelper)
            ViewState("vs_valHelp") = value
        End Set
    End Property

    Public ReadOnly Property PersPropQA() As String
        Get
            Return Session("sess_PersPropQA")
        End Get
    End Property

    Public ReadOnly Property IsAppPageMode As Boolean
        Get
            If PersPropQA = "App" Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public Property PersPropList() As DataTable
        Get
            Return ViewState("vs_PersProp")
        End Get
        Set(ByVal value As DataTable)
            ViewState("vs_PersProp") = value
        End Set
    End Property

    Public Property PeakSeasonList() As DataTable
        Get
            Return ViewState("vs_PeakSeason")
        End Get
        Set(ByVal value As DataTable)
            ViewState("vs_PeakSeason") = value
        End Set
    End Property

    Public Property PersPropLineCount() As Integer
        Get
            Return Session("sess_PersPropLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_PersPropLineCnt") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Populate()
        End If
    End Sub

    Public Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Private Function CreatePersPropDataTable() As DataTable
        Dim dtPersProp As New DataTable
        dtPersProp.Columns.Add("CoverageName", GetType(String))
        dtPersProp.Columns.Add("Description", GetType(String))
        dtPersProp.Columns.Add("Earthquake", GetType(String))
        dtPersProp.Columns.Add("Limit", GetType(String))
        dtPersProp.Columns.Add("Deductible", GetType(String))
        dtPersProp.Columns.Add("Premium", GetType(String))
        dtPersProp.Columns.Add("PersPropRowNum", GetType(String))

        Return dtPersProp
    End Function

    Private Function CreatePeakSeasonCoverageDataTable() As DataTable
        Dim dtPeakSeason As New DataTable
        dtPeakSeason.Columns.Add("Description", GetType(String))
        dtPeakSeason.Columns.Add("StartDate", GetType(String))
        dtPeakSeason.Columns.Add("EndDate", GetType(String))
        dtPeakSeason.Columns.Add("Limit", GetType(String))
        dtPeakSeason.Columns.Add("Premium", GetType(String))
        dtPeakSeason.Columns.Add("PersPropRowNum", GetType(String))
        dtPeakSeason.Columns.Add("PeakRowNum", GetType(String))

        Return dtPeakSeason
    End Function

    Public Sub Populate() Implements IVRUI_P.Populate
        If LocalQuickQuote.Locations(0).ProgramTypeId = "6" Or LocalQuickQuote.Locations(0).ProgramTypeId = "7" Then
            TogglePersProp(False)
            dlPersProp.DataSource = Nothing
            PersPropList = CreatePersPropDataTable()
            PeakSeasonList = CreatePeakSeasonCoverageDataTable()
            Dim blnktPersPropIdx As Integer = 0
            Dim totalPrem As Decimal = 0

            If LocalQuickQuote IsNot Nothing Then

                Dim parts = Me.qqHelper.MultiStateQuickQuoteObjects(LocalQuickQuote)
                Dim SubQuoteFirst As QuickQuoteObject
                If parts IsNot Nothing Then
                    SubQuoteFirst = parts.GetItemAtIndex(0)
                End If


                Dim GovStQt As QuickQuote.CommonObjects.QuickQuoteObject = IFM.VR.Common.Helpers.MultiState.General.GoverningStateQuote(LocalQuickQuote)
                If GovStQt IsNot Nothing Then
                    If GovStQt.ScheduledPersonalPropertyCoverages IsNot Nothing AndAlso GovStQt.ScheduledPersonalPropertyCoverages.Count > 0 Then
                        Dim persPropIdx As Integer = 0
                        For Each persProp As QuickQuoteScheduledPersonalPropertyCoverage In GovStQt.ScheduledPersonalPropertyCoverages
                            Dim lim As String = persProp.IncreasedLimit
                            Dim CovPremMain As String = Nothing
                            If lim IsNot Nothing AndAlso lim.ToString <> String.Empty Then
                                CovPremMain = SumScheduledPersonalPropertyPremium(LocalQuickQuote, "MAIN", persProp.Description.ToUpper, lim)
                            Else
                                CovPremMain = SumScheduledPersonalPropertyPremium(LocalQuickQuote, "MAIN", persProp.Description.ToUpper)
                            End If
                            Dim CovPremEQ As String = SumScheduledPersonalPropertyPremium(LocalQuickQuote, "EQ", persProp.Description)
                            Dim coveragePrem As String = (CDec(CovPremMain) + CDec(CovPremEQ)).ToString
                            'Dim coveragePrem As Integer = CDec(CovPremMain) + CDec(CovPremEQ)
                            ''Dim coveragePrem As Integer = Decimal.Parse(persProp.MainCoveragePremium.Replace("$", "").Replace(",", "")) + Decimal.Parse(persProp.EarthquakePremium.Replace("$", "").Replace(",", ""))
                            If FarmExtenderHelper.IsFarmExtenderAvailable(LocalQuickQuote) AndAlso persProp.CoverageType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_Rented_or_borrowed_Equipment AndAlso SubQuoteFirst IsNot Nothing AndAlso SubQuoteFirst.HasFarmExtender Then
                                If IsNumeric(coveragePrem) AndAlso (coveragePrem = "0" OrElse coveragePrem = ".00" OrElse coveragePrem = "0.00") Then
                                    coveragePrem = "Included"
                                Else
                                    coveragePrem = IFM.Common.InputValidation.InputHelpers.TryToFormatAsCurrency(coveragePrem.ToString(), True)
                                End If                               
                            Else
                                coveragePrem = IFM.Common.InputValidation.InputHelpers.TryToFormatAsCurrency(coveragePrem.ToString(), True)
                            End If
                            PersPropList.Rows.Add(persProp.CoverageType, persProp.Description, persProp.HasEarthquakeCoverage, lim, GovStQt.Farm_F_and_G_DeductibleLimitId, coveragePrem, persPropIdx)
                            'totalPrem = totalPrem + Decimal.Parse(persProp.TotalPremium.Replace("$", "").Replace(",", ""))
                            If coveragePrem <> "Included" Then
                                totalPrem += CDec(coveragePrem)
                            End If
                            'totalPrem += coveragePrem
                            PersPropLineCount += 1

                            If persProp.PeakSeasons IsNot Nothing Then
                                Dim peakSeasonIdx As Integer = 0
                                For Each peakSeason As QuickQuotePeakSeason In persProp.PeakSeasons
                                    PeakSeasonList.Rows.Add(peakSeason.Description, peakSeason.EffectiveDate, peakSeason.ExpirationDate, peakSeason.IncreasedLimit, peakSeason.Premium, persPropIdx, peakSeasonIdx)
                                    peakSeasonIdx += 1
                                    PersPropLineCount += 1
                                Next
                            End If

                            persPropIdx += 1
                        Next

                        blnktPersPropIdx = persPropIdx
                        TogglePersProp(True)
                    End If

                    ' The optional coverages and unscheduled personal property coverages need to read from the Governing State part instead of the State Parts MGB 2-1-19 Bug 31175
                    If GovStQt.OptionalCoverages IsNot Nothing AndAlso GovStQt.OptionalCoverages.Count > 0 Then
                        Dim optPersPropIdx As Integer = blnktPersPropIdx

                        For Each optProp As QuickQuoteOptionalCoverage In GovStQt.OptionalCoverages
                            If optProp.CoverageType <> QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense Then
                                Dim CovPrem As String = SumOptionalCoveragePremium(LocalQuickQuote, optProp.CoverageCodeId)
                                'PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, False, optProp.IncreasedLimit, "", optProp.Premium, optPersPropIdx)
                                If optProp.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Cattle _
                                    OrElse optProp.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Equine _
                                    OrElse optProp.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Poultry _
                                    OrElse optProp.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Swine Then
                                    ' For suffocation of livestock we show one line for each animal type and building
                                    Dim LocBldgTags As New List(Of String)
                                    Dim L As Integer = 0
                                    Dim B As Integer = 0
                                    If LocalQuickQuote.Locations IsNot Nothing AndAlso LocalQuickQuote.Locations.Count > 0 Then
                                        For Each LOC As QuickQuoteLocation In LocalQuickQuote.Locations
                                            L += 1
                                            B = 0
                                            If LOC.Buildings IsNot Nothing AndAlso LOC.Buildings.Count > 0 Then
                                                B += 1
                                                LocBldgTags.Add(L.ToString & "." & B.ToString)
                                            End If
                                        Next
                                    End If
                                    Dim ndx As Integer = -1
                                    For Each LB As String In LocBldgTags
                                        ndx += 1
                                        If ndx = 0 Then
                                            ' Only show the premium on the first one in the list
                                            'PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, LB, optProp.IncreasedLimit, "", Format(CDec(CovPrem), "c"), optPersPropIdx)
                                            PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, "", optProp.IncreasedLimit, "", Format(CDec(CovPrem), "c"), optPersPropIdx)
                                            optPersPropIdx += 1
                                            PersPropLineCount += 1
                                        Else
                                            'PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, LB, optProp.IncreasedLimit, "", "Included", optPersPropIdx)
                                            PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, "", optProp.IncreasedLimit, "", "Included", optPersPropIdx)
                                            optPersPropIdx += 1
                                            PersPropLineCount += 1
                                        End If
                                    Next
                                Else
                                    PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, False, optProp.IncreasedLimit, "", Format(CDec(CovPrem), "c"), optPersPropIdx)
                                    optPersPropIdx += 1
                                    PersPropLineCount += 1
                                End If
                                'totalPrem = totalPrem + Decimal.Parse(optProp.Premium.Replace("$", "").Replace(",", ""))
                                totalPrem += CovPrem
                                'optPersPropIdx += 1  MGB
                                'PersPropLineCount += 1  MGB
                            End If
                        Next

                        blnktPersPropIdx = optPersPropIdx
                        TogglePersProp(True)
                    End If

                    If GovStQt.UnscheduledPersonalPropertyCoverage IsNot Nothing AndAlso GovStQt.UnscheduledPersonalPropertyCoverage.IncreasedLimit <> "" AndAlso (Not qqHelper.IsZeroAmount(GovStQt.UnscheduledPersonalPropertyCoverage.IncreasedLimit)) Then
                        Dim coveragePrem As Integer = Decimal.Parse(GovStQt.UnscheduledPersonalPropertyCoverage.MainCoveragePremium.Replace("$", "").Replace(",", "")) + Decimal.Parse(GovStQt.UnscheduledPersonalPropertyCoverage.EarthquakePremium.Replace("$", "").Replace(",", ""))
                        PersPropList.Rows.Add("UnschedBlnkt", GovStQt.UnscheduledPersonalPropertyCoverage.Description, GovStQt.UnscheduledPersonalPropertyCoverage.HasEarthquakeCoverage, GovStQt.UnscheduledPersonalPropertyCoverage.IncreasedLimit, GovStQt.Farm_F_and_G_DeductibleLimitId, IFM.Common.InputValidation.InputHelpers.TryToFormatAsCurrency(coveragePrem, True), blnktPersPropIdx)
                        totalPrem = totalPrem + Decimal.Parse(GovStQt.UnscheduledPersonalPropertyCoverage.TotalPremium.Replace("$", "").Replace(",", ""))
                        PersPropLineCount += 1

                        If GovStQt.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
                            Dim peakSeasonIdx As Integer = 0
                            For Each peakSeason As QuickQuotePeakSeason In GovStQt.UnscheduledPersonalPropertyCoverage.PeakSeasons
                                PeakSeasonList.Rows.Add(peakSeason.Description, peakSeason.EffectiveDate, peakSeason.ExpirationDate, peakSeason.IncreasedLimit, peakSeason.Premium, blnktPersPropIdx, peakSeasonIdx)
                                peakSeasonIdx += 1
                                PersPropLineCount += 1
                            Next
                        End If
                        TogglePersProp(True)
                    End If
                End If
             
       
                
                ''Updated 9/10/18 for multi state MLW LocalQuickQuote to SubQuoteFirst
                'Dim parts = Me.qqHelper.MultiStateQuickQuoteObjects(LocalQuickQuote)
                'If parts IsNot Nothing Then
                '    Dim SubQuoteFirst = parts.GetItemAtIndex(0)
                '    If SubQuoteFirst IsNot Nothing Then
                '        If SubQuoteFirst.ScheduledPersonalPropertyCoverages IsNot Nothing AndAlso SubQuoteFirst.ScheduledPersonalPropertyCoverages.Count > 0 Then
                '            Dim persPropIdx As Integer = 0
                '            For Each persProp As QuickQuoteScheduledPersonalPropertyCoverage In SubQuoteFirst.ScheduledPersonalPropertyCoverages
                '                Dim lim As String = persProp.IncreasedLimit
                '                Dim CovPremMain As String = Nothing
                '                If lim IsNot Nothing AndAlso lim.ToString <> String.Empty Then
                '                    CovPremMain = SumScheduledPersonalPropertyPremium(LocalQuickQuote, "MAIN", persProp.Description.ToUpper, lim)
                '                Else
                '                    CovPremMain = SumScheduledPersonalPropertyPremium(LocalQuickQuote, "MAIN", persProp.Description.ToUpper)
                '                End If
                '                Dim CovPremEQ As String = SumScheduledPersonalPropertyPremium(LocalQuickQuote, "EQ", persProp.Description)
                '                Dim coveragePrem As Integer = CDec(CovPremMain) + CDec(CovPremEQ)
                '                'Dim coveragePrem As Integer = Decimal.Parse(persProp.MainCoveragePremium.Replace("$", "").Replace(",", "")) + Decimal.Parse(persProp.EarthquakePremium.Replace("$", "").Replace(",", ""))
                '                PersPropList.Rows.Add(persProp.CoverageType, persProp.Description, persProp.HasEarthquakeCoverage, persProp.IncreasedLimit, SubQuoteFirst.Farm_F_and_G_DeductibleLimitId, IFM.Common.InputValidation.InputHelpers.TryToFormatAsCurrency(coveragePrem.ToString(), True), persPropIdx)
                '                'totalPrem = totalPrem + Decimal.Parse(persProp.TotalPremium.Replace("$", "").Replace(",", ""))
                '                totalPrem += coveragePrem
                '                PersPropLineCount += 1

                '                If persProp.PeakSeasons IsNot Nothing Then
                '                    Dim peakSeasonIdx As Integer = 0
                '                    For Each peakSeason As QuickQuotePeakSeason In persProp.PeakSeasons
                '                        PeakSeasonList.Rows.Add(peakSeason.Description, peakSeason.EffectiveDate, peakSeason.ExpirationDate, peakSeason.IncreasedLimit, peakSeason.Premium, persPropIdx, peakSeasonIdx)
                '                        peakSeasonIdx += 1
                '                        PersPropLineCount += 1
                '                    Next
                '                End If

                '                persPropIdx += 1
                '            Next

                '            blnktPersPropIdx = persPropIdx
                '            TogglePersProp(True)
                '        End If

                '        Dim GovStQt As QuickQuote.CommonObjects.QuickQuoteObject = IFM.VR.Common.Helpers.MultiState.General.GoverningStateQuote(LocalQuickQuote)
                '        If GovStQt IsNot Nothing Then
                '            ' The optional coverages and unscheduled personal property coverages need to read from the Governing State part instead of the State Parts MGB 2-1-19 Bug 31175
                '            If GovStQt.OptionalCoverages IsNot Nothing AndAlso GovStQt.OptionalCoverages.Count > 0 Then
                '                Dim optPersPropIdx As Integer = blnktPersPropIdx

                '                For Each optProp As QuickQuoteOptionalCoverage In GovStQt.OptionalCoverages
                '                    If optProp.CoverageType <> QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense Then
                '                        Dim CovPrem As String = SumOptionalCoveragePremium(LocalQuickQuote, optProp.CoverageCodeId)
                '                        'PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, False, optProp.IncreasedLimit, "", optProp.Premium, optPersPropIdx)
                '                        If optProp.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Cattle _
                '                            OrElse optProp.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Equine _
                '                            OrElse optProp.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Poultry _
                '                            OrElse optProp.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock_Swine Then
                '                            ' For suffocation of livestock we show one line for each animal type and building
                '                            Dim LocBldgTags As New List(Of String)
                '                            Dim L As Integer = 0
                '                            Dim B As Integer = 0
                '                            If LocalQuickQuote.Locations IsNot Nothing AndAlso LocalQuickQuote.Locations.Count > 0 Then
                '                                For Each LOC As QuickQuoteLocation In LocalQuickQuote.Locations
                '                                    L += 1
                '                                    B = 0
                '                                    If LOC.Buildings IsNot Nothing AndAlso LOC.Buildings.Count > 0 Then
                '                                        B += 1
                '                                        LocBldgTags.Add(L.ToString & "." & B.ToString)
                '                                    End If
                '                                Next
                '                            End If
                '                            Dim ndx As Integer = -1
                '                            For Each LB As String In LocBldgTags
                '                                ndx += 1
                '                                If ndx = 0 Then
                '                                    ' Only show the premium on the first one in the list
                '                                    'PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, LB, optProp.IncreasedLimit, "", Format(CDec(CovPrem), "c"), optPersPropIdx)
                '                                    PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, "", optProp.IncreasedLimit, "", Format(CDec(CovPrem), "c"), optPersPropIdx)
                '                                    optPersPropIdx += 1
                '                                    PersPropLineCount += 1
                '                                Else
                '                                    'PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, LB, optProp.IncreasedLimit, "", "Included", optPersPropIdx)
                '                                    PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, "", optProp.IncreasedLimit, "", "Included", optPersPropIdx)
                '                                    optPersPropIdx += 1
                '                                    PersPropLineCount += 1
                '                                End If
                '                            Next
                '                        Else
                '                            PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, False, optProp.IncreasedLimit, "", Format(CDec(CovPrem), "c"), optPersPropIdx)
                '                            optPersPropIdx += 1
                '                            PersPropLineCount += 1
                '                        End If
                '                        'totalPrem = totalPrem + Decimal.Parse(optProp.Premium.Replace("$", "").Replace(",", ""))
                '                        totalPrem += CovPrem
                '                        'optPersPropIdx += 1  MGB
                '                        'PersPropLineCount += 1  MGB
                '                    End If
                '                Next

                '                blnktPersPropIdx = optPersPropIdx
                '                TogglePersProp(True)
                '            End If

                '            If GovStQt.UnscheduledPersonalPropertyCoverage IsNot Nothing AndAlso GovStQt.UnscheduledPersonalPropertyCoverage.IncreasedLimit <> "" AndAlso (Not qqHelper.IsZeroAmount(GovStQt.UnscheduledPersonalPropertyCoverage.IncreasedLimit)) Then
                '                Dim coveragePrem As Integer = Decimal.Parse(GovStQt.UnscheduledPersonalPropertyCoverage.MainCoveragePremium.Replace("$", "").Replace(",", "")) + Decimal.Parse(GovStQt.UnscheduledPersonalPropertyCoverage.EarthquakePremium.Replace("$", "").Replace(",", ""))
                '                PersPropList.Rows.Add("UnschedBlnkt", GovStQt.UnscheduledPersonalPropertyCoverage.Description, GovStQt.UnscheduledPersonalPropertyCoverage.HasEarthquakeCoverage, GovStQt.UnscheduledPersonalPropertyCoverage.IncreasedLimit, GovStQt.Farm_F_and_G_DeductibleLimitId, IFM.Common.InputValidation.InputHelpers.TryToFormatAsCurrency(coveragePrem, True), blnktPersPropIdx)
                '                totalPrem = totalPrem + Decimal.Parse(GovStQt.UnscheduledPersonalPropertyCoverage.TotalPremium.Replace("$", "").Replace(",", ""))
                '                PersPropLineCount += 1

                '                If GovStQt.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
                '                    Dim peakSeasonIdx As Integer = 0
                '                    For Each peakSeason As QuickQuotePeakSeason In GovStQt.UnscheduledPersonalPropertyCoverage.PeakSeasons
                '                        PeakSeasonList.Rows.Add(peakSeason.Description, peakSeason.EffectiveDate, peakSeason.ExpirationDate, peakSeason.IncreasedLimit, peakSeason.Premium, blnktPersPropIdx, peakSeasonIdx)
                '                        peakSeasonIdx += 1
                '                        PersPropLineCount += 1
                '                    Next
                '                End If
                '                TogglePersProp(True)
                '            End If
                '        End If

                '        'If SubQuoteFirst.OptionalCoverages IsNot Nothing AndAlso SubQuoteFirst.OptionalCoverages.Count > 0 Then
                '        '    Dim optPersPropIdx As Integer = blnktPersPropIdx

                '        '    For Each optProp As QuickQuoteOptionalCoverage In SubQuoteFirst.OptionalCoverages
                '        '        If optProp.CoverageType <> QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense Then
                '        '            Dim CovPrem As String = SumOptionalCoveragePremium(LocalQuickQuote, optProp.CoverageCodeId)
                '        '            'PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, False, optProp.IncreasedLimit, "", optProp.Premium, optPersPropIdx)
                '        '            PersPropList.Rows.Add(optProp.CoverageType, optProp.Description, False, optProp.IncreasedLimit, "", Format(CDec(CovPrem), "c"), optPersPropIdx)
                '        '            'totalPrem = totalPrem + Decimal.Parse(optProp.Premium.Replace("$", "").Replace(",", ""))
                '        '            totalPrem += CovPrem
                '        '            optPersPropIdx += 1
                '        '            PersPropLineCount += 1
                '        '        End If
                '        '    Next

                '        '    blnktPersPropIdx = optPersPropIdx
                '        '    TogglePersProp(True)
                '        'End If

                '        'If SubQuoteFirst.UnscheduledPersonalPropertyCoverage IsNot Nothing AndAlso SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IncreasedLimit <> "" AndAlso (Not qqHelper.IsZeroAmount(SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IncreasedLimit)) Then
                '        '    Dim coveragePrem As Integer = Decimal.Parse(SubQuoteFirst.UnscheduledPersonalPropertyCoverage.MainCoveragePremium.Replace("$", "").Replace(",", "")) + Decimal.Parse(SubQuoteFirst.UnscheduledPersonalPropertyCoverage.EarthquakePremium.Replace("$", "").Replace(",", ""))
                '        '    PersPropList.Rows.Add("UnschedBlnkt", SubQuoteFirst.UnscheduledPersonalPropertyCoverage.Description, SubQuoteFirst.UnscheduledPersonalPropertyCoverage.HasEarthquakeCoverage, SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IncreasedLimit, SubQuoteFirst.Farm_F_and_G_DeductibleLimitId, IFM.Common.InputValidation.InputHelpers.TryToFormatAsCurrency(coveragePrem, True), blnktPersPropIdx)
                '        '    totalPrem = totalPrem + Decimal.Parse(SubQuoteFirst.UnscheduledPersonalPropertyCoverage.TotalPremium.Replace("$", "").Replace(",", ""))
                '        '    PersPropLineCount += 1

                '        '    If SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing Then
                '        '        Dim peakSeasonIdx As Integer = 0
                '        '        For Each peakSeason As QuickQuotePeakSeason In SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons
                '        '            PeakSeasonList.Rows.Add(peakSeason.Description, peakSeason.EffectiveDate, peakSeason.ExpirationDate, peakSeason.IncreasedLimit, peakSeason.Premium, blnktPersPropIdx, peakSeasonIdx)
                '        '            peakSeasonIdx += 1
                '        '            PersPropLineCount += 1
                '        '        Next
                '        '    End If
                '        '    TogglePersProp(True)
                '        'End If
                '    End If
                'End If

                If PersPropLineCount = 0 Then
                    PersPropLineCount = 1
                End If

                dlPersProp.DataSource = PersPropList
                dlPersProp.DataBind()

                lblTotalPremData.Text = IFM.Common.InputValidation.InputHelpers.TryToFormatAsCurrency(totalPrem.ToString(), True)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Sums up the premium across states for the passed optional coverage
    ''' </summary>
    ''' <param name="CovCodeId"></param>
    ''' <returns></returns>
    Private Function SumOptionalCoveragePremium(ByRef TopQuote As QuickQuote.CommonObjects.QuickQuoteObject, ByVal CovCodeId As String) As String
        Dim tot As Decimal = 0

        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In IFM.VR.Common.Helpers.MultiState.SubQuotes(TopQuote)
            If sq.OptionalCoverages IsNot Nothing Then
                For Each oc As QuickQuote.CommonObjects.QuickQuoteOptionalCoverage In sq.OptionalCoverages
                    If oc.CoverageCodeId = CovCodeId Then
                        If IsNumeric(oc.Premium) Then tot += CDec(oc.Premium)
                        Exit For
                    End If
                Next
            End If
        Next

        Return tot.ToString
    End Function

    ''' <summary>
    ''' Sums the premium for the passed Scheduled Personal Property coverage
    ''' Can either be MAIN or EQ
    ''' </summary>
    ''' <param name="MainOrEQ"></param>
    ''' <returns></returns>
    Private Function SumScheduledPersonalPropertyPremium(ByRef TopQuote As QuickQuote.CommonObjects.QuickQuoteObject, ByVal MainOrEQ As String, ByVal CovDesc As String, Optional ByVal CovLimit As String = Nothing) As String
        Dim tot As Decimal = 0

        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In IFM.VR.Common.Helpers.MultiState.SubQuotes(TopQuote)
            If sq.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                If MainOrEQ.ToUpper = "MAIN" Then
                    For Each spp As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage In sq.ScheduledPersonalPropertyCoverages
                        If CovLimit IsNot Nothing AndAlso CovLimit.Trim <> String.Empty Then
                            If spp.Description.ToUpper = CovDesc.ToUpper AndAlso spp.IncreasedLimit = CovLimit Then
                                If IsNumeric(spp.MainCoveragePremium) Then tot += spp.MainCoveragePremium
                            End If
                        Else
                            If spp.Description.ToUpper = CovDesc.ToUpper Then
                                If IsNumeric(spp.MainCoveragePremium) Then tot += spp.MainCoveragePremium
                            End If
                        End If
                    Next
                ElseIf MainOrEQ.ToUpper = "EQ" Then
                    For Each spp As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage In sq.ScheduledPersonalPropertyCoverages
                        If spp.Description.ToUpper = CovDesc.ToUpper Then
                            If IsNumeric(spp.EarthquakePremium) Then tot += spp.EarthquakePremium
                        End If
                    Next
                End If
            End If
        Next

        Return tot.ToString
    End Function

    Private Sub TogglePersProp(state As Boolean)
        RaiseEvent FarmPropertyExist(state)
    End Sub

    Private Sub dlPersProp_ItemDataBound(sender As Object, e As DataListItemEventArgs) Handles dlPersProp.ItemDataBound
        'Updated 9/10/18 for multi state MLW
        'If LocalQuickQuote IsNot Nothing AndAlso LocalQuickQuote.ScheduledPersonalPropertyCoverages IsNot Nothing Then
        Dim parts = Me.qqHelper.MultiStateQuickQuoteObjects(LocalQuickQuote)
        If parts IsNot Nothing Then
            Dim SubQuoteFirst = parts.GetItemAtIndex(0)
            If SubQuoteFirst IsNot Nothing Then
                Dim persProp As ctlPersonalProperty = e.Item.FindControl("ctlPersonalProperty")
                persProp.RowNumber = e.Item.ItemIndex
                persProp.PersPropList = PersPropList
                persProp.PeakSeasonList = PeakSeasonList
                persProp.Populate()
            End If
        End If
    End Sub

    Public Function Save() As Boolean Implements IVRUI_P.Save
        Return False
    End Function

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm

    End Sub
End Class