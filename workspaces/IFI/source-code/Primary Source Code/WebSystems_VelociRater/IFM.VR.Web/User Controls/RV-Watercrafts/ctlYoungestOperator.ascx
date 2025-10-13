<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlYoungestOperator.ascx.vb" Inherits="IFM.VR.Web.ctlYoungestOperator" %>

<div runat="server" id="divYoungestOperator">
    <h3>
        <asp:Label ID="lblHeader" runat="server" Text="Youngest Operator in the Household"></asp:Label>
        <span style="float: right">
            <asp:LinkButton ID="lnkBtnClear" CssClass="RemovePanelLink" ToolTip="Clear Operator" runat="server">Clear</asp:LinkButton>                
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" OnClientClick="StopEventPropagation(event);DisableFormOnSaveRemoves();" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div id="divYoungestOperatorInformation" runat="server">
        <table style="width: 100%;">
            <tr>
                <td style="text-align: right">
                    <label for="<%=txtFirstName.ClientID %>">*First Name</label>
                </td>
                <td>
                    <asp:TextBox ID="txtFirstName" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right">
                    <label for="<%=Me.txtLastName.ClientID%>">*Last Name</label>
                </td>
                <td>
                    <asp:TextBox ID="txtLastName" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right">
                    <label for="<%=Me.txtBirthDate.ClientID%>">*Birth Date</label>
                </td>
                <td>
                    <asp:TextBox ID="txtBirthDate" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hiddenOperatorId" runat="server" />
    </div>
</div>
<asp:HiddenField ID="HiddenField1" runat="server" />
<asp:HiddenField ID="HiddenFieldMainAccord" runat="server" />