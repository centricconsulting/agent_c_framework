Public Class ctlPaymentOptions
    Inherits System.Web.UI.UserControl

    Dim quickQuote As QuickQuote.CommonObjects.QuickQuoteObject
    Dim QQxml As New QuickQuote.CommonMethods.QuickQuoteXML
    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

    'EFT Monthly
    Public Property EFTMonthlyDownPercent As String
        Get
            Return Me.lblEftMonthly_DownPaymentPercentage.Text
        End Get
        Set(value As String)
            Me.lblEftMonthly_DownPaymentPercentage.Text = value
        End Set
    End Property

    Public Property EFTMonthlyDeposit As String
        Get
            Return Me.lblEftMonthly_DepositAmount.Text
        End Get
        Set(value As String)
            Me.lblEftMonthly_DepositAmount.Text = value
        End Set
    End Property

    Public Property EFTMonthlyDown As String
        Get
            Return Me.lblEftMonthly_BasicDownPayment.Text
        End Get
        Set(value As String)
            Me.lblEftMonthly_BasicDownPayment.Text = value
        End Set
    End Property

    Public Property EFTMonthlyNumInstall As String
        Get
            Return Me.lblEftMonthly_NumberOfInstallments.Text
        End Get
        Set(value As String)
            Me.lblEftMonthly_NumberOfInstallments.Text = value
        End Set
    End Property

    Public Property EFTMonthlyRemainInstall As String
        Get
            Return Me.lblEftMonthly_RemainingInstallments.Text
        End Get
        Set(value As String)
            Me.lblEftMonthly_RemainingInstallments.Text = value
        End Set
    End Property

    Public Property EFTMonthlyInstallAmt As String
        Get
            Return Me.lblEftMonthly_InstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblEftMonthly_InstallmentAmount.Text = value
        End Set
    End Property

    Public Property EFTMonthlyBasicInstall As String
        Get
            Return Me.lblEftMonthly_BasicInstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblEftMonthly_BasicInstallmentAmount.Text = value
        End Set
    End Property

    Public Property EFTMonthlyInstallFee As String
        Get
            Return Me.lblEftMonthly_InstallmentFee.Text
        End Get
        Set(value As String)
            Me.lblEftMonthly_InstallmentFee.Text = value
        End Set
    End Property

    'Direct Bill Monthly
    Public Property DirectMonthlyDownPercent As String
        Get
            Return Me.lblMonthly_DownPaymentPercentage.Text
        End Get
        Set(value As String)
            Me.lblMonthly_DownPaymentPercentage.Text = value
        End Set
    End Property

    Public Property DirectMonthlyDeposit As String
        Get
            Return Me.lblMonthly_DepositAmount.Text
        End Get
        Set(value As String)
            Me.lblMonthly_DepositAmount.Text = value
        End Set
    End Property

    Public Property DirectMonthlyDown As String
        Get
            Return Me.lblMonthly_BasicDownPayment.Text
        End Get
        Set(value As String)
            Me.lblMonthly_BasicDownPayment.Text = value
        End Set
    End Property

    Public Property DirectMonthlyNumInstall As String
        Get
            Return Me.lblMonthly_NumberOfInstallments.Text
        End Get
        Set(value As String)
            Me.lblMonthly_NumberOfInstallments.Text = value
        End Set
    End Property

    Public Property DirectMonthlyRemainInstall As String
        Get
            Return Me.lblMonthly_RemainingInstallments.Text
        End Get
        Set(value As String)
            Me.lblMonthly_RemainingInstallments.Text = value
        End Set
    End Property

    Public Property DirectMonthlyInstallAmt As String
        Get
            Return Me.lblMonthly_InstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblMonthly_InstallmentAmount.Text = value
        End Set
    End Property

    Public Property DirectMonthlyBasicInstall As String
        Get
            Return Me.lblMonthly_BasicInstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblMonthly_BasicInstallmentAmount.Text = value
        End Set
    End Property

    Public Property DirectMonthlyInstallFee As String
        Get
            Return Me.lblMonthly_InstallmentFee.Text
        End Get
        Set(value As String)
            Me.lblMonthly_InstallmentFee.Text = value
        End Set
    End Property

    'Direct Bill Quarterly
    Public Property DirectQuarterlyDownPercent As String
        Get
            Return Me.lblQuarterly_DownPaymentPercentage.Text
        End Get
        Set(value As String)
            Me.lblQuarterly_DownPaymentPercentage.Text = value
        End Set
    End Property

    Public Property DirectQuarterlyDeposit As String
        Get
            Return Me.lblQuarterly_DepositAmount.Text
        End Get
        Set(value As String)
            Me.lblQuarterly_DepositAmount.Text = value
        End Set
    End Property

    Public Property DirectQuarterlyDown As String
        Get
            Return Me.lblQuarterly_BasicDownPayment.Text
        End Get
        Set(value As String)
            Me.lblQuarterly_BasicDownPayment.Text = value
        End Set
    End Property

    Public Property DirectQuarterlyNumInstall As String
        Get
            Return Me.lblQuarterly_NumberOfInstallments.Text
        End Get
        Set(value As String)
            Me.lblQuarterly_NumberOfInstallments.Text = value
        End Set
    End Property

    Public Property DirectQuarterlyRemainInstall As String
        Get
            Return Me.lblQuarterly_RemainingInstallments.Text
        End Get
        Set(value As String)
            Me.lblQuarterly_RemainingInstallments.Text = value
        End Set
    End Property

    Public Property DirectQuarterlyInstallAmt As String
        Get
            Return Me.lblQuarterly_InstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblQuarterly_InstallmentAmount.Text = value
        End Set
    End Property

    Public Property DirectQuarterlyBasicInstall As String
        Get
            Return Me.lblQuarterly_BasicInstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblQuarterly_BasicInstallmentAmount.Text = value
        End Set
    End Property

    Public Property DirectQuarterlyInstallFee As String
        Get
            Return Me.lblQuarterly_InstallmentFee.Text
        End Get
        Set(value As String)
            Me.lblQuarterly_InstallmentFee.Text = value
        End Set
    End Property

    'Direct Bill Semi-Annual
    Public Property DirectSemiAnnualDownPercent As String
        Get
            Return Me.lblSemiAnnual_DownPaymentPercentage.Text
        End Get
        Set(value As String)
            Me.lblSemiAnnual_DownPaymentPercentage.Text = value
        End Set
    End Property

    Public Property DirectSemiAnnualDeposit As String
        Get
            Return Me.lblSemiAnnual_DepositAmount.Text
        End Get
        Set(value As String)
            Me.lblSemiAnnual_DepositAmount.Text = value
        End Set
    End Property

    Public Property DirectSemiAnnualDown As String
        Get
            Return Me.lblSemiAnnual_BasicDownPayment.Text
        End Get
        Set(value As String)
            Me.lblSemiAnnual_BasicDownPayment.Text = value
        End Set
    End Property

    Public Property DirectSemiAnnualNumInstall As String
        Get
            Return Me.lblSemiAnnual_NumberOfInstallments.Text
        End Get
        Set(value As String)
            Me.lblSemiAnnual_NumberOfInstallments.Text = value
        End Set
    End Property

    Public Property DirectSemiAnnualRemainInstall As String
        Get
            Return Me.lblSemiAnnual_RemainingInstallments.Text
        End Get
        Set(value As String)
            Me.lblSemiAnnual_RemainingInstallments.Text = value
        End Set
    End Property

    Public Property DirectSemiAnnualInstallAmt As String
        Get
            Return Me.lblSemiAnnual_InstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblSemiAnnual_InstallmentAmount.Text = value
        End Set
    End Property

    Public Property DirectSemiAnnualBasicInstall As String
        Get
            Return Me.lblSemiAnnual_BasicInstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblSemiAnnual_BasicInstallmentAmount.Text = value
        End Set
    End Property

    Public Property DirectSemiAnnualInstallFee As String
        Get
            Return Me.lblSemiAnnual_InstallmentFee.Text
        End Get
        Set(value As String)
            Me.lblSemiAnnual_InstallmentFee.Text = value
        End Set
    End Property

    'Direct Bill Annually
    Public Property DirectAnnualDownPercent As String
        Get
            Return Me.lblAnnual_DownPaymentPercentage.Text
        End Get
        Set(value As String)
            Me.lblAnnual_DownPaymentPercentage.Text = value
        End Set
    End Property

    Public Property DirectAnnualDeposit As String
        Get
            Return Me.lblAnnual_DepositAmount.Text
        End Get
        Set(value As String)
            Me.lblAnnual_DepositAmount.Text = value
        End Set
    End Property

    Public Property DirectAnnualDown As String
        Get
            Return Me.lblAnnual_BasicDownPayment.Text
        End Get
        Set(value As String)
            Me.lblAnnual_BasicDownPayment.Text = value
        End Set
    End Property

    Public Property DirectAnnualNumInstall As String
        Get
            Return Me.lblAnnual_NumberOfInstallments.Text
        End Get
        Set(value As String)
            Me.lblAnnual_NumberOfInstallments.Text = value
        End Set
    End Property

    Public Property DirectAnnualRemainInstall As String
        Get
            Return Me.lblAnnual_RemainingInstallments.Text
        End Get
        Set(value As String)
            Me.lblAnnual_RemainingInstallments.Text = value
        End Set
    End Property

    Public Property DirectAnnualInstallAmt As String
        Get
            Return Me.lblAnnual_InstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblAnnual_InstallmentAmount.Text = value
        End Set
    End Property

    Public Property DirectAnnualBasicInstall As String
        Get
            Return Me.lblAnnual_BasicInstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblAnnual_BasicInstallmentAmount.Text = value
        End Set
    End Property

    Public Property DirectAnnualInstallFee As String
        Get
            Return Me.lblAnnual_InstallmentFee.Text
        End Get
        Set(value As String)
            Me.lblAnnual_InstallmentFee.Text = value
        End Set
    End Property

    'Credit Card Monthly
    Public Property CreditDownPercent As String
        Get
            Return Me.lblCreditCardMonthly_DownPaymentPercentage.Text
        End Get
        Set(value As String)
            Me.lblCreditCardMonthly_DownPaymentPercentage.Text = value
        End Set
    End Property

    Public Property CreditDeposit As String
        Get
            Return Me.lblCreditCardMonthly_DepositAmount.Text
        End Get
        Set(value As String)
            Me.lblCreditCardMonthly_DepositAmount.Text = value
        End Set
    End Property

    Public Property CreditDown As String
        Get
            Return Me.lblCreditCardMonthly_BasicDownPayment.Text
        End Get
        Set(value As String)
            Me.lblCreditCardMonthly_BasicDownPayment.Text = value
        End Set
    End Property

    Public Property CreditNumInstall As String
        Get
            Return Me.lblCreditCardMonthly_NumberOfInstallments.Text
        End Get
        Set(value As String)
            Me.lblCreditCardMonthly_NumberOfInstallments.Text = value
        End Set
    End Property

    Public Property CreditRemainInstall As String
        Get
            Return Me.lblCreditCardMonthly_RemainingInstallments.Text
        End Get
        Set(value As String)
            Me.lblCreditCardMonthly_RemainingInstallments.Text = value
        End Set
    End Property

    Public Property CreditInstallAmt As String
        Get
            Return Me.lblCreditCardMonthly_InstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblCreditCardMonthly_InstallmentAmount.Text = value
        End Set
    End Property

    Public Property CreditBasicInstall As String
        Get
            Return Me.lblCreditCardMonthly_BasicInstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblCreditCardMonthly_BasicInstallmentAmount.Text = value
        End Set
    End Property

    Public Property CreditInstallFee As String
        Get
            Return Me.lblCreditCardMonthly_InstallmentFee.Text
        End Get
        Set(value As String)
            Me.lblCreditCardMonthly_InstallmentFee.Text = value
        End Set
    End Property

    'Renewal Credit Card Monthly
    Public Property RenewalCreditDownPercent As String
        Get
            Return Me.lblRenewalCreditCardMonthly_DownPaymentPercentage.Text
        End Get
        Set(value As String)
            Me.lblRenewalCreditCardMonthly_DownPaymentPercentage.Text = value
        End Set
    End Property

    Public Property RenewalCreditDeposit As String
        Get
            Return Me.lblRenewalCreditCardMonthly_DepositAmount.Text
        End Get
        Set(value As String)
            Me.lblRenewalCreditCardMonthly_DepositAmount.Text = value
        End Set
    End Property

    Public Property RenewalCreditDown As String
        Get
            Return Me.lblRenewalCreditCardMonthly_BasicDownPayment.Text
        End Get
        Set(value As String)
            Me.lblRenewalCreditCardMonthly_BasicDownPayment.Text = value
        End Set
    End Property

    Public Property RenewalCreditNumInstall As String
        Get
            Return Me.lblRenewalCreditCardMonthly_NumberOfInstallments.Text
        End Get
        Set(value As String)
            Me.lblRenewalCreditCardMonthly_NumberOfInstallments.Text = value
        End Set
    End Property

    Public Property RenewalCreditRemainInstall As String
        Get
            Return Me.lblRenewalCreditCardMonthly_RemainingInstallments.Text
        End Get
        Set(value As String)
            Me.lblRenewalCreditCardMonthly_RemainingInstallments.Text = value
        End Set
    End Property

    Public Property RenewalCreditInstallAmt As String
        Get
            Return Me.lblRenewalCreditCardMonthly_InstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblRenewalCreditCardMonthly_InstallmentAmount.Text = value
        End Set
    End Property

    Public Property RenewalCreditBasicInstall As String
        Get
            Return Me.lblRenewalCreditCardMonthly_BasicInstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblRenewalCreditCardMonthly_BasicInstallmentAmount.Text = value
        End Set
    End Property

    Public Property RenewalCreditInstallFee As String
        Get
            Return Me.lblRenewalCreditCardMonthly_InstallmentFee.Text
        End Get
        Set(value As String)
            Me.lblRenewalCreditCardMonthly_InstallmentFee.Text = value
        End Set
    End Property

    'Renewal EFT Monthly
    Public Property RenewalEftDownPercent As String
        Get
            Return Me.lblRenewalEftMonthly_DownPaymentPercentage.Text
        End Get
        Set(value As String)
            Me.lblRenewalEftMonthly_DownPaymentPercentage.Text = value
        End Set
    End Property

    Public Property RenewalEftDeposit As String
        Get
            Return Me.lblRenewalEftMonthly_DepositAmount.Text
        End Get
        Set(value As String)
            Me.lblRenewalEftMonthly_DepositAmount.Text = value
        End Set
    End Property

    Public Property RenewalEftDown As String
        Get
            Return Me.lblRenewalEftMonthly_BasicDownPayment.Text
        End Get
        Set(value As String)
            Me.lblRenewalEftMonthly_BasicDownPayment.Text = value
        End Set
    End Property

    Public Property RenewalEftNumInstall As String
        Get
            Return Me.lblRenewalEftMonthly_NumberOfInstallments.Text
        End Get
        Set(value As String)
            Me.lblRenewalEftMonthly_NumberOfInstallments.Text = value
        End Set
    End Property

    Public Property RenewalEftRemainInstall As String
        Get
            Return Me.lblRenewalEftMonthly_RemainingInstallments.Text
        End Get
        Set(value As String)
            Me.lblRenewalEftMonthly_RemainingInstallments.Text = value
        End Set
    End Property

    Public Property RenewalEftInstallAmt As String
        Get
            Return Me.lblRenewalEftMonthly_InstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblRenewalEftMonthly_InstallmentAmount.Text = value
        End Set
    End Property

    Public Property RenewalEftBasicInstall As String
        Get
            Return Me.lblRenewalEftMonthly_BasicInstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblRenewalEftMonthly_BasicInstallmentAmount.Text = value
        End Set
    End Property

    Public Property RenewalEftInstallFee As String
        Get
            Return Me.lblRenewalEftMonthly_InstallmentFee.Text
        End Get
        Set(value As String)
            Me.lblRenewalEftMonthly_InstallmentFee.Text = value
        End Set
    End Property

    'Annual MTG
    Public Property AnnualMtgDownPercent As String
        Get
            Return Me.lblAnnualMtg_DownPaymentPercentage.Text
        End Get
        Set(value As String)
            Me.lblAnnualMtg_DownPaymentPercentage.Text = value
        End Set
    End Property

    Public Property AnnualMtgDeposit As String
        Get
            Return Me.lblAnnualMtg_DepositAmount.Text
        End Get
        Set(value As String)
            Me.lblAnnualMtg_DepositAmount.Text = value
        End Set
    End Property

    Public Property AnnualMtgDown As String
        Get
            Return Me.lblAnnualMtg_BasicDownPayment.Text
        End Get
        Set(value As String)
            Me.lblAnnualMtg_BasicDownPayment.Text = value
        End Set
    End Property

    Public Property AnnualMtgNumInstall As String
        Get
            Return Me.lblAnnualMtg_NumberOfInstallments.Text
        End Get
        Set(value As String)
            Me.lblAnnualMtg_NumberOfInstallments.Text = value
        End Set
    End Property

    Public Property AnnualMtgRemainInstall As String
        Get
            Return Me.lblAnnualMtg_RemainingInstallments.Text
        End Get
        Set(value As String)
            Me.lblAnnualMtg_RemainingInstallments.Text = value
        End Set
    End Property

    Public Property AnnualMtgInstallAmt As String
        Get
            Return Me.lblAnnualMtg_InstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblAnnualMtg_InstallmentAmount.Text = value
        End Set
    End Property

    Public Property AnnualMtgBasicInstall As String
        Get
            Return Me.lblAnnualMtg_BasicInstallmentAmount.Text
        End Get
        Set(value As String)
            Me.lblAnnualMtg_BasicInstallmentAmount.Text = value
        End Set
    End Property

    Public Property AnnualMtgInstallFee As String
        Get
            Return Me.lblAnnualMtg_InstallmentFee.Text
        End Get
        Set(value As String)
            Me.lblAnnualMtg_InstallmentFee.Text = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then

        End If
    End Sub
End Class