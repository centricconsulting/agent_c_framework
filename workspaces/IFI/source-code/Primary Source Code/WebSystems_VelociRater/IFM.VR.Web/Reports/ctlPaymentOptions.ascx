<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPaymentOptions.ascx.vb" Inherits="IFM.VR.Web.ctlPaymentOptions" %>
<%@ Register Src="~/Reports/ctlDisclaimer.ascx" TagPrefix="uc1" TagName="ctlDisclaimer" %>

<br />
<p width="100%" align="center" class="tableRowHeader">
    Payment Schedule
</p>
<table width="100%" class="quickQuoteSectionTable">
    <tr style="border-bottom: 1px solid black;">
        <td class="tableFieldHeader">
            <b>Plan</b>
        </td>
        <td id="Td1" class="tableFieldHeader" runat="server" visible="false">
            <b>Down Payment %</b>
        </td>
        <td id="Td2" class="tableFieldHeader" runat="server" visible="false">
            <b>Deposit Amount</b>
        </td>
        <td class="tableFieldHeader">
            <b>Down Payment</b>
        </td>
        <td id="Td3" class="tableFieldHeader" runat="server" visible="false">
            <b>Number of Installments</b>
        </td>
        <td class="tableFieldHeader">
            <b>Remaining Installments</b>
        </td>
        <td id="Td4" class="tableFieldHeader" runat="server" visible="false">
            <b>Installment Amount</b>
        </td>
        <td class="tableFieldHeader">
            <b>Installment Amount</b>
        </td>
        <td id="Td5" class="tableFieldHeader" runat="server" visible="false">
            <b>Installment Fee</b>
        </td>
    </tr>
    <tr>
        <td class="tableFieldValue">Simple Pay (EFT)*
        </td>
        <td id="Td6" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblEftMonthly_DownPaymentPercentage"></asp:Label>
        </td>
        <td id="Td7" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblEftMonthly_DepositAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblEftMonthly_BasicDownPayment"></asp:Label>
        </td>
        <td id="Td8" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblEftMonthly_NumberOfInstallments"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblEftMonthly_RemainingInstallments"></asp:Label>
        </td>
        <td id="Td9" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblEftMonthly_InstallmentAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblEftMonthly_BasicInstallmentAmount"></asp:Label>
        </td>
        <td id="Td10" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblEftMonthly_InstallmentFee"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="tableFieldValue">Direct Bill Monthly**
        </td>
        <td id="Td11" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblMonthly_DownPaymentPercentage"></asp:Label>
        </td>
        <td id="Td12" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblMonthly_DepositAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblMonthly_BasicDownPayment"></asp:Label>
        </td>
        <td id="Td13" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblMonthly_NumberOfInstallments"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblMonthly_RemainingInstallments"></asp:Label>
        </td>
        <td id="Td14" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblMonthly_InstallmentAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblMonthly_BasicInstallmentAmount"></asp:Label>
        </td>
        <td id="Td15" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblMonthly_InstallmentFee"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="tableFieldValue">Direct/Agent Bill Quarterly<asp:Label runat="server" ID="lblQuarterly_Plan" Visible="false"></asp:Label>
        </td>
        <td id="Td16" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblQuarterly_DownPaymentPercentage"></asp:Label>
        </td>
        <td id="Td17" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblQuarterly_DepositAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblQuarterly_BasicDownPayment"></asp:Label>
        </td>
        <td id="Td18" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblQuarterly_NumberOfInstallments"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblQuarterly_RemainingInstallments"></asp:Label>
        </td>
        <td id="Td19" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblQuarterly_InstallmentAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblQuarterly_BasicInstallmentAmount"></asp:Label>
        </td>
        <td id="Td20" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblQuarterly_InstallmentFee"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="tableFieldValue">Direct/Agent Bill Semi Annual
        </td>
        <td id="Td21" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblSemiAnnual_DownPaymentPercentage"></asp:Label>
        </td>
        <td id="Td22" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblSemiAnnual_DepositAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblSemiAnnual_BasicDownPayment"></asp:Label>
        </td>
        <td id="Td23" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblSemiAnnual_NumberOfInstallments"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblSemiAnnual_RemainingInstallments"></asp:Label>
        </td>
        <td id="Td24" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblSemiAnnual_InstallmentAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblSemiAnnual_BasicInstallmentAmount"></asp:Label>
        </td>
        <td id="Td25" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblSemiAnnual_InstallmentFee"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="tableFieldValue">Direct/Agent Bill Annual<asp:Label runat="server" ID="lblAnnual_Plan" Visible="false"></asp:Label>
        </td>
        <td id="Td26" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblAnnual_DownPaymentPercentage"></asp:Label>
        </td>
        <td id="Td27" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblAnnual_DepositAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblAnnual_BasicDownPayment"></asp:Label>
        </td>
        <td id="Td28" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblAnnual_NumberOfInstallments"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblAnnual_RemainingInstallments"></asp:Label>
        </td>
        <td id="Td29" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblAnnual_InstallmentAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblAnnual_BasicInstallmentAmount"></asp:Label>
        </td>
        <td id="Td30" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblAnnual_InstallmentFee"></asp:Label>
        </td>
    </tr>
    <tr id="Tr1" runat="server" visible="false">
        <td class="tableFieldValue">Credit Card Monthly
        </td>
        <td id="Td31" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblCreditCardMonthly_DownPaymentPercentage"></asp:Label>
        </td>
        <td id="Td32" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblCreditCardMonthly_DepositAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblCreditCardMonthly_BasicDownPayment"></asp:Label>
        </td>
        <td id="Td33" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblCreditCardMonthly_NumberOfInstallments"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblCreditCardMonthly_RemainingInstallments"></asp:Label>
        </td>
        <td id="Td34" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblCreditCardMonthly_InstallmentAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblCreditCardMonthly_BasicInstallmentAmount"></asp:Label>
        </td>
        <td id="Td35" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblCreditCardMonthly_InstallmentFee"></asp:Label>
        </td>
    </tr>
    <tr id="Tr2" runat="server" visible="false">
        <td class="tableFieldValue">Renewal Credit Card Monthly<asp:Label runat="server" ID="lblRenewalCreditCardMonthly_Plan" Visible="false"></asp:Label>
        </td>
        <td id="Td36" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblRenewalCreditCardMonthly_DownPaymentPercentage"></asp:Label>
        </td>
        <td id="Td37" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblRenewalCreditCardMonthly_DepositAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblRenewalCreditCardMonthly_BasicDownPayment"></asp:Label>
        </td>
        <td id="Td38" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblRenewalCreditCardMonthly_NumberOfInstallments"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblRenewalCreditCardMonthly_RemainingInstallments"></asp:Label>
        </td>
        <td id="Td39" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblRenewalCreditCardMonthly_InstallmentAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblRenewalCreditCardMonthly_BasicInstallmentAmount"></asp:Label>
        </td>
        <td id="Td40" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblRenewalCreditCardMonthly_InstallmentFee"></asp:Label>
        </td>
    </tr>
    <tr id="Tr3" runat="server" visible="false">
        <td class="tableFieldValue">Renewal EFT Monthly
        </td>
        <td id="Td41" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblRenewalEftMonthly_DownPaymentPercentage"></asp:Label>
        </td>
        <td id="Td42" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblRenewalEftMonthly_DepositAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblRenewalEftMonthly_BasicDownPayment"></asp:Label>
        </td>
        <td id="Td43" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblRenewalEftMonthly_NumberOfInstallments"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblRenewalEftMonthly_RemainingInstallments"></asp:Label>
        </td>
        <td id="Td44" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblRenewalEftMonthly_InstallmentAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblRenewalEftMonthly_BasicInstallmentAmount"></asp:Label>
        </td>
        <td id="Td45" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblRenewalEftMonthly_InstallmentFee"></asp:Label>
        </td>
    </tr>
    <tr id="Tr4" runat="server" visible="false">
        <td class="tableFieldValue">Annual MTG
        </td>
        <td id="Td46" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblAnnualMtg_DownPaymentPercentage"></asp:Label>
        </td>
        <td id="Td47" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblAnnualMtg_DepositAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblAnnualMtg_BasicDownPayment"></asp:Label>
        </td>
        <td id="Td48" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblAnnualMtg_NumberOfInstallments"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblAnnualMtg_RemainingInstallments"></asp:Label>
        </td>
        <td id="Td49" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblAnnualMtg_InstallmentAmount"></asp:Label>
        </td>
        <td class="tableFieldValue">
            <asp:Label runat="server" ID="lblAnnualMtg_BasicInstallmentAmount"></asp:Label>
        </td>
        <td id="Td50" class="tableFieldValue" runat="server" visible="false">
            <asp:Label runat="server" ID="lblAnnualMtg_InstallmentFee"></asp:Label>
        </td>
    </tr>
</table>
<div class="tableFieldHeader">
    <br />
    The payment plan options listed above are a summary of all policies quoted and are displayed for evaluation purposes only. Currently all policies are separately billed.
    <br />
    *If down payment is not collected, the annual premium will be spread amongst the remaining payments.
    <br />
    **$3 Service fee applies to installments
</div>
<br />
<br />
<uc1:ctlDisclaimer runat="server" ID="ctlDisclaimer" />
<br />