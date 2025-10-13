<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_CAP_Summary.ascx.vb" Inherits="controls_Proposal_VRProposal_CAP_Summary" %>
<br />
<table width="100%" class="quickQuoteSectionTable">
    <tr>
        <td colspan="2" class="tableFieldHeaderLarger">Commercial Auto -
            <asp:Label runat="server" ID="lblQuoteNumber"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="EffDateRow">
        <td align="left" class="tableFieldValue">
            <b>Effective Date:</b> <asp:Label runat="server" ID="lblEffectiveDate"></asp:Label>
        </td>
        <td align="right" class="tableFieldValue">
            <b>Expiration Date:</b> <asp:Label runat="server" ID="lblExpirationDate"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="LiabilityRow">
        <td class="tableFieldValue">Liability
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblLiabilityPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="MedPayRow">
        <td class="tableFieldValue">Medical Payments
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblMedPayPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="UmUimRow">
        <td class="tableFieldValue">UM/UIM
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblUmUimPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="UmRow" Visible="false">
        <td class="tableFieldValue">Uninsured Motorist
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblUmPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="UmpdRow" Visible="false">
        <td class="tableFieldValue">Uninsured Motorist Property Damage
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblUmpdPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="UimRow" Visible="false">
        <td class="tableFieldValue">Underinsured Motorist
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblUimPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="CompRow">
        <td class="tableFieldValue">Comprehensive
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCompPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="CollRow">
        <td class="tableFieldValue">Collision
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCollPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="TowRow">
        <td class="tableFieldValue">Towing and Labor
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblTowPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="RentRow">
        <td class="tableFieldValue">Rental Reimbursement
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblRentPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="GarageKeepersRow">
        <td class="tableFieldValue">Garage Keepers
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblGarageKeepersPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="EnhEndRow">
        <td class="tableFieldValue">Enhancement Endorsement
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblEnhEndPrem"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="MinPremAdjRow">
        <td class="tableFieldValue">Minimum Premium Adjustment
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblMinPremAdj"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="OptCovsRow">
        <td class="tableFieldValue">Additional Coverages
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblOptCovsPremium"></asp:Label>
        </td>
    </tr>
    <tr align="left" runat="server" id="trLargePremiumDiscount" visible="false">
        <td class="tableFieldValue">Total Ohio Premium Discount
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label ID="lblTotalLargePremiumDiscount" runat="server"></asp:Label>
        </td>
    </tr>
    <tr style="border-top: 1px solid black;">
        <td class="tableFieldHeader">Total Premium Due
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblTotalPremium"></asp:Label>
        </td>
    </tr>
</table>
<br />
