'added 3/24/2017
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Partial Class controls_Proposal_VRProposal_BOP_Summary
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
    Private _LinesInControl As Integer = 10 'breaks and rows; will need to adjust if any breaks are added
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
                Me.lblTotalPremium.Text = .TotalQuotedPremium
                Dim ExpDate As Date = Date.Today
                If IsDate(.EffectiveDate) Then
                    ExpDate = DateAdd(DateInterval.Year, +1, CDate(.EffectiveDate))
                End If                               
                Me.lblEffectiveDate.Text = .EffectiveDate
                Me.lblExpirationDate.Text = ExpDate
                'Me.lblBuildingPrem.Text = .Dec_BuildingLimit_All_Premium
                'Me.lblPersPropPrem.Text = .Dec_BuildingPersPropLimit_All_Premium
                'Me.lblOccLiabPrem.Text = .OccurrencyLiabilityQuotedPremium
                'Me.lblEnhEndPrem.Text = .BusinessMasterEnhancementQuotedPremium
                'Me.lblMinPremAdj.Text = .MinimumPremiumAdjustment
                'Me.lblOptCovsPrem.Text = .Dec_BOP_OptCovs_Premium
                'updated 10/11/2018 for multi-state
                Me.lblBuildingPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .Dec_BuildingLimit_All_Premium, maintainFormattingOrDefaultValue:=True)
                Me.lblPersPropPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .Dec_BuildingPersPropLimit_All_Premium, maintainFormattingOrDefaultValue:=True)
                Me.lblOccLiabPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .OccurrencyLiabilityQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblEnhEndPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .BusinessMasterEnhancementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblMinPremAdj.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .MinimumPremiumAdjustment, maintainFormattingOrDefaultValue:=True)
                Me.lblOptCovsPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .Dec_BOP_OptCovs_Premium, maintainFormattingOrDefaultValue:=True)
            End With
        End If

        Dim proposalPrems As String = "" 'added 5/16/2017; note: already has OptCovs based off of prems that QuickQuoteObject takes into account

        'added 5/29/2013
        If qqHelper.IsZeroPremium(Me.lblBuildingPrem.Text) = True Then
            Me.BuildingRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblBuildingPrem.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblPersPropPrem.Text) = True Then
            Me.PersPropRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblPersPropPrem.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblOccLiabPrem.Text) = True Then
            Me.OccLiabRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblOccLiabPrem.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblEnhEndPrem.Text) = True Then
            Me.EnhEndRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblEnhEndPrem.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblMinPremAdj.Text) = True Then
            Me.MinPremAdjRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblMinPremAdj.Text)
        End If
        'If qqHelper.IsZeroPremium(Me.lblOptCovsPrem.Text) = True Then '5/17/2017 - moved below
        '    Me.OptCovsRow.Visible = False
        '    _LinesInControl -= 1
        'End If

        'added 2021-09-08
        Const COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT = "80558"
        Dim LPDP_Coverage As QuickQuoteCoverage = Nothing
        Dim lpdpValue As Decimal
        If Me.GoverningStateQuote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
            LPDP_Coverage = ProposalHelperClass.Find_First_PolicyLevelCoverage(SubQuotes, COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT)

            If LPDP_Coverage IsNot Nothing Then
                lpdpValue = ProposalHelperClass.TryToGetDec(LPDP_Coverage.FullTermPremium)                

                'show LPDP section
                trLargePremiumDiscount.Visible = True
                lblTotalLargePremiumDiscount.Text = qqHelper.QuotedPremiumFormat(lpdpValue)
                'Me.lblOptCovsPrem.Text = qqHelper.getDiff(Me.lblOptCovsPrem.Text, lpdpValue.ToString())
                _LinesInControl += 1
            End If
        End If




        'added 5/16/2017
        'Me.lblOptCovsPrem.Text = "" 'added 5/17/2017 to reset in the future... commented for now
        If qqHelper.IsPositiveDecimalString(proposalPrems) = True Then
            If lpdpValue < 0 Then
                proposalPrems = qqHelper.getSum(proposalPrems, lpdpValue) 'Added 10/07/2022 for bug 65640 MLW
            End If
            If qqHelper.IsPositiveDecimalString(Me.lblTotalPremium.Text) = True AndAlso CDec(Me.lblTotalPremium.Text) > CDec(proposalPrems) Then
                'needs optional covs; see if difference is already the same as Me.lblOptCovsPrem.Text
                Dim estOptCovs As String = qqHelper.getDiff(Me.lblTotalPremium.Text, proposalPrems)
                If qqHelper.IsPositiveDecimalString(estOptCovs) = True Then
                    'looks like valid amount to show
                    'If qqHelper.IsPositiveDecimalString(Me.lblOptCovsPrem.Text) = True Then
                    '    If CDec(Me.lblOptCovsPrem.Text) = CDec(estOptCovs) Then
                    '        'same amount; nothing needed
                    '    Else
                    '        'should probably overwrite label w/ estOptCovs; use qqHelper.QuotedPremiumFormat
                    '        '5/17/2017 note: will update w/ QuoteSummary... since both already have MinPremAdj and OptCovs

                    '    End If
                    'Else
                    '    'likely not showing yet; need to set label to estOptCovs and show OptCovsRow; use qqHelper.QuotedPremiumFormat
                    '    '5/17/2017 note: will update w/ QuoteSummary... since both already have MinPremAdj and OptCovs

                    'End If
                    'updated 5/17/2017 after adding reset code above
                    '5/17/2017 note: will wait to update w/ QuoteSummary... since both already have MinPremAdj and OptCovs
                    Me.lblOptCovsPrem.Text = qqHelper.QuotedPremiumFormat(estOptCovs)
                End If
            End If
        End If

        If qqHelper.IsZeroPremium(Me.lblOptCovsPrem.Text) = True Then '5/17/2017 - moved here from above
            Me.OptCovsRow.Visible = False
            _LinesInControl -= 1
        End If
    End Sub
End Class
