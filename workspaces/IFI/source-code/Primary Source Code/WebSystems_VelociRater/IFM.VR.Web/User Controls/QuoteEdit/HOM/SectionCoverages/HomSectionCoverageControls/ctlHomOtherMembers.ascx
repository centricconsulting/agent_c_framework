<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomOtherMembers.ascx.vb" Inherits="IFM.VR.Web.ctlHomOtherMembers" %>
<table style="width:100%">
    <tr>
        <td runat="server" id="tdOtherDescription" style="vertical-align:top;">
            <div runat="server" id="divViewLink" visible="true" style="display:block;">
                <asp:LinkButton ID="lnkView" runat="server">View/Edit Other Member</asp:LinkButton>
            </div>
            <div runat="server" id="divOtherDescription" visible="true" style="display:block;">
                <asp:Label ID="lblOtherDescription" runat="server">*Description</asp:Label>
                <br />
                <asp:TextBox ID="txtOtherDescription" Width="150" MaxLength="250" runat="server"></asp:TextBox>
            </div>
        </td>
        <td runat="server" id="tdOtherName" style="vertical-align:bottom;">
            <div runat="server" id="divOtherName" style="display:block;">
                <asp:Label ID="lblName" runat="server">*First Name</asp:Label>
                <br />
                <asp:TextBox ID="txtOtherFirstName" Width="125" MaxLength="50" runat="server"></asp:TextBox>
                <br />
                <asp:Label ID="lblOtherMiddleName" runat="server">Middle Name</asp:Label>
                <br />
                <asp:TextBox ID="txtOtherMiddleName" Width="125" MaxLength="50" runat="server"></asp:TextBox>
                <br />
                <asp:Label ID="lblOtherLastName" runat="server">*Last Name</asp:Label>
                <br />
                <asp:TextBox ID="txtOtherLastName" Width="125" MaxLength="50" runat="server"></asp:TextBox>
                <br />
                <asp:Label ID="lblOtherSuffix" runat="server">Suffix</asp:Label>
                <br />
                <asp:DropDownList ID="ddOtherSuffix" runat="server"></asp:DropDownList>
            </div>
        </td>
        <td style="vertical-align:bottom;"><asp:LinkButton ID="lnkDelete" runat="server">Delete</asp:LinkButton></td>
    </tr>
</table>


