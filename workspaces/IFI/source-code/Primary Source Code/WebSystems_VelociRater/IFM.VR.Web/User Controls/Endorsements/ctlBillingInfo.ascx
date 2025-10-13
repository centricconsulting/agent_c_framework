<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlBillingInfo.ascx.vb" Inherits="IFM.VR.Web.ctlBillingInfo" %>
<%@ Register Src="~/User Controls/Endorsements/Billing/billingAccountSummary.ascx" TagPrefix="uc1" TagName="billingAccountSummary" %>
<%@ Register Src="~/User Controls/Endorsements/Billing/billingStatementInfo.ascx" TagPrefix="uc1" TagName="billingStatementInfo" %>
<%@ Register Src="~/User Controls/Endorsements/Billing/billingFutureInfo.ascx" TagPrefix="uc1" TagName="billingFutureInfo" %>
<%@ Register Src="~/User Controls/Endorsements/Billing/billingTransactionHistory.ascx" TagPrefix="uc1" TagName="billingTransactionHistory" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Billing_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Billing_Info_PPA" %>

<uc1:billingAccountSummary runat="server" id="billingAccountSummary" />
<uc1:billingStatementInfo runat="server" id="billingStatementInfo" />
<uc1:billingFutureInfo runat="server" id="billingFutureInfo" />
<uc1:billingTransactionHistory runat="server" id="billingTransactionHistory" />
<uc1:ctl_Billing_Info_PPA runat="server" id="ctl_Billing_Info_PPA" />
<style>
    .printBtn {
        margin-top: 10px;
    }
</style>
<div align="center">
    <asp:Button ID="btnPolicyHistory" runat="server" Text="Policy History" CssClass="StandardSaveButton printBtn" min-Width="150px" TabIndex="500"/>
    <asp:Button ID="btnPrintHistory" runat="server" Text="Print History" CssClass="StandardSaveButton printBtn" min-Width="150px" TabIndex="500" />
    <asp:Button ID="btnRate" runat="server" Text="Rate" CssClass="StandardSaveButton printBtn" min-Width="150px" TabIndex="500"/>
</div>