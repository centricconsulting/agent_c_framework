<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlBillingChangeSummary.ascx.vb" Inherits="IFM.VR.Web.ctlBillingChangeSummary" %>
<%@ Register Src="~/User Controls/Endorsements/Billing/billingAccountSummary.ascx" TagPrefix="uc1" TagName="billingAccountSummary" %>
<%@ Register Src="~/User Controls/Endorsements/Billing/billingStatementInfo.ascx" TagPrefix="uc1" TagName="billingStatementInfo" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlQuoteSummaryActions.ascx" TagPrefix="uc1" TagName="ctlQuoteSummaryActions" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlPayPlanOptions.ascx" TagPrefix="uc1" TagName="ctlPayPlanOptions" %>

<uc1:billingAccountSummary runat="server" id="ctlBillingChangeSummary" />
<uc1:ctlPayPlanOptions runat="server" ID="ctlPayPlanOptions" />
<uc1:billingStatementInfo runat="server" id="ctlChangeBillingDetails" />

<uc1:ctlQuoteSummaryActions runat="server" ID="ctlQuoteSummaryActions" />