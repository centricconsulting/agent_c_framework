<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_NewPayment_Page.ascx.vb" Inherits="controls_Proposal_VRProposal_NewPayment_Page" %>
<%@ Register Src="~/controls/Proposal/VRProposal_Footer.ascx" TagPrefix="uc" TagName="VRProposal_Footer" %> <%--added 3/22/2021--%>

<style type="text/css">
    .proposalPaymentGenericBodyText {
        font-family: "Segoe UI";
        font-size: 11pt;
        font-weight: normal;
        color: #000000; 
        line-height: 1.5;
        margin-bottom: 360px;
        margin-top: 1in;
        margin-left: .5in;
        margin-right: .5in;
    }
    .proposalPaymentGenericSectionHeader {
            font-size: 14pt;
            font-weight: bold;
            color: #00AFCF; 
            margin-top: 30px;
    }
    .propsalPaymentGenericBold {
        font-weight: bold;
    }
    .proposalPaymentGenericFooter {
        position: fixed;
        bottom: 0;
    }
</style>

<div class="proposalPaymentGenericBodyText">

    <div class="proposalPaymentGenericSectionHeader">Pay Plans</div> 

    <span class="propsalPaymentGenericBold">Annual Pay</span>  - 1 payment <br />
    <span class="propsalPaymentGenericBold">Semi Annual Pay</span>  - 2 installments <br />
    <span class="propsalPaymentGenericBold">Quarterly Pay</span>  - 4 installments <br />
    <span class="propsalPaymentGenericBold">Monthly Pay</span>  - 12 installments: A $3 service fee applies to each policy per installment when using
    Monthly Pay; however, this can be avoided by signing up for automatic payments by electronic funds
    transfer. <br />

    <div class="proposalPaymentGenericSectionHeader">Payment Options</div> 
    <span class="propsalPaymentGenericBold">Pay online</span> - Use your credit card, debit card, or echeck to pay online at indianafarmers.com. You can
    make a one-time payment or register to create an account to make ongoing payments. <br /><br />
    <span class="propsalPaymentGenericBold">Recurring payments</span> <span ID="RecurringPaymentsText" runat="server"></span><br /><br />
    <span class="propsalPaymentGenericBold">Pay by phone</span>  - Call 800.477.1660 for assistance from our customer service staff members or let our
    automated system lead you through the payment process. You can use your credit card or transfer funds
    through your checking or savings account. <br /><br />
    <span class="propsalPaymentGenericBold">Pay by U. S. Mail</span> - Send a physical check or money order to our payment address at Indiana Farmers
    Insurance, PO Box 856, Indianapolis, IN 46206 <br />

    <div class="proposalPaymentGenericSectionHeader">About Indiana Farmers Insurance</div> 
    Indiana Farmers Insurance is a mutual insurance company and has existed to serve our customers since
    1877. We have no stockholders who expect a return on their investment. We focus exclusively on the
    financial protection of our policyholders and are not burdened by the expectations of the financial
    markets. All this means that when you buy a policy from Indiana Farmers Insurance, your premiums only
    go to pay for losses and to operate the company.<br /><br />
    Insurance can be complicated so it is important to have a trusted expert who will guide you through your
    insurance experience. We partner with only the best agents, and like us, most of our agencies have been
    serving customers for generations. <br />

    <div class="proposalPaymentGenericSectionHeader">Thank you!</div> 
    Thank you for considering <asp:Label runat="server" ID="lblAgencyName"></asp:Label> and Indiana Farmers Insurance for your insurance needs.
    Please reach out to us if you have any questions. We look forward to serving you! <br />
</div>
<uc:VRProposal_Footer ID="footer1" runat="server" />