<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_CPR_Summary.ascx.vb" Inherits="controls_Proposal_VRProposal_CPR_Summary" %>

<br runat="server" id="startBreak" />
<table width="100%" class="quickQuoteSectionTable">
    <tr runat="server" id="Monoline_header_Row" visible="true">
        <td align="left" colspan="2" class="tableFieldHeaderLarger">Commercial Property<span runat="server" id="quoteNumberSection"> -
            <asp:Label runat="server" ID="lblQuoteNumber"></asp:Label></span>
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
    <tr runat="server" id="CPP_header_Row" visible="false">
        <td align="left" colspan="2" class="tableFieldHeader">Commercial Property
        </td>
    </tr>
    <tr runat="server" id="BuildingRow">
        <td align="left" class="tableFieldValue">Building Coverage
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblBuildingPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="PersPropRow">
        <td align="left" class="tableFieldValue">Personal Property Coverage
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblPersPropPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="PersPropOfOthersRow">
        <td align="left" class="tableFieldValue">Personal Property Of Others
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblPersPropOfOthersPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="BusIncRow">
        <td align="left" class="tableFieldValue">
            <asp:Label ID="lblBIC" runat="server" Text="Business Income Coverage"></asp:Label>
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblBusIncPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="PropInTheOpenRow">
        <td align="left" class="tableFieldValue">Property in the Open
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblPropInTheOpenPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="EnhEndRow">
        <td align="left" class="tableFieldValue">
            <asp:Label ID="lblCPREnhancementEndorsementText" runat="server" Text="Enhancement Endorsement"></asp:Label>
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblEnhEndPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="EbRow">
        <td align="left" class="tableFieldValue">Equipment Breakdown
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblEbPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="EqRow">
        <td align="left" class="tableFieldValue">Earthquake
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblEqPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="FoodManufRow">
        <td align="left" class="tableFieldValue">Food Manufacturers Enhancement Endorsement Package
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblFoodManufPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCPRAmountToMeetMinimumPremiumRow">
        <td align="left" class="tableFieldValue">Amount to Equal Minimum Premium
            <%--            Amount to Equal Minimum Premium - (<asp:Label runat="server" ID="lblCPRAmountToMeetMinimumAmountText"></asp:Label>)--%>
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCPRAmountToEqualMinimumPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="OptCovsRow">
        <td class="tableFieldValue">Additional Coverages
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblOptCovsPremium"></asp:Label>
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
