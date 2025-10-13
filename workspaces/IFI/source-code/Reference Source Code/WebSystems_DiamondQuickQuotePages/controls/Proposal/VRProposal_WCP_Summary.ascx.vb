'added 3/24/2017
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Partial Class controls_Proposal_VRProposal_WCP_Summary
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
    'Private _LinesInControl As Integer = 15 'breaks and rows; will need to adjust if any breaks are added
    'updated 5/17/2017 for OptCovs row
    'Private _LinesInControl As Integer = 16 'breaks and rows; will need to adjust if any breaks are added
    ' Updated 3/20/19 for Catastrophe other than terrorism Bug 32242 MGB
    'Private _LinesInControl As Integer = 17 'breaks and rows; will need to adjust if any breaks are added
    'updated 7/16/2019 for KY
    'Private _LinesInControl As Integer = 19 'breaks and rows; will need to adjust if any breaks are added
    'updated 7/16/2019 for KY
    Private _LinesInControl As Integer = 20 'breaks and rows; will need to adjust if any breaks are added

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
                Me.lblTotalPremium.Text = .TotalQuotedPremium 'Me.lblDec_TotEst_Prem.Text
                Dim ExpDate As Date = Date.Today
                If IsDate(.EffectiveDate) Then
                    ExpDate = DateAdd(DateInterval.Year, +1, CDate(.EffectiveDate))
                End If                               
                Me.lblEffectiveDate.Text = .EffectiveDate
                Me.lblExpirationDate.Text = ExpDate
                'Me.lblEstPlanPrem.Text = .TotalEstimatedPlanPremium
                'Me.lblIncreasedLimit.Text = .EmployersLiabilityQuotedPremium
                'Me.lblExpMod.Text = .ExpModQuotedPremium
                'Me.lblSchedMod.Text = .ScheduleModQuotedPremium
                'Me.lblPremDiscount.Text = .PremDiscountQuotedPremium
                'Me.lblLossConstant.Text = .Dec_LossConstantPremium
                'Me.lblExpConstant.Text = .Dec_ExpenseConstantPremium
                'Me.lblCertActsOfTerr.Text = .TerrorismQuotedPremium
                'Me.lblMinPrem.Text = .MinimumQuotedPremium
                'Me.lblAmtForMinPrem.Text = .MinimumPremiumAdjustment
                'Me.lblSecInjFund.Text = .SecondInjuryFundQuotedPremium
                'Me.lblTotalPlusSecInjFund.Text = .Dec_WC_TotalPremiumDue
                'updated 10/11/2018 for multi-state
                Me.lblEstPlanPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .TotalEstimatedPlanPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblIncreasedLimit.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .EmployersLiabilityQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblExpMod.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .ExpModQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblSchedMod.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .ScheduleModQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblPremDiscount.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .PremDiscountQuotedPremium, maintainFormattingOrDefaultValue:=True)
                If SubQuoteFirst IsNot Nothing Then
                    Me.lblLossConstant.Text = SubQuoteFirst.Dec_LossConstantPremium
                    Me.lblExpConstant.Text = SubQuoteFirst.Dec_ExpenseConstantPremium
                Else
                    Me.lblLossConstant.Text = .Dec_LossConstantPremium
                    Me.lblExpConstant.Text = .Dec_ExpenseConstantPremium
                End If
                Me.lblCertActsOfTerr.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .TerrorismQuotedPremium, maintainFormattingOrDefaultValue:=True)
                ' Added 3/20/19 MGB Bug 32242
                Me.lblCatastropheOtherThanTerrorism.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CatastropheOtherThanCertifiedActsOfTerrorismQuotedPremium, maintainFormattingOrDefaultValue:=True)
                If SubQuoteFirst IsNot Nothing Then
                    Me.lblMinPrem.Text = SubQuoteFirst.MinimumQuotedPremium
                Else
                    Me.lblMinPrem.Text = .MinimumQuotedPremium
                End If
                Me.lblAmtForMinPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .MinimumPremiumAdjustment, maintainFormattingOrDefaultValue:=True)
                Me.lblSecInjFund.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .SecondInjuryFundQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblKY_SpecFundAssess.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .KentuckySpecialFundAssessmentQuotedPremium, maintainFormattingOrDefaultValue:=True) 'added 7/16/2019 for KY
                Me.lblKY_Surcharge.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .WCP_KY_PremSurcharge, maintainFormattingOrDefaultValue:=True) 'added 7/16/2019 for KY
                Me.lblTotalPlusSecInjFund.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .Dec_WC_TotalPremiumDue, maintainFormattingOrDefaultValue:=True)
                Me.lblIL_CommissionsOperationsFundSurcharge.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .IL_WCP_CommissionOperationsFundSurcharge, maintainFormattingOrDefaultValue:=True)
            End With
        End If

        Dim proposalPrems As String = "" 'added 5/16/2017

        'added 5/29/2013
        If qqHelper.IsZeroPremium(Me.lblEstPlanPrem.Text) = True Then
            Me.EstPlanRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblEstPlanPrem.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblIncreasedLimit.Text) = True Then
            Me.IncreasedLimitRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblIncreasedLimit.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblExpMod.Text) = True Then
            Me.ExpModRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblExpMod.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblSchedMod.Text) = True Then
            Me.SchedModRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblSchedMod.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblPremDiscount.Text) = True Then
            Me.PremDiscountRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblPremDiscount.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblLossConstant.Text) = True Then
            Me.LossConstantRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblLossConstant.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblExpConstant.Text) = True Then
            Me.ExpConstantRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblExpConstant.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCertActsOfTerr.Text) = True Then
            Me.CertActsOfTerrRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCertActsOfTerr.Text)
        End If
        ' Added 3/20/19 MGB Bug 32242
        If qqHelper.IsZeroPremium(Me.lblCatastropheOtherThanTerrorism.Text) = True Then
            Me.CatastropheOtherThanTerrorismRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCatastropheOtherThanTerrorism.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblAmtForMinPrem.Text) = True Then
            Me.MinPremAdjRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblAmtForMinPrem.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblTotalPremium.Text) = True Then
            Me.TotPremRow.Visible = False
            _LinesInControl -= 1
        End If
        If qqHelper.IsZeroPremium(Me.lblSecInjFund.Text) = True Then
            Me.SecInjFundRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblSecInjFund.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblKY_SpecFundAssess.Text) = True Then 'added 7/16/2019
            Me.KY_SpecFundAssessRow.Visible = False
            _LinesInControl -= 1
        Else
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblKY_SpecFundAssess.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblKY_Surcharge.Text) = True Then 'added 7/16/2019
            Me.KY_SurchargeRow.Visible = False
            _LinesInControl -= 1
        Else
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblKY_Surcharge.Text)
        End If

        If qqHelper.IsZeroPremium(Me.lblIL_CommissionsOperationsFundSurcharge.Text) = True Then 'added 1/13/2020
            Me.IL_CommissionsFundSurchargeRow.Visible = False
            _LinesInControl -= 1
        Else
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblIL_CommissionsOperationsFundSurcharge.Text)
        End If


        'added 5/16/2017
        If qqHelper.IsPositiveDecimalString(proposalPrems) = True Then
            If qqHelper.IsPositiveDecimalString(Me.lblTotalPlusSecInjFund.Text) = True AndAlso CDec(Me.lblTotalPlusSecInjFund.Text) > CDec(proposalPrems) Then
                'needs optional covs; get difference
                Dim estOptCovs As String = qqHelper.getDiff(Me.lblTotalPlusSecInjFund.Text, proposalPrems)
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

    End Sub

    Public Function getILCommissionsFundSurcharge(quote As QuickQuoteObject) As String
        Dim ILQuote As QuickQuoteObject = qqHelper.QuickQuoteObjectForState(_QuickQuote, QuickQuoteHelperClass.QuickQuoteState.Illinois)
        If ILQuote IsNot Nothing Then
            Dim cov As QuickQuoteCoverage = qqHelper.QuickQuoteCoverageForCoverageCodeId(ILQuote.PolicyCoverages, "80559")
            If cov IsNot Nothing Then
                Return qqHelper.QuotedPremiumFormat(cov.WrittenPremium)
            End If
        End If
        Return "0"
    End Function
End Class
