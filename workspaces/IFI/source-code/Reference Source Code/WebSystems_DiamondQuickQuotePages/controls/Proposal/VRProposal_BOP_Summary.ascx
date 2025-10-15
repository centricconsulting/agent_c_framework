<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_BOP_Summary.ascx.vb" Inherits="controls_Proposal_VRProposal_BOP_Summary" %>
<br />
<table width="100%" class="quickQuoteSectionTable">
    <tr>
        <td colspan="2" class="tableFieldHeaderLarger">Commercial BOP -
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
    <tr runat="server" id="BuildingRow">
        <td class="tableFieldValue">Building
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblBuildingPrem"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="PersPropRow">
        <td class="tableFieldValue">Personal Property
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblPersPropPrem"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="OccLiabRow">
        <td class="tableFieldValue">Occurrence Liability
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblOccLiabPrem"></asp:Label>
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
            <asp:Label runat="server" ID="lblOptCovsPrem"></asp:Label>
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
