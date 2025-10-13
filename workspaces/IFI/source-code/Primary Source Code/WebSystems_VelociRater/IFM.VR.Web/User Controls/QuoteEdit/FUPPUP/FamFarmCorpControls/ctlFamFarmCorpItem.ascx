<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFamFarmCorpItem.ascx.vb" Inherits="IFM.VR.Web.ctlFamFarmCorpItem" %>
<div id="dvSchedLimit" runat="server" class="div">
    <table style="width: 100%" class="tableBorder">
        <tr>
            <td style="width: 15px"></td>
            <td style="width: 100px; vertical-align: central" class="CovTableColumn">
                <asp:Label ID="lblDescription" runat="server" Text="Label">Description: </asp:Label>
            </td>
            <td style="vertical-align: central">
                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Width="175px" MaxLength="250" ToolTip="250 Max Characters"></asp:TextBox>
            </td>
            <td style="width: 100px">
                <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" OnClick="OnConfirm" OnClientClick="ConfirmDialog()">Delete </asp:LinkButton>
            </td>
        </tr>
    </table>
</div>