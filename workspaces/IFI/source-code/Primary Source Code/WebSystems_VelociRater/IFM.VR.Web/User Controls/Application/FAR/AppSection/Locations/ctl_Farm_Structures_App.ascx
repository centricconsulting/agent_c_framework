<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Farm_Structures_App.ascx.vb" Inherits="IFM.VR.Web.ctl_Farm_Structures_App" %>

<div runat="server" id="divMain">
    <h3>
        <asp:Label ID="lblHeader" runat="server" Text="Label"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table style="width: 300px; margin-left: 50px;">
                    <tr>
                        <td style="width: 160px;">Structure</td>
                        <td>*Description</td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:Label ID="lblLineNumber" AssociatedControlID="txtDescription" runat="server"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtDescription" Width="200" runat="server"></asp:TextBox></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</div>
<asp:HiddenField ID="hddnAccord" runat="server" />