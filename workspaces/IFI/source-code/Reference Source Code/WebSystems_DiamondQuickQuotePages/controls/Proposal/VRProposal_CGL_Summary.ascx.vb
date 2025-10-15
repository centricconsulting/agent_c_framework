'added 3/24/2017
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports System.Collections.Generic

Partial Class controls_Proposal_VRProposal_CGL_Summary
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
    'Private _LinesInControl As Integer = 11 'breaks and rows; will need to adjust if any breaks are added; note 8/17/2015: 9 rows (8 + Spacer... actually 1 additional row for CPP header but only 1 visible at-a-time) and 2 breaks
    'updated 5/17/2017 for MinPremAdj
    'Private _LinesInControl As Integer = 12 'breaks and rows; will need to adjust if any breaks are added
    ' Changed to 13 for Food Manufacturers
    Private _LinesInControl As Integer = 13 'breaks and rows; will need to adjust if any breaks are added
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
                'If Me.Quote.EffectiveDate <> "" AndAlso IsDate(Me.Quote.EffectiveDate) = True AndAlso av.AvDate <> "" AndAlso IsDate(av.AvDate) = True AndAlso CDate(av.AvDate) >= DateAdd(DateInterval.Year, -3, CDate(Me.Quote.EffectiveDate)) Then
                If .LobType = QuickQuoteLobType.CommercialGeneralLiability Then
                    Dim ExpDate As Date = Date.Today
                    If IsDate(.EffectiveDate) Then
                        ExpDate = DateAdd(DateInterval.Year, +1, CDate(.EffectiveDate))
                    End If                               
                    Me.lblEffectiveDate.Text = .EffectiveDate
                    Me.lblExpirationDate.Text = ExpDate
                Else
                    EffDateRow.Visible = False
                End If
                'Me.lblTotalPremium.Text = .TotalQuotedPremium
                'Me.lblEnhEndPrem.Text = .BusinessMasterEnhancementQuotedPremium
                'Me.lblPremOps.Text = .GL_PremisesTotalQuotedPremium
                'Me.lblProdCompOps.Text = .GL_ProductsTotalQuotedPremium
                'Me.lblOptCovs.Text = .Dec_GL_OptCovs_Premium
                'Me.lblMinPrem_Prem.Text = .GL_PremisesMinimumQuotedPremium
                'Me.lblAmtForMinPrem_Prem.Text = .GL_PremisesMinimumPremiumAdjustment
                'Me.lblMinPrem_Prod.Text = .GL_ProductsMinimumQuotedPremium
                'Me.lblAmtForMinPrem_Prod.Text = .GL_ProductsMinimumPremiumAdjustment
                'updated 10/11/2018 for multi-state
                Me.lblFoodManufPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_CGL_FoodManufacturersEnhancementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblPremOps.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .GL_PremisesTotalQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblProdCompOps.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .GL_ProductsTotalQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblOptCovs.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .Dec_GL_OptCovs_Premium, maintainFormattingOrDefaultValue:=True)
                If SubQuoteFirst IsNot Nothing Then
                    Me.lblMinPrem_Prem.Text = SubQuoteFirst.GL_PremisesMinimumQuotedPremium
                Else
                    Me.lblMinPrem_Prem.Text = .GL_PremisesMinimumQuotedPremium
                End If
                Me.lblAmtForMinPrem_Prem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .GL_PremisesMinimumPremiumAdjustment, maintainFormattingOrDefaultValue:=True)
                If SubQuoteFirst IsNot Nothing Then
                    Me.lblMinPrem_Prod.Text = SubQuoteFirst.GL_ProductsMinimumQuotedPremium
                Else
                    Me.lblMinPrem_Prod.Text = .GL_ProductsMinimumQuotedPremium
                End If
                Me.lblAmtForMinPrem_Prod.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .GL_ProductsMinimumPremiumAdjustment, maintainFormattingOrDefaultValue:=True)

                'added 8/17/2015
                If .LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    'CGL packagePart for CPP lob
                    Me.CPP_header_Row.Visible = True
                    Me.Monoline_header_Row.Visible = False
                    'Me.lblTotalPremium.Text = .CPP_GL_PackagePart_QuotedPremium
                    'updated 10/11/2018 for multi-state
                    Me.lblTotalPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_GL_PackagePart_QuotedPremium, maintainFormattingOrDefaultValue:=True)
                    Me.lblPremiumText.Text = "General Liability Total Premium"
                    '8/18/2015 note: logic directly below assume Contractors and Manufacturers Enhancements can only be added for CPP and not mono-line
                    'If qqHelper.IsZeroPremium(.CPP_CGL_ContractorsEnhancementQuotedPremium) = False Then 'could also check for .HasContractorsEnhancement = True
                    '    lblCGLEnhancementEndorsementText.Text = "Contractor Enhancement Endorsement"
                    '    lblEnhEndPrem.Text = .CPP_CGL_ContractorsEnhancementQuotedPremium
                    'ElseIf qqHelper.IsZeroPremium(.CPP_CGL_ManufacturersEnhancementQuotedPremium) = False Then 'could also check for .HasManufacturersEnhancement = True
                    '    lblCGLEnhancementEndorsementText.Text = "Manufacturer Enhancement Endorsement"
                    '    lblEnhEndPrem.Text = .CPP_CGL_ManufacturersEnhancementQuotedPremium
                    'Else
                    '    lblCGLEnhancementEndorsementText.Text = "Enhancement Endorsement" 'should be default
                    '    Me.lblEnhEndPrem.Text = .PackageGL_EnhancementEndorsementQuotedPremium
                    'End If
                    'updated 10/11/2018 for multi-state
                    If qqHelper.IsZeroPremium(qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_CGL_ContractorsEnhancementQuotedPremium, maintainFormattingOrDefaultValue:=True)) = False Then 'could also check for .HasContractorsEnhancement = True
                        lblCGLEnhancementEndorsementText.Text = "Contractor Enhancement Endorsement"
                        lblEnhEndPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_CGL_ContractorsEnhancementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                    ElseIf qqHelper.IsZeroPremium(qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_CGL_ManufacturersEnhancementQuotedPremium, maintainFormattingOrDefaultValue:=True)) = False Then 'could also check for .HasManufacturersEnhancement = True
                        lblCGLEnhancementEndorsementText.Text = "Manufacturer Enhancement Endorsement"
                        lblEnhEndPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_CGL_ManufacturersEnhancementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                    Else
                        lblCGLEnhancementEndorsementText.Text = "Enhancement Endorsement" 'should be default
                        Me.lblEnhEndPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .PackageGL_EnhancementEndorsementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                    End If

                    Me.startBreak.Visible = False
                    Me.endBreak.Visible = False
                    _LinesInControl -= 2
                    Me.quoteNumberSection.Visible = False 'no longer needed since there are 2 separate header rows (since the font size varies between CPP and monoline)
                    Me.SpacerRow.Visible = True
                    'Me.lblMinPremAdj.Text = .CPP_MinPremAdj_CGL 'added 5/17/2017
                    'updated 10/11/2018 for multi-state
                    Me.lblMinPremAdj.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_MinPremAdj_CGL, maintainFormattingOrDefaultValue:=True)
                Else
                    'CGL lob
                    Me.Monoline_header_Row.Visible = True
                    Me.CPP_header_Row.Visible = False
                    Me.lblTotalPremium.Text = .TotalQuotedPremium
                    Me.lblPremiumText.Text = "Total Premium Due" 'should be default
                    'Me.lblEnhEndPrem.Text = .BusinessMasterEnhancementQuotedPremium
                    'updated 10/11/2018 for multi-state
                    Me.lblEnhEndPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .BusinessMasterEnhancementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                    lblCGLEnhancementEndorsementText.Text = "Enhancement Endorsement" 'should be default

                    Me.startBreak.Visible = True
                    Me.endBreak.Visible = True
                    Me.quoteNumberSection.Visible = True 'no longer needed since there are 2 separate header rows (since the font size varies between CPP and monoline)
                    Me.SpacerRow.Visible = False
                    _LinesInControl -= 1
                    'Me.lblMinPremAdj.Text = .MinimumPremiumAdjustment 'added 5/17/2017
                    'updated 10/11/2018 for multi-state
                    Me.lblMinPremAdj.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .MinimumPremiumAdjustment, maintainFormattingOrDefaultValue:=True)
                End If
                'General Liability PLUS Enhancement Endorsement
                Dim GlpPremium = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .PackageGL_PlusEnhancementEndorsementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                If qqHelper.IsZeroPremium(Me.lblEnhEndPrem.Text) = True AndAlso qqHelper.IsZeroPremium(GlpPremium) = False Then
                    lblCGLEnhancementEndorsementText.Text = "General Liability PLUS Enhancement Endorsement"
                    Me.lblEnhEndPrem.Text = GlpPremium
                End If
            End With
        End If

        Dim proposalPrems As String = "" 'added 5/16/2017; note: already has OptCovs based off of prems that QuickQuoteObject takes into account

        'added 5/29/2013
        If qqHelper.IsZeroPremium(Me.lblEnhEndPrem.Text) = True Then
            Me.EnhEndRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblEnhEndPrem.Text)
        End If
        ' Added Food Manufacturing 7/19/21
        If qqHelper.IsZeroPremium(Me.lblFoodManufPrem.Text) = True Then
            Me.FoodManufRow.Visible = False
            _LinesInControl -= 1
        Else
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblFoodManufPrem.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblPremOps.Text) = True Then
            Me.PremOpsRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblPremOps.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblProdCompOps.Text) = True Then
            Me.ProdCompOpsRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblProdCompOps.Text)
        End If
        'If qqHelper.IsZeroPremium(Me.lblOptCovs.Text) = True Then '5/17/2017 - moved below
        '    Me.OptCovsRow.Visible = False
        '    _LinesInControl -= 1
        'End If
        If qqHelper.IsZeroPremium(Me.lblAmtForMinPrem_Prem.Text) = True Then
            Me.MinPrem_PremRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblAmtForMinPrem_Prem.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblAmtForMinPrem_Prod.Text) = True Then
            Me.MinPrem_ProdRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblAmtForMinPrem_Prod.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblMinPremAdj.Text) = True Then 'added 5/17/2017
            Me.MinPremAdjRow.Visible = False
            _LinesInControl -= 1
        Else
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblMinPremAdj.Text)
        End If



        'discounts
        Const COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT = "80558"
        Dim discounts As New List(Of SummaryPremiumDataItem)
        Dim sumOfDiscounts As Decimal
        If Me.GoverningStateQuote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then


            Dim LPDP_Coverage As QuickQuoteCoverage = Nothing

            If QuickQuote.LobType = QuickQuoteLobType.CommercialGeneralLiability Then
                LPDP_Coverage = ProposalHelperClass.Find_First_PolicyLevelCoverage(SubQuotes, COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT)
            ElseIf QuickQuote.LobType = QuickQuoteLobType.CommercialPackage Then
                Dim ppList = SubQuotes.SelectMany(Of QuickQuotePackagePart)(Function(sq) sq.PackageParts).ToList()
                Dim partForType = qqHelper.PackagePartForLobType(ppList, QuickQuoteLobType.CommercialGeneralLiability)
                If partForType IsNot Nothing Then
                    LPDP_Coverage = ProposalHelperClass.Find_First_PackageLevelCoverage(partForType, COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT)
                End If
            End If

            If LPDP_Coverage IsNot Nothing Then

                Dim lpdpItem As New SummaryPremiumDataItem With
                {
                    .Premium = Decimal.Parse(LPDP_Coverage.FullTermPremium),
                    .Description = "Total Ohio General Liability Premium Discount"
                }

                discounts.Add(lpdpItem)               
            End If
        End If

        If discounts.Count > 0 Then
            rptDiscounts.Visible = True
            rptDiscounts.DataSource = discounts
            rptDiscounts.DataBind()
            sumOfDiscounts = discounts.Sum(Function(disc) disc.Premium)
            _LinesInControl += discounts.Count
        Else
            rptDiscounts.Visible = False
            rptDiscounts.DataSource = Nothing
        End If

        'added 5/16/2017
        Me.lblOptCovs.Text = "" 'resetting as-of 5/17/2017
        If qqHelper.IsPositiveDecimalString(proposalPrems) = True Then
            If sumOfDiscounts < 0 Then
                proposalPrems = qqHelper.getSum(proposalPrems, sumOfDiscounts) 'Added 10/07/2022 for bug 65640 MLW            
            End If
            If qqHelper.IsPositiveDecimalString(Me.lblTotalPremium.Text) = True AndAlso CDec(Me.lblTotalPremium.Text) > CDec(proposalPrems) Then
                'needs optional covs; see if difference is already the same as Me.lblOptCovsPrem.Text; may need to check MinPremAdj (diff prop for monoline vs CPP) and add row for that too
                Dim estOptCovs As String = qqHelper.getDiff(Me.lblTotalPremium.Text, proposalPrems)
                If qqHelper.IsPositiveDecimalString(estOptCovs) = True Then
                    'looks like valid amount to show
                    'If qqHelper.IsPositiveDecimalString(Me.lblOptCovs.Text) = True Then
                    '    If CDec(Me.lblOptCovs.Text) = CDec(estOptCovs) Then
                    '        'same amount; nothing needed
                    '    Else
                    '        'should probably overwrite label w/ estOptCovs; use qqHelper.QuotedPremiumFormat

                    '    End If
                    'Else
                    '    'likely not showing yet; need to set label to estOptCovs and show OptCovsRow; use qqHelper.QuotedPremiumFormat

                    'End If
                    'updated 5/17/2017 after adding reset code above
                    'Me.lblOptCovs.Text = qqHelper.QuotedPremiumFormat(qqHelper.getDiff(estOptCovs, sumOfDiscounts.ToString()))
                    Me.lblOptCovs.Text = qqHelper.QuotedPremiumFormat(estOptCovs)
                End If
            End If
        End If
        If qqHelper.IsZeroPremium(Me.lblOptCovs.Text) = True Then '5/17/2017 - moved here from above
            Me.OptCovsRow.Visible = False
            _LinesInControl -= 1
        End If

        'added 6/15/2017
        If Me.PremOpsRow.Visible = True AndAlso Me.MinPrem_PremRow.Visible = True Then
            Dim totalPremisesPremium As String = qqHelper.getSum(Me.lblPremOps.Text, Me.lblAmtForMinPrem_Prem.Text)
            If qqHelper.IsPositiveDecimalString(totalPremisesPremium) = True AndAlso QuickQuoteHelperClass.isTextMatch(Me.lblPremOps.Text, totalPremisesPremium, matchType:=QuickQuoteHelperClass.TextMatchType.DecimalOnly) = False Then
                Me.lblPremOps.Text = qqHelper.QuotedPremiumFormat(totalPremisesPremium)
                Me.MinPrem_PremRow.Visible = False
                _LinesInControl -= 1
            End If
        End If
        If Me.ProdCompOpsRow.Visible = True AndAlso Me.MinPrem_ProdRow.Visible = True Then
            Dim totalProductsPremium As String = qqHelper.getSum(Me.lblProdCompOps.Text, Me.lblAmtForMinPrem_Prod.Text)
            If qqHelper.IsPositiveDecimalString(totalProductsPremium) = True AndAlso QuickQuoteHelperClass.isTextMatch(Me.lblProdCompOps.Text, totalProductsPremium, matchType:=QuickQuoteHelperClass.TextMatchType.DecimalOnly) = False Then
                Me.lblProdCompOps.Text = qqHelper.QuotedPremiumFormat(totalProductsPremium)
                Me.MinPrem_ProdRow.Visible = False
                _LinesInControl -= 1
            End If
        End If

    End Sub

    Protected Structure SummaryPremiumDataItem
        Public Property Description As String
        Public Property Detail As String
        Public Property Limit As String
        Public Property Premium As Decimal
    End Structure
End Class
