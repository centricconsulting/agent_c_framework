Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports PdfSharp.Pdf.Content.Objects

Public Class ctlDwelling
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Dim QuickQuote As QuickQuoteObject

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

    Public ReadOnly Property CurrentForm As String
        Get
            Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, LocalQuickQuote.Locations(RowNumber).FormTypeId)
        End Get
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

    Public ReadOnly Property IsAppPageMode As Boolean
        Get
            If TypeOf Me.Page Is VR3FarmApp Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public Property DwellingLineCount() As Integer
        Get
            Return Session("sess_DwellingLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_DwellingLineCnt") = value
        End Set
    End Property

    Public Property AddlDwellingLineCount() As Integer
        Get
            Return Session("sess_AddlDwellingLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_AddlDwellingLineCnt") = value
        End Set
    End Property

    Public Property UseAddlDwelling() As Boolean
        Get
            Return ViewState("vs_UseAddlDwelling")
            'Return Session("sess_UseAddlDwelling")
        End Get
        Set(ByVal value As Boolean)
            ViewState("vs_UseAddlDwelling") = value
            'Session("sess_UseAddlDwelling") = value
        End Set
    End Property

    Private Const FO2ANDFO3DefaultPrivatePowerPolesIncludedLimit = "1,500"
    Private Const FO5DefaultPrivatePowerPolesIncludedLimit = "2,500"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack AndAlso RowNumber = 0 Then
            LoadStaticData()
            Populate()
        End If
    End Sub

    Protected Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate
        '
        ' Dwelling
        '
        If RowNumber = 0 Then
            Me.lblAddlDwelling.Visible = False
        End If
        Me.lblAddlDwelling.Text = String.Format("{0} {1}", "Dwelling Property Coverage - loc", RowNumber + 1)

        If LocalQuickQuote.Locations(0).ProgramTypeId <> "6" And LocalQuickQuote.Locations(0).ProgramTypeId <> "8" Then
            DwellingLineCount = 0
            AddlDwellingLineCount = 0
        Else
            If LocalQuickQuote.Locations(RowNumber).FormTypeId <> "" And LocalQuickQuote.Locations(RowNumber).FormTypeId <> "13" Then
                With LocalQuickQuote.Locations(RowNumber)
                    lblAddress.Text = String.Format("{0} {1}", .Address.HouseNum, .Address.StreetName)
                    lblCityState.Text = String.Format("{0} {1}", .Address.City, .Address.Zip)

                    lblFormData.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, .FormTypeId)
                    lblDeductibleData.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, .DeductibleLimitId)
                    lblConstructionData.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId, .ConstructionTypeId)
                    lblYearBuiltData.Text = .YearBuilt
                    lblSqFeetData.Text = .SquareFeet
                    lblStructData.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.StructureTypeId, .StructureTypeId)

                    If Not UseAddlDwelling Then
                        DwellingLineCount += 6
                    Else
                        AddlDwellingLineCount += 6
                    End If

                    'Base Coverages
                    Select Case lblFormData.Text
                        Case "FO-4"
                            HideCoverageA()
                            HideCoverageB()
                            ShowCoverageC()
                            ShowCoverageD(CurrentForm)

                            If Not UseAddlDwelling Then
                                DwellingLineCount += 2
                            Else
                                AddlDwellingLineCount += 2
                            End If
                        Case Else
                            ShowCoverageA()
                            ShowCoverageB()
                            ShowCoverageC()
                            ShowCoverageD(CurrentForm)

                            If Not UseAddlDwelling Then
                                DwellingLineCount += 4
                            Else
                                AddlDwellingLineCount += 4
                            End If
                    End Select

                    ' Private Power/Light Poles
                    'Do not show Private Power/Light Poles on PF when FO-4 is selected
                    If Common.Helpers.FARM.PrivatePowerPolesHelper.IsPrivatePowerPolesAvailable(LocalQuickQuote) AndAlso LocalQuickQuote.Locations(RowNumber).FormTypeId <> "17" Then
                        If LocalQuickQuote.Locations(RowNumber).IncidentalDwellingCoverages IsNot Nothing Then
                            Dim PrivatePowerPoles As QuickQuoteCoverage
                            PrivatePowerPoles = LocalQuickQuote.Locations(RowNumber).IncidentalDwellingCoverages.Find(Function(p) p.CoverageCodeId = "70145")
                            If PrivatePowerPoles Is Nothing Then
                                PrivatePowerPoles = New QuickQuoteCoverage
                                If LocalQuickQuote.Locations(RowNumber).FormTypeId = "15" OrElse LocalQuickQuote.Locations(RowNumber).FormTypeId = "16" Then
                                    PrivatePowerPoles.ManualLimitAmount = FO2ANDFO3DefaultPrivatePowerPolesIncludedLimit
                                Else
                                    PrivatePowerPoles.ManualLimitAmount = FO5DefaultPrivatePowerPolesIncludedLimit
                                End If
                                PrivatePowerPoles.ManualLimitIncreased = 0
                            End If
                            tblPrivatePowerPoles.Visible = True
                            lblPrivatePowerPoles_Limit.Text = Format(CDec(PrivatePowerPoles.ManualLimitAmount), "###,###,###")
                            If qqHelper.IsPositiveIntegerString(PrivatePowerPoles.ManualLimitIncreased) Then
                                lblPrivatePowerPoles_Prem.Text = Format(CDec(PrivatePowerPoles.FullTermPremium), "$###,###,##0.00")
                            Else
                                lblPrivatePowerPoles_Prem.Text = "Included"
                            End If

                            If Not UseAddlDwelling Then
                                DwellingLineCount += 1
                            Else
                                AddlDwellingLineCount += 1
                            End If
                        End If
                    End If

                    ' Other Dwelling Coverages
                    If .SectionICoverages IsNot Nothing Then
                        For Each sc As QuickQuoteSectionICoverage In .SectionICoverages
                            Select Case sc.CoverageType
                                Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Replacement_Value_Personal_Property_Cov_C_
                                    tblReplaceCost.Visible = True
                                    lblReplaceCost_Limit.Text = If(sc.IncludedLimit = "0", "N/A", sc.IncludedLimit)
                                    lblReplaceCost_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblReplaceCost_Prem.Text = sc.Premium
                                    Else
                                        lblReplaceCost_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB
                                    tblMineSubCost.Visible = True
                                    If .Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                                        If .A_Dwelling_Limit IsNot Nothing AndAlso IsNumeric(.A_Dwelling_Limit) Then
                                            If CDec(.A_Dwelling_Limit) < 300000 Then
                                                lblMineSubCost_Limit.Text = .A_Dwelling_Limit
                                            Else
                                                lblMineSubCost_Limit.Text = "300,000"
                                            End If
                                        End If
                                    Else
                                        lblMineSubCost_Limit.Text = If(sc.IncludedLimit = "0", "N/A", sc.IncludedLimit)
                                    End If
                                    lblMineSubCost_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblMineSubCost_Prem.Text = sc.Premium
                                    Else
                                        lblMineSubCost_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Business_Property_Increased_Limits
                                    tblBusiness.Visible = True
                                    lblBusiness_Limit.Text = If(sc.TotalLimit = "0", "N/A", sc.TotalLimit)
                                    lblBusiness_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblBusiness_Prem.Text = sc.Premium
                                    Else
                                        lblBusiness_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Money_Increased_Limits
                                    tblMoney.Visible = True
                                    lblMoney_Limit.Text = If(sc.TotalLimit = "0", "N/A", sc.TotalLimit)
                                    lblMoney_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblMoney_Prem.Text = sc.Premium
                                    Else
                                        lblMoney_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Securities_Increased_Limits
                                    tblSecurities.Visible = True
                                    lblSecurities_Limit.Text = If(sc.TotalLimit = "0", "N/A", sc.TotalLimit)
                                    lblSecurities_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblSecurities_Prem.Text = sc.Premium
                                    Else
                                        lblSecurities_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Silverware_Goldware_and_Pewterware_Increased_Limits
                                    tblSilverware.Visible = True
                                    lblSilverware_Limit.Text = If(sc.TotalLimit = "0", "N/A", sc.TotalLimit)
                                    lblSilverware_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblSilverware_Prem.Text = sc.Premium
                                    Else
                                        lblSilverware_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Guns_Increased_Limits
                                    tblGuns.Visible = True
                                    lblGuns_Limit.Text = If(sc.TotalLimit = "0", "N/A", sc.TotalLimit)
                                    lblGuns_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblGuns_Prem.Text = sc.Premium
                                    Else
                                        lblGuns_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Jewelry_Increased_Limits
                                    tblJewelry.Visible = True
                                    lblJewelry_Limit.Text = If(sc.TotalLimit = "0", "N/A", sc.TotalLimit)
                                    lblJewelry_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblJewelry_Prem.Text = sc.Premium
                                    Else
                                        lblJewelry_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlement
                                    tblACV.Visible = True
                                    lblACV_Limit.Text = If(sc.IncludedLimit = "0", "N/A", sc.IncludedLimit)
                                    lblACV_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblACV_Prem.Text = sc.Premium
                                    Else
                                        lblACV_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing
                                    tblACVWind.Visible = True
                                    lblACVWind_Limit.Text = If(sc.IncludedLimit = "0", "N/A", sc.IncludedLimit)
                                    lblACVWind_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblACVWind_Prem.Text = sc.Premium
                                    Else
                                        lblACVWind_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Dwelling_Under_Construction_Theft
                                    tblTheft.Visible = True
                                    lblTheft_Limit.Text = If(sc.IncreasedLimit = "0", "N/A", sc.IncreasedLimit)
                                    lblTheft_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblTheft_Prem.Text = sc.Premium
                                    Else
                                        lblTheft_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.Earthquake_Location
                                    tblEarthquake.Visible = True
                                    lblEarthquake_Limit.Text = If(sc.IncludedLimit = "0", "N/A", sc.IncludedLimit)
                                    lblEarthquake_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblEarthquake_Prem.Text = sc.Premium
                                    Else
                                        lblEarthquake_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Expanded_Replacement_Cost
                                    tblExpandReplace.Visible = True
                                    lblExpandReplace_Limit.Text = If(sc.IncludedLimit = "0", "N/A", sc.IncludedLimit)
                                    lblExpandReplace_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblExpandReplace_Prem.Text = sc.Premium
                                    Else
                                        lblExpandReplace_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.FunctionalReplacementCostLossAssessment
                                    tblFunReplace.Visible = True
                                    lblFunReplace_Limit.Text = If(sc.IncludedLimit = "0", "N/A", sc.IncludedLimit)
                                    lblFunReplace_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblFunReplace_Prem.Text = sc.Premium
                                    Else
                                        lblFunReplace_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA
                                    tblMineSub.Visible = True
                                    lblMineSub_Limit.Text = If(sc.IncludedLimit = "0", "N/A", sc.IncludedLimit)
                                    lblMineSub_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblMineSub_Prem.Text = sc.Premium
                                    Else
                                        lblMineSub_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_PrivateStructures_IncreasedLimitsForSpecificStructures
                                    tblRPS.Visible = True
                                    lblRPS_Limit.Text = If(sc.IncreasedLimit = "0", "N/A", sc.IncreasedLimit)
                                    lblRPS_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblRPS_Prem.Text = sc.Premium
                                    Else
                                        lblRPS_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.UndergroundServiceLine
                                    tblUndergroundServiceLine.Visible = True
                                    lblUndergroundServiceLine_Limit.Text = If(sc.IncludedLimit = "0", "N/A", sc.IncludedLimit)
                                    lblUndergroundServiceLine_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblUndergroundServiceLine_Prem.Text = sc.Premium
                                    Else
                                        lblUndergroundServiceLine_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Motorized_Vehicles_Increased_Limits
                                    tblVehicle.Visible = True
                                    lblVehicle_Limit.Text = If(sc.TotalLimit = "0", "N/A", sc.TotalLimit)
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblVehicle_Prem.Text = sc.Premium
                                    Else
                                        lblVehicle_Prem.Text = "Included"
                                    End If
                                Case QuickQuoteSectionICoverage.SectionICoverageType.Cosmetic_Damage_Exclusion
                                    tblCosDamageEx.Visible = True
                                    lblCosDamageEx_Limit.Text = If(sc.TotalLimit = "0", "N/A", sc.TotalLimit)
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblCosDamageEx_Prem.Text = sc.Premium
                                    Else
                                        lblCosDamageEx_Prem.Text = "Included"
                                    End If
                            End Select
                        Next
                    End If

                    If .SectionIICoverages IsNot Nothing Then
                        For Each sc As QuickQuoteSectionIICoverage In .SectionIICoverages
                            Select Case sc.CoverageType
                                Case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Personal_Injury
                                    tblPersInjury.Visible = True
                                    lblPersInjury_Limit.Text = If(sc.IncreasedLimit = "0", "N/A", sc.IncreasedLimit)
                                    lblPersInjury_Prem.Text = sc.Premium
                                    If Not UseAddlDwelling Then
                                        DwellingLineCount += 1
                                    Else
                                        AddlDwellingLineCount += 1
                                    End If

                                    Dim PremInt As Integer = sc.Premium
                                    If PremInt <> 0 Then
                                        lblPersInjury_Prem.Text = sc.Premium
                                    Else
                                        lblPersInjury_Prem.Text = "Included"
                                    End If
                            End Select
                        Next
                    End If
                End With
            Else
                ' Hide Dwelling Information
                dvDwelling.Visible = False
            End If
        End If
    End Sub

    Protected Function Save() As Boolean Implements IVRUI_P.Save
        Return False
    End Function

    Private Sub ShowCoverageA()
        Dim PremiumInt As Integer = 0
        Dim A_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(LocalQuickQuote.Locations(RowNumber).A_Dwelling_LimitIncluded)
        Dim A_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(LocalQuickQuote.Locations(RowNumber).A_Dwelling_LimitIncreased)
        lblCovALimit.Text = (A_included + A_increased).ToString("N0")

        PremiumInt = LocalQuickQuote.Locations(RowNumber).A_Dwelling_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovAPrem.Text = LocalQuickQuote.Locations(RowNumber).A_Dwelling_QuotedPremium
        Else
            lblCovAPrem.Text = "Included"
        End If
    End Sub

    Private Sub HideCoverageA()
        pnlCovADwelling.Visible = False
    End Sub

    Private Sub ShowCoverageB()
        Dim PremiumInt As Integer = 0
        Dim B_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(LocalQuickQuote.Locations(RowNumber).B_OtherStructures_LimitIncluded)
        Dim B_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(LocalQuickQuote.Locations(RowNumber).B_OtherStructures_LimitIncreased)
        'lblCovBLimit.Text = (B_included + B_increased).ToString("N0")
        lblCovBLimit.Text = (B_included).ToString("N0")
        lblCovBPrem.Text = "Included"

        'PremiumInt = LocalQuickQuote.Locations(RowNumber).B_OtherStructures_QuotedPremium
        'If PremiumInt <> 0 Then
        '    lblCovBPrem.Text = LocalQuickQuote.Locations(RowNumber).B_OtherStructures_QuotedPremium
        'Else
        '    lblCovBPrem.Text = "Included"
        'End If
    End Sub

    Private Sub HideCoverageB()
        pnlCovBStruct.Visible = False
    End Sub

    Private Sub ShowCoverageC()
        Dim PremiumInt As Integer = 0
        Dim C_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(LocalQuickQuote.Locations(RowNumber).C_PersonalProperty_LimitIncluded)
        Dim C_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(LocalQuickQuote.Locations(RowNumber).C_PersonalProperty_LimitIncreased)
        lblCovCLimit.Text = (C_included + C_increased).ToString("N0")

        PremiumInt = LocalQuickQuote.Locations(RowNumber).C_PersonalProperty_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovCPrem.Text = LocalQuickQuote.Locations(RowNumber).C_PersonalProperty_QuotedPremium
        Else
            lblCovCPrem.Text = "Included"
        End If
    End Sub

    Private Sub ShowCoverageD(formNum As String)
        Dim PremiumInt As Integer = 0
        Dim D_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(LocalQuickQuote.Locations(RowNumber).D_LossOfUse_LimitIncluded)
        Dim D_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(LocalQuickQuote.Locations(RowNumber).D_LossOfUse_LimitIncreased)

        'If formNum <> "ML-2" And formNum <> "ML-4" Then
        '    lblCovDLimit.Text = "See <sup>2</sup>"
        '    lblCovDPrem.Text = "Included"
        'Else
        lblCovDLimit.Text = (D_included + D_increased).ToString("N0")

        PremiumInt = LocalQuickQuote.Locations(RowNumber).D_LossOfUse_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovDPrem.Text = LocalQuickQuote.Locations(RowNumber).D_LossOfUse_QuotedPremium
        Else
            lblCovDPrem.Text = "Included"
        End If
        'End If
    End Sub

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm

    End Sub
End Class