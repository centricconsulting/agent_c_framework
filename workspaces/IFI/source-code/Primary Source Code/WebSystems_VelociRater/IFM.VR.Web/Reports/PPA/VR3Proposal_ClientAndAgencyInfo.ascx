<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="VR3Proposal_ClientAndAgencyInfo.ascx.vb" Inherits="IFM.VR.Web.VR3Proposal_ClientAndAgencyInfo" %>
<br />
<table width="100%" class="quickQuoteSectionTable">
    <tr>
        <td width="50%" valign="top">
            <asp:Label ID="lblPrepared" runat="server" Text="Prepared for:"></asp:Label>
            <br />
            <asp:Label runat="server" ID="lblClientInfo"></asp:Label>
        </td>
        <td width="50%" valign="top">
            <asp:Label ID="lblAgent" runat="server" Text="Prepared by:"></asp:Label>
            <br />
            <asp:Label runat="server" ID="lblAgencyInfo"></asp:Label>
            <br />
            <asp:Label ID="lblProducer" runat="server" Text="Producer Code: "></asp:Label>
            <asp:Label ID="lblProducerCode" runat="server"></asp:Label>
        </td>
    </tr>
</table>
<br />