<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlResidentName_Commercial_App.ascx.vb" Inherits="IFM.VR.Web.ctlResidentName_Commercial_App" %>

<tr>
    <td>
        <asp:TextBox ID="txtComm1Name" Width="185px" MaxLength="40" runat="server" disabled="disabled"></asp:TextBox></td>
    <td>
        <asp:TextBox ID="txtComm2Name" Width="185px" MaxLength="40" runat="server" disabled="disabled"></asp:TextBox>
    </td>
    <td>
        <asp:LinkButton ID="lnkDelete" OnClientClick="return confirm('Delete?');" runat="server">Delete</asp:LinkButton>
    </td>
</tr>