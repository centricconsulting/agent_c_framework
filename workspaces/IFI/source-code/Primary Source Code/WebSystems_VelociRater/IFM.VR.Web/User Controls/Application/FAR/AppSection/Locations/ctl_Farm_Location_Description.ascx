<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Farm_Location_Description.ascx.vb" Inherits="IFM.VR.Web.ctl_Farm_Location_Description" %>

<table style="margin-left: 40px;">
    <tr>
        <td>
            <label for="<%=txtAcres.ClientID%>">Acres</label><br />
            <asp:TextBox ID="txtAcres" MaxLength="6" Width="50" runat="server"></asp:TextBox></td>
        <td>
            <label for="<%=txtSection.ClientID %>">Section</label><br />
            <asp:TextBox ID="txtSection" MaxLength="15" Width="50" runat="server"></asp:TextBox></td>
        <td>
            <label for="<%=txtTownshipNum.ClientID %>">Township</label><br />
            <asp:TextBox ID="txtTownshipNum" MaxLength="15" Width="50" runat="server"></asp:TextBox></td>
        <td>
            <label for="<%=txtRange.ClientID %>">Range</label><br />
            <asp:TextBox ID="txtRange" MaxLength="15" Width="50" runat="server"></asp:TextBox></td>
        <td>
            <label for="<%=txtCounty.ClientID %>">County</label><br />
            <asp:TextBox ID="txtCounty" MaxLength="50" Width="90" runat="server"></asp:TextBox>
        </td>
        <td ID="tdStateAbbrev" runat="server">
            <label for="<%=ddStateAbbrev.ClientID %>">State</label><br />
            <asp:DropDownList ID="ddStateAbbrev" runat="server"></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td colspan="3">
            <label for="<%=txtDescription.ClientID %>">Description</label><br />
            <asp:TextBox ID="txtDescription" MaxLength="200" Width="240px" runat="server"></asp:TextBox></td>
        <td colspan="2">
            <label for="<%=ddTownshipName.ClientID %>">Township Name</label><br />
            <asp:DropDownList ID="ddTownshipName" Width="80px" runat="server"></asp:DropDownList>
            <asp:LinkButton Style="float: right" ID="lnkDelete" runat="server">Delete</asp:LinkButton>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdnTownshipName" runat="server" />