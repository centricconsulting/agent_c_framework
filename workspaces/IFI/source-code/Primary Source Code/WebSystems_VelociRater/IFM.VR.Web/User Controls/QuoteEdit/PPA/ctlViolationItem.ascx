<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlViolationItem.ascx.vb" Inherits="IFM.VR.Web.ctlViolationItem" %>

<h3>
    <asp:Label ID="lblAccordianHeader" runat="server" Text="Label"></asp:Label><span style="float: right">
        <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" runat="server">Remove</asp:LinkButton></span></h3>
<div>
    <table style="width: 100%;">
        <tr>
            <td>
                <label for="<%=Me.ddViolationType.ClientID%>">*Violation Type</label></td>
            <td>
                <asp:DropDownList ID="ddViolationType" Width="250px" runat="server"></asp:DropDownList></td>
        </tr>
        <tr id="trViolationDate" runat="server">
            <td>
                <label for="<%=Me.txtViolationDate.ClientID%>">*Violation Date</label></td>
            <td>
                <asp:TextBox ID="txtViolationDate" MaxLength="10" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td colspan="2">
                <div style="width: 100%; text-align: center;">
                    <asp:Button Visible="false" ID="btnSubmit" runat="server" CssClass="StandardBlueButton" Text="Save" />
                </div>
            </td>
        </tr>
    </table>
</div>