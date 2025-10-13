<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CAP_Driver.ascx.vb" Inherits="IFM.VR.Web.ctl_CAP_Driver" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlViolationList.ascx" TagPrefix="uc1" TagName="ctlViolationList" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Driver #"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkBtnAdd" runat="server" ToolTip="Add Driver" CssClass="RemovePanelLink">Add Driver</asp:LinkButton>
        <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" runat="server">Remove</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" runat="server">Save</asp:LinkButton>
    </span>
</h3>
<div id="divCAPDriver" runat="server">
    <table style="width:100%;">
        <tr>
            <td>
                <label for="<%=Me.txtFirstName.ClientID%>">*First Name:</label><br />
                <asp:TextBox ID="txtFirstName" MaxLength="55" runat="server"></asp:TextBox>
            </td>
            <td>
                <label for="<%=Me.txtBirthDate.ClientID%>">*Birth Date:</label><br />
                <asp:TextBox ID="txtBirthDate" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label for="<%=Me.txtMiddleName.ClientID%>">Middle Name:</label><br />
                <asp:TextBox ID="txtMiddleName" MaxLength="55" runat="server"></asp:TextBox>
            </td>
            <td>
                <label for="<%=Me.txtDLNumber.ClientID%>">*DL Number:</label><br />
                <asp:TextBox ID="txtDLNumber" MaxLength="55" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <label for="<%=Me.txtLastName.ClientID%>">*Last Name:</label><br />
                <asp:TextBox ID="txtLastName" MaxLength="55" runat="server"></asp:TextBox>
            </td>
            <td>
                <label for="<%=Me.ddlDLState.ClientID%>">*DL State:</label><br />
                <asp:DropDownList ID="ddlDLState" runat="server"></asp:DropDownList>
            </td>
        </tr>
    </table>
    <uc1:ctlViolationList runat="server" ID="ctlViolationList" visible="false" />
   </div>
   
