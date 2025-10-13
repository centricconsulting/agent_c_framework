<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_PaymentOptions_Generic.ascx.vb" Inherits="controls_Proposal_VRProposal_PaymentOptions_Generic" %>
<br />
<style type="text/css">
    p.paymentP {
        font-size: 22px;
        color: #00AFCF;
        text-align: left;
    }

    td.paymentTdLeft {
        font-family: Calibri, Candara, Segoe, Segoe UI, Optima, Arial, sans-serif;
        vertical-align: top;
        font-weight: normal;
        font-size: 18px;
        text-align: justify;
        width: 50%;
    }

    td.paymentTdRight {
        font-family: Calibri, Candara, Segoe, Segoe UI, Optima, Arial, sans-serif;
        vertical-align: top;
        font-weight: normal;
        font-size: 18px;
        text-align: left;
        width: 50%;
    }

    th.paymentThLeft {
        text-align: justify;
        font-family: Calibri, Candara, Segoe, Segoe UI, Optima, Arial, sans-serif;
        vertical-align: top;
        font-size: 18px;
        font-weight: bold;
        width: 50%;
    }

    th.paymentThRight {
        text-align: justify;
        font-family: Calibri, Candara, Segoe, Segoe UI, Optima, Arial, sans-serif;
        vertical-align: top;
        font-size: 18px;
        font-weight: bold;
        width: 50%;
    }
</style>

<table class="quickQuoteSectionTable">
    <tr>
        <td>
            <img runat="server" id="MainPageLogo" class="QuickQuoteProposalHeaderLogo" />
        </td>
        <td style="text-align: right; padding-left: 100px;">
            <asp:Label runat="server" ID="lblLogo" class="QuickQuoteProposalHeaderLogo"></asp:Label>
        </td>
    </tr>
</table>
<br />
<br />

<div style="background-color: #00AFCF; text-align: center; font-size: 32px; color: white; padding-top: 10px; padding-bottom: 10px" class="tableRowHeaderLarger">Your Payment Options</div>
<br />

<table class="quickQuoteSectionTable">
    <tr>
        <td colspan="8">
            <p class="tableField paymentP">
                Available Pay Plans. Currently all policies are billed separately:
            </p>
        </td>
    </tr>
</table>
<br />
<table class="quickQuoteSectionTable">
    <tr>
        <th class="tableFieldHeader paymentThLeft">Annual Pay
        </th>
        <th style="padding-left: 160px;" class="tableFieldHeader paymentThRight">Semi_Annual Pay
        </th>
    </tr>
    <tr>
        <td class="paymentTdLeft">No installments</td>
        <td style="padding-left: 160px;" class="paymentTdRight">2 installments</td>
    </tr>
</table>
<br />

<table class="quickQuoteSectionTable">
    <tr>
        <th class="tableFieldHeader paymentThLeft">Quarterly Pay
        </th>
        <th class="tableFieldHeader paymentThRight">Monthly Pay
        </th>
    </tr>
    <tr>
        <td class="paymentTdLeft">4 installments</td>
        <td class="paymentTdRight">12 installments. A $3 servive fee applies to each
        policy per installments when using regular Monthly
        Pay, however, this can avoided by signing up for
        automatic payments by EFT or RCC. </td>
    </tr>
</table>
<table class="quickQuoteSectionTable">
    <tr>
        <td colspan="8">
            <p class="tableField paymentP">
                Available Payment Options:
            </p>
        </td>
    </tr>
    <br />
    <tr>
        <th class="tableFieldHeader paymentThLeft">Credit/Debit Card
        </th>
        <th class="tableFieldHeader paymentThRight">eCheck
        </th>
    </tr>
    <tr>
        <td class="paymentTdLeft">Visa, MasterCard, Discover, American Express
        </td>
        <td class="paymentTdRight">One-time electronic payment from a checking or
        savings account.
        </td>
    </tr>
</table>
<br />

<table class="quickQuoteSectionTable">
    <tr>
        <th class="tableFieldHeader paymentThLeft">Online
        </th>
        <th class="tableFieldHeader paymentThRight">Phone
        </th>
    </tr>
    <tr>
        <td class="paymentTdLeft">IndianaFarmers.com. You can make a one-time
            <br />
            payment or create an account.
        </td>
        <td class="paymentTdRight">800.477.1660--payment with assistance of our
       customer service staff or via voice recognition.
        </td>
    </tr>
</table>


<br />

<table class="quickQuoteSectionTable">

    <tr>
        <th class="tableFieldHeader paymentThLeft">RCC
        </th>
        <th class="tableFieldHeader paymentThRight">EFT
        </th>
    </tr>
    <tr>
        <td class="paymentTdLeft">Recurring Credit/Debit card monthly payment 
        program
        </td>
        <td class="paymentTdRight">Electronic Funds Transfer(EFT) through our EFT
       payment program. Once your EFT acount is
       established, all future monthly payments will be
       handled through the EFT program.
        </td>
    </tr>
</table>
<br />

<table class="quickQuoteSectionTable">
    <tr>
        <th class="tableFieldHeader paymentThLeft">Lockbox
        </th>
    </tr>
    <tr>
        <td class="paymentTdLeft">Payment via physical checks to our lockbox 
        address.
        </td>
    </tr>
</table>
<br />
<table class="quickQuoteSectionTable">

    <tr>
        <td colspan="8">
            <p class="tableField paymentP">
                Your agent can help you choose the pay plan and payment option that best fit your needs.
            </p>
        </td>
    </tr>
</table>
<br />
