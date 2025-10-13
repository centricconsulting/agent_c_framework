<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WaterCraftOperatorItem.ascx.vb" Inherits="IFM.VR.Web.WaterCraftOperatorItem" %>

<div runat="server" id="divAccord" class="standardSubSection">


    <h3>
        <asp:Label ID="lblAccordianHeader" runat="server" Text="Operator" ></asp:Label>
        <span style="float: right;">
            
            <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" OnClientClick="StopEventPropagation(event);DisableFormOnSaveRemoves();" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
            <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" OnClientClick="StopEventPropagation(event);DisableFormOnSaveRemoves();" ToolTip="Remove Operator" runat="server">Delete</asp:LinkButton>
        </span>
    </h3>
    <div>
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
    </div>
</div>

<asp:HiddenField ID="HiddenFieldMainAccord" runat="server" />
