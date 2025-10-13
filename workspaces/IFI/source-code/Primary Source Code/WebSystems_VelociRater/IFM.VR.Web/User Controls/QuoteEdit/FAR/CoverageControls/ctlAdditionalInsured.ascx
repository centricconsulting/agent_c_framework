<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlAdditionalInsured.ascx.vb" Inherits="IFM.VR.Web.ctlAdditionalInsured" %>

<style type="text/css">
    .div {
        border-collapse: collapse;
        margin: 0;
        padding: 0;
    }
</style>

<div id="dvFarmAddlInsured" runat="server" class="div" onkeydown="return (event.keyCode!=13)">
    <table style="width: 100%">
        <tr>
            <td class="CovTableColumn" style="width: 100px">
                <asp:Label ID="lblAIType" runat="server" Text="Type"></asp:Label>
            </td>
            <td class="CovTableColumn">
                <asp:DropDownList ID="ddlAIType" runat="server"></asp:DropDownList>
            </td>
        </tr>
    </table>
    <table id="tblAI" runat="server" style="width: 100%">
        <tr>
            <td style="width: 140px">
                <asp:Label ID="lblAIBusinessReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblAIBusiness" runat="server" Text="Business Name"></asp:Label>
            </td>
            <td style="width: 10px"></td>
            <td style="width: 140px">
                <asp:Label ID="lblAILastReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblAILastName" runat="server" Text="Last Name"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblAIFirstReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblAIFirstName" runat="server" Text="First Name"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtAIBusiness" runat="server" MaxLength="50"></asp:TextBox>
            </td>
            <td style="width: 10px">
                <asp:Label ID="lblAIOr" runat="server" Text="or"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtAILastName" runat="server" MaxLength="50"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="txtAIFirstName" runat="server" MaxLength="50"></asp:TextBox>
            </td>
            <td style="width: 50px">
                <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" OnClick="OnConfirm" OnClientClick="ConfirmDialog()">Delete </asp:LinkButton>
            </td>
        </tr>
    </table>
</div>