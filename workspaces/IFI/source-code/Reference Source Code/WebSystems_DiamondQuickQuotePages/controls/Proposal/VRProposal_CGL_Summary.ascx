<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_CGL_Summary.ascx.vb" Inherits="controls_Proposal_VRProposal_CGL_Summary" %>
<br runat="server" id="startBreak" />
<table width="100%" class="quickQuoteSectionTable">
    <tr runat="server" id="Monoline_header_Row" visible="true">
        <td align="left" colspan="2" class="tableFieldHeaderLarger">
            Commercial General Liability<span runat="server" id="quoteNumberSection"> - <asp:Label runat="server" ID="lblQuoteNumber"></asp:Label></span>
        </td>
    </tr>
    <tr runat="server" id="CPP_header_Row" visible="false">
        <td align="left" colspan="2" class="tableFieldHeader">
            Commercial General Liability
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
    <tr runat="server" id="EnhEndRow">
        <td align="left" class="tableFieldValue">
            <asp:Label ID="lblCGLEnhancementEndorsementText" runat="server" Text="Enhancement Endorsement" ></asp:Label>
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblEnhEndPrem"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="FoodManufRow">
        <td align="left" class="tableFieldValue">
            Food Manufacturers Enhancement Endorsement Package
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblFoodManufPrem"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="PremOpsRow">
        <td align="left" class="tableFieldValue">
            Premises/Operations
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblPremOps"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="ProdCompOpsRow">
        <td align="left" class="tableFieldValue">
            Products/Completed Operations
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblProdCompOps"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="OptCovsRow">
        <td align="left" class="tableFieldValue">
            Additional Coverages
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblOptCovs"></asp:Label>
        </td>
    </tr>
    <asp:Repeater ID="rptDiscounts" runat="server">
        <ItemTemplate>
            <tr>
                <td width="520px" class="tableFieldValue">
                    <%# Eval("Description") %>
                </td>
                <td align="right" class="tableFieldValue">
                    <%# Eval("Premium", "{0:$#.00}") %>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr runat="server" id="MinPremAdjRow">
        <td class="tableFieldValue">
            Minimum Premium Adjustment
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblMinPremAdj"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="MinPrem_PremRow">
        <td align="left" class="tableFieldValue">
            Amount to Equal Minimum Premium (Premises) - (<asp:Label runat="server" ID="lblMinPrem_Prem"></asp:Label>)
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblAmtForMinPrem_Prem"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="MinPrem_ProdRow">
        <td align="left" class="tableFieldValue">
            Amount to Equal Minimum Premium (Products) - (<asp:Label runat="server" ID="lblMinPrem_Prod"></asp:Label>)
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblAmtForMinPrem_Prod"></asp:Label>
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