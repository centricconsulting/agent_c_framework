<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlResidentName_App.ascx.vb" Inherits="IFM.VR.Web.ctlResidentName_App" %>

<tr>
    <td>
        <asp:TextBox ID="txtFirstName" Width="85px" MaxLength="40" runat="server"></asp:TextBox></td>
    <td>
        <asp:TextBox ID="txtLastName" Width="85px" MaxLength="40" runat="server"></asp:TextBox>
    </td>
    <td>
        <asp:TextBox ID="txtDOB" Width="85px" MaxLength="12" runat="server"></asp:TextBox>
    </td>
    <td>
        <asp:TextBox ID="txtRelationship" Width="85px" MaxLength="100" runat="server"></asp:TextBox>
    </td>
    <td>
        <asp:LinkButton ID="lnkDelete" OnClientClick="return confirm('Delete?');" runat="server">Delete</asp:LinkButton></td>
</tr>