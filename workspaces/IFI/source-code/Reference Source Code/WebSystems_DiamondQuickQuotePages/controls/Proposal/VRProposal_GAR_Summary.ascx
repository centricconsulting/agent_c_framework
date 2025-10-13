<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_GAR_Summary.ascx.vb" Inherits="controls_Proposal_VRProposal_GAR_Summary" %>
<br runat="server" id="startBreak" />
<table width="100%" class="quickQuoteSectionTable">
    <tr runat="server" id="Monoline_header_Row" visible="true">
        <td align="left" colspan="2" class="tableFieldHeaderLarger">
            Commercial Garage<span runat="server" id="quoteNumberSection"> - <asp:Label runat="server" ID="lblQuoteNumber"></asp:Label></span>
        </td>
    </tr>
    <tr runat="server" id="CPP_header_Row" visible="false">
        <td align="left" colspan="2" class="tableFieldHeader">
            Commercial Garage
        </td>
    </tr>
    <tr runat="server" id="LiabilityRow">
        <td class="tableFieldValue">
            Liability
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblLiabilityPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="MedPayRow">
        <td class="tableFieldValue">
            Medical Payments
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblMedPayPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="UmUimRow">
        <td class="tableFieldValue">
            UM/UIM
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblUmUimPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="CompRow">
        <td class="tableFieldValue">
            Comprehensive
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCompPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="CollRow">
        <td class="tableFieldValue">
            Collision
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCollPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="TowRow">
        <td class="tableFieldValue">
            Towing and Labor
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblTowPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="RentRow">
        <td class="tableFieldValue">
            Rental Reimbursement
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblRentPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="GarageKeepersRow">
        <td class="tableFieldValue">
            Garage Keepers
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblGarageKeepersPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="MinPremAdjRow">
        <td class="tableFieldValue">
            Minimum Premium Adjustment
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblMinPremAdj"></asp:Label>
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