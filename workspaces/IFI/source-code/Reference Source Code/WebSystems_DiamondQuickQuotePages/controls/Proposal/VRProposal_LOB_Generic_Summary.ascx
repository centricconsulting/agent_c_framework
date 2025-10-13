<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_LOB_Generic_Summary.ascx.vb" Inherits="controls_Proposal_VRProposal_LOB_Generic_Summary" %>
<br />
<table width="100%" class="quickQuoteSectionTable">
    <tr>
        <td colspan="2" class="tableFieldHeaderLarger">
            <span runat="server" id="LobNameSection" visible="false"><asp:Label runat="server" ID="lblLobName"></asp:Label> - </span><asp:Label runat="server" ID="lblPolicyNumber"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="CommentsRow" visible="false">
        <td colspan="2" class="tableFieldValue">
            <asp:Label runat="server" ID="lblComments"></asp:Label>
        </td>
    </tr>
    <tr style="border-top: 1px solid black;">
        <td class="tableFieldHeader">
            Total Premium Due
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblTotalPremium"></asp:Label>
        </td>
    </tr>
</table>
<br />