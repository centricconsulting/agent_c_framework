<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_WCP_Summary.ascx.vb" Inherits="controls_Proposal_VRProposal_WCP_Summary" %>
<br />
<table width="100%" class="quickQuoteSectionTable">
    <tr>
        <td colspan="2" class="tableFieldHeaderLarger">
            Workers Compensation - <asp:Label runat="server" ID="lblQuoteNumber"></asp:Label>
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
    <tr runat="server" id="EstPlanRow">
        <td class="tableFieldValue">
            Total Estimated Plan Premium
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblEstPlanPrem"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="IncreasedLimitRow">
        <td class="tableFieldValue">
            Increased Limit
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblIncreasedLimit"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="ExpModRow">
        <td class="tableFieldValue">
            Experience Modification
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblExpMod"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="SchedModRow">
        <td class="tableFieldValue">
            Schedule Modification
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblSchedMod"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="PremDiscountRow">
        <td class="tableFieldValue">
            Premium Discount
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblPremDiscount"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="LossConstantRow">
        <td class="tableFieldValue">
            Loss Constant
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblLossConstant"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="ExpConstantRow">
        <td class="tableFieldValue">
            Expense Constant
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblExpConstant"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="CertActsOfTerrRow">
        <td class="tableFieldValue">
            Certified Acts of Terrorism Coverage
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCertActsOfTerr"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="CatastropheOtherThanTerrorismRow">  
        <td class="tableFieldValue">
            Catastrophe (other than Certified Acts of Terrorism)
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCatastropheOtherThanTerrorism"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="MinPremAdjRow">
        <td class="tableFieldValue">
            Amount to Equal Minimum Premium - (<asp:Label runat="server" ID="lblMinPrem"></asp:Label>)
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblAmtForMinPrem"></asp:Label>
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
    <tr runat="server" id="TotPremRow">
        <td class="tableFieldValue">
            Total Estimated Written Premium
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblTotalPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="SecInjFundRow">
        <td class="tableFieldValue">
            Indiana Second Injury Fund Surcharge
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblSecInjFund"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="KY_SpecFundAssessRow">
        <td class="tableFieldValue">
            Kentucky Special Fund Assessment
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblKY_SpecFundAssess"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="KY_SurchargeRow">
        <td class="tableFieldValue">
            Kentucky Surcharge
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblKY_Surcharge"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="IL_CommissionsFundSurchargeRow">
        <td class="tableFieldValue">
            Illinois Commissions Operations Fund Surcharge
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblIL_CommissionsOperationsFundSurcharge"></asp:Label>
        </td>
    </tr>
    <tr style="border-top: 1px solid black;">
        <td class="tableFieldHeader">
            Total Premium Due
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblTotalPlusSecInjFund"></asp:Label>
        </td>
    </tr>
</table>
<br />
<!--testing-->
<%--<br /><br /><br /><br /><br /><br /><br /><br />--%>