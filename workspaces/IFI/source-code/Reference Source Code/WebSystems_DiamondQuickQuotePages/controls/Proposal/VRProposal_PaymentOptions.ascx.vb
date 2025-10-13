'added 3/24/2017
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Partial Class controls_Proposal_VRProposal_PaymentOptions
    Inherits System.Web.UI.UserControl

    Dim qqHelper As New QuickQuoteHelperClass 'added 5/23/2013 to perform basic payment option calculations

    Private _AnnualPaymentOption As QuickQuotePaymentOption
    Private _SemiAnnualPaymentOption As QuickQuotePaymentOption
    Private _QuarterlyPaymentOption As QuickQuotePaymentOption
    Private _MonthlyPaymentOption As QuickQuotePaymentOption
    Private _EftMonthlyPaymentOption As QuickQuotePaymentOption
    Private _CreditCardMonthlyPaymentOption As QuickQuotePaymentOption
    Private _RenewalCreditCardMonthlyPaymentOption As QuickQuotePaymentOption
    Private _RenewalEftMonthlyPaymentOption As QuickQuotePaymentOption
    Private _AnnualMtgPaymentOption As QuickQuotePaymentOption

    Public Property AnnualPaymentOption As QuickQuotePaymentOption
        Get
            Return _AnnualPaymentOption
        End Get
        Set(value As QuickQuotePaymentOption)
            _AnnualPaymentOption = value
            SetPaymentOptionLabels(_AnnualPaymentOption, Me.lblAnnual_Plan, Me.lblAnnual_DownPaymentPercentage, Me.lblAnnual_DepositAmount, Me.lblAnnual_NumberOfInstallments, Me.lblAnnual_InstallmentAmount, Me.lblAnnual_InstallmentFee)
        End Set
    End Property
    Public Property SemiAnnualPaymentOption As QuickQuotePaymentOption
        Get
            Return _SemiAnnualPaymentOption
        End Get
        Set(value As QuickQuotePaymentOption)
            _SemiAnnualPaymentOption = value
            SetPaymentOptionLabels(_SemiAnnualPaymentOption, Me.lblSemiAnnual_Plan, Me.lblSemiAnnual_DownPaymentPercentage, Me.lblSemiAnnual_DepositAmount, Me.lblSemiAnnual_NumberOfInstallments, Me.lblSemiAnnual_InstallmentAmount, Me.lblSemiAnnual_InstallmentFee)
        End Set
    End Property
    Public Property QuarterlyPaymentOption As QuickQuotePaymentOption
        Get
            Return _QuarterlyPaymentOption
        End Get
        Set(value As QuickQuotePaymentOption)
            _QuarterlyPaymentOption = value
            SetPaymentOptionLabels(_QuarterlyPaymentOption, Me.lblQuarterly_Plan, Me.lblQuarterly_DownPaymentPercentage, Me.lblQuarterly_DepositAmount, Me.lblQuarterly_NumberOfInstallments, Me.lblQuarterly_InstallmentAmount, Me.lblQuarterly_InstallmentFee)
        End Set
    End Property
    Public Property MonthlyPaymentOption As QuickQuotePaymentOption
        Get
            Return _MonthlyPaymentOption
        End Get
        Set(value As QuickQuotePaymentOption)
            _MonthlyPaymentOption = value
            SetPaymentOptionLabels(_MonthlyPaymentOption, Me.lblMonthly_Plan, Me.lblMonthly_DownPaymentPercentage, Me.lblMonthly_DepositAmount, Me.lblMonthly_NumberOfInstallments, Me.lblMonthly_InstallmentAmount, Me.lblMonthly_InstallmentFee)
        End Set
    End Property
    Public Property EftMonthlyPaymentOption As QuickQuotePaymentOption
        Get
            Return _EftMonthlyPaymentOption
        End Get
        Set(value As QuickQuotePaymentOption)
            _EftMonthlyPaymentOption = value
            SetPaymentOptionLabels(_EftMonthlyPaymentOption, Me.lblEftMonthly_Plan, Me.lblEftMonthly_DownPaymentPercentage, Me.lblEftMonthly_DepositAmount, Me.lblEftMonthly_NumberOfInstallments, Me.lblEftMonthly_InstallmentAmount, Me.lblEftMonthly_InstallmentFee)
        End Set
    End Property
    Public Property CreditCardMonthlyPaymentOption As QuickQuotePaymentOption
        Get
            Return _CreditCardMonthlyPaymentOption
        End Get
        Set(value As QuickQuotePaymentOption)
            _CreditCardMonthlyPaymentOption = value
            SetPaymentOptionLabels(_CreditCardMonthlyPaymentOption, Me.lblCreditCardMonthly_Plan, Me.lblCreditCardMonthly_DownPaymentPercentage, Me.lblCreditCardMonthly_DepositAmount, Me.lblCreditCardMonthly_NumberOfInstallments, Me.lblCreditCardMonthly_InstallmentAmount, Me.lblCreditCardMonthly_InstallmentFee)
        End Set
    End Property
    Public Property RenewalCreditCardMonthlyPaymentOption As QuickQuotePaymentOption
        Get
            Return _RenewalCreditCardMonthlyPaymentOption
        End Get
        Set(value As QuickQuotePaymentOption)
            _RenewalCreditCardMonthlyPaymentOption = value
            SetPaymentOptionLabels(_RenewalCreditCardMonthlyPaymentOption, Me.lblRenewalCreditCardMonthly_Plan, Me.lblRenewalCreditCardMonthly_DownPaymentPercentage, Me.lblRenewalCreditCardMonthly_DepositAmount, Me.lblRenewalCreditCardMonthly_NumberOfInstallments, Me.lblRenewalCreditCardMonthly_InstallmentAmount, Me.lblRenewalCreditCardMonthly_InstallmentFee)
        End Set
    End Property
    Public Property RenewalEftMonthlyPaymentOption As QuickQuotePaymentOption
        Get
            Return _RenewalEftMonthlyPaymentOption
        End Get
        Set(value As QuickQuotePaymentOption)
            _RenewalEftMonthlyPaymentOption = value
            SetPaymentOptionLabels(_RenewalEftMonthlyPaymentOption, Me.lblRenewalEftMonthly_Plan, Me.lblRenewalEftMonthly_DownPaymentPercentage, Me.lblRenewalEftMonthly_DepositAmount, Me.lblRenewalEftMonthly_NumberOfInstallments, Me.lblRenewalEftMonthly_InstallmentAmount, Me.lblRenewalEftMonthly_InstallmentFee)
        End Set
    End Property
    Public Property AnnualMtgPaymentOption As QuickQuotePaymentOption
        Get
            Return _AnnualMtgPaymentOption
        End Get
        Set(value As QuickQuotePaymentOption)
            _AnnualMtgPaymentOption = value
            SetPaymentOptionLabels(_AnnualMtgPaymentOption, Me.lblAnnualMtg_Plan, Me.lblAnnualMtg_DownPaymentPercentage, Me.lblAnnualMtg_DepositAmount, Me.lblAnnualMtg_NumberOfInstallments, Me.lblAnnualMtg_InstallmentAmount, Me.lblAnnualMtg_InstallmentFee)
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then

        End If
    End Sub

    Private Sub SetPaymentOptionLabels(ByVal pmtOption As QuickQuotePaymentOption, ByRef lblPlan As Label, ByRef lblDownPaymentPercentage As Label, ByRef lblDepositAmount As Label, ByRef lblNumberOfInstallments As Label, ByRef lblInstallmentAmount As Label, ByRef lblInstallmentFee As Label)
        If pmtOption IsNot Nothing Then
            With pmtOption
                lblPlan.Text = .Description
                lblDownPaymentPercentage.Text = .DownPaymentPercentage
                lblDepositAmount.Text = .DepositAmount
                lblNumberOfInstallments.Text = .NumInstalls
                lblInstallmentAmount.Text = .InstallmentAmount
                lblInstallmentFee.Text = .InstallmentFee
            End With
        End If
    End Sub

    'added 5/23/2013 to just calculate everything based off of total premium
    Public Sub CalculateBasicPaymentOptions(ByVal totalPrem As String, ByVal numberOfQuotes As Integer)
        If totalPrem <> "" AndAlso IsNumeric(totalPrem) = True AndAlso numberOfQuotes <> Nothing AndAlso numberOfQuotes > 0 Then
            Dim monthlyInstallmentFeeAmountPerQuote As String = qqHelper.getDivisionQuotient(Me.lblMonthly_InstallmentFee.Text, numberOfQuotes.ToString)
            qqHelper.ConvertToQuotedPremiumFormat(monthlyInstallmentFeeAmountPerQuote)

            Me.lblAnnual_BasicDownPayment.Text = totalPrem
            Me.lblSemiAnnual_BasicDownPayment.Text = qqHelper.getDivisionQuotient(totalPrem, "2")
            qqHelper.ConvertToQuotedPremiumFormat(Me.lblSemiAnnual_BasicDownPayment.Text)
            Me.lblQuarterly_BasicDownPayment.Text = qqHelper.getDivisionQuotient(totalPrem, "4")
            qqHelper.ConvertToQuotedPremiumFormat(Me.lblQuarterly_BasicDownPayment.Text)
            Me.lblMonthly_BasicDownPayment.Text = qqHelper.getDivisionQuotient(totalPrem, "12") 'may also need to include installment fee
            qqHelper.ConvertToQuotedPremiumFormat(Me.lblMonthly_BasicDownPayment.Text)
            Me.lblEftMonthly_BasicDownPayment.Text = qqHelper.getDivisionQuotient(totalPrem, "12")
            qqHelper.ConvertToQuotedPremiumFormat(Me.lblEftMonthly_BasicDownPayment.Text)
            Me.lblCreditCardMonthly_BasicDownPayment.Text = Me.lblEftMonthly_BasicDownPayment.Text
            Me.lblRenewalCreditCardMonthly_BasicDownPayment.Text = Me.lblEftMonthly_BasicDownPayment.Text
            Me.lblRenewalEftMonthly_BasicDownPayment.Text = Me.lblEftMonthly_BasicDownPayment.Text
            Me.lblAnnualMtg_BasicDownPayment.Text = Me.lblAnnual_BasicDownPayment.Text

            Me.lblAnnual_RemainingInstallments.Text = "N/A"
            Me.lblSemiAnnual_RemainingInstallments.Text = "1"
            Me.lblQuarterly_RemainingInstallments.Text = "3"
            Me.lblMonthly_RemainingInstallments.Text = "11"
            Me.lblEftMonthly_RemainingInstallments.Text = Me.lblMonthly_RemainingInstallments.Text
            Me.lblCreditCardMonthly_RemainingInstallments.Text = Me.lblMonthly_RemainingInstallments.Text
            Me.lblRenewalCreditCardMonthly_RemainingInstallments.Text = Me.lblMonthly_RemainingInstallments.Text
            Me.lblRenewalEftMonthly_RemainingInstallments.Text = Me.lblMonthly_RemainingInstallments.Text
            Me.lblAnnualMtg_RemainingInstallments.Text = Me.lblAnnual_RemainingInstallments.Text

            Me.lblAnnual_BasicInstallmentAmount.Text = "N/A"
            Me.lblSemiAnnual_BasicInstallmentAmount.Text = Me.lblSemiAnnual_BasicDownPayment.Text
            Me.lblQuarterly_BasicInstallmentAmount.Text = Me.lblQuarterly_BasicDownPayment.Text
            Me.lblMonthly_BasicInstallmentAmount.Text = qqHelper.getSum(Me.lblMonthly_BasicDownPayment.Text, Me.lblMonthly_InstallmentFee.Text) 'may also need to include installment fee
            qqHelper.ConvertToQuotedPremiumFormat(Me.lblMonthly_BasicInstallmentAmount.Text)
            Me.lblEftMonthly_BasicInstallmentAmount.Text = Me.lblEftMonthly_BasicDownPayment.Text
            Me.lblCreditCardMonthly_BasicInstallmentAmount.Text = Me.lblCreditCardMonthly_BasicDownPayment.Text
            Me.lblRenewalCreditCardMonthly_BasicInstallmentAmount.Text = Me.lblRenewalCreditCardMonthly_BasicDownPayment.Text
            Me.lblRenewalEftMonthly_BasicInstallmentAmount.Text = Me.lblRenewalEftMonthly_BasicDownPayment.Text
            Me.lblAnnualMtg_BasicInstallmentAmount.Text = Me.lblAnnual_BasicInstallmentAmount.Text
        End If
    End Sub
End Class
