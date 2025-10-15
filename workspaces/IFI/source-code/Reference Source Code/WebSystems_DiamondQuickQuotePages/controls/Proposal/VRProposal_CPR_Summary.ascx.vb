'added 3/24/2017
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects.QuickQuoteObject

Partial Class controls_Proposal_VRProposal_CPR_Summary
    Inherits System.Web.UI.UserControl

    Dim qqHelper As New QuickQuoteHelperClass 'added 5/29/2013 to check for zero premiums

    Private _QuickQuote As QuickQuoteObject
    Private _SubQuotes As List(Of QuickQuoteObject) 'added 10/11/2018
    Public Property QuickQuote As QuickQuoteObject
        Get
            Return _QuickQuote
        End Get
        Set(value As QuickQuoteObject)
            _QuickQuote = value
            SetSummaryLabels()
        End Set
    End Property
    Public Property SubQuotes As List(Of QuickQuoteObject) 'added 10/11/2018
        Get
            If (_SubQuotes Is Nothing OrElse _SubQuotes.Count = 0) AndAlso _QuickQuote IsNot Nothing Then
                _SubQuotes = qqHelper.MultiStateQuickQuoteObjects(_QuickQuote)
            End If
            Return _SubQuotes
        End Get
        Set(value As List(Of QuickQuoteObject))
            _SubQuotes = value
        End Set
    End Property
    Public ReadOnly Property SubQuoteFirst As QuickQuoteObject 'added 10/11/2018
        Get
            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                Return SubQuotes.Item(0)
            Else
                Return Nothing
            End If
        End Get
    End Property
    Public ReadOnly Property GoverningStateQuote As QuickQuoteObject 'added 10/11/2018
        Get
            Return qqHelper.GoverningStateQuote(_QuickQuote, subQuotes:=Me.SubQuotes)
        End Get
    End Property
    'added 6/28/2013
    'Private _LinesInControl As Integer = 14 'breaks and rows; will need to adjust if any breaks are added; note 8/17/2015: 12 rows (11 + Spacer... actually 1 additional row for CPP header but only 1 visible at-a-time) and 2 breaks
    'updated 5/17/2017 for OptCovs row
    'Private _LinesInControl As Integer = 15 'breaks and rows; will need to adjust if any breaks are added
    ' Added a line for food manufacturers.
    Private _LinesInControl As Integer = 16 'breaks and rows; will need to adjust if any breaks are added
    Public ReadOnly Property LinesInControl As Integer
        Get
            Return _LinesInControl
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then

        End If
    End Sub
    Private Sub SetSummaryLabels()
        If _QuickQuote IsNot Nothing Then
            With _QuickQuote
                'Me.lblQuoteNumber.Text = .QuoteNumber
                'updated 4/5/2017 for Diamond Proposals
                Me.lblQuoteNumber.Text = .PolicyNumber
                Dim ExpDate As Date = Date.Today
                If IsDate(.EffectiveDate) Then
                    ExpDate = DateAdd(DateInterval.Year, +1, CDate(.EffectiveDate))
                End If                               
                Me.lblEffectiveDate.Text = .EffectiveDate
                Me.lblExpirationDate.Text = ExpDate
                'Me.lblTotalPremium.Text = .TotalQuotedPremium
                'Me.lblBuildingPremium.Text = .CPR_BuildingsTotal_BuildingCovQuotedPremium
                'Me.lblPersPropPremium.Text = .CPR_BuildingsTotal_PersPropCoverageQuotedPremium
                'Me.lblPersPropOfOthersPremium.Text = .CPR_BuildingsTotal_PersPropOfOthersQuotedPremium
                'If (.HasBusinessIncomeALS) Then  ' Matt A - ALS change
                '    lblBIC.Text = "Business Income Coverage"
                '    Me.lblBusIncPremium.Text = .CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium
                'Else
                '    lblBIC.Text = "Business Income Coverage"
                '    Me.lblBusIncPremium.Text = .CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium
                'End If
                'Me.lblPropInTheOpenPremium.Text = .LocationsTotal_PropertyInTheOpenRecords_QuotedPremium 'added 5/6/2013
                ''Me.lblEnhEndPremium.Text = .BusinessMasterEnhancementQuotedPremium 'added 5/13/2013
                'Me.lblEbPremium.Text = .LocationsTotal_EquipmentBreakdownQuotedPremium
                ''Me.lblEqPremium.Text = .CPR_BuildingsTotal_EQ_QuotedPremium
                ''5/6/2013 - updated to include PITO
                'Me.lblEqPremium.Text = .LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium
                'updated 10/11/2018 for multi-state
                Me.lblBuildingPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPR_BuildingsTotal_BuildingCovQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblPersPropPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPR_BuildingsTotal_PersPropCoverageQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblPersPropOfOthersPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPR_BuildingsTotal_PersPropOfOthersQuotedPremium, maintainFormattingOrDefaultValue:=True)
                If (qqHelper.HasAnyTruePropertyValues(SubQuotes, Function() .HasBusinessIncomeALS)) Then  ' Matt A - ALS change
                    lblBIC.Text = "Business Income Coverage"
                    Me.lblBusIncPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Else
                    lblBIC.Text = "Business Income Coverage"
                    Me.lblBusIncPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium, maintainFormattingOrDefaultValue:=True)
                End If
                Me.lblPropInTheOpenPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .LocationsTotal_PropertyInTheOpenRecords_QuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblEbPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .LocationsTotal_EquipmentBreakdownQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblEqPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium, maintainFormattingOrDefaultValue:=True)
                Me.lblFoodManufPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_CPR_FoodManufacturersEnhancementQuotedPremium, maintainFormattingOrDefaultValue:=True)

                'added 8/17/2015
                If .LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    'CPR packagePart for CPP lob
                    Me.CPP_header_Row.Visible = True
                    Me.Monoline_header_Row.Visible = False
                    'Me.lblTotalPremium.Text = .CPP_CPR_PackagePart_QuotedPremium
                    'updated 10/11/2018 for multi-state
                    Me.lblTotalPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_CPR_PackagePart_QuotedPremium, maintainFormattingOrDefaultValue:=True)
                    Me.lblPremiumText.Text = "Property Total Premium"
                    '8/18/2015 note: logic directly below assume Contractors and Manufacturers Enhancements can only be added for CPP and not mono-line
                    'If qqHelper.IsZeroPremium(.CPP_CPR_ContractorsEnhancementQuotedPremium) = False Then 'could also check for .HasContractorsEnhancement = True
                    '    lblCPREnhancementEndorsementText.Text = "Contractor Enhancement Endorsement"
                    '    lblEnhEndPremium.Text = .CPP_CPR_ContractorsEnhancementQuotedPremium
                    'ElseIf qqHelper.IsZeroPremium(.CPP_CPR_ManufacturersEnhancementQuotedPremium) = False Then 'could also check for .HasManufacturersEnhancement = True
                    '    lblCPREnhancementEndorsementText.Text = "Manufacturer Enhancement Endorsement"
                    '    lblEnhEndPremium.Text = .CPP_CPR_ManufacturersEnhancementQuotedPremium
                    'Else
                    '    lblCPREnhancementEndorsementText.Text = "Enhancement Endorsement" 'should be default
                    '    Me.lblEnhEndPremium.Text = .PackageCPR_EnhancementEndorsementQuotedPremium
                    'End If
                    'updated 10/11/2018 for multi-state
                    If qqHelper.IsZeroPremium(qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_CPR_ContractorsEnhancementQuotedPremium, maintainFormattingOrDefaultValue:=True)) = False Then 'could also check for .HasContractorsEnhancement = True
                        lblCPREnhancementEndorsementText.Text = "Contractor Enhancement Endorsement"
                        lblEnhEndPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_CPR_ContractorsEnhancementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                    ElseIf qqHelper.IsZeroPremium(qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_CPR_ManufacturersEnhancementQuotedPremium, maintainFormattingOrDefaultValue:=True)) = False Then 'could also check for .HasManufacturersEnhancement = True
                        lblCPREnhancementEndorsementText.Text = "Manufacturer Enhancement Endorsement"
                        lblEnhEndPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_CPR_ManufacturersEnhancementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                    ElseIf SubQuoteFirst.Has_PackageCPR_PlusEnhancementEndorsement = True Then ' 06/30/2022 CAH - Why do the processing?
                        lblCPREnhancementEndorsementText.Text = "Property PLUS Enhancement Endorsement"
                        lblEnhEndPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .PackageCPR_PlusEnhancementEndorsementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                    Else
                        lblCPREnhancementEndorsementText.Text = "Enhancement Endorsement" 'should be default
                        Me.lblEnhEndPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .PackageCPR_EnhancementEndorsementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                    End If

                    Me.startBreak.Visible = False
                    Me.endBreak.Visible = False
                    _LinesInControl -= 2
                    Me.quoteNumberSection.Visible = False 'no longer needed since there are 2 separate header rows (since the font size varies between CPP and monoline)
                    Me.SpacerRow.Visible = True
                    'added 5/17/2017; no longer need calc below
                    'Me.lblCPRAmountToEqualMinimumPremium.Text = .CPP_MinPremAdj_CPR
                    'updated 10/11/2018 for multi-state
                    Me.lblCPRAmountToEqualMinimumPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_MinPremAdj_CPR, maintainFormattingOrDefaultValue:=True)
                Else
                    'CPR lob
                    Me.Monoline_header_Row.Visible = True
                    Me.CPP_header_Row.Visible = False
                    Me.lblTotalPremium.Text = .TotalQuotedPremium
                    Me.lblPremiumText.Text = "Total Premium Due" 'should be default
                    'Me.lblEnhEndPremium.Text = .BusinessMasterEnhancementQuotedPremium
                    'updated 10/11/2018 for multi-state

                    If SubQuoteFirst.Has_PackageCPR_PlusEnhancementEndorsement = True Then ' 06/30/2022 CAH - Why do the processing?
                        lblCPREnhancementEndorsementText.Text = "Property PLUS Enhancement Endorsement"
                        lblEnhEndPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .PackageCPR_PlusEnhancementEndorsementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                    Else
                        lblCPREnhancementEndorsementText.Text = "Enhancement Endorsement" 'should be default
                        Me.lblEnhEndPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .PackageCPR_EnhancementEndorsementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                    End If

                    Me.startBreak.Visible = True
                    Me.endBreak.Visible = True
                    Me.quoteNumberSection.Visible = True 'no longer needed since there are 2 separate header rows (since the font size varies between CPP and monoline)
                    Me.SpacerRow.Visible = False
                    _LinesInControl -= 1
                    'added 5/17/2017; no longer need calc below
                    'Me.lblCPRAmountToEqualMinimumPremium.Text = .MinimumPremiumAdjustment
                    'updated 10/11/2018 for multi-state
                    Me.lblCPRAmountToEqualMinimumPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .MinimumPremiumAdjustment, maintainFormattingOrDefaultValue:=True)
                End If

                ''Dim totprem As Decimal = 0
                'Dim amtto100 As Decimal = 0
                'If qqHelper.IsNumericString(Me.lblTotalPremium.Text) = True Then
                '    'totprem = CDec(Me.lblTotalPremium.Text)
                '    Dim partsprem As Decimal = 0
                '    If IsNumeric(.CPR_BuildingsTotal_BuildingCovQuotedPremium) Then partsprem += CDec(.CPR_BuildingsTotal_BuildingCovQuotedPremium)
                '    If IsNumeric(.CPR_BuildingsTotal_PersPropCoverageQuotedPremium) Then partsprem += CDec(.CPR_BuildingsTotal_PersPropCoverageQuotedPremium)
                '    If IsNumeric(.CPR_BuildingsTotal_PersPropOfOthersQuotedPremium) Then partsprem += CDec(.CPR_BuildingsTotal_PersPropOfOthersQuotedPremium)
                '    If IsNumeric(.CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium) Then partsprem += CDec(.CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium)
                '    If IsNumeric(.LocationsTotal_PropertyInTheOpenRecords_QuotedPremium) Then partsprem += CDec(.LocationsTotal_PropertyInTheOpenRecords_QuotedPremium)
                '    If IsNumeric(Me.lblEnhEndPremium.Text) Then partsprem += CDec(Me.lblEnhEndPremium.Text)
                '    If IsNumeric(.LocationsTotal_EquipmentBreakdownQuotedPremium) Then partsprem += CDec(.LocationsTotal_EquipmentBreakdownQuotedPremium)
                '    If IsNumeric(.LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium) Then partsprem += CDec(.LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium)

                '    If partsprem < 100 Then
                '        amtto100 = 100 - partsprem
                '        ' Remove amount in parenthesis per Kristi MGB 11/17/15
                '        'lblCPRAmountToMeetMinimumAmountText.Text = Format("100", "c")
                '        lblCPRAmountToEqualMinimumPremium.Text = Format(amtto100, "c")
                '        'totprem = 100
                '    Else
                '        'trCRMAmtToEqualMinumum.Visible = False
                '        'updated 8/17/2015 to use correct row; nothing needed here since it's being checked below
                '        'trCPRAmountToMeetMinimumPremiumRow.Visible = False
                '        '_LinesInControl -= 1
                '    End If
                'End If
                'removed calc logic 5/17/2017; now using props above; also correctly formatted minAmt since it was just showing c... not using anymore but here just in case
                'Me.lblCPRAmountToMeetMinimumAmountText.Text = qqHelper.QuotedPremiumFormat("100")
            End With
        End If

        Dim proposalPrems As String = "" 'added 5/16/2017; note: amtToEqualMinPrem may be incorrect since it's based off of prems that are already taken into account

        'added 5/29/2013
        If qqHelper.IsZeroPremium(Me.lblBuildingPremium.Text) = True Then
            Me.BuildingRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblBuildingPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblPersPropPremium.Text) = True Then
            Me.PersPropRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblPersPropPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblPersPropOfOthersPremium.Text) = True Then
            Me.PersPropOfOthersRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblPersPropOfOthersPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblBusIncPremium.Text) = True Then
            Me.BusIncRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblBusIncPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblPropInTheOpenPremium.Text) = True Then
            Me.PropInTheOpenRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblPropInTheOpenPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblEnhEndPremium.Text) = True Then
            Me.EnhEndRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblEnhEndPremium.Text)
        End If

        If qqHelper.IsZeroPremium(Me.lblEbPremium.Text) = True Then
            Me.EbRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblEbPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblEqPremium.Text) = True Then
            Me.EqRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblEqPremium.Text)
        End If
        ' Added food manufacturing 7/19/21 MGB
        If qqHelper.IsZeroPremium(Me.lblFoodManufPremium.Text) = True Then
            Me.FoodManufRow.Visible = False
            _LinesInControl -= 1
        Else
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblFoodManufPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCPRAmountToEqualMinimumPremium.Text) Then
            Me.trCPRAmountToMeetMinimumPremiumRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCPRAmountToEqualMinimumPremium.Text)
        End If

        'discounts
        Const COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT = "80558"
        Dim discounts As New List(Of SummaryPremiumDataItem)
        Dim sumOfDiscounts As Decimal
        If Me.GoverningStateQuote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then


            Dim LPDP_Coverage As QuickQuoteCoverage = Nothing

            If QuickQuote.LobType = QuickQuoteLobType.CommercialProperty Then
                LPDP_Coverage = ProposalHelperClass.Find_First_PolicyLevelCoverage(SubQuotes, COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT)
            ElseIf QuickQuote.LobType = QuickQuoteLobType.CommercialPackage Then
                Dim ppList = SubQuotes.SelectMany(Of QuickQuotePackagePart)(Function(sq) sq.PackageParts).ToList()
                Dim partForType = qqHelper.PackagePartForLobType(ppList, QuickQuoteLobType.CommercialProperty)
                If partForType IsNot Nothing Then
                    LPDP_Coverage = ProposalHelperClass.Find_First_PackageLevelCoverage(partForType, COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT)
                End If
            End If

            If LPDP_Coverage IsNot Nothing Then

                Dim lpdpItem As New SummaryPremiumDataItem With
                {
                    .Premium = Decimal.Parse(LPDP_Coverage.FullTermPremium),
                    .Description = "Total Ohio Property Premium Discount"
                }

                discounts.Add(lpdpItem)
            End If
        End If

        If discounts.Count > 0 Then
            Me.Visible = True
            rptDiscounts.DataSource = discounts
            rptDiscounts.DataBind()
            sumOfDiscounts = discounts.Sum(Function(disc) disc.Premium)
            _LinesInControl += discounts.Count
        End If

        'added 5/16/2017
        If qqHelper.IsPositiveDecimalString(proposalPrems) = True Then
            If sumOfDiscounts < 0 Then
                proposalPrems = qqHelper.getSum(proposalPrems, sumOfDiscounts) 'Added 10/07/2022 for bug 65640 MLW            
            End If
            If qqHelper.IsPositiveDecimalString(Me.lblTotalPremium.Text) = True AndAlso CDec(Me.lblTotalPremium.Text) > CDec(proposalPrems) Then
                'needs optional covs; need to use new logic for MinPremAdj (diff prop for monoline vs CPP)
                Dim estOptCovs As String = qqHelper.getDiff(Me.lblTotalPremium.Text, proposalPrems)
                If qqHelper.IsPositiveDecimalString(estOptCovs) = True Then
                    'looks like valid amount to show; need to set label to estOptCovs and show OptCovsRow; use qqHelper.QuotedPremiumFormat
                    Me.lblOptCovsPremium.Text = qqHelper.QuotedPremiumFormat(estOptCovs) 'added 5/17/2017
                End If
            End If
        End If
        If qqHelper.IsZeroPremium(Me.lblOptCovsPremium.Text) = True Then 'added 5/17/2017
            Me.OptCovsRow.Visible = False
            _LinesInControl -= 1
        End If

        'Moved above 10/04/2022
        ''discounts
        'Const COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT = "80558"
        'Dim discounts As New List(Of SummaryPremiumDataItem)
        'If Me.GoverningStateQuote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then


        '    Dim LPDP_Coverage As QuickQuoteCoverage = Nothing

        '    If QuickQuote.LobType = QuickQuoteLobType.CommercialProperty Then
        '        LPDP_Coverage = ProposalHelperClass.Find_First_PolicyLevelCoverage(SubQuotes, COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT)
        '    ElseIf QuickQuote.LobType = QuickQuoteLobType.CommercialPackage Then
        '        Dim ppList = SubQuotes.SelectMany(Of QuickQuotePackagePart)(Function(sq) sq.PackageParts).ToList()
        '        Dim partForType = qqHelper.PackagePartForLobType(ppList, QuickQuoteLobType.CommercialProperty)
        '        If partForType IsNot Nothing Then
        '            LPDP_Coverage = ProposalHelperClass.Find_First_PackageLevelCoverage(partForType, COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT)
        '        End If
        '    End If

        '    If LPDP_Coverage IsNot Nothing Then

        '        Dim lpdpItem As New SummaryPremiumDataItem With
        '        {
        '            .Premium = Decimal.Parse(LPDP_Coverage.FullTermPremium),
        '            .Description = "Total Ohio Property Premium Discount"
        '        }

        '        discounts.Add(lpdpItem)
        '    End If
        'End If

        'If discounts.Count > 0 Then
        '    Me.Visible = True
        '    rptDiscounts.DataSource = discounts
        '    rptDiscounts.DataBind()
        'End If
        ''TODO: line break calculation
        ''TODO: Optional premium calculation

    End Sub

    Protected Structure SummaryPremiumDataItem
        Public Property Description As String
        Public Property Detail As String
        Public Property Limit As String
        Public Property Premium As Decimal
    End Structure
End Class
