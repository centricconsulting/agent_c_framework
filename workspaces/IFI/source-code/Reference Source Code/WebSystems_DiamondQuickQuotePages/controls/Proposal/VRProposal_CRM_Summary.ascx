<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_CRM_Summary.ascx.vb" Inherits="controls_Proposal_VRProposal_CRM_Summary" %>
<br runat="server" id="startBreak" />
<table width="100%" class="quickQuoteSectionTable">
    <tr runat="server" id="Monoline_header_Row" visible="true">
        <td align="left" colspan="2" class="tableFieldHeaderLarger">Commercial Crime<span runat="server" id="quoteNumberSection"> -
            <asp:Label runat="server" ID="lblQuoteNumber"></asp:Label></span>
        </td>
    </tr>
    <tr runat="server" id="CPP_header_Row" visible="false">
        <td align="left" colspan="2" class="tableFieldHeader">Commercial Crime
        </td>
    </tr>
    <tr runat="server" id="trCRMEmployeeTheft">
        <td align="left" class="tableFieldValue">Employee Theft
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCRMEmployeeTheft"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCRMInsidePremises">
        <td align="left" class="tableFieldValue">Inside the Premises - Money and Securities
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCRMInsidePremises"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCRMOutsideThePremises">
        <td align="left" class="tableFieldValue">Outside the Premises
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCRMOutsideThePremises"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCRMForgeryAlteration">
        <td align="left" class="tableFieldValue">Forgery Or Alteration
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCRMForgeryAlteration"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCRMComputerFraud">
        <td align="left" class="tableFieldValue">Computer Fraud
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCRMComputerFraud"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCRMFundsTransferFraud">
        <td align="left" class="tableFieldValue">Funds Transfer Fraud
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCRMFundsTransferFraud"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCRMAmtToEqualMinumum">
        <td align="left" class="tableFieldValue">Amount to Equal Minimum Premium - (<asp:Label runat="server" ID="lblCRMAmtToEqualMinPremiumText"></asp:Label>)
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCRMAmtToEqualMinPremiumAmount"></asp:Label>
        </td>
    </tr>   
    <tr runat="server" id="OptCovsRow">
        <td class="tableFieldValue">
            Additional Coverages
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblOptCovsPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="CommentsRow" visible="false">
        <td colspan="2" class="tableFieldValue">
            <asp:Label runat="server" ID="lblComments"></asp:Label>
        </td>
    </tr>
    <tr style="border-top: 1px solid black;">
        <td align="left" class="tableFieldHeader">
            <asp:Label runat="server" ID="lblPremiumText" Text="Total Premium Due"></asp:Label>
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblTotalPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="SpacerRow">
        <td colspan="2">&nbsp;</td>
    </tr>
</table>
<br runat="server" id="endBreak" />