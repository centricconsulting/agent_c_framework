<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomMultipleNames.ascx.vb" Inherits="IFM.VR.Web.ctlHomMultipleNames" %>

<!-- Added 1/23/18 for HOM Upgrade MLW -->
<table style="width:100%">
    <tr>
        <td runat="server" id="tdName"><asp:Label ID="lblName" runat="server">Name</asp:Label></td>
        <td runat="server" id="tdDescription"><asp:Label ID="lblDescription" runat="server">Description</asp:Label></td>
    </tr>
    <tr>
        <td runat="server" id="tdName1"><asp:TextBox ID="txtName" Width="100" MaxLength="250" runat="server"></asp:TextBox></td>
        <td runat="server" id="tdDescription1"> <asp:TextBox ID="txtDescription" TextMode="MultiLine" Width="300" runat="server"></asp:TextBox>
                <br />
                <asp:Label ID="lblMaxChar" runat="server" Text="Max Characters:" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>&nbsp;
                <asp:Label ID="lblMaxCharCount" runat="server" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label></td>

        <td><asp:LinkButton ID="lnkDelete" runat="server">Delete</asp:LinkButton></td>
    </tr>
</table>
<asp:HiddenField ID="hiddenIncreasedLimit" runat="server" Value="0" />
<asp:HiddenField ID="hiddenMaxCharCount" runat="server" />
