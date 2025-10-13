<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlMobileHome.ascx.vb" Inherits="IFM.VR.Web.ctlMobileHome" %>

<div runat="server" id="MobileHomeDiv" class="standardSubSection">
    <h3>Mobile Home
                <span style="float: right;">
                    <asp:LinkButton ID="lnkClearMobileHome" runat="server" ToolTip="Clear Mobile Home" CssClass="RemovePanelLink">Clear</asp:LinkButton>
                    <asp:LinkButton ID="lnkSaveMobileHome" runat="server" ToolTip="Save Page" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
    </h3>
    <div id="MobileHomeContentDiv">
        <table style="width: 100%">
            <tr>
                <td>
                    <label for="<%=Me.ddlTieDown.ClientID%>">*Tie Down</label><br />
                    <asp:DropDownList ID="ddlTieDown" Width="150px" TabIndex="15" runat="server"></asp:DropDownList></td>
                <td>
                    <label for="<%=Me.txtMake.ClientID%>">Make</label><br />
                    <asp:TextBox ID="txtMake" MaxLength="30" TabIndex="18" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.ddlSkirting.ClientID%>">*Skirting</label><br />
                    <asp:DropDownList ID="ddlSkirting" Width="150px" TabIndex="16" runat="server"></asp:DropDownList></td>
                <td>
                    <label for="<%=Me.txtModel.ClientID%>">Model</label><br />
                    <asp:TextBox ID="txtModel" MaxLength="30" TabIndex="19" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.ddlFoundation.ClientID%>">*Foundation Type</label><br />
                    <asp:DropDownList ID="ddlFoundation" Width="150px" TabIndex="17" runat="server"></asp:DropDownList></td>
                <td>
                    <label for="<%=Me.txtVin.ClientID%>">VIN</label><br />
                    <asp:TextBox ID="txtVin" MaxLength="30" TabIndex="20" runat="server"></asp:TextBox></td>
            </tr>
        </table>
        <asp:HiddenField ID="HiddenField1" runat="server" />
    </div>
</div>