Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctlPageBreak
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Public Event BreakAddlDwelling(state As Boolean, control As String)
    Public Event BreakBuildings(state As Boolean, control As String)
    Public Event BreakPersProp(state As Boolean, control As String)
    Public Event BreakInlandMarine(state As Boolean, control As String)
    Public Event BreakRVWater(state As Boolean, control As String)
    Public Event BreakAddlCoverage(state As Boolean, control As String)
    Public Event BreakAdjustments(state As Boolean, control As String)
    Public Event BreakSuper(state As Boolean, control As String)
    Public Event BreakPaymentOpt(state As Boolean, control As String)

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

    Public Property PolicyLineCount() As Integer
        Get
            Return Session("sess_PolicyLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_PolicyLineCnt") = value
        End Set
    End Property

    Public Property LocationLineCount() As Integer
        Get
            Return Session("sess_LocationLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_LocationLineCnt") = value
        End Set
    End Property

    Public Property LiabilityLineCount() As Integer
        Get
            Return Session("sess_LiabilityLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_LiabilityLineCnt") = value
        End Set
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

    Public Property BuildingsLineCount() As Integer
        Get
            Return Session("sess_BuildingLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_BuildingLineCnt") = value
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

    Public Property IMLineCount() As Integer
        Get
            Return Session("sess_IMLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_IMLineCnt") = value
        End Set
    End Property

    Public Property RVWaterLineCount() As Integer
        Get
            Return Session("sess_RVWLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_RVWLineCnt") = value
        End Set
    End Property

    Public Property AddlCovLineCount() As Integer
        Get
            Return Session("sess_AddlCoverageLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_AddlCoverageLineCnt") = value
        End Set
    End Property

    Public Property SurLineCount() As Integer
        Get
            Return Session("sess_SurChargeLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_SurChargeLineCnt") = value
        End Set
    End Property

    Public Property CreditLineCount() As Integer
        Get
            Return Session("sess_CreditLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_CreditLineCnt") = value
        End Set
    End Property

    'Public ReadOnly Property LocationList() As List(Of QuickQuoteLocation)
    '    Get
    '        Return Session("sess_LocationList")
    '    End Get
    'End Property

    Public Property ShowIMRV() As Boolean
        Get
            Return Session("sess_IMRV")
        End Get
        Set(ByVal value As Boolean)
            Session("sess_IMRV") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Populate()
        End If
    End Sub

    Public Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate
        Dim linesOnPage As Integer = 55
        AddlCovLineCount += 3
        If LocalQuickQuote.Locations(0).ProgramTypeId = "7" Then
            DwellingLineCount = 0
            PersPropLineCount += 3
        Else
            If LocalQuickQuote.Locations(0).ProgramTypeId = "8" Then
                DwellingLineCount = 0
                BuildingsLineCount = 0
                PersPropLineCount = 0
                IMLineCount = 0
                RVWaterLineCount = 0
            Else
                'DwellingLineCount += 4
                BuildingsLineCount += 3
                PersPropLineCount += 3
                'IMLineCount += 4
                'RVWaterLineCount += 4
            End If
        End If

        'PolicyLineCount = 20
        LiabilityLineCount = 8
        'LocationLineCount = LocalQuickQuote.Locations.Count
        'LocationLineCount += 5

        'Dim primDwelling As Boolean = True
        'Dim addlDwellingCnt As Integer = 0
        'For Each location In LocalQuickQuote.Locations.Where(Function(p) p.FormTypeId <> "" And p.FormTypeId <> "13")
        '    'Find Dwellings
        '    If location.SectionICoverages IsNot Nothing Then
        '        'If primDwelling Then
        '        '    DwellingLineCount += 6
        '        'Else
        '        '    addlDwellingCnt += 6
        '        'End If

        '        If location.FormTypeId = "17" Then
        '            If primDwelling Then
        '                DwellingLineCount += 2
        '            Else
        '                addlDwellingCnt += 2
        '            End If
        '        Else
        '            If primDwelling Then
        '                DwellingLineCount += 4
        '            Else
        '                addlDwellingCnt += 4
        '            End If
        '        End If

        '        For Each sc As QuickQuoteSectionICoverage In location.SectionICoverages
        '            Select Case sc.CoverageType
        '                Case QuickQuoteSectionICoverage.SectionICoverageType.Farm_Replacement_Value_Personal_Property_Cov_C_,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.Farm_Business_Property_Increased_Limits,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.Farm_Money_Increased_Limits,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.Farm_Motorized_Vehicles_Increased_Limits,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.Farm_Securities_Increased_Limits,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.Farm_Silverware_Goldware_and_Pewterware_Increased_Limits,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.Farm_Guns_Increased_Limits,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.Farm_Jewelry_Increased_Limits,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlement,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.Farm_Dwelling_Under_Construction_Theft,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.Earthquake_Location,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.Farm_Expanded_Replacement_Cost,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.FunctionalReplacementCostLossAssessment,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA,
        '                    QuickQuoteSectionICoverage.SectionICoverageType.Cov_B_Related_Private_Structures
        '                    If primDwelling Then
        '                        DwellingLineCount += 1
        '                    Else
        '                        addlDwellingCnt += 1
        '                    End If
        '            End Select
        '        Next
        '    End If

        '    If location.SectionIICoverages IsNot Nothing Then
        '        For Each sc As QuickQuoteSectionIICoverage In location.SectionIICoverages
        '            Select Case sc.CoverageType
        '                Case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Personal_Injury
        '                    If primDwelling Then
        '                        DwellingLineCount += 1
        '                    Else
        '                        addlDwellingCnt += 1
        '                    End If
        '            End Select
        '        Next
        '    End If

        '    ''Find Barns and Buildings
        '    'If location.Buildings IsNot Nothing Then
        '    '    BuildingsLineCount += location.Buildings.Count
        '    'Else
        '    '    BuildingsLineCount += 2
        '    'End If

        '    primDwelling = False
        'Next

        Dim formTotal As Integer = PolicyLineCount + LiabilityLineCount + LocationLineCount + DwellingLineCount
        Dim basicPolicyLineCount = formTotal

        If formTotal <> linesOnPage Then
            If formTotal > (linesOnPage - 1) Then
                'RaiseEvent BreakAddlDwelling(True, "AddlDwelling")
                formTotal -= linesOnPage

                If formTotal < 0 Then
                    formTotal = 0
                End If
            End If

            If (linesOnPage - (formTotal + AddlDwellingLineCount)) < 4 Then
                If AddlDwellingLineCount > 0 Then
                    RaiseEvent BreakBuildings(True, "AddlDwelling")
                    formTotal = AddlDwellingLineCount
                    'Else
                    '    RaiseEvent BreakBuildings(True, "Building")
                End If
            Else
                formTotal += AddlDwellingLineCount
            End If

            If (linesOnPage - (formTotal + BuildingsLineCount)) < 4 Then
                If BuildingsLineCount > 0 Then
                    RaiseEvent BreakBuildings(True, "Building")
                    formTotal = BuildingsLineCount
                    'Else
                    '    RaiseEvent BreakBuildings(True, "Building")
                End If
            Else
                formTotal += BuildingsLineCount
            End If

            'If formTotal = linesOnPage Then
            '    If AddlDwellingLineCount > 0 Then
            '        RaiseEvent BreakBuildings(True, "AddlDwelling")
            '    Else
            '        RaiseEvent BreakBuildings(True, "Building")
            '    End If
            '    formTotal = 0
            'End If
        Else
            formTotal = 0
        End If

        'formTotal += AddlDwellingLineCount
        'If formTotal > linesOnPage Then
        '    formTotal -= linesOnPage
        'End If

        'If formTotal = linesOnPage Then
        '    RaiseEvent BreakBuildings(True, "Building")
        '    formTotal = 0
        'End If

        'formTotal += BuildingsLineCount
        'If formTotal >= linesOnPage Then
        '    RaiseEvent BreakBuildings(True, "Building")
        '    formTotal = BuildingsLineCount
        'End If

        'If formTotal = linesOnPage Then
        '    RaiseEvent BreakPersProp(True, "PersProp")
        '    formTotal = 0
        'End If

        'If BuildingsLineCount > (formTotal - 10) Then

        'End If
        'formTotal += BuildingsLineCount
        'If formTotal > linesOnPage Then
        '    'RaiseEvent BreakPersProp(True, "PersProp")
        '    formTotal = formTotal - linesOnPage
        '    'formTotal = BuildingsLineCount - formTotal

        '    If BuildingsLineCount > linesOnPage Then
        '        formTotal -= 4
        '    End If
        'End If

        If formTotal > linesOnPage Then
            formTotal -= linesOnPage
        Else
            If (linesOnPage - (formTotal + PersPropLineCount)) < 4 Then
                If PersPropLineCount > 0 Then
                    RaiseEvent BreakBuildings(True, "PersProp")
                End If
                formTotal = 0
            End If
        End If
        formTotal += PersPropLineCount

        'If formTotal >= linesOnPage Then
        '    RaiseEvent BreakPersProp(True, "PersProp")
        '    formTotal = 0
        'End If

        'If formTotal = linesOnPage Then
        '    RaiseEvent BreakInlandMarine(True, "IM")
        '    formTotal = 0
        'End If
        If ShowIMRV Then
            formTotal += IMLineCount
            If LocalQuickQuote.Locations(0).InlandMarines.Count > 0 Then
                If formTotal > linesOnPage Then
                    RaiseEvent BreakInlandMarine(True, "IM")
                    IMLineCount = formTotal - linesOnPage
                    formTotal = IMLineCount
                End If
            Else
                If (formTotal > linesOnPage) Or ((formTotal - linesOnPage) > IMLineCount) Then
                    RaiseEvent BreakInlandMarine(True, "IM")
                    formTotal = 0
                End If
            End If

            If formTotal = linesOnPage Then
                RaiseEvent BreakRVWater(True, "RVW")
                formTotal = 0
            End If

            formTotal += RVWaterLineCount
            If LocalQuickQuote.Locations(0).RvWatercrafts.Count > 0 Then
                If formTotal > linesOnPage Then
                    RVWaterLineCount = formTotal - linesOnPage

                    If (RVWaterLineCount - 8) <= 0 And (RVWaterLineCount - 8) > -2 Then
                        RaiseEvent BreakRVWater(True, "RVW")
                    End If

                    formTotal = RVWaterLineCount
                End If

                If formTotal = linesOnPage Then
                    RaiseEvent BreakAdjustments(True, "Additional")
                    formTotal = 0
                End If
            Else
                If (formTotal > linesOnPage) Or ((formTotal - linesOnPage) > RVWaterLineCount) Then
                    RaiseEvent BreakRVWater(True, "RVW")
                    formTotal = 0
                End If
            End If
        End If

        'If formTotal = linesOnPage Then
        '    RaiseEvent BreakAddlCoverage(True, "Additional")
        '    formTotal = 0
        'End If


        'If formTotal > linesOnPage Then
        '    formTotal -= linesOnPage
        'Else
        '    If (linesOnPage - (formTotal + PersPropLineCount)) < 4 Then
        '        If PersPropLineCount > 0 Then
        '            RaiseEvent BreakBuildings(True, "PersProp")
        '        End If
        '        formTotal = 0
        '    End If
        'End If
        'formTotal += PersPropLineCount

        If formTotal > linesOnPage Then
            formTotal -= linesOnPage
        Else
            If (linesOnPage - (formTotal + AddlCovLineCount)) < 1 Then
                If AddlCovLineCount > 0 Then
                    RaiseEvent BreakAddlCoverage(True, "Additional")
                End If
                formTotal = 0
            End If
        End If
        formTotal += AddlCovLineCount

        If SurLineCount > CreditLineCount Then
            formTotal += SurLineCount
        Else
            formTotal += CreditLineCount
        End If

        If formTotal > linesOnPage Then
            RaiseEvent BreakAdjustments(True, "Adj")

            If SurLineCount > CreditLineCount Then
                formTotal = SurLineCount
            Else
                formTotal = CreditLineCount
            End If
        End If

        'If formTotal = linesOnPage Then
        '    RaiseEvent BreakAdjustments(True, "Adj")
        '    formTotal = 0
        'End If

        formTotal += 3
        If formTotal > linesOnPage Then
            RaiseEvent BreakAdjustments(True, "Adj")
        End If

        RaiseEvent BreakPaymentOpt(True, "Payment")
    End Sub

    Public Function Save() As Boolean Implements IVRUI_P.Save
        Return False
    End Function

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm

    End Sub
End Class